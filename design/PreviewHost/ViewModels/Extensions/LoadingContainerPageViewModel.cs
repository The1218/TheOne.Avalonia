using Avalonia.Media;
using Material.Icons;
using ReactiveUI;
namespace TheOne.DesignPreview;

public sealed class LoadingContainerPageViewModel : LoadingDemoViewModel
{
    private static readonly Geometry CustomIcon = Geometry.Parse("M3,1 H13 V4 L9,8 L13,12 V15 H3 V12 L7,8 L3,4 Z");
    public double OverlayOpacity { get; set => this.RaiseAndSetIfChanged(ref field, value); } = .65;
    public bool UseCustomIcon
    {
        get;
        set { if (field == value) return; this.RaiseAndSetIfChanged(ref field, value); this.RaisePropertyChanged(nameof(LoadingIcon)); }
    }
    public bool UseMaterialIcon
    {
        get;
        set { if (field == value) return; this.RaiseAndSetIfChanged(ref field, value); this.RaisePropertyChanged(nameof(LoadingIcon)); }
    }
    public object? LoadingIcon => UseCustomIcon ? UseMaterialIcon ? (object)MaterialIconKind.CloudDownload : CustomIcon : null;
}
