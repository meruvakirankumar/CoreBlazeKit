# CoreBlaze

A lightweight, extensible Blazor component library (.NET 8+) and demo application.

## Solution layout

```
CoreBlaze.sln
src/
â”œâ”€â”€ CoreBlaze.Components/        # Razor Class Library (NuGet-ready)
â”‚   â”œâ”€â”€ Base/                       # BaseInputComponent<T>, BaseComponent
â”‚   â”œâ”€â”€ Components/
â”‚   â”‚   â”œâ”€â”€ Inputs/                 # TextInput, TextArea, NumberInput, DatePicker,
â”‚   â”‚   â”‚                           # Dropdown, MultiSelectDropdown, FileUpload
â”‚   â”‚   â”œâ”€â”€ Grid/                   # DataGrid<T>, GridColumn<T>, GridCommandColumn<T>, GridPager
â”‚   â”‚   â””â”€â”€ Shared/                 # Small internal building blocks (icons, spinners, â€¦)
â”‚   â”œâ”€â”€ Models/                     # Public DTOs / enums (RowState, SortDescriptor, â€¦)
â”‚   â”œâ”€â”€ Services/                   # ThemeService, ValidationService, GridStateService
â”‚   â”œâ”€â”€ Utilities/                  # Reflection helpers, CSS merger, debouncer, JS interop
â”‚   â”œâ”€â”€ Styles/                     # Source styles (design tokens, SCSS)
â”‚   â”œâ”€â”€ wwwroot/                    # Published static assets (CoreBlaze.css)
â”‚   â”œâ”€â”€ _Imports.razor
â”‚   â””â”€â”€ CoreBlaze.Components.csproj
â””â”€â”€ CoreBlaze.Demo/              # Blazor Web App (Interactive Server) â€” showcase
    â”œâ”€â”€ Components/
    â”‚   â”œâ”€â”€ Layout/
    â”‚   â”œâ”€â”€ Pages/                  # Home, Counter, Weather, InputsDemo, GridDemo,
    â”‚   â”‚                           # UploadDemo, FormDemo
    â”‚   â”œâ”€â”€ App.razor
    â”‚   â”œâ”€â”€ Routes.razor
    â”‚   â””â”€â”€ _Imports.razor
    â”œâ”€â”€ wwwroot/
    â”œâ”€â”€ Program.cs
    â””â”€â”€ CoreBlaze.Demo.csproj
```

## Prerequisites

- .NET 8.0 SDK (or newer)

## Build & run

```powershell
dotnet restore
dotnet build
dotnet run --project src/CoreBlaze.Demo
```

The demo starts at the URL shown in the terminal (typically `https://localhost:5001`).
Navigate to **Inputs**, **DataGrid**, **File Upload**, or **Form Validation** in the side menu.

## Referencing the library

The demo project already references the RCL via ProjectReference:

```xml
<ProjectReference Include="..\CoreBlaze.Components\CoreBlaze.Components.csproj" />
```

To use the components in another project, register any services (once implemented)
and add the stylesheet:

```html
<link rel="stylesheet" href="_content/CoreBlaze.Components/CoreBlaze.css" />
```

CSS-isolation styles for each component are automatically bundled by the Razor SDK
into `CoreBlaze.Components.bundle.scp.css`, which is served by the consuming app.

## Packing the RCL as a NuGet package

The RCL csproj already carries packaging metadata (`PackageId`, `Version`,
`Description`, `Authors`, `PackageTags`, symbol package generation, source link).
Update those values, then run:

```powershell
dotnet pack src/CoreBlaze.Components -c Release -o ./artifacts
```

This produces:

- `artifacts/CoreBlaze.Components.<version>.nupkg`
- `artifacts/CoreBlaze.Components.<version>.snupkg` (debug symbols)

Publish to nuget.org (or a private feed):

```powershell
dotnet nuget push ./artifacts/CoreBlaze.Components.<version>.nupkg `
    --api-key <YOUR_KEY> --source https://api.nuget.org/v3/index.json
```

Bump `<Version>` in `CoreBlaze.Components.csproj` before each publish.

## Next steps

This commit contains **only the solution scaffold**. Upcoming work:

1. Implement `BaseInputComponent<T>` in `Base/`.
2. Implement each input control in `Components/Inputs/`.
3. Implement `DataGrid<T>` with sorting / filtering / paging / editing in `Components/Grid/`.
4. Flesh out demo pages `InputsDemo`, `GridDemo`, `UploadDemo`, `FormDemo`.
5. Optional: theme service, reusable validation service, column templates, grid state store.
