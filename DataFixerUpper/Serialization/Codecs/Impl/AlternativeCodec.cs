using System;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class AlternativeCodec<T, TAlt>(Codec<T> codec, Codec<TAlt> altCodec, Func<TAlt, T> converter) : Codec<T>
{
    public override ValueHolder<string> CodecNameHolder => $"AlternativeCodec[{codec} {altCodec}]";

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return codec.Encode(input, ops, prefix);
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        DataResult<(T, TObject?)> result = codec.Decode(ops, input);
        if (result.IsSuccess)
        {
            return result;
        }

        DataResult<(TAlt, TObject?)> altResult = altCodec.Decode(ops, input);
        if (altResult.IsSuccess)
        {
            return altResult.Map(p => p.MapFirst(converter));
        }

        if (result.HasResultOrPartial)
        {
            return result;
        }
        
        if (altResult.HasResultOrPartial)
        {
            return altResult.Map(p => p.MapFirst(converter));
        }

        return result.Combine(Functions.LiftFirst, altResult);
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
}