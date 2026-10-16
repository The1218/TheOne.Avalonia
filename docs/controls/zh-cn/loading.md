# 加载状态与 LoadingContainer

**简体中文** · [English](../en/loading.md)

普通 Button 保留官方类型，使用 `ButtonLoading.IsLoading` 附加属性；IconButton 直接提供 `IsLoading`。两者加载期间显示指示图标并禁用原生输入、Command、Click 与 Flyout 触发。普通 Button 的 IsEnabled 值／绑定保持不变（包括 TwoWay），内部利用原生命令可执行状态临时阻止输入，加载结束后恢复原命令，并继续遵守原来的禁用状态。Command 按原生默认 OneWay 绑定；执行门禁不包装或代理业务命令。调用方直接执行 ICommand 不属于控件输入，业务命令仍应管理自己的并发。

```xml
<Button Content="保存" Command="{Binding SaveCommand}"
        one:ButtonLoading.IsLoading="{Binding IsLoading}" />
<one:IconButton Icon="{Binding SaveIcon}" Content="保存"
                IconPlacement="Right" IsLoading="{Binding IsLoading}"
                Command="{Binding SaveCommand}" />
```

加载不会自动判断任务状态；VM 在执行前设置 IsLoading，在 finally 中清除。PreviewHost 使用 ReactiveCommand 异步示例与 ReactiveObject 属性通知。视图声明 x:DataType 即可使用编译绑定，无须反射或动态创建控件。

LoadingContainer 继承 ContentControl，支持 Content／ContentTemplate、Padding、CornerRadius 与内容对齐。`IsLoading` 默认 false，`LoadingText` 默认“正在加载…”，可以绑定或设为 null。`OverlayOpacity` 默认 0.65，范围为 0～1（0 完全透明、1 完全不透明），只作用于遮罩背景，不降低加载图标和文字透明度；`OverlayBrush` 默认使用 One.Surface，可覆盖颜色。即使透明度为 0，覆盖层仍阻止内容区鼠标和键盘操作；内容实例、数据上下文与输入值保留。覆盖层不改变容器测量或虚拟化约束；大型列表仍需有界视口与虚拟化面板。

```xml
<one:LoadingContainer IsLoading="{Binding IsLoading}" LoadingText="{Binding ProgressText}"
                      OverlayOpacity="0.65" LoadingIcon="{Binding ProgressIcon}" Height="320">
  <Grid><!-- 正常内容，包括编译绑定与命令 --></Grid>
</one:LoadingContainer>
```

`LoadingIcon` 可以传 Geometry、PathIcon／其他 Control，或由 `LoadingIconTemplate` 渲染的数据。默认 Geometry 模板使用 PathIcon；消费项目可提供带 x:DataType 的 Material 图标模板。LoadingIcon 和 LoadingIconTemplate 都为空时恢复内置旋转指示器；自定义图标不自动旋转，其模板可自行提供动效。`LoadingIconSize` 控制图标大小，默认跟随三档密度。`LoadingText` 为空／空白时隐藏文字和间隔。控件实例不能共享视觉父级，共用图标请使用 Geometry 或模板。

`ControlSizing.Size` 支持 Small／Normal／Large，按钮指示图标为 14／16／20px，容器为 22／28／34px。使用 One.* 语义色，跟随 Light／Dark。`Motion.Enabled=False` 停止旋转、保留静态图标和文字；在加载中重新设置 true 恢复旋转，隐藏／卸载时取消动画。LoadingIndicator 也可独立使用，IsActive 控制运行状态，Foreground 控制颜色。

PreviewHost 的 Button、IconButton 页面提供加载与动效交互，LoadingContainer 有独立扩展控件页。验证与定向截图入口见 [PreviewHost](../../../design/PreviewHost/Program.cs)，原生消费示例见 [NativeAotSmoke](../../../design/PreviewHost/Consumers/NativeAotSmoke/Program.cs)。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 在各控件页直接标明示例的 Classes、尺寸及状态；公共属性、API／事件和模型字段使用 Description 组件说明，包含属性类型、默认值、绑定模式、校验条件及 API 签名。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### ButtonLoading

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.ButtonLoading.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L13) | `bool` | 可写; OneWay; 附加属性 | `false` | 显示忙碌指示；按钮保留尺寸并阻止重复操作，容器显示加载层。任务完成后由宿主复位。 |

### LoadingContainer

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.LoadingContainer.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`HasCustomLoadingIcon`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L31) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasLoadingText`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L33) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L24) | `bool` | 可写; OneWay | `false` | 显示忙碌指示；按钮保留尺寸并阻止重复操作，容器显示加载层。任务完成后由宿主复位。 |
| [`LoadingIcon`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L28) | `object?` | 可写; OneWay | `null` | 加载状态图标；为空时沿用默认指示器。 |
| [`LoadingIconSize`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L30) | `double` | 可写; OneWay | `28` | 加载图标的 DIP 尺寸；不控制覆盖层的面积。 校验：`value => double.IsFinite(value) && value > 0` |
| [`LoadingIconTemplate`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L29) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`LoadingText`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L25) | `string?` | 可写; OneWay | `"正在加载…"` | 加载中的说明文本，可本地化；不代表任务完成结果。 |
| [`OverlayBrush`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L27) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 加载覆盖层画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`OverlayOpacity`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L26) | `double` | 可写; OneWay | `.65` | 覆盖层不透明度；范围及有限数约束见校验列。 校验：`value => double.IsFinite(value) && value is >= 0 and <= 1` |
| [`ShowDefaultLoadingIndicator`](../../../TheOne.Avalonia/Controls/LoadingContainer.cs#L32) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### LoadingIndicator

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.LoadingIndicator.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Foreground`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L19) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 前景画刷；自定义时保持正文、状态与禁用对比度。 |
| [`IsActive`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L20) | `bool` | 可写; OneWay | `true` | 指示器是否运行动画；关闭时保留控件布局，全局动效开关仍生效。 |
| [`IsSpinning`](../../../TheOne.Avalonia/Controls/LoadingIndicator.cs#L21) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

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
| LoadingContainer | `one-large` | 模板自动类；不手动绑定 | ^.one-large => LoadingIconSize=34 |
| LoadingContainer | `one-small` | 模板自动类；不手动绑定 | ^.one-small => LoadingIconSize=22 |
| LoadingIndicator | `one-large` | 模板自动类；不手动绑定 | ^.one-large => Width=20; Height=20 |
| LoadingIndicator | `one-small` | 模板自动类；不手动绑定 | ^.one-small => Width=14; Height=14 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.ButtonLoading`](../../api/zh-cn/TheOne.Avalonia.Controls.ButtonLoading.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.LoadingContainer`](../../api/zh-cn/TheOne.Avalonia.Controls.LoadingContainer.md) | Class | 10 | 1 | 0 |
| [`TheOne.Avalonia.Controls.LoadingIndicator`](../../api/zh-cn/TheOne.Avalonia.Controls.LoadingIndicator.md) | Class | 3 | 2 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
