using Lib.Analysis;

namespace Tests.Lib.Colors;

[TestClass]
public sealed class TestEntryLab
{
    [TestMethod]
    public void Test_ParallelSafeCopy()
    {
        EntryLab expected = new (new (100, 0, 0), new (100, 0, 0), 0);
        EntryLab original = new (new (100, 0, 0), new (100, 0, 0), 0);
        EntryLab copy = original.ParallelSafeCopy();
        copy.Cluster = new (0, 0, 0);
        copy.Mean = new (50, 0, 0);
        copy.Count = 1;

        Assert.AreEqual(expected, original);
    }
}