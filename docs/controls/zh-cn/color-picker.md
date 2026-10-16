# ColorPicker / ColorView 颜色选择器

[English](../en/color-picker.md) · [控件目录](README.md)

`OneTheme` 自动加载官方 `Avalonia.Controls.ColorPicker` **12.1.4** 及其 Fluent
颜色编辑模板，再叠加 TheOne 外观及独立的下拉按钮外框。直接使用官方 `Avalonia.Controls.ColorPicker` 下拉选择器
和内嵌 `ColorView`，不需要包装控件或额外加载颜色选择器主题。

```xml
<!-- 视图以实际 ViewModel 声明 x:DataType，并保持编译绑定。 -->
<StackPanel Spacing="16">
  <ColorPicker Color="{Binding SelectedColor, Mode=TwoWay}" />
  <ColorView Color="{Binding SelectedColor, Mode=TwoWay}"
             IsAlphaEnabled="True" />
</StackPanel>
```

`SelectedColor` 使用 `Avalonia.Media.Color`。官方 `HsvColor`、`ColorChanged`、
`ColorModel`、`Palette`、可见性开关、颜色范围、`IsAlphaEnabled`、`IsAlphaVisible`
及 `HexInputAlphaPosition` 均保留原有行为。输入八位 Hex 时按配置的透明度位置填写，
不要把它和 `Color.ToString()` 的 `#AARRGGBB` 顺序混为一谈。

下拉入口默认显示透明棋盘底色块与当前颜色值，悬停、按下和展开时由同一个圆角外框绘制边界；消费者显式设置 `Content` /
`ContentTemplate` 时仍以消费者内容为准。面板参考 Eremex 的紧凑颜色编辑器布局，默认打开色板（`SelectedIndex="1"`），仅提供 Custom / Palette 两个文字标签。
Custom（`SelectedIndex="0"`）在饱和度／亮度色谱下方集中显示 RGB / HSV 切换、横排四个分量输入、Hex、色相与透明度滑条；不再提供独立 Values 页（索引 2）。
内嵌面板与下拉弹层中的 RGB / HSV 模型按钮均保留 4 px 间距。
RGB / HSV、Hex 和数值编辑仍使用官方控件实例、命名部件与双向绑定。`IsColorComponentsVisible` 控制整个输入组；`IsColorSpectrumVisible="False"` 时仍可在 Custom 使用分量输入。
默认色板按 10 个色相列、5 档明度排列，包含中性色和主题青绿色；设置官方 `Palette` 属性可替换该色板。
默认隐藏底部大预览条及衍生颜色（`IsColorPreviewVisible="False"`、`IsAccentColorsVisible="False"`）；这些官方属性仍可由消费者显式开启。
真实色谱、透明度棋盘、颜色值和基于颜色对比度的色谱指示器继续由官方控件绘制，
不会被主题青绿色替换。

面板、文本、边框、选中标签及焦点反馈使用共享 `One.*` 资源，跟随 Light / Dark。
`ControlSizing.Size` 的 Small / Normal / Large 提供 30 / 36 / 44 px
下拉入口与文本输入高度。内嵌面板可适配 280–340 px 的宽度；禁用状态保留原生行为并显示主题反馈。
色谱支持 Tab 聚焦和官方方向键调色。

PreviewHost 导航包含 **ColorPicker**；`--colorpicker` 可直接打开桌面演示，
`--verify-colorpicker [output]` 只测试 ColorPicker / ColorView 的布局、主题、绑定、
指针、键盘和实际像素结果。
`--verify-colorpicker-model-spacing [output]` 仅验证模型按钮间距、文字容纳、原生指针／键盘切换及间隙像素，覆盖明暗主题和三档尺寸。

控件逻辑及基础模板由官方包提供：
[Avalonia ColorPicker 文档](https://docs.avaloniaui.net/controls/input/selectors/colorpicker)。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 在各控件自己的页面完整展示原始示例，并补充样式、额外属性、API／事件与内置模型；仅 Button、Input、Select 合并比较。

本页的原生控件没有同名 TheOne 包装类；额外能力来自以下样式及公共附加属性。

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
| ColorPicker | `one-large` | 模板自动类；不手动绑定 | ColorPicker.one-large => MinHeight=44; FontSize=16; ColorPicker.one-large /template/ DropDownButton => MinHeight=44 |
| ColorPicker | `one-normal` | 模板自动类；不手动绑定 | ColorPicker.one-normal /template/ DropDownButton => MinHeight=36 |
| ColorPicker | `one-small` | 模板自动类；不手动绑定 | ColorPicker.one-small => MinHeight=30; FontSize=12; ColorPicker.one-small /template/ DropDownButton => MinHeight=30 |
| ColorView | `one-large` | 模板自动类；不手动绑定 | :is(ColorView).one-large /template/ TextBox#PART_HexTextBox, :is(ColorView).one-large /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-large TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-large NumericUpDown => MinHeight=44; MaxHeight=44; FontSize=16; :is(ColorView).one-large /template/ TextBox#PART_HexTextBox, :is(ColorView).one-large /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-large TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-large NumericUpDown => MinHeight=44; MaxHeight=44; FontSize=16; :is(ColorView).one-large /template/ Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker-large Grid#HexInputGrid > Border => MinHeight=44; MaxHeight=44 |
| ColorView | `one-small` | 模板自动类；不手动绑定 | :is(ColorView).one-small /template/ TextBox#PART_HexTextBox, :is(ColorView).one-small /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-small TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-small NumericUpDown => MinHeight=30; MaxHeight=30; FontSize=12; :is(ColorView).one-small /template/ TextBox#PART_HexTextBox, :is(ColorView).one-small /template/ NumericUpDown, FlyoutPresenter.one-colorpicker-small TextBox#PART_HexTextBox, FlyoutPresenter.one-colorpicker-small NumericUpDown => MinHeight=30; MaxHeight=30; FontSize=12; :is(ColorView).one-small /template/ Grid#HexInputGrid > Border, FlyoutPresenter.one-colorpicker-small Grid#HexInputGrid > Border => MinHeight=30; MaxHeight=30 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
