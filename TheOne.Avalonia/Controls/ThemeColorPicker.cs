using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using TheOne.Avalonia.Themes;

namespace TheOne.Avalonia.Controls;

/// <summary>Selects the application OneTheme color palette independently of Light / Dark.</summary>
public class ThemeColorPicker : ComboBox
{
    /// <summary>The selected palette, suitable for a persisted two-way enum binding.</summary>
    public static readonly StyledProperty<ThemeColor> SelectThemeColorProperty =
        AvaloniaProperty.Register<ThemeColorPicker, ThemeColor>(nameof(SelectThemeColor),
            defaultBindingMode: BindingMode.TwoWay, validate: value => Enum.IsDefined(value));

    public static readonly StyledProperty<ThemeColor> SelectedColorProperty =
        AvaloniaProperty.Register<ThemeColorPicker, ThemeColor>(nameof(SelectedColor),
            defaultBindingMode: BindingMode.TwoWay, validate: value => Enum.IsDefined(value));

    private bool _attachedOnce;
    private bool _pendingColor;
    private bool _syncing;
    private bool _syncingColorProperties;
    private OneTheme? _theme;

    public ThemeColorPicker()
    {
        ItemsSource = OneTheme.ColorThemes;
        ItemTemplate = new FuncDataTemplate<ThemeColor>((color, _) =>
            new StackPanel
            {
                Orientation = Orientation.Horizontal, Spacing = 8,
                Children =
                {
                    new Border
                    {
                        Background = ThemePalettes.AccentBrush(color, false), Width = 12, Height = 20,
                        CornerRadius = new CornerRadius(3, 0, 0, 3)
                    },
                    new Border
                    {
                        Background = ThemePalettes.AccentBrush(color, true), Width = 12, Height = 20,
                        Margin = new Thickness(-8, 0, 0, 0), CornerRadius = new CornerRadius(0, 3, 3, 0)
                    },
                    new TextBlock { Text = color.ToString(), VerticalAlignment = VerticalAlignment.Center }
                }
            });
        AutomationProperties.SetName(this, "主题配色");
        ToolTip.SetTip(this, "选择主题配色，明暗模式由主题按钮切换");
        SelectionChanged += (_, _) =>
        {
            if (!_syncing && SelectedItem is ThemeColor color) SetCurrentValue(SelectedColorProperty, color);
        };
        SynchronizeSelection();
    }

    public ThemeColor SelectThemeColor
    {
        get => GetValue(SelectThemeColorProperty);
        set
        {
            if (_theme is null) _pendingColor = true;
            SetValue(SelectThemeColorProperty, value);
        }
    }

    public ThemeColor SelectedColor
    {
        get => GetValue(SelectedColorProperty);
        set
        {
            if (_theme is null) _pendingColor = true;
            SetValue(SelectedColorProperty, value);
        }
    }

    protected override Type StyleKeyOverride => typeof(ComboBox);

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _theme = OneTheme.ApplicationTheme;
        if (_theme is null) return;
        _theme.PropertyChanged += ThemeChanged;
        if (_pendingColor || (!_attachedOnce && (IsSet(SelectedColorProperty) || IsSet(SelectThemeColorProperty))))
            _theme.SetCurrentValue(OneTheme.ColorThemeProperty, SelectedColor);
        else SetCurrentValue(SelectedColorProperty, _theme.ColorTheme);
        _pendingColor = false;
        _attachedOnce = true;
        SynchronizeSelection();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_theme is not null) _theme.PropertyChanged -= ThemeChanged;
        _theme = null;
        base.OnDetachedFromVisualTree(e);
    }

    private void ThemeChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.Property == OneTheme.ColorThemeProperty && _theme is not null)
            SetCurrentValue(SelectedColorProperty, _theme.ColorTheme);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectThemeColorProperty && !_syncingColorProperties)
            SetCurrentValue(SelectedColorProperty, SelectThemeColor);
        if (change.Property != SelectedColorProperty) return;
        _syncingColorProperties = true;
        try
        {
            SetCurrentValue(SelectThemeColorProperty, SelectedColor);
        }
        finally
        {
            _syncingColorProperties = false;
        }

        if (_theme is not null) _theme.SetCurrentValue(OneTheme.ColorThemeProperty, SelectedColor);
        else if (!_syncing) _pendingColor = true;
        SynchronizeSelection();
    }

    private void SynchronizeSelection()
    {
        _syncing = true;
        try
        {
            SetCurrentValue(SelectedItemProperty, SelectedColor);
        }
        finally
        {
            _syncing = false;
        }
    }
}