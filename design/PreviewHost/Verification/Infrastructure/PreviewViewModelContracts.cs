using System.ComponentModel;
using Avalonia;
using Avalonia.Animation;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

/// <summary>Notification and state dependencies exercised without reflection or generated UI probes.</summary>
internal static class PreviewViewModelContracts
{
    public static void Verify(Action<bool, string> check)
    {
        void Change(ReactiveObject model, Action change, params string[] names)
        {
            var counts = new Dictionary<string, int>();
            void Changed(object? sender, PropertyChangedEventArgs args)
            {
                if (args.PropertyName is { } name) counts[name] = counts.GetValueOrDefault(name) + 1;
            }
            model.PropertyChanged += Changed;
            try
            {
                change();
                check(names.All(name => counts.GetValueOrDefault(name) == 1), $"PreviewHost/notifications/{model.GetType().Name}: {string.Join(", ", names)} notify once");
                counts.Clear(); change();
                check(names.All(name => counts.GetValueOrDefault(name) == 0), $"PreviewHost/notifications/{model.GetType().Name}: equal values suppress dependent notifications");
            }
            finally { model.PropertyChanged -= Changed; }
        }

        var basic = new BasicControlViewModel();
        Change(basic, () => basic.Number = 93m, nameof(basic.Number));
        Change(basic, () => basic.Date = null, nameof(basic.Date));
        check(ReferenceEquals(basic.Items, basic.Items) && ReferenceEquals(basic.TreeRoots, basic.TreeRoots), "PreviewHost/notifications: lazy field properties retain collection identity");
        var shell = new GalleryViewModel();
        Change(shell, () => shell.NavigationCollapsed = true, nameof(shell.NavigationCollapsed), nameof(shell.EffectiveNavigationCollapsed));
        var destination = shell.NavigationRoots.SelectMany(group => group.Entries).Last();
        Change(shell, () => shell.SelectedNavigation = destination, nameof(shell.SelectedNavigation), nameof(shell.CurrentPage), nameof(shell.NavigationTitle));
        shell.Search = "按钮";
        var result = shell.SearchResults.First(); shell.SelectedSearchResult = result;
        check(shell.CurrentPage?.Id == result.Id && shell.Search == "" && shell.SelectedSearchResult is null, "PreviewHost/notifications: search result navigation keeps the original clear-and-select sequence");

        var carousel = new CarouselDemoViewModel();
        Change(carousel, () => carousel.SelectedIndex = 999, nameof(carousel.SelectedIndex), nameof(carousel.Position), nameof(carousel.CanPrevious), nameof(carousel.CanNext));
        check(!carousel.CanNext && carousel.CanPrevious, "PreviewHost/notifications: distant carousel selection updates navigation availability");
        var card = new InfoCardDemoViewModel();
        Change(card, () => card.ShowHeader = false, nameof(card.ShowHeader), nameof(card.VisibleTitle));
        Change(card, () => card.ShowIcon = false, nameof(card.ShowIcon), nameof(card.VisibleHeaderIcon));
        Change(card, () => card.ShowHeaderActions = false, nameof(card.ShowHeaderActions), nameof(card.VisibleHeaderActions));
        Change(card, () => card.ShowFooter = false, nameof(card.ShowFooter), nameof(card.VisibleFooter));
        Change(card, () => card.ShowHeaderSeparator = false, nameof(card.ShowHeaderSeparator));
        Change(card, () => card.ShowFooterSeparator = false, nameof(card.ShowFooterSeparator));
        var revision = card.Revision; card.UpdateCommand.Execute(null);
        check(card.Revision == revision + 1 && card.Description.Contains(card.Revision.ToString()), "PreviewHost/notifications: command revision still updates the derived card description");

        var chart = new ChartDemoViewModel();
        Change(chart, () => chart.Kind = ChartKind.Gauge, nameof(chart.Kind), nameof(chart.KindIndex), nameof(chart.IsGauge), nameof(chart.IsCartesian), nameof(chart.ChartTitle), nameof(chart.Unit));
        Change(chart, () => chart.SamplingEnabled = false, nameof(chart.SamplingEnabled), nameof(chart.SamplingMode));
        Change(chart, () => chart.SharedTooltip = false, nameof(chart.SharedTooltip), nameof(chart.TooltipMode));
        Change(chart, () => chart.Viewport = new Rect(.1, .2, .5, .5), nameof(chart.Viewport), nameof(chart.NavigationStatus));
        Change(chart, () => chart.AnimationDurationIndex = 2, nameof(chart.AnimationDurationIndex), nameof(chart.UpdateAnimationDuration));
        chart.AnimationDurationIndex = 8;
        check(chart.AnimationDurationIndex == 2, "PreviewHost/notifications: guarded animation duration rejects out-of-range input");
        Change(chart, () => chart.ShowComparison = false, nameof(chart.ShowComparison));
        check(!chart.Series[1].IsVisible, "PreviewHost/notifications: comparison toggle writes the observed series");
        Change(chart, () => chart.Series[1].IsVisible = true, nameof(chart.ShowComparison));
        Change(chart, () => chart.Series[1].YAxis = ChartYAxis.Secondary, nameof(chart.UseSecondaryAxis));
        check(chart.UseSecondaryAxis, "PreviewHost/notifications: series axis changes write back without recursive notifications");
        chart.StopObservingSeries(); chart.Series[1].IsVisible = false;
        check(chart.ShowComparison, "PreviewHost/notifications: detached chart observations stop writing back");
        chart.StartObservingSeries();
        check(!chart.ShowComparison, "PreviewHost/notifications: reattached observations synchronize existing state");
        chart.StopObservingSeries();

        var navigation = new NavMenuDemoViewModel();
        Change(navigation, () => navigation.Search = "三级页面 1000", nameof(navigation.Search));
        Change(navigation, () => navigation.Selected = navigation.Roots[0].Entries[0].Entries[999], nameof(navigation.Selected));
        var node = new PreviewTreeNode("树", "回收状态");
        Change(node, () => node.IsExpanded = true, nameof(node.IsExpanded));
        var entry = new NavEntry("菜单");
        Change(entry, () => entry.Title = "新菜单", nameof(entry.Title));
        var tabs = new TabStripDemoViewModel();
        Change(tabs, () => tabs.SelectedIndex = 999, nameof(tabs.SelectedIndex), nameof(tabs.SelectedTitle));
        var transition = new ContentTransitionDemoViewModel();
        Change(transition, () => transition.Content = null, nameof(transition.Content), nameof(transition.Position));
        Change(transition, () => transition.TransitionIndex = 1, nameof(transition.TransitionIndex), nameof(transition.Transition));
        check(transition.Transition is PageSlide, "PreviewHost/notifications: transition replacement keeps its configured type");
        var form = (TransitionForm)transition.Pages[1];
        Change(form, () => form.Draft = "保留草稿", nameof(form.Draft));
        var records = (TransitionRecords)transition.Pages[2];
        Change(records, () => records.SelectedRow = records.Rows[999], nameof(records.SelectedRow));

        var refresh = new RefreshDemoViewModel();
        var refreshedRows = RefreshDemoViewModel.CreateRows(1);
        Change(refresh, () => refresh.Rows = refreshedRows, nameof(refresh.Rows));
        var flyout = new FlyoutDemoViewModel();
        Change(flyout, () => flyout.Name = "新工作区", nameof(flyout.Name));
        check(flyout.Apply() && flyout.AppliedName == "新工作区", "PreviewHost/notifications: flyout command state retains input validation and applied value");
        var notification = new NotificationDemoViewModel();
        Change(notification, () => notification.PositionIndex = 2, nameof(notification.PositionIndex), nameof(notification.Position));
        var message = new MessagePageViewModel();
        Change(message, () => message.ShowClose = false, nameof(message.ShowClose));
        var window = new WindowPageViewModel();
        Change(window, () => window.ShowTheme = false, nameof(window.ShowTheme));
        var split = new SplitButtonDemoViewModel(); ControlPreview.Flush(); var commandChanges = 0;
        split.MainActionCommand.CanExecuteChanged += (_, _) => commandChanges++;
        Change(split, () => split.AllowCommand = false, nameof(split.AllowCommand));
        ControlPreview.Flush();
        check(!split.MainActionCommand.CanExecute(null) && commandChanges == 1, "PreviewHost/notifications: split button availability changes exactly once");
        var tag = new TagDemoViewModel();
        Change(tag, () => tag.Text = "正在处理", nameof(tag.Text));
        var steps = new VerificationStepDemoViewModel();
        Change(steps.Account, () => steps.Account.Email = "user@example.com", nameof(steps.Account.Email));
        var legacy = new LegacyGalleryViewModel();
        Change(legacy, () => legacy.Email = "invalid", nameof(legacy.Email), nameof(legacy.EmailError), nameof(legacy.HasErrors));
        Change(legacy, () => legacy.Progress = 150, nameof(legacy.Progress));
        check(legacy.HasErrors && legacy.Progress == 100, "PreviewHost/notifications: legacy validation and progress coercion remain intact");
        var dialog = new DialogEditorViewModel(); var dialogChanges = 0;
        dialog.PropertyChanged += (_, args) => { if (args.PropertyName == nameof(dialog.Name)) dialogChanges++; };
        dialog.Name = "表单"; dialog.Name = "表单";
        check(dialogChanges == 1, "PreviewHost/notifications: DialogContext field setter retains its own notification contract");
    }
}
