using System.Collections.Generic;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class LifecycleEncoder<T>(IEncoder<T> baseEncoder, Lifecycle lifecycle) : EncoderBase<T>
{
    private readonly IEncoder<T> _baseEncoder = baseEncoder;

    private readonly Lifecycle _lifecycle = lifecycle;

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return _baseEncoder.Encode(input, ops, prefix).SetLifecycle(_lifecycle);
    }

    public override bool Equals(object? obj)
    {
        return obj is LifecycleEncoder<T> encoder && _baseEncoder.Equals(encoder._baseEncoder) && _lifecycle.Equals(encoder._lifecycle);
    }

    public override int GetHashCode()
    {
        return _baseEncoder.GetHashCode() + _lifecycle.GetHashCode() * 31;
    }
}

internal sealed class LifecycleDecoder<T>(IDecoder<T> baseDecoder, Lifecycle lifecycle) : DecoderBase<T>
{
    private readonly IDecoder<T> _baseDecoder = baseDecoder;

    private readonly Lifecycle _lifecycle = lifecycle;

    public override DataResult<Pair<T, TObject?>> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _baseDecoder.Decode(ops, input).SetLifecycle(_lifecycle);
    }

    public override bool Equals(object? obj)
    {
        return obj is LifecycleDecoder<T> decoder && _baseDecoder.Equals(decoder._baseDecoder) && _lifecycle.Equals(decoder._lifecycle);
    }

    public override int GetHashCode()
    {
        return _baseDecoder.GetHashCode() + _lifecycle.GetHashCode() * 31;
    }
}

internal sealed class LifecycleCodec<T>(Codec<T> baseCodec, Lifecycle lifecycle)
    : Codec<T>
{
    private readonly Codec<T> _baseCodec = baseCodec;

    private readonly Lifecycle _lifecycle = lifecycle;

    public override ValueHolder<string> CodecNameHolder => _baseCodec.CodecNameHolder;

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return _baseCodec.Encode(input, ops, prefix).SetLifecycle(_lifecycle);
    }

    public override DataResult<Pair<T, TObject?>> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _baseCodec.Decode(ops, input).SetLifecycle(_lifecycle);
    }

    public override bool Equals(object? obj)
    {
        return obj is LifecycleCodec<T> codec && _baseCodec.Equals(codec._baseCodec) && _lifecycle.Equals(codec._lifecycle);
    }

    public override int GetHashCode()
    {
        return _baseCodec.GetHashCode() + _lifecycle.GetHashCode() * 31;
    }
}

internal sealed class LifecycleMapEncoder<T>(IMapEncoder<T> baseEncoder, Lifecycle lifecycle) : MapEncoderBase<T>
{
    private readonly IMapEncoder<T> _baseEncoder = baseEncoder;

    private readonly Lifecycle _lifecycle = lifecycle;

    public override RecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return _baseEncoder.Encode(input, ops, prefix).SetLifecycle(_lifecycle);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _baseEncoder.GetKeys(ops);
    }

    public override bool Equals(object? obj)
    {
        return obj is LifecycleMapEncoder<T> encoder && _baseEncoder.Equals(encoder._baseEncoder) && _lifecycle.Equals(encoder._lifecycle);
    }

    public override int GetHashCode()
    {
        return _baseEncoder.GetHashCode() + _lifecycle.GetHashCode() * 31;
    }
}

internal sealed class LifecycleMapDecoder<T>(IMapDecoder<T> baseDecoder, Lifecycle lifecycle) : MapDecoderBase<T>
{
    private readonly IMapDecoder<T> _baseDecoder = baseDecoder;

    private readonly Lifecycle _lifecycle = lifecycle;

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return _baseDecoder.Decode(ops, input).SetLifecycle(_lifecycle);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _baseDecoder.GetKeys(ops);
    }

    public override bool Equals(object? obj)
    {
        return obj is LifecycleMapDecoder<T> decoder && _baseDecoder.Equals(decoder._baseDecoder) && _lifecycle.Equals(decoder._lifecycle);
    }

    public override int GetHashCode()
    {
        return _baseDecoder.GetHashCode() + _lifecycle.GetHashCode() * 31;
    }
}

internal sealed class LifecycleMapCodec<T>(MapCodec<T> baseCodec, Lifecycle lifecycle)
    : MapCodec<T>
{
    private readonly MapCodec<T> _baseCodec = baseCodec;

    private readonly Lifecycle _lifecycle = lifecycle;

    public override ValueHolder<string> CodecNameHolder => _baseCodec.CodecNameHolder;

    public override RecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return _baseCodec.Encode(input, ops, prefix).SetLifecycle(_lifecycle);
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return _baseCodec.Decode(ops, input).SetLifecycle(_lifecycle);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _baseCodec.GetKeys(ops);
    }

    public override bool Equals(object? obj)
    {
        return obj is LifecycleMapCodec<T> codec && _baseCodec.Equals(codec._baseCodec) && _lifecycle.Equals(codec._lifecycle);
    }

    public override int GetHashCode()
    {
        return _baseCodec.GetHashCode() + _lifecycle.GetHashCode() * 31;
    }
}