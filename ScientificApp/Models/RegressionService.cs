namespace ScientificApp.Models;

/// <summary>
/// Service for regression model training and comparison.
/// </summary>
public class RegressionService
{
    private readonly List<RegressionModel> _models = [];

    public RegressionService()
    {
        // Try to use Python.NET models; fall back to C# implementations if unavailable
        _models.Add(new PythonLinearRegression());
        _models.Add(new PythonPolynomialRegression(2));
        _models.Add(new PythonPolynomialRegression(3));
    }

    /// <summary>
    /// Get all available models.
    /// </summary>
    public List<RegressionModel> GetAllModels() => new(_models);

    /// <summary>
    /// Train selected models and return metrics.
    /// </summary>
    public List<ModelMetrics> TrainModels(List<DataPoint> data, List<string> selectedModelNames)
    {
        var results = new List<ModelMetrics>();

        foreach (var model in _models)
        {
            if (!selectedModelNames.Contains(model.Name)) continue;

            try
            {
                model.Fit(data);
                results.Add(new ModelMetrics
                {
                    ModelName = model.Name,
                    AIC = model.GetAIC(data.Count),
                    RSquared = model.GetRSquared(),
                    RMSE = model.GetRMSE(),
                    ParameterCount = GetParameterCount(model)
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error training {model.Name}: {ex.Message}");
            }
        }

        return results.OrderBy(m => m.AIC).ToList(); // Sort by AIC (lower is better)
    }

    /// <summary>
    /// Get predictions for a trained model across a range.
    /// </summary>
    public List<DataPoint> GetPredictions(RegressionModel model, double xMin, double xMax, int count = 100)
    {
        var predictions = new List<DataPoint>();
        for (int i = 0; i < count; i++)
        {
            double x = xMin + (xMax - xMin) * i / (count - 1);
            double y = model.Predict(x);
            predictions.Add(new DataPoint(x, y));
        }
        return predictions;
    }

    private static int GetParameterCount(RegressionModel model)
    {
        return model.ParameterCount;
    }

    /// <summary>
    /// Get model by index.
    /// </summary>
    public RegressionModel GetModel(int index)
    {
        if (index < 0 || index >= _models.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Model index out of range");
        return _models[index];
    }

    /// <summary>
    /// Get model name by index.
    /// </summary>
    public string GetModelName(int index)
    {
        return GetModel(index).Name;
    }

    /// <summary>
    /// Get number of models.
    /// </summary>
    public int GetModelCount() => _models.Count;
}
