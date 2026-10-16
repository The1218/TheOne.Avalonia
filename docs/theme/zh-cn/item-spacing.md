# 项表面间距

**简体中文** · [English](../en/item-spacing.md)

列表和导航中的可选择项表面与外框至少相隔 4px，相邻表面间隔 4px；弹层原有的更大留白继续保留。横向排列时，每项左右各 2px 形成 4px 项间距，由父容器补齐边缘留白。嵌套结构在可见标题表面设置间距，避免整个分支的边距随深度累加。

| 控件族 | 实现 |
| --- | --- |
| ListBoxItem / ComboBoxItem | `Margin="4,2"` |
| TreeViewItem / NavMenuItem | 标题表面 `PART_LayoutRoot` 使用 `Margin="4,2"`，保留原生子项缩进 |
| AutoCompleteBox / MultiSelectBox 结果列表 | 继承 ListBoxItem 间距 |
| 纵向 MenuItem | `Margin="0,2"`；ContextMenu 和子菜单保留 6px 外框内边距 |
| Menu / 横向菜单浮层 | 项使用 `Margin="2"`；菜单栏父容器补 2px 内边距，浮层保留 6px 内边距 |
| MenuFlyoutPresenter | 默认 6px 内边距传递给实际边框，局部 Padding 设置仍然有效 |
| 普通 TabControl / TabItem | 标题滚动区留 2px 内边距、项留 2px 外边距，支持上／下／左／右四种位置 |
| TabStripItem | `Margin="2"`，兼顾横向和竖向面板；TabStrip 保留原有 4px 内边距 |

菜单栏使用最小高度并随菜单项自然增长，使三档尺寸都有完整留白。可以局部设置项的 Margin 或 Presenter 的 Padding 来覆盖默认值。

检查中也区分了下列结构：

- 日历格已有 2px 外边距，二维网格中相邻表面间隔 4px。
- Pagination 页码按钮已有 6px 间距，FlowStep 有显式节点间距；VerificationStep 使用指示器连线，不存在相邻填充选中表面。
- 卡片式标签页保留分隔线和选中页接缝；DataGrid 单元格保留连续网格和数据对齐；Gantt 的派生行主题显式设置零 Margin，避免继承列表间隔后产生时间轴网格缝隙；Carousel 页面保留整视口翻页，不追加列表边距。
- 通用 ItemsControl 的内容由调用方定义，没有默认选中表面，不在模板中强加项边距。WorkflowEditor 使用共享 TabStrip 主题，节点库已有显式间隔。

PreviewHost 提供独立入口，只运行受影响控件族：

```text
--verify-menu-item-spacing [output]
--verify-tab-item-spacing [output]
--verify-tabstrip-item-spacing [output]
--verify-gantt [output]
```

覆盖明暗主题、Small / Normal / Large、宽窄视口、实际表面与间距像素、禁用项以及原生按键／指针交互。菜单专项包含 ContextMenu、纵向／横向浮层和嵌套子菜单；TabControl 覆盖四种位置，TabStrip 覆盖两种面板方向。
