using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace TheOne.Avalonia.Reporting;

public enum ReportValueKind { Text, Number, Date }
public enum ReportValueFormat { Automatic, Text, Number, Currency, Date }
public enum ReportElementKind { Text, Table, Line, Rectangle, PageNumber, Image, CheckBox, Barcode }
public enum ReportTextAlignment { Left, Center, Right, Justify }
public enum ReportTextListStyle { None, Bullet, Check }
public enum ReportImageFit { Contain, Cover, Fill }
public enum ReportCondition { GreaterThan, LessThan, Equal }

/// <summary>A freely positioned report element. Geometry is measured in millimetres on the page.</summary>
public sealed class ReportElement : ReportObservable
{
    public string Name { get; set { Set(ref field, value); } } = "元素";
    public ReportElementKind Kind { get; init; }
    public double X { get; set { Set(ref field, value); } } = 14;
    public double Y { get; set { Set(ref field, value); } } = 14;
    public double Width { get; set { Set(ref field, value); } } = 60;
    public double Height { get; set { Set(ref field, value); } } = 12;
    public string Text { get; set { Set(ref field, value); } } = "文本";
    public string FieldKey { get; set { Set(ref field, value); } } = "";
    public bool IsChecked { get; set { Set(ref field, value); } }
    public bool BarcodeShowText { get; set { Set(ref field, value); } } = true;
    public double FontSize { get; set { Set(ref field, value); } } = 12;
    public bool Bold { get; set { Set(ref field, value); } }
    public string FontFamily { get; set { Set(ref field, value); } } = "Microsoft YaHei";
    public bool Italic { get; set { Set(ref field, value); } }
    public bool Underline { get; set { Set(ref field, value); } }
    public ReportTextAlignment TextAlignment { get; set { Set(ref field, value); } }
    public ReportTextListStyle ListStyle { get; set { Set(ref field, value); } }
    /// <summary>Embedded PNG/JPEG/WebP image bytes, encoded as base64. No external file dependency.</summary>
    public string ImageData { get; set { Set(ref field, value); } } = "";
    public string ImageName { get; set { Set(ref field, value); } } = "";
    public string ImageUrl { get; set { Set(ref field, value); } } = "";
    internal string ResolvedImageUrl { get; set; } = "";
    internal string ResolvedImageData { get; set; } = "";
    public double TableHeaderHeightMm { get; set { Set(ref field, value); } } = 11;
    public ReportImageFit ImageFit { get; set { Set(ref field, value); } }
    public string Foreground { get; set { Set(ref field, value); } } = "#212222";
    public string Fill { get; set { Set(ref field, value); } } = "Transparent";
    public bool ShowBorder { get; set { Set(ref field, value); } }
}

/// <summary>Mutable single-table report definition. Geometry is in millimetres; text and row sizes are in DIPs.</summary>
public sealed class ReportDocument : ReportObservable
{
    public string Title { get; set { Set(ref field, value); } } = "新建报表";
    public string Subtitle { get; set { Set(ref field, value); } } = "DETAIL REPORT";
    public string Description { get; set { Set(ref field, value); } } = "";
    public string Footer { get; set { Set(ref field, value); } } = "";
    public double PageWidthMm { get; set { Set(ref field, value); } } = 210;
    public double PageHeightMm { get; set { Set(ref field, value); } } = 297;
    public double MarginMm { get; set { Set(ref field, value); } } = 14;
    public double? MarginLeftMm { get; set { Set(ref field, value); } }
    public double? MarginTopMm { get; set { Set(ref field, value); } }
    public double? MarginRightMm { get; set { Set(ref field, value); } }
    public double? MarginBottomMm { get; set { Set(ref field, value); } }
    public double HeaderHeightMm { get; set { Set(ref field, value); } } = 32;
    public double FooterHeightMm { get; set { Set(ref field, value); } } = 18;
    public string DataSourceUrl { get; set { Set(ref field, value); } } = "";
    public string DataSourceMethod { get; set { Set(ref field, value); } } = "GET";
    public string DataSourceParameters { get; set { Set(ref field, value); } } = "";
    public ReportApiParameterMode DataSourceParameterMode { get; set { Set(ref field, value); } }
    public ReportApiAuthentication? DataSourceAuthentication { get; set { Set(ref field, value); } }
    public string DataBodyPointer { get; set { Set(ref field, value); } } = "";
    [JsonIgnore] public double LeftMargin => MarginLeftMm ?? MarginMm;
    [JsonIgnore] public double TopMargin => MarginTopMm ?? MarginMm;
    [JsonIgnore] public double RightMargin => MarginRightMm ?? MarginMm;
    [JsonIgnore] public double BottomMargin => MarginBottomMm ?? MarginMm;
    [JsonIgnore] public double HeaderBottom => TopMargin + HeaderHeightMm;
    [JsonIgnore] public double FooterTop => PageHeightMm - BottomMargin - FooterHeightMm;
    /// <summary>Returns a portable JSON definition including public API/image URLs, but no fetched records.</summary>
    public string Save() => ReportTemplate.Save(this);
    public static ReportDocument Load(string json) => ReportTemplate.Load(json);
    public double FontSize { get; set { Set(ref field, value); } } = 12;
    public double MinimumRowHeight { get; set { Set(ref field, value); } } = 30;
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public ObservableCollection<ReportColumn> Columns { get; } = [];
    [JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
    public ObservableCollection<ReportElement> Elements { get; } = [];
}

/// <summary>A detail column. FieldKey identifies an explicitly registered getter, never a reflection path.</summary>
public sealed class ReportColumn : ReportObservable
{
    public string Header { get; set { Set(ref field, value); } } = "新列";
    public string FieldKey { get; set { Set(ref field, value); } } = "";
    public string Text { get; set { Set(ref field, value); } } = "";
    public double WidthWeight { get; set { Set(ref field, value); } } = 1;
    public ReportValueFormat Format { get; set { Set(ref field, value); } }
    public int DecimalPlaces { get; set { Set(ref field, value); } } = 2;
    public bool UseThousandsSeparator { get; set { Set(ref field, value); } } = true;
    public bool ShowSum { get; set { Set(ref field, value); } }
    public bool ConditionEnabled { get; set { Set(ref field, value); } }
    public ReportCondition Condition { get; set { Set(ref field, value); } }
    public decimal ConditionValue { get; set { Set(ref field, value); } }
    public string ConditionColor { get; set { Set(ref field, value); } } = "#C43C3C";
}

public abstract class ReportObservable : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected bool Set<T>(ref T storage, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
        storage = value;
        Notify(name);
        return true;
    }
    protected void Notify([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
