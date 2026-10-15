# MessageBox 确认框

`OneMessageBox` 只处理确认 / 取消，复用 `OneWindow`、`LoginWindow` 已有的窗口内对话框层，不创建额外原生窗口。表单和自定义内容继续使用 `OneDialog`。

```csharp
// 在 ViewModel 的异步 ICommand 中调用。
var confirmed = await OneMessageBox.ConfirmAsync(
    "删除这个文件？",
    "文件及其预览图将永久删除，删除后无法恢复。",
    new MessageBoxOptions
    {
        ConfirmText = "确认删除",
        CancelText = "保留文件",
        IsDangerous = true
    });
if (confirmed)
    await fileService.DeleteAsync(fileId);
```

只有执行确认命令返回 `true`；取消、Escape、关闭所属窗口均返回 `false`。初始焦点位于取消按钮，危险操作也保持这一默认行为。Tab 切换焦点，Enter 执行当前按钮。点击遮罩保持确认框打开。关闭后由现有对话框宿主恢复原先焦点。

`MessageBoxOptions` 提供 `ConfirmText`（默认“确认”）、`CancelText`（默认“取消”）、`IsDangerous`（默认 `false`）及可选的 `ControlSize? Size`。不设置 `Size` 时继承窗口的 Small / Normal / Large 尺寸。标题、正文及按钮使用 `One.*` 主题资源，支持 Light / Dark。标题、消息和按钮文案不能为空白。

可选 `owner` 参数指定已打开的 `OneWindow` 或 `LoginWindow`；省略时与 `OneDialog` 一样使用注册的默认窗口。确认框与其他对话框共用每个窗口的顺序队列。取消 `cancellationToken` 会取消任务并释放对话框，不把任务取消视为用户选择。

PreviewHost 的 MessageBox 页面通过 ViewModel 命令打开确认框，展示可配置确认和危险操作确认；等待布尔结果后才执行业务操作，全部使用编译绑定。
