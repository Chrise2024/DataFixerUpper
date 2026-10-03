using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class SimpleCodec<T>(IEncoder<T> encoder, IDecoder<T> decoder, ValueHolder<string> codecNameHolder)
    : Codec<T>

{
    public override ValueHolder<string> CodecNameHolder { get; } = codecNameHolder;

    public SimpleCodec(IEncoder<T> encoder, IDecoder<T> decoder) : this(encoder, decoder, $"Codec[{encoder} {decoder}]") { }

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return encoder.Encode(input, ops, prefix);
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return decoder.Decode(ops, input);
    }
}

internal sealed class SimpleMapCodec<T>(IMapEncoder<T> encoder, IMapDecoder<T> decoder, ValueHolder<string> codecNameHolder) : MapCodec<T>
{
    public override ValueHolder<string> CodecNameHolder => codecNameHolder;

    public SimpleMapCodec(IMapEncoder<T> encoder, IMapDecoder<T> decoder) : this(encoder, decoder, $"MapCodec[{encoder} {decoder}]") { }

    public override RecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return encoder.Encode(input, ops, prefix);
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return decoder.Decode(ops, input);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return encoder.GetKeys(ops).Concat(decoder.GetKeys(ops));
    }
}