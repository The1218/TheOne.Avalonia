using Avalonia.Automation;
using Avalonia.Media;
using Avalonia.Automation.Peers;

namespace Avalonia.Controls.Automation.Peers;

#if !DATAGRID_INTERNAL
public
#endif
class DataGridCellAutomationPeer : ContentControlAutomationPeer
{
    public DataGridCellAutomationPeer(DataGridCell owner)
        : base(owner)
    {
    }

    public new DataGridCell Owner => (DataGridCell)base.Owner;

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.Custom;
    }

    protected override bool IsContentElementCore() => true;

    protected override bool IsControlElementCore() => true;

    protected override bool IsOffscreenCore()
    {
        // Native clipping must also hide a retained current cell/header from automation.
        // Explicit consumer overrides keep their usual semantics.
        if (Owner.OwningColumn?.OwningGrid is { FrozenRightColumnCount: > 0 } &&
            (AutomationProperties.GetIsOffscreenBehavior(Owner) is IsOffscreenBehavior.Default or IsOffscreenBehavior.FromClip) &&
            Owner.Clip is RectangleGeometry clip && (clip.Rect.Width <= 0 || clip.Rect.Height <= 0))
            return true;
        return base.IsOffscreenCore();
    }
}
