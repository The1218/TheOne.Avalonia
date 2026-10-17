using System.Windows.Input;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class MessageBoxDemoViewModel : ReactiveObject
{
    public MessageBoxDemoViewModel()
    {
        var canConfirm = this.WhenAnyValue(model => model.Title, model => model.Message, model => model.ConfirmText,
            model => model.CancelText,
            (title, message, confirm, cancel) => !string.IsNullOrWhiteSpace(title) &&
                                                 !string.IsNullOrWhiteSpace(message)
                                                 && !string.IsNullOrWhiteSpace(confirm) &&
                                                 !string.IsNullOrWhiteSpace(cancel));
        ConfirmCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var confirmed = await OneMessageBox.ConfirmAsync(Title, Message,
                new MessageBoxOptions
                    { ConfirmText = ConfirmText, CancelText = CancelText, IsDangerous = IsDangerous });
            if (confirmed) ConfirmedCount++;
            Status = confirmed ? "已确认 · 返回 true" : "已取消 · 返回 false";
        }, canConfirm);
        DeleteCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var confirmed = await OneMessageBox.ConfirmAsync("删除这个文件？", "文件删除后将无法恢复，请确认是否继续。",
                new MessageBoxOptions { ConfirmText = "确认删除", CancelText = "保留文件", IsDangerous = true });
            Status = confirmed ? "已确认删除 · 返回 true" : "已保留文件 · 返回 false";
        });
    }

    public string Title
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "确认提交更改？";

    public string Message
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "提交后，最新配置将应用到当前工作区。你可以在提交前继续检查内容。";

    public string ConfirmText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "确认提交";

    public string CancelText
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "稍后再说";

    public bool IsDangerous
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string Status
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = "等待操作 · 仅明确确认返回 true";

    public int ConfirmedCount
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public ICommand ConfirmCommand { get; }
    public ICommand DeleteCommand { get; }
}