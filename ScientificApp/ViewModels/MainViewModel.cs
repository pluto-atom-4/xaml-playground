using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ScientificApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public DatasetViewModel DatasetViewModel { get; } = new();
    public RegressionViewModel RegressionViewModel { get; } = new();
    public MetricsViewModel MetricsViewModel { get; } = new();

    [ObservableProperty]
    private string _statusMessage = "Phase 2: Regression Model Comparison";

    public MainViewModel()
    {
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