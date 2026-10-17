using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TheOne.Avalonia.Themes;

namespace TheOne.DesignPreview;

public sealed class ThemeColorApiModel : INotifyPropertyChanged
{
    public ThemeColor Color
    {
        get;
        set
        {
            if (field == value) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public partial class ThemeColorApiFixture : UserControl
{
    public ThemeColorApiFixture()
    {
        DataContext = Model;
        AvaloniaXamlLoader.Load(this);
    }

    public ThemeColorApiModel Model { get; } = new();
}