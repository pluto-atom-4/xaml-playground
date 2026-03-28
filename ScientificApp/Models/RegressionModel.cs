namespace ScientificApp.Models;

/// <summary>
/// Base class for regression models.
/// </summary>
public abstract class RegressionModel
{
    public string Name { get; protected set; } = "Model";
    public List<DataPoint> Data { get; protected set; } = [];

    public abstract void Fit(List<DataPoint> data);
    public abstract double Predict(double x);
    public abstract double GetAIC(int n);
    public abstract double GetRSquared();
    public abstract double GetRMSE();

    protected double CalculateMeanSquaredError(List<DataPoint> data)
    {
        if (data.Count == 0) return 0;
        double mse = data.Sum(d => Math.Pow(d.Y - Predict(d.X), 2)) / data.Count;
        return mse;
    }

    protected double CalculateMean(List<double> values)
    {
        return values.Count == 0 ? 0 : values.Average();
    }
}
