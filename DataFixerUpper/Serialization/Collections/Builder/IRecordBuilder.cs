using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Serialization.Codecs;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Collections.Builder;

/// <summary>
/// Builds a serialized map by adding entries to it.
/// </summary>
/// <typeparam name="TObject">The type of the serialized form.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.Collections.Builder.MapBuilder`1"/>
public interface IRecordBuilder<TObject>
    where TObject : notnull
{
    /// <summary>
    /// Gets the ops used to create the serialized keys and values.
    /// </summary>
    DynamicOps<TObject> Ops { get; }

    /// <summary>
    /// Adds an entry that maps <paramref name="key"/> to <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <returns>This builder.</returns>
    IRecordBuilder<TObject> Add(TObject key, TObject? value);

    /// <summary>
    /// Adds the given <paramref name="pair"/>.
    /// </summary>
    /// <param name="pair">The entry to add.</param>
    /// <returns>This builder.</returns>
    IRecordBuilder<TObject> Add(KeyValuePair<TObject, TObject?> pair)
    {
        return Add(pair.Key, pair.Value);
    }

    /// <summary>
    /// Adds all entries of <paramref name="pairs"/>.
    /// </summary>
    /// <param name="pairs">The entries to add.</param>
    /// <returns>This builder.</returns>
    public IRecordBuilder<TObject> AddRange(IEnumerable<KeyValuePair<TObject, TObject?>> pairs)
    {
        return pairs.Aggregate(this, (builder, pair) => builder.Add(pair));
    }

    /// <summary>
    /// Adds an entry that maps <paramref name="key"/> to the value of <paramref name="value"/>, carrying over the errors of <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The result containing the value of the entry to add.</param>
    /// <returns>This builder.</returns>
    IRecordBuilder<TObject> Add(TObject key, DataResult<TObject> value);

    /// <summary>
    /// Adds an entry that maps the key of <paramref name="key"/> to the value of <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The result containing the key of the entry to add.</param>
    /// <param name="value">The result containing the value of the entry to add.</param>
    /// <returns>This builder.</returns>
    IRecordBuilder<TObject> Add(DataResult<TObject> key, DataResult<TObject> value);

    /// <summary>
    /// Adds an entry that maps the given string <paramref name="key"/> to <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <returns>This builder.</returns>
    /// <remarks>
    /// The key is converted with <c>CreateString</c>.
    /// </remarks>
    IRecordBuilder<TObject> Add(string key, TObject value)
    {
        return Add(Ops.CreateString(key), value);
    }

    /// <summary>
    /// Adds an entry that maps the given string <paramref name="key"/> to the value of <paramref name="value"/>, carrying over the errors of <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The result containing the value of the entry to add.</param>
    /// <returns>This builder.</returns>
    /// <remarks>
    /// The key is converted with <c>CreateString</c>.
    /// </remarks>
    IRecordBuilder<TObject> Add(string key, DataResult<TObject> value)
    {
        return Add(Ops.CreateString(key), value);
    }

    /// <summary>
    /// Adds an entry that maps the given string <paramref name="key"/> to <paramref name="value"/> encoded with <paramref name="encoder"/>.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value to encode and add.</param>
    /// <param name="encoder">The encoder used to serialize <paramref name="value"/>.</param>
    /// <typeparam name="T">The type of the value to encode.</typeparam>
    /// <returns>This builder.</returns>
    /// <remarks>
    /// The key is converted with <c>CreateString</c>.
    /// </remarks>
    IRecordBuilder<TObject> Add<T>(string key, T value, IEncoder<T> encoder)
    {
        return Add(key, encoder.EncodeStart(Ops, value));
    }

    /// <summary>
    /// Carries the errors of <paramref name="result"/> over to this builder.
    /// </summary>
    /// <param name="result">The result whose errors are carried over.</param>
    /// <typeparam name="TOther">The type of the value contained in <paramref name="result"/>.</typeparam>
    /// <returns>This builder.</returns>
    IRecordBuilder<TObject> WithErrorsFrom<TOther>(DataResult<TOther> result);

    /// <summary>
    /// Sets the lifecycle of the built value.
    /// </summary>
    /// <param name="lifecycle">The lifecycle to use.</param>
    /// <returns>This builder.</returns>
    IRecordBuilder<TObject> SetLifecycle(Lifecycle lifecycle);

    /// <summary>
    /// Transforms the error message of this builder with the given <paramref name="mapper"/>.
    /// </summary>
    /// <param name="mapper">The function that transforms the error message.</param>
    /// <returns>This builder.</returns>
    IRecordBuilder<TObject> MapError(UnaryOperation<string> mapper);

    /// <summary>
    /// Builds the map, merging it into <paramref name="prefix"/>.
    /// </summary>
    /// <param name="prefix">The existing map to merge the built map into, which may be empty.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the built map, or an error if the entries could not be merged.</returns>
    DataResult<TObject> Build(TObject? prefix);

    /// <summary>
    /// Builds the map, merging it into the value of <paramref name="prefix"/>.
    /// </summary>
    /// <param name="prefix">The result containing the existing map to merge the built map into.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the built map, or an error if the prefix is an error or the entries could not be merged.</returns>
    DataResult<TObject> Build(DataResult<TObject> prefix)
    {
        return prefix.FlatMap(Build);
    }
}
