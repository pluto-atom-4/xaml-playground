namespace ScientificApp.Models;

/// <summary>
/// Represents a single data point (X, Y) for regression analysis.
/// </summary>
public class DataPoint
{
    public double X { get; set; }
    public double Y { get; set; }

    public DataPoint(double x, double y)
    {
        X = x;
        Y = y;
    }
}
