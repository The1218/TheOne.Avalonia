using Avalonia.Controls;
using Avalonia.Threading;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

public partial class CarouselDemoView : UserControl
{
    public const int DataCount = 1000;
    private readonly HashSet<Control> _created = new(ReferenceEqualityComparer.Instance);
    private bool _syncMotion;

    public CarouselDemoView()
    {
        DataContext = Model;
        InitializeComponent();
        Model.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(CarouselDemoViewModel.Animate) && !_syncMotion)
                Motion.SetEnabled(CarouselSample, Model.Animate);
        };
        CarouselSample.PropertyChanged += (_, args) =>
        {
            if (args.Property == Motion.EnabledProperty)
            {
                _syncMotion = true;
                try
                {
                    Model.Animate = Motion.GetEnabled(CarouselSample);
                }
                finally
                {
                    _syncMotion = false;
                }
            }
        };
        CarouselSample.ContainerPrepared += (_, args) => _created.Add(args.Container);
        LayoutUpdated += (_, _) => UpdateMetrics();
        DetachedFromVisualTree += (_, _) => Model.StopReverse();
        Model.FocusRequested += (_, _) => Dispatcher.UIThread.Post(() =>
        {
            if (CarouselSample.IsEffectivelyVisible) CarouselSample.Focus();
            UpdateMetrics();
        }, DispatcherPriority.Loaded);
    }

    public CarouselDemoViewModel Model { get; } = new();
    public IReadOnlyList<CarouselSlide> Slides => Model.Slides;
    public int ActionCount => Model.ActionCount;
    public int CreatedContainerCount => _created.Count;
    public bool IsReversing => Model.IsReversing;

    public void UpdateMetrics()
    {
        Model.Metrics =
            $"当前容器 {CarouselSample.GetRealizedContainers().Count()} / {Slides.Count:N0} · 页面池 {CarouselSample.ItemsPanelRoot?.Children.Count ?? 0} · 累计创建 {CreatedContainerCount}";
    }
}