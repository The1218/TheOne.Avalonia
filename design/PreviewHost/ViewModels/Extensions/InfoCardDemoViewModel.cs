using System.Windows.Input;
using Avalonia.Media;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class InfoCardDemoViewModel : ReactiveObject
{
    public bool HoverEnabled { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public bool ShadowEnabled { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    private static readonly Geometry InfoGeometry =
        Geometry.Parse(
            "F0 M 12,2 A 10,10 0 1 1 12,22 A 10,10 0 1 1 12,2 Z M 12,4 A 8,8 0 1 1 12,20 A 8,8 0 1 1 12,4 Z M 11,10 L 13,10 L 13,17 L 11,17 Z M 11,6 L 13,6 L 13,8 L 11,8 Z");

    private static readonly Geometry UpdatedGeometry = Geometry.Parse("M 3,12 L 5,10 L 10,15 L 19,6 L 21,8 L 10,19 Z");

    public InfoCardDemoViewModel()
    {
        UpdateCommand = ReactiveCommand.Create<object?>(_ =>
        {
            Revision++;
            this.RaisePropertyChanged(nameof(Description));
            this.RaisePropertyChanged(nameof(IconData));
        });
        SaveCommand = ReactiveCommand.Create<object?>(_ =>
        {
            if (string.IsNullOrWhiteSpace(WorkspaceName))
            {
                SaveStatus = "请输入工作区名称。";
                return;
            }

            SaveCount++;
            SaveStatus = $"已保存 {SaveCount} 次 · {WorkspaceName.Trim()}";
        });
        DetailsCommand = ReactiveCommand.Create<object?>(_ =>
        {
            DetailsCount++;
            this.RaisePropertyChanged(nameof(DetailsLabel));
            FooterStatus = $"详情查看 {DetailsCount} 次 · 当前版本 {Revision}";
        });
        SyncCommand = ReactiveCommand.Create<object?>(_ =>
        {
            SyncCount++;
            FooterStatus = $"已同步 {SyncCount} 次 · {WorkspaceName.Trim()}";
        });
        ResetCommand = ReactiveCommand.Create<object?>(_ =>
        {
            ShowHeader = ShowIcon = ShowHeaderActions = ShowFooter = Enabled = true;
            ShowHeaderSeparator = ShowFooterSeparator = true;
            HoverEnabled = ShadowEnabled = false;
            WorkspaceName = "TheOne workspace";
            SaveCount = 0;
            DetailsCount = SyncCount = 0;
            SaveStatus = "修改工作区名称，然后点击标题中的保存按钮。";
            FooterStatus = "尚未同步 · 使用标题操作或页脚按钮更新状态。";
            Revision = 1;
            this.RaisePropertyChanged(nameof(Description));
            this.RaisePropertyChanged(nameof(IconData));
            this.RaisePropertyChanged(nameof(DetailsLabel));
        });
    }

    public string Title => "工作区概览";
    public string? VisibleTitle => ShowHeader ? Title : null;
    public InfoCardDemoViewModel? VisibleHeaderIcon => ShowIcon ? this : null;
    public InfoCardDemoViewModel? VisibleHeaderActions => ShowHeaderActions ? this : null;
    public InfoCardDemoViewModel? VisibleFooter => ShowFooter ? this : null;
    public Geometry IconData => Revision % 2 == 0 ? UpdatedGeometry : InfoGeometry;
    public string DetailsLabel => DetailsCount == 0 ? "详情" : $"详情 {DetailsCount}";
    public string Description => $"TheOne 主题 · 当前版本 {Revision}。信息卡片使用统一的表面、细边框与圆角；正文自动换行，标题和正文可替换为自定义控件或数据模板。";

    public int Revision
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = 1;

    public int SaveCount
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int DetailsCount
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int SyncCount
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool ShowHeader
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(VisibleTitle));
        }
    } = true;

    public bool ShowHeaderSeparator
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool ShowFooterSeparator
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool Enabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public int SelectedIndex
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = -1;

    public bool ShowIcon
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(VisibleHeaderIcon));
        }
    } = true;

    public bool ShowHeaderActions
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(VisibleHeaderActions));
        }
    } = true;

    public bool ShowFooter
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(VisibleFooter));
        }
    } = true;

    public string WorkspaceName
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "TheOne workspace";

    public string SaveStatus
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = "修改工作区名称，然后点击标题中的保存按钮。";

    public string FooterStatus
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = "尚未同步 · 使用标题操作或页脚按钮更新状态。";

    public IReadOnlyList<string> Members { get; } =
        Enumerable.Range(1, 1_000).Select(index => $"成员 {index:D4} · TheOne workspace").ToArray();

    public ICommand UpdateCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand DetailsCommand { get; }
    public ICommand SyncCommand { get; }
}
