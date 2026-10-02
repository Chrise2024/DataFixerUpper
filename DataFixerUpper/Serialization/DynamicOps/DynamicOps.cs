using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.DynamicOps;

/// <summary>
/// An adapter for a hierarchical serialization format. Clients may use this class to interact with serialization formats such as JSON without knowing the specific serialization format being used.
/// </summary>
/// <remarks>
/// This class, along with <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicLike`1"/>, is a low-level serialization abstraction used in the implementation of <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>. The functionality offered by <see cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/> is more easily composed than the fixed class offered here.
/// </remarks>
/// <typeparam name="TObject">The type this class serializes to and deserializes from, for example <see cref="T:System.Text.Json.Nodes.JsonNode"/>.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicLike`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.DynamicOps.OptionalDynamic`1"/>
public abstract class DynamicOps<TObject>
    where TObject : notnull
{
    /// <summary>
    /// Returns the empty value of this format.
    /// </summary>
    /// <remarks>
    /// In <c>System.Text.Json</c>, <see langword="null"/> represents both empty value and absent value, in <c>Newtonsoft.Json</c>, empty value is <c>JValue.CreateNull()</c>.
    /// </remarks>
    /// <returns>The empty <typeparamref name="TObject"/>, or <see langword="null"/> if this format has no empty value type.</returns>
    public abstract TObject Empty();

    /// <summary>
    /// Gets an empty serialized map.
    /// </summary>
    /// <returns>The empty map.</returns>
    public virtual TObject EmptyMap()
    {
        return CreateMap(Enumerable.Empty<KeyValuePair<TObject, TObject>>());
    }

    /// <summary>
    /// Gets an empty serialized list.
    /// </summary>
    /// <returns>The empty list.</returns>
    public virtual TObject EmptyList()
    {
        return CreateList(Enumerable.Empty<TObject>());
    }

    /// <summary>
    /// Converts the given <paramref name="input"/> to the format of <paramref name="otherOp"/>.
    /// </summary>
    /// <param name="otherOp">The ops of the format to convert to.</param>
    /// <param name="input">The serialized value to convert.</param>
    /// <typeparam name="TOther">The type the other ops serializes to and deserializes from.</typeparam>
    /// <returns>The converted value, or the empty value of <paramref name="otherOp"/> if <paramref name="input"/> is empty.</returns>
    public abstract TOther ConvertTo<TOther>(DynamicOps<TOther> otherOp, TObject input)
        where TOther : notnull;

    /// <summary>
    /// Reads the given <paramref name="input"/> as a <see langword="byte"/>.
    /// </summary>
    /// <param name="input">The serialized value to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the <see langword="byte"/>, or an error if <paramref name="input"/> is not a number that can be represented by <see langword="byte"/>.</returns>
    public virtual DataResult<byte> GetByteValue(TObject input)
    {
        return GetLongValue(input).Map(ConvertUtil.ToByte);
    }

    /// <summary>
    /// Creates a serialized <see langword="byte"/> from the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The <see langword="byte"/> to serialize.</param>
    /// <returns>The serialized <see langword="byte"/>.</returns>
    public virtual TObject CreateByte(byte value)
    {
        return CreateLong(value);
    }

    /// <summary>
    /// Reads the given <paramref name="input"/> as a <see langword="short"/>.
    /// </summary>
    /// <param name="input">The serialized value to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the <see langword="short"/>, or an error if <paramref name="input"/> is not a number that can be represented by <see langword="short"/>.</returns>
    public virtual DataResult<short> GetShortValue(TObject input)
    {
        return GetLongValue(input).Map(ConvertUtil.ToShort);
    }


    /// <summary>
    /// Creates a serialized <see langword="short"/> from the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The <see langword="short"/> to serialize.</param>
    /// <returns>The serialized <see langword="short"/>.</returns>
    public virtual TObject CreateShort(short value)
    {
        return CreateLong(value);
    }

    /// <summary>
    /// Reads the given <paramref name="input"/> as an <see langword="int"/>.
    /// </summary>
    /// <param name="input">The serialized value to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the <see langword="int"/>, or an error if <paramref name="input"/> is not a number that can be represented by <see langword="int"/>.</returns>
    public virtual DataResult<int> GetIntValue(TObject input)
    {
        return GetLongValue(input).Map(ConvertUtil.ToInt);
    }

    /// <summary>
    /// Creates a serialized <see langword="int"/> from the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The <see langword="int"/> to serialize.</param>
    /// <returns>The serialized <see langword="int"/>.</returns>
    public virtual TObject CreateInt(int value)
    {
        return CreateLong(value);
    }

    /// <summary>
    /// Reads the given <paramref name="input"/> as a <see langword="long"/>.
    /// </summary>
    /// <param name="input">The serialized value to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the <see langword="long"/>, or an error if <paramref name="input"/> is not a number that can be represented by <see langword="long"/>.</returns>
    public abstract DataResult<long> GetLongValue(TObject input);

    /// <summary>
    /// Creates a serialized <see langword="long"/> from the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The <see langword="long"/> to serialize.</param>
    /// <returns>The serialized <see langword="long"/>.</returns>
    public abstract TObject CreateLong(long value);

    /// <summary>
    /// Reads the given <paramref name="input"/> as a <see langword="float"/>.
    /// </summary>
    /// <param name="input">The serialized value to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the <see langword="float"/>, or an error if <paramref name="input"/> is not a number that can be represented by <see langword="float"/>.</returns>
    public virtual DataResult<float> GetFloatValue(TObject input)
    {
        return GetDoubleValue(input).Map(ConvertUtil.ToFloat);
    }

    /// <summary>
    /// Creates a serialized <see langword="float"/> from the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The <see langword="float"/> to serialize.</param>
    /// <returns>The serialized <see langword="float"/>.</returns>
    public virtual TObject CreateFloat(float value)
    {
        return CreateDouble(value);
    }

    /// <summary>
    /// Reads the given <paramref name="input"/> as a <see langword="double"/>.
    /// </summary>
    /// <param name="input">The serialized value to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the <see langword="double"/>, or an error if <paramref name="input"/> is not a number that can be represented by <see langword="double"/>.</returns>
    public abstract DataResult<double> GetDoubleValue(TObject input);

    /// <summary>
    /// Creates a serialized <see langword="double"/> from the given <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The <see langword="double"/> to serialize.</param>
    /// <returns>The serialized <see langword="double"/>.</returns>
    public abstract TObject CreateDouble(double value);

    /// <summary>
    /// Reads the given <paramref name="input"/> as a stream.
    /// </summary>
    /// <param name="input">The serialized list of bytes to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the stream, or an error if <paramref name="input"/> is not a list or contains elements that are not bytes.</returns>
    public DataResult<Stream> GetStream(TObject input)
    {
        return GetList(input).FlatMap(l =>
            {
                ImmutableArray<byte>.Builder builder = ImmutableArray.CreateBuilder<byte>();
                foreach (TObject obj in l)
                {
                    DataResult<long> numberResult = GetLongValue(obj);
                    if (!numberResult.TryGetResult(out long number) || number > byte.MaxValue)
                    {
                        builder.Clear();
                        return DataResult.CreateError<Stream>($"Some elements are not bytes: {input}");
                    }

                    builder.Add(decimal.ToByte(number));
                }

                byte[] array = builder.ToArray();
                return DataResult.CreateSuccess<Stream>(new MemoryStream(array, false));
            }
        );
    }

    /// <summary>
    /// Reads the given <paramref name="string"/> as a string.
    /// </summary>
    /// <param name="string">The serialized value to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the string, or an error if <paramref name="string"/> is not a string.</returns>
    public abstract DataResult<string> GetStringValue(TObject @string);

    /// <summary>
    /// Creates a serialized string from the given <paramref name="string"/>.
    /// </summary>
    /// <param name="string">The string to serialize.</param>
    /// <returns>The serialized string.</returns>
    public abstract TObject CreateString(string @string);

    /// <summary>
    /// Reads the given <paramref name="bool"/> as a boolean.
    /// </summary>
    /// <param name="bool">The serialized value to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the boolean, or an error if <paramref name="bool"/> is not a boolean.</returns>
    public abstract DataResult<bool> GetBoolValue(TObject @bool);

    /// <summary>
    /// Creates a serialized boolean from the given <paramref name="bool"/>.
    /// </summary>
    /// <param name="bool">The boolean to serialize.</param>
    /// <returns>The serialized boolean.</returns>
    public abstract TObject CreateBoolValue(bool @bool);


    /// <summary>
    /// Creates a serialized list of bytes from the given <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The stream to read the bytes from.</param>
    /// <returns>The serialized list, or an empty list if <paramref name="stream"/> cannot be read.</returns>
    public virtual TObject CreateStream(Stream stream)
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent((int) stream.Length);
        TObject result;
        try
        {
            int n = stream.Read(buffer, 0, (int) stream.Length);
            result = CreateList(buffer.Take(n).Select(CreateByte));
        }
        catch
        {
            result = CreateList(Enumerable.Empty<TObject>());
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        return result;
    }

    /// <summary>
    /// Creates a serialized list of <see langword="int"/> from the given <paramref name="numbers"/>.
    /// </summary>
    /// <param name="numbers">The <see langword="int"/> list to serialize.</param>
    /// <returns>The serialized list.</returns>
    public virtual TObject CreateIntList(IEnumerable<int> numbers)
    {
        return CreateList(numbers.Select(CreateInt));
    }

    /// <summary>
    /// Creates a serialized list of <see langword="long"/> from the given <paramref name="numbers"/>.
    /// </summary>
    /// <param name="numbers">The <see langword="long"/> list to serialize.</param>
    /// <returns>The serialized list.</returns>
    public virtual TObject CreateLongList(IEnumerable<long> numbers)
    {
        return CreateList(numbers.Select(CreateLong));
    }

    /// <summary>
    /// Creates a serialized list from the given <paramref name="list"/>.
    /// </summary>
    /// <param name="list">The elements of the list.</param>
    /// <returns>The serialized list.</returns>
    public abstract TObject CreateList(IEnumerable<TObject> list);

    /// <summary>
    /// Creates a serialized map from the given <paramref name="entries"/>.
    /// </summary>
    /// <param name="entries">The entries of the map.</param>
    /// <returns>The serialized map.</returns>
    public abstract TObject CreateMap(IEnumerable<KeyValuePair<TObject, TObject>> entries);

    /// <summary>
    /// Creates a serialized map from entries with string keys.
    /// </summary>
    /// <param name="entries">The entries of the map.</param>
    /// <returns>The serialized map.</returns>
    /// <remarks>
    /// The keys are converted with <c>CreateString</c>.
    /// </remarks>
    public virtual TObject CreateMap(IEnumerable<KeyValuePair<string, TObject>> entries)
    {
        return CreateMap(entries.Select(pair => pair.MapKey(CreateString)));
    }

    /// <summary>
    /// Creates a new serialized primitive from the given serialized primitive with the given value added.
    /// </summary>
    /// <param name="prefix">The existing primitive value to add to.</param>
    /// <param name="value">The primitive value to merge into <paramref name="prefix"/>.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the merged primitive, or an error message if the serialized primitive is of an incorrect type.</returns>
    public virtual DataResult<TObject> MergeToPrimitive(TObject prefix, TObject value)
    {
        return IsEmpty(prefix)
            ? DataResult.CreateSuccess(value)
            : DataResult.CreateError($"Do not know how to append a primitive value {value} to {prefix}", Optional.Create(value));
    }

    /// <summary>
    /// Creates a new builder that builds a map of this format.
    /// </summary>
    /// <returns>A new <see cref="T:DataFixerUpper.Serialization.Collections.Builder.RecordBuilder`1"/> for <typeparamref name="TObject"/> values.</returns>
    public virtual RecordBuilder<TObject> CreateMapBuilder()
    {
        return new MapBuilder<TObject>(this);
    }

    /// <summary>
    /// Creates a new builder that builds a list of this format.
    /// </summary>
    /// <returns>A new <see cref="T:DataFixerUpper.Serialization.Collections.Builder.IListBuilder`1"/> for <typeparamref name="TObject"/> values.</returns>
    public virtual ListBuilderBase<TObject> CreateListBuilder()
    {
        return new ListBuilder<TObject>(this);
    }

    /// <summary>
    /// Returns a copy of <paramref name="list"/> with <paramref name="other"/> merged into it.
    /// </summary>
    /// <param name="list">The list to merge into, which may be empty.</param>
    /// <param name="other">The value to merge into <paramref name="list"/>.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the merged list, or an error if <paramref name="list"/> is not a list or cannot hold <paramref name="other"/>.</returns>
    public abstract DataResult<TObject> MergeToList(TObject list, TObject other);

    /// <summary>
    /// Returns a copy of <paramref name="list"/> with all values of <paramref name="values"/> merged into it.
    /// </summary>
    /// <param name="list">The list to merge into, which may be empty.</param>
    /// <param name="values">The values to merge into <paramref name="list"/>.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the merged list, or an error if <paramref name="list"/> is not a list or cannot hold the given values.</returns>
    /// <remarks>
    /// If <paramref name="list"/> is empty, the result is created with <c>CreateList</c>. Otherwise, the values are merged one at a time with <c>MergeToList</c>.
    /// </remarks>
    public virtual DataResult<TObject> MergeToList(TObject list, IEnumerable<TObject> values)
    {
        if (IsEmpty(list))
        {
            return DataResult.CreateSuccess(CreateList(values));
        }

        return values.Aggregate(
            DataResult.CreateSuccess(list),
            (seed, current) => seed.FlatMap(r => MergeToList(r, current))
        );
    }

    /// <summary>
    /// Returns a copy of <paramref name="map"/> with <paramref name="key"/> set to <paramref name="value"/>.
    /// </summary>
    /// <param name="map">The map to merge into, which may be empty.</param>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to set <paramref name="key"/> to.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the merged map, or an error if <paramref name="map"/> is not a map or cannot hold <paramref name="key"/>.</returns>
    public abstract DataResult<TObject> MergeToMap(TObject map, TObject key, TObject value);

    /// <summary>
    /// Returns a copy of <paramref name="map"/> with the given string <paramref name="key"/> set to <paramref name="value"/>.
    /// </summary>
    /// <param name="map">The map to merge into, which may be empty.</param>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to set <paramref name="key"/> to.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the merged map, or an error if <paramref name="map"/> is not a map.</returns>
    /// <remarks>
    /// The key is converted with <c>CreateString</c>.
    /// </remarks>
    public virtual DataResult<TObject> MergeToMap(TObject map, string key, TObject value)
    {
        ThrowIfKeyNull(key);

        return MergeToMap(map, CreateString(key), value);
    }

    /// <summary>
    /// Returns a copy of <paramref name="map"/> with all entries of <paramref name="values"/> merged into it.
    /// </summary>
    /// <param name="map">The map to merge into, which may be empty.</param>
    /// <param name="values">The entries to merge into <paramref name="map"/>.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the merged map, or an error if <paramref name="map"/> is not a map.</returns>
    /// <remarks>
    /// If <paramref name="map"/> is empty, the result is created with <c>CreateMap</c>. Otherwise, the entries are merged one at a time with <c>MergeToMap</c>.
    /// </remarks>
    public virtual DataResult<TObject> MergeToMap(TObject map, IEnumerable<KeyValuePair<string, TObject>> values)
    {
        if (IsEmpty(map))
        {
            return DataResult.CreateSuccess(CreateMap(values));
        }

        return values.Aggregate(
            DataResult.CreateSuccess(map),
            (seed, pair) => seed.FlatMap(r => MergeToMap(r, pair.Key, pair.Value))
        );
    }

    /// <summary>
    /// Returns a copy of <paramref name="map"/> with all entries of <paramref name="values"/> merged into it.
    /// </summary>
    /// <param name="map">The map to merge into, which may be empty.</param>
    /// <param name="values">The entries to merge into <paramref name="map"/>.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the merged map, or an error if <paramref name="map"/> is not a map or cannot hold the given keys.</returns>
    /// <remarks>
    /// If <paramref name="map"/> is empty, the result is created with <c>CreateMap</c>. Otherwise, the entries are merged one at a time with <c>MergeToMap</c>.
    /// </remarks>
    public virtual DataResult<TObject> MergeToMap(TObject map, IEnumerable<KeyValuePair<TObject, TObject>> values)
    {
        if (IsEmpty(map))
        {
            return DataResult.CreateSuccess(CreateMap(values));
        }

        return values.Aggregate(
            DataResult.CreateSuccess(map),
            (seed, pair) => seed.FlatMap(r => MergeToMap(r, pair.Key, pair.Value))
        );
    }

    /// <summary>
    /// Extracts a consumer from the given value that iterates over the elements of the serialized list. The returned value logically encapsulates an iteration over the elements in the given list, performing some user-specified action on each element.
    /// </summary>
    /// <param name="input">The serialized list to read.</param>
    /// <returns>An iteration over the elements in the input, which may perform some user-specified action on each element.</returns>
    public virtual DataResult<Consumer<Consumer<TObject>>> GetListValues(TObject input)
    {
        return GetList(input).Map(MakeForeach);

        static Consumer<Consumer<TObject>> MakeForeach(IEnumerable<TObject> enumerable)
        {
            return func =>
            {
                foreach (TObject obj in enumerable)
                {
                    func.Accept(obj);
                }
            };
        }
    }

    /// <summary>
    /// Reads the elements of the given <paramref name="input"/> list.
    /// </summary>
    /// <param name="input">The serialized list to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the elements of the list, or an error if <paramref name="input"/> is not a list.</returns>
    public abstract DataResult<IEnumerable<TObject>> GetList(TObject input);

    /// <summary>
    /// Reads the given <paramref name="input"/> as an indexed map.
    /// </summary>
    /// <param name="input">The serialized map to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the map, or an error if <paramref name="input"/> is not a map or its entries cannot be indexed.</returns>
    /// <remarks>
    /// The default implementation builds the map from the entries returned by <c>GetMapValues</c>.
    /// </remarks>
    public virtual DataResult<MapLike<TObject>> GetMap(TObject input)
    {
        return GetMapValues(input).FlatMap(pairs =>
            {
                try
                {
                    return DataResult.CreateSuccess(
                        MapLike<TObject>.ForMap(
                            pairs.ToDictionary(
                                p => p.Key,
                                p => p.Value
                            ), this
                        )
                    );
                }
                catch (Exception e)
                {
                    return DataResult.CreateError<MapLike<TObject>>($"Error while building map: {e.Message}");
                }
            }
        );
    }

    /// <summary>
    /// Reads the entries of the given <paramref name="input"/> map.
    /// </summary>
    /// <param name="input">The serialized map to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the entries of the map, or an error if <paramref name="input"/> is not a map.</returns>
    public abstract DataResult<IEnumerable<KeyValuePair<TObject, TObject>>> GetMapValues(TObject input);


    /// <summary>
    /// Extracts a Consumer from the given value that iterates over the entries of the serialized map. The returned value logically encapsulates a iteration over the entries in the given map, performing some user-specified action on each entry.
    /// </summary>
    /// <param name="input">The serialized map to read.</param>
    /// <returns>An iteration over the entries in the input, which may perform some user-specified action on each entry.</returns>
    public virtual DataResult<Consumer<BiConsumer<TObject, TObject>>> GetMapEntries(TObject input)
    {
        return GetMapValues(input).Map(MakeForeach);

        static Consumer<BiConsumer<TObject, TObject>> MakeForeach(IEnumerable<KeyValuePair<TObject, TObject>> keyValuePairs)
        {
            return func =>
            {
                foreach (KeyValuePair<TObject, TObject> pair in keyValuePairs)
                {
                    func.Accept(pair.Key, pair.Value);
                }
            };
        }
    }

    /// <summary>
    /// Reads the given <paramref name="input"/> as a list of <see langword="int"/>.
    /// </summary>
    /// <param name="input">The serialized list to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the <see langword="int"/>, or an error if <paramref name="input"/> is not a list or contains elements that are not numbers.</returns>
    public virtual DataResult<IEnumerable<int>> GetIntList(TObject input)
    {
        return GetList(input).FlatMap(l =>
            {
                ImmutableList<int>.Builder builder = ImmutableList.CreateBuilder<int>();
                DataResult<ImmutableList<int>.Builder> initResult = DataResult.CreateSuccess(builder);
                return l.Aggregate(
                    initResult,
                    (seed, obj) => seed.CombineStable(Functions.AddToFirst, GetIntValue(obj))
                ).Map(IEnumerable<int> (b) => b.ToImmutable());
            }
        );
    }

    /// <summary>
    /// Reads the given <paramref name="input"/> as a list of <see langword="long"/>.
    /// </summary>
    /// <param name="input">The serialized list to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the <see langword="long"/>, or an error if <paramref name="input"/> is not a list or contains elements that are not numbers.</returns>
    public virtual DataResult<IEnumerable<long>> GetLongList(TObject input)
    {
        return GetList(input).FlatMap(l =>
            {
                ImmutableList<long>.Builder builder = ImmutableList.CreateBuilder<long>();
                DataResult<ImmutableList<long>.Builder> initResult = DataResult.CreateSuccess(builder);
                return l.Aggregate(
                    initResult,
                    (seed, obj) => seed.CombineStable(Functions.AddToFirst, GetLongValue(obj))
                ).Map(IEnumerable<long> (b) => b.ToImmutable());
            }
        );
    }

    /// <summary>
    /// Reads the value stored under <paramref name="key"/> in <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The serialized map to read from.</param>
    /// <param name="key">The key of the value to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the value, or an error if <paramref name="input"/> is not a map or does not contain <paramref name="key"/>.</returns>
    public abstract DataResult<TObject> Get(TObject input, TObject key);

    /// <summary>
    /// Reads the value stored under the given string <paramref name="key"/> in <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The serialized map to read from.</param>
    /// <param name="key">The key of the value to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the value, or an error if <paramref name="input"/> is not a map or does not contain <paramref name="key"/>.</returns>
    /// <remarks>
    /// The key is converted with <c>CreateString</c>.
    /// </remarks>
    public virtual DataResult<TObject> Get(TObject input, string key)
    {
        ThrowIfKeyNull(key);

        return Get(input, CreateString(key));
    }

    /// <summary>
    /// Returns a copy of <paramref name="input"/> with <paramref name="key"/> set to <paramref name="value"/>.
    /// </summary>
    /// <param name="input">The serialized map to modify.</param>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to set <paramref name="key"/> to.</param>
    /// <returns>The modified map, or <paramref name="input"/> itself if it is empty or the change cannot be applied.</returns>
    /// <remarks>
    /// The change is applied with <c>MergeToMap</c>.
    /// </remarks>
    public virtual TObject Set(TObject input, TObject key, TObject value)
    {
        ThrowIfKeyNull(key);

        if (IsEmpty(input))
        {
            return input;
        }

        return MergeToMap(input, key, value).GetResultOrDefault(input);
    }

    /// <summary>
    /// Returns a copy of <paramref name="input"/> with the given string <paramref name="key"/> set to <paramref name="value"/>.
    /// </summary>
    /// <param name="input">The serialized map to modify.</param>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to set <paramref name="key"/> to.</param>
    /// <returns>The modified map, or <paramref name="input"/> itself if it is empty or the change cannot be applied.</returns>
    /// <remarks>
    /// The key is converted with <c>CreateString</c>, and the change is applied with <c>MergeToMap</c>.
    /// </remarks>
    public virtual TObject Set(TObject input, string key, TObject value)
    {
        ThrowIfKeyNull(key);

        if (IsEmpty(input))
        {
            return input;
        }

        return MergeToMap(input, CreateString(key), value).GetResultOrDefault(input);
    }

    /// <summary>
    /// Returns a copy of <paramref name="input"/> with the value stored under <paramref name="key"/> replaced by the result of <paramref name="updater"/>.
    /// </summary>
    /// <param name="input">The serialized map to modify.</param>
    /// <param name="key">The key of the value to replace.</param>
    /// <param name="updater">The function that produces the new value from the current one.</param>
    /// <returns>The modified map, or <paramref name="input"/> itself if it is empty or does not contain <paramref name="key"/>.</returns>
    public virtual TObject Update(TObject input, TObject key, Func<TObject, TObject> updater)
    {
        ThrowIfKeyNull(key);

        if (IsEmpty(input))
        {
            return input;
        }

        DataResult<TObject> result = Get(input, key);
        if (!result.TryGetResult(out TObject kr))
        {
            return input;
        }

        return MergeToMap(input, key, updater.Apply(kr)).GetResultOrDefault(input);
    }

    /// <summary>
    /// Returns a copy of <paramref name="input"/> with the value stored under the given string <paramref name="key"/> replaced by the result of <paramref name="updater"/>.
    /// </summary>
    /// <param name="input">The serialized map to modify.</param>
    /// <param name="key">The key of the value to replace.</param>
    /// <param name="updater">The function that produces the new value from the current one.</param>
    /// <returns>The modified map, or <paramref name="input"/> itself if it is empty or does not contain <paramref name="key"/>.</returns>
    /// <remarks>
    /// The key is converted with <c>CreateString</c>.
    /// </remarks>
    public virtual TObject Update(TObject input, string key, Func<TObject, TObject> updater)
    {
        ThrowIfKeyNull(key);

        return Update(input, CreateString(key), updater);
    }

    /// <summary>
    /// Returns a copy of <paramref name="input"/> without the value stored under <paramref name="key"/>.
    /// </summary>
    /// <param name="input">The serialized map to modify.</param>
    /// <param name="key">The key of the value to remove.</param>
    /// <returns>The modified map, or <paramref name="input"/> itself if it is empty or does not contain <paramref name="key"/>.</returns>
    public abstract TObject Remove(TObject input, TObject key);

    /// <summary>
    /// Returns a copy of <paramref name="input"/> without the value stored under the given string <paramref name="key"/>.
    /// </summary>
    /// <param name="input">The serialized map to modify.</param>
    /// <param name="key">The key of the value to remove.</param>
    /// <returns>The modified map, or <paramref name="input"/> itself if it is empty or does not contain <paramref name="key"/>.</returns>
    /// <remarks>
    /// The key is converted with <c>CreateString</c>.
    /// </remarks>
    public virtual TObject Remove(TObject input, string key)
    {
        ThrowIfKeyNull(key);

        return Remove(input, CreateString(key));
    }

    /// <summary>
    /// Returns a copy of <paramref name="source"/> that can be modified without affecting <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The serialized value to copy.</param>
    /// <returns>A copy of <paramref name="source"/>.</returns>
    public abstract TObject Copy(TObject source);

    /// <summary>
    /// Gets a value indicating whether the given <paramref name="input"/> is the empty value of this format.
    /// </summary>
    /// <param name="input">The serialized value to test.</param>
    /// <returns>A <see langword="true"/> if <paramref name="input"/> is empty; otherwise, <see langword="false"/>. When this method returns <see langword="false"/>, <paramref name="input"/> is not <see langword="null"/>.</returns>
    public abstract bool IsEmpty(TObject input);

    /// <summary>
    /// Whether the caller should serialize maps using a compressed representation.
    /// </summary>
    /// <remarks>
    /// The default implementation returns <see langword="false"/>. Implementations should override the default and return <see langword="true"/>e if callers would gain space savings by compressing maps before serializing them.
    /// </remarks>
    /// <seealso cref="T:DataFixerUpper.Serialization.Collections.KeyCompressor`1"/>
    /// <returns>A <see langword="true"/> if serialize maps using a compressed representation or not.</returns>
    public virtual bool CompressMaps()
    {
        return false;
    }

    /// <summary>
    /// Helper to throw <see cref="T:System.ArgumentNullException"/> if key is null.
    /// </summary>
    protected static void ThrowIfKeyNull(string key)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key), "Key cannot be null");
        }
    }

    /// <summary>
    /// Helper to throw <see cref="T:System.ArgumentNullException"/> if key is null.
    /// </summary>
    protected static void ThrowIfKeyNull(TObject key)
    {
        if (key is null)
        {
            throw new ArgumentNullException(nameof(key), "Key cannot be null");
        }
    }
}