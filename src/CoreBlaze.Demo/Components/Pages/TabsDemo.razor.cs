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
        <!-- Buttons stay highlighted to show which tab is active -->
        @foreach (var label in new[] { "Overview", "Details", "History" })
        {
            <button class="btn btn-sm @(_activeTab == label ? "btn-primary" : "btn-outline-secondary")"
                    @onclick="() => _activeTab = label">
                @label
            </button>
        }

        <Tabs @bind-ActiveTitle="_activeTab">
            <ChildContent>
                <Tab Title="Overview"><p>Overview content.</p></Tab>
                <Tab Title="Details"><p>Details content.</p></Tab>
                <Tab Title="History"><p>Active: <strong>@_activeTab</strong></p></Tab>
            </ChildContent>
        </Tabs>

        <!-- OnTabChanged fires on every tab switch (optional) -->
        <Tabs OnTabChanged="OnTabChanged">
            <ChildContent>
                <Tab Title="A"><p>Tab A.</p></Tab>
                <Tab Title="B"><p>Tab B.</p></Tab>
            </ChildContent>
        </Tabs>

        @code {
            string? _activeTab = "Overview";

            void OnTabChanged(Tab tab)
            {
                // tab.Title holds the newly active tab's title
            }
        }
        """;
}
