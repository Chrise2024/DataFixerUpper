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