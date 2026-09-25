using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs.Impl;

internal sealed record DictionaryCodecState<TKey, TValue>(
    Codec<TKey> KeyCodec,
    Func<TKey, Codec<TValue>> ValueCodecDispatcher,
    bool Mutable
)
    where TKey : notnull
{
    public DataResult<IDictionary<TKey, TValue>> DecodeDictionary<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> map)
        where TObject : notnull
    {
        IDictionary<TKey, TValue> read = Mutable
            ? new Dictionary<TKey, TValue>()
            : ImmutableDictionary.CreateBuilder<TKey, TValue>();
        ImmutableDictionary<TObject, TObject?>.Builder fails = ImmutableDictionary.CreateBuilder<TObject, TObject?>();

        DataResult<Unit> aggregatedResult = map.Aggregate(
            DataResult.CreateSuccess(Unit.Instance, Lifecycle.Stable),
            (seed, entry) =>
            {
                DataResult<TKey> keyResult = KeyCodec.Parse(ops, entry.Key);
                DataResult<TValue> valueResult = keyResult.FlatMap(k => ValueCodecDispatcher.Apply(k).Parse(ops, entry.Value));
                DataResult<KeyValuePair<TKey, TValue>> entryResult = keyResult.CombineStable(KeyValuePair.Create, valueResult);

                if (entryResult.TryGetResultOrPartial(out KeyValuePair<TKey, TValue> combinedEntry))
                {
                    if (!read.TryAdd(combinedEntry.Key, combinedEntry.Value))
                    {
                        fails.Add(entry);
                        seed = seed.CombineStable(
                            Functions.LiftFirst,
                            DataResult.CreateError<Unit>($"Duplicate entry for key: '{entry.Key}'")
                        );
                    }
                }

                if (entryResult.IsError)
                {
                    fails.Add(entry);
                }

                return seed.CombineStable(Functions.LiftFirst, entryResult);
            }
        );
        IDictionary<TKey, TValue> result = Mutable
            ? read
            : ((ImmutableDictionary<TKey, TValue>.Builder) read).ToImmutable();
        TObject errors = ops.CreateMap(fails.ToImmutable());
        return aggregatedResult.Map(_ => result).SetPartial(result).MapError(error => $"{error} missed input {errors}");
    }

    public IRecordBuilder<TObject> EncodeDictionary<TObject>(IDictionary<TKey, TValue> input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
        where TObject : notnull
    {
        return input.Aggregate(
            prefix,
            (seed, entry) => seed.Add(KeyCodec.EncodeStart(ops, entry.Key), ValueCodecDispatcher.Apply(entry.Key).EncodeStart(ops, entry.Value))
        );
    }
}

internal sealed class SimpleDictionaryCodec<TKey, TValue>(
    Codec<TKey> keyCodec,
    Codec<TValue> valueCodec,
    IKeyable keys,
    bool mutable = false
) :
    MapCodec<IDictionary<TKey, TValue>>
    where TKey : notnull
{
    private readonly DictionaryCodecState<TKey, TValue> _state = new(keyCodec, _ => valueCodec, mutable);
    public override ValueHolder<string> CodecNameHolder => $"SimpleMapCodec[{keyCodec} => {valueCodec}]";

    public override IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
    {
        return keys.GetKeys(ops);
    }

    public override IRecordBuilder<TObject> Encode<TObject>(
        IDictionary<TKey, TValue> input,
        DynamicOps<TObject> ops,
        IRecordBuilder<TObject> prefix
    )
    {
        return _state.EncodeDictionary(input, ops, prefix);
    }

    public override DataResult<IDictionary<TKey, TValue>> Decode<TObject>(
        DynamicOps<TObject> ops,
        IMapLike<TObject> input
    )
    {
        return _state.DecodeDictionary(ops, input);
    }
}

internal sealed class UnboundedDictionaryCodec<TKey, TValue>(
    Codec<TKey> keyCodec,
    Codec<TValue> valueCodec,
    bool mutable = false
) :
    Codec<IDictionary<TKey, TValue>>
    where TKey : notnull
{
    private readonly DictionaryCodecState<TKey, TValue> _state = new(keyCodec, _ => valueCodec, mutable);
    public override ValueHolder<string> CodecNameHolder => $"UnboundedMapCodec[{keyCodec} => {valueCodec}]";

    public override DataResult<TObject> Encode<TObject>(
        IDictionary<TKey, TValue> input,
        DynamicOps<TObject> ops,
        TObject? prefix
    )
        where TObject : default
    {
        return _state.EncodeDictionary(input, ops, ops.CreateMapBuilder()).Build(prefix);
    }

    public override DataResult<(IDictionary<TKey, TValue>, TObject?)> Decode<TObject>(
        DynamicOps<TObject> ops,
        TObject? input
    )
        where TObject : default
    {
        return ops.GetMap(input)
            .SetLifecycle(Lifecycle.Stable)
            .FlatMap(map => _state.DecodeDictionary(ops, map).Map(result => (result, input)));
    }
}

internal sealed class DispatchedDictionaryCodec<TKey, TValue>(
    Codec<TKey> keyCodec,
    Func<TKey, Codec<TValue>> valueCodecDispatcher,
    bool mutable = false
) :
    Codec<IDictionary<TKey, TValue>>
    where TKey : notnull
{
    public override ValueHolder<string> CodecNameHolder => $"UnboundedMapCodec[{keyCodec} => {valueCodecDispatcher}]";

    private readonly DictionaryCodecState<TKey, TValue> _state = new(keyCodec, valueCodecDispatcher, mutable);

    public override DataResult<TObject> Encode<TObject>(
        IDictionary<TKey, TValue> input,
        DynamicOps<TObject> ops,
        TObject? prefix
    )
        where TObject : default
    {
        return _state.EncodeDictionary(input, ops, ops.CreateMapBuilder()).Build(prefix);
    }

    public override DataResult<(IDictionary<TKey, TValue>, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : default
    {
        return ops.GetMap(input)
            .SetLifecycle(Lifecycle.Stable)
            .FlatMap(map => _state.DecodeDictionary(ops, map).Map(result => (result, input)));
    }
}