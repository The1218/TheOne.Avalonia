# Avalonia 与 Skia：本项目的开发依据

查阅日期：2026-10-12。使用了 MCP `avalonia-docs` 的专家规则、主题文档、自定义绘制文档和诊断工具配置说明，并阅读 Avalonia 技能中的 XAML、布局、样式、自定义控件、应用结构、图形、控件分类、设计系统、主题、组件、无障碍和真实渲染预览工作流。

## 核心判断

Avalonia 已经通过 Skia 渲染常规控件。使用 Skia 不意味着把所有 UI 重写成 `SKCanvas` 绘图。按钮、输入、选择、弹层等优先使用 Avalonia 属性系统和模板，保留布局、绑定、输入和可访问性支持；复杂可视化确有需要时再直接访问 Skia。

| 场景 | 首选方式 | 原因 |
| --- | --- | --- |
| 通用扩展控件 | TemplatedControl + ControlTheme | 行为与外观分离，可由宿主替换模板 |
| 图形装饰、基础仪表 | Control.Render + DrawingContext | 保持 Avalonia 渲染抽象，较易维护 |
| 特殊路径、复杂图表 | ICustomDrawOperation + ISkiaSharpApiLeaseFeature | 直接使用 SkiaSharp，需要管理渲染线程与资源生命周期 |
| 持续高频动画 | 评估 Composition API | 避免阻塞 UI 线程，按实际 API 与测量结果决定 |
| 静态设计图 | Headless + UseSkia + RenderTargetBitmap | 使用真实布局、字体、模板和 Skia 软件渲染 |

## 自绘规则

- `Control.Render` 记录绘制操作时捕获不可变数据。渲染操作不能直接读取或修改正在变化的 UI 控件状态。
- 使用 `AffectsRender` 注册影响外观的属性；变更时触发重绘，避免用无意义的循环强制刷新。
- 通过 feature lease 访问 `SKCanvas`，及时释放 lease；调用 Save / Restore，保护共享画布状态。
- 对自己创建的 SKPaint、SKPath、shader 等资源负责释放；不要释放框架拥有的 canvas / surface。
- 逻辑布局采用 DIP；不要在框架已经处理缩放时再次乘以屏幕 DPI。
- 高频绘制按需缓存几何、画笔和文字；减少整屏无效重绘和大面积阴影、模糊。
- 自绘控件需要另外考虑命中测试、键盘行为和 AutomationPeer。不能仅因图形可见就认为可访问性完成。
- GPU 专有控件不一定能被 RenderTargetBitmap 正确捕获。当前设计图只验证软件 Skia 路径。

旧版直接 Skia 圆环实验已随历史设计场景移除。当前图表采用 Avalonia 的 DrawingContext 绘制，参见 [ChartBase](../../TheOne.Avalonia/Controls/ChartBase.cs)；底层仍由 Skia 渲染。

## 用当前 API 校准技能示例

技能与文档是指导材料，实际 NuGet 和编译结果是最终依据。本次使用 12.1.4；修正了示例中的这些差异：

- `TextBox.PlaceholderText` 取代已经弃用的 `Watermark`。
- PNG 保存使用 `PngBitmapEncoderOptions`，避免旧 Save 重载。
- `Grid.ColumnDefinitions` / `RowDefinitions` 是 CLR 属性，不能直接作为 Style Setter。预览宿主在尺寸变化时调整列行定义，主题资源仍通过样式管理。
- 旧文档中 `Avalonia.Skia 11.2.*` 配合 `SkiaSharp 2.88.*` 的安装片段不适用于本次版本。使用 Avalonia.Skia 当前传递依赖。
- `ControlTheme` 放在资源字典内；浅色 / 深色切换由 Application / Window 的 RequestedThemeVariant 控制。
- 使用新版 `AvaloniaUI.DiagnosticsSupport`。机器已安装 AvaloniaUI.DeveloperTools 2.2.1，无需重复全局安装；预览宿主已在 Debug 配置入口和连接。

## 官方依据

- [Avalonia 12.1.4 NuGet](https://www.nuget.org/packages/Avalonia/12.1.4)
- [Avalonia.Skia NuGet](https://www.nuget.org/packages/Avalonia.Skia/12.1.4)
- [Themes](https://docs.avaloniaui.net/docs/styling/themes)
- [Control themes](https://docs.avaloniaui.net/docs/styling/control-themes)
- [Theme variants](https://docs.avaloniaui.net/docs/styling/theme-variants)
- [Custom rendering](https://docs.avaloniaui.net/docs/graphics-animation/custom-rendering)
- [Drawing graphics](https://docs.avaloniaui.net/docs/graphics-animation/drawing-graphics)
- [Headless platform](https://docs.avaloniaui.net/docs/testing/setting-up-the-headless-platform)

后续工作应按任务加载相关技能，继续用 MCP 查找具体 API；不因为已阅读一次就假定所有版本和控件规则永远不变。
