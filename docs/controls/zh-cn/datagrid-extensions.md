# DataGrid 筛选、冻结与统计

**简体中文** · [English](../en/datagrid-extensions.md)

保留官方 DataGrid、列类型、虚拟化 Presenter 与键盘行为，提供自动实体属性筛选、原生右侧冻结和底部数值统计。


绑定类型推断、泛型集合视图、列头排序按钮及单元格尺寸见 [DataGrid 绑定与排序](datagrid-binding.md)。

## 使用列筛选

实体属性直接由列的编译绑定自动识别，开启表格筛选即可。消费方在创建表格或集合视图前调用 `DataGridAot.PreserveRowType<StockRecord>()`；显示／编辑绑定继续使用 CompiledBinding。

```xml
<DataGrid ItemsSource="{Binding Records}" AutoGenerateColumns="False"
          MaxColumnWidth="320" one:DataGridFeatures.ShowColumnFilters="True">
  <DataGrid.Columns>
    <DataGridTextColumn Header="名称"
        Binding="{CompiledBinding Name, DataType={x:Type vm:StockRecord}}" />
    <DataGridTextColumn Header="数量"
        Binding="{CompiledBinding Quantity, DataType={x:Type vm:StockRecord}}" />
  </DataGrid.Columns>
</DataGrid>
```

加载 OneTheme 后，可筛选的列显示漏斗入口；生效条件使用强调色。文本支持包含搜索，数值支持精确值和包含上下界的范围；范围允许只填一端，空条件清除该列。布尔、日期、操作模板列及无法确定实体属性的列不显示筛选按钮；`FilterEnabled=False` 或 `FilterKind=None` 也会隐藏单列入口。运行时更改配置会更新表头，并关闭已不可用的筛选弹层。

```csharp
var result = DataGridFeatures.ApplyFilter(grid, grid.Columns[0], new TextFilterCondition("显示器"));
if (!result.Succeeded) { /* 显示 result.Error */ }
DataGridFeatures.ApplyFilter(grid, grid.Columns[1],
    new NumericFilterCondition<int>(NumericFilterMode.Range, minimum: 5, maximum: 20));
DataGridFeatures.ClearFilter(grid, grid.Columns[0]);
DataGridFeatures.ClearAllFilters(grid);
```

多个条件为 AND，再与原有业务 Filter 组合。清除全部列条件不会清除业务筛选、排序或分组。隐藏所有入口不会清除条件；关闭某列筛选能力则清除该列条件。以上操作返回 DataGridFilterApplyResult，须检查 Succeeded；均在 UI 线程调用。

可通过 `FilterChangingEvent` 取消变更，`FilterChangedEvent` 查询列及新旧条件。ClearAll 的任一变更被取消时，整批保持原状。`FilterCondition` 默认 TwoWay；列不是普通 DataContext 容器，需要绑定 VM 条件时可显式提供编译绑定的 source：

```csharp
column.Bind(DataGridColumnOptions.FilterConditionProperty,
    CompiledBinding.Create<StockViewModel, DataGridFilterCondition?>(
        vm => vm.NameCondition, source: model, mode: BindingMode.TwoWay));
```

不兼容的条件绑定会恢复上一有效值，错误通过 `DataGridColumnOptions.GetFilterError(column)` 查询。弹层草稿不改数据；应用／Enter 提交，Esc／取消／外部关闭放弃草稿并返回入口。无效数值、倒置范围和未通过原生编辑验证均保留旧条件。

### 数据视图与生命周期边界

支持官方 DataGridCollectionView 及其派生的 [DataGridTreeCollectionView](datagrid-tree.md)（普通 IEnumerable 的 ItemsSource 也会被官方 DataGrid 自动包装为普通视图）。它提供公开编辑状态与 Filter 通知；不反射调用内部 editable 接口。自定义 IDataGridCollectionView 暂不接入列筛选，入口保持不可用并说明限制。

同一 CollectionView 同时只允许一个表格管理活动列条件；需要独立筛选的多个表格应为同一模型集合各建一个 CollectionView。行为保留后续业务 Filter 的替换，卸载时只撤销自身安装的谓词；编辑尚未结束时延迟恢复，恢复回调不保留已卸载 UI。

数据项 INotifyPropertyChanged、源集合增删、数据源替换会合并更新过滤结果，包含未创建容器的模型；无活动列条件时解除筛选专用行订阅。字段取值失败会拒绝应用并提供错误，不能以显示字符串或隐式类型转换代替原始字段。自动取值使用 Avalonia 编译路径的生成 getter，忽略显示 Converter／StringFormat，不按字段名反射，也不创建数据行／探针控件。嵌套属性链支持中间 null、子对象通知与替换；Binding 更换会重新识别字段。

自定义 HeaderTemplate（表头内容）继续使用原生合约。若完全替换 ColumnHeaderTheme／ControlTemplate，需要在自己的表头中嵌入 DataGridFilterButton；入口使用公开 GetColumnContainingElement 取得列身份，不按标题匹配。

## 当前属性

DataGridFeatures.ShowColumnFilters 默认为 false。DataGridColumnOptions 的列配置如下：

| 属性 | 类型／默认值 | 契约 |
| --- | --- | --- |
| `ValueAccessor` | IDataGridValueAccessor?／null | 可选覆盖：普通绑定列自动识别；模板列或特殊计算字段可提供强类型委托 |
| `FilterEnabled` | bool／true | 表格开启筛选入口后，允许关闭单列入口；关闭该列筛选能力时清除其条件 |
| `FilterKind` | DataGridFilterKind／Auto | `Auto`、`Text`、`Number`、`None`；Auto 根据 ValueAccessor 的原始字段类型判断 |
| `FilterCondition` | DataGridFilterCondition?／null | 已应用的不可变条件；支持 TwoWay；null 表示无条件；弹层草稿单独保存 |
| `FilterError` | string?／null | 阶段 2 的只读查询属性；条件／字段错误说明，使用 GetFilterError 查询 |


自动识别只适用于面向当前实体的编译属性链；模板列包含多个字段时不猜测身份，可选 ValueAccessor 覆盖或关闭 FilterEnabled。数值支持 byte／sbyte／short／ushort／int／uint／long／ulong／float／double／decimal 及 Nullable；按原始类型与 CurrentCulture 解析，精确比较不增加浮点容差，null／NaN／Infinity 不匹配有效条件。

筛选验证入口为 `--verify-datagrid-filters`，定向渲染为 DataGridFilters；执行方式见[预览专项入口](../../../design/PreviewHost/Program.cs)。原生布局的包消费示例见 [DataGridPackageConsumer](../../../design/PreviewHost/Consumers/DataGridPackageConsumer/Program.cs)。

## 原生右侧冻结

```xml
<DataGrid ItemsSource="{CompiledBinding Records}" AutoGenerateColumns="False"
          MaxColumnWidth="320" FrozenColumnCount="1"
          one:DataGridFeatures.FrozenRightColumnCount="1">
  <!-- 显式编译绑定列；最后一个可见列进入右侧区。 -->
</DataGrid>
```

FrozenRightColumnCount 为 int，默认 0，可绑定／由 Style 设置；C# 使用 GetFrozenRightColumnCount／SetFrozenRightColumnCount。按 DisplayIndex 末尾 N 个可见列确定右区，左侧沿用原生 FrozenColumnCount；中央区独立滚动与裁剪。原生列 IsFrozen 对左右区均为 true，绑定不随滚动改变。

配置负数或对已存在列配置超出可见列数量／左右重叠会拒绝并保留旧值。XAML 可在列创建前设置数量；后续隐藏／删除导致列不足时，有效右区按剩余非左冻结可见列缩减，保留配置值，列恢复后重新计算。极窄窗口优先保留左区，右区从左边裁剪，中央区可用宽度最小为零；扩宽后恢复完整布局。

鼠标拖动重排限同一冻结区，中央区不会被拖入冻结区；程序设置 DisplayIndex 则按新末尾位置重算右区。右列 ScrollIntoView 不改变横向偏移，中央列滚入范围排除左右冻结宽度。列宽、自动行高、原生编辑／排序／分组、筛选和行回收继续使用官方路径。

实现需要原生布局扩展：类库引用 [12.1.2 源码项目](../../../vendor/Avalonia.Controls.DataGrid/README.md)，保留官方程序集身份、类型、Presenter 和 PART_*，替代官方 DataGrid NuGet。消费项目不要同时再引入官方同名包；升级需维护该局部补丁。核心 Avalonia 保持 12.1.4。

PreviewHost 的 DataGrid 页可调节“右侧冻结”数量，默认 0。专项为 `--verify-datagrid-right`，包含右区、已有筛选及左侧原生回归；定向渲染名称为 DataGridRightFrozen。命令与平台边界见[预览专项入口](../../../design/PreviewHost/Program.cs)。

### 列拖动反馈

列拖动保留原生落点及冻结区域限制：源表头淡出，拖影使用独立卡片并省略重复操作按钮，插入提示短缓动。拖影保持原始抓取位置；Esc、表头上下范围外释放或卸载取消拖动并清理捕获。`Motion.Enabled=False` 或 `One.Motion.Fast` 为零会关闭插入提示动效。包消费示例见 [DataGridPackageConsumer](../../../design/PreviewHost/Consumers/DataGridPackageConsumer/Program.cs)。

## 底部数值统计



```xml
<DataGrid ItemsSource="{CompiledBinding Records}" AutoGenerateColumns="False"
          MaxColumnWidth="320" one:DataGridFeatures.ShowColumnFilters="True"
          one:DataGridFeatures.ShowSummaryRow="True"
          one:DataGridFeatures.SummaryScope="FilteredItems">
  <DataGrid.Columns>
    <DataGridTextColumn Header="数量" Binding="{CompiledBinding Quantity, DataType={x:Type vm:StockRecord}}"
        one:DataGridColumnOptions.SummaryEnabled="True"
        one:DataGridColumnOptions.SummaryKind="Sum"
        one:DataGridColumnOptions.SummaryFormat="N0" />
    <DataGridTextColumn Header="单价" Binding="{CompiledBinding Price, DataType={x:Type vm:StockRecord}}"
        one:DataGridColumnOptions.SummaryEnabled="True"
        one:DataGridColumnOptions.SummaryKind="Average"
        one:DataGridColumnOptions.SummaryFormat="0.00" />
  </DataGrid.Columns>
</DataGrid>
```

| 配置 | 类型／默认值 | 行为 |
| --- | --- | --- |
| DataGridFeatures.ShowSummaryRow | bool／false | 独立统计行；关闭停止计算与订阅，保留列配置 |
| SummaryScope | DataGridSummaryScope／FilteredItems | 业务与列筛选后的本地源模型、分页前；SourceItems 忽略筛选，VisibleItems 使用当前视图枚举；树视图 FilteredItems 只计直接命中节点 |
| SummaryLeavesOnly | bool／false | 树视图仅统计原始叶节点，避免父级与子项重复累加；普通行均视为叶节点 |
| SummaryLabel | string／合计 | 原生行头槽可见时显示，不覆盖第一列结果 |
| SummaryEmptyText | string／— | 无有效数值时的 Sum／Average／Min／Max 文字；Count 为 0 |
| DataGridColumnOptions.SummaryEnabled | bool／false | 显式启用该列，限数值或可空数值属性 |
| SummaryKind | DataGridSummaryKind／Sum | Sum、Average、Min、Max、Count，每列一个 |
| SummaryFormat | string?／null | .NET 数值格式，独立于列显示 Converter，使用数据视图 Culture |

支持 11 种基础数值类型及 Nullable；null、NaN、Infinity 不参与任何聚合，Count 仅计算有效数值。整数／decimal 的合计与平均使用 decimal 累加，浮点使用 double；Min／Max 保留原始类型，Count 返回 long。累加溢出、字段读取失败、非数值列配置和格式错误通过结果 Error 报告，行内显示 !；不静默显示无穷大或错误数字。

`DataGridFeatures.GetSummaryResult(grid, column)` 返回 Value、ValidCount、Error；订阅 SummaryUpdatedEvent 获取合并更新通知。过滤、编辑、未创建行的 PropertyChanged、集合增删／替换及嵌套对象变化都会更新结果。隐藏列暂停该列计算与监听，恢复后重算；统计行不参与选择、编辑、排序或 ItemsSource。列宽／重排／横向滚动与左右冻结共用原生布局描述，窄窗口沿用冻结区裁剪规则。

首期只统计本地已加载源模型，不能代表服务端未加载数据。自定义完整 DataGrid 模板需嵌入 DataGridSummaryPresenter，默认 OneTheme 已包含。PreviewHost 可切换“显示统计”；专项 `--verify-datagrid-summary` 包含统计、自动筛选、冻结及原生回归；截图名称 DataGridSummary 和 DataGridFilters。执行方式见[预览专项入口](../../../design/PreviewHost/Program.cs)。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节只列出本项目增加的 Classes、属性、扩展 API／事件及关联模型，不重复 Avalonia 原生继承 API。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用可见示例对照本项目附加样式，用 Description 组件说明专属属性、API 签名与模型字段；ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### DataGridCollectionView

[API / model reference](../../api/zh-cn/Avalonia.Collections.DataGridCollectionView_T_.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### DataGridColumn

[API / model reference](../../api/zh-cn/Avalonia.Controls.DataGridColumn.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`SortPriority`](../../../vendor/Avalonia.Controls.DataGrid/DataGridColumn.cs#L743) | `int` | 只读; CLR / n/a | `accessor-defined / state` | Gets the one-based priority, or zero if this column is not sorted. |

### DataGridColumnOptions

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridColumnOptions.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`FilterCondition`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L14) | `TheOne.Avalonia.Controls.DataGridFilterCondition?` | 可写; TwoWay; 附加属性 | `null` | DataGridFilterCondition?／null ; 已应用的不可变条件；支持 TwoWay；null 表示无条件；弹层草稿单独保存 |
| [`FilterEnabled`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L12) | `bool` | 可写; OneWay; 附加属性 | `true` | bool／true ; 表格开启筛选入口后，允许关闭单列入口；关闭该列筛选能力时清除其条件 |
| [`FilterError`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L15) | `string?` | 只读; OneWay; 附加属性 | `null` | string?／null ; 阶段 2 的只读查询属性；条件／字段错误说明，使用 GetFilterError 查询 |
| [`FilterKind`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L13) | `TheOne.Avalonia.Controls.DataGridFilterKind` | 可写; OneWay; 附加属性 | `DataGridFilterKind.Auto` | DataGridFilterKind／Auto ; `Auto`、`Text`、`Number`、`None`；Auto 根据 ValueAccessor 的原始字段类型判断 可选值：`Auto`, `Text`, `Number`, `None` |
| [`SummaryEnabled`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L16) | `bool` | 可写; OneWay; 附加属性 | `false` | bool／false ; 显式启用该列，限数值或可空数值属性 |
| [`SummaryFormat`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L18) | `string?` | 可写; OneWay; 附加属性 | `null` | string?／null ; .NET 数值格式，独立于列显示 Converter，使用数据视图 Culture |
| [`SummaryKind`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L17) | `TheOne.Avalonia.Controls.DataGridSummaryKind` | 可写; OneWay; 附加属性 | `DataGridSummaryKind.Sum` | DataGridSummaryKind／Sum ; Sum、Average、Min、Max、Count，每列一个 校验：`Enum.IsDefined` 可选值：`Sum`, `Average`, `Min`, `Max`, `Count` |
| [`ValueAccessor`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L11) | `TheOne.Avalonia.Controls.IDataGridValueAccessor?` | 可写; OneWay; 附加属性 | `null` | IDataGridValueAccessor?／null ; 可选覆盖：普通绑定列自动识别；模板列或特殊计算字段可提供强类型委托 |

### DataGridFeatures

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFeatures.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`FrozenRightColumnCount`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L29) | `int` | 可写; OneWay; 附加属性 | `0 / default` | 右侧冻结列数；与左侧冻结区域共同占用可用宽度。 校验：`value => value >= 0` |
| [`ShowColumnFilters`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L33) | `bool` | 可写; OneWay; 附加属性 | `false` | 启用表格列筛选入口；具体列还必须允许对应筛选能力。 |
| [`ShowSummaryRow`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L36) | `bool` | 可写; OneWay; 附加属性 | `false` | bool／false ; 独立统计行；关闭停止计算与订阅，保留列配置 |
| [`SummaryEmptyText`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L40) | `string` | 可写; OneWay; 附加属性 | `"—"` | string／— ; 无有效数值时的 Sum／Average／Min／Max 文字；Count 为 0 |
| [`SummaryLabel`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L39) | `string` | 可写; OneWay; 附加属性 | `"合计"` | string／合计 ; 原生行头槽可见时显示，不覆盖第一列结果 |
| [`SummaryLeavesOnly`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L38) | `bool` | 可写; OneWay; 附加属性 | `false` | bool／false ; 树视图仅统计原始叶节点，避免父级与子项重复累加；普通行均视为叶节点 |
| [`SummaryScope`](../../../TheOne.Avalonia/Controls/DataGridFeatures.cs#L37) | `TheOne.Avalonia.Controls.DataGridSummaryScope` | 可写; OneWay; 附加属性 | `DataGridSummaryScope.FilteredItems` | DataGridSummaryScope／FilteredItems ; 业务与列筛选后的本地源模型、分页前；SourceItems 忽略筛选，VisibleItems 使用当前视图枚举；树视图 FilteredItems 只计直接命中节点 校验：`Enum.IsDefined` 可选值：`FilteredItems`, `SourceItems`, `VisibleItems` |

### DataGridFilterButton

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFilterButton.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### DataGridSortButton

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridSortButton.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Direction`](../../../TheOne.Avalonia/Controls/DataGridSortButton.cs#L40) | `System.ComponentModel.ListSortDirection?` | 只读; OneWay | `computed / state` | 展开或端口方向枚举；具体枚举类型列明可选值，不用布尔值代替方向。 可选值：`Ascending`, `Descending` |
| [`SortPriority`](../../../TheOne.Avalonia/Controls/DataGridSortButton.cs#L41) | `int` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### DataGridSummaryPresenter

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridSummaryPresenter.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Background`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L22) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 背景画刷；主题相关背景使用 DynamicResource，避免固定颜色破坏明暗切换。 |
| [`Foreground`](../../../TheOne.Avalonia/Controls/DataGridSummaryPresenter.cs#L23) | `Avalonia.Media.IBrush?` | 可写; OneWay | `null` | 前景画刷；自定义时保持正文、状态与禁用对比度。 |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| DataGrid | `borderless` | 显式 Classes，可与尺寸类组合 | ListBox.borderless, DataGrid.borderless => BorderThickness=0 |
| DataGrid | `one-large` | 模板自动类；不手动绑定 | DataGrid.one-large => FontSize=16 |
| DataGrid | `one-normal` | 模板自动类；不手动绑定 | DataGrid.one-normal => FontSize=14 |
| DataGrid | `one-small` | 模板自动类；不手动绑定 | DataGrid.one-small => FontSize=12 |

`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出附加外观类的控件使用默认主题及其专属属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`Avalonia.Collections.DataGridCollectionView<T>`](../../api/zh-cn/Avalonia.Collections.DataGridCollectionView_T_.md) | Class | 0 | 2 | 0 |
| [`Avalonia.Controls.DataGridColumn`](../../api/zh-cn/Avalonia.Controls.DataGridColumn.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridColumnOptions`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridColumnOptions.md) | Class | 8 | 16 | 0 |
| [`TheOne.Avalonia.Controls.DataGridFeatures`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFeatures.md) | Class | 7 | 19 | 0 |
| [`TheOne.Avalonia.Controls.DataGridFilterApplyResult`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFilterApplyResult.md) | Struct | 2 | 2 | 0 |
| [`TheOne.Avalonia.Controls.DataGridFilterButton`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFilterButton.md) | Class | 0 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridFilterCondition`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFilterCondition.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridFilterKind`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFilterKind.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridSortButton`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridSortButton.md) | Class | 2 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridSummaryKind`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridSummaryKind.md) | Enum | 5 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridSummaryPresenter`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridSummaryPresenter.md) | Class | 2 | 2 | 0 |
| [`TheOne.Avalonia.Controls.DataGridSummaryResult`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridSummaryResult.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridSummaryScope`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridSummaryScope.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.IDataGridValueAccessor`](../../api/zh-cn/TheOne.Avalonia.Controls.IDataGridValueAccessor.md) | Interface | 1 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
