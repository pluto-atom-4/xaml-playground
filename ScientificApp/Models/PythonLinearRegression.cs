using System;
using System.Collections.Generic;
using System.Linq;

namespace ScientificApp.Models;

/// <summary>
/// Linear regression model using Python.NET backend.
/// Falls back to C# implementation if Python unavailable.
/// </summary>
public class PythonLinearRegression : RegressionModel
{
    private const string ModelName = "Linear Regression (Python)";
    private double _intercept = 0;
    private double _slope = 0;

    public override string Name => ModelName;
    public override int ParameterCount => 2; // intercept + slope

    public override void Fit(List<DataPoint> data)
    {
        if (data == null || data.Count < 2)
        {
            throw new ArgumentException("Need at least 2 data points for linear regression");
        }

        Data = new List<DataPoint>(data);
        var xData = data.Select(d => d.X).ToList();
        var yData = data.Select(d => d.Y).ToList();

        // Try Python backend first
        if (TryFitWithPython(xData, yData))
        {
            return;
        }

        // Fallback to C# implementation
        FitWithCSharp(xData, yData);
    }

    /// <summary>
    /// Attempt to fit using Python.NET backend
    /// </summary>
    private bool TryFitWithPython(List<double> xData, List<double> yData)
    {
        try
        {
            var backend = PythonBackend.Instance;
            if (!backend.IsAvailable)
                return false;

            var result = backend.CallFunction("linear_regression", xData, yData);
            
            if (result == null)
                return false;

            if (!result.TryGetValue("success", out var successObj))
                return false;

            bool success = successObj switch
            {
                bool b => b,
                string s => bool.TryParse(s, out var parsed) && parsed,
                _ => false
            };

            if (!success)
                return false;

            // Extract coefficients
            if (!result.TryGetValue("intercept", out var interceptObj) ||
                !result.TryGetValue("slope", out var slopeObj))
                return false;

            _intercept = ConvertToDouble(interceptObj);
            _slope = ConvertToDouble(slopeObj);

            return true;
        }
        catch (Exception ex)
        {
            // Log but don't throw - we'll fall back to C#
            System.Diagnostics.Debug.WriteLine($"Python linear regression failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Fallback: C# implementation of linear regression
    /// </summary>
    private void FitWithCSharp(List<double> xData, List<double> yData)
    {
        int n = xData.Count;
        double sumX = xData.Sum();
        double sumY = yData.Sum();
        double sumXX = xData.Sum(x => x * x);
        double sumXY = xData.Zip(yData, (x, y) => x * y).Sum();

        double denominator = n * sumXX - sumX * sumX;
        if (Math.Abs(denominator) < 1e-10)
        {
            throw new InvalidOperationException("Singular matrix: cannot fit linear regression");
        }

        _slope = (n * sumXY - sumX * sumY) / denominator;
        _intercept = (sumY - _slope * sumX) / n;
    }

    public override double Predict(double x)
    {
        return _intercept + _slope * x;
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

    /// <summary>
    /// Helper: convert object to double
    /// </summary>
    private static double ConvertToDouble(object? obj)
    {
        return obj switch
        {
            double d => d,
            float f => f,
            int i => i,
            long l => l,
            string s => double.TryParse(s, out var result) ? result : 0,
            _ => 0
        };
    }
}
