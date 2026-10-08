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
    private readonly IEncoder<TOri> _baseEncoder = baseEncoder;

    private readonly Func<TNew, TOri> _func = func;

    public override DataResult<TObject> Encode<TObject>(TNew input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return _baseEncoder.Encode(_func.Apply(input), ops, prefix);
    }

    public override string ToString()
    {
        return _baseEncoder + "[CoMapped]";
    }

    public override bool Equals(object? obj)
    {
        return obj is CoMappedEncoder<TNew, TOri> encoder && _baseEncoder.Equals(encoder._baseEncoder) && _func.Equals(encoder._func);
    }

    public override int GetHashCode()
    {
        return _baseEncoder.GetHashCode() + _func.GetHashCode() * 31;
    }
}

internal sealed class CoMappedMapEncoder<TNew, TOri>(IMapEncoder<TOri> baseEncoder, Func<TNew, TOri> mapper)
    : MapEncoderBase<TNew>
{
    private readonly IMapEncoder<TOri> _baseEncoder = baseEncoder;

    private readonly Func<TNew, TOri> _mapper = mapper;

    public override RecordBuilder<TObject> Encode<TObject>(TNew input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return _baseEncoder.Encode(_mapper.Apply(input), ops, prefix);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _baseEncoder.GetKeys(ops);
    }

    public override string ToString()
    {
        return _baseEncoder + "[CoMapped]";
    }

    public override bool Equals(object? obj)
    {
        return obj is CoMappedMapEncoder<TNew, TOri> encoder && _baseEncoder.Equals(encoder._baseEncoder) && _mapper.Equals(encoder._mapper);
    }

    public override int GetHashCode()
    {
        return _baseEncoder.GetHashCode() + _mapper.GetHashCode() * 31;
    }
}

internal sealed class FlatCoMappedEncoder<TNew, TOri>(IEncoder<TOri> baseEncoder, Func<TNew, DataResult<TOri>> func)
    : EncoderBase<TNew>
{
    private readonly IEncoder<TOri> _baseEncoder = baseEncoder;

    private readonly Func<TNew, DataResult<TOri>> _func = func;

    public override DataResult<TObject> Encode<TObject>(TNew input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return _func.Apply(input).FlatMap(mapped => _baseEncoder.Encode(mapped, ops, prefix));
    }

    public override string ToString()
    {
        return _baseEncoder + "[FlatCoMapped]";
    }

    public override bool Equals(object? obj)
    {
        return obj is FlatCoMappedEncoder<TNew, TOri> encoder && _baseEncoder.Equals(encoder._baseEncoder) && _func.Equals(encoder._func);
    }

    public override int GetHashCode()
    {
        return _baseEncoder.GetHashCode() + _func.GetHashCode() * 31;
    }
}

internal sealed class FlatCoMappedMapEncoder<TNew, TOri>(IMapEncoder<TOri> baseEncoder, Func<TNew, DataResult<TOri>> mapper)
    : MapEncoderBase<TNew>
{
    private readonly IMapEncoder<TOri> _baseEncoder = baseEncoder;

    private readonly Func<TNew, DataResult<TOri>> _mapper = mapper;

    public override RecordBuilder<TObject> Encode<TObject>(TNew input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        DataResult<TOri> oriResult = _mapper.Apply(input);
        RecordBuilder<TObject> resultBuilder = prefix.WithErrorsFrom(oriResult);
        return oriResult.Map(r => _baseEncoder.Encode(r, ops, resultBuilder)).GetResultOrDefault(resultBuilder);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _baseEncoder.GetKeys(ops);
    }

    public override string ToString()
    {
        return _baseEncoder + "[FlatCoMapped]";
    }

    public override bool Equals(object? obj)
    {
        return obj is FlatCoMappedMapEncoder<TNew, TOri> encoder && _baseEncoder.Equals(encoder._baseEncoder) && _mapper.Equals(encoder._mapper);
    }

    public override int GetHashCode()
    {
        return _baseEncoder.GetHashCode() + _mapper.GetHashCode() * 31;
    }
}

internal sealed class MappedDecoder<TOri, TNew>(IDecoder<TOri> baseDecoder, Func<TOri, TNew> func)
    : DecoderBase<TNew>
{
    private readonly IDecoder<TOri> _baseDecoder = baseDecoder;

    private readonly Func<TOri, TNew> _func = func;

    public override DataResult<(TNew, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _baseDecoder.Decode(ops, input).Map(result => result.MapFirst(_func));
    }

    public override string ToString()
    {
        return _baseDecoder + "[Mapped]";
    }

    public override bool Equals(object? obj)
    {
        return obj is MappedDecoder<TOri, TNew> decoder && _baseDecoder.Equals(decoder._baseDecoder) && _func.Equals(decoder._func);
    }

    public override int GetHashCode()
    {
        return _baseDecoder.GetHashCode() + _func.GetHashCode() * 31;
    }
}

internal sealed class MappedMapDecoder<TOri, TNew>(IMapDecoder<TOri> baseDecoder, Func<TOri, TNew> mapper)
    : MapDecoderBase<TNew>
{
    private readonly IMapDecoder<TOri> _baseDecoder = baseDecoder;

    private readonly Func<TOri, TNew> _mapper = mapper;

    public override DataResult<TNew> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return _baseDecoder.Decode(ops, input).Map(_mapper);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _baseDecoder.GetKeys(ops);
    }

    public override string ToString()
    {
        return _baseDecoder + "[Mapped]";
    }

    public override bool Equals(object? obj)
    {
        return obj is MappedMapDecoder<TOri, TNew> decoder && _baseDecoder.Equals(decoder._baseDecoder) && _mapper.Equals(decoder._mapper);
    }

    public override int GetHashCode()
    {
        return _baseDecoder.GetHashCode() + _mapper.GetHashCode() * 31;
    }
}

internal sealed class DispatchMappedMapDecoder<TOri, TNew>(IMapDecoder<TOri> baseDecoder, IMapDecoder<Func<TOri, TNew>> dispatcher) : MapDecoderBase<TNew>
{
    private readonly IMapDecoder<TOri> _baseDecoder = baseDecoder;

    private readonly IMapDecoder<Func<TOri, TNew>> _dispatcher = dispatcher;

    public override DataResult<TNew> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return _baseDecoder.Decode(ops, input).FlatMap(ori => _dispatcher.Decode(ops, input).Map(dispatcherFunc => dispatcherFunc.Apply(ori)));
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _baseDecoder.GetKeys(ops).Concat(_dispatcher.GetKeys(ops));
    }

    public override bool Equals(object? obj)
    {
        return obj is DispatchMappedMapDecoder<TOri, TNew> decoder && _baseDecoder.Equals(decoder._baseDecoder) && _dispatcher.Equals(decoder._dispatcher);
    }

    public override int GetHashCode()
    {
        return _baseDecoder.GetHashCode() + _dispatcher.GetHashCode() * 31;
    }
}

internal sealed class FlatMappedDecoder<TOri, TNew>(IDecoder<TOri> baseDecoder, Func<TOri, DataResult<TNew>> func)
    : DecoderBase<TNew>
{
    private readonly IDecoder<TOri> _baseDecoder = baseDecoder;

    private readonly Func<TOri, DataResult<TNew>> _func = func;

    public override DataResult<(TNew, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _baseDecoder.Decode(ops, input).FlatMap(result => _func.Apply(result.First).Map(mapped => (mapped, result.Second)));
    }

    public override string ToString()
    {
        return _baseDecoder + "[FlatMapped]";
    }

    public override bool Equals(object? obj)
    {
        return obj is FlatMappedDecoder<TOri, TNew> decoder && _baseDecoder.Equals(decoder._baseDecoder) && _func.Equals(decoder._func);
    }

    public override int GetHashCode()
    {
        return _baseDecoder.GetHashCode() + _func.GetHashCode() * 31;
    }
}

internal sealed class PromptPartialDecoder<T>(IDecoder<T> baseDecoder, Consumer<string> onError) : DecoderBase<T>
{
    private readonly IDecoder<T> _baseDecoder = baseDecoder;

    private readonly Consumer<string> _onError = onError;

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _baseDecoder.Decode(ops, input).PromotePartial(_onError);
    }

    public override string ToString()
    {
        return _baseDecoder + "[PromotePartial]";
    }

    public override bool Equals(object? obj)
    {
        return obj is PromptPartialDecoder<T> decoder && _baseDecoder.Equals(decoder._baseDecoder) && _onError.Equals(decoder._onError);
    }

    public override int GetHashCode()
    {
        return _baseDecoder.GetHashCode() + _onError.GetHashCode() * 31;
    }
}

internal sealed class FlatMappedMapDecoder<TOri, TNew>(IMapDecoder<TOri> baseDecoder, Func<TOri, DataResult<TNew>> mapper)
    : MapDecoderBase<TNew>
{
    private readonly IMapDecoder<TOri> _baseDecoder = baseDecoder;

    private readonly Func<TOri, DataResult<TNew>> _mapper = mapper;

    public override DataResult<TNew> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return _baseDecoder.Decode(ops, input).FlatMap(_mapper);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _baseDecoder.GetKeys(ops);
    }

    public override string ToString()
    {
        return _baseDecoder + "[FlatMapped]";
    }

    public override bool Equals(object? obj)
    {
        return obj is FlatMappedMapDecoder<TOri, TNew> decoder && _baseDecoder.Equals(decoder._baseDecoder) && _mapper.Equals(decoder._mapper);
    }

    public override int GetHashCode()
    {
        return _baseDecoder.GetHashCode() + _mapper.GetHashCode() * 31;
    }
}

internal sealed class ResultMappedCodec<T>(Codec<T> baseCodec, Codec<T>.IResultMapper mapper) : Codec<T>
{
    private readonly Codec<T> _baseCodec = baseCodec;

    private readonly IResultMapper _mapper = mapper;

    public override ValueHolder<string> CodecNameHolder => _baseCodec.NewNameForTransform($"[ResultMapped {_mapper}]");

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return _mapper.CoApply(ops, input, _baseCodec.Encode(input, ops, prefix));
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _mapper.Apply(ops, input, _baseCodec.Decode(ops, input));
    }

    public override bool Equals(object? obj)
    {
        return obj is ResultMappedCodec<T> codec && _baseCodec.Equals(codec._baseCodec) && _mapper.Equals(codec._mapper);
    }

    public override int GetHashCode()
    {
        return _baseCodec.GetHashCode() + _mapper.GetHashCode() * 31;
    }
}

internal sealed class ResultMappedMapCodec<T>(MapCodec<T> baseCodec, MapCodec<T>.IResultMapper mapper) : MapCodec<T>
{
    private readonly MapCodec<T> _baseCodec = baseCodec;

    private readonly IResultMapper _mapper = mapper;

    public override ValueHolder<string> CodecNameHolder => ValueHolder.Create(() => _baseCodec.ToString($"[ResultMapped {_mapper}]"));

    public override RecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return _mapper.CoApply(ops, input, _baseCodec.Encode(input, ops, prefix));
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return _mapper.Apply(ops, input, _baseCodec.Decode(ops, input));
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _baseCodec.GetKeys(ops);
    }

    public override bool Equals(object? obj)
    {
        return obj is ResultMappedMapCodec<T> codec && _baseCodec.Equals(codec._baseCodec) && _mapper.Equals(codec._mapper);
    }

    public override int GetHashCode()
    {
        return _baseCodec.GetHashCode() + _mapper.GetHashCode() * 31;
    }
}