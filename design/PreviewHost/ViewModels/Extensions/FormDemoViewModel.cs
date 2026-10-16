using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using ReactiveUI;
using TheOne.Avalonia.Controls;

namespace TheOne.DesignPreview;

public sealed class FormAccountModel : ReactiveObject
{
    [Required(ErrorMessage = "请输入姓名"), StringLength(24, MinimumLength = 2, ErrorMessage = "姓名应为 2–24 个字符")]
    public string Name { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";
    [Required(ErrorMessage = "请输入邮箱"), EmailAddress(ErrorMessage = "邮箱格式不正确")]
    public string Email { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";
    [StepCompare(nameof(Email), ErrorMessage = "两次邮箱输入不一致")]
    public string ConfirmEmail { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";
    [Range(18, 120, ErrorMessage = "年龄应为 18–120")]
    public int Age { get; set => this.RaiseAndSetIfChanged(ref field, value); } = 28;
}

public sealed class FormDemoViewModel : ReactiveObject
{
    public FormAccountModel Account { get; } = new();
    public IStepValidator Validator { get; }
    public ICommand SaveCommand { get; }
    public int SavedCount { get; private set => this.RaiseAndSetIfChanged(ref field, value); }
    public FormDemoViewModel()
    {
        Validator = StepValidation.For(Account);
        SaveCommand = ReactiveCommand.Create(() => SavedCount++);
    }
}
