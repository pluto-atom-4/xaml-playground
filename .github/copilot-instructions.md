# Copilot Instructions for Scientific App

This repository is a C# desktop application for data analysis and scientific computing using WPF (.NET 8.0) with MVVM architecture. For security constraints and detailed patterns, see `CLAUDE.md`.

## Build & Verification Commands

```bash
# Build the solution
dotnet build

# Run the application
dotnet run --project ScientificApp/ScientificApp.csproj

# Watch mode (rebuild on file changes)
dotnet watch run --project ScientificApp/ScientificApp.csproj

# Format code
dotnet format

# Clean build artifacts
dotnet clean
```

**Note:** There are currently no automated tests in the repository. When adding test projects in Phase 4, follow the pattern `dotnet test` for full suite and `dotnet test --filter "ClassName"` for targeted tests.

## High-Level Architecture

**Scientific App** bridges Python analytical models (from CS109x coursework) with professional-grade desktop UIs using strict MVVM separation:

### MVVM Structure
- **Models** (`ScientificApp/Models/`) — Pure business logic, data structures, and calculations. No UI dependencies. Interfaces with Python via FastAPI/Flask (Phase 3 Option A) or Python.NET (Phase 3 Option B).
- **ViewModels** (`ScientificApp/ViewModels/`) — Expose model data and commands for UI binding. All inherit from `CommunityToolkit.Mvvm.ComponentModel.ObservableObject`. Use `[ObservableProperty]` attributes for automatic INotifyPropertyChanged implementation.
- **Views** (`ScientificApp/Views/`) — XAML UI definitions (WPF Windows and UserControls). Bind to ViewModels via `{Binding PropertyName}` with DataContext set to ViewModel. Keep code-behind minimal—only navigation and non-bindable UI setup.

### MVVM Example
```xaml
<!-- MainWindow.xaml -->
<TextBlock Text="{Binding StatusMessage}"/>
```
```csharp
// MainViewModel.cs
[ObservableProperty]
private string _statusMessage = "Ready for Data Analysis";
```

### Key Constraint: No UI Freezing
ViewModels must use `async Task` for long-running operations (Python model execution, data processing). Use `async/await` to keep the UI responsive.

## Critical Conventions

### File Naming & Namespacing
- ViewModels: `*ViewModel.cs` → namespace `ScientificApp.ViewModels`
- Models: `*.cs` → namespace `ScientificApp.Models`
- Views: `*.xaml` + `*.xaml.cs` → namespace `ScientificApp.Views`

### ViewModel Creation Pattern
```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace ScientificApp.ViewModels;

public partial class FeatureViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Default";

    [ObservableProperty]
    private int _count;
}
```

### Dependencies
- **CommunityToolkit.Mvvm** v8.4.2+ — Provides `ObservableObject` and `[ObservableProperty]` for MVVM implementation.
- **.NET 8.0-windows** — Target framework with WPF enabled (`<UseWPF>true</UseWPF>`).
- New packages must be added to `ScientificApp/ScientificApp.csproj` as `<PackageReference>` entries.

### Python Integration (Phase 3)
The project is designed for two integration approaches (chosen in Phase 3):
- **Option A (Recommended for web-scale):** FastAPI/Flask backend called via `HttpClient` from C#.
- **Option B (For tight coupling):** Python.NET library for direct Python function calls from C#.

Always use `async/await` patterns regardless of integration choice.

## Learning Context

This is a four-phase project structured to progress from UI/MVVM basics to Python integration:
- **Phase 1:** MVVM patterns and XAML layouts.
- **Phase 2:** User stories and data-intensive UI design.
- **Phase 3:** Python backend integration.
- **Phase 4:** Automated testing and input validation.

See `docs/start-from-here.md` for detailed learning objectives and capstone project guidance.

## Project Directories

```
ScientificApp/
├── Views/              # XAML UI definitions
├── ViewModels/         # MVVM presentation logic
├── Models/             # Business logic
├── App.xaml            # WPF application resources (colors, fonts, brushes)
├── App.xaml.cs         # Application startup logic
└── ScientificApp.csproj
docs/                   # Conceptual documentation
.github/               # GitHub configuration
```

## Additional Context

- **Early-stage project:** Focus on separating concerns. Views should be thin, logic in Models, presentation in ViewModels.
- **XAML resources:** Use `App.xaml` for global colors, fonts, and brushes to maintain UI consistency.
- **Code-behind:** Should be minimal. Prefer binding and commands over event handlers in code-behind.
