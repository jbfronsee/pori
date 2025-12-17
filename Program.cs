using System.Drawing;
using ImageMagick;
using ImageMagick.Colors;
using ImageMagick.Drawing;

internal class Program
{
    private static Options GetOpts(string[] args)
    {
        Options opts = new Options();
        foreach (string arg in args)
        {
            if (arg.StartsWith("-"))
            {
                if (arg.Length == 2)
                {
                    char flagChar = arg[1];
                    switch (flagChar)
                    {
                        case 'h':
                            opts.PrintHistogram = true;
                            break;
                        case 'i':
                            opts.PrintImage = true;
                            break;
                    }
                }
                else
                {
                    // Handle multi-character single-dash flags
                }
            }
            else
            {
                // Process non-flag arguments (operands/values)
                Console.WriteLine($"Invalid Arg: {arg}");
            }
        }

        return opts;
    }

    private static void Main(string[] args)
    {
        Options opts = GetOpts(args);
        
        //Console.WriteLine("Processing Image...");

        var imageFromFile = new MagickImage("test.jpg");

        // if(imageFromFile.Width > 1080 || imageFromFile.Height > 1080)
        // {
        //     imageFromFile.Resize(new Percentage(75));
        // }

        var hist = imageFromFile.Histogram();

        Dictionary<ColorHSV, uint> maxValues = new Dictionary<ColorHSV, uint>();
        foreach (var color in hist)
        {
            ColorHSV? maybeColorHSV = ColorHSV.FromMagickColor(color.Key);
            ColorHSV colorHSV;
            if (maybeColorHSV != null)
            {
                colorHSV = maybeColorHSV;
            }
            else
            {
                break;
            }
            if (opts.PrintHistogram)
            {
                Console.WriteLine($"Color: {color.Key} Occurences: {color.Value}");
            }
            else
            {

                ColorHSV? index = null;
                uint min = uint.MaxValue;
                ColorHSV? indexReplace = null;
                foreach (var max in maxValues)

                {
                    ColorHSV hColor = max.Key;
                    double deltaH = Math.Abs(hColor.Hue - colorHSV.Hue);
                    double deltaS = Math.Abs(hColor.Saturation - colorHSV.Saturation);
                    double deltaV = Math.Abs(hColor.Value - colorHSV.Value);

                    // TODO constants
                    //TODO hue is angular
                    double thresh_h = .02;
                    double thresh_s = .3;
                    double thresh_v = .3;

                    if(colorHSV.Value < .2)
                    {
                        thresh_s = 1;
                        thresh_h = 1;
                    }
                    if(colorHSV.Value > .2 && colorHSV.Value < .4)
                    {
                        thresh_s = .8;
                        thresh_h *= 2;
                    }
                    if(colorHSV.Value > .4 && colorHSV.Value < .6)
                    {
                        thresh_s *= 2;
                    }

                    if (deltaV > thresh_v || deltaH > thresh_h || deltaS > thresh_s)
                    {
                        if(color.Value > min || color.Value > max.Value)
                        {
                            if(max.Value < min)
                            {
                                indexReplace = max.Key;
                                // Console.WriteLine($"hColor.Hue {hColor.Hue}");
                                // Console.WriteLine($"hColor.Saturation {hColor.Saturation}");
                                // Console.WriteLine($"hColor.Value {hColor.Value}");

                                // Console.WriteLine($"deltaH: {deltaH}");
                                // Console.WriteLine($"deltaS: {deltaS}");
                                // Console.WriteLine($"deltaV: {deltaV}");
                            }
                        }
                    }
                    else
                    {
                        index = hColor;
                        //Console.WriteLine($"Why Not? ==== {index.ToMagickColor().ToHexString()}");
                        break;
                    }

                    min = Math.Min(max.Value, min);
                }

                if (index != null)
                {
                    maxValues[index] += color.Value;
                    //Console.WriteLine($"Why Not? {index.ToMagickColor().ToHexString()}");
                }
                else if (maxValues.Count < 16)
                {
                    maxValues[colorHSV] = color.Value;
                }
                else if (indexReplace != null)
                {
                    maxValues.Remove(indexReplace);
                    maxValues[colorHSV] = color.Value;
                }
            }
        }

        List<string> palette = new List<string>();
        foreach (var color in maxValues.OrderByDescending(c => c.Value))
        {
            palette.Add(color.Key.ToMagickColor().ToHexString());
        }

        KmeansSettings kmeans = new KmeansSettings();
        kmeans.SeedColors = string.Join(";", palette);
        kmeans.NumberColors = 16;
        kmeans.MaxIterations = 16;

        imageFromFile.Kmeans(kmeans);

        var newHist = imageFromFile.Histogram();

        palette = new List<string>();
        foreach (var color in newHist.OrderByDescending(c => c.Value))
        {
            palette.Add(color.Key.ToHexString());
        }

        
        //Console.WriteLine($"new hist {newHist.Count}");

        // Console.WriteLine("Palette: ");
        // foreach(string item in palette)
        // {
        //     Console.WriteLine($"Color: {item}");
        // }


        List<string> gplLines = new List<string>
        {
            "GIMP Palette",
            "Name: Test",
            $"Columns: {palette.Count}",
            "#"
        };
        int i = 0;
        foreach (string color in palette)
        {
            var colorM = new MagickColor(color);
            gplLines.Add($"{colorM.R,3} {colorM.G,3} {colorM.B,3}\t#{i}");
            i++;
        }
        File.WriteAllLines(@"test.gpl", gplLines);
        
        MagickImage paletteImage = new MagickImage(MagickColors.Transparent, 512, 128);

        Drawables canvas = new Drawables();

        double x = 0, y = 0, width = 64, height = 64;
        foreach(string color in palette)
        {
            // Define the rectangle's properties
            canvas
                .StrokeColor(new MagickColor(color))
                .StrokeWidth(2)
                .FillColor(new MagickColor(color))
                .Rectangle(x, y, x + width, y + height);

            x += width;
            if (x > 512)
            {
                x = 0;
                y += height;
            }
        }

        // Draw the square onto the image
        canvas.Draw(paletteImage);

        // Save the result

        // paletteImage.Format = MagickFormat.Png;
        // paletteImage.Write(Console.OpenStandardOutput());

        paletteImage.Format = MagickFormat.Png;
        paletteImage.Write("out.png");
    }
}