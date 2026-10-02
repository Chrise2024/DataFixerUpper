using DataFixerUpper.Utils;

namespace DataFixerUpper.Test;

[TestClass]
public class ConvertTest
{
    public static double MaxDouble = double.MaxValue;
    public static double MinDouble = double.MinValue;
    public static long MaxLong = long.MaxValue;
    public static long MinLong = long.MinValue;

    [TestMethod]
    public void test_saturateConvert_Fp()
    {
        Assert.AreEqual(byte.MaxValue, ConvertUtil.ToByte(MaxDouble));
        Assert.AreEqual(byte.MinValue, ConvertUtil.ToByte(MinDouble));
        Assert.AreEqual(short.MaxValue, ConvertUtil.ToShort(MaxDouble));
        Assert.AreEqual(short.MinValue, ConvertUtil.ToShort(MinDouble));
        Assert.AreEqual(int.MaxValue, ConvertUtil.ToInt(MaxDouble));
        Assert.AreEqual(int.MinValue, ConvertUtil.ToInt(MinDouble));
        Assert.AreEqual(long.MaxValue, ConvertUtil.ToLong(MaxDouble));
        Assert.AreEqual(long.MinValue, ConvertUtil.ToLong(MinDouble));
        Assert.AreEqual(float.MaxValue, ConvertUtil.ToFloat(MaxDouble));
        Assert.AreEqual(float.MinValue, ConvertUtil.ToFloat(MinDouble));
    }
    
    [TestMethod]
    public void test_saturateConvert_Int()
    {
        Assert.AreEqual(byte.MaxValue, ConvertUtil.ToByte(MaxLong));
        Assert.AreEqual(byte.MinValue, ConvertUtil.ToByte(MinLong));
        Assert.AreEqual(short.MaxValue, ConvertUtil.ToShort(MaxLong));
        Assert.AreEqual(short.MinValue, ConvertUtil.ToShort(MinLong));
        Assert.AreEqual(int.MaxValue, ConvertUtil.ToInt(MaxLong));
        Assert.AreEqual(int.MinValue, ConvertUtil.ToInt(MinLong));
    }
}