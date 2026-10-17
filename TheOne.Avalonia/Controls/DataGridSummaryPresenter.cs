using System.Globalization;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

/// <summary>One drawn footer, aligned and clipped with native DataGrid column geometry.</summary>
public sealed class DataGridSummaryPresenter : Control
{
    public static readonly StyledProperty<IBrush?> LineBrushProperty =
        AvaloniaProperty.Register<DataGridSummaryPresenter, IBrush?>("LineBrush");

    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        AvaloniaProperty.Register<DataGridSummaryPresenter, IBrush?>("Background");

    public static readonly StyledProperty<IBrush?> ForegroundProperty =
        AvaloniaProperty.Register<DataGridSummaryPresenter, IBrush?>("Foreground");

    private DataGrid? _grid;
    private IDisposable? _line, _background, _foreground;
    private DataGridSummaryState? _state;

    public DataGridSummaryPresenter()
    {
        IsHitTestVisible = false;
        Focusable = false;
        IsVisible = false;
        ClipToBounds = true;
        AutomationProperties.SetName(this, "数值统计行");
    }

    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public IBrush? Foreground
    {
        get => GetValue(ForegroundProperty);
        set => SetValue(ForegroundProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs args)
    {
        base.OnAttachedToVisualTree(args);
        _grid = this.GetVisualAncestors().OfType<DataGrid>().FirstOrDefault();
        if (_grid is null) return;
        _grid.PropertyChanged += GridChanged;
        _grid.ColumnLayoutChanged += LayoutChanged;
        _state = DataGridFeatures.SummaryState(_grid);
        _state.Updated += LayoutChanged;
        _line = Bind(LineBrushProperty, new DynamicResourceExtension("One.Line"));
        _background = Bind(BackgroundProperty, new DynamicResourceExtension("One.Subtle"));
        _foreground = Bind(ForegroundProperty, new DynamicResourceExtension("One.Text"));
        Synchronize();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs args)
    {
        if (_grid is not null)
        {
            _grid.PropertyChanged -= GridChanged;
            _grid.ColumnLayoutChanged -= LayoutChanged;
        }

        if (_state is not null) _state.Updated -= LayoutChanged;
        _line?.Dispose();
        _background?.Dispose();
        _foreground?.Dispose();
        _grid = null;
        _state = null;
        base.OnDetachedFromVisualTree(args);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ControlSizing.SizeProperty) InvalidateMeasure();
        if (change.Property == BackgroundProperty || change.Property == ForegroundProperty ||
            change.Property == LineBrushProperty) InvalidateVisual();
    }

    private void GridChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == DataGridFeatures.ShowSummaryRowProperty) Synchronize();
        else InvalidateVisual();
    }

    private void Synchronize()
    {
        IsVisible = _grid is not null && DataGridFeatures.GetShowSummaryRow(_grid);
        _state?.Synchronize();
        InvalidateMeasure();
        InvalidateVisual();
    }

    private void LayoutChanged(object? sender, EventArgs args)
    {
        if (IsVisible) InvalidateVisual();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return new Size(0,
            ControlSizing.GetSize(this) switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 });
    }

    public override void Render(DrawingContext context)
    {
        if (_grid is null || _state is null) return;
        if (Background is { } background) context.FillRectangle(background, new Rect(Bounds.Size));
        var line = GetValue(LineBrushProperty);
        if (line is not null) context.DrawLine(new Pen(line), default, new Point(Bounds.Width, 0));
        foreach (var column in _grid.Columns.Where(column => column.IsVisible))
        {
            var layout = _grid.GetColumnLayout(column, Bounds.Height);
            if (layout.VisibleBounds.Width <= 0) continue;
            using (context.PushClip(layout.VisibleBounds))
            {
                Draw(_state.GetText(column), layout.Bounds);
                if (line is not null)
                    context.DrawLine(new Pen(line), new Point(layout.Bounds.Right, 0),
                        new Point(layout.Bounds.Right, Bounds.Height));
            }
        }

        if (_grid.LayoutRowHeaderWidth > 0)
            Draw(DataGridFeatures.GetSummaryLabel(_grid), new Rect(0, 0, _grid.LayoutRowHeaderWidth, Bounds.Height));

        void Draw(string value, Rect bounds)
        {
            var text = new FormattedText(value, _grid.CollectionView?.Culture ?? CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(TextElement.GetFontFamily(this), FontStyle.Normal, FontWeight.SemiBold),
                TextElement.GetFontSize(this), Foreground);
            text.MaxTextWidth = Math.Max(0.1, bounds.Width - 12);
            text.Trimming = TextTrimming.CharacterEllipsis;
            context.DrawText(text,
                new Point(bounds.X + (bounds.Width - text.Width) / 2, (Bounds.Height - text.Height) / 2));
        }
    }
}