# InfoCard 信息卡片

**简体中文** · [English](../en/info-card.md)

`InfoCard` 继承官方 `HeaderedContentControl`，用于展示标题与内容，并提供标题图标、标题操作区、页脚区域。加载 `OneTheme` 后自动使用 TheOne 表面、细边框和圆角，并继承明暗主题及三种尺寸。卡片固定展示存在的区域；放入其中的按钮、输入框和列表保留原生交互。

```xml
<one:InfoCard Header="工作区概览"
              Content="使用统一的标题、正文和间距展示信息。" />
```

`one` 命名空间为 `using:TheOne.Avalonia.Controls`。标题、图标与操作区均为空时移除整个标题行；`Header` 为 null、空字符串或空白字符串时只移除标题文字，图标或操作区仍可独立显示。各空内容区域自动隐藏，不保留额外间隔。

## 自定义标题与内容

标题和内容支持字符串、控件或数据模型。控件实例直接成为对应 presenter 的子元素，输入、焦点、命令及验证仍由原生控件负责。

```xml
<one:InfoCard>
  <one:InfoCard.Header>
    <Grid ColumnDefinitions="*,Auto" ColumnSpacing="12">
      <TextBlock Text="工作区设置" FontWeight="SemiBold" />
      <Button Grid.Column="1" Content="保存" Command="{Binding SaveCommand}" />
    </Grid>
  </one:InfoCard.Header>
  <TextBox Text="{Binding WorkspaceName, Mode=TwoWay}" />
</one:InfoCard>
```

## 标题图标、操作区与页脚

`HeaderIcon` 位于标题前，`HeaderActions` 位于标题右侧，`Footer` 位于内容下方。这些属性均接受控件、文本或模型，分别通过 `HeaderIconTemplate`、`HeaderActionsTemplate` 和 `FooterTemplate` 展示数据模型。

```xml
<one:InfoCard Header="工作区设置">
  <one:InfoCard.HeaderIcon>
    <PathIcon Data="M 2,2 L 18,2 L 18,18 L 2,18 Z" />
  </one:InfoCard.HeaderIcon>
  <one:InfoCard.HeaderActions>
    <Button Content="详情" Command="{Binding DetailsCommand}" />
  </one:InfoCard.HeaderActions>
  <one:InfoCard.Footer>
    <Grid ColumnDefinitions="*,Auto" ColumnSpacing="12">
      <TextBlock Text="{Binding Status}" TextWrapping="Wrap" />
      <Button Grid.Column="1" Content="同步" Command="{Binding SyncCommand}" />
    </Grid>
  </one:InfoCard.Footer>
  <TextBox Text="{Binding WorkspaceName, Mode=TwoWay}" />
</one:InfoCard>
```

显式数据模板可以接收 null 数据，因此只有内容为 null 时，已设置的模板仍可展示该区域。需要彻底隐藏图标、操作区或页脚时，应同时将该区域的内容和对应模板设为 null；PreviewHost 的显示开关同时切换两者，收回区域与其间距。自定义图标可通过 `HeaderIconSize` 统一调整尺寸，标题各部分之间的间距由 `HeaderContentSpacing` 控制，页脚与前方区域的间距由 `FooterSpacing` 控制。

页脚也支持明确类型的编译绑定模板：

```xml
<one:InfoCard Header="工作区概览" Content="{Binding Description}" Footer="{Binding CurrentWorkspace}">
  <one:InfoCard.FooterTemplate>
    <DataTemplate x:DataType="vm:WorkspaceViewModel">
      <Grid ColumnDefinitions="*,Auto" ColumnSpacing="12">
        <TextBlock Text="{Binding Status}" TextWrapping="Wrap" />
        <Button Grid.Column="1" Content="同步" Command="{Binding SyncCommand}" />
      </Grid>
    </DataTemplate>
  </one:InfoCard.FooterTemplate>
</one:InfoCard>
```

在使用绑定的视图根节点声明准确的 `x:DataType`。`HeaderTemplate` 和 `ContentTemplate` 使用官方 `IDataTemplate` 合约；各数据模板也应声明自己的类型，保持编译绑定与 NativeAOT 兼容。

```xml
<one:InfoCard Header="{Binding CurrentWorkspace}" Content="{Binding CurrentWorkspace}">
  <one:InfoCard.HeaderTemplate>
    <DataTemplate x:DataType="vm:WorkspaceViewModel">
      <TextBlock Text="{Binding Name}" FontWeight="SemiBold" />
    </DataTemplate>
  </one:InfoCard.HeaderTemplate>
  <one:InfoCard.ContentTemplate>
    <DataTemplate x:DataType="vm:WorkspaceViewModel">
      <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
    </DataTemplate>
  </one:InfoCard.ContentTemplate>
</one:InfoCard>
```

## 区域分割线

`ShowHeaderSeparator`、`ShowFooterSeparator` 默认均为 `true`，可独立设置、绑定或通过 Style setter 控制。

```xml
<one:InfoCard Header="工作区" Footer="已保存"
              ShowHeaderSeparator="True"
              ShowFooterSeparator="False"
              Content="正文内容" />
```

标题分割线在标题与正文均存在时显示；页脚分割线在页脚存在且前方有正文或标题时显示。无对应区域时不留空线。线条采用 `One.Line` 语义色、厚度为 1 DIP，从左边框内缘连接至右边框内缘，位于相邻区域背景的交界处。隐藏分割线仅移除该 1 DIP 行，不改变区域内边距。

## 区域背景与内边距

Header 保留较明显的背景区别，Content 与 Footer 只有细微色差。三者默认分别采用 `One.Subtle`、`One.Surface`、`One.InfoCard.Footer`，随 Light / Dark 主题切换，铺满各自区域并裁切到卡片圆角。Footer 浅色为 `#FCFCFC`（正文 `#FFFFFF`），深色为 `#131313`（正文 `#0F0F0F`），每个 RGB 通道的差值不超过 4。

| 属性 | 用途 |
| --- | --- |
| `HeaderBackground` | 标题区域背景，默认 `One.Subtle` |
| `Background` | 正文区域及卡片底色，默认 `One.Surface` |
| `FooterBackground` | 页脚区域背景，默认 `One.InfoCard.Footer` |
| `Padding` | 正文四周内边距，以及标题／页脚的左右内边距 |
| `HeaderSpacing` | 标题区域的上下内边距 |
| `FooterSpacing` | 页脚区域的上下内边距 |

`HeaderPadding`／`FooterPadding` 是由上述设置计算的只读布局属性，供模板使用。缺少区域时不保留该区域的背景或内边距。禁用时三个区域统一采用 `One.DisabledSurface`；实例属性与 Style setter 可覆盖背景。

```xml
<one:InfoCard Header="标题" Content="正文" Footer="页脚"
              HeaderBackground="{DynamicResource One.Subtle}"
              Background="{DynamicResource One.Surface}"
              FooterBackground="{DynamicResource One.InfoCard.Footer}" />
```

## 尺寸与样式

`Classes="shadow"` 显示常驻外阴影；`Classes="hove"` 在鼠标移入时强调边框、调整正文底色并显示悬浮阴影，移出后恢复。类名使用 `hove`。两者可组合，`Classes="hove shadow"` 在常驻阴影上提供更明显的悬浮效果；禁用时这些类的阴影与悬浮效果停止。

```xml
<one:InfoCard Classes="hove shadow" Header="可交互卡片" Margin="16"
              Content="鼠标移入查看强调边框和阴影。" />
```

效果不移动内容、不增加布局尺寸，也不拦截卡片中的原生输入。外阴影由独立的 `PART_Shadow` 绘制，内容仍由 `PART_Surface` 裁切圆角；父容器裁切时应留出足够 Margin / Padding。`BoxShadow` 可通过实例属性或样式设置，例如 `BoxShadow="0 2 10 0 #30000000"`；显式本地值优先于类样式。默认阴影使用随明暗切换的 `One.InfoCard.Shadow` 与 `One.InfoCard.HoverShadow` 资源。

`Classes="outlined"` 使用主色边框与柔和标题背景；`Classes="glow"` 使用柔光渐变标题背景；`Classes="flat"` 去除外框与分割线，标题和页脚透明，适合嵌入布局。这些样式保留内容槽与模板接口，无展开折叠行为。

通过 `ControlSizing.Size` 继承父级尺寸，或使用 `Classes="small"` / `normal` / `large` 在单张卡片上指定尺寸。以下属性也可通过 Style setter 或实例属性覆盖。

| 属性 | small | normal | large |
| --- | ---: | ---: | ---: |
| `Padding` | 16 | 20 | 24 |
| `FontSize` | 12 | 14 | 16 |
| `HeaderFontSize` | 14 | 16 | 18 |
| `HeaderSpacing` | 8 | 12 | 16 |
| `HeaderIconSize` | 16 | 18 | 20 |
| `HeaderContentSpacing` | 8 | 10 | 12 |
| `FooterSpacing` | 8 | 12 | 16 |

卡片沿用 `Background`、`Foreground`、`BorderBrush`、`BorderThickness` 和 `CornerRadius`。`HeaderSpacing`／`FooterSpacing` 控制各自区域的上下内边距；未设置固定高度时，卡片按内容高度布局。禁用卡片时，标题、正文及页脚内的原生控件也被禁用。

## 列表内容

InfoCard 不替换内容的布局，也不额外包裹无限高度的滚动容器。可增长的集合应放入有界且支持虚拟化的原生控件，用 `ItemsSource` 提供模型，不预创建大量控件。

```xml
<one:InfoCard Header="成员">
  <ListBox Height="200" ItemsSource="{Binding Members}" />
</one:InfoCard>
```

也可为卡片设置有限的 `Height` 或 `MaxHeight`，在正文中使用 `Grid` 的 `*` 行承载列表，使列表继承剩余的有限视口。折叠与恢复仍遵循该布局约束；不要把列表放入无限高度的 `StackPanel` 而又不设置列表高度。

PreviewHost 的 InfoCard 预览包含标题、图标、操作区、页脚及两处分割线的独立开关，原生输入与按钮，明确类型的标题、正文、图标、操作区和页脚编译绑定模板，以及固定高度的 1,000 条成员列表和末尾跳转。固定高度与 MaxHeight 专项检查千条列表回收和页脚可见性。标题操作和页脚按钮可更新状态，禁用卡片会同时禁用各区域的交互。仅导出当前控件的测试图：

执行命令与测试范围见[预览专项入口](../../../design/PreviewHost/Program.cs)。

专项可保存悬浮和阴影的实际像素截图：

```bash
dotnet design/PreviewHost/bin/Debug/net10.0/PreviewHost.dll --verify-infocard .preview/infocard-effects/managed
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 -o .preview/infocard-effects/native
.preview/infocard-effects/native/NativeAotSmoke --require-aot --verify-infocard .preview/infocard-effects/native-pixels
```

原生产物按现有 NativeAotSmoke 消费端编译，使用 macOS xcrun 的 clang 与 SDK。专项涵盖明暗主题、三档尺寸、鼠标进出、组合类、禁用、本地值覆盖和阴影实际像素。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节只列出本项目增加的 Classes、属性、扩展 API／事件及关联模型，不重复 Avalonia 原生继承 API。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用可见示例对照本项目附加样式，用 Description 组件说明专属属性、API 签名与模型字段；ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### InfoCard

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.InfoCard.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`BoxShadow`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L50) | `Avalonia.Media.BoxShadows` | 可写; OneWay | `inherited registration` | Outer shadow, supplied by the shadow and hove theme classes. |
| [`Footer`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L56) | `object?` | 可写; OneWay | `null` | 底部区域内容；与主体和标题分别定制。 |
| [`FooterBackground`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L61) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 页脚区域背景，默认 `One.InfoCard.Footer` |
| [`FooterPadding`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L65) | `Avalonia.Thickness` | 只读; OneWay | `computed / state` | Computed footer insets: horizontal Padding and vertical FooterSpacing. |
| [`FooterSpacing`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L74) | `double` | 可写; OneWay | `12` | 页脚区域的上下内边距 校验：`value => double.IsFinite(value) && value >= 0` |
| [`FooterTemplate`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L57) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`HeaderActions`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L54) | `object?` | 可写; OneWay | `null` | 标题区右侧的操作内容；可放置业务按钮，保持命令和焦点行为。 |
| [`HeaderActionsTemplate`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L55) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`HeaderBackground`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L59) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 标题区域背景，默认 `One.Subtle` |
| [`HeaderContentSpacing`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L69) | `double` | 可写; OneWay | `10` | 8 ; 10 ; 12 校验：`value => double.IsFinite(value) && value >= 0` |
| [`HeaderFontSize`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L67) | `double` | 可写; OneWay | `16` | 14 ; 16 ; 18 校验：`value => double.IsFinite(value) && value > 0` |
| [`HeaderIcon`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L52) | `object?` | 可写; OneWay | `null` | 标题区图标内容，与标题文本分开定制。 |
| [`HeaderIconSize`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L68) | `double` | 可写; OneWay | `18` | 16 ; 18 ; 20 校验：`value => double.IsFinite(value) && value > 0` |
| [`HeaderIconTemplate`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L53) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`HeaderPadding`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L63) | `Avalonia.Thickness` | 只读; OneWay | `computed / state` | Computed header insets: horizontal Padding and vertical HeaderSpacing. |
| [`HeaderSpacing`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L66) | `double` | 可写; OneWay | `12` | 标题区域的上下内边距 校验：`value => double.IsFinite(value) && value >= 0` |
| [`ShowFooterSeparator`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L73) | `bool` | 可写; OneWay | `true` | Shows a divider before an existing footer when another region precedes it. |
| [`ShowHeaderSeparator`](../../../TheOne.Avalonia/Controls/InfoCard.cs#L71) | `bool` | 可写; OneWay | `true` | Shows an edge-to-edge divider between the header and body. |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| InfoCard | `flat` | 显式 Classes，可与尺寸类组合 | one&#124;InfoCard.flat => BorderThickness=0; HeaderBackground=Transparent; FooterBackground=Transparent; ShowHeaderSeparator=False; ShowFooterSeparator=False; one&#124;InfoCard.glow:disabled, one&#124;InfoCard.flat:disabled => HeaderBackground={DynamicResource One.DisabledSurface}; FooterBackground={DynamicResource One.DisabledSurface} |
| InfoCard | `glow` | 显式 Classes，可与尺寸类组合 | one&#124;InfoCard.glow => HeaderBackground={DynamicResource One.Gradient.Glow}; one&#124;InfoCard.glow:disabled, one&#124;InfoCard.flat:disabled => HeaderBackground={DynamicResource One.DisabledSurface}; FooterBackground={DynamicResource One.DisabledSurface} |
| InfoCard | `hove` | 显式 Classes，可与尺寸类组合 | ^.hove:pointerover:not(:disabled) => BorderBrush={DynamicResource One.Accent}; Background={DynamicResource One.SurfaceHover}; BoxShadow={DynamicResource One.InfoCard.HoverShadow} |
| InfoCard | `one-large` | 模板自动类；不手动绑定 | one&#124;InfoCard.one-large => Padding=24; FontSize=16; HeaderFontSize=18; HeaderSpacing=16; HeaderIconSize=20; HeaderContentSpacing=12; FooterSpacing=16 |
| InfoCard | `one-normal` | 模板自动类；不手动绑定 | one&#124;InfoCard.one-normal => Padding=20; FontSize=14; HeaderFontSize=16; HeaderSpacing=12; HeaderIconSize=18; HeaderContentSpacing=10; FooterSpacing=12 |
| InfoCard | `one-small` | 模板自动类；不手动绑定 | one&#124;InfoCard.one-small => Padding=16; FontSize=12; HeaderFontSize=14; HeaderSpacing=8; HeaderIconSize=16; HeaderContentSpacing=8; FooterSpacing=8 |
| InfoCard | `outlined` | 显式 Classes，可与尺寸类组合 | one&#124;InfoCard.outlined => BorderBrush={DynamicResource One.Accent}; HeaderBackground={DynamicResource One.AccentSoft}; one&#124;InfoCard.outlined:disabled => BorderBrush={DynamicResource One.Line}; HeaderBackground={DynamicResource One.DisabledSurface} |
| InfoCard | `shadow` | 显式 Classes，可与尺寸类组合 | ^.shadow:not(:disabled) => BoxShadow={DynamicResource One.InfoCard.Shadow} |

`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出附加外观类的控件使用默认主题及其专属属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.InfoCard`](../../api/zh-cn/TheOne.Avalonia.Controls.InfoCard.md) | Class | 18 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
