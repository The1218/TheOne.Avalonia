# DataGrid 扩展计划

## 当前状态与实施顺序

| 阶段 | 状态 | 下一步／验收 |
| --- | --- | --- |
| 1：冻结可行性、接口 | 已完成 | 公开偏移方案被否决；源码布局方案在阶段 3 验证通过 |
| 2：列筛选 | 已实现 | [当前 API](../controls/datagrid-extensions.md)；文本包含、数值精确／范围、多列 AND、清除与原生回收 |
| 3：右侧冻结 | 已完成 | 原型、正式接入、DataGrid 专项与实际 NativeAOT 通过；API 见使用文档 |
| 4：数值统计行及自动筛选 | 已完成 | 五种统计、隐式编译属性识别和弹层布局通过 DataGrid 专项、定向截图及实际 NativeAOT |
| 5：集成验收 | 已完成，保留平台边界 | 包消费与本机桌面／NativeAOT 实跑；六种桌面 RID 发布。其他操作系统尚未实跑，见验收说明 |

使用说明见[当前 API](../controls/datagrid-extensions.md)，阶段 5 的范围及平台证据见[集成验收](../workflow/datagrid-acceptance.md)。不以视觉偏移实验交付正式冻结功能。

## 右侧冻结的验证结论

环境：.NET 10、核心 Avalonia 12.1.4、Avalonia.Controls.DataGrid 12.1.2。读取的官方源码提交为 `1e12211eb0c52a09e314ca57a344147c37d0a26c`，与该源码的版本文件一致。

**否决“只移动表头和单元格”的实现。** 官方布局没有右侧冻结的原生属性；单元格呈现器为 sealed，列的冻结状态及滚入可见区的决策由内部代码维护。公开 RenderTransform 能把表头移到右边，但不能同时改变原生的列可见性、测量、裁剪和定位决策。

阶段 1 的实验使用单个官方 DataGrid、1,000 个模型、固定列宽、左侧冻结 1 列及一个位于视口外的末列，验证：

1. 表格只创建可见行及缓冲行，保留官方 DataGridRowsPresenter／DataGridCellsPresenter。
2. 初始视口中，末列表头在视口外，多数末列单元格由原生布局隐藏。
3. 只移动表头与已有单元格后，表头进入视口，缺失的末列单元格仍未显示，中间内容也没有为右列建立独立裁剪区。
4. 对末列调用原生 ScrollIntoView 仍改变横向滚动位置。
5. 实际水平滚轮操作会重新移动一次性偏移的表头。
6. 清除实验偏移后，第 1,000 行定位、行容器往返回收及原生数值编辑正常。

这些结论覆盖 Light／Dark × Small／Normal／Large；它们是对所测试偏移方案的否定，不是对所有第三方扩展方案的穷尽证明。

官方实现依据（需要调查布局时读取）：

- [DataGridCellsPresenter：sealed 类型、测量、可见性和裁剪](https://github.com/AvaloniaUI/Avalonia.Controls.DataGrid/blob/1e12211eb0c52a09e314ca57a344147c37d0a26c/src/Avalonia.Controls.DataGrid/Primitives/DataGridCellsPresenter.cs)
- [DataGridColumnHeadersPresenter：冻结区排列及拖动指示器](https://github.com/AvaloniaUI/Avalonia.Controls.DataGrid/blob/1e12211eb0c52a09e314ca57a344147c37d0a26c/src/Avalonia.Controls.DataGrid/Primitives/DataGridColumnHeadersPresenter.cs)
- [DataGridColumns：ScrollColumnIntoView、显示列索引和冻结状态](https://github.com/AvaloniaUI/Avalonia.Controls.DataGrid/blob/1e12211eb0c52a09e314ca57a344147c37d0a26c/src/Avalonia.Controls.DataGrid/DataGridColumns.cs)

### 已采用实施路径

筛选和统计行沿用原生 DataGrid 与附加属性方案。右侧冻结采用与当前版本对应的最小源码补丁，保留官方控件类型和 PART_* 合约。

源码扩展需共同覆盖单元格测量／排列／裁剪、表头与拖动指示器、显示列索引、ScrollColumnIntoView、键盘导航、宽度与滚动条计算及自动化可见性。正式实现前，应先用左右同时冻结的最小源码原型验证这些项目。阶段 3 已在独立原型验证后接入 vendor 源码项目；程序集名和控件类型保持不变。

不把仅绘制副本或拼接多个表格作为默认实现：这些方案需要额外证明选择、编辑、行高、分组、焦点和无障碍行为的一致性。

## 集成验收与后续边界

统计接口、自动属性识别、数值规则及更新边界统一见[当前 API](../controls/datagrid-extensions.md#底部数值统计)，不在计划重复属性表。

筛选／统计／冻结联动、源更新、列布局变化、千行回收和列拖动通过 Light / Dark × 三尺寸专项。独立 NuGet 消费端在 osx-arm64 完成托管与实际 NativeAOT 的 Headless／桌面检查；六种桌面 RID 完成框架依赖发布。其他宿主的实跑、NativeAOT、输入／字体／GPU 差异及正式安装包仍需对应环境验证。远程聚合和服务端未加载数据不在首期范围。

## 验证入口与证据

冻结实验：PreviewHost `--datagrid-stage1` 桌面窗口，`--verify-datagrid-stage1` 自动检查，DataGridStage1 定向渲染。阶段 4 使用 `--verify-datagrid-summary`，证据保存在 `.preview/datagrid-summary/`。测试与 AOT 命令见[测试策略](../workflow/testing.md)；历史阶段结果见[归档](../archive/datagrid-2026-10-13.md)。
