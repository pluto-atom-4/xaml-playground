using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScientificApp.Models;

namespace ScientificApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    // Expose services for all ViewModels
    public RegressionService RegressionService { get; } = new();
    public SampleData SampleData { get; } = new();
    
    public DatasetViewModel DatasetViewModel { get; }
    public RegressionViewModel RegressionViewModel { get; }
    public MetricsViewModel MetricsViewModel { get; } = new();

    [ObservableProperty]
    private string _statusMessage = "Phase 2 & 3: Regression Analysis & Visualization";

    public MainViewModel()
    {
        // Create ViewModels with shared services
        DatasetViewModel = new DatasetViewModel(SampleData);
        RegressionViewModel = new RegressionViewModel(RegressionService);
        
        // Wire up workflow: when dataset loads, enable regression training
        // This will be enhanced in Phase 3 with async notifications
    }

    [RelayCommand]
    private async Task TrainAndCompare()
    {
        var data = DatasetViewModel.GetData();
        if (data.Count == 0)
        {
            StatusMessage = "Error: Please load a dataset first";
            return;
        }

        RegressionViewModel.SetData(data);
        await RegressionViewModel.TrainModelsCommand.ExecuteAsync(null);

        if (RegressionViewModel.TrainedMetrics != null)
        {
            MetricsViewModel.UpdateMetrics(RegressionViewModel.TrainedMetrics);
            StatusMessage = "Training complete. See metrics below.";
        }
    }
}