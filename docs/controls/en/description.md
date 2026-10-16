# Description

[简体中文](../zh-cn/description.md) · **English**

`Description` aligns labels and values in rows for control properties, APIs, configuration and model fields. `DescriptionItem.Header` supplies the label and `Content` supplies its description. `OneTheme` supplies the current palette, Light / Dark appearance and Small / Normal / Large density.

```xml
<one:Description LabelWidth="180" StackWidth="520">
  <one:DescriptionItem Header="Classes" Content="primary outline: an outlined accent button." />
  <one:DescriptionItem Header="IsEnabled" Content="Boolean, default True. False displays the disabled state." />
  <one:DescriptionItem Header="Command" Content="ICommand executed by the native button, preserving keyboard and focus behavior." />
</one:Description>
```

Use `xmlns:one="using:TheOne.Avalonia.Controls"`. Rows grow to fit their contents and wrap text. The control has no internal scrolling container; use an outer `ScrollViewer` for the page rather than fixing the description list height.

## Public properties

| Control / property | Type and default | Details |
| --- | --- | --- |
| `Description.LabelWidth` | `double`, `180` | Label column width in DIPs in two-column layouts. Bindable, styleable and inherited by rows; must be finite and nonnegative. |
| `Description.StackWidth` | `double`, `520` | Rows narrower than this value place the label above the value. `0` retains two columns; must be finite and nonnegative. |
| `DescriptionItem.LabelWidth` | `double`, inherited | Overrides the label width for one row. |
| `DescriptionItem.StackWidth` | `double`, inherited | Overrides the stacking threshold for one row. |
| `Description.Items` / `ItemsSource` | Native `ItemsControl` contract | Prefer explicit `DescriptionItem` instances or bind a collection of those rows. Collection changes use the native container lifecycle. Plain data receives value containers; callers explicitly configure their labels. |
| `DescriptionItem.Header` / `Content` | `object?`, `null` | Strings, controls or data models for labels and values. A null header retains the label column. |
| `DescriptionItem.HeaderTemplate` / `ContentTemplate` | `IDataTemplate?`, `null` | Native templates for either cell; declare the correct `x:DataType` on each template. |
| `Padding` / `FontSize` / `Foreground` | Native themed properties | List padding surrounds the whole list; row padding applies separately to the label and value cells. Font and text color can be inherited. |

`LabelWidth` and `StackWidth` are StyledProperties with the default OneWay binding mode; the control does not write back to them. It adds no dedicated events, methods or business models.

## Styles and density

| Setting | Effect |
| --- | --- |
| Default | 1 DIP outer border, rounded corners, subtle label backgrounds and row separators using `One.Surface`, `One.Subtle`, `One.Text` and `One.Line`. |
| `Classes="borderless"` | Removes the outer border and rounding while retaining cell and row separators. |
| `ControlSizing.Size="Small"` | Font size 12 and cell padding `10,7`. |
| `ControlSizing.Size="Normal"` | Font size 14 and cell padding `12,10`. |
| `ControlSizing.Size="Large"` | Font size 16 and cell padding `14,13`. |

Density uses `xmlns:styling="using:TheOne.Avalonia.Styling"` and may be set on the list or an ancestor. Internal `:first` and `:stacked` pseudoclasses reflect container indices and measured width; callers do not set them directly.

## Control content and compiled bindings

```xml
<one:Description>
  <one:DescriptionItem Header="Theme color">
    <one:ThemeColorPicker SelectThemeColor="{Binding SelectedColor, Mode=TwoWay}" />
  </one:DescriptionItem>
  <one:DescriptionItem Header="Notes" Content="{Binding Notes}" />
</one:Description>
```

Child controls retain native commands, focus and input handling. The description list has no selection state. Declare the view's correct `x:DataType`; application-owned models need no reflection binding, reflection factory or additional registration.

The equivalent code API is:

```csharp
var details = new Description { LabelWidth = 180, StackWidth = 520 };
details.Items.Add(new DescriptionItem { Header = "API", Content = "Native Header / Content and data templates." });
```

<!-- generated-contracts:start -->
## Additional properties, styles and complete API

This section lists only Classes, properties, extension APIs/events and related models added by this project, without repeating inherited Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost visibly compares project-added styles and uses Description components for control-specific properties, API signatures and model fields. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.

### Description

[API / model reference](../../api/en/TheOne.Avalonia.Controls.Description.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/Description.cs#L18) | `double` | writable; OneWay; inherits | `180` | `double`, `180` ; Label column width in DIPs in two-column layouts. Bindable, styleable and inherited by rows; must be finite and nonnegative. Validation: `value => double.IsFinite(value) && value >= 0` |
| [`StackWidth`](../../../TheOne.Avalonia/Controls/Description.cs#L21) | `double` | writable; OneWay; inherits | `520` | `double`, `520` ; Rows narrower than this value place the label above the value. `0` retains two columns; must be finite and nonnegative. Validation: `value => double.IsFinite(value) && value >= 0` |

### DescriptionItem

[API / model reference](../../api/en/TheOne.Avalonia.Controls.DescriptionItem.md)

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L18) | `double` | writable; OneWay; inherits | `180` | `double`, `180` ; Label column width in DIPs in two-column layouts. Bindable, styleable and inherited by rows; must be finite and nonnegative. Validation: `value => double.IsFinite(value) && value >= 0` |
| [`StackWidth`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L21) | `double` | writable; OneWay; inherits | `520` | `double`, `520` ; Rows narrower than this value place the label above the value. `0` retains two columns; must be finite and nonnegative. Validation: `value => double.IsFinite(value) && value >= 0` |

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.

### Style classes and automatic state

| Control | Class | Scope | Effective setters/resources |
| --- | --- | --- | --- |
| Description | `borderless` | explicit Classes; combine with density | ^.borderless => BorderThickness=0; CornerRadius=0 |
| Description | `one-large` | automatic template class; do not bind manually | ^.one-large => FontSize=16 |
| Description | `one-small` | automatic template class; do not bind manually | ^.one-small => FontSize=12 |
| DescriptionItem | `one-large` | automatic template class; do not bind manually | ^.one-large => Padding=14,13 |
| DescriptionItem | `one-small` | automatic template class; do not bind manually | ^.one-small => Padding=10,7 |

`:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without added appearance classes use their default theme and control-specific properties.

### APIs, events and built-in models

| Type | Kind | Properties/enum values | Methods/constructors | Events |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.Description`](../../api/en/TheOne.Avalonia.Controls.Description.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DescriptionItem`](../../api/en/TheOne.Avalonia.Controls.DescriptionItem.md) | Class | 2 | 1 | 0 |

[Complete public API index](../../api/en/README.md)
<!-- generated-contracts:end -->
