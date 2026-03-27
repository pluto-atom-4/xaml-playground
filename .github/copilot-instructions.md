# Copilot Instructions for Scientific App

C# desktop application for data analysis (WPF, .NET 8.0, MVVM). See `CLAUDE.md` for security constraints and detailed patterns.

## Quick Reference

| Command | Purpose |
|---------|---------|
| `dotnet build` | Build solution |
| `dotnet run --project ScientificApp/ScientificApp.csproj` | Run app |
| `dotnet watch run --project ScientificApp/ScientificApp.csproj` | Watch mode |
| `dotnet test` | Run tests (Phase 4+) |
| `dotnet format` | Format code |

## Architecture: MVVM

**Models** → Business logic, no UI deps  
**ViewModels** → Inherit `ObservableObject`, use `[ObservableProperty]`  
**Views** → XAML UI, minimal code-behind, bind via `{Binding PropertyName}`

```csharp
// ViewModel: private field with [ObservableProperty]
[ObservableProperty] private string _statusMessage = "Ready";

// View: bind to public property (auto-generated from _statusMessage)
<TextBlock Text="{Binding StatusMessage}"/>
```

**Critical:** Always use `async Task` for long operations. Never block UI with `.Result` or `.Wait()`.

## Naming & Structure

```
ScientificApp/
├── Views/              # *.xaml + *.xaml.cs
├── ViewModels/         # *ViewModel.cs
├── Models/             # *.cs (services, data)
└── ScientificApp.csproj
```

**Namespaces:** `ScientificApp.{Models|ViewModels|Views}`  
**ViewModel pattern:** `public partial class *ViewModel : ObservableObject`  
**Key dependency:** CommunityToolkit.Mvvm v8.4.2+

---

## Phase 3: Python Integration

### Option A: Web API (Recommended)
Wrap Python in FastAPI/Flask, call via `HttpClient`:

```csharp
public class PredictionService
{
    private readonly HttpClient _http = new();
    
    public async Task<Result> PredictAsync(Input data)
    {
        var response = await _http.PostAsJsonAsync("http://localhost:8000/predict", data);
        return await response.Content.ReadAsAsync<Result>();
    }
}
```

**In ViewModel:**
```csharp
[RelayCommand]
private async Task RunPrediction()
{
    IsBusy = true;
    try
    {
        var result = await _service.PredictAsync(input);
        Output = result.ToString();
    }
    finally { IsBusy = false; }
}
```

### Option B: Python.NET (Embedded)
Direct Python calls in-process:

```csharp
using Python.Runtime;

public class EmbeddedModel
{
    private dynamic _module;
    
    public EmbeddedModel(string path)
    {
        PythonEngine.Initialize();
        using (Py.GIL()) _module = Py.Import(Path.GetFileNameWithoutExtension(path));
    }
    
    public async Task<double> PredictAsync(double[] features)
    {
        return await Task.Run(() =>
        {
            using (Py.GIL()) return _module.predict(features).As<double>();
        });
    }
}
```

**Setup:** `dotnet add package pythonnet`

### Considerations
- Wrap calls in try-catch; provide user-friendly error messages
- Use `CancellationToken` for cancellable operations
- Set timeouts on `HttpClient` to prevent hangs
- Never block UI thread—use `async/await` always

---

## Phase 4: Testing

### Setup
```bash
dotnet new xunit -n ScientificApp.Tests
dotnet add ScientificApp.Tests reference ScientificApp
```

### Conventions
- Test files: `*Tests.cs` (e.g., `PredictionServiceTests.cs`)
- Methods: `{Method}_{Scenario}_{Result}` (e.g., `PredictAsync_WithValidInput_ReturnsResult`)

### Example
```csharp
using Xunit;
using Moq;

[Fact]
public async Task RunPrediction_UpdatesOutput()
{
    var mock = new Mock<IPredictionService>();
    mock.Setup(s => s.PredictAsync(It.IsAny<Input>())).ReturnsAsync(new Result { Value = 42 });
    
    var vm = new MainViewModel(mock.Object);
    await vm.RunPredictionCommand.ExecuteAsync(null);
    
    Assert.Equal("42", vm.Output);
}
```

### Run Tests
```bash
dotnet test                      # All tests
dotnet test --filter "ClassName" # Specific class
dotnet watch test                # Watch mode
```

**Guidelines:**
- Mock external dependencies (services, Python backend)
- Test business logic, not UI bindings
- Keep tests isolated—no shared state
- Use Xunit for async support

---

## Troubleshooting MVVM Binding

| Problem | Cause | Fix |
|---------|-------|-----|
| Property not updating | No `[ObservableProperty]` or missing `ObservableObject` | Use `[ObservableProperty] private string _field;` |
| Binding path error (40) | Wrong property name or null DataContext | Check spelling (case-sensitive) and DataContext assignment |
| Command not firing | Command not public or wrong binding name | Use `[RelayCommand] private async Task MethodName()` and bind to `MethodNameCommand` |
| UI freezes | Blocking `.Result` or `.Wait()` calls | Always use `await` for async operations |
| Button stays clickable | Missing `IsBusy` flag or improper cleanup | Set `IsBusy = false` in `finally` block |

---

## Key Resources

- **Phase learning:** `docs/start-from-here.md`
- **Security & detailed patterns:** `CLAUDE.md`
- **Microsoft WPF docs:** https://learn.microsoft.com/en-us/dotnet/desktop/wpf/
- **CommunityToolkit.Mvvm:** https://github.com/CommunityToolkit/dotnet/tree/main/components/MVVM
