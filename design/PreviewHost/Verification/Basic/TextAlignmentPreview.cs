using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static class TextAlignmentPreview
{
    public static void Verify(Action<bool, string> check)
    {
        foreach (var theme in new[] { ThemeVariant.Light, ThemeVariant.Dark })
        foreach (var size in Enum.GetValues<ControlSize>())
        {
            var samples = new Control[]
            {
                new Button { Content = "继续操作", Width = 200 },
                new ToggleButton { Content = "切换 Toggle", Width = 200 },
                new RepeatButton { Content = "持续 Repeat", Width = 200 },
                new Button { Content = new TextBlock { Text = "文字内容 Text" }, Width = 200 },
                new TextBox { Text = "输入内容 Text", Width = 240 },
                new TextBox { PlaceholderText = "输入提示 Placeholder", Width = 240 },
                new TextBox { Text = "password", PasswordChar = '●', Width = 240 },
                new TextBox { Text = "加高输入 Text", Height = 60, Width = 240 },
                new NumericUpDown { Value = 42, Width = 240 },
                new AutoCompleteBox { Text = "自动完成 Text", Width = 240 },
                new ComboBox { ItemsSource = new[] { "下拉选项 Text" }, SelectedIndex = 0, Width = 240 },
                new CalendarDatePicker { SelectedDate = new DateTime(2026, 10, 13), Width = 240 },
                new ComboBoxItem { Content = "下拉条目 Text", Width = 240 },
                new ListBoxItem { Content = "列表条目 Text", Width = 240 },
                new CheckBox { Content = "勾选 Check", Width = 240 },
                new RadioButton { Content = "单选 Radio", Width = 240 },
                new ToggleSwitch { OnContent = "启用 On", IsChecked = true, Width = 240 },
                new SplitButton { Content = "拆分按钮 Split", Width = 200 },
                new ToggleSplitButton { Content = "拆分切换 Toggle", Width = 200 }
            };
            var multiline = new TextBox { Text = "第一行\n第二行", AcceptsReturn = true, Height = 90, Width = 240 };
            var aligned = new TextBox
                { Text = "自定义顶对齐", VerticalContentAlignment = VerticalAlignment.Top, Height = 60, Width = 240 };
            var alignedButton = new Button
            {
                Content = "自定义左上对齐", HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Top, Height = 60, Width = 240
            };
            var panel = new StackPanel { Spacing = 8 };
            foreach (var sample in samples) panel.Children.Add(sample);
            panel.Children.Add(multiline);
            panel.Children.Add(aligned);
            panel.Children.Add(alignedButton);
            var window = new Window { Width = 400, Height = 1400, RequestedThemeVariant = theme, Content = panel };
            ControlSizing.SetSize(window, size);
            Motion.SetEnabled(window, false);
            window.Show();
            ControlPreview.Flush();
            foreach (var sample in samples)
            {
                var label =
                    $"TextAlignment/{theme.Key}/{size}/{sample.GetType().Name}/{Array.IndexOf(samples, sample)}";
                var presenter = sample.GetVisualDescendants().OfType<TextPresenter>().FirstOrDefault();
                var text = sample.GetVisualDescendants().OfType<TextBlock>().FirstOrDefault(block =>
                    block.IsEffectivelyVisible && !string.IsNullOrEmpty(block.Text));
                var visual = (Control?)presenter ??
                             text ?? throw new InvalidOperationException($"{label}: missing native text visual");
                var height = presenter?.TextLayout.Height ?? text!.TextLayout.Height;
                if (sample is TextBox { Text: null or "" })
                {
                    visual = text!;
                    height = text!.TextLayout.Height;
                }

                var origin = visual.TranslatePoint(default, sample)!.Value;
                var centre = origin.Y + height / 2;
                check(Math.Abs(centre - sample.Bounds.Height / 2) <= 1,
                    $"{label}: actual text is vertically centered (text={centre:0.##}, control={sample.Bounds.Height / 2:0.##})");
                check(origin.Y >= 0 && origin.Y + height <= sample.Bounds.Height + 1,
                    $"{label}: text is fully visible within its control");
                if (sample.StyleKey == typeof(Button) || sample.StyleKey == typeof(ToggleButton) ||
                    sample.StyleKey == typeof(RepeatButton))
                {
                    var width = text!.TextLayout.WidthIncludingTrailingWhitespace;
                    check(Math.Abs(origin.X + width / 2 - sample.Bounds.Width / 2) <= 1,
                        $"{label}: button text is horizontally centered");
                }
            }

            var multilineText = multiline.GetVisualDescendants().OfType<TextPresenter>().Single();
            var topText = aligned.GetVisualDescendants().OfType<TextPresenter>().Single();
            check(
                multilineText.TextLayout.TextLines.Count == 2 &&
                multilineText.TranslatePoint(default, multiline)!.Value.Y < 15,
                $"TextAlignment/{theme.Key}/{size}: multiline input starts at the top and keeps both lines");
            check(topText.TranslatePoint(default, aligned)!.Value.Y < 15,
                $"TextAlignment/{theme.Key}/{size}: explicit consumer top alignment remains effective");
            var buttonText = alignedButton.GetVisualDescendants().OfType<TextBlock>().Single();
            var buttonOrigin = buttonText.TranslatePoint(default, alignedButton)!.Value;
            check(buttonOrigin.X < 20 && buttonOrigin.Y < 15,
                $"TextAlignment/{theme.Key}/{size}: explicit consumer left/top button alignment remains effective");
            var editor = (TextBox)samples[4];
            editor.Focus();
            editor.SelectAll();
            window.KeyTextInput("编辑内容 Text");
            ControlPreview.Flush();
            var editorText = editor.GetVisualDescendants().OfType<TextPresenter>().Single();
            check(
                editor.Text == "编辑内容 Text" && Math.Abs(editorText.TranslatePoint(default, editor)!.Value.Y +
                    editorText.TextLayout.Height / 2 - editor.Bounds.Height / 2) <= 1,
                $"TextAlignment/{theme.Key}/{size}: native keyboard editing and caret scrolling retain centered text");
            ControlSizing.SetSize(window, size == ControlSize.Small ? ControlSize.Large : ControlSize.Small);
            ControlPreview.Flush();
            check(
                Math.Abs(editorText.TranslatePoint(default, editor)!.Value.Y + editorText.TextLayout.Height / 2 -
                         editor.Bounds.Height / 2) <= 1,
                $"TextAlignment/{theme.Key}/{size}: live inherited density changes retain centered text");
            window.Close();
            ControlPreview.Flush();
        }
    }
}