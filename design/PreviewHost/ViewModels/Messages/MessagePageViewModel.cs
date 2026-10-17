using System.Windows.Input;
using Avalonia.Controls.Notifications;
using ReactiveUI;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

public sealed class MessagePageViewModel : ReactiveObject
{
    public MessagePageViewModel()
    {
        ToastCommand = ReactiveCommand.Create<object?>(_ =>
        {
            ConfigurationRequested?.Invoke(this, EventArgs.Empty);
            OneToast.Show(CreateNotification(false));
            Status = $"已显示 {Type} Toast";
        });
        NotificationCommand = ReactiveCommand.Create<object?>(_ =>
        {
            ConfigurationRequested?.Invoke(this, EventArgs.Empty);
            OneNotification.Show(CreateNotification());
            Status = $"已发送 {Type} 通知";
        });
        CloseToastCommand = ReactiveCommand.Create<object?>(_ => OneToast.CloseAll());
        CloseNotificationCommand = ReactiveCommand.Create<object?>(_ => OneNotification.CloseAll());
        DialogCommand = ReactiveCommand.CreateFromTask<object?>(async _ =>
        {
            ConfigurationRequested?.Invoke(this, EventArgs.Empty);
            Status = $"结果：{await OneDialog.ShowAsync<MessageDialogView, MessagePageViewModel>(this, Options(false))}";
        });
        FormDialogCommand = ReactiveCommand.CreateFromTask<object?>(async _ =>
        {
            ConfigurationRequested?.Invoke(this, EventArgs.Empty);
            var model = new DialogEditorViewModel();
            var result = await OneDialog.ShowAsync<DialogEditorView, DialogEditorViewModel>(model, Options(true));
            Status = $"VM 返回：{result} · {model.Name}";
        });
    }

    public string Title
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "工作区已更新";

    public string Message
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "保存完成，所有改动已同步。";

    public string Status
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "准备就绪";

    public NotificationType Type
    {
        get;
        set
        {
            if (field == value) return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(MessageStatus));
        }
    } = NotificationType.Success;

    public TagStatus MessageStatus => Type switch
    {
        NotificationType.Success => TagStatus.Success, NotificationType.Warning => TagStatus.Warning,
        NotificationType.Error => TagStatus.Error, _ => TagStatus.Information
    };

    public NotificationPosition Position
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = NotificationPosition.TopCenter;

    public ControlSize Size
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = ControlSize.Normal;

    public decimal? Seconds
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 4;

    public decimal? Maximum
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 4;

    public bool AutoClose
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool MotionEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool ShowTitle
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool ShowClose
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool ShowButtons
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool OverlayClose
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool EscapeClose
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public OneDialogButtons Buttons
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = OneDialogButtons.OkCancel;

    public IReadOnlyList<NotificationType> Types { get; } = Enum.GetValues<NotificationType>();
    public IReadOnlyList<NotificationPosition> Positions { get; } = Enum.GetValues<NotificationPosition>();
    public IReadOnlyList<ControlSize> Sizes { get; } = Enum.GetValues<ControlSize>();
    public IReadOnlyList<OneDialogButtons> ButtonSets { get; } = Enum.GetValues<OneDialogButtons>();

    public int ClickCount
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int CloseCount
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public ICommand ToastCommand { get; }
    public ICommand NotificationCommand { get; }
    public ICommand CloseToastCommand { get; }
    public ICommand CloseNotificationCommand { get; }
    public ICommand DialogCommand { get; }
    public ICommand FormDialogCommand { get; }
    public event EventHandler? ConfigurationRequested;

    private Notification CreateNotification(bool includeTitle = true)
    {
        return new Notification(includeTitle && ShowTitle ? Title : "", Message, Type,
            AutoClose ? TimeSpan.FromSeconds((double)Math.Clamp(Seconds ?? 4, .1m, 120m)) : TimeSpan.Zero,
            () => Status = $"消息点击 {++ClickCount} 次", () => Status = $"消息关闭 {++CloseCount} 次");
    }

    private DialogOptions Options(bool form)
    {
        return new DialogOptions
        {
            Size = Size, MotionEnabled = MotionEnabled, Title = ShowTitle ? Title : null, ShowTitle = ShowTitle,
            ShowCloseButton = ShowClose, CloseOnOverlayClick = OverlayClose, CloseOnEscape = EscapeClose,
            ShowButtons = form ? false : ShowButtons, Buttons = Buttons
        };
    }
}