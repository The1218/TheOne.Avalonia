# Chart 对照 Apache ECharts 的缺口与优化方案

评估日期：2026-10-19。源码基线：`d2b8169` / `1.0.0-preview.7`。

2026-10-20 实施进度：第一批 `HeatmapChart`、`RadarChart`、`FunnelChart` 已实现；第二批增加 `BoxPlotChart`、`CandlestickChart`、`TreemapChart`、`SunburstChart`，共 21 种 ChartBase 图表复用 Auto / None / Fade / Reveal、主题和绘制缓存。第二批提供预计算统计模型、数值视口、稳定层级 Key / Root 下钻，预览按真实可视区创建样式、动画和代码区。详见[首批图表](../controls/zh-cn/chart-specialized.md)、[统计图](../controls/zh-cn/chart-statistical.md)、[层级图](../controls/zh-cn/chart-hierarchy.md)及[第二批性能／专项记录](../controls/zh-cn/chart-next-batch-performance-20261020.md)。下文保留原评估基线；交易日历、自动分位统计、squarified 布局、地理／网络等后续能力仍是计划，不表示本轮已实现。

本文是源码审查与官方资料对照方案，不是性能实测报告，不表示已排期或已实现。未修改图表实现、未执行全量回归、未重新运行历史性能专项。优先级按桌面业务应用、监控看板、工作流与报表场景判断。

## 1. 建议结论

建议先完善公共能力与更新管线，再分批增加图种。第一批公共能力是类型化坐标轴、柱线混合、标注与视觉映射；第一批独立图种是二维 Heatmap、Radar、Funnel。实时数据的性能任务优先做集合增量和批量通知，再处理属性失效范围、单点更新分配以及 Pie/Doughnut 几何复用。

Scatter 的预算 2000 尾延迟单独立项，先取得真实后端调用栈与线程调度证据，再选择绘制缓存方案。不能把降低抽样预算作为原规则不变情况下的性能改善，也不能把减少托管分配等同于改善光栅 p99。

暂不追求完全复刻 ECharts：地理、通用 Graph、复杂多坐标系、自定义系列生态与 3D 的研发维护成本明显高于常用业务图表，需由明确场景驱动。

## 2. 对照口径与现有基础

ECharts 的面积、横向、堆积等主要是系列配置；本库把其中不少变体公开为独立控件。因此“14 个控件”不等于覆盖 14 个 ECharts 图种。本库目前主要覆盖五个系列家族：

| 家族 | 已有控件 |
| --- | --- |
| 折线/面积 | LineChart、AreaChart、StackedAreaChart、PercentStackedAreaChart |
| 柱/条形 | BarChart、HorizontalBarChart、StackedBarChart、StackedHorizontalBarChart、PercentStackedBarChart、PercentStackedHorizontalBarChart |
| 散点 | ScatterChart |
| 饼/环形 | PieChart、DoughnutChart |
| 仪表 | GaugeChart，已有多种仪表样式 |

已有公共能力不应重新列为缺失：双 Y 轴、滚轮/键盘缩放与平移、Viewport、共享提示、十字准线、图例交互、键盘点选、基础 AutomationPeer、平滑折线/普通面积、数据标签避让、MinMax 抽样、选择保留、动画、PNG 导出、主题与三档尺寸。

已有性能基础：引用计数增量订阅、范围聚合树、堆积列更新、片段抽样索引、坐标帧/线面几何缓存、Scatter 批量自定义绘制与选择快照复用。后续在这些能力上扩展，不能回退到每个点一个 Control。

额外区分：RelationGraph 是单根树形关系控件，文档明确没有内置折叠/缩放/虚拟化；它只部分覆盖 Tree 图。WorkflowEditor 是流程编辑器，不是通用 Graph。GanttChart 是业务扩展，不将其计入 ECharts 核心系列的一一对照。

对照来源：[ECharts 系列与组件分类](https://echarts.apache.org/zh/cheat-sheet.html)、[官方系列导出清单](https://github.com/apache/echarts/blob/master/src/export/charts.ts)。官网速查页未列出全部 v6 新项，另以 [ECharts 6 新特性](https://echarts.apache.org/handbook/zh/basics/release-note/v6-feature/) 补充 Chord、矩阵坐标系等内容。

## 3. 缺失图种与引入建议

成本为相对研发/验证成本，包含模型、命中、交互、主题、导出与文档，不是工期承诺。

| 批次 | 图种 | 当前缺口与首版边界 | 成本 |
| --- | --- | --- | --- |
| 第一批 | HeatmapChart 热力图 | 先做二维网格与连续/分段色标；稀疏格、缺失值、负值、命中；日历热力和地理热力后续单列 | 中 |
| 第一批 | RadarChart 雷达图 | 多维轴、每维范围、系列叠加、填充/轮廓、标签避让；不把所有维度默认归一成同一含义 | 中 |
| 第一批 | FunnelChart 漏斗图 | 阶段值、转化率、排序策略、标签布局；默认保留业务顺序，不能擅自按值重排 | 中低 |
| 第二批 | TreemapChart 矩形树图 | 层级面积编码、稳定 Key、布局、下钻与返回；叶子极小面积的命中规则 | 中高 |
| 第二批 | SunburstChart 旭日图 | 复用层级模型，增加径向层级布局、路径和下钻；不是 Doughnut 简单多套一层 | 中高 |
| 第二批 | BoxPlotChart 箱线图 | 五数概括、离群值、分位算法约定；优先接收预计算统计，原始样本统计作为可选工具 | 中 |
| 第二批/需求驱动 | SankeyChart 桑基图 | 带权边、流量布局、交叉控制；首版明确是否只支持 DAG，反馈环不静默丢弃 | 高 |
| 行业驱动 | CandlestickChart K 线 | OHLC 专用模型、时间轴、涨跌样式、成交量组合；依赖坐标轴和混合系列 | 中高 |
| 按需完善 | Tree | RelationGraph 增加折叠、缩放与裁剪；小型内容树和大型统计树需区分布局策略 | 中高 |
| 后续 | GraphChart 关系网络 | 多根、多对多、环、有向边、力导向/环形布局；保留 RelationGraph 单根树契约 | 高 |
| 后续 | Map / Lines | GeoJSON、投影、区域命中、轨迹、资源来源与许可；Lines 是连线路径系列，不是普通折线图 | 高 |
| 后续 | Parallel 平行坐标 | 多维轴、轴区间过滤、密集折线与刷选联动 | 高 |
| 后续 | ThemeRiver 主题河流 | 连续时间与流式堆积布局；不能直接当作现有堆积面积图 | 中高 |
| 后续 | Chord 和弦图 | ECharts 6 已加入，适合带权关系流量；有向/无向与重复边规则必须明确 | 高 |
| 低优先级 | PictorialBar / EffectScatter | 象形柱、涟漪散点偏展示效果，收益通常低于分析和数据选择能力 | 中 |
| 基础设施，后置 | Custom series | 消费者扩展绘制、布局、命中、提示和导出；待内部有多个新增图种后再稳定扩展协议 | 高 |

不要机械增加独立控件：

- 气泡图：先扩展 Scatter 的逐点大小和颜色映射。需要定义按面积还是半径映射，默认建议按面积。
- 直方图：柱图加分箱统计；明确区间闭开、边界、异常值、分箱宽度，不只是改标签。
- 瀑布图：柱图增加起止区间、累计与合计语义、连接线；避免用透明堆积的技巧替代正式模型。
- 范围柱/范围带/误差线：建立 Low/High 模型，明确命中和 Tooltip；不把上下界塞进 Label。
- 阶梯折线：作为 Line/Area 的路径插值选项，与已有平滑规则互斥。
- 玫瑰图：可作为 Pie 的半径/面积映射模式；必须解释面积语义。
- 抖动/蜂群：作为 Scatter 布局扩展，保留真实数值维度，偏移仅用于显示，Tooltip 返回原值。

ECharts 6 的小提琴、轮廓、阶段、分段环形、范围柱与范围折线由官方自定义系列项目提供，不应全部算作核心内置系列。3D/GL 另属扩展技术范围。参考：[v6 新特性及官方自定义系列](https://echarts.apache.org/handbook/zh/basics/release-note/v6-feature/)。

## 4. 公共能力差距，比单纯新增图种更优先

| 能力 | 本库现状 | 建议范围 | 优先级 |
| --- | --- | --- | --- |
| 轴类型 | 数值/分类逻辑由图种决定，有数值刻度；无统一时间/对数轴模型 | AxisKind 与尺度策略；时间刻度、时区/夏令时、对数合法域、反向轴；先时间轴再对数轴 | P0 |
| 混合系列 | Chart.Kind 决定整个图的类型，ChartSeries 没有系列绘制类型 | 先柱+折线+面积、共享 X、双 Y、统一图例和命中；保留旧控件 API | P0 |
| 标注 | 未发现统一标记点/线/区域模型 | 阈值线、目标区间、事件标记；坐标锚定、Tooltip 与导出一致 | P0 |
| 视觉映射 | 系列 Brush、六色主题循环；ChartPoint 只有 X/Y/Label | 强类型逐点颜色/大小/透明度映射，连续/分段色标及过滤；先满足 Bubble 和 Heatmap | P1 |
| 数据映射 | 需提供 IList<ChartPoint> | 强类型 getter/适配器、稳定 Key、共享只读数据；保留现有 API，避免字符串反射 | P1 |
| 范围导航 | 已有 Viewport 和缩放平移 | 增加缩略范围导航器与数据单位的可绑定范围；不是重做滚轮缩放 | P1 |
| 刷选与多选 | 公开选择是单点 Selection | 矩形刷选、SelectedKeys、筛选/集合更新后的选择规则；套索后置 | P1 |
| 跨图联动 | 宿主可订阅 ViewportChanged 手动同步 | 提供数据范围/指针/选择联动组，带来源标识防循环；不同数据范围不能只拷贝归一化 Rect | P1 |
| 分组/变换 | 宿主自行构造系列 | 可复用过滤、排序、分箱与统计工具，记录输入版本；不用第一版就实现全套声明式管线 | P2 |
| 下钻 | 无统一层级图协议 | Key、路径、返回和加载状态；与 Treemap/Sunburst 同步设计 | P1，随层级图 |
| 大规模/流式 | 同步集合事件、抽样和缓存；无公开追加批次/更新事务 | 批量更新、末端追加、环形窗口；必要时分帧准备可取消快照 | P0/P1 |
| 导出 | 已有 PNG；未发现 Chart SVG 公共接口 | 先统一渲染快照，再评估 SVG/报表矢量输出；避免直接复制交互中的临时帧 | P2 |
| 无障碍 | 已有键盘导航与 AutomationPeer | 补图表摘要、数据表替代视图、纹理/线型区分与本地化；不能说目前完全没有无障碍 | P1 |
| 多坐标编排 | 单图坐标区域、双 Y 轴 | 多 grid、极坐标、日历、矩阵按需求引入，不与第一轮轴类型重构捆绑 | P2/P3 |

ECharts 提供 [轴与混合图](https://echarts.apache.org/handbook/zh/concepts/axis/)、[dataset/encode](https://echarts.apache.org/handbook/zh/concepts/dataset/)、[视觉映射](https://echarts.apache.org/handbook/zh/concepts/visual-map/)、[数据转换](https://echarts.apache.org/handbook/zh/concepts/data-transform/)；标注、DataZoom、Brush 等组件见[官方速查](https://echarts.apache.org/zh/cheat-sheet.html)。本库应吸收其能力分层，API 仍按 Avalonia 强类型和绑定习惯设计。

## 5. 已有图表应完善的行为

| 对象 | 保留已有优势 | 下一步完善 |
| --- | --- | --- |
| Line / Area | 数值 X、平滑、缺口、抽样、共享提示 | 时间轴、阶梯路径、范围带、事件/阈值标注；刷新时保留用户范围 |
| Bar / Stacked / Percent | 横向、正负堆积、双轴、标签和百分比 | 分组堆积、系列 StackKey、区间柱/瀑布、长分类标签；异步不同系列按稳定分类键对齐的可选模式 |
| Scatter | 原始点命中、选择保留、抽样索引、批量绘制 | 逐点大小/颜色、框选与过滤、稳定 Key、密集点布局；单独解决真实后端尾延迟 |
| Pie / Doughnut | 已有选中/悬浮反馈、标签与比例 | 扇区几何缓存、外部标签引导线与避让、小扇区聚合规则、玫瑰模式 |
| Gauge | 多样式、分段、刻度、动画、PNG | 模板间公共配置一致性、阈值状态语义、低频外观失效；暂无证据支持优先再加更多仪表皮肤 |
| RelationGraph | 模板化节点与原生交互 | 折叠、缩放、视口裁剪；有状态节点回收必须保留编辑/焦点 |

现有堆积按索引对齐等规则应作为兼容模式保留，新模式显式选择。新增能力不可改变缺口、极值、负值、百分比与选中点含义。

## 6. 从源码确认的优化入口

下列“确认”指代码路径存在，不表示已测得该路径占总耗时的百分比。

### 6.1 集合变化后的缓存增量（优先）

证据：[ChartBase.Subscriptions.cs](../../TheOne.Avalonia/Controls/ChartBase.Subscriptions.cs) 的 `DataChanged()` 清空 `_cartesianData` 和 `_percentages`；[ChartBase.DataCache.cs](../../TheOne.Avalonia/Controls/ChartBase.DataCache.cs) 构造时遍历可见系列所有点并重算列聚合。

已解决的是订阅增量，不是所有结构变化的布局/数据增量。新增一个点或替换一个点仍可能触发整份准备工作。

实施：

1. 为 Replace、尾部 Add、尾部 Remove 建立专门路径；先不承诺任意位置插入常数时间。
2. 保留无关系列聚合和采样索引，只更新受影响列、尾段与可见范围。
3. 增加更新事务，批量输入时只合并准备/刷新工作，原始数据与事件语义明确。
4. Reset、来源切换、未知集合事件保持安全回退；清除旧订阅与过期快照。
5. 原始索引和稳定 Key 分工明确：旧索引 API 保留，新 Key API 为可选扩展。

验收：固定范围下单次尾增不枚举无关系列；共享点/共享集合订阅正确；Range Add/Replace、Reset、ItemsSource 切换与选中状态专项通过。

### 6.2 属性变化的失效范围

证据：`OnDataObjectChanged()` 对非 X/Y、非可见性等专门分支走 `MarkGeometryDirty()`。名称、画刷、格式等属性因而可能进入较大的刷新范围。

实施：将“数据/范围、坐标布局、曲线几何、文字/图例、画刷、交互叠层”失效分开处理。颜色通常不影响几何；标签文本若改变测量结果才影响相关布局。主题切换复用不依赖颜色的数据聚合和坐标。

验收：单改系列颜色不重扫原始点；名称/单位变化的 Tooltip 与图例更新；字号/长标签改变仍能正确触发布局；不能一概禁止文字导致的几何更新。

### 6.3 单点更新仍有临时集合和局部全重建

证据：[ChartBase.PointUpdates.cs](../../TheOne.Avalonia/Controls/ChartBase.PointUpdates.cs) 每次快路径仍分配 Distinct 数组、HashSet、List、Dictionary；更新片段后清空 LineGeometry/FillGeometry/ScatterMarkers，并刷新标签。X、极值范围、拓扑或动画条件变化仍有回退。

实施：先为“单一所有者、单个点”增加低分配路径；复用有界工作区并防重入；只刷新受影响标签。随后评估长线段的几何分块，缓存边界保留连接邻点及平滑所需控制点。多次通知合并成最终状态后再准备图形。

边界：极值改变会影响映射，允许更广更新；X 重排或缺口变化不能套用原槽位。不能为了减少扫描破坏软预算和极值保留。

### 6.4 Pie / Doughnut 绘制几何复用

证据：[ChartBase.Radial.cs](../../TheOne.Avalonia/Controls/ChartBase.Radial.cs) 的 `DrawRadialSector()` 在绘制时生成 AnnularSector，环形场景另构造 CombinedGeometry/EllipseGeometry。

实施：按布局版本缓存静态扇区几何，按动画/选中位移区分缓存有效性；复用内部孔洞资源与静态文字。保留原填充、覆盖顺序、抬升和命中规则。

这是源码可确认的分配点，但尚未测量实际收益；先用少量/较多扇区、悬浮切换与动画专项建立基线。

### 6.5 Scatter 原生光栅 p99：证据先行

历史文档记录了三系列各 100,000 点、每系列预算 2000 的未入样点选择场景：总准备点 3,991，完整周期 p99 为 **19.0080 → 19.7188 ms**，分配显著下降但尾延迟未改善。来源：[chart-rendering.md](../controls/zh-cn/chart-rendering.md#scatter-selection-p99)。这是过去的 macOS ARM64 Skia headless 数据，不是本轮实测，不是桌面屏幕呈现延迟。

当前 [ChartBase.ScatterDrawing.cs](../../TheOne.Avalonia/Controls/ChartBase.ScatterDrawing.cs) 已复用快照、画刷和自定义绘制操作，不能重新宣称问题来自“逐点 new Pen”。旧记录没有证明剩余慢帧来自重复绘制或托管分配。

实施顺序：

1. 用相同数据、预算、画布尺寸、DPI、主题、交互脚本，采集真实桌面后端 CPU 原生栈与线程调度，关联慢帧。
2. 区分 Skia 光栅、合成/提交、UI 等待、系统抢占；缺少符号或平台工具时报告证据缺口。
3. 若主要是静态背景点重复光栅，再试验稳定数据层缓存和选择叠层。
4. 特别处理半透明点、原始点次序、重叠、插入的选中样本与描边；简单把选中点画到最上面会改变原行为，不能直接采用。
5. 任何位图缓存都设 DPI/主题/视口失效与内存上限；PNG 导出使用正确分辨率的完整快照。

接受条件：原采样/命中/选择规则不变，真实后端 p99 多轮稳定改善，无像素偏差和缓存内存失控。若无法满足，则保留当前正确路径，不以更低采样量替换对照组。

## 7. 底层架构建议

采用逐步抽取，不一次重写 ChartBase：

```text
强类型数据源 / 稳定 Key / 更新事务
                 ↓
数据快照、范围聚合、过滤/堆积
                 ↓
坐标尺度 / 系列布局 / 抽样与空间命中
                 ↓
不可变绘制快照 → 图形层 / 文字层 / 交互层
                 ↓
Avalonia 直接绘制、原生优化路径、导出
```

- 保留现有 ChartPoint、ChartSeries 和具体控件；新增数据适配层和混合容器可先作为可选 API。
- 新类型使用正确模型：RadarDimension、HeatmapCell、HierarchyNode、WeightedEdge、OhlcPoint、BoxPlotSummary。名称为方案暂定，不表示公共 API 已存在。
- 各图种只管理自身布局和绘制，不把所有新类型继续堆入 ChartKind 的分支。
- 耗时布局可在不可变快照上后台计算，带取消/版本号；AvaloniaObject 的读取/修改仍遵守 UI 线程约束，旧任务不得覆盖新数据。
- ECharts 的 dataset、渐进绘制、可替换 Canvas/SVG 值得借鉴，但不等于给 Avalonia 图表增加 WebView。原生直接绘制继续作为默认架构。

ECharts 使用 Canvas/SVG，由 ZRender 抽象；不同后端有不同内存和绘制特性，见[官方后端说明](https://echarts.apache.org/handbook/zh/best-practices/canvas-vs-svg/)。其大数据、流式能力具有图种和配置边界，不能依据官网展示规模声称本库慢多少倍，或承诺任意系列都能恒定帧率。[官方特性](https://echarts.apache.org/zh/feature.html)

## 8. 推荐实施批次

| 阶段 | 主要工作 | 完成标准 |
| --- | --- | --- |
| A：更新成本 | 集合 Replace/尾增量、更新事务、属性失效拆分、单点分配；Pie/Doughnut 独立专项 | 记录前后时间/分配/扫描与重建次数，保持行为 |
| B：公共能力 | 时间轴、混合柱线、标注；数据适配及视觉映射第一版 | 真实业务例：销售额柱+转化率线+目标线；保留原 API |
| C：首批新图种 | 二维 Heatmap → Radar → Funnel | 专用模型、命中/选择、主题、PNG、双语文档与独立预览 |
| D：分析与层级 | 范围导航、框选/联动、Treemap/Sunburst、BoxPlot | 稳定 Key、下钻/返回、过滤与数据替换后状态一致 |
| E：行业扩展 | 按需求选 Sankey、Candlestick、Graph、Map 等 | 有真实场景和样例数据后单独设计，不捆绑发布 |

Scatter 原生尾延迟是独立专题；不阻塞 Heatmap 等正常开发，也不因新图种推进而宣称其已解决。各阶段可以拆成小版本，必须逐项验证后交付。

## 9. 性能对比与专项验收

### 对 ECharts 的公平对照

本次没有运行跨库基准，因此不给出倍数或帧率优劣结论。若后续做对照，分两种口径：

1. 等价输出：相同数据、可见点、画布、DPI、标签、标记、动画、抗锯齿条件；抽样前后分别比较。
2. 用户任务：相同交互、相近可视质量、完整命中/选择能力；启用 ECharts large/progressive 时说明功能与最终完成时间，不只统计首批出现。

浏览器与原生应用不能仅比较一个绘图 API 的 CPU 时间。分别记录启动/首图、更新到完成呈现、交互响应、p50/p95/p99、CPU/原生/GPU 内存；注明机器、系统、后端、JIT/AOT、构建版本及原始/可见/实际绘制点数。

### 本库场景矩阵

- 数据规模：1k、10k、100k；确有需求再扩到 1M，不把所有图种强行推到百万项。
- 更新：单点 Y、极值、X、缺口边界、Replace、批量追加、Move、Reset、切换源。
- 交互：已入样/未入样选择、悬浮、连续缩放平移、图例隐藏、跨图联动。
- 绘制：长折线、大量条形、碎片缺口、密集散点、少量/较多扇区。
- 记录：源枚举数、订阅变更、聚合/几何重建、采样扫描、绘制批次、分配/GC、缓存容量以及完整帧时序。
- p99 使用足够样本和多轮独立运行；建议单场景至少数千次操作并报告轮间波动，不能用几十次样本证明尾延迟改善。

### 兼容与验证边界

每次只验证本次修改的图种及明确受影响调用方。公共坐标改动覆盖相关 Line/Area/Bar/Scatter；径向缓存只测 Pie/Doughnut；新 Heatmap 单独验证。焦点/交互用真实按键/指针；绘制/布局改动补明暗主题、三档尺寸、窄视口与像素对照。公共绑定/AOT 相关变更才追加对应原生消费验证。

不得改变极值、缺口、选中点、软抽样预算、层叠顺序、百分比正负分母与导出内容；Reset 或过期任务不能复活旧数据。

## 10. 文档与演示同步

- 每个新增图种有独立预览，基础变体可按家族合并；不能回到通用“能力实验室”大页。
- 样式直接演示，额外属性用 Description 和短代码；API 只展示真正业务操作，Model 说明字段含义与绑定。
- 每页提供空/异常数据、动态更新、选择、主题、导出示例；公共尺寸与主题约定只链接集中说明。
- 性能文档长期保存方法、样本条件、前后表和局限；临时工具/日志放 `.preview/<任务>/`，不往 PreviewHost 启动入口添加测试参数。
- 官网应区分现有、试验与规划能力；不能因为路线图列有某图种就出现在“已支持”清单。

## 11. 主要依据

本地：

- [Chart 模型](../../TheOne.Avalonia/Controls/ChartData.cs)、[订阅与失效](../../TheOne.Avalonia/Controls/ChartBase.Subscriptions.cs)、[单点更新](../../TheOne.Avalonia/Controls/ChartBase.PointUpdates.cs)。
- [数据缓存](../../TheOne.Avalonia/Controls/ChartBase.DataCache.cs)、[抽样索引](../../TheOne.Avalonia/Controls/ChartBase.SamplingIndex.cs)、[Scatter 绘制](../../TheOne.Avalonia/Controls/ChartBase.ScatterDrawing.cs)、[径向绘制](../../TheOne.Avalonia/Controls/ChartBase.Radial.cs)。
- [坐标与采样](../controls/zh-cn/chart-cartesian.md)、[交互](../controls/zh-cn/chart-interaction.md)、[绘制与历史性能数据](../controls/zh-cn/chart-rendering.md)、[RelationGraph](../controls/zh-cn/relation-graph.md)。

外部：

- [ECharts 官网系列/组件](https://echarts.apache.org/zh/cheat-sheet.html)、[ECharts 6 新特性](https://echarts.apache.org/handbook/zh/basics/release-note/v6-feature/)。
- [坐标轴](https://echarts.apache.org/handbook/zh/concepts/axis/)、[Dataset](https://echarts.apache.org/handbook/zh/concepts/dataset/)、[VisualMap](https://echarts.apache.org/handbook/zh/concepts/visual-map/)、[数据转换](https://echarts.apache.org/handbook/zh/concepts/data-transform/)。
- [Canvas/SVG](https://echarts.apache.org/handbook/zh/best-practices/canvas-vs-svg/)、[无障碍](https://echarts.apache.org/handbook/zh/best-practices/aria/)、[官方图种源码](https://github.com/apache/echarts/blob/master/src/export/charts.ts)。

官网示例索引依赖 JavaScript，本次未逐个实跑所有示例；图种清单以官方手册、v6 特性页与官方导出清单交叉核验。源码 master 链接可能随上游更新，具体实施时锁定实际对照版本。
