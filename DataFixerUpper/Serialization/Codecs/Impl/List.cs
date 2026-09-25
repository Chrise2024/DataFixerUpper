using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed class ListCodec<T>(Codec<T> elementCodec, int minSize = 0, int maxSize = int.MaxValue, bool mutable = false) : Codec<IList<T>>
{
    public override ValueHolder<string> CodecNameHolder => $"ListCodec[{elementCodec}]";

    public override DataResult<TObject> Encode<TObject>(IList<T> input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        if (input.Count < minSize || input.Count > maxSize)
        {
            return GetInvalidLength<TObject>(input.Count);
        }

        IListBuilder<TObject> builder = ops.CreateListBuilder();
        builder.AddRange(input, elementCodec);
        return builder.Build(prefix);
    }

    public override DataResult<(IList<T>, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return ops.GetListValues(input).FlatMap(l =>
            {
                LinkedList<T> values = new();
                ImmutableList<TObject?>.Builder fails = System.Collections.Immutable.ImmutableList.CreateBuilder<TObject?>();
                DataResult<Unit> initResult = DataResult.CreateSuccess(Unit.Instance, Lifecycle.Stable);
                int count = 0;
                foreach (TObject? element in l)
                {
                    count++;
                    if (count > maxSize)
                    {
                        fails.Add(element);
                        continue;
                    }

                    DataResult<(T, TObject?)> elementResult = elementCodec.Decode(ops, element);
                    elementResult.IfError(_ => fails.Add(element));
                    if (elementResult.TryGetResultOrPartial(out (T, TObject?) result))
                    {
                        values.AddLast(result.First);
                    }

                    initResult = initResult.CombineStable(Functions.LiftFirst, elementResult);
                }

                if (values.Count < minSize)
                {
                    return GetInvalidLength<(IList<T>, TObject?)>(values.Count);
                }

                if (count > maxSize)
                {
                    initResult = initResult.CombineStable(Functions.LiftFirst, GetInvalidLength<Unit>(count));
                }

                TObject errors = ops.CreateList(fails.ToImmutable());
                IList<T> resultValues = mutable ? new List<T>(values) : System.Collections.Immutable.ImmutableList.CreateRange(values);
                (IList<T> value, TObject result) rp = (resultValues, errors);

                return initResult.Map<(IList<T>, TObject?)>(_ => rp).SetPartial(rp);
            }
        );
    }

    private DataResult<TR> GetInvalidLength<TR>(int length)
    {
        return DataResult.CreateError<TR>($"List size {length} is out of bounds [{minSize}, {maxSize}]");
    }
}

internal sealed class ArrayCodec<T>(Codec<T> elementCodec, int length)
    : Codec<T[]>
{
    public override ValueHolder<string> CodecNameHolder => $"ArrayCodec[{elementCodec}]";

    public override DataResult<TObject> Encode<TObject>(T[] input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : default
    {
        if (input.Length != length)
        {
            return GetInvalidLength<TObject>(input.Length);
        }

        IListBuilder<TObject> builder = ops.CreateListBuilder();
        builder.AddRange(input, elementCodec);
        return builder.Build(prefix);
    }

    public override DataResult<(T[], TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return ops.GetListValues(input).FlatMap(l =>
            {
                TObject?[] objects = l.ToArray();

                if (objects.Length != length)
                {
                    return GetInvalidLength<(T[], TObject?)>(objects.Length);
                }

                ImmutableList<TObject?>.Builder fails =System.Collections.Immutable.ImmutableList.CreateBuilder<TObject?>();
                
                DataResult<Unit> initResult = DataResult.CreateSuccess(Unit.Instance);
                T[] resultArray = new T[length];

                for (int i = 0; i < length; i++)
                {
                    TObject? element = objects[i];
                    DataResult<(T, TObject?)> elementResult = elementCodec.Decode(ops, element);
                    elementResult.IfError(_ => fails.Add(element));
                    if (elementResult.TryGetResultOrPartial(out (T, TObject?) result))
                    {
                        resultArray[i] = result.First;
                    }
                    
                    initResult = initResult.CombineStable(Functions.LiftFirst, elementResult);
                }

                (T[], TObject) rp = (resultArray, ops.CreateList(fails.ToImmutable()));
                return initResult.Map<(T[], TObject?)>(_ => rp).SetPartial(rp);
            }
        );
    }

    private DataResult<TR> GetInvalidLength<TR>(int l)
    {
        return DataResult.CreateError<TR>($"Array size {l} is invalid, expect {length}");
    }
}