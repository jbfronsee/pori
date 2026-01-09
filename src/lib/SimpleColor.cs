namespace SimpleColor
{
    public record struct HSV(double H, double S, double V);

    public record struct Lab(double L, double A, double B);

    public record struct RGB(byte R, byte G, byte B);

    // Records implement IEquatable but not IComparable
    public class HSVComparer() : IComparer<HSV>
    {
        public virtual int Compare(HSV x, HSV y)
        {
            int result = x.H.CompareTo(y.H);
            if (result != 0) return result;

            result = x.S.CompareTo(y.S);
            if (result != 0) return result;

            return x.V.CompareTo(y.V);
        }
    }

    public class LabComparer() : IComparer<Lab>
    {
        public virtual int Compare(Lab x, Lab y)
        {
            int result = x.L.CompareTo(y.L);
            if (result != 0) return result;

            result = x.A.CompareTo(y.A);
            if (result != 0) return result;

            return x.B.CompareTo(y.B);
        }
    }

    public class RGBComparer() : IComparer<RGB>
    {
        public virtual int Compare(RGB x, RGB y)
        {
            int result = x.R.CompareTo(y.R);
            if (result != 0) return result;

            result = x.G.CompareTo(y.G);
            if (result != 0) return result;

            return x.B.CompareTo(y.B);
        }
    }
}