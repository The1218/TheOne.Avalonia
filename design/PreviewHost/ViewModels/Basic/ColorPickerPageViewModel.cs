using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Media;

namespace TheOne.DesignPreview;

public sealed class ColorPickerPageViewModel : INotifyPropertyChanged
{
    private Color _color = Color.Parse("#006061");
    private bool _alpha = true, _palette = true;
    public event PropertyChangedEventHandler? PropertyChanged;
    public Color SelectedColor
    {
        get => _color;
        set
        {
            if (_color == value) return;
            _color = value; Notify(); Notify(nameof(Hex)); Notify(nameof(Channels)); Notify(nameof(SelectedBrush));
        }
    }
    public bool AlphaEnabled { get => _alpha; set { if (_alpha == value) return; _alpha = value; Notify(); } }
    public bool PaletteVisible { get => _palette; set { if (_palette == value) return; _palette = value; Notify(); } }
    public string Hex => SelectedColor.ToString();
    public string Channels => $"R {SelectedColor.R}   G {SelectedColor.G}   B {SelectedColor.B}   A {SelectedColor.A}";
    public IBrush SelectedBrush => new SolidColorBrush(SelectedColor);
    private void Notify([CallerMemberName] string? property = null) => PropertyChanged?.Invoke(this, new(property));
}
