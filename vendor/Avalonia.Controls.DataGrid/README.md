# DataGrid 原生布局扩展

基于 Avalonia.Controls.DataGrid 12.1.2，保留 Avalonia.Controls.DataGrid 程序集名、官方类型、原生 Presenter 与 PART_*。此源码项目替代官方 DataGrid NuGet，供 TheOne 的右侧冻结使用；消费项目不要再同时引用官方同名包。

上游提交与原始文件 SHA256 见 [UPSTREAM.json](UPSTREAM.json)，许可见 [LICENSE.md](LICENSE.md)；原始版权头保留。Avalonia 核心仍为 12.1.4。

NuGet 包版本与主库共用根目录 [PackageVersions.props](../../PackageVersions.props)，不表示上游版本；程序集名、12.1.2.0 版本和强名称保持不变。消费方应一起升级 TheOne.Avalonia 和 TheOne.Avalonia.DataGrid。

变更集中于：

- DataGridRowMetadata.cs、ReflectionHelper.cs、集合排序与视图：显式注册公共属性和构造函数，强类型比较器替代动态泛型构造；编辑、排序、分组和索引器共用注册元数据。
- DataGridValueConverter.cs、DataGridColumns.cs：基础类型明确转换与绑定错误，自动列生成编译属性访问器；启用 AOT 分析，不抑制裁剪警告。

- DataGridColumnValueReader.cs：复用编译属性 getter 自动读取实体原始值，保持空源类型和数值精度，不反射或创建 UI 探针。
- DataGrid.ColumnLayout.cs：对外提供统一列位置／可见区，统计行与表头／冻结区保持一致。
- DataGridBoundColumn.cs：Binding 更换通知，自动取值重新注册。
- DataGrid.RightFrozen.cs：右区属性、有效冻结成员、三区几何与中央滚入／滚动定位。
- DataGridColumns.cs、DataGridColumn.cs、DataGrid.cs：冻结状态、列增删／隐藏／重排、滚动条视口。
- DataGridCellsPresenter／DataGridColumnHeadersPresenter：原生测量、排列与双侧裁剪。
- Collections/DataGridCollectionView.Generic.cs：泛型视图继承原生集合视图，保留排序／筛选／分组并提供编译列类型推断；基类开放继承。
- DataGridColumn.cs：修饰键排序入口、三态 CycleSort、方向／优先级读取及列排序能力变更通知。
- DataGridColumnHeader.cs：冻结区内拖动、原始抓取位置、指针捕获及 Esc／越界释放／卸载清理；表头 Presenter 使用测量尺寸排列拖影和插入提示。三态排序复用提交与 Sorting 事件，Shift 保留其他列优先级。
- DataGridRow.cs：数据视图重置时避免选择状态的重复回写，保留槽位断言。
- 单元格／表头 AutomationPeer：被完全裁剪的元素不作为屏内元素报告。

右侧冻结为零时保留上游布局路径。升级时先比较上游变更，再运行 DataGrid 相关专项与实际 NativeAOT，不能只更换版本号。当前实现、窄窗口规则和配置示例见 [使用文档](../../docs/controls/datagrid-extensions.md)。

自动取值使用 Avalonia 12.1.4 的 friend API；升级核心版本时必须检查编译路径元素与 IPropertyInfo 合约，并运行自动字段与原生专项。

使用属性路径前调用 `DataGridAot.PreserveRowType<T>()`，只引用此包时使用 `DataGridRowMetadata.Register<T>()`。嵌套模型和自定义比较值逐个注册，自定义转换提供列 Converter。完整契约见 [DataGrid NativeAOT](../../docs/controls/datagrid-binding.md#nativeaot)。
