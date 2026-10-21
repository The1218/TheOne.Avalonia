# AttachmentList

[English](../en/TheOne.Avalonia.Controls.AttachmentList.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.AttachmentList.md) · [API index](README.md)

`TheOne.Avalonia.Controls.AttachmentList` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

A multi-file picker and attachment presenter. The consumer owns accepted selections and business commands.

[Source / 声明](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L16)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Description`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L57) | `string?` | 可写; OneWay | `null` | 描述文本；由宿主提供业务说明，未提供时沿用初值。 |
| [`EmptyText`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L59) | `string` | 可写; OneWay | `"暂无附件"` | 没有可显示数据时的提示文本；由应用覆盖以进行本地化。 |
| [`ErrorMessage`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L72) | `string?` | 只读; OneWay | `computed / state` | 当前失败／校验说明；展示错误状态，不能据此假定操作已经成功完成。 |
| [`FilePicker`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L69) | `TheOne.Avalonia.Controls.IAttachmentFilePicker?` | 可写; OneWay | `null` | AttachmentList 的 FilePicker 配置字段；值类型和初值如表，通过 FilePickerProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`Files`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L64) | `System.Collections.Generic.IEnumerable<TheOne.Avalonia.Controls.IAttachmentItem>?` | 可写; OneWay | `null` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`HasFiles`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L70) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HintText`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L60) | `string?` | 可写; OneWay | `"可一次选择多个文件。"` | AttachmentList 的 HintText 配置字段；值类型和初值如表，通过 HintTextProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`IsBusy`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L63) | `bool` | 可写; OneWay | `false` | 上传处理中的状态；由异步上传过程维护，业务提交应尊重此状态。 |
| [`IsRemoveVisible`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L62) | `bool` | 可写; OneWay | `true` | AttachmentList 的 IsRemoveVisible 配置字段；值类型和初值如表，通过 IsRemoveVisibleProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`IsSelecting`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L71) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsUploadVisible`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L61) | `bool` | 可写; OneWay | `true` | AttachmentList 的 IsUploadVisible 配置字段；值类型和初值如表，通过 IsUploadVisibleProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`RemoveCommand`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L68) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | Receives the original item. The control never mutates Files or deletes a server file. |
| [`RequestPreviewCommand`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L75) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`RequestRemoveCommand`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L74) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`SelectCommand`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L73) | `System.Windows.Input.ICommand` | 只读; CLR / n/a | `accessor-defined / state` | 宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。 |
| [`Title`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L56) | `string` | 可写; OneWay | `"附件上传"` | 显示标题；不等于业务主键或本地化资源键。 |
| [`UploadButtonText`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L58) | `string` | 可写; OneWay | `"上传附件"` | AttachmentList 的 UploadButtonText 配置字段；值类型和初值如表，通过 UploadButtonTextProperty 参与 Avalonia 属性系统与通知。具体上下文见控件用法及链接处的声明／访问器。 |
| [`UploadCommand`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L66) | `System.Windows.Input.ICommand?` | 可写; OneWay | `null` | Receives one IReadOnlyList<UploadFile> batch. Accepted files must be disposed by the command owner. |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`Task AttachmentList.SelectFilesAsync(CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L87) | Selects a true multi-file batch. Cancellation preserves Files. Only accepted commands receive ownership. |
| [`AttachmentList.AttachmentList()`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L43) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler<AttachmentPreviewRequestedEventArgs>? AttachmentList.PreviewRequested`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L76) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<string> AttachmentList.TitleProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> AttachmentList.DescriptionProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> AttachmentList.UploadButtonTextProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string> AttachmentList.EmptyTextProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> AttachmentList.HintTextProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> AttachmentList.IsUploadVisibleProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> AttachmentList.IsRemoveVisibleProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> AttachmentList.IsBusyProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IEnumerable<IAttachmentItem>?> AttachmentList.FilesProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ICommand?> AttachmentList.UploadCommandProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L27) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ICommand?> AttachmentList.RemoveCommandProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IAttachmentFilePicker?> AttachmentList.FilePickerProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<AttachmentList, bool> AttachmentList.HasFilesProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<AttachmentList, bool> AttachmentList.IsSelectingProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L31) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<AttachmentList, string?> AttachmentList.ErrorMessageProperty`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L32) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<AttachmentPreviewRequestedEventArgs> AttachmentList.PreviewRequestedEvent`](../../../TheOne.Avalonia/Controls/AttachmentList.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.AttachmentPreviewRequestedEventArgs`](TheOne.Avalonia.Controls.AttachmentPreviewRequestedEventArgs.md)
- [`TheOne.Avalonia.Controls.IAttachmentFilePicker`](TheOne.Avalonia.Controls.IAttachmentFilePicker.md)
- [`TheOne.Avalonia.Controls.IAttachmentItem`](TheOne.Avalonia.Controls.IAttachmentItem.md)
