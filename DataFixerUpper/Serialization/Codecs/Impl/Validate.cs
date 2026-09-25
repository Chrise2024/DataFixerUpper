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
    public override ValueHolder<string> CodecNameHolder => baseCodec.CodecNameHolder;

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return validator.Apply(input).FlatMap(validated => baseCodec.Encode(validated, ops, prefix));
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return baseCodec.Decode(ops, input).FlatMap(result =>
            {
                T value = result.Item1;
                TObject? remainder = result.Item2;
                return validator.Apply(value).Map(validated => (validated, remainder));
            }
        );
    }
}

internal sealed class ValidateMapCodec<T>(MapCodec<T> baseCodec, Func<T, DataResult<T>> validator) : MapCodec<T>
{
    public override ValueHolder<string> CodecNameHolder => baseCodec.CodecNameHolder;

    public override IRecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        DataResult<T> validated = validator.Apply(input);
        IRecordBuilder<TObject> result = prefix.WithErrorsFrom(validated);
        return validated.Map(v => baseCodec.Encode(v, ops, prefix)).GetResultOrDefault(result);
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        return baseCodec.Decode(ops, input).FlatMap(validator);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return baseCodec.GetKeys(ops);
    }
}