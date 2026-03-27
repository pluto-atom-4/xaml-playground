# Agent Runbooks - Specialized Workflows

Quick reference for invoking each agent in the specialized team.

---

## Issue Analyzer Agent

**Purpose:** Understand and decompose GitHub issues into actionable requirements

### How to Use
```bash
# Triggered automatically in /fix-github-issues Phase 2
# Or invoke manually:
claude code "Analyze this GitHub issue and provide:
1. Problem statement
2. Affected MVVM components (Model, ViewModel, View, or Model)
3. Acceptance criteria
4. Input/output examples
5. Edge cases to consider
6. Suggested test cases"
```

### Example Prompt
```
Issue Title: "Add unit test coverage for LoginViewModel"
Issue Body: "The LoginViewModel needs tests for email validation,
password hashing, and API calls. Target: 80% code coverage."

Please analyze and break this down...
```

### Expected Output
```
ANALYSIS: Add LoginViewModel Unit Tests

Components Affected:
├─ Models: LoginValidator, PasswordHasher
├─ ViewModels: LoginViewModel
└─ Views: None (backend only)

Requirements:
1. Email validation test cases
   - Valid: user@domain.com
   - Invalid: missing @, no domain
   - Edge: special chars, max length
2. Password hashing tests
   - Verify hash consistency
   - Check security standards
3. API call mocking
   - Mock failed auth
   - Mock network timeout

Test Coverage Target: 80%
Estimated Test Count: 15-20 tests
```

---

## Code Implementer Agent

**Purpose:** Write or modify code following MVVM patterns and project standards

### How to Use
```bash
# Triggered automatically in /fix-github-issues Phase 3a
# Or invoke manually for non-issue code:
claude code "Implement the following feature:
[Detailed requirements from Issue Analyzer]

Follow these patterns:
- Models: Pure data, no UI dependencies
- ViewModels: ObservableProperty, commands for user actions
- Views: XAML only, all logic in ViewModel
- Keep code readable and testable"
```

### Template: Add a New Feature

**Step 1: Define the Model** (if needed)
```csharp
// ScientificApp/Models/FeatureName.cs
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; }
}
```

**Step 2: Create ViewModel**
```csharp
// ScientificApp/ViewModels/FeatureViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;

public partial class FeatureViewModel : ObservableObject
{
    [ObservableProperty]
    private string input = string.Empty;

    [ObservableProperty]
    private string result = string.Empty;

    public void ProcessInput()
    {
        // Business logic here
        Result = Validate(Input);
    }
}
```

**Step 3: Update View**
```xaml
<!-- ScientificApp/Views/FeatureWindow.xaml -->
<Window ...>
    <StackPanel Margin="20">
        <TextBlock Text="Input:" FontWeight="Bold"/>
        <TextBox Text="{Binding Input}" Margin="0,5"/>
        <Button Content="Process" Click="{Binding ProcessInput}"/>
        <TextBlock Text="{Binding Result}" Foreground="Green" Margin="0,10"/>
    </StackPanel>
</Window>
```

### Common Patterns

#### Observable Property (Auto-notify UI)
```csharp
[ObservableProperty]
private string statusMessage = "Ready";

// UI gets notified automatically when StatusMessage changes
```

#### Command (Button click handler)
```csharp
[RelayCommand]
public void Submit()
{
    // This will be bound to Button.Command in XAML
}

// In XAML:
// <Button Command="{Binding SubmitCommand}"/>
```

#### Data Binding
```xaml
<!-- OneWay (View reads from ViewModel) -->
<TextBlock Text="{Binding Message}"/>

<!-- TwoWay (View and ViewModel sync) -->
<TextBox Text="{Binding Input, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"/>
```

---

## Test Validator Agent

**Purpose:** Ensure code quality, correctness, and compliance with patterns

### How to Use
```bash
# Triggered automatically via PostToolUse hook
# Or invoke manually:
claude code "Validate the following code for:
1. Compilation errors (run: dotnet build)
2. MVVM pattern compliance
3. Unit test coverage
4. Input validation and edge cases
5. XAML binding syntax"
```

### Manual Testing Checklist
```bash
# 1. Run build
dotnet build

# 2. Run tests (if tests exist)
dotnet test

# 3. Check formatting
dotnet format --verify-no-changes

# 4. Code review checklist
- [ ] Views have no C# business logic
- [ ] ViewModels use [ObservableProperty]
- [ ] Models are pure data structures
- [ ] All properties properly named
- [ ] Edge cases handled
- [ ] Input validated
```

### Example: Write Unit Tests
```csharp
// ScientificApp/Tests/LoginViewModelTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class LoginViewModelTests
{
    [TestMethod]
    public void ValidateEmail_WithValidEmail_ReturnsTrue()
    {
        // Arrange
        var viewModel = new LoginViewModel();

        // Act
        var result = viewModel.ValidateEmail("user@domain.com");

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void ValidateEmail_WithInvalidEmail_ReturnsFalse()
    {
        var viewModel = new LoginViewModel();
        var result = viewModel.ValidateEmail("invalid-email");
        Assert.IsFalse(result);
    }
}
```

### Validation Gates
```
✓ Code compiles (0 errors)
✓ All tests pass
✓ No MVVM violations
✓ Input validation present
✓ Build time < 60 seconds
```

---

## PR Creator Agent

**Purpose:** Finalize changes and create pull requests linked to issues

### How to Use
```bash
# Triggered automatically in /fix-github-issues Phase 4
# Or invoke manually:
claude code "Create a pull request with:
1. Feature branch: fix/issue-{number}
2. Commit message: 'fix: description (Fixes #{number})'
3. PR title: 'Fix: description'
4. PR body: Analysis + implementation summary
5. Link to issue"
```

### PR Template

**Title:**
```
Fix: Add email validation to LoginViewModel
```

**Body:**
```markdown
## Summary
Implements email validation in LoginViewModel with comprehensive test coverage.

## Changes
- Added EmailValidator class to Models/
- Updated LoginViewModel with email validation logic
- Added unit tests for validation edge cases
- Created XAML bindings in LoginWindow.xaml

## Test Plan
- [x] Unit tests pass (15 new tests)
- [x] Build succeeds (0 warnings)
- [x] Manual testing of validation UI
- [x] Edge case testing (special chars, max length)

## Fixes #35

Co-Authored-By: Claude Haiku 4.5 <noreply@anthropic.com>
```

### Commit Message Format
```
fix: Add email validation to LoginViewModel (Fixes #35)

- Validate email format using regex
- Display validation messages in UI
- Add unit tests for validation edge cases
- Update XAML bindings for error display

Co-Authored-By: Claude Haiku 4.5 <noreply@anthropic.com>
```

---

## Parallel Agent Workflows

### For Multiple Issues
```bash
# Fix multiple issues in sequence
/fix-github-issues
# Select: 35,42,51

# Agents work on #35 → #42 → #51
# Each issue goes through full workflow
```

### Specialist Sequences
```
Issue Analyzer
    ↓
Code Implementer
    ↓
Test Validator (via hook)
    ↓
Test Validator (manual)
    ↓
PR Creator
```

---

## Integration with Hooks

### Auto-Format Hook
```json
{
  "matcher": "Write|Edit",
  "command": "dotnet format --include {filePath}"
}
```
- **When:** After every file edit
- **Effect:** Automatically formats C# code
- **No interaction needed:** Silent if succeeds

### Auto-Build Hook
```json
{
  "matcher": "Write|Edit",
  "command": "dotnet build --configuration Debug"
}
```
- **When:** After C#/XAML/project file changes
- **Effect:** Validates build immediately
- **Timeout:** 60 seconds

---

## Agent Collaboration Example

### Scenario: Implement "Add progress bar to model execution"

```
┌─────────────────────────────────────────────────────┐
│ PHASE 1: Issue Analyzer                             │
│ Input:  GitHub Issue #50                             │
│ Output: Detailed requirements + test plan            │
└─────────────────────────────────────────────────────┘
                      ↓
┌─────────────────────────────────────────────────────┐
│ PHASE 2: Code Implementer                            │
│ Input:  Requirements from analyzer                   │
│ Output: Models/, ViewModels/, Views/ updated         │
└─────────────────────────────────────────────────────┘
                      ↓
              [Auto-Hook: Format]
              [Auto-Hook: Build]
                      ↓
┌─────────────────────────────────────────────────────┐
│ PHASE 3: Test Validator                              │
│ Input:  New code from implementer                    │
│ Output: Test suite + validation report               │
└─────────────────────────────────────────────────────┘
                      ↓
┌─────────────────────────────────────────────────────┐
│ PHASE 4: PR Creator                                  │
│ Input:  All changes + test results                   │
│ Output: Pull request linked to issue #50             │
└─────────────────────────────────────────────────────┘
```

---

## Quick Reference

| Task | Agent | Command |
|------|-------|---------|
| Understand issue | Issue Analyzer | (auto) |
| Write code | Code Implementer | (auto) |
| Run tests | Test Validator | `dotnet test` |
| Format code | (auto-hook) | (on save) |
| Build check | (auto-hook) | (on edit) |
| Create PR | PR Creator | (auto) |

---

## Troubleshooting

### Analyzer output is incomplete
**Fix:** Provide more context. Include full issue description and any related code.

### Implementer wrote code in wrong location
**Fix:** Correct the file path. Remind: `Models/`, `ViewModels/`, `Views/` structure.

### Validator found errors after code written
**Fix:** This is expected. Test Validator catches issues. Implementer fixes them.

### PR not auto-linking to issue
**Fix:** Ensure commit message includes `Fixes #N` format.

---

## Next Steps

1. Create a test issue on GitHub
2. Run `/fix-github-issues`
3. Let the agent team work through each phase
4. Review the auto-created PR
5. Merge after approval

All agents work together seamlessly with hooks, formatting, and validation!
