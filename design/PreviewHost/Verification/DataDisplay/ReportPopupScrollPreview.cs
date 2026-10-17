using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Reporting;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class ReportAuthenticationPreview
{
    internal static void VerifyPopupScroll(Action<bool, string> check)
    {
        var designer = new ReportDesign { Document = new ReportDocument() };
        var owner = new Window { Content = designer, Width = 1200, Height = 800 };
        Motion.SetEnabled(owner, false);
        owner.Show(); Flush();
        try
        {
            Click(owner, Part<TabItem>(designer, "DataTabButton"));
            Click(owner, Part<Button>(designer, "AddDataSourceButton"));
            var dialog = owner.OwnedWindows.Single(w => w.Title == "添加数据源");
            Motion.SetEnabled(dialog, false);
            dialog.Height = 650;
            Click(dialog, Part<CheckBox>(dialog, "ApiRequiresAuthentication"));
            var header = Part<AutoCompleteBox>(dialog, "ApiAuthHeaderName");
            var outer = Part<ScrollViewer>(dialog, "ApiDataSourceFormScroll");
            header.BringIntoView(); Flush();
            check(outer.Extent.Height > outer.Viewport.Height, "Report popup scroll: dialog form overflows");
            var popup = header.GetVisualDescendants().OfType<Popup>().Single(p => p.Name == "PART_Popup");
            ListBox Results() => popup.Child!.GetVisualDescendants().OfType<ListBox>()
                .Single(p => p.Name == "PART_SelectingItemsControl");
            ScrollViewer Inner() => Results().GetVisualDescendants().OfType<ScrollViewer>()
                .Single(p => p.Name == "PART_ScrollViewer");
            void Wheel(Control control, Vector delta, Point? at = null)
            {
                var host = TopLevel.GetTopLevel(control)!;
                var point = control.TranslatePoint(at ?? new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
                host.MouseWheel(point, delta); Flush();
            }
            void Open()
            {
                Click(dialog, Part<Button>(dialog, "ApiAuthHeaderSuggestions"));
                check(header.IsDropDownOpen, "Report popup scroll: native suggestions open");
            }

            Open();
            var offset = outer.Offset;
            check(Inner().Extent.Height <= Inner().Viewport.Height + 1, "Report popup scroll: six suggestions need no scrollbar");
            Wheel(Results(), new Vector(0, -3));
            check(outer.Offset == offset && header.IsDropDownOpen, "Report popup scroll: short list does not scroll the form");
            Wheel(Results(), new Vector(0, 3));
            check(outer.Offset == offset, "Report popup scroll: short list contains upward wheel input");
            Wheel(popup.Child!, new Vector(0, -3), new Point(1, 1));
            check(outer.Offset == offset, "Report popup scroll: popup border contains wheel input");
            var second = (ListBoxItem)Results().ContainerFromIndex(1)!;
            var popupHost = TopLevel.GetTopLevel(second)!;
            var selectedPoint = second.TranslatePoint(new Point(second.Bounds.Width / 2, second.Bounds.Height / 2), popupHost)!.Value;
            popupHost.MouseDown(selectedPoint, MouseButton.Left);
            popupHost.MouseUp(selectedPoint, MouseButton.Left); Flush();
            check(header.Text == "X-API-Key" && !header.IsDropDownOpen, "Report popup scroll: native pointer selection still works");
            Open();
            Wheel(Results(), new Vector(0, -3));
            check(outer.Offset == offset, "Report popup scroll: reopened short list remains isolated");
            header.IsDropDownOpen = false; Flush();

            header.Text = "";
            header.ItemsSource = Enumerable.Range(0, 1000).Select(i => $"Header-{i:0000}").ToArray();
            header.MaxDropDownHeight = 180;
            Open();
            var inner = Inner();
            check(inner.Extent.Height > inner.Viewport.Height &&
                  inner.GetVisualDescendants().OfType<ScrollBar>().Any(b => b.Orientation == Orientation.Vertical && b.IsEffectivelyVisible),
                "Report popup scroll: long suggestions have an internal scrollbar");
            Wheel(Results(), new Vector(0, -3));
            check(inner.Offset.Y > 0 && outer.Offset == offset, "Report popup scroll: long list scrolls internally");
            Wheel(Results(), new Vector(0, -10000));
            Wheel(Results(), new Vector(0, -3));
            check(outer.Offset == offset, "Report popup scroll: bottom boundary remains isolated");
            Wheel(Results(), new Vector(0, 10000));
            Wheel(Results(), new Vector(0, 3));
            check(inner.Offset.Y == 0 && outer.Offset == offset, "Report popup scroll: top boundary remains isolated");
            header.IsDropDownOpen = false; Flush();
            Wheel(header, new Vector(0, offset.Y > 0 ? 3 : -3));
            check(outer.Offset != offset, "Report popup scroll: closed popup restores ordinary form scrolling");
            dialog.Close();
        }
        finally { owner.Close(); }
    }
}
