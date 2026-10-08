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
    private readonly MapCodec<TInstance> _baseCodec = baseCodec;

    private readonly MapCodec<TElement> _initialInstance = initialInstance;

    private readonly Func<TInstance, (TElement, MapCodec<TElement>)> _splitter = splitter;

    private readonly Func<TInstance, TElement, TInstance> _combiner = combiner;

    public override ValueHolder<string> CodecNameHolder => _baseCodec.NewNameForTransform($"Dependent[{_initialInstance}]");

    public override RecordBuilder<TObject> Encode<TObject>(TInstance input, DynamicOps<TObject> ops, RecordBuilder<TObject> prefix)
    {
        _baseCodec.Encode(input, ops, prefix);
        (TElement e, MapCodec<TElement> ec) = _splitter.Apply(input);
        ec.Encode(e, ops, prefix);
        return prefix.SetLifecycle(Lifecycle.Experimental);
    }

    public override DataResult<TInstance> Decode<TObject>(DynamicOps<TObject> ops, MapLike<TObject> input)
    {
        return _baseCodec.Decode(ops, input).FlatMap(i =>
            {
                (_, MapCodec<TElement> ec) = _splitter.Apply(i);
                DataResult<TElement> er = ec.Decode(ops, input);
                return er.Map(e => _combiner.Apply(i, e)).SetLifecycle(Lifecycle.Experimental);
            }
        );
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _baseCodec.GetKeys(ops).Concat(_initialInstance.GetKeys(ops));
    }

    public override bool Equals(object? obj)
    {
        return obj is DependentMapCodec<TInstance, TElement> codec
            && _baseCodec.Equals(codec._baseCodec)
            && _initialInstance.Equals(codec._initialInstance)
            && _splitter.Equals(codec._splitter)
            && _combiner.Equals(codec._combiner);
    }

    public override int GetHashCode()
    {
        int hash = _baseCodec.GetHashCode();
        hash = hash * 31 + _initialInstance.GetHashCode();
        hash = hash * 31 + _splitter.GetHashCode();
        hash = hash * 31 + _combiner.GetHashCode();
        return hash;
    }
}