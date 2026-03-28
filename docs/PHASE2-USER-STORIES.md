# Phase 2 User Stories: Regression Model Selection

## Context
**Persona:** Data Scientist / Analytical Manager  
**Goal:** Quickly compare regression models to find the best fit for analytical data  
**CS109x Reference:** Lecture 7-8 (Model Selection via AIC)

---

## User Story 1: Load and Explore Data

**As a** data analyst  
**I want to** load different datasets to see how model performance varies  
**So that** I can understand which models work best for different data types

### Acceptance Criteria
- ✅ User can click "Linear Data" to load 50 synthetic linear points (Y = 2 + 0.5X + noise)
- ✅ User can click "Quadratic Data" to load 50 synthetic quadratic points (Y = 1 + 0.1X + 0.05X²)
- ✅ User can click "Cubic Data" to load 50 synthetic cubic points (Y = 0.01X³ - 0.5X)
- ✅ Point count updates in the UI (shows "Points: 50")
- ✅ Dataset description displays current selection
- ✅ UI remains responsive during data loading

### Implemented
✅ DatasetViewModel with 3 load commands  
✅ SampleData generates reproducible synthetic datasets  
✅ Status messages show current dataset info

---

## User Story 2: Select and Configure Models

**As a** regression analyst  
**I want to** choose which regression models to train  
**So that** I can focus on relevant comparisons and avoid unnecessary computation

### Acceptance Criteria
- ✅ User sees checkboxes for "Linear Regression", "Polynomial (degree 2)", "Polynomial (degree 3)"
- ✅ Linear Regression is selected by default
- ✅ User can toggle any model on/off
- ✅ At least one model must be selected before training
- ✅ Training button is visible and enabled

### Implemented
✅ RegressionViewModel with ObservableCollection of ModelOption objects  
✅ XAML checkboxes bound to IsSelected property  
✅ Default Linear Regression pre-checked

---

## User Story 3: Train and Compare Models

**As a** data scientist  
**I want to** train all selected models on the current dataset with a single click  
**So that** I can quickly evaluate which model performs best

### Acceptance Criteria
- ✅ "TRAIN MODELS" button is visible and clickable
- ✅ Clicking triggers async training (UI doesn't freeze)
- ✅ Status message updates: "Training models..." → "Training complete"
- ✅ Results appear in metrics table within 1-2 seconds
- ✅ Error message if no dataset is loaded
- ✅ Error message if no models are selected

### Implemented
✅ TrainAndCompareCommand coordinates workflow  
✅ RegressionService.TrainModels runs async  
✅ IsBusy flag prevents double-clicks  
✅ Error handling with user-friendly messages

---

## User Story 4: View Model Performance Metrics

**As a** an analytical professional  
**I want to** see model performance ranked by AIC score  
**So that** I can quickly identify the best model for my data

### Acceptance Criteria
- ✅ Metrics table shows: Model name, AIC, R², RMSE, Parameter count
- ✅ Models are sorted by AIC (lowest first = best)
- ✅ "Best" model highlighted at top of table
- ✅ All numeric values displayed with appropriate precision (AIC: 2 decimals, R²: 4 decimals)
- ✅ Helper text explains "Lower AIC = Better model"
- ✅ Table updates when user trains new models

### Implemented
✅ MetricsViewModel with DataGrid binding  
✅ ModelMetrics sorted by AIC ascending  
✅ BestModel property updates after training  
✅ ModelMetricsDisplay with formatted display properties

---

## User Story 5: Understand Model Complexity Trade-offs

**As a** a student learning model selection  
**I want to** see how adding model parameters affects both fit quality and AIC score  
**So that** I understand the bias-variance tradeoff and Akaike penalty for complexity

### Acceptance Criteria
- ✅ Parameter count visible for each model (Linear: 2, Poly-d2: 3, Poly-d3: 4)
- ✅ Polynomial models have higher AIC penalty despite potentially lower RMSE
- ✅ User can observe that overfitting (too many parameters) increases AIC
- ✅ Example: Cubic data with Polynomial(d=3) balances lower RMSE with AIC penalty

### Implemented
✅ RegressionService calculates correct parameter counts  
✅ AIC includes 2*k penalty term  
✅ DataGrid displays Parameters column  
✅ Real observable behavior matches statistical theory

---

## User Story 6: Iterate and Experiment

**As a** an analytical researcher  
**I want to** easily switch between datasets and model combinations  
**So that** I can experiment and build intuition about model selection

### Acceptance Criteria
- ✅ No page reloads between experiments
- ✅ Can load new dataset at any time
- ✅ Can change model selections at any time
- ✅ Can re-train without restarting application
- ✅ Previous results clear when new training starts
- ✅ Workflow is predictable: Load → Select → Train → Compare

### Implemented
✅ All commands work in any order  
✅ No page navigation (single-window dashboard)  
✅ Metrics table updates on each training  
✅ Clear workflow guidance in UI

---

## Phase 2 Learning Objectives (Addressed)

| Objective | Implementation | User Story |
|-----------|----------------|------------|
| Translate requirements to user stories | 6 user stories defined | ✅ All |
| Data-intensive UX design | DataGrid, 3-panel layout | US #4 |
| Model comparison workflow | Load → Select → Train → Compare | US #6 |
| Real-time feedback | Status messages, async operations | US #3 |
| Educational value | AIC penalty visible, parameter counts shown | US #5 |
| Professional polish | Colored UI, sorted results, helper text | US #4 |

---

## Next Phase (Phase 3)

These user stories set foundation for Phase 3 enhancements:
- **US #1 Enhancement:** "I want to upload my own CSV data"
- **US #4 Enhancement:** "I want to visualize the fitted curve vs. actual data"
- **US #5 Enhancement:** "I want to see residuals plot"
- **New US:** "I want Python models integrated for complex algorithms"

---

## Validation

All user stories have been **implemented and tested**:
- Application launches and displays 3-panel dashboard
- Datasets load and point count updates
- Models can be selected/deselected
- Training button works and updates metrics
- Results display in sorted table
- Experimentation workflow is fluid and responsive

**Result:** Phase 2 requirements met. User can translate analytical problem (model selection via AIC) into actionable dashboard workflow. ✅
