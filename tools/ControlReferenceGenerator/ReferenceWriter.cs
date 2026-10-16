using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TheOne.Documentation;

internal static class ReferenceWriter
{
    private const string Start = "<!-- generated-contracts:start -->";
    private const string End = "<!-- generated-contracts:end -->";
    internal static readonly Dictionary<string, string[]> Topics = new()
    {
        ["basic"] = ["Button", "RepeatButton", "ToggleButton", "TextBox", "CheckBox", "RadioButton", "ToggleSwitch", "ComboBox", "AutoCompleteBox", "NumericUpDown", "ButtonSpinner", "Slider", "ProgressBar", "ToolTip", "ButtonLoading"],
        ["text-display"] = ["TextBlock", "Label", "SelectableTextBlock"],
        ["icon-button"] = ["IconButton", "ButtonLoading", "ThemeButton"],
        ["split-button"] = ["SplitButton", "ToggleSplitButton"],
        ["date-time"] = ["Calendar", "CalendarDatePicker", "DatePicker", "TimePicker", "CalendarInput", "CalendarOptions", "PickerSizing", "PickerScroll"],
        ["color-picker"] = ["ColorPicker", "ColorView", "ColorSpectrum", "ColorSlider", "ColorPreviewer"],
        ["collections"] = ["ItemsControl", "ListBox", "TreeView", "TreeViewItem", "TabControl", "TabStrip", "TabFeatures", "Virtualization", "DataGrid"],
        ["collection-content"] = ["Carousel", "CarouselOptions", "RefreshContainer", "RefreshVisualizer", "RefreshOptions", "RefreshMotion"],
        ["content-feedback"] = ["TransitioningContentControl", "ContentTransitionOptions", "NotificationCard", "NotificationMotion"],
        ["layout"] = ["Menu", "MenuItem", "ContextMenu", "MenuFlyout", "Flyout", "FlyoutPresenter", "Expander", "SplitView", "GridSplitter", "ScrollViewer", "ScrollBar", "Separator"],
        ["loading"] = ["LoadingContainer", "LoadingIndicator", "ButtonLoading"],
        ["description"] = ["Description", "DescriptionItem"],
        ["form"] = ["Form", "FormItem", "StepValidation", "StepValidationError", "IStepValidator", "StepCompareAttribute"],
        ["multi-select-box"] = ["MultiSelectBox", "MultiSelectItemAdapter", "MultiSelectBoxItem", "MultiSelectTag"],
        ["nav-menu"] = ["NavMenu", "NavMenuItem", "NavMenuSearchText", "NavMenuFilterBox", "NavMenuFilterResult"],
        ["nav-menu-search"] = ["NavMenu", "NavMenuSearchText", "NavMenuFilterBox", "NavMenuFilterResult"],
        ["nav-menu-popups"] = ["NavMenu", "NavMenuItem"],
        ["nav-menu-motion"] = ["NavMenu", "NavMenuItem", "Motion", "RevealHost"],
        ["windows"] = ["OneWindow", "ThemeButton", "OneToast", "OneNotification", "OneDialog", "DialogContext", "DialogOptions", "OneDialogPresenter"],
        ["window-owners"] = ["OneWindow", "LoginWindow", "OneToast", "OneNotification", "OneDialog", "DialogContext"],
        ["login-window"] = ["LoginWindow", "LoginWindowLifecycle", "OneWindow"],
        ["message-box"] = ["MessageBox", "DialogContext", "DialogOptions", "OneDialog"],
        ["upload"] = ["Upload", "UploadFile", "UploadChangedEventArgs"],
        ["tag-verification-step"] = ["Tag", "VerificationStep", "StepField", "StepValidation", "IStepValidator", "StepValidationError", "StepCompareAttribute"],
        ["flow-step"] = ["FlowStep", "FlowNode", "FlowStepPanel", "FlowNodePanel", "FlowStepConnectors"],
        ["avatar"] = ["Avatar"], ["pagination"] = ["Pagination"], ["image-preview"] = ["ImagePreview", "ImagePreviewSurface"],
        ["info-card"] = ["InfoCard"], ["gantt-chart"] = ["GanttChart", "GanttHeaderPresenter"],
        ["relation-graph"] = ["RelationGraph", "RelationGraphNode", "RelationGraphNodePresenter"],
        ["workflow-editor"] = ["WorkflowEditor", "WorkflowCanvas", "WorkflowMiniMap"],
        ["report-design"] = ["ReportDesign"], ["report-preview"] = ["ReportPreview"],
        ["datagrid-binding"] = ["DataGrid", "DataGridAot", "DataGridColumnOptions", "DataGridSortButton", "DataGridValueAccessor", "DataGridRowMetadata", "DataGridCollectionView", "DataGridColumn"],
        ["datagrid-extensions"] = ["DataGrid", "DataGridFeatures", "DataGridColumnOptions", "DataGridFilterButton", "DataGridSortButton", "DataGridSummaryPresenter", "DataGridColumn", "DataGridCollectionView"],
        ["datagrid-tree"] = ["DataGrid", "DataGridTreeFeatures", "DataGridFeatures", "DataGridColumnOptions", "DataGridTreeCollectionView", "IDataGridTreeCollectionView", "IDataGridTreeQueryView", "DataGridTreeNodeState", "DataGridTreeChangingEventArgs"],
        ["charts"] = ["ChartBase", "CartesianChartBase", "RadialChartBase", "LineChart", "AreaChart", "BarChart", "HorizontalBarChart", "ScatterChart", "PieChart", "DoughnutChart", "GaugeChart", "StackedBarChart", "StackedHorizontalBarChart", "StackedAreaChart", "PercentStackedBarChart", "PercentStackedHorizontalBarChart", "PercentStackedAreaChart"],
        ["chart-cartesian"] = ["ChartBase", "CartesianChartBase", "LineChart", "AreaChart", "BarChart", "HorizontalBarChart", "ScatterChart", "StackedBarChart", "StackedHorizontalBarChart", "StackedAreaChart", "PercentStackedBarChart", "PercentStackedHorizontalBarChart", "PercentStackedAreaChart"],
        ["chart-radial"] = ["ChartBase", "RadialChartBase", "PieChart", "DoughnutChart", "GaugeChart"],
        ["chart-interaction"] = ["ChartBase"], ["chart-rendering"] = ["ChartBase"],
        ["theme-colors"] = ["OneTheme", "ThemeColor"], ["theme-color-picker"] = ["ThemeColorPicker", "ThemeButton", "OneTheme", "ThemeColor"],
        ["item-spacing"] = ["ListBox", "ComboBox", "TreeView", "NavMenu", "MultiSelectBox", "MenuItem", "TabControl"]
    };

    internal static int Write(string root, ContractSet data, bool check)
    {
        var outputs = new Dictionary<string, string>();
        var original = new Dictionary<(string Lang, string Topic), string>();
        foreach (var category in new[] { "controls", "theme" })
        foreach (var lang in new[] { "en", "zh-cn" })
        foreach (var file in Directory.GetFiles(Path.Combine(root, "docs", category, lang), "*.md"))
        {
            var topic = Path.GetFileNameWithoutExtension(file);
            if (!Topics.ContainsKey(topic)) continue;
            original[(lang, topic)] = Strip(File.ReadAllText(file));
        }
        foreach (var lang in new[] { "en", "zh-cn" })
        {
            var zh = lang == "zh-cn";
            var index = new StringBuilder(zh ? "# 完整公共 API 与内置模型\n\n" : "# Public API and built-in model reference\n\n");
            index.AppendLine(zh ? "由同版本源码与主题选择器生成，包含主库全部公开类型及 DataGrid fork 的新增 API；原生 Avalonia 的继承 API 不重复声明。\n" : "Generated from same-version source and theme selectors, covering every main-library public type and added DataGrid fork APIs. Inherited native Avalonia APIs are not redeclared.\n");
            index.AppendLine("[English](../en/README.md) · [简体中文](../zh-cn/README.md)\n");
            index.AppendLine(zh ? "| 类型 | 类别 | 额外属性 | 方法／构造 | 事件 |\n| --- | --- | ---: | ---: | ---: |" : "| Type | Kind | Declared properties | Methods/constructors | Events |\n| --- | --- | ---: | ---: | ---: |");
            foreach (var type in data.Types)
            {
                index.AppendLine($"| [{Code(type.Id)}]({FileName(type)}) | {type.Kind}{(type.IsControl ? zh ? "／控件" : "/control" : "")} | {type.Properties.Length} | {type.Methods.Length} | {type.Events.Length} |");
                var file = Path.Combine(root, "docs/api", lang, FileName(type));
                var text = new StringBuilder($"# {type.Name}\n\n");
                text.AppendLine($"[English](../en/{FileName(type)}) · [简体中文](../zh-cn/{FileName(type)}) · [API index](README.md)\n");
                text.AppendLine($"`{type.Id}` · {type.Kind}" + (type.Base.Length > 0 ? $" · `{type.Base}`" : "") + "\n");
                if (data.Types.FirstOrDefault(t => t.Id == type.Base) is { } parent)
                    text.AppendLine($"[{(zh ? "继承的额外成员" : "Inherited additional members")}: {parent.Name}]({FileName(parent)})\n");
                if (type.Summary.Length > 0) text.AppendLine(type.Summary + "\n");
                text.AppendLine(Source(root, file, type.Source) + "\n");
                text.AppendLine(zh ? "以下默认值为源码注册／模型初始化值，OneTheme 样式或调用方局部值可能覆盖它；只读成员由状态计算，不能双向写入。复杂对象、模板、命令与集合需遵循相应控件用法文档。\n" : "Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.\n");
                AppendProperties(text, type, type.Properties, lang, root, file, original);
                Members(text, zh ? "方法与构造" : "Methods and constructors", type.Methods.Select(m => m with { Summary = MemberDescription(type, m, lang, original) }).ToArray(), root, file, zh);
                Members(text, zh ? "事件" : "Events", type.Events.Select(m => m with { Summary = MemberDescription(type, m, lang, original) }).ToArray(), root, file, zh);
                Members(text, zh ? "枚举值" : "Enum values", type.Values, root, file, zh);
                Members(text, zh ? "公开字段" : "Public fields", type.Fields, root, file, zh);
                if (type.IsControl)
                {
                    text.AppendLine(zh ? "## 样式与密度\n" : "## Styles and density\n");
                    text.AppendLine(zh ? "尺寸、动效等所有控件通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 统一演示。`one-*` 为模板维护类；伪类由实际输入与状态驱动，不手工赋值。\n" : "See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.\n");
                    var targets = new HashSet<string> { type.Name };
                    for (var current = type; current is not null; current = data.Types.FirstOrDefault(t => t.Id == current.Base))
                        targets.Add(current.Base.Split('.').Last());
                    var ownStyles = data.Styles.Where(s => targets.Contains(s.Target) && s.Target != "Control" && !s.Internal).ToArray();
                    if (ownStyles.Length > 0)
                    {
                        text.AppendLine(zh ? "| 类名 | 选择器 | Setter／资源 |\n| --- | --- | --- |" : "| Class | Selector | Setters/resources |\n| --- | --- | --- |");
                        foreach (var style in ownStyles) text.AppendLine($"| {Code(style.Name)} | [{Code(style.Selector)}]({SourceUrl(root, file, style.Source)}) | {Cell(style.Effect)} |");
                        text.AppendLine();
                    }
                    else text.AppendLine(zh ? "没有额外公开外观类；通过公开属性和默认主题配置。\n" : "No additional public appearance classes; configure public properties and the default theme.\n");
                }
                var related = data.Types.Where(t => type.Related.Contains(t.Id)).ToArray();
                if (related.Length > 0) { text.AppendLine(zh ? "## 相关 API 与内置模型\n" : "## Related APIs and built-in models\n"); foreach (var t in related) text.AppendLine($"- [{Code(t.Id)}]({FileName(t)})"); }
                outputs[file] = text.ToString();
            }
            outputs[Path.Combine(root, "docs/api", lang, "README.md")] = index.ToString();
            foreach (var category in new[] { "controls", "theme" })
            foreach (var file in Directory.GetFiles(Path.Combine(root, "docs", category, lang), "*.md"))
            {
                var topic = Path.GetFileNameWithoutExtension(file);
                if (!Topics.TryGetValue(topic, out var names)) continue;
                var content = new StringBuilder(original[(lang, topic)].TrimEnd() + "\n\n" + Start + "\n");
                content.AppendLine(zh ? "## 额外属性、样式与完整 API\n" : "## Additional properties, styles and complete API\n");
                content.AppendLine(zh ? "本节只列出本项目增加的 Classes、属性、扩展 API／事件及关联模型，不重复 Avalonia 原生继承 API。注册默认值与主题生效值不同，显式局部值优先。PreviewHost 用可见示例对照本项目附加样式，用 Description 组件说明专属属性、API 签名与模型字段；ControlSizing、Motion 等通用配置集中在 Home。仅 Button、Input、Select 合并比较，其余控件保留独立完整示例。\n" : "This section lists only Classes, properties, extension APIs/events and related models added by this project, without repeating inherited Avalonia APIs. Registration defaults differ from themed values, and explicit local values take precedence. PreviewHost visibly compares project-added styles and uses Description components for control-specific properties, API signatures and model fields. Shared ControlSizing and Motion settings are centralized on Home. Only Button, Input and Select share comparison pages; other controls retain independent complete demos.\n");
                var owners = data.Types.Where(t => names.Contains(t.Name) && t.Name is not "ControlSizing" and not "Motion").ToArray();
                foreach (var owner in owners)
                {
                    content.AppendLine($"### {owner.Name}\n\n[API / model reference](../../api/{lang}/{FileName(owner)})\n");
                    AppendProperties(content, owner, owner.Properties, lang, root, file, original);
                }
                if (owners.Length == 0) content.AppendLine(zh ? "本页的原生控件没有同名 TheOne 包装类；本项目增加的能力来自下列样式。\n" : "These native controls have no same-name TheOne wrappers; project-added capabilities come from the styles below.\n");
                content.AppendLine(zh ? "尺寸、动效等通用配置见[统一主题说明](../../theme/zh-cn/README.md#shared-configuration)，PreviewHost 在 Home 集中演示。\n" : "See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home.\n");
                var styles = data.Styles.Where(s => names.Contains(s.Target) && s.Target != "Control").ToArray();
                content.AppendLine(zh ? "### Style 类与自动状态\n\n| 控件 | 类名 | 使用边界 | 生效 Setter／资源 |\n| --- | --- | --- | --- |" : "### Style classes and automatic state\n\n| Control | Class | Scope | Effective setters/resources |\n| --- | --- | --- | --- |");
                foreach (var style in styles) content.AppendLine($"| {style.Target} | {Code(style.Name)} | {(style.Internal ? zh ? "模板自动类；不手动绑定" : "automatic template class; do not bind manually" : zh ? "显式 Classes，可与尺寸类组合" : "explicit Classes; combine with density")} | {Cell(style.Effect)} |");
                content.AppendLine(zh ? "\n`:pointerover`、`:pressed`、`:disabled`、`:selected`、`:error` 等是由输入或状态驱动的伪类，不是手工添加的 Classes。未列出附加外观类的控件使用默认主题及其专属属性。\n" : "\n`:pointerover`, `:pressed`, `:disabled`, `:selected`, `:error` etc. are input/state pseudoclasses, not manually assigned Classes. Controls without added appearance classes use their default theme and control-specific properties.\n");
                var family = Related(data, owners, topic);
                content.AppendLine(zh ? "### API、事件与内置模型\n\n| 类型 | 类别 | 属性／枚举值 | 方法／构造 | 事件 |\n| --- | --- | ---: | ---: | ---: |" : "### APIs, events and built-in models\n\n| Type | Kind | Properties/enum values | Methods/constructors | Events |\n| --- | --- | ---: | ---: | ---: |");
                foreach (var t in family) content.AppendLine($"| [{Code(t.Id)}](../../api/{lang}/{FileName(t)}) | {t.Kind} | {t.Properties.Length + t.Values.Length} | {t.Methods.Length} | {t.Events.Length} |");
                content.AppendLine($"\n[Complete public API index](../../api/{lang}/README.md)\n{End}");
                outputs[file] = content.ToString();
            }
        }
        var previewData = data with { Types = data.Types.Select(t => t with
        {
            Properties = t.Properties.Select(p => p with { Summary = Description(t, p, "zh-cn", original) }).ToArray(),
            Methods = t.Methods.Select(m => m with { Summary = MemberDescription(t, m, "zh-cn", original) }).ToArray(),
            Events = t.Events.Select(m => m with { Summary = MemberDescription(t, m, "zh-cn", original) }).ToArray()
        }).ToArray() };
        outputs[Path.Combine(root, "design/PreviewHost/Reference/ControlContracts.json")] = JsonSerializer.Serialize(previewData, new JsonSerializerOptions { WriteIndented = true }) + "\n";
        outputs = outputs.ToDictionary(p => p.Key, p => p.Value.TrimEnd() + "\n");
        var changed = outputs.Where(p => !File.Exists(p.Key) || File.ReadAllText(p.Key) != p.Value).ToArray();
        if (check) { Console.WriteLine($"Generated reference drift: {changed.Length} files."); foreach (var p in changed.Take(10)) Console.WriteLine(Path.GetRelativePath(root, p.Key)); return changed.Length == 0 ? 0 : 1; }
        foreach (var p in changed) { Directory.CreateDirectory(Path.GetDirectoryName(p.Key)!); File.WriteAllText(p.Key, p.Value); }
        Console.WriteLine($"Updated {changed.Length} reference/documentation/catalog files.");
        return 0;
    }

    private static IEnumerable<TypeContract> Related(ContractSet data, TypeContract[] owners, string topic)
    {
        var ids = owners.Select(t => t.Id).ToHashSet();
        if (topic.StartsWith("report-")) foreach (var t in data.Types.Where(t => t.Namespace.EndsWith(".Reporting"))) ids.Add(t.Id);
        if (topic == "workflow-editor") foreach (var t in data.Types.Where(t => t.Namespace.EndsWith(".Workflow"))) ids.Add(t.Id);
        if (topic is "charts" or "chart-cartesian" or "chart-radial" or "chart-interaction" or "chart-rendering") foreach (var t in data.Types.Where(t => t.Name.StartsWith("Chart"))) ids.Add(t.Id);
        var changed = true;
        while (changed) { changed = false; foreach (var t in data.Types.Where(t => ids.Contains(t.Id)).ToArray()) foreach (var id in t.Related) if (data.Types.Any(t => t.Id == id)) changed |= ids.Add(id); }
        return data.Types.Where(t => ids.Contains(t.Id));
    }

    private static void AppendProperties(StringBuilder b, TypeContract owner, PropertyContract[] properties, string lang, string root, string output, Dictionary<(string Lang, string Topic), string> docs)
    {
        var zh = lang == "zh-cn";
        if (properties.Length == 0) { b.AppendLine(zh ? "没有在该类型上另外声明 CLR／附加属性；继承成员见其基类。\n" : "No additional CLR/attached properties are declared on this type; see its base for inherited members.\n"); return; }
        b.AppendLine(zh ? "| 属性 | 类型 | 读写／绑定／继承 | 注册或模型初值 | 说明与约束 |\n| --- | --- | --- | --- | --- |" : "| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |\n| --- | --- | --- | --- | --- |");
        foreach (var p in properties)
        {
            var description = Description(owner, p, lang, docs);
            if (p.Validation.Length > 0) description += (zh ? " 校验：" : " Validation: ") + Code(p.Validation);
            if (p.Choices.Length > 0) description += (zh ? " 可选值：" : " Values: ") + string.Join(", ", p.Choices.Select(Code));
            b.AppendLine($"| [{Code(p.Name)}]({SourceUrl(root, output, p.Source)}) | {Code(p.Type)} | {(p.Writable ? zh ? "可写" : "writable" : zh ? "只读" : "read-only")}; {p.Binding}{(p.Attached ? zh ? "; 附加属性" : "; attached" : "")}{(p.Inherits ? zh ? "; 向后代继承" : "; inherits" : "")} | {Code(p.Default)} | {Cell(description)} |");
        }
        b.AppendLine();
    }

    private static string Description(TypeContract owner, PropertyContract p, string lang, Dictionary<(string Lang, string Topic), string> docs)
    {
        var zh = lang == "zh-cn";
        foreach (var pair in Topics.Where(t => t.Value.Contains(owner.Name)))
        {
            if (!docs.TryGetValue((lang, pair.Key), out var text)) continue;
            foreach (var line in text.Split('\n').Where(l => l.TrimStart().StartsWith('|')))
            {
                var cells = line.Split('|');
                if (cells.Length >= 4 && Regex.IsMatch(cells[1], @"\b" + Regex.Escape(p.Name) + @"\b")) return string.Join("; ", cells.Skip(2).Take(cells.Length - 3)).Trim();
            }
        }
        if (p.Summary.Length > 0) return p.Summary;
        if (PropertyHelp.Describe(owner, p, zh) is { } help) return help;
        if (Descriptions.TryGetValue(p.Name, out var d)) return zh ? d.Zh : d.En;
        if (p.Type.Contains("ICommand")) return zh ? "宿主提供的命令入口；具体操作与权限由业务实现，配对的 CommandParameter 作为命令参数。" : "Host command entry; business code owns the operation/authorization and the paired CommandParameter supplies its argument.";
        if (p.Type.Contains("DataTemplate")) return zh ? "原生数据模板扩展点；保持正确 x:DataType，模板数据作用域见本控件接入章节。" : "Native data-template extension point; retain the correct x:DataType and follow the control's template data scope.";
        if (p.Type.Contains("IEnumerable") || p.Type.Contains("Collection<") || p.Type.Contains("IList") || p.Type.EndsWith("[]")) return zh ? "模型或结果集合；集合及元素类型见签名。动态业务数据需提供通知；只读集合由控件维护，不直接替换。" : "Model/result collection; the signature defines element types. Dynamic business data needs notifications; control-owned read-only collections must not be replaced.";
        if (!p.Writable) return zh ? "只读运行状态或派生结果；由对应控件／模型更新，供展示和命令条件读取，不作为双向输入。" : "Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input.";
        return zh ? $"{owner.Name} 的 {p.Name} 配置字段；值类型和初值如表，{(p.Identifier.Length > 0 ? $"通过 {p.Identifier} 参与 Avalonia 属性系统与通知" : "通过此模型的公开属性访问器读写")}。具体上下文见控件用法及链接处的声明／访问器。" : $"{p.Name} configuration on {owner.Name}; type and initial value are listed here. {(p.Identifier.Length > 0 ? $"{p.Identifier} participates in Avalonia property notifications" : "Read/write uses the model's public accessor")}. See the usage contract and linked declaration/accessor for its context.";
    }

    private static string MemberDescription(TypeContract owner, MemberContract member, string lang, Dictionary<(string Lang, string Topic), string> docs)
    {
        if (member.Summary.Length > 0) return member.Summary;
        var name = Regex.Match(member.Signature, @"\.([A-Za-z_]\w*)(?:<[^>]+>)?\(").Groups[1].Value;
        if (name.Length == 0) name = Regex.Match(member.Signature, @"\.([A-Za-z_]\w*)$").Groups[1].Value;
        if (name == owner.Name) return lang == "zh-cn" ? "创建该类型实例；构造参数及可选初值见完整签名。共享模型与资源的所有权由调用方维护。" : "Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources.";
        if (owner.Properties.FirstOrDefault(p => p.Attached && (name == "Get" + p.Name || name == "Set" + p.Name)) is { } attached)
            return (lang == "zh-cn" ? name.StartsWith("Get", StringComparison.Ordinal) ? "读取附加属性。" : "设置附加属性。" : name.StartsWith("Get", StringComparison.Ordinal) ? "Reads the attached property. " : "Sets the attached property. ") + Description(owner, attached, lang, docs);
        if (name.Length > 0)
        foreach (var topic in Topics.Where(t => t.Value.Contains(owner.Name)))
        {
            if (!docs.TryGetValue((lang, topic.Key), out var text)) continue;
            foreach (var line in text.Split('\n').Where(l => l.TrimStart().StartsWith('|')))
            {
                var cells = line.Split('|');
                if (cells.Length >= 4 && Regex.IsMatch(cells[1], @"\b" + Regex.Escape(name) + @"\b")) return string.Join("; ", cells.Skip(2).Take(cells.Length - 3)).Trim();
            }
        }
        if (MethodHelp.Describe(name, lang == "zh-cn") is { } help) return help;
        return member.Signature.StartsWith("event ", StringComparison.Ordinal)
            ? lang == "zh-cn" ? "状态／操作通知；在宿主生命周期内订阅并在释放时解除，事件参数类型见签名。" : "State/operation notification; subscribe for the host lifetime and detach on disposal. The signature defines event arguments."
            : "";
    }

    private static readonly Dictionary<string, (string Zh, string En)> Descriptions = new()
    {
        ["Id"] = ("模型稳定标识；持久化／引用使用，避免在同一文档中重复。", "Stable model identifier for persistence/references; avoid duplicates in one document."),
        ["Name"] = ("名称字段，用于业务展示或标识；与显示模板、键值字段保持一致。", "Name used for presentation/identity; keep it consistent with templates and keys."),
        ["Title"] = ("显示标题；不等于业务主键或本地化资源键。", "Display title, distinct from business identity or a localization key."),
        ["Description"] = ("描述文本；由宿主提供业务说明，未提供时沿用初值。", "Descriptive text supplied by the host; omitted values keep their initial state."),
        ["ItemsSource"] = ("绑定原始模型集合，不预建大量 Control；动态增删使用带通知的集合，并提供有限视口。", "Bind source models rather than precreated controls; use notified collections for changes and a bounded viewport."),
        ["SelectedItem"] = ("当前选择的模型对象；保留模型身份，不以显示文本替代对象。空值表示没有选择。", "Currently selected model object; retain identity rather than substituting display text. Null represents no selection."),
        ["SelectedItems"] = ("多选模型集合；元素必须来自源集合，变更与清空行为遵循控件的禁选／只读契约。", "Selected model collection; elements belong to the source and mutations follow disabled/read-only selection semantics."),
        ["Content"] = ("宿主内容或业务展示对象；具体模板作用域见内容模板契约，不把视图对象当作持久化数据。", "Host content/presentation object; follow the content-template scope and do not persist visual objects as business data."),
        ["Document"] = ("当前业务文档模型；替换时重接订阅、选择和历史的具体行为见编辑器文档。", "Current business document model; editor documentation defines subscription, selection and history behavior on replacement."),
        ["DataSource"] = ("显式数据来源；由宿主提供符合目标模型契约的数据，不能用显示模板代替数据适配。", "Explicit data source supplied by the host according to the model contract; templates do not replace data adaptation."),
        ["IsReadOnly"] = ("是否阻止编辑；展示、选择与导航是否仍可用由各控件的只读契约决定。", "Whether editing is blocked; the control's read-only contract determines remaining display, selection and navigation behavior."),
        ["IsEnabled"] = ("业务可用性；不可用项的选择、移除和命令行为遵循控件禁用规则。", "Business availability; selection, removal and commands follow the control's disabled-item rules."),
        ["IsExpanded"] = ("当前节点展开状态；需要在回收或数据重新加载后保留时，将其绑定到稳定模型。", "Current node expansion; bind to a stable model when it must survive recycling or reload."),
        ["IsSelected"] = ("当前项的选择状态；与控件选中模型保持一致，不与无关项共享状态。", "Current-item selection state, synchronized with the selected model and not shared with unrelated items."),
        ["Header"] = ("标题区内容，可为文本或模型；自定义显示由 HeaderTemplate 负责。", "Header content, as text or a model; HeaderTemplate controls custom presentation."),
        ["Footer"] = ("底部区域内容；与主体和标题分别定制。", "Footer content, customized independently of body/header."),
        ["Text"] = ("文本输入／显示值；变更通知和校验按所在模型或原生输入契约处理。", "Input/display text; notifications and validation follow the model/native editor contract."),
        ["Value"] = ("当前值；单位、范围及空值含义以所属模型和校验条件为准。", "Current value; units, range and null semantics are defined by its model and validation constraints."),
        ["Color"] = ("颜色值；作为模型配置使用，主题动态颜色优先通过 One.* 资源获取。", "Configured model color; prefer One.* resources for theme-dependent UI colors."),
        ["Background"] = ("背景画刷；主题相关背景使用 DynamicResource，避免固定颜色破坏明暗切换。", "Background brush; use DynamicResource for theme-dependent surfaces."),
        ["Foreground"] = ("前景画刷；自定义时保持正文、状态与禁用对比度。", "Foreground brush; retain text, status and disabled contrast when overriding."),
        ["ErrorMessage"] = ("当前失败／校验说明；展示错误状态，不能据此假定操作已经成功完成。", "Current failure/validation message; an error display is not evidence of successful completion."),
        ["State"] = ("模型的状态枚举；可选值及持久化含义见对应枚举和所属控件契约。", "Model state enum; see its values and owning control for persistence/behavior semantics."),
        ["Children"] = ("子级模型集合；保持父子与对象身份，避免循环和跨父级共享可变节点。", "Child model collection; retain identity/parent relationships and avoid cycles/shared mutable nodes."),
        ["Key"] = ("字段／参数键，用于关联业务值；与显示名称分开维护。", "Field/parameter key used to resolve business values, distinct from display names."),
        ["PropertyName"] = ("目标模型属性名或路径；须与具体编译绑定／注册元数据匹配。", "Target model property name/path; match the concrete compiled binding/registered metadata."),
        ["CommandParameter"] = ("传递给关联命令的业务参数；宿主负责其类型与生命周期。", "Business argument passed to the associated command; the host owns its type/lifetime."),
        ["Duration"] = ("持续时间；动效仍遵守继承的 Motion.Enabled，不能仅用时长替代关闭开关。", "Duration; motion still follows inherited Motion.Enabled rather than using duration as its only switch."),
        ["Width"] = ("宽度配置；物理单位或布局单位遵循所属模型说明，不能据此改变数据坐标含义。", "Width configuration; units follow the owning model, without changing data-coordinate meaning."),
        ["Height"] = ("高度配置；保留内容、字号及数据坐标契约，固定值可能覆盖主题密度。", "Height configuration; preserve content/text/data contracts; fixed values can override theme density."),
        ["X"] = ("模型横向坐标／数值；按所属图表、流程或报表坐标系统解释。", "Horizontal model coordinate/value in its chart, workflow or report coordinate system."),
        ["Y"] = ("模型纵向坐标／数值；按所属图表、流程或报表坐标系统解释。", "Vertical model coordinate/value in its chart, workflow or report coordinate system."),
        ["Label"] = ("显示标签文本；不作为模型身份、字段路径或序列化标识。", "Display label, not a substitute for model identity, field paths or serialized identifiers."),
        ["Items"] = ("当前项集合；具体元素与增删权限见类型签名和控件文档。", "Current item collection; element types and mutation rights follow the signature/control contract."),
        ["Source"] = ("源对象／地址；按控件支持的类型提供，外部资源的所有权与加载限制见用法章节。", "Source object/address of a supported type; usage documentation defines resource ownership/loading limits."),
    };

    private static void Members(StringBuilder b, string heading, MemberContract[] members, string root, string file, bool zh)
    {
        if (members.Length == 0) return;
        b.AppendLine($"## {heading}\n");
        b.AppendLine(zh ? "| 完整签名／值 | 说明 |\n| --- | --- |" : "| Complete signature/value | Contract |\n| --- | --- |");
        var fallback = heading is "枚举值" or "Enum values"
            ? zh ? "枚举成员及其底层数值；持久化使用同版本定义，不自行重排数值。" : "Enum member and underlying value; persist against the matching version rather than renumbering values."
            : heading is "公开字段" or "Public fields"
                ? zh ? "公开标识符／常量；Property、Event 后缀字段分别用于原生 Avalonia 属性系统和路由事件，对应的 CLR 属性／事件见上文。" : "Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above."
                : zh ? "签名列明参数类型、可选初值及返回类型；具体操作与异常边界见所属控件用法和链接处实现。" : "The signature specifies parameter types, optional defaults and return types; see the owning usage contract and linked implementation for operation/error boundaries.";
        foreach (var m in members) b.AppendLine($"| [{Code(m.Signature)}]({SourceUrl(root, file, m.Source)}) | {Cell(m.Summary.Length > 0 ? m.Summary : fallback)} |");
        b.AppendLine();
    }
    internal static string FileName(TypeContract t) => Regex.Replace(t.Id, @"[^A-Za-z0-9_.-]", "_") + ".md";
    private static string Strip(string s) { var i = s.IndexOf(Start, StringComparison.Ordinal); return i < 0 ? s : s[..i].TrimEnd() + "\n"; }
    private static string Code(string s) => "`" + Cell(s).Replace('`', '\'') + "`";
    private static string Cell(string s) => Regex.Replace(s.Replace("|", "&#124;"), @"\s+", " ").Trim();
    private static string Source(string root, string file, string source) => $"[Source / 声明]({SourceUrl(root, file, source)})";
    private static string SourceUrl(string root, string file, string source)
    {
        var split = source.LastIndexOf(':'); if (split < 0) return "README.md";
        return Path.GetRelativePath(Path.GetDirectoryName(file)!, Path.Combine(root, source[..split])).Replace('\\', '/') + "#L" + source[(split + 1)..];
    }
}
