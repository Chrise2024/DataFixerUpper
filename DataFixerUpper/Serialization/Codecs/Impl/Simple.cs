using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class SimpleCodec<T>(IEncoder<T> encoder, IDecoder<T> decoder, ValueHolder<string> codecNameHolder) : Codec<T>
{
    private readonly IEncoder<T> _encoder = encoder;

    private readonly IDecoder<T> _decoder = decoder;

    public override ValueHolder<string> CodecNameHolder { get; } = codecNameHolder;

    public SimpleCodec(IEncoder<T> encoder, IDecoder<T> decoder) : this(encoder, decoder, $"Codec[{encoder} {decoder}]") { }

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return _encoder.Encode(input, ops, prefix);
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _decoder.Decode(ops, input);
    }

    public override bool Equals(object? obj)
    {
        return obj is SimpleCodec<T> codec && _encoder.Equals(codec._encoder) && _decoder.Equals(codec._decoder);
    }

    public override int GetHashCode()
    {
        return _encoder.GetHashCode() + _decoder.GetHashCode() * 31;
    }
}

internal sealed class SimpleMapCodec<T>(IMapEncoder<T> encoder, IMapDecoder<T> decoder, ValueHolder<string> codecNameHolder) : MapCodec<T>
{
    private readonly IMapEncoder<T> _encoder = encoder;

    private readonly IMapDecoder<T> _decoder = decoder;

    public override ValueHolder<string> CodecNameHolder => codecNameHolder;

    public SimpleMapCodec(IMapEncoder<T> encoder, IMapDecoder<T> decoder) : this(encoder, decoder, $"MapCodec[{encoder} {decoder}]") { }

    public override RecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return _encoder.Encode(input, ops, prefix);
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return _decoder.Decode(ops, input);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _encoder.GetKeys(ops).Concat(_decoder.GetKeys(ops));
    }

    public override bool Equals(object? obj)
    {
        return obj is SimpleMapCodec<T> codec && _encoder.Equals(codec._encoder) && _decoder.Equals(codec._decoder);
    }

    public override int GetHashCode()
    {
        return _encoder.GetHashCode() + _decoder.GetHashCode() * 31;
    }
}