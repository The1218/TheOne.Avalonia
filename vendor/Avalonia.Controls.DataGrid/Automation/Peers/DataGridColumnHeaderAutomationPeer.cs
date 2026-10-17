using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Media;

namespace Avalonia.Controls.Automation.Peers;

#if !DATAGRID_INTERNAL
public
#endif
    class DataGridColumnHeaderAutomationPeer : ContentControlAutomationPeer
{
    public DataGridColumnHeaderAutomationPeer(DataGridColumnHeader owner)
        : base(owner)
    {
    }

    public new DataGridColumnHeader Owner => (DataGridColumnHeader)base.Owner;

    protected override AutomationControlType GetAutomationControlTypeCore()
    {
        return AutomationControlType.HeaderItem;
    }

    protected override bool IsContentElementCore()
    {
        return false;
    }

    protected override bool IsControlElementCore()
    {
        return true;
    }

    protected override bool IsOffscreenCore()
    {
        // Native clipping must also hide a retained current cell/header from automation.
        // Explicit consumer overrides keep their usual semantics.
        if (Owner.OwningGrid is { FrozenRightColumnCount: > 0 } &&
            AutomationProperties.GetIsOffscreenBehavior(Owner) is IsOffscreenBehavior.Default
                or IsOffscreenBehavior.FromClip &&
            Owner.Clip is RectangleGeometry clip && (clip.Rect.Width <= 0 || clip.Rect.Height <= 0))
            return true;
        return base.IsOffscreenCore();
    }
}