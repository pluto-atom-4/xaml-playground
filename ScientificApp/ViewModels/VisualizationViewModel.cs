using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot;
using ScientificApp.Models;

namespace ScientificApp.ViewModels;

/// <summary>
/// ViewModel for visualization tab.
/// Manages chart generation and display for regression model diagnostics.
/// </summary>
public partial class VisualizationViewModel : ObservableObject
{
    private readonly RegressionService _regressionService;
    private readonly SampleData _sampleData;

    [ObservableProperty]
    private PlotModel? residualsPlot;

    [ObservableProperty]
    private PlotModel? predictionsPlot;

    [ObservableProperty]
    private bool isGenerating;

    [ObservableProperty]
    private int selectedModelIndex = 0;

    [ObservableProperty]
    private string statusMessage = "Select a model and click Generate Charts";

    [ObservableProperty]
    private string residualStats = "";

    public ObservableCollection<string> AvailableModels { get; } = new();

    public VisualizationViewModel(RegressionService regressionService, SampleData sampleData)
    {
        _regressionService = regressionService ?? throw new ArgumentNullException(nameof(regressionService));
        _sampleData = sampleData ?? throw new ArgumentNullException(nameof(sampleData));

        // Populate available models
        for (int i = 0; i < _regressionService.GetModelCount(); i++)
        {
            var name = _regressionService.GetModelName(i);
            AvailableModels.Add(name);
        }
    }

    /// <summary>
    /// Command to generate both residuals and predictions charts.
    /// </summary>
    [RelayCommand]
    public async Task GenerateCharts()
    {
        try
        {
            IsGenerating = true;
            StatusMessage = "Generating charts...";

            await Task.Run(() =>
            {
                // Get current dataset
                var currentData = _sampleData.GetCurrentData();
                if (currentData.Count == 0)
                {
                    StatusMessage = "No data loaded. Please select a dataset first.";
                    return;
                }

                // Check if models have been trained
                var modelCount = _regressionService.GetModelCount();
                if (modelCount == 0)
                {
                    StatusMessage = "No models available. Please train models first.";
                    return;
                }

                // Get selected model
                if (SelectedModelIndex < 0 || SelectedModelIndex >= modelCount)
                {
                    StatusMessage = "Invalid model selection.";
                    return;
                }

                var model = _regressionService.GetModel(SelectedModelIndex);
                var modelName = _regressionService.GetModelName(SelectedModelIndex);

                // Generate predictions
                var predictions = currentData.Select(dp => model.Predict(dp.X)).ToList();

                // Generate charts
                ResidualsPlot = VisualizationModel.GenerateResidualsPlot(currentData, predictions, modelName);
                PredictionsPlot = VisualizationModel.GeneratePredictionsCurve(currentData, predictions, modelName);

                // Calculate and display residual statistics
                var residuals = VisualizationModel.CalculateResiduals(currentData, predictions);
                var stats = VisualizationModel.CalculateResidualStats(residuals);

                ResidualStats = FormatResidualStats(stats);
                StatusMessage = $"Charts generated for {modelName}";
            });
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            ResidualsPlot = null;
            PredictionsPlot = null;
        }
        finally
        {
            IsGenerating = false;
        }
    }

    /// <summary>
    /// Format residual statistics for display.
    /// </summary>
    private static string FormatResidualStats(Dictionary<string, double> stats)
    {
        var lines = new List<string>
        {
            "Residual Statistics:",
            $"  Mean: {stats["Mean"]:F6}",
            $"  Std Dev: {stats["StdDev"]:F6}",
            $"  Min: {stats["Min"]:F6}",
            $"  Max: {stats["Max"]:F6}",
            $"  MAE: {stats["MAE"]:F6}"
        };

        return string.Join(Environment.NewLine, lines);
    }
}
