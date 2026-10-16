# ImagePreview

[English](../en/TheOne.Avalonia.Controls.ImagePreview.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ImagePreview.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ImagePreview` · Class · `Avalonia.Controls.Primitives.TemplatedControl`

A single/multiple image viewer for HTTP(S), local paths and caller-owned Bitmap instances.

[Source / 声明](../../../TheOne.Avalonia/Controls/ImagePreview.Loading.cs#L8)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanNext`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L56) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanPrevious`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L55) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ChromeHeight`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L49) | `double` | 可写; OneWay | `44` | 图片预览工具栏／装饰区高度，单位 DIP。 校验：`value => double.IsFinite(value) && value > 0` |
| [`CurrentImage`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L64) | `Avalonia.Media.IImage?` | 只读; CLR / n/a | `null` | The currently decoded image. Images created from paths are owned by the viewer. |
| [`DisplayTitle`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L59) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ErrorMessage`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L58) | `string?` | 只读; OneWay | `computed / state` | 当前失败／校验说明；展示错误状态，不能据此假定操作已经成功完成。 |
| [`HasError`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L53) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasImage`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L52) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ImageCount`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L50) | `int` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsFitToWindow`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L47) | `bool` | 可写; TwoWay | `true` | 启用适合视口的图片缩放；手动缩放与此状态的关系见图片预览交互约定。 |
| [`IsFullScreen`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L57) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L51) | `bool` | 只读; OneWay | `computed / state` | 显示忙碌指示；按钮保留尺寸并阻止重复操作，容器显示加载层。任务完成后由宿主复位。 |
| [`IsMultiple`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L54) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L42) | `System.Collections.IEnumerable?` | 可写; OneWay | `null` | 绑定原始模型集合，不预建大量 Control；动态增删使用带通知的集合，并提供有限视口。 |
| [`PageText`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L60) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`SelectedIndex`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L43) | `int` | 可写; TwoWay | `0` | 当前显示项的零基索引；越界处理遵循所属控件的选择契约。 |
| [`ShowCloseButton`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L46) | `bool` | 可写; OneWay | `false` | 显示关闭按钮；关闭行为和权限仍由宿主及相关命令决定。 |
| [`ShowHeader`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L45) | `bool` | 可写; OneWay | `true` | 显示标题区域；不删除模型中的标题内容。 |
| [`Source`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L41) | `object?` | 可写; OneWay | `null` | 源对象／地址；按控件支持的类型提供，外部资源的所有权与加载限制见用法章节。 |
| [`Title`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L44) | `string?` | 可写; OneWay | `null` | 显示标题；不等于业务主键或本地化资源键。 |
| [`ZoomFactor`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L48) | `double` | 可写; TwoWay | `1` | 图片显示缩放倍率；改变视图，不改写源图片像素。 校验：`value => double.IsFinite(value) && value is >= .001 and <= 64` |
| [`ZoomText`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L61) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`void ImagePreview.Previous()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L193) | 切换到上一张可用预览图片。 |
| [`void ImagePreview.Next()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L194) | 切换到下一张可用预览图片。 |
| [`void ImagePreview.Reload()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L195) | 重新加载当前图片；原始来源与错误状态遵循图片加载契约。 |
| [`void ImagePreview.FitToWindow()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L196) | 将图片缩放到适合当前视口，不修改源像素。 |
| [`void ImagePreview.ActualSize()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L200) | 按实际像素比例显示图片；视口可能出现滚动范围。 |
| [`void ImagePreview.ZoomIn()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L201) | 放大当前预览视图；遵守控件的缩放范围。 |
| [`void ImagePreview.ZoomOut()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L202) | 缩小当前预览视图；遵守控件的缩放范围。 |
| [`void ImagePreview.ToggleFullScreen()`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L224) | 切换图片预览的全屏展示状态。 |
| [`ImagePreview.ImagePreview()`](../../../TheOne.Avalonia/Controls/ImagePreview.Loading.cs#L8) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 事件

| 完整签名／值 | 说明 |
| --- | --- |
| [`event EventHandler<RoutedEventArgs>? ImagePreview.CloseRequested`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L62) | 状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<object?> ImagePreview.SourceProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L19) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<IEnumerable?> ImagePreview.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L20) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<int> ImagePreview.SelectedIndexProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<string?> ImagePreview.TitleProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L22) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> ImagePreview.ShowHeaderProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> ImagePreview.ShowCloseButtonProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L24) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<bool> ImagePreview.IsFitToWindowProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> ImagePreview.ZoomFactorProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L26) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<double> ImagePreview.ChromeHeightProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L27) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ImagePreview, int> ImagePreview.ImageCountProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L28) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.IsLoadingProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L29) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.HasImageProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L30) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.HasErrorProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L31) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.IsMultipleProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L32) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.CanPreviousProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L33) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.CanNextProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L34) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ImagePreview, bool> ImagePreview.IsFullScreenProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L35) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ImagePreview, string?> ImagePreview.ErrorMessageProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L36) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ImagePreview, string> ImagePreview.DisplayTitleProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L37) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ImagePreview, string> ImagePreview.PageTextProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L38) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<ImagePreview, string> ImagePreview.ZoomTextProperty`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L39) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<RoutedEventArgs> ImagePreview.CloseRequestedEvent`](../../../TheOne.Avalonia/Controls/ImagePreview.cs#L40) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

没有额外公开外观类；通过公开属性和默认主题配置。
