namespace DataFixerUpper.Utils;

internal static class ConvertUtil
{
    public static byte ToByte(long l)
    {
        return l >= byte.MaxValue ? byte.MaxValue :
            l <= byte.MinValue ? byte.MinValue : (byte) l;
    }

    public static short ToShort(long l)
    {
        return l >= short.MaxValue ? short.MaxValue :
            l <= short.MinValue ? short.MinValue : (short) l;
    }

    public static int ToInt(long l)
    {
        return l >= int.MaxValue ? int.MaxValue :
            l <= int.MinValue ? int.MinValue : (int) l;
    }

    public static byte ToByte(double d)
    {
        return d >= byte.MaxValue ? byte.MaxValue :
            d <= byte.MinValue ? byte.MinValue : (byte) d;
    }

    public static short ToShort(double d)
    {
        return d >= short.MaxValue ? short.MaxValue :
            d <= short.MinValue ? short.MinValue : (short) d;
    }

    public static int ToInt(double d)
    {
        return d >= int.MaxValue ? int.MaxValue :
            d <= int.MinValue ? int.MinValue : (int) d;
    }

    public static long ToLong(double d)
    {
        return d >= long.MaxValue ? long.MaxValue :
            d <= long.MinValue ? long.MinValue : (long) d;
    }

    public static float ToFloat(double d)
    {
        return d >= float.MaxValue ? float.MaxValue :
            d <= float.MinValue ? float.MinValue : (float) d;
    }
}