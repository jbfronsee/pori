using Lib.Colors;

namespace Tests.Lib.Colors;

[TestClass]
public sealed class TestColorMath
{

    // Magenta Color
    public VectorLab MeanColor = new(50, 75, -50);

    // Mean of Desaturated Blue and Green
    public VectorLab MeanColor2 = new(76, -42, 20);

    /////////////////////
    /// CalculateNewMean
    /////////////////////

    [TestMethod]
    public void Test_Calculate_New_Mean()
    {
        // 4, 55, 666, 7777 mean = 2113
        double expected = 2125.5;
        double actual = ColorMath.CalculateNewMean(241.666666666666666, 3, 7777);
        Assert.AreEqual(expected, actual, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Calculate_New_Mean_Zero()
    {
        double expected = 4;
        double actual = ColorMath.CalculateNewMean(1, 0, 4);
        Assert.AreEqual(expected, actual, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Calculate_New_Mean_Color()
    {
        VectorLab expected = new (63, 16.5, -15);
        VectorLab actual = ColorMath.CalculateNewMean(MeanColor, 1, MeanColor2);
        Assert.AreEqual(expected.L, actual.L, SharedConstants.Epsilon);
        Assert.AreEqual(expected.A, actual.A, SharedConstants.Epsilon);
        Assert.AreEqual(expected.B, actual.B, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Calculate_New_Mean_Color_Zero()
    {
        VectorLab expected = MeanColor2;
        VectorLab actual = ColorMath.CalculateNewMean(MeanColor, 0, MeanColor2);
        Assert.AreEqual(expected.L, actual.L, SharedConstants.Epsilon);
        Assert.AreEqual(expected.A, actual.A, SharedConstants.Epsilon);
        Assert.AreEqual(expected.B, actual.B, SharedConstants.Epsilon);
    }

    /////////////////////
    /// AddMeans
    /////////////////////

    [TestMethod]
    public void Test_Add_Means()
    {
        // 4, 55, 666, 7777 mean = 2113
        double expected = 2125.5;
        double actual = ColorMath.AddMeans(29.5, 2, 4221.5, 2);
        Assert.AreEqual(expected, actual, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Add_Means_Left_Zero()
    {
        double expected = 29.5;
        double actual = ColorMath.AddMeans(1, 0, 29.5, 2);
        Assert.AreEqual(expected, actual, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Add_Means_Right_Zero()
    {
        double expected = 4221.5;
        double actual = ColorMath.AddMeans(4221.5, 2, 1, 0);
        Assert.AreEqual(expected, actual, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Add_Means_Color()
    {
        VectorLab expected = new (67.33333333333333, -3, -3.3333333333333333);
        VectorLab actual = ColorMath.AddMeans(MeanColor, 1, MeanColor2, 2);
        Assert.AreEqual(expected.L, actual.L, SharedConstants.Epsilon);
        Assert.AreEqual(expected.A, actual.A, SharedConstants.Epsilon);
        Assert.AreEqual(expected.B, actual.B, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Add_Means_Color_Left_Zero()
    {
        VectorLab expected = MeanColor;
        VectorLab actual = ColorMath.AddMeans(new(0, 0, 0), 0, MeanColor, 1);
        Assert.AreEqual(expected.L, actual.L, SharedConstants.Epsilon);
        Assert.AreEqual(expected.A, actual.A, SharedConstants.Epsilon);
        Assert.AreEqual(expected.B, actual.B, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Add_Means_Color_Right_Zero()
    {
        VectorLab expected = MeanColor;
        VectorLab actual = ColorMath.AddMeans(MeanColor, 1, new(0, 0, 0), 0);
        Assert.AreEqual(expected.L, actual.L, SharedConstants.Epsilon);
        Assert.AreEqual(expected.A, actual.A, SharedConstants.Epsilon);
        Assert.AreEqual(expected.B, actual.B, SharedConstants.Epsilon);
    }

    [TestMethod]
    public void Test_Add_Means_Color_Both_Zero()
    {
        VectorLab expected = MeanColor;
        VectorLab actual = ColorMath.AddMeans(MeanColor, 0, new(0, 0, 0), 0);
        Assert.AreEqual(expected.L, actual.L, SharedConstants.Epsilon);
        Assert.AreEqual(expected.A, actual.A, SharedConstants.Epsilon);
        Assert.AreEqual(expected.B, actual.B, SharedConstants.Epsilon);
    }

    ///////////////////////
    /// CalculateDistance
    ///////////////////////
     
    [TestMethod]
    public void Test_Calculate_Distance()
    {
        double expected = 138.7984;
        double actual = ColorMath.CalculateDistance(MeanColor, MeanColor2);
        Assert.AreEqual(expected, actual, SharedConstants.EpsilonFourPlaces);
    }

    [TestMethod]
    public void Test_Calculate_Distance_Zero()
    {
        double expected = 0;
        double actual = ColorMath.CalculateDistance(MeanColor, MeanColor);
        Assert.AreEqual(expected, actual, SharedConstants.Epsilon);
    }

    /////////////////////////////
    /// CalculateDistanceSquared
    /////////////////////////////
         
    [TestMethod]
    public void Test_Calculate_Distance_Squared()
    {
        double expected = 19265;
        double actual = ColorMath.CalculateDistanceSquared(MeanColor, MeanColor2);
        Assert.AreEqual(expected, actual, SharedConstants.EpsilonFourPlaces);
    }

    [TestMethod]
    public void Test_Calculate_Distance_Squared_Zero()
    {
        double expected = 0;
        double actual = ColorMath.CalculateDistanceSquared(MeanColor, MeanColor);
        Assert.AreEqual(expected, actual, SharedConstants.Epsilon);
    }
}
