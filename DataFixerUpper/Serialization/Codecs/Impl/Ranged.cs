using System;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class RangedCodec<T>(Codec<T> baseCodec, T minInclusive, T maxInclusive) : Codec<T>
    where T : IComparable<T>
{
    private readonly Codec<T> _baseCodec = baseCodec;

    private readonly T _minInclusive = minInclusive;

    private readonly T _maxInclusive = maxInclusive;

    public override ValueHolder<string> CodecNameHolder => _baseCodec.NewNameForTransform($"Range[{_minInclusive}:{_maxInclusive}]");

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        if (input.CompareTo(_minInclusive) >= 0 && input.CompareTo(_maxInclusive) <= 0)
        {
            return _baseCodec.Encode(input, ops, prefix);
        }

        return GetOutOfRangeResult<TObject>(input);
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _baseCodec.Decode(ops, input).FlatMap(t =>
            {
                T value = t.First;
                if (value.CompareTo(_minInclusive) >= 0 && value.CompareTo(_maxInclusive) <= 0)
                {
                    return DataResult.CreateSuccess(t);
                }

                return GetOutOfRangeResult<(T, TObject?)>(value);
            }
        );
    }

    private DataResult<TR> GetOutOfRangeResult<TR>(T value)
    {
        return DataResult.CreateError<TR>($"Value {value} outside of range [{_minInclusive}:{_maxInclusive}]");
    }

    public override bool Equals(object? obj)
    {
        return obj is RangedCodec<T> codec
            && _baseCodec.Equals(codec._baseCodec)
            && _minInclusive.Equals(codec._minInclusive)
            && _maxInclusive.Equals(codec._maxInclusive);
    }

    public override int GetHashCode()
    {
        int hash = _baseCodec.GetHashCode();
        hash = hash * 31 + _minInclusive.GetHashCode();
        hash = hash * 31 + _maxInclusive.GetHashCode();
        return hash;
    }
}