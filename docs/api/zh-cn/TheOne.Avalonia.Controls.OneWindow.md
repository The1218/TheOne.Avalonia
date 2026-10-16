# OneWindow

[English](../en/TheOne.Avalonia.Controls.OneWindow.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneWindow.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneWindow` · Class · `Avalonia.Controls.Window`

A native Window with a customizable title bar and automatically managed message hosts.

[Source / 声明](../../../TheOne.Avalonia/Controls/OneWindow.cs#L12)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Current`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L39) | `TheOne.Avalonia.Controls.OneWindow?` | 只读; CLR / n/a | `accessor-defined / state` | UI 线程。 |
| [`IsDialogOpen`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L38) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`LeftContent`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L30) | `object?` | 可写; OneWay | `null` | 窗口标题栏左侧扩展内容，位于库提供的标题栏结构内。 |
| [`LeftContentTemplate`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L32) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`RightContent`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L31) | `object?` | 可写; OneWay | `null` | 窗口标题栏右侧扩展内容；避免覆盖原生窗口操作区域。 |
| [`RightContentTemplate`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L33) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`ShowThemeButton`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L34) | `bool` | 可写; OneWay | `true` | 显示窗口内置明暗切换按钮；配色资源仍由 ThemeColorPicker 或 OneTheme 控制。 |
| [`ShowTitleBar`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L36) | `bool` | 可写; OneWay | `true` | 显示库提供的标题栏区域；窗口生命周期与 owner 不因此变化。 |
| [`ThemeButtonScope`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L35) | `TheOne.Avalonia.Controls.ThemeButtonScope` | 可写; OneWay | `0 / default` | 窗口内置主题按钮的切换作用域；使用枚举区分应用与窗口。 可选值：`Window`, `Application` |
| [`TitleBarHeight`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L37) | `double` | 可写; OneWay | `48` | 标题栏区域的 DIP 高度；改变时保留标题与窗口按钮点击空间。 校验：`value => double.IsFinite(value) && value >= 0` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`OneWindow.OneWindow()`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L41) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<object?> OneWindow.LeftContentProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L14) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<object?> OneWindow.RightContentProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L15) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> OneWindow.LeftContentTemplateProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> OneWindow.RightContentTemplateProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L17) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> OneWindow.ShowThemeButtonProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ThemeButtonScope> OneWindow.ThemeButtonScopeProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> OneWindow.ShowTitleBarProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> OneWindow.TitleBarHeightProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<OneWindow, bool> OneWindow.IsDialogOpenProperty`](../../../TheOne.Avalonia/Controls/OneWindow.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.ThemeButtonScope`](TheOne.Avalonia.Controls.ThemeButtonScope.md)
