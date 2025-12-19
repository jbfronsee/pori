using ImageMagick;
using ImageMagick.Colors;

class Palette
{
    /// <summary>
    /// Creates Palette from image using Histogram
    /// </summary>
    /// 
    /// <param name="color1">First color to compare</param>
    /// <param name="color2">Second color to compare</param>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    public static bool ColorWithinThreshold(ColorHSV color1, ColorHSV color2)
    {
        // Hue is an angular slider that wraps around like a circle.
        double hDegrees1 = color1.Hue * 360;
        double hDegrees2 = color2.Hue * 360;
        double hueDiff = Math.Abs(hDegrees2 - hDegrees1);
        double deltaH = Math.Min(360 - hueDiff, hueDiff);

        double deltaS = Math.Abs(color2.Saturation - color1.Saturation);
        double deltaV = Math.Abs(color2.Value - color1.Value);

        // Bright value threshold
        double threshH = 7.2;
        double threshS = .3;
        double threshV = .3;

        // Darkest values
        if(color1.Value < .2)
        {
            threshS = 1;
            threshH = 360;
        }
        // Dark values
        else if(color1.Value > .2 && color1.Value < .4)
        {
            threshS = .8;
            threshH *= 2;
        }
        // Midtones
        if(color1.Value > .4 && color1.Value < .6)
        {
            threshS *= 2;
        }

        if (deltaH > threshH || deltaS > threshS || deltaV > threshV)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Creates Palette from image using Histogram
    /// </summary>
    /// 
    /// <param name="image">The image to create palette from.</param>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    public static List<IMagickColor<byte>> PaletteFromImage(MagickImage image)
    {
        var hist = image.Histogram();

        Dictionary<ColorHSV, uint> maxValues = new Dictionary<ColorHSV, uint>();
        foreach (var color in hist)
        {
            ColorHSV colorHSV = ColorHSV.FromMagickColor(color.Key) ?? new ColorHSV(0, 0, 0);
            
            ColorHSV? similarKey = null;
            
            uint min = uint.MaxValue;
            ColorHSV? minKey = null;
            foreach (var max in maxValues)
            {
                if (ColorWithinThreshold(colorHSV, max.Key))
                {
                    similarKey = max.Key;
                    break;
                }

                if (color.Value > max.Value && max.Value <= min)
                {
                    minKey = max.Key;
                }

                min = Math.Min(max.Value, min);
            }

            if (similarKey != null)
            {
                maxValues[similarKey] += color.Value;
            }
            else if (maxValues.Count < 16)
            {
                maxValues[colorHSV] = color.Value;
            }
            else if (minKey != null)
            {
                maxValues.Remove(minKey);
                maxValues[colorHSV] = color.Value;
            }    
        }

        List<IMagickColor<byte>> palette = new List<IMagickColor<byte>>();
        foreach (var color in maxValues.OrderByDescending(c => c.Value))
        {
            palette.Add(color.Key.ToMagickColor());
        }

        return palette;
    }

    /// <summary>
    /// Creates Palette from image using Kmeans. Modifies image object.
    /// </summary>
    /// 
    /// <param name="image">The image to create palette from.</param>
    /// <param name="seeds">The seed values for the Kmeans clusters.</param>
    /// <exception cref="MagickException">Thrown when an error is raised by ImageMagick.</exception>
    public static List<IMagickColor<byte>> PaletteFromImageKmeans(MagickImage image, List<IMagickColor<byte>> seeds)
    {
        KmeansSettings kmeans = new KmeansSettings();
        kmeans.SeedColors = string.Join(";", seeds);
        kmeans.NumberColors = (uint)seeds.Count;
        kmeans.MaxIterations = 16;

        image.Kmeans(kmeans);

        var newHist = image.Histogram();

        List<IMagickColor<byte>> palette = new List<IMagickColor<byte>>();
        foreach (var color in newHist.OrderByDescending(c => c.Value))
        {
            palette.Add(color.Key);
        }

        return palette;
    }
}