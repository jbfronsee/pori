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
            int result = base.Compare(x, y);
            if (result != 0 && ColorWithinThreshold(x, y, mTolerances))
            {
                result = 0;
            }

            return result;
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
    public static List<IMagickColor<byte>> FromPixels(ReadOnlySpan<SimpleColor.HSV> pixels, Tolerances tolerances)
    {
        SortedDictionary<SimpleColor.HSV, int> histogram = new(new ThresholdHSVComparer(tolerances));
        
        // Sample uniformly around image in steps.
        int step = (pixels.Length / 256) + 1;
        for (int i = 0; i < pixels.Length; i += step)
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
        foreach (var (color, _) in maxes.OrderBy(g => g.Key, new SimpleColor.HSVComparer()))
        {
            palette.Add(new ColorHSV(color.H, color.S, color.V).ToMagickColor());
        }

        return palette;
    }

    public static List<IMagickColor<byte>> FromImage(MagickImage image, Tolerances tolerances)
    {
        ReadOnlySpan<SimpleColor.HSV> hsvPixels = image.GetPixelColors().Select(c =>
        {
            var hsv = ColorHSV.FromMagickColor(new MagickColor(c.R, c.G, c.B)) ?? new ColorHSV(0, 0, 0);
            return new SimpleColor.HSV(hsv.Hue, hsv.Saturation, hsv.Value);
        }).ToArray();

        return FromPixels(hsvPixels, tolerances);
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

    private static List<SimpleColor.Lab> KMeansCluster(ReadOnlySpan<SimpleColor.Lab> pixels, List<SimpleColor.Lab> clusters, Tolerances tolerances)
    {
        SimpleColor.Lab[] nClusters = clusters.Distinct().ToArray();
        Dictionary<SimpleColor.Lab, (SimpleColor.Lab, int)> means = nClusters.ToDictionary(c => c, c => (c, 0));
        Dictionary<SimpleColor.Lab, int> memoizedCluster = [];
        foreach (var color in pixels)
        {
            int bestClusterIndex = 0;
            if (!memoizedCluster.TryGetValue(color, out bestClusterIndex))
            {
                double bestDistance = double.MaxValue;
                for (int i = 0; i < nClusters.Length; i++)
                {
                    double distance = CalculateDistance(nClusters[i], color);
                    if (distance < bestDistance)
                    {
                        bestClusterIndex = i;
                        bestDistance = distance;
                    }
                }

                memoizedCluster[color] = bestClusterIndex;
            }
            
            SimpleColor.Lab bestCluster = nClusters[bestClusterIndex];
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

    public static List<IMagickColor<byte>> FromPixelsKmeans(ReadOnlySpan<SimpleColor.Lab> pixels, List<IMagickColor<byte>> seeds, Tolerances tolerances, bool verbose = false)
    {
        // TODO case where things are removed?
        int maxIterations = 32;
        List<SimpleColor.Lab> clusters = seeds.Select(c => 
        {
            var (l, a, b) = new Unicolour(ColourSpace.Rgb255, c.R, c.G, c.B).Lab;
            return new SimpleColor.Lab(l, a, b);
        }).ToList();

        if (verbose)
        {
            Console.WriteLine($"K-Means Cluster Index: ");
        }

        bool finished = false;
        for (int i = 0; (i < maxIterations) && !finished; i++)
        {
            if (verbose)
            {
                Console.WriteLine(i);
            }

            List<SimpleColor.Lab> newClusters = KMeansCluster(pixels, clusters, tolerances);
            finished = newClusters.Zip(clusters).Aggregate(true, (total, pair) =>
            {
                var (cluster1, cluster2) = pair;
                return total && (CalculateDistance(cluster1, cluster2) <= 1.0);
            });

            if (!finished)
            {
                clusters = newClusters;
            }
        }

        var palette = clusters.Select(c =>
        {
            Unicolour color = new Unicolour(ColourSpace.Lab, c.L, c.A, c.B);
            var (h, s, v) = color.Hsb;
            return new SimpleColor.HSV(h / 360, s, v);
        }).ToList();

        palette.Sort(new SimpleColor.HSVComparer());
        return palette.Select(c => new ColorHSV(c.H, c.S, c.V).ToMagickColor()).ToList();
    }

    public static List<IMagickColor<byte>> FromImageKmeans(MagickImage image, List<IMagickColor<byte>> seeds, Tolerances tolerances, bool verbose = false)
    {
        ReadOnlySpan<SimpleColor.Lab> labPixels = image.GetPixelColors().Select(c =>
        {
            var (l, a, b) = new Unicolour(ColourSpace.Rgb255, c.R, c.G, c.B).Lab;
            return new SimpleColor.Lab(l, a, b);
        }).ToArray();
            
        return FromPixelsKmeans(labPixels, seeds, tolerances, verbose);
    }
}