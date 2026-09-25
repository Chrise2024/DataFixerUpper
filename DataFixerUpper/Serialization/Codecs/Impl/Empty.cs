using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class EmptyMapEncoder<T> : MapEncoderBase<T>
{
    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return Enumerable.Empty<TObject>();
    }

    public override IRecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        return prefix;
    }

    public override string ToString()
    {
        return "EmptyEncoder";
    }
}

internal sealed record ErrorEncoder<T>(string Message) : IEncoder<T>
{
    public DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : notnull
    {
        return DataResult.CreateError<TObject>($"{Message} {input}");
    }

    public override string ToString()
    {
        return $"ErrorEncoder[{Message}]";
    }
}

internal sealed record ErrorDecoder<T>(string Message) : IDecoder<T>
{
    public DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : notnull
    {
        return DataResult.CreateError<(T, TObject?)>(Message);
    }

    public override string ToString()
    {
        return $"ErrorDecoder[{Message}]";
    }
}