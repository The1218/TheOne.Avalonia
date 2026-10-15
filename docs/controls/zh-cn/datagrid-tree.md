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
