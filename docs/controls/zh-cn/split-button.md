# SplitButton 与 ToggleSplitButton

**简体中文** · [English](../en/split-button.md)

两者沿用 Avalonia 原生控件。主区域执行 Command，箭头区域打开 Flyout；打开菜单不执行主命令。ToggleSplitButton 的主区域额外切换 bool 类型的 IsChecked，箭头和菜单不会改变勾选状态。它不提供 ToggleButton 的 bool? 三态。

```xml
<SplitButton Content="导出" Command="{Binding ExportCommand}"
             CommandParameter="PDF" Classes="primary">
  <SplitButton.Flyout>
    <MenuFlyout>
      <MenuItem Header="导出 PDF" Command="{Binding ExportCommand}" CommandParameter="PDF" />
      <MenuItem Header="导出 CSV" Command="{Binding ExportCommand}" CommandParameter="CSV" />
    </MenuFlyout>
  </SplitButton.Flyout>
</SplitButton>
<ToggleSplitButton Content="自动同步" IsChecked="{Binding AutoSync, Mode=TwoWay}">
  <SplitButton.Flyout>
    <MenuFlyout><MenuItem Header="立即同步" Command="{Binding SyncCommand}" /></MenuFlyout>
  </SplitButton.Flyout>
</ToggleSplitButton>
```

视图声明 VM 的 x:DataType，命令及 AutoSync 由消费方提供。Content / ContentTemplate、Command / CommandParameter、Flyout、IsEnabled 均使用原生属性；没有 Flyout 时箭头没有可打开的内容。命令 CanExecute 和禁用状态保留原生语义，不在点击事件重复执行同一命令。

焦点位于控件时 Enter / Space 执行主操作，F4 或 Alt + ↓ 打开菜单，Esc 关闭并返回触发控件。两区各自反馈悬停和按下，外框保持连续。支持默认、primary 和 gradient 外观，small / normal / large 继承 30 / 36 / 44 px 的最小高度；显式 Padding、CornerRadius、BorderThickness 优先。

大量选项使用 MenuFlyout.ItemsSource 和有类型的数据模板，让原生菜单按需创建容器；不要提前创建成千上万个 MenuItem。ItemContainerTheme 的 x:DataType 应是选项模型，并用编译绑定设置 Command / CommandParameter。Flyout 中需要不同 VM 时显式提供有类型的数据上下文，不能关闭编译绑定。

预览在按钮组合页展示主操作、切换、禁用及大量菜单选项。`--verify-buttons-messages` 包含真实指针／键盘、命令次数、菜单选择与虚拟回收检查。原生执行及定向渲染见[预览专项入口](../../../design/PreviewHost/Program.cs)；菜单与弹层见[布局文档](layout.md)。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 的“控件能力实验室”（各页右上入口）按样式、属性、API／事件和模型分类演示；复杂模型使用原业务示例。

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
| SplitButton | `accent` | 显式 Classes，可与尺寸类组合 | ^.accent => Background={DynamicResource One.Accent}; Foreground={DynamicResource One.OnAccent}; BorderBrush={DynamicResource One.Accent}; ^.accent:checked => Background={DynamicResource One.AccentPressed}; BorderBrush={DynamicResource One.AccentPressed}; ^.accent /template/ Border#SeparatorBorder => Background={DynamicResource One.OnAccent}; Opacity=0.35; ^:disabled, ^:checked:disabled, ^.primary:disabled, ^.accent:disabled, ^.primary:checked:disabled, ^.accent:checked:disabled => Background={DynamicResource One.DisabledSurface}; Foreground={DynamicResource One.DisabledText}; BorderBrush={DynamicResource One.Line}; ^:disabled, ^:checked:disabled, ^.primary:disabled, ^.accent:disabled, ^.primary:checked:disabled, ^.accent:checked:disabled => Background={DynamicResource One.DisabledSurface}; Foreground={DynamicResource One.DisabledText}; BorderBrush={DynamicResource One.Line}; :is(SplitButton).accent /template/ Button:pointerover /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).accent:checked /template/ Button:pointerover /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentHover}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton).accent /template/ Button:pointerover /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).accent:checked /template/ Button:pointerover /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentHover}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton).accent /template/ Button:pressed /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).accent:checked /template/ Button:pressed /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentPressed}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton).accent /template/ Button:pressed /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).accent:checked /template/ Button:pressed /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentPressed}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton).accent:pressed /template/ Button#PART_PrimaryButton /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentPressed}; :is(SplitButton).accent:flyout-open /template/ Button#PART_SecondaryButton /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentPressed}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton):disabled /template/ Button /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).primary:disabled /template/ Button /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).accent:disabled /template/ Button /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.DisabledSurface}; Foreground={DynamicResource One.DisabledText}; :is(SplitButton):disabled /template/ Border#SeparatorBorder, :is(SplitButton).primary:disabled /template/ Border#SeparatorBorder, :is(SplitButton).accent:disabled /template/ Border#SeparatorBorder => Background={DynamicResource One.Line}; Opacity=1 |
| SplitButton | `gradient` | 显式 Classes，可与尺寸类组合 | :is(SplitButton).gradient => Background={DynamicResource One.Gradient.Brand}; Foreground={DynamicResource One.OnAccent}; BorderBrush={DynamicResource One.Accent}; :is(SplitButton).gradient /template/ Button /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Gradient.Brand}; Foreground={DynamicResource One.OnAccent}; BorderBrush=Transparent; :is(SplitButton).gradient /template/ Button:pointerover /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Gradient.BrandHover}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton).gradient /template/ Button:pressed /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).gradient:pressed /template/ Button#PART_PrimaryButton /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Gradient.BrandPressed}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton).gradient /template/ Button:pressed /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).gradient:pressed /template/ Button#PART_PrimaryButton /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Gradient.BrandPressed}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton).gradient:checked /template/ Button /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).gradient:flyout-open /template/ Button#PART_SecondaryButton /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Gradient.BrandPressed}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton).gradient:checked /template/ Button /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).gradient:flyout-open /template/ Button#PART_SecondaryButton /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.Gradient.BrandPressed}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton).gradient:disabled => Background={DynamicResource One.DisabledSurface}; Foreground={DynamicResource One.DisabledText}; BorderBrush={DynamicResource One.Line}; :is(SplitButton).gradient:disabled /template/ Button /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.DisabledSurface}; Foreground={DynamicResource One.DisabledText}; BorderBrush={DynamicResource One.Line}; :is(SplitButton).gradient /template/ Border#SeparatorBorder => Background={DynamicResource One.OnAccent}; Opacity=0.35; :is(SplitButton).gradient:disabled /template/ Border#SeparatorBorder => Background={DynamicResource One.Line}; Opacity=1 |
| SplitButton | `one-large` | 模板自动类；不手动绑定 | :is(SplitButton).one-large => MinHeight={DynamicResource One.Size.Large.Height}; FontSize=16; Padding=16,6; :is(SplitButton).one-large /template/ Button#PART_SecondaryButton => Width={DynamicResource One.SplitButton.Large.SecondaryWidth}; :is(SplitButton).one-large /template/ Button#PART_PrimaryButton.one-large => MinHeight=0; MinWidth=0; Padding={Binding $parent[SplitButton].Padding}; :is(SplitButton).one-large /template/ Button#PART_SecondaryButton.one-large => MinHeight=0; MinWidth=0; Padding=0 |
| SplitButton | `one-normal` | 模板自动类；不手动绑定 | :is(SplitButton).one-normal => MinHeight={DynamicResource One.Size.Height}; FontSize=14; Padding=12,7; :is(SplitButton).one-normal /template/ Button#PART_SecondaryButton => Width={DynamicResource One.SplitButton.SecondaryWidth}; :is(SplitButton).one-normal /template/ Button#PART_PrimaryButton.one-normal => MinHeight=0; MinWidth=0; Padding={Binding $parent[SplitButton].Padding}; :is(SplitButton).one-normal /template/ Button#PART_SecondaryButton.one-normal => MinHeight=0; MinWidth=0; Padding=0 |
| SplitButton | `one-small` | 模板自动类；不手动绑定 | :is(SplitButton).one-small => MinHeight={DynamicResource One.Size.Small.Height}; FontSize=12; Padding=10,4; :is(SplitButton).one-small /template/ Button#PART_SecondaryButton => Width={DynamicResource One.SplitButton.Small.SecondaryWidth}; :is(SplitButton).one-small /template/ Button#PART_PrimaryButton.one-small => MinHeight=0; MinWidth=0; Padding={Binding $parent[SplitButton].Padding}; :is(SplitButton).one-small /template/ Button#PART_SecondaryButton.one-small => MinHeight=0; MinWidth=0; Padding=0 |
| SplitButton | `primary` | 显式 Classes，可与尺寸类组合 | ^.primary => Background={DynamicResource One.Accent}; Foreground={DynamicResource One.OnAccent}; BorderBrush={DynamicResource One.Accent}; ^.primary:checked => Background={DynamicResource One.AccentPressed}; BorderBrush={DynamicResource One.AccentPressed}; ^.primary /template/ Border#SeparatorBorder => Background={DynamicResource One.OnAccent}; Opacity=0.35; ^:disabled, ^:checked:disabled, ^.primary:disabled, ^.accent:disabled, ^.primary:checked:disabled, ^.accent:checked:disabled => Background={DynamicResource One.DisabledSurface}; Foreground={DynamicResource One.DisabledText}; BorderBrush={DynamicResource One.Line}; ^:disabled, ^:checked:disabled, ^.primary:disabled, ^.accent:disabled, ^.primary:checked:disabled, ^.accent:checked:disabled => Background={DynamicResource One.DisabledSurface}; Foreground={DynamicResource One.DisabledText}; BorderBrush={DynamicResource One.Line}; :is(SplitButton).primary /template/ Button:pointerover /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).primary:checked /template/ Button:pointerover /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentHover}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton).primary /template/ Button:pointerover /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).primary:checked /template/ Button:pointerover /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentHover}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton).primary /template/ Button:pressed /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).primary:checked /template/ Button:pressed /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentPressed}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton).primary /template/ Button:pressed /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).primary:checked /template/ Button:pressed /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentPressed}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton).primary:pressed /template/ Button#PART_PrimaryButton /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentPressed}; :is(SplitButton).primary:flyout-open /template/ Button#PART_SecondaryButton /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.AccentPressed}; Foreground={DynamicResource One.OnAccent}; :is(SplitButton):disabled /template/ Button /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).primary:disabled /template/ Button /template/ ContentPresenter#PART_ContentPresenter, :is(SplitButton).accent:disabled /template/ Button /template/ ContentPresenter#PART_ContentPresenter => Background={DynamicResource One.DisabledSurface}; Foreground={DynamicResource One.DisabledText}; :is(SplitButton):disabled /template/ Border#SeparatorBorder, :is(SplitButton).primary:disabled /template/ Border#SeparatorBorder, :is(SplitButton).accent:disabled /template/ Border#SeparatorBorder => Background={DynamicResource One.Line}; Opacity=1 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
