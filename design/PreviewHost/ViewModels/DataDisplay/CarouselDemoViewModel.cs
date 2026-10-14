using System.Windows.Input;
using Avalonia.Threading;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed record CarouselSlide(int Number, string Title, string Description, string Category);

public sealed class CarouselDemoViewModel : ReactiveObject
{
    private DispatcherTimer? _reverse;

    public CarouselDemoViewModel()
    {
        PreviousCommand = ReactiveCommand.Create<object?>(_ =>
            Select(Wrap ? (SelectedIndex - 1 + Slides.Count) % Slides.Count : SelectedIndex - 1));
        NextCommand =
            ReactiveCommand.Create<object?>(_ => Select(Wrap ? (SelectedIndex + 1) % Slides.Count : SelectedIndex + 1));
        JumpCommand = ReactiveCommand.Create<object?>(_ => Select((int)(Target ?? 1) - 1));
        LastCommand = ReactiveCommand.Create<object?>(_ => Select(Slides.Count - 1));
        ResetCommand = ReactiveCommand.Create<object?>(_ =>
        {
            StopReverse();
            Enabled = true;
            Swipe = true;
            Wrap = false;
            ViewportIndex = 0;
            SelectedIndex = 0;
            Target = 901;
            ActionCount = 0;
            Status = "尚未查看详情";
        });
        SlideActionCommand = ReactiveCommand.Create<object?>(parameter =>
        {
            if (parameter is CarouselSlide slide)
                Status = $"已查看：{slide.Title} · 操作 {++ActionCount} 次";
        });
        ReverseCommand = ReactiveCommand.Create<object?>(_ =>
        {
            StopReverse();
            var origin = SelectedIndex;
            var target = (origin + 1) % Slides.Count;
            var step = 0;
            _reverse = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(35)
            };
            _reverse.Tick += (_, _) =>
            {
                SelectedIndex = ++step % 2 == 1 ? target : origin;
                if (step == 5)
                    StopReverse();
            };
            _reverse.Start();
        });
    }

    public IReadOnlyList<CarouselSlide> Slides { get; } = Enumerable.Range(1, 1000).Select(number =>
        new CarouselSlide(number, $"工作区 {number:0000}", "让当前任务保持清晰。收集想法、整理资料，与团队一起推进每一个细节。", (number % 3) switch
        {
            0 => "团队协作",
            1 => "项目概览",
            _ => "资料归档"
        })).ToArray();

    public int SelectedIndex
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(Position));
            this.RaisePropertyChanged(nameof(CanPrevious));
            this.RaisePropertyChanged(nameof(CanNext));
        }
    }

    public int ViewportIndex
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(ViewportFraction));
        }
    }

    public double ViewportFraction => ViewportIndex == 1 ? 0.72 : 1;

    public bool Enabled
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            if (!value)
                StopReverse();
            this.RaisePropertyChanged(nameof(CanPrevious));
            this.RaisePropertyChanged(nameof(CanNext));
        }
    } = true;

    public bool Swipe
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool Wrap
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(CanPrevious));
            this.RaisePropertyChanged(nameof(CanNext));
        }
    }

    public bool Animate
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public decimal? Target
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 901;

    public string Status
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "尚未查看详情";

    public string Metrics
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string Position => $"{SelectedIndex + 1:N0} / {Slides.Count:N0}";
    public bool CanPrevious => Enabled && (Wrap || SelectedIndex > 0);
    public bool CanNext => Enabled && (Wrap || SelectedIndex < Slides.Count - 1);
    public bool IsReversing => _reverse is not null;

    public int ActionCount
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public ICommand PreviousCommand { get; }
    public ICommand NextCommand { get; }
    public ICommand JumpCommand { get; }
    public ICommand LastCommand { get; }
    public ICommand ReverseCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand SlideActionCommand { get; }

    public event EventHandler? FocusRequested;

    private void Select(int index)
    {
        StopReverse();
        SelectedIndex = Math.Clamp(index, 0, Slides.Count - 1);
        FocusRequested?.Invoke(this, EventArgs.Empty);
    }

    public void StopReverse()
    {
        _reverse?.Stop();
        _reverse = null;
    }
}