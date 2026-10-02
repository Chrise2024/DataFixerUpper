# DataFixerUpper

.Net porting of [com.mojang.datafixerupper](https://github.com/mojang/datafixerupper)@10.0.21.

Serialization part only.

## Basic Usage

```csharp
string jsonString = "[1,2,3,4,5]";
JsonNode json = JsonNode.Parse(jsonString)!;
Codec<IList<int>> intListCodec = Codec.Int.List(0, 100);
DataResult<IList<int>> result = intListCodec.Parse(JsonOps.Instance, json);
result.IfSuccess(l => Console.WriteLine(l.Sum()));
```

See also:

[Fabric Doc](https://docs.fabricmc.net/develop/serialization/codecs)

[Neoforge Doc](https://docs.neoforged.net/docs/datastorage/codecs)

## Api Reference

### Types

- Nested type under generic classes is moved to same name static class.

- `K1` -> `Anchor`.

- `Map<K, V>` -> `IDictionary<TKey, TValue>`.

- `Stream<T>` -> `IEnumerable<T>`.

- `ByteBuffer` -> `Stream`.

### Methods

- Method name's `Else` -> `Default`(`orElse` -> `OrDefault`).

- Static method of `Codec<A>` to create `MapCodec<A>` is moved to static class `MapCodec`.

- Static method to create `Codec<A>` and `MapCodec<A>` is renamed with prefix `Create`.

- `Codec<A>` and `MapCodec<A>`'s derive method end with `Of` is renamed without `Of`(`listOf` -> `List`).

- `DataResult<R>.apply2/3` -> `DataResult<T>.Combine`.

- `DataResult<R>.ap` -> `DataResult<T>.Map`.

- `Applicative<F, Mu>.ap` -> `Applicative<TFunctor, TMu>.Select`.

- `Applicative<F, Mu>.apN` -> `Applicative<TFunctor, TMu>.Combine`.

- `Applicative<F, Mu>.LiftN` -> `Applicative<TFunctor, TMu>.Lift`.