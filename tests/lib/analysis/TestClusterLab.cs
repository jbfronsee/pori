
using Lib.Analysis;

namespace Tests.Lib.Colors;

[TestClass]
public sealed class TestClusterLab
{
    [TestMethod]
    public void Test_ParallelSafeCopy()
    {
        SafeClusterLab expected = new (new (100, 0, 0), new (100, 0, 0), 0);
        SafeClusterLab original = new (new (100, 0, 0), new (100, 0, 0), 0);
        SafeClusterLab copy = original.ParallelSafeCopy();
        copy.Cluster = new (0, 0, 0);
        copy.Mean = new (50, 0, 0);
        copy.Count = 1;

        Assert.AreEqual(expected, original);
    }
}