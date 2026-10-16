# 图标按钮与操作层次

**简体中文** · [English](../en/icon-button.md)

IconButton 继承官方 Button，保留 Command、CommandParameter、Click、键盘、焦点、Flyout 与禁用行为。只需加载 OneTheme。Icon 为独立属性，Content 继续用于文字／内容；IconTemplate 用于数据图标。IconPlacement 支持 Left（默认）、Right、Top、Bottom，图标与文字始终居中排列，缺少图标或文字时不保留空白间隔。IsLoading 会用加载图标替换原图标并阻止重复操作，详见[加载控件](loading.md)。IconSize、IconSpacing 可通过样式或绑定覆盖，默认随 small / normal / large 使用 13 / 15 / 19px 图标。空 Icon 不占位，空 Content 使用方形图标按钮；纯图标按钮须配置 AutomationProperties.Name 与 Tooltip。

```xml
<one:IconButton Icon="{Binding AddGeometry}" Content="新建" Classes="primary"
                Command="{Binding AddCommand}" />
<one:IconButton AutomationProperties.Name="收藏" ToolTip.Tip="收藏">
  <one:IconButton.Icon><PathIcon Data="M8,1 L10,6 L15,6 L11,9 L13,15 L8,12 L3,15 L5,9 L1,6 L6,6 Z" /></one:IconButton.Icon>
</one:IconButton>
```

Geometry 由默认 PathIcon 模板渲染；已有 PathIcon 或其他图标 Control 可以直接传入 Icon。控件实例仅用于一个视觉父级，共享图标数据优先使用 Geometry 或模板。

Material 图标由消费项目引用 [Material.Icons.Avalonia](https://www.nuget.org/packages/Material.Icons.Avalonia/)，并在 Application.Styles 加载 MaterialIconStyles。类库不强制依赖第三方图标库；PreviewHost 引用 3.0.2 演示编译模板：

```xml
<!-- material: Material.Icons.Avalonia；kinds: Material.Icons -->
<one:IconButton Icon="{Binding IconKind}" Content="工作区">
  <one:IconButton.IconTemplate>
    <DataTemplate x:DataType="kinds:MaterialIconKind">
      <material:MaterialIcon Kind="{Binding}" />
    </DataTemplate>
  </one:IconButton.IconTemplate>
</one:IconButton>
```

官方 Button 与 IconButton 共用 primary/accent、soft、gradient、outline、ghost、link、success、info、warning、danger 样式。默认样式用于次要操作；primary 推进主要流程，ghost 为轻操作，outline 强调边界，success/info/warning/danger 表达完成、提示、风险与危险操作。悬停、按下、焦点、禁用保留原生状态与 TheOne 语义色。每组按钮宜只保留一个主要操作，文字明确说明操作。

编译绑定声明视图 x:DataType；数据 IconTemplate 声明图标数据类型。实现使用 StyledProperty、TemplateBinding 与编译模板，不反射创建图标。独立编译绑定和原生消费示例见 [NativeAotSmoke](../../../design/PreviewHost/Consumers/NativeAotSmoke/Program.cs)。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 在各控件自己的页面完整展示原始示例，并补充样式、额外属性、API／事件与内置模型；仅 Button、Input、Select 合并比较。

### ButtonLoading

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.ButtonLoading.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L13) | `bool` | 可写; OneWay; 附加属性 | `false` | 显示忙碌指示；按钮保留尺寸并阻止重复操作，容器显示加载层。任务完成后由宿主复位。 |

### IconButton

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.IconButton.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`HasIcon`](../../../TheOne.Avalonia/Controls/IconButton.cs#L30) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasLabel`](../../../TheOne.Avalonia/Controls/IconButton.cs#L31) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Icon`](../../../TheOne.Avalonia/Controls/IconButton.cs#L24) | `object?` | 可写; OneWay | `null` | 图标内容或矢量几何，具体类型见签名；外部位图所有权仍由宿主负责。 |
| [`IconPlacement`](../../../TheOne.Avalonia/Controls/IconButton.cs#L22) | `TheOne.Avalonia.Controls.IconPlacement` | 可写; OneWay | `0 / default` | 图标相对文本的位置枚举；左右或上下布局由模板处理。 校验：`value => Enum.IsDefined(value)` 可选值：`Left`, `Right`, `Top`, `Bottom` |
| [`IconSize`](../../../TheOne.Avalonia/Controls/IconButton.cs#L26) | `double` | 可写; OneWay | `15` | 图标绘制尺寸，单位 DIP；独立于文本字号与按钮点击区域。 校验：`value => double.IsFinite(value) && value > 0` |
| [`IconSpacing`](../../../TheOne.Avalonia/Controls/IconButton.cs#L27) | `double` | 可写; OneWay | `8` | 图标与相邻文本／内容的 DIP 间距。 校验：`value => double.IsFinite(value) && value >= 0` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/IconButton.cs#L25) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/IconButton.cs#L23) | `bool` | 可写; OneWay | `inherited registration` | 显示忙碌指示；按钮保留尺寸并阻止重复操作，容器显示加载层。任务完成后由宿主复位。 |
| [`ShowIcon`](../../../TheOne.Avalonia/Controls/IconButton.cs#L21) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### ThemeButton

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeButton.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Scope`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L16) | `TheOne.Avalonia.Controls.ThemeButtonScope` | 可写; OneWay | `0 / default` | 主题按钮切换的作用域；影响 Light/Dark，不切换 ThemeColor 颜色枚举。 可选值：`Window`, `Application` |

### 公共附加属性

[ControlSizing](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSizing.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | 可写; OneWay; 附加属性; 向后代继承 | `ControlSize.Normal` | Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 可选值：`Small`, `Normal`, `Large` |

[Motion](../../api/zh-cn/TheOne.Avalonia.Styling.Motion.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | 可写; OneWay; 附加属性; 向后代继承 | `true` | 向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| IconButton | `one-large` | 模板自动类；不手动绑定 | ^.one-large:icon-only => MinWidth=44; ^.one-large => IconSize=19; IconSpacing=10 |
| IconButton | `one-small` | 模板自动类；不手动绑定 | ^.one-small:icon-only => MinWidth=30; ^.one-small => IconSize=13; IconSpacing=6 |
| ThemeButton | `one-large` | 模板自动类；不手动绑定 | one&#124;ThemeButton.one-large => Width=44; Height=44 |
| ThemeButton | `one-normal` | 模板自动类；不手动绑定 | one&#124;ThemeButton.one-normal => Width=36; Height=36 |
| ThemeButton | `one-small` | 模板自动类；不手动绑定 | one&#124;ThemeButton.one-small => Width=30; Height=30 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.ButtonLoading`](../../api/zh-cn/TheOne.Avalonia.Controls.ButtonLoading.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.IconButton`](../../api/zh-cn/TheOne.Avalonia.Controls.IconButton.md) | Class | 9 | 1 | 0 |
| [`TheOne.Avalonia.Controls.IconPlacement`](../../api/zh-cn/TheOne.Avalonia.Controls.IconPlacement.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ThemeButton`](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeButton.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.ThemeButtonScope`](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeButtonScope.md) | Enum | 2 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
