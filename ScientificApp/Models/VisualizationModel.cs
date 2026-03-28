using System;
using System.Collections.Generic;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using OxyPlot.Annotations;

namespace ScientificApp.Models;

/// <summary>
/// Manages chart data and OxyPlot model generation for visualizations.
/// Provides residual plots and prediction curves.
/// </summary>
public static class VisualizationModel
{
    /// <summary>
    /// Generate residual plot data.
    /// X-axis: Predicted values, Y-axis: Residuals (actual - predicted)
    /// </summary>
    public static PlotModel GenerateResidualsPlot(
        List<DataPoint> originalData,
        List<double> predictions,
        string modelName)
    {
        if (originalData.Count == 0 || predictions.Count == 0)
            throw new ArgumentException("Data cannot be empty");

        if (originalData.Count != predictions.Count)
            throw new ArgumentException("Data and predictions must have same length");

        var plotModel = new PlotModel
        {
            Title = $"Residual Plot - {modelName}",
            Background = OxyColors.White
        };

        // Calculate residuals
        var residuals = new List<(double predicted, double residual)>();
        for (int i = 0; i < originalData.Count; i++)
        {
            var residual = originalData[i].Y - predictions[i];
            residuals.Add((predictions[i], residual));
        }

        // Create scatter series
        var scatterSeries = new ScatterSeries
        {
            Title = "Residuals",
            MarkerType = MarkerType.Circle,
            MarkerSize = 5,
            MarkerFill = OxyColor.FromRgb(100, 150, 200)
        };

        foreach (var (predicted, residual) in residuals)
        {
            scatterSeries.Points.Add(new ScatterPoint(predicted, residual));
        }

        plotModel.Series.Add(scatterSeries);

        // Add horizontal line at y=0 (perfect prediction)
        var zeroLine = new LineAnnotation
        {
            Type = LineAnnotationType.Horizontal,
            Y = 0,
            Color = OxyColors.Red,
            StrokeThickness = 2
        };
        plotModel.Annotations.Add(zeroLine);

        // Setup axes
        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Title = "Predicted Values",
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        });

        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = "Residuals",
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        });

        return plotModel;
    }

    /// <summary>
    /// Generate prediction curve plot.
    /// Shows actual data points and fitted curve overlaid.
    /// </summary>
    public static PlotModel GeneratePredictionsCurve(
        List<DataPoint> originalData,
        List<double> predictions,
        string modelName)
    {
        if (originalData.Count == 0 || predictions.Count == 0)
            throw new ArgumentException("Data cannot be empty");

        if (originalData.Count != predictions.Count)
            throw new ArgumentException("Data and predictions must have same length");

        var plotModel = new PlotModel
        {
            Title = $"Predictions - {modelName}",
            Background = OxyColors.White
        };

        // Create indices paired with data for sorting
        var indexedData = originalData
            .Select((point, idx) => (index: idx, point))
            .OrderBy(x => x.point.X)
            .ToList();

        // Actual data series (scatter)
        var actualSeries = new ScatterSeries
        {
            Title = "Actual Data",
            MarkerType = MarkerType.Circle,
            MarkerSize = 6,
            MarkerFill = OxyColor.FromRgb(50, 100, 200),
            MarkerStroke = OxyColors.Black,
            MarkerStrokeThickness = 1
        };

        foreach (var point in originalData)
        {
            actualSeries.Points.Add(new ScatterPoint(point.X, point.Y));
        }

        plotModel.Series.Add(actualSeries);

        // Fitted curve series (line)
        var fittedSeries = new LineSeries
        {
            Title = "Fitted Line",
            Color = OxyColor.FromRgb(200, 50, 50),
            StrokeThickness = 2.5,
            MarkerType = MarkerType.None
        };

        // Add points in sorted order for continuous curve
        foreach (var (idx, point) in indexedData)
        {
            fittedSeries.Points.Add(new OxyPlot.DataPoint(point.X, predictions[idx]));
        }

        plotModel.Series.Add(fittedSeries);

        // Setup axes
        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Title = "X (Independent Variable)",
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        });

        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = "Y (Dependent Variable)",
            MajorGridlineStyle = LineStyle.Solid,
            MinorGridlineStyle = LineStyle.Dot
        });

        return plotModel;
    }

    /// <summary>
    /// Calculate residuals for a fitted model.
    /// </summary>
    public static List<double> CalculateResiduals(
        List<DataPoint> originalData,
        List<double> predictions)
    {
        if (originalData.Count != predictions.Count)
            throw new ArgumentException("Data and predictions must have same length");

        return originalData
            .Select((point, idx) => point.Y - predictions[idx])
            .ToList();
    }

    /// <summary>
    /// Calculate residual statistics (mean, std dev, etc).
    /// </summary>
    public static Dictionary<string, double> CalculateResidualStats(List<double> residuals)
    {
        if (residuals.Count == 0)
            throw new ArgumentException("Residuals cannot be empty");

        var mean = residuals.Average();
        var variance = residuals.Average(r => Math.Pow(r - mean, 2));
        var stdDev = Math.Sqrt(variance);
        var absResiduals = residuals.Select(Math.Abs).ToList();

        return new Dictionary<string, double>
        {
            { "Mean", mean },
            { "StdDev", stdDev },
            { "Min", residuals.Min() },
            { "Max", residuals.Max() },
            { "MAE", absResiduals.Average() } // Mean absolute error
        };
    }
}
