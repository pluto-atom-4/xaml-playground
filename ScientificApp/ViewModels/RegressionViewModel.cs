using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScientificApp.Models;
using System.Collections.ObjectModel;

namespace ScientificApp.ViewModels;

public partial class RegressionViewModel : ObservableObject
{
    private readonly RegressionService _service;
    private List<DataPoint> _currentData = [];

    [ObservableProperty]
    private ObservableCollection<ModelOption> availableModels = [];

    [ObservableProperty]
    private bool isTraining;

    [ObservableProperty]
    private string statusMessage = "Select models and click Train";

    public RegressionViewModel(RegressionService regressionService)
    {
        _service = regressionService ?? throw new ArgumentNullException(nameof(regressionService));
        InitializeModels();
    }

    private void InitializeModels()
    {
        var models = _service.GetAllModels();
        AvailableModels = new ObservableCollection<ModelOption>(
            models.Select(m => new ModelOption { Name = m.Name, IsSelected = m.Name == "Linear Regression" }).ToList()
        );
    }

    public void SetData(List<DataPoint> data)
    {
        _currentData = new List<DataPoint>(data);
    }

    [RelayCommand]
    private async Task TrainModels()
    {
        if (_currentData.Count == 0)
        {
            StatusMessage = "Error: No data loaded";
            return;
        }

        IsTraining = true;
        StatusMessage = "Training models...";

        try
        {
            var selectedModels = AvailableModels.Where(m => m.IsSelected).Select(m => m.Name).ToList();
            if (selectedModels.Count == 0)
            {
                StatusMessage = "Error: Select at least one model";
                IsTraining = false;
                return;
            }

            await Task.Run(() =>
            {
                var metrics = _service.TrainModels(_currentData, selectedModels);
                // Store metrics for MetricsViewModel to use
                TrainedMetrics = metrics;
            });

            StatusMessage = $"Training complete: {AvailableModels.Count(m => m.IsSelected)} model(s) trained";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsTraining = false;
        }
    }

    public List<ModelMetrics>? TrainedMetrics { get; set; }
}

public class ModelOption : ObservableObject
{
    private bool _isSelected;

    public string Name { get; set; } = "";

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }
}
