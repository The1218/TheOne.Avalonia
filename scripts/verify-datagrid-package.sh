#!/usr/bin/env bash
# Build fresh local NuGet packages; the independent consumer has no project references.
set -euo pipefail
task_root="$(cd "$(dirname "$0")/.." && pwd)"
cd "$task_root"
task_output="${THEONE_DATAGRID_OUTPUT:-$task_root/.preview/datagrid-integration}"
mkdir -p "$task_output/feed" "$task_output/packages"
task_output="$(cd "$task_output" && pwd)"
task_cross=false task_native=false task_desktop=false
for task_arg in "$@"; do
  case "$task_arg" in
    --cross) task_cross=true ;;
    --native) task_native=true ;;
    --desktop) task_desktop=true ;;
    *) echo "Usage: $0 [--cross] [--native] [--desktop]"; exit 2 ;;
  esac
done
case "$(uname -s)/$(uname -m)" in
  Darwin/arm64) task_rid=osx-arm64 ;;
  Darwin/x86_64) task_rid=osx-x64 ;;
  Linux/aarch64) task_rid=linux-arm64 ;;
  Linux/x86_64) task_rid=linux-x64 ;;
  *) echo "Run the consumer with the matching host RID; see docs/workflow/datagrid-acceptance.md."; exit 2 ;;
esac
task_consumer=tests/DataGridPackageConsumer/DataGridPackageConsumer.csproj
if [[ -n "${THEONE_PACKAGE_VERSION:-}" ]]; then
  task_version="$THEONE_PACKAGE_VERSION"
else
  task_version="$(dotnet msbuild TheOne.Avalonia/TheOne.Avalonia.csproj -nologo -getProperty:TheOnePackageVersion)"
fi
task_version_args=("-p:TheOnePackageVersion=$task_version")
echo "Release train: $task_version"
task_global="$(dotnet nuget locals global-packages --list | sed 's/^[^:]*: //')"
# Only the two owned package-cache entries are discarded; user NuGet caches are untouched.
python3 - "$task_output" <<'PY'
from pathlib import Path
import shutil,sys
p=Path(sys.argv[1])
for name in ['theone.avalonia','theone.avalonia.datagrid']:
    shutil.rmtree(p/'packages'/name,ignore_errors=True)
for item in (p/'feed').glob('*.nupkg'): item.unlink()
PY
dotnet pack vendor/Avalonia.Controls.DataGrid -c Release "${task_version_args[@]}" -o "$task_output/feed" > "$task_output/pack-vendor.log" 2>&1
dotnet pack TheOne.Avalonia -c Release "${task_version_args[@]}" -o "$task_output/feed" > "$task_output/pack-library.log" 2>&1
python3 - "$task_output/feed" "$task_version" <<'PY'
from pathlib import Path
from zipfile import ZipFile
import sys,xml.etree.ElementTree as E
p=Path(sys.argv[1]); version=sys.argv[2]; names=[]
for path in sorted(p.glob('*.nupkg')):
    with ZipFile(path) as z:
        root=E.fromstring(z.read(next(n for n in z.namelist() if n.endswith('.nuspec'))))
        package=root.find('.//{*}metadata/{*}id').text; names.append(package)
        assert root.find('.//{*}metadata/{*}version').text==version
        readme=root.find('.//{*}metadata/{*}readme').text
        license=root.find('.//{*}metadata/{*}license')
        assert readme=='README.md' and len(z.read(readme))>100
        assert license.attrib['type']=='file' and len(z.read(license.text))>100
        assert root.find('.//{*}metadata/{*}releaseNotes').text.strip()
        deps=[(n.attrib['id'],n.attrib.get('version')) for n in root.findall('.//{*}dependency')]
        if package=='TheOne.Avalonia':
            assert ('TheOne.Avalonia.DataGrid',version) in deps,deps
            assert all(n!='Avalonia.Controls.DataGrid' for n,v in deps),deps
            assert 'THIRD-PARTY-NOTICES.md' in z.namelist()
        else:
            assert 'UPSTREAM.json' in z.namelist()
        print('PACKAGE',package,'dependencies',deps)
assert set(names)=={'TheOne.Avalonia','TheOne.Avalonia.DataGrid'},names
for log in p.parent.glob('pack-*.log'):
    assert 'NU5104' not in log.read_text(),log
PY
task_restore=(--source "$task_output/feed" --source "$task_global" --source https://api.nuget.org/v3/index.json --packages "$task_output/packages")
dotnet restore "$task_consumer" "${task_restore[@]}" "${task_version_args[@]}" > "$task_output/restore.log" 2>&1
dotnet build "$task_consumer" -c Release --no-restore "${task_version_args[@]}" > "$task_output/consumer-build.log" 2>&1
python3 - "$task_output" "$task_version" <<'PY'
from pathlib import Path
from zipfile import ZipFile
import hashlib,json,sys,xml.etree.ElementTree as E
p=Path(sys.argv[1]); assets=json.loads(Path('tests/DataGridPackageConsumer/obj/project.assets.json').read_text())
assert all(v['type']=='package' for k,v in assets['libraries'].items() if k.startswith('TheOne.Avalonia'))
assert {k for k in assets['libraries'] if k.startswith('TheOne.Avalonia')}=={f'TheOne.Avalonia/{sys.argv[2]}',f'TheOne.Avalonia.DataGrid/{sys.argv[2]}'}
assert not any(k.startswith('Avalonia.Controls.DataGrid/') for k in assets['libraries'])
for package in (p/'feed').glob('*.nupkg'):
    with ZipFile(package) as z:
        root=E.fromstring(z.read(next(n for n in z.namelist() if n.endswith('.nuspec'))))
        name=root.find('.//{*}metadata/{*}id').text.lower(); version=root.find('.//{*}metadata/{*}version').text
    cached=p/'packages'/name/version/package.name.lower()
    assert hashlib.sha256(package.read_bytes()).digest()==hashlib.sha256(cached.read_bytes()).digest(),name
print('PASS: package-only assets and restored nupkg hashes match the fresh local feed.')
PY
task_consumer_dll="$task_root/tests/DataGridPackageConsumer/bin/Release/net10.0/DataGridPackageConsumer.dll"
dotnet "$task_consumer_dll" > "$task_output/consumer-headless.log" 2>&1
cat "$task_output/consumer-headless.log"
if $task_desktop; then
  dotnet "$task_consumer_dll" --desktop > "$task_output/consumer-desktop.log" 2>&1
  cat "$task_output/consumer-desktop.log"
fi
if $task_native; then
  # Let publish restore its own SDK runtime-pack graph; a plain restore differs on .NET 10.
  task_publish=(dotnet publish "$task_consumer" -c Release -r "$task_rid" "${task_version_args[@]}" -p:PublishAot=true -p:TrimmerSingleWarn=false "-p:RestorePackagesPath=$task_output/packages" "-p:RestoreAdditionalProjectSources=$task_global" -o "$task_output/native/$task_rid")
  if [[ "$task_rid" == osx-* ]]; then
    task_sdk="$(xcrun --sdk macosx --show-sdk-path)"
    SDKROOT="$task_sdk" "${task_publish[@]}" "-p:CppCompilerAndLinker=$(xcrun --find clang)" > "$task_output/consumer-native-publish.log" 2>&1
  else
    "${task_publish[@]}" > "$task_output/consumer-native-publish.log" 2>&1
  fi
  "$task_output/native/$task_rid/DataGridPackageConsumer" --require-aot > "$task_output/consumer-native.log" 2>&1
  cat "$task_output/consumer-native.log"
  if $task_desktop; then
    "$task_output/native/$task_rid/DataGridPackageConsumer" --require-aot --desktop > "$task_output/consumer-native-desktop.log" 2>&1
    cat "$task_output/consumer-native-desktop.log"
  fi
fi
if $task_cross; then
  for task_target in osx-arm64 osx-x64 win-x64 win-arm64 linux-x64 linux-arm64; do
    dotnet publish "$task_consumer" -c Release -r "$task_target" "${task_version_args[@]}" --self-contained false -p:PublishAot=false "-p:RestorePackagesPath=$task_output/packages" "-p:RestoreAdditionalProjectSources=$task_global" -o "$task_output/publish/$task_target" > "$task_output/publish-$task_target.log" 2>&1
    echo "PUBLISHED (not executed): $task_target"
  done
fi
