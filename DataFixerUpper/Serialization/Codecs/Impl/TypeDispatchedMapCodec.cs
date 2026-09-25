using System;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

/// <summary>
/// Dispatch codec via <typeparamref name="TType"/>.
/// </summary>
/// <typeparam name="TType">Type of object's type</typeparam>
/// <typeparam name="TValue">Type of object's content.</typeparam>
internal sealed class TypeDispatchMapCodec<TType, TValue> : MapCodec<TValue>
{
    private const string DefaultTypeKey = "type";
    private const string CompressedValueKey = "value";
    private readonly MapCodec<TType> _typeCodec;
    private readonly Func<TValue, DataResult<TType>> _typeSelector;
    private readonly Func<TValue, DataResult<IMapEncoder<TValue>>> _encoderDispatcher;
    private readonly Func<TType, DataResult<IMapDecoder<TValue>>> _decoderDispatcher;

    public TypeDispatchMapCodec(
        MapCodec<TType> typeCodec,
        Func<TValue, DataResult<TType>> typeSelector,
        Func<TValue, DataResult<IMapEncoder<TValue>>> encoderDispatcher,
        Func<TType, DataResult<IMapDecoder<TValue>>> decoderDispatcher
    )
    {
        _typeCodec = typeCodec;
        _typeSelector = typeSelector;
        _encoderDispatcher = encoderDispatcher;
        _decoderDispatcher = decoderDispatcher;
    }

    public TypeDispatchMapCodec(
        Codec<TType> typeCodec,
        Func<TValue, DataResult<TType>> typeSelector,
        Func<TValue, DataResult<IMapEncoder<TValue>>> encoderDispatcher,
        Func<TType, DataResult<IMapDecoder<TValue>>> decoderDispatcher
    ) : this(typeCodec.Field(DefaultTypeKey), typeSelector, encoderDispatcher, decoderDispatcher) { }

    public TypeDispatchMapCodec(
        MapCodec<TType> typeCodec,
        Func<TValue, DataResult<TType>> typeSelector,
        Func<TType, DataResult<MapCodec<TValue>>> codecDispatcher
    )
    {
        _typeCodec = typeCodec;
        _typeSelector = typeSelector;
        _encoderDispatcher = v => typeSelector
            .Then(r => r.FlatMap(codecDispatcher).Map(codec => codec.AsMapEncoder()))(v);
        _decoderDispatcher = t => codecDispatcher
            .Then(r => r.Map(codec => codec.AsMapDecoder()))(t);
    }

    public TypeDispatchMapCodec(
        Codec<TType> typeCodec,
        Func<TValue, DataResult<TType>> typeSelector,
        Func<TType, DataResult<MapCodec<TValue>>> codecDispatcher
    ) : this(typeCodec.Field(DefaultTypeKey), typeSelector, codecDispatcher) { }

    public override ValueHolder<string> CodecNameHolder => $"TypeDispatchCodec[{typeof(TType).Name} {typeof(TValue).Name}]";

    public override IRecordBuilder<TObject> Encode<TObject>(TValue input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        DataResult<IMapEncoder<TValue>> encoderResult = _encoderDispatcher.Apply(input);
        DataResult<TType> typeResult = _typeSelector.Apply(input);

        IRecordBuilder<TObject> builder = prefix.WithErrorsFrom(encoderResult).WithErrorsFrom(typeResult);
        if (!encoderResult.TryGetResult(out IMapEncoder<TValue>? valueEncoder) || !typeResult.TryGetResult(out TType? type))
        {
            return builder;
        }

        if (ops.CompressMaps())
        {
            return _typeCodec.Encode(type, ops, builder).Add(CompressedValueKey, valueEncoder.AsEncoder().EncodeStart(ops, input));
        }

        return _typeCodec.Encode(type, ops, valueEncoder.Encode(input, ops, builder));
    }

    public override DataResult<TValue> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        return _typeCodec.Decode(ops, input).FlatMap(type =>
            {
                return _decoderDispatcher.Apply(type).FlatMap(decoder =>
                    {
                        if (!ops.CompressMaps())
                        {
                            return decoder.Decode(ops, input);
                        }

                        TObject? value = input[CompressedValueKey];
                        if (value is null)
                        {
                            return DataResult.CreateError<TValue>($"Input does not have a \"value\" entry: {input}");
                        }

                        return decoder.AsDecoder().Parse(ops, value);
                    }
                );
            }
        );
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _typeCodec.GetKeys(ops).Append(ops.CreateString(CompressedValueKey));
    }
}