using System.Collections.Generic;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class FieldEncoder<T>(string name, IEncoder<T> encoder) : MapEncoderBase<T>
{
    private readonly string _name = name;

    private readonly IEncoder<T> _encoder = encoder;

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        yield return ops.CreateString(_name);
    }

    public override RecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return prefix.Add(_name, _encoder.EncodeStart(ops, input));
    }

    public override bool Equals(object? obj)
    {
        return obj is FieldEncoder<T> encoder && _name.Equals(encoder._name) && _encoder.Equals(encoder._encoder);
    }

    public override int GetHashCode()
    {
        return _name.GetHashCode() + _encoder.GetHashCode() * 31;
    }
}

internal sealed class FieldDecoder<T>(string name, IDecoder<T> decoder) : MapDecoderBase<T>
{
    private readonly string _name = name;

    private readonly IDecoder<T> _decoder = decoder;

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        yield return ops.CreateString(_name);
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        TObject? value = input[_name];
        return value is null
            ? DataResult.CreateError<T>($"No key {_name} in {input}")
            : _decoder.Parse(ops, value);
    }

    public override bool Equals(object? obj)
    {
        return obj is FieldDecoder<T> decoder && _name.Equals(decoder._name) && _decoder.Equals(decoder._decoder);
    }

    public override int GetHashCode()
    {
        return _name.GetHashCode() + _decoder.GetHashCode() * 31;
    }
}

internal sealed class OptionalFieldCodec<T>(
    string name,
    Codec<T> baseCodec,
    bool lenient,
    Lifecycle fieldLifecycle,
    Optional<T> defaultValue,
    Lifecycle defaultLifecycle
)
    : MapCodec<Optional<T>>

{
    private readonly string _name = name;

    private readonly Codec<T> _baseCodec = baseCodec;

    private readonly bool _lenient = lenient;

    private readonly Lifecycle _fieldLifecycle = fieldLifecycle;

    private readonly Optional<T> _defaultValue = defaultValue;

    private readonly Lifecycle _defaultLifecycle = defaultLifecycle;

    public override ValueHolder<string> CodecNameHolder => $"OptionalField[{_name}:{_baseCodec}]";

    public OptionalFieldCodec(string name, Codec<T> baseCodec, bool lenient = false) : this(name, baseCodec, lenient, Lifecycle.Stable, Optional<T>.Empty, Lifecycle.Stable) { }

    public OptionalFieldCodec(string name, Codec<T> baseCodec, bool lenient, Optional<T> defaultValue) : this(name, baseCodec, lenient, Lifecycle.Stable, defaultValue, Lifecycle.Stable) { }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        yield return ops.CreateString(_name);
    }

    public override RecordBuilder<TObject> Encode<TObject>(Optional<T> input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return input.HasValue ? prefix.Add(_name, _baseCodec.EncodeStart(ops, input.Value)) : prefix;
    }

    public override DataResult<Optional<T>> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        TObject? value = input[_name];
        if (value is null)
        {
            return DefaultResult();
        }

        DataResult<T> result = _baseCodec.Parse(ops, value);
        if (result.IsError && _lenient)
        {
            return DefaultResult();
        }

        return result.Map(Optional.Create).SetPartial(result.GetResultOrPartial).SetLifecycle(_fieldLifecycle);
    }

    private DataResult<Optional<T>> DefaultResult()
    {
        return DataResult.CreateSuccess(_defaultValue, _defaultLifecycle);
    }

    public override bool Equals(object? obj)
    {
        return obj is OptionalFieldCodec<T> codec
            && _name.Equals(codec._name)
            && _baseCodec.Equals(codec._baseCodec)
            && _lenient.Equals(codec._lenient)
            && _fieldLifecycle.Equals(codec._fieldLifecycle)
            && _defaultValue.Equals(codec._defaultValue)
            && _defaultLifecycle.Equals(codec._defaultLifecycle);
    }

    public override int GetHashCode()
    {
        int hash = _name.GetHashCode();
        hash = hash * 31 + _baseCodec.GetHashCode();
        hash = hash * 31 + _lenient.GetHashCode();
        hash = hash * 31 + _fieldLifecycle.GetHashCode();
        hash = hash * 31 + _defaultValue.GetHashCode();
        hash = hash * 31 + _defaultLifecycle.GetHashCode();
        return hash;
    }
}