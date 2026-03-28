namespace ScientificApp.Models;

/// <summary>
/// Container for model performance metrics.
/// </summary>
public class ModelMetrics
{
    public string ModelName { get; set; } = "";
    public double AIC { get; set; }
    public double RSquared { get; set; }
    public double RMSE { get; set; }
    public int ParameterCount { get; set; }

    public override string ToString()
    {
        return $"{ModelName}: AIC={AIC:F2}, R²={RSquared:F4}, RMSE={RMSE:F4}";
    }
}
