using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Controls.ApplicationLifetimes;

namespace TheOne.DesignPreview;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (!Program.IsHeadless && ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.Args?.Contains("--datagrid-stage1") == true)
                desktop.MainWindow = new Window { Title = "DataGrid stage 1", Width = 900, Height = 650, Content = new DataGridStage1View() };
            else if (desktop.Args?.Contains("--login") == true)
            {
                desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
                var mainFactoryCalls = 0;
                Action<string>? trace = desktop.Args.Contains("--trace-login")
                    ? message => Console.WriteLine($"LOGIN_TRACE {message}")
                    : null;
                if (trace is not null)
                    desktop.Exit += (_, args) => trace($"desktop exited code={args.ApplicationExitCode} mainFactoryCalls={mainFactoryCalls} windows={desktop.Windows.Count}");
                desktop.MainWindow = LoginDemo.CreateWindow(() =>
                {
                    if (trace is not null) mainFactoryCalls++;
                    return new MainWindow();
                }, trace);
            }
            else
            {
                desktop.MainWindow = new MainWindow();
            }
        }
        base.OnFrameworkInitializationCompleted();
    }
}
