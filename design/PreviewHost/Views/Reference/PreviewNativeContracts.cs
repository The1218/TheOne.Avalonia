namespace TheOne.DesignPreview;

// Native inherited contracts complement the generated TheOne-only additions.
internal static class PreviewNativeContracts
{
    internal static IEnumerable<(string Name, string Detail)> Properties(string name)
    {
        if (Reference.ContractCatalog.Find(name)?.IsStatic == true)
        {
            yield return ("静态服务", "通过 API 方法调用；owner 明确指定承载窗口。服务本身不提供控件布局、选中或尺寸属性。");
            yield break;
        }
        yield return ("Classes", "外观类名，多个类用空格分隔；本页 Style 区列出实际支持的公开类。small / normal / large 可局部覆盖全局尺寸。");
        yield return ("IsEnabled / IsVisible", "bool · 默认 true。控制可交互性与显示状态；禁用态由主题自动处理。");
        yield return ("Margin / Width / Height", "外边距与可选尺寸；Width / Height 默认 Auto。示例按内容自然增长，仅虚拟列表和画布保留有限视口。");
        yield return ("HorizontalAlignment / VerticalAlignment", "布局对齐方式；默认 Stretch。配合 MinWidth / MaxWidth 与容器布局控制自适应。");
        foreach (var row in Specific(name)) yield return row;
    }
    private static IEnumerable<(string Name, string Detail)> Specific(string name) => name switch
    {
        "Description" => Rows(("Items / ItemsSource / ItemTemplate", "添加 DescriptionItem，或绑定 DescriptionItem 集合。普通数据生成内容行，ItemTemplate 展示其内容；标签由行的 Header 明确配置。")),
        "DescriptionItem" => Rows(("Header / HeaderTemplate", "左列标签及模板；窄宽度下自动位于内容上方。"), ("Content / ContentTemplate", "右列说明或自定义控件；文本自动换行，内容决定高度。")),
        "Button" or "RepeatButton" or "ToggleButton" or "IconButton" or "SplitButton" or "ToggleSplitButton" => Rows(("Content", "显示文本、图标或自定义内容。"), ("Command / CommandParameter", "ICommand 与参数，保留原生命令 CanExecute 与键盘交互。")),
        "TextBox" => Rows(("Text", "string? · 默认 null · TwoWay。支持编译绑定及原生数据校验。"), ("PlaceholderText / IsReadOnly / AcceptsReturn", "占位提示、只读和多行输入；PasswordChar 用于密码显示。")),
        "NumericUpDown" => Rows(("Value / Minimum / Maximum / Increment", "decimal? 值、范围和步长；Value 默认双向绑定。"), ("FormatString / IsReadOnly", "格式化数值显示和只读模式。")),
        "AutoCompleteBox" => Rows(("Text / SelectedItem / ItemsSource", "输入文本、选中项和候选集合。"), ("FilterMode / MinimumPrefixLength", "匹配模式及触发筛选所需的最少字符数。")),
        "ComboBox" or "ListBox" or "TabControl" or "TabStrip" or "Carousel" => Rows(("Items / ItemsSource / ItemTemplate", "集合与模板；模板使用带 x:DataType 的编译绑定。"), ("SelectedItem / SelectedIndex", "同步当前选择的数据项或索引。")),
        "TreeView" => Rows(("Items / ItemsSource / ItemTemplate", "分层集合与 TreeDataTemplate；模板使用带 x:DataType 的编译绑定。"), ("SelectedItem / SelectedItems / SelectionMode", "当前选择、选中集合与选择模式；TreeView 不提供索引式选择。")),
        "ItemsControl" => Rows(("Items / ItemsSource / ItemTemplate", "集合与模板；模板使用带 x:DataType 的编译绑定。ItemsControl 自身无选择状态。")),
        "CheckBox" or "RadioButton" or "ToggleSwitch" => Rows(("Content / IsChecked", "显示内容与选中状态；IsChecked 为 bool?，默认 false。")),
        "Slider" or "ProgressBar" => Rows(("Minimum / Maximum / Value", "double · 默认范围 0–100。Slider 可编辑，ProgressBar 展示进度。")),
        "Calendar" or "CalendarDatePicker" or "DatePicker" => Rows(("SelectedDate", "可绑定的选中日期，支持未选择值。")),
        "TimePicker" => Rows(("SelectedTime", "可绑定的选中时间，支持未选择值。")),
        "Label" => Rows(("Content / Target", "显示内容及关联的焦点目标。")),
        "SelectableTextBlock" => Rows(("Text", "显示文本，支持原生文本选取与复制。")),
        _ => Array.Empty<(string, string)>()
    };
    internal static IEnumerable<(string Name, string Detail)> Api(string name) => name switch
    {
        "Button" or "RepeatButton" or "ToggleButton" or "IconButton" or "SplitButton" or "ToggleSplitButton" => Rows(("Click / Command", "点击、Enter、Space 沿用原生按钮交互；优先绑定 Command，也可订阅 Click。")),
        "TextBox" => Rows(("TextChanged / TextChanging", "原生输入事件；业务数据优先通过 Text 双向绑定同步。")),
        "ComboBox" or "ListBox" or "TreeView" or "TabControl" => Rows(("SelectionChanged", "当前选择变化事件；选中项优先使用绑定。")),
        _ => Array.Empty<(string, string)>()
    };
    internal static string Model(string name) => name is "ItemsControl" or "ListBox" or "ComboBox" or "TreeView" or "TabControl" or "AutoCompleteBox"
        ? "无额外内置模型。ItemsSource 接受业务集合，ItemTemplate 使用业务类型的编译绑定。"
        : "无额外内置模型；使用原生属性、命令与事件。";
    private static (string Name, string Detail)[] Rows(params (string Name, string Detail)[] rows) => rows;
}
