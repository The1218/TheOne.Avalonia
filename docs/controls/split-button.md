# SplitButton 与 ToggleSplitButton

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

预览在按钮组合页展示主操作、切换、禁用及大量菜单选项。`--verify-buttons-messages` 包含真实指针／键盘、命令次数、菜单选择与虚拟回收检查。原生执行及定向渲染见[测试策略](../workflow/testing.md)；菜单与弹层见[布局文档](layout.md)。
