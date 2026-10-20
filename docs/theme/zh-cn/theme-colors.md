# 默认主题配色与控件外观

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

**简体中文** · [English](../en/theme-colors.md)

本页说明默认配色；新增的 9 套可选资源、配色下拉选择器及独立明暗开关见 [ThemeColorPicker / ThemeButton](theme-color-picker.md)。

默认颜色按 [Eremex DeltaDesign 的 Nova 资源](https://github.com/Eremex/controlthemes/blob/5f6cb3ad8b904cead6bd432f62596648cc9a1039/Source/Eremex.Avalonia.Themes.DeltaDesign/PaletteResources/Nova.axaml)逐项映射，并与 `Eremex.Avalonia.Themes.DeltaDesign 1.5.53-preview3` 的运行时资源交叉核对。参考 Demo 使用的主题入口见 [App.axaml](https://github.com/Eremex/controls-demo/blob/2edf87b8eea9f58f935b0383bbf724abfd43122a/DemoCenter/DemoCenter/App.axaml)。不再根据旧截图取近似色，也不将强调色人为压暗或增艳。

颜色直接写入类库 `Themes/Colors.axaml`；加载 OneTheme 即生效。公共 `One.*` 名称保留，按钮、输入、选择、列表、表格、导航、菜单、浮层、窗口与消息共用这些资源，无需宿主注入。保留 Avalonia 原生模板、行为与三档尺寸，不引入 Eremex 依赖或复制其模板实现。

| One 语义 / Nova 角色 | Light | Dark |
| --- | --- | --- |
| Canvas、Surface / Primary Enabled | `#FFFFFF` | `#0F0F0F` |
| Subtle、CardTabStrip / Secondary Enabled | `#EDEDED` | `#282929` |
| NeutralHover / Secondary Hover | `#D6D7D7` | `#3F4141` |
| NeutralPressed / Secondary Pressed | `#CECFCF` | `#494B4B` |
| Text / Text Neutral Primary | `#212222` | `#F2F2F2` |
| Muted / Text Neutral Secondary | `#474D4D` | `#CACECE` |
| Line、InputLine / Neutral Light | `#D2D5D5` | `#3D4242` |
| CardTabBorder / On Secondary Light | `#CACECE` | `#474D4D` |
| Accent / Enabled | `#006061` | `#63D9DE` |
| AccentHover / Hover | `#006A6B` | `#5DD2D5` |
| AccentPressed / Pressed | `#007475` | `#55CBCE` |
| AccentSoft / Item Enabled | `#C1DCDC` | `#284343` |
| AccentSoftHover / Item Hover | `#B9D4D4` | `#324E4E` |
| AccentSoftPressed / Item Pressed | `#B1CDCD` | `#3C5858` |
| RowSelected / Row Enabled | `#DFF1F1` | `#162C2C` |
| RowSelectedHover / Row Hover | `#D6EBEB` | `#1F3838` |
| RowSelectedPressed / Row Pressed | `#CDE4E5` | `#264040` |
| Focus / Outline Accent Focus | `#129190` | `#36AFB0` |
| OnAccent / Text On Accent | `#F7F8F8` | `#181B1B` |
| Success、Info / Outline Green、Blue | `#007A06`、`#395DC6` | `#57D15D`、`#9EB4FF` |
| Warning、Danger / Outline Yellow、Error | `#855D00`、`#99000D` | `#DDB35F`、`#FFBDC2` |

完整的 48 组 Light / Dark 映射、原资源名与来源提交在 [theme-reference-nova.json](../theme-reference-nova.json)。静态检查验证 96 个原值，运行时回归覆盖明暗主题与三档密度，防止默认主题仍落回旧色。普通按钮与编辑器默认采用 Secondary 填充，按钮文字为强调色；弹窗和内容区域采用 Primary。悬停、按下不再复用分割线颜色，列表与表格分别使用 Item、Row 高亮。

默认几何参数按参考主题分角色设置，而非所有控件共用一个圆角：

| 区域 | 圆角 / 描边 |
| --- | --- |
| 按钮、编辑器、列表外框 | 6 px；编辑器默认透明描边，焦点描边 1 px |
| ListBoxItem、ComboBoxItem | Small 为 4 px，Normal / Large 为 6 px |
| 菜单、Flyout、日期时间弹层、通知、对话框 | 8 px；弹层使用 OutlineStrong |
| ToolTip、CheckBox 标记 | 分别 4 px、3 px |
| DataGrid | 原生方形外框，OutlineMedium 分隔线 |
| 自定义 InfoCard | 保留 12 px 及既有区域分割线 |

CheckBox / RadioButton 的未选填充、选中图标和禁用状态分别映射 Secondary、IconOnAccent、DisabledSelection，开关轨道与滑块单独映射，避免复用文字色。通知色条使用 `SuccessFill / InfoFill / WarningFill / DangerFill`，图标仍用状态描边色。列表、菜单的未选悬停为中性底色。

图表单独映射参考 `Variants/Light|Dark/ChartColors.axaml`：背景为白 / 黑，文字为 `#385252 / #DBE6E6`，网格为 `#CADADA / #707575`；不再复用普通内容表面的 Surface / Line。系列组合与渐变维持既有扩展。

Card Tab 标题背景无外围描边，正文保留细边框；正文四角为 4 px，选中标题顶部为 6 px，靠正文一侧为内凹连接。四种标签方向均保留内容四角，圆角后方显示 Secondary 底色。

原始 Nova 的中性边框对比度低于 3:1，浅色 ToggleButton 按下时 Accent / AccentSoftPressed 文字对比度约 4.39:1。为满足原色一致的要求，保留其值，在 [contrast-report.json](../contrast-report.json) 中单独记录实际比值和未达标项；不将报告表述为全面符合 WCAG。这些原始参考值作为明确例外记录，其余正文、焦点、状态文字和渐变文字继续检查可读性。需要高对比边界的宿主可覆盖 InputLine / CardTabBorder。

## 渐变与扩展色

全部 10 个 `ThemeColor` 的 Light / Dark 均提供以下 8 种渐变；两类各 4 种。切换配色或明暗模式时，通过 `DynamicResource` 自动更新。

| 用途 | 柔和（渐变不明显） | 明显（强调色渐变） |
| --- | --- | --- |
| 品牌 | `One.Gradient.Subtle.Brand` | `One.Gradient.Vivid.Brand` |
| 冷色 | `One.Gradient.Subtle.Cool` | `One.Gradient.Vivid.Cool` |
| 自然 | `One.Gradient.Subtle.Natural` | `One.Gradient.Vivid.Natural` |
| 暖色 | `One.Gradient.Subtle.Warm` | `One.Gradient.Vivid.Warm` |

每个键是不可变 LinearGradientBrush，并同时提供同名前缀的 `.StartColor`、`.EndColor`。Subtle 从低比例语义色过渡至较高比例，适合卡片表面，文字使用 `One.Text`。Vivid 从当前 Accent 过渡到明暗端或 Info / Success / Warning，适合装饰条或强调图形；叠加文字时需按所选配色检查对比度。原有 Brand / BrandHover / BrandPressed / Glow / Data 键继续保留。

```xml
<Border Background="{DynamicResource One.Gradient.Subtle.Brand}" Padding="16">
  <TextBlock Text="主题卡片" Foreground="{DynamicResource One.Text}" />
</Border>
```

自定义颜色通过宿主 Application.Resources 或控件局部 Resources 覆盖同名键；需要同时覆盖 Light / Dark 时使用 ThemeDictionaries。不要在每次切换时移除或重新加载整个 OneTheme；调用 [枚举 API](theme-color-picker.md) 即可，保留业务控件与绑定。

渐变、状态浅底和图表系列组合为 TheOne 扩展，不宣称是 Eremex 原生效果：

- `One.Gradient.Brand` 从 Nova Accent 过渡到相邻青蓝色，另有 Hover / Pressed，使用 OnAccent 文字。
- `One.Gradient.Glow` 从 Primary Hover 过渡到 Row Enabled；适用于卡片标题，文字使用 Text / Muted。
- `One.Gradient.Data` 从 Item Enabled 过渡到 Accent，仅用于数据填充。
- 状态 `*Soft` 从 Primary 与对应原始状态色混合；InfoCard 正文为 Primary，Footer 为 Primary 向 Primary Hover 混合 20% 的派生色，保持细微差异。
- 图表六色取自 Eremex Graphics 的 Blue Green、Blue、Yellow、Purple、Purple Red、Green 色阶；浅色使用 Highest、深色使用 Lowest，以保持饼图百分比文字可读。系列组合属于本库选择，不等同于参考图表默认顺序。

渐变采用浅角度横向过渡（0%,0% → 100%,35%），缩小品牌渐变两端色相差，Hover / Pressed 保持独立明度层级。ToggleButton 单独外观类的空值态按参考使用 Primary 表面及独立短横线；显式语义色与外观组合则保留对应语义色及同一短横线。

本轮审查了全部 65 个控件主题文件，默认颜色由共享语义资源提供；窗口遮罩、渐变和自定义控件为明确扩展。核对范围见 [reference-audit.json](../reference-audit.json)。颜色一致不表示 Eremex 自有控件模板、布局和交互完全相同；本库继续使用 Avalonia 原生结构。

品牌渐变提供同名 `.StartColor` / `.EndColor` 资源。覆盖画刷时同时检查各状态及渐变中间色。模板使用 DynamicResource，支持实时主题切换。

## 多样式取舍

| 控件 | 新样式 | 理由 |
| --- | --- | --- |
| Button / IconButton / ToggleButton / RepeatButton | `soft`、`gradient` | 区分次要操作与重点操作；保留原有 primary / outline / ghost / link 和状态类 |
| Button / ToggleButton / RepeatButton / IconButton 组合 | `solid / ghost / link / outline / soft / gradient` + `primary / danger / success / warning / info` | 颜色与外观分离，如 `danger outline`、`success soft`；[优先级与兼容说明](../../controls/zh-cn/basic.md#button语义色与外观组合) |
| SplitButton / ToggleSplitButton | `gradient` | 双区操作共享视觉层级，保留原生 Flyout |
| TextBox / ComboBox / NumericUpDown / AutoCompleteBox | `filled`、`underlined` | 填充或底线外观，保留原生输入、焦点与校验 |
| CalendarDatePicker / DatePicker / TimePicker | `filled` | 填充输入，保留原生选择器与文字居中 |
| TabControl | `pills` | 胶囊标题，保留原生虚拟化、键盘与内容 Presenter |
| ProgressBar | `gradient`、`success`、`info`、`warning`、`danger` | 区分普通进度与任务状态 |
| InfoCard | `glow`、`flat`、`outlined` | 分别用于重点信息与嵌入式布局；无展开折叠 |
| ListBox / DataGrid | `borderless`；ListBox 另有 `cards` | 避免嵌入卡片后出现双重外框；保留原生虚拟化与选择 |

导航、日期时间、菜单及表格选中态使用纯色，保证焦点、选中与禁用清楚。Tag 已有 Soft / Outline / Solid 及状态属性，无需再增加重复的样式类。图表使用独立六系列色，通过画笔属性和已有类型选项表达差异，不给每个图表添加相同的样式类。

```xml
<Button Classes="gradient" Content="创建工作区" Command="{Binding CreateCommand}" />
<Button Classes="soft small" Content="稍后处理" />
<TextBox Classes="filled" Text="{Binding Name, Mode=TwoWay}" />
<ProgressBar Classes="success" Value="100" />
<one:InfoCard Classes="glow" Header="概览" Content="内容" />
```

每个控件选择一种主要外观类，避免同时设置 `primary danger` 或 `soft outline` 等同组冲突类。尺寸类可叠加；本地属性与自定义主题仍遵循 Avalonia 的优先级。

图表画笔默认对应 `One.Chart.Series1..6`，超过六个系列循环。类别较多时结合图例、标记及线型；颜色不承担唯一识别作用。`ChartSeries.Brush` 优先，`FifthBrush` / `SixthBrush` 与原有四个画笔一样可以显式覆盖并用于导出。

外观类在各控件原有页面中直接展示，不再提供独立主题预览页。 对比度报告见 [contrast-report.json](../contrast-report.json)。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### OneTheme

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Themes.OneTheme.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ColorTheme`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L30) | `TheOne.Avalonia.Themes.ThemeColor` | 可写; OneWay | `0 / default` | Color palette; changing this does not change the requested Light / Dark variant. 校验：`value => Enum.IsDefined(value)` 可选值：`Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |
| [`ColorThemes`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L36) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Themes.ThemeColor>` | 只读; CLR / n/a | `Array.AsReadOnly(Enum.GetValues<ThemeColor>())` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### ThemeColor

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Themes.ThemeColor.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`OneTheme`](../../api/zh-cn/TheOne.Avalonia.Themes.OneTheme.md) | `static void OneTheme.SetColor(ThemeColor color)` | Switches the application's OneTheme palette on the UI thread, preserving Light / Dark and property bindings. |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Themes.OneTheme`](../../api/zh-cn/TheOne.Avalonia.Themes.OneTheme.md) | 2 |
| [`TheOne.Avalonia.Themes.ThemeColor`](../../api/zh-cn/TheOne.Avalonia.Themes.ThemeColor.md) | 10 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
