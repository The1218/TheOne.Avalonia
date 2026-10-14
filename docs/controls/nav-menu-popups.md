# NavMenu 侧栏与级联弹层

基础接入见[NavMenu](nav-menu.md)。

## 折叠侧栏与子菜单弹层

设置 `IsCollapsed="True"` 后，主导航只显示根菜单图标，隐藏文字、缩进、行内箭头及子级区域。没有 IconTemplate 时提供默认图形，避免把模型文字挤进图标栏；有子级的图标默认悬停打开菜单；没有子级的图标仍通过原内容模板显示提示。折叠不修改模型的 IsExpanded 或当前 SelectedItem。

```xml
<one:NavMenu IsCollapsed="{Binding NavigationCollapsed, Mode=TwoWay}"
             ItemsSource="{Binding MenuRoots}"
             ChildrenBinding="{Binding Entries}"
             ExpandedBinding="{Binding IsExpanded, Mode=TwoWay}"
             SelectedItem="{Binding SelectedMenu, Mode=TwoWay}"
             IconTemplate="{StaticResource MenuIconTemplate}"
             ContentTemplate="{StaticResource MenuContentTemplate}" />
```

悬停有子级的根图标默认等待 100ms 后打开右侧弹层；点击图标或在其获得焦点后按 Right / Enter / Space 立即打开。没有子级的普通项目直接导航；分组图标打开子级，不替换当前页面。每一级弹层使用有界 NavMenu，仅展示该级集合。悬停、点击右侧箭头或按 Right 继续打开下一层；不会展开行内子项，也不修改模型 IsExpanded。每级独立按视口创建与回收容器。

弹层中的选择同步到根导航，调用叶子项时只执行一次根 ItemInvoked / ItemInvokedCommand，然后关闭。选中子级时根图标保持强调状态。Left / Esc 关闭当前一级并将键盘焦点返回上一层锚点；点击弹层外、展开侧栏、禁用父级、根数据源替换、锚点容器回收或控件卸载都会关闭并释放弹层；显式打开的弹层关闭后焦点返回锚点；悬停打开不抢占当前焦点。关闭再打开时，双向模型绑定和同一模型身份的默认弱缓存保留展开状态。

弹层使用 Avalonia 原生 Flyout 的窗口内 overlay 宿主，位置受当前窗口约束。主题、密度、图标模板、内容模板、行高及 Motion.Enabled 在弹层打开期间也能实时更新。折叠侧栏本身立即隐藏行内子级，弹层继承 Motion.Enabled，悬停延迟独立于动效开关，关闭动效后仍可悬停导航。

| 属性 | Small | Normal | Large |
| --- | ---: | ---: | ---: |
| CollapsedWidth | 44 | 52 | 60 |
| SubmenuWidth | 240 | 280 | 320 |
| SubmenuMaxHeight | 240 | 320 | 400 |

以上属性支持 Style、绑定或局部覆盖。折叠宽度由主题设置 Width；若消费方显式设置了固定 Width，局部值会优先，应同时调整容器列宽和控件 Width。父级可用 Grid 的 Auto 列，让图标栏按实际密度占用空间。

```csharp
menu.OpenSubmenu(0);        // 打开第一个根项目的子级。
menu.SelectPath(0, 0, 999); // 折叠模式下自动打开弹层，并定位三级末项。
menu.CloseSubmenu();
```

IsSubmenuOpen 与 Submenu 是只读可观察属性；Submenu 在打开时指向当前一级的虚拟化子导航，关闭后为 null。继续读取 Submenu.Submenu 可观察已打开的级联层。不要保存其容器引用作为模型状态。

PreviewHost 的标题栏“☰”可切换真实主导航；扩展控件 → NavMenu 页面提供折叠开关、打开／关闭弹层、悬停开关、千条根／二级／三级末项定位及各级实时容器数。顶部尺寸和主题切换可在弹层打开时验证。定向导出覆盖 NavMenu 展开／级联、边缘翻转与窄窗口 × 三尺寸 × 明暗主题，共 48 张当前控件图片（含多级搜索场景和动效中间帧）；导出 PreviewHost 时另有主导航截图。

## 悬停与级联配置

| 属性 | 默认值 | 行为 |
| --- | --- | --- |
| OpenOnHover | true | 折叠根图标和各级弹层中的分支支持悬停打开；false 时仍可点击、键盘或调用 API。 |
| HoverOpenDelay | 00:00:00.100 | 快速经过图标不打开，停留后才打开；可设为零。 |
| HoverCloseDelay | 00:00:00.300 | 离开锚点与弹层链后延迟关闭，进入任一级弹层或返回锚点取消待关闭操作。 |

这些 StyledProperty 支持 Style、编译绑定和局部值；延迟非负，自动传递到所有级联层。移到兄弟叶子项后只关闭旧的下一级；移到兄弟分支后替换对应子弹层。移动经过锚点与弹层的间隙时有关闭缓冲。触摸点击沿用显式操作，不触发鼠标悬停计时。

调用最深层叶子只向根导航发送一次 ItemInvoked / ItemInvokedCommand，并关闭整条链。锚点回收、禁用、移除最后一个子项、数据源替换和控件卸载会清理对应弹层及计时器，防止旧模型在延迟结束后重新弹出。

专项鼠标验证：

执行命令与测试范围见[测试策略](../workflow/testing.md)。

`--verify-navmenu`、`--verify-controls` 和 NativeAOT 消费验证同样包含悬停与级联场景，覆盖三尺寸 × 明暗主题、跨层移动、延迟关闭取消、快速切换、逐级键盘返回、千项远距离选择、回收与卸载后的计时器清理。

## 窗口边缘与窄窗口

弹层优先沿上一层方向打开。空间不足且另一侧不会覆盖可见祖先菜单时自动翻转；根图标旁的弹层也会按剩余空间缩窄。宽高与位置限制在窗口客户区内，实时缩放时重新计算。

无法并排容纳时采用单级展示：暂时隐藏父层并关闭其命中测试，下一层显示返回按钮。返回恢复父层的选择和滚动状态；鼠标停在返回后露出的原分支时不会立即重开，离开再进入恢复悬停。Left / Esc 同样逐级返回。加宽窗口后恢复级联。

只读 Avalonia 属性 IsCompactSubmenu 和 SubmenuOpensLeft 可用于观察布局。鼠标移动按可见弹层链判断，布局改变时保留关闭缓冲，避免翻转或缩放后误关闭。每一级仍保留有限视口与容器回收。

PreviewHost → 扩展控件 → NavMenu 提供窗口宽度、靠右选项和边缘预览窗口，每级 1,000 项。`--verify-navmenu-edge` 覆盖三尺寸、明暗主题、左右边缘、260／420／900 宽度、实时宽高变化、返回及远距离选择；完整 NavMenu 验证包含相同检查。
