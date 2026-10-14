#!/usr/bin/env bash
# Restore only from NuGet.org, then compare signed remote package payloads with the approved feed.
set -euo pipefail
task_root="$(cd "$(dirname "$0")/.." && pwd)"
cd "$task_root"
task_version="${THEONE_PACKAGE_VERSION:-$(dotnet msbuild TheOne.Avalonia/TheOne.Avalonia.csproj -nologo -getProperty:TheOnePackageVersion)}"
task_output="${THEONE_REMOTE_OUTPUT:-$task_root/.preview/nuget-publish-$task_version/consumer}"
mkdir -p "$task_output/packages"
task_output="$(cd "$task_output" && pwd)"
task_desktop=false
for task_arg in "$@"; do
  case "$task_arg" in --desktop) task_desktop=true ;; *) echo "Usage: $0 [--desktop]"; exit 2 ;; esac
done
python3 - "$task_output" "$task_version" <<'PY'
from pathlib import Path
import hashlib,json,shutil,sys
p=Path(sys.argv[1]); version=sys.argv[2]
audit=json.loads(Path('.preview/package-versions/verification.json').read_text())
channel=next(v for v in audit['channels'].values() if v['version']==version)
for info in channel['packages']:
    f=Path('.preview/package-versions')/info['file']
    assert hashlib.sha256(f.read_bytes()).hexdigest()==info['sha256'],f
for name in ['theone.avalonia','theone.avalonia.datagrid']:
    shutil.rmtree(p/'packages'/name,ignore_errors=True)
(p/'NuGet.Config').write_text('<configuration><packageSources><clear/><add key="nuget.org" value="https://api.nuget.org/v3/index.json"/></packageSources><fallbackPackageFolders><clear/></fallbackPackageFolders></configuration>\n')
print('PASS: immutable approved package hashes; public-only source and isolated package cache.')
PY
task_project=tests/DataGridPackageConsumer/DataGridPackageConsumer.csproj
dotnet restore "$task_project" --configfile "$task_output/NuGet.Config" --packages "$task_output/packages" --no-http-cache --force -p:DisableImplicitLibraryPacksFolder=true -p:DisableImplicitNuGetFallbackFolder=true "-p:TheOnePackageVersion=$task_version" > "$task_output/restore.log" 2>&1
python3 - "$task_output" "$task_version" <<'PY'
from pathlib import Path
from zipfile import ZipFile
import hashlib,json,sys
p=Path(sys.argv[1]); version=sys.argv[2]
assets=json.loads(Path('tests/DataGridPackageConsumer/obj/project.assets.json').read_text())
assert set(assets['project']['restore']['sources'])=={'https://api.nuget.org/v3/index.json'}
assert {str(Path(n).resolve()) for n in assets['packageFolders']}=={str((p/'packages').resolve())}
assert {k for k in assets['libraries'] if k.startswith('TheOne.Avalonia')}=={f'TheOne.Avalonia/{version}',f'TheOne.Avalonia.DataGrid/{version}'}
assert not any(k.startswith('Avalonia.Controls.DataGrid/') for k in assets['libraries'])
audit=json.loads(Path('.preview/package-versions/verification.json').read_text())
channel=next(v for v in audit['channels'].values() if v['version']==version); records=[]
for info in channel['packages']:
    name=info['id'].lower(); directory=p/'packages'/name/version.lower()
    metadata=json.loads((directory/'.nupkg.metadata').read_text())
    assert metadata['source'].rstrip('/')=='https://api.nuget.org/v3/index.json',metadata['source']
    local=Path('.preview/package-versions')/info['file']; remote=directory/f'{name}.{version.lower()}.nupkg'
    with ZipFile(local) as approved, ZipFile(remote) as published:
        old=set(approved.namelist()); new=set(published.namelist())
        assert old-{'.signature.p7s'}==new-{'.signature.p7s'},info['id']
        assert '.signature.p7s' in new,'NuGet.org repository signature missing'
        for entry in old-{'.signature.p7s'}: assert approved.read(entry)==published.read(entry),(info['id'],entry)
    records.append({'id':info['id'],'version':version,'source':metadata['source'],'approved_sha256':info['sha256'],'remote_sha256':hashlib.sha256(remote.read_bytes()).hexdigest(),'payload_matches':True,'repository_signature_present':True})
    print('PASS:',info['id'],version,'restored from NuGet.org; all approved payload entries match.')
(p/'remote-packages.json').write_text(json.dumps(records,indent=2)+'\n')
PY
dotnet build "$task_project" -c Release --no-restore "-p:TheOnePackageVersion=$task_version" > "$task_output/build.log" 2>&1
task_dll="$task_root/tests/DataGridPackageConsumer/bin/Release/net10.0/DataGridPackageConsumer.dll"
dotnet "$task_dll" > "$task_output/headless.log" 2>&1
cat "$task_output/headless.log"
if $task_desktop; then
  dotnet "$task_dll" --desktop > "$task_output/desktop.log" 2>&1
  cat "$task_output/desktop.log"
fi
