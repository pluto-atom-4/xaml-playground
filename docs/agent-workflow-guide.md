# Agent Team Workflow for GitHub Issue Fixing

This guide explains how the specialized agent team integrates with the `/fix-github-issues` skill to automate the scientific app development workflow.

---

## Agent Team Structure

### 1. Issue Analyzer Agent
**Role:** Understand and decompose GitHub issues
**Responsibilities:**
- Parse issue descriptions and translate them to requirements
- Identify which part of the MVVM architecture needs changes (Model, ViewModel, View)
- Extract acceptance criteria and test requirements
- Flag edge cases or input validation needs

**Trigger:** After issue selection in `/fix-github-issues` Phase 2

---

### 2. Code Implementer Agent
**Role:** Implement fixes following MVVM patterns
**Responsibilities:**
- Write C# code following the project structure
- Create or update Models for business logic
- Create/update ViewModels with `[ObservableProperty]` attributes
- Create/update XAML Views with proper binding syntax
- Maintain separation of concerns (keep Views thin, logic in ViewModels)

**Trigger:** Phase 3 - after issue analysis

---

### 3. Test Validator Agent
**Role:** Ensure quality and correctness
**Responsibilities:**
- Run `dotnet build` to catch compilation errors
- Create unit tests for Model and ViewModel logic
- Verify input validation and edge cases
- Check for MVVM pattern violations
- Validate XAML binding syntax

**Trigger:** After code implementation (PostToolUse hook)

---

### 4. PR Creator Agent
**Role:** Finalize and submit changes
**Responsibilities:**
- Create feature branch: `fix/issue-{number}`
- Run final validation build
- Write descriptive commit message: `fix: description (Fixes #{number})`
- Create pull request with linked issue
- Generate PR description from issue analysis

**Trigger:** After tests pass

---

## Workflow Phases

### Phase 1: Preprocessing (Automatic)
```bash
bash ~/.claude/skills/fix-github-issues/scripts/preprocess.sh .
```

Outputs:
- Syncs with remote main branch
- Lists open GitHub issues with numbers and titles

**Agent involvement:** None (fully automated by the skill)

---

### Phase 2: Issue Selection & Analysis
```
[SELECT] Which issues would you like to fix?
Enter selection: 35,42
```

**Agent involvement:**
1. **Issue Analyzer** reads the selected issue(s)
2. Breaks down:
   - Problem statement
   - MVVM component(s) affected
   - Input/output requirements
   - Test cases needed

**Example output:**
```
Issue #35: "Add form validation to LoginView"

Analysis:
- **Affected:** ViewModel (LoginViewModel) + View (LoginWindow.xaml)
- **Requirement:** Validate email format, password length
- **Test Case:** EmailValidator should reject invalid formats
- **Edge Cases:** Empty strings, special characters, max length
```

---

### Phase 3: Implementation

#### Step 3a: Code Implementation
**Code Implementer Agent** creates/modifies:

**Model (if needed):** Pure data structures
```csharp
public class ValidationResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; }
}
```

**ViewModel:** Observable properties and commands
```csharp
[ObservableProperty]
private string email = string.Empty;

[ObservableProperty]
private string validationMessage = string.Empty;

public void ValidateEmail()
{
    var result = EmailValidator.Validate(Email);
    ValidationMessage = result.IsValid ? "Valid" : result.ErrorMessage;
}
```

**View:** XAML binding to ViewModel
```xaml
<TextBlock Text="{Binding ValidationMessage}" Foreground="Red"/>
<Button Command="{Binding ValidateCommand}" Content="Submit"/>
```

#### Step 3b: Hooks Auto-Execute
**Automatic triggers (no agent):**
1. `PostToolUse` hook runs `dotnet format` after Write/Edit
2. `PostToolUse` hook runs `dotnet build` to catch errors

#### Step 3c: Testing & Validation
**Test Validator Agent:**
- Runs unit tests for validation logic
- Verifies no binding errors in XAML
- Checks input edge cases
- Validates build succeeds with 0 warnings

**Example test:**
```csharp
[TestMethod]
public void ValidateEmail_WithInvalidEmail_ReturnsFalse()
{
    var result = EmailValidator.Validate("invalid-email");
    Assert.IsFalse(result.IsValid);
}
```

---

### Phase 4: Commit & PR Creation

**PR Creator Agent:**
1. Creates feature branch: `fix/issue-35`
2. Stages changes: `git add .`
3. Commits with message:
   ```
   fix: Add form validation to LoginView (Fixes #35)

   - Validate email format and password length
   - Display validation messages in UI
   - Add unit tests for EmailValidator

   Co-Authored-By: Claude Haiku 4.5 <noreply@anthropic.com>
   ```
4. Creates PR with:
   - Title: "Fix: Add form validation to LoginView"
   - Body: Issue analysis + implementation summary
   - Linked issue: `Fixes #35`

---

## Example Workflow in Action

### Scenario: Fix #42 "Add dark mode support"

```
[PHASE 2] Issue Analysis
Issue #42: Add dark mode support

Issue Analyzer Output:
├─ Problem: App should have light/dark theme toggle
├─ Affected: App.xaml (resources), MainViewModel, MainWindow.xaml
├─ Requirements:
│  ├─ Add ThemeMode property to ViewModel
│  ├─ Create theme resource dictionaries
│  └─ Update UI to respond to theme changes
└─ Tests Needed: Theme toggle functionality

[PHASE 3a] Code Implementation
Code Implementer:
├─ Models/Theme.cs (enum: Light, Dark)
├─ ViewModels/MainViewModel.cs (ThemeMode property)
└─ Views/App.xaml (theme resources + binding)

[Auto-Hook] Format & Build
✓ Code formatted with dotnet format
✓ Build succeeded (0 errors, 0 warnings)

[PHASE 3b] Testing
Test Validator:
├─ Unit tests for theme switching
├─ Visual check of theme resources
└─ Build validation passed

[PHASE 4] Commit & PR
PR Creator:
✓ Branch created: fix/issue-42
✓ Commit: "fix: Add dark mode support (Fixes #42)"
✓ PR created with full analysis and test results
```

---

## Integration with Settings

### Hooks Configuration
The `.claude/settings.json` automatically:
- Formats code after every Write/Edit (via `dotnet format`)
- Validates build after code changes (via `dotnet build`)

### Permissions
Pre-configured for issue-fixing workflow:
- `git:*` — All git operations (branch, commit, push)
- `dotnet:*` — Build, test, format
- `gh:*` — GitHub CLI (for PR creation)
- `Read/Write/Edit/Glob/Grep` — Code inspection and modification

---

## When to Use Each Agent

| Scenario | Agent(s) | Command |
|----------|----------|---------|
| Understand new issue | Issue Analyzer | (auto in Phase 2) |
| Write/update code | Code Implementer | (auto in Phase 3a) |
| Run tests/validation | Test Validator | (auto via hooks) |
| Create PR | PR Creator | (auto in Phase 4) |
| Review code quality | Test Validator | `/simplify` (optional) |
| Fix issues manually | All agents | `/fix-github-issues` |

---

## Best Practices

1. **Keep issues focused** — One feature per issue, easier to test
2. **Follow MVVM strictly** — Views should never contain logic
3. **Unit test first** — Write tests before implementation
4. **Use descriptive commits** — "fix: what" not "fixed thing"
5. **One agent at a time** — Let each phase complete before proceeding
6. **Check build always** — Hooks auto-validate, but verify manually too

---

## Troubleshooting

### Issue: Build validation fails in hook
**Cause:** Syntax error in code
**Solution:** Test Validator agent will catch it. Fix the error and re-save.

### Issue: Format hook removes important code
**Cause:** Rare edge case with complex expressions
**Solution:** Disable hook temporarily if needed: check `.claude/settings.json`

### Issue: GitHub CLI not authenticated
**Cause:** `gh` not logged in
**Solution:** Run `gh auth login` before using `/fix-github-issues`

### Issue: PR not linking to issue
**Cause:** Commit message missing `Fixes #N`
**Solution:** PR Creator agent handles this automatically

---

## Next Steps

1. Create test GitHub issues (`/fix-github-issues` will show them)
2. Try fixing an issue: `/fix-github-issues`
3. Let the agent team handle each phase
4. Review and merge the auto-created PR

For detailed phase information, see [`fix-github-issues` skill documentation](../.claude/skills/fix-github-issues/references/workflow.md).
