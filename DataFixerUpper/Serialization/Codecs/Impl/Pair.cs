using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class PairCodec<T1, T2>(Codec<T1> leftCodec, Codec<T2> rightCodec) : Codec<(T1, T2)>
{
    public override ValueHolder<string> CodecNameHolder => $"PairCodec[{leftCodec} {rightCodec}]";

    public override DataResult<TObject> Encode<TObject>((T1, T2) input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return leftCodec.Encode(input.Item1, ops, prefix).FlatMap(firstEncoded =>
            rightCodec.Encode(input.Item2, ops, firstEncoded)
        );
    }

    public override DataResult<((T1, T2), TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return leftCodec.Decode(ops, input).FlatMap(firstDecoded =>
            rightCodec.Decode(ops, firstDecoded.Item2).Map(secondDecoded =>
                ((firstDecoded.Item1, secondDecoded.Item1), secondDecoded.Item2)
            )
        );
    }
}

internal sealed class PairMapCodec<T1, T2>(MapCodec<T1> leftCodec, MapCodec<T2> rightCodec) : MapCodec<(T1, T2)>
{
    public override ValueHolder<string> CodecNameHolder => $"PairCodec[{leftCodec} {rightCodec}]";

    public override IRecordBuilder<TObject> Encode<TObject>((T1, T2) input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        return leftCodec.Encode(input.Item1, ops, rightCodec.Encode(input.Item2, ops, prefix));
    }

    public override DataResult<(T1, T2)> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        return leftCodec.Decode(ops, input).FlatMap(firstDecoded => rightCodec.Decode(ops, input).Map(secondDecoded => (firstDecoded, secondDecoded)));
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return leftCodec.GetKeys(ops).Concat(rightCodec.GetKeys(ops));
    }
}