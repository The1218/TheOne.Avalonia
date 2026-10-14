namespace TheOne.Avalonia.Controls;

/// <summary>Immutable settings for a modal dialog inside OneWindow.</summary>
public sealed record DialogOptions
{
    public global::TheOne.Avalonia.Styling.ControlSize? Size { get; init; }
    public bool? MotionEnabled { get; init; }
    public string? Title { get; init; }
    public bool ShowTitle { get; init; } = true;
    public bool ShowCloseButton { get; init; } = true;
    public bool CloseOnOverlayClick { get; init; }
    public bool CloseOnEscape { get; init; } = true;
    public bool ShowButtons { get; init; }
    public OneDialogButtons Buttons { get; init; } = OneDialogButtons.OkCancel;
}
