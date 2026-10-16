using System.Runtime.InteropServices;
using System.Windows.Input;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class FormPreview
{
    public static void Verify(Action<bool, string> check, string? output = null)
    {
        VerifyFields(check); VerifyAsync(check); VerifyOwnership(check);
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        foreach (var width in new[] { 760, 320 }) VerifyAppearance(theme, size, width, check, output);
    }
    private static Window Open(Control content, ThemeVariant? theme = null, ControlSize size = ControlSize.Normal, int width = 760)
    {
        var window = new Window { Width = width, Height = 900, WindowDecorations = WindowDecorations.None, RequestedThemeVariant = theme ?? ThemeVariant.Light,
            Content = new Border { Padding = new Thickness(20), Child = content } };
        ((Border)window.Content).Bind(Border.BackgroundProperty, new global::Avalonia.Markup.Xaml.MarkupExtensions.DynamicResourceExtension("One.Surface"));
        ControlSizing.SetSize(window, size); Motion.SetEnabled(window, false); window.Show(); Flush(); return window;
    }
    private static T Part<T>(Control root, string name) where T : Control => root.GetVisualDescendants().OfType<T>().Single(control => control.Name == name);
    private static void Valid(FormAccountModel model) { model.Name = "Ada"; model.Email = model.ConfirmEmail = "ada@example.com"; model.Age = 28; Flush(); }
    private static void VerifyFields(Action<bool, string> check)
    {
        var view = new FormDemoView(); var form = view.Sample; var window = Open(view);
        try
        {
            check(!Wait(form.SubmitAsync()) && view.Model.SavedCount == 0 && form.HasErrors && !form.IsValidated, "Form/validation: attributes block submission before the host command");
            var name = Part<TextBox>(view, "NameInput"); var email = Part<TextBox>(view, "EmailInput"); var age = Part<TextBox>(view, "AgeInput");
            check(Part<FormItem>(view, "NameField").HasErrors && Part<FormItem>(view, "EmailField").HasErrors && name.IsFocused, "Form/validation: errors map to member fields and focus the first native input");
            check(!form.HasUnmappedErrors && form.UnmappedErrors.Count == 0 && !Part<Border>(form, "PART_ErrorSummary").IsEffectivelyVisible,
                "Form/summary: mapped field errors appear once below their inputs and leave no summary space");
            check(AutomationProperties.GetLabeledBy(name) is Label { Content: "姓名" }, "Form/accessibility: labels are associated with caller-owned native inputs");
            var nameField = Part<FormItem>(view, "NameField"); nameField.Label = null; Flush();
            check(AutomationProperties.GetLabeledBy(name) is null, "Form/accessibility: removing a label removes its owned input association");
            var explicitLabel = new Label { Content = "Explicit label" }; AutomationProperties.SetLabeledBy(name, explicitLabel); nameField.Label = "姓名"; Flush();
            check(ReferenceEquals(AutomationProperties.GetLabeledBy(name), explicitLabel), "Form/accessibility: explicit caller label associations survive field changes");
            name.ClearValue(AutomationProperties.LabeledByProperty); nameField.Label = "姓名 "; nameField.Label = "姓名"; Flush();
            name.Focus(); Press(window, Key.Tab); check(email.IsFocused, "Form/keyboard: Tab moves through fields without stopping in error messages");
            Press(window, Key.Tab, true); check(name.IsFocused, "Form/keyboard: Shift+Tab returns to the preceding input");
            name.SelectAll(); window.KeyTextInput("Ada"); Flush();
            check(view.Model.Account.Name == "Ada" && !form.HasErrors, "Form/binding: real text input writes the model through compiled binding and invalidates old messages");
            Valid(view.Model.Account); age.Text = "invalid draft"; Flush();
            check(view.Model.Account.Age == 28 && !Wait(form.ValidateAsync()) && form.ValidationErrors.Any(error => error.MemberName == "Age") && age.IsFocused,
                "Form/conversion: native conversion errors retain the last model value and block validation");
            form.ClearValidation(); check(age.Text == "invalid draft" && !form.HasErrors && !Wait(form.SubmitAsync()), "Form/conversion: clearing messages retains invalid drafts and cannot bypass conversion validation");
            age.Text = "31"; Flush(); Click(Part<Button>(view, "SaveButton"));
            check(view.Model.Account.Age == 31 && view.Model.SavedCount == 1 && form.IsValidated, "Form/submit: pointer activation validates before executing the host command once");
            form.Validator = StepValidation.For(view.Model.Account, nameof(FormAccountModel.ConfirmEmail)); Flush();
            check(Wait(form.ValidateAsync()), "Form/scope: existing member-scoped validators can be reused unchanged");
            view.Model.Account.Email = "changed@example.com"; Flush();
            check(!form.IsValidated && !Wait(form.ValidateAsync()) && form.ValidationErrors.Single().MemberName == "ConfirmEmail", "Form/scope: StepCompare dependencies invalidate validation when another field changes");
            form.ShowErrorSummary = false; Flush();
            check(!Part<Border>(form, "PART_ErrorSummary").IsEffectivelyVisible && form.HasErrors, "Form/summary: hiding the summary does not disable validation");
            var template = form.Template; form.Template = new FuncControlTemplate<Form>((_, _) => new Border()); Flush(); form.Template = template; Flush();
            check(Part<FormItem>(view, "NameField").Label == "姓名" && !Wait(form.ValidateAsync()), "Form/template: replacing and restoring the template preserves caller-owned fields and validation");
        }
        finally { window.Close(); Flush(); }
    }
    private static void VerifyAsync(Action<bool, string> check)
    {
        var model = new FormAccountModel(); var command = new TestCommand(); var validator = new DeferredValidator();
        var form = new Form { ValidationModel = model, Validator = validator, SubmitCommand = command, Content = new TextBox() };
        var host = new ContentControl { Content = form }; var window = Open(host);
        try
        {
            var task = form.SubmitAsync(); Flush();
            check(form.IsBusy && !form.CanSubmit && !((Control)form.Content!).IsEffectivelyEnabled && !Wait(form.SubmitAsync()), "Form/async: pending validation disables content and rejects duplicate submission");
            model.Name = "Changed"; Flush(); validator.Completion.SetResult([new("Name", "obsolete")]);
            check(!Wait(task) && !form.IsBusy && !form.HasErrors && command.Count == 0, "Form/async: model changes cancel validation even when the service ignores cancellation");
            validator = new DeferredValidator(); form.Validator = validator; task = form.SubmitAsync(); command.Allowed = false; validator.Completion.SetResult([]);
            check(!Wait(task) && command.Count == 0 && !form.CanSubmit, "Form/async: CanExecute is checked again after validation");
            command.Allowed = true; Flush(); check(form.CanSubmit, "Form/command: CanExecuteChanged updates the public submission state");
            validator = new DeferredValidator(); form.Validator = validator; using var cancellation = new CancellationTokenSource(); task = form.ValidateAsync(cancellation.Token); cancellation.Cancel();
            check(!Wait(task) && !form.IsBusy && !form.IsValidated, "Form/async: caller cancellation clears busy state without reporting an error");
            validator = new DeferredValidator(); form.Validator = validator; task = form.ValidateAsync(); host.Content = null; Flush(); validator.Completion.SetResult([new("", "obsolete")]);
            check(!Wait(task) && !form.HasErrors && !form.CanSubmit, "Form/lifecycle: detaching cancels validation and removes command subscriptions");
            host.Content = form; form.Validator = new ResultValidator([new("", "Service unavailable"), new("Missing", "Hidden field failed")]); Flush();
            check(!Wait(form.ValidateAsync()) && form.ValidationErrors.Count == 2 && form.UnmappedErrors.Count == 2 && Part<Border>(form, "PART_ErrorSummary").IsEffectivelyVisible,
                "Form/summary: object-level and unmapped errors stay visible after remounting");
            form.Validator = new ResultValidator([], true);
            check(!Wait(form.SubmitAsync()) && form.ValidationErrors.Single().MemberName == "" && form.ValidationErrors[0].Message == "service failed", "Form/async: validator failures become summary errors and never execute the command");
            validator = new DeferredValidator(); form.Validator = validator; task = form.ValidateAsync(); form.Content = new TextBox(); Flush(); validator.Completion.SetResult([]);
            check(!Wait(task) && !form.IsValidated, "Form/lifecycle: replacing content rejects results for old fields");
            form.Validator = new ResultValidator([]); form.SubmitCommandParameter = model;
            check(Wait(form.SubmitAsync()) && command.Count == 1 && ReferenceEquals(command.Parameter, model), "Form/submit: remounted forms use the current command parameter");
            host.IsEnabled = false; Flush(); check(!form.CanSubmit && !Wait(form.SubmitAsync()), "Form/disabled: ancestor disabled state updates public submission availability");
            host.IsEnabled = true; Flush(); check(form.CanSubmit, "Form/disabled: enabling the ancestor restores submission availability");
            form.IsEnabled = false; check(!Wait(form.SubmitAsync()), "Form/disabled: API submission respects disabled state");
        }
        finally { window.Close(); Flush(); }
    }
    private static void VerifyOwnership(Action<bool, string> check)
    {
        var inner = new Form { Validator = new ResultValidator([new("Name", "inner error")]), Content = new FormItem { MemberName = "Name", Label = "Inner", Content = new TextBox() } };
        var outerField = new FormItem { MemberName = "Name", Label = "Outer", Content = new TextBox() };
        var outer = new Form { Content = new StackPanel { Children = { outerField, inner } } }; var window = Open(outer);
        try
        {
            check(!Wait(inner.ValidateAsync()) && Wait(outer.ValidateAsync()) && !outerField.HasErrors && ((FormItem)inner.Content!).HasErrors,
                "Form/ownership: nested forms keep independent errors and outer validation does not overwrite inner fields");
        }
        finally { window.Close(); Flush(); }
        var input = new TextBox { Name = "InnerDraft" }; inner = new Form { Content = input };
        outer = new Form { Content = new FormItem { MemberName = "OuterField", Content = inner } }; window = Open(outer);
        try
        {
            DataValidationErrors.SetErrors(input, new object[] { new FormatException("Invalid inner draft") });
            check(!Wait(inner.ValidateAsync()) && inner.ValidationErrors.Single().MemberName == "InnerDraft" && Wait(outer.ValidateAsync()),
                "Form/ownership: native errors never inherit a field name from outside their nearest validation host");
        }
        finally { window.Close(); Flush(); }
        var field = new FormItem { MemberName = "Name", Label = "In a step", Content = new TextBox() };
        var step = new VerificationStep { ItemsSource = new[] { new StepItem { Content = field, Validator = new ResultValidator([new("Name", "step error")]) } } };
        window = Open(step);
        try { check(!Wait(step.GoNextAsync()) && field.HasErrors, "FormItem/compatibility: standalone items reuse VerificationStep field error mapping"); }
        finally { window.Close(); Flush(); }
    }
    private static void VerifyAppearance(ThemeVariant theme, ControlSize size, int width, Action<bool, string> check, string? output)
    {
        var view = new FormDemoView(); var form = view.Sample; form.LabelOrientation = width > 400 ? Orientation.Horizontal : Orientation.Vertical; form.LabelWidth = 112;
        var window = Open(view, theme, size, width);
        try
        {
            var item = Part<FormItem>(view, "NameField"); var input = Part<TextBox>(view, "NameInput"); var label = $"Form/{theme.Key}/{size}/{width}";
            check(input.Bounds.Height == (size switch { ControlSize.Small => 30, ControlSize.Large => 44, _ => 36 }), label + ": native inputs inherit all three densities");
            check(item.LabelOrientation == form.LabelOrientation && (width > 400 ? item.LabelDock == Dock.Left && item.EffectiveLabelWidth == 112 : item.LabelDock == Dock.Top && double.IsNaN(item.EffectiveLabelWidth)), label + ": label orientation and width inherit from the form");
            item.LabelOrientation = Orientation.Vertical; Flush(); check(item.LabelDock == Dock.Top, label + ": an item can override the inherited label layout"); item.ClearValue(FormItem.LabelOrientationProperty); Flush();
            check(item.Bounds.Width <= form.Bounds.Width && input.Bounds.Width > 80 && Part<TextBlock>(item, "PART_HelpText").Bounds.Width <= item.Bounds.Width, label + ": labels, inputs and help text stay inside the available width");
            item.Label = "很长的字段标签用于检查换行和文字缩放"; Flush();
            check(Part<Label>(item, "PART_Label").DesiredSize.Width <= item.Bounds.Width && input.Bounds.Width > 80, label + ": long field labels wrap within the field");
            item.Label = "姓名"; Flush();
            Wait(form.ValidateAsync()); Flush();
            check(!Part<Border>(form, "PART_ErrorSummary").IsEffectivelyVisible, label + ": inline field errors are not repeated at the top");
            form.Validator = new ResultValidator([new("", "Service unavailable"), new("Name", "Name is required")]);
            Wait(form.ValidateAsync()); Flush();
            check(form.ValidationErrors.Count == 2 && form.UnmappedErrors.Count == 1 && item.HasErrors, label + ": mixed errors retain the model summary and inline field message");
            var summary = Part<Border>(form, "PART_ErrorSummary"); form.TryFindResource("One.DangerSoft", form.ActualThemeVariant, out var resource);
            using var bitmap = new RenderTargetBitmap(new PixelSize((int)summary.Bounds.Width, (int)summary.Bounds.Height), new Vector(96, 96)); bitmap.Render(summary);
            check(Pixel(bitmap, 5, (int)summary.Bounds.Height / 2) == ((ISolidColorBrush)resource!).Color, label + ": actual summary pixels follow the current semantic theme resource");
            if (output is not null)
            {
                Directory.CreateDirectory(output); using var frame = window.CaptureRenderedFrame()!;
                frame.Save(Path.Combine(output, $"form-{theme.Key}-{size}-{width}.png"), PngBitmapEncoderOptions.Default);
            }
        }
        finally { window.Close(); Flush(); }
    }
    private sealed class DeferredValidator : IStepValidator
    {
        public TaskCompletionSource<IReadOnlyList<StepValidationError>> Completion { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
        public Task<IReadOnlyList<StepValidationError>> ValidateAsync(CancellationToken cancellationToken) => Completion.Task;
    }
    private sealed class ResultValidator(IReadOnlyList<StepValidationError> errors, bool fail = false) : IStepValidator
    {
        public Task<IReadOnlyList<StepValidationError>> ValidateAsync(CancellationToken cancellationToken) => fail ? throw new InvalidOperationException("service failed") : Task.FromResult(errors);
    }
    private sealed class TestCommand : ICommand
    {
        private bool _allowed = true;
        public bool Allowed { get => _allowed; set { _allowed = value; CanExecuteChanged?.Invoke(this, EventArgs.Empty); } }
        public int Count; public object? Parameter;
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => Allowed;
        public void Execute(object? parameter) { Count++; Parameter = parameter; }
    }
    private static bool Wait(Task<bool> task)
    {
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (!task.IsCompleted && DateTime.UtcNow < deadline) { Flush(); Thread.Sleep(1); }
        if (!task.IsCompleted) throw new TimeoutException("Form validation did not complete.");
        Flush(); return task.GetAwaiter().GetResult();
    }
    private static void Press(Window window, Key key, bool reverse = false)
    {
        var modifiers = reverse ? RawInputModifiers.Shift : RawInputModifiers.None;
        window.KeyPress(key, modifiers, PhysicalKey.Tab, null); window.KeyRelease(key, modifiers, PhysicalKey.Tab, null); Flush();
    }
    private static void Click(Control control)
    {
        control.BringIntoView(); Flush(); var window = (Window)TopLevel.GetTopLevel(control)!;
        var point = control.TranslatePoint(new Point(control.Bounds.Width / 2, control.Bounds.Height / 2), window)!.Value;
        window.MouseMove(point); window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Flush();
    }
    private static Color Pixel(Bitmap bitmap, int x, int y)
    {
        var bytes = new byte[4]; var pin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        try { bitmap.CopyPixels(new PixelRect(x, y, 1, 1), pin.AddrOfPinnedObject(), 4, 4); } finally { pin.Free(); }
        return bitmap.Format == PixelFormat.Rgba8888 ? Color.FromArgb(bytes[3], bytes[0], bytes[1], bytes[2]) : Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
    }
    private static void Flush() { Dispatcher.UIThread.RunJobs(); AvaloniaHeadlessPlatform.ForceRenderTimerTick(); Dispatcher.UIThread.RunJobs(); }
}
