using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;
using ReactiveUI.Primitives;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class PreviewCommandContracts
{
    public static void Verify(Action<bool, string> check)
    {
        var parameter = new object(); object? received = null;
        using (var command = ReactiveCommand.Create<object?>(value => received = value))
        {
            ICommand action = command;
            action.Execute(parameter); ControlPreview.Flush();
            check(ReferenceEquals(parameter, received), "PreviewHost/commands: direct ReactiveCommand retains the original parameter identity");
            action.Execute(null); ControlPreview.Flush();
            check(received is null, "PreviewHost/commands: null parameters remain valid");
        }
        var basic = new BasicControlViewModel();
        basic.InvokeCommand.Execute("parameter"); ControlPreview.Flush();
        check(basic.InvokeCommand is ReactiveCommand<object?, RxVoid> && basic.Status.Contains("parameter"), "PreviewHost/commands: VM exposes the direct command instance through ICommand");
        basic.LastCommand.Execute(parameter); ControlPreview.Flush();
        check(basic.SelectedIndex == 999, "PreviewHost/commands: parameterless actions still accept and ignore control parameters");

        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var completed = false; var executing = false; var calls = 0;
        using (var command = ReactiveCommand.CreateFromTask<object?>(async value =>
        {
            received = value; calls++; await gate.Task; completed = true;
        }))
        using (command.IsExecuting.Subscribe(value => executing = value))
        {
            ICommand action = command;
            var button = new Button { Command = action, CommandParameter = parameter };
            var asyncHost = new Window { Width = 240, Height = 140, Content = button };
            asyncHost.Show(); ControlPreview.Flush();
            action.Execute(parameter); ControlPreview.Flush();
            check(executing && !completed && !action.CanExecute(null) && ReferenceEquals(parameter, received), "PreviewHost/commands: asynchronous execution tracks unfinished work and command availability");
            Click(button);
            check(calls == 1 && !button.IsEffectivelyEnabled, "PreviewHost/commands: bound button respects CanExecute during asynchronous execution");
            gate.SetResult(); WaitFor(() => completed && !executing && action.CanExecute(null));
            check(completed && action.CanExecute(null) && button.IsEffectivelyEnabled, "PreviewHost/commands: async completion restores command and button availability");
            asyncHost.Close(); ControlPreview.Flush();
        }
        var failure = new InvalidOperationException("expected command error"); Exception? reported = null;
        using (var command = ReactiveCommand.CreateFromTask<object?>(_ => Task.FromException(failure)))
        using (command.ThrownExceptions.Subscribe(error => reported = error))
        {
            ((ICommand)command).Execute(null); WaitFor(() => reported is not null && ((ICommand)command).CanExecute(null));
            check(ReferenceEquals(reported, failure), "PreviewHost/commands: asynchronous errors reach ThrownExceptions without async-void dispatch");
        }

        var split = new SplitButtonDemoView();
        var host = new Window { Width = 960, Height = 700, Content = split };
        Motion.SetEnabled(host, false); host.Show(); ControlPreview.Flush();
        try
        {
            var model = (SplitButtonDemoViewModel)split.DataContext!;
            var export = split.FindControl<SplitButton>("ExportSplit")!;
            model.MainActionCommand.Execute(export.CommandParameter); ControlPreview.Flush();
            check(model.MainCount == 1 && model.Status.Contains("PDF") && ReferenceEquals(export.Command, model.MainActionCommand), "PreviewHost/commands: compiled split button binding preserves its command and parameter");
            model.AllowCommand = false; ControlPreview.Flush();
            check(!model.MainActionCommand.CanExecute(null), "PreviewHost/commands: split CanExecute follows the property stream");
            model.AllowCommand = true; ControlPreview.Flush();
            check(model.MainActionCommand.CanExecute(null), "PreviewHost/commands: split availability recovers after enabling");
        }
        finally { host.Close(); ControlPreview.Flush(); }

        var window = new OneWindow { Width = 720, Height = 480 };
        Motion.SetEnabled(window, false); window.Show(); ControlPreview.Flush();
        try
        {
            var model = new MessagePageViewModel();
            var command = (ReactiveCommand<object?, RxVoid>)model.FormDialogCommand;
            var executingDialog = false;
            using var observation = command.IsExecuting.Subscribe(value => executingDialog = value);
            model.FormDialogCommand.Execute(null); ControlPreview.Flush();
            check(executingDialog && window.IsDialogOpen && !model.FormDialogCommand.CanExecute(null), "PreviewHost/commands: form dialog command remains executing while awaiting its result");
            var editor = window.GetVisualDescendants().OfType<DialogEditorView>().Single();
            var context = (DialogEditorViewModel)editor.DataContext!;
            context.Name = "command result"; context.SaveCommand.Execute(null);
            WaitFor(() => !executingDialog && model.Status.Contains("command result"));
            check(!window.IsDialogOpen && model.Status.Contains(nameof(OneDialogResult.Ok)), "PreviewHost/commands: dialog VM command closes the form and the awaited result updates its parent");
        }
        finally { window.Close(); ControlPreview.Flush(); }

        var login = new LoginDemoViewModel(() => Task.FromResult(false), () => { }) { Account = "user", Password = "password" };
        login.LoginCommand.Execute(null); ControlPreview.Flush();
        check(login.IsBusy && !login.LoginCommand.CanExecute(null), "PreviewHost/commands: direct login command activates busy state and CanExecute gating");
        WaitFor(() => !login.IsBusy && login.LoginCommand.CanExecute(null));
        login.Stop(); ControlPreview.Flush();
        check(!login.LoginCommand.CanExecute(null), "PreviewHost/commands: closed login disables its command through the property stream");
    }

    private static void Click(Control target)
    {
        var host = TopLevel.GetTopLevel(target)!;
        var point = target.TranslatePoint(new Point(target.Bounds.Width / 2, target.Bounds.Height / 2), host)!.Value;
        host.MouseMove(point); host.MouseDown(point, MouseButton.Left); host.MouseUp(point, MouseButton.Left);
        ControlPreview.Flush();
    }

    private static void WaitFor(Func<bool> completed)
    {
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (!completed() && DateTime.UtcNow < deadline)
        {
            var frame = new DispatcherFrame();
            var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            timer.Tick += (_, _) => { timer.Stop(); frame.Continue = false; };
            timer.Start(); Dispatcher.UIThread.PushFrame(frame); ControlPreview.Flush();
        }
        if (!completed()) throw new TimeoutException("ReactiveCommand did not settle.");
    }
}
