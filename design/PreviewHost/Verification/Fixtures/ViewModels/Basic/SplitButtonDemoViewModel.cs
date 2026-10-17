using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Primitives;

namespace TheOne.DesignPreview;

public sealed class SplitButtonDemoViewModel : ReactiveObject
{
    private readonly Lazy<IReadOnlyList<SplitTemplateOption>> _templates;
    private string _format = "PDF", _frequency = "每天";
    private string _selectedTemplate = "模板 00001";

    public SplitButtonDemoViewModel()
    {
        var canExecute = Changed.Where(change => change.PropertyName == nameof(AllowCommand))
            .Select(_ => AllowCommand).StartWith(AllowCommand).DistinctUntilChanged();
        MainActionCommand = ReactiveCommand.Create<object?>(Invoke, canExecute);
        ChooseFormatCommand = ReactiveCommand.Create<object?>(parameter =>
        {
            _format = parameter?.ToString() ?? "PDF";
            this.RaisePropertyChanged(nameof(ExportLabel));
            Choose($"导出格式：{_format}");
        });
        ChooseFrequencyCommand = ReactiveCommand.Create<object?>(parameter =>
        {
            _frequency = parameter?.ToString() ?? "每天";
            this.RaisePropertyChanged(nameof(FrequencyLabel));
            Choose($"发布频率：{_frequency}");
        });
        MenuCommand = ReactiveCommand.Create<object?>(parameter => Choose($"更多操作：{parameter}"));
        TemplateCommand = ReactiveCommand.Create<object?>(parameter =>
        {
            _selectedTemplate = parameter?.ToString() ?? "模板 00001";
            Choose($"已选择：{_selectedTemplate}");
        });
        ResetCommand = ReactiveCommand.Create<object?>(_ =>
        {
            MainCount = ChoiceCount = 0;
            IsEnabled = AllowCommand = true;
            AutoPublish = false;
            _format = "PDF";
            _frequency = "每天";
            _selectedTemplate = "模板 00001";
            Status = "已重置交互状态";
            this.RaisePropertyChanged(nameof(ExportLabel));
            this.RaisePropertyChanged(nameof(FrequencyLabel));
            this.RaisePropertyChanged(nameof(Counts));
        });
        _templates = new Lazy<IReadOnlyList<SplitTemplateOption>>(() =>
            Enumerable.Range(1, 10000).Select(i => new SplitTemplateOption($"模板 {i:00000}", TemplateCommand))
                .ToArray());
    }

    public ICommand MainActionCommand { get; }
    public ICommand ChooseFormatCommand { get; }
    public ICommand ChooseFrequencyCommand { get; }
    public ICommand MenuCommand { get; }
    public ICommand TemplateCommand { get; }
    public ICommand ResetCommand { get; }
    public IReadOnlyList<SplitTemplateOption> Templates => _templates.Value;
    public string ExportLabel => $"导出 · {_format}";
    public string FrequencyLabel => $"发布频率：{_frequency}";
    public string Counts => $"主操作 {MainCount} 次 · 菜单选择 {ChoiceCount} 次 · 自动发布 {(AutoPublish ? "开启" : "关闭")}";

    public int MainCount
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public int ChoiceCount
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool IsEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool AllowCommand
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    public bool AutoPublish
    {
        get;
        set
        {
            if (field == value)
                return;
            this.RaiseAndSetIfChanged(ref field, value);
            this.RaisePropertyChanged(nameof(Counts));
        }
    }

    public string Status
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "准备就绪 · 主操作与更多选择分别响应";

    public string MenuMetrics
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "打开模板菜单可查看实际容器数量";

    private void Choose(string value)
    {
        ChoiceCount++;
        Status = value;
        this.RaisePropertyChanged(nameof(Counts));
    }

    private void Invoke(object? parameter)
    {
        MainCount++;
        Status = parameter?.ToString() switch
        {
            "导出" => $"已导出 {_format} 报表",
            "使用模板" => $"已使用：{_selectedTemplate}",
            _ => $"主操作：{parameter}"
        };
        this.RaisePropertyChanged(nameof(Counts));
    }
}