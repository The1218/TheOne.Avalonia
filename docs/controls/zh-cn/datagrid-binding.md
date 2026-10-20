# DataGrid 绑定、排序与单元格密度

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

**简体中文** · [English](../en/datagrid-binding.md)

DataGridColumnHeader、DataGridRowHeader、DataGridRowGroupHeader 是表头、行头和分组模板的原生结构，OneTheme 已提供主题。优先使用 HeaderTemplate 等公开扩展点；替换完整模板时保留 PART_*、列身份和原生布局合约，不单独创建这些容器。

视图根节点声明 ViewModel 的 x:DataType，并启用默认编译绑定。ItemsSource 绑定 List<Record>、ObservableCollection<Record> 或 IEnumerable<Record> 时，官方 Binding／CellTemplate／CellEditingTemplate 元数据自动推断 Record；列和单元格模板无需重复 x:DataType。只设置运行时 DataContext 无法提供静态类型；声明为 object 或非泛型 IEnumerable 的数据源也无法自动推断。

需要原生筛选、排序、分组时使用泛型集合视图：

```csharp
using Avalonia.Collections;
using TheOne.Avalonia.Controls;
// 在创建视图前注册；托管和原生使用同一元数据路径。
_ = DataGridAot.PreserveRowType<Record>();
var view = new DataGridCollectionView<Record>(records);
```

```xml
<DataGrid ItemsSource="{Binding View}" AutoGenerateColumns="False" MaxColumnWidth="320">
  <DataGrid.Columns>
    <DataGridTextColumn Header="名称" Binding="{Binding Name, Mode=TwoWay}" />
    <DataGridCheckBoxColumn Header="启用" Binding="{Binding Enabled, Mode=TwoWay}" />
  </DataGrid.Columns>
</DataGrid>
```

DataGridCollectionView<T> 使用原生实现及枚举顺序；T 仅补充行类型，不复制数据、不预建控件。ObservableCollection<T> 的增删继续通知原生视图；普通 List<T> 不具备变更通知，动态增删应改用 ObservableCollection<T> 或显式刷新视图。托管与 NativeAOT 均在创建视图前调用 DataGridAot.PreserveRowType<Record>()；嵌套模型与自定义比较值分别注册。空的自定义集合需 RegisterCollection<TCollection, Record>()，见下方 [NativeAOT](#nativeaot)。

每列表头包含 DataGridSortButton：未排序显示上下对称双箭头，升／降序显示强调色方向标记。点击、Enter 或 Space 按「未排序 → 升序 → 降序 → 未排序」循环，沿用原生编辑提交与 Sorting 事件。Shift 保留已有排序并追加列，降序后再次激活只移除当前列；多列排序在按钮内显示从 1 开始的优先级，改变方向保持顺序，移除后自动重新编号。平台 Ctrl／⌘ 清除全部排序，配合 Shift 则只清除当前列。

CanUserSortColumns 与列 CanUserSort 控制可用性，原有表头点击、拖动、调整宽度保留。CycleSort(KeyModifiers) 为三态入口；原 Sort()／Sort(KeyModifiers)／Sort(ListSortDirection) 保持原生行为。列 SortDirection、SortPriority 从原生视图读取；按钮 Direction、SortPriority 为可编译绑定的只读 DirectProperty。排序集合外部变更及数据源替换也同步显示，未排序优先级为 0，单列排序不显示编号。CanUserSortChanged 通知动态列可用性变化。

DataGrid 预览的 ViewModel 与 GridRecord 直接继承 ReactiveObject，使用 .NET 10 的 field 属性和 RaiseAndSetIfChanged，不使用额外的属性通知封装。保留行编辑取消、数据验证与选中行的派生属性通知。

居中文本使用完整内容区宽度，避免排序与回收后按旧文本尺寸裁切，同时保留其他控件的内容对齐。small 单元格为固定 20px 复选标记保留完整高度；按钮／ToggleButton／RepeatButton 使用单元格密度内边距和最小高度，Auto 行高继续适应内容，不压缩字体或强制截断。显式固定 RowHeight 过小时仍由消费方调整。

[筛选、冻结与统计](datagrid-extensions.md)。

## NativeAOT

类库模板采用 TemplateBinding 或编译绑定。消费视图和数据模板声明正确的 `x:DataType`；代码后置设置运行时 DataContext 不能提供静态类型。非泛型集合视图的列需显式声明行类型；嵌套绑定作用域也需声明对应模型类型，不通过关闭编译绑定或 ReflectionBinding 绕过检查。

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
  <IsAotCompatible>true</IsAotCompatible>
  <!-- 仅可执行应用设置；类库不设置 PublishAot。 -->
  <PublishAot>true</PublishAot>
</PropertyGroup>
```

`DataGridAot.PreserveRowType<T>()` 在创建视图或表格前注册具体行类型，只保留该类型的公共实例属性、访问器和公共无参构造函数，不扫描或保留整个程序集。没有公共无参构造函数的行仍可显示和编辑，只是不提供 AddNew。嵌套路径沿途的模型均需注册；自定义 `IComparable<T>` 字段也注册该类型，排序使用编译期实例化的 `Comparer<T>.Default`。

自动列从注册属性生成编译访问器。基础数字、可空值、日期时间和 Guid 使用明确转换 API；格式错误和溢出报告绑定错误。枚举、自定义 TypeConverter、Parse 或隐式转换需显式列 Converter。注册属性支持单参数 int／string 索引器。筛选和统计复用编译 getter。

`List<T>`、`ObservableCollection<T>`、`T[]`、`DataGridCollectionView<T>` 的空源行类型由注册表提供；其他空集合形状调用 `DataGridRowMetadata.RegisterCollection<MyCollection, Record>()`。非空源可由首项识别类型，但不会据此保留属性。未注册的属性路径明确报错，避免裁剪后悄悄成为只读或排序无效。

路径排序可显式设置 `SortMemberPath`；有限 `MaxColumnWidth` 在创建列前设置：

```xml
<DataGrid ItemsSource="{Binding Records}" AutoGenerateColumns="False" MaxColumnWidth="320">
  <DataGrid.Columns>
    <DataGridTextColumn Header="名称" SortMemberPath="Name"
        Binding="{Binding Name, Mode=TwoWay}" />
  </DataGrid.Columns>
</DataGrid>
```

本库替代官方 DataGrid 包，保留原生程序集身份、类型和 Presenter；不要同时引用官方 `Avalonia.Controls.DataGrid` 包。源码与许可见 [vendor README](../../../vendor/Avalonia.Controls.DataGrid/README.md)。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### DataGridCollectionView

[完整源码契约](../../api/zh-cn/Avalonia.Collections.DataGridCollectionView_T_.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### DataGridColumn

[完整源码契约](../../api/zh-cn/Avalonia.Controls.DataGridColumn.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`SortPriority`](../../../vendor/Avalonia.Controls.DataGrid/DataGridColumn.cs#L430) | `int` | 只读; CLR / n/a | `accessor-defined / state` | Gets the one-based priority, or zero if this column is not sorted. |

### DataGridRowMetadata

[完整源码契约](../../api/zh-cn/Avalonia.Controls.DataGridRowMetadata.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### DataGridAot

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridAot.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### DataGridColumnOptions

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridColumnOptions.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`FilterCondition`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L21) | `TheOne.Avalonia.Controls.DataGridFilterCondition?` | 可写; TwoWay; 附加属性 | `null` | DataGridFilterCondition?／null ; 已应用的不可变条件；支持 TwoWay；null 表示无条件；弹层草稿单独保存 |
| [`FilterEnabled`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L15) | `bool` | 可写; OneWay; 附加属性 | `true` | bool／true ; 表格开启筛选入口后，允许关闭单列入口；关闭该列筛选能力时清除其条件 |
| [`FilterError`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L25) | `string?` | 只读; OneWay; 附加属性 | `null` | string?／null ; 阶段 2 的只读查询属性；条件／字段错误说明，使用 GetFilterError 查询 |
| [`FilterKind`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L18) | `TheOne.Avalonia.Controls.DataGridFilterKind` | 可写; OneWay; 附加属性 | `0 / default` | DataGridFilterKind／Auto ; `Auto`、`Text`、`Number`、`None`；Auto 根据 ValueAccessor 的原始字段类型判断 可选值：`Auto`, `Text`, `Number`, `None` |
| [`SummaryEnabled`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L28) | `bool` | 可写; OneWay; 附加属性 | `false` | bool／false ; 显式启用该列，限数值或可空数值属性 |
| [`SummaryFormat`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L35) | `string?` | 可写; OneWay; 附加属性 | `null` | string?／null ; .NET 数值格式，独立于列显示 Converter，使用数据视图 Culture |
| [`SummaryKind`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L31) | `TheOne.Avalonia.Controls.DataGridSummaryKind` | 可写; OneWay; 附加属性 | `0 / default` | DataGridSummaryKind／Sum ; Sum、Average、Min、Max、Count，每列一个 校验：`Enum.IsDefined` 可选值：`Sum`, `Average`, `Min`, `Max`, `Count` |
| [`ValueAccessor`](../../../TheOne.Avalonia/Controls/DataGridColumnOptions.cs#L11) | `TheOne.Avalonia.Controls.IDataGridValueAccessor?` | 可写; OneWay; 附加属性 | `null` | IDataGridValueAccessor?／null ; 可选覆盖：普通绑定列自动识别；模板列或特殊计算字段可提供强类型委托 |

### DataGridSortButton

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridSortButton.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Direction`](../../../TheOne.Avalonia/Controls/DataGridSortButton.cs#L78) | `System.ComponentModel.ListSortDirection?` | 只读; OneWay | `computed / state` | 展开或端口方向枚举；具体枚举类型列明可选值，不用布尔值代替方向。 可选值：`Ascending`, `Descending` |
| [`SortPriority`](../../../TheOne.Avalonia/Controls/DataGridSortButton.cs#L84) | `int` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### DataGridValueAccessor

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridValueAccessor.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

- `DataGrid`: `Classes="borderless"`

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`DataGridColumn`](../../api/zh-cn/Avalonia.Controls.DataGridColumn.md) | `void DataGridColumn.CycleSort(KeyModifiers modifiers = KeyModifiers.None)` | Cycles unsorted, ascending, descending, then unsorted through the native edit pipeline. |
| [`DataGridFilterCondition`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFilterCondition.md) | `abstract bool DataGridFilterCondition.Matches(object? value)` | 判断输入值是否符合此筛选条件；具体空值和比较规则见条件类型。 |
| [`IDataGridValueAccessor`](../../api/zh-cn/TheOne.Avalonia.Controls.IDataGridValueAccessor.md) | `object? IDataGridValueAccessor.GetValue(object item)` | 通过显式字段键／类型化 getter 从原始行读取值；不使用反射路径。 |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`Avalonia.Controls.DataGridColumn`](../../api/zh-cn/Avalonia.Controls.DataGridColumn.md) | 1 |
| [`TheOne.Avalonia.Controls.DataGridColumnOptions`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridColumnOptions.md) | 8 |
| [`TheOne.Avalonia.Controls.DataGridFilterCondition`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFilterCondition.md) | 1 |
| [`TheOne.Avalonia.Controls.DataGridFilterKind`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFilterKind.md) | 4 |
| [`TheOne.Avalonia.Controls.DataGridSummaryKind`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridSummaryKind.md) | 5 |
| [`TheOne.Avalonia.Controls.IDataGridValueAccessor`](../../api/zh-cn/TheOne.Avalonia.Controls.IDataGridValueAccessor.md) | 1 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
