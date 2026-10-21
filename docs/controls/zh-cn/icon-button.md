# 图标按钮与操作层次

历史命令行验证入口与消费测试项目已移除，文中的专项覆盖和测量结果为历史记录。请正常启动 PreviewHost，从菜单进入对应控件演示页；目标平台行为需在实际应用中验证。

**简体中文** · [English](../en/icon-button.md)

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

`Icon` 现可直接传入或绑定 `OneIconKind`，如 `Icon="{x:Static one:OneIconKind.Save}"`；内置模板创建独立 OneIcon。`IconTemplate` 仍可自定义 Filled、业务模型或第三方图标，且优先于内置回退；见 [OneIcon 图标宿主](one-icon.md#图标宿主与-icontemplate)。

<!-- visual-guide:start -->
## 外观与场景图示

[配图口径与控件图鉴](visual-index.md)；下图是当前源码的真实渲染，绑定与交互以本页用法说明为准。

### IconButton

![IconButton — 图标与正文在各 Classes 下对齐；图标来源、位置和间距可独立配置。](../images/IconButton-light.png)

图标与正文在各 Classes 下对齐；图标来源、位置和间距可独立配置。

[查看 Dark 对照](../images/IconButton-dark.png)

<!-- visual-guide:end -->

## 语义色与外观组合

IconButton 与 Button、ToggleButton、RepeatButton 共用[组合及优先级规则](basic.md#button语义色与外观组合)。颜色 `primary/accent`、`danger`、`success`、`warning`、`info` 可组合 `solid`、`ghost`、`link`、`outline`、`soft`、`gradient`；例如：

```xml
<one:IconButton Classes="danger outline" Icon="{CompiledBinding DeleteIcon}"
                Content="删除" Command="{CompiledBinding DeleteCommand}" />
<one:IconButton Classes="success soft" Icon="{CompiledBinding CheckIcon}"
                Content="完成" IsLoading="{CompiledBinding IsSaving}" />
```

单独 Classes 保留旧的启用外观；所有组合禁用时统一为 DisabledSurface / DisabledText / Line。颜色冲突顺序为 info > warning > success > danger > primary/accent，外观为 gradient > soft > outline > link > ghost > solid，与书写顺序无关。动态切换 Classes、Light/Dark 或 ColorTheme 同步刷新图标与文字前景，不改变 IconPlacement、显式 IconSize/IconSpacing、IconTemplate 和原生命令行为。加载时仍以指示器替换图标并阻止重复操作。PreviewHost 的独立 IconButton 页提供组合、禁用、位置和加载演示。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### ButtonLoading

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.ButtonLoading.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/ButtonLoading.cs#L12) | `bool` | 可写; OneWay; 附加属性 | `false` | 显示忙碌指示；按钮保留尺寸并阻止重复操作，容器显示加载层。任务完成后由宿主复位。 |

### IconButton

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.IconButton.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`HasIcon`](../../../TheOne.Avalonia/Controls/IconButton.cs#L89) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`HasLabel`](../../../TheOne.Avalonia/Controls/IconButton.cs#L95) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`Icon`](../../../TheOne.Avalonia/Controls/IconButton.cs#L65) | `object?` | 可写; OneWay | `null` | 图标内容或矢量几何，具体类型见签名；外部位图所有权仍由宿主负责。 |
| [`IconPlacement`](../../../TheOne.Avalonia/Controls/IconButton.cs#L53) | `TheOne.Avalonia.Controls.IconPlacement` | 可写; OneWay | `0 / default` | 图标相对文本的位置枚举；左右或上下布局由模板处理。 校验：`value => Enum.IsDefined(value)` 可选值：`Left`, `Right`, `Top`, `Bottom` |
| [`IconSize`](../../../TheOne.Avalonia/Controls/IconButton.cs#L77) | `double` | 可写; OneWay | `15` | 图标绘制尺寸，单位 DIP；独立于文本字号与按钮点击区域。 校验：`value => double.IsFinite(value) && value > 0` |
| [`IconSpacing`](../../../TheOne.Avalonia/Controls/IconButton.cs#L83) | `double` | 可写; OneWay | `8` | 图标与相邻文本／内容的 DIP 间距。 校验：`value => double.IsFinite(value) && value >= 0` |
| [`IconTemplate`](../../../TheOne.Avalonia/Controls/IconButton.cs#L71) | `Avalonia.Controls.Templates.IDataTemplate?` | 可写; OneWay | `null` | 原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。 |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/IconButton.cs#L59) | `bool` | 可写; OneWay | `false` | 显示忙碌指示；按钮保留尺寸并阻止重复操作，容器显示加载层。任务完成后由宿主复位。 |
| [`ShowIcon`](../../../TheOne.Avalonia/Controls/IconButton.cs#L47) | `bool` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### ThemeButton

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeButton.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Scope`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L30) | `TheOne.Avalonia.Controls.ThemeButtonScope` | 可写; OneWay | `0 / default` | 主题按钮切换的作用域；影响 Light/Dark，不切换 ThemeColor 颜色枚举。 可选值：`Window`, `Application` |

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

- `IconButton`: `Classes="danger"` / `Classes="ghost"` / `Classes="gradient"` / `Classes="info"` / `Classes="link"` / `Classes="outline"` / `Classes="soft"` / `Classes="solid"` / `Classes="success"` / `Classes="warning"`

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

没有额外业务方法；通过本节专属属性配置。

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.ButtonLoading`](../../api/zh-cn/TheOne.Avalonia.Controls.ButtonLoading.md) | 1 |
| [`TheOne.Avalonia.Controls.IconPlacement`](../../api/zh-cn/TheOne.Avalonia.Controls.IconPlacement.md) | 4 |
| [`TheOne.Avalonia.Controls.ThemeButtonScope`](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeButtonScope.md) | 2 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
