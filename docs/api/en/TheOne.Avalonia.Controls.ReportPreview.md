# ReportPreview

[English](../en/TheOne.Avalonia.Controls.ReportPreview.md) · [简体中文](../zh-cn/TheOne.Avalonia.Controls.ReportPreview.md) · [API index](README.md)

`TheOne.Avalonia.Controls.ReportPreview` · Class · `Avalonia.Controls.UserControl`

Standalone read-only report viewer. Parses document JSON and loads its optional API/images.

[Source / 声明](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L14)

Defaults below are source registration/model initialization values; OneTheme styles or local values can override them. Read-only members are computed state, not two-way inputs. Complex objects, templates, commands and collections follow the control's usage contract.

| Property | Type | Access/binding/inheritance | Registration/model default | Meaning and constraints |
| --- | --- | --- | --- | --- |
| [`CurrentPageNumber`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L29) | `int` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`DataSource`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L21) | `TheOne.Avalonia.Reporting.ReportDataSource?` | writable; OneWay | `null` | Explicit data source supplied by the host according to the model contract; templates do not replace data adaptation. |
| [`Document`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L22) | `TheOne.Avalonia.Reporting.ReportDocument?` | read-only; OneWay | `computed / state` | Current business document model; editor documentation defines subscription, selection and history behavior on replacement. |
| [`DocumentJson`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L20) | `string?` | writable; OneWay | `null` | Report document JSON; loading failures/replacement/data refresh follow the report loading contract. |
| [`Error`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L23) | `string?` | read-only; OneWay | `computed / state` | Structured/text failure information; inspect it together with the success flag. |
| [`HttpClient`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L26) | `System.Net.Http.HttpClient?` | writable; CLR / n/a | `null` | Optional consumer-owned client for authenticated API and image requests. Not serialized. |
| [`Layout`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L24) | `TheOne.Avalonia.Reporting.ReportLayout?` | read-only; CLR / n/a | `accessor-defined / state` | Read-only runtime/derived state, updated by the control/model for display or command conditions; not a two-way input. |
| [`PrintHandler`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L28) | `System.Func<string, TheOne.Avalonia.Reporting.ReportPrintOptions?, System.Threading.CancellationToken, System.Threading.Tasks.Task<TheOne.Avalonia.Reporting.ReportPrintResult>>?` | writable; CLR / n/a | `null` | Optional host printer. The PDF exists until the returned task completes; KeepDocument controls its subsequent lifetime. |
| [`Zoom`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L30) | `double` | writable; CLR / n/a | `.75` | View scale factor, where 1 means 100%; does not modify report millimetres or workflow coordinates. |

## Methods and constructors

| Complete signature/value | Contract |
| --- | --- |
| [`Task ReportPreview.LoadAsync(string json, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L103) | Loads report JSON and associated data/images asynchronously, respecting cancellation, failures and stale-load isolation. |
| [`void ReportPreview.GoToPage(int page)`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L137) | Navigates to a requested page; paging/report contracts define indexing and range handling. |
| [`Task ReportPreview.ExportPdfAsync(Stream destination, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L149) | Asynchronously exports the current report layout to PDF; see output/cancellation parameters. |
| [`Task<ReportPrintResult> ReportPreview.PrintAsync(ReportPrintOptions? options = null, CancellationToken cancellationToken = default(CancellationToken))`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L155) | Invokes the configured print handler and awaits its result; the host chooses printing dependencies/submission policy. |
| [`ReportPreview.ReportPreview()`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L44) | Creates an instance; see the signature for required arguments and optional defaults. Callers retain ownership of shared models/resources. |

## Public fields

| Complete signature/value | Contract |
| --- | --- |
| [`static readonly StyledProperty<string?> ReportPreview.DocumentJsonProperty`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L16) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly StyledProperty<ReportDataSource?> ReportPreview.DataSourceProperty`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L17) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ReportPreview, ReportDocument?> ReportPreview.DocumentProperty`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L18) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |
| [`static readonly DirectProperty<ReportPreview, string?> ReportPreview.ErrorProperty`](../../../TheOne.Avalonia/Controls/ReportPreview.cs#L19) | Public identifier/constant; Property/Event suffixes identify native Avalonia properties/routed events. See their CLR properties/events above. |

## Styles and density

See [shared theme configuration](../../theme/en/README.md#shared-configuration) for common sizing and motion settings, demonstrated centrally on PreviewHost Home. `one-*` classes are template-owned; pseudoclasses follow real input/state rather than manual assignment.

No additional public appearance classes; configure public properties and the default theme.

## Related APIs and built-in models

- [`TheOne.Avalonia.Reporting.ReportDataSource`](TheOne.Avalonia.Reporting.ReportDataSource.md)
- [`TheOne.Avalonia.Reporting.ReportDocument`](TheOne.Avalonia.Reporting.ReportDocument.md)
- [`TheOne.Avalonia.Reporting.ReportLayout`](TheOne.Avalonia.Reporting.ReportLayout.md)
- [`TheOne.Avalonia.Reporting.ReportPrintOptions`](TheOne.Avalonia.Reporting.ReportPrintOptions.md)
- [`TheOne.Avalonia.Reporting.ReportPrintResult`](TheOne.Avalonia.Reporting.ReportPrintResult.md)
