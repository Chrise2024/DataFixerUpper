using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Numerics;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Codecs;
using DataFixerUpper.Utils;

// ReSharper disable once CheckNamespace
namespace DataFixerUpper.Serialization.DynamicOps;

/// <summary>
/// Extensions for creating, reading and writing values with <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> and <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicLike`1"/>.
/// </summary>
public static class DynamicExtension
{
    #region DynamicOps.Create

    extension<TObject>(DynamicOps<TObject> ops)
        where TObject : notnull
    {
        /// <summary>
        /// Creates a serialized list of bytes from the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to read the bytes from.</param>
        /// <returns>The serialized list, or an empty list if <paramref name="stream"/> cannot be read.</returns>
        public TObject CreateStream(Stream stream)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent((int) stream.Length);
            TObject result;
            try
            {
                int n = stream.Read(buffer, 0, (int) stream.Length);
                result = ops.CreateList(buffer.Take(n).Select(ops.CreateNumber));
            }
            catch
            {
                result = ops.CreateList(Enumerable.Empty<TObject>());
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }

            return result;
        }

        /// <summary>
        /// Creates a serialized list of numbers from the given <paramref name="numbers"/>.
        /// </summary>
        /// <param name="numbers">The numbers to serialize.</param>
        /// <typeparam name="TNumber">The type of the numbers to serialize.</typeparam>
        /// <returns>The serialized list.</returns>
        public TObject CreateNumberList<TNumber>(IEnumerable<TNumber> numbers)
            where TNumber : INumber<TNumber>
        {
            return ops.CreateList(numbers.Select(ops.CreateNumber));
        }

        /// <summary>
        /// Creates an empty serialized list.
        /// </summary>
        /// <returns>The empty list.</returns>
        public TObject CreateList()
        {
            return ops.CreateList(Enumerable.Empty<TObject>());
        }

        /// <summary>
        /// Creates an empty serialized map.
        /// </summary>
        /// <returns>The empty map.</returns>
        public TObject CreateMap()
        {
            return ops.CreateMap(Enumerable.Empty<KeyValuePair<TObject, TObject?>>());
        }

        /// <summary>
        /// Returns a function that encodes a value with <paramref name="encoder"/>.
        /// </summary>
        /// <param name="encoder">The encoder to use.</param>
        /// <typeparam name="T">The type of the value to encode.</typeparam>
        /// <returns>A function that produces the encoded value, or an error if the value cannot be encoded.</returns>
        public Func<T, DataResult<TObject>> WithEncoder<T>(IEncoder<T> encoder)
        {
            return value => encoder.Encode(value, ops, ops.Empty());
        }

        /// <summary>
        /// Returns a function that decodes a serialized value with <paramref name="decoder"/>.
        /// </summary>
        /// <param name="decoder">The decoder to use.</param>
        /// <typeparam name="T">The type of the value to decode.</typeparam>
        /// <returns>A function that produces the decoded value together with the remaining input, or an error if the value cannot be decoded.</returns>
        public Func<TObject, DataResult<(T, TObject?)>> WithDecoder<T>(IDecoder<T> decoder)
        {
            return obj => decoder.Decode(ops, obj);
        }

        /// <summary>
        /// Returns a function that parses a serialized value with <paramref name="decoder"/>, returning only the decoded value.
        /// </summary>
        /// <param name="decoder">The decoder to use.</param>
        /// <typeparam name="T">The type of the value to decode.</typeparam>
        /// <returns>A function that produces the decoded value, or an error if the value cannot be decoded.</returns>
        public Func<TObject, DataResult<T>> WithParser<T>(IDecoder<T> decoder)
        {
            return obj => decoder.Parse(ops, obj);
        }
    }

    #endregion

    #region DynamicOps.Get

    extension<TObject>(DynamicOps<TObject> ops)
        where TObject : notnull
    {
        /// <summary>
        /// Reads the given <paramref name="input"/> as a stream of bytes.
        /// </summary>
        /// <param name="input">The serialized list of bytes to read.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the stream, or an error if <paramref name="input"/> is not a list or contains elements that are not bytes.</returns>
        public DataResult<Stream> GetStream(TObject? input)
        {
            return ops.GetNumberList<TObject, byte>(input).Map(Stream (l) => new MemoryStream(l.ToArray(), false));
        }

        /// <summary>
        /// Reads the given <paramref name="input"/> as a list of numbers.
        /// </summary>
        /// <param name="input">The serialized list to read.</param>
        /// <typeparam name="TNumber">The type of number to read.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the numbers, or an error if <paramref name="input"/> is not a list or contains elements that are not numbers of type <typeparamref name="TNumber"/>.</returns>
        public DataResult<IEnumerable<TNumber>> GetNumberList<TNumber>(TObject? input)
            where TNumber : INumber<TNumber>
        {
            return ops.GetListValues(input).FlatMap(l =>
                {
                    ImmutableList<TNumber>.Builder builder = ImmutableList.CreateBuilder<TNumber>();
                    DataResult<ImmutableList<TNumber>.Builder> initResult = DataResult.CreateSuccess(builder);
                    return l.Aggregate(
                        initResult,
                        (seed, obj) =>
                        {
                            DataResult<TNumber> result = ops.GetNumberValue<TNumber>(obj);
                            return seed.CombineStable(Functions.AddToFirst, result);
                        }
                    ).Map(IEnumerable<TNumber> (b) => b.ToImmutable());
                }
            );
        }

        /// <summary>
        /// Reads the given <paramref name="input"/> as a number, falling back to <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="input">The serialized value to read.</param>
        /// <param name="defaultValue">The value to return if <paramref name="input"/> is not a number.</param>
        /// <typeparam name="TNumber">The type of number to read.</typeparam>
        /// <returns>The number, or <paramref name="defaultValue"/> if <paramref name="input"/> is not a number of type <typeparamref name="TNumber"/>.</returns>
        public TNumber GetNumberValue<TNumber>(TObject? input, TNumber defaultValue)
            where TNumber : INumber<TNumber>
        {
            return ops.GetNumberValue<TNumber>(input).GetResultOrDefault(defaultValue);
        }
    }

    #endregion

    #region DynamicOps.Convert

    extension<TObject>(DynamicOps<TObject> ops)
        where TObject : notnull
    {
        /// <summary>
        /// Converts the given map <paramref name="input"/> to the map format of <paramref name="otherOp"/>.
        /// </summary>
        /// <param name="otherOp">The ops of the format to convert to.</param>
        /// <param name="input">The serialized map value to convert.</param>
        /// <typeparam name="TOther">The type the other ops serializes to and deserializes from.</typeparam>
        /// <returns>The converted map.</returns>
        public TOther ConvertMap<TOther>(DynamicOps<TOther> otherOp, TObject input)
            where TOther : notnull
        {
            IEnumerable<KeyValuePair<TOther, TOther?>> entries = ops.GetMapValues(input)
                .GetResultOrDefault(Enumerable.Empty<KeyValuePair<TObject, TObject?>>())
                .Select(p => KeyValuePair.Create(ops.ConvertTo(otherOp, p.Key), ops.ConvertTo(otherOp, p.Value)));
            return otherOp.CreateMap(entries);
        }

        /// <summary>
        /// Converts the given list <paramref name="input"/> to the list format of <paramref name="otherOp"/>.
        /// </summary>
        /// <param name="otherOp">The ops of the format to convert to.</param>
        /// <param name="input">The serialized list value to convert.</param>
        /// <typeparam name="TOther">The type the other ops serializes to and deserializes from.</typeparam>
        /// <returns>The converted list.</returns>
        public TOther ConvertList<TOther>(DynamicOps<TOther> otherOp, TObject input)
            where TOther : notnull
        {
            IEnumerable<TOther?> entries = ops.GetListValues(input)
                .GetResultOrDefault(Enumerable.Empty<TObject>())
                .Select(e => ops.ConvertTo(otherOp, e));
            return otherOp.CreateList(entries);
        }
    }

    #endregion

    #region DynamicLike.As

    extension<TObject>(DynamicLike<TObject> dynamic)
        where TObject : notnull
    {
        /// <summary>
        /// Reads this value as a stream of bytes.
        /// </summary>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the stream, or an error if this value is not a list or contains elements that are not bytes.</returns>
        public DataResult<Stream> AsStreamOpt()
        {
            return dynamic.AsNumberListOpt<TObject, byte>().Map(Stream (l) => new MemoryStream(l.ToArray(), false));
        }

        /// <summary>
        /// Reads this value as a list of numbers.
        /// </summary>
        /// <typeparam name="TNumber">The type of number to read.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the numbers, or an error if this value is not a list or contains elements that are not numbers of type <typeparamref name="TNumber"/>.</returns>
        public DataResult<IEnumerable<TNumber>> AsNumberListOpt<TNumber>()
            where TNumber : INumber<TNumber>
        {
            return dynamic.AsListValues().FlatMap(l =>
                {
                    ImmutableList<TNumber>.Builder builder = ImmutableList.CreateBuilder<TNumber>();
                    DataResult<ImmutableList<TNumber>.Builder> initResult = DataResult.CreateSuccess(builder);
                    return l.Aggregate(
                        initResult,
                        (seed, d) =>
                        {
                            DataResult<TNumber> result = d.AsNumber<TNumber>();
                            return seed.CombineStable(Functions.AddToFirst, result);
                        }
                    ).Map(IEnumerable<TNumber> (b) => b.ToImmutable());
                }
            );
        }

        /// <summary>
        /// Reads this value as a list, converting every element with <paramref name="elementDeserializer"/>.
        /// </summary>
        /// <param name="elementDeserializer">The function that converts an element.</param>
        /// <typeparam name="T">The type produced by <paramref name="elementDeserializer"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the list, or an error if this value is not a list.</returns>
        public DataResult<ImmutableList<T>> AsListOpt<T>(Func<Dynamic<TObject>, T> elementDeserializer)
        {
            return dynamic.AsListValues().Map(l => l.Select(elementDeserializer).ToImmutableList());
        }

        /// <summary>
        /// Reads this value as a map, converting every key and value with the given functions.
        /// </summary>
        /// <param name="keyDeserializer">The function that converts a key.</param>
        /// <param name="valueDeserializer">The function that converts a value.</param>
        /// <typeparam name="TKey">The type produced by <paramref name="keyDeserializer"/>.</typeparam>
        /// <typeparam name="TValue">The type produced by <paramref name="valueDeserializer"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the map, or an error if this value is not a map.</returns>
        public DataResult<ImmutableDictionary<TKey, TValue>> AsMapOpt<TKey, TValue>(Func<DynamicLike<TObject>, TKey> keyDeserializer, Func<DynamicLike<TObject>, TValue> valueDeserializer)
            where TKey : notnull
        {
            return dynamic.AsMapEntries().Map(l =>
                l.ToImmutableDictionary(pair => keyDeserializer.Apply(pair.Key), pair => valueDeserializer.Apply(pair.Value))
            );
        }
    }

    #endregion

    #region DynamicLike.Get

    extension<TObject>(DynamicLike<TObject> dynamic)
        where TObject : notnull
    {
        /// <summary>
        /// Reads the entry stored under the given string <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entry to read.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the value, or an error if this value is not a map or has no entry under <paramref name="key"/>.</returns>
        public DataResult<TObject> GetElement(string key)
        {
            return dynamic.Get(dynamic.Ops.CreateString(key));
        }

        /// <summary>
        /// Reads the entry stored under the given <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key of the entry to read.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the value, or an error if this value is not a map or has no entry under <paramref name="key"/>.</returns>
        public DataResult<TObject> GetElement(TObject key)
        {
            return dynamic.Get(key);
        }
    }

    #endregion

    #region DynamicLike.Read

    extension<TObject>(DynamicLike<TObject> dynamic)
        where TObject : notnull
    {
        /// <summary>
        /// Decodes this value with <paramref name="decoder"/>, returning only the decoded value.
        /// </summary>
        /// <param name="decoder">The decoder to use.</param>
        /// <typeparam name="TResult">The type of the decoded value.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded value, or an error if this value cannot be decoded.</returns>
        public DataResult<TResult> Read<TResult>(IDecoder<TResult> decoder)
        {
            return dynamic.Decode(decoder).Map(tuple => tuple.Item1);
        }

        /// <summary>
        /// Reads this value as a list, decoding every element with <paramref name="elementDecoder"/>.
        /// </summary>
        /// <remarks>Invalid elements will be passed.</remarks>
        /// <param name="elementDecoder">The function that decodes an element.</param>
        /// <typeparam name="TElement">The type of the decoded elements.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded elements, or an error if this value is not a list or an element cannot be decoded.</returns>
        public DataResult<ImmutableList<TElement>> ReadList<TElement>(
            Func<Dynamic<TObject>, DataResult<TElement>> elementDecoder
        )
        {
            return dynamic.AsListValues().FlatMap(l =>
                {
                    ImmutableList<TElement>.Builder builder = ImmutableList.CreateBuilder<TElement>();
                    DataResult<ImmutableList<TElement>.Builder> initResult = DataResult.CreateSuccess(builder);
                    return l.Aggregate(
                            initResult,
                            (seed, value) =>
                            {
                                DataResult<TElement> elementResult = elementDecoder.Apply(value);
                                return seed.CombineStable(Functions.AddToFirst, elementResult);
                            }
                    ).Map(_ => builder.ToImmutable());
                }
            );
        }

        /// <summary>
        /// Reads this value as a list, decoding every element with <paramref name="elementDecoder"/>.
        /// </summary>
        /// <remarks>Invalid elements will be passed.</remarks>
        /// <param name="elementDecoder">The decoder used to decode an element.</param>
        /// <typeparam name="TElement">The type of the decoded elements.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded elements, or an error if this value is not a list or an element cannot be decoded.</returns>
        public DataResult<ImmutableList<TElement>> ReadList<TElement>(IDecoder<TElement> elementDecoder)
        {
            return dynamic.AsListValues().FlatMap(l =>
                {
                    ImmutableList<TElement>.Builder builder = ImmutableList.CreateBuilder<TElement>();
                    DataResult<ImmutableList<TElement>.Builder> initResult = DataResult.CreateSuccess(builder);
                    return l.Aggregate(
                            initResult,
                            (seed, value) =>
                            {
                                DataResult<TElement> elementResult = value.Read(elementDecoder);
                                return seed.CombineStable(Functions.AddToFirst, elementResult);
                            }
                        ).Map(b => b.ToImmutable());
                }
            );
        }

        /// <summary>
        /// Reads this value as a map, converting every key and value with the given functions.
        /// </summary>
        /// <param name="keyDecoder">The function that converts a key.</param>
        /// <param name="valueDecoder">The function that converts a value.</param>
        /// <typeparam name="TKey">The type produced by <paramref name="keyDecoder"/>.</typeparam>
        /// <typeparam name="TValue">The type produced by <paramref name="valueDecoder"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the map, or an error if this value is not a map.</returns>
        public DataResult<ImmutableDictionary<TKey, TValue?>> ReadMap<TKey, TValue>(Func<DynamicLike<TObject>, TKey> keyDecoder, Func<DynamicLike<TObject>, TValue?> valueDecoder)
            where TKey : notnull
        {
            return dynamic.AsMapEntries().Map(l =>
                l.ToImmutableDictionary(pair => keyDecoder.Apply(pair.Key), pair => valueDecoder.Apply(pair.Value))
            );
        }

        /// <summary>
        /// Reads this value as a map, decoding every key and value with the given decoders.
        /// </summary>
        /// <param name="keyDecoder">The decoder used to decode a key.</param>
        /// <param name="valueDecoder">The decoder used to decode a value.</param>
        /// <typeparam name="TKey">The type of the decoded keys.</typeparam>
        /// <typeparam name="TValue">The type of the decoded values.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded entries, or an error if this value is not a map or an entry cannot be decoded.</returns>
        public DataResult<ImmutableDictionary<TKey, TValue>> ReadMap<TKey, TValue>(IDecoder<TKey> keyDecoder, IDecoder<TValue> valueDecoder)
            where TKey : notnull
        {
            return dynamic.AsMapEntries().FlatMap(l =>
                {
                    ImmutableDictionary<TKey, TValue>.Builder builder = ImmutableDictionary.CreateBuilder<TKey, TValue>();
                    DataResult<ImmutableDictionary<TKey, TValue>.Builder> initResult = DataResult.CreateSuccess(builder);
                    return l.Aggregate(
                            initResult,
                            (seed, pair) =>
                            {
                                DataResult<KeyValuePair<TKey, TValue>> entry = pair.Key
                                    .Read(keyDecoder)
                                    .Combine(KeyValuePair.Create, pair.Value.Read(valueDecoder));
                                return seed.CombineStable(Functions.AddToFirst, entry);
                            }
                        ).Map(b => b.ToImmutable());
                }
            );
        }

        /// <summary>
        /// Reads this value as a map, decoding every value with a decoder selected by its key.
        /// </summary>
        /// <param name="keyDecoder">The decoder used to decode a key.</param>
        /// <param name="valueDecoderDispatcher">The function that returns the decoder for the value of a key.</param>
        /// <typeparam name="TKey">The type of the decoded keys.</typeparam>
        /// <typeparam name="TValue">The type of the decoded values.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded entries, or an error if this value is not a map or an entry cannot be decoded.</returns>
        public DataResult<ImmutableDictionary<TKey, TValue>> ReadMap<TKey, TValue>(IDecoder<TKey> keyDecoder, Func<TKey, IDecoder<TValue>> valueDecoderDispatcher)
            where TKey : notnull
        {
            return dynamic.AsMapEntries().FlatMap(l =>
                {
                    ImmutableDictionary<TKey, TValue>.Builder builder = ImmutableDictionary.CreateBuilder<TKey, TValue>();
                    DataResult<ImmutableDictionary<TKey, TValue>.Builder> initResult = DataResult.CreateSuccess(builder);
                    return l.Aggregate(
                            initResult,
                            (seed, pair) =>
                            {
                                DataResult<KeyValuePair<TKey, TValue>> entry = pair.Key.Read(keyDecoder)
                                    .FlatMap(key =>
                                        pair.Value.Read(valueDecoderDispatcher.Apply(key))
                                            .Map(value => KeyValuePair.Create(key, value))
                                    );
                                return seed.Combine(Functions.AddToFirst, entry);
                            }
                        ).Map(b => b.ToImmutable());
                }
            );
        }

        /// <summary>
        /// Reads this value as a map, accumulating every entry into <paramref name="empty"/> with <paramref name="func"/>.
        /// </summary>
        /// <param name="empty">The result to start accumulating from.</param>
        /// <param name="func">The function that adds an entry to the accumulated result.</param>
        /// <typeparam name="TResult">The type of the accumulated result.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the accumulated result, or an error if this value is not a map or an entry cannot be accumulated.</returns>
        public DataResult<TResult> ReadMap<TResult>(DataResult<TResult> empty, Func<TResult, DynamicLike<TObject>, DynamicLike<TObject>, DataResult<TResult>> func)
        {
            return dynamic.AsMapEntries().FlatMap(l =>
                {
                    return l.Aggregate(
                        empty,
                        (seed, pair) => seed.FlatMap(s => func.Apply(s, pair.Key, pair.Value))
                    );
                }
            );
        }
    }

    #endregion

    #region DynamicLike.Unwrap

    extension<TObject>(DynamicLike<TObject> dynamic)
        where TObject : notnull
    {
        /// <summary>
        /// Reads this value as a number, falling back to <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="defaultValue">The value to return if this value is not a number.</param>
        /// <typeparam name="TNumber">The type of number to read.</typeparam>
        /// <returns>The number, or <paramref name="defaultValue"/> if this value is not a number of type <typeparamref name="TNumber"/>.</returns>
        public TNumber AsNumber<TNumber>(TNumber defaultValue)
            where TNumber : INumber<TNumber>
        {
            return dynamic.AsNumber<TNumber>().GetResultOrDefault(defaultValue);
        }

        /// <summary>
        /// Reads this value as a string, falling back to <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="defaultValue">The value to return if this value is not a string.</param>
        /// <returns>The string, or <paramref name="defaultValue"/> if this value is not a string.</returns>
        public string AsString(string defaultValue)
        {
            return dynamic.AsString().GetResultOrDefault(defaultValue);
        }

        /// <summary>
        /// Reads this value as a boolean, falling back to <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="defaultValue">The value to return if this value is not a boolean.</param>
        /// <returns>The boolean, or <paramref name="defaultValue"/> if this value is not a boolean.</returns>
        public bool AsBool(bool defaultValue)
        {
            return dynamic.AsBool().GetResultOrDefault(defaultValue);
        }

        /// <summary>
        /// Reads this value as a stream, falling back to <see cref="P:System.IO.Stream.Null"/>.
        /// </summary>
        /// <returns>The stream, or <see cref="P:System.IO.Stream.Null"/> if this value is not a list of bytes.</returns>
        public Stream AsStream()
        {
            return dynamic.AsStreamOpt().GetResultOrDefault(Stream.Null);
        }

        /// <summary>
        /// Reads this value as a list of numbers, falling back to an empty list.
        /// </summary>
        /// <typeparam name="TNumber">The type of number to read.</typeparam>
        /// <returns>The numbers, or an empty list if this value is not a list of numbers of type <typeparamref name="TNumber"/>.</returns>
        public ImmutableList<TNumber> AspNumberList<TNumber>()
            where TNumber : INumber<TNumber>
        {
            if (dynamic.AsNumberListOpt<TObject, TNumber>().TryGetResult(out IEnumerable<TNumber>? numbers))
            {
                return numbers.ToImmutableList();
            }

            return ImmutableList<TNumber>.Empty;
        }

        /// <summary>
        /// Reads this value as a list, converting every element with <paramref name="elementDecoder"/> and falling back to an empty list.
        /// </summary>
        /// <param name="elementDecoder">The function that converts an element.</param>
        /// <typeparam name="TElement">The type produced by <paramref name="elementDecoder"/>.</typeparam>
        /// <returns>The list, or an empty list if this value is not a list.</returns>
        public ImmutableList<TElement> AsList<TElement>(Func<Dynamic<TObject>, TElement> elementDecoder)
        {
            return dynamic.AsListOpt(elementDecoder).GetResultOrDefault(ImmutableList<TElement>.Empty);
        }

        /// <summary>
        /// Reads this value as a map, converting every key and value with the given functions and falling back to an empty map.
        /// </summary>
        /// <param name="keyDecoder">The function that converts a key.</param>
        /// <param name="valueDecoder">The function that converts a value.</param>
        /// <typeparam name="TKey">The type produced by <paramref name="keyDecoder"/>.</typeparam>
        /// <typeparam name="TValue">The type produced by <paramref name="valueDecoder"/>.</typeparam>
        /// <returns>The map, or an empty map if this value is not a map.</returns>
        public ImmutableDictionary<TKey, TValue> AsMap<TKey, TValue>(Func<DynamicLike<TObject>, TKey> keyDecoder, Func<DynamicLike<TObject>, TValue> valueDecoder)
            where TKey : notnull
        {
            return dynamic.AsMapOpt(keyDecoder, valueDecoder).GetResultOrDefault(ImmutableDictionary<TKey, TValue>.Empty);
        }
    }

    #endregion

    #region DynamicLike.Create

    extension<TObject>(DynamicLike<TObject> dynamic)
        where TObject : notnull
    {
        /// <summary>
        /// Creates a dynamic containing a list created from the given <paramref name="list"/>.
        /// </summary>
        /// <param name="list">The elements of the list.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created list.</returns>
        public Dynamic<TObject> CreateList(IEnumerable<Dynamic<TObject>> list)
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateList(list.Select(d => d.Value)));
        }

        /// <summary>
        /// Creates a dynamic containing an empty list.
        /// </summary>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the empty list.</returns>
        public Dynamic<TObject> CreateList()
        {
            return dynamic.CreateList(Enumerable.Empty<Dynamic<TObject>>());
        }

        /// <summary>
        /// Creates a dynamic containing a map created from the given <paramref name="entries"/>.
        /// </summary>
        /// <param name="entries">The entries of the map.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created map.</returns>
        /// <remarks>
        /// Entries whose key is empty are ignored.
        /// </remarks>
        public Dynamic<TObject> CreateMap(IEnumerable<KeyValuePair<Dynamic<TObject>, Dynamic<TObject>>> entries)
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateMap(entries.Where(pair => pair.Key.Value is not null).Select(pair => KeyValuePair.Create(pair.Key.Value!, pair.Value.Value))));
        }

        /// <summary>
        /// Creates a dynamic containing an empty map.
        /// </summary>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the empty map.</returns>
        public Dynamic<TObject> CreateMap()
        {
            return dynamic.CreateMap(Enumerable.Empty<KeyValuePair<Dynamic<TObject>, Dynamic<TObject>>>());
        }

        /// <summary>
        /// Creates a dynamic containing the given <paramref name="number"/>.
        /// </summary>
        /// <param name="number">The number to serialize.</param>
        /// <typeparam name="TNumber">The type of the number to serialize.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created number.</returns>
        public Dynamic<TObject> CreateNumber<TNumber>(TNumber number)
            where TNumber : INumber<TNumber>
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateNumber(number));
        }

        /// <summary>
        /// Creates a dynamic containing the given <paramref name="stringValue"/>.
        /// </summary>
        /// <param name="stringValue">The string to serialize.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created string.</returns>
        public Dynamic<TObject> CreateString(string stringValue)
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateString(stringValue));
        }

        /// <summary>
        /// Creates a dynamic containing the given <paramref name="boolValue"/>.
        /// </summary>
        /// <param name="boolValue">The boolean to serialize.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created boolean.</returns>
        public Dynamic<TObject> CreateBool(bool boolValue)
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateBoolValue(boolValue));
        }

        /// <summary>
        /// Creates a dynamic containing a list of numbers created from the given <paramref name="numberList"/>.
        /// </summary>
        /// <param name="numberList">The numbers to serialize.</param>
        /// <typeparam name="TNumber">The type of the numbers to serialize.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created list.</returns>
        public Dynamic<TObject> CreateNumberList<TNumber>(IEnumerable<TNumber> numberList)
            where TNumber : INumber<TNumber>
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateNumberList(numberList));
        }
    }

    #endregion
}