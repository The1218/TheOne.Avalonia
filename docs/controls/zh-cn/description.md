# Description 说明列表

**简体中文** · [English](../en/description.md)

`Description` 将属性名和说明按行对齐，适合控件属性、API、配置和模型字段说明。`DescriptionItem` 的 `Header` 是名称，`Content` 是说明。加载 `OneTheme` 后自动使用当前配色、Light / Dark 和 Small / Normal / Large 密度。

```xml
<one:Description LabelWidth="180" StackWidth="520">
  <one:DescriptionItem Header="Classes" Content="primary outline：主色描边按钮。" />
  <one:DescriptionItem Header="IsEnabled" Content="Boolean，默认 True；False 时使用禁用状态。" />
  <one:DescriptionItem Header="Command" Content="ICommand；由原生按钮执行，保留键盘与焦点行为。" />
</one:Description>
```

命名空间：`xmlns:one="using:TheOne.Avalonia.Controls"`。每行内容自然增高，文本自动换行，不内置滚动容器。长页面由外层 `ScrollViewer` 统一滚动；不要为整个说明列表设置固定高度。

## 公共属性

| 控件 / 属性 | 类型与默认值 | 说明 |
| --- | --- | --- |
| `Description.LabelWidth` | `double`，`180` | 双列布局中名称列宽度，单位 DIP。可绑定、可通过样式设置，并继承至行；必须为有限非负值。 |
| `Description.StackWidth` | `double`，`520` | 行可用宽度小于此值时，名称移到内容上方。`0` 保持双列；必须为有限非负值。 |
| `DescriptionItem.LabelWidth` | `double`，继承列表 | 单独覆盖该行名称列宽度。 |
| `DescriptionItem.StackWidth` | `double`，继承列表 | 单独覆盖该行上下布局阈值。 |
| `Description.Items` / `ItemsSource` | 原生 `ItemsControl` 合约 | 推荐显式 `DescriptionItem`，也可绑定到 `DescriptionItem` 集合；集合变化沿用原生容器生命周期。普通数据会生成内容行，名称需要调用方明确配置。 |
| `DescriptionItem.Header` / `Content` | `object?`，`null` | 名称与内容可使用字符串、控件或数据模型；名称为空时仍保留名称列。 |
| `DescriptionItem.HeaderTemplate` / `ContentTemplate` | `IDataTemplate?`，`null` | 使用原生数据模板展示名称或内容。模板声明准确的 `x:DataType`。 |
| `Padding` / `FontSize` / `Foreground` | 原生主题属性 | 列表 `Padding` 控制整体内边距；行 `Padding` 分别作用于名称和内容单元格。字体与文字颜色可继承。 |

`LabelWidth` 和 `StackWidth` 为 OneWay 默认绑定模式的 StyledProperty；本控件不回写它们，未增加专属事件、方法或内置业务模型。

## 样式与密度

| 设置 | 效果 |
| --- | --- |
| 默认 | 1 DIP 外边框、圆角、浅色名称单元格及行分隔线，使用 `One.Surface`、`One.Subtle`、`One.Text`、`One.Line`。 |
| `Classes="borderless"` | 去除外框与圆角，保留单元格和行分隔线。 |
| `ControlSizing.Size="Small"` | 字号 12，单元格内边距 `10,7`。 |
| `ControlSizing.Size="Normal"` | 字号 14，单元格内边距 `12,10`。 |
| `ControlSizing.Size="Large"` | 字号 16，单元格内边距 `14,13`。 |

密度使用 `xmlns:styling="using:TheOne.Avalonia.Styling"`，可设置在列表或祖先控件上。内部 `:first`、`:stacked` 伪类由容器索引及测量宽度维护，不作为使用者手动设置的样式。

## 控件内容与编译绑定

```xml
<one:Description>
  <one:DescriptionItem Header="主题颜色">
    <one:ThemeColorPicker SelectThemeColor="{Binding SelectedColor, Mode=TwoWay}" />
  </one:DescriptionItem>
  <one:DescriptionItem Header="说明" Content="{Binding Notes}" />
</one:Description>
```

控件内容保留原生命令、焦点和输入行为。列表本身不提供选择状态。绑定所在视图声明准确的 `x:DataType`；数据模型采用应用自己的类型，无需反射绑定、反射工厂或额外注册。

代码创建与 XAML 相同：

```csharp
var details = new Description { LabelWidth = 180, StackWidth = 520 };
details.Items.Add(new DescriptionItem { Header = "API", Content = "使用原生 Header / Content 和数据模板。" });
```

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节只列出本项目增加的 Classes、属性、扩展 API／事件及关联模型，不重复 Avalonia 原生继承 API。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用可见示例对照本项目附加样式，用 Description 组件说明专属属性、API 签名与模型字段；ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### Description

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.Description.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/Description.cs#L18) | `double` | 可写; OneWay; 向后代继承 | `180` | `double`，`180` ; 双列布局中名称列宽度，单位 DIP。可绑定、可通过样式设置，并继承至行；必须为有限非负值。 校验：`value => double.IsFinite(value) && value >= 0` |
| [`StackWidth`](../../../TheOne.Avalonia/Controls/Description.cs#L21) | `double` | 可写; OneWay; 向后代继承 | `520` | `double`，`520` ; 行可用宽度小于此值时，名称移到内容上方。`0` 保持双列；必须为有限非负值。 校验：`value => double.IsFinite(value) && value >= 0` |

### DescriptionItem

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.DescriptionItem.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`LabelWidth`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L18) | `double` | 可写; OneWay; 向后代继承 | `180` | `double`，`180` ; 双列布局中名称列宽度，单位 DIP。可绑定、可通过样式设置，并继承至行；必须为有限非负值。 校验：`value => double.IsFinite(value) && value >= 0` |
| [`StackWidth`](../../../TheOne.Avalonia/Controls/DescriptionItem.cs#L21) | `double` | 可写; OneWay; 向后代继承 | `520` | `double`，`520` ; 行可用宽度小于此值时，名称移到内容上方。`0` 保持双列；必须为有限非负值。 校验：`value => double.IsFinite(value) && value >= 0` |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| Description | `borderless` | 显式 Classes，可与尺寸类组合 | ^.borderless => BorderThickness=0; CornerRadius=0 |
| Description | `one-large` | 模板自动类；不手动绑定 | ^.one-large => FontSize=16 |
| Description | `one-small` | 模板自动类；不手动绑定 | ^.one-small => FontSize=12 |
| DescriptionItem | `one-large` | 模板自动类；不手动绑定 | ^.one-large => Padding=14,13 |
| DescriptionItem | `one-small` | 模板自动类；不手动绑定 | ^.one-small => Padding=10,7 |

`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出附加外观类的控件使用默认主题及其专属属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.Description`](../../api/zh-cn/TheOne.Avalonia.Controls.Description.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DescriptionItem`](../../api/zh-cn/TheOne.Avalonia.Controls.DescriptionItem.md) | Class | 2 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
