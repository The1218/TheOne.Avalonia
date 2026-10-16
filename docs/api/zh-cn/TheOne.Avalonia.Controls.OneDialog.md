# OneDialog

[English](../en/TheOne.Avalonia.Controls.OneDialog.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.OneDialog.md) · [API index](README.md)

`TheOne.Avalonia.Controls.OneDialog` · Class · `object`

[Source / 声明](../../../TheOne.Avalonia/Controls/OneMessages.cs#L119)

以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。

| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |
| --- | --- | --- | --- | --- |
| [`Owner`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L123) | `Avalonia.Controls.Window?` | 可写; CLR / n/a | `accessor-defined / state` | `child.Show(parent)` / `child.ShowDialog<T>(parent)` 建立的原生窗口所属关系，影响平台窗口生命周期。 |

## 方法与构造

| 完整签名／值 | 说明 |
| --- | --- |
| [`static Task<OneDialogResult> OneDialog.ShowAsync<TView, TViewModel>(TViewModel viewModel, DialogOptions? options = null, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken)) where TView : UserControl, new() where TViewModel : class`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L125) | Creates a typed view lazily on the UI thread; compiled bindings and NativeAOT are supported. |
| [`static Task<OneDialogResult> OneDialog.ShowAsync(string title, string message, OneDialogButtons buttons = OneDialogButtons.Ok, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L136) | 内部调度至 UI 线程。 |
| [`static Task<OneDialogResult> OneDialog.ShowContentAsync(object? content, string? title = null, OneDialogButtons buttons = OneDialogButtons.OkCancel, IDataTemplate? contentTemplate = null, Window? owner = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/OneMessages.cs#L138) | 内部调度至 UI 线程。 |

## 相关 API 与内置模型

- [`TheOne.Avalonia.Controls.DialogOptions`](TheOne.Avalonia.Controls.DialogOptions.md)
- [`TheOne.Avalonia.Controls.OneDialogButtons`](TheOne.Avalonia.Controls.OneDialogButtons.md)
- [`TheOne.Avalonia.Controls.OneDialogResult`](TheOne.Avalonia.Controls.OneDialogResult.md)
