using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

internal sealed class DialogCoordinator(Window window, Func<Panel> dialogLayer, Action<bool> setDialogOpen)
{
    private readonly LinkedList<Request> _pending = [];
    private Request? _active;
    private bool _closed;
    private Panel? _layer;
    private IInputElement? _previousFocus;

    public Task<OneDialogResult> Enqueue(object? content, DialogOptions options, IDataTemplate? template,
        CancellationToken token, Func<UserControl>? factory = null, DialogContext? context = null)
    {
        if (_closed) throw new InvalidOperationException("This dialog owner is closed.");
        if (options.Size is { } size && !Enum.IsDefined(size)) throw new ArgumentOutOfRangeException(nameof(options));
        if (!Enum.IsDefined(options.Buttons)) throw new ArgumentOutOfRangeException(nameof(options));
        if (token.IsCancellationRequested) return Task.FromCanceled<OneDialogResult>(token);
        if (content is Control { Parent: not null })
            throw new ArgumentException("Dialog content must not already have a parent.", nameof(content));
        _ = dialogLayer();
        var request = new Request(content, options, template, token, factory, context);
        if (context is not null)
        {
            request.CloseHandler = (_, args) =>
            {
                if (Dispatcher.UIThread.CheckAccess()) Finish(request, args.Result);
                else Dispatcher.UIThread.Post(() => Finish(request, args.Result));
            };
            context.Attach(request, request.CloseHandler);
        }

        request.Node = _pending.AddLast(request);
        if (token.CanBeCanceled)
            request.Cancellation = token.Register(() =>
                Dispatcher.UIThread.Post(() => Finish(request, OneDialogResult.Cancel, true)));
        StartNext();
        return request.Completion.Task;
    }

    public void TemplateChanged(Panel? layer)
    {
        if (_active?.Presenter is not { } presenter) return;
        _layer?.Children.Remove(presenter);
        _layer = layer;
        if (_layer is not null) _layer.Children.Add(presenter);
        else
            Finish(_active, OneDialogResult.Closed,
                error: new InvalidOperationException("The window template must contain PART_DialogLayer."));
    }

    private void StartNext()
    {
        if (_active is not null || _pending.First is not { } node || _closed) return;
        _pending.Remove(node);
        node.Value.Node = null;
        _active = node.Value;
        _previousFocus = window.FocusManager?.GetFocusedElement();
        try
        {
            var request = _active;
            var content = request.Factory is { } factory ? factory() : request.Content;
            if (request.Completion.Task.IsCompleted) return;
            var options = request.Options;
            request.Presenter = new OneDialogPresenter
            {
                Header = options.Title, Content = content, ContentTemplate = request.Template,
                Buttons = options.Buttons,
                ShowTitle = options.ShowTitle && !string.IsNullOrEmpty(options.Title),
                ShowCloseButton = options.ShowCloseButton,
                ShowButtons = options.ShowButtons, CloseOnEscape = options.CloseOnEscape
            };
            if (options.Size is { } size) ControlSizing.SetSize(request.Presenter, size);
            if (options.MotionEnabled is { } motion) Motion.SetEnabled(request.Presenter, motion);
            AutomationProperties.SetName(request.Presenter, request.Options.Title ?? "对话框");
            request.Presenter.Complete = result => Finish(request, result);
            _layer = dialogLayer();
            setDialogOpen(true);
            _layer.Children.Add(request.Presenter);
            Dispatcher.UIThread.Post(() => _active?.Presenter?.FocusPrimary(), DispatcherPriority.Loaded);
        }
        catch (Exception error)
        {
            Finish(node.Value, OneDialogResult.Closed, error: error);
        }
    }

    private void Finish(Request request, OneDialogResult result, bool cancel = false, Exception? error = null)
    {
        if (request.Completion.Task.IsCompleted) return;
        request.Cancellation.Dispose();
        if (request.Context is { } context && request.CloseHandler is { } handler) context.Detach(request, handler);
        if (request.Presenter is { } presenter)
        {
            presenter.Complete = null;
            presenter.Content = null;
        }

        if (request.Node is { } node)
        {
            _pending.Remove(node);
            request.Node = null;
        }

        if (_active == request)
        {
            if (request.Presenter is { } current) _layer?.Children.Remove(current);
            _active = null;
            setDialogOpen(false);
            if (_previousFocus is Control { IsEffectivelyEnabled: true } control && control.IsAttachedToVisualTree())
                control.Focus();
            _previousFocus = null;
        }

        if (error is not null) request.Completion.TrySetException(error);
        else if (cancel) request.Completion.TrySetCanceled(request.Token);
        else request.Completion.TrySetResult(result);
        StartNext();
    }

    public void OverlayClicked()
    {
        if (_active is { Options.CloseOnOverlayClick: true } request) Finish(request, OneDialogResult.Closed);
    }

    public void CloseWindow()
    {
        _closed = true;
        if (_active is { } active) Finish(active, OneDialogResult.Closed);
        while (_pending.First is { } node) Finish(node.Value, OneDialogResult.Closed);
        _layer = null;
    }

    private sealed class Request(
        object? content,
        DialogOptions options,
        IDataTemplate? template,
        CancellationToken token,
        Func<UserControl>? factory,
        DialogContext? context)
    {
        public CancellationTokenRegistration Cancellation;
        public EventHandler<DialogCloseRequestedEventArgs>? CloseHandler;
        public LinkedListNode<Request>? Node;
        public OneDialogPresenter? Presenter;
        public object? Content { get; } = content;
        public DialogOptions Options { get; } = options;
        public Func<UserControl>? Factory { get; } = factory;
        public DialogContext? Context { get; } = context;
        public IDataTemplate? Template { get; } = template;
        public CancellationToken Token { get; } = token;

        public TaskCompletionSource<OneDialogResult> Completion { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
    }
}