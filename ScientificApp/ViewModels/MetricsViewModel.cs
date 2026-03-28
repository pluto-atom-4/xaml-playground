using CommunityToolkit.Mvvm.ComponentModel;
using ScientificApp.Models;
using System.Collections.ObjectModel;

namespace ScientificApp.ViewModels;

public partial class MetricsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ModelMetricsDisplay> metricsTable = [];

    [ObservableProperty]
    private string bestModel = "None";

    [ObservableProperty]
    private double bestAIC = double.PositiveInfinity;

    public void UpdateMetrics(List<ModelMetrics> metrics)
    {
        if (metrics.Count == 0)
        {
            MetricsTable.Clear();
            BestModel = "None";
            return;
        }

        var displays = metrics.Select(m => new ModelMetricsDisplay
        {
            ModelName = m.ModelName,
            AIC = m.AIC,
            RSquared = m.RSquared,
            RMSE = m.RMSE,
            Parameters = m.ParameterCount
        }).ToList();

        MetricsTable = new ObservableCollection<ModelMetricsDisplay>(displays);
        BestModel = displays.First().ModelName;
        BestAIC = displays.First().AIC;
    }
}

public class ModelMetricsDisplay : ObservableObject
{
    public string ModelName { get; set; } = "";
    public double AIC { get; set; }
    public double RSquared { get; set; }
    public double RMSE { get; set; }
    public int Parameters { get; set; }

    public string AICDisplay => AIC.ToString("F2");
    public string RSquaredDisplay => RSquared.ToString("F4");
    public string RMSEDisplay => RMSE.ToString("F4");
}
