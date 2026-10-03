using System;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class RangedCodec<T>(Codec<T> baseCodec, T minInclusive, T maxInclusive) : Codec<T>
    where T : IComparable<T>
{
    public override ValueHolder<string> CodecNameHolder => baseCodec.NewNameForTransform($"Range[{minInclusive}:{maxInclusive}]");

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        if (input.CompareTo(minInclusive) >= 0 && input.CompareTo(maxInclusive) <= 0)
        {
            return baseCodec.Encode(input, ops, prefix);
        }

        return GetOutOfRangeResult<TObject>(input);
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return baseCodec.Decode(ops, input).FlatMap(t =>
            {
                T value = t.First;
                if (value.CompareTo(minInclusive) >= 0 && value.CompareTo(maxInclusive) <= 0)
                {
                    return DataResult.CreateSuccess(t);
                }

                return GetOutOfRangeResult<(T, TObject?)>(value);
            }
        );
    }

    private DataResult<TR> GetOutOfRangeResult<TR>(T value)
    {
        return DataResult.CreateError<TR>($"Value {value} outside of range [{minInclusive}:{maxInclusive}]");
    }
}