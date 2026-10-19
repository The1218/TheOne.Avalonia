# 控件扩展与图表能力路线图

评估日期：2026-10-19。本文保存本次源码及 ECharts 官网对照分析，属于建议路线，不代表所有项目已经排期。优先级基于本库当前表单、流程、报表与桌面业务系统的用途。没有进行跨库性能实测。

## 判断与边界

优先补业务选择控件与图表基础能力，再逐步增加图种。已有 Form、MultiSelectBox、NavPage、WorkflowEditor、ReportDesign、Upload、ImagePreview、QRCode 等不重复建设。基础 Avalonia 能力优先通过主题和行为扩展，避免仅为换外观增加包装控件。

## 业务扩展控件

| 优先级 | 建议 | 首要范围 |
| --- | --- | --- |
| P0 | DateRangePicker / DateTimeRangePicker | 起止范围、快捷范围、起止约束、清空、双向绑定、跨月选择；已有单日期、日期时间、月、年输入不能替代完整范围输入 |
| P0 | TreeSelect | 单选/多选、搜索保留祖先、父子勾选规则、禁选项、稳定 Key 与显示字段映射、虚拟化、键盘和集合生命周期 |
| P0 | Cascader | 逐级路径选择、异步加载、回显、中间层选择和过期请求取消；与 TreeSelect 分开建模 |
| P1 | Drawer | 遮罩、焦点、Esc/外部关闭、方向、尺寸、异步关闭确认；复用现有布局及消息设施 |
| P1 | Skeleton / EmptyState | 首次加载骨架、空数据、无搜索结果、失败状态；不重复 LoadingContainer 的忙碌覆盖 |
| P1 | 多文件上传 / 附件列表 | 基于现有 Upload 扩展队列、进度、取消重试、并发限制及附件模板 |
| P1 | Timeline | 按时间排列的操作记录、审批历史、事件；与 FlowStep 的步骤进度不同 |
| P2 | Badge / Statistic | 菜单角标、待办数量、指标值、趋势；保持轻量 |
| P2 | Breadcrumb / CommandPalette | 路径导航、全局搜索与键盘命令；与 NavPage 共用导航状态 |
| P2 | Transfer | 搜索、虚拟化、禁选、稳定 Key 与双列表转移 |

富文本、代码编辑器、Docking 和地图引擎暂不优先自研。TreeSelect 与 Cascader 不合并成万能控件。

## 图表现状

现有 14 个具体类型主要覆盖五个系列家族：

- 折线/面积：Line、Area、StackedArea、PercentStackedArea。
- 柱/条形：Bar、HorizontalBar、StackedBar、StackedHorizontalBar、PercentStackedBar、PercentStackedHorizontalBar。
- 散点：Scatter。
- 饼/环形：Pie、Doughnut。
- 仪表：Gauge，多种样式。

ECharts 把面积、横向、堆叠等作为系列配置或组合，不能用控件个数直接比较。RelationGraph 是单根树，无内置折叠、缩放及虚拟化，部分覆盖 Tree；WorkflowEditor 是流程编辑器，不等同于通用 Graph。GanttChart 是现有业务优势，但仍为只读，后续可评估编辑、依赖关系和工作日历。

## 图种缺口

| 批次 | 类型 | 价值 / 范围 | 相对成本 |
| --- | --- | --- | --- |
| 第一批 | RadarChart | 多维评价，先明确维度轴模型 | 中 |
| 第一批 | HeatmapChart | 先二维网格，再日历/地图；时段负载、相关性、设备状态 | 中 |
| 第一批 | FunnelChart | 转化、审批阶段流失 | 中低 |
| 第一批 | TreemapChart | 层级费用、库存、部门占比 | 中高 |
| 第二批 | SunburstChart | 与 Treemap 共用层级模型，补下钻 | 中高 |
| 第二批 | BoxPlotChart | 分布、离群值、耗时，明确四分位与统计规则 | 中 |
| 按行业 | CandlestickChart | OHLC 模型、价格区间 | 中高 |
| 第二批 | SankeyChart | 资金、能耗、转化路径；布局成本高 | 高 |
| 第二批 | GraphChart | 多根、多对多、环、力导向/环形；不破坏 RelationGraph 树约束 | 高 |
| 完善已有 | Tree 能力 | RelationGraph 折叠、缩放、径向布局及大树裁剪 | 中高 |
| 后续 | Map / Lines | 地理数据、投影、区域命中、轨迹、资源管理 | 高 |
| 后续 | Parallel / ThemeRiver / Chord | 多维、时变主题、关系流量；专门需求 | 高 |
| 后续 | PictorialBar / EffectScatter | 象形柱、涟漪散点，低于分析类图种优先级 | 中 |
| 基础设施 | Custom series | 消费者扩展绘制并复用坐标、命中、提示和导出 | 高 |

气泡图优先扩展 Scatter 逐点大小/颜色；直方图需分箱统计；瀑布图需起点、累计、合计和连接线；范围柱/范围带/误差线需上下界模型。蜂群与抖动属于散点布局。ECharts 小提琴、等高线、范围图等还存在官方自定义系列，不全部计为核心内置系列。Chord 是 ECharts 6 的正式新系列；3D/GL 属于另一个技术范围，不进入近期计划。

## 比增加图种更先补的能力

已有缩放平移、双 Y 轴、共享提示、十字准线、图例交互、抽样、更新动画及 PNG 导出，不列为缺失。

| 能力 | 建议 |
| --- | --- |
| 混合系列 | 先柱+折线，共享坐标/图例/命中；双 Y 轴不等于混合系列 |
| 轴类型 | 分类、数值、时间、对数及专用刻度策略 |
| 数据映射 | 强类型 getter/编译映射，保留现有 ChartPoint API；避免属性名反射与裁剪问题 |
| 视觉映射 | 逐点大小、颜色、透明度、连续/分段图例 |
| 标注 | 标记点、警戒线、目标线、区域和事件 |
| 多选/框选 | 选择集合、稳定身份、刷新保留规则 |
| 范围导航器 | 可绑定范围与缩略概览，不重复现有滚轮缩放 |
| 跨图联动 | 指针、视口、选择共享，防循环 |
| 下钻 | 稳定节点 Key、路径和返回事件 |

## 实施顺序和架构

1. 业务控件先 DateRangePicker、TreeSelect，按实际需求决定 Cascader。
2. 图表先混合系列、轴类型及标注。
3. 新增 Heatmap、Radar、Funnel。
4. 共享树形模型支持 Treemap、Sunburst，完善 RelationGraph。
5. 按需求做 Sankey、金融、地图和 Graph。

保留现有具体控件和 API，逐步抽出坐标、数据映射、布局和绘制能力。不要将新图种全部塞入 ChartBase，也不要以 X/Y 点勉强表达 OHLC、层级或图边。每步仅验证新增和明确受影响的控件，补双语文档、独立预览和真实绘制/输入验证。

## 参考

实施记录：2026-10-19 已落地 [TreeSelect 首版](../controls/zh-cn/tree-select.md)，包含单选/多选、级联、稳定 Key、模型适配器、搜索及行虚拟化。异步子项加载仍由宿主负责，不作为已实现的内置加载能力。

- [本库控件目录](../controls/zh-cn/README.md)、[图表](../controls/zh-cn/charts.md)、[RelationGraph](../controls/zh-cn/relation-graph.md)。
- [ECharts 官方文档索引](https://echarts.apache.org/en/llms.txt)、[图表/组件分类](https://echarts.apache.org/en/cheat-sheet.html)。
- [ECharts 6 新能力](https://echarts.apache.org/handbook/en/basics/release-note/v6-feature/)。
- [Axis](https://echarts.apache.org/handbook/en/concepts/axis/)、[Dataset](https://echarts.apache.org/handbook/en/concepts/dataset/)、[VisualMap](https://echarts.apache.org/handbook/en/concepts/visual-map/)。
