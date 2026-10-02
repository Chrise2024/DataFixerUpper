namespace DataFixerUpper.Utils;

internal static class ConvertUtil
{
    public static byte ToByte(long d)
    {
        return (byte) d;
    }

    public static short ToShort(long d)
    {
        return (short) d;
    }

    public static int ToInt(long d)
    {
        return (int) d;
    }

    public static float ToFloat(double d)
    {
        return (float) d;
    }
}