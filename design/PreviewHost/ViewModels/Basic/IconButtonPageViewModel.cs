using Avalonia.Media;
using Material.Icons;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class IconButtonPageViewModel : LoadingDemoViewModel
{
    public Geometry Icon { get; set => this.RaiseAndSetIfChanged(ref field, value); } = Geometry.Parse("M6,0 H10 V6 H16 V10 H10 V16 H6 V10 H0 V6 H6 Z");
    public MaterialIconKind MaterialIcon { get; set => this.RaiseAndSetIfChanged(ref field, value); } = MaterialIconKind.Home;
}
