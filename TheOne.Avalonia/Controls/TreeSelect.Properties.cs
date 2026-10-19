using System.Collections;
using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Data;

namespace TheOne.Avalonia.Controls;

/// <summary>Parent/child check behavior in multiple selection mode.</summary>
public enum TreeSelectCheckMode { Independent, Cascade }

public partial class TreeSelect
{
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<TreeSelect, IEnumerable?>(nameof(ItemsSource), null);
    public IEnumerable? ItemsSource { get => GetValue(ItemsSourceProperty); set => SetValue(ItemsSourceProperty, value); }

    public static readonly StyledProperty<IList?> SelectedKeysProperty =
        AvaloniaProperty.Register<TreeSelect, IList?>(nameof(SelectedKeys), null, defaultBindingMode: BindingMode.TwoWay);
    public IList? SelectedKeys { get => GetValue(SelectedKeysProperty); set => SetValue(SelectedKeysProperty, value); }

    public static readonly StyledProperty<object?> SelectedKeyProperty =
        AvaloniaProperty.Register<TreeSelect, object?>(nameof(SelectedKey), null, defaultBindingMode: BindingMode.TwoWay);
    public object? SelectedKey { get => GetValue(SelectedKeyProperty); set => SetValue(SelectedKeyProperty, value); }

    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<TreeSelect, object?>(nameof(SelectedItem), null, defaultBindingMode: BindingMode.TwoWay);
    public object? SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }

    public static readonly StyledProperty<bool> IsMultiSelectProperty =
        AvaloniaProperty.Register<TreeSelect, bool>(nameof(IsMultiSelect), false);
    public bool IsMultiSelect { get => GetValue(IsMultiSelectProperty); set => SetValue(IsMultiSelectProperty, value); }

    public static readonly StyledProperty<TreeSelectCheckMode> CheckModeProperty =
        AvaloniaProperty.Register<TreeSelect, TreeSelectCheckMode>(nameof(CheckMode), TreeSelectCheckMode.Independent, validate: value => Enum.IsDefined(value));
    public TreeSelectCheckMode CheckMode { get => GetValue(CheckModeProperty); set => SetValue(CheckModeProperty, value); }

    public static readonly StyledProperty<TreeSelectItemAdapter?> ItemAdapterProperty =
        AvaloniaProperty.Register<TreeSelect, TreeSelectItemAdapter?>(nameof(ItemAdapter), null);
    public TreeSelectItemAdapter? ItemAdapter { get => GetValue(ItemAdapterProperty); set => SetValue(ItemAdapterProperty, value); }

    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        AvaloniaProperty.Register<TreeSelect, IDataTemplate?>(nameof(ItemTemplate), null);
    public IDataTemplate? ItemTemplate { get => GetValue(ItemTemplateProperty); set => SetValue(ItemTemplateProperty, value); }

    public static readonly StyledProperty<IDataTemplate?> SelectedItemTemplateProperty =
        AvaloniaProperty.Register<TreeSelect, IDataTemplate?>(nameof(SelectedItemTemplate), null);
    public IDataTemplate? SelectedItemTemplate { get => GetValue(SelectedItemTemplateProperty); set => SetValue(SelectedItemTemplateProperty, value); }

    public static readonly StyledProperty<string> SearchTextProperty =
        AvaloniaProperty.Register<TreeSelect, string>(nameof(SearchText), "", defaultBindingMode: BindingMode.TwoWay);
    public string SearchText { get => GetValue(SearchTextProperty); set => SetValue(SearchTextProperty, value); }

    public static readonly StyledProperty<string> PlaceholderTextProperty =
        AvaloniaProperty.Register<TreeSelect, string>(nameof(PlaceholderText), "请选择");
    public string PlaceholderText { get => GetValue(PlaceholderTextProperty); set => SetValue(PlaceholderTextProperty, value); }

    public static readonly StyledProperty<string> SearchPlaceholderTextProperty =
        AvaloniaProperty.Register<TreeSelect, string>(nameof(SearchPlaceholderText), "搜索节点");
    public string SearchPlaceholderText { get => GetValue(SearchPlaceholderTextProperty); set => SetValue(SearchPlaceholderTextProperty, value); }

    public static readonly StyledProperty<string> EmptyTextProperty =
        AvaloniaProperty.Register<TreeSelect, string>(nameof(EmptyText), "无匹配节点");
    public string EmptyText { get => GetValue(EmptyTextProperty); set => SetValue(EmptyTextProperty, value); }

    public static readonly StyledProperty<string> ClearTextProperty =
        AvaloniaProperty.Register<TreeSelect, string>(nameof(ClearText), "清空可编辑选择");
    public string ClearText { get => GetValue(ClearTextProperty); set => SetValue(ClearTextProperty, value); }

    public static readonly StyledProperty<string> OpenTextProperty =
        AvaloniaProperty.Register<TreeSelect, string>(nameof(OpenText), "展开或收起树");
    public string OpenText { get => GetValue(OpenTextProperty); set => SetValue(OpenTextProperty, value); }

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<TreeSelect, bool>(nameof(IsReadOnly), false);
    public bool IsReadOnly { get => GetValue(IsReadOnlyProperty); set => SetValue(IsReadOnlyProperty, value); }

    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<TreeSelect, bool>(nameof(IsDropDownOpen), false, defaultBindingMode: BindingMode.TwoWay);
    public bool IsDropDownOpen { get => GetValue(IsDropDownOpenProperty); set => SetValue(IsDropDownOpenProperty, value); }

    public static readonly StyledProperty<int> MaxVisibleTagsProperty =
        AvaloniaProperty.Register<TreeSelect, int>(nameof(MaxVisibleTags), 3, validate: value => value is >= 0 and <= 100);
    public int MaxVisibleTags { get => GetValue(MaxVisibleTagsProperty); set => SetValue(MaxVisibleTagsProperty, value); }

    public static readonly StyledProperty<double> MaxDropDownHeightProperty =
        AvaloniaProperty.Register<TreeSelect, double>(nameof(MaxDropDownHeight), 320, validate: value => double.IsFinite(value) && value >= 80);
    public double MaxDropDownHeight { get => GetValue(MaxDropDownHeightProperty); set => SetValue(MaxDropDownHeightProperty, value); }

    public static readonly DirectProperty<TreeSelect, IReadOnlyList<TreeSelectItem>> VisibleItemsProperty =
        AvaloniaProperty.RegisterDirect<TreeSelect, IReadOnlyList<TreeSelectItem>>(nameof(VisibleItems), control => control.VisibleItems);
    private IReadOnlyList<TreeSelectItem> _visibleItems = [];
    public IReadOnlyList<TreeSelectItem> VisibleItems => _visibleItems;

    public static readonly DirectProperty<TreeSelect, IReadOnlyList<TreeSelectItem>> DisplayTagsProperty =
        AvaloniaProperty.RegisterDirect<TreeSelect, IReadOnlyList<TreeSelectItem>>(nameof(DisplayTags), control => control.DisplayTags);
    private IReadOnlyList<TreeSelectItem> _displayTags = [];
    public IReadOnlyList<TreeSelectItem> DisplayTags => _displayTags;

    public static readonly DirectProperty<TreeSelect, IReadOnlyList<object>> SelectedItemsProperty =
        AvaloniaProperty.RegisterDirect<TreeSelect, IReadOnlyList<object>>(nameof(SelectedItems), control => control.SelectedItems);
    private IReadOnlyList<object> _selectedItems = [];
    public IReadOnlyList<object> SelectedItems => _selectedItems;

    public static readonly DirectProperty<TreeSelect, int> SelectedCountProperty =
        AvaloniaProperty.RegisterDirect<TreeSelect, int>(nameof(SelectedCount), control => control.SelectedCount);
    private int _selectedCount = 0;
    public int SelectedCount => _selectedCount;

    public static readonly DirectProperty<TreeSelect, int> HiddenSelectedCountProperty =
        AvaloniaProperty.RegisterDirect<TreeSelect, int>(nameof(HiddenSelectedCount), control => control.HiddenSelectedCount);
    private int _hiddenCount = 0;
    public int HiddenSelectedCount => _hiddenCount;

    public static readonly DirectProperty<TreeSelect, bool> HasSelectionProperty =
        AvaloniaProperty.RegisterDirect<TreeSelect, bool>(nameof(HasSelection), control => control.HasSelection);
    private bool _hasSelection = false;
    public bool HasSelection => _hasSelection;

    public static readonly DirectProperty<TreeSelect, bool> HasHiddenSelectionProperty =
        AvaloniaProperty.RegisterDirect<TreeSelect, bool>(nameof(HasHiddenSelection), control => control.HasHiddenSelection);
    private bool _hasHidden = false;
    public bool HasHiddenSelection => _hasHidden;

    public static readonly DirectProperty<TreeSelect, bool> HasNoResultsProperty =
        AvaloniaProperty.RegisterDirect<TreeSelect, bool>(nameof(HasNoResults), control => control.HasNoResults);
    private bool _hasNoResults = true;
    public bool HasNoResults => _hasNoResults;

    public static readonly DirectProperty<TreeSelect, bool> CanClearProperty =
        AvaloniaProperty.RegisterDirect<TreeSelect, bool>(nameof(CanClear), control => control.CanClear);
    private bool _canClear = false;
    public bool CanClear => _canClear;

    public static readonly DirectProperty<TreeSelect, string?> ValidationErrorProperty =
        AvaloniaProperty.RegisterDirect<TreeSelect, string?>(nameof(ValidationError), control => control.ValidationError);
    private string? _validationError = null;
    public string? ValidationError => _validationError;

    public static readonly DirectProperty<TreeSelect, double> ItemHeightProperty =
        AvaloniaProperty.RegisterDirect<TreeSelect, double>(nameof(ItemHeight), control => control.ItemHeight);
    private double _itemHeight = 36;
    public double ItemHeight => _itemHeight;

}
