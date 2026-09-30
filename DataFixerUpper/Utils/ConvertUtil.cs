using System;

namespace DataFixerUpper.Utils;

internal static class ConvertUtil
{
    public static byte ToByte(decimal d)
    {
        return (byte) Math.Min(d, byte.MaxValue);
    }

    public static short ToShort(decimal d)
    {
        return (short) Math.Min(d, short.MaxValue);
    }

    public static int ToInt(decimal d)
    {
        return (int) Math.Min(d, int.MaxValue);
    }

    public static long ToLong(decimal d)
    {
        return (long) Math.Min(d, long.MaxValue);
    }

    public static float ToFloat(decimal d)
    {
        return (float) d;
    }

    public static double ToDouble(decimal d)
    {
        return (double) d;
    }

    public static decimal ToDecimal(float f)
    {
        return new decimal(Math.Min(f, (float) decimal.MaxValue));
    }

    public static decimal ToDecimal(double d)
    {
        return new decimal(Math.Min(d, (double) decimal.MaxValue));
    }
}