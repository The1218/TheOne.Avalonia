using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using TheOne.Avalonia.Controls;
using ReactiveUI;
using System.Windows.Input;
using Avalonia.Layout;

namespace TheOne.DesignPreview;

public sealed class TagDemoViewModel : ReactiveObject
{
    public IReadOnlyList<TagStatus> Statuses { get; } = Enum.GetValues<TagStatus>();
    public IReadOnlyList<TagVariant> Variants { get; } = Enum.GetValues<TagVariant>();
    public TagStatus Status { get; set => this.RaiseAndSetIfChanged(ref field, value); } = TagStatus.Success;
    public TagVariant Variant { get; set => this.RaiseAndSetIfChanged(ref field, value); }
    public string Text { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "已完成";
    public bool Enabled { get; set => this.RaiseAndSetIfChanged(ref field, value); } = true;
}

public sealed class StepAccountModel : ReactiveObject
{
    [Display(Name = "姓名", Prompt = "2–24 个字符", Order = 0)]
    [Required(ErrorMessage = "请输入姓名。")]
    [StringLength(24, MinimumLength = 2, ErrorMessage = "姓名需为 2–24 个字符。")]
    public string Name { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";

    [Display(Name = "邮箱", Prompt = "name@example.com", Order = 1)]
    [Required(ErrorMessage = "请输入邮箱。")]
    [EmailAddress(ErrorMessage = "邮箱格式不正确。")]
    public string Email { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";

    [Display(Name = "确认邮箱", Prompt = "再次输入邮箱", Order = 2)]
    [Required(ErrorMessage = "请再次输入邮箱。")]
    [StepCompare(nameof(Email), ErrorMessage = "两次邮箱输入不一致。")]
    public string ConfirmEmail { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";
}

public sealed class StepProfileModel : ReactiveObject
{
    [Display(Name = "年龄", Prompt = "18–120")]
    [Required(ErrorMessage = "请输入年龄。")]
    [Range(18, 120, ErrorMessage = "年龄需在 18–120 之间。")]
    public decimal? Age { get; set => this.RaiseAndSetIfChanged(ref field, value); } = 16;

    [Display(Name = "联系地址", Prompt = "6–120 个字符")]
    [Required(ErrorMessage = "请输入联系地址。")]
    [StringLength(120, MinimumLength = 6, ErrorMessage = "联系地址需为 6–120 个字符。")]
    public string Address { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "";
}

public sealed class StepConfirmModel : ReactiveObject
{
    [Display(Name = "我已核对以上信息")]
    [AllowedValues(true, ErrorMessage = "请确认已核对以上信息。")]
    public bool Accepted { get; set => this.RaiseAndSetIfChanged(ref field, value); }
}

public sealed class StepNoteModel : ReactiveObject
{
    [Display(Name = "步骤内容")]
    [Required(ErrorMessage = "步骤内容不能为空。")]
    public string Note { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "示例内容";
}

public sealed class StepDemoViewModel : ReactiveObject
{
    public IReadOnlyList<HorizontalAlignment> HeaderAlignments { get; } = [HorizontalAlignment.Center, HorizontalAlignment.Stretch, HorizontalAlignment.Left, HorizontalAlignment.Right];
    public HorizontalAlignment HeaderAlignment { get; set => this.RaiseAndSetIfChanged(ref field, value); } = HorizontalAlignment.Center;
    public StepAccountModel Account { get; private set => this.RaiseAndSetIfChanged(ref field, value); } = new();
    public StepProfileModel Profile { get; private set => this.RaiseAndSetIfChanged(ref field, value); } = new();
    public StepConfirmModel Confirmation { get; private set => this.RaiseAndSetIfChanged(ref field, value); } = new();
    public ObservableCollection<StepItem> Steps { get; private set => this.RaiseAndSetIfChanged(ref field, value); } = [];
    public string Status { get; set => this.RaiseAndSetIfChanged(ref field, value); } = "填写当前步骤，验证通过后才能前进。";

    public ICommand CompletedCommand { get; }
    public int CompletionCount { get; private set; }
    public StepDemoViewModel()
    {
        CompletedCommand = ReactiveCommand.Create(() =>
        {
            CompletionCount++;
            Status = "所有步骤验证通过，已执行 VM 的 CompletedCommand。";
        });
        Reset();
    }
    public void Reset()
    {
        Account = new();
        Profile = new();
        Confirmation = new();
        Steps = [
            new StepItem<StepAccountModel>(Account) { Title = "基本资料", Description = "必填 · 邮箱 · 一致性" },
            new StepItem<StepProfileModel>(Profile) { Title = "联系信息", Description = "范围 · 文本长度" },
            new StepItem<StepConfirmModel>(Confirmation) { Title = "确认提交", Description = "标准属性验证确认状态" }
        ];
        Status = "填写当前步骤，验证通过后才能前进。";
    }

    public void LoadThousand()
    {
        Steps = new(Enumerable.Range(1, 1000).Select(index =>
        {
            var model = new StepNoteModel
            {
                Note = $"步骤 {index} 的模型内容"};
            return new StepItem<StepNoteModel>(model)
            {
                Title = $"步骤 {index}",
                Description = "有界虚拟化导航",
            };
        }));
        Status = "已载入 1,000 个模型；仅创建可见标题和当前步骤内容。不能跳过未验证步骤。";
    }
}
