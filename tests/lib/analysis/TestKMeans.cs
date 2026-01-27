
using Lib.Analysis;
using Lib.Colors;

namespace Tests.Lib.Colors;

[TestClass]
public sealed class TestKMeans
{
    private PackedLab FromLab(double l, double a, double b)
    {
        VectorLab lab = new(l, a, b);
        return lab.Pack();
    }

    private SafeClusterLab BuildCluster(double l, double a, double b)
    {
        return new(new(l, a, b), new(l, a, b), 0);
    }

    private KMeansLab BuildKMeans()
    {
        Dictionary<ColorRgb, PackedLab> colormap = [
            
        ];
        SafeClusterLab[] clusters = [
            // Black
            BuildCluster(0, 0, 0),
            // R: 255
            BuildCluster(53.23288178584245, 80.10930952982204, 67.22006831026425),
            // G: 255
            BuildCluster(87.73703347354422, -86.18463649762525, 83.18116474777854),
            // B: 255
            BuildCluster(32.302586667249486, 79.19666178930935, -107.86368104495168),
            // White
            BuildCluster(100, 0, 0)
        ];

        return new(clusters, colormap);
    }

    [TestMethod]
    public void Test_Cluster()
    {
        KMeansLab kmeans = BuildKMeans();
    }
}