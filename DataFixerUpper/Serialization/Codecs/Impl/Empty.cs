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

    public override RecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        return prefix;
    }

    public override string ToString()
    {
        return "EmptyEncoder";
    }

    public override bool Equals(object? obj)
    {
        return obj is EmptyMapEncoder<T>;
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
}

internal sealed class ErrorEncoder<T>(string message) : EncoderBase<T>
{
    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return DataResult.CreateError<TObject>($"{message} {input}");
    }

    public override string ToString()
    {
        return $"ErrorEncoder[{message}]";
    }

    public override bool Equals(object? obj)
    {
        return obj is ErrorEncoder<T>;
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
}

internal sealed class ErrorDecoder<T>(string message) : DecoderBase<T>
{
    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return DataResult.CreateError<(T, TObject?)>(message);
    }

    public override string ToString()
    {
        return $"ErrorDecoder[{message}]";
    }

    public override bool Equals(object? obj)
    {
        return obj is ErrorDecoder<T>;
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }
}