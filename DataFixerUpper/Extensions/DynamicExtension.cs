using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using DataFixerUpper.Datafixers.Kinds;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Codecs;

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
            return ops.CreateMap(Enumerable.Empty<KeyValuePair<TObject, TObject>>());
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
        public Func<TObject, DataResult<(T, TObject)>> WithDecoder<T>(IDecoder<T> decoder)
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
        /// Reads the given <paramref name="input"/> as a <see langword="decimal"/>, falling back to <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="input">The serialized value to read.</param>
        /// <param name="defaultValue">The value to return if <paramref name="input"/> is not a number.</param>
        /// <returns>The <see langword="decimal"/>, or <paramref name="defaultValue"/> if <paramref name="input"/> is not a number.</returns>
        public double GetNumberValue(TObject input, double defaultValue)
        {
            return ops.GetDoubleValue(input).GetResultOrDefault(defaultValue);
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
            IEnumerable<KeyValuePair<TOther, TOther>> entries = ops.GetMapValues(input)
                .GetResultOrDefault(Enumerable.Empty<KeyValuePair<TObject, TObject>>())
                .Select(p => new KeyValuePair<TOther, TOther>(ops.ConvertTo(otherOp, p.Key), ops.ConvertTo(otherOp, p.Value)));
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
            IEnumerable<TOther> entries = ops.GetList(input)
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
        /// Reads this value as a list, converting every element with <paramref name="elementDeserializer"/>.
        /// </summary>
        /// <param name="elementDeserializer">The function that converts an element.</param>
        /// <typeparam name="T">The type produced by <paramref name="elementDeserializer"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the list, or an error if this value is not a list.</returns>
        public DataResult<IEnumerable<T>> AsListOpt<T>(Func<Dynamic<TObject>, T> elementDeserializer)
        {
            return dynamic.AsListOpt().Map(l => l.Select(elementDeserializer));
        }

        /// <summary>
        /// Reads this value as a map, converting every key and value with the given functions.
        /// </summary>
        /// <param name="keyDeserializer">The function that converts a key.</param>
        /// <param name="valueDeserializer">The function that converts a value.</param>
        /// <typeparam name="TKey">The type produced by <paramref name="keyDeserializer"/>.</typeparam>
        /// <typeparam name="TValue">The type produced by <paramref name="valueDeserializer"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the map, or an error if this value is not a map.</returns>
        public DataResult<IEnumerable<KeyValuePair<TKey, TValue>>> AsMapOpt<TKey, TValue>(Func<DynamicLike<TObject>, TKey> keyDeserializer, Func<DynamicLike<TObject>, TValue> valueDeserializer)
            where TKey : notnull
        {
            return dynamic.AsMapOpt().Map(l =>
                l.Select(pair => new KeyValuePair<TKey, TValue>(keyDeserializer.Apply(pair.Key), valueDeserializer.Apply(pair.Value)))
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
        public DataResult<IList<TElement>> ReadList<TElement>(
            Func<Dynamic<TObject>, DataResult<TElement>> elementDecoder
        )
        {
            return dynamic.AsListOpt()
                .Map(l => l.Select(elementDecoder).Select(IApp<DataResult.Mu, TElement> (d) => d).ToImmutableList())
                .FlatMap(l => DataResult.Unbox(ListBox.Flip(DataResultOperator.Instance, l)));
        }

        /// <summary>
        /// Reads this value as a list, decoding every element with <paramref name="elementDecoder"/>.
        /// </summary>
        /// <remarks>Invalid elements will be passed.</remarks>
        /// <param name="elementDecoder">The decoder used to decode an element.</param>
        /// <typeparam name="TElement">The type of the decoded elements.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded elements, or an error if this value is not a list or an element cannot be decoded.</returns>
        public DataResult<IList<TElement>> ReadList<TElement>(IDecoder<TElement> elementDecoder)
        {
            return dynamic.AsListOpt()
                .Map(l => l.Select(IApp<DataResult.Mu, TElement> (e) => e.Read(elementDecoder)).ToImmutableList())
                .FlatMap(l => DataResult.Unbox(ListBox.Flip(DataResultOperator.Instance, l)));
        }

        /// <summary>
        /// Reads this value as a map, converting every key and value with the given functions.
        /// </summary>
        /// <param name="keyDecoder">The function that converts a key.</param>
        /// <param name="valueDecoder">The function that converts a value.</param>
        /// <typeparam name="TKey">The type produced by <paramref name="keyDecoder"/>.</typeparam>
        /// <typeparam name="TValue">The type produced by <paramref name="valueDecoder"/>.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the map, or an error if this value is not a map.</returns>
        public DataResult<IList<KeyValuePair<TKey, TValue>>> ReadMap<TKey, TValue>(Func<DynamicLike<TObject>, DataResult<TKey>> keyDecoder, Func<DynamicLike<TObject>, DataResult<TValue>> valueDecoder)
            where TKey : notnull
        {
            return dynamic.AsMapOpt()
                .Map(l => l.Select(IApp<DataResult.Mu, KeyValuePair<TKey, TValue>> (p) => keyDecoder.Apply(p.Key).FlatMap(k => valueDecoder.Apply(p.Value).Map(v => new KeyValuePair<TKey, TValue>(k, v)))).ToImmutableList())
                .FlatMap(l => DataResult.Unbox(ListBox.Flip(DataResultOperator.Instance, l)));
        }

        /// <summary>
        /// Reads this value as a map, decoding every key and value with the given decoders.
        /// </summary>
        /// <param name="keyDecoder">The decoder used to decode a key.</param>
        /// <param name="valueDecoder">The decoder used to decode a value.</param>
        /// <typeparam name="TKey">The type of the decoded keys.</typeparam>
        /// <typeparam name="TValue">The type of the decoded values.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded entries, or an error if this value is not a map or an entry cannot be decoded.</returns>
        public DataResult<IList<KeyValuePair<TKey, TValue>>> ReadMap<TKey, TValue>(IDecoder<TKey> keyDecoder, IDecoder<TValue> valueDecoder)
            where TKey : notnull
        {
            return dynamic.AsMapOpt()
                .Map(l => l.Select(IApp<DataResult.Mu, KeyValuePair<TKey, TValue>> (p) => p.Key.Read(keyDecoder).FlatMap(k => p.Value.Read(valueDecoder).Map(v => new KeyValuePair<TKey, TValue>(k, v)))).ToImmutableList())
                .FlatMap(l => DataResult.Unbox(ListBox.Flip(DataResultOperator.Instance, l)));
        }

        /// <summary>
        /// Reads this value as a map, decoding every value with a decoder selected by its key.
        /// </summary>
        /// <param name="keyDecoder">The decoder used to decode a key.</param>
        /// <param name="valueDecoderDispatcher">The function that returns the decoder for the value of a key.</param>
        /// <typeparam name="TKey">The type of the decoded keys.</typeparam>
        /// <typeparam name="TValue">The type of the decoded values.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded entries, or an error if this value is not a map or an entry cannot be decoded.</returns>
        public DataResult<IList<KeyValuePair<TKey, TValue>>> ReadMap<TKey, TValue>(IDecoder<TKey> keyDecoder, Func<TKey, IDecoder<TValue>> valueDecoderDispatcher)
            where TKey : notnull
        {
            return dynamic.AsMapOpt()
                .Map(l => l.Select(IApp<DataResult.Mu, KeyValuePair<TKey, TValue>> (p) => p.Key.Read(keyDecoder).FlatMap(k => p.Value.Read(valueDecoderDispatcher.Apply(k)).Map(v => new KeyValuePair<TKey, TValue>(k, v)))).ToImmutableList())
                .FlatMap(l => DataResult.Unbox(ListBox.Flip(DataResultOperator.Instance, l)));
        }

        /// <summary>
        /// Reads this value as a map, accumulating every entry into <paramref name="empty"/> with <paramref name="combiner"/>.
        /// </summary>
        /// <param name="empty">The result to start accumulating from.</param>
        /// <param name="combiner">The function that adds an entry to the accumulated result.</param>
        /// <typeparam name="TResult">The type of the accumulated result.</typeparam>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the accumulated result, or an error if this value is not a map or an entry cannot be accumulated.</returns>
        public DataResult<TResult> ReadMap<TResult>(DataResult<TResult> empty, Func<TResult, DynamicLike<TObject>, DynamicLike<TObject>, DataResult<TResult>> combiner)
        {
            // sync & ordered
            return dynamic.AsMapOpt().FlatMap(l =>
                {
                    return l.Aggregate(
                        empty,
                        (seed, pair) => seed.FlatMap(s => combiner.Apply(s, pair.Key, pair.Value))
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
        /// <returns>The number, or <paramref name="defaultValue"/> if this value is not a number.</returns>
        public double AsNumber(double defaultValue)
        {
            return dynamic.AsNumber().GetResultOrDefault(defaultValue);
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
        /// Reads this value as a list of <see langword="int"/>, falling back to an empty list.
        /// </summary>
        /// <returns>The numbers, or an empty list if this value is not a list of numbers.</returns>
        public IEnumerable<int> AsIntList()
        {
            return dynamic.AsIntListOpt().GetResultOrDefault(Enumerable.Empty<int>());
        }

        /// <summary>
        /// Reads this value as a list of <see langword="long"/>, falling back to an empty list.
        /// </summary>
        /// <returns>The numbers, or an empty list if this value is not a list of numbers.</returns>
        public IEnumerable<long> AsLongList()
        {
            return dynamic.AsLongListOpt().GetResultOrDefault(Enumerable.Empty<long>());
        }

        /// <summary>
        /// Reads this value as a list and falling back to an empty list.
        /// </summary>
        /// <returns>The list, or an empty list if this value is not a list.</returns>
        public IEnumerable<Dynamic<TObject>> AsList()
        {
            return dynamic.AsListOpt().GetResultOrDefault(Enumerable.Empty<Dynamic<TObject>>());
        }

        /// <summary>
        /// Reads this value as a list, converting every element with <paramref name="elementDecoder"/> and falling back to an empty list.
        /// </summary>
        /// <param name="elementDecoder">The function that converts an element.</param>
        /// <typeparam name="TElement">The type produced by <paramref name="elementDecoder"/>.</typeparam>
        /// <returns>The list, or an empty list if this value is not a list.</returns>
        public IEnumerable<TElement> AsList<TElement>(Func<Dynamic<TObject>, TElement> elementDecoder)
        {
            return dynamic.AsListOpt(elementDecoder).GetResultOrDefault(Enumerable.Empty<TElement>());
        }

        /// <summary>
        /// Reads this value as a map and value with the given functions and falling back to an empty map.
        /// </summary>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the entries of the map, or an error if this value is not a map.</returns>
        public IEnumerable<KeyValuePair<Dynamic<TObject>, Dynamic<TObject>>> AsMap()
        {
            return dynamic.AsMapOpt().GetResultOrDefault(Enumerable.Empty<KeyValuePair<Dynamic<TObject>, Dynamic<TObject>>>());
        }

        /// <summary>
        /// Reads this value as a map, converting every key and value with the given functions and falling back to an empty map.
        /// </summary>
        /// <param name="keyDecoder">The function that converts a key.</param>
        /// <param name="valueDecoder">The function that converts a value.</param>
        /// <typeparam name="TKey">The type produced by <paramref name="keyDecoder"/>.</typeparam>
        /// <typeparam name="TValue">The type produced by <paramref name="valueDecoder"/>.</typeparam>
        /// <returns>The map, or an empty map if this value is not a map.</returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> AsMap<TKey, TValue>(Func<DynamicLike<TObject>, TKey> keyDecoder, Func<DynamicLike<TObject>, TValue> valueDecoder)
            where TKey : notnull
        {
            return dynamic.AsMapOpt(keyDecoder, valueDecoder).GetResultOrDefault(Enumerable.Empty<KeyValuePair<TKey, TValue>>());
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
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateMap(entries.Where(pair => pair.Key.Value is not null).Select(pair => new KeyValuePair<TObject, TObject>(pair.Key.Value!, pair.Value.Value))));
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
        /// <param name="number">The <see langword="byte"/> to serialize.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created number.</returns>
        public Dynamic<TObject> CreateByte(byte number)
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateByte(number));
        }

        /// <summary>
        /// Creates a dynamic containing the given <paramref name="number"/>.
        /// </summary>
        /// <param name="number">The <see langword="short"/> to serialize.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created number.</returns>
        public Dynamic<TObject> CreateShort(short number)
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateShort(number));
        }

        /// <summary>
        /// Creates a dynamic containing the given <paramref name="number"/>.
        /// </summary>
        /// <param name="number">The <see langword="int"/> to serialize.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created number.</returns>
        public Dynamic<TObject> CreateInt(int number)
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateInt(number));
        }

        /// <summary>
        /// Creates a dynamic containing the given <paramref name="number"/>.
        /// </summary>
        /// <param name="number">The <see langword="long"/> to serialize.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created number.</returns>
        public Dynamic<TObject> CreateLong(long number)
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateLong(number));
        }

        /// <summary>
        /// Creates a dynamic containing the given <paramref name="number"/>.
        /// </summary>
        /// <param name="number">The <see langword="float"/> to serialize.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created number.</returns>
        public Dynamic<TObject> CreateFloat(float number)
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateFloat(number));
        }

        /// <summary>
        /// Creates a dynamic containing the given <paramref name="number"/>.
        /// </summary>
        /// <param name="number">The <see langword="double"/> to serialize.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created number.</returns>
        public Dynamic<TObject> CreateDouble(double number)
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateDouble(number));
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
        /// Creates a dynamic containing a list of <see langword="byte"/> created from the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The stream to read the bytes from.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created stream.</returns>
        public Dynamic<TObject> CreateStream(Stream stream)
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateStream(stream));
        }

        /// <summary>
        /// Creates a dynamic containing a list of <see langword="int"/> created from the given <paramref name="numberList"/>.
        /// </summary>
        /// <param name="numberList">The numbers to serialize.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created list.</returns>
        public Dynamic<TObject> CreateIntList(IEnumerable<int> numberList)
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateIntList(numberList));
        }

        /// <summary>
        /// Creates a dynamic containing a list of <see langword="long"/> created from the given <paramref name="numberList"/>.
        /// </summary>
        /// <param name="numberList">The numbers to serialize.</param>
        /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the created list.</returns>
        public Dynamic<TObject> CreateLongList(IEnumerable<long> numberList)
        {
            return new Dynamic<TObject>(dynamic.Ops, dynamic.Ops.CreateLongList(numberList));
        }
    }

    #endregion
}