# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Scientific App** is a desktop application for data analysis and scientific computing, written in C# with WPF (.NET 8.0). The goal is to bridge Python-based analytical models (from CS109x coursework) with professional-grade interactive desktop UIs using MVVM architecture.

**Key Resources:**
- Project concept and learning phases: `docs/start-from-here.md`
- Expected architecture: MVVM pattern with separated concerns (Models for business logic, ViewModels for presentation, Views for UI)

## Build & Run

```bash
# Restore dependencies and build
dotnet build

# Run the application
dotnet run --project ScientificApp/ScientificApp.csproj

# Build for release
dotnet build -c Release

# Clean build artifacts
dotnet clean
```

## Development Commands

```bash
# Watch mode (rebuild on file changes during development)
dotnet watch run --project ScientificApp/ScientificApp.csproj

# Format code
dotnet format

# Check for .NET issues
dotnet analyze
```

## Project Structure

- **`ScientificApp/Views/`** — XAML UI definitions. Each view is a WPF Window or UserControl.
- **`ScientificApp/ViewModels/`** — ViewModel classes that inherit from `ObservableObject` (from CommunityToolkit.Mvvm). These expose `ObservableProperty` attributes for data binding.
- **`ScientificApp/Models/`** — Business logic and data models. Separate from UI concerns.
- **`docs/`** — Conceptual documentation and learning guides.

## Architecture: MVVM Pattern

This project follows the **Model-View-ViewModel** pattern:

1. **Model:** Pure business logic (calculations, data processing, Python model integration via API or Python.NET)
2. **ViewModel:** Exposes properties and commands for the View to bind to. Uses `[ObservableProperty]` attributes from CommunityToolkit.Mvvm for automatic INotifyPropertyChanged implementation.
3. **View:** XAML UI that binds to ViewModel properties via `{Binding PropertyName}`.

**Example binding flow:**
```xaml
<!-- View (MainWindow.xaml) -->
<TextBlock Text="{Binding StatusMessage}"/>
```
```csharp
// ViewModel (MainViewModel.cs)
[ObservableProperty]
private string _statusMessage = "Ready for Data Analysis";
```

## Dependencies

- **CommunityToolkit.Mvvm** (v8.4.2+) — Provides `ObservableObject` and `[ObservableProperty]` for simplified MVVM implementation.
- **.NET 8.0 (windows)** — Desktop framework with WPF enabled via `<UseWPF>true</UseWPF>` in .csproj.

When adding new packages, update `ScientificApp/ScientificApp.csproj` with `<PackageReference>` entries.

## Development Workflow

1. **Define the Model** — Write business logic in `Models/` (data structures, calculations, or Python integration).
2. **Create the ViewModel** — Expose model data and commands via `ObservableProperty` for binding.
3. **Build the View** — Design the XAML UI to bind to ViewModel properties.
4. **Test integration** — Wire up event handlers or commands to validate the flow.

Keep UI logic in the ViewModel, not in code-behind. Code-behind should only contain navigation or non-bindable UI setup.

## Learning Phases (from docs/start-from-here.md)

- **Phase 1 (Weeks 1-2):** Master MVVM pattern and XAML layouts (Grid, StackPanel).
- **Phase 2 (Weeks 3-4):** Translate analytical requirements to user stories and data-intensive UI designs.
- **Phase 3 (Weeks 5-6):** Integrate Python backend via FastAPI/Flask (Option A) or Python.NET (Option B).
- **Phase 4 (Weeks 7-8):** Add automated testing and input validation.

## Common Development Patterns

### Creating a new ViewModel
```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace ScientificApp.ViewModels;

public partial class NewViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Default Title";
}
```

### Binding a ViewModel to a View
In the View's code-behind, set the DataContext:
```csharp
DataContext = new NewViewModel();
```

Or configure in XAML for better separation.

### Async Operations
Use `async Task` methods in ViewModels and bind them to Button Commands. Leverage `async/await` to prevent UI freezing during long-running operations (e.g., Python model execution).

## Python Integration

The project is designed to call Python models for data analysis:
- **Option A (Web API):** Wrap Python in FastAPI/Flask, call via HTTP from C#.
- **Option B (Embedded):** Use Python.NET to call Python functions directly from C#.

When implementing, ensure UI remains responsive using `async/await`.

## Notes for Future Developers

- This is an early-stage project with foundational MVVM scaffolding.
- Focus on separating concerns: keep Views thin, business logic in Models, presentation logic in ViewModels.
- XAML can be verbose; prioritize clarity and maintainability over brevity.
- Use XAML resources (colors, fonts, brushes) in `App.xaml` to maintain consistency.

---

## Security Constraints

This section defines mandatory security boundaries for all AI-assisted development in this project. These constraints apply in addition to the permission rules in `.claude/settings.json`.

### Prohibited Operations

**Never perform the following, even if asked:**

1. **Network access from the shell** — Do not use `curl`, `wget`, `Invoke-WebRequest`, or any other tool to make outbound HTTP/HTTPS requests from Bash. The application communicates with external services only through C# `HttpClient` in source code, never through shell commands.

2. **Python/Node execution** — Do not run `python`, `python3`, `pip`, `node`, or `npm` commands. This is a .NET 8 / C# project. Python integration (Phase 3) uses Python.NET or FastAPI via the C# HTTP client — not direct shell execution of Python scripts.

3. **Force-push to git remote** — Do not use `git push --force` or `git push -f`. History rewrites require explicit developer confirmation outside this session.

4. **Modify secrets files** — Do not write to `.env`, `*.key`, `*.pem`, `*.pfx`, or any file whose name suggests it contains credentials or cryptographic material.

5. **Recursive deletion** — Do not use `rm -rf` or equivalent. File cleanup should use targeted `rm <specific-file>` with explicit paths.

6. **PowerShell execution policy bypass** — Do not invoke PowerShell with `-ExecutionPolicy Bypass` or `-EncodedCommand` flags.

### Permitted Tool Usage

The following tools are permitted for this project's workflows:

- `dotnet:*` — build, test, format, run, clean, publish
- `git:*` — status, diff, add, commit, log, branch, checkout, pull, push (no force)
- `gh:*` — issue create/list/view/close, pr create/list/merge
- Read, Write, Edit, Glob, Grep — for source file work

### Sensitive Files

The following files must never be created or modified by Claude:

| File Pattern | Reason |
|---|---|
| `.env`, `.env.*` | Environment variables including API keys |
| `*.key`, `*.pem`, `*.pfx`, `*.p12` | Cryptographic certificates and keys |
| `appsettings.Local.json` | Local developer configuration with potential secrets |
| `credentials.json`, `secrets.json` | Explicit credential storage |
| `.claude/settings.json` | Claude Code security configuration itself |

### Approved External Services

This project may only communicate with these external services, and only through C# source code (never shell commands):

- GitHub API — via `gh` CLI for issue/PR management (shell, explicitly allowed)
- Anthropic Claude API — for future AI feature integration via C# SDK
- Local `localhost` services — for Python backend integration in Phase 3

### Build Validation

After every `.cs` or `.xaml` file modification, the PostToolUse hook runs `dotnet build`. If the build fails, do not proceed with additional changes until the build error is resolved. Do not silence build errors with `|| true` in commands you construct.
