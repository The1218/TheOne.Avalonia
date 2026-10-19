namespace TheOne.DesignPreview;

public partial class IconButtonPage : BasicControlPage
{
    public IconButtonPage() : base(new IconButtonPageViewModel())
    {
        InitializeComponent();
        ButtonFamilyExamples.Populate(CombinationsPanel, "IconButton",
            () => new TheOne.Avalonia.Controls.IconButton
            {
                Icon = global::Avalonia.Media.Geometry.Parse("M6,1 L9,1 L9,6 L14,6 L14,9 L9,9 L9,14 L6,14 L6,9 L1,9 L1,6 L6,6 Z")
            }, Model.InvokeCommand);
    }
}
