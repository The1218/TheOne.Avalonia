# HierarchyChartBase

[English](../en/TheOne.Avalonia.Controls.HierarchyChartBase.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.HierarchyChartBase.md) · [API index](README.md)

`TheOne.Avalonia.Controls.HierarchyChartBase` · Class · `TheOne.Avalonia.Controls.SpecializedChartBase`

[继承的额外成员: SpecializedChartBase](TheOne.Avalonia.Controls.SpecializedChartBase.md)

Observable hierarchy projection shared by native treemap and sunburst charts.

[Source / 声明](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L14)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanDrillUp`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L67) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasInvalidHierarchy`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L69) | `bool` | 只读; OneWay | `computed / state` | True when duplicate keys, repeated nodes or cycles were encountered. |
| [`ItemsSource`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L63) | `System.Collections.Generic.IList<TheOne.Avalonia.Controls.ChartHierarchyNode>?` | 可写; OneWay | `null` | The authoritative forest. Do not assign the inherited SeriesSource projection. |
| [`Root`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L65) | `TheOne.Avalonia.Controls.ChartHierarchyNode?` | 可写; TwoWay | `null` | The displayed branch; null displays the complete forest. Unreachable roots reset to null. |
| [`SelectedNode`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L66) | `TheOne.Avalonia.Controls.ChartHierarchyNode?` | 只读; OneWay | `computed / state` | 当前选中的节点模型；空值清除节点选择。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`bool HierarchyChartBase.DrillDown(ChartHierarchyNode node)`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L72) | Show a reachable branch's children. Leaves and branches without positive weights are rejected. |
| [`bool HierarchyChartBase.DrillUp()`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L80) | 签名列明参数类型、可选初值及返回类型；具体操作与异常边界见所属控件用法和链接处实现。 |
| [`void HierarchyChartBase.ResetRoot()`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L87) | 签名列明参数类型、可选初值及返回类型；具体操作与异常边界见所属控件用法和链接处实现。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly StyledProperty<IList<ChartHierarchyNode>?> HierarchyChartBase.ItemsSourceProperty`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L16) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly StyledProperty<ChartHierarchyNode?> HierarchyChartBase.RootProperty`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L18) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<HierarchyChartBase, ChartHierarchyNode?> HierarchyChartBase.SelectedNodeProperty`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L21) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<HierarchyChartBase, bool> HierarchyChartBase.CanDrillUpProperty`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L23) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly DirectProperty<HierarchyChartBase, bool> HierarchyChartBase.HasInvalidHierarchyProperty`](../../../TheOne.Avalonia/Controls/HierarchyChartBase.cs#L25) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 样式与密度

尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。

| 类名 | 选择器 | Setter／资源 |
| --- | --- | --- |
| `animation-fade` | [`^.animation-fade`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L27) | ^.animation-fade => AnimationStyle=Fade |
| `animation-none` | [`^.animation-none`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L33) | ^.animation-none => AnimationStyle=None |
| `animation-reveal` | [`^.animation-reveal`](../../../TheOne.Avalonia/Themes/Controls/Chart.axaml#L30) | ^.animation-reveal => AnimationStyle=Reveal |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.ChartHierarchyNode`](TheOne.Avalonia.Controls.ChartHierarchyNode.md)
