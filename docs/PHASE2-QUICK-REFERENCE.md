# Phase 2 Quick Reference

## Dashboard Workflow

```
┌─────────────────────────────────────────────────────────────┐
│ Scientific App - Regression Model Comparison (Phase 2)     │
├──────────────┬──────────────────────┬──────────────────────┤
│ LEFT PANEL   │ CENTER PANEL        │ RIGHT PANEL          │
├──────────────┼──────────────────────┼──────────────────────┤
│ 1. DATASET   │ Visualization        │ 3. METRICS           │
│ ✓ Linear     │ (Phase 3)            │ ┌────────────────┐   │
│ ✓ Quadratic  │                      │ │ Best: Linear   │   │
│ ✓ Cubic      │                      │ ├────────────────┤   │
│              │                      │ │ Model    AIC   │   │
│ 2. MODELS    │                      │ │ Linear   120   │   │
│ ☑ Linear     │                      │ │ Poly(2)  125   │   │
│ ☑ Poly(d=2)  │                      │ │ Poly(3)  130   │   │
│ ☑ Poly(d=3)  │                      │ └────────────────┘   │
│              │                      │ (sorted by AIC)      │
│ [TRAIN]      │                      │                      │
└──────────────┴──────────────────────┴──────────────────────┘
```

## Models Layer

| Class | Purpose | Key Methods |
|-------|---------|------------|
| DataPoint | (X, Y) sample | constructor(x, y) |
| RegressionModel | Base class | Fit(), Predict(), GetAIC(), GetR², GetRMSE() |
| LinearRegression | Y = a + bX | Fit() → solves via normal equations |
| PolynomialRegression | Y = polynomial | Fit() → Gaussian elimination |
| ModelMetrics | Results container | String representations |
| RegressionService | Orchestrator | TrainModels(), GetAllModels() |
| SampleData | Dataset generator | GenerateLinearData(), GenerateQuadraticData() |

## ViewModels Layer

| Class | Properties | Commands |
|-------|-----------|----------|
| DatasetViewModel | DatasetInfo, DataPointCount, IsLoading | LoadLinearData, LoadQuadraticData, LoadCubicData |
| RegressionViewModel | AvailableModels, IsTraining, StatusMessage | TrainModels |
| MetricsViewModel | MetricsTable, BestModel, BestAIC | UpdateMetrics() |
| MainViewModel | StatusMessage | TrainAndCompare |

## Key Algorithms

### Linear Regression (Least Squares)
```
slope = Σ(x - x̄)(y - ȳ) / Σ(x - x̄)²
intercept = ȳ - slope * x̄
```

### Polynomial Regression (Gaussian Elimination)
```
Solve: (X^T * X) * a = X^T * y
Where X = [1, x, x², ..., x^d] (Vandermonde matrix)
```

### AIC (Akaike Information Criterion)
```
AIC = n * ln(MSE) + 2*k
where k = number of parameters
Lower AIC = Better model (balances fit quality with complexity)
```

### Metrics
```
R² = 1 - (SS_res / SS_tot)  // Goodness of fit (0-1)
RMSE = √(MSE)                // Prediction error
MSE = Σ(y - ŷ)² / n          // Mean squared error
```

## Testing Checklist

- [ ] Click "Linear Data" → Point count updates to 50
- [ ] Verify Linear Regression checkbox auto-selected
- [ ] Click "TRAIN MODELS" → Status shows "Training complete"
- [ ] Metrics table shows 3 models sorted by AIC
- [ ] Click "Quadratic Data" → Switch models to Polynomial(d=2)
- [ ] Retrain → Verify Poly(d=2) has lowest AIC
- [ ] Load "Cubic Data" → Try Polynomial(d=3)
- [ ] Verify cubic fits best with degree-3 polynomial

## Best Fits (Expected Results)

| Dataset | Best Model | Why |
|---------|-----------|-----|
| Linear Data | Linear Regression | Matches true Y = 2 + 0.5X |
| Quadratic Data | Polynomial (d=2) | Matches Y = 1 + 0.1X + 0.05X² |
| Cubic Data | Polynomial (d=3) | Matches Y = 0.01X³ - 0.5X |

## User Stories Implemented

1. ✅ Load and explore data (3 synthetic datasets)
2. ✅ Select and configure models (checkboxes)
3. ✅ Train and compare models (async button)
4. ✅ View performance metrics (sorted DataGrid)
5. ✅ Understand complexity trade-offs (AIC penalty visible)
6. ✅ Iterate and experiment (no reloads needed)

## Files Created

**Models (7 files):**
- DataPoint.cs
- RegressionModel.cs
- LinearRegression.cs
- PolynomialRegression.cs
- ModelMetrics.cs
- RegressionService.cs
- SampleData.cs

**ViewModels (4 files):**
- MainViewModel.cs (updated)
- DatasetViewModel.cs
- RegressionViewModel.cs
- MetricsViewModel.cs

**Views (1 file):**
- MainWindow.xaml (updated)

**Docs (3 files):**
- PHASE2-SUMMARY.md (this file)
- PHASE2-IMPLEMENTATION.md
- PHASE2-USER-STORIES.md

## Build Status

```
✅ dotnet build: SUCCESS
   - 0 errors
   - 0 warnings
   - Output: ScientificApp.dll (net8.0-windows)
```

## Next Steps (Phase 3)

- [ ] Add charting library (OxyPlot or Live Charts)
- [ ] Visualize fitted curves vs. actual data
- [ ] Add residuals plot
- [ ] Integrate Python backend (FastAPI or Python.NET)
- [ ] Add CSV upload functionality
- [ ] Extend algorithm library

---

**Phase 2 Complete!** Dashboard ready for user testing and Phase 3 enhancements.
