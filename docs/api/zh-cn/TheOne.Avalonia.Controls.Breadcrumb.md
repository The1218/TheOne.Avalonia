# Breadcrumb

[English](../en/TheOne.Avalonia.Controls.Breadcrumb.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.Breadcrumb.md) · [API index](README.md)

`TheOne.Avalonia.Controls.Breadcrumb` · Class · `Avalonia.Controls.ItemsControl`

A caller-owned navigation path. The final item is the noninteractive current page.

[Source / 声明](../../../TheOne.Avalonia/Controls/Breadcrumb.Input.cs#L9)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CurrentItem`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L43) | `object?` | 只读; OneWay | `computed / state` | null ; 只读当前末项；`breadcrumb.CurrentItem`。 |
| [`ItemInvokedCommand`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L42) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | null ; `ItemInvokedCommand="{CompiledBinding NavigateCommand}"`；参数是原始项，不是 Key 或索引。 |
| [`Separator`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L39) | `object?` | 可写; OneWay | `"/"` | `"/"` ; `Separator="›"` 替换分隔符；`Separator="{x:Null}"` 隐藏。使用可重复的值；视觉内容用模板创建独立实例。 |
| [`SeparatorTemplate`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L40) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | null ; 分隔符模板。与路径项分离，不参与导航或焦点。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`Breadcrumb.Breadcrumb()`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L37) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler<BreadcrumbItemInvokedEventArgs>? Breadcrumb.ItemInvoked`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L44) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<object?> Breadcrumb.SeparatorProperty`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IDataTemplate?> Breadcrumb.SeparatorTemplateProperty`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ICommand?> Breadcrumb.ItemInvokedCommandProperty`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<Breadcrumb, object?> Breadcrumb.CurrentItemProperty`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<BreadcrumbItemInvokedEventArgs> Breadcrumb.ItemInvokedEvent`](../../../TheOne.Avalonia/Controls/Breadcrumb.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.BreadcrumbItem`](TheOne.Avalonia.Controls.BreadcrumbItem.md)
- [`TheOne.Avalonia.Controls.BreadcrumbItemInvokedEventArgs`](TheOne.Avalonia.Controls.BreadcrumbItemInvokedEventArgs.md)
- [`TheOne.Avalonia.Controls.BreadcrumbItemModel`](TheOne.Avalonia.Controls.BreadcrumbItemModel.md)
