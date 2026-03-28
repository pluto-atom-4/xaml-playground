namespace ScientificApp.Models;

/// <summary>
/// Generates sample datasets for regression analysis.
/// </summary>
public static class SampleData
{
    /// <summary>
    /// Generate synthetic linear data with noise.
    /// Y = 2 + 0.5*X + noise
    /// </summary>
    public static List<DataPoint> GenerateLinearData(int count = 50, double noiseStdDev = 1.5)
    {
        var random = new Random(42); // Fixed seed for reproducibility
        var data = new List<DataPoint>();

        for (int i = 0; i < count; i++)
        {
            double x = i * 0.5; // X from 0 to ~25
            double noise = random.NextGaussian() * noiseStdDev;
            double y = 2 + 0.5 * x + noise;
            data.Add(new DataPoint(x, y));
        }

        return data;
    }

    /// <summary>
    /// Generate synthetic quadratic data with noise.
    /// Y = 1 + 0.1*X + 0.05*X² + noise
    /// </summary>
    public static List<DataPoint> GenerateQuadraticData(int count = 50, double noiseStdDev = 2.0)
    {
        var random = new Random(42);
        var data = new List<DataPoint>();

        for (int i = 0; i < count; i++)
        {
            double x = (i - count / 2.0) * 0.5; // X centered around 0
            double noise = random.NextGaussian() * noiseStdDev;
            double y = 1 + 0.1 * x + 0.05 * x * x + noise;
            data.Add(new DataPoint(x, y));
        }

        return data;
    }

    /// <summary>
    /// Generate synthetic cubic data with noise.
    /// Y = 0.01*X³ - 0.5*X + noise
    /// </summary>
    public static List<DataPoint> GenerateCubicData(int count = 50, double noiseStdDev = 3.0)
    {
        var random = new Random(42);
        var data = new List<DataPoint>();

        for (int i = 0; i < count; i++)
        {
            double x = (i - count / 2.0) * 0.4; // X centered around 0
            double noise = random.NextGaussian() * noiseStdDev;
            double y = 0.01 * x * x * x - 0.5 * x + noise;
            data.Add(new DataPoint(x, y));
        }

        return data;
    }
}

/// <summary>
/// Extension method for generating Gaussian random numbers.
/// </summary>
public static class RandomExtensions
{
    public static double NextGaussian(this Random random)
    {
        // Box-Muller transform
        double u1 = random.NextDouble();
        double u2 = random.NextDouble();
        return Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
    }
}
