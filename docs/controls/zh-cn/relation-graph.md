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
dotnet build TheOne.Avalonia.slnx --disable-build-servers -m:1
dotnet design/PreviewHost/bin/Debug/net10.0/PreviewHost.dll --verify-relation-graph .preview/relation-graph/managed
```

专项检查明暗主题、三档尺寸、横纵布局、真实连接线像素、窄视口、自动换行、空值、集合及属性替换、无效树恢复、订阅重挂载、原生控件交互以及 XAML 编译绑定。独立 NativeAOT 消费端复用同一专项：

```sh
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 -o .preview/relation-graph/native
.preview/relation-graph/native/NativeAotSmoke --require-aot --verify-relation-graph .preview/relation-graph/native-pixels
```

选择专项覆盖明暗主题、三档尺寸、两种方向、真实选中像素、指针／键盘选择、节点内按钮命令与焦点、节点移除和无效树的选择清理，以及默认 / 自定义模型的双向编译绑定。原生消费执行同一 `--verify-relation-graph` 专项。
