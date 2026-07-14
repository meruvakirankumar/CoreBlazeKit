using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages.Inputs;

public partial class DatePickerDemo : ComponentBase
{
    private DateTime? _hireDate = DateTime.Today;
    private DateTime? _upcoming;
    private DateTime? _errorDate;  // error clears once a date is picked

    private const string _code = """
        @using CoreBlaze.Components.Inputs

        <!-- Playground — unbounded -->
        <DatePicker @bind-Value="_hireDate" Label="Hire date" />

        <!-- Bounded range (today → +1 year) -->
        <DatePicker @bind-Value="_upcoming"
                    Label="Upcoming event"
                    MinDate="DateTime.Today"
                    MaxDate="DateTime.Today.AddYears(1)" />

        <!-- Disabled -->
        <DatePicker Label="Disabled example"
                    Value="new DateTime(2024, 1, 1)"
                    Disabled="true" />

        <!-- Error message -->
        <DatePicker Label="Error example"
                    ErrorMessage="Date is invalid." />

        @code {
            DateTime? _hireDate = DateTime.Today;
            DateTime? _upcoming;
        }
        """;
}
