using System;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class CoMappedEncoder<TNew, TOri>(IEncoder<TOri> baseEncoder, Func<TNew, TOri> func)
    : EncoderBase<TNew>
{
    public override DataResult<TObject> Encode<TObject>(TNew input, DynamicOps<TObject> ops, TObject prefix)
        where TObject : default
    {
        return baseEncoder.Encode(func.Apply(input), ops, prefix);
    }

    public override string ToString()
    {
        return baseEncoder + "[CoMapped]";
    }
}

internal sealed class CoMappedMapEncoder<TNew, TOri>(IMapEncoder<TOri> baseEncoder, Func<TNew, TOri> mapper)
    : MapEncoderBase<TNew>
{
    public override RecordBuilder<TObject> Encode<TObject>(TNew input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return baseEncoder.Encode(mapper.Apply(input), ops, prefix);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return baseEncoder.GetKeys(ops);
    }

    public override string ToString()
    {
        return baseEncoder + "[CoMapped]";
    }
}

internal sealed class FlatCoMappedEncoder<TNew, TOri>(IEncoder<TOri> baseEncoder, Func<TNew, DataResult<TOri>> func)
    : EncoderBase<TNew>
{
    public override DataResult<TObject> Encode<TObject>(TNew input, DynamicOps<TObject> ops, TObject prefix)
        where TObject : default
    {
        return func.Apply(input).FlatMap(mapped => baseEncoder.Encode(mapped, ops, prefix));
    }

    public override string ToString()
    {
        return baseEncoder + "[FlatCoMapped]";
    }
}

internal sealed class FlatCoMappedMapEncoder<TNew, TOri>(IMapEncoder<TOri> baseEncoder, Func<TNew, DataResult<TOri>> mapper)
    : MapEncoderBase<TNew>
{
    public override RecordBuilder<TObject> Encode<TObject>(TNew input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        DataResult<TOri> oriResult = mapper.Apply(input);
        RecordBuilder<TObject> resultBuilder = prefix.WithErrorsFrom(oriResult);
        return oriResult.Map(r => baseEncoder.Encode(r, ops, resultBuilder)).GetResultOrDefault(resultBuilder);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return baseEncoder.GetKeys(ops);
    }

    public override string ToString()
    {
        return baseEncoder + "[FlatCoMapped]";
    }
}

internal sealed class MappedDecoder<TOri, TNew>(IDecoder<TOri> baseDecoder, Func<TOri, TNew> func)
    : DecoderBase<TNew>
{
    public override DataResult<(TNew, TObject)> Decode<TObject>(DynamicOps<TObject> ops, TObject input)
        where TObject : default
    {
        return baseDecoder.Decode(ops, input).Map(result => result.MapFirst(func));
    }

    public override string ToString()
    {
        return baseDecoder + "[Mapped]";
    }
}

internal sealed class MappedMapDecoder<TOri, TNew>(IMapDecoder<TOri> baseDecoder, Func<TOri, TNew> mapper)
    : MapDecoderBase<TNew>
{
    public override DataResult<TNew> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return baseDecoder.Decode(ops, input).Map(mapper);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return baseDecoder.GetKeys(ops);
    }

    public override string ToString()
    {
        return baseDecoder + "[Mapped]";
    }
}

internal sealed class DispatchMappedMapDecoder<TOri, TNew>(IMapDecoder<TOri> baseDecoder, IMapDecoder<Func<TOri, TNew>> dispatcher) : MapDecoderBase<TNew>
{
    public override DataResult<TNew> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return baseDecoder.Decode(ops, input).FlatMap(ori => dispatcher.Decode(ops, input).Map(dispatcherFunc => dispatcherFunc.Apply(ori)));
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return baseDecoder.GetKeys(ops).Concat(dispatcher.GetKeys(ops));
    }
}

internal sealed class FlatMappedDecoder<TOri, TNew>(IDecoder<TOri> baseDecoder, Func<TOri, DataResult<TNew>> func)
    : DecoderBase<TNew>
{
    public override DataResult<(TNew, TObject)> Decode<TObject>(DynamicOps<TObject> ops, TObject input)
        where TObject : default
    {
        return baseDecoder.Decode(ops, input).FlatMap(result => func.Apply(result.First).Map(mapped => (mapped, result.Second)));
    }

    public override string ToString()
    {
        return baseDecoder + "[FlatMapped]";
    }
}

internal sealed class PromptPartialDecoder<T>(IDecoder<T> baseDecoder, Consumer<string> onError) : DecoderBase<T>
{
    public override DataResult<(T, TObject)> Decode<TObject>(DynamicOps<TObject> ops, TObject input)
        where TObject : default
    {
        return baseDecoder.Decode(ops, input).PromotePartial(onError);
    }

    public override string ToString()
    {
        return baseDecoder + "[PromotePartial]";
    }
}

internal sealed class FlatMappedMapDecoder<TOri, TNew>(IMapDecoder<TOri> baseDecoder, Func<TOri, DataResult<TNew>> mapper)
    : MapDecoderBase<TNew>
{
    public override DataResult<TNew> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return baseDecoder.Decode(ops, input).FlatMap(mapper);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return baseDecoder.GetKeys(ops);
    }

    public override string ToString()
    {
        return baseDecoder + "[FlatMapped]";
    }
}

internal sealed class ResultMappedCodec<T>(Codec<T> baseCodec, Codec<T>.IResultMapper mapper) : Codec<T>
{
    public override ValueHolder<string> CodecNameHolder => ValueHolder.Create(() => baseCodec.ToString($"[ResultMapped {mapper}]"));

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject prefix)
        where TObject : default
    {
        return mapper.CoApply(ops, input, baseCodec.Encode(input, ops, prefix));
    }

    public override DataResult<(T, TObject)> Decode<TObject>(DynamicOps<TObject> ops, TObject input)
        where TObject : default
    {
        return mapper.Apply(ops, input, baseCodec.Decode(ops, input));
    }
}

internal sealed class ResultMappedMapCodec<T>(MapCodec<T> baseCodec, MapCodec<T>.IResultMapper mapper) : MapCodec<T>
{
    public override ValueHolder<string> CodecNameHolder => ValueHolder.Create(() => baseCodec.ToString($"[ResultMapped {mapper}]"));

    public override RecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return mapper.CoApply(ops, input, baseCodec.Encode(input, ops, prefix));
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return mapper.Apply(ops, input, baseCodec.Decode(ops, input));
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return baseCodec.GetKeys(ops);
    }
}