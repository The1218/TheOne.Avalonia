# TabFeatures

[English](../en/TheOne.Avalonia.Controls.TabFeatures.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TabFeatures.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TabFeatures` · Class · `Avalonia.AvaloniaObject`

Optional closing for native tabs and strips; reordering remains specific to TabControl.

[Source / 声明](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L13)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanClose`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L21) | `bool` | 可写; OneWay; 附加属性 | `false` | 允许关闭；可显式设为 False |
| [`CanReorder`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L22) | `bool` | 可写; OneWay; 附加属性 | `false` | 允许当前 TabControl 内拖动与键盘排序 |
| [`CloseCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L33) | `System.Windows.Input.ICommand?` | 可写; OneWay; 附加属性 | `null` | 宿主关闭命令，参数为数据项；指定后由命令负责更新集合 |
| [`CloseTabCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L35) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `new CloseTab()` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`IsClosable`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L23) | `bool` | 可写; OneWay; 附加属性 | `true` | 设置在单个 TabItem 上，False 禁止关闭，排序仍允许 |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L24) | `bool` | 可写; OneWay; 附加属性 | `false` | 显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static bool TabFeatures.GetCanClose(TabStrip tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L17) | 读取附加属性。允许关闭；可显式设为 False |
| [`static void TabFeatures.SetCanClose(TabStrip tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L18) | 设置附加属性。允许关闭；可显式设为 False |
| [`static bool TabFeatures.GetIsClosable(TabStripItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L19) | 读取附加属性。设置在单个 TabItem 上，False 禁止关闭，排序仍允许 |
| [`static void TabFeatures.SetIsClosable(TabStripItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L20) | 设置附加属性。设置在单个 TabItem 上，False 禁止关闭，排序仍允许 |
| [`static bool TabFeatures.GetShowCloseButton(TabStripItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L21) | 读取附加属性。显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`static void TabFeatures.SetShowCloseButton(TabStripItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L22) | 设置附加属性。显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`static ICommand? TabFeatures.GetCloseCommand(TabStrip tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L23) | 读取附加属性。宿主关闭命令，参数为数据项；指定后由命令负责更新集合 |
| [`static void TabFeatures.SetCloseCommand(TabStrip tabs, ICommand? value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L24) | 设置附加属性。宿主关闭命令，参数为数据项；指定后由命令负责更新集合 |
| [`static bool TabFeatures.Close(TabStripItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L55) | 尝试关闭容器对应的数据项 |
| [`static bool TabFeatures.GetShowCloseButton(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L25) | 读取附加属性。显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`static void TabFeatures.SetShowCloseButton(TabItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L26) | 设置附加属性。显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`static bool TabFeatures.GetCanClose(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L50) | 读取附加属性。允许关闭；可显式设为 False |
| [`static void TabFeatures.SetCanClose(TabControl tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L51) | 设置附加属性。允许关闭；可显式设为 False |
| [`static bool TabFeatures.GetCanReorder(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L52) | 读取附加属性。允许当前 TabControl 内拖动与键盘排序 |
| [`static void TabFeatures.SetCanReorder(TabControl tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L53) | 设置附加属性。允许当前 TabControl 内拖动与键盘排序 |
| [`static bool TabFeatures.GetIsClosable(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L54) | 读取附加属性。设置在单个 TabItem 上，False 禁止关闭，排序仍允许 |
| [`static void TabFeatures.SetIsClosable(TabItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L55) | 设置附加属性。设置在单个 TabItem 上，False 禁止关闭，排序仍允许 |
| [`static ICommand? TabFeatures.GetCloseCommand(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L56) | 读取附加属性。宿主关闭命令，参数为数据项；指定后由命令负责更新集合 |
| [`static void TabFeatures.SetCloseCommand(TabControl tabs, ICommand? value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L57) | 设置附加属性。宿主关闭命令，参数为数据项；指定后由命令负责更新集合 |
| [`static bool TabFeatures.Close(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L91) | 尝试关闭容器对应的数据项 |
| [`static bool TabFeatures.Move(TabControl tabs, int from, int to)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L119) | 按零基索引尝试重排 |
| [`TabFeatures.TabFeatures()`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L13) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<bool> TabFeatures.CanCloseProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> TabFeatures.CanReorderProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> TabFeatures.IsClosableProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> TabFeatures.ShowCloseButtonProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<ICommand?> TabFeatures.CloseCommandProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<TabClosingEventArgs> TabFeatures.TabClosingEvent`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L34) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 新增导航契约

固定页显示固钉，不能拖动，也不能通过 Move 或排序快捷键移动；普通页的拖动预览和放置位置不能越过固定区。ShowItemIcon 不隐藏固钉。简短示例见下方用法链接。

[用法与示例](../../controls/zh-cn/collections.md)

| Name | Type | Default / binding |
| --- | --- | --- |
| [EnableContextMenu](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L12) | `bool` | `false`; OneWay |
| [Icon](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L18) | `object?` | `null`; OneWay |
| [IconTemplate](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L20) | `Avalonia.Controls.Templates.IDataTemplate?` | `DefaultIconTemplate`; OneWay |
| [IsIconVisible](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L24) | `bool` | `false`; OneWay |
| [IsPinned](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L22) | `bool` | `false`; OneWay |
| [ShowItemIcon](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L16) | `bool` | `true`; OneWay |

`static bool TabFeatures.Pin(TabItem item, bool pinned = true)`

`static int TabFeatures.CloseAll(TabControl tabs)`
