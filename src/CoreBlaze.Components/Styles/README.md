# Styles

Source-only style folder (design tokens, SCSS partials, theme files).

The published bundle stylesheet lives in `/wwwroot/CoreBlaze.css` and is
served as a static web asset at:

    _content/CoreBlaze.Components/CoreBlaze.css

Per-component styles use CSS isolation (`*.razor.css` next to the component)
and are auto-bundled by the Razor SDK into `CoreBlaze.Components.bundle.scp.css`.
