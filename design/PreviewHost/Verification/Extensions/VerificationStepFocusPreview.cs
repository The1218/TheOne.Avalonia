using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Headless;
using Avalonia.Styling;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class TagVerificationStepPreview
{
    private static void Tab(Window window, bool reverse = false)
    {
        var modifiers = reverse ? RawInputModifiers.Shift : RawInputModifiers.None;
        window.KeyPress(Key.Tab, modifiers, PhysicalKey.Tab, null);
        window.KeyRelease(Key.Tab, modifiers, PhysicalKey.Tab, null);
        Flush();
    }
    private static void VerifyFocus(ThemeVariant theme, ControlSize size, Action<bool, string> check)
    {
        var view = new VerificationStepDemoView(); var step = view.Sample; var window = Open(view, theme, size);
        try
        {
            var name = Part<TextBox>(step, "Name"); var email = Part<TextBox>(step, "Email"); var confirm = Part<TextBox>(step, "ConfirmEmail");
            var form = Part<ListBox>(step, "PART_FormFields");
            name.Focus(); Tab(window);
            check(email.IsFocused, $"VerificationStep/focus/{theme.Key}/{size}: Tab moves from Name to Email; actual={window.FocusManager!.GetFocusedElement()}, form={KeyboardNavigation.GetTabNavigation(form)}, presenter={KeyboardNavigation.GetTabNavigation(Part<ItemsPresenter>(form, "PART_ItemsPresenter"))}");
            Tab(window); check(confirm.IsFocused, "VerificationStep/focus: Tab visits the next generated field");
            Tab(window, true); check(email.IsFocused, "VerificationStep/focus: Shift+Tab visits the preceding generated field");
            Tab(window, true); check(name.IsFocused, "VerificationStep/focus: reverse traversal reaches the first generated field");
            confirm.Focus(); Tab(window);
            check(Part<Button>(step, "PART_NextButton").IsFocused, "VerificationStep/focus: Tab leaves the last field for the native next button");
            Tab(window, true); check(confirm.IsFocused, "VerificationStep/focus: Shift+Tab re-enters the last generated field");
            email.IsEnabled = false; name.Focus(); Tab(window);
            check(confirm.IsFocused, "VerificationStep/focus: native Tab traversal skips disabled fields");
            email.IsEnabled = true; KeyboardNavigation.SetIsTabStop(email, false); name.Focus(); Tab(window);
            check(confirm.IsFocused, "VerificationStep/focus: native IsTabStop=false is respected");
            KeyboardNavigation.SetIsTabStop(email, true);
            check(!Next(step), "VerificationStep/focus: invalid fields still block the next step");
            name.Focus(); Tab(window);
            check(email.IsFocused, "VerificationStep/focus: inline validation errors do not interrupt field traversal");
        }
        finally { window.Close(); Flush(); }

        var first = new TextBox(); var disabled = new TextBox { IsEnabled = false }; var choice = new CheckBox { Content = "Accept" }; var last = new Button { Content = "Custom action" };
        var custom = new VerificationStep
        {
            ItemsSource = new[] { new StepItem<StepAccountModel>(new StepAccountModel()) },
            ContentTemplate = new FuncDataTemplate<StepAccountModel>((_, _) => new StackPanel { Children = { first, disabled, choice, last } })
        };
        window = Open(custom, theme, size);
        try
        {
            first.Focus(); Tab(window); check(choice.IsFocused, "VerificationStep/focus: custom templates retain native focus and skip disabled inputs");
            Tab(window); check(last.IsFocused, "VerificationStep/focus: custom child controls remain in Tab order");
            Tab(window, true); check(choice.IsFocused, "VerificationStep/focus: custom templates support reverse traversal");
        }
        finally { window.Close(); Flush(); }
    }
}
