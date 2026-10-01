using System;

namespace DataFixerUpper.Utils;

internal static class ConvertUtil
{
    public static byte ToByte(decimal d)
    {
        return (byte) Math.Max(Math.Min(d, byte.MaxValue), byte.MinValue);
    }

    public static short ToShort(decimal d)
    {
        return (short) Math.Max(Math.Min(d, short.MaxValue), short.MinValue);
    }

    public static int ToInt(decimal d)
    {
        return (int) Math.Max(Math.Min(d, int.MaxValue), int.MinValue);
    }

    public static long ToLong(decimal d)
    {
        return (long) Math.Max(Math.Min(d, long.MaxValue), long.MinValue);
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
        return new decimal(Math.Max(Math.Min(f, (float) decimal.MaxValue), (float) decimal.MinValue));
    }

    public static decimal ToDecimal(double d)
    {
        return new decimal(Math.Max(Math.Min(d, (double) decimal.MaxValue), (double) decimal.MinValue));
    }
}