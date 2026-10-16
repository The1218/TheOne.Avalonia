# Control reference authoring

Generates bilingual property/style/API/model references and PreviewHost's typed contract catalog from the same source. It reads every exported main-library declaration, the added DataGrid fork contracts, native theme selectors and authored semantics in `PropertyHelp.cs` / existing usage tables. Upstream Avalonia inherited APIs are outside this inventory. `DataGridColumn` is limited to this fork's `CycleSort` and `SortPriority` additions.

Run from the repository root with the .NET 10 SDK:

```sh
dotnet build TheOne.Avalonia/TheOne.Avalonia.csproj -c Release --artifacts-path .preview/api-reference/artifacts
dotnet run --project tools/ControlReferenceGenerator/ControlReferenceGenerator.csproj -c Release --artifacts-path .preview/api-reference/generator-artifacts -- "$PWD" "$PWD/.preview/api-reference/artifacts/obj/TheOne.Avalonia/project.assets.json" "$PWD/.preview/api-reference/contracts.json" --generate
```

Use the same command with `--check` instead of `--generate` to fail on reference/catalog/accessor drift. No modification timestamps or absolute machine paths are embedded. The tool uses Roslyn shipped with the SDK and adds no production dependency. Its JSON output argument is diagnostic data and belongs under `.preview/`.

For a no-argument `AddOwner<T>()`, the generator follows the source field when available to preserve the original default, binding mode, validation and inheritance metadata. External Avalonia registrations without source remain marked as inherited registrations. An explicit metadata override is read from its own declaration.

Tracked outputs:

- `docs/api/en/` and `docs/api/zh-cn/`: each exported type, declared properties with access/default/binding/validation, signatures, events, enum values, related models and source links.
- `docs/controls/` and `docs/theme/`: generated sections between `generated-contracts` markers. Edit the authored prose/tables above the marker or `PropertyHelp.cs`, then regenerate. Do not edit generated sections directly.
- `design/PreviewHost/Reference/ControlContracts.json`: embedded descriptions, models and styles; deserialized with a source-generated JSON context.
- `design/PreviewHost/Reference/ContractAccessors.g.cs`: direct typed property reads/writes, restoration and event subscriptions. No runtime reflection activation or binding is used.

PreviewHost keeps complete demos on their individual pages. Visible example cards compare styles, sizes and states, labeling the exact `Classes` and property values beside the rendered examples. `Description` components present public properties, API/event signatures and built-in model fields. Property rows include their type, registration default, binding mode, read/write scope and validation; model rows retain member types and meanings. Native inherited APIs, such as `ItemsSource`, `Header` and `Content`, remain Avalonia APIs and should be identified as inherited in authored examples. `Description` and `DescriptionItem` have their own guide, generated contracts and paired API links.

Only Button, Input and Select share comparison pages. The original demo is never extracted or reparented. Pages and reference rows grow naturally; canvas editors own a finite viewport and keep reference content reachable in a separate tab. Typed property examples that update StyledProperty/AttachedProperty members use SetCurrentValue to preserve bindings. Theme/window/service examples retain their real ownership rules. Template-only classes are documented as automatic state and are not offered as public appearance choices. The catalog supplies description content; property dropdown editors are not the reference presentation.

After updating preview behavior, build only PreviewHost with isolated artifacts and run its independent `--verify-control-pages` entry. This checks the new catalog/demos and does not request a full control-library regression. The MultiSelectBox scroll fix has separate coverage in `--verify-multiselect`; the Home-specific partial verification file is not linked into the native package consumer.

默认值是源码注册、构造参数或模型初值；主题 Setter、构造逻辑和局部值可能覆盖它。`CLR / init-only` 表示仅可在初始化时赋值，`CLR / n/a` 不声明 Avalonia 默认绑定模式。只读状态和访问器定义值不会被错误地当成可写默认配置。公开模板辅助类可供审查，但不意味着应由业务页面独立创建；使用入口以控件指南为准。
