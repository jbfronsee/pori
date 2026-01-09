using ImageMagick;
using ImageMagick.Colors;
using Wacton.Unicolour;

public class Palette
{
    private class ThresholdHSVComparer(Tolerances tolerances) : SimpleColor.HSVComparer
    {
        private Tolerances mTolerances = tolerances;

        public override int Compare(SimpleColor.HSV x, SimpleColor.HSV y)
        {
            if (ColorWithinThreshold(x, y, mTolerances))
            {
                return 0;
            }
            else
            {
                return base.Compare(x, y);
            }
        }
    }

    /// <summary>
    /// Calculates difference in Hue from normalized value to a Degree Value (0 - 360)
    /// </summary>
    /// 
    /// <param name="hue1">First hue to compare (0 - 1) normalized value</param>
    /// <param name="hue2">Second hue to compare (0 - 1) normalized value</param>
    public static double CalculateDeltaH(double hue1, double hue2)
    {
        // Hue is an angular slider that wraps around like a circle.
        double hDegrees1 = hue1 * 360;
        double hDegrees2 = hue2 * 360;
        double hueDiff = Math.Abs(hDegrees2 - hDegrees1);
        return Math.Min(360 - hueDiff, hueDiff);
    }

    /// <summary>
    /// Creates Palette from image using Histogram
    /// </summary>
    /// 
    /// <param name="color1">First color to compare</param>
    /// <param name="color2">Second color to compare</param>
    public static bool ColorWithinThreshold(SimpleColor.HSV color1, SimpleColor.HSV color2, Tolerances tolerances)
    {
        double deltaH = CalculateDeltaH(color1.H, color2.H);
        double deltaS = Math.Abs(color2.S - color1.S);
        double deltaV = Math.Abs(color2.V - color1.V);

        ThresholdHSV thresh = tolerances.GetThreshold(color1.V);
        return deltaH <= thresh.Hue && deltaS <= thresh.Saturation && deltaV <= thresh.Value;
    }

    /// <summary>
    /// Creates Palette from image using Histogram
    /// </summary>
    /// 
    /// <param name="image">The image to create palette from with alpha channel removed.</param>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    public static List<IMagickColor<byte>> PaletteFromPixels(List<SimpleColor.HSV> pixels, Tolerances tolerances)
    {
        SortedDictionary<SimpleColor.HSV, int> histogram = new(new ThresholdHSVComparer(tolerances));
        
        // Sample uniformly around image in steps.
        int step = (pixels.Count / 256) + 1;
        for (int i = 0; i < pixels.Count; i += step)
        {
            histogram.TryAdd(pixels[i], 1);
        }

        foreach(var color in pixels)
        {
            // If the color is within threshold update the max value.
            if (histogram.ContainsKey(color))
            {
                histogram[color]++;
            }
            else
            {
                histogram.Add(color, 1);
            }
        }

        var maxes = histogram.OrderByDescending(g => g.Value).Distinct().Take(16).ToList();
        List<IMagickColor<byte>> palette = [];
        foreach (var (color, _) in maxes)
        {
            palette.Add(new ColorHSV(color.H, color.S, color.V).ToMagickColor());
        }

        return palette;
    }

    private static double UpdateMean(double mean, double count, double newValue)
    {
        double sum = mean * count;
        return (sum + newValue) / (count + 1);
    }

    public static double CalculateDistance(SimpleColor.Lab color1, SimpleColor.Lab color2)
    {

        double deltaL = Math.Abs(color2.L - color1.L);
        double deltaA = Math.Abs(color2.A - color1.A);
        double deltaB = Math.Abs(color2.B - color1.B);
        return Math.Sqrt(deltaL * deltaL + deltaA * deltaA + deltaB * deltaB);
    }

    private static List<SimpleColor.Lab> KMeansCluster(List<SimpleColor.Lab> pixels, List<SimpleColor.Lab> clusters, Tolerances tolerances)
    {
        clusters = clusters.Distinct().ToList();
        Dictionary<SimpleColor.Lab, (SimpleColor.Lab, int)> means = clusters.ToDictionary(c => c, c => (c, 0));
        foreach (var color in pixels)
        {
            SimpleColor.Lab bestCluster = clusters.First();
            double bestEpsilon = double.MaxValue;

            foreach (SimpleColor.Lab cluster in clusters)
            {
                double epsilon = CalculateDistance(cluster, color);
                if (epsilon < bestEpsilon)
                {
                    bestCluster = cluster;
                    bestEpsilon = epsilon;
                }
            }
            
            (SimpleColor.Lab mean, int count) = means[bestCluster];
            var newMean = new SimpleColor.Lab(UpdateMean(mean.L, count, color.L), UpdateMean(mean.A, count, color.A), UpdateMean(mean.B, count, color.B));
            means[bestCluster] = (newMean, count + 1);
        }

        return means.Values.Select(m =>
        {
            var (mean, _) = m;
            return mean;
        }).ToList();
    }

    public static List<IMagickColor<byte>> PaletteFromPixelsKmeans(List<SimpleColor.Lab> pixels, List<IMagickColor<byte>> seeds, Tolerances tolerances)
    {
        int maxIterations = 16;
        List<SimpleColor.Lab> clusters = seeds.Select(c => 
        {
            var (l, a, b) = new Unicolour(ColourSpace.Rgb255, c.R, c.G, c.B).Lab;
            return new SimpleColor.Lab(l, a, b);
        }).ToList();

        for (int i = 0; i < maxIterations; i++)
        {
            clusters = KMeansCluster(pixels, clusters, tolerances);
        }

        return clusters.Select(c =>
        {
            Unicolour color = new Unicolour(ColourSpace.Lab, c.L, c.A, c.B);
            var rgb = color.Rgb.Byte255;
            return new MagickColor((byte)rgb.R, (byte)rgb.G, (byte)rgb.B);
        }).ToList<IMagickColor<byte>>();
    }
}