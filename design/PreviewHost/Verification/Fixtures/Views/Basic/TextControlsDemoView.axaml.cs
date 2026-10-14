using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace TheOne.DesignPreview;

public partial class TextControlsDemoView : UserControl
{
    public string SampleText => "Theme makes readable interfaces. Select a word, a line, or the whole paragraph.\n拖动鼠标选择这段文字；双击可选词，三击可全选。复制快捷键遵循当前操作系统。\n文本按可用宽度自动换行，切换字号与主题会保留仍有效的选区。";
    public int CopyCount { get; private set; }
    public string LastCopiedText { get; private set; } = "";
    public TextControlsDemoView()
    {
        DataContext = this;
        InitializeComponent();
        SelectionSample.PropertyChanged += (_, args) =>
        {
            if (args.Property == SelectableTextBlock.SelectionStartProperty || args.Property == SelectableTextBlock.SelectionEndProperty || args.Property == SelectableTextBlock.SelectedTextProperty) UpdateSelectionMetrics();
        };
        SelectionSample.CopyingToClipboard += (_, _) =>
        {
            LastCopiedText = SelectionSample.SelectedText;
            TextStatus.Text = $"复制请求 {++CopyCount} 次 · {LastCopiedText.Length} 个字符";
        };
        WrapText.IsCheckedChanged += (_, _) => SelectionSample.TextWrapping = WrapText.IsChecked == true ? TextWrapping.Wrap : TextWrapping.NoWrap;
        AttachedToVisualTree += (_, _) => UpdateSelectionMetrics();
    }
    public void UpdateSelectionMetrics()
    {
        SelectionMetrics.Text = $"选区 {SelectionSample.SelectionStart} → {SelectionSample.SelectionEnd} · {SelectionSample.SelectedText.Length} 个字符 · {(SelectionSample.CanCopy ? "可以复制" : "尚未选中")}";
        SelectionValue.Text = SelectionSample.CanCopy ? SelectionSample.SelectedText : "拖动文本，选区内容会显示在这里。";
    }
    private void OnTargetFocused(object? sender, FocusChangedEventArgs args) => TextStatus.Text = sender == NameInput ? "标签目标：工作区名称已聚焦" : "标签目标：工作区角色已聚焦";
    private void OnSelectAll(object? sender, RoutedEventArgs args)
    {
        if (!SelectionSample.IsEffectivelyEnabled) return;
        SelectionSample.Focus(); SelectionSample.SelectAll();
    }
    private void OnCopy(object? sender, RoutedEventArgs args) { if (SelectionSample.IsEffectivelyEnabled) SelectionSample.Copy(); }
    private void OnClear(object? sender, RoutedEventArgs args) => SelectionSample.ClearSelection();
    private void OnReset(object? sender, RoutedEventArgs args)
    {
        TextDemoEnabled.IsChecked = WrapText.IsChecked = true;
        NameInput.Text = "TheOne workspace"; RoleInput.SelectedIndex = 0;
        SelectionSample.ClearSelection(); RichSample.ClearSelection();
        CopyCount = 0; LastCopiedText = "";
        TextStatus.Text = "已重置标签与文本交互";
        UpdateSelectionMetrics();
    }
}
