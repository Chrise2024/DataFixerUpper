using System.Collections.Generic;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class FieldEncoder<T>(string name, IEncoder<T> encoder) : MapEncoderBase<T>
{
    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        yield return ops.CreateString(name);
    }

    public override RecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return prefix.Add(name, encoder.EncodeStart(ops, input));
    }
}

internal sealed class FieldDecoder<T>(string name, IDecoder<T> decoder) : MapDecoderBase<T>
{
    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        yield return ops.CreateString(name);
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        TObject value = input[name];
        return value is null
            ? DataResult.CreateError<T>($"No key {name} in {input}")
            : decoder.Parse(ops, value);
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
    public override ValueHolder<string> CodecNameHolder => $"OptionalField[{name}:{baseCodec}]";

    public OptionalFieldCodec(string name, Codec<T> baseCodec, bool lenient = false) : this(name, baseCodec, lenient, Lifecycle.Stable, Optional<T>.Empty, Lifecycle.Stable) { }

    public OptionalFieldCodec(string name, Codec<T> baseCodec, bool lenient, Optional<T> defaultValue) : this(name, baseCodec, lenient, Lifecycle.Stable, defaultValue, Lifecycle.Stable) { }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        yield return ops.CreateString(name);
    }

    public override RecordBuilder<TObject> Encode<TObject>(Optional<T> input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return input.HasValue ? prefix.Add(name, baseCodec.EncodeStart(ops, input.Value)) : prefix;
    }

    public override DataResult<Optional<T>> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        TObject value = input[name];
        if (value is null)
        {
            return DefaultResult();
        }

        DataResult<T> result = baseCodec.Parse(ops, value);
        if (result.IsError && lenient)
        {
            return DefaultResult();
        }

        return result.Map(Optional.Create).SetPartial(result.GetResultOrPartial).SetLifecycle(fieldLifecycle);
    }

    private DataResult<Optional<T>> DefaultResult()
    {
        return DataResult.CreateSuccess(defaultValue, defaultLifecycle);
    }
}