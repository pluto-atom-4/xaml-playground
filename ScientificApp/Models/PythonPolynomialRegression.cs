using System;
using System.Collections.Generic;
using System.Linq;

namespace ScientificApp.Models;

/// <summary>
/// Polynomial regression model using Python.NET backend.
/// Falls back to C# implementation if Python unavailable.
/// </summary>
public class PythonPolynomialRegression : RegressionModel
{
    private readonly int _degree;
    private List<double> _coefficients = new();

    public override string Name => $"Polynomial (d={_degree}) (Python)";
    public override int ParameterCount => _degree + 1;

    public PythonPolynomialRegression(int degree)
    {
        if (degree < 1 || degree > 5)
            throw new ArgumentException("Degree must be between 1 and 5");
        _degree = degree;
    }

    public override void Fit(List<DataPoint> data)
    {
        if (data == null || data.Count < _degree + 1)
        {
            throw new ArgumentException($"Need at least {_degree + 1} data points for degree {_degree} polynomial");
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

            var result = backend.CallFunction("polynomial_regression", xData, yData, _degree);
            
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
            if (!result.TryGetValue("coefficients", out var coeffObj))
                return false;

            _coefficients = ConvertToDoubleList(coeffObj);
            return _coefficients.Count == _degree + 1;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Python polynomial regression failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Fallback: C# Gaussian elimination for polynomial fitting
    /// </summary>
    private void FitWithCSharp(List<double> xData, List<double> yData)
    {
        int n = xData.Count;
        int m = _degree + 1;

        // Build Vandermonde matrix
        double[,] A = new double[n, m];
        for (int i = 0; i < n; i++)
        {
            double xi = xData[i];
            for (int j = 0; j < m; j++)
            {
                A[i, j] = Math.Pow(xi, j);
            }
        }

        // Build normal equations: X^T * X * c = X^T * y
        double[,] XTX = new double[m, m];
        double[] XTy = new double[m];

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < m; j++)
            {
                for (int k = 0; k < n; k++)
                {
                    XTX[i, j] += A[k, i] * A[k, j];
                }
            }
            for (int k = 0; k < n; k++)
            {
                XTy[i] += A[k, i] * yData[k];
            }
        }

        // Solve using Gaussian elimination
        _coefficients = GaussianElimination(XTX, XTy);

        // Reverse to get highest degree first (numpy convention)
        _coefficients.Reverse();
    }

    /// <summary>
    /// Gaussian elimination solver for Ax = b
    /// </summary>
    private static List<double> GaussianElimination(double[,] A, double[] b)
    {
        int n = A.GetLength(0);
        
        // Forward elimination
        for (int i = 0; i < n; i++)
        {
            // Find pivot
            int maxRow = i;
            for (int k = i + 1; k < n; k++)
            {
                if (Math.Abs(A[k, i]) > Math.Abs(A[maxRow, i]))
                    maxRow = k;
            }

            // Swap rows
            for (int k = i; k < n; k++)
            {
                (A[i, k], A[maxRow, k]) = (A[maxRow, k], A[i, k]);
            }
            (b[i], b[maxRow]) = (b[maxRow], b[i]);

            // Check for singular matrix
            if (Math.Abs(A[i, i]) < 1e-10)
                throw new InvalidOperationException("Singular matrix: cannot solve polynomial regression");

            // Eliminate column
            for (int k = i + 1; k < n; k++)
            {
                double factor = A[k, i] / A[i, i];
                for (int j = i; j < n; j++)
                {
                    A[k, j] -= factor * A[i, j];
                }
                b[k] -= factor * b[i];
            }
        }

        // Back substitution
        var x = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            x[i] = b[i];
            for (int j = i + 1; j < n; j++)
            {
                x[i] -= A[i, j] * x[j];
            }
            x[i] /= A[i, i];
        }

        return new List<double>(x);
    }

    public override double Predict(double x)
    {
        double result = 0;
        for (int i = 0; i < _coefficients.Count; i++)
        {
            result += _coefficients[i] * Math.Pow(x, _coefficients.Count - 1 - i);
        }
        return result;
    }

    public override double GetAIC(int n)
    {
        if (Data.Count == 0) return double.PositiveInfinity;
        double mse = CalculateMeanSquaredError(Data);
        double k = _degree + 1; // parameters: degree + 1 coefficients
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
    /// Helper: convert object to List<double>
    /// </summary>
    private static List<double> ConvertToDoubleList(object? obj)
    {
        var result = new List<double>();
        
        if (obj is List<object> list)
        {
            foreach (var item in list)
            {
                result.Add(ConvertToDouble(item));
            }
        }
        else if (obj is System.Collections.IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                result.Add(ConvertToDouble(item));
            }
        }

        return result;
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
