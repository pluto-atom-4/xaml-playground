using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScientificApp.Models;

namespace ScientificApp.ViewModels;

public partial class DatasetViewModel : ObservableObject
{
    private List<DataPoint> _currentData = [];

    [ObservableProperty]
    private int dataPointCount;

    [ObservableProperty]
    private string datasetInfo = "No dataset loaded";

    [ObservableProperty]
    private bool isLoading;

    public List<DataPoint> GetData() => new(_currentData);

    [RelayCommand]
    private async Task LoadLinearData()
    {
        await Task.Run(() =>
        {
            IsLoading = true;
            _currentData = SampleData.GenerateLinearData(50);
            DataPointCount = _currentData.Count;
            DatasetInfo = "Linear data: Y = 2 + 0.5*X + noise (50 points)";
        });
        IsLoading = false;
    }

    [RelayCommand]
    private async Task LoadQuadraticData()
    {
        await Task.Run(() =>
        {
            IsLoading = true;
            _currentData = SampleData.GenerateQuadraticData(50);
            DataPointCount = _currentData.Count;
            DatasetInfo = "Quadratic data: Y = 1 + 0.1*X + 0.05*X² + noise (50 points)";
        });
        IsLoading = false;
    }

    [RelayCommand]
    private async Task LoadCubicData()
    {
        await Task.Run(() =>
        {
            IsLoading = true;
            _currentData = SampleData.GenerateCubicData(50);
            DataPointCount = _currentData.Count;
            DatasetInfo = "Cubic data: Y = 0.01*X³ - 0.5*X + noise (50 points)";
        });
        IsLoading = false;
    }
}
