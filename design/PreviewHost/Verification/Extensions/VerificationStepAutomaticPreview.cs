using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Styling;
using Avalonia.VisualTree;
using TheOne.Avalonia.Controls;
using TheOne.Avalonia.Styling;

namespace TheOne.DesignPreview;

internal static partial class TagVerificationStepPreview
{
    private static void VerifyAutoForms(Action<bool, string> check)
    {
        var demo = new VerificationStepDemoView();
        var window = Open(demo, ThemeVariant.Light, ControlSize.Normal);
        check(
            demo.DataTemplates.Count == 0 && demo.Model.Steps.All(item => item.Validator is null) &&
            Part<ListBox>(demo.Sample, "PART_FormFields").ItemCount == 3,
            "VerificationStep/automatic: ItemsSource alone supplies the form and validation without consumer templates or validators");
        check(Part<TextBox>(demo.Sample, "Name").PlaceholderText == "2–24 个字符",
            "VerificationStep/automatic: Display metadata configures generated labels and prompts");
        var first = demo.Model.Steps[0];
        var replacement = new StepAccountModel();
        first.Content = replacement;
        Flush();
        check(
            ReferenceEquals(demo.Sample.CurrentContent, replacement) && !Next(demo.Sample) &&
            demo.Sample.ValidationErrors.Count == 3,
            "VerificationStep/automatic: replacing the typed model rebuilds its rules and form");
        var rejected = false;
        try
        {
            first.Content = new StepProfileModel();
        }
        catch (ArgumentException)
        {
            rejected = true;
        }

        check(rejected && ReferenceEquals(first.Content, replacement),
            "VerificationStep/automatic: incompatible model types cannot silently disable attribute validation");
        rejected = false;
        try
        {
            first.Content = null;
        }
        catch (ArgumentNullException)
        {
            rejected = true;
        }

        check(rejected, "VerificationStep/automatic: a missing typed model is rejected");
        demo.Sample.ContentTemplate =
            new FuncDataTemplate<StepAccountModel>((_, _) => new TextBlock { Text = "Custom step content" });
        Flush();
        check(
            !demo.Sample.GetVisualDescendants().OfType<ListBox>().Any(list => list.Name == "PART_FormFields") &&
            !Next(demo.Sample) && demo.Sample.ValidationErrors.Count == 3,
            "VerificationStep/automatic: explicit content templates override the default editor while retaining automatic attribute validation");
        window.Close();
        Flush();

        var options = new AutomaticOptions();
        var step = new VerificationStep { ItemsSource = new[] { new StepItem<AutomaticOptions>(options) } };
        window = Open(step, ThemeVariant.Dark, ControlSize.Large);
        check(
            Part<ListBox>(step, "PART_FormFields").ItemCount == 3 &&
            Part<TextBox>(step, "Secret").PasswordChar == '●' && Part<TextBox>(step, "Locked").IsReadOnly,
            "VerificationStep/automatic: Display, DataType and Editable stay library-owned and hidden properties create no input");
        Part<TextBox>(step, "Secret").Text = "secret";
        Flush();
        check(options.Secret == "secret" && Next(step),
            "VerificationStep/automatic: generated compiled bindings write directly to a plain model without a validation interface");
        window.Close();
        Flush();

        var model = new ManyFormFields { Field00 = 28 };
        step = new VerificationStep { Height = 380, ItemsSource = new[] { new StepItem<ManyFormFields>(model) } };
        window = Open(step, ThemeVariant.Light, ControlSize.Normal, 760, 540);
        var form = Part<ListBox>(step, "PART_FormFields");
        check(form.GetRealizedContainers().Count() is > 0 and < 20 && form.ItemCount == 64,
            "VerificationStep/automatic: generated properties use a finite viewport with actual container virtualization");
        Part<TextBox>(step, "Field00").Text = "invalid draft";
        Flush();
        check(model.Field00 == 28, "VerificationStep/automatic: invalid numeric text keeps the last valid model value");
        form.ScrollIntoView(63);
        Flush();
        check(
            form.ContainerFromIndex(63) is not null && form.ContainerFromIndex(0) is null &&
            form.GetRealizedContainers().Count() < 20,
            "VerificationStep/automatic: far generated inputs recycle actual field containers");
        check(
            !Next(step) && step.ValidationErrors.Any(error => error.MemberName == "Field00") && !step.HasUnmappedErrors,
            "VerificationStep/automatic: offscreen invalid drafts still block completion and remain assigned to their own field");
        Flush();
        form.ScrollIntoView(0);
        Flush();
        check(Part<TextBox>(step, "Field00").Text == "invalid draft",
            "VerificationStep/automatic: recycled editors restore invalid user drafts");
        Part<TextBox>(step, "Field00").Text = "29";
        Flush();
        form.ScrollIntoView(63);
        Flush();
        check(model.Field00 == 29 && Next(step),
            "VerificationStep/automatic: correcting a recycled input clears its conversion error and allows completion");
        step.Reset();
        Flush();
        form = Part<ListBox>(step, "PART_FormFields");
        form.ScrollIntoView(0);
        Flush();
        Part<TextBox>(step, "Field00").Text = "another invalid draft";
        Flush();
        form.ScrollIntoView(63);
        Flush();
        model.Field00 = 42;
        Flush();
        check(Next(step), "VerificationStep/automatic: model notifications clear superseded offscreen drafts");
        window.Close();
        Flush();
    }

    public sealed class AutomaticOptions
    {
        [Display(Order = 0, Prompt = "请输入密码")]
        [DataType(DataType.Password)]
        [Required]
        public string Secret { get; set; } = "";

        [Editable(false)] public string Locked { get; set; } = "Read only";
        public bool Enabled { get; set; }
        [Display(AutoGenerateField = false)] public string Hidden { get; set; } = "hidden";
    }

    public sealed class ManyFormFields : INotifyPropertyChanged
    {
        private readonly int[] _values = new int[64];

        [Range(0, 120)]
        public int Field00
        {
            get => _values[0];
            set => Set(0, value);
        }

        public int Field01
        {
            get => _values[1];
            set => Set(1, value);
        }

        public int Field02
        {
            get => _values[2];
            set => Set(2, value);
        }

        public int Field03
        {
            get => _values[3];
            set => Set(3, value);
        }

        public int Field04
        {
            get => _values[4];
            set => Set(4, value);
        }

        public int Field05
        {
            get => _values[5];
            set => Set(5, value);
        }

        public int Field06
        {
            get => _values[6];
            set => Set(6, value);
        }

        public int Field07
        {
            get => _values[7];
            set => Set(7, value);
        }

        public int Field08
        {
            get => _values[8];
            set => Set(8, value);
        }

        public int Field09
        {
            get => _values[9];
            set => Set(9, value);
        }

        public int Field10
        {
            get => _values[10];
            set => Set(10, value);
        }

        public int Field11
        {
            get => _values[11];
            set => Set(11, value);
        }

        public int Field12
        {
            get => _values[12];
            set => Set(12, value);
        }

        public int Field13
        {
            get => _values[13];
            set => Set(13, value);
        }

        public int Field14
        {
            get => _values[14];
            set => Set(14, value);
        }

        public int Field15
        {
            get => _values[15];
            set => Set(15, value);
        }

        public int Field16
        {
            get => _values[16];
            set => Set(16, value);
        }

        public int Field17
        {
            get => _values[17];
            set => Set(17, value);
        }

        public int Field18
        {
            get => _values[18];
            set => Set(18, value);
        }

        public int Field19
        {
            get => _values[19];
            set => Set(19, value);
        }

        public int Field20
        {
            get => _values[20];
            set => Set(20, value);
        }

        public int Field21
        {
            get => _values[21];
            set => Set(21, value);
        }

        public int Field22
        {
            get => _values[22];
            set => Set(22, value);
        }

        public int Field23
        {
            get => _values[23];
            set => Set(23, value);
        }

        public int Field24
        {
            get => _values[24];
            set => Set(24, value);
        }

        public int Field25
        {
            get => _values[25];
            set => Set(25, value);
        }

        public int Field26
        {
            get => _values[26];
            set => Set(26, value);
        }

        public int Field27
        {
            get => _values[27];
            set => Set(27, value);
        }

        public int Field28
        {
            get => _values[28];
            set => Set(28, value);
        }

        public int Field29
        {
            get => _values[29];
            set => Set(29, value);
        }

        public int Field30
        {
            get => _values[30];
            set => Set(30, value);
        }

        public int Field31
        {
            get => _values[31];
            set => Set(31, value);
        }

        public int Field32
        {
            get => _values[32];
            set => Set(32, value);
        }

        public int Field33
        {
            get => _values[33];
            set => Set(33, value);
        }

        public int Field34
        {
            get => _values[34];
            set => Set(34, value);
        }

        public int Field35
        {
            get => _values[35];
            set => Set(35, value);
        }

        public int Field36
        {
            get => _values[36];
            set => Set(36, value);
        }

        public int Field37
        {
            get => _values[37];
            set => Set(37, value);
        }

        public int Field38
        {
            get => _values[38];
            set => Set(38, value);
        }

        public int Field39
        {
            get => _values[39];
            set => Set(39, value);
        }

        public int Field40
        {
            get => _values[40];
            set => Set(40, value);
        }

        public int Field41
        {
            get => _values[41];
            set => Set(41, value);
        }

        public int Field42
        {
            get => _values[42];
            set => Set(42, value);
        }

        public int Field43
        {
            get => _values[43];
            set => Set(43, value);
        }

        public int Field44
        {
            get => _values[44];
            set => Set(44, value);
        }

        public int Field45
        {
            get => _values[45];
            set => Set(45, value);
        }

        public int Field46
        {
            get => _values[46];
            set => Set(46, value);
        }

        public int Field47
        {
            get => _values[47];
            set => Set(47, value);
        }

        public int Field48
        {
            get => _values[48];
            set => Set(48, value);
        }

        public int Field49
        {
            get => _values[49];
            set => Set(49, value);
        }

        public int Field50
        {
            get => _values[50];
            set => Set(50, value);
        }

        public int Field51
        {
            get => _values[51];
            set => Set(51, value);
        }

        public int Field52
        {
            get => _values[52];
            set => Set(52, value);
        }

        public int Field53
        {
            get => _values[53];
            set => Set(53, value);
        }

        public int Field54
        {
            get => _values[54];
            set => Set(54, value);
        }

        public int Field55
        {
            get => _values[55];
            set => Set(55, value);
        }

        public int Field56
        {
            get => _values[56];
            set => Set(56, value);
        }

        public int Field57
        {
            get => _values[57];
            set => Set(57, value);
        }

        public int Field58
        {
            get => _values[58];
            set => Set(58, value);
        }

        public int Field59
        {
            get => _values[59];
            set => Set(59, value);
        }

        public int Field60
        {
            get => _values[60];
            set => Set(60, value);
        }

        public int Field61
        {
            get => _values[61];
            set => Set(61, value);
        }

        public int Field62
        {
            get => _values[62];
            set => Set(62, value);
        }

        public int Field63
        {
            get => _values[63];
            set => Set(63, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Set(int index, int value, [CallerMemberName] string? name = null)
        {
            if (_values[index] == value) return;
            _values[index] = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}