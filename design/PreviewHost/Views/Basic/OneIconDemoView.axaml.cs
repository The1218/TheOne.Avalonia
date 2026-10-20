using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.Automation;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public partial class OneIconDemoView : UserControl
{
    private readonly Grid _layout;
    private readonly StackPanel _inspector;
    private readonly StackPanel _gallery;
    private bool? _narrow;

    public OneIconDemoView()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
        _layout = this.FindControl<Grid>("GalleryLayout")!;
        _inspector = this.FindControl<StackPanel>("Inspector")!;
        _gallery = this.FindControl<StackPanel>("Gallery")!;
        _gallery.SizeChanged += (_, _) => Model.SetGalleryWidth(_gallery.Bounds.Width);
    }

    public OneIconDemoViewModel Model { get; } = new();

    private void IconClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: OneIconKind kind }) Model.SelectedIcon = OneIconCatalog.Get(kind);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (double.IsFinite(availableSize.Width))
        {
            var narrow = availableSize.Width < 820;
            if (_narrow != narrow)
            {
                _narrow = narrow;
                _layout.ColumnDefinitions[1].Width = narrow ? new GridLength(0) : new GridLength(280);
                Grid.SetColumn(_inspector, narrow ? 0 : 1);
                Grid.SetRow(_gallery, narrow ? 1 : 0);
                _gallery.Margin = narrow ? new Thickness(0, 16, 0, 0) : new Thickness(0, 0, 16, 0);
            }
            Model.SetGalleryWidth(availableSize.Width - (narrow ? 0 : 296));
        }
        return base.MeasureOverride(availableSize);
    }
}

/// <summary>A native Button with cached labels and the same OneIcon drawing path, used only by this gallery.</summary>
public sealed class OneIconGalleryTile : Button
{
    public static readonly StyledProperty<OneIconInfo?> InfoProperty =
        AvaloniaProperty.Register<OneIconGalleryTile, OneIconInfo?>(nameof(Info));
    private readonly OneIcon _icon = new() { IconSize = 24 };
    private TextLayout? _name, _label;
    private Typeface _typeface;
    private IBrush? _textBrush;
    private Color _textColor;
    private double _textWidth;
    private Pen? _borderPen;
    public OneIconGalleryTile()
    {
        _icon.Measure(new Size(24, 24));
        _icon.Arrange(new Rect(0, 0, 24, 24));
    }
    static OneIconGalleryTile() => AffectsRender<OneIconGalleryTile>(InfoProperty, ForegroundProperty,
        BackgroundProperty, BorderBrushProperty, BorderThicknessProperty, CornerRadiusProperty,
        FontFamilyProperty, FontStyleProperty, FontWeightProperty, FontStretchProperty);
    protected override Type StyleKeyOverride => typeof(Button);
    public OneIconInfo? Info { get => GetValue(InfoProperty); set => SetValue(InfoProperty, value); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == InfoProperty)
        {
            _name = _label = null;
            Tag = Info?.Kind;
            AutomationProperties.SetName(this, Info?.Name);
            ToolTip.SetTip(this, Info?.DisplayName);
        }
    }
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Info is not { } info || Bounds.Width <= 0 || Bounds.Height <= 0) return;
        var brush = Foreground;
        var thickness = BorderThickness.Left;
        if (_borderPen is null || !ReferenceEquals(_borderPen.Brush, BorderBrush) || _borderPen.Thickness != thickness)
            _borderPen = new Pen(BorderBrush, thickness);
        context.DrawRectangle(Background, _borderPen, new RoundedRect(new Rect(Bounds.Size).Deflate(thickness / 2), CornerRadius));
        var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        var color = (brush as ISolidColorBrush)?.Color ?? default;
        var width = Math.Max(1, Bounds.Width - 10);
        if (_name is null || !_typeface.Equals(typeface) || !ReferenceEquals(_textBrush, brush) ||
            _textColor != color || _textWidth != width || brush is not ISolidColorBrush)
        {
            _typeface = typeface; _textBrush = brush; _textColor = color; _textWidth = width;
            _name = new TextLayout(info.Name, typeface, 11, brush, textAlignment: TextAlignment.Center,
                textTrimming: TextTrimming.CharacterEllipsis, maxWidth: width);
            _label = new TextLayout(info.DisplayName, typeface, 11, brush, textAlignment: TextAlignment.Center,
                textTrimming: TextTrimming.CharacterEllipsis, maxWidth: width);
        }
        var top = (Bounds.Height - (24 + 10 + _name.Height + _label!.Height)) / 2;
        _icon.Kind = info.Kind;
        _icon.Foreground = brush;
        using (context.PushTransform(Matrix.CreateTranslation((Bounds.Width - 24) / 2, top))) _icon.Render(context);
        _name.Draw(context, new Point(5, top + 29));
        _label.Draw(context, new Point(5, top + 34 + _name.Height));
    }
}

/// <summary>Retains each row's visual tree when the native virtualizing panel clears its data.</summary>
public sealed class OneIconRowsControl : ItemsControl
{
    public IDataTemplate? TileTemplate { get; set; }
    protected override Type StyleKeyOverride => typeof(ItemsControl);
    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) =>
        new RowContainer(TileTemplate);
    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        ((RowContainer)container).SetRow(item as OneIconDemoRow);
    }
    protected override void ClearContainerForItemOverride(Control container)
    {
        // Slots only refer to immutable catalog entries. Keep their compiled bindings
        // while parked; preparing the container replaces the complete row before display.
        base.ClearContainerForItemOverride(container);
    }

    private sealed class RowContainer : StackPanel
    {
        private readonly TextBlock _header = new() { FontWeight = FontWeight.SemiBold, Margin = new Thickness(0, 12, 0, 0) };
        private readonly OneIconGalleryRow _tiles;
        public RowContainer(IDataTemplate? template)
        {
            Spacing = 8;
            _tiles = new OneIconGalleryRow { TileTemplate = template };
            Children.Add(_header);
            Children.Add(_tiles);
        }
        public void SetRow(OneIconDemoRow? row)
        {
            _header.Text = row?.Title;
            _header.IsVisible = row?.HasTitle == true;
            _tiles.Row = row;
        }
    }
}

/// <summary>Preview-only stable native Button slots, retained when a virtualized row is reused.</summary>
public sealed class OneIconGalleryRow : WrapPanel
{
    public static readonly StyledProperty<OneIconDemoRow?> RowProperty =
        AvaloniaProperty.Register<OneIconGalleryRow, OneIconDemoRow?>(nameof(Row));

    public OneIconDemoRow? Row { get => GetValue(RowProperty); set => SetValue(RowProperty, value); }
    public IDataTemplate? TileTemplate { get; set; }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == RowProperty) UpdateSlots();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateSlots();
    }

    private void UpdateSlots()
    {
        if (TileTemplate is null) return;
        var icons = Row?.Icons;
        var count = icons?.Count ?? 0;
        while (Children.Count < count)
        {
            var child = TileTemplate.Build(icons![Children.Count]);
            if (child is null) break;
            Children.Add(child);
        }
        for (var i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            child.DataContext = i < count ? icons![i] : null;
            child.IsVisible = i < count;
        }
    }
}
