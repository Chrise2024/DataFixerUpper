using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class MapCodecCodec<T>(MapCodec<T> baseCodec) : Codec<T>

{
    private readonly MapCodec<T> _baseCodec = baseCodec;

    public override ValueHolder<string> CodecNameHolder => _baseCodec.CodecNameHolder;

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return _baseCodec.Encode(input, ops, _baseCodec.GetCompressedBuilder(ops)).Build(prefix);
    }

    public override DataResult<Pair<T, TObject?>> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _baseCodec.CompressedDecode(ops, input).Map(r => Pair.Create(r, input));
    }

    public override bool Equals(object? obj)
    {
        return obj is MapCodecCodec<T> codec && _baseCodec.Equals(codec._baseCodec);
    }

    public override int GetHashCode()
    {
        return _baseCodec.GetHashCode();
    }
}

internal sealed class MapEncoderEncoder<T>(IMapEncoder<T> baseEncoder) : EncoderBase<T>
{
    private readonly IMapEncoder<T> _baseEncoder = baseEncoder;

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return _baseEncoder.Encode(input, ops, _baseEncoder.GetCompressedBuilder(ops)).Build(prefix);
    }

    public override bool Equals(object? obj)
    {
        return obj is MapEncoderEncoder<T> encoder && _baseEncoder.Equals(encoder._baseEncoder);
    }

    public override int GetHashCode()
    {
        return _baseEncoder.GetHashCode();
    }
}

internal sealed class MapDecoderDecoder<T>(IMapDecoder<T> baseDecoder) : DecoderBase<T>
{
    private readonly IMapDecoder<T> _baseDecoder = baseDecoder;

    public override DataResult<Pair<T, TObject?>> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _baseDecoder.CompressedDecode(ops, input).Map(r => Pair.Create(r, input));
    }

    public override bool Equals(object? obj)
    {
        return obj is MapDecoderDecoder<T> decoder && _baseDecoder.Equals(decoder._baseDecoder);
    }

    public override int GetHashCode()
    {
        return _baseDecoder.GetHashCode();
    }
}