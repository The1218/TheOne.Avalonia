using System.Collections.ObjectModel;
using Avalonia.Media;
using TheOne.Avalonia.Reporting;

namespace TheOne.Avalonia.Controls;

internal sealed class ReportDesignState : ReportObservable
{
    public ReportDocument Document
    {
        get;
        set => Set(ref field, value);
    } = new();

    public ObservableCollection<ReportFieldInfo> Fields { get; } = [];
    public ObservableCollection<ReportDataNode> DataNodes { get; } = [];
    public ObservableCollection<ReportStructureNode> Structure { get; } = [];
    public ObservableCollection<ReportDocument> OpenDocuments { get; } = [];

    public bool HasDocument
    {
        get;
        set
        {
            Set(ref field, value);
            Notify(nameof(CanExport));
        }
    }

    public ReportFieldInfo? PendingField
    {
        get;
        set => Set(ref field, value);
    }

    public ReportColumn? SelectedColumn
    {
        get;
        set
        {
            Set(ref field, value);
            Notify(nameof(SelectionLabel));
            Notify(nameof(HasTableColumn));
            Notify(nameof(StatusDisplay));
        }
    }

    public ReportElement? SelectedElement
    {
        get;
        set
        {
            Set(ref field, value);
            Notify(nameof(SelectionLabel));
            Notify(nameof(HasElement));
            Notify(nameof(HasTableColumn));
            Notify(nameof(HasTextElement));
            Notify(nameof(HasBindableElement));
            Notify(nameof(HasImageElement));
            Notify(nameof(HasCheckBoxElement));
            Notify(nameof(HasBarcodeElement));
            Notify(nameof(HasQRCodeElement));
            Notify(nameof(StatusDisplay));
        }
    }

    public string SelectionLabel => HasTableColumn ? SelectedColumn!.Header + "单元格" : SelectedElement?.Name ?? "页面属性";
    public bool HasElement => SelectedElement is not null;

    public bool HasTextElement =>
        SelectedElement?.Kind is ReportElementKind.Text or ReportElementKind.CheckBox or ReportElementKind.Barcode
            or ReportElementKind.QRCode;

    public bool HasCheckBoxElement => SelectedElement?.Kind == ReportElementKind.CheckBox;
    public bool HasBarcodeElement => SelectedElement?.Kind == ReportElementKind.Barcode;
    public bool HasQRCodeElement => SelectedElement?.Kind == ReportElementKind.QRCode;
    public IReadOnlyList<QRCodeErrorCorrection> QRCodeLevels { get; } = Array.AsReadOnly(Enum.GetValues<QRCodeErrorCorrection>());

    public bool HasImageElement => SelectedElement?.Kind == ReportElementKind.Image;

    // Capture once per designer, after its platform font manager has been configured.
    public ObservableCollection<string> FontFamilies { get; } = new(FontManager.Current.SystemFonts
        .Select(family => family.Name).Where(name => !string.IsNullOrWhiteSpace(name))
        .Distinct(StringComparer.OrdinalIgnoreCase).Order(StringComparer.OrdinalIgnoreCase));

    public ObservableCollection<double> FontSizes { get; } =
        [6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 26, 28, 32, 36, 48, 72];

    public string SourceName
    {
        get;
        set => Set(ref field, value);
    } = "未连接";

    public string SaveStatus
    {
        get;
        set => Set(ref field, value);
    } = "未保存";

    public bool IsExporting
    {
        get;
        set
        {
            Set(ref field, value);
            Notify(nameof(CanExport));
        }
    }

    public bool CanExport => HasDocument && !IsExporting;

    public bool HasBindableElement => SelectedElement?.Kind is ReportElementKind.Text or ReportElementKind.Table
        or ReportElementKind.CheckBox or ReportElementKind.Barcode or ReportElementKind.QRCode;

    public bool HasTableColumn => SelectedElement?.Kind == ReportElementKind.Table && SelectedColumn is not null;

    public bool IsDataTab
    {
        get;
        set
        {
            Set(ref field, value);
            Notify(nameof(IsComponentsTab));
        }
    }

    public bool IsComponentsTab => !IsDataTab;

    public int PropertyTab
    {
        get;
        set
        {
            Set(ref field, value);
            Notify(nameof(StyleTab));
            Notify(nameof(DataTab));
            Notify(nameof(LayoutTab));
        }
    }

    public bool StyleTab => PropertyTab == 0;
    public bool DataTab => PropertyTab == 1;
    public bool LayoutTab => PropertyTab == 2;

    public bool Snap
    {
        get;
        set => Set(ref field, value);
    } = true;

    public string ApiUrl
    {
        get;
        set { if (Set(ref field, value)) Notify(nameof(HasApiSource)); }
    } = "";

    public bool HasApiSource => ApiUrl.Length > 0;

    public string ApiStatus
    {
        get;
        set
        {
            Set(ref field, value);
            HasApiError = false;
        }
    } = "数据源可选；先设计，再连接接口。";

    public bool HasApiError
    {
        get;
        set => Set(ref field, value);
    }

    public bool IsFetching
    {
        get;
        set
        {
            Set(ref field, value);
            Notify(nameof(CanFetch));
        }
    }

    public bool CanFetch => !IsFetching;

    public string ResponsePreview
    {
        get;
        set => Set(ref field, value);
    } = "";

    public ObservableCollection<ReportJsonBody> Bodies { get; } = [];

    public ReportJsonBody? SelectedBody
    {
        get;
        set => Set(ref field, value);
    }

    public string SourceLabel
    {
        get;
        set => Set(ref field, value);
    } = "未连接数据源";

    public string Status
    {
        get;
        set
        {
            Set(ref field, value);
            Notify(nameof(StatusDisplay));
        }
    } = "拖入组件以设计报表；数据源可选";

    public string StatusDisplay => Status.Contains("个元素 ·", StringComparison.Ordinal) && SelectedElement is { } element
        ? element.Kind == ReportElementKind.Table
            ? $"明细 / {element.Name} / {SelectedColumn?.FieldKey ?? SelectedColumn?.Header ?? ""}"
            : element.Name
        : Status;

    public string BindingStatus
    {
        get;
        set
        {
            Set(ref field, value);
            Notify(nameof(BindingSummary));
        }
    } = "";

    public string BindingSummary => BindingStatus.Length > 0 && BindingStatus != "尚未绑定有效字段" ? "●  绑定有效" : "○  未绑定数据";

    public string PageStatus
    {
        get;
        set => Set(ref field, value);
    } = "";

    public string FormatSample
    {
        get;
        set => Set(ref field, value);
    } = "";

    public bool IsPreview
    {
        get;
        set
        {
            if (Set(ref field, value))
            {
                Notify(nameof(IsDesign));
                Notify(nameof(ViewTabIndex));
            }
        }
    }

    public bool IsDesign => !IsPreview;

    public int ViewTabIndex
    {
        get => IsPreview ? 1 : 0;
        set => IsPreview = value == 1;
    }

    public bool CanPrevious
    {
        get;
        set => Set(ref field, value);
    }

    public bool CanNext
    {
        get;
        set => Set(ref field, value);
    }

    public bool CanUndo
    {
        get;
        set => Set(ref field, value);
    }

    public bool CanRedo
    {
        get;
        set => Set(ref field, value);
    }

    public double Zoom
    {
        get;
        set
        {
            Set(ref field, value);
            Notify(nameof(ZoomLabel));
        }
    } = .72;

    public string ZoomLabel => Zoom.ToString("P0");

    public string PaperLabel
    {
        get;
        set => Set(ref field, value);
    } = "A4 · 纵向";

    internal string EnsureFontFamily(string name)
    {
        var existing = FontFamilies.FirstOrDefault(family => StringComparer.OrdinalIgnoreCase.Equals(family, name));
        if (existing is not null) return existing;
        // A server template may name a font that is not installed on this computer.
        FontFamilies.Insert(
            FontFamilies.TakeWhile(family => StringComparer.OrdinalIgnoreCase.Compare(family, name) < 0).Count(), name);
        return name;
    }
}
