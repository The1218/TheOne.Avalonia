# Control reference authoring

Generates bilingual property/style/API/model references and PreviewHost's typed contract catalog from the same source. It reads every exported main-library declaration, the added DataGrid fork contracts, native theme selectors and authored semantics in `PropertyHelp.cs` / existing usage tables. Upstream Avalonia inherited APIs are outside this inventory. `DataGridColumn` is limited to this fork's `CycleSort` and `SortPriority` additions.

Run from the repository root with the .NET 10 SDK:

```sh
dotnet build TheOne.Avalonia/TheOne.Avalonia.csproj -c Release --artifacts-path .preview/api-reference/artifacts
dotnet run --project tools/ControlReferenceGenerator/ControlReferenceGenerator.csproj -c Release --artifacts-path .preview/api-reference/generator-artifacts -- "$PWD" "$PWD/.preview/api-reference/artifacts/obj/TheOne.Avalonia/project.assets.json" "$PWD/.preview/api-reference/contracts.json" --generate
```

Use the same command with `--check` instead of `--generate` to fail on reference/catalog drift. No modification timestamps or absolute machine paths are embedded. The tool uses Roslyn shipped with the SDK and adds no production dependency. Its JSON output argument is diagnostic data and belongs under `.preview/`.

Append `--verify-examples` to the same three arguments to statically compile the actual shared business snippets displayed by PreviewHost. It can run alone or alongside `--generate` / `--check`. The tool links `ControlUsageExamples.cs` directly, wraps each snippet in its own async method with concrete application-object parameters, and checks only diagnostics attached to the new snippet syntax trees. Source-analysis diagnostics caused by absent generated XAML code are excluded from a standalone snippet check. Snippets are never invoked, so their file, network, export and print operations do not run. The targeted scope is ReportDesign, ReportPreview, WorkflowEditor, WorkflowSerializer, WorkflowDocument, Form, ChartBase and MultiSelectBox. This checks C# business actions, not XAML property snippets, runtime behavior or the full library.

For a no-argument `AddOwner<T>()`, the generator follows the source field when available to preserve the original default, binding mode, validation and inheritance metadata. External Avalonia registrations without source remain marked as inherited registrations. An explicit metadata override is read from its own declaration.

Tracked outputs:

- `docs/api/en/` and `docs/api/zh-cn/`: complete exported source contracts for precise lookup, including properties with access/default/binding/validation, constructors, methods, events, accessors, identifiers, enum values, related models and source links. This is separate from practical control usage.
- `docs/controls/` and `docs/theme/`: generated usage sections between `generated-contracts` markers, with project-specific properties, compact public Classes usage, practical business actions and model links. They do not emit raw style selector/effect tables or constructor/accessor/identifier inventories. Edit the authored prose/tables above the marker or `PropertyHelp.cs`, then regenerate. Do not edit generated sections directly.
- `design/PreviewHost/Reference/ControlContracts.json`: embedded descriptions, models and styles; deserialized with a source-generated JSON context.

PreviewHost keeps complete demos on their individual pages. Classes appear only as actual labeled examples, without a selector/effect explanation table. `Description` components explain project-added properties, practical business actions and related model fields; properties and actions include short usage snippets. Business actions include saving/loading, exporting, validation, navigation and undo/redo. Constructors, property Get/Set wrappers, AvaloniaProperty/RoutedEvent identifiers and public fields belong to the full source contract rather than the business API section. Generated usage tables also exclude rendering/converter methods and generic initialization helpers. Workflow persistence is correctly identified as `WorkflowSerializer.Save/Load`; `WorkflowEditor` exposes the host-owned `SaveCommand`, not a Save method.

Inherited native properties and APIs are omitted. Global `ControlSizing` and `Motion` settings appear once on Home and in the shared theme guide; generated control guides link there rather than repeating their contracts. Control-specific helpers, including `ButtonLoading`, `DataGridFeatures`, `RefreshMotion` and `NotificationMotion`, remain with their affected control. Universal styles targeting `Control` and internal template classes are excluded from per-control Classes usage. `Description` and `DescriptionItem` have their own guide, generated contracts and paired API links. The complete API files still retain every exported source declaration, including the standalone shared configuration types.

Only Button, Input and Select share comparison pages. The original demo is never extracted or reparented. Pages and reference rows grow naturally; canvas editors own a finite viewport and keep reference content reachable in a separate tab. Theme/window/service examples retain their real ownership rules. Template-only classes are maintained automatically. The catalog supplies description content without an executable property editor or runtime accessor generation.

After updating preview behavior, build only PreviewHost with isolated artifacts and run its independent `--verify-control-pages` entry. This checks the new catalog/demos and does not request a full control-library regression. The MultiSelectBox scroll fix has separate coverage in `--verify-multiselect`; the Home-specific partial verification file is not linked into the native package consumer.

默认值是源码注册、构造参数或模型初值；主题 Setter、构造逻辑和局部值可能覆盖它。`CLR / init-only` 表示仅可在初始化时赋值，`CLR / n/a` 不声明 Avalonia 默认绑定模式。只读状态和访问器定义值不会被错误地当成可写默认配置。公开模板辅助类可供审查，但不意味着应由业务页面独立创建；使用入口以控件指南为准。
