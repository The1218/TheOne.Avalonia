# DataGrid 树视图原型

**简体中文** · [English](../en/datagrid-tree.md)

第一阶段原型：复用原生 DataGrid 的单元格、行回收、编辑事务和当前位置，增加本地树的展开与折叠。API 尚处于原型阶段。

```csharp
DataGridAot.PreserveRowType<Node>();
var tree = new DataGridTreeCollectionView<Node>(roots, node => node.Children);
// DataGridTreeCollectionView 位于 Avalonia.Collections。
```

```xml
<DataGrid ItemsSource="{Binding Tree}" AutoGenerateColumns="False"
          one:DataGridTreeFeatures.IsEnabled="True">
  <DataGrid.Columns>
    <one:DataGridTreeTextColumn Header="名称"
        Binding="{CompiledBinding Name, DataType={x:Type vm:Node}, Mode=TwoWay}" />
  </DataGrid.Columns>
</DataGrid>
```

视图和列始终使用原业务模型，不添加 `Item` 包装层；泛型视图为编译绑定提供行类型，空源也通过显式集合形状注册识别。创建视图前注册具体行及嵌套属性类型，见 [NativeAOT](datagrid-binding.md#nativeaot)。Children 通过强类型委托读取，不按属性名反射。一次只将一个树视图绑定到一个已挂载表格；独立状态需要独立视图。

## 第一阶段行为

- 默认折叠。`TrySetExpanded(node, true/false)` 返回是否成功；`TryGetNodeState` 查询深度、父节点、是否有子节点和展开状态。
- 只接受同类型、非空、无环、无共享对象实例的本地树；按引用识别节点。内部索引全部已加载节点，UI 只创建可见区域的原生行容器。
- 折叠保留后代展开状态；再次展开恢复。每次可见序列变化发送单项增删通知，不替换 ItemsSource、不发送范围通知或 Reset。
- 启用 `DataGridTreeFeatures.IsEnabled` 后，树变化先通过原生行提交。验证失败取消变化并保留编辑器；独立视图在编辑未结束时拒绝展开折叠。
- 单选后代被折叠隐藏时，选择和当前单元格回到折叠节点；多选移除隐藏后代，保留其他选择。展开不重新选回隐藏后代。
- 树列非编辑态下，右键展开／进入首个子节点，左键折叠／回到父节点；编辑器和其他列保留原生导航。
- `INotifyCollectionChanged` 的根与子集合均被监听，包括折叠分支；模型属性通知触发子集合替换检查。普通集合改变后调用 `RefreshTree()`。结构变化发生在未完成编辑期间时，保持旧投影，编辑结束后合并刷新。
- `TreeChanging` 可取消，`TreeChanged` 在成功应用后通知；源刷新时事件的 Item／Expanded 为 null。错误源在有效索引建立前被拒绝，调用方应修复源集合。
- 生命周期由消费方拥有：先解绑／关闭表格并结束编辑，再 `Dispose()` 视图以解除源集合及模型订阅。所有操作在 UI 线程调用。

## 限制

首版关闭排序、列／业务筛选、分组、分页、AddNew 和视图删除。修改业务根／子集合来增删节点。`SourceCollection` 是只读的可见投影，不代表整棵树；暂勿启用现有底部统计。树感知筛选、同级排序、完整统计范围、异步加载、拖放及完整无障碍展开模式不属于这一阶段。

原生文本编辑器保持 TextBox，不用容器替换编辑目标；编辑取消能力继续遵循 `IEditableObject` 原生契约。树列缩进默认 16，`Indent` 可设置非负有限值。首版应在创建列时配置缩进。

## PreviewHost 验证

```bash
dotnet build TheOne.Avalonia.slnx
dotnet run --project design/PreviewHost --no-build -- --verify-datagrid-tree .preview/datagrid-tree/frames
dotnet run --project design/PreviewHost --no-build -- --datagrid-tree
```

专项覆盖源变化、环／共享实例拒绝、编译绑定、鼠标／键盘展开折叠、编辑提交／取消／验证失败、单选／多选、回收、明暗主题、三档尺寸和窄视口截图。

```bash
SDKROOT="$(xcrun --sdk macosx --show-sdk-path)" dotnet publish design/PreviewHost/Consumers/NativeAotSmoke -c Release -r osx-arm64 -p:PublishAot=true -p:TrimmerSingleWarn=false -p:CppCompilerAndLinker="$(xcrun --find clang)" --disable-build-servers -m:1 -o .preview/datagrid-tree/native
.preview/datagrid-tree/native/NativeAotSmoke --verify-datagrid-tree --require-aot
```

这是当前 macOS 宿主的原生消费入口，不能据此宣称 Windows／Linux 实跑通过。现有普通表格回归入口仍见 [Program.cs](../../../design/PreviewHost/Program.cs)。
