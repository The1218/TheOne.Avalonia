using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace TheOne.DesignPreview;

/// <summary>Local preview validation only; applications provide their own authentication.</summary>
public sealed class LoginDemoViewModel : ReactiveObject
{
    private readonly Func<Task<bool>> _completeLogin;
    private readonly CancellationTokenSource _lifetime = new();
    private readonly Action<string>? _showError;
    private bool _isClosed;

    public LoginDemoViewModel(Func<Task<bool>> completeLogin, Action cancel, Action<string>? showError = null)
    {
        _completeLogin = completeLogin;
        _showError = showError;
        var canExecute = Changed.Where(change => change.PropertyName == nameof(CanEdit))
            .Select(_ => CanEdit).StartWith(CanEdit).DistinctUntilChanged();
        LoginCommand = ReactiveCommand.CreateFromTask<object?>(_ => LoginAsync(), canExecute);
        CancelCommand = ReactiveCommand.Create<object?>(_ => cancel());
    }

    public string Account
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string Password
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "";

    public string Status
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    } = "输入账号和密码后登录。";

    public bool IsBusy
    {
        get;
        private set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(CanEdit));
        }
    }

    public bool CanEdit => !IsBusy && !_isClosed;
    public ICommand LoginCommand { get; }
    public ICommand CancelCommand { get; }

    public async Task LoginAsync()
    {
        if (!CanEdit)
            return;
        if (string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(Password))
        {
            Status = "请输入账号和密码。";
            _showError?.Invoke(Status);
            return;
        }

        IsBusy = true;
        Status = "正在执行演示校验…";
        try
        {
            await Task.Delay(350, _lifetime.Token);
            if (_isClosed)
                return;
            if (!await _completeLogin() && !_isClosed)
                Status = "窗口关闭被取消，登录窗体继续保留。";
        }
        catch (OperationCanceledException)when (_isClosed)
        {
        }
        catch (Exception exception)
        {
            if (!_isClosed)
            {
                Status = $"打开主窗体失败：{exception.Message}";
                _showError?.Invoke(Status);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void Stop()
    {
        if (_isClosed)
            return;
        _isClosed = true;
        _lifetime.Cancel();
        _lifetime.Dispose();
        Password = "";
        this.RaisePropertyChanged(nameof(CanEdit));
    }
}