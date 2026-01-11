public static class LabConverter
{
    private const double LScale = byte.MaxValue / 100;

    private const int ABShift = 128;

    private static byte LToByte(double l)
    {
        return (byte)(l * LScale);
    }

    private static byte ABToByte(double ab)
    {
        return (byte)(ab + ABShift);
    }

    private static double LToSimple(byte l)
    {
        return l / LScale;
    }

    private static double ABToSimple(byte ab)
    {
        return (double)ab - ABShift;
    }


    /// <summary>
    /// Convert Lab value from double representation to byte representation.
    /// Precision is lost during conversion.
    /// </summary>
    /// <param name="l">l</param>
    /// <param name="a">a</param>
    /// <param name="b">b</param>
    /// <returns></returns>
    public static ByteColor.Lab ToByte(double l, double a, double b)
    {
        return new ByteColor.Lab(LToByte(l), ABToByte(a), ABToByte(b));
    }

    /// <summary>
    /// Convert Lab value from double representation to byte representation.
    /// Precision is lost during conversion.
    /// </summary>
    /// <param name="lab">Color to convert.</param>
    /// <returns></returns>
    public static ByteColor.Lab ToByte(SimpleColor.Lab lab)
    {
        return ToByte(lab.L, lab.A, lab.B);
    }

    /// <summary>
    /// Convert Lab value from byte representation to double representation.
    /// </summary>
    /// <param name="l">l</param>
    /// <param name="a">a</param>
    /// <param name="b">b</param>
    /// <returns></returns>
    public static SimpleColor.Lab ToSimple(byte l, byte a, byte b)
    {
        return new SimpleColor.Lab(LToSimple(l), ABToSimple(a), ABToSimple(b));
    }

    /// <summary>
    /// Convert Lab value from byte representation to double representation.
    /// </summary>
    /// <param name="lab">Color to convert.</param>
    /// <returns></returns>
    public static SimpleColor.Lab ToSimple(ByteColor.Lab lab)
    {
        return ToSimple(lab.L, lab.A, lab.B);
    }
}