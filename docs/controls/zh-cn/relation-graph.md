# RelationGraph 树形关系图

**简体中文** · [English](../en/relation-graph.md)

`RelationGraph` 接收一个根节点，按树形层级绘制关系。`Orientation="Horizontal"` 从左向右展开，`Vertical` 从上向下展开；节点内部始终保持左侧图标、右侧标题和内容。先加载 `OneTheme`。

## 默认模型

```csharp
var root = new RelationGraphNode
{
    Title = "研发中心",
    Content = "产品与技术协同"
};
root.Children!.Add(new RelationGraphNode
{
    Title = "应用开发", Content = "桌面与跨平台应用"
});
```

```xml
<ScrollViewer Height="540" HorizontalScrollBarVisibility="Auto"
              VerticalScrollBarVisibility="Auto">
  <one:RelationGraph RootNode="{Binding Root}" Orientation="Horizontal" />
</ScrollViewer>
```

`RelationGraphNode` 内置 `Icon`、`Title`、`Content`（均为 `object?`），以及 `IList<RelationGraphNode>? Children`。每个实例默认拥有独立的 `ObservableCollection<RelationGraphNode>`。属性通过 `INotifyPropertyChanged` 通知；`Children` 可以替换，`null` 表示叶子。

`RootNode` 是单个对象，不能用多个根节点集合代替。根节点为 `null` 时图为空。树中不允许循环、重复引用同一个节点或 `null` 子节点；错误时停止显示，并通过只读 `ValidationError` 给出原因。可观察数据修正后自动恢复。

## 自定义模型与 ChildrenSource

无需继承默认模型或复制树。`ChildrenSource` 保存绑定表达式，并在**每个节点的 DataContext** 中求值；支持不同名称的子节点集合和嵌套属性路径。`IconSource`、`TitleSource`、`ContentSource` 按相同方式选择显示字段。

假设 `OrganizationUnit` 定义 `Branches`、`Symbol`、`Name`、`Description`，视图模型定义一个 `OrganizationUnit Root`：

```xml
<one:RelationGraph RootNode="{Binding Root}" Orientation="Vertical"
    ChildrenSource="{CompiledBinding Branches, DataType={x:Type vm:OrganizationUnit}}"
    IconSource="{CompiledBinding Symbol, DataType={x:Type vm:OrganizationUnit}}"
    TitleSource="{CompiledBinding Name, DataType={x:Type vm:OrganizationUnit}}"
    ContentSource="{CompiledBinding Description, DataType={x:Type vm:OrganizationUnit}}" />
```

这些绑定需要显式声明**节点类型**，不同于视图根元素的视图模型类型。默认模型省略 Source 属性即可；自定义模型必须提供 `ChildrenSource`，其结果必须为 `IEnumerable` 或 `null`（字符串无效）。省略自定义模型的显示 Source 会使对应区域为 `null`。

变化属性实现 `INotifyPropertyChanged`；变化集合实现 `INotifyCollectionChanged`，如 `ObservableCollection<T>`。属性替换和集合增删自动更新；普通 `List<T>` 原地修改后调用 `RefreshData()`。数据与集合变化应发生在 UI 线程。控件脱离可视树后解除订阅，重新挂载时读取最新数据。

C# 可用 `Avalonia.Data.CompiledBinding.Create<TNode, TValue>(node => node.Branches)` 设置 `ChildrenSource`。不要把属性名称字符串当作绑定；NativeAOT 项目使用上述编译绑定，控件不会反射扫描用户模型。

## 节点内容与空值

| 区域 | 数据 / 模板 | 为 null 时 |
| --- | --- | --- |
| 左侧图标 | `Icon` / `IconTemplate` | 图标及间距不占空间，文本占据整个宽度 |
| 右上标题 | `Title` / `TitleTemplate` | 内容在整个文本区竖向居中 |
| 右下内容 | `Content` / `ContentTemplate` | 标题在整个文本区竖向居中 |

标题和内容同时存在时，整个文字组合竖向居中。只有图标时，在整个节点中居中。所有区域为 `null` 时保留空节点外框及连接关系。模板本身不会为 `null` 数据预留区域；空字符串仍为非 null 数据。

文本自动换行，内容可让节点高于 `NodeMinHeight`。`Icon` 可直接提供 `PathIcon` 等原生控件，或提供 Geometry / 业务对象并设置模板。例如：

```xml
<one:RelationGraph.IconTemplate>
  <DataTemplate x:DataType="media:Geometry">
    <PathIcon Data="{Binding}" />
  </DataTemplate>
</one:RelationGraph.IconTemplate>
```

在视图中声明 `xmlns:media="using:Avalonia.Media"`。模板的数据上下文是相应字段值；显示字段可承载原生控件，保留焦点、按钮命令和键盘行为。同一个控件实例只能由一个节点区域拥有，需要重复图标时共享 Geometry 并由模板分别创建控件。

## 节点选择

`SelectItem` 保存当前选中的**节点模型对象**，默认值为 null，默认绑定方向是 TwoWay。点击任意有效节点会同步选中状态；节点获得键盘焦点后可按 Enter / Space 选中，Tab 导航仍由 Avalonia 处理。点击节点内的按钮或输入框也会选中节点，同时保留这些原生控件的命令和焦点。

```xml
<one:RelationGraph RootNode="{Binding Root}"
                   SelectItem="{Binding SelectedNode, Mode=TwoWay}" />
```

`SelectedNode` 的类型应与节点模型一致，如 `RelationGraphNode?` 或自定义 `OrganizationUnit?`；自定义模型仍需配置 ChildrenSource 等编译绑定。选中值按对象引用匹配当前树，不能传入另建的相同内容对象或 Presenter。模型属性初始化可早于 RootNode，首轮布局后会校验选择。选中节点被移除、根节点清空或数据变为无效树时，SelectItem 自动清空并写回绑定；运行时设置树外对象也会清空。设置 null 可主动取消选择。

节点 Presenter 提供只读 `IsSelected` 与 `:selected` 伪类，默认用强调色底色、轮廓和图标区显示选择。NodeTheme 可定制该状态；选择不改变节点几何尺寸。禁用节点不会响应指针和键盘选择。

## 布局与外观

| 属性 | 默认 Normal 值 | 用途 |
| --- | --- | --- |
| `Orientation` | Horizontal | 树的展开方向 |
| `NodeWidth` / `NodeMinHeight` | 260 / 88 | 固定卡片宽度 / 自然内容高度下限 |
| `NodeSpacing` | 36 | 相邻子树在横截方向的间距 |
| `LevelSpacing` | 80 | 层级间距；每层按最大节点高度或宽度计算 |
| `Padding` | 16 | 整张图边缘留白 |
| `ConnectorBrush` / `ConnectorThickness` | One.Accent / 1.5 | 连接线颜色与宽度，宽度为 0 时隐藏 |
| `ConnectorCornerRadius` / `PortRadius` | 8 / 3 | 正交拐角与连接端点半径 |
| `NodeTheme` | 默认主题 | 替换 `RelationGraphNodePresenter` 的 ControlTheme |
| `NodeCount` / `ValidationError` | 只读 | 当前有效树的节点数量 / 数据校验错误 |

尺寸、间距、连接线和模板均可通过属性调整。`NodeTheme` 可基于默认节点主题定义填充、轮廓、字体、图标尺寸、区域间距和模板。`RelationGraphNodePresenter.Header` 对应模型 `Title`，并提供 `IsRoot`、`IconBackground`、`IconForeground`、`ContentForeground`、`HeaderFontSize`、`IconSize`、`IconTileSize`、`IconSpacing`、`TextSpacing`。默认模板部件为 `PART_Surface`、`PART_Layout`、`PART_IconHost`、`PART_IconPresenter`、`PART_TextHost`、`PART_HeaderPresenter`、`PART_ContentPresenter`。

默认样式使用 `One.Surface`、`One.Text`、`One.Muted`、`One.Line`、`One.Accent`、`One.AccentSoft`、`One.RowSelected`。根节点通过 `:root` 显示强调轮廓和底色。`ControlSizing.Size` 支持 Small / Normal / Large：节点宽度为 220 / 260 / 300，高度下限为 72 / 88 / 104，字体与图标同步变化；显式属性覆盖尺寸默认值。

控件按整棵树自然尺寸布局，使用外层原生 `ScrollViewer` 处理窄视口；不会压缩节点文字。当前完整实现所有节点，无虚拟化，适合有限规模的关系树。没有内置折叠、拖动或缩放。布局遍历使用迭代方式，连接线直接绘制，节点内容使用原生 Presenter。

## 预览与验证

PreviewHost 的“数据展示 → RelationGraph”展示方向切换、宽度与间距、根节点空值、集合增删、自定义模型绑定和八种区域组合。

```sh
dotnet build design/PreviewHost/PreviewHost.csproj -c Release --artifacts-path .preview/relation-graph/artifacts --disable-build-servers -m:1
dotnet .preview/relation-graph/artifacts/bin/PreviewHost/release/PreviewHost.dll --verify-relation-graph .preview/relation-graph/managed
```

专项检查明暗主题、三档尺寸、横纵布局、真实连接线像素、窄视口、自动换行、空值、集合及属性替换、无效树恢复、订阅重挂载、原生控件交互以及 XAML 编译绑定。独立 NativeAOT 消费端复用同一专项：

```sh
# macOS：仅在涉及 AOT／绑定／元数据时执行原生专项。
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 --artifacts-path .preview/relation-graph/native-artifacts -o .preview/relation-graph/native
.preview/relation-graph/native/NativeAotSmoke --require-aot --verify-relation-graph .preview/relation-graph/native-pixels
```

选择专项覆盖明暗主题、三档尺寸、两种方向、真实选中像素、指针／键盘选择、节点内按钮命令与焦点、节点移除和无效树的选择清理，以及默认 / 自定义模型的双向编译绑定。原生消费执行同一 `--verify-relation-graph` 专项。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 在各控件页直接标明示例的 Classes、尺寸及状态；公共属性、API／事件和模型字段使用 Description 组件说明，包含属性类型、默认值、绑定模式、校验条件及 API 签名。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### RelationGraph

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.RelationGraph.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Background`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L53) | `Avalonia.Media.IBrush?` | 可写; OneWay | `inherited registration` | 背景画刷；主题相关背景使用 DynamicResource，避免固定颜色破坏明暗切换。 |
| [`ChildrenSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L60) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | A binding evaluated against each node, not against the graph's data context. |
| [`ConnectorBrush`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L73) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | One.Accent / 1.5 ; 连接线颜色与宽度，宽度为 0 时隐藏 |
| [`ConnectorCornerRadius`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L75) | `double` | 可写; OneWay | `8` | 8 / 3 ; 正交拐角与连接端点半径 校验：`NonNegative` |
| [`ConnectorThickness`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L74) | `double` | 可写; OneWay | `1.5` | One.Accent / 1.5 ; 连接线颜色与宽度，宽度为 0 时隐藏 校验：`NonNegative` |
| [`ContentSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L63) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取关系节点正文的类型化字段访问器；不使用字符串反射路径。 |
| [`ContentTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L66) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`IconSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L61) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取关系节点图标的类型化字段访问器。 |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L64) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`LevelSpacing`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L71) | `double` | 可写; OneWay | `80` | 80 ; 层级间距；每层按最大节点高度或宽度计算 校验：`NonNegative` |
| [`NodeCount`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L79) | `int` | 只读; OneWay | `computed / state` | 只读 ; 当前有效树的节点数量 / 数据校验错误 |
| [`NodeMinHeight`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L69) | `double` | 可写; OneWay | `88` | 260 / 88 ; 固定卡片宽度 / 自然内容高度下限 校验：`Positive` |
| [`NodeSpacing`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L70) | `double` | 可写; OneWay | `36` | 36 ; 相邻子树在横截方向的间距 校验：`NonNegative` |
| [`NodeTheme`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L67) | `Avalonia.Styling.ControlTheme?` | 可写; OneWay | `null` | 默认主题 ; 替换 `RelationGraphNodePresenter` 的 ControlTheme |
| [`NodeWidth`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L68) | `double` | 可写; OneWay | `260` | 260 / 88 ; 固定卡片宽度 / 自然内容高度下限 校验：`Positive` |
| [`Orientation`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L58) | `Avalonia.Layout.Orientation` | 可写; OneWay | `Orientation.Horizontal` | Horizontal ; 树的展开方向 校验：`Enum.IsDefined` 可选值：`Horizontal`, `Vertical` |
| [`Padding`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L72) | `Avalonia.Thickness` | 可写; OneWay | `inherited registration` | 16 ; 整张图边缘留白 |
| [`PortRadius`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L76) | `double` | 可写; OneWay | `3` | 8 / 3 ; 正交拐角与连接端点半径 校验：`NonNegative` |
| [`RootNode`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L55) | `object?` | 可写; OneWay | `null` | 关系图的根模型；子节点通过模型树或访问器获取。 |
| [`SelectItem`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L57) | `object?` | 可写; TwoWay | `null` | The selected node model, matched by reference. Null clears selection. |
| [`TitleSource`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L62) | `Avalonia.Data.BindingBase?` | 可写; OneWay | `null` | 读取关系节点标题的类型化字段访问器。 |
| [`TitleTemplate`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L65) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`ValidationError`](../../../TheOne.Avalonia/Controls/RelationGraph.cs#L78) | `string?` | 只读; OneWay | `computed / state` | 只读 ; 当前有效树的节点数量 / 数据校验错误 |

### RelationGraphNode

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.RelationGraphNode.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Children`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L18) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.RelationGraphNode>?` | 可写; CLR / n/a | `new ObservableCollection<RelationGraphNode>()` | 子级模型集合；保持父子与对象身份，避免循环和跨父级共享可变节点。 |
| [`Content`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L17) | `object?` | 可写; CLR / n/a | `null` | 宿主内容或业务展示对象；具体模板作用域见内容模板契约，不把视图对象当作持久化数据。 |
| [`Icon`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L15) | `object?` | 可写; CLR / n/a | `null` | 图标内容或矢量几何，具体类型见签名；外部位图所有权仍由宿主负责。 |
| [`Title`](../../../TheOne.Avalonia/Controls/RelationGraphNode.cs#L16) | `object?` | 可写; CLR / n/a | `null` | 显示标题；不等于业务主键或本地化资源键。 |

### RelationGraphNodePresenter

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.RelationGraphNodePresenter.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ContentForeground`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L39) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 节点正文文字画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`HeaderFontSize`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L38) | `double` | 可写; OneWay | `16` | 节点标题字号，单位 DIP，按实际文本度量参与布局。 校验：`Positive` |
| [`Icon`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L32) | `object?` | 可写; OneWay | `null` | 图标内容或矢量几何，具体类型见签名；外部位图所有权仍由宿主负责。 |
| [`IconBackground`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L40) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 图标底板画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`IconForeground`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L41) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 图标前景画刷；动态主题使用 One.* 资源，显式局部值优先。 |
| [`IconSize`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L34) | `double` | 可写; OneWay | `24` | 图标绘制尺寸，单位 DIP；独立于文本字号与按钮点击区域。 校验：`Positive` |
| [`IconSpacing`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L36) | `double` | 可写; OneWay | `14` | 图标与相邻文本／内容的 DIP 间距。 校验：`NonNegative` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L33) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`IconTileSize`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L35) | `double` | 可写; OneWay | `44` | 图标承载底板的 DIP 尺寸，独立于图标自身尺寸。 校验：`Positive` |
| [`IsRoot`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L42) | `bool` | 可写; OneWay | `false` | 此节点是否为根节点；由关系图生成器维护。 |
| [`IsSelected`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L44) | `bool` | 只读; OneWay | `computed / state` | Selection state maintained by the owning RelationGraph. |
| [`TextSpacing`](../../../TheOne.Avalonia/Controls/RelationGraphNodePresenter.cs#L37) | `double` | 可写; OneWay | `6` | 标题与正文之间的 DIP 间距。 校验：`NonNegative` |

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
| RelationGraph | `one-large` | 模板自动类；不手动绑定 | ^.one-large => NodeWidth=300; NodeMinHeight=104; NodeSpacing=44; LevelSpacing=100 |
| RelationGraph | `one-small` | 模板自动类；不手动绑定 | ^.one-small => NodeWidth=220; NodeMinHeight=72; NodeSpacing=28; LevelSpacing=60 |
| RelationGraphNodePresenter | `one-large` | 模板自动类；不手动绑定 | ^.one-large => Padding=20,18; FontSize=16; HeaderFontSize=18; IconSize=28; IconTileSize=52; IconSpacing=18; TextSpacing=8 |
| RelationGraphNodePresenter | `one-small` | 模板自动类；不手动绑定 | ^.one-small => Padding=12,10; FontSize=12; HeaderFontSize=14; IconSize=20; IconTileSize=36; IconSpacing=10; TextSpacing=4 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.RelationGraph`](../../api/zh-cn/TheOne.Avalonia.Controls.RelationGraph.md) | Class | 23 | 4 | 0 |
| [`TheOne.Avalonia.Controls.RelationGraphNode`](../../api/zh-cn/TheOne.Avalonia.Controls.RelationGraphNode.md) | Class | 4 | 1 | 1 |
| [`TheOne.Avalonia.Controls.RelationGraphNodePresenter`](../../api/zh-cn/TheOne.Avalonia.Controls.RelationGraphNodePresenter.md) | Class | 12 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
