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
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

本页的原生控件没有同名 TheOne 包装类；本项目增加的能力来自下列样式。

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

- `SplitButton`: `Classes="accent"` / `Classes="gradient"` / `Classes="primary"`

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

没有额外业务方法；通过本节专属属性配置。

### 关联模型

没有额外关联数据模型。

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
