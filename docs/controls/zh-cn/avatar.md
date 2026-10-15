# Avatar 头像

**简体中文** · [English](../en/avatar.md)

`Avatar` 展示本地或网络图片，无图片时显示最多两个 Unicode 文本元素或默认人物图标。使用前加载 `OneTheme`。头像本身用于展示；需要个人资料等点击操作时，将其放入原生 `Button`。

```xml
<one:Avatar Source="/Users/me/Pictures/avatar.jpg" Text="LM" AutomationProperties.Name="林默的头像" />
<one:Avatar Source="https://example.com/avatar.png" Text="林默" Shape="Rounded" />
<one:Avatar Source="{Binding AvatarPath}" Text="AK" Shape="Square" />
```

声明 `xmlns:one="using:TheOne.Avalonia.Controls"`，使用编译绑定的视图应声明具体的 `x:DataType`。

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
