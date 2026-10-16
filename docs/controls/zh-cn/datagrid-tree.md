# DataGrid 树视图原型

**简体中文** · [English](../en/datagrid-tree.md)

第二阶段原型：在原生 DataGrid 的编辑、选择与行回收上，增加同级排序、保留祖先路径的筛选和树感知统计。API 尚处于原型阶段。

```csharp
DataGridAot.PreserveRowType<Node>();
var tree = new DataGridTreeCollectionView<Node>(roots, node => node.Children);
// DataGridTreeCollectionView 位于 Avalonia.Collections。
```

```xml
<DataGrid ItemsSource="{Binding Tree}" AutoGenerateColumns="False"
          one:DataGridTreeFeatures.IsEnabled="True"
          one:DataGridFeatures.ShowColumnFilters="True"
          one:DataGridFeatures.ShowSummaryRow="True"
          one:DataGridFeatures.SummaryScope="FilteredItems"
          one:DataGridFeatures.SummaryLeavesOnly="True">
  <DataGrid.Columns>
    <one:DataGridTreeTextColumn Header="名称"
        Binding="{CompiledBinding Name, DataType={x:Type vm:Node}, Mode=TwoWay}" />
    <DataGridTextColumn Header="数量"
        Binding="{CompiledBinding Quantity, DataType={x:Type vm:Node}}"
        one:DataGridColumnOptions.SummaryEnabled="True" />
  </DataGrid.Columns>
</DataGrid>
```

视图和列始终使用原业务模型，不添加 `Item` 包装层；泛型视图为编译绑定提供行类型，空源也通过显式集合形状注册识别。创建视图前注册具体行及嵌套属性类型，见 [NativeAOT](datagrid-binding.md#nativeaot)。Children 通过强类型委托读取，不按属性名反射。一次只将一个树视图绑定到一个已挂载表格；独立状态需要独立视图。

## 树与编辑契约

- 默认折叠。`TrySetExpanded(node, true/false)` 返回是否成功；`TryGetNodeState` 查询深度、父节点、筛选后是否有子节点和有效展开状态，以及 `IsFilterMatch`／`IsForcedExpanded`。
- 只接受同类型、非空、无环、无共享对象实例的本地树；按引用识别节点。内部索引全部已加载节点，UI 只创建可见区域的原生行容器。
- 折叠保留后代展开状态；再次展开恢复。每次可见序列变化发送单项增删通知，不替换 ItemsSource、不发送范围通知或 Reset。
- 启用 `DataGridTreeFeatures.IsEnabled` 后，树变化先通过原生行提交。验证失败取消变化并保留编辑器；独立视图在编辑未结束时拒绝展开折叠。
- 单选后代被折叠隐藏时，选择和当前单元格回到折叠节点；多选移除隐藏后代，保留其他选择。展开不重新选回隐藏后代。
- 树列非编辑态下，右键展开／进入首个子节点，左键折叠／回到父节点；编辑器和其他列保留原生导航。
- `INotifyCollectionChanged` 的根与子集合均被监听，包括折叠分支；模型属性通知触发子集合替换检查。普通集合改变后调用 `RefreshTree()`。结构变化发生在未完成编辑期间时，保持旧投影，编辑结束后合并刷新。
- `TreeChanging` 可取消，`TreeChanged` 在成功应用后通知；源刷新或查询变化时事件的 Item／Expanded 为 null。`LoadedItemsChanged` 只报告源结构变化，不因展开或查询反复触发。错误源在有效索引建立前被拒绝，调用方应修复源集合。
- 生命周期由消费方拥有：先解绑／关闭表格并结束编辑与查询延迟作用域，再 `Dispose()` 视图以解除源集合及模型订阅。所有操作在 UI 线程调用。

## 同级排序与筛选

沿用原生表头三态排序、Shift 多列优先级和 Ctrl／⌘ 清除。根节点及每组兄弟节点分别稳定排序，父节点始终先于子树；清除后按各层源集合顺序恢复，不修改业务集合。多个描述一起设置时使用 `using (tree.DeferRefresh())`，只发布一次投影。无效排序路径在加入描述前拒绝。排序复用原生注册元数据与比较器，支持嵌套路径及嵌套对象通知／替换。

`tree.Filter` 是直接命中谓词；原有列筛选继续调用 `DataGridFeatures.ApplyFilter`，与业务谓词组合为 AND。查询遍历全部已加载节点，包括折叠分支。显示命中节点及其祖先，父节点命中不使子节点自动通过。命中路径临时强制展开；该路径暂不可折叠，清除后恢复用户的展开状态。`IsFilterMatch(item)` 区分直接命中与仅作路径上下文的祖先。

有效编辑在提交前保持行位置，提交后再排序／筛选；无效编辑阻止表头查询。独立视图必须先完成编辑再设置谓词或排序。谓词抛错时 setter 拒绝并保留旧谓词；排序失败／取消恢复旧排序描述，`QueryError` 报告原因；模型变化引起的查询失败保留最后有效投影，下一次成功刷新清除错误。任意业务委托无法自动识别其依赖对象；嵌套通知自动跟随已声明的列绑定和排序路径，其他外部依赖改变后调用 `RefreshTree()`。

## 树统计

| 配置 | 范围 |
| --- | --- |
| `SummaryScope=SourceItems` | 全部已加载节点，忽略筛选和折叠 |
| `SummaryScope=FilteredItems`（默认） | 直接满足业务与列条件的节点；不计只为上下文保留的祖先，也不受折叠影响 |
| `SummaryScope=VisibleItems` | 当前实际展开显示的节点，包含路径祖先，随折叠变化 |
| `SummaryLeavesOnly=True`（默认 false） | 在所选范围中再排除有实际子节点的模型，防止父级汇总与子项重复累加 |

列仍使用现有 `SummaryEnabled`／`SummaryKind`／`SummaryFormat` 与编译 getter。叶节点按原始树判断，即使筛选隐藏其所有子节点，父节点仍不是叶节点。普通 DataGrid 的所有行都视为叶节点，`VisibleItems` 使用原生集合视图（包含分页）的当前枚举。

`SourceCollection` 继续是原生使用的只读可见投影，不代表整棵树；业务遍历使用 `LoadedItems`，由独立的可选 `IDataGridTreeQueryView` 契约提供，原有树呈现接口不增加必需成员。树文本单元格提供展开／折叠自动化模式、名称和层级帮助文本，且通知状态变化；原生行与列的自动化结构保留。自动化模式测试不代表已完成各平台读屏器实跑。

## 限制

本阶段仍不支持分组、分页、AddNew、视图删除、异步加载或跨父节点拖放。通过业务根／子集合增删节点。全部已加载模型有索引及必要通知订阅，行虚拟化不等于数据虚拟化。普通集合／外部查询依赖使用 `RefreshTree()`，不以继承的原生 `Refresh()` 替代。

原生文本编辑器保持 TextBox，不用容器替换编辑目标；编辑取消能力继续遵循 `IEditableObject` 原生契约。树列缩进默认 16，`Indent` 可设置非负有限值。首版应在创建列时配置缩进。

## PreviewHost 验证

正常启动 PreviewHost 后，在 **数据展示 → DataGridTree** 打开树表格，也可以搜索 `DataGridTree`。页面复用树表格专项示例，切换页面后保留展开和选择状态。

```bash
dotnet build design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/datagrid-tree-stage2/artifacts
dotnet .preview/datagrid-tree-stage2/artifacts/bin/PreviewHost/debug/PreviewHost.dll --verify-datagrid-tree-query .preview/datagrid-tree-stage2/frames
dotnet .preview/datagrid-tree-stage2/artifacts/bin/PreviewHost/debug/PreviewHost.dll --verify-datagrid-tree
dotnet .preview/datagrid-tree-stage2/artifacts/bin/PreviewHost/debug/PreviewHost.dll --datagrid-tree
```

查询专项覆盖同级稳定排序、多列延迟提交、嵌套通知、失败回滚、真实表头／筛选弹层、三种统计范围、叶节点、活动查询中的原生编辑和自动化展开。第一阶段树专项保持独立。绘制覆盖明暗主题、三档尺寸、宽窄视口。只运行受本次改动影响的 DataGrid 入口。

```bash
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 --artifacts-path .preview/datagrid-tree-stage2/aot-artifacts -o .preview/datagrid-tree-stage2/native
.preview/datagrid-tree-stage2/native/NativeAotSmoke --verify-datagrid-tree-query --require-aot
```

这是当前 macOS 宿主的原生消费入口，不能据此宣称 Windows／Linux 实跑通过。现有普通表格回归入口仍见 [Program.cs](../../../design/PreviewHost/Program.cs)。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 在各控件页直接标明示例的 Classes、尺寸及状态；公共属性、API／事件和模型字段使用 Description 组件说明，包含属性类型、默认值、绑定模式、校验条件及 API 签名。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### DataGridTreeChangingEventArgs

[API / model reference](../../api/zh-cn/Avalonia.Collections.DataGridTreeChangingEventArgs.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Expanded`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L25) | `bool?` | 只读; CLR / n/a | `expanded` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Item`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L24) | `object?` | 只读; CLR / n/a | `item` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### DataGridTreeCollectionView

[API / model reference](../../api/zh-cn/Avalonia.Collections.DataGridTreeCollectionView_T_.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CanAddNew`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L80) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanChangePage`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L86) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanFilter`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L83) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanGroup`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L85) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanRemove`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L81) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`CanSort`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L82) | `bool` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Filter`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L55) | `System.Func<object, bool>` | 可写; CLR / n/a | `accessor-defined / state` | Direct match predicate. Ancestors of matches remain visible without matching this predicate. |
| [`LoadedItems`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L96) | `System.Collections.IEnumerable` | 只读; CLR / n/a | `accessor-defined / state` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |
| [`PageSize`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L87) | `int` | 可写; CLR / n/a | `accessor-defined / state` | 每页行数；树形视图不支持分页，此成员遵循该视图的限制。 |
| [`QueryError`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L25) | `string?` | 只读; CLR / n/a | `accessor-defined / state` | Failure of a live query or sort change. The last valid projection is retained. |
| [`SortDescriptions`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.Query.cs#L27) | `Avalonia.Collections.DataGridSortDescriptionCollection` | 只读; CLR / n/a | `accessor-defined / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### DataGridTreeNodeState

[API / model reference](../../api/zh-cn/Avalonia.Collections.DataGridTreeNodeState.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Depth`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L15) | `int` | 可写; CLR / init-only | `required constructor argument` | 节点深度，根级从 0 开始；用于计算缩进。 |
| [`HasChildren`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L15) | `bool` | 可写; CLR / init-only | `required constructor argument` | 节点是否存在子项；用于显示展开入口，不代表当前已展开。 |
| [`IsExpanded`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L15) | `bool` | 可写; CLR / init-only | `required constructor argument` | 当前节点展开状态；需要在回收或数据重新加载后保留时，将其绑定到稳定模型。 |
| [`IsFilterMatch`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L18) | `bool` | 可写; CLR / init-only | `false` | 当前节点是否直接匹配树筛选条件；祖先保留与直接命中分开。 |
| [`IsForcedExpanded`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L17) | `bool` | 可写; CLR / init-only | `false` | 筛选是否强制展开当前节点以呈现匹配后代；与用户保存的展开状态分开。 |
| [`Parent`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L15) | `object?` | 可写; CLR / init-only | `required constructor argument` | 树节点的直接父对象；根节点为空，保持原始模型身份。 |

### IDataGridTreeCollectionView

[API / model reference](../../api/zh-cn/Avalonia.Collections.IDataGridTreeCollectionView.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

### IDataGridTreeQueryView

[API / model reference](../../api/zh-cn/Avalonia.Collections.IDataGridTreeQueryView.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`LoadedItems`](../../../vendor/Avalonia.Controls.DataGrid/Collections/DataGridTreeCollectionView.cs#L41) | `System.Collections.IEnumerable` | 只读; CLR / n/a | `accessor-defined / state` | 模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。 |

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

### DataGridTreeFeatures

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridTreeFeatures.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsEnabled`](../../../TheOne.Avalonia/Controls/DataGridTreeFeatures.cs#L14) | `bool` | 可写; OneWay; 附加属性 | `false` | 业务可用性；不可用项的选择、移除和命令行为遵循控件禁用规则。 |

### 公共附加属性

[ControlSizing](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSizing.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | 可写; OneWay; 附加属性; 向后代继承 | `ControlSize.Normal` | Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 可选值：`Small`, `Normal`, `Large` |

[Motion](../../api/zh-cn/TheOne.Avalonia.Styling.Motion.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | 可写; OneWay; 附加属性; 向后代继承 | `true` | 向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| DataGrid | `borderless` | 显式 Classes，可与尺寸类组合 | ListBox.borderless, DataGrid.borderless => BorderThickness=0 |
| DataGrid | `one-large` | 模板自动类；不手动绑定 | DataGrid.one-large => FontSize=16 |
| DataGrid | `one-normal` | 模板自动类；不手动绑定 | DataGrid.one-normal => FontSize=14 |
| DataGrid | `one-small` | 模板自动类；不手动绑定 | DataGrid.one-small => FontSize=12 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`Avalonia.Collections.DataGridTreeChangingEventArgs`](../../api/zh-cn/Avalonia.Collections.DataGridTreeChangingEventArgs.md) | Class | 2 | 1 | 0 |
| [`Avalonia.Collections.DataGridTreeCollectionView<T>`](../../api/zh-cn/Avalonia.Collections.DataGridTreeCollectionView_T_.md) | Class | 11 | 8 | 3 |
| [`Avalonia.Collections.DataGridTreeNodeState`](../../api/zh-cn/Avalonia.Collections.DataGridTreeNodeState.md) | Struct | 6 | 2 | 0 |
| [`Avalonia.Collections.IDataGridTreeCollectionView`](../../api/zh-cn/Avalonia.Collections.IDataGridTreeCollectionView.md) | Interface | 0 | 2 | 2 |
| [`Avalonia.Collections.IDataGridTreeQueryView`](../../api/zh-cn/Avalonia.Collections.IDataGridTreeQueryView.md) | Interface | 1 | 2 | 1 |
| [`TheOne.Avalonia.Controls.DataGridColumnOptions`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridColumnOptions.md) | Class | 8 | 16 | 0 |
| [`TheOne.Avalonia.Controls.DataGridFeatures`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFeatures.md) | Class | 7 | 19 | 0 |
| [`TheOne.Avalonia.Controls.DataGridFilterApplyResult`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFilterApplyResult.md) | Struct | 2 | 2 | 0 |
| [`TheOne.Avalonia.Controls.DataGridFilterCondition`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFilterCondition.md) | Class | 1 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridFilterKind`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridFilterKind.md) | Enum | 4 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridSummaryKind`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridSummaryKind.md) | Enum | 5 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridSummaryResult`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridSummaryResult.md) | Class | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridSummaryScope`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridSummaryScope.md) | Enum | 3 | 1 | 0 |
| [`TheOne.Avalonia.Controls.DataGridTreeFeatures`](../../api/zh-cn/TheOne.Avalonia.Controls.DataGridTreeFeatures.md) | Class | 1 | 3 | 0 |
| [`TheOne.Avalonia.Controls.IDataGridValueAccessor`](../../api/zh-cn/TheOne.Avalonia.Controls.IDataGridValueAccessor.md) | Interface | 1 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
