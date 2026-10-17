# NavPage 导航页面

**简体中文** · [English](../en/nav-page.md)

NavPage 集成左侧 NavMenu 和右侧内容区。右侧由可选的 `RightHeader` 与内置 `TabControl Classes="card"` 的 RightBody 区域组成。模型实例标识一个页面，重复打开只激活原页；已打开的视图保留，切换标签不重建编辑器。关闭后释放控件持有的引用，宿主负责 ViewModel 的资源生命周期。

```csharp
var home = new NavPageModel { Title = "首页", ViewModel = homeVm, IsClosable = false };
var editor = new NavPageModel { Title = "编辑", ViewModel = editorVm, Icon = documentGeometry };
var shell = new NavPage
{
    HeaderTitle = "THE ONE", HeaderIcon = brandGeometry,
    DefaultHome = home, ItemsSource = new[] { home, editor }
};
```

使用宿主 DataTemplates 将 ViewModel 映射为视图，或指定 PageTemplate。放在具有有限视口的区域，例如 Grid 的 `*` 行，库内不写死整体高度。

## 附加属性

| 属性 | 说明与简短示例 |
| --- | --- |
| `ItemsSource` | 导航集合，默认项为 NavPageModel。`ItemsSource = routes` |
| `DefaultHome` | 模板就绪后自动打开的首页，关闭权限取自模型。`DefaultHome = home` |
| `ViewModelBinding` | 自定义导航模型的 ViewModel 来源。`CompiledBinding.Create<Route, object?>(r => r.Screen)` |
| `TitleBinding` / `IconBinding` / `ChildrenBinding` | 自定义模型的标题、图标和子级编译绑定；默认读取 NavPageModel。见下节 |
| `PageTemplate` | 页内容模板；未指定时沿用宿主 DataTemplates。`<one:NavPage.PageTemplate><DataTemplate x:DataType="vm:EditorVm"><views:EditorView /></DataTemplate></one:NavPage.PageTemplate>` |
| `HeaderTitle` / `HeaderIcon` | 默认品牌栏文字／图标；图标点击展开收缩，收缩时只展示图标。`HeaderTitle="业务系统"` |
| `RightHeader` | 右侧顶部的自定义控件或模板数据，默认 null；null 时不占高度，RightBody 占满右侧。`RightHeader = toolbar` |
| `RightHeaderTemplate` | RightHeader 数据的显示模板，模板数据为 RightHeader 本身。`RightHeaderTemplate = toolbarTemplate` |
| `HeaderTemplate` / `FooterTemplate` | 侧栏顶底模板，数据为 NavPage。自定义顶栏需提供自己的切换入口。见下节 |
| `IsCollapsed` | 支持 TwoWay 的收缩状态。`IsCollapsed="{Binding Compact, Mode=TwoWay}"` |
| `ExpandedWidth` / `CollapsedWidth` | 侧栏宽度，默认 260 / 64 DIP。`ExpandedWidth="280"` |
| `TransitionDuration` | 主题默认 One.Motion.Expand；零时长或 Motion.Enabled=False 立即切换。`TransitionDuration="0:0:0.2"` |
| `ShowItemIcon` | 是否展示页签图标，默认 True。`ShowItemIcon="False"` |
| `SelectedPage` | 只读当前导航源模型。`Text="{Binding #Shell.SelectedPage}"` |

通用尺寸和动效见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)。图标默认接受 Geometry 或文本，复杂品牌图标可用 HeaderTemplate。

## 自定义模型与模板

右侧工具栏按内容确定高度，RightBody 使用剩余空间。设置或清空 RightHeader 不会重建页签或页面；为 null 时即使指定了 RightHeaderTemplate，也不预留顶部空间。

```xml
<one:NavPage ItemsSource="{Binding Routes}">
  <one:NavPage.RightHeader>
    <StackPanel Orientation="Horizontal" Spacing="8" Margin="16,10">
      <TextBlock Text="工作区" VerticalAlignment="Center" />
      <Button Content="保存" Command="{Binding Save}" />
    </StackPanel>
  </one:NavPage.RightHeader>
</one:NavPage>
```

也可使用 `RightHeader="{Binding Toolbar}"` 搭配 `RightHeaderTemplate`，在 DataTemplate 中指定对应的 x:DataType。RightBody 是内置区域，不需要另行赋值。

不使用字符串反射路径，支持编译绑定与裁剪。自定义模型提供 Avalonia 或 INotifyPropertyChanged 通知。

```xml
<one:NavPage ItemsSource="{Binding Routes}" x:Name="Shell"
             ViewModelBinding="{CompiledBinding Screen, DataType=vm:Route}"
             TitleBinding="{CompiledBinding Name, DataType=vm:Route}"
             IconBinding="{CompiledBinding Glyph, DataType=vm:Route}"
             ChildrenBinding="{CompiledBinding Nodes, DataType=vm:Route}">
  <one:NavPage.HeaderTemplate>
    <DataTemplate x:DataType="one:NavPage">
      <ToggleButton Content="导航" IsChecked="{Binding IsCollapsed, Mode=TwoWay}" />
    </DataTemplate>
  </one:NavPage.HeaderTemplate>
  <one:NavPage.FooterTemplate>
    <DataTemplate x:DataType="one:NavPage"><TextBlock Text="业务系统 v1" Margin="12" /></DataTemplate>
  </one:NavPage.FooterTemplate>
</one:NavPage>
```

```csharp
shell.ViewModelBinding = CompiledBinding.Create<Route, object?>(r => r.Screen);
shell.TitleBinding = CompiledBinding.Create<Route, string>(r => r.Name);
shell.ChildrenBinding = CompiledBinding.Create<Route, IReadOnlyList<Route>>(r => r.Nodes);
```

ViewModel 为空的条目不会打开页面。自定义模型默认可关闭，也可右键固定。业务关闭保护通过 TabFeatures.TabClosingEvent 取消，args.Item 为 TabItem，其 DataContext 为导航源模型。

## 业务 API

```csharp
shell.OpenPage(editor);       // 同一实例只激活，不重复创建
shell.ClosePage(editor);      // 固定、不可关闭或被取消时返回 false
int count = shell.CloseAllPages(); // 保留固定、不可关闭和取消关闭的页面
```

右键含关闭当前页、固定到左侧／取消固定、关闭所有页。固定页排在普通页左侧并显示固钉图标，不能拖动或通过 Move／排序快捷键移动；普通页的拖动预览和放置位置均不能越过固定区。取消固定后恢复普通页行为。固钉不受 ShowItemIcon 影响。关闭按钮、快捷键、菜单和 API 使用相同关闭规则。

## 内置 NavPageModel

| 成员 | 说明 |
| --- | --- |
| `Title` | 导航与页签标题，默认空字符串 |
| `Icon` | 导航与页签图标，默认 null |
| `ViewModel` | 页面内容 ViewModel，默认 null |
| `Children` | 可观察子级集合，分组一般不设置 ViewModel |
| `IsClosable` | 默认 True；False 禁止关闭 |
| `IsPinned` | 默认 False；True 固定到左侧、显示固钉，禁止关闭和拖动，右键操作回写模型 |

模型继承 AvaloniaObject，提供属性变化通知。修改 ViewModel 更新打开页面的数据；关闭再打开创建新视图。更换 ItemsSource 更新菜单，已打开页面仍由关闭 API 管理，避免动态菜单更新丢失草稿。
