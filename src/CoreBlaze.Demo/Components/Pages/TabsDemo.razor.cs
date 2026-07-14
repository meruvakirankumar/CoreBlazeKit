using CoreBlaze.Components.Shared;
using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages;

public partial class TabsDemo : ComponentBase
{
    private string? _activeTab = "Overview";

    private const string _code = """
        @using CoreBlaze.Components.Shared

        <!-- Basic: declare Tab children inside <ChildContent> -->
        <Tabs>
            <ChildContent>
                <Tab Title="Profile" Icon="👤">
                    <p>Profile content here.</p>
                </Tab>
                <Tab Title="Settings" Icon="⚙">
                    <p>Settings content here.</p>
                </Tab>
                <Tab Title="Billing" Icon="💳" Disabled="true">
                    <p>Locked.</p>
                </Tab>
            </ChildContent>
        </Tabs>

        <!-- Two-way bind: control active tab externally -->
        <Tabs @bind-ActiveTitle="_activeTab" OnTabChanged="OnTabChanged">
            <ChildContent>
                <Tab Title="Overview">…</Tab>
                <Tab Title="Details">…</Tab>
                <Tab Title="History">Active: @_activeTab</Tab>
            </ChildContent>
        </Tabs>

        @code {
            string? _activeTab = "Overview";
            void OnTabChanged(Tab t) => Console.WriteLine($"→ {t.Title}");
        }
        """;
}
