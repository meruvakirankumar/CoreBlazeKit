using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages.Inputs;

public partial class NumberInputDemo : ComponentBase
{
    private int _age = 25;
    private decimal _priceUsd = 9.99m;
    private decimal _priceEur = 12.50m;
    private decimal _priceInr = 799m;
    private decimal _priceGbp = 8.25m;
    private double? _weight;
    private decimal _progress = 45.00m;

    private const string _code = """
        @using CoreBlaze.Components.Inputs

        <!-- Plain int -->
        <NumberInput TNumber="int"
                     @bind-Value="_age"
                     Label="Age"
                     Min="0" Max="130" Step="1" />

        <!-- Currency: USD (prefix only, 2 dp) -->
        <NumberInput TNumber="decimal"
                     @bind-Value="_priceUsd"
                     Label="USD price"
                     Prefix="$"
                     Min="0" Step="0.01" DecimalPlaces="2"
                     Placeholder="0.00" />

        <!-- Currency: EUR (prefix AND suffix) -->
        <NumberInput TNumber="decimal"
                     @bind-Value="_priceEur"
                     Label="EUR price"
                     Prefix="€"
                     Suffix="EUR"
                     Min="0" Step="0.01" DecimalPlaces="2" />

        <!-- Currency: INR (different symbol / step) -->
        <NumberInput TNumber="decimal"
                     @bind-Value="_priceInr"
                     Label="INR price"
                     Prefix="₹"
                     Min="0" Step="0.5" DecimalPlaces="2" />

        <!-- Currency: GBP (suffix only) -->
        <NumberInput TNumber="decimal"
                     @bind-Value="_priceGbp"
                     Label="GBP price"
                     Suffix="GBP"
                     Min="0" Step="0.01" DecimalPlaces="2" />

        <!-- Unit suffix + nullable double -->
        <NumberInput TNumber="double?"
                     @bind-Value="_weight"
                     Label="Weight"
                     Suffix="kg"
                     Step="0.1"
                     Placeholder="Leave blank if unknown" />

        <!-- Percentage -->
        <NumberInput TNumber="decimal"
                     @bind-Value="_progress"
                     Label="Completion"
                     Suffix="%"
                     Min="0" Max="100"
                     Step="0.01" DecimalPlaces="2" />

        <!-- Disabled -->
        <NumberInput TNumber="int"
                     Value="42"
                     Label="Disabled example"
                     Prefix="$"
                     Disabled="true" />

        @code {
            int _age = 25;
            decimal _priceUsd = 9.99m, _priceEur = 12.50m, _priceInr = 799m, _priceGbp = 8.25m;
            double? _weight;
            decimal _progress = 45.00m;
        }
        """;
}
