# Avatar 头像

**简体中文** · [English](../en/avatar.md)

`Avatar` 展示本地或网络图片，无图片时显示最多两个 Unicode 文本元素或默认人物图标。使用前加载 `OneTheme`。头像本身用于展示；需要个人资料等点击操作时，将其放入原生 `Button`。

```xml
<one:Avatar Source="avares://YourApp/Assets/avatar.png" Text="LM" AutomationProperties.Name="林默的头像" />
<one:Avatar Source="https://example.com/avatar.png" Text="林默" Shape="Rounded" />
<one:Avatar Source="{Binding AvatarPath}" Text="AK" Shape="Square" />
```

声明 `xmlns:one="using:TheOne.Avalonia.Controls"`，使用编译绑定的视图应声明具体的 `x:DataType`。示例的 YourApp 替换为实际资源程序集名，并把图片声明为 AvaloniaResource；本地文件可通过 AvatarPath 提供，不复制其他设备的绝对路径。

| 属性 | 契约 |
| --- | --- |
| `Source` | 本地绝对／相对路径、`file://` URI、HTTP(S) 地址、`avares://` URI，或调用方持有的 `IImage`；地址支持字符串和 `Uri` 对象 |
| `Text` | 占位文字；去除两端空白后最多显示两个字素，不拆分组合 emoji；空文字显示人物图标 |
| `Shape` | 默认 `Circle` 圆形，也支持 `Rounded` 圆角方形和 `Square` 方形 |
| `CornerRadius` | `Rounded` 的圆角，默认 8；圆形头像请保持 Width / Height 相等 |
| `Stretch` | 原生图片拉伸模式，默认 `UniformToFill`，保持比例并裁剪填充 |
| `CurrentImage`、`HasImage` | 只读的已解码图片与成功加载状态 |
| `IsLoading`、`ErrorMessage` | 只读加载状态与失败原因；清空来源会清除错误 |
| `Reload()` | 控件已挂载时重新加载当前来源 |

继承 Small / Normal / Large 密度，尺寸为 32 / 40 / 56 px，文字字号为 12 / 14 / 18 px。显式 Width / Height 和字号优先，占位内容会按可用空间缩放。明暗主题使用 `One.AccentSoft`、`One.Accent` 和 `One.Line`；可覆盖标准 Background、Foreground、BorderBrush、BorderThickness。

图片在挂载到可视树后加载；切换 Source 或移除控件会取消未完成任务，旧响应不会覆盖新头像。控件创建的位图在替换或移除时释放，外部传入的 `IImage` 仍由调用方持有。重新挂载会再次加载当前来源。本地文件和网络响应在后台解码；网络加载总超时 30 秒，响应最大 16 MiB。不提供持久图片缓存、登录认证界面、SVG 解码或动态图播放。相对文件路径基于进程工作目录，HTTPS 使用平台正常证书校验。

加载中及 HTTP、文件、解码失败时均保留占位。伪类为 `:loading`、`:error`、`:has-image`、`:has-text`。默认模板保留 `PART_Surface`、`PART_Fallback`、`PART_Icon`、`PART_Text`、`PART_Image`。用 `AutomationProperties.Name` 标注人物身份；未指定时，无障碍名称使用 Text，最后回退为“Avatar”。

预览入口为“扩展控件 / Avatar”。运行 `dotnet run --project design/PreviewHost -- --verify-avatar .preview/avatar`，检查本地／HTTP 加载、取消、外部图片所有权、密度、Unicode 占位与实际裁剪像素，并导出明暗主题及窄视口截图。原生消费程序支持 `--require-aot --verify-avatar .preview/avatar-native`。网络专项使用可重复的本机 HTTP 服务，不依赖第三方图片网站。

<!-- generated-contracts:start -->
## 额外属性、样式与完整 API

本节按源码声明列出本库增加的成员；原生控件继承属性仍使用 Avalonia API。注册默认值与主题生效值不同，显式局部值优先。模型、方法、事件、枚举的完整签名可从下表的类型链接查看。PreviewHost 在各控件自己的页面完整展示原始示例，并补充样式、额外属性、API／事件与内置模型；仅 Button、Input、Select 合并比较。

### Avatar

[API / model reference](../../api/zh-cn/TheOne.Avalonia.Controls.Avatar.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`CurrentImage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L34) | `Avalonia.Media.IImage?` | 只读; OneWay | `computed / state` | 只读的已解码图片与成功加载状态 |
| [`DisplayText`](../../../TheOne.Avalonia/Controls/Avatar.cs#L38) | `string` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`EffectiveCornerRadius`](../../../TheOne.Avalonia/Controls/Avatar.cs#L39) | `Avalonia.CornerRadius` | 只读; OneWay | `computed / state` | 只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。 |
| [`ErrorMessage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L37) | `string?` | 只读; OneWay | `computed / state` | 只读加载状态与失败原因；清空来源会清除错误 |
| [`HasImage`](../../../TheOne.Avalonia/Controls/Avatar.cs#L36) | `bool` | 只读; OneWay | `computed / state` | 只读的已解码图片与成功加载状态 |
| [`IsLoading`](../../../TheOne.Avalonia/Controls/Avatar.cs#L35) | `bool` | 只读; OneWay | `computed / state` | 只读加载状态与失败原因；清空来源会清除错误 |
| [`Shape`](../../../TheOne.Avalonia/Controls/Avatar.cs#L32) | `TheOne.Avalonia.Controls.AvatarShape` | 可写; OneWay | `0 / default` | 默认 `Circle` 圆形，也支持 `Rounded` 圆角方形和 `Square` 方形 校验：`Enum.IsDefined` 可选值：`Circle`, `Rounded`, `Square` |
| [`Source`](../../../TheOne.Avalonia/Controls/Avatar.cs#L30) | `object?` | 可写; OneWay | `null` | 本地绝对／相对路径、`file://` URI、HTTP(S) 地址、`avares://` URI，或调用方持有的 `IImage`；地址支持字符串和 `Uri` 对象 |
| [`Stretch`](../../../TheOne.Avalonia/Controls/Avatar.cs#L33) | `Avalonia.Media.Stretch` | 可写; OneWay | `Stretch.UniformToFill` | 原生图片拉伸模式，默认 `UniformToFill`，保持比例并裁剪填充 校验：`Enum.IsDefined` 可选值：`None`, `Fill`, `Uniform`, `UniformToFill` |
| [`Text`](../../../TheOne.Avalonia/Controls/Avatar.cs#L31) | `string?` | 可写; OneWay | `null` | 占位文字；去除两端空白后最多显示两个字素，不拆分组合 emoji；空文字显示人物图标 |

### 公共附加属性

[ControlSizing](../../api/zh-cn/TheOne.Avalonia.Styling.ControlSizing.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Size`](../../../TheOne.Avalonia/Styling/ControlSizing.cs#L11) | `TheOne.Avalonia.Styling.ControlSize` | 可写; OneWay; 附加属性; 向后代继承 | `ControlSize.Normal` | Small / Normal / Large 共享密度，向后代继承；显式单控件值或尺寸类可覆盖。 可选值：`Small`, `Normal`, `Large` |

[Motion](../../api/zh-cn/TheOne.Avalonia.Styling.Motion.md)

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Enabled`](../../../TheOne.Avalonia/Styling/Motion.cs#L9) | `bool` | 可写; OneWay; 附加属性; 向后代继承 | `true` | 向后代继承的动效开关；关闭时转入最终布局和状态，保留原生输入。 |

### Style 类与自动状态

| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |
| --- | --- | --- | --- |
| Avatar | `one-large` | 模板自动类；不手动绑定 | one&#124;Avatar.one-large => Width=56; Height=56; FontSize=18 |
| Avatar | `one-normal` | 模板自动类；不手动绑定 | one&#124;Avatar.one-normal => Width=40; Height=40; FontSize=14 |
| Avatar | `one-small` | 模板自动类；不手动绑定 | one&#124;Avatar.one-small => Width=32; Height=32; FontSize=12 |

`small` / `normal` / `large` 是公开密度入口；`one-small` / `one-normal` / `one-large` 为自动生成的内部类。`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出额外外观类的控件使用默认主题及其属性。

### API、事件与内置模型

| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |
| --- | --- | ---: | ---: | ---: |
| [`TheOne.Avalonia.Controls.Avatar`](../../api/zh-cn/TheOne.Avalonia.Controls.Avatar.md) | Class | 10 | 2 | 0 |
| [`TheOne.Avalonia.Controls.AvatarShape`](../../api/zh-cn/TheOne.Avalonia.Controls.AvatarShape.md) | Enum | 3 | 1 | 0 |

[Complete public API index](../../api/zh-cn/README.md)
<!-- generated-contracts:end -->
