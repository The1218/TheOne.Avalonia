using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal static partial class MultiSelectBoxPreview
{
    static partial void VerifyPopupScroll(Action<bool, string> check)
    {
        var window = new MainWindow { Width = 1100, Height = 700 };
        window.Model.MotionEnabled = false;
        window.Show();
        Flush();
        try
        {
            var box =
                window.FindControl<ControlGallery>("Gallery")!.CurrentView!
                    .FindControl<MultiSelectBox>("Capabilities")!;
            var outer = window.GetVisualDescendants().OfType<ScrollViewer>().Single(s => s.Name == "PageScroll");
            box.BringIntoView();
            Flush();
            box.OpenDropDown();
            Flush();
            var offset = outer.Offset;

            void Wheel(Control target, Vector delta)
            {
                var host = TopLevel.GetTopLevel(target)!;
                var point = target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), host)!
                    .Value;
                host.MouseWheel(point, delta);
                Flush();
            }

            var list = Results(box);
            Wheel(list, new Vector(0, -3));
            check(outer.Offset == offset,
                "MultiSelectBox/Home: a short popup consumes wheel input instead of scrolling the page");
            Wheel(PopupPart<TextBox>(box, "PART_SearchBox"), new Vector(0, 3));
            check(outer.Offset == offset, "MultiSelectBox/Home: wheel over popup search stays inside the popup");
            box.ItemsSource = Enumerable.Range(0, 1000).Select(i => $"Option {i}").ToArray();
            box.MaxDropDownHeight = 220;
            Flush();
            var inner = list.GetVisualDescendants().OfType<ScrollViewer>().Single(s => s.Name == "PART_ScrollViewer");
            check(
                inner.Extent.Height > inner.Viewport.Height && inner.GetVisualDescendants().OfType<ScrollBar>()
                    .Any(b => b.Orientation == Orientation.Vertical && b.IsEffectivelyVisible),
                "MultiSelectBox/Home: overflowing popup has a bounded viewport and visible vertical scrollbar");
            Wheel(list, new Vector(0, -3));
            check(inner.Offset.Y > 0 && outer.Offset == offset,
                "MultiSelectBox/Home: native wheel scrolls results without moving the outer page");
            Wheel(list, new Vector(0, -1000));
            Wheel(list, new Vector(0, -3));
            check(outer.Offset == offset, "MultiSelectBox/Home: bottom boundary does not chain to the outer page");
            Wheel(list, new Vector(0, 1000));
            Wheel(list, new Vector(0, 3));
            check(inner.Offset.Y == 0 && outer.Offset == offset,
                "MultiSelectBox/Home: top boundary does not chain to the outer page");
            box.SearchText = "no matching option";
            Flush();
            Wheel(PopupPart<TextBlock>(box, "PART_Empty"), new Vector(0, -3));
            check(box.HasNoResults && outer.Offset == offset,
                "MultiSelectBox/Home: empty results also contain wheel input");
            box.CloseDropDown();
            Wheel(box, new Vector(0, -3));
            check(outer.Offset.Y > offset.Y, "MultiSelectBox/Home: closing the popup restores ordinary page scrolling");
        }
        finally
        {
            window.Close();
        }
    }
}