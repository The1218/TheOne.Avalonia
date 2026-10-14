using System.Windows.Input;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Threading;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class ContentTransitionDemoViewModel : ReactiveObject
{
    public IReadOnlyList<TransitionPage> Pages { get; } = [new TransitionOverview(), new TransitionForm(), new TransitionRecords()];

    private DispatcherTimer? _rapid;
    public int CompletedCount { get; private set => this.RaiseAndSetIfChanged(ref field, value); }
    public int AnimatedCount { get; private set => this.RaiseAndSetIfChanged(ref field, value); }
    public int ActionCount { get; private set => this.RaiseAndSetIfChanged(ref field, value); }
    public bool IsRapidSwitching => _rapid is not null;
    public int PageIndex { get; private set => this.RaiseAndSetIfChanged(ref field, value); }

    public object? Content
    {
        get;
        set
        {
            if (EqualityComparer<object?>.Default.Equals(field, value))
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(Position));
        }
    }

    public bool Enabled { get; set => this.RaiseAndSetIfChanged(ref field, value); } = true;
    public bool Reverse { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public bool MotionEnabled { get; set => this.RaiseAndSetIfChanged(ref field, value); } = true;
    public string Status { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "尚未执行页面操作";
    public string Metrics { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";
    public string Position => $"{PageIndex + 1} / {Pages.Count} · {(Content as TransitionPage)?.Title ?? "空内容"}";

    public int TransitionIndex
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            Transition = CreateTransition(value);
        }
    }

    public IPageTransition? Transition { get; private set => this.RaiseAndSetIfChanged(ref field, value); } = CreateTransition(0);

    private static IPageTransition? CreateTransition(int index) => index switch
    {
        1 => new PageSlide(TimeSpan.FromMilliseconds(200), PageSlide.SlideAxis.Horizontal)
        {
            SlideInEasing = new CubicEaseOut(),
            SlideOutEasing = new CubicEaseOut()
        },
        2 => new PageSlide(TimeSpan.FromMilliseconds(200), PageSlide.SlideAxis.Vertical)
        {
            SlideInEasing = new CubicEaseOut(),
            SlideOutEasing = new CubicEaseOut()
        },
        3 => null,
        _ => new CrossFade(TimeSpan.FromMilliseconds(200))
        {
            FadeInEasing = new CubicEaseOut(),
            FadeOutEasing = new CubicEaseOut()
        }
    };
    public ICommand PreviousCommand { get; }
    public ICommand NextCommand { get; }
    public ICommand RapidCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand RestoreCommand { get; }
    public ICommand LastRecordCommand { get; }
    public ICommand PageActionCommand { get; }
    public ICommand ResetCommand { get; }

    public event EventHandler? ScrollRequested, ResetMotionRequested;
    public ContentTransitionDemoViewModel()
    {
        Content = Pages[0];
        PreviousCommand = ReactiveCommand.Create<object?>(_ =>
        {
            StopRapid();
            SelectPage(PageIndex - 1);
        });
        NextCommand = ReactiveCommand.Create<object?>(_ =>
        {
            StopRapid();
            SelectPage(PageIndex + 1);
        });
        RapidCommand = ReactiveCommand.Create<object?>(_ =>
        {
            StopRapid();
            var step = 0;
            _rapid = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(45)
            };
            _rapid.Tick += (_, _) =>
            {
                SelectPage(++step % 2 == 1 ? 1 : 0);
                if (step == 5)
                    StopRapid();
            };
            _rapid.Start();
        });
        ClearCommand = ReactiveCommand.Create<object?>(_ =>
        {
            StopRapid();
            Content = null;
        });
        RestoreCommand = ReactiveCommand.Create<object?>(_ =>
        {
            StopRapid();
            SelectPage(PageIndex);
        });
        LastRecordCommand = ReactiveCommand.Create<object?>(_ =>
        {
            StopRapid();
            var records = (TransitionRecords)Pages[2];
            records.SelectedRow = records.Rows[^1];
            SelectPage(2);
            ScrollRequested?.Invoke(this, EventArgs.Empty);
        });
        PageActionCommand = ReactiveCommand.Create<object?>(parameter =>
        {
            if (parameter is TransitionPage page)
                Status = $"已操作：{page.Title} · {++ActionCount} 次";
        });
        ResetCommand = ReactiveCommand.Create<object?>(_ =>
        {
            StopRapid();
            TransitionIndex = 0;
            Reverse = false;
            Enabled = true;
            ((TransitionForm)Pages[1]).Draft = "完善下一组基础控件";
            ((TransitionRecords)Pages[2]).SelectedRow = null;
            ActionCount = CompletedCount = AnimatedCount = 0;
            Status = "尚未执行页面操作";
            SelectPage(0);
            ResetMotionRequested?.Invoke(this, EventArgs.Empty);
        });
    }

    public void SelectPage(int index)
    {
        PageIndex = (index % Pages.Count + Pages.Count) % Pages.Count;
        Content = Pages[PageIndex];
        this.RaisePropertyChanged(nameof(Position));
    }

    public void CompleteTransition(bool animated)
    {
        CompletedCount++;
        if (animated)
            AnimatedCount++;
    }

    public void StopRapid()
    {
        _rapid?.Stop();
        _rapid = null;
    }
}
