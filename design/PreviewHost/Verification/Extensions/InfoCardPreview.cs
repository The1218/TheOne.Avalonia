using ReactiveUI;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

// Shared with the independent NativeAOT consumer; no reflection or preview-window dependency.
internal static class InfoCardPreview
{
    public static InfoCard CreateSample() => new()
    {
        Header = "工作区概览",
        Content = "Still 主题 · 标题与正文可自由组合。\n分区背景、细边框和圆角，内容按可用宽度自动换行。",
        HorizontalAlignment = HorizontalAlignment.Stretch
    };

    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            VerifyDensity(theme, size, check);
            VerifySlotsDensity(theme, size, check);
            VerifySeparators(theme, size, check);
            VerifyRegionChrome(theme, size, check);
        }
        VerifyTemplatesAndInteraction(check);
        VerifySlotTemplatesAndInteraction(check);
        VerifyVirtualContent(check);
        VerifyDemoConsumer(check);
        VerifySlotsDemoConsumer(check);
        VerifyConstrainedContent(check);
    }

    private static Window Open(Control content, ThemeVariant theme, ControlSize size, double width = 620, double height = 480)
    {
        var window = new Window
        {
            Width = width, Height = height, WindowDecorations = WindowDecorations.None,
            RequestedThemeVariant = theme,
            Content = new Border { Padding = new Thickness(24), Child = content }
        };
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false);
        window.Show(); Flush(); return window;
    }

    private static void VerifyDensity(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var card = CreateSample(); card.VerticalAlignment = VerticalAlignment.Top;
        var window = Open(card, theme, size);
        var label = $"InfoCard/{theme.Key}/{size}";
        var font = size switch { ControlSize.Small => 12, ControlSize.Large => 16, _ => 14 };
        var padding = size switch { ControlSize.Small => 16, ControlSize.Large => 24, _ => 20 };
        var gap = size switch { ControlSize.Small => 8, ControlSize.Large => 16, _ => 12 };
        var surface = Part<Border>(card, "PART_Surface");
        var header = Part<ContentPresenter>(card, "PART_HeaderPresenter");
        var body = Part<ContentPresenter>(card, "PART_ContentPresenter");
        var host = Part<Control>(card, "PART_HeaderHost");
        check(card.FontSize == font && card.HeaderFontSize == font + 2 && card.Padding == new Thickness(padding) && card.HeaderSpacing == gap, $"{label}: inherited density controls text, header, padding and spacing");
        check(Colour(surface.Background) == Resource(card, "One.Surface") && Colour(surface.BorderBrush) == Resource(card, "One.Line") && Colour(card.Foreground) == Resource(card, "One.Text"), $"{label}: template uses Still semantic surface, outline and text");
        check(!card.Focusable && card.Bounds.Width > 500 && card.Bounds.Height < 220 && double.IsNaN(card.Height), $"{label}: presentation card sizes to content without adding a tab stop");
        check(header.Content == card.Header && body.Content == card.Content && host.IsVisible && Math.Abs(body.TranslatePoint(default, window)!.Value.Y - header.TranslatePoint(new Point(0, header.Bounds.Height), window)!.Value.Y - gap - padding - 1) < 1, $"{label}: header and content retain independent presenters and density-aware section padding and a divider");
        var title = header.GetVisualDescendants().OfType<TextBlock>().First();
        var text = body.GetVisualDescendants().OfType<TextBlock>().First();
        check(title.FontSize == font + 2 && text.FontSize == font && text.TextLayout.TextLines.Count >= 2, $"{label}: string presenters inherit header/body typography and wrap multiline text");
        var originalHeight = card.Bounds.Height;
        card.Header = null; Flush();
        check(!host.IsVisible && card.Bounds.Height < originalHeight && Math.Abs(body.TranslatePoint(default, window)!.Value.Y - card.TranslatePoint(new Point(0, padding + card.BorderThickness.Top), window)!.Value.Y) < 1, $"{label}: removing the header removes its height and spacing");
        card.Header = ""; Flush();
        check(!host.IsVisible, $"{label}: an empty title leaves no empty header row");
        card.Header = "   "; Flush();
        check(!host.IsVisible, $"{label}: a whitespace title leaves no empty header row");
        card.Header = "更新后的标题"; card.Content = null; Flush();
        check(host.IsVisible && !Part<Border>(card, "PART_HeaderGap").IsVisible, $"{label}: a title-only card has no trailing body gap");
        card.Content = "新内容";
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        ControlSizing.SetSize(window, size == ControlSize.Large ? ControlSize.Small : ControlSize.Large); Flush();
        check(Colour(surface.Background) == Resource(card, "One.Surface") && card.Padding.Left == (size == ControlSize.Large ? 16 : 24) && body.Content as string == "新内容", $"{label}: live theme and density changes update the existing content");
        card.IsEnabled = false; Flush();
        check(Colour(surface.Background) == Resource(card, "One.DisabledSurface") && Colour(card.Foreground) == Resource(card, "One.DisabledText"), $"{label}: disabled surface and text use shared semantic colors");
        window.Close();
    }

    private static void VerifySeparators(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var card = new InfoCard { Header = "标题", Content = "正文", Footer = "页脚", VerticalAlignment = VerticalAlignment.Top };
        var window = Open(card, theme, size);
        var label = $"InfoCard/separators/{theme.Key}/{size}";
        var header = Part<Border>(card, "PART_HeaderSeparator");
        var footer = Part<Border>(card, "PART_FooterSeparator");
        var headerGap = Part<Border>(card, "PART_HeaderGap");
        var footerGap = Part<Border>(card, "PART_FooterGap");
        var body = Part<ContentPresenter>(card, "PART_ContentPresenter");
        bool Visible(Control control) => control.IsVisible && control.GetVisualAncestors().OfType<Control>().All(parent => parent.IsVisible);
        check(card.ShowHeaderSeparator && card.ShowFooterSeparator && Visible(header) && Visible(footer), $"{label}: both dividers are enabled by default between populated regions");
        check(header.Bounds.Height == 1 && footer.Bounds.Height == 1 && Math.Abs(header.Bounds.Width - (card.Bounds.Width - card.BorderThickness.Left - card.BorderThickness.Right)) < 1 &&
            Colour(header.Background) == Resource(card, "One.Line") && Colour(footer.Background) == Resource(card, "One.Line") &&
            !header.IsHitTestVisible && !footer.IsHitTestVisible, $"{label}: dividers span from the left outline to the right outline with semantic brushes and no input interception");
        var height = card.Bounds.Height;
        card.ShowHeaderSeparator = false; Flush();
        check(!Visible(header) && Visible(footer) && card.Bounds.Height == height - 1 && headerGap.Bounds.Height == 0,
            $"{label}: hiding the header divider preserves the footer and established spacing");
        card.ShowHeaderSeparator = true; card.ShowFooterSeparator = false; Flush();
        check(Visible(header) && !Visible(footer) && card.Bounds.Height == height - 1 && footerGap.Bounds.Height == 0,
            $"{label}: hiding the footer divider independently preserves layout");
        card.ShowFooterSeparator = true; card.HeaderSpacing = card.FooterSpacing = 0; Flush();
        check(headerGap.Bounds.Height == 1 && footerGap.Bounds.Height == 1 && header.Bounds.Height == 1 && footer.Bounds.Height == 1,
            $"{label}: zero spacing still reserves one divider row without overlapping adjacent content");
        card.ShowHeaderSeparator = card.ShowFooterSeparator = false; Flush();
        check(headerGap.Bounds.Height == 0 && footerGap.Bounds.Height == 0, $"{label}: zero spacing with hidden dividers leaves no reserved rows");
        card.ShowHeaderSeparator = card.ShowFooterSeparator = true;
        card.Content = null; Flush();
        check(!Visible(header) && Visible(footer), $"{label}: header plus footer without a body gets a single footer divider");
        card.Header = null; Flush();
        check(!Visible(header) && !Visible(footer) && !footerGap.IsVisible, $"{label}: a footer-only card has no orphan divider or gap");
        card.Content = "正文"; card.Footer = null; Flush();
        check(!Visible(header) && !Visible(footer), $"{label}: a body-only card has no orphan dividers");
        card.Header = "标题"; card.Footer = "页脚";
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light; Flush();
        check(Visible(header) && Visible(footer) && Colour(header.Background) == Resource(card, "One.Line") && Colour(footer.Background) == Resource(card, "One.Line"),
            $"{label}: populated regions stay visible and live theme changes refresh both dividers");
        window.Close();
    }

    private static void VerifyRegionChrome(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var card = new InfoCard { Header = "标题", Content = "正文", Footer = "页脚", VerticalAlignment = VerticalAlignment.Top };
        var window = Open(card, theme, size);
        var label = $"InfoCard/regions/{theme.Key}/{size}";
        var header = Part<Border>(card, "PART_HeaderHost");
        var body = Part<Border>(card, "PART_ContentHost");
        var footer = Part<Border>(card, "PART_FooterHost");
        var topLine = Part<Border>(card, "PART_HeaderSeparator");
        var bottomLine = Part<Border>(card, "PART_FooterSeparator");
        bool EdgeToEdge(Control region) => Math.Abs(region.TranslatePoint(default, card)!.Value.X - card.BorderThickness.Left) < 0.01 &&
            Math.Abs(region.TranslatePoint(new Point(region.Bounds.Width, 0), card)!.Value.X - card.Bounds.Width + card.BorderThickness.Right) < 0.01;
        check(Colour(header.Background) == Resource(card, "One.Subtle") && Colour(body.Background) == Resource(card, "One.Surface") &&
            Colour(footer.Background) == Resource(card, "One.InfoCard.Footer") && Colour(header.Background) != Colour(body.Background) &&
            Colour(footer.Background) != Colour(body.Background) && Colour(header.Background) != Colour(footer.Background),
            $"{label}: header, body and footer use three distinct theme-aware defaults");
        Click(header);
        check(body.IsVisible && footer.IsVisible && !card.GetVisualDescendants().OfType<RevealHost>().Any(),
            $"{label}: the static header has no expansion host and clicking it keeps body and footer visible");
        var bodyColor = Colour(body.Background)!.Value; var footerColor = Colour(footer.Background)!.Value;
        check(Math.Abs(bodyColor.R - footerColor.R) <= 4 && Math.Abs(bodyColor.G - footerColor.G) <= 4 && Math.Abs(bodyColor.B - footerColor.B) <= 4,
            $"{label}: footer differs from the content by at most four levels per color channel");
        check(EdgeToEdge(header) && EdgeToEdge(body) && EdgeToEdge(footer) && EdgeToEdge(topLine) && EdgeToEdge(bottomLine),
            $"{label}: every background and divider meets both outline edges without horizontal insets");
        check(Math.Abs(topLine.TranslatePoint(default, card)!.Value.Y - header.TranslatePoint(new Point(0, header.Bounds.Height), card)!.Value.Y) < 0.01 &&
            Math.Abs(bottomLine.TranslatePoint(new Point(0, bottomLine.Bounds.Height), card)!.Value.Y - footer.TranslatePoint(default, card)!.Value.Y) < 0.01,
            $"{label}: dividers are flush with the boundary between neighboring backgrounds");
        card.Padding = new Thickness(13, 9, 27, 17); card.HeaderSpacing = 7; card.FooterSpacing = 5;
        card.BorderThickness = new Thickness(2, 3, 4, 5); card.CornerRadius = new CornerRadius(18); window.Width = 420; Flush();
        check(EdgeToEdge(header) && EdgeToEdge(body) && EdgeToEdge(footer) && EdgeToEdge(topLine) && EdgeToEdge(bottomLine) &&
            header.Padding == new Thickness(13, 7, 27, 7) && body.Padding == card.Padding && footer.Padding == new Thickness(13, 5, 27, 5),
            $"{label}: asymmetric padding, outline widths and resizing preserve connected chrome and independent region insets");
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light; Flush();
        check(Colour(header.Background) == Resource(card, "One.Subtle") && Colour(body.Background) == Resource(card, "One.Surface") && Colour(footer.Background) == Resource(card, "One.InfoCard.Footer"),
            $"{label}: all three backgrounds react to a live theme change");
        card.HeaderBackground = Brushes.AliceBlue; card.Background = Brushes.Ivory; card.FooterBackground = Brushes.Lavender; Flush();
        check(header.Background == Brushes.AliceBlue && body.Background == Brushes.Ivory && footer.Background == Brushes.Lavender,
            $"{label}: callers can override the backgrounds of all three regions independently");
        card.ClearValue(InfoCard.HeaderBackgroundProperty); card.ClearValue(InfoCard.FooterBackgroundProperty); card.ClearValue(InfoCard.BackgroundProperty);
        card.IsEnabled = false; Flush();
        check(Colour(header.Background) == Resource(card, "One.DisabledSurface") && Colour(body.Background) == Resource(card, "One.DisabledSurface") && Colour(footer.Background) == Resource(card, "One.DisabledSurface"),
            $"{label}: disabled styling covers the complete card including header and footer");
        window.Close();
    }

    private static void VerifyTemplatesAndInteraction(Action<bool, string> check)
    {
        var model = new SampleModel();
        var card = new InfoCard { Header = model, Content = model, VerticalAlignment = VerticalAlignment.Top };
        card.HeaderTemplate = new FuncDataTemplate<SampleModel>((_, _) =>
        {
            var text = new TextBlock { Name = "InfoCardCompiledHeader" };
            text.Bind(TextBlock.TextProperty, CompiledBinding.Create<SampleModel, string>(value => value.Title));
            return text;
        });
        card.ContentTemplate = new FuncDataTemplate<SampleModel>((_, _) =>
        {
            var text = new TextBlock { Name = "InfoCardCompiledContent" };
            text.Bind(TextBlock.TextProperty, CompiledBinding.Create<SampleModel, string>(value => value.Message));
            return text;
        });
        var window = Open(card, ThemeVariant.Light, ControlSize.Normal);
        var title = card.GetVisualDescendants().OfType<TextBlock>().Single(value => value.Name == "InfoCardCompiledHeader");
        var content = card.GetVisualDescendants().OfType<TextBlock>().Single(value => value.Name == "InfoCardCompiledContent");
        check(title.Text == "编译绑定标题" && content.Text == "编译绑定正文" && ReferenceEquals(title.DataContext, model) && ReferenceEquals(content.DataContext, model), "InfoCard: both native presenters supply the typed model to compiled data templates");
        model.Update(); Flush();
        check(title.Text == "更新标题" && content.Text == "更新正文", "InfoCard: typed header and body bindings react to property changes without rebuilding the card");

        var count = 0;
        var button = new Button { Content = "保存", Command = ReactiveCommand.Create<object?>(_ => count++) };
        var editor = new TextBox { Text = "workspace" };
        card.HeaderTemplate = null; card.ContentTemplate = null;
        card.Header = button; card.Content = editor; Flush();
        check(ReferenceEquals(Part<ContentPresenter>(card, "PART_HeaderPresenter").Child, button) && ReferenceEquals(Part<ContentPresenter>(card, "PART_ContentPresenter").Child, editor), "InfoCard: caller-owned header controls and content preserve their identity");
        Click(editor); window.KeyTextInput(" edited"); Flush();
        check(editor.IsFocused && editor.Text!.Contains("edited"), "InfoCard: embedded TextBox retains pointer focus and native text input");
        Click(button);
        check(count == 1, "InfoCard: real header button activation invokes the embedded command");
        card.IsEnabled = false; Flush(); Click(button);
        check(count == 1 && !button.IsEffectivelyEnabled && !editor.IsEffectivelyEnabled, "InfoCard: disabling the card also disables embedded native interactions");
        card.IsEnabled = true;
        card.Classes.Add("small"); Flush();
        check(card.FontSize == 12 && card.HeaderFontSize == 14 && card.Padding.Left == 16 && card.HeaderSpacing == 8, "InfoCard: an explicit small style class overrides the inherited normal density");
        card.Classes.Remove("small");
        card.Padding = new Thickness(11); card.FontSize = 19; card.HeaderFontSize = 23; card.HeaderSpacing = 7;
        card.BorderBrush = Brushes.Red; card.BorderThickness = new Thickness(2); card.CornerRadius = new CornerRadius(6);
        ControlSizing.SetSize(window, ControlSize.Large); Flush();
        var surface = Part<Border>(card, "PART_Surface");
        check(Part<Border>(card, "PART_ContentHost").Padding == new Thickness(11) && Part<Border>(card, "PART_HeaderHost").Padding == new Thickness(11, 7, 11, 7) && card.FontSize == 19 && card.HeaderFontSize == 23 && Part<Border>(card, "PART_HeaderGap").Bounds.Height == 1 && surface.BorderBrush == Brushes.Red && surface.BorderThickness == new Thickness(2) && surface.CornerRadius == new CornerRadius(6), "InfoCard: local styling overrides density and reaches the visible surface");
        window.Close();
    }

    private static void VerifySlotsDensity(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var action = new Button { Content = "编辑" };
        var card = CreateSample();
        card.HeaderIcon = CreateIcon(); card.HeaderActions = action; card.Footer = "最近更新 · 已保存";
        card.VerticalAlignment = VerticalAlignment.Top;
        var window = Open(card, theme, size);
        var label = $"InfoCard/slots/{theme.Key}/{size}";
        var iconSize = size switch { ControlSize.Small => 16, ControlSize.Large => 20, _ => 18 };
        var insideGap = size switch { ControlSize.Small => 8, ControlSize.Large => 12, _ => 10 };
        var footerGap = size switch { ControlSize.Small => 8, ControlSize.Large => 16, _ => 12 };
        var icon = Part<ContentPresenter>(card, "PART_HeaderIconPresenter");
        var title = Part<ContentPresenter>(card, "PART_HeaderPresenter");
        var actions = Part<ContentPresenter>(card, "PART_HeaderActionsPresenter");
        var body = Part<ContentPresenter>(card, "PART_ContentPresenter");
        var footer = Part<ContentPresenter>(card, "PART_FooterPresenter");
        var layout = Part<Grid>(card, "PART_Layout");
        check(icon.Bounds.Size == new Size(iconSize, iconSize) && card.HeaderIconSize == iconSize && card.HeaderContentSpacing == insideGap && card.FooterSpacing == footerGap, $"{label}: icon bounds and new spacing inherit the selected density");
        check(ReferenceEquals(icon.Child, card.HeaderIcon) && ReferenceEquals(actions.Child, action) && footer.Child is TextBlock { Text: "最近更新 · 已保存" } && footer.FontSize == card.FontSize && Colour(footer.Foreground) == Resource(card, "One.Text"), $"{label}: new slots render native controls and semantic footer typography");
        check(Math.Abs(title.TranslatePoint(default, window)!.Value.X - icon.TranslatePoint(new Point(icon.Bounds.Width, 0), window)!.Value.X - insideGap) < 1 && Math.Abs(actions.TranslatePoint(new Point(actions.Bounds.Width, 0), layout)!.Value.X - layout.Bounds.Width + card.Padding.Right) < 1, $"{label}: icon gap is exact and actions align with the card's inner right edge");
        check(Math.Abs(footer.TranslatePoint(default, window)!.Value.Y - body.TranslatePoint(new Point(0, body.Bounds.Height), window)!.Value.Y - footerGap - card.Padding.Bottom - 1) < 1, $"{label}: footer follows the body with one density-aware gap");
        var originalHeight = card.Bounds.Height;
        card.Footer = null; Flush();
        check(!footer.IsVisible && !Part<Border>(card, "PART_FooterGap").IsVisible && card.Bounds.Height < originalHeight, $"{label}: removing the footer removes its row and spacing");
        card.Footer = " "; Flush();
        check(!footer.IsVisible, $"{label}: a whitespace footer leaves no empty row");
        card.Footer = "页脚"; card.Content = null; Flush();
        var header = Part<Border>(card, "PART_HeaderHost");
        check(!Part<Border>(card, "PART_HeaderGap").IsVisible && Math.Abs(footer.TranslatePoint(default, window)!.Value.Y - header.TranslatePoint(new Point(0, header.Bounds.Height), window)!.Value.Y - footerGap - 1) < 1, $"{label}: a header/footer card without a body has one footer gap");
        card.Header = null; Flush();
        check(header.IsVisible && !title.IsVisible && icon.IsVisible && actions.IsVisible && !Part<Border>(card, "PART_HeaderActionsGap").IsVisible, $"{label}: icon and actions retain the header without title text or a redundant title gap");
        card.HeaderIcon = null; Flush();
        check(header.IsVisible && actions.IsVisible && !Part<Border>(card, "PART_HeaderIconGap").IsVisible, $"{label}: actions alone retain right alignment with no empty icon gap");
        card.HeaderActions = null; Flush();
        check(!header.IsVisible && !Part<Border>(card, "PART_FooterGap").IsVisible && Math.Abs(footer.TranslatePoint(default, layout)!.Value.Y - footerGap) < 1, $"{label}: a footer-only card retains only its own vertical padding");
        card.HeaderIcon = CreateIcon(); Flush();
        check(header.IsVisible && !Part<Border>(card, "PART_HeaderIconGap").IsVisible && !Part<Border>(card, "PART_HeaderActionsGap").IsVisible, $"{label}: an icon-only header has no unused horizontal gaps");
        card.Header = "这是一段需要在窄视口中换行的较长信息卡片标题"; card.HeaderActions = action; card.Content = "正文";
        window.Width = 360; Flush();
        var titleText = title.GetVisualDescendants().OfType<TextBlock>().Single();
        check(titleText.TextLayout.TextLines.Count > 1 && title.Bounds.Width > 0 && title.TranslatePoint(new Point(title.Bounds.Width, 0), window)!.Value.X <= actions.TranslatePoint(default, window)!.Value.X && footer.Bounds.Width <= layout.Bounds.Width, $"{label}: narrow layout wraps the title and preserves action/footer bounds");
        window.RequestedThemeVariant = theme == ThemeVariant.Light ? ThemeVariant.Dark : ThemeVariant.Light;
        ControlSizing.SetSize(window, size == ControlSize.Large ? ControlSize.Small : ControlSize.Large); Flush();
        check(icon.Bounds.Width == (size == ControlSize.Large ? 16 : 20) && footer.FontSize == card.FontSize && Colour(icon.Foreground) == Resource(card, "One.Text") && Colour(footer.Foreground) == Resource(card, "One.Text"), $"{label}: live theme and density changes update every added slot");
        card.IsEnabled = false; Flush();
        check(!action.IsEffectivelyEnabled && Colour(icon.Foreground) == Resource(card, "One.DisabledText") && Colour(footer.Foreground) == Resource(card, "One.DisabledText"), $"{label}: disabled actions and new presenter text use the shared disabled state");
        window.Close();
    }

    private static void VerifySlotTemplatesAndInteraction(Action<bool, string> check)
    {
        var model = new SampleModel();
        var card = new InfoCard { Header = "模板", HeaderIcon = model, HeaderActions = model, Footer = model, Content = "正文", VerticalAlignment = VerticalAlignment.Top };
        card.HeaderIconTemplate = new FuncDataTemplate<SampleModel>((_, _) => CreateIcon());
        card.HeaderActionsTemplate = new FuncDataTemplate<SampleModel>((_, _) =>
        {
            var text = new TextBlock { Name = "InfoCardCompiledActions" };
            text.Bind(TextBlock.TextProperty, CompiledBinding.Create<SampleModel, string>(value => value.Title));
            return text;
        });
        card.FooterTemplate = new FuncDataTemplate<SampleModel>((_, _) =>
        {
            var text = new TextBlock { Name = "InfoCardCompiledFooter" };
            text.Bind(TextBlock.TextProperty, CompiledBinding.Create<SampleModel, string>(value => value.Message));
            return text;
        });
        var window = Open(card, ThemeVariant.Dark, ControlSize.Normal);
        var actions = card.GetVisualDescendants().OfType<TextBlock>().Single(value => value.Name == "InfoCardCompiledActions");
        var footer = card.GetVisualDescendants().OfType<TextBlock>().Single(value => value.Name == "InfoCardCompiledFooter");
        check(actions.Text == model.Title && footer.Text == model.Message && ReferenceEquals(actions.DataContext, model) && ReferenceEquals(footer.DataContext, model) && Part<ContentPresenter>(card, "PART_HeaderIconPresenter").Child is PathIcon, "InfoCard: added slots supply independent typed models to compiled data templates");
        model.Update(); Flush();
        check(actions.Text == "更新标题" && footer.Text == "更新正文", "InfoCard: compiled actions and footer bindings update live without rebuilding the card");
        card.HeaderIconTemplate = null; card.HeaderActionsTemplate = null; card.FooterTemplate = null;
        var count = 0;
        var icon = CreateIcon();
        var action = new Button { Content = "操作", Command = ReactiveCommand.Create<object?>(_ => count++) };
        var footerAction = new Button { Content = "提交", Command = ReactiveCommand.Create<object?>(_ => count += 10) };
        var editor = new TextBox { Text = "页脚输入" };
        var footerPanel = new StackPanel { Spacing = 8, Children = { editor, footerAction } };
        card.DataContext = model; card.HeaderIcon = icon; card.HeaderActions = action; card.Footer = footerPanel; Flush();
        check(ReferenceEquals(icon.Parent, card) && ReferenceEquals(action.Parent, card) && ReferenceEquals(footerPanel.Parent, card) && ReferenceEquals(action.DataContext, model) && ReferenceEquals(editor.DataContext, model), "InfoCard: caller-owned slots share native logical ownership and inherit the card data context");
        Click(action); Click(footerAction);
        check(count == 11, "InfoCard: real pointer activation reaches header actions and footer commands");
        Click(editor); editor.SelectAll(); window.KeyTextInput("edited footer"); Flush();
        check(editor.IsFocused && editor.Text == "edited footer", "InfoCard: footer editor retains native keyboard focus and text input");
        card.IsEnabled = false; Flush(); Click(action); Click(footerAction);
        check(count == 11 && !action.IsEffectivelyEnabled && !footerAction.IsEffectivelyEnabled && !editor.IsEffectivelyEnabled, "InfoCard: disabling the card blocks header/footer commands and editor input");
        card.IsEnabled = true;
        var template = card.Template!;
        card.Template = new FuncControlTemplate<InfoCard>((_, _) => new Border()); Flush();
        card.Template = template; Flush();
        check(ReferenceEquals(Part<ContentPresenter>(card, "PART_HeaderIconPresenter").Child, icon) && ReferenceEquals(Part<ContentPresenter>(card, "PART_HeaderActionsPresenter").Child, action) && ReferenceEquals(Part<ContentPresenter>(card, "PART_FooterPresenter").Child, footerPanel) && ReferenceEquals(action.Parent, card), "InfoCard: replacing and restoring the control template reattaches caller-owned slots without a parent conflict");
        Click(action); Click(footerAction);
        check(count == 22, "InfoCard: header/footer native commands still work after the control template is replaced");
        card.HeaderIcon = null; card.HeaderActions = null; card.Footer = null; Flush();
        check(icon.Parent is null && action.Parent is null && footerPanel.Parent is null, "InfoCard: clearing caller-owned slots releases their logical parents");
        card.Header = null; card.Content = null;
        var emptyTemplate = new FuncDataTemplate<object>((_, _) => new TextBlock { Text = "模板提供内容" });
        card.HeaderTemplate = emptyTemplate; card.HeaderIconTemplate = emptyTemplate; card.HeaderActionsTemplate = emptyTemplate; card.FooterTemplate = emptyTemplate; Flush();
        check(Part<Border>(card, "PART_HeaderHost").IsVisible && Part<ContentPresenter>(card, "PART_HeaderPresenter").Child is TextBlock { Text: "模板提供内容" } && Part<ContentPresenter>(card, "PART_HeaderIconPresenter").IsVisible && Part<ContentPresenter>(card, "PART_HeaderActionsPresenter").IsVisible && Part<ContentPresenter>(card, "PART_FooterPresenter").Child is TextBlock { Text: "模板提供内容" }, "InfoCard: explicit templates can render null data and independently keep their regions visible");
        card.HeaderTemplate = null; card.HeaderIconTemplate = null; card.HeaderActionsTemplate = null; card.FooterTemplate = null; Flush();
        check(!Part<Border>(card, "PART_HeaderHost").IsVisible && !Part<ContentPresenter>(card, "PART_FooterPresenter").IsVisible && !Part<Border>(card, "PART_HeaderGap").IsVisible && !Part<Border>(card, "PART_FooterGap").IsVisible, "InfoCard: clearing template-only regions collapses the complete empty layout");
        card.Header = "标题"; card.HeaderIcon = CreateIcon(); card.HeaderActions = "操作"; card.Content = "内容"; card.Footer = "底部";
        card.HeaderIconSize = 25; card.HeaderContentSpacing = 5; card.FooterSpacing = 19; Flush();
        check(Part<ContentPresenter>(card, "PART_HeaderIconPresenter").Bounds.Width == 25 && Part<Border>(card, "PART_HeaderIconGap").Bounds.Width == 5 && Part<Border>(card, "PART_HeaderActionsGap").Bounds.Width == 5 && Part<Border>(card, "PART_FooterHost").Padding.Top == 19 && Part<Border>(card, "PART_FooterGap").Bounds.Height == 1, "InfoCard: caller styling controls actual icon dimensions and independent header/footer gaps");
        window.Close();
    }

    private static PathIcon CreateIcon() => new()
    {
        Data = Geometry.Parse("M12,0 L15,8 L24,9 L18,15 L20,24 L12,19 L4,24 L6,15 L0,9 L9,8 Z")
    };

    private static void VerifyConstrainedContent(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var rows = Enumerable.Range(1, 1_000).Select(index => $"成员 {index:D4}").ToArray();
            var list = new ListBox { ItemsSource = rows };
            var grid = new Grid { RowDefinitions = RowDefinitions.Parse("*") }; grid.Children.Add(list);
            var footer = new Button { Content = "保持页脚可见" };
            var card = new InfoCard { Header = "仅卡片高度约束列表", Height = 320, Content = grid, Footer = footer, VerticalAlignment = VerticalAlignment.Top };
            var window = Open(card, theme, size);
            var label = $"InfoCard/constrained/{theme.Key}/{size}";
            var initial = list.GetRealizedContainers().ToArray();
            check(double.IsNaN(list.Height) && list.Bounds.Height is > 60 and < 260 && initial.Length is > 0 and < 40 && list.ContainerFromIndex(999) is null &&
                footer.TranslatePoint(new Point(0, footer.Bounds.Height), card)!.Value.Y <= card.Bounds.Height - card.BorderThickness.Bottom,
                $"{label}: fixed card height supplies a finite virtual viewport and keeps the footer visible");
            list.SelectedIndex = 999; list.ScrollIntoView(999); Flush();
            var distant = list.GetRealizedContainers().ToArray();
            check(list.ContainerFromIndex(999) is { IsVisible: true } && list.ContainerFromIndex(0) is null && distant.Length is > 0 and < 40 && distant.Any(initial.Contains),
                $"{label}: distant selection recycles containers instead of realizing all thousand rows");
            card.Height = double.NaN; card.MaxHeight = 320; Flush();
            check(card.Bounds.Height <= 320 && list.Bounds.Height > 0 && list.GetRealizedContainers().Count() is > 0 and < 40 &&
                footer.TranslatePoint(new Point(0, footer.Bounds.Height), card)!.Value.Y <= card.Bounds.Height - card.BorderThickness.Bottom,
                $"{label}: MaxHeight also constrains the native list and retains the footer");
            window.Close();
        }
    }

    private static void VerifyVirtualContent(Action<bool, string> check)
    {
        var rows = Enumerable.Range(1, 1_000).Select(index => $"成员 {index:D4}").ToArray();
        var list = new ListBox { Height = 200, ItemsSource = rows };
        var card = new InfoCard { Header = "成员", Content = list, VerticalAlignment = VerticalAlignment.Top };
        var window = Open(card, ThemeVariant.Dark, ControlSize.Normal);
        var initial = list.GetRealizedContainers().ToArray();
        check(list.ItemCount == 1_000 && initial.Length is > 0 and < 40 && list.ContainerFromIndex(999) is null && list.Bounds.Height == 200, "InfoCard: bounded list content realizes only a viewport of 1,000 data models");
        list.SelectedIndex = 999; list.ScrollIntoView(999); Flush();
        var distant = list.GetRealizedContainers().ToArray();
        check(list.SelectedItem as string == rows[999] && list.ContainerFromIndex(999) is { IsVisible: true } && list.ContainerFromIndex(0) is null && distant.Length is > 0 and < 40 && distant.Any(initial.Contains), "InfoCard: distant selection materializes the last member and recycles native list containers");
        list.ContainerFromIndex(999)!.Focus();
        window.KeyPress(Key.Up, RawInputModifiers.None, PhysicalKey.ArrowUp, null);
        window.KeyRelease(Key.Up, RawInputModifiers.None, PhysicalKey.ArrowUp, null); Flush();
        check(list.SelectedIndex == 998 && list.SelectedItem as string == rows[998], "InfoCard: embedded list preserves keyboard selection after a distant scroll");
        window.Close();
    }

    private static void VerifyDemoConsumer(Action<bool, string> check)
    {
        var demo = new InfoCardDemoView();
        var scroll = new ScrollViewer { Content = demo, HorizontalScrollBarVisibility = global::Avalonia.Controls.Primitives.ScrollBarVisibility.Disabled };
        var window = Open(scroll, ThemeVariant.Dark, ControlSize.Normal, height: 640);
        var card = demo.FindControl<InfoCard>("InfoCardSample")!;
        var templated = demo.FindControl<InfoCard>("InfoCardTemplated")!;
        var typedTitle = templated.GetVisualDescendants().OfType<TextBlock>().Single(value => value.Name == "InfoCardTypedTitle");
        var typedContent = templated.GetVisualDescendants().OfType<TextBlock>().Single(value => value.Name == "InfoCardTypedDescription");
        check(card.Header as string == demo.Model.VisibleTitle && card.Content as string == demo.Model.Description && typedTitle.Text == demo.Model.Title && typedContent.Text == demo.Model.Description && ReferenceEquals(typedTitle.DataContext, demo.Model) && ReferenceEquals(typedContent.DataContext, demo.Model), "InfoCard/consumer: compiled AXAML renders ordinary bindings and typed header/content templates");
        var toggle = demo.FindControl<CheckBox>("InfoCardShowHeader")!;
        ClickVisible(toggle);
        check(!demo.Model.ShowHeader && card.Header is null && !Part<Control>(card, "PART_HeaderHost").IsVisible, "InfoCard/consumer: two-way title toggle removes the bound header and its spacing");
        ClickVisible(toggle);
        ClickVisible(demo.FindControl<Button>("InfoCardUpdate")!);
        check(demo.Model.Revision == 2 && card.Content as string == demo.Model.Description && typedContent.Text == demo.Model.Description && typedTitle.Text == demo.Model.Title, "InfoCard/consumer: compiled command updates both ordinary content and typed template content");
        var editor = demo.FindControl<TextBox>("InfoCardName")!;
        ClickVisible(editor); editor.SelectAll(); window.KeyTextInput("AOT workspace"); Flush();
        check(demo.Model.WorkspaceName == "AOT workspace", "InfoCard/consumer: compiled two-way editor binding updates the typed view model");
        ClickVisible(demo.FindControl<Button>("InfoCardHeaderAction")!);
        check(demo.Model.SaveCount == 1 && demo.FindControl<TextBlock>("InfoCardSaveStatus")!.Text!.Contains("AOT workspace"), "InfoCard/consumer: custom header invokes the compiled save command and renders its status");
        ClickVisible(demo.FindControl<CheckBox>("InfoCardEnabled")!);
        check(!demo.Model.Enabled && !editor.IsEffectivelyEnabled && !demo.FindControl<Button>("InfoCardHeaderAction")!.IsEffectivelyEnabled, "InfoCard/consumer: the bound enable switch disables embedded native controls");
        var list = demo.FindControl<ListBox>("InfoCardMembers")!;
        list.BringIntoView(); Flush();
        check(list.ItemCount == 1_000 && list.GetRealizedContainers().Count() is > 0 and < 40 && list.ContainerFromIndex(999) is null, "InfoCard/consumer: finite outer scrolling preserves the bounded thousand-member virtual viewport");
        ClickVisible(demo.FindControl<Button>("InfoCardJumpLast")!);
        check(list.SelectedIndex == 999 && list.ContainerFromIndex(999) is { IsVisible: true } && list.GetRealizedContainers().Count() is > 0 and < 40 && demo.FindControl<TextBlock>("InfoCardListMetrics")!.Text!.Contains("1000"), "InfoCard/consumer: preview jump action realizes the distant model and updates metrics");
        window.Close();
    }

    private static void VerifySlotsDemoConsumer(Action<bool, string> check)
    {
        var demo = new InfoCardDemoView();
        var scroll = new ScrollViewer { Content = demo, HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled };
        var window = Open(scroll, ThemeVariant.Light, ControlSize.Normal, height: 680);
        var card = demo.FindControl<InfoCard>("InfoCardSlots")!;
        T Named<T>(string name) where T : Control => card.GetVisualDescendants().OfType<T>().Single(value => value.Name == name);
        var icon = Named<PathIcon>("InfoCardTypedIcon");
        var actions = Named<Button>("InfoCardTypedHeaderAction");
        var footerAction = Named<Button>("InfoCardFooterAction");
        var status = Named<TextBlock>("InfoCardTypedFooterStatus");
        check(ReferenceEquals(icon.DataContext, demo.Model) && ReferenceEquals(actions.DataContext, demo.Model) && ReferenceEquals(status.DataContext, demo.Model) && ReferenceEquals(icon.Data, demo.Model.IconData) && status.Text == demo.Model.FooterStatus, "InfoCard/slots/consumer: compiled AXAML supplies typed icon, actions and footer templates with their own models");
        ClickVisible(demo.FindControl<CheckBox>("InfoCardShowHeaderSeparator")!);
        check(!demo.Model.ShowHeaderSeparator && !card.ShowHeaderSeparator && !Part<Border>(card, "PART_HeaderSeparator").IsVisible && Part<Border>(card, "PART_FooterSeparator").IsVisible,
            "InfoCard/separators/consumer: header checkbox updates its compiled binding independently");
        ClickVisible(demo.FindControl<CheckBox>("InfoCardShowFooterSeparator")!);
        check(!demo.Model.ShowFooterSeparator && !card.ShowFooterSeparator && !Part<Border>(card, "PART_FooterSeparator").IsVisible,
            "InfoCard/separators/consumer: footer checkbox updates its compiled binding independently");
        ClickVisible(actions);
        check(demo.Model.DetailsCount == 1 && actions.Content as string == demo.Model.DetailsLabel && status.Text == demo.Model.FooterStatus, "InfoCard/slots/consumer: the native header action updates its label and footer status through compiled bindings");
        ClickVisible(footerAction);
        check(demo.Model.SyncCount == 1 && status.Text == demo.Model.FooterStatus && status.Text!.Contains("已同步"), "InfoCard/slots/consumer: the native footer command updates the bound status");
        var geometry = icon.Data;
        ClickVisible(demo.FindControl<Button>("InfoCardUpdate")!);
        check(demo.Model.Revision == 2 && !ReferenceEquals(icon.Data, geometry) && ReferenceEquals(icon.Data, demo.Model.IconData), "InfoCard/slots/consumer: updating the typed model changes the bound icon geometry");
        ClickVisible(demo.FindControl<CheckBox>("InfoCardShowHeader")!);
        check(card.Header is null && Part<Border>(card, "PART_HeaderHost").IsVisible && !Part<ContentPresenter>(card, "PART_HeaderPresenter").IsVisible, "InfoCard/slots/consumer: removing the title preserves independent icon and action regions");
        ClickVisible(demo.FindControl<CheckBox>("InfoCardShowHeaderActions")!);
        check(!demo.Model.ShowHeaderActions && card.HeaderActions is null && card.HeaderActionsTemplate is null && !Part<ContentPresenter>(card, "PART_HeaderActionsPresenter").IsVisible && !Part<Border>(card, "PART_HeaderActionsGap").IsVisible, "InfoCard/slots/consumer: the action switch removes content, template and all action spacing");
        ClickVisible(demo.FindControl<CheckBox>("InfoCardShowIcon")!);
        check(!demo.Model.ShowIcon && card.HeaderIcon is null && card.HeaderIconTemplate is null && !Part<Border>(card, "PART_HeaderHost").IsVisible && !Part<Border>(card, "PART_HeaderGap").IsVisible, "InfoCard/slots/consumer: removing the last header region collapses the complete header row");
        ClickVisible(demo.FindControl<CheckBox>("InfoCardShowFooter")!);
        check(!demo.Model.ShowFooter && card.Footer is null && card.FooterTemplate is null && !Part<ContentPresenter>(card, "PART_FooterPresenter").IsVisible && !Part<Border>(card, "PART_FooterGap").IsVisible, "InfoCard/slots/consumer: the footer switch removes its template, height and gap");
        ClickVisible(demo.FindControl<CheckBox>("InfoCardShowIcon")!);
        ClickVisible(demo.FindControl<CheckBox>("InfoCardShowHeaderActions")!);
        ClickVisible(demo.FindControl<CheckBox>("InfoCardShowFooter")!);
        ClickVisible(Named<Button>("InfoCardTypedHeaderAction")); ClickVisible(Named<Button>("InfoCardFooterAction"));
        check(demo.Model.DetailsCount == 2 && demo.Model.SyncCount == 2 && Named<TextBlock>("InfoCardTypedFooterStatus").Text == demo.Model.FooterStatus && card.FooterTemplate is not null, "InfoCard/slots/consumer: restoring all templates rebinds native commands and current state");
        foreach (var theme in new[] { ThemeVariant.Dark, ThemeVariant.Light })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            window.RequestedThemeVariant = theme; ControlSizing.SetSize(window, size); card.BringIntoView(); Flush();
            var expected = size switch { ControlSize.Small => 16, ControlSize.Large => 20, _ => 18 };
            var currentIcon = Named<PathIcon>("InfoCardTypedIcon");
            check(currentIcon.Bounds.Width == expected && currentIcon.Bounds.Height == expected && Named<Button>("InfoCardTypedHeaderAction").IsEffectivelyEnabled && Named<Button>("InfoCardFooterAction").FontSize == card.FontSize && Colour(Part<ContentPresenter>(card, "PART_FooterPresenter").Foreground) == Resource(card, "One.Text"), $"InfoCard/slots/consumer/{theme.Key}/{size}: compiled parent icon sizing and native buttons follow live theme and density");
        }
        ClickVisible(demo.FindControl<CheckBox>("InfoCardEnabled")!);
        ClickVisible(Named<Button>("InfoCardTypedHeaderAction")); ClickVisible(Named<Button>("InfoCardFooterAction"));
        check(!demo.Model.Enabled && demo.Model.DetailsCount == 2 && demo.Model.SyncCount == 2 && !Named<Button>("InfoCardFooterAction").IsEffectivelyEnabled, "InfoCard/slots/consumer: disabled binding prevents both templated native commands");
        ClickVisible(demo.FindControl<Button>("InfoCardReset")!);
        check(demo.Model.ShowHeaderSeparator && demo.Model.ShowFooterSeparator && card.ShowHeaderSeparator && card.ShowFooterSeparator && demo.Model.ShowHeader && demo.Model.ShowIcon && demo.Model.ShowHeaderActions && demo.Model.ShowFooter && demo.Model.Enabled && demo.Model.Revision == 1 && demo.Model.DetailsCount == 0 && demo.Model.SyncCount == 0 && Named<TextBlock>("InfoCardTypedFooterStatus").Text == demo.Model.FooterStatus, "InfoCard/slots/consumer: reset restores all regions and their current typed state");
        window.Close();
    }

    public static void Render(string output)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var cards = new StackPanel { Spacing = 18 };
            cards.Children.Add(CreateSample());
            cards.Children.Add(new InfoCard
            {
                Header = "工作区设置", HeaderIcon = CreateIcon(), HeaderActions = new Button { Content = "详情" },
                Content = new StackPanel { Spacing = 12, Children = { new Label { Content = "工作区名称" }, new TextBox { Text = "Still workspace" } } },
                Footer = new Grid
                {
                    ColumnDefinitions = ColumnDefinitions.Parse("*,Auto"), ColumnSpacing = 12,
                    Children =
                    {
                        new TextBlock { Text = "最近更新 · 已保存", VerticalAlignment = VerticalAlignment.Center },
                        new Button { Content = "同步", Classes = { "primary" }, [Grid.ColumnProperty] = 1 }
                    }
                }
            });
            cards.Children.Add(new InfoCard { Content = "未设置标题时，正文保留内边距，自动移除标题间隔。", Footer = "Footer · 支持自定义控件与编译绑定模板" });
            cards.Children.Add(new InfoCard { Header = "仅显示页脚分割线", Content = "标题线可独立关闭，分区内边距保持不变。", Footer = "Footer", ShowHeaderSeparator = false });
            cards.Children.Add(new InfoCard { Header = "仅显示标题分割线", Content = "页脚线可独立关闭。", Footer = "Footer", ShowFooterSeparator = false });
            var window = Open(cards, theme, size, height: 1300);
            using var frame = window.CaptureRenderedFrame()!;
            var path = Path.Combine(output, $"infocard-{theme.Key}-{size}.png");
            frame.Save(path, PngBitmapEncoderOptions.Default); Console.WriteLine($"Rendered {path}");
            window.Close();
        }
    }

    private static T Part<T>(Control card, string name) where T : Control => card.GetVisualDescendants().OfType<T>().Single(value => value.Name == name && value.TemplatedParent == card);
    private static Color? Colour(IBrush? brush) => (brush as ISolidColorBrush)?.Color;
    private static Color? Resource(Control control, string key) => control.TryFindResource(key, control.ActualThemeVariant, out var value) ? Colour(value as IBrush) : null;
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
    private static void Click(Control control)
    {
        var host = TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), host)!.Value;
        host.MouseMove(point); host.MouseDown(point, MouseButton.Left); host.MouseUp(point, MouseButton.Left); Flush();
    }
    private static void ClickVisible(Control control) { control.BringIntoView(); Flush(); Click(control); }

    private sealed class SampleModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public string Title { get; private set; } = "编译绑定标题";
        public string Message { get; private set; } = "编译绑定正文";
        public void Update()
        {
            Title = "更新标题"; Message = "更新正文";
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Title)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Message)));
        }
    }

}
