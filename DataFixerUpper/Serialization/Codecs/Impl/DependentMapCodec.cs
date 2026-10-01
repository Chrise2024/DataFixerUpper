using System;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal class DependentMapCodec<TInstance, TElement>(
    MapCodec<TInstance> baseCodec,
    MapCodec<TElement> initialInstance,
    Func<TInstance, (TElement, MapCodec<TElement>)> splitter,
    Func<TInstance, TElement, TInstance> combiner
) : MapCodec<TInstance>
{
    public override ValueHolder<string> CodecNameHolder => baseCodec.NewNameForTransform($"Dependent[{initialInstance}]");

    public override RecordBuilder<TObject> Encode<TObject>(TInstance input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        baseCodec.Encode(input, ops, prefix);
        (TElement e, MapCodec<TElement> ec) = splitter.Apply(input);
        ec.Encode(e, ops, prefix);
        return prefix.SetLifecycle(Lifecycle.Experimental);
    }

    public override DataResult<TInstance> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return baseCodec.Decode(ops, input).FlatMap(i =>
            {
                (_, MapCodec<TElement> ec) = splitter.Apply(i);
                DataResult<TElement> er = ec.Decode(ops, input);
                return er.Map(e => combiner.Apply(i, e)).SetLifecycle(Lifecycle.Experimental);
            }
        );
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return baseCodec.GetKeys(ops).Concat(initialInstance.GetKeys(ops));
    }
}