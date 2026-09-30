using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class MapCodecCodec<T>(MapCodec<T> baseCodec) : Codec<T>

{
    public override ValueHolder<string> CodecNameHolder => baseCodec.CodecNameHolder;

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject prefix)
        where TObject : default
    {
        return baseCodec.Encode(input, ops, baseCodec.GetCompressedBuilder(ops)).Build(prefix);
    }

    public override DataResult<(T, TObject)> Decode<TObject>(DynamicOps<TObject> ops, TObject input)
        where TObject : default
    {
        return baseCodec.CompressedDecode(ops, input).Map(r => (r, input));
    }
}

internal sealed class MapEncoderEncoder<T>(IMapEncoder<T> baseEncoder) : EncoderBase<T>
{
    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject prefix)
        where TObject : default
    {
        return baseEncoder.Encode(input, ops, baseEncoder.GetCompressedBuilder(ops)).Build(prefix);
    }
}

internal sealed class MapDecoderDecoder<T>(IMapDecoder<T> baseDecoder) : DecoderBase<T>
{
    public override DataResult<(T, TObject)> Decode<TObject>(DynamicOps<TObject> ops, TObject input)
        where TObject : default
    {
        return baseDecoder.CompressedDecode(ops, input).Map(r => (r, input));
    }
}