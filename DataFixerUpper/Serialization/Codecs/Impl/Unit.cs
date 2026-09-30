using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class UnitMapCodec<T>(ValueHolder<T> valueHolder) : MapCodec<T>
{
    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return Enumerable.Empty<TObject>();
    }

    public override ValueHolder<string> CodecNameHolder => $"Unit[{valueHolder.Value}]";

    public override RecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return prefix;
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return DataResult.CreateSuccess(valueHolder.Value);
    }

    public override Codec<T> AsCodec()
    {
        return new UnitCodec<T>(valueHolder);
    }
}

internal sealed class UnitMapDecoder<T>(ValueHolder<T> valueHolder) : MapDecoderBase<T>

{
    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return DataResult.CreateSuccess(valueHolder.Value);
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return Enumerable.Empty<TObject>();
    }

    public override string ToString()
    {
        return $"UnitDecoder[{valueHolder.Value}]";
    }
}

internal sealed class UnitCodec<T>(ValueHolder<T> valueHolder) : Codec<T>
{
    public override ValueHolder<string> CodecNameHolder => $"Unit[{valueHolder.Value}]";

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject prefix)
        where TObject : default
    {
        return ops.MergeToMap(prefix, MapLike<TObject>.Empty);
    }

    public override DataResult<(T, TObject)> Decode<TObject>(DynamicOps<TObject> ops, TObject input)
        where TObject : default
    {
        return ops.CompressMaps()
            ? ops.GetList(input).Map(_ => (valueHolder.Value, input))
            : ops.GetMapValues(input).Map(_ => (valueHolder.Value, input));
    }
}