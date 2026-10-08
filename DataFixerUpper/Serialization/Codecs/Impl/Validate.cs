using System;
using System.Collections.Generic;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class ValidateCodec<T>(Codec<T> baseCodec, Func<T, DataResult<T>> validator) : Codec<T>
{
    private readonly Codec<T> _baseCodec = baseCodec;

    private readonly Func<T, DataResult<T>> _validator = validator;

    public override ValueHolder<string> CodecNameHolder => _baseCodec.CodecNameHolder;

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return _validator.Apply(input).FlatMap(validated => _baseCodec.Encode(validated, ops, prefix));
    }

    public override DataResult<Pair<T, TObject?>> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _baseCodec.Decode(ops, input).FlatMap(result =>
            {
                T value = result.First;
                TObject? remainder = result.Second;
                return _validator.Apply(value).Map(validated => Pair.Create(validated, remainder));
            }
        );
    }

    public override bool Equals(object? obj)
    {
        return obj is ValidateCodec<T> codec && _baseCodec.Equals(codec._baseCodec) && _validator.Equals(codec._validator);
    }

    public override int GetHashCode()
    {
        return _baseCodec.GetHashCode() + _validator.GetHashCode() * 31;
    }
}

internal sealed class ValidateMapCodec<T>(MapCodec<T> baseCodec, Func<T, DataResult<T>> validator) : MapCodec<T>
{
    private readonly MapCodec<T> _baseCodec = baseCodec;

    private readonly Func<T, DataResult<T>> _validator = validator;

    public override ValueHolder<string> CodecNameHolder => _baseCodec.CodecNameHolder;

    public override RecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        DataResult<T> validated = _validator.Apply(input);
        RecordBuilder<TObject> result = prefix.WithErrorsFrom(validated);
        return validated.Map(v => _baseCodec.Encode(v, ops, prefix)).GetResultOrDefault(result);
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return _baseCodec.Decode(ops, input).FlatMap(_validator);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _baseCodec.GetKeys(ops);
    }

    public override bool Equals(object? obj)
    {
        return obj is ValidateMapCodec<T> codec && _baseCodec.Equals(codec._baseCodec) && _validator.Equals(codec._validator);
    }

    public override int GetHashCode()
    {
        return _baseCodec.GetHashCode() + _validator.GetHashCode() * 31;
    }
}