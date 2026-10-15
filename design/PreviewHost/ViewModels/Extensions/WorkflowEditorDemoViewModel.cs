using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TheOne.Avalonia.Workflow;
using Avalonia.Media;
using Material.Icons;

namespace TheOne.DesignPreview;

public sealed class WorkflowBusinessSummary : WorkflowObservable
{
    public string Caption { get; }
    public string Detail { get; }
    public WorkflowNode Node { get; }
    public MaterialIconKind Icon => Node.TypeId switch { "start" => MaterialIconKind.PlayCircle, "end" => MaterialIconKind.StopCircle, "approval" => MaterialIconKind.Account, "condition" => MaterialIconKind.RhombusOutline, "parallel" => MaterialIconKind.SourceBranch, "merge" => MaterialIconKind.SourceMerge, "note" => MaterialIconKind.NoteTextOutline, _ => MaterialIconKind.Cog };
    public IBrush Accent => new SolidColorBrush(Color.Parse(Node.TypeId switch { "start" => "#38A76D", "end" => "#E04A5D", "approval" => "#3495E8", "condition" => "#F48016", "parallel" => "#368FF0", "merge" => "#7F919D", "note" => "#B68718", _ => "#8151D5" }));
    public IBrush IconBackground => new SolidColorBrush(Color.FromArgb(35, ((SolidColorBrush)Accent).Color.R, ((SolidColorBrush)Accent).Color.G, ((SolidColorBrush)Accent).Color.B));
    public IBrush StateBrush => new SolidColorBrush(Color.Parse(Node.State switch { WorkflowNodeState.Completed => "#259961", WorkflowNodeState.Running => "#009FA9", WorkflowNodeState.Failed => "#E04455", _ => "#8B99A4" }));
    public string StateIcon => Node.State == WorkflowNodeState.Completed ? "✓" : Node.State == WorkflowNodeState.Failed ? "!" : "●";
    public bool IsNote => Node.IsAnnotation;
    public bool IsNotNote => !IsNote;
    public bool HasWarning => Node.Properties.Any(p => p.IsRequired && p.Reference == null && string.IsNullOrWhiteSpace(p.Value));
    public string Warning => Node.TypeId == "approval" && HasWarning ? "⚠ 缺少审批人" : HasWarning ? "⚠ 缺少配置" : "";
    public WorkflowBusinessSummary(string caption, string detail, WorkflowNode node)
    {
        Caption = caption; Detail = detail; Node = node;
        node.PropertyChanged += (_, _) => { Notify(nameof(StateBrush)); Notify(nameof(StateIcon)); Notify(nameof(HasWarning)); Notify(nameof(Warning)); };
    }
}
public sealed record WorkflowRole(string Id, string Title);

/// <summary>Consumer-owned factories, business metadata, field editors and persistence.</summary>
public sealed class WorkflowEditorDemoViewModel : WorkflowObservable
{
    private WorkflowDocument _document = new();
    private WorkflowNode? _selected;
    private string _json = "", _message = "节点与业务数据均由消费端提供。选择节点编辑字段，选择上游输出建立关联。", _nodeTitle = "";
    private readonly bool _designSample;
    private readonly IWorkflowEditorFileService? _files;
    private bool _showConsumerExamples;
    public bool ShowConsumerExamples { get => _showConsumerExamples; set => Set(ref _showConsumerExamples, value); }
    private bool _restoringContent;
    private long _savedRevision = -1;
    public string SaveStateText => _savedRevision == Document.Revision ? "已保存" : "有未保存更改";
    public void MarkSaved(WorkflowDocument document, long revision) { if (document == Document) { _savedRevision = revision; Notify(nameof(SaveStateText)); } }
    public WorkflowRole[] Roles { get; } = [new("department-manager", "直属主管"), new("finance", "财务负责人"), new("owner", "流程负责人")];
    public WorkflowRole? SelectedRole { get => Roles.FirstOrDefault(r => r.Id == Value("roleId")); set { SetField("roleId", value?.Id ?? ""); Notify(); } }
    public bool AnyApproval { get => Value("mode") != "all"; set { if (value) SetField("mode", "any"); } }
    public bool AllApproval { get => Value("mode") == "all"; set { if (value) SetField("mode", "all"); } }
    public decimal? TimeoutHours { get => decimal.TryParse(Value("timeout"), out var number) ? number : 24; set => SetField("timeout", (value ?? 24).ToString(System.Globalization.CultureInfo.InvariantCulture)); }
    public bool AllowDelegate { get => Value("delegate") != "false"; set => SetField("delegate", value ? "true" : "false"); }
    public string Description { get => Value("description"); set => SetField("description", value); }
    public bool IsApproval => SelectedNode?.TypeId == "approval";
    public bool IsOtherNode => !IsApproval;
    public string SelectedNodeType => SelectedNode?.TypeId switch { "approval" => "审批节点", "request" => "任务节点", "condition" => "条件节点", "start" => "开始节点", "end" => "结束节点", "merge" => "合并节点", "parallel" => "并行节点", "note" => "备注", _ => "通知节点" };
    public string OutputSummary => SelectedNode == null ? "" : string.Join("\n", SelectedNode.Ports.SelectMany(p => (p.Fields ?? []).Select(f => $"{(p.Id == "approved" ? "result" : p.Id)}.{f.Path} : {(f.DataType == "boolean" ? "Boolean" : f.DataType)}")));
    public string[] TimeoutUnits { get; } = ["小时", "天"];
    public string TimeoutUnit { get => Value("timeoutUnit") is { Length: > 0 } unit ? unit : "小时"; set => SetField("timeoutUnit", value); }
    public string[] RejectionModes { get; } = ["结束流程", "退回申请人"];
    public string RejectionMode { get => Value("rejection") is { Length: > 0 } mode ? mode : "结束流程"; set => SetField("rejection", value); }
    private string Value(string key) => SelectedNode?.Properties.FirstOrDefault(p => p.Key == key)?.Value ?? "";
    private void SetField(string key, string value) { if (SelectedNode?.Properties.FirstOrDefault(p => p.Key == key) is { } p) Document.SetProperty(p, value, null); }
    private void NotifyInspector() { foreach (var key in new[] { nameof(SelectedRole), nameof(AnyApproval), nameof(AllApproval), nameof(TimeoutHours), nameof(AllowDelegate), nameof(Description), nameof(IsApproval), nameof(IsOtherNode), nameof(TimeoutUnit), nameof(RejectionMode), nameof(SelectedNodeType), nameof(OutputSummary) }) Notify(key); }
    public WorkflowDocument Document { get => _document; set { if (_document == value) return; _document.Changed -= DocumentChanged; _savedRevision = -1; Set(ref _document, value); _document.Changed += DocumentChanged; SelectedNode = null; Notify(nameof(SaveStateText)); } }
    public WorkflowNode? SelectedNode
    {
        get => _selected;
        set { if (!Set(ref _selected, value)) return; NodeTitle = value?.Title ?? ""; Fields.Clear(); if (value != null) foreach (var p in value.Properties) Fields.Add(new(Document, value, p)); NotifyInspector(); }
    }
    public string NodeTitle { get => _nodeTitle; set => Set(ref _nodeTitle, value); }
    public string Json { get => _json; set => Set(ref _json, value); }
    public string Message { get => _message; set => Set(ref _message, value); }
    public ObservableCollection<WorkflowFieldEditor> Fields { get; } = [];
    public ObservableCollection<WorkflowNodeDefinition> NodeDefinitions { get; }
    public ICommand SaveCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand OpenCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand ApplyTitleCommand { get; }
    public ICommand PreviewStateCommand { get; }
    public ICommand ResolveDataCommand { get; }
    public WorkflowEditorDemoViewModel(bool designSample = false, IWorkflowEditorFileService? files = null)
    {
        _designSample = designSample; _files = files;
        NodeDefinitions = [
            Definition("start", "开始", "流程入口", "基础节点", () => Endpoint(true)),
            Definition("end", "结束", "流程出口", "基础节点", () => Endpoint(false)),
            new() { TypeId = "request", Title = "提交申请", Description = "提供申请人、金额等输出", CreateNode = () => Request() },
            new() { TypeId = "approval", Title = "审批", Description = "消费端提供审批角色与限额", CreateNode = () => Approval() },
            Definition("condition", "条件", "按规则分支", "逻辑节点", Condition),
            Definition("parallel", "并行", "同时执行", "逻辑节点", () => Junction(false)),
            Definition("merge", "合并", "汇聚分支", "逻辑节点", () => Junction(true)),
            Definition("note", "备注", "添加说明", "逻辑节点", Note),
            new() { TypeId = "notification", Title = "发送通知", Description = "收件人可引用上游输出", Category = "业务示例", CreateNode = () => Notification() }
        ];
        // Library ordering and presentation data belong to this consumer, not the editor.
        var requestDefinition = NodeDefinitions.First(d => d.TypeId == "request"); var approvalDefinition = NodeDefinitions.First(d => d.TypeId == "approval");
        NodeDefinitions.Remove(requestDefinition); NodeDefinitions.Remove(approvalDefinition);
        NodeDefinitions.Insert(2, Definition("approval", "审批", "人工处理", "基础节点", () => Approval()));
        NodeDefinitions.Insert(3, Definition("request", "任务", "自动执行", "基础节点", () => Request()));
        ExportCommand = new DemoCommand(() => { Json = WorkflowSerializer.Save(Document); ShowConsumerExamples = true; Message = "已生成 JSON：保存节点、端口、连线与字段关联，不保存模板、Content 和运行状态。"; });
        SaveCommand = new WorkflowFileCommand(SaveAsync, ex => Message = "保存失败：" + ex.Message);
        OpenCommand = new WorkflowFileCommand(OpenAsync, ex => Message = "打开失败：" + ex.Message);
        LoadCommand = new DemoCommand(LoadJson);
        ResetCommand = new DemoCommand(Reset);
        ApplyTitleCommand = new DemoCommand(() => { if (SelectedNode is not { } n) return; var old = n.Title; var title = NodeTitle; if (old != title) Document.Edit("修改标题", () => n.Title = title, () => n.Title = old); });
        PreviewStateCommand = new DemoCommand(() => { for (var i = 0; i < Document.Nodes.Count; i++) Document.Nodes[i].State = i == 0 ? WorkflowNodeState.Completed : i == 1 ? WorkflowNodeState.Running : WorkflowNodeState.Pending; Message = "运行状态仅为宿主注入的演示数据，没有执行后台任务。"; });
        ResolveDataCommand = new DemoCommand(() =>
        {
            // Host output adapter: a real application supplies execution outputs here.
            var outputs = new Dictionary<WorkflowDataReference, string>();
            foreach (var source in Document.Nodes.Where(n => n.TypeId == "request"))
                foreach (var p in source.Properties.Where(p => p.Reference == null)) outputs[new(source.Id, "request", p.Key)] = p.Value;
            var resolved = Document.Nodes.SelectMany(n => n.Properties.Where(p => p.Reference != null).Select(p => $"{n.Title}.{p.Title} = {outputs.GetValueOrDefault(p.Reference!, "[输出尚未提供]")}"));
            Message = "宿主通过稳定 ID 解析引用：" + string.Join("；", resolved);
        });
        Reset();
    }
    private async Task SaveAsync()
    {
        if (_files == null) { Message = "当前宿主没有文件保存服务，请使用 JSON 文本。"; return; }
        var document = Document; var revision = document.Revision; var json = WorkflowSerializer.Save(document);
        if (!await _files.SaveAsync(json)) return;
        Json = json; MarkSaved(document, revision); Message = "流程文件已保存。";
    }
    private async Task OpenAsync()
    {
        if (_files == null) { Message = "当前宿主没有文件打开服务，请使用 JSON 文本。"; return; }
        var json = await _files.OpenAsync();
        if (json != null) { Json = json; LoadJson(); }
    }
    public void LoadJson()
    {
        try
        {
            var loaded = WorkflowSerializer.Load(Json);
            foreach (var n in loaded.Nodes) RestoreContent(n);
            Document = loaded;
            var issues = loaded.Validate();
            Message = $"加载成功：{loaded.Nodes.Count} 个节点、{loaded.Connections.Count} 条连线、{issues.Count} 项校验问题。已由消费端按 TypeId 恢复 Content。";
        }
        catch (System.Text.Json.JsonException e) { Message = "加载失败，保留原流程：" + e.Message; }
    }
    public void Reset()
    {
        if (_designSample) { ResetDesign(); return; }
        var doc = new WorkflowDocument { Title = "采购申请 → 主管审批 → 通知申请人" };
        var submit = Request("submit_01"); var approval = Approval("approval_01"); var notification = Notification("notify_01");
        submit.X = 0; approval.X = 300; notification.X = 600;
        submit.Y = approval.Y = notification.Y = 80;
        doc.Nodes.Add(submit); doc.Nodes.Add(approval); doc.Nodes.Add(notification);
        doc.TryConnect(submit.Id, "request", approval.Id, "in", out _);
        doc.TryConnect(approval.Id, "approved", notification.Id, "in", out _);
        doc.Connections[0].Label = "申请资料"; doc.Connections[1].Label = "审批通过";
        approval.Properties.Single(p => p.Key == "amount").Reference = new(submit.Id, "request", "amount");
        notification.Properties.Single(p => p.Key == "recipient").Reference = new(submit.Id, "request", "applicant.email");
        doc.ClearHistory(); Document = doc; SelectedNode = notification;
    }
    private void DocumentChanged(object? sender, EventArgs e)
    {
        if (_restoringContent) return;
        // Clipboard and external document edits carry data, while presentation is rebuilt by the host.
        _restoringContent = true;
        try { foreach (var node in Document.Nodes.Where(n => n.Content == null)) RestoreContent(node); }
        finally { _restoringContent = false; }
        foreach (var row in Fields) row.Refresh();
        if (SelectedNode != null && !Document.Nodes.Contains(SelectedNode)) SelectedNode = null;
        NotifyInspector(); Notify(nameof(SaveStateText));
    }
    private static WorkflowNodeDefinition Definition(string type, string title, string description, string category, Func<WorkflowNode> create)
    { var sample = create(); return new() { TypeId = type, Title = title, Description = description, Category = category, CreateNode = create, Content = sample.Content }; }
    private void ResetDesign()
    {
        var doc = new WorkflowDocument { Title = "采购审批流程" };
        var start = Endpoint(true, "start_01"); var request = Request("submit_01"); var condition = Condition(); var supervisor = Approval("approval_01"); var finance = Approval("finance_01"); var merge = Junction(true); var end = Endpoint(false, "end_01");
        start.X = 0; start.Y = 200; request.X = 155; request.Y = 190; condition.X = 335; condition.Y = 190;
        supervisor.X = 550; supervisor.Y = 89; finance.X = 550; finance.Y = 292; merge.X = 760; merge.Y = 200; end.X = 920; end.Y = 200;
        start.Width = end.Width = 110; request.Width = 160; condition.Width = 165; supervisor.Width = finance.Width = 175; merge.Width = 130;
        request.Description = "提交采购申请"; condition.Description = "判断采购金额"; supervisor.Description = "由直属主管审批"; finance.Title = "财务审批"; finance.Description = "由财务部门审批";
        finance.Properties.Single(p => p.Key == "roleId").Value = "";
        foreach (var node in new[] { supervisor, finance }) { var port = node.Ports.Single(p => p.Id == "approved"); node.Ports[node.Ports.IndexOf(port)] = port with { MaxConnections = 1 }; }
        foreach (var n in new[] { start, request, condition, supervisor, finance, merge, end }) doc.Nodes.Add(n);
        doc.TryConnect(start.Id, "out", request.Id, "in", out _); doc.TryConnect(request.Id, "request", condition.Id, "in", out _);
        doc.TryConnect(condition.Id, "yes", supervisor.Id, "in", out _); doc.Connections.Last().Label = "≤ ¥5,000";
        doc.TryConnect(condition.Id, "no", finance.Id, "in", out _); doc.Connections.Last().Label = "> ¥5,000";
        doc.TryConnect(supervisor.Id, "approved", merge.Id, "in", out _); doc.TryConnect(finance.Id, "approved", merge.Id, "in", out _); doc.TryConnect(merge.Id, "out", end.Id, "in", out _);
        foreach (var node in new[] { supervisor, finance }) node.Properties.Single(p => p.Key == "amount").Reference = new(request.Id, "request", "amount");
        start.State = request.State = condition.State = WorkflowNodeState.Completed; supervisor.State = WorkflowNodeState.Running;
        var note = Note(); note.X = 400; note.Y = 403; doc.Nodes.Add(note);
        doc.ClearHistory(); Document = doc; SelectedNode = supervisor;
    }
    private static WorkflowNode Endpoint(bool start, string? id = null)
    {
        var node = new WorkflowNode { Id = id ?? Guid.NewGuid().ToString("N"), TypeId = start ? "start" : "end", Title = start ? "开始" : "结束", Description = start ? "流程入口" : "流程出口", Width = 120, Shape = WorkflowNodeShape.Pill };
        node.Ports.Add(new(start ? "out" : "in", start ? "输出" : "输入", start ? WorkflowPortDirection.Output : WorkflowPortDirection.Input)); RestoreContent(node); return node;
    }
    private static WorkflowNode Junction(bool merge)
    {
        var node = new WorkflowNode { TypeId = merge ? "merge" : "parallel", Title = merge ? "汇合" : "并行", Description = merge ? "汇聚分支" : "同时执行", Width = 135 };
        node.Ports.Add(new("in", "输入", WorkflowPortDirection.Input, "flow", merge ? 0 : 1)); node.Ports.Add(new("out", "输出", WorkflowPortDirection.Output, "flow", merge ? 1 : 0)); RestoreContent(node); return node;
    }
    private static WorkflowNode Note()
    { var n = new WorkflowNode { TypeId = "note", Title = "备注", Description = "超过 5,000 元需\n财务审批。", Width = 150, Shape = WorkflowNodeShape.Note, IsAnnotation = true }; RestoreContent(n); return n; }
    private static WorkflowNode Request(string? id = null)
    {
        var n = new WorkflowNode { Id = id ?? Guid.NewGuid().ToString("N"), TypeId = "request", Title = "提交申请", Description = "消费端定义业务字段与输出描述", Width = 170 };
        n.Ports.Add(new("in", "输入", WorkflowPortDirection.Input));
        n.Ports.Add(new("request", "申请数据", WorkflowPortDirection.Output, "flow", 0, [new("applicant.email", "申请人邮箱", "email"), new("amount", "采购金额", "number")]));
        n.Properties.Add(new() { Key = "applicant.email", Title = "申请人邮箱", DataType = "email", IsRequired = true, Value = "user@example.com" });
        n.Properties.Add(new() { Key = "amount", Title = "采购金额", DataType = "number", Kind = WorkflowValueKind.Number, IsRequired = true, Value = "6800" }); RestoreContent(n); return n;
    }
    private static WorkflowNode Approval(string? id = null)
    {
        var n = new WorkflowNode { Id = id ?? Guid.NewGuid().ToString("N"), TypeId = "approval", Title = "主管审批", Description = "角色 ID 关联宿主的组织架构数据" };
        n.Ports.Add(new("in", "输入", WorkflowPortDirection.Input));
        n.Ports.Add(new("approved", "通过", WorkflowPortDirection.Output, "flow", 0, [new("approved", "审批结果", "boolean")]));
        n.Properties.Add(new() { Key = "roleId", Title = "审批角色 ID", IsRequired = true, Value = "department-manager" });
        n.Properties.Add(new() { Key = "amount", Title = "待审批金额", DataType = "number", Kind = WorkflowValueKind.Number, IsRequired = true, Value = "6800" });
        n.Properties.Add(new() { Key = "mode", Title = "审批方式", Value = "any" });
        n.Properties.Add(new() { Key = "timeout", Title = "超时设置", Kind = WorkflowValueKind.Number, Value = "24" });
        n.Properties.Add(new() { Key = "timeoutUnit", Title = "超时单位", Value = "小时" });
        n.Properties.Add(new() { Key = "delegate", Title = "允许转交", Kind = WorkflowValueKind.Boolean, Value = "true" });
        n.Properties.Add(new() { Key = "description", Title = "说明", Value = "请核对采购明细与预算。" });
        n.Properties.Add(new() { Key = "rejection", Title = "驳回处理", Value = "结束流程" }); RestoreContent(n); return n;
    }
    private static WorkflowNode Notification(string? id = null)
    {
        var n = new WorkflowNode { Id = id ?? Guid.NewGuid().ToString("N"), TypeId = "notification", Title = "发送通知", Description = "收件人字段引用提交申请的邮箱" };
        n.Ports.Add(new("in", "输入", WorkflowPortDirection.Input));
        n.Properties.Add(new() { Key = "recipient", Title = "收件人", DataType = "email", IsRequired = true });
        n.Properties.Add(new() { Key = "subject", Title = "通知标题", IsRequired = true, Value = "您的采购申请已通过" }); RestoreContent(n); return n;
    }
    private static WorkflowNode Condition()
    {
        var n = new WorkflowNode { TypeId = "condition", Title = "金额判断", Description = "由宿主解释分支规则", Width = 170 };
        n.Ports.Add(new("in", "输入", WorkflowPortDirection.Input)); n.Ports.Add(new("yes", "满足", WorkflowPortDirection.Output)); n.Ports.Add(new("no", "不满足", WorkflowPortDirection.Output));
        n.Properties.Add(new() { Key = "threshold", Title = "金额阈值", Kind = WorkflowValueKind.Number, DataType = "number", Value = "5000" }); RestoreContent(n); return n;
    }
    private static void RestoreContent(WorkflowNode n) => n.Content = new WorkflowBusinessSummary(n.TypeId switch { "request" => "业务数据源", "approval" => "人工处理", "notification" => "数据引用", _ => "分支规则" }, "宿主按 TypeId 创建的强类型 Content", n);
    private sealed class DemoCommand(Action execute) : ICommand
    { public event EventHandler? CanExecuteChanged { add { } remove { } } public bool CanExecute(object? p) => true; public void Execute(object? p) => execute(); }
}

/// <summary>Awaitable command with native CanExecute updates and observed failures.</summary>
public sealed class WorkflowFileCommand(Func<Task> execute, Action<Exception> failed) : ICommand
{
    private bool _running;
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => !_running;
    public async void Execute(object? parameter) => await ExecuteAsync();
    public async Task ExecuteAsync()
    {
        if (_running) return;
        _running = true; CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        try { await execute(); }
        catch (Exception ex) { failed(ex); }
        finally { _running = false; CanExecuteChanged?.Invoke(this, EventArgs.Empty); }
    }
}

public sealed class WorkflowFieldEditor : WorkflowObservable
{
    private readonly WorkflowDocument _document;
    private readonly WorkflowNode _node;
    public WorkflowProperty Property { get; }
    public string Title => Property.Title + (Property.IsRequired ? " *" : "");
    public string TypeHint => Property.Kind + " · " + Property.DataType;
    public string Literal { get => Property.Value; set => _document.SetProperty(Property, value ?? "", Property.Reference); }
    public bool IsLiteral => Property.Reference == null;
    public ObservableCollection<WorkflowReferenceOption> Options { get; } = [];
    public WorkflowReferenceOption? SelectedOption
    {
        get => Options.FirstOrDefault(o => o.Reference == Property.Reference) ?? Options.FirstOrDefault();
        set { if (value != null) _document.SetProperty(Property, Property.Value, value.Reference.NodeId.Length == 0 ? null : value.Reference); }
    }
    public WorkflowFieldEditor(WorkflowDocument document, WorkflowNode node, WorkflowProperty property) { _document = document; _node = node; Property = property; Refresh(); }
    public void Refresh()
    {
        var options = _document.GetReferenceOptions(_node, Property).Prepend(new WorkflowReferenceOption("固定值", new("", "", ""))).ToList();
        if (Property.Reference is { } r && options.All(o => o.Reference != r)) options.Add(new("无效引用：" + Property.DisplayValue, r));
        if (!Options.SequenceEqual(options)) { Options.Clear(); foreach (var o in options) Options.Add(o); }
        Notify(nameof(Literal)); Notify(nameof(SelectedOption)); Notify(nameof(IsLiteral));
    }
}
