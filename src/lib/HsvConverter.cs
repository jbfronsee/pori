public static class HsvConverter
{
    private static byte ToByte(double value)
    {
        return (byte)(value * byte.MaxValue);
    }

    private static double ToSimple(byte value)
    {
        return (double)value / byte.MaxValue;
    }

    /// <summary>
    /// Convert HSV value from double representation to byte representation.
    /// Precision is lost during conversion.
    /// </summary>
    /// <param name="h">Hue</param>
    /// <param name="s">Saturation</param>
    /// <param name="v">Value</param>
    /// <returns></returns>
    public static ByteColor.Hsv ToByte(double h, double s, double v)
    {
        return new ByteColor.Hsv(ToByte(h), ToByte(s), ToByte(v));
    }

    /// <summary>
    /// Convert HSV value from double representation to byte representation.
    /// Precision is lost during conversion.
    /// </summary>
    /// <param name="hsv">Color to convert.</param>
    /// <returns></returns>
    public static ByteColor.Hsv ToByte(SimpleColor.Hsv hsv)
    {
        return ToByte(hsv.H, hsv.S, hsv.V);
    }

    /// <summary>
    /// Convert HSV value from byte representation to double representation.
    /// </summary>
    /// <param name="h">Hue</param>
    /// <param name="s">Saturation</param>
    /// <param name="v">Value</param>
    /// <returns></returns>
    public static SimpleColor.Hsv ToSimple(byte h, byte s, byte v)
    {
        return new SimpleColor.Hsv(ToSimple(h), ToSimple(s), ToSimple(v));
    }

    /// <summary>
    /// Convert HSV value from byte representation to double representation.
    /// </summary>
    /// <param name="hsv">Color to convert.</param>
    /// <returns></returns>
    public static SimpleColor.Hsv ToSimple(ByteColor.Hsv hsv)
    {
        return ToSimple(hsv.H, hsv.S, hsv.V);
    }
}