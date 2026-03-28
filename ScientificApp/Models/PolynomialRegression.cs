namespace ScientificApp.Models;

/// <summary>
/// Polynomial regression: Y = a0 + a1*X + a2*X² + ... + an*X^n
/// Uses least squares fitting via normal equations.
/// </summary>
public class PolynomialRegression : RegressionModel
{
    public int Degree { get; private set; }
    public double[] Coefficients { get; private set; } = [];

    public PolynomialRegression(int degree = 2)
    {
        if (degree < 1 || degree > 5)
            throw new ArgumentException("Degree must be between 1 and 5");

        Degree = degree;
        Name = $"Polynomial (degree {degree})";
    }

    public override void Fit(List<DataPoint> data)
    {
        if (data.Count <= Degree)
            throw new ArgumentException($"Need at least {Degree + 1} data points");

        Data = new List<DataPoint>(data);
        int n = data.Count;

        // Build Vandermonde matrix: X = [1, x, x², ..., x^d]
        double[,] X = new double[n, Degree + 1];
        double[] y = new double[n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j <= Degree; j++)
                X[i, j] = Math.Pow(data[i].X, j);
            y[i] = data[i].Y;
        }

        // Solve normal equations: (X^T * X) * a = X^T * y
        var XtX = MultiplyMatrices(TransposeMatrix(X, n, Degree + 1), X, Degree + 1, n, Degree + 1);
        var Xty = MatrixVectorMultiply(TransposeMatrix(X, n, Degree + 1), y, Degree + 1, n);

        Coefficients = SolveLinearSystem(XtX, Xty, Degree + 1);
    }

    public override double Predict(double x)
    {
        double result = 0;
        for (int i = 0; i < Coefficients.Length; i++)
            result += Coefficients[i] * Math.Pow(x, i);
        return result;
    }

    public override double GetAIC(int n)
    {
        if (Data.Count == 0) return double.PositiveInfinity;
        double mse = CalculateMeanSquaredError(Data);
        int k = Degree + 1; // number of coefficients
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

    private static double[,] TransposeMatrix(double[,] matrix, int rows, int cols)
    {
        double[,] result = new double[cols, rows];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                result[j, i] = matrix[i, j];
        return result;
    }

    private static double[,] MultiplyMatrices(double[,] A, double[,] B, int aRows, int aCols, int bCols)
    {
        double[,] result = new double[aRows, bCols];
        for (int i = 0; i < aRows; i++)
            for (int j = 0; j < bCols; j++)
                for (int k = 0; k < aCols; k++)
                    result[i, j] += A[i, k] * B[k, j];
        return result;
    }

    private static double[] MatrixVectorMultiply(double[,] matrix, double[] vector, int rows, int cols)
    {
        double[] result = new double[rows];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                result[i] += matrix[i, j] * vector[j];
        return result;
    }

    private static double[] SolveLinearSystem(double[,] A, double[] b, int n)
    {
        // Gaussian elimination with partial pivoting
        double[,] augmented = new double[n, n + 1];
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                augmented[i, j] = A[i, j];
            augmented[i, n] = b[i];
        }

        for (int col = 0; col < n; col++)
        {
            int pivotRow = col;
            for (int row = col + 1; row < n; row++)
                if (Math.Abs(augmented[row, col]) > Math.Abs(augmented[pivotRow, col]))
                    pivotRow = row;

            // Swap rows
            for (int j = 0; j <= n; j++)
                (augmented[col, j], augmented[pivotRow, j]) = (augmented[pivotRow, j], augmented[col, j]);

            // Forward elimination
            for (int row = col + 1; row < n; row++)
            {
                double factor = augmented[row, col] / augmented[col, col];
                for (int j = col; j <= n; j++)
                    augmented[row, j] -= factor * augmented[col, j];
            }
        }

        // Back substitution
        double[] x = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            x[i] = augmented[i, n];
            for (int j = i + 1; j < n; j++)
                x[i] -= augmented[i, j] * x[j];
            x[i] /= augmented[i, i];
        }

        return x;
    }
}
