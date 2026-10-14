# 按钮、输入、选择与反馈

[返回控件文档](README.md)。以下绑定属性与命令由业务 ViewModel 提供，根视图需声明 `x:DataType`。

外观扩展包括 `soft` / `gradient` 按钮、`filled` / `underlined` 输入、日期时间的 `filled` 及状态进度条。适用控件、渐变资源及选型见 [主题配色与样式](../theme/theme-colors.md)；每个控件选择一种主要外观，尺寸类可叠加。

## Button、RepeatButton、ToggleButton

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
  <Button Content="普通操作" Command="{Binding OpenCommand}" />
  <Button Classes="primary" Content="保存" Command="{Binding SaveCommand}" />
  <Button Classes="ghost" Content="取消" />
  <Button Classes="link" Content="查看详情" />
  <Button Classes="danger" Content="删除" Command="{Binding DeleteCommand}" />
</StackPanel>
```

`primary` 是主操作，`accent` 是兼容类名；`ghost` 为轻操作，`link` 为链接外观，`danger` 为危险操作。禁用使用 `IsEnabled="False"`。外观类不会自动实现确认对话框或业务逻辑。

```xml
<RepeatButton Content="持续增加" Command="{Binding IncrementCommand}"
              Delay="400" Interval="80" />
<ToggleButton Classes="segment" Content="网格视图"
              IsChecked="{Binding IsGridView, Mode=TwoWay}" />
```

RepeatButton 长按重复触发；`segment` 只提供分段外观。互斥选择由业务状态或同组 RadioButton 实现。

三个按钮的内容默认水平、垂直居中，small／normal／large 均使用实际内容尺寸布局。需要靠左或靠上时，显式设置 `HorizontalContentAlignment`／`VerticalContentAlignment`。

设置 `ToggleButton.IsThreeState="True"` 后，点击或 Space 按 false → true → null → false 循环。空值态使用 Primary 表面，并在按钮底部显示独立短横线；soft、gradient、primary、outline、ghost 等外观均保留该标记。标记跟随实际文字色，渐变和禁用状态仍保持可辨识。部分选中无需只靠背景色辨认。

## TextBox


```xml
<TextBox Classes="clearButton" PlaceholderText="邮箱"
         Text="{Binding Email, Mode=TwoWay}" />
<TextBox Classes="revealPasswordButton" PasswordChar="●"
         Text="{Binding Password, Mode=TwoWay}" />
<TextBox Classes="error" Text="需要修正的值" />
<TextBox IsReadOnly="True" Text="只读说明" />
<TextBox AcceptsReturn="True" TextWrapping="Wrap" MinHeight="96"
         Text="{Binding Notes, Mode=TwoWay}" />
```

`clearButton` 提供清除操作，`revealPasswordButton` 提供密码显示切换，`error` 仅设置显式错误外观。实际绑定验证仍使用 Avalonia 数据验证机制，真实验证错误也会触发主题错误状态；业务应同时给出具体错误文本。

单行输入的文本、占位文字和密码默认垂直居中，AutoCompleteBox 内嵌输入框同样适用。`AcceptsReturn="True"` 的多行输入默认顶部对齐；显式设置 `VerticalContentAlignment` 可覆盖默认值。

## CheckBox、RadioButton、ToggleSwitch

```xml
<CheckBox Content="接收通知" IsChecked="{Binding Notify, Mode=TwoWay}" />
<CheckBox Content="部分选择" IsThreeState="True" IsChecked="{x:Null}" />
<RadioButton GroupName="ExportFormat" Content="CSV" IsChecked="True" />
<RadioButton GroupName="ExportFormat" Content="JSON" />
<ToggleSwitch OnContent="已启用" OffContent="已停用"
              IsChecked="{Binding Enabled, Mode=TwoWay}" />
```

CheckBox 默认不绘制文字行背景，勾选框自身保留填色与状态标记；局部 Background 可自定义。CheckBox 支持 `bool?` 三态；RadioButton 的文字行默认透明，仅圆形标记承担选中／悬停／按下／禁用反馈，局部 Background 可覆盖。RadioButton 用 `GroupName` 指定互斥组；ToggleSwitch 整行默认透明，仅轨道和滑块显示开启／关闭及交互状态；禁用时滑块与轨道保留可辨识的色差。通过 `IsChecked` 表达开关状态，保留鼠标、Space 和滑块拖动操作。

## ComboBox 与 AutoCompleteBox

```xml
<ComboBox ItemsSource="{Binding Categories}"
          SelectedItem="{Binding SelectedCategory, Mode=TwoWay}" />
<AutoCompleteBox ItemsSource="{Binding Suggestions}"
                 Text="{Binding SearchText, Mode=TwoWay}"
                 MinimumPrefixLength="1" />
```

该示例的 `Categories` 和 `Suggestions` 为字符串集合。对象集合可设置 ItemTemplate；AutoCompleteBox 还需要相应的文本取值方式，例如 `ValueMemberBinding="{ReflectionBinding Name}"`。模板只负责候选项显示，搜索与远程数据加载由宿主实现。

下拉与建议列表使用主题的虚拟化模板；使用模型集合，不提前创建 ComboBoxItem。长列表弹层高度受 `One.DataFlyoutMaxHeight` 约束。

## NumericUpDown 与 ButtonSpinner

```xml
<NumericUpDown Minimum="0" Maximum="100" Increment="1"
               FormatString="0" Value="{Binding Quantity, Mode=TwoWay}" />
```

`Value` 为可空数值，业务属性建议匹配 `decimal?`。官方步进按钮和输入验证由控件提供。默认 NumericUpDown 已包含 ButtonSpinner；单独使用 ButtonSpinner 时，宿主处理 Spin 事件来更新自己的值：

```xml
<ButtonSpinner Spin="OnSpin">
  <TextBlock Text="自定义数值" />
</ButtonSpinner>
```

`OnSpin` 是视图中的事件处理方法，接收 `SpinEventArgs` 并按 `SpinDirection` 更新状态。可运行的输入示例见 [ControlGallery.axaml](../../design/PreviewHost/Verification/Fixtures/LegacyControlGallery.axaml)。

## Slider、ProgressBar、ToolTip

Slider 和 ScrollBar 的 Thumb 由原生模板管理拖动、捕获与位置；主题只修改外观。调整业务值使用父控件的 Value，替换模板时保留原生 Track / Thumb 结构。

```xml
<Slider Minimum="0" Maximum="100" TickFrequency="5"
        IsSnapToTickEnabled="True" Value="{Binding Progress, Mode=TwoWay}"
        ToolTip.Tip="拖动或使用方向键调整进度" />
<ProgressBar Minimum="0" Maximum="100" Value="{Binding Progress}" />
<ProgressBar IsIndeterminate="True" />
```

Slider 保留原生指针与键盘操作。ProgressBar 使用相同业务进度；未知总量时设置 `IsIndeterminate`。通过 `ToolTip.Tip` 添加提示，不需要手动创建弹层。

在 PreviewHost 的“选择控件对比”页可同时查看三种尺寸与明暗主题的固定状态样例，并在实时区域验证点击、键盘、三态、互斥、开关拖动、禁用及重置。

在 PreviewHost 的“按钮控件对比”页可比较三档尺寸、明暗主题、普通／悬停／按下／禁用状态，以及 ToggleButton 的选中状态和 primary 主按钮。实时区可测试单次操作、选中切换、鼠标／Space 长按重复、释放／禁用停止与重置。
