namespace ScientificApp.Models;

/// <summary>
/// Simple linear regression: Y = a + b*X
/// </summary>
public class LinearRegression : RegressionModel
{
    public double Intercept { get; private set; }
    public double Slope { get; private set; }

    public override string Name => "Linear Regression";
    public override int ParameterCount => 2;

    public override void Fit(List<DataPoint> data)
    {
        if (data.Count < 2) throw new ArgumentException("Need at least 2 data points");

        Data = new List<DataPoint>(data);
        double n = data.Count;
        double xMean = data.Average(d => d.X);
        double yMean = data.Average(d => d.Y);

        double numerator = 0;
        double denominator = 0;

        foreach (var point in data)
        {
            numerator += (point.X - xMean) * (point.Y - yMean);
            denominator += Math.Pow(point.X - xMean, 2);
        }

        Slope = denominator == 0 ? 0 : numerator / denominator;
        Intercept = yMean - Slope * xMean;
    }

    public override double Predict(double x)
    {
        return Intercept + Slope * x;
    }

    public override double GetAIC(int n)
    {
        if (Data.Count == 0) return double.PositiveInfinity;
        double mse = CalculateMeanSquaredError(Data);
        double k = 2; // 2 parameters: intercept and slope
        return n * Math.Log(mse) + 2 * k;
    }

    public override double GetRSquared()
    {
        if (Data.Count < 2) return 0;
        double yMean = Data.Average(d => d.Y);
        double ssRes = Data.Sum(d => Math.Pow(d.Y - Predict(d.X), 2));
        double ssTot = Data.Sum(d => Math.Pow(d.Y - yMean, 2));
        return ssTot == 0 ? 0 : 1 - (ssRes / ssTot);
    }

    public override double GetRMSE()
    {
        return Math.Sqrt(CalculateMeanSquaredError(Data));
    }
}
