namespace TheOne.DesignPreview;

public partial class RepeatButtonPage : BasicControlPage
{
    public RepeatButtonPage() : base(new RepeatButtonPageViewModel())
    {
        InitializeComponent();
        ButtonFamilyExamples.Populate(CombinationsPanel, "RepeatButton",
            () => new global::Avalonia.Controls.RepeatButton { Delay = 200, Interval = 60 }, Model.InvokeCommand);
    }
}
