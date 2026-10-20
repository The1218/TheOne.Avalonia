# Timeline

A vertical history of operations, deliveries or approvals. Timeline displays collection order; it does not sort, execute approvals or track a current step. Use [FlowStep](flow-step.md) for step progress. Register OneTheme; `one` maps to `using:TheOne.Avalonia.Controls`.

```xml
<one:Timeline>
    <one:TimelineItem Header="Submitted" Timestamp="09:00" Content="Purchase request submitted." Status="Success" />
    <one:TimelineItem Header="Awaiting approval" Timestamp="09:30" Content="Waiting for the supervisor." Status="Pending" />
</one:Timeline>
```

## Added properties and layout

| Property | Default | Meaning |
| --- | --- | --- |
| Timeline.Mode | Left | Left: left rail/right content; Right: right rail/left content; Alternate: centered rail, alternating by visual order, starting on the right |
| Timeline.Reverse | false | Reverses visual arrangement without mutating data; alternation and the last connector follow visual order |
| Timeline.ItemSpacing | 16 DIP | Finite nonnegative spacing inherited by TimelineItem; individually overridable; themed Small/Normal/Large values are 12/16/20 |
| TimelineItem.Status | Default | Default (accent), Success, Warning, Error, Pending; Pending is a static hollow marker without looping animation |
| TimelineItem.Timestamp / TimestampTemplate | null | Timestamp content/template; null hides it. Above the body in Left/Right, opposite the rail in Alternate |
| TimelineItem.Dot / DotTemplate | null | Custom marker content/template; non-null replaces the dot, returning to null restores current Status |
| TimelineItem.ItemSpacing | inherited | Per-event spacing override; the visually last item has no extra trailing space or connector |

```xml
<one:Timeline ItemsSource="{CompiledBinding Events}" Mode="Alternate" Reverse="True" ItemSpacing="20" />
```

The default TimelinePanel uses natural height without fixed height or an inner scroller. It is intended for bounded histories and is **not virtualized**. Paginate or load incrementally for large histories. A replacement ItemsPanel must implement reverse arrangement itself. Alternate wraps with available width without automatically changing Mode; prefer Left on narrow screens. Reverse does not change native keyboard Tab order within content.

There are no extra public Classes, commands or persistence APIs. See [shared theme configuration](../../theme/en/README.md#shared-configuration) for Small/Normal/Large, Light/Dark and palettes.

## Built-in model and binding

```csharp
public ObservableCollection<TimelineItemModel> Events { get; } =
[
    new() { Key = "submit", Header = "Submitted", Content = "PO-001",
            Timestamp = "2026-10-20 09:00", Status = TimelineStatus.Success },
    new() { Key = "review", Header = "Supervisor review", Content = "Pending", Status = TimelineStatus.Pending }
];
// Update on the UI thread; existing containers follow compiled bindings.
Events[1].Content = "Approved";
Events[1].Status = TimelineStatus.Success;
```

TimelineItemModel is an observable AvaloniaObject: Key is caller-owned identity; Header, Content, Timestamp and Dot are optional content, default null; Status defaults to Default. Timestamp is not implicitly formatted and never determines order; localization/time zones belong to the host. Observable add/remove/Move/Replace/Reset and ItemsSource replacement are supported. Do not modify UI models or collections from background threads.

Native ItemTemplate receives the original item and defines only the body. Built-in models still map Header/Timestamp/Status/Dot; without ItemTemplate the body uses model.Content.

```xml
<one:Timeline ItemsSource="{CompiledBinding Events}">
    <one:Timeline.ItemTemplate>
        <DataTemplate x:DataType="one:TimelineItemModel">
            <TextBlock Text="{CompiledBinding Content}" TextWrapping="Wrap" />
        </DataTemplate>
    </one:Timeline.ItemTemplate>
</one:Timeline>
```

Arbitrary business types can use ItemTemplate for their body. For separate title/time/status mappings, use native ItemContainerTheme with typed CompiledBinding setters. No reflection getters or string member configuration is required. Put business buttons in Content to retain native input, focus and commands.

## Custom markers and templates

```xml
<one:TimelineItem Header="Approval" Content="Finance approved." Status="Success">
    <one:TimelineItem.Dot><one:OneIcon Kind="Approval" IconSize="18" /></one:TimelineItem.Dot>
</one:TimelineItem>
<!-- Use templates to create independent controls for repeated markers. -->
<one:TimelineItem Header="Complete" Dot="done">
    <one:TimelineItem.DotTemplate>
        <DataTemplate><one:OneIcon Kind="Success" IconSize="18" /></DataTemplate>
    </one:TimelineItem.DotTemplate>
</one:TimelineItem>
```

The rail is 24 DIP wide; keep custom markers within 24 DIP. Do not share a Control instance between items. HeaderTemplate/ContentTemplate retain native behavior; TimestampTemplate/DotTemplate use their own x:DataType and CompiledBinding for ordinary bindings. Markers are decorative: include meaningful status in title/body text rather than relying on color alone.

PreviewHost → 数据展示 → Timeline provides a dedicated page with live events, three modes, reverse order, five states, custom markers, rich content and compiled templates. Description entries include concise property/model code. Verification is limited to Timeline and its preview, collection lifecycle, template changes, theme/density/narrow layouts and content buttons; no full-library regression is implied.

2026-10-20: managed and macOS ARM64 NativeAOT scoped runs each passed **426 checks**. Coverage includes add/remove/Move/Replace/Reset, replacing/clearing ItemsSource, old-model unbinding, reverse/alternate/final connectors, template reversal, status/custom-marker restoration, resource resolution, themes/palettes/three densities, narrow layouts, unload/reload and embedded Button pointer/Space/focus behavior. An actual macOS desktop run additionally verified adding events, reverse order, keyboard layout selection, scrolling and status buttons. The normal Release build passed with the existing LoginDemoViewModel nullability warning; no production dependency or trim/AOT warning was added. No full-library regression or Windows/Linux runtime checks were performed.

<!-- generated-contracts:start -->
## Control-specific properties, Classes usage and business APIs

This section lists project-added control-specific properties, public Classes usage, practical business actions and related models, without repeating inherited native APIs, constructors, property Get/Set wrappers or public identifiers. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost shows Classes through labeled examples rather than template-selector/setter inventories; properties and business actions in Description include short usage snippets. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### Timeline

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.Timeline.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`ItemSpacing`](../../../TheOne.Avalonia/Controls/Timeline.cs#L34) | `double` | writable; OneWay; inherits | `16` | 16 DIP ; Finite nonnegative spacing inherited by TimelineItem; individually overridable; themed Small/Normal/Large values are 12/16/20 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`Mode`](../../../TheOne.Avalonia/Controls/Timeline.cs#L30) | `TheOne.Avalonia.Controls.TimelineMode` | writable; OneWay | `0 / default` | Left ; Left: left rail/right content; Right: right rail/left content; Alternate: centered rail, alternating by visual order, starting on the right Validation: `Enum.IsDefined` Values: `Left`, `Right`, `Alternate` |
| [`Reverse`](../../../TheOne.Avalonia/Controls/Timeline.cs#L32) | `bool` | writable; OneWay; inherits | `false` | false ; Reverses visual arrangement without mutating data; alternation and the last connector follow visual order |

### TimelineItem

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.TimelineItem.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Dot`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L30) | `object?` | writable; OneWay | `null` | null ; Custom marker content/template; non-null replaces the dot, returning to null restores current Status |
| [`DotTemplate`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L31) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | null ; Custom marker content/template; non-null replaces the dot, returning to null restores current Status |
| [`ItemSpacing`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L33) | `double` | writable; OneWay; inherits | `16` | 16 DIP ; Finite nonnegative spacing inherited by TimelineItem; individually overridable; themed Small/Normal/Large values are 12/16/20 Validation: `value => double.IsFinite(value) && value >= 0` |
| [`Status`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L32) | `TheOne.Avalonia.Controls.TimelineStatus` | writable; OneWay | `0 / default` | Default ; Default (accent), Success, Warning, Error, Pending; Pending is a static hollow marker without looping animation Validation: `Enum.IsDefined` Values: `Default`, `Success`, `Warning`, `Error`, `Pending` |
| [`Timestamp`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L28) | `object?` | writable; OneWay | `null` | null ; Timestamp content/template; null hides it. Above the body in Left/Right, opposite the rail in Alternate |
| [`TimestampTemplate`](../../../TheOne.Avalonia/Controls/TimelineItem.cs#L29) | `Avalonia.Controls.Templates.IDataTemplate?` | writable; OneWay | `null` | null ; Timestamp content/template; null hides it. Above the body in Left/Right, opposite the rail in Alternate |

### TimelineItemModel

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.TimelineItemModel.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`Content`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L16) | `object?` | writable; OneWay | `null` | Host content/presentation object; follow the content-template scope and do not persist visual objects as business data. |
| [`Dot`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L18) | `object?` | writable; OneWay | `null` | null ; Custom marker content/template; non-null replaces the dot, returning to null restores current Status |
| [`Header`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L15) | `object?` | writable; OneWay | `null` | Header content, as text or a model; HeaderTemplate controls custom presentation. |
| [`Key`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L14) | `object?` | writable; OneWay | `null` | Field/parameter key used to resolve business values, distinct from display names. |
| [`Status`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L19) | `TheOne.Avalonia.Controls.TimelineStatus` | writable; OneWay | `0 / default` | Default ; Default (accent), Success, Warning, Error, Pending; Pending is a static hollow marker without looping animation Validation: `Enum.IsDefined` Values: `Default`, `Success`, `Warning`, `Error`, `Pending` |
| [`Timestamp`](../../../TheOne.Avalonia/Controls/TimelineItemModel.cs#L17) | `object?` | writable; OneWay | `null` | null ; Timestamp content/template; null hides it. Above the body in Left/Right, opposite the rail in Alternate |

### TimelineMode

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.TimelineMode.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### TimelinePanel

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.TimelinePanel.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

### TimelineStatus

[Complete source contract](../../api/en/TheOne.Avalonia.Controls.TimelineStatus.md)

No additional CLR/attached properties are declared on this type; see its base for inherited members.

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Classes usage

No added public appearance classes; use the default theme and control-specific properties.
These are individually supported public classes. Hover, pressed, disabled and selected states are control-owned; do not assign template classes manually. See the labeled PreviewHost examples for their appearance.

### Business APIs

No additional business methods; configure the control-specific properties above.

### Related models

| Model | Properties/enum values |
| --- | ---: |
| [`TheOne.Avalonia.Controls.TimelineItemModel`](../../api/en/TheOne.Avalonia.Controls.TimelineItemModel.md) | 6 |
| [`TheOne.Avalonia.Controls.TimelineMode`](../../api/en/TheOne.Avalonia.Controls.TimelineMode.md) | 3 |
| [`TheOne.Avalonia.Controls.TimelineStatus`](../../api/en/TheOne.Avalonia.Controls.TimelineStatus.md) | 5 |

[Complete source contract (including constructors, property accessors and identifiers for precise lookup)](../../api/en/README.md)
<!-- generated-contracts:end -->
