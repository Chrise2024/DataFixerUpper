using System.Collections.Generic;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

/// <summary>
/// Transform <see cref="T:DataFixerUpper.Codecs.Codec`1"/> into <see cref="T:DataFixerUpper.Codecs.MapCodec`1"/>
/// </summary>
internal sealed class CodecMapCodec<T>(Codec<T> baseCodec) : MapCodec<T>
{
    private const string CompressedValueKey = "value";

    private readonly Codec<T> _baseCodec = baseCodec;

    public override ValueHolder<string> CodecNameHolder => _baseCodec.CodecNameHolder;

    public override RecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        DataResult<TObject> encoded = _baseCodec.EncodeStart(ops, input);
        if (ops.CompressMaps())
        {
            return prefix.Add(CompressedValueKey, encoded);
        }

        DataResult<MapLike<TObject>> mapResult = encoded.FlatMap(ops.GetMap);
        return mapResult.Map(prefix.AddRange).GetResultOrDefault(prefix.WithErrorsFrom(mapResult));
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        if (!ops.CompressMaps())
        {
            return _baseCodec.Parse(ops, ops.CreateMap(input));
        }

        TObject? value = input[CompressedValueKey];
        return value is null
            ? DataResult.CreateError<T>("Missing value")
            : _baseCodec.Parse(ops, value);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        yield return ops.CreateString(CompressedValueKey);
    }

    public override Codec<T> AsCodec()
    {
        return _baseCodec;
    }

    public override bool Equals(object? obj)
    {
        return obj is CodecMapCodec<T> codec && _baseCodec.Equals(codec._baseCodec);
    }

    public override int GetHashCode()
    {
        return _baseCodec.GetHashCode();
    }
}