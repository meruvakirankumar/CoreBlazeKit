# CoreBlazeKit

> A lightweight, extensible, production-ready **Blazor component library** for .NET 8+  
> — with a full-featured DataGrid, rich input controls, layout primitives,  
> display components, and a live interactive demo app.

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com)

---

## What's inside

### Input components
| Component | Description |
|---|---|
| `TextInput` | Single-line text; supports `text`, `email`, `password`, `url`, `tel` |
| `TextArea` | Multi-line text with configurable rows and optional `MaxLength` |
| `NumberInput<T>` | Generic numeric input — `int`, `decimal`, `double?`, … — with `Prefix` / `Suffix` adornments (currency symbols, units, %) and `DecimalPlaces` rounding |
| `DatePicker` | Native HTML5 date picker bound to `DateTime?`, with `MinDate` / `MaxDate` |
| `Dropdown<TItem, TValue>` | Single-select generic dropdown |
| `MultiSelectDropdown<TItem, TValue>` | Popup multi-select with chip or text-summary display modes, Select-all toolbar |
| `FileUpload` | Drag-and-drop or click to browse; binds to `IReadOnlyList<IBrowserFile>` |

All inputs share a `BaseInputComponent<T>` base that wires `EditContext`, `FieldIdentifier`, two-way `@bind-Value`, validation messages, label, placeholder, disabled, required, and `OnChange` / `OnBlur` / `OnFocus` events.

---

### DataGrid `DataGrid<T>`
| Feature | Details |
|---|---|
| Sorting | Click header to sort asc/desc; **Shift-click** for multi-column sort with numbered ordinal badges |
| Filtering | Per-column filter row **+** global search box |
| Paging | Pager with configurable page-size selector |
| Edit modes | Row mode and Cell (double-click) mode; Enter to save, Esc to cancel |
| CRUD | Add / Edit / Delete / Save / Cancel with row-state tracking (Added / Modified / Deleted) |
| Selection | Off / Single / Multiple (checkbox column + `@bind-SelectedItems`) |
| Column templates | `Template` (display) + `EditTemplate` + `HeaderTemplate` per column |
| Column visibility | ⛁ Columns dropdown in toolbar to show/hide columns |
| Export | `⇩ Export` dropdown with **CSV**, real **Excel (.xlsx)** via ClosedXML, and real **PDF** via QuestPDF |

---

### Shared / Layout components
| Component | Description |
|---|---|
| `Modal` | Dialog with blurred backdrop, `@bind-IsOpen`, `Title`, `ChildContent`, `Footer`, `ModalSize` presets |
| `Toaster` + `ToastService` | Non-blocking notifications (Info / Success / Warning / Error), auto-dismiss, sticky mode |
| `Spinner` | Four variants: Ring, Pulse (radar), Wave (breathing), Gradient (conic); custom colours, centre image/icon, `LoadingOverlay` wrapper |
| `Tabs` + `Tab` | Tab strip with icon support, `@bind-ActiveTitle`, disabled tabs, slide-in animation |
| `Divider` | Horizontal/vertical separator with optional centre label |
| `Stack` | Flex container with `Direction`, `Gap` (0–6), `Align`, `Justify`, `Wrap` |

---

### Display components
| Component | Description |
|---|---|
| `Badge` | Pill label (text, count, dot); anchors to any wrapped child |
| `ProgressBar` | Labelled progress bar with variants, striped, and indeterminate modes |
| `Alert` | Inline contextual banner (stays in document flow); four variants, dismissible |
| `Toggle` | iOS-style switch with three sizes, `@bind-Value`, track text |
| `Avatar` | User avatar — photo or auto-coloured initials derived from name; badge overlay slot |

---

## Architecture

```
CoreBlaze.sln
├── src/
│   ├── CoreBlaze.Components/      # Razor Class Library (NuGet-ready)
│   │   ├── Base/                  # BaseInputComponent<T>
│   │   ├── Components/
│   │   │   ├── Inputs/            # 7 input controls
│   │   │   ├── Grid/              # DataGrid + GridColumn + GridPager
│   │   │   └── Shared/            # Modal, Toast, Spinner, Tabs, Stack, Divider,
│   │   │                          # Badge, ProgressBar, Alert, Toggle, Avatar
│   │   ├── Models/                # RowState, SortDescriptor, Toast, …
│   │   ├── Services/              # ThemeService, ToastService, ValidationService
│   │   ├── Utilities/             # CssBuilder, PropertyAccessor (compiled getters)
│   │   └── wwwroot/               # coreblaze.css (global CSS vars) · coreblaze.js
│   │
│   └── CoreBlaze.Demo/            # Blazor Web App — Interactive Server
│       ├── Components/
│       │   ├── Layout/            # MainLayout (sidebar + topbar + ThemeService toggle)
│       │   └── Pages/
│       │       ├── Inputs/        # /inputs  /inputs/text  /textarea  /number
│       │       │                  # /date  /dropdown  /multiselect  /fileupload
│       │       ├── GridDemo       # /grid
│       │       ├── DisplayDemo    # /display  (Badge / Progress / Alert / Toggle / Avatar)
│       │       ├── FeedbackDemo   # /feedback  (Toasts + Modal)
│       │       ├── SpinnerDemo    # /layout/spinner
│       │       ├── TabsDemo       # /layout/tabs
│       │       ├── StandardLayout # /layout/standard
│       │       ├── CustomLayouts  # /layout/custom  (Centered / Split / Dashboard / Master-detail)
│       │       └── FormDemo       # /form  (EditForm + DataAnnotations)
│       └── wwwroot/
│
└── tests/
    └── CoreBlaze.Components.Tests/ # xUnit + bUnit (36 tests)
```

---

## Prerequisites

- **.NET 8.0 SDK** or newer — download from <https://dotnet.microsoft.com/download>

---

## Get started

```powershell
# Clone
git clone https://github.com/meruvakirankumar/CoreBlazeKit.git
cd CoreBlazeKit

# Restore, build, run demo
dotnet restore
dotnet build
dotnet run --project src/CoreBlaze.Demo
```

The demo starts on `http://localhost:5141` (or the URL shown in the terminal).  
Browse the sidebar — every component has a dedicated page with an interactive playground, variants, live bound values, and a **copyable code snippet**.

---

## Using the library in your own project

### 1. Reference the project (or install the NuGet package)

```xml
<!-- project reference (monorepo / local) -->
<ProjectReference Include="path/to/CoreBlaze.Components/CoreBlaze.Components.csproj" />
```

### 2. Register services in `Program.cs`

```csharp
builder.Services.AddCoreBlaze(); // registers ThemeService, ToastService, ValidationService
```

### 3. Link the stylesheet + script in `App.razor`

```html
<link rel="stylesheet" href="_content/CoreBlaze.Components/coreblaze.css" />
<!-- inside <body> -->
<script src="_content/CoreBlaze.Components/coreblaze.js"></script>
```

### 4. Add the Toaster to your layout (once)

```razor
<Toaster Position="ToastPosition.TopRight" />
```

### 5. Add global `@using` to `_Imports.razor`

```razor
@using CoreBlaze.Components.Inputs
@using CoreBlaze.Components.Grid
@using CoreBlaze.Components.Shared
@using CoreBlaze.Components.Services
@using CoreBlaze.Components.Models
```

### Quick examples

```razor
<TextInput @bind-Value="_name" Label="Name" Required="true" MaxLength="80" />

<NumberInput TNumber="decimal"
             @bind-Value="_price"
             Label="Unit price"
             Prefix="$"
             Step="0.01"
             DecimalPlaces="2" />

<MultiSelectDropdown TItem="string" TValue="string"
                     Items="_departments"
                     ValueSelector="x => x"
                     TextSelector="x => x"
                     @bind-Value="_selected"
                     DisplayMode="MultiSelectDisplayMode.Chips" />

<DataGrid T="Order"
          Data="_orders"
          EditMode="GridEditMode.Row"
          SelectionMode="GridSelectionMode.Multiple"
          @bind-SelectedItems="_selectedOrders"
          EnableGlobalSearch="true"
          ExportFormats="GridExportFormat.All"
          ExportFileName="orders">
    <Columns>
        <GridColumn T="Order" Field="Id"   Title="ID" Editable="false" />
        <GridColumn T="Order" Field="Name" Title="Customer" />
    </Columns>
</DataGrid>

<Spinner Variant="SpinnerVariant.Pulse"
         Size="SpinnerSize.Large"
         Colors="@(new[] { "#4f46e5", "#a855f7", "#ec4899" })"
         CenterIcon="🔄" />

<Alert Variant="AlertVariant.Success" Title="Saved" Dismissible="true">
    Your changes were persisted successfully.
</Alert>

<Toggle @bind-Value="_notifications" Label="Push notifications" />

<Avatar Name="Alice Anderson" Size="AvatarSize.Large">
    <span class="cb-avatar-dot"></span>
</Avatar>
```

---

## Dark mode

Add `class="cb-theme-dark"` to any ancestor element (typically the layout root):

```razor
<div class="@(isDark ? "cb-theme-dark" : null)">
    ...
</div>
```

Use `ThemeService` to toggle it from anywhere:

```csharp
@inject ThemeService Theme

void ToggleTheme() => Theme.SetTheme(isDark ? "light" : "dark");
```

---

## Theming (CSS variables)

All colours, radii, shadows, and fonts are exposed as CSS custom properties:

```css
/* Light (default) */
:root {
    --cb-primary:      #4f46e5;   /* indigo */
    --cb-danger:       #dc2626;
    --cb-success:      #16a34a;
    --cb-warning:      #f59e0b;
    --cb-surface:      #ffffff;
    --cb-border:       #e5e7eb;
    --cb-text:         #1f2937;
    --cb-text-muted:   #64748b;
    --cb-radius:       0.5rem;
    --cb-font:         'Inter', system-ui, sans-serif;
}

/* Override anything globally */
:root {
    --cb-primary: #7c3aed;   /* switch to violet */
}
```

---

## Running tests

```powershell
dotnet test tests/CoreBlaze.Components.Tests
```

36 tests covering `CssBuilder`, `PropertyAccessor`, `ThemeService`, `ValidationService`, `TextInput` (bUnit), and `DataGrid` (bUnit).

---

## Packing as NuGet

```powershell
dotnet pack src/CoreBlaze.Components -c Release -o ./artifacts
```

Produces `artifacts/CoreBlaze.Components.0.1.0.nupkg` + `.snupkg` (symbols).

To publish:

```powershell
dotnet nuget push ./artifacts/CoreBlaze.Components.0.1.0.nupkg `
    --api-key <YOUR_KEY> `
    --source https://api.nuget.org/v3/index.json
```

Bump `<Version>` in `CoreBlaze.Components.csproj` before each release.

---

## Repository

GitHub: <https://github.com/meruvakirankumar/CoreBlazeKit>

```powershell
git clone https://github.com/meruvakirankumar/CoreBlazeKit.git
```

---

## License

MIT — see [LICENSE](LICENSE) for details.
