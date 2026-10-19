# ColorPicker / ColorView 颜色选择器

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

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
下拉弹层按当前页内容的自然高度展开，不受通用 Flyout 的 480 DIP 高度上限限制，Custom 的底部滑条无需滚动即可显示。若屏幕可用空间不足，仍由原生弹层定位和自动滚动保证所有内容可达；不强制隐藏滚动条或裁剪内容。该规则只作用于 ColorPicker 下拉弹层，不改变内嵌 ColorView 或其他 Flyout。
RGB / HSV、Hex 和数值编辑仍使用官方控件实例、命名部件与双向绑定。`IsColorComponentsVisible` 控制整个输入组；`IsColorSpectrumVisible="False"` 时仍可在 Custom 使用分量输入。
默认色板按 10 个色相列、5 档明度排列，包含中性色和主题青绿色；设置官方 `Palette` 属性可替换该色板。
默认隐藏底部大预览条及衍生颜色（`IsColorPreviewVisible="False"`、`IsAccentColorsVisible="False"`）；这些官方属性仍可由消费者显式开启。
真实色谱、透明度棋盘、颜色值和基于颜色对比度的色谱指示器继续由官方控件绘制，
不会被主题青绿色替换。

面板、文本、边框、选中标签及焦点反馈使用共享 `One.*` 资源，跟随 Light / Dark。
`ControlSizing.Size` 的 Small / Normal / Large 提供 30 / 36 / 44 px
下拉入口与文本输入高度。内嵌面板可适配 280–340 px 的宽度；禁用状态保留原生行为并显示主题反馈。
色谱支持 Tab 聚焦和官方方向键调色。

控件逻辑及基础模板由官方包提供：
[Avalonia ColorPicker 文档](https://docs.avaloniaui.net/controls/input/selectors/colorpicker)。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

本页的原生控件没有同名 TheOne 包装类；本项目增加的能力来自下列样式。

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

没有额外业务方法；通过本节专属属性配置。

### 关联模型

没有额外关联数据模型。

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
