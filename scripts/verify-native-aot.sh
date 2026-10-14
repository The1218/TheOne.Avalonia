#!/usr/bin/env bash
set -euo pipefail
task_root="$(cd "$(dirname "$0")/.." && pwd)"
cd "$task_root"
case "$(uname -s)/$(uname -m)" in
  Darwin/arm64) task_rid=osx-arm64 ;;
  Darwin/x86_64) task_rid=osx-x64 ;;
  Linux/aarch64) task_rid=linux-arm64 ;;
  Linux/x86_64) task_rid=linux-x64 ;;
  *) echo "Run dotnet publish with your host RID, then NativeAotSmoke --require-aot."; exit 1 ;;
esac
task_output="${THEONE_AOT_OUTPUT:-$task_root/.preview/native-aot/$task_rid}"
python3 scripts/check-aot-contracts.py
publish_project() {
  local task_project="$1" task_destination="$2"
  local task_publish=(dotnet publish "$task_project" -c Release -r "$task_rid" -p:PublishAot=true -p:TrimmerSingleWarn=false -o "$task_destination")
  if [[ "$task_rid" == osx-* ]]; then
    local task_clang task_sdk
    task_clang="$(xcrun --find clang)"
    task_sdk="$(xcrun --sdk macosx --show-sdk-path)"
    SDKROOT="$task_sdk" "${task_publish[@]}" "-p:CppCompilerAndLinker=$task_clang"
  else
    "${task_publish[@]}"
  fi
}
if [[ "${1:-}" == --verify-preview-pages || "${1:-}" == --verify-buttons-messages ]]; then
  task_host_output="$task_output/previewhost"
  publish_project design/PreviewHost/PreviewHost.csproj "$task_host_output"
  "$task_host_output/PreviewHost" "$1"
  exit 0
fi
publish_project tests/NativeAotSmoke/NativeAotSmoke.csproj "$task_output"
task_verify_args=(--require-aot)
if [[ "${1:-}" == --verify-charts ]]; then
  task_verify_args+=(--verify-charts)
elif [[ "${1:-}" == --verify-datagrid-binding ]]; then
  task_verify_args+=(--verify-datagrid-binding)
elif [[ "${1:-}" == --verify-datagrid-summary ]]; then
  task_verify_args+=(--verify-datagrid-summary)
elif [[ "${1:-}" == --verify-datagrid-right ]]; then
  task_verify_args+=(--verify-datagrid-right)
elif [[ "${1:-}" == --verify-datagrid-filters ]]; then
  task_verify_args+=(--verify-datagrid-filters)
elif [[ "${1:-}" == --verify-datagrid-stage1 ]]; then
  task_verify_args+=(--verify-datagrid-stage1)
elif [[ "${1:-}" == --verify-navmenu-filter ]]; then
  task_verify_args+=(--verify-navmenu-filter)
elif [[ "${1:-}" == --verify-tag-step ]]; then
  task_verify_args+=(--verify-tag-step)
elif [[ "${1:-}" == --verify-navmenu-motion ]]; then
  task_verify_args+=(--verify-navmenu-motion)
fi
"$task_output/NativeAotSmoke" "${task_verify_args[@]}"
if [[ "${1:-}" == --full ]]; then
  task_host_output="$task_output/previewhost"
  publish_project design/PreviewHost/PreviewHost.csproj "$task_host_output"
  "$task_host_output/PreviewHost" --verify-controls
fi
