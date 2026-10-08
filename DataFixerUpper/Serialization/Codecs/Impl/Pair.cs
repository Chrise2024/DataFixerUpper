using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class PairCodec<T1, T2>(Codec<T1> leftCodec, Codec<T2> rightCodec) : Codec<Pair<T1, T2>>
{
    private readonly Codec<T1> _leftCodec = leftCodec;

    private readonly Codec<T2> _rightCodec = rightCodec;

    public override ValueHolder<string> CodecNameHolder => $"PairCodec[{_leftCodec} {_rightCodec}]";

    public override DataResult<TObject> Encode<TObject>(Pair<T1, T2> input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return _leftCodec.Encode(input.First, ops, prefix).FlatMap(firstEncoded =>
            _rightCodec.Encode(input.Second, ops, firstEncoded)
        );
    }

    public override DataResult<Pair<Pair<T1, T2>, TObject?>> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _leftCodec.Decode(ops, input).FlatMap(firstDecoded =>
            _rightCodec.Decode(ops, firstDecoded.Second).Map(secondDecoded =>
                Pair.Create(Pair.Create(firstDecoded.First, secondDecoded.First), secondDecoded.Second)
            )
        );
    }

    public override bool Equals(object? obj)
    {
        return obj is PairCodec<T1, T2> codec && _leftCodec.Equals(codec._leftCodec) && _rightCodec.Equals(codec._rightCodec);
    }

    public override int GetHashCode()
    {
        return _leftCodec.GetHashCode() + _rightCodec.GetHashCode() * 31;
    }
}

internal sealed class PairMapCodec<T1, T2>(MapCodec<T1> leftCodec, MapCodec<T2> rightCodec) : MapCodec<Pair<T1, T2>>
{
    private readonly MapCodec<T1> _leftCodec = leftCodec;

    private readonly MapCodec<T2> _rightCodec = rightCodec;

    public override ValueHolder<string> CodecNameHolder => $"PairCodec[{_leftCodec} {_rightCodec}]";

    public override RecordBuilder<TObject> Encode<TObject>(Pair<T1, T2> input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return _leftCodec.Encode(input.First, ops, _rightCodec.Encode(input.Second, ops, prefix));
    }

    public override DataResult<Pair<T1, T2>> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return _leftCodec.Decode(ops, input).FlatMap(firstDecoded => _rightCodec.Decode(ops, input).Map(secondDecoded => Pair.Create(firstDecoded, secondDecoded)));
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _leftCodec.GetKeys(ops).Concat(_rightCodec.GetKeys(ops));
    }

    public override bool Equals(object? obj)
    {
        return obj is PairMapCodec<T1, T2> codec && _leftCodec.Equals(codec._leftCodec) && _rightCodec.Equals(codec._rightCodec);
    }

    public override int GetHashCode()
    {
        return _leftCodec.GetHashCode() + _rightCodec.GetHashCode() * 31;
    }
}