using global::Avalonia.Interactivity;

namespace TheOne.Avalonia.Controls;

public enum PaginationLayout { Standard, Compact }

/// <summary>A bounded navigation entry. Page zero represents a non-interactive ellipsis.</summary>
public sealed record PaginationPageItem(long Page, bool IsCurrent)
{
    public bool IsPage => Page > 0;
    public bool IsEllipsis => Page == 0;
    public string Text => IsPage ? Page.ToString(System.Globalization.CultureInfo.CurrentCulture) : "…";
    public string AccessibleName => IsCurrent ? $"第 {Text} 页，当前页" : $"第 {Text} 页";
}

public sealed record PaginationPageSize(int Value, string Text);

/// <summary>One notification after page and size have both been normalized.</summary>
public sealed class PaginationPageChangedEventArgs(RoutedEvent routedEvent, long oldPage, long page, int oldPageSize, int pageSize) : RoutedEventArgs(routedEvent)
{
    public long OldPage { get; } = oldPage;
    public long CurrentPage { get; } = page;
    public int OldPageSize { get; } = oldPageSize;
    public int PageSize { get; } = pageSize;
}
