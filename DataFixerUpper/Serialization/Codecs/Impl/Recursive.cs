using System;
using System.Collections.Generic;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class RecursiveCodec<T> : Codec<T>
{
    private readonly ValueHolder<Codec<T>> _wrapped;
    private readonly string _name;
    public override ValueHolder<string> CodecNameHolder => $"RecursiveCodec[{_name}]";

    public RecursiveCodec(string name, Func<Codec<T>, Codec<T>> wrapped)
    {
        _name = name;
        _wrapped = ValueHolder.Create(() => wrapped.Apply(this));
    }

    public override DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        return _wrapped.Value.Encode(input, ops, prefix);
    }

    public override DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return _wrapped.Value.Decode(ops, input);
    }
}

internal sealed class RecursiveMapCodec<T> : MapCodec<T>
{
    private readonly ValueHolder<MapCodec<T>> _wrapped;
    private readonly string _name;
    public override ValueHolder<string> CodecNameHolder => $"RecursiveMapCodec[{_name}]";

    public RecursiveMapCodec(string name, Func<Codec<T>, MapCodec<T>> wrapped)
    {
        _name = name;
        _wrapped = ValueHolder.Create(() => wrapped.Apply(AsCodec()));
    }

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return _wrapped.Value.GetKeys(ops);
    }

    public override IRecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
    {
        return _wrapped.Value.Encode(input, ops, prefix);
    }

    public override DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
    {
        return _wrapped.Value.Decode(ops, input);
    }
}