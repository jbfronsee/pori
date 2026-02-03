
using Lib.Analysis;
using Lib.Colors;

namespace Tests.Lib.Colors;

[TestClass]
public sealed class TestKMeans
{
    private ColorRgb[] TestPixels = [
        // Green
        new(0, 255, 0),
        // Yellow
        new(255, 255, 0),
        // Red
        new(255, 0, 0),
        // Blue
        new(0, 0, 255)
    ];

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
        Dictionary<ColorRgb, PackedLab> colormap = new()
        {
            // Black
            { new(0, 0, 0), FromLab(0, 0, 0) },
            // Red
            { new(255, 0, 0), FromLab(53.23288178584245, 80.10930952982204, 67.22006831026425) },
            // Yellow
            { new(255, 255, 0), FromLab(97.13824698129729, -21.555908334832285, 94.48248544644461) },
            // Magenta
            { new(255, 0, 255), FromLab(60.319933664076004, 98.25421868616114, -60.84298422386232) },
            // Green
            { new(0, 255, 0), FromLab(87.73703347354422, -86.18463649762525, 83.18116474777854) },
            // Blue
            { new(0, 0, 255), FromLab(32.302586667249486, 79.19666178930935, -107.86368104495168) },
            // White
            { new(255, 255, 255), FromLab(100, 0, 0) }
        };
        
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
    public void Test_Cluster_Count()
    {
        KMeansLab kmeans = BuildKMeans();

        kmeans.Cluster(TestPixels);

        int[] actual = kmeans.Clusters.Select(c => c.Count).ToArray();

        int[] expected = [0, 1, 2, 1, 0];

        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_ClusterParallel_Count()
    {
        KMeansLab kmeans = BuildKMeans();

        kmeans.ClusterParallel(TestPixels);

        int[] actual = kmeans.Clusters.Select(c => c.Count).ToArray();

        int[] expected = [0, 1, 2, 1, 0];

        CollectionAssert.AreEqual(expected, actual);
    }
}