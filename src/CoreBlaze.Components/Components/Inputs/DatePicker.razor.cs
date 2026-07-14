using System.Globalization;
using CoreBlaze.Components.Base;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Inputs;

/// <summary>A native HTML5 date picker bound to <see cref="DateTime"/>?.</summary>
public partial class DatePicker : BaseInputComponent<DateTime?>
{
    /// <summary>Minimum selectable date (optional).</summary>
    [Parameter] public DateTime? MinDate { get; set; }

    /// <summary>Maximum selectable date (optional).</summary>
    [Parameter] public DateTime? MaxDate { get; set; }

    private string? FormattedValue => Value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private async Task OnDateChangedAsync(ChangeEventArgs e)
    {
        var text = e.Value?.ToString();
        if (string.IsNullOrWhiteSpace(text))
        {
            await SetValueAsync(null);
            return;
        }

        if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            await SetValueAsync(parsed);
    }
}
