using System;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class AlternativeCodec<T, TAlt>(Codec<T> codec, Codec<TAlt> altCodec, Func<TAlt, T> converter) : Codec<T>
{
    private readonly Codec<T> _codec = codec;

    private readonly Codec<TAlt> _altCodec = altCodec;

    private readonly Func<TAlt, T> _converter = converter;

    public override ValueHolder<string> CodecNameHolder => $"AlternativeCodec[{_codec} {_altCodec}]";

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return _codec.Encode(input, ops, prefix);
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        DataResult<(T, TObject?)> result = _codec.Decode(ops, input);
        if (result.IsSuccess)
        {
            return result;
        }

        DataResult<(TAlt, TObject?)> altResult = _altCodec.Decode(ops, input);
        if (altResult.IsSuccess)
        {
            return altResult.Map(p => p.MapFirst(_converter));
        }

        if (result.HasResultOrPartial)
        {
            return result;
        }

        if (altResult.HasResultOrPartial)
        {
            return altResult.Map(p => p.MapFirst(_converter));
        }

        return result.Combine(Functions.LiftFirst, altResult);
    }

    public override bool Equals(object? obj)
    {
        return obj is AlternativeCodec<T, TAlt> codec
            && _codec.Equals(codec._codec)
            && _altCodec.Equals(codec._altCodec)
            && _converter.Equals(codec._converter);
    }

    public override int GetHashCode()
    {
        int hash = _codec.GetHashCode();
        hash = hash * 31 + _altCodec.GetHashCode();
        hash = hash * 31 + _converter.GetHashCode();
        return hash;
    }
}

internal sealed class AlternativeCodec<T>(Codec<T> codec, Codec<T> altCodec)
    : Codec<T>
{
    private readonly AlternativeCodec<T, T> _impl = new(codec, altCodec, Functions.Identity);

    public override ValueHolder<string> CodecNameHolder => _impl.CodecNameHolder;

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return _impl.Encode(input, ops, prefix);
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _impl.Decode(ops, input);
    }

    public override bool Equals(object? obj)
    {
        return obj is AlternativeCodec<T> codec && _impl.Equals(codec._impl);
    }

    public override int GetHashCode()
    {
        return _impl.GetHashCode();
    }
}