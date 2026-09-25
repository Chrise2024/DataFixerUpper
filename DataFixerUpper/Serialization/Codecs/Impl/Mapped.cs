using System;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed record CoMappedEncoder<TNew, TOri>(IEncoder<TOri> BaseEncoder, Func<TNew, TOri> Func)
    : IEncoder<TNew>
{
    public DataResult<TObject> Encode<TObject>(TNew input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : notnull
    {
        return BaseEncoder.Encode(Func.Apply(input), ops, prefix);
    }

    public override string ToString()
    {
        return BaseEncoder + "[CoMapped]";
    }
}

internal sealed class CoMappedMapEncoder<TNew, TOri>(IMapEncoder<TOri> baseEncoder, Func<TNew, TOri> mapper)
    : MapEncoderBase<TNew>
{
    public override IRecordBuilder<TObject> Encode<TObject>(TNew input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
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

internal sealed record FlatCoMappedEncoder<TNew, TOri>(IEncoder<TOri> BaseEncoder, Func<TNew, DataResult<TOri>> Func)
    : IEncoder<TNew>
{
    public DataResult<TObject> Encode<TObject>(TNew input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : notnull
    {
        return Func.Apply(input).FlatMap(mapped => BaseEncoder.Encode(mapped, ops, prefix));
    }

    public override string ToString()
    {
        return BaseEncoder + "[FlatCoMapped]";
    }
}

internal sealed class FlatCoMappedMapEncoder<TNew, TOri>(IMapEncoder<TOri> baseEncoder, Func<TNew, DataResult<TOri>> mapper)
    : MapEncoderBase<TNew>
{
    public override IRecordBuilder<TObject> Encode<TObject>(TNew input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        DataResult<TOri> oriResult = mapper.Apply(input);
        IRecordBuilder<TObject> resultBuilder = prefix.WithErrorsFrom(oriResult);
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

internal sealed record MappedDecoder<TOri, TNew>(IDecoder<TOri> BaseDecoder, Func<TOri, TNew> Func)
    : IDecoder<TNew>
{
    public DataResult<(TNew, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : notnull
    {
        return BaseDecoder.Decode(ops, input).Map(result => result.MapFirst(Func));
    }

    public override string ToString()
    {
        return BaseDecoder + "[Mapped]";
    }
}

internal sealed class MappedMapDecoder<TOri, TNew>(IMapDecoder<TOri> baseDecoder, Func<TOri, TNew> mapper)
    : MapDecoderBase<TNew>
{
    public override DataResult<TNew> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
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
    public override DataResult<TNew> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        return baseDecoder.Decode(ops, input).FlatMap(ori => dispatcher.Decode(ops, input).Map(dispatcherFunc => dispatcherFunc.Apply(ori)));
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return baseDecoder.GetKeys(ops).Concat(dispatcher.GetKeys(ops));
    }
}

internal sealed record FlatMappedDecoder<TOri, TNew>(IDecoder<TOri> BaseDecoder, Func<TOri, DataResult<TNew>> Func)
    : IDecoder<TNew>
{
    public DataResult<(TNew, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : notnull
    {
        return BaseDecoder.Decode(ops, input).FlatMap(result => Func.Apply(result.First).Map(mapped => (mapped, result.Second)));
    }

    public override string ToString()
    {
        return BaseDecoder + "[FlatMapped]";
    }
}

internal sealed record PromptPartialDecoder<T>(IDecoder<T> BaseDecoder, Consumer<string> OnError) : IDecoder<T>
{
    public DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : notnull
    {
        return BaseDecoder.Decode(ops, input).PromotePartial(OnError);
    }

    public override string ToString()
    {
        return BaseDecoder + "[PromotePartial]";
    }
}

internal sealed class FlatMappedMapDecoder<TOri, TNew>(IMapDecoder<TOri> baseDecoder, Func<TOri, DataResult<TNew>> mapper)
    : MapDecoderBase<TNew>
{
    public override DataResult<TNew> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
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

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return mapper.CoApply(ops, input, baseCodec.Encode(input, ops, prefix));
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return mapper.Apply(ops, input, baseCodec.Decode(ops, input));
    }
}

internal sealed class ResultMappedMapCodec<T>(MapCodec<T> baseCodec, MapCodec<T>.IResultMapper mapper) : MapCodec<T>
{
    public override ValueHolder<string> CodecNameHolder => ValueHolder.Create(() => baseCodec.ToString($"[ResultMapped {mapper}]"));

    public override IRecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        return mapper.CoApply(ops, input, baseCodec.Encode(input, ops, prefix));
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        return mapper.Apply(ops, input, baseCodec.Decode(ops, input));
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return baseCodec.GetKeys(ops);
    }
}