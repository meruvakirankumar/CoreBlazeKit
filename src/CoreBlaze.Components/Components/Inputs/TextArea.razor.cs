using CoreBlaze.Components.Base;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Components.Inputs;

/// <summary>A multi-line text input.</summary>
public partial class TextArea : BaseInputComponent<string?>
{
    /// <summary>Number of visible rows.</summary>
    [Parameter] public int Rows { get; set; } = 3;

    /// <summary>Optional maximum length. 0 means unlimited.</summary>
    [Parameter] public int? MaxLength { get; set; }
}
