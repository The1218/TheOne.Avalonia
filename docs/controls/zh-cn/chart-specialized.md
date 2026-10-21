# Heatmap、Radar 与 Funnel

**简体中文** · [English](../en/chart-specialized.md) · [图表入口](charts.md)

三种控件基于 `SpecializedChartBase`，使用 Avalonia 原生直接绘制，没有 WebView 或 ECharts 运行时依赖。本轮按[原方案](../../plans/chart-echarts-gap-and-optimization.md)第一批落地；语义参考 [ECharts Heatmap](https://echarts.apache.org/en/llms-documents/option-parts/option.series-heatmap.md)、[Radar](https://echarts.apache.org/en/llms-documents/option-parts/option.series-radar.md) 和 [Funnel](https://echarts.apache.org/en/llms-documents/option-parts/option.series-funnel.md)，公共 API 保持 Avalonia 强类型与编译绑定方式。

加载 `OneTheme` 后，每种控件都能使用 Light / Dark、`small / normal / large` 密度、`SeriesSource`、`SelectedSeriesIndex` / `SelectedIndex` 双向绑定、键盘选择、图例、单点提示和 `ExportPng`。动画见[渲染说明](chart-rendering.md)。首版没有地图／日历坐标、Cartesian 视口缩放、双 Y 轴、堆积、共享 X 提示或抽样；这些属性不会改变三种新图表的坐标含义。

<!-- visual-guide:start -->
## 外观与场景图示

[配图口径与控件图鉴](visual-index.md)；下图是当前源码的真实渲染，绑定与交互以本页用法说明为准。

### HeatmapChart

![HeatmapChart — X/Y 决定网格位置，Value 独立决定色阶；空白格为缺口。](../images/HeatmapChart-light.png)

X/Y 决定网格位置，Value 独立决定色阶；空白格为缺口。

[查看 Dark 对照](../images/HeatmapChart-dark.png)

### RadarChart

![RadarChart — 每维按自己的上下限归一化，原始值仍保留在数据模型中。](../images/RadarChart-light.png)

每维按自己的上下限归一化，原始值仍保留在数据模型中。

[查看 Dark 对照](../images/RadarChart-dark.png)

### FunnelChart

![FunnelChart — 各阶段宽度表达数值，默认保持业务阶段顺序。](../images/FunnelChart-light.png)

各阶段宽度表达数值，默认保持业务阶段顺序。

[查看 Dark 对照](../images/FunnelChart-dark.png)

<!-- visual-guide:end -->

## HeatmapChart

输入 `ChartSeries.ItemsSource` 中的 `HeatmapPoint`，普通 `ChartPoint` 不生成热力格。

| 额外属性 | 类型／默认值 | 含义 |
| --- | --- | --- |
| `CellWidth` | `double`，1 | 单元格在 X 数据坐标中的宽度，有限且大于 0 |
| `CellHeight` | `double`，1 | 单元格在 Y 数据坐标中的高度，有限且大于 0 |
| `CellGap` | `double`，2 | 单元格之间的视觉留白，DIP，有限且非负；过大时按格子大小限制 |
| `ValueMinimum` | `double`，NaN | 色阶下限，NaN 表示从可见有效 Value 自动计算 |
| `ValueMaximum` | `double`，NaN | 色阶上限，NaN 自动；上下限颠倒时按较小值／较大值处理 |
| `LowColor` | `Color?`，null | 最小值颜色；null 使用主题派生色 |
| `MidColor` | `Color?`，null | 正负混合范围中零值的颜色；null 使用背景中性色 |
| `HighColor` | `Color?`，null | 最大值颜色；null 使用主题主系列色 |

热力图使用所有可见格共享的统一色阶；使用 LowColor／MidColor／HighColor 配色，不把每个系列的 Brush 当作独立色阶。

`HeatmapPoint` 继承 `ChartPoint`，增加可观察的 `double Value`。`X` / `Y` 指定格子中心，`Value` 是独立颜色维度，`Label` 是可选提示名称。构造方式：

```csharp
new HeatmapPoint(x: 2, y: 3, value: -25, label: "工作日 · 时段 3");
```

有限坐标、非有限 Value 的格子不绘制，但仍保留其坐标对范围的贡献，缺失值不会压缩稀疏坐标。范围跨零时使用以零为中点的双色渐变；单侧范围线性映射。绘制将颜色量化到 256 档，互不重叠的格子可按颜色合并路径，重叠、透明色与标签保持源绘制顺序；提示与选中数据仍保留原始 Value。重叠格子的命中优先最后绘制的格子。无单独色标控件，可通过固定 ValueMinimum / ValueMaximum 保持跨图比较含义。

```csharp
public ObservableCollection<ChartSeries> LoadSeries { get; } =
[
    new()
    {
        Name = "负载偏差",
        ItemsSource = new ObservableCollection<ChartPoint>
        {
            new HeatmapPoint(0, 0, -25), new HeatmapPoint(1, 0, 80),
            new HeatmapPoint(0, 1, 15), new HeatmapPoint(1, 1, double.NaN)
        }
    }
];
```

视图根元素声明实际 ViewModel 的 `x:DataType` 并启用编译绑定：

```xml
<one:HeatmapChart SeriesSource="{CompiledBinding LoadSeries}"
                  ValueMinimum="-100" ValueMaximum="100"
                  CellGap="2" ShowDataLabels="True"
                  AnimationStyle="Fade" MinHeight="260" />
```

数据标签只在格内能完整容纳时显示，且有效格超过 5000 时省略格内数值，避免大量文本排版；坐标刻度仍保留。

## RadarChart

| 额外属性 | 类型／默认值 | 含义 |
| --- | --- | --- |
| `Axes` | `IList<RadarAxis>?`，每实例一个可观察空集合 | 维度及各维数值范围，至少三维 |
| `GridLevels` | `int`，5 | 多边形网格层数，1–20 |
| `FillOpacity` | `double`，0.16 | 闭合多边形的填充透明度，0–1；0 只画轮廓 |

`RadarAxis` 是可观察的 AvaloniaObject，公开 `Name`（默认 Axis）、`Minimum`（默认 0）、`Maximum`（默认 100）；数值须有限。每维的 Maximum 必须大于 Minimum 才有效。`ChartSeries.ItemsSource` 按集合位置与 Axes 对应，点 Y 是原始值，X 需有限但不参与维度排序。超出 Axes 数量的点不绘制、不选中；轴范围非法或数据缺失时，只连接相邻有效维度，不跨缺口填充闭合区域。超范围值在轴端裁剪，提示保留原值。

```csharp
public ObservableCollection<RadarAxis> Dimensions { get; } =
[
    new() { Name = "质量", Maximum = 100 },
    new() { Name = "吞吐", Maximum = 200 },
    new() { Name = "覆盖", Maximum = 100 }
];
public ObservableCollection<ChartSeries> Capabilities { get; } =
[
    new() { Name = "当前", ItemsSource = new ObservableCollection<ChartPoint>
    { new(0, 85), new(1, 140), new(2, 72) } },
    new() { Name = "目标", ItemsSource = new ObservableCollection<ChartPoint>
    { new(0, 95), new(1, 180), new(2, 90) } }
];
```

```xml
<one:RadarChart Axes="{CompiledBinding Dimensions}"
                SeriesSource="{CompiledBinding Capabilities}"
                GridLevels="5" FillOpacity="0.16"
                ShowDataLabels="True" AnimationStyle="Reveal" />
```

可观察 Axes 的增删、Reset、Replace、来源替换和每个轴的属性变化会刷新图形；卸载释放订阅，重新挂载恢复。普通 IList 原地变化后调用共享 `RefreshData()`，让图表重新读取数据及维度。

## FunnelChart

| 额外属性 | 类型／默认值 | 含义 |
| --- | --- | --- |
| `SortMode` | `FunnelSortMode`，None | None 保留业务顺序；Ascending／Descending 显式按 Y 排序，同值保持原顺序 |
| `Gap` | `double`，4 | 阶段之间的垂直留白，DIP，有限且非负；过大时自适应限制 |
| `MinimumWidthRatio` | `double`，0 | 相对于最宽阶段的最小宽度，0–1；默认保留真实比例，非零是显式视觉夸大 |

使用 `ChartPoint.Y` 表示阶段值，`Label` 表示名称；负 Y 或非有限 X / Y 跳过，负 X 可以使用，零 Y 在默认配置下不获得正面积。多个可见系列并排展示，每个系列按自身最大值缩放，不代表跨系列绝对宽度可直接比较。排序只改变绘制位置，`SelectedIndex` 和提示保持源集合索引。

```csharp
public ObservableCollection<ChartSeries> Conversions { get; } =
[
    new() { Name = "转化", ItemsSource = new ObservableCollection<ChartPoint>
    { new(0, 1200, "访问"), new(1, 800, "注册"), new(2, 420, "试用"), new(3, 180, "成交") } }
];
```

```xml
<one:FunnelChart SeriesSource="{CompiledBinding Conversions}"
                 SortMode="None" MinimumWidthRatio="0" Gap="4"
                 ShowDataLabels="True" AnimationStyle="Fade" />
```

每阶段上边宽度对应当前值，下边过渡至下一阶段值；末段收至尖端。标签仅在完整容纳时绘制；命中按真实梯形而非外接矩形判断。转化率可由业务层根据原值计算，首版没有隐式的排序或转化率 API。

## 数据更新、交互与导出

- `ChartSeries`、`ChartPoint` 与可观察集合复用现有引用计数订阅；修改 `HeatmapPoint.Value`、轴范围或阶段值不会重建全部数据订阅。
- 普通 IList 变化后使用 `RefreshData()`。Radar 的普通 Axes 集合在这次刷新时同步读取；重新挂载仍会协调订阅。
- 设置 `SelectedIndex`／`SelectedSeriesIndex` 或使用指针／方向键选择，读取 `Selection`；Heatmap 的 `Selection.Point` 可以转换为 `HeatmapPoint` 取得 Value。选择改变不重建色阶、布局或全部几何。
- 原始索引 API 沿用现有规则；来源替换或 Reset 后应由业务层按需要恢复业务主键对应的选择，不承诺稳定 Key 下钻协议。
- 动画 `Auto` 首次为 Reveal，数据更新为 Fade。`None`、关闭 Motion 或动画开关即时呈现。超过 6000 个绘制标记时停用动画。
- `ExportPng(Stream, Size?, double, bool)` 使用相同具体类型及额外属性生成完整目标快照，不等待动画结束、不修改实时选择或动画状态。

各控件在 PreviewHost 的“图表控件”中有独立页面，使用 Description 展示额外属性与短代码，并提供数据更新、动画类型与样式变体。专项范围与性能测量见[本轮验证记录](chart-echarts-performance-20261020.md)。
