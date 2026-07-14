namespace CoreBlaze.Components.Grid;

/// <summary>
/// Enabled export formats for <see cref="DataGrid{T}"/>. Combine with the <c>|</c> operator.
/// </summary>
[Flags]
public enum GridExportFormat
{
    /// <summary>No export button rendered.</summary>
    None = 0,

    /// <summary>Comma-separated values (RFC-4180-ish). Fastest, no dependencies.</summary>
    Csv = 1,

    /// <summary>Real Office Open XML workbook (<c>.xlsx</c>). Uses <c>ClosedXML</c>.</summary>
    Excel = 2,

    /// <summary>Portable Document Format (<c>.pdf</c>). Uses <c>QuestPDF</c>.</summary>
    Pdf = 4,

    /// <summary>All supported formats.</summary>
    All = Csv | Excel | Pdf,
}
