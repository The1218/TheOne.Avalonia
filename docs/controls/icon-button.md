# 图标按钮与操作层次

IconButton 继承官方 Button，保留 Command、CommandParameter、Click、键盘、焦点、Flyout 与禁用行为。只需加载 OneTheme。Icon 为独立属性，Content 继续用于文字／内容；IconTemplate 用于数据图标。IconPlacement 支持 Left（默认）、Right、Top、Bottom，图标与文字始终居中排列，缺少图标或文字时不保留空白间隔。IsLoading 会用加载图标替换原图标并阻止重复操作，详见[加载控件](loading.md)。IconSize、IconSpacing 可通过样式或绑定覆盖，默认随 small / normal / large 使用 13 / 15 / 19px 图标。空 Icon 不占位，空 Content 使用方形图标按钮；纯图标按钮须配置 AutomationProperties.Name 与 Tooltip。

```xml
<one:IconButton Icon="{Binding AddGeometry}" Content="新建" Classes="primary"
                Command="{Binding AddCommand}" />
<one:IconButton AutomationProperties.Name="收藏" ToolTip.Tip="收藏">
  <one:IconButton.Icon><PathIcon Data="M8,1 L10,6 L15,6 L11,9 L13,15 L8,12 L3,15 L5,9 L1,6 L6,6 Z" /></one:IconButton.Icon>
</one:IconButton>
```

Geometry 由默认 PathIcon 模板渲染；已有 PathIcon 或其他图标 Control 可以直接传入 Icon。控件实例仅用于一个视觉父级，共享图标数据优先使用 Geometry 或模板。

Material 图标由消费项目引用 [Material.Icons.Avalonia](https://www.nuget.org/packages/Material.Icons.Avalonia/)，并在 Application.Styles 加载 MaterialIconStyles。类库不强制依赖第三方图标库；PreviewHost 引用 3.0.2 演示编译模板：

```xml
<!-- material: Material.Icons.Avalonia；kinds: Material.Icons -->
<one:IconButton Icon="{Binding IconKind}" Content="工作区">
  <one:IconButton.IconTemplate>
    <DataTemplate x:DataType="kinds:MaterialIconKind">
      <material:MaterialIcon Kind="{Binding}" />
    </DataTemplate>
  </one:IconButton.IconTemplate>
</one:IconButton>
```

官方 Button 与 IconButton 共用 primary/accent、soft、gradient、outline、ghost、link、success、info、warning、danger 样式。默认样式用于次要操作；primary 推进主要流程，ghost 为轻操作，outline 强调边界，success/info/warning/danger 表达完成、提示、风险与危险操作。悬停、按下、焦点、禁用保留原生状态与 TheOne 语义色。每组按钮宜只保留一个主要操作，文字明确说明操作。

编译绑定声明视图 x:DataType；数据 IconTemplate 声明图标数据类型。实现使用 StyledProperty、TemplateBinding 与编译模板，不反射创建图标。NativeAOT 验证边界与第三方包的验证见 [NativeAOT](../native-aot.md)。
