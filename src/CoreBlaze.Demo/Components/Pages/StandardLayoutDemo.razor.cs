using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages;

public partial class StandardLayoutDemo : ComponentBase
{
    private const string _structureCode = """
        @* MainLayout.razor *@
        @inherits LayoutComponentBase
        @inject ThemeService Theme

        <div class="app-shell @(isDark ? "cb-theme-dark" : null)">
            <aside class="app-shell__sidebar">
                <div class="app-shell__brand">MyApp</div>
                <NavMenu />
            </aside>

            <main class="app-shell__main">
                <header class="app-shell__topbar">
                    <span class="app-shell__breadcrumb">@pageTitle</span>
                    <button @onclick="ToggleTheme">Toggle theme</button>
                </header>

                <article class="app-shell__content">
                    @Body
                </article>
            </main>
        </div>

        <Toaster Position="ToastPosition.TopRight" />
        """;

    private const string _cssCode = """
        .app-shell {
            display: flex;
            min-height: 100vh;
            background: var(--cb-surface-2);
            font-family: var(--cb-font);
        }

        .app-shell__sidebar {
            width: 260px;
            flex-shrink: 0;
            height: 100vh;
            position: sticky;
            top: 0;
            background: linear-gradient(180deg, #1e1b4b 0%, #312e81 45%, #4c1d95 100%);
            overflow-y: auto;
        }

        .app-shell__main {
            flex: 1;
            display: flex;
            flex-direction: column;
            min-width: 0;
        }

        .app-shell__topbar {
            height: 3.5rem;
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 0 1.5rem;
            background: var(--cb-surface);
            border-bottom: 1px solid var(--cb-border);
            position: sticky;
            top: 0;
            z-index: 10;
        }

        .app-shell__content {
            padding: 1.5rem 2rem;
            flex: 1;
            overflow-y: auto;
        }

        @media (max-width: 640px) {
            .app-shell { flex-direction: column; }
            .app-shell__sidebar { width: 100%; height: auto; position: static; }
        }
        """;
}
