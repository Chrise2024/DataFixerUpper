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
    public override ValueHolder<string> CodecNameHolder => baseCodec.CodecNameHolder;

    public override IRecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        DataResult<TObject> encoded = baseCodec.EncodeStart(ops, input);
        if (ops.CompressMaps())
        {
            return prefix.Add(CompressedValueKey, encoded);
        }

        DataResult<IMapLike<TObject>> mapResult = encoded.FlatMap(ops.GetMap);
        return mapResult.Map(prefix.AddRange).GetResultOrDefault(prefix.WithErrorsFrom(mapResult));
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        if (!ops.CompressMaps())
        {
            return baseCodec.Parse(ops, ops.CreateMap(input));
        }

        TObject? value = input[CompressedValueKey];
        return value is null
            ? DataResult.CreateError<T>("Missing value")
            : baseCodec.Parse(ops, value);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        yield return ops.CreateString(CompressedValueKey);
    }

    public override Codec<T> AsCodec()
    {
        return baseCodec;
    }
}