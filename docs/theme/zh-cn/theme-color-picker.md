# ThemeColorPicker 与明暗切换

**简体中文** · [English](../en/theme-color-picker.md)

配色与明暗模式是两个独立设置：`OneTheme.ColorTheme` 选择 `One.*` 颜色资源，`RequestedThemeVariant` 选择 Light / Dark。`ThemeColorPicker` 是原生 ComboBox 的专用派生控件，`ThemeButton` 是带内置图标的 ToggleButton。

加载 OneTheme 后，在 UI 线程传入枚举即可一键切换，无需查找窗口、资源字典或主题实例：

```csharp
using TheOne.Avalonia.Themes;

OneTheme.SetColor(ThemeColor.Atlas);
OneTheme.SetColor(ThemeColor.Nova); // 恢复默认 Nova。
```

此 API 更新应用配色，保留属性绑定，不改变 Light / Dark。无效枚举抛出 ArgumentOutOfRangeException；未在 Application.Styles 注册 OneTheme 时抛出 InvalidOperationException。

## 配色资源

```xml
<Application.Styles>
  <themes:OneTheme ColorTheme="Atlas" />
</Application.Styles>
<!-- themes: using:TheOne.Avalonia.Themes -->
```

`ThemeColor` 包含 `Nova`、`Atlas`、`AtlasDeep`、`Helios`、`HeliosDeep`、`NovaDeep`、`Terra`、`TerraDeep`、`Vega`、`VegaDeep`。默认项恢复为 **Nova**；原 Default 改名为 Nova，数值仍为 0，其余枚举数值不变。其余 9 套的 Light / Dark 语义颜色从 [Eremex 配色资源](https://github.com/Eremex/controlthemes/tree/5f6cb3ad8b904cead6bd432f62596648cc9a1039/Source/Eremex.Avalonia.Themes.DeltaDesign/PaletteResources)映射，未引入 Eremex 包或模板。

资源同时覆盖中性表面、正文、强调色、选中、焦点、边框及状态色。状态浅底、卡片 Footer 和渐变为 TheOne 派生色；图表基础色和六色系列沿用共享资源。配色切换不会重新创建视图、修改密度或切换 Light / Dark，Fluent 后备控件的强调色也同步更新。

## 下拉选择器

```xml
<one:ThemeColorPicker Width="170"
                     SelectThemeColor="{Binding Palette, Mode=TwoWay}" />
<!-- one: using:TheOne.Avalonia.Controls -->
```

VM 的 `Palette` 类型为 `ThemeColor`。也可不绑定，选择器会连接 `Application.Styles` 中直接注册的 `OneTheme`。每项展示名称和 Light / Dark 强调色色块；支持原生鼠标、方向键与 Enter 选择，沿用 ComboBox 的边框、项间距、禁用及三档尺寸。

这是枚举选择器：`ItemsSource` 和 `OneTheme.ColorThemes` 均为 `ThemeColor` 枚举列表，`SelectedItem` 是枚举值，不再使用 ThemeColorOption 包装对象。`SelectedColor` 提供强类型属性；也可通过原生 SelectedItem 读取选中的枚举。

`SelectThemeColor` 默认双向绑定，用户选择会更新应用 OneTheme；程序修改 `OneTheme.ColorTheme` 也会同步到已附加的选择器。设置配色应在 UI 线程执行。未找到应用 OneTheme 时，控件仍可选择，但不会改变其他主题库；局部 Styles 中的独立 OneTheme 应由应用自行设置 ColorTheme。

```csharp
using TheOne.Avalonia.Themes;

OneTheme.SetColor(ThemeColor.Vega);
// 只读枚举列表，可直接供 ComboBox 使用：OneTheme.ColorThemes。
```

## 切换与刷新

### 保存与启动恢复

`SelectThemeColor` 是 `StyledProperty<ThemeColor>`，默认 TwoWay；保留 `SelectedColor` 作为兼容属性，两个属性始终同步。同一个控件选择一个属性绑定，避免绑定两个互相冲突的配置源。首次附加时，显式绑定的 Nova（枚举值 0）也会应用；未绑定时跟随当前应用配色。卸载再附加不会用旧值覆盖应用的新设置。

```xml
<!-- 视图需声明业务 VM 的 x:DataType；Palette 的类型为 ThemeColor。 -->
<one:ThemeColorPicker SelectThemeColor="{Binding Palette}" />
```

控件只提供配置值与绑定，不自动读写磁盘。应用可保存 `Palette.ToString()`，下次在 App 初始化完成、创建主窗口之前恢复：

```csharp
// savedName 来自应用自己的配置存储；在 UI 线程、注册 OneTheme 后调用。
var palette = Enum.TryParse<ThemeColor>(savedName, out var value) && Enum.IsDefined(value)
    ? value : ThemeColor.Nova;
OneTheme.SetColor(palette);
// 同时用 palette 初始化 VM.Palette；后续属性变化由应用异步保存。
```

Light / Dark 是独立配置，应单独保存与恢复 `Application.Current.RequestedThemeVariant`。不要在每次选择回调中同步执行磁盘 I/O。

### 刷新边界

纯色画刷保留同一资源键的实例，颜色改变时重绘已有字形，避免触发无关的文字重新排版。PreviewHost 的代码示例仅在编辑器资源值改变时通知其内部调色订阅；One.* 背景、明暗模式和自定义语法资源覆盖仍正常刷新。甘特图的语义别名和工作流的画布、便签、选中阴影及缩略图跟随资源，重绘不重建节点、不修改业务文档或选择。

NavMenu 复用滚动时的图标、文字与提示容器，缩进不变时不重复写入行资源。首次加载、首次创建配色缓存与实际文本重排仍会有成本；不要将批处理通知理解为所有设备都能达到固定毫秒数。

明暗颜色快照和不可变渐变按需创建并缓存；已发布的纯色画刷按资源键保留实例，在批量通知前更新颜色。One.*Color 值资源和 Fluent 强调色仍接收正常资源通知。不要修改主题拥有的画刷；自定义配色请覆盖资源键。切换不移除或重建资源树。

ColorTheme 和资源查询立即返回新配色，界面资源通知合并到下一次 Render。同一轮连续多次调用只刷新最后一项；重复设置当前项、或在刷新前切回原配色，不发出多余通知。业务若在同一 UI 线程同步读取控件画刷，应在下一轮渲染后检查最终视觉状态。此行为避免每个中间选择都同步遍历整个可见界面。

## 图标式 ThemeButton

```xml
<one:ThemeButton Scope="Application" />
```

ThemeButton 现在继承 ToggleButton，不显示文本，只显示一个内置月亮／太阳图标；保留提示文本和辅助功能名称。`IsChecked=true` 表示当前有效模式为 Dark，false 表示 Light，外部模式变化会同步勾选状态。点击或 Space 切换模式，不改变 ColorTheme；禁用状态保留原生行为。

`Scope="Window"` 为默认值，更新最近窗口的 RequestedThemeVariant；`Application` 更新应用设置，具有显式局部主题的窗口仍遵循自己的设置。OneWindow 标题栏内的按钮通过 `ThemeButtonScope` 配置范围。窗口和 owner 关系见[窗口文档](../../controls/zh-cn/windows.md)。

从 Button 改为 ToggleButton 是基类变化；针对 ThemeButton 的样式应按 ToggleButton 状态编写。默认外观由独立的 ThemeButton 控件主题提供，内置图标不依赖 Content。

## PreviewHost 与验证

PreviewHost 启动默认打开独立一级菜单 Home；顶部 ThemeColorPicker 选择配色，标题栏 ThemeButton 仅切换明暗。Home 代码片段使用 NuGet 包 `TheOne.Avalonia.CodeEditor` 的只读高亮、换行和复制功能。每页底部可展开源码区，切换对应 AXAML、C# 及相关 VM 文件；源码嵌入应用，无需本机仓库路径。源码面板按需加载并使用有界视口。

```sh
dotnet build design/PreviewHost/PreviewHost.csproj --artifacts-path .preview/theme-code/artifacts
# 使用以上产物中的 PreviewHost.dll 分别执行：
dotnet <PreviewHost.dll> --verify-theme-colors .preview/theme-code/palettes
dotnet <PreviewHost.dll> --verify-preview-code .preview/theme-code/code
dotnet <PreviewHost.dll> --verify-home .preview/theme-code/home
# 本次枚举 API、编译绑定及切换性能专项：
dotnet <PreviewHost.dll> --verify-theme-api
dotnet <PreviewHost.dll> --verify-theme-performance .preview/theme-api/optimized.json
```

专项范围为配色选择器、ThemeButton、代码面板及 Home 导航，检查原生键鼠、资源实时更新、明暗／尺寸／窄视口与实际绘制。不会触发其他控件的全量回归。

原生消费可在既有 NativeAotSmoke 项目设置 `ThemeOnlyPreview=true`、`PublishAot=true`、`PublishTrimmed=true`，发布后传入 `--require-aot`，只执行枚举选择器及编译绑定专项，不执行其他控件回归。

<!-- generated-contracts:start -->
## 专属属性、Classes 用法与业务 API

本节只列本项目的专属属性、公开 Classes 用法、实际业务动作及关联模型，不重复原生继承 API、构造器、属性 Get/Set 包装或公开标识符。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用实际标签样例展示 Classes，不展示模板选择器／Setter 明细；Description 中的属性与业务动作附短用法代码。ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。

### ThemeButton

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeButton.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Scope`](../../../TheOne.Avalonia/Controls/ThemeButton.cs#L16) | `TheOne.Avalonia.Controls.ThemeButtonScope` | 可写; OneWay | `0 / default` | 主题按钮切换的作用域；影响 Light/Dark，不切换 ThemeColor 颜色枚举。 可选值：`Window`, `Application` |

### ThemeColorPicker

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeColorPicker.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`SelectThemeColor`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L17) | `TheOne.Avalonia.Themes.ThemeColor` | 可写; TwoWay | `0 / default` | The selected palette, suitable for a persisted two-way enum binding. 校验：`value => Enum.IsDefined(value)` 可选值：`Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |
| [`SelectedColor`](../../../TheOne.Avalonia/Controls/ThemeColorPicker.cs#L25) | `TheOne.Avalonia.Themes.ThemeColor` | 可写; TwoWay | `0 / default` | SelectedThemeColor 的兼容别名；两者表示同一颜色枚举，优先绑定 SelectedThemeColor。 校验：`value => Enum.IsDefined(value)` 可选值：`Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |

### OneTheme

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Themes.OneTheme.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`ColorTheme`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L14) | `TheOne.Avalonia.Themes.ThemeColor` | 可写; OneWay | `0 / default` | Color palette; changing this does not change the requested Light / Dark variant. 校验：`value => Enum.IsDefined(value)` 可选值：`Nova`, `Atlas`, `AtlasDeep`, `Helios`, `HeliosDeep`, `NovaDeep`, `Terra`, `TerraDeep`, `Vega`, `VegaDeep` |
| [`ColorThemes`](../../../TheOne.Avalonia/Themes/OneTheme.axaml.cs#L15) | `System.Collections.Generic.IReadOnlyList<TheOne.Avalonia.Themes.ThemeColor>` | 只读; CLR / n/a | `Array.AsReadOnly(Enum.GetValues<ThemeColor>())` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |

### ThemeColor

[完整源码契约](../../api/zh-cn/TheOne.Avalonia.Themes.ThemeColor.md)

没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。

尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。

### Classes 用法

没有额外公开外观类，使用默认主题及专属属性。

以上是分别使用的公开类名；悬停、按下、禁用和选中等状态由控件维护，不手工添加模板类。实际外观见 PreviewHost 的标签样例。

### 业务 API

| 类型 | 动作签名 | 用途 |
| --- | --- | --- |
| [`OneTheme`](../../api/zh-cn/TheOne.Avalonia.Themes.OneTheme.md) | `static void OneTheme.SetColor(ThemeColor color)` | Switches the application's OneTheme palette on the UI thread, preserving Light / Dark and property bindings. |

### 关联模型

| 模型 | 属性／枚举值 |
| --- | ---: |
| [`TheOne.Avalonia.Controls.ThemeButtonScope`](../../api/zh-cn/TheOne.Avalonia.Controls.ThemeButtonScope.md) | 2 |
| [`TheOne.Avalonia.Themes.OneTheme`](../../api/zh-cn/TheOne.Avalonia.Themes.OneTheme.md) | 2 |
| [`TheOne.Avalonia.Themes.ThemeColor`](../../api/zh-cn/TheOne.Avalonia.Themes.ThemeColor.md) | 10 |

[完整源码契约（含构造、属性访问器和标识符，供精确查阅）](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
