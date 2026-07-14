using Microsoft.AspNetCore.Components;

namespace CoreBlaze.Demo.Components.Pages;

public partial class CustomLayoutsDemo : ComponentBase
{
    // ── Centered ────────────────────────────────────────────────────────────

    private const string _centeredHtml = """
        <div class="page-centered">
            <header class="page-centered__topbar">
                <strong>MyApp</strong>
            </header>
            <main class="page-centered__body">
                <div class="page-centered__panel">
                    <h2>Sign in</h2>
                    <!-- form / card content here -->
                </div>
            </main>
        </div>
        """;
    private const string _centeredCss = """
        .page-centered {
            min-height: 100vh;
            display: flex;
            flex-direction: column;
        }
        .page-centered__topbar {
            height: 3.5rem;
            display: flex; align-items: center;
            padding: 0 1.5rem;
            background: var(--cb-surface);
            border-bottom: 1px solid var(--cb-border);
        }
        .page-centered__body {
            flex: 1;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 2rem 1rem;
            background: var(--cb-surface-2);
        }
        .page-centered__panel {
            width: 100%;
            max-width: 28rem;
            background: var(--cb-surface);
            border: 1px solid var(--cb-border);
            border-radius: 0.75rem;
            padding: 2rem;
            box-shadow: var(--cb-shadow-md);
        }
        """;

    private const string _splitCss = """
        .page-split {
            min-height: 100vh;
            display: grid;
            grid-template-columns: 1fr 1fr;
        }
        .page-split__left {
            background: linear-gradient(135deg, #1e1b4b, #4f46e5);
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            padding: 3rem;
            color: #fff;
        }
        .page-split__right {
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 3rem;
            background: var(--cb-surface);
        }
        @media (max-width: 768px) {
            .page-split { grid-template-columns: 1fr; }
            .page-split__left { min-height: 12rem; }
        }
        """;

    private const string _dashCss = """
        .page-dashboard {
            display: flex;
            flex-direction: column;
            gap: 1.25rem;
            padding: 1.5rem;
        }
        .page-dashboard__stats {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 1rem;
        }
        .page-dashboard__charts {
            display: grid;
            grid-template-columns: 2fr 1fr;
            gap: 1rem;
        }
        .page-dashboard__table { width: 100%; }
        .stat-card, .chart-card, .table-card {
            background: var(--cb-surface);
            border: 1px solid var(--cb-border);
            border-radius: 0.75rem;
            padding: 1.25rem;
            box-shadow: var(--cb-shadow-sm);
        }
        @media (max-width: 900px) {
            .page-dashboard__stats  { grid-template-columns: repeat(2, 1fr); }
            .page-dashboard__charts { grid-template-columns: 1fr; }
        }
        """;

    private const string _masterDetailCss = """
        .page-master-detail {
            display: flex;
            height: 100vh;
        }
        .page-master-detail__list {
            width: 280px;
            flex-shrink: 0;
            background: var(--cb-surface);
            border-right: 1px solid var(--cb-border);
            overflow-y: auto;
        }
        .page-master-detail__detail {
            flex: 1;
            min-width: 0;
            padding: 1.5rem 2rem;
            overflow-y: auto;
            background: var(--cb-surface-2);
        }
        @media (max-width: 640px) {
            .page-master-detail { flex-direction: column; }
            .page-master-detail__list { width: 100%; height: 40vh; }
        }
        """;

    // ── HTML structure snippets ──────────────────────────────────────────────

    private const string _splitHtml = """
        <div class="page-split">
            <aside class="page-split__left">
                <h1>Brand</h1>
                <p>Your tagline here.</p>
            </aside>
            <main class="page-split__right">
                <div style="max-width:28rem; width:100%">
                    <!-- form / content here -->
                </div>
            </main>
        </div>
        """;

    private const string _dashHtml = """
        <div class="page-dashboard">
            <div class="page-dashboard__stats">
                <div class="stat-card"><!-- stat 1 --></div>
                <div class="stat-card"><!-- stat 2 --></div>
                <div class="stat-card"><!-- stat 3 --></div>
                <div class="stat-card"><!-- stat 4 --></div>
            </div>
            <div class="page-dashboard__charts">
                <div class="chart-card"><!-- main chart (2 cols wide) --></div>
                <div class="chart-card"><!-- secondary chart --></div>
            </div>
            <div class="page-dashboard__table">
                <div class="table-card"><!-- data table --></div>
            </div>
        </div>
        """;

    private const string _masterDetailHtml = """
        <div class="page-master-detail">
            <nav class="page-master-detail__list">
                <!-- list of items, e.g. <SimpleGrid> or <ul> -->
            </nav>
            <section class="page-master-detail__detail">
                <!-- detail panel content -->
            </section>
        </div>
        """;
}
