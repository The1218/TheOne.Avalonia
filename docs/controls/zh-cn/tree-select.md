# TreeSelect 树选择器

[English](../en/tree-select.md) · [控件目录](README.md) · [扩展路线图](../../plans/control-and-chart-roadmap.md)

加载 OneTheme 后使用 `TheOne.Avalonia.Controls.TreeSelect`。单选默认允许选择任意可用节点；多选可独立勾选或父子级联。选择保存稳定 Key，更换为同 Key 的新实例后，选中项指向新实例。使用有界原生虚拟 ListBox，不为所有节点创建 Control。

## 默认模型与绑定

```csharp
public ObservableCollection<TreeSelectNode> Departments { get; } =
[
    new() { Key = "engineering", Text = "研发中心", Children =
    [ new() { Key = "frontend", Text = "前端组" }, new() { Key = "backend", Text = "后端组" } ] }
];
```

```xml
<!-- 根视图声明实际 ViewModel 的 x:DataType；one 为 using:TheOne.Avalonia.Controls。 -->
<one:TreeSelect ItemsSource="{CompiledBinding Departments}"
                SelectedKey="{CompiledBinding DepartmentKey, Mode=TwoWay}" />
<one:TreeSelect IsMultiSelect="True" CheckMode="Cascade" MaxVisibleTags="1"
                ItemsSource="{CompiledBinding Departments}"
                SelectedKeys="{CompiledBinding DepartmentKeys, Mode=TwoWay}" />
```

TreeSelectNode 实现属性通知，包含 Key（object）、Text（string）、IsEnabled（bool，默认 true）、可替换的 Children（ObservableCollection&lt;TreeSelectNode&gt;?，默认空集合）。null Children 表示叶子。Key 非 null、按 Equals 全树唯一且稳定；默认生成的 Key 只在实例生命周期稳定，持久化和重载必须显式使用业务 Key，不要修改已挂载模型的 Key。

单选绑定 SelectedKey 或 SelectedItem，多选绑定 SelectedKeys，不要同时绑定到互相冲突的业务属性。SelectedItems 只读返回原始模型。多选时 SelectedKey/SelectedItem 表示第一项，写入任一属性会替换整个选择。切回单选保留第一项。SelectedKeys 支持 IList 整体 TwoWay 替换和可观察集合原地变更；尽量修改原可写集合，固定长度/只读集合需要时替换。重复和有效源中不存在的 Key 会被清理。

配置可在挂载前赋值；ItemsSource 尚为 null 时允许等待 Key，第一次非 null 源会清理缺失 Key。清空已加载的源会清空选择。

## 自定义模型与编译模板

```csharp
public TreeSelectItemAdapter DepartmentAdapter { get; } =
    TreeSelectItemAdapter.Create<Department>(
        item => item.Id, item => item.Name, item => item.Teams, item => item.IsEnabled);
```

```xml
<one:TreeSelect ItemsSource="{CompiledBinding Departments}"
                ItemAdapter="{CompiledBinding DepartmentAdapter}"
                SelectedKey="{CompiledBinding DepartmentKey, Mode=TwoWay}">
  <one:TreeSelect.ItemTemplate>
    <DataTemplate x:DataType="vm:Department">
      <TextBlock Text="{CompiledBinding Name}" />
    </DataTemplate>
  </one:TreeSelect.ItemTemplate>
</one:TreeSelect>
```

适配器依次指定稳定 Key、展示/搜索文字、IEnumerable? 子项和可选可用状态。使用强类型 getter，不按字符串反射属性，兼容裁剪。ItemTemplate / SelectedItemTemplate 接收原始模型，默认显示适配器文字；自定义模板声明自己的 x:DataType。无适配器时只接受 TreeSelectNode。

## 额外属性与勾选规则

| 属性 | 默认与说明 |
| --- | --- |
| ItemsSource / ItemAdapter | IEnumerable? 根集合 / 可选强类型模型映射 |
| IsMultiSelect / CheckMode | false / Independent；单选忽略勾选模式 |
| SelectedKey / SelectedItem | object?，TwoWay；单项 Key / 原始对象 |
| SelectedKeys | IList?，TwoWay；默认独立可观察集合 |
| ItemTemplate / SelectedItemTemplate | null；列表 / 标签模板 |
| SearchText | 空字符串，TwoWay；忽略大小写搜索，保留匹配节点的祖先 |
| PlaceholderText / SearchPlaceholderText | “请选择” / “搜索节点” |
| EmptyText / ClearText / OpenText | “无匹配节点” / “清空可编辑选择” / “展开或收起树”；空状态及可访问操作文字 |
| IsReadOnly / IsDropDownOpen | false / false；浏览不改变选择 / TwoWay 展开状态 |
| MaxVisibleTags | 3，0–100；单选显示一项，多选超出显示 +N，0 仅计数 |
| MaxDropDownHeight | 320 DIP，有限且 ≥80；列表视口高度上限 |

```xml
<one:TreeSelect ItemsSource="{CompiledBinding Departments}" IsMultiSelect="True"
                SearchText="{CompiledBinding Filter, Mode=TwoWay}" MaxVisibleTags="1"
                MaxDropDownHeight="280" IsReadOnly="{CompiledBinding IsLocked}" />
```

Independent 父子分别保存 Key。Cascade 切换父项的全部可用叶子，Key 仅保存叶子；程序传父 Key 时展开为可用叶子 Key。父项从可用叶子派生 true / false / null 三态。禁用节点的子树不能编辑但仍可展开查看；预选禁用叶子不能移除或清空，不参与父项全选判断。搜索不缩小级联范围，清空搜索恢复原展开状态。

无额外公开外观 Classes；明暗、配色和三档尺寸遵循 Home。只读状态：VisibleItems、DisplayTags、SelectedItems、SelectedCount、HiddenSelectedCount、HasSelection、HasHiddenSelection、HasNoResults、CanClear、ValidationError、ItemHeight。TreeSelectItem 是展示行，含 Item、Key、Text、Depth/Indent、HasChildren、IsExpanded、IsEnabled、IsChecked、ShowCheckBox、CanRemove、展开/移除文字和模板展示信息；持久化保存模型 Key，不保存展示行。

## 业务 API 与输入

```csharp
tree.SetExpanded("engineering", true); // bool；搜索时不改写展开状态
bool opened = tree.OpenDropDown();
tree.CloseDropDown();
bool changed = tree.ToggleItem(department); // 原始模型，遵守只读和禁选
bool cleared = tree.ClearSelection(); // 保留已选禁用项
tree.RefreshItems(); // 无通知模型变化后显式重新读取
tree.SelectionChanged += (_, e) => SaveKeys(tree.SelectedKeys);
```

SelectionChanged 为冒泡事件，AddedItems/RemovedItems 为原始模型。API、模型与集合变更在 UI 线程执行。

Enter/Space/↓、Alt+↓ 或 F4 打开；搜索中 ↓ 进入列表，列表 ↑↓ 移动、→ 展开、← 折叠或回父项、Enter/Space 选择；Esc 关闭返回焦点；Tab/Shift+Tab 继续焦点导航。字段聚焦且搜索为空时 Backspace 移除最后一个可编辑标签。展开按钮不改变选择。滚轮、触摸滚动和惯性阶段在弹层边界截断，防止影响外层页面。

## 集合生命周期与范围

可观察集合 Add/Remove/Move/Replace/Reset、根源/子项替换及 INotifyPropertyChanged 会重新核对索引；保留有效选择和展开 Key，旧集合/模型解除订阅。搜索只筛选缓存，不重新读取适配器或重建订阅。脱离视觉树解除订阅，重新挂载读取最新数据。

重复 Key、null 节点、循环或重复模型引用通过 ValidationError 报告且不显示旧行；修复可观察源后恢复。刷新会扫描整棵模型树，虚拟化的是行视图，不是懒加载数据。首版没有内置异步子项加载器、拖动排序和服务请求；宿主更新可观察 Children 并负责取消过期请求。极深层级缩进有上限，长路径可用自定义模板/提示补充。

独立 PreviewHost 页包含单选、独立多选、级联、自定义模型、标签上限及按需加载一万节点示例。

精确声明：[TreeSelect](../../api/zh-cn/TheOne.Avalonia.Controls.TreeSelect.md)、[默认模型](../../api/zh-cn/TheOne.Avalonia.Controls.TreeSelectNode.md)、[适配器](../../api/zh-cn/TheOne.Avalonia.Controls.TreeSelectItemAdapter.md)、[展示行](../../api/zh-cn/TheOne.Avalonia.Controls.TreeSelectItem.md)、[勾选枚举](../../api/zh-cn/TheOne.Avalonia.Controls.TreeSelectCheckMode.md)。
