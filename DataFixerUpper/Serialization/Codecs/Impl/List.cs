using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class ListCodec<T>(Codec<T> elementCodec, int minSize = 0, int maxSize = int.MaxValue, bool mutable = false) : Codec<IList<T>>
{
    private readonly Codec<T> _elementCodec = elementCodec;

    private readonly int _minSize = minSize;

    private readonly int _maxSize = maxSize;

    private readonly bool _mutable = mutable;

    public override ValueHolder<string> CodecNameHolder => $"ListCodec[{_elementCodec}]";

    public override DataResult<TObject> Encode<TObject>(IList<T> input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        if (input.Count < _minSize || input.Count > _maxSize)
        {
            return GetInvalidLength<TObject>(input.Count);
        }

        ListBuilderBase<TObject> builderBase = ops.CreateListBuilder();
        builderBase.AddRange(input, _elementCodec);
        return builderBase.Build(prefix);
    }

    public override DataResult<Pair<IList<T>, TObject?>> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return ops.GetList(input).FlatMap(l =>
            {
                LinkedList<T> values = new();
                ImmutableList<TObject?>.Builder fails = System.Collections.Immutable.ImmutableList.CreateBuilder<TObject?>();
                DataResult<Unit> initResult = DataResult.CreateSuccess(Unit.Instance, Lifecycle.Stable);
                int count = 0;
                foreach (TObject? element in l)
                {
                    count++;
                    if (count > _maxSize)
                    {
                        fails.Add(element);
                        continue;
                    }

                    DataResult<Pair<T, TObject?>> elementResult = _elementCodec.Decode(ops, element);
                    elementResult.IfError(_ => fails.Add(element));
                    if (elementResult.TryGetResultOrPartial(out Pair<T, TObject?> result))
                    {
                        values.AddLast(result.First);
                    }

                    initResult = initResult.CombineStable(Functions.LiftFirst, elementResult);
                }

                if (values.Count < _minSize)
                {
                    return GetInvalidLength<Pair<IList<T>, TObject?>>(values.Count);
                }

                if (count > _maxSize)
                {
                    initResult = initResult.CombineStable(Functions.LiftFirst, GetInvalidLength<Unit>(count));
                }

                TObject errors = ops.CreateList(fails.ToImmutable());
                IList<T> resultValues = _mutable ? new List<T>(values) : System.Collections.Immutable.ImmutableList.CreateRange(values);
                Pair<IList<T>, TObject?> rp = Pair.Create<IList<T>, TObject?>(resultValues, errors);

                return initResult.Map<Pair<IList<T>, TObject?>>(_ => rp).SetPartial(rp);
            }
        );
    }

    private DataResult<TR> GetInvalidLength<TR>(int length)
    {
        return DataResult.CreateError<TR>($"List size {length} is out of bounds [{_minSize}, {_maxSize}]");
    }

    public override bool Equals(object? obj)
    {
        return obj is ListCodec<T> codec
            && _elementCodec.Equals(codec._elementCodec)
            && _minSize.Equals(codec._minSize)
            && _maxSize.Equals(codec._maxSize)
            && _mutable.Equals(codec._mutable);
    }

    public override int GetHashCode()
    {
        int hash = _elementCodec.GetHashCode();
        hash = hash * 31 + _minSize.GetHashCode();
        hash = hash * 31 + _maxSize.GetHashCode();
        hash = hash * 31 + _mutable.GetHashCode();
        return hash;
    }
}

internal sealed class ArrayCodec<T>(Codec<T> elementCodec, int length)
    : Codec<T[]>
{
    private readonly Codec<T> _elementCodec = elementCodec;

    private readonly int _length = length;

    public override ValueHolder<string> CodecNameHolder => $"ArrayCodec[{_elementCodec}]";

    public override DataResult<TObject> Encode<TObject>(T[] input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        if (input.Length != _length)
        {
            return GetInvalidLength<TObject>(input.Length);
        }

        ListBuilderBase<TObject> builderBase = ops.CreateListBuilder();
        builderBase.AddRange(input, _elementCodec);
        return builderBase.Build(prefix);
    }

    public override DataResult<Pair<T[], TObject?>> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return ops.GetList(input).FlatMap(l =>
            {
                TObject?[] objects = l.ToArray();

                if (objects.Length != _length)
                {
                    return GetInvalidLength<Pair<T[], TObject?>>(objects.Length);
                }

                ImmutableList<TObject?>.Builder fails = System.Collections.Immutable.ImmutableList.CreateBuilder<TObject?>();

                DataResult<Unit> initResult = DataResult.CreateSuccess(Unit.Instance);
                T[] resultArray = new T[_length];

                for (int i = 0; i < _length; i++)
                {
                    TObject? element = objects[i];
                    DataResult<Pair<T, TObject?>> elementResult = _elementCodec.Decode(ops, element);
                    elementResult.IfError(_ => fails.Add(element));
                    if (elementResult.TryGetResultOrPartial(out Pair<T, TObject?> result))
                    {
                        resultArray[i] = result.First;
                    }

                    initResult = initResult.CombineStable(Functions.LiftFirst, elementResult);
                }

                Pair<T[], TObject?> rp = Pair.Create<T[], TObject?>(resultArray, ops.CreateList(fails.ToImmutable()));
                return initResult.Map<Pair<T[], TObject?>>(_ => rp).SetPartial(rp);
            }
        );
    }

    private DataResult<TR> GetInvalidLength<TR>(int l)
    {
        return DataResult.CreateError<TR>($"Array size {l} is invalid, expect {_length}");
    }

    public override bool Equals(object? obj)
    {
        return obj is ArrayCodec<T> codec && _elementCodec.Equals(codec._elementCodec) && _length.Equals(codec._length);
    }

    public override int GetHashCode()
    {
        return _elementCodec.GetHashCode() + _length.GetHashCode() * 31;
    }
}