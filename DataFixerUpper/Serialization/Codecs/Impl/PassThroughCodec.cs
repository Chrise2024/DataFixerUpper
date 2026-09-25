using System;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class PassThroughCodec : Codec<IDynamic>
{
    public override ValueHolder<string> CodecNameHolder => "PassThrough";

    public override DataResult<TObject> Encode<TObject>(IDynamic input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        if (input.IsEmpty())
        {
            return DataResult.CreateSuccess(prefix!);
        }

        TObject casted = input.Convert(ops).Value ?? throw new InvalidCastException("Casting not empty value into empty.");
        DataResult<TObject> toMap = ops.GetMapValues(casted).FlatMap(m => ops.MergeToMap(prefix, m));
        if (toMap.TryGetResult(out TObject? mr))
        {
            return DataResult.CreateSuccess(mr);
        }

        DataResult<TObject> toList = ops.GetListValues(casted).FlatMap(l => ops.MergeToList(prefix, l));
        if (toList.TryGetResult(out TObject? lr))
        {
            return DataResult.CreateSuccess(lr);
        }

        return DataResult.CreateError<TObject>($"Don't know how to merge {prefix} and {casted}");
    }

    public override DataResult<(IDynamic, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return DataResult.CreateSuccess(((IDynamic) new Dynamic<TObject>(ops, input), ops.Empty()));
    }
}