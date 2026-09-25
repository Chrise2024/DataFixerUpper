using System.Collections.Generic;
using DataFixerUpper.Serialization.Codecs;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Collections.Builder;

/// <summary>
/// Builds a serialized list by adding elements to it.
/// </summary>
/// <typeparam name="TObject">The type of the serialized form.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.Collections.Builder.ListBuilder`1"/>
public interface IListBuilder<TObject>
    where TObject : notnull
{
    /// <summary>
    /// Gets the ops used to create the serialized elements.
    /// </summary>
    DynamicOps<TObject> Ops { get; }

    /// <summary>
    /// Adds <paramref name="value"/> to the list.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>This builder.</returns>
    IListBuilder<TObject> Add(TObject? value);

    /// <summary>
    /// Adds the value of <paramref name="value"/> to the list, carrying over the errors of <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The result containing the value to add.</param>
    /// <returns>This builder.</returns>
    IListBuilder<TObject> Add(DataResult<TObject> value);

    /// <summary>
    /// Adds <paramref name="value"/> encoded with <paramref name="encoder"/> to the list.
    /// </summary>
    /// <param name="value">The value to encode and add.</param>
    /// <param name="encoder">The encoder used to serialize <paramref name="value"/>.</param>
    /// <typeparam name="T">The type of the value to encode.</typeparam>
    /// <returns>This builder.</returns>
    IListBuilder<TObject> Add<T>(T value, IEncoder<T> encoder)
    {
        return Add(encoder.EncodeStart(Ops, value));
    }

    /// <summary>
    /// Adds all values of <paramref name="values"/> to the list.
    /// </summary>
    /// <param name="values">The values to add.</param>
    /// <returns>This builder.</returns>
    IListBuilder<TObject> AddRange(IEnumerable<TObject?> values)
    {
        foreach (TObject? value in values)
        {
            Add(value);
        }

        return this;
    }

    /// <summary>
    /// Adds the values of <paramref name="values"/> to the list, carrying over the errors of each result.
    /// </summary>
    /// <param name="values">The results containing the values to add.</param>
    /// <returns>This builder.</returns>
    IListBuilder<TObject> AddRange(IEnumerable<DataResult<TObject>> values)
    {
        foreach (DataResult<TObject> value in values)
        {
            Add(value);
        }

        return this;
    }

    /// <summary>
    /// Adds all values of <paramref name="values"/> encoded with <paramref name="encoder"/> to the list.
    /// </summary>
    /// <param name="values">The values to encode and add.</param>
    /// <param name="encoder">The encoder used to serialize the values.</param>
    /// <typeparam name="T">The type of the values to encode.</typeparam>
    /// <returns>This builder.</returns>
    IListBuilder<TObject> AddRange<T>(IEnumerable<T> values, IEncoder<T> encoder)
    {
        foreach (T value in values)
        {
            Add(encoder.EncodeStart(Ops, value));
        }

        return this;
    }

    /// <summary>
    /// Carries the errors of <paramref name="result"/> over to this builder.
    /// </summary>
    /// <param name="result">The result whose errors are carried over.</param>
    /// <typeparam name="TOther">The type of the value contained in <paramref name="result"/>.</typeparam>
    /// <returns>This builder.</returns>
    IListBuilder<TObject> WithErrorsFrom<TOther>(DataResult<TOther> result);

    /// <summary>
    /// Transforms the error message of this builder with the given <paramref name="mapper"/>.
    /// </summary>
    /// <param name="mapper">The function that transforms the error message.</param>
    /// <returns>This builder.</returns>
    IListBuilder<TObject> MapError(UnaryOperation<string> mapper);

    /// <summary>
    /// Builds the list, merging it into <paramref name="prefix"/>.
    /// </summary>
    /// <param name="prefix">The existing list to merge the built list into, which may be empty.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the built list, or an error if the elements could not be merged.</returns>
    DataResult<TObject> Build(TObject? prefix);

    /// <summary>
    /// Builds the list, merging it into the value of <paramref name="prefix"/>.
    /// </summary>
    /// <param name="prefix">The result containing the existing list to merge the built list into.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the built list, or an error if the prefix is an error or the elements could not be merged.</returns>
    DataResult<TObject> Build(DataResult<TObject> prefix)
    {
        return prefix.FlatMap(Build);
    }
}
