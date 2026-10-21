# TreemapChart 与 SunburstChart 层级图表

`TreemapChart` 使用矩形面积展示树形数据；`SunburstChart` 使用从内向外的环形层级展示同一棵树。两者采用原生直接绘制、缓存布局和命中信息，不为节点创建控件。通用图表标题、主题、动画和 PNG 导出参见 [图表](charts.md)。

## 数据与绑定

```csharp
using System.Collections.ObjectModel;
using TheOne.Avalonia.Controls;

public ObservableCollection<ChartHierarchyNode> Departments { get; } =
[
    new("研发", 0, "engineering")
    {
        Children = new ObservableCollection<ChartHierarchyNode>
        {
            new("客户端", 45, "client"),
            new("服务端", 55, "server")
        }
    },
    new("运营", 30, "operations")
];
```

```xml
<one:TreemapChart ItemsSource="{CompiledBinding Departments}"
                  Title="部门投入" Gap="3" HeaderHeight="20" />
<one:SunburstChart ItemsSource="{CompiledBinding Departments}"
                   Title="部门层级" InnerRadius="0.18" RingGap="2" />
```

`ItemsSource` 是唯一业务数据入口。继承的 `SeriesSource` 由控件维护为一个扁平投影，供通用选择、键盘和导出设施使用；不要再绑定、替换或修改该投影。`SelectedSeriesIndex` 固定使用 `0`，`SelectedIndex` 是当前森林的深度优先先序索引，结构变化后可能改变。业务逻辑优先使用 `SelectedNode` 或 `Key`。

## ChartHierarchyNode

| 属性 | 类型 / 默认值 | 说明 |
| --- | --- | --- |
| `Label` | `string?` / `null` | 继承自 `ChartPoint`，用于节点文字和提示。 |
| `Y` | `double` / `0` | 叶节点权重；仅有限正数参与布局。 |
| `X` | `double` / `0` | 继承成员，层级图不使用其坐标含义。 |
| `Key` | `string?` / `null` | 业务标识，按区分大小写的字符串比较；建议在整个森林中唯一。 |
| `Children` | `IList<ChartHierarchyNode>?` / 独立的 `ObservableCollection` | 子节点；非空时父节点权重取有效子节点权重之和，忽略父节点自己的 `Y`。 |
| `Brush` | `IBrush?` / `null` | 可选分支颜色；后代继承最近祖先显式指定的画刷，未指定时使用主题系列色。 |

构造方式：`new ChartHierarchyNode("客户端", 45, "client")`。父节点不会与子节点重复计权；零值、负值、NaN 和无穷大的叶节点不占面积或角度。父节点包含子项但没有有效正权重时，该分支不显示。权重使用缩放后的对数汇总，避免多个极大有限值求和溢出后破坏面积比例。

## 层级导航与选择

| 额外属性 / API | 说明与简短示例 |
| --- | --- |
| `Root` | 可双向绑定的 `ChartHierarchyNode?`；`null` 展示整个森林，分支节点展示其子树。先设置 `ItemsSource`，再设置 `Root`。`Root="{CompiledBinding CurrentBranch, Mode=TwoWay}"`。 |
| `SelectedNode` | 只读，当前选中的原始节点；没有选择时为 `null`。`var key = chart.SelectedNode?.Key;`。 |
| `CanDrillUp` | 只读，当前 `Root` 非空时为 `true`。`backButton.IsEnabled = chart.CanDrillUp;`。 |
| `HasInvalidHierarchy` | 只读，检测到循环、同一节点引用重复出现或重复 `Key` 时为 `true`。 |
| `DrillDown(node)` | 返回是否进入可达且具有有效正权重子项的分支；叶节点返回 `false`。`chart.DrillDown(chart.SelectedNode!)`，调用前先检查非空。 |
| `DrillUp()` | 返回是否返回上一层；已在完整森林时返回 `false`。 |
| `ResetRoot()` | 返回完整森林。 |
| `RefreshData()` | 手动刷新普通 `IList` 的结构和权重；可观察集合及节点属性变化自动刷新。 |
| `SelectionChanged` | 继承的选择事件，`e.Selection?.Point` 为原始 `ChartHierarchyNode`。 |

双击可命中的分支可进入下一层；Treemap 中可双击分支标题或露出的父区域。返回入口由应用使用 `DrillUp()` / `ResetRoot()` 提供。键盘沿当前可显示的扁平节点导航；本版不提供笛卡尔坐标图的平移、缩放和共享 X 提示。

增删、移动、Reset、Replace 或切换 `ItemsSource` 后，先按对象引用保留选择；对象已替换时，仅在替换前后 `Key` 均唯一的情况下按 `Key` 恢复。重复 `Key` 不合并节点。已移除、被隐藏或失去有效权重的节点会清除选择。原 `Root` 被移除或变成叶节点时自动返回完整森林，`CanDrillUp` 随之复位，不按 `Key` 自动跳入另一个根。

树遍历和布局均使用显式栈，避免深层树递归栈溢出。循环和共享节点按源顺序仅接受第一次出现，后续边跳过并标记 `HasInvalidHierarchy`，不会无限遍历。修复数据后再次更新集合或调用 `RefreshData()` 可清除标记。

## TreemapChart 额外设置

| 属性 | 默认值 | 说明 |
| --- | --- | --- |
| `Gap` | `3` | 相邻矩形之间的逻辑像素留白。`chart.Gap = 4;`。 |
| `HeaderHeight` | `20` | 父区域标题的最大高度；空间不足时缩小，关闭 `ShowDataLabels` 后不保留标题高度。 |

使用稳定的 slice-and-dice 布局，每深入一层交替切分方向，保留源数据顺序。本版不是 squarified 算法，细长数据分布可能形成细长矩形。权重决定留白和标题预留之前的分配面积；标题与间距占用父节点内部分配空间，不增加业务权重。只绘制实际能容纳的文字，完整名称可通过提示或选择取得。

## SunburstChart 额外设置

| 属性 | 默认值 | 说明 |
| --- | --- | --- |
| `RingGap` | `2` | 环之间和相邻扇区之间的逻辑像素间距。 |
| `StartAngle` | `-90` | 起始角度，单位为度，顺时针布局；`-90` 从顶部开始。 |
| `InnerRadius` | `0.18` | 继承属性，在 Sunburst 中默认较小；为外半径的比例，范围 `[0,1)`。 |

子扇区只划分其父节点对应的角度，环数按当前可见子树最大深度确定。命中同时检查环半径和扇区角度，中心空洞、环间隙、兄弟间隙及极小扇区外部不会命中。文字矩形四个角都位于当前扇区内时才绘制，避免跨环或跨扇区。

## 更新、缓存与导出

单个叶节点 `Y` 变化只更新其祖先权重缓存，保留树索引、扁平投影及现有订阅；实际布局在下一次绘制前重建。结构变化重新建立先序索引，同时只为新增节点、集合接入订阅，并移除已离开的订阅；共享集合仅订阅一次。卸载后释放事件，重载时重新同步。

每次布局准备缓存矩形 / 扇区、文字、画刷、画笔和命中区域；稳定绘制不重新枚举业务树、不创建逐节点控件。动画复用通用 `AnimationStyle`、`IsAnimationEnabled` 和运动开关，超过软预算时直接呈现目标状态。导出创建独立的有效树快照，使用当前根和外观参数，关闭动画后渲染目标状态，不修改运行中控件。

节点文字另有最多 16,384 项的测量缓存；权重变化后可复用相同节点、文字、字体、文化、方向和文字画刷的 `FormattedText`，结构变化时移除已离开节点，卸载时释放。字体、主题、文字颜色等上下文变化会清空缓存。矩形仍按实际文字尺寸判断，Sunburst 仍使用文字四角的精确扇区检查，不通过隐藏额外节点换取性能。

当文字仍使用主题默认 `One.Chart.Text` 时，不透明纯色填充会自动选择对比度更高的黑色或白色文字；已知不透明背景上的半透明纯色按合成色判断。渐变或无法确定底色时保留现有文字画刷。显式设置或绑定 `Foreground`，以及自定义样式中的非默认文字画刷，会保留使用者选择；复杂透明叠加时也可显式指定，例如 `Foreground="White"`。自动选择复用共享画刷，不为每个节点创建新画刷。

自动文字对比度仅观察实际使用的唯一可变纯色填充和背景画刷；原地修改其 `Color` / `Opacity` 时重新计算文字颜色，替换后解除旧画刷事件，卸载时全部释放。导出快照保留原控件的自动或显式文字策略，且不会订阅外部画刷。普通离屏控件或修改当前线程文化后，调用 `RefreshData()` 再读取布局。极深树的祖先画刷解析仍可进一步优化，本轮没有改变其继承规则。

```csharp
chart.DrillDown(Departments[0]);
chart.ExportPng("department.png", new Avalonia.Size(1200, 800), dpi: 144);
chart.ResetRoot();
```

本轮仅验证这两个新增层级图及其直接依赖的图表机制；验证数据和未覆盖的桌面平台范围以对应专项记录为准。
