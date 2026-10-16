using System.ComponentModel;
using Avalonia.Controls;
using TheOne.Avalonia.Themes;

namespace TheOne.DesignPreview;

public sealed class ThemeColorApiModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public ThemeColor Color
    {
        get;
        set
        {
            if (field == value) return;
            field = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
        }
    }
}

public partial class ThemeColorApiFixture : UserControl
{
    public ThemeColorApiModel Model { get; } = new();
    public ThemeColorApiFixture() { DataContext = Model; global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this); }
}
