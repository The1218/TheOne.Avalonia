using System;
using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Metadata;
using global::Avalonia.Controls.Templates;
using global::Avalonia.Media;

namespace TheOne.Avalonia.Controls;

/// <summary>Hosts content with a modal, theme-aware loading overlay without replacing its DataContext or layout.</summary>
[PseudoClasses(":loading")]
public class LoadingContainer : ContentControl
{
    public static readonly StyledProperty<bool> IsLoadingProperty = AvaloniaProperty.Register<LoadingContainer, bool>(nameof(IsLoading));
    public static readonly StyledProperty<string?> LoadingTextProperty = AvaloniaProperty.Register<LoadingContainer, string?>(nameof(LoadingText), "正在加载…");
    public static readonly StyledProperty<double> OverlayOpacityProperty = AvaloniaProperty.Register<LoadingContainer, double>(nameof(OverlayOpacity), .65, validate: value => double.IsFinite(value) && value is >= 0 and <= 1);
    public static readonly StyledProperty<IBrush?> OverlayBrushProperty = AvaloniaProperty.Register<LoadingContainer, IBrush?>(nameof(OverlayBrush));
    public static readonly StyledProperty<object?> LoadingIconProperty = AvaloniaProperty.Register<LoadingContainer, object?>(nameof(LoadingIcon));
    public static readonly StyledProperty<IDataTemplate?> LoadingIconTemplateProperty = AvaloniaProperty.Register<LoadingContainer, IDataTemplate?>(nameof(LoadingIconTemplate));
    public static readonly StyledProperty<double> LoadingIconSizeProperty = AvaloniaProperty.Register<LoadingContainer, double>(nameof(LoadingIconSize), 28, validate: value => double.IsFinite(value) && value > 0);
    public static readonly DirectProperty<LoadingContainer, bool> HasCustomLoadingIconProperty = AvaloniaProperty.RegisterDirect<LoadingContainer, bool>(nameof(HasCustomLoadingIcon), control => control.HasCustomLoadingIcon);
    public static readonly DirectProperty<LoadingContainer, bool> ShowDefaultLoadingIndicatorProperty = AvaloniaProperty.RegisterDirect<LoadingContainer, bool>(nameof(ShowDefaultLoadingIndicator), control => control.ShowDefaultLoadingIndicator);
    public static readonly DirectProperty<LoadingContainer, bool> HasLoadingTextProperty = AvaloniaProperty.RegisterDirect<LoadingContainer, bool>(nameof(HasLoadingText), control => control.HasLoadingText);
    public bool IsLoading { get => GetValue(IsLoadingProperty); set => SetValue(IsLoadingProperty, value); }
    public string? LoadingText { get => GetValue(LoadingTextProperty); set => SetValue(LoadingTextProperty, value); }
    public double OverlayOpacity { get => GetValue(OverlayOpacityProperty); set => SetValue(OverlayOpacityProperty, value); }
    public IBrush? OverlayBrush { get => GetValue(OverlayBrushProperty); set => SetValue(OverlayBrushProperty, value); }
    public object? LoadingIcon { get => GetValue(LoadingIconProperty); set => SetValue(LoadingIconProperty, value); }
    public IDataTemplate? LoadingIconTemplate { get => GetValue(LoadingIconTemplateProperty); set => SetValue(LoadingIconTemplateProperty, value); }
    public double LoadingIconSize { get => GetValue(LoadingIconSizeProperty); set => SetValue(LoadingIconSizeProperty, value); }
    public bool HasCustomLoadingIcon { get; private set => SetAndRaise(HasCustomLoadingIconProperty, ref field, value); }
    public bool ShowDefaultLoadingIndicator { get; private set => SetAndRaise(ShowDefaultLoadingIndicatorProperty, ref field, value); }
    public bool HasLoadingText { get; private set => SetAndRaise(HasLoadingTextProperty, ref field, value); }
    public LoadingContainer() => UpdateState();
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsLoadingProperty || change.Property == LoadingIconProperty || change.Property == LoadingIconTemplateProperty || change.Property == LoadingTextProperty) UpdateState();
    }
    private void UpdateState()
    {
        HasCustomLoadingIcon = LoadingIcon is not null || LoadingIconTemplate is not null;
        ShowDefaultLoadingIndicator = IsLoading && !HasCustomLoadingIcon;
        HasLoadingText = !string.IsNullOrWhiteSpace(LoadingText);
        PseudoClasses.Set(":loading", IsLoading);
    }
}
