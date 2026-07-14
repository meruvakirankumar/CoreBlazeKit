# CustomBlazor

A lightweight, extensible Blazor component library (.NET 8+) and demo application.

## Solution layout

```
CustomBlazor.sln
src/
├── CustomBlazor.Components/        # Razor Class Library (NuGet-ready)
│   ├── Base/                       # BaseInputComponent<T>, BaseComponent
│   ├── Components/
│   │   ├── Inputs/                 # TextInput, TextArea, NumberInput, DatePicker,
│   │   │                           # Dropdown, MultiSelectDropdown, FileUpload
│   │   ├── Grid/                   # DataGrid<T>, GridColumn<T>, GridCommandColumn<T>, GridPager
│   │   └── Shared/                 # Small internal building blocks (icons, spinners, …)
│   ├── Models/                     # Public DTOs / enums (RowState, SortDescriptor, …)
│   ├── Services/                   # ThemeService, ValidationService, GridStateService
│   ├── Utilities/                  # Reflection helpers, CSS merger, debouncer, JS interop
│   ├── Styles/                     # Source styles (design tokens, SCSS)
│   ├── wwwroot/                    # Published static assets (customblazor.css)
│   ├── _Imports.razor
│   └── CustomBlazor.Components.csproj
└── CustomBlazor.Demo/              # Blazor Web App (Interactive Server) — showcase
    ├── Components/
    │   ├── Layout/
    │   ├── Pages/                  # Home, Counter, Weather, InputsDemo, GridDemo,
    │   │                           # UploadDemo, FormDemo
    │   ├── App.razor
    │   ├── Routes.razor
    │   └── _Imports.razor
    ├── wwwroot/
    ├── Program.cs
    └── CustomBlazor.Demo.csproj
```

## Prerequisites

- .NET 8.0 SDK (or newer)

## Build & run

```powershell
dotnet restore
dotnet build
dotnet run --project src/CustomBlazor.Demo
```

The demo starts at the URL shown in the terminal (typically `https://localhost:5001`).
Navigate to **Inputs**, **DataGrid**, **File Upload**, or **Form Validation** in the side menu.

## Referencing the library

The demo project already references the RCL via ProjectReference:

```xml
<ProjectReference Include="..\CustomBlazor.Components\CustomBlazor.Components.csproj" />
```

To use the components in another project, register any services (once implemented)
and add the stylesheet:

```html
<link rel="stylesheet" href="_content/CustomBlazor.Components/customblazor.css" />
```

CSS-isolation styles for each component are automatically bundled by the Razor SDK
into `CustomBlazor.Components.bundle.scp.css`, which is served by the consuming app.

## Packing the RCL as a NuGet package

The RCL csproj already carries packaging metadata (`PackageId`, `Version`,
`Description`, `Authors`, `PackageTags`, symbol package generation, source link).
Update those values, then run:

```powershell
dotnet pack src/CustomBlazor.Components -c Release -o ./artifacts
```

This produces:

- `artifacts/CustomBlazor.Components.<version>.nupkg`
- `artifacts/CustomBlazor.Components.<version>.snupkg` (debug symbols)

Publish to nuget.org (or a private feed):

```powershell
dotnet nuget push ./artifacts/CustomBlazor.Components.<version>.nupkg `
    --api-key <YOUR_KEY> --source https://api.nuget.org/v3/index.json
```

Bump `<Version>` in `CustomBlazor.Components.csproj` before each publish.

## Next steps

This commit contains **only the solution scaffold**. Upcoming work:

1. Implement `BaseInputComponent<T>` in `Base/`.
2. Implement each input control in `Components/Inputs/`.
3. Implement `DataGrid<T>` with sorting / filtering / paging / editing in `Components/Grid/`.
4. Flesh out demo pages `InputsDemo`, `GridDemo`, `UploadDemo`, `FormDemo`.
5. Optional: theme service, reusable validation service, column templates, grid state store.
