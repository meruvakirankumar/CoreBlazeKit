using CoreBlaze.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages;

public partial class TabsDemo : ComponentBase
{
    private string? _activeTab = "Overview";

    private const string _code = """
        @using CoreBlaze.Components.Shared

        <!-- Basic: declare Tab children inside ChildContent -->
        <Tabs>
            <ChildContent>
                <Tab Title="Profile" Icon="👤">
                    <p>Profile content here.</p>
                </Tab>
                <Tab Title="Settings" Icon="⚙">
                    <p>Settings content here.</p>
                </Tab>
                <Tab Title="Billing" Icon="💳" Disabled="true">
                    <p>Upgrade to access billing.</p>
                </Tab>
            </ChildContent>
        </Tabs>

        <!-- Two-way bind: drive the active tab from C# -->
        <button @onclick='() => _activeTab = "Details"'>Go to Details</button>

        <Tabs @bind-ActiveTitle="_activeTab" OnTabChanged="OnTabChanged">
            <ChildContent>
                <Tab Title="Overview"><p>Overview content.</p></Tab>
                <Tab Title="Details"><p>Details content.</p></Tab>
                <Tab Title="History"><p>Switched to: @_activeTab</p></Tab>
            </ChildContent>
        </Tabs>

        @code {
            string? _activeTab = "Overview";

            void OnTabChanged(Tab tab)
            {
                // fires whenever the active tab changes
            }
        }
        """;
}
