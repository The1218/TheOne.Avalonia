# DataGridFeatures

[English](../en/TheOne.Avalonia.Controls.DataGridFeatures.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.DataGridFeatures.md) · [API index](README.md)

`TheOne.Avalonia.Controls.DataGridFeatures` · Class · `Avalonia.AvaloniaObject`

Column filtering and native right freezing; preserves the data view and presenters.

[Source / 声明](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L33)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`FrozenRightColumnCount`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L35) | `int` | 可写; OneWay; 附加属性 | `0 / default` | 右侧冻结列数；与左侧冻结区域共同占用可用宽度。 校验：`value => value >= 0` |
| [`ShowColumnFilters`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L38) | `bool` | 可写; OneWay; 附加属性 | `false` | 启用表格列筛选入口；具体列还必须允许对应筛选能力。 |
| [`ShowSummaryRow`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L47) | `bool` | 可写; OneWay; 附加属性 | `false` | bool／false ; 独立统计行；关闭停止计算与订阅，保留列配置 |
| [`SummaryEmptyText`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L60) | `string` | 可写; OneWay; 附加属性 | `"—"` | string／— ; 无有效数值时的 Sum／Average／Min／Max 文字；Count 为 0 |
| [`SummaryLabel`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L57) | `string` | 可写; OneWay; 附加属性 | `"合计"` | string／合计 ; 原生行头槽可见时显示，不覆盖第一列结果 |
| [`SummaryLeavesOnly`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L54) | `bool` | 可写; OneWay; 附加属性 | `false` | bool／false ; 树视图仅统计原始叶节点，避免父级与子项重复累加；普通行均视为叶节点 |
| [`SummaryScope`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L50) | `TheOne.Avalonia.Controls.DataGridSummaryScope` | 可写; OneWay; 附加属性 | `0 / default` | DataGridSummaryScope／FilteredItems ; 业务与列筛选后的本地源模型、分页前；SourceItems 忽略筛选，VisibleItems 使用当前视图枚举；树视图 FilteredItems 只计直接命中节点 校验：`Enum.IsDefined` 可选值：`FilteredItems`, `SourceItems`, `VisibleItems` |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static int DataGridFeatures.GetFrozenRightColumnCount(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L79) | 读取附加属性。右侧冻结列数；与左侧冻结区域共同占用可用宽度。 |
| [`static void DataGridFeatures.SetFrozenRightColumnCount(DataGrid grid, int count)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L84) | 设置附加属性。右侧冻结列数；与左侧冻结区域共同占用可用宽度。 |
| [`static bool DataGridFeatures.GetShowSummaryRow(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L89) | 读取附加属性。bool／false ; 独立统计行；关闭停止计算与订阅，保留列配置 |
| [`static void DataGridFeatures.SetShowSummaryRow(DataGrid grid, bool value)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L94) | 设置附加属性。bool／false ; 独立统计行；关闭停止计算与订阅，保留列配置 |
| [`static DataGridSummaryScope DataGridFeatures.GetSummaryScope(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L99) | 读取附加属性。DataGridSummaryScope／FilteredItems ; 业务与列筛选后的本地源模型、分页前；SourceItems 忽略筛选，VisibleItems 使用当前视图枚举；树视图 FilteredItems 只计直接命中节点 |
| [`static void DataGridFeatures.SetSummaryScope(DataGrid grid, DataGridSummaryScope value)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L104) | 设置附加属性。DataGridSummaryScope／FilteredItems ; 业务与列筛选后的本地源模型、分页前；SourceItems 忽略筛选，VisibleItems 使用当前视图枚举；树视图 FilteredItems 只计直接命中节点 |
| [`static bool DataGridFeatures.GetSummaryLeavesOnly(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L109) | 读取附加属性。bool／false ; 树视图仅统计原始叶节点，避免父级与子项重复累加；普通行均视为叶节点 |
| [`static void DataGridFeatures.SetSummaryLeavesOnly(DataGrid grid, bool value)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L114) | 设置附加属性。bool／false ; 树视图仅统计原始叶节点，避免父级与子项重复累加；普通行均视为叶节点 |
| [`static string DataGridFeatures.GetSummaryLabel(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L119) | 读取附加属性。string／合计 ; 原生行头槽可见时显示，不覆盖第一列结果 |
| [`static void DataGridFeatures.SetSummaryLabel(DataGrid grid, string value)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L124) | 设置附加属性。string／合计 ; 原生行头槽可见时显示，不覆盖第一列结果 |
| [`static string DataGridFeatures.GetSummaryEmptyText(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L129) | 读取附加属性。string／— ; 无有效数值时的 Sum／Average／Min／Max 文字；Count 为 0 |
| [`static void DataGridFeatures.SetSummaryEmptyText(DataGrid grid, string value)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L134) | 设置附加属性。string／— ; 无有效数值时的 Sum／Average／Min／Max 文字；Count 为 0 |
| [`static DataGridSummaryResult? DataGridFeatures.GetSummaryResult(DataGrid grid, DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L139) | 读取指定列的当前统计结果，包括有效数量、汇总值和错误。 |
| [`static bool DataGridFeatures.GetShowColumnFilters(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L149) | 读取附加属性。启用表格列筛选入口；具体列还必须允许对应筛选能力。 |
| [`static void DataGridFeatures.SetShowColumnFilters(DataGrid grid, bool value)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L154) | 设置附加属性。启用表格列筛选入口；具体列还必须允许对应筛选能力。 |
| [`static DataGridFilterApplyResult DataGridFeatures.ApplyFilter(DataGrid grid, DataGridColumn column, DataGridFilterCondition? condition)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L159) | 应用列筛选条件并返回应用结果；失败信息通过结果／筛选错误状态获取。 |
| [`static DataGridFilterApplyResult DataGridFeatures.ClearFilter(DataGrid grid, DataGridColumn column)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L165) | 清除指定列筛选，不重置其他列的条件。 |
| [`static DataGridFilterApplyResult DataGridFeatures.ClearAllFilters(DataGrid grid)`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L170) | 清除表格所有列筛选并刷新可见行。 |
| [`DataGridFeatures.DataGridFeatures()`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L33) | 创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。 |

## 公开字段

| 完整签名／值 | 说明 |
| --- | --- |
| [`static readonly AttachedProperty<int> DataGridFeatures.FrozenRightColumnCountProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L35) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> DataGridFeatures.ShowColumnFiltersProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L38) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<DataGridFilterChangingEventArgs> DataGridFeatures.FilterChangingEvent`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L41) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<DataGridFilterChangedEventArgs> DataGridFeatures.FilterChangedEvent`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L44) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> DataGridFeatures.ShowSummaryRowProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L47) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<DataGridSummaryScope> DataGridFeatures.SummaryScopeProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L50) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<bool> DataGridFeatures.SummaryLeavesOnlyProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L54) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<string> DataGridFeatures.SummaryLabelProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L57) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly AttachedProperty<string> DataGridFeatures.SummaryEmptyTextProperty`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L60) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |
| [`static readonly RoutedEvent<DataGridSummaryUpdatedEventArgs> DataGridFeatures.SummaryUpdatedEvent`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L63) | 公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.DataGridFilterApplyResult`](TheOne.Avalonia.Controls.DataGridFilterApplyResult.md)
- [`TheOne.Avalonia.Controls.DataGridFilterCondition`](TheOne.Avalonia.Controls.DataGridFilterCondition.md)
- [`TheOne.Avalonia.Controls.DataGridSummaryResult`](TheOne.Avalonia.Controls.DataGridSummaryResult.md)
- [`TheOne.Avalonia.Controls.DataGridSummaryScope`](TheOne.Avalonia.Controls.DataGridSummaryScope.md)
