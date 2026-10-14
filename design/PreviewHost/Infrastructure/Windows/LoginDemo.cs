using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

internal static class LoginDemo
{
    public static LoginWindow CreateWindow(Func<OneWindow>? mainWindowFactory = null, Action<string>? trace = null)
    {
        var window = new LoginWindow { Title = "TheOne · 登录" };
        var completionRequests = 0;
        var mainFactoryCalls = 0;
        var model = new LoginDemoViewModel(Complete, window.Close, message => window.ShowErrorToast(message));
        window.Content = new LoginDemoView { DataContext = model };
        window.Closed += (_, _) => model.Stop();
        var validationStarts = 0;
        if (trace is not null)
        {
            window.Opened += (_, _) => trace("login opened");
            window.Closed += (_, _) => trace($"login closed validationStarts={validationStarts} completionRequests={completionRequests} mainFactoryCalls={mainFactoryCalls}");
            model.PropertyChanged += (_, args) =>
            {
                if (args.PropertyName != nameof(LoginDemoViewModel.IsBusy)) return;
                if (model.IsBusy) validationStarts++;
                trace($"validation busy={model.IsBusy} starts={validationStarts}");
            };
        }
        return window;

        Task<bool> Complete()
        {
            if (trace is not null)
                trace($"completion requested count={++completionRequests}");
            if (mainWindowFactory is not null)
                return window.CompleteLoginAsync(trace is null ? mainWindowFactory() : CreateMainWindow());
            // A secondary preview demonstrates custom content without replacing the app's main window.
            window.Close();
            return Task.FromResult(!window.IsVisible);
        }

        OneWindow CreateMainWindow()
        {
            var count = ++mainFactoryCalls;
            trace!($"main factory called count={count}");
            var mainWindow = mainWindowFactory!();
            mainWindow.Opened += (_, _) => trace!($"main opened count={count}");
            mainWindow.Closed += (_, _) => trace!($"main closed count={count}");
            return mainWindow;
        }
    }
}
