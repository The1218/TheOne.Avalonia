# Project integration and migration

Historical command-line checks and consumer test projects have been removed. Check coverage and measurements in this guide are historical records. Start PreviewHost normally and open the relevant control page; validate target-platform behavior in the consuming application.

**English** · [简体中文](../zh-cn/README.md) · [Documentation index](../../README.md)

Use this guide when integrating an existing project on another machine, including work delegated to Codex. The baseline is TheOne.Avalonia / TheOne.Avalonia.DataGrid **1.0.0-preview.9**, **.NET 10**, and **Avalonia 12.1.4**. Identify the target stack before changing it. Running PreviewHost is not a prerequisite for consuming the library.

## 1. Inspect the target project first

| Record | Reason |
| --- | --- |
| SDK, TargetFramework, Avalonia versions, target OS / RID | The library targets net10.0 / Avalonia 12.1.4; do not assume an Avalonia 11 or net8.0 application is compatible |
| UI framework and application lifetime | Existing Avalonia applications can replace their theme; WPF / WinForms applications require a framework migration first, beyond package or XML namespace replacement |
| Startup, main window, login handoff and shutdown policy | Preserve DI, authentication and business navigation instead of copying the preview application's startup |
| Installed themes, DataGrid source and custom templates | Find existing Fluent / Semi / Ursa themes and overrides to prevent overlapping themes and duplicate assemblies |
| MVVM, model notifications, commands and validation | The main library does not require ReactiveUI; retain the existing framework and supply the business members used in examples |
| Pages in scope, data size and existing checks | Start with one representative editing page and one collection page |

Read the target project's instructions and run `git status --short`. Make SDK/framework upgrades, lifetime changes and third-party control replacements explicit. Preserve the user's existing changes.

## 2. Install and initialize the theme

For an existing compatible Avalonia project:

```sh
dotnet add path/to/YourApp.csproj package TheOne.Avalonia --version 1.0.0-preview.9
```

This is a fragment of an existing application project, not a complete new project:

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
</PropertyGroup>
<ItemGroup>
  <PackageReference Include="TheOne.Avalonia" Version="1.0.0-preview.9" />
</ItemGroup>
```

Keep the application's desktop/single-view startup and required Avalonia platform packages, aligning its Avalonia dependencies with the 12.1.4 baseline. The main package brings `TheOne.Avalonia.DataGrid >= 1.0.0-preview.9`; upgrade any explicit reference to the companion package together. Remove the official `Avalonia.Controls.DataGrid` reference: this fork retains assembly name `Avalonia.Controls.DataGrid`, version `12.1.2.0` and strong-name identity, so the two implementations cannot coexist. The main assembly version remains `1.0.0.0`; it is distinct from its NuGet version.

Replace the old base theme registration with one `OneTheme` in `App.axaml`, retaining the actual `x:Class`, DataTemplates and business resources:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:themes="using:TheOne.Avalonia.Themes"
             x:Class="YourApp.App"
             RequestedThemeVariant="Default">
  <Application.Styles>
    <themes:OneTheme ColorTheme="Nova" />
    <!-- Keep reviewed application styles after the base theme. -->
  </Application.Styles>
</Application>
```

Keep the existing App XAML loading and ViewModel initialization. `OneTheme` already includes Fluent support and DataGrid themes: do not additionally register FluentTheme, SimpleTheme or official DataGrid Fluent styles. Review application overrides, especially hardcoded colors, Height and complete ControlTemplates.

A source reference is an alternative: see [quick start](../../../README.en.md#quick-start). Do not reference both the source project and its NuGet package in the same application.

Check APIs against the repository commit recorded in the package's nuspec, rather than a later main branch. Preview.9 adds seven statistical/specialized/hierarchy charts, chart animation options, AttachmentList, DataGrid row numbers and workflow/dialog integration, plus preview navigation improvements described in the [release notes](../../releases/en/1.0.0-preview.9.md). Use current migration guidance while checking member declarations against the target package version.

### Library dependencies differ from preview dependencies

Preview.6 appends `WorkflowValueKind.MultiSelect = 3` and `ReportElementKind.QRCode = 8`. Older libraries cannot read documents using these new types. Persist workflow approver/role selections as stable keys; reload the host catalog and runtime report data separately. See the [upgrade notes](../../releases/en/1.0.0-preview.6.md).

| Capability | Consumer application | Current PreviewHost |
| --- | --- | --- |
| Themes, Form, MultiSelectBox, charts, windows | Main package and its transitive dependencies | Source ProjectReference to the main library |
| QRCode and report QR elements | `Net.Codecrete.QrCodeGenerator` 3.2.1, restored transitively by the main package | Same dependency; no separate preview package |
| Code editing / highlighting | Optional `TheOne.Avalonia.CodeEditor` package | Version 1.0.0-preview.2 with CodeEditorTheme |
| Print settings / preview | Optional `TheOne.PrintingTools.Avalonia` package | Version 1.0.0-preview.3, theme registered locally in print windows |
| ReactiveUI, Material icons, Inter fonts, Headless, diagnostics | Choose according to application needs | Used by examples and verification |

The optional packages are not installed transitively with the main library and have independent versions. The editor package is named **TheOne.Avalonia.CodeEditor**, not TheOne.Avalonia.Editor. Do not make PreviewHost, Verification or sibling local repositories application dependencies. See [ReportPreview](../../controls/en/report-preview.md#pdf-and-printing) for the printing bridge.

## 3. Migrate by feature and preserve business code

| Existing feature | Integration path |
| --- | --- |
| Native Button, TextBox, ComboBox, ListBox, TreeView, Calendar, etc. | Keep native controls, commands and bindings; apply OneTheme first. Read [basic controls](../../controls/en/basic.md), [collections](../../controls/en/collections.md), [dates](../../controls/en/date-time.md) |
| Third-party / private controls | Compare behavior and public contracts; replacing xmlns is insufficient and similarly named properties are not necessarily equivalent |
| Editing and validation | [Form / FormItem](../../controls/en/form.md), IStepValidator, MemberName-to-model mapping and native editor validation |
| Multiselect input | [MultiSelectBox](../../controls/en/multi-select-box.md), SelectedItems, typed ItemAdapter, disabled selection and clear semantics |
| Tree selection | [TreeSelect](../../controls/en/tree-select.md), stable keys, typed adapters, Independent/Cascade, disabled and reset semantics |
| Sidebar navigation | [NavMenu](../../controls/en/nav-menu.md), model hierarchy, selection, expansion and commands in the VM, bounded viewport |
| Data tables | [DataGrid binding](../../controls/en/datagrid-binding.md) followed by [filtering / freezing / summaries](../../controls/en/datagrid-extensions.md); do not copy internal Presenters |
| Login, windows and messages | [OneWindow](../../controls/en/windows.md), [LoginWindow](../../controls/en/login-window.md), [owner routing](../../controls/en/window-owners.md) |
| Charts, Gantt, workflows, reports | Follow the [control index](../../controls/en/README.md); check model contracts, editing, serialization and scale limits first |

Business ViewModels do not need to inherit the preview application's types. Its login example only performs local demonstration validation; it must not replace application authentication, permissions or token handling.

### Compiled bindings and DataGrid

Declare the business VM's `x:DataType` on each view and the model type in template scopes. Runtime DataContext assignment does not supply static types. Model and collection notifications remain the application's responsibility. Do not hide integration failures using ReflectionBinding, disabled compiled bindings, NoWarn or whole-assembly preservation.

Register concrete row and nested types before creating a grid/view, for both managed and AOT execution:

```csharp
using Avalonia.Collections;
using TheOne.Avalonia.Controls;

_ = DataGridAot.PreserveRowType<OrderRow>();
// For Customer.Name, also register the actual nested Customer model.
_ = DataGridAot.PreserveRowType<Customer>();
var view = new DataGridCollectionView<OrderRow>(orders);
```

`OrderRow`, `Customer` and `orders` are application placeholders. Read the [DataGrid NativeAOT section](../../controls/en/datagrid-binding.md#nativeaot) for custom empty collections, comparers, converters and precise preservation requirements. Do not expect compiled row inference from an untyped object data source.

### Window owners and messages

OneToast, OneNotification and OneDialog target an opened OneWindow or LoginWindow. Theming a plain Window does not add message hosts. Prefer explicit `owner:` in multiwindow applications: native `Show(owner)`, `desktop.MainWindow` and each message service's default `Owner` are separate concepts. Implement login handoff, cancellation, queues and close cleanup according to [window ownership](../../controls/en/window-owners.md).

## 4. Colors, sizes and older examples

Palette and Light / Dark are separate settings. On the UI thread, after registering an application-level OneTheme:

```csharp
using Avalonia;
using Avalonia.Styling;
using TheOne.Avalonia.Themes;

OneTheme.SetColor(ThemeColor.Nova);
Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;
```

Use semantic `DynamicResource` values such as One.Surface, One.Text, One.Line and One.Accent for business views. See [color resources](../../theme/en/theme-colors.md) for gradients. Density inherits through `styling:ControlSizing.Size="Small|Normal|Large"` or small / normal / large classes. Standard interaction heights are 30 / 36 / 44, but margins, multiline tags and expanded content can make total heights different; avoid globally fixed Height values.

| Older usage | Current contract |
| --- | --- |
| ThemeButton styled as Button | It derives from ToggleButton, uses built-in icons / checked state and switches only Light / Dark |
| Palette picker treated as a wrapper-object list | ThemeColorPicker selects enum values; bind `SelectThemeColor` to ThemeColor |
| Default / ThemeColorOption in early source examples | Current default is `ThemeColor.Nova` (0), and the picker uses ThemeColor directly. This concerns early source examples, not a claim that released preview.2 exposed those types |
| Binding both SelectThemeColor and SelectedColor | Bind one; SelectedColor remains compatible and shares state |
| Synchronous configuration file writes on selection | Persist asynchronously in the host; restore palette and brightness separately after theme registration |

See [ThemeColorPicker / ThemeButton](../../theme/en/theme-color-picker.md). Resource notifications are coalesced to the next render; synchronous pixel observations are not final visual state.

## 5. Platform boundaries

| Target / feature | Integration and validation |
| --- | --- |
| Windows / macOS / Linux desktop | Preserve platform startup, fonts and native services; verify windows, focus, shortcuts, DPI, IME, file dialogs and case-sensitive resource paths on the target OS |
| Android / iOS / Browser | This repository does not provide complete integration or execution guarantees for these targets; adapt single-view pages separately rather than copying OneWindow / LoginWindow desktop lifetime and message hosts |
| PDF / printing | Export and physical printing have separate acceptance checks; default printing uses a Windows PDF handler or CUPS; optional printing packages have their own platform constraints |
| NativeAOT / trimming | Verify affected consumers/controls only; producing a RID artifact is not proof of execution on that OS |
| Images, fonts and external resources | Use the actual application's avares assembly/resource paths, not a maintainer's local absolute files; handle networking and font fallback in the application |

Existing macOS Headless / Skia results are not Windows or Linux desktop execution evidence. State OS, RID, execution mode and untested capabilities explicitly.

## 6. Implementation and acceptance

1. Preserve a build/behavior baseline and define pages in scope and business flows that must stay intact.
2. Establish dependency compatibility and one theme; compile and verify one native editor page and one collection page.
3. Migrate forms, multiselect, DataGrid, window owners and specialized controls as needed, checking bindings, commands and model identity at each step.
4. Add palette/density and preference restoration; check both brightness modes, all sizes, narrow viewports, focus, selection and virtualization.
5. Run only affected-control checks, then verify real business behavior in the target application and OS. PreviewHost success is not application acceptance.

Start the normal preview from the repository root, then open the relevant control page:

```sh
dotnet run --project design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/demo/artifacts
```

Report changes, public contract changes, actual builds/interactions/OS execution, limitations and next steps. Do not report unrun checks as passed. The [complete public API and model reference](../../api/en/README.md) lists every library public type, property, constructor, method, event and enum with declaration links; control guides also enumerate added properties and styles. PreviewHost includes per-control complete demos with local style/property/API/model sections; only Button, Input and Select share comparison pages. Canvas editors keep their full viewport and place reference content in a separate tab. Published packages still lack complete XML API documentation. New repository documentation/demos describe current source; consumers must check the commit recorded in their target package.

## 7. Portable brief for another Codex session

Replace placeholders and provide this repository URL or guide. These instructions are distributed with the docs and do not require a maintainer's ignored local AGENTS.md / skill files.

```text
Goal: integrate TheOne.Avalonia 1.0.0-preview.9 into [project / specific pages].
Target OS/RID: [fill in]. Existing framework/SDK: [fill in]. Scope: [fill in].
Preserve authentication, models, commands, DI, routing and existing user changes.

Read the target project's instructions, git status --short and
docs/integration/en/README.md, then only relevant control/theme documents.
Identify version/theme/DataGrid/lifetime conflicts before phased implementation.

Baseline: net10.0, Avalonia 12.1.4, main and companion DataGrid packages preview.9.
Load one OneTheme. Keep native controls; do not invent TheOne wrapper classes.
Do not copy preview authentication or make optional editor/printing packages mandatory.
Use correct x:DataType / compiled bindings; register concrete DataGrid row/nested types.
Do not disable compiled bindings, suppress warnings or preserve whole assemblies.
Use explicit OneWindow/LoginWindow owners for desktop messages, not plain Window hosts.
Retain bounded viewports / virtualization and One.* DynamicResource colors.
Persist palette and Light/Dark independently in application configuration.

Modify and verify only scoped pages/controls with isolated build outputs; no full regression.
Use real pointer/keyboard input for interaction fixes; check light/dark, sizes,
narrow viewports and rendered pixels for visual changes.
Run affected native-consumer checks only for AOT/binding/metadata changes;
distinguish cross-RID builds from target-OS execution.
Surface blocking compatibility/business decisions; continue independent work.
Report changes, evidence, untested platforms and remaining issues.
Do not bump versions, push or publish NuGet without separate authorization.
```
