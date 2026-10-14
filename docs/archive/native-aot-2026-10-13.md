# 历史验证记录（2026-10-13）

仅供追溯；以下是各轮当时的结果与限制，不代表当前工作区已通过验证。当前规则见 [测试策略](../workflow/testing.md)。

缩放、平移与堆叠扩展验证：构建 0 警告 / 0 错误；图表专用 1,368 项、工作区全控件 9,535 项检查通过。相同 Chart 源码的独立构建目录在 macOS ARM64 发布并实际运行 NativeAOT，动态代码不可用，5,897 项检查通过；仍仅有已有 13 条官方 DataGrid 警告。54 对主题对比度检查通过，仅导出当前堆叠与视口场景的 30 张 PNG；其他 RID 尚未实机验证。

百分比堆叠、双轴与采样扩展验证（2026-10-13）：解决方案构建 0 警告 / 0 错误，`PreviewHost --verify-charts` 2,085 项通过。相同图表及消费源码的独立构建目录在 macOS ARM64 实际运行 `verify-native-aot.sh --verify-charts`，动态代码不可用，3,018 项图表与编译消费检查通过；保留已有 13 条上游 DataGrid 警告。54 对语义色对比通过，仅导出新增五个场景的 30 张主题／尺寸 PNG。工作区全控件回归和默认原生回归均在 InfoCard 的收起动画中间帧断言处停止；单独运行 `PreviewHost --verify-infocard` 也复现该断言，因此本次不宣称全控件通过。其他 RID 尚未实机验证。

十字准线、联动提示与交互图例验证（2026-10-13）：完整解决方案构建 0 警告 / 0 错误，图表专项 2,415 项、工作区全控件 10,793 项通过，本轮 InfoCard 动画检查通过。相同 17 个图表及消费源码文件的独立构建目录在 macOS ARM64 实际运行 NativeAOT：图表及编译消费专项 3,384 项、默认完整消费检查 7,230 项通过，动态代码不可用；保留已有 13 条上游 DataGrid 警告。54 对主题对比度与静态编译绑定契约通过，仅导出十字准线、真实原生共享提示和隐藏图例三个场景的 18 张主题／尺寸 PNG。其他 RID 尚未实机验证。

平滑、更新动画、PNG 导出及图形悬停验证（2026-10-13）：最终解决方案构建 0 警告 / 0 错误，图表专项 2,659 项通过，包含三类径向图真实中间帧、连续更新、十万点有界动画、稳定命中与透明／DPI／极小尺寸导出。工作区全控件回归 10,995 项通过；后续补充的径向动画和导出边界检查已通过最终专项及完整原生回归。相同 23 个 Chart 与消费源码文件的独立构建目录在 macOS ARM64 实际发布并运行 NativeAOT：图表及编译消费专项 3,654 项、默认完整消费检查 7,500 项通过，动态代码不可用；仅保留原有 13 条官方 DataGrid 告警。静态契约与 54 对语义色对比检查通过。仅导出当前六个场景的 36 张 Light／Dark × 三档尺寸图，柱形悬停图开启数值标签。日志、源码哈希和定向图片保存在 `.preview/chart-polish*`；其他 RID 及真实桌面文件选择器尚未实机验证。

独立图表控件、同行等大单位与标签避让验证（2026-10-13）：构建 0 警告 / 0 错误，Chart 专项 2,864 项通过。相同 42 个图表与消费源码文件在 macOS ARM64 原生运行：图表及编译消费专项 3,948 项、默认完整消费 8,183 项通过，动态代码不可用，保留原有 13 条 DataGrid 告警。静态契约覆盖 60 字典 / 120 主题键，54 对对比度通过；仅导出 14 种图表及标签、指针场景的 96 张图。托管全控件回归在 NavMenu/compact/Light/Small 的 Right 打开子菜单断言处中断，单独 --verify-navmenu 也可复现，本次不宣称托管全控件通过。日志及图片保存在 `.preview/chart-split`；其他 RID 尚未实机验证。

## 2026-10-13 审查结果

- 静态检查覆盖 52 份字典、93 个主题键，类库、PreviewHost 和独立消费项目无反射绑定／关闭编译绑定的声明；常规构建 0 警告、0 错误。
- 独立消费项目原生运行 774 项检查通过，RuntimeFeature.IsDynamicCodeSupported=False；未为全部 ViewModel 保留反射元数据，仅按 DataGrid 的要求保留行类型公共属性。
- PreviewHost 的原生完整交互回归 4,531 项通过；生成 337 张原生控件截图及 12 张设计变体截图。抽查通知、DataGrid、紧凑目录的静态帧，与托管渲染逐像素一致。
- Expander 快速反转检查改为观察 Animation 优先级的第一帧实际进度，并核对后续布局方向，避免把一次真实渲染调度的可变耗时误判为跳变；托管和原生的 Light／Dark × 四方向检查通过，控件动画实现未因此修改。
- 发布的 13 条警告均来自官方 DataGrid 12.1.2，保留原始诊断；已验证的显式编译列、行类型属性保留、编辑、路径排序、分组与回收配置正常。

## OneWindow 扩展验证

新增 OneWindow、ThemeButton、OneDialogPresenter 后，静态清单为 55 份字典、97 个主题键。独立消费程序在 macOS ARM64 NativeAOT 下通过 967 项检查，包括窗口默认／显式所有者、标题栏编译绑定、后台调用、主题图标、模态焦点、取消、表单解除父级后复用，以及千条对话框请求仅创建一个活动呈现器。PreviewHost 主窗体已继承 OneWindow，其原生 `--verify-onewindow` 检查通过；全量托管交互回归 4,701 项通过，生成 351 张 Skia 截图。原有 DataGrid 的 13 条依赖警告保持可见，没有新增类库裁剪／AOT 警告。

窗口接入与静态消息 API 见[窗口与消息](../controls/windows.md)。真实平台的拖动、缩放、贴靠及 DPI 行为仍需对应桌面验证。

## 泛型 Dialog 与 VM 关闭事件（2026-10-13）

OneDialog.ShowAsync<TView,TViewModel>(vm, DialogOptions) 使用泛型无参构造创建 UserControl，赋值 DataContext，并在实际显示时才构建 View。可选 DialogContext 通过 CloseRequested 事件返回 OneDialogResult；后台事件回到 UI 线程处理，完成、取消、窗口关闭时解绑。没有引入 ViewLocator、运行时 Type 查找、反射绑定或额外程序集保留。

独立 NativeAotSmoke 链接 PreviewHost 的 DialogEditorView.axaml 与 VM，实际消费带 x:DataType 的双向编译绑定、ICommand 和关闭事件；不能只用 C# 构建的假表单替代 AXAML 消费验证。千条泛型请求只创建一个可见 View，宿主关闭后所有 Task 完成，原 VM 可在新窗口复用。

本次验证：解决方案 Debug 构建 0 警告 / 0 错误；PreviewHost 全量交互 4,744 项通过，定向窗口与标签检查 309 项通过；实际 Skia 输出 363 张 PNG。osx-arm64 NativeAOT 原生程序报告 RuntimeFeature.IsDynamicCodeSupported=False，1,010 项编译绑定、主题与交互检查通过。发布保留原有 13 条官方 DataGrid 裁剪 / AOT 警告，无新增控件库或泛型对话框警告。Windows / Linux / 其他 RID 尚未实机验证。

## DatePicker / TimePicker 弹层修复与定向渲染（2026-10-13）

内部 Fluent 选项／按钮主题替换为四个 Still 专用 ControlTheme，保留原生 DateTimePickerPanel 的选择、循环、回收和事件。PickerSizing 的附加属性通过编译绑定同步选中条、条目高度与弹层上限，不使用反射绑定。新增资源字典及主题键已纳入 ThemeManifest；当前为 56 个控件字典、101 个主题键。

构建 0 警告 / 0 错误；日期时间定向交互 554 项通过，全量控件回归 5,298 项通过，46 对语义色对比度检查通过。实际 osx-arm64 NativeAOT 可执行程序报告动态代码不可用，1,588 项检查通过。新增检查覆盖两种主题、三档尺寸、选中文字与条目居中、12 / 24 小时制、秒开关、滚轮、键盘、确认／取消、按下及长按重复，并验证千年范围内实际创建的行数和远距离选择。

截图仅通过显式 `--render-controls .preview/pickers DatePicker TimePicker` 生成当前两个控件的 18 张弹层图，直接渲染 Presenter，避免宿主留白；NativeAOT 验证脚本仍不生成图片。省略控件名或使用未知名字会报错，不回退到全量；历史全量导出需显式 `--all`。发布仍有 13 条原有官方 DataGrid 警告，没有新增控件库裁剪／AOT 警告；其他 RID 尚未实机验证。

## 日期时间小数滚动、文字遮挡及重开修复（2026-10-13）

保留原生 DateTimePickerPanel。PickerScroll 使用直接事件、弱表及 UI DispatcherTimer，停止滚轮 140 ms 后吸附最近的整数行；真正的滚动手势进行中不吸附，结束时先限制非循环边界再对齐。Gesture 事件只冒泡，因此用 ScrollContentPresenter 的类处理器先于原生完成处理执行，不依赖隧道处理器。鼠标确定／Enter 先对齐再提交。

箭头占独立 Grid 行，保留原生 Panel 模板合约，正常高度下滚动视口最多五行且停稳后无半行。选项保留原生选择／回收，用两个文字 Presenter 与按条带交集计算的矩形裁剪，保证滚动中条带内外的文字颜色均可见。新绑定为编译附加属性绑定，没有反射。缓存的原生 Presenter 在同值 Date / Time 下不会初始化，因此重新附加后按日期依赖顺序恢复已提交的年月日／时间；关闭停止计时及解绑，弱表避免宿主被长期保留。

构建 0 警告 / 0 错误；新日期时间交互 3,139 项及全量 7,883 项检查通过。覆盖连续小幅真实滚轮事件、正反方向偏移、手势进行／结束、短列表边界、立即确认、取消、关闭中断与重新打开、回收后的文字裁剪、五行视口以及千年范围选择。osx-arm64 NativeAOT 原生程序动态代码不可用，4,173 项通过；仍仅有原有 13 条官方 DataGrid 警告。46 对语义色对比检查通过。仅导出当前日期／时间控件的 30 张 Skia 图，normal 档额外包含 fractional / snapped；未全量导图。实际触控板硬件手感及其他 RID 尚未实机验证。

## InfoCard 扩展控件（2026-10-13）

InfoCard 继承 HeaderedContentControl，复用 Header / HeaderTemplate、Content / ContentTemplate 合约，主题只使用 TemplateBinding；HeaderSpacing / HeaderFontSize 为带数值验证的 StyledProperty。三档尺寸、语义色、空标题／纯标题间距、本地样式覆盖及原生子控件交互均验证。独立 NativeAotSmoke 同时链接 InfoCardDemoView.axaml、code-behind 和共享检查，实际实例化并验证强类型模板、双向输入和命令，避免链接但未使用的示例被裁掉。

Debug 与 Release 解决方案构建 0 警告 / 0 错误。InfoCard 定向 85 项及当前全量回归 8,727 项通过，包含千条列表的真实视口、远距离键盘选择与回收。osx-arm64 原生可执行程序动态代码不可用，当前 5,050 项检查通过；当前资源为 58 个控件字典、103 个主题键（包含并行任务加入的 Chart）。本次仅导出 InfoCard 六张图片；54 对当前语义色对比检查通过。其他 RID 未实机验证；已有 DataGrid 上游限制继续按前述章节处理。

构建验证遇到沙盒对 Avalonia 统计日志与构建服务器通信的限制，采用本次命令行的 UsedAvaloniaProducts 空值和 --disable-build-servers / UseSharedCompilation=false，不更改项目调试配置或全局开发环境。Headless 检查运行 Release 版本，避开 Debug 开发工具通信；NativeAOT 在授权的本地执行中完成。verify-native-aot.sh 新增可选 THEONE_AOT_OUTPUT，默认行为保持原样，本次输出独立的 .preview/infocard-native，减少与并行 Chart 验证的输出冲突。

### InfoCard 标题扩展与 Footer（2026-10-13）

新增 HeaderIcon / HeaderActions / Footer 及对应 IDataTemplate StyledProperty；HeaderIconSize、HeaderContentSpacing、FooterSpacing 使用有限数值验证并按三档尺寸配置。主题全部使用 TemplateBinding，新插槽注册原生 ContentPresenter Host 并维护 LogicalChildren。实际检查覆盖替换 ControlTemplate 后控件身份与命令仍可用、空区域折叠、只有图标／操作／页脚、无正文时单次间隔、窄宽换行，以及六种主题尺寸的布局和禁用状态。

PreviewHost 的三个新增模板声明明确 x:DataType，显隐时同时清空内容与模板。共享原生消费者增加 `--verify-infocard` 定向入口，运行实际编译 AXAML 的状态、命令与图标更新；既有千条列表的远距离选择、键盘及容器回收检查保留。为复用共享消费项目，同时修正 LoginDemoView 链接到 NativeAotSmoke 时无法生成 InitializeComponent 的加载方式，使用编译器可重写的 AvaloniaXamlLoader.Load。

Debug / Release 解决方案构建 0 警告 / 0 错误，InfoCard 198 项定向检查通过；osx-arm64 原生可执行程序以 `--require-aot --verify-infocard` 通过同一组 198 项检查，RuntimeFeature.IsDynamicCodeSupported=False。标准 verify-native-aot.sh 发布并执行完整原生消费项目，5,877 项检查通过；当前 59 个字典及 104 个主题键的编译绑定／AOT 合约检查通过，54 对语义色对比检查通过。原生发布仍保留已记录的 13 条官方 DataGrid 包裁剪／AOT 警告，不通过 NoWarn 或整程序集保留隐藏它们。其他 RID 未实机验证。本次仅导出 InfoCard 的六张图片。

最新 PreviewHost 整体回归 9,533 项通过（`.preview/infocard-slots-controls-final.log`）。期间遇到一次既有 ContentTransition 中途关闭动效的时间敏感断言失败；发布结束后独立复跑通过，保留该项原检查。InfoCard 定向结果独立保存在 `.preview/infocard-slots-verify.log` 和 `.preview/infocard-slots-native-focused.log`，原生全量结果在 `.preview/infocard-slots-native.log`。

### InfoCard 可折叠与动效（2026-10-13）

新增 IsCollapsible（默认 false）、IsExpanded（默认 true、TwoWay）、ExpandButtonSize 及只读 IsContentOpen。标题操作与原生 ToggleButton 分离，正文与 Footer 共用现有 RevealHost；外框保留完整圆角与描边。Arrow Transitions 及视口都遵循 Motion.Enabled，关闭动效立即落在目标状态，重启不重播。焦点从关闭内容移回标题按钮，关闭区域跳过 Tab；快速反转继续当前视口，不重建子控件。所有新绑定仍为 TemplateBinding 或编译绑定。

RevealHost 新增 opt-in ConstrainContent，默认保留 Expander / TreeView 既有测量方式。InfoCard 启用有限父约束，避免卡片 Height / MaxHeight + Grid 星号行中的 ListBox 失去虚拟化视口。实际千条检查覆盖容器数量、远端回收和选择、折叠恢复、动态 MaxHeight、动画中移除与重新挂载。预览支持双向草稿、独立操作、快速切换计时器与动画中关闭动效；离开预览停止计时器，初始化选择事件在名称作用域准备完成后才读取指标。

Debug / Release 解决方案构建 0 警告 / 0 错误。InfoCard 定向 409 项通过；实际 osx-arm64 原生程序 `--require-aot --verify-infocard` 通过相同 409 项，动态代码不可用；标准 verify-native-aot.sh 完整原生消费项目 6,864 项通过。59 个字典、104 个主题键的静态契约以及 54 对语义色对比检查通过，仅导出当前 InfoCard 六张图。发布仍仅有原有 13 条官方 DataGrid 告警；其他 RID 尚未实机验证。结果在 `.preview/infocard-collapse-*`，图片在 `.preview/infocard-collapse`。

完整 PreviewHost 回归 10,463 项通过（`.preview/infocard-collapse-controls.log`），包含 Expander / TreeView 既有四方向、快速反转、回收及边框连接检查，确认共享 RevealHost 的默认行为保留。动效专项通过 DispatcherFrame 驱动真实 UI 时钟，先验证默认 200 ms，布局 probe 使用 400 ms 为真实键盘导航留出帧间时间；实际编译 AXAML 交互仍使用主题默认时长。

## LoginWindow 与桌面生命周期

LoginWindow 直接继承 Window，保留 Content / ContentTemplate 和原生视觉层，模板使用 TemplateBinding。开发者提供认证内容，成功后通过 CompleteLogin / CompleteLoginAsync 的显式工厂创建 OneWindow；不使用反射、自动 ViewLocator 或程序集保留。工厂在 UI 线程上运行，登录失败或 Closing 取消时不创建主窗体。

NativeAotSmoke 共享编译绑定登录表单和 LoginWindowChecks，支持 `--require-aot --verify-loginwindow`。实际 osx-arm64 原生程序通过 86 项检查，包括六种主题／尺寸、编译双向输入、演示校验、后台完成、迟到结果、取消关闭、重入、失败回滚、原生子窗清理、拒绝接管已打开或隐藏的旧窗口，以及登录关闭后仍有效的通知、Toast、对话框和 Popup 宿主。

`--require-aot --verify-login-lifetime <ShutdownMode>` 与 `--require-aot --verify-login-lifetime-close <ShutdownMode>` 均使用实际 ClassicDesktopStyleApplicationLifetime 主循环。在独立进程中分别验证 OnMainWindowClose、OnLastWindowClose、OnExplicitShutdown，共六个用例通过；后一入口还验证登录 Closed 回调立即关闭新主窗体时的退出行为。应用 Generic Host 仍由消费方在应用退出路径释放。

解决方案构建 0 警告 / 0 错误，PreviewHost 完整控件回归 9,535 项通过。标准 `bash scripts/verify-native-aot.sh` 发布并执行原生消费项目，5,897 项检查通过，RuntimeFeature.IsDynamicCodeSupported=False；当前静态合约覆盖 59 个字典、104 个主题键。发布保留原有 13 条官方 DataGrid 裁剪／AOT 告警，无新增类库告警。仅导出 LoginWindow 六张 Skia 图片；其他 RID 与真实桌面装饰行为尚未实机验证。日志与图片位于 `.preview/loginwindow`。

## NavMenu 多级导航

NavMenu 继承官方 TreeView，所有层级使用可回收的 VirtualizingStackPanel；ChildrenBinding、ExpandedBinding、IsGroupBinding、IsItemEnabledBinding 通过声明节点类型的 CompiledBinding 指定业务字段，内部模板的尺寸及内容绑定使用 CompiledBinding.Create。IconTemplate / ContentTemplate 使用节点类型的 DataTemplate；不按字符串反射解析子级，不动态创建业务模型。公开用法见 [NavMenu](../controls/nav-menu.md)。

独立 NativeAotSmoke 链接 NavMenuDemoView 编译 XAML 和同一专项，验证根 / 二级 / 三级各 1,000 项、实际容器数量、远距离选择、回收后的展开状态、键盘导航、分组、禁用和实时模板 / 尺寸 / 主题切换。原生程序可通过 `--require-aot --verify-navmenu` 单独执行。

NavMenu 本次验证：托管与实际 osx-arm64 NativeAOT 专项均通过 203 项；标准原生消费回归通过 7,715 项，工作区控件回归通过 11,244 项。仅导出 NavMenu 的六张 Light / Dark × 三档尺寸图片。之后并行图表任务开始 Chart / ChartBase 类型迁移，全量解决方案构建暂受并行图表代码迁移影响；使用 `.preview/navmenu-isolated` 独立消费入口对最新类库及主题再次发布并运行原生 NavMenu 专项，通过相同 203 项，不将图表迁移中的代码视作已验证。既有官方 DataGrid / TypeHelper 裁剪告警保留，无新增 NavMenu 告警。其他 RID 未实机验证。源码、构建日志与图片位于 `.preview/navmenu*`。

### NavMenu 折叠模式

IsCollapsed 和弹层尺寸为 Avalonia 属性，弹层根集合复用带节点类型的 ChildrenBinding；图标、内容、密度和尺寸使用 CompiledBinding.Create，标题及菜单内容继续使用已编译 DataTemplate。折叠图标的默认模板不反射读取业务字段，弹层只按需创建一个有界 NavMenu；所有层级保留原生虚拟化面板。窗口内 Flyout 不需要额外宿主注册。

独立消费项目复用 NavMenuCollapsedPreview 和编译 DemoView，验证明暗主题／三档尺寸、千条根与各级子项、程序／键盘远距离选择、模型展开与选择、动态子集合、父级禁用、外部点击／Esc 关闭、容器回收、卸载清理，以及弹层打开时切换主题、尺寸和动效。无 ExpandedBinding 时，弹层与主导航共享同一份以模型身份为键的弱展开缓存，不通过反射或整程序集保留维持状态。

折叠模式完成验证（2026-10-13）：Debug / Release 解决方案构建 0 警告 / 0 错误；NavMenu 托管与实际 osx-arm64 NativeAOT 专项均通过 398 项，动态代码不可用。标准脚本发布并执行完整原生消费项目 8,192 项通过；工作区完整控件回归通过。静态契约覆盖 60 个字典／120 个主题键，54 对语义色对比通过。仅导出当前 NavMenu 展开／折叠的 12 张定向 Skia 图，并检查明暗主题及三档尺寸。发布仍为原有 13 条官方 DataGrid／TypeHelper 告警，无新增 NavMenu 裁剪或 AOT 告警。其他 RID 未实机验证；日志与图片位于 `.preview/navmenu-collapse*`。


### NavMenu 悬停与级联弹层

折叠图标与各级弹层默认悬停 100ms 打开、离开 300ms 后关闭，延迟与 OpenOnHover 均为可设置 Style / 编译绑定的属性。各级 Flyout 复用带节点类型的 ChildrenBinding，按需创建有界的 NavMenu。根窗口负责一次外部点击关闭，悬停可穿越父级及兄弟弹层；弹层以窗口坐标随祖先布局更新，避免一次 SelectPath 打开多级时位置错位。进入弹层取消待关闭计时，切换分支替换对应层，Left / Esc 逐级返回，叶子调用仅执行一次根命令。源码不使用属性名反射、动态代码或整程序集保留。

编译 DemoView 提供 5,000 个模型，每个增长集合 1,000 项；实际弹层行容器为 8–10 个，千项末尾选择与操作仍可用。共享 NavMenuCascadePreview 覆盖三档尺寸 × 明暗主题、悬停不抢焦点、跨层与间隙移动、关闭取消、兄弟分支和图标切换、逐级键盘操作、显式箭头按钮、同步多级 API 定位、父级禁用、回收与卸载后的计时器清理，以及打开时切换主题、尺寸和动效。

2026-10-13：Debug / Release 构建均为 0 警告 / 0 错误。NavMenu 托管与实际原生专项均通过 620 项，完整控件托管回归通过 11,869 项。标准脚本发布并运行实际 osx-arm64 NativeAOT 消费程序，8,420 项检查通过，RuntimeFeature.IsDynamicCodeSupported=False。静态契约覆盖 60 个字典／121 个主题键，54 对语义色对比通过。仅导出当前 NavMenu 展开／级联折叠的 12 张 Skia 图，检查两种主题与三种尺寸。既有官方 DataGrid / TypeHelper 的 13 条裁剪／AOT 告警继续可见，无新增 NavMenu 告警；其他 RID 及实机 GPU 尚未验证。日志与图片位于 `.preview/navmenu-cascade*`。

### NavMenu 边缘适配与 PreviewHost 独立页面（2026-10-13）

新增窗口客户区约束、自动翻转、根弹层缩窄与窄窗口单级返回模式。只读 IsCompactSubmenu / SubmenuOpensLeft 使用 Avalonia DirectProperty；弹层变位时保留悬停关闭缓冲，返回恢复原父层选择与滚动状态。所有层级仍由带明确节点类型的编译 ChildrenBinding 创建有限视口，不反射读取模型字段。边缘专项覆盖 Light / Dark × 三尺寸、260／420／900 宽度与左右放置、实时缩放、逐级返回、每级 1,000 项与远端调用；实际行容器为 5–14 个。

PreviewHost 按六类拆分 Views / ViewModels / Models / Infrastructure / Verification，提供 63 个独立页面，包括 14 个固定类型图表及独立 Toast / Dialog / OneNotification 页面。页面工厂显式引用类型，不使用反射；输入、选择和配置使用 TwoWay 编译绑定。TabControl 使用可修改模型集合，保持排序、关闭和新增；Flyout 内容显式绑定页面 DataContext，避免关闭后双向选择被弹层卸载清空。旧组合 Gallery 和对比页面仅为回归夹具。结构规范见 [PreviewHost](../preview-host.md)。

最终 Debug / Release 解决方案构建均为 0 警告 / 0 错误；托管全量 12,677 项通过，独立页面 172 项、NavMenu 1,256 项、Flyout 89 项专项通过。静态契约覆盖 60 个字典／121 个主题键，54 对语义色对比通过。仅导出 NavMenu 的 24 张定向图及新主导航两张图，未导出其他控件。

`bash scripts/verify-native-aot.sh --full` 发布并运行实际 osx-arm64 原生消费项目，9,056 项通过，RuntimeFeature.IsDynamicCodeSupported=False。随后实际原生 PreviewHost 全量在既有 Expander 首帧计时断言处失败；该断言在 Flush 已推进真实时钟后仍假定布局接近初始值。改为推进时钟前检查现有视口及输入状态，保留中间帧、连续收缩、测量复用、快速反转和结束几何检查。重新发布 PreviewHost 后，原生 Expander 专项 108 项与实际原生 PreviewHost 全量 12,677 项均通过；没有修改生产动画代码。

原生发布继续保留原有 13 条官方 DataGrid / TypeHelper 裁剪与 AOT 告警，无新增类库告警；其他 RID 与实机 GPU 未验证。最终日志为 `.preview/preview-restructure-controls-final.log`、`.preview/preview-restructure-native-complete.log`、`.preview/preview-restructure-native-host-publish-final.log`、`.preview/preview-restructure-native-expander-final.log` 和 `.preview/preview-restructure-native-host-final.log`；图片位于 `.preview/navmenu-edge-final`。

### NavMenu 多级搜索过滤（2026-10-13）

新增 SearchText / SearchTextBinding 与只读 IsSearching / SearchMatchCount。SearchTextBinding、ChildrenBinding、ExpandedBinding 均使用带节点类型的编译绑定读取；一次扫描只创建一个非 Control 的 StyledElement 绑定探针，不预创建 UI 行，也不反射读取模型字段。ItemsSource 的有效值为匹配项目与祖先路径投影，原始 ViewModel 集合与模型身份保留。普通树自动展开路径但不写回业务展开属性，折叠级联共享过滤结果。

搜索会话保存首次进入时的选择、展开与视口，修改关键词保留快照；清空恢复原值。未绑定展开属性时同时恢复弱缓存的完整子集合与展开值，避免后续未创建项操作仍读取过滤集合。动态节点／集合通知触发刷新，卸载取消订阅，重新挂载刷新结果；循环引用在扫描时截断。公开用法见 [NavMenu 搜索](../controls/nav-menu-search.md#多级搜索过滤)。

搜索专项 132 项通过，覆盖 Light / Dark × 三档尺寸、每级 1,000 项、完整父路径、无结果、多词／大小写、远处项选择、实际容器回收、键盘导航、动态改名与子集合更新、折叠级联、未绑定展开时的缓存恢复和完整未创建子集合遍历。实际 osx-arm64 NativeAOT 消费程序通过 9,188 项检查，RuntimeFeature.IsDynamicCodeSupported=False；重新发布后的实际原生 PreviewHost 全量通过 12,797 项。托管全量通过 12,789 项，随后新增的八个默认 RevealHost 200ms 时长断言也在原生全量中通过。

Debug / Release 构建均为 0 警告 / 0 错误，静态契约覆盖 60 字典／121 主题键，54 对语义色对比通过。只导出当前 NavMenu 的 30 张 Skia 图（含六张搜索），未导出其他控件。共享工作区同时调整其他预览页；验证期间修正了命名空间／占位符编译问题，以及 Carousel 新增示例后的验证窗口高度和目录按钮滚动定位。ContentTransition 预览缓存过渡实例以保持 MVVM 配置身份；保留默认 200ms 断言，在测试中延长取消／多帧采样 probe 时长，生产主题动效时长不变。

最终日志：`.preview/nav-search-focused.log`、`.preview/nav-search-controls-all-final.log`、`.preview/nav-search-native-final.log`、`.preview/nav-search-native-host-publish.log`、`.preview/nav-search-native-host-finish.log`。原生发布仍仅有原有 13 条官方 DataGrid / TypeHelper 告警，无新增 NavMenu 告警；其他 RID 与实机 GPU 未验证。图片位于 `.preview/navmenu-search`。

### NavMenu 搜索高亮与输入框键盘导航（2026-10-13）

新增附加属性 NavMenu.SearchOwner，使用编译元素绑定关联 TextBox 和 NavMenu。Text / SearchText 保持 ViewModel 双向绑定；无修饰的 Up / Down 按过滤投影深度优先顺序选择可调用的直接匹配项，跳过分组与禁用路径，Enter 单次调用，Esc 清空并恢复首次搜索快照。目标路径按需实现原生容器；折叠模式使用 Transient 级联，不把焦点移入菜单。关闭已有 Standard 弹层时同样恢复搜索框焦点。

新增 NavMenuSearchText，SourceText 使用编译绑定，HighlightQuery 沿逻辑树继承，命中词合并成加粗／下划线片段，保留原生字号与语义前景色。未命中时使用普通 TextBlock.Text，命中时才生成 Inlines；不重写消费方的普通 TextBlock 或复杂模板，不反射读取文字。默认 NavMenu 内容及弹层标题支持高亮；自定义模板可显式使用该文字控件。

新增交互验证 114 项，覆盖三尺寸 × 明暗主题、真实输入、跳过禁用项、连续方向键至第 1,000 项、实际容器回收与数量、输入焦点及位置、无结果、单次调用、Esc 状态恢复、折叠级联和显式弹层关闭、默认／自定义编译模板、大小写与重叠命中。与原有搜索验证合计 246 项。托管全控件回归 13,287 项通过；实际 osx-arm64 NativeAOT 消费程序 9,302 项通过，RuntimeFeature.IsDynamicCodeSupported=False。

解决方案构建为 0 警告／0 错误；静态合约覆盖 60 字典／121 主题键，54 组主题对比度通过。只导出 NavMenu 的 30 张 Skia 图片，位于 `.preview/navmenu-highlight`。共享图表预览的原有定位验证按现有 API 修正为先通过 VM 选中、再定位，未修改图表实现。

本次日志为 `.preview/nav-highlight-build.log`、`.preview/nav-highlight-controls-complete.log`、`.preview/nav-highlight-native-complete.log`；原生发布继续保留每个项目 13 条官方 DataGrid / TypeHelper 告警，无新增 NavMenu 告警；其他 RID 与实机 GPU 未验证。

同一 `--full` 流程发布并运行实际原生 PreviewHost，最终全控件 13,287 项通过，包含新增的 114 项搜索交互检查。原生消费程序及 PreviewHost 均执行成功，没有用普通 dotnet run 替代原生验证。

### NavMenu 展开与收起动效（2026-10-13）

新增可样式化／编译绑定的 TransitionDuration，默认取 One.Motion.Expand（200ms），收起为 70% 时长。NavMenuItem 的 RevealHost 通过明确类型的编译绑定获取时长与有限视口行程；绑定随模板、挂载与卸载更新／释放。RevealHost 的 TransitionExtent 默认无限，普通 TreeView 的既有路径保持原方式；有限视口使用 UI 帧定时器和单调时钟推进立方缓出，完成后恢复完整列表范围，不依赖布局末段的渲染时钟收尾。保留完整子布局与原生虚拟化面板，反转从当前进度继续。

侧栏在一次布局更新后仅对可见根项目做图标平移和展开文字渐显，一个进度动画驱动生成的视觉元素；不逐帧调整页面 Width／Height，不对回收项目重放入场。取消、动效关闭、容器回收、尺寸变化和卸载恢复变换及透明度，模型展开与选择不变。

托管动效专项及实际 osx-arm64 NativeAOT 动效专项各通过 96 项，覆盖明暗主题 × 三尺寸、千条实际容器回收、真实中间帧、反转、完整范围恢复、远距离导航、动效中途关闭与恢复、零时长和卸载清理。原生执行 RuntimeFeature.IsDynamicCodeSupported=False。测试使用实际 Dispatcher 帧循环并推进 Skia 帧，不通过普通 dotnet run 替代原生验证。可使用 `bash scripts/verify-native-aot.sh --verify-navmenu-motion` 复现。

构建 0 警告／0 错误，静态合约仍覆盖 60 字典／121 主题键；原生发布保留原有 13 条官方 DataGrid / TypeHelper 告警，无新增类库告警。只导出当前 NavMenu 48 张 Skia 图，包括 18 张动效中间帧，位于 `.preview/navmenu-motion`。本次日志为 `.preview/nav-motion-build.log`、`.preview/nav-motion-focused.log`、`.preview/nav-motion-controls.log` 和 `.preview/nav-motion-native.log`；其他 RID 与实机 GPU 帧率未验证。

最终托管全控件回归通过 13,422 项检查，包含上述 96 项新增动效检查；未发现其他控件的模板或交互回归。

### NavMenu 顶部自动完成搜索（2026-10-13）

新增 ShowFilter（默认 False）、FilterPlaceholder，以及模板内的 NavMenuFilterBox。搜索输入沿用官方 AutoCompleteBox 模板、原生选择适配器与有界虚拟化建议列表；模板使用 TemplateBinding，建议文字与路径使用明确类型的编译绑定。建议索引通过现有 ChildrenBinding / SearchTextBinding 读取原始各级模型，不使用反射读取属性，也不预创建菜单控件。

方向键预选保留查询，Enter／鼠标确认先退出搜索投影，再定位原始模型与父级路径；折叠模式定位级联弹层。建议容器不抢走输入焦点，动态模型与集合更新重建轻量索引，隐藏／卸载释放订阅。PreviewHost 主导航与独立页面启用 ShowFilter，双向绑定 SearchText / SelectedItem 的消费示例保持编译绑定。

托管搜索专项及实际 osx-arm64 NativeAOT 搜索专项各通过 90 项，覆盖明暗主题 × 三尺寸、千条深层建议的实际容器数量／回收、第 1,000 项键盘与鼠标定位、折叠级联、输入焦点、Esc 恢复、动态更新与根数据源替换。原生执行 RuntimeFeature.IsDynamicCodeSupported=False，可通过 `bash scripts/verify-native-aot.sh --verify-navmenu-filter` 复现。

静态合约覆盖 60 字典／121 主题键。原生发布保留原有 13 条官方 DataGrid / TypeHelper 告警，没有新增 NavMenu 裁剪／AOT 告警；仅生成当前 NavMenu 的 54 张 Skia 图片（其中 6 张内置搜索建议），位于 `.preview/navmenu-filter`，已检查 Light / Dark 图片。日志为 `.preview/nav-filter-focused.log`、`.preview/nav-filter-native.log` 和 `.preview/nav-filter-render.log`；其他 RID 与实机 GPU 未验证。

完整 NavMenu 托管与实际原生回归各通过 1,688 项，包含原有多级导航、折叠侧栏、悬停级联、边缘适配、搜索高亮／外置搜索键盘操作、动效和新增内置搜索。日志为 `.preview/nav-filter-navmenu.log` 与 `.preview/nav-filter-native-navmenu.log`，原生程序使用 `--require-aot --verify-navmenu` 执行。

最终解决方案构建为 0 警告／0 错误，托管全控件回归通过 13,715 项，包含 PreviewHost 主导航与独立页的编译绑定、主题／尺寸继承和窄窗口布局。日志为 `.preview/nav-filter-build.log` 与 `.preview/nav-filter-controls.log`。

### NavMenu 折叠搜索图标与弹层（2026-10-13）

折叠模式通过模板的搜索图标打开 Popup，复用原内联 NavMenuFilterBox：输入框在两个宿主之间移动，TemplateBinding 与明确类型的建议编译绑定保持原样。只读 IsFilterOpen 支持编译绑定观察，OpenFilter / CloseFilter 提供打开与取消操作。按钮的尺寸与图标大小取 NavMenu 的 ItemHeight / IconSize，弹层按当前主题、ControlSizing 和窗口可用空间更新。

确认结果关闭搜索输入／建议弹层，按原始模型路径定位级联菜单，保留目标菜单弹层并将焦点返回搜索图标。Esc／外部点击取消并恢复状态；关闭时不会因 ViewModel 的查询更新而弹出隐藏建议。折叠切换、隐藏／禁用和卸载处理弹层关闭，关闭／重套模板时释放事件与窗口监听。建议继续使用原生 AutoCompleteBox 选择适配器和虚拟化 ListBox，不预创建项目。

托管及实际 osx-arm64 NativeAOT 搜索专项各通过 174 项（比原专项增加 84 项），覆盖三种尺寸 × 明暗主题、图标鼠标／键盘激活、千条建议实际回收与末项确认、鼠标定位、取消恢复、焦点返回、打开期间主题／尺寸切换、窄窗口缩放及卸载。原生 RuntimeFeature.IsDynamicCodeSupported=False。日志为 `.preview/nav-compact-filter-focused.log` 与 `.preview/nav-compact-filter-native.log`。

只生成当前 NavMenu 的 66 张 Skia 图，其中新增 12 张折叠搜索图标／输入弹层截图，位于 `.preview/navmenu-compact-filter`；已检查明暗主题渲染。构建零警告／零错误，静态合约覆盖 60 字典／121 主题键。原生发布保留原有 13 条官方 DataGrid / TypeHelper 告警，没有新增 NavMenu 告警；其他 RID 与实机 GPU 未验证。

完整实际原生 NavMenu 回归通过 1,772 项，包含原有级联／边缘适配、外置搜索键盘、高亮和动效检查。原生程序使用 `--require-aot --verify-navmenu` 执行，日志为 `.preview/nav-compact-filter-native-navmenu.log`。

最终托管全控件回归通过 13,825 项，覆盖 PreviewHost 主导航、独立控件页面与窄窗口布局，日志为 `.preview/nav-compact-filter-controls.log`。解决方案构建日志为 `.preview/nav-compact-filter-build.log`。
