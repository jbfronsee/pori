using Lib.Colors.Interfaces;

namespace Lib.Colors;

public static class ColorMath
{
    /// <summary>
    /// Add Means of 2 Colors represented with a 3-dimensional ColorVector.
    /// If both are zero it returns the first mean.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mean"></param>
    /// <param name="count">0 or greater</param>
    /// <param name="mean2"></param>
    /// <param name="count2">0 or greater</param>
    /// <returns>New Mean Color</returns>
    public static T AddMeans<T>(T mean, int count, T mean2, int count2) where T: IColorVector<double>
    {
        if (count == 0 && count2 == 0)
        {
            return mean;
        }

        mean.X = AddMeans(mean.X, count, mean2.X, count2);
        mean.Y = AddMeans(mean.Y, count, mean2.Y, count2);
        mean.Z = AddMeans(mean.Z, count, mean2.Z, count2);
        return mean;
    }

    /// <summary>
    /// Adds 2 means together to create a new mean of both lists. At least one count must be greater than zero.
    /// </summary>
    /// <param name="mean"></param>
    /// <param name="count">0 or greater</param>
    /// <param name="otherMean"></param>
    /// <param name="otherCount">0 or greater</param>
    /// <returns>The new mean</returns>
    public static double AddMeans(double mean, double count, double otherMean, double otherCount)
    {
        double sum1 = mean * count;
        double sum2 = otherMean * otherCount;
        return (sum1 + sum2) / (count + otherCount);
    }

    /// <summary>
    /// Calculates new mean given a new vale. Assumes count is valid (Greater than or equal to 0).
    /// </summary>
    /// <param name="mean">Previous mean</param>
    /// <param name="count">How many items make up the mean (0 or higher)</param>
    /// <param name="newValue">The new value to add to mean</param>
    /// <returns>The new Mean</returns>
    public static double CalculateNewMean(double mean, double count, double newValue)
    {
        double sum = mean * count;
        return (sum + newValue) / (count + 1);
    }

    /// <summary>
    /// Calculate new mean of 2 Colors represented with a 3-dimensional ColorVector.
    /// Assumes count is valid (Greater than or equal to 0).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mean"></param>
    /// <param name="count">0 or greater</param>
    /// <param name="newColor"></param>
    /// <returns></returns>
    public static T CalculateNewMean<T>(T mean, int count, T newColor) where T: IColorVector<double>
    {
        mean.X = CalculateNewMean(mean.X, count, newColor.X);
        mean.Y = CalculateNewMean(mean.Y, count, newColor.Y);
        mean.Z = CalculateNewMean(mean.Z, count, newColor.Z);
        return mean;
    }

    /// <summary>
    /// Calculate distance between 2 colors in a Euclidean space.
    /// </summary>
    /// <param name="color1">First Color.</param>
    /// <param name="color2">Second Color.</param>
    /// <returns>The distance between them.</returns>
    public static double CalculateDistance<T>(T color1, T color2) where T: IColorVector<double>
    {
        return Math.Sqrt(CalculateDistanceSquared(color1, color2));
    }

    /// <summary>
    /// Calculate squared distance between 2 colors in a Euclidean space.
    /// </summary>
    /// <param name="color1">First Color.</param>
    /// <param name="color2">Second Color.</param>
    /// <returns>The squared distance between them.</returns>
    public static double CalculateDistanceSquared<T>(T color1, T color2) where T: IColorVector<double>
    {
        double deltaX = Math.Abs(color2.X - color1.X);
        double deltaY = Math.Abs(color2.Y - color1.Y);
        double deltaZ = Math.Abs(color2.Z - color1.Z);
        return deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ;
    }
}
