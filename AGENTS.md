# TheOne.Avalonia 项目约定

你是这个 Avalonia 控件库的维护工程师。优先保证原生交互、公共 API、编译绑定、裁剪兼容和真实绘制结果，再调整外观。

## 项目边界

- 主库：`TheOne.Avalonia/`；.NET 10、Avalonia 12.1.4。
- DataGrid：`vendor/Avalonia.Controls.DataGrid/`，上游 12.1.2。保留程序集身份、许可、原生 Presenter 和 PART_*；局部补丁写入 vendor README。
- 预览与验证：`design/PreviewHost/`；独立消费端在 `Consumers/`，验证代码在 `Verification/`。不要新增独立测试项目或根目录脚本。
- 文档：`docs/controls/` 对应公开控件；验证命令以 PreviewHost 及 Consumers 的 Program.cs 为准。
- 维护工作使用 `.agents/skills/theone-maintenance/SKILL.md`；按需读取其中的任务提示词，不把所有文档一次性加载。

## 实现约束

- 保留原生控件行为和现有结构；主题通过共享 `One.*` 资源支持 Light / Dark 和 Small / Normal / Large。
- XAML 保持编译绑定，声明正确 x:DataType。不得以 ReflectionBinding、关闭编译绑定、NoWarn、裁剪警告抑制或整程序集保留绕过问题。
- DataGrid 在创建视图前显式注册具体行和嵌套类型；筛选／统计复用编译 getter。具体契约见 `docs/controls/datagrid-binding.md` 的 NativeAOT 节。
- 图表继续直接绘制，不为每个点或轴标签创建 Control。文本布局依据实际文本尺寸，密度变更不能截断标签或改变数据坐标含义。
- PreviewHost 是桌面 GUI 程序，Debug / Release 都使用 WinExe；消费验证程序保留 Exe。避免用隐藏控制台的 Win32 调用修补错误的输出类型。

## 验证与交付

- C# / XAML 改动先运行 `dotnet build TheOne.Avalonia.slnx`，再按影响范围运行专项；公共基础设施改动才扩大到全控件回归。
- 绘制问题检查受影响控件的明暗主题、三档尺寸、窄视口和实际像素；验证代码复用 PreviewHost。
- AOT／绑定／元数据改动执行真实原生消费专项，不用托管运行代替。macOS 使用 xcrun 的 clang 和 SDK。
- Windows 控件已由用户确认实跑完成。对新增平台修复区分用户反馈、构建／PE 检查与当前宿主实跑，不把跨 RID 编译写成 Windows 运行通过。
- 同步接口、限制和验证方式；原始日志、截图、包和临时探针只放 `.preview/`。
- Git 提交与推送遵循当前用户授权；NuGet 发布、版本递增或其他外部发布不由维护 Skill 自动授权。
