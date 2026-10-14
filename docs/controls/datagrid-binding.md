# DataGrid 绑定、排序与单元格密度

视图根节点声明 ViewModel 的 x:DataType，并启用默认编译绑定。ItemsSource 绑定 List<Record>、ObservableCollection<Record> 或 IEnumerable<Record> 时，官方 Binding／CellTemplate／CellEditingTemplate 元数据自动推断 Record；列和单元格模板无需重复 x:DataType。只设置运行时 DataContext 无法提供静态类型；声明为 object 或非泛型 IEnumerable 的数据源也无法自动推断。

需要原生筛选、排序、分组时使用泛型集合视图：

```csharp
using Avalonia.Collections;
public DataGridCollectionView<Record> View { get; } = new(records);
```

```xml
<DataGrid ItemsSource="{Binding View}" AutoGenerateColumns="False" MaxColumnWidth="320">
  <DataGrid.Columns>
    <DataGridTextColumn Header="名称" Binding="{Binding Name, Mode=TwoWay}" />
    <DataGridCheckBoxColumn Header="启用" Binding="{Binding Enabled, Mode=TwoWay}" />
  </DataGrid.Columns>
</DataGrid>
```

DataGridCollectionView<T> 使用原生实现及枚举顺序；T 仅补充行类型，不复制数据、不预建控件。ObservableCollection<T> 的增删继续通知原生视图；普通 List<T> 不具备变更通知，动态增删应改用 ObservableCollection<T> 或显式刷新视图。NativeAOT 仍调用 DataGridAot.PreserveRowType<Record>()。

每列表头包含 DataGridSortButton：未排序显示上下对称双箭头，升／降序显示强调色方向标记。点击、Enter 或 Space 按「未排序 → 升序 → 降序 → 未排序」循环，沿用原生编辑提交与 Sorting 事件。Shift 保留已有排序并追加列，降序后再次激活只移除当前列；多列排序在按钮内显示从 1 开始的优先级，改变方向保持顺序，移除后自动重新编号。平台 Ctrl／⌘ 清除全部排序，配合 Shift 则只清除当前列。

CanUserSortColumns 与列 CanUserSort 控制可用性，原有表头点击、拖动、调整宽度保留。CycleSort(KeyModifiers) 为三态入口；原 Sort()／Sort(KeyModifiers)／Sort(ListSortDirection) 保持原生行为。列 SortDirection、SortPriority 从原生视图读取；按钮 Direction、SortPriority 为可编译绑定的只读 DirectProperty。排序集合外部变更及数据源替换也同步显示，未排序优先级为 0，单列排序不显示编号。CanUserSortChanged 通知动态列可用性变化。

DataGrid 预览的 ViewModel 与 GridRecord 直接继承 ReactiveObject，使用 .NET 10 的 field 属性和 RaiseAndSetIfChanged，不使用额外的属性通知封装。保留行编辑取消、数据验证与选中行的派生属性通知。

居中文本使用完整内容区宽度，避免排序与回收后按旧文本尺寸裁切，同时保留其他控件的内容对齐。small 单元格为固定 20px 复选标记保留完整高度；按钮／ToggleButton／RepeatButton 使用单元格密度内边距和最小高度，Auto 行高继续适应内容，不压缩字体或强制截断。显式固定 RowHeight 过小时仍由消费方调整。

PreviewHost 的 DataGrid 页包含独立集合操作区域与 1,000 项实际回收验证；专项为 --verify-datagrid-binding，定向图片为 DataGridBinding。未增加对比页面。

[筛选、冻结与统计](datagrid-extensions.md)。
