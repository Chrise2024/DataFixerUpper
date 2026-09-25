using System.Collections.Generic;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed record LifecycleEncoder<T>(IEncoder<T> BaseEncoder, Lifecycle Lifecycle) : IEncoder<T>
{
    public DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : notnull
    {
        return BaseEncoder.Encode(input, ops, prefix).SetLifecycle(Lifecycle);
    }
}

internal sealed record LifecycleDecoder<T>(IDecoder<T> BaseDecoder, Lifecycle Lifecycle) : IDecoder<T>
{
    public DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : notnull
    {
        return BaseDecoder.Decode(ops, input).SetLifecycle(Lifecycle);
    }
}

internal sealed class LifecycleCodec<T>(Codec<T> baseCodec, Lifecycle lifecycle)
    : Codec<T>
{
    public override ValueHolder<string> CodecNameHolder => baseCodec.CodecNameHolder;

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return baseCodec.Encode(input, ops, prefix).SetLifecycle(lifecycle);
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return baseCodec.Decode(ops, input).SetLifecycle(lifecycle);
    }
}

internal sealed class LifecycleMapEncoder<T>(IMapEncoder<T> baseEncoder, Lifecycle lifecycle) : MapEncoderBase<T>
{
    public override IRecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        return baseEncoder.Encode(input, ops, prefix).SetLifecycle(lifecycle);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return baseEncoder.GetKeys(ops);
    }
}

internal sealed class LifecycleMapDecoder<T>(IMapDecoder<T> baseDecoder, Lifecycle lifecycle) : MapDecoderBase<T>
{
    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        return baseDecoder.Decode(ops, input).SetLifecycle(lifecycle);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return baseDecoder.GetKeys(ops);
    }
}

internal sealed class LifecycleMapCodec<T>(MapCodec<T> baseCodec, Lifecycle lifecycle)
    : MapCodec<T>
{
    public override ValueHolder<string> CodecNameHolder => baseCodec.CodecNameHolder;

    public override IRecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        return baseCodec.Encode(input, ops, prefix).SetLifecycle(lifecycle);
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        return baseCodec.Decode(ops, input).SetLifecycle(lifecycle);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return baseCodec.GetKeys(ops);
    }
}