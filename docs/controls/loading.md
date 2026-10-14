# 加载状态与 LoadingContainer

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

PreviewHost 的 Button、IconButton 页面提供加载与动效交互，LoadingContainer 有独立扩展控件页。验证入口、定向截图与原生平台边界见[测试策略](../workflow/testing.md)与 [NativeAOT](../native-aot.md)。
