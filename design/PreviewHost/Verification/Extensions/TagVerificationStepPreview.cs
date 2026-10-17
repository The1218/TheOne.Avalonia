using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
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

internal static partial class TagVerificationStepPreview
{
    private static void Flush()
    {
        Dispatcher.UIThread.RunJobs();
        AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        Dispatcher.UIThread.RunJobs();
    }

    private static T Part<T>(Control owner, string name) where T : Control
    {
        return owner.GetVisualDescendants().OfType<T>().Single(control => control.Name == name);
    }

    private static void Wait(Task task)
    {
        var watch = Stopwatch.StartNew();
        while (!task.IsCompleted && watch.ElapsedMilliseconds < 4000)
        {
            var frame = new DispatcherFrame();
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            timer.Tick += (_, _) =>
            {
                timer.Stop();
                frame.Continue = false;
            };
            timer.Start();
            Dispatcher.UIThread.PushFrame(frame);
            Flush();
        }

        if (!task.IsCompleted) throw new InvalidOperationException("VerificationStep operation did not finish.");
        task.GetAwaiter().GetResult();
        Flush();
    }

    private static bool Next(VerificationStep step)
    {
        var task = step.GoNextAsync();
        Wait(task);
        return task.Result;
    }

    private static void Click(Control control)
    {
        var top = TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), top)!.Value;
        top.MouseMove(point);
        top.MouseDown(point, MouseButton.Left);
        top.MouseUp(point, MouseButton.Left);
        Flush();
    }

    private static Window Open(Control content, ThemeVariant theme, ControlSize size, double width = 980,
        double height = 820)
    {
        var frame = new Border { Child = content, Padding = new Thickness(20) };
        var window = new Window { Content = frame, Width = width, Height = height, RequestedThemeVariant = theme };
        window.TryFindResource("One.Canvas", theme, out var canvas);
        frame.Background = (IBrush?)canvas;
        ControlSizing.SetSize(window, size);
        Motion.SetEnabled(window, false);
        window.Show();
        Flush();
        return window;
    }

    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
            VerifyTag(theme, size, check);
        VerifyStep(check);
    }

    public static void VerifyStep(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            VerifyFocus(theme, size, check);
            VerifyStep(theme, size, check);
        }

        VerifyAsync(check);
        VerifyCommands(check);
        VerifyVirtualization(check);
        VerifyErrorVirtualization(check);
        VerifyFieldMapping(check);
        VerifyRules(check);
        VerifyAutoForms(check);
    }

    private static void VerifyTag(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var view = new TagDemoView();
        var window = Open(view, theme, size);
        var sample = view.FindControl<Tag>("TagSample")!;
        var prefix = $"Tag/{theme.Key}/{size}";
        var expectedSize = size == ControlSize.Small ? 11 : size == ControlSize.Large ? 15 : 12;
        check(sample.FontSize == expectedSize && sample.IconSize == 13,
            $"{prefix}: compact typography and scaled caller icon inherit all three densities");
        foreach (var status in Enum.GetValues<TagStatus>())
        foreach (var variant in Enum.GetValues<TagVariant>())
        {
            view.Model.Status = status;
            view.Model.Variant = variant;
            view.Model.Text = "状态文字";
            Flush();
            var content = Part<ContentPresenter>(sample, "PART_ContentPresenter");
            var icon = Part<ContentPresenter>(sample, "PART_IconPresenter");
            check(
                sample.Status == status && sample.Variant == variant && Equals(content.Content, "状态文字") &&
                icon.Child is PathIcon && icon.IsVisible,
                $"{prefix}/{status}/{variant}: compiled status/content bindings retain the caller's icon");
            var text = content.GetVisualDescendants().OfType<TextBlock>().Single();
            check(
                Math.Abs(text.TranslatePoint(default, sample)!.Value.Y + text.TextLayout.Height / 2 -
                         sample.Bounds.Height / 2) < 1.1,
                $"{prefix}/{status}/{variant}: icon and text remain vertically centered at inherited density");
            check(
                sample.Foreground is ISolidColorBrush foreground && sample.Background is ISolidColorBrush background &&
                Contrast(foreground.Color, background.Color, window) >= 4.5,
                $"{prefix}/{status}/{variant}: semantic tag text meets contrast against its effective surface");
        }

        view.Model.Enabled = false;
        Flush();
        sample.TryFindResource("One.DisabledText", theme, out var disabled);
        check(
            sample.Foreground is ISolidColorBrush actual && disabled is ISolidColorBrush expected &&
            actual.Color == expected.Color, $"{prefix}: disabled state overrides all semantic variants");
        view.Model.Enabled = true;
        var iconControl = sample.Icon;
        sample.Icon = null;
        Flush();
        check(!Part<ContentPresenter>(sample, "PART_IconPresenter").IsVisible,
            $"{prefix}: text-only tag removes its icon slot");
        sample.Icon = iconControl;
        Flush();
        check(ReferenceEquals(Part<ContentPresenter>(sample, "PART_IconPresenter").Child, iconControl),
            $"{prefix}: replacing and restoring caller-owned icons preserves the native logical tree");
        view.Model.Text = "";
        Flush();
        var iconOnly = Part<ContentPresenter>(sample, "PART_IconPresenter");
        check(
            Math.Abs(iconOnly.TranslatePoint(new Point(iconOnly.Bounds.Width / 2, iconOnly.Bounds.Height / 2), sample)!
                .Value.X - sample.Bounds.Width / 2) < 1.1,
            $"{prefix}: icon-only tag removes trailing content spacing and stays centered");
        window.Close();
        Flush();
    }

    private static void VerifyStep(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var view = new VerificationStepDemoView();
        var window = Open(view, theme, size);
        var step = view.Sample;
        var model = view.Model;
        var prefix = $"VerificationStep/{theme.Key}/{size}";
        VerifyHeaderLayout(view, window, prefix, check);
        check(
            step.SelectedIndex == 0 && step.CurrentContent == model.Account &&
            step.GetVisualDescendants().OfType<TextBox>().Count() == 3,
            $"{prefix}: compiled templates create only current step inputs");
        check(
            !Next(step) && step.SelectedIndex == 0 && step.HasErrors &&
            step.ValidationErrors.Any(error => error.MemberName == nameof(StepAccountModel.Name)),
            $"{prefix}: required property attributes block forward navigation");
        var headers = Part<ListBox>(step, "PART_Steps");
        headers.SelectedIndex = 2;
        Flush();
        check(step.SelectedIndex == 0 && headers.SelectedIndex == 0,
            $"{prefix}: changing native header selection cannot skip validation");
        Click(Part<Button>(step, "PART_NextButton"));
        check(
            step.SelectedIndex == 0 && step.HasErrors && !Part<ListBox>(step, "PART_ValidationErrors").IsVisible &&
            step.GetVisualDescendants().OfType<StepField>().All(field => field.HasErrors),
            $"{prefix}: real pointer input maps typed errors to individual fields without a duplicate summary");
        foreach (var field in step.GetVisualDescendants().OfType<StepField>())
        {
            var input = field.GetVisualDescendants().OfType<TextBox>().Single();
            var errors = Part<ListBox>(field, "PART_FieldErrors");
            check(field.Errors.All(error => error.MemberName == field.MemberName) && errors.IsVisible &&
                  errors.Bounds.Height > 0 && errors.Bounds.Height < 30
                  && errors.TranslatePoint(default, field)!.Value.Y >=
                  input.TranslatePoint(default, field)!.Value.Y + input.Bounds.Height,
                $"{prefix}/{field.MemberName}: own validation messages occupy layout directly below the input");
        }

        var indicators = headers.GetRealizedContainers().Select(header => Part<Tag>(header, "PART_Indicator"))
            .ToArray();
        var expectedDiameter = size == ControlSize.Small ? 30 : size == ControlSize.Large ? 42 : 36;
        check(indicators.Length == 3 && indicators.All(indicator =>
                                         Math.Abs(indicator.Bounds.Width - expectedDiameter) < 1 &&
                                         Math.Abs(indicator.Bounds.Height - expectedDiameter) < 1)
                                     && indicators.Select(indicator => indicator.TranslatePoint(default, step)!.Value.Y)
                                         .Distinct().Count() == 1,
            $"{prefix}: three equally sized circles share a horizontal center line at the inherited density");
        var containers = headers.GetRealizedContainers().ToArray();
        check(!Part<Border>(containers[0], "PART_LeftConnector").IsVisible && !Part<Border>(containers[2],
                                                                               "PART_RightConnector").IsVisible
                                                                           && Part<Border>(containers[0],
                                                                               "PART_RightConnector").Bounds.Width >
                                                                           0 && Part<Border>(containers[1],
                                                                               "PART_LeftConnector").Bounds.Width > 0,
            $"{prefix}: connectors join adjacent circles and stop at the first and last step");
        var name = Part<TextBox>(step, "Name");
        name.Focus();
        name.SelectAll();
        window.KeyTextInput("张三");
        Flush();
        Part<TextBox>(step, "Email").Text = "wrong-email";
        Part<TextBox>(step, "ConfirmEmail").Text = "other@example.com";
        Flush();
        check(
            model.Account.Name == "张三" && !Next(step) &&
            step.ValidationErrors.Any(error => error.MemberName == nameof(StepAccountModel.Email)) &&
            step.ValidationErrors.Any(error => error.MemberName == nameof(StepAccountModel.ConfirmEmail)),
            $"{prefix}: native keyboard binding plus EmailAddress and Compare validation use current model values");
        model.Account.Email = model.Account.ConfirmEmail = "one@example.com";
        Flush();
        check(
            Next(step) && step.SelectedIndex == 1 && model.Steps[0].State == StepState.Success &&
            Part<TextBox>(step, "Age").Text == model.Profile.Age?.ToString(),
            $"{prefix}: successful validation opens the next typed content and preserves completed state");
        check(
            Part<Border>(containers[0], "PART_RightProgress").Opacity == 1 &&
            Part<Border>(containers[1], "PART_LeftProgress").Opacity == 1 &&
            Part<Border>(containers[1], "PART_RightProgress").Opacity == 0,
            $"{prefix}: completed connector fills through the current circle and future connector stays muted");
        check(!Next(step) && step.ValidationErrors.Any(error => error.MemberName == nameof(StepProfileModel.Age)),
            $"{prefix}: Range blocks the second step independently");
        model.Profile.Age = 28;
        model.Profile.Address = "北京市朝阳区示例路";
        Flush();
        var ageInput = Part<TextBox>(step, "Age");
        ageInput.Text = "invalid-number";
        Flush();
        check(model.Profile.Age == 28 && !Next(step) && step.SelectedIndex == 1 && step.HasErrors,
            $"{prefix}: native conversion errors block advancement even when the model retains its last valid value");
        check(
            step.GetVisualDescendants().OfType<StepField>().Single(field => field.MemberName == "Age").HasErrors &&
            DataValidationErrors.GetHasErrors(ageInput),
            $"{prefix}: conversion error stays owned by native binding and also appears below its field");
        ageInput.Text = "28";
        Flush();
        check(Next(step) && step.SelectedIndex == 2 && !Next(step) && !step.IsCompleted,
            $"{prefix}: final step also enforces standard AllowedValues validation");
        model.Confirmation.Accepted = true;
        Flush();
        check(Next(step) && step.IsCompleted && model.CompletionCount == 1 && Next(step) && model.CompletionCount == 1,
            $"{prefix}: compiled VM CompletedCommand executes once after final success");
        check(step.GoBack() && step.SelectedIndex == 1 && !step.IsCompleted && model.Account.Name == "张三",
            $"{prefix}: back navigation preserves model edits and reopens completed workflows");
        model.Account.Email = "invalidated";
        Flush();
        check(
            step.SelectedIndex == 0 && model.Steps.Skip(1).All(item => item.State == StepState.Waiting) && !Next(step),
            $"{prefix}: editing an earlier model invalidates all dependent steps and revisits the required step");
        view.Model.Reset();
        Flush();
        check(
            step.SelectedIndex == 0 && step.CurrentContent == view.Model.Account && !step.IsCompleted &&
            !step.HasErrors,
            $"{prefix}: replacing ItemsSource resets workflow and templates without retaining old errors");
        window.Close();
        Flush();
    }

    private static void VerifyHeaderLayout(VerificationStepDemoView view, Window window, string prefix,
        Action<bool, string> check)
    {
        var step = view.Sample;
        var headers = Part<ListBox>(step, "PART_Steps");

        double Center(Control control)
        {
            return control.TranslatePoint(new Point(control.Bounds.Width / 2, 0), step)!.Value.X;
        }

        var firstIndicator = Part<Tag>(headers.ContainerFromIndex(0)!, "PART_Indicator");
        var lastIndicator = Part<Tag>(headers.ContainerFromIndex(2)!, "PART_Indicator");
        check(step.HeaderHorizontalAlignment == HorizontalAlignment.Center
              && Math.Abs((Center(firstIndicator) + Center(lastIndicator)) / 2 -
                          Center(Part<ListBox>(step, "PART_FormFields"))) < 1.1,
            $"{prefix}/header: actual indicator group and form share a horizontal center by default");
        foreach (var alignment in new[]
                 {
                     HorizontalAlignment.Center, HorizontalAlignment.Stretch, HorizontalAlignment.Left,
                     HorizontalAlignment.Right
                 })
        {
            view.Model.HeaderAlignment = alignment;
            Flush();
            var root = headers.GetVisualParent()!;
            var left = headers.TranslatePoint(default, root)!.Value.X;
            var right = root.Bounds.Width - left - headers.Bounds.Width;
            var expected = alignment == HorizontalAlignment.Stretch ? headers.Bounds.Width / 3 : 200;
            check(
                step.HeaderHorizontalAlignment == alignment && headers.GetRealizedContainers()
                    .All(item => Math.Abs(item.Bounds.Width - expected) < 1.1),
                $"{prefix}/header/{alignment}: compiled alignment binding applies equal item widths");
            check(alignment switch
                {
                    HorizontalAlignment.Left => Math.Abs(left) < 1.1,
                    HorizontalAlignment.Right => Math.Abs(right) < 1.1,
                    _ => Math.Abs(left - right) < 1.1
                }, $"{prefix}/header/{alignment}: measured viewport follows the requested alignment");
            if (alignment == HorizontalAlignment.Stretch)
            {
                var scrollView = Part<ScrollViewer>(headers, "PART_ScrollViewer");
                check(scrollView.Extent.Width <= scrollView.Viewport.Width + .1,
                    $"{prefix}/header: fitted items introduce no fractional-pixel horizontal overflow");
            }

            var first = headers.ContainerFromIndex(0)!;
            var second = headers.ContainerFromIndex(1)!;
            var outgoing = Part<Border>(first, "PART_RightConnector");
            var incoming = Part<Border>(second, "PART_LeftConnector");
            check(
                Math.Abs(outgoing.TranslatePoint(new Point(outgoing.Bounds.Width, 0), step)!.Value.X -
                         incoming.TranslatePoint(default, step)!.Value.X) < 1.1,
                $"{prefix}/header/{alignment}: adjacent connector segments stay joined");
        }

        view.Model.HeaderAlignment = HorizontalAlignment.Stretch;
        window.Width = 620;
        Flush();
        check(headers.GetRealizedContainers().All(item => Math.Abs(item.Bounds.Width - headers.Bounds.Width / 3) < 1.1),
            $"{prefix}/header: stretch recalculates equal widths after resizing");
        window.Width = 360;
        Flush();
        var scroll = Part<ScrollViewer>(headers, "PART_ScrollViewer");
        check(
            headers.GetRealizedContainers().All(item => Math.Abs(item.Bounds.Width - 120) < 1.1) &&
            scroll.Extent.Width > scroll.Viewport.Width,
            $"{prefix}/header: narrow windows preserve minimum widths and native horizontal scrolling");
        window.Width = 1600;
        Flush();
        check(
            headers.Bounds.Width <= step.HeaderMaxWidth &&
            Math.Abs(Center(headers) - Center(Part<ListBox>(step, "PART_FormFields"))) < 1.1,
            $"{prefix}/header: maximum viewport remains centered on wide windows");
        window.Width = 980;
        view.Model.HeaderAlignment = HorizontalAlignment.Center;
        Flush();
        check(step.SelectedIndex == 0 && !step.HasErrors && !step.IsCompleted,
            $"{prefix}/header: layout changes preserve validation and navigation state");
    }

    private static void VerifyAsync(Action<bool, string> check)
    {
        var deferred = new DeferredValidator();
        var model = new StepNoteModel();
        var items = new[]
        {
            new StepItem { Title = "Async", Content = model, Validator = deferred }, new StepItem { Title = "Next" }
        };
        var step = new VerificationStep { ItemsSource = items };
        var window = Open(step, ThemeVariant.Light, ControlSize.Normal);
        var pending = step.GoNextAsync();
        Flush();
        var repeated = step.GoNextAsync();
        Wait(repeated);
        check(
            step.IsBusy && !repeated.Result && deferred.Calls == 1 &&
            !Part<Button>(step, "PART_NextButton").IsEffectivelyEnabled,
            "VerificationStep/async: repeated clicks share no second validation or forward transition");
        model.Note = "changed while waiting";
        Wait(pending);
        deferred.Completion.SetResult([]);
        Flush();
        check(!pending.Result && step.SelectedIndex == 0 && !step.IsBusy && !step.IsCompleted,
            "VerificationStep/async: model edits invalidate a late successful validation even when validator ignores cancellation");
        deferred = new DeferredValidator();
        items[0].Validator = deferred;
        Flush();
        pending = step.GoNextAsync();
        window.Close();
        Flush();
        Wait(pending);
        deferred.Completion.SetResult([]);
        Flush();
        check(!pending.Result && step.SelectedIndex == 0 && !step.IsBusy,
            "VerificationStep/async: detached controls cannot advance from delayed validation callbacks");
        var failing = new FailingValidator();
        step = new VerificationStep { ItemsSource = new[] { new StepItem { Validator = failing } } };
        window = Open(step, ThemeVariant.Light, ControlSize.Normal);
        check(!Next(step) && step.HasErrors && !step.IsCompleted && !step.IsBusy,
            "VerificationStep/async: service validation exceptions retain current step and allow retry");
        window.Close();
        Flush();
    }

    private static void VerifyCommands(Action<bool, string> check)
    {
        var command = new CompletionCommand();
        var parameter = new object();
        var step = new VerificationStep
        {
            ItemsSource = new[] { new StepItem { Title = "Finish" } }, CompletedCommand = command,
            CompletedCommandParameter = parameter
        };
        var window = Open(step, ThemeVariant.Light, ControlSize.Normal);
        check(command.SubscriberCount == 1, "VerificationStep/command: attachment observes command availability once");
        check(!step.CanGoNext && !Next(step) && !step.IsCompleted && command.Calls == 0,
            "VerificationStep/command: CanExecute=false blocks final completion");
        command.Enabled = true;
        command.Refresh();
        Flush();
        Click(Part<Button>(step, "PART_NextButton"));
        check(
            step.IsCompleted && command.Calls == 1 && ReferenceEquals(command.Parameter, parameter) && Next(step) &&
            command.Calls == 1,
            "VerificationStep/command: real completion click passes the command parameter exactly once");
        window.Close();
        Flush();
        check(command.SubscriberCount == 0,
            "VerificationStep/command: detached steps release the command availability subscription");
        var deferred = new DeferredValidator();
        command = new CompletionCommand { Enabled = true };
        step = new VerificationStep
            { ItemsSource = new[] { new StepItem { Validator = deferred } }, CompletedCommand = command };
        window = Open(step, ThemeVariant.Light, ControlSize.Normal);
        var pending = step.GoNextAsync();
        Flush();
        check(!Next(step) && command.Calls == 0,
            "VerificationStep/command: repeated final clicks cannot execute the command during validation");
        command.Enabled = false;
        command.Refresh();
        deferred.Completion.SetResult([]);
        Wait(pending);
        check(!pending.Result && !step.IsCompleted && command.Calls == 0,
            "VerificationStep/command: CanExecute is rechecked after asynchronous validation");
        deferred = new DeferredValidator();
        step.ItemsSource = new[] { new StepItem { Validator = deferred } };
        command.Enabled = true;
        command.Refresh();
        pending = step.GoNextAsync();
        window.Close();
        Wait(pending);
        deferred.Completion.SetResult([]);
        Flush();
        check(!pending.Result && command.Calls == 0 && !step.IsCompleted,
            "VerificationStep/command: detached final validation cannot execute a late completion command");
    }

    private static void VerifyVirtualization(Action<bool, string> check)
    {
        var view = new VerificationStepDemoView();
        view.Model.LoadThousand();
        view.Model.HeaderAlignment = HorizontalAlignment.Stretch;
        var window = Open(view, ThemeVariant.Dark, ControlSize.Normal);
        var step = view.Sample;
        var headers = Part<ListBox>(step, "PART_Steps");
        check(
            headers.GetRealizedContainers().Count() is > 0 and < 20 &&
            step.GetVisualDescendants().OfType<TextBox>().Count() == 1,
            "VerificationStep/1000: bounded horizontal headers realize fewer than 20 containers and one content view");
        var jump = step.GoToAsync(999);
        Wait(jump);
        check(!jump.Result && step.SelectedIndex == 0,
            "VerificationStep/1000: far navigation cannot bypass unvalidated intermediate steps");
        for (var index = 0; index < 999; index++)
            if (!Next(step))
                throw new InvalidOperationException($"VerificationStep {index} should validate.");
        Flush();
        check(
            step.SelectedIndex == 999 && headers.ContainerFromIndex(999) is not null &&
            headers.ContainerFromIndex(0) is null && headers.GetRealizedContainers().Count() < 20,
            "VerificationStep/1000: sequential validation reaches far item while actual header containers recycle");
        view.Model.HeaderAlignment = HorizontalAlignment.Center;
        Flush();
        check(
            step.SelectedIndex == 999 && headers.ContainerFromIndex(999) is not null &&
            headers.GetRealizedContainers().Count() < 20,
            "VerificationStep/1000: switching to centered layout retains the far selected step and bounded realization");
        view.Model.HeaderAlignment = HorizontalAlignment.Stretch;
        window.Width = 660;
        Flush();
        check(
            step.SelectedIndex == 999 && headers.ContainerFromIndex(999) is not null &&
            headers.GetRealizedContainers().Count() < 20,
            "VerificationStep/1000: resizing a stretched header retains the far selected step and bounded realization");
        check(
            view.Model.Steps[0].State == StepState.Success && view.Model.Steps[998].State == StepState.Success &&
            view.Model.Steps[999].State == StepState.Current,
            "VerificationStep/1000: success and current states belong to independent models after recycling");
        headers.ContainerFromIndex(999)!.Focus();
        window.KeyPress(Key.Left, RawInputModifiers.None, PhysicalKey.ArrowLeft, null);
        window.KeyRelease(Key.Left, RawInputModifiers.None, PhysicalKey.ArrowLeft, null);
        Flush();
        check(step.SelectedIndex == 998 && headers.GetRealizedContainers().Count() < 20,
            "VerificationStep/1000: real keyboard navigation works on remotely realized headers");
        view.Model.Steps.RemoveAt(0);
        Flush();
        check(step.SelectedIndex == 0 && headers.ItemCount == 999 && headers.GetRealizedContainers().Count() < 20,
            "VerificationStep/1000: observable collection edits reset selection and preserve bounded realization");
        window.Close();
        Flush();
        var unbounded = new VerificationStep
        {
            ItemsSource = Enumerable.Range(1, 1000)
                .Select(index => new StepItem { Title = $"VerificationStep {index}" }).ToArray()
        };
        window = Open(new StackPanel { Orientation = Orientation.Horizontal, Children = { unbounded } },
            ThemeVariant.Light, ControlSize.Normal);
        var finite = Part<ListBox>(unbounded, "PART_Steps");
        check(finite.Bounds.Width <= unbounded.HeaderMaxWidth && finite.GetRealizedContainers().Count() is > 0 and < 20,
            "VerificationStep/1000: default header viewport stays finite even inside a horizontally unbounded parent");
        window.Close();
        Flush();
    }

    private static void VerifyErrorVirtualization(Action<bool, string> check)
    {
        var step = new VerificationStep
            { ItemsSource = new[] { new StepItem { Title = "Many errors", Validator = new ManyErrorsValidator() } } };
        var window = Open(step, ThemeVariant.Light, ControlSize.Normal);
        check(!Next(step) && step.ValidationErrors.Count == 1000,
            "VerificationStep/errors: all 1,000 validation results remain in the model");
        var errors = Part<ListBox>(step, "PART_ValidationErrors");
        check(errors.GetRealizedContainers().Count() is > 0 and < 20 && errors.Bounds.Height <= 120,
            "VerificationStep/errors: bounded error viewport realizes fewer than 20 native containers");
        errors.ScrollIntoView(999);
        Flush();
        check(
            errors.ContainerFromIndex(999) is not null && errors.ContainerFromIndex(0) is null &&
            errors.GetRealizedContainers().Count() < 20,
            "VerificationStep/errors: far error navigation recycles actual containers");
        window.Close();
        Flush();
    }

    private static void VerifyFieldMapping(Action<bool, string> check)
    {
        var alpha = new TextBox { Name = "UnrelatedA" };
        var beta = new TextBox { Name = "UnrelatedB" };
        var first = new StepField { MemberName = "Alpha", Content = alpha };
        var second = new StepField { MemberName = "Beta", Content = beta };
        var step = new VerificationStep
            { ItemsSource = new[] { new StepItem { Content = new StackPanel { Children = { first, second } } } } };
        var window = Open(step, ThemeVariant.Dark, ControlSize.Normal);
        DataValidationErrors.SetErrors(alpha, new object[] { "Same conversion message" });
        DataValidationErrors.SetErrors(beta, new object[] { "Same conversion message" });
        check(!Next(step) && first.Errors.Count == 1 && second.Errors.Count == 1 && !step.HasUnmappedErrors,
            "VerificationStep/fields: identical native messages map independently by explicit member, regardless of input Name");
        check(DataValidationErrors.GetHasErrors(alpha) && DataValidationErrors.GetHasErrors(beta),
            "VerificationStep/fields: step validation preserves native binding errors");
        DataValidationErrors.SetErrors(alpha, null);
        DataValidationErrors.SetErrors(beta, null);
        check(Next(step) && !first.HasErrors && !second.HasErrors,
            "VerificationStep/fields: corrected inputs clear their inline errors and permit retry");
        window.Close();
        Flush();
        var field = new StepField { MemberName = "Alpha", Content = new TextBox() };
        step = new VerificationStep
            { ItemsSource = new[] { new StepItem { Content = field, Validator = new ManyErrorsValidator("Alpha") } } };
        window = Open(step, ThemeVariant.Light, ControlSize.Normal);
        check(!Next(step) && field.Errors.Count == 1000 && !step.HasUnmappedErrors,
            "VerificationStep/fields: all 1,000 member errors are routed below the matching input");
        Flush();
        var list = Part<ListBox>(field, "PART_FieldErrors");
        check(list.Bounds.Height <= 96 && list.GetRealizedContainers().Count() is > 0 and < 20,
            "VerificationStep/fields: inline error viewport also bounds actual native container realization");
        list.ScrollIntoView(999);
        Flush();
        check(
            list.ContainerFromIndex(999) is not null && list.ContainerFromIndex(0) is null &&
            list.GetRealizedContainers().Count() < 20,
            "VerificationStep/fields: remote inline messages recycle containers");
        window.Close();
        Flush();
    }

    private static void VerifyRules(Action<bool, string> check)
    {
        var model = new StepAccountModel { Name = "有效姓名" };
        var scope = StepValidation.For(model, nameof(StepAccountModel.Name)).ValidateAsync(default);
        Wait(scope);
        check(scope.Result.Count == 0,
            "VerificationStep/rules: explicit member scope validates only attributes belonging to that step");
        var cross = StepValidation.For(new CrossFieldModel()).ValidateAsync(default);
        Wait(cross);
        check(cross.Result.Single().MemberName == nameof(CrossFieldModel.Value),
            "VerificationStep/rules: IValidatableObject supplies cross-field errors without dynamic bindings");
        var bound = new StepAccountModel { Name = "Bound model" };
        var content = new TextBox { DataContext = bound };
        var step = new VerificationStep
        {
            ItemsSource = new[]
            {
                new StepItem
                    { Content = content, Validator = StepValidation.For(bound, nameof(StepAccountModel.Name)) },
                new StepItem { Content = bound, Validator = StepValidation.For(bound, nameof(StepAccountModel.Email)) }
            }
        };
        var window = Open(step, ThemeVariant.Light, ControlSize.Normal);
        check(Next(step) && step.SelectedIndex == 1,
            "VerificationStep/rules: caller-owned control content uses its explicitly registered bound model");
        bound.Email = "new@example.com";
        Flush();
        check(step.SelectedIndex == 1,
            "VerificationStep/rules: unrelated shared-model members do not invalidate earlier scoped steps");
        bound.Name = "";
        Flush();
        check(step.SelectedIndex == 0 && !Next(step),
            "VerificationStep/rules: editing a registered model behind control content invalidates dependent steps");
        window.Close();
        Flush();
    }

    private static double Contrast(Color foreground, Color background, Window window)
    {
        if (background.A == 0)
        {
            window.TryFindResource("One.Surface", window.ActualThemeVariant, out var surface);
            background = ((ISolidColorBrush)surface!).Color;
        }

        static double Luminance(Color color)
        {
            static double Channel(byte b)
            {
                var value = b / 255d;
                return value <= .04045 ? value / 12.92 : Math.Pow((value + .055) / 1.055, 2.4);
            }

            return .2126 * Channel(color.R) + .7152 * Channel(color.G) + .0722 * Channel(color.B);
        }

        var a = Luminance(foreground);
        var b = Luminance(background);
        return (Math.Max(a, b) + .05) / (Math.Min(a, b) + .05);
    }

    public static void Render(string output, bool tags, bool steps)
    {
        Directory.CreateDirectory(output);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            if (tags) Save(new TagDemoView(), "tag", 980, 760);
            if (steps)
            {
                var view = new VerificationStepDemoView();
                Save(view, "verification-step", 980, 820);
                var window = Open(view, theme, size);
                Next(view.Sample);
                SaveFrame(view, "verification-step-errors");
                view.Model.Account.Name = "张三";
                view.Model.Account.Email = view.Model.Account.ConfirmEmail = "one@example.com";
                Flush();
                Next(view.Sample);
                SaveFrame(view, "verification-step-progress");
                view.Model.HeaderAlignment = HorizontalAlignment.Stretch;
                Flush();
                SaveFrame(view, "verification-step-stretch");
                window.Close();
                Flush();
            }

            void Save(Control view, string stem, int width, int height)
            {
                var window = Open(view, theme, size, width, height);
                SaveFrame(view, stem);
                if (window.Content is Border frame) frame.Child = null;
                window.Content = null;
                window.Close();
                Flush();
            }

            void SaveFrame(Control view, string stem)
            {
                var target = view.GetVisualParent() as Border ?? view;
                using var bitmap =
                    new RenderTargetBitmap(new PixelSize((int)target.Bounds.Width, (int)target.Bounds.Height),
                        new Vector(96, 96));
                bitmap.Render(target);
                var path = Path.Combine(output, $"{stem}-{theme.Key}-{size}.png");
                bitmap.Save(path, PngBitmapEncoderOptions.Default);
                Console.WriteLine($"Rendered {path}");
            }
        }
    }

    private sealed class CompletionCommand : ICommand
    {
        public int Calls;
        public bool Enabled;
        public object? Parameter;
        public int SubscriberCount => CanExecuteChanged?.GetInvocationList().Length ?? 0;
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return Enabled;
        }

        public void Execute(object? parameter)
        {
            Calls++;
            Parameter = parameter;
        }

        public void Refresh()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private sealed class ManyErrorsValidator(string? memberName = null) : IStepValidator
    {
        public Task<IReadOnlyList<StepValidationError>> ValidateAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<StepValidationError>>(Enumerable.Range(1, 1000)
                .Select(index => new StepValidationError(memberName ?? $"Field{index}", $"Validation error {index}"))
                .ToArray());
        }
    }

    public sealed class CrossFieldModel : IValidatableObject
    {
        public string Value { get; set; } = "";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield return new ValidationResult("Cross-field failure", [nameof(Value)]);
        }
    }

    private sealed class DeferredValidator : IStepValidator
    {
        public int Calls;

        public TaskCompletionSource<IReadOnlyList<StepValidationError>> Completion { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task<IReadOnlyList<StepValidationError>> ValidateAsync(CancellationToken cancellationToken)
        {
            Calls++;
            return Completion.Task;
        }
    }

    private sealed class FailingValidator : IStepValidator
    {
        public Task<IReadOnlyList<StepValidationError>> ValidateAsync(CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Validation service unavailable");
        }
    }
}