# Quick Start: Agent Team for GitHub Issue Fixing

Your Scientific App project is now configured with a specialized agent team for automated issue fixing. Here's how to get started.

---

## Prerequisites

### 1. GitHub CLI Authentication
```bash
gh auth status
# If not authenticated, run:
gh auth login
```

### 2. Git Configuration
```bash
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"
```

### 3. Verify Project Setup
```bash
cd C:\Users\nobu\RiderProjects\ScientificApp

# Check build works
dotnet build

# Check git is configured
git log --oneline | head -3

# Check formatting works
dotnet format --verify-no-changes
```

---

## Step 1: Create Test Issues (Optional)

If you don't have GitHub issues yet, create some:

```bash
# Create a test issue via GitHub web UI or CLI:
gh issue create --title "Add unit tests for MainViewModel" \
  --body "Create comprehensive unit tests for MainViewModel with 80% code coverage"

gh issue create --title "Implement dark mode support" \
  --body "Add theme switching capability to the application"
```

---

## Step 2: Run the Fix GitHub Issues Workflow

```bash
# Invoke the skill
/fix-github-issues
```

**What happens:**
1. **Preprocessing (automatic):**
   - Checks out main branch
   - Syncs with remote
   - Lists open issues

2. **Issue Selection (interactive):**
   ```
   OPEN ISSUES:
   ============================================================
   35 | Add unit tests for MainViewModel
   42 | Implement dark mode support
   ============================================================

   [SELECT] Which issues would you like to fix?
   Enter selection: 35
   ```

---

## Step 3: Agent Team Takes Over

The agents automatically handle each phase:

### Phase 1: Issue Analysis
**Issue Analyzer Agent** reads your issue and generates:
- Problem decomposition
- MVVM component breakdown
- Acceptance criteria
- Test cases

### Phase 2: Code Implementation
**Code Implementer Agent** creates:
- `Models/` — Pure business logic
- `ViewModels/` — Observable properties & commands
- `Views/` — XAML UI with bindings

### Phase 3: Validation (Automatic Hooks)
**Hooks auto-run:**
1. `dotnet format` — Formats code automatically
2. `dotnet build` — Validates compilation

**Test Validator Agent** verifies:
- Build succeeds (0 errors)
- MVVM patterns followed
- Tests pass
- Input validation present

### Phase 4: Create PR
**PR Creator Agent:**
- Creates feature branch: `fix/issue-35`
- Commits with message: `fix: ... (Fixes #35)`
- Creates pull request linked to issue

---

## Step 4: Review and Merge

The PR is created automatically with:
- ✓ Linked issue
- ✓ Tested code
- ✓ Formatted correctly
- ✓ Full description

Review on GitHub and merge when ready.

---

## Example: Complete Workflow

```bash
# 1. Invoke skill
/fix-github-issues

# 2. Preprocessing runs automatically
[GIT] Syncing with main branch...
   -> Checked out main
   -> Synced with remote

# 3. Select issue
OPEN ISSUES:
35 | Add unit tests for MainViewModel

Enter selection: 35

# 4. Issue Analyzer runs
[ANALYZING] Issue #35: Add unit tests for MainViewModel
├─ Affected: ViewModels/MainViewModel.cs
├─ Requirements: 80% code coverage, 15+ tests
└─ Test Strategy: AAA pattern (Arrange, Act, Assert)

# 5. Code Implementer runs
[IMPLEMENTING] Writing tests for MainViewModel...
   -> Created Tests/MainViewModelTests.cs
   -> Added 18 test methods
   -> Mocked dependencies

# 6. Hooks auto-run
[AUTO-HOOK] Formatting code with dotnet format...
[AUTO-HOOK] Validating build with dotnet build...
✓ Build succeeded (0 errors, 0 warnings)

# 7. Test Validator runs
[VALIDATING] Running unit tests...
✓ All 18 tests passed
✓ Code coverage: 82%
✓ No MVVM violations

# 8. PR Creator runs
[CREATING PR] Creating pull request...
✓ Branch: fix/issue-35
✓ Commit: fix: Add unit tests for MainViewModel (Fixes #35)
✓ PR created: https://github.com/nobu/ScientificApp/pull/123

# 9. Done!
[SUCCESS] Issue #35 fixed and PR created!
Check your GitHub repo to review and merge.
```

---

## Configuration Files

Your project now has:

### `.claude/settings.json`
```json
{
  "hooks": {
    "PostToolUse": [
      {
        "matcher": "Write|Edit",
        "command": "dotnet format --include {filePath}"
      },
      {
        "matcher": "Write|Edit",
        "command": "dotnet build --configuration Debug"
      }
    ]
  },
  "permissions": {
    "allow": [
      "Bash(git:*)",
      "Bash(dotnet:*)",
      "Bash(gh:*)",
      "Read", "Write", "Edit", "Glob", "Grep"
    ]
  }
}
```

### `CLAUDE.md`
Comprehensive development guide with MVVM patterns, build commands, and architecture.

### `docs/agent-workflow-guide.md`
Detailed explanation of the 4-phase workflow and agent responsibilities.

### `docs/agent-runbooks.md`
Specialized prompts and templates for each agent.

---

## Common Commands

```bash
# Build the project
dotnet build

# Run the app
dotnet run --project ScientificApp/ScientificApp.csproj

# Format code manually
dotnet format

# Run tests (if tests exist)
dotnet test

# Fix a GitHub issue
/fix-github-issues

# Check GitHub CLI status
gh auth status

# View open issues
gh issue list
```

---

## Tips & Tricks

### 1. Multiple Issues at Once
```
[SELECT] Which issues would you like to fix?
Enter selection: 35,42,51
```
Agents work on each issue sequentially, creating separate PRs.

### 2. Disable Auto-Hooks Temporarily
Edit `.claude/settings.json` and comment out hook entries:
```json
"disableAllHooks": true
```

### 3. Inspect Auto-Generated Code
The Code Implementer creates industry-standard MVVM code:
- Readable and maintainable
- Follows C# conventions
- Test-friendly

### 4. Customize Agent Behavior
Use specialized prompts from `docs/agent-runbooks.md` for:
- Different code styles
- Specific requirements
- Complex scenarios

### 5. Manual Override
If you disagree with agent output:
1. Edit the code yourself
2. Run `dotnet format` to reformat
3. Run `dotnet build` to validate
4. Commit manually: `git commit -am "fix: description"`

---

## Workflow Customization

### Add More Specialized Agents
Create agents for:
- **Code Reviewer** — Peer review of agent-written code
- **Documentation** — Auto-generate code comments
- **Performance** — Profile and optimize hot paths
- **Security** — Audit for vulnerabilities

### Extend Hooks
Add more post-tool-use hooks:
```json
"hooks": {
  "PostToolUse": [
    { "matcher": "Bash(dotnet test)", "command": "..." },
    { "matcher": "Edit(*.xaml)", "command": "xaml-validator" }
  ]
}
```

---

## Troubleshooting

### "gh: command not found"
**Solution:** Install GitHub CLI: https://cli.github.com

### "Not authenticated with GitHub"
**Solution:** Run `gh auth login`

### "Branch already exists"
**Solution:** The agent automatically handles this. If manual branch exists, delete it:
```bash
git branch -D fix/issue-35
```

### "Build validation failed"
**Solution:** The agent will detect and report. Fix the error, save again, and hooks re-run.

### "No open issues found"
**Solution:** Create issues first via GitHub web UI or `gh issue create`

---

## Next Steps

1. ✓ Set up GitHub CLI authentication
2. ✓ Create test GitHub issues (optional)
3. ✓ Run `/fix-github-issues`
4. ✓ Let the agent team handle the workflow
5. ✓ Review and merge the auto-created PR
6. ✓ Repeat for the next issue!

---

## Resources

- **CLAUDE.md** — Project guidelines and setup
- **agent-workflow-guide.md** — Detailed phase-by-phase workflow
- **agent-runbooks.md** — Agent prompts and templates
- **docs/start-from-here.md** — Learning phases for WPF/MVVM

---

## Getting Help

If something doesn't work:
1. Check `.claude/settings.json` for hooks configuration
2. Verify `gh auth status` shows you're logged in
3. Run `dotnet build` manually to diagnose issues
4. Check the agent output for specific error messages

Your agent team is ready! 🚀

Start with: `/fix-github-issues`
