namespace TheOne.DesignPreview;

public partial class ToggleButtonPage : BasicControlPage
{
    public ToggleButtonPage() : base(new ToggleButtonPageViewModel())
    {
        InitializeComponent();
        ButtonFamilyExamples.Populate(CombinationsPanel, "ToggleButton",
            () => new global::Avalonia.Controls.Primitives.ToggleButton(), Model.InvokeCommand);
    }
}
