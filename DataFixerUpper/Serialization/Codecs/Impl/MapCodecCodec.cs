using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class MapCodecCodec<T>(MapCodec<T> baseCodec) : Codec<T>

{
    public override ValueHolder<string> CodecNameHolder => baseCodec.CodecNameHolder;

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return baseCodec.Encode(input, ops, baseCodec.GetCompressedBuilder(ops)).Build(prefix);
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return baseCodec.CompressedDecode(ops, input).Map(r => (r, input));
    }
}

internal sealed record MapEncoderEncoder<T>(IMapEncoder<T> BaseEncoder) : IEncoder<T>
{
    public DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : notnull
    {
        return BaseEncoder.Encode(input, ops, BaseEncoder.GetCompressedBuilder(ops)).Build(prefix);
    }
}

internal sealed record MapDecoderDecoder<T>(IMapDecoder<T> BaseDecoder) : IDecoder<T>
{
    public DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : notnull
    {
        return BaseDecoder.CompressedDecode(ops, input).Map(r => (r, input));
    }
}