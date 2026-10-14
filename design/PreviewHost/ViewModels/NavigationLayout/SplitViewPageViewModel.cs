using Avalonia.Controls;
using ReactiveUI;

namespace TheOne.DesignPreview;

public sealed class SplitViewPageViewModel : BasicControlViewModel
{
    public IReadOnlyList<SplitViewDisplayMode> Modes { get; } = Enum.GetValues<SplitViewDisplayMode>();
    public IReadOnlyList<SplitViewPanePlacement> Placements { get; } = Enum.GetValues<SplitViewPanePlacement>();
    public SplitViewDisplayMode DisplayMode { get; set => this.RaiseAndSetIfChanged(ref field, value); } = SplitViewDisplayMode.CompactInline;
    public SplitViewPanePlacement PanePlacement { get; set => this.RaiseAndSetIfChanged(ref field, value); }
}
