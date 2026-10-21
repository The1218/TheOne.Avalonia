# TabFeatures

[English](../en/TheOne.Avalonia.Controls.TabFeatures.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.TabFeatures.md) · [API index](README.md)

`TheOne.Avalonia.Controls.TabFeatures` · Class · `Avalonia.AvaloniaObject`

Optional closing for native tabs and strips; reordering remains specific to TabControl.

[Source / 声明](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L13)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanClose`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L21) | `bool` | 可写; OneWay; 附加属性 | `false` | 允许关闭；可显式设为 False |
| [`CanReorder`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L24) | `bool` | 可写; OneWay; 附加属性 | `false` | 允许当前 TabControl 内拖动与键盘排序 |
| [`CloseCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L33) | `System.Windows.Input.ICommand?` | 可写; OneWay; 附加属性 | `null` | 宿主关闭命令，参数为数据项；指定后由命令负责更新集合 |
| [`CloseTabCommand`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L66) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `new CloseTab()` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`EnableContextMenu`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L12) | `bool` | 可写; OneWay; 附加属性 | `false` | TabFeatures 的 EnableContextMenu 配置字段；值类型和初值如表，通过 EnableContextMenuProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`Icon`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L24) | `object?` | 可写; OneWay; 附加属性 | `null` | 图标内容或矢量几何，具体类型见签名；外部位图所有权仍由宿主负责。 |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L26) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay; 附加属性 | `DefaultIconTemplate` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`IsClosable`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L27) | `bool` | 可写; OneWay; 附加属性 | `true` | 设置在单个 TabItem 上，False 禁止关闭，排序仍允许 |
| [`IsIconVisible`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L30) | `bool` | 只读; OneWay; 附加属性 | `false` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsPinned`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L28) | `bool` | 可写; OneWay; 附加属性 | `false` | TabFeatures 的 IsPinned 配置字段；值类型和初值如表，通过 IsPinnedProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L30) | `bool` | 可写; OneWay; 附加属性 | `false` | 显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`ShowItemIcon`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L22) | `bool` | 可写; OneWay; 附加属性 | `true` | TabFeatures 的 ShowItemIcon 配置字段；值类型和初值如表，通过 ShowItemIconProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static bool TabFeatures.GetCanClose(TabStrip tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L21) | 读取附加属性。允许关闭；可显式设为 False |
| [`static void TabFeatures.SetCanClose(TabStrip tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L26) | 设置附加属性。允许关闭；可显式设为 False |
| [`static bool TabFeatures.GetIsClosable(TabStripItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L31) | 读取附加属性。设置在单个 TabItem 上，False 禁止关闭，排序仍允许 |
| [`static void TabFeatures.SetIsClosable(TabStripItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L36) | 设置附加属性。设置在单个 TabItem 上，False 禁止关闭，排序仍允许 |
| [`static bool TabFeatures.GetShowCloseButton(TabStripItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L41) | 读取附加属性。显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`static void TabFeatures.SetShowCloseButton(TabStripItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L46) | 设置附加属性。显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`static ICommand? TabFeatures.GetCloseCommand(TabStrip tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L51) | 读取附加属性。宿主关闭命令，参数为数据项；指定后由命令负责更新集合 |
| [`static void TabFeatures.SetCloseCommand(TabStrip tabs, ICommand? value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L56) | 设置附加属性。宿主关闭命令，参数为数据项；指定后由命令负责更新集合 |
| [`static bool TabFeatures.Close(TabStripItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L109) | 尝试关闭容器对应的数据项 |
| [`static bool TabFeatures.GetShowItemIcon(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L37) | 读取附加属性。TabFeatures 的 ShowItemIcon 配置字段；值类型和初值如表，通过 ShowItemIconProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`static bool TabFeatures.GetEnableContextMenu(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L38) | 读取附加属性。TabFeatures 的 EnableContextMenu 配置字段；值类型和初值如表，通过 EnableContextMenuProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`static void TabFeatures.SetEnableContextMenu(TabControl tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L39) | 设置附加属性。TabFeatures 的 EnableContextMenu 配置字段；值类型和初值如表，通过 EnableContextMenuProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`static void TabFeatures.SetShowItemIcon(TabControl tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L40) | 设置附加属性。TabFeatures 的 ShowItemIcon 配置字段；值类型和初值如表，通过 ShowItemIconProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`static object? TabFeatures.GetIcon(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L41) | 读取附加属性。图标内容或矢量几何，具体类型见签名；外部位图所有权仍由宿主负责。 |
| [`static void TabFeatures.SetIcon(TabItem item, object? value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L42) | 设置附加属性。图标内容或矢量几何，具体类型见签名；外部位图所有权仍由宿主负责。 |
| [`static IDataTemplate? TabFeatures.GetIconTemplate(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L43) | 读取附加属性。原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`static void TabFeatures.SetIconTemplate(TabItem item, IDataTemplate? value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L44) | 设置附加属性。原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`static bool TabFeatures.GetIsPinned(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L45) | 读取附加属性。TabFeatures 的 IsPinned 配置字段；值类型和初值如表，通过 IsPinnedProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`static void TabFeatures.SetIsPinned(TabItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L46) | 设置附加属性。TabFeatures 的 IsPinned 配置字段；值类型和初值如表，通过 IsPinnedProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`static bool TabFeatures.GetIsIconVisible(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L47) | 读取附加属性。只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`static bool TabFeatures.Pin(TabItem item, bool pinned = true)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L136) | Pins a page before all unpinned pages, preserving selection. Does not require drag reordering. |
| [`static int TabFeatures.CloseAll(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L155) | Closes a snapshot of closable pages, honoring cancellation, commands and pin state. |
| [`static bool TabFeatures.GetShowCloseButton(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L68) | 读取附加属性。显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`static void TabFeatures.SetShowCloseButton(TabItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L73) | 设置附加属性。显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`static bool TabFeatures.GetCanClose(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L85) | 读取附加属性。允许关闭；可显式设为 False |
| [`static void TabFeatures.SetCanClose(TabControl tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L90) | 设置附加属性。允许关闭；可显式设为 False |
| [`static bool TabFeatures.GetCanReorder(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L95) | 读取附加属性。允许当前 TabControl 内拖动与键盘排序 |
| [`static void TabFeatures.SetCanReorder(TabControl tabs, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L100) | 设置附加属性。允许当前 TabControl 内拖动与键盘排序 |
| [`static bool TabFeatures.GetIsClosable(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L105) | 读取附加属性。设置在单个 TabItem 上，False 禁止关闭，排序仍允许 |
| [`static void TabFeatures.SetIsClosable(TabItem item, bool value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L110) | 设置附加属性。设置在单个 TabItem 上，False 禁止关闭，排序仍允许 |
| [`static ICommand? TabFeatures.GetCloseCommand(TabControl tabs)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L115) | 读取附加属性。宿主关闭命令，参数为数据项；指定后由命令负责更新集合 |
| [`static void TabFeatures.SetCloseCommand(TabControl tabs, ICommand? value)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L120) | 设置附加属性。宿主关闭命令，参数为数据项；指定后由命令负责更新集合 |
| [`static bool TabFeatures.Close(TabItem item)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L182) | 尝试关闭容器对应的数据项 |
| [`static bool TabFeatures.Move(TabControl tabs, int from, int to)`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L224) | 按零基索引尝试重排 |
| [`TabFeatures.TabFeatures()`](../../../TheOne.Avalonia/Controls/TabFeatures.Close.cs#L13) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<bool> TabFeatures.EnableContextMenuProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L12) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> TabFeatures.ShowItemIconProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<object?> TabFeatures.IconProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<IDataTemplate?> TabFeatures.IconTemplateProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> TabFeatures.IsPinnedProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> TabFeatures.IsIconVisibleProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.Pages.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> TabFeatures.CanCloseProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> TabFeatures.CanReorderProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> TabFeatures.IsClosableProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L27) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> TabFeatures.ShowCloseButtonProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<ICommand?> TabFeatures.CloseCommandProperty`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<TabClosingEventArgs> TabFeatures.TabClosingEvent`](../../../TheOne.Avalonia/Controls/TabFeatures.cs#L36) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
