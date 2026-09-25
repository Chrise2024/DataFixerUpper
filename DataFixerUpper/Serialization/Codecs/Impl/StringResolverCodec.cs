using System;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class StringResolverCodec<T>(Func<T, string?> toString, Func<string, T?> fromString) : Codec<T>
{
    public override ValueHolder<string> CodecNameHolder => $"StringResolver[{typeof(T).Name}]";

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        string? str = toString(input);
        if (str is null)
        {
            return DataResult.CreateError<TObject>($"Element with unknown name: {input}");
        }

        return ops.MergeToPrimitive(prefix, ops.CreateString(str));
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return ops.GetStringValue(input).FlatMap<(T, TObject?)>(t =>
            {
                T? item = fromString(t);
                if (item is null)
                {
                    return DataResult.CreateError<(T, TObject?)>($"Unknown element name: {t}");
                }

                return DataResult.CreateSuccess((item, input));
            }
        );
    }
}