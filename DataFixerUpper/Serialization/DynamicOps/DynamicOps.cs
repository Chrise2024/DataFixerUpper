using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
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
    public abstract TObject? Empty();
    
    /// <summary>
    /// Gets an empty serialized map.
    /// </summary>
    /// <returns>The empty map.</returns>
    public virtual TObject EmptyMap()
    {
        return CreateMap(Enumerable.Empty<KeyValuePair<TObject, TObject?>>());
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
    [return: NotNullIfNotNull(nameof(input))] 
    public abstract TOther? ConvertTo<TOther>(DynamicOps<TOther> otherOp, TObject? input)
        where TOther : notnull;

    /// <summary>
    /// Reads the given <paramref name="input"/> as a number.
    /// </summary>
    /// <param name="input">The serialized value to read.</param>
    /// <typeparam name="TNumber">The type of number to read.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the number, or an error if <paramref name="input"/> is not a number that can be represented by <typeparamref name="TNumber"/>.</returns>
    public abstract DataResult<TNumber> GetNumberValue<TNumber>(TObject? input)
        where TNumber : INumber<TNumber>;

    /// <summary>
    /// Creates a serialized number from the given <paramref name="number"/>.
    /// </summary>
    /// <param name="number">The number to serialize.</param>
    /// <typeparam name="TNumber">The type of the number to serialize.</typeparam>
    /// <returns>The serialized number.</returns>
    public abstract TObject CreateNumber<TNumber>(TNumber number)
        where TNumber : INumber<TNumber>;

    /// <summary>
    /// Reads the given <paramref name="string"/> as a string.
    /// </summary>
    /// <param name="string">The serialized value to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the string, or an error if <paramref name="string"/> is not a string.</returns>
    public abstract DataResult<string> GetStringValue(TObject? @string);

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
    public abstract DataResult<bool> GetBoolValue(TObject? @bool);

    /// <summary>
    /// Creates a serialized boolean from the given <paramref name="bool"/>.
    /// </summary>
    /// <param name="bool">The boolean to serialize.</param>
    /// <returns>The serialized boolean.</returns>
    public abstract TObject CreateBoolValue(bool @bool);

    /// <summary>
    /// Creates a serialized list from the given <paramref name="list"/>.
    /// </summary>
    /// <param name="list">The elements of the list.</param>
    /// <returns>The serialized list.</returns>
    public abstract TObject CreateList(IEnumerable<TObject?> list);

    /// <summary>
    /// Creates a serialized map from the given <paramref name="entries"/>.
    /// </summary>
    /// <param name="entries">The entries of the map.</param>
    /// <returns>The serialized map.</returns>
    public abstract TObject CreateMap(IEnumerable<KeyValuePair<TObject, TObject?>> entries);

    /// <summary>
    /// Creates a serialized map from entries with string keys.
    /// </summary>
    /// <param name="entries">The entries of the map.</param>
    /// <returns>The serialized map.</returns>
    /// <remarks>
    /// The keys are converted with <c>CreateString</c>.
    /// </remarks>
    public virtual TObject CreateMap(IEnumerable<KeyValuePair<string, TObject?>> entries)
    {
        return CreateMap(entries.Select(pair => pair.MapKey(CreateString)));
    }

    /// <summary>
    /// Creates a new serialized primitive from the given serialized primitive with the given value added.
    /// </summary>
    /// <param name="prefix">The existing primitive value to add to.</param>
    /// <param name="value">The primitive value to merge into <paramref name="prefix"/>.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the merged primitive, or an error message if the serialized primitive is of an incorrect type.</returns>
    public virtual DataResult<TObject> MergeToPrimitive(TObject? prefix, TObject value)
    {
        return IsEmpty(prefix)
            ? DataResult.CreateSuccess(value)
            : DataResult.CreateError($"Do not know how to append a primitive value {value} to {prefix}", Optional.Create(value));
    }

    /// <summary>
    /// Creates a new builder that builds a map of this format.
    /// </summary>
    /// <returns>A new <see cref="T:DataFixerUpper.Serialization.Collections.Builder.IRecordBuilder`1"/> for <typeparamref name="TObject"/> values.</returns>
    public virtual IRecordBuilder<TObject> CreateMapBuilder()
    {
        return new MapBuilder<TObject>(this);
    }

    /// <summary>
    /// Creates a new builder that builds a list of this format.
    /// </summary>
    /// <returns>A new <see cref="T:DataFixerUpper.Serialization.Collections.Builder.IListBuilder`1"/> for <typeparamref name="TObject"/> values.</returns>
    public virtual IListBuilder<TObject> CreateListBuilder()
    {
        return new ListBuilder<TObject>(this);
    }

    /// <summary>
    /// Returns a copy of <paramref name="list"/> with <paramref name="other"/> merged into it.
    /// </summary>
    /// <param name="list">The list to merge into, which may be empty.</param>
    /// <param name="other">The value to merge into <paramref name="list"/>.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the merged list, or an error if <paramref name="list"/> is not a list or cannot hold <paramref name="other"/>.</returns>
    public abstract DataResult<TObject> MergeToList(TObject? list, TObject? other);

    /// <summary>
    /// Returns a copy of <paramref name="list"/> with all values of <paramref name="values"/> merged into it.
    /// </summary>
    /// <param name="list">The list to merge into, which may be empty.</param>
    /// <param name="values">The values to merge into <paramref name="list"/>.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the merged list, or an error if <paramref name="list"/> is not a list or cannot hold the given values.</returns>
    /// <remarks>
    /// If <paramref name="list"/> is empty, the result is created with <c>CreateList</c>. Otherwise, the values are merged one at a time with <c>MergeToList</c>.
    /// </remarks>
    public virtual DataResult<TObject> MergeToList(TObject? list, IEnumerable<TObject?> values)
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
    public abstract DataResult<TObject> MergeToMap(TObject? map, TObject key, TObject? value);

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
    public virtual DataResult<TObject> MergeToMap(TObject? map, string key, TObject? value)
    {
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
    public virtual DataResult<TObject> MergeToMap(TObject? map, IEnumerable<KeyValuePair<string, TObject?>> values)
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
    public virtual DataResult<TObject> MergeToMap(TObject? map, IEnumerable<KeyValuePair<TObject, TObject?>> values)
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
    /// Reads the entries of the given <paramref name="input"/> map.
    /// </summary>
    /// <param name="input">The serialized map to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the entries of the map, or an error if <paramref name="input"/> is not a map.</returns>
    public abstract DataResult<IEnumerable<KeyValuePair<TObject, TObject?>>> GetMapValues(TObject? input);

    /// <summary>
    /// Reads the elements of the given <paramref name="input"/> list.
    /// </summary>
    /// <param name="input">The serialized list to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the elements of the list, or an error if <paramref name="input"/> is not a list.</returns>
    public abstract DataResult<IEnumerable<TObject?>> GetListValues(TObject? input);

    /// <summary>
    /// Reads the given <paramref name="input"/> as an indexed map.
    /// </summary>
    /// <param name="input">The serialized map to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the map, or an error if <paramref name="input"/> is not a map or its entries cannot be indexed.</returns>
    /// <remarks>
    /// The default implementation builds the map from the entries returned by <c>GetMapValues</c>.
    /// </remarks>
    public virtual DataResult<IMapLike<TObject>> GetMap(TObject? input)
    {
        return GetMapValues(input).FlatMap(pairs =>
            {
                try
                {
                    return DataResult.CreateSuccess(IMapLike<TObject>.ForMap(pairs.ToDictionary(), this));
                }
                catch (Exception e)
                {
                    return DataResult.CreateError<IMapLike<TObject>>($"Error while building map: {e.Message}");
                }
            }
        );
    }

    /// <summary>
    /// Reads the given <paramref name="input"/> as an immutable list.
    /// </summary>
    /// <param name="input">The serialized list to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the elements of the list, or an error if <paramref name="input"/> is not a list.</returns>
    /// <remarks>
    /// The default implementation builds the list from the elements returned by <c>GetListValues</c>.
    /// </remarks>
    public virtual DataResult<ImmutableList<TObject?>> GetList(TObject? input)
    {
        return GetListValues(input).FlatMap(pairs =>
            {
                try
                {
                    return DataResult.CreateSuccess(pairs.ToImmutableList());
                }
                catch (Exception e)
                {
                    return DataResult.CreateError<ImmutableList<TObject?>>($"Error while building map: {e.Message}");
                }
            }
        );
    }

    /// <summary>
    /// Reads the value stored under <paramref name="key"/> in <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The serialized map to read from.</param>
    /// <param name="key">The key of the value to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the value, or an error if <paramref name="input"/> is not a map or does not contain <paramref name="key"/>.</returns>
    public abstract DataResult<TObject> Get(TObject? input, TObject key);

    /// <summary>
    /// Reads the value stored under the given string <paramref name="key"/> in <paramref name="input"/>.
    /// </summary>
    /// <param name="input">The serialized map to read from.</param>
    /// <param name="key">The key of the value to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the value, or an error if <paramref name="input"/> is not a map or does not contain <paramref name="key"/>.</returns>
    /// <remarks>
    /// The key is converted with <c>CreateString</c>.
    /// </remarks>
    public virtual DataResult<TObject> Get(TObject? input, string key)
    {
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
    public virtual TObject? Set(TObject? input, TObject key, TObject? value)
    {
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
    public virtual TObject? Set(TObject? input, string key, TObject? value)
    {
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
    public virtual TObject? Update(TObject? input, TObject key, Func<TObject, TObject> updater)
    {
        if (IsEmpty(input))
        {
            return input;
        }

        DataResult<TObject> result = Get(input, key);
        if (!result.TryGetResult(out TObject? kr))
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
    public virtual TObject? Update(TObject? input, string key, Func<TObject, TObject> updater)
    {
        return Update(input, CreateString(key), updater);
    }

    /// <summary>
    /// Returns a copy of <paramref name="input"/> without the value stored under <paramref name="key"/>.
    /// </summary>
    /// <param name="input">The serialized map to modify.</param>
    /// <param name="key">The key of the value to remove.</param>
    /// <returns>The modified map, or <paramref name="input"/> itself if it is empty or does not contain <paramref name="key"/>.</returns>
    public abstract TObject? Remove(TObject? input, TObject key);

    /// <summary>
    /// Returns a copy of <paramref name="input"/> without the value stored under the given string <paramref name="key"/>.
    /// </summary>
    /// <param name="input">The serialized map to modify.</param>
    /// <param name="key">The key of the value to remove.</param>
    /// <returns>The modified map, or <paramref name="input"/> itself if it is empty or does not contain <paramref name="key"/>.</returns>
    /// <remarks>
    /// The key is converted with <c>CreateString</c>.
    /// </remarks>
    public virtual TObject? Remove(TObject? input, string key)
    {
        return Remove(input, CreateString(key));
    }

    /// <summary>
    /// Returns a copy of <paramref name="source"/> that can be modified without affecting <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The serialized value to copy.</param>
    /// <returns>A copy of <paramref name="source"/>.</returns>
    public abstract TObject? Copy(TObject? source);

    /// <summary>
    /// Gets a value indicating whether the given <paramref name="input"/> is the empty value of this format.
    /// </summary>
    /// <param name="input">The serialized value to test.</param>
    /// <returns>A <see langword="true"/> if <paramref name="input"/> is empty; otherwise, <see langword="false"/>. When this method returns <see langword="false"/>, <paramref name="input"/> is not <see langword="null"/>.</returns>
    public abstract bool IsEmpty([NotNullWhen(false)] TObject? input);

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
}
