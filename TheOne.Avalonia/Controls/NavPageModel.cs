using System.Collections.ObjectModel;
using Avalonia;

namespace TheOne.Avalonia.Controls;

/// <summary>Default navigation data. ViewModel is resolved by the host's data templates.</summary>
public class NavPageModel : AvaloniaObject
{
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<NavPageModel, string>(nameof(Title), "");
    public static readonly StyledProperty<object?> IconProperty = AvaloniaProperty.Register<NavPageModel, object?>(nameof(Icon));
    public static readonly StyledProperty<object?> ViewModelProperty = AvaloniaProperty.Register<NavPageModel, object?>(nameof(ViewModel));
    public static readonly StyledProperty<bool> IsClosableProperty = AvaloniaProperty.Register<NavPageModel, bool>(nameof(IsClosable), true);
    public static readonly StyledProperty<bool> IsPinnedProperty = AvaloniaProperty.Register<NavPageModel, bool>(nameof(IsPinned));
    public string Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public object? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public object? ViewModel { get => GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }
    public bool IsClosable { get => GetValue(IsClosableProperty); set => SetValue(IsClosableProperty, value); }
    public bool IsPinned { get => GetValue(IsPinnedProperty); set => SetValue(IsPinnedProperty, value); }
    public ObservableCollection<NavPageModel> Children { get; } = [];
}
