using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using ScientificApp.Models;

namespace ScientificApp.ViewModels;

public partial class DatasetViewModel : ObservableObject
{
    private readonly SampleData _sampleData;
    private List<Models.DataPoint> _currentData = [];

    [ObservableProperty]
    private int dataPointCount;

    [ObservableProperty]
    private string datasetInfo = "No dataset loaded";

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private PlotModel? previewPlotModel;

    public DatasetViewModel(SampleData sampleData)
    {
        _sampleData = sampleData ?? throw new ArgumentNullException(nameof(sampleData));
    }

    public List<Models.DataPoint> GetData() => new(_currentData);

    /// <summary>
    /// Generate a scatter plot preview of the dataset.
    /// </summary>
    private PlotModel GeneratePreviewChart(List<Models.DataPoint> data)
    {
        var plotModel = new PlotModel
        {
            Title = "Data Preview",
            Background = OxyColors.White
        };

        if (data.Count == 0)
            return plotModel;

        // Create scatter series
        var scatterSeries = new ScatterSeries
        {
            Title = "Data Points",
            MarkerType = MarkerType.Circle,
            MarkerSize = 5,
            MarkerFill = OxyColor.FromRgb(100, 150, 200)
        };

        foreach (var point in data)
        {
            scatterSeries.Points.Add(new ScatterPoint(point.X, point.Y));
        }

        plotModel.Series.Add(scatterSeries);

        // Setup X-axis
        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Title = "X (Independent Variable)",
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        });

        // Setup Y-axis
        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = "Y (Dependent Variable)",
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        });

        return plotModel;
    }

    [RelayCommand]
    private async Task LoadLinearData()
    {
        await Task.Run(() =>
        {
            IsLoading = true;
            _currentData = _sampleData.GenerateLinearData(50);
            DataPointCount = _currentData.Count;
            DatasetInfo = "Linear data: Y = 2 + 0.5*X + noise (50 points)";
            PreviewPlotModel = GeneratePreviewChart(_currentData);
        });
        IsLoading = false;
    }

    [RelayCommand]
    private async Task LoadQuadraticData()
    {
        await Task.Run(() =>
        {
            IsLoading = true;
            _currentData = _sampleData.GenerateQuadraticData(50);
            DataPointCount = _currentData.Count;
            DatasetInfo = "Quadratic data: Y = 1 + 0.1*X + 0.05*X² + noise (50 points)";
            PreviewPlotModel = GeneratePreviewChart(_currentData);
        });
        IsLoading = false;
    }

    [RelayCommand]
    private async Task LoadCubicData()
    {
        await Task.Run(() =>
        {
            IsLoading = true;
            _currentData = _sampleData.GenerateCubicData(50);
            DataPointCount = _currentData.Count;
            DatasetInfo = "Cubic data: Y = 0.01*X³ - 0.5*X + noise (50 points)";
            PreviewPlotModel = GeneratePreviewChart(_currentData);
        });
        IsLoading = false;
    }
}
