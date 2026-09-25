using System.Collections.Generic;
using System.Numerics;
using DataFixerUpper.Serialization.Codecs;

namespace DataFixerUpper.Serialization.DynamicOps;

/// <summary>
/// An abstract base class for objects that encapsulate a value of some serialized type. This allows methods that return generic serialized values to define strongly typed signatures without knowing the actual types of the serialized form.
/// </summary>
/// <remarks>
/// Instances of this class effectively encapsulate a serialized value and the <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> that can produce and parse it. This allows for more fluid handling of serialized values without needing to separately pass a <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> object everywhere.
/// </remarks>
/// <typeparam name="TObject">The type this class serializes to and deserializes from, for example <see cref="T:System.Text.Json.Nodes.JsonNode"/>.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.DynamicOps.OptionalDynamic`1"/>
public abstract class DynamicLike<TObject>
    where TObject : notnull
{
    /// <summary>
    /// The <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> instance used to deserialize values.
    /// </summary>
    public DynamicOps<TObject> Ops { get; }

    /// <param name="ops">The <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> instance used to deserialize values.</param>
    protected DynamicLike(DynamicOps<TObject> ops)
    {
        Ops = ops;
    }

    /// <summary>
    /// Decodes this value with the given <paramref name="decoder"/>.
    /// </summary>
    /// <param name="decoder">The decoder used to parse this value.</param>
    /// <typeparam name="TResult">The type of the decoded value.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded value together with the remaining input, or an error if this value cannot be decoded.</returns>
    public abstract DataResult<(TResult, TObject?)> Decode<TResult>(IDecoder<TResult> decoder);

    /// <summary>
    /// Reads this value as a number.
    /// </summary>
    /// <typeparam name="TNumber">The type of number to read.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the number, or an error if this value is not a number that can be represented by <typeparamref name="TNumber"/>.</returns>
    public abstract DataResult<TNumber> AsNumber<TNumber>()
        where TNumber : INumber<TNumber>;

    /// <summary>
    /// Reads this value as a <see langword="string"/>.
    /// </summary>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the string, or an error if this value is not a string.</returns>
    public abstract DataResult<string> AsString();

    /// <summary>
    /// Reads this value as a <see langword="bool"/>.
    /// </summary>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the boolean, or an error if this value is not a boolean.</returns>
    public abstract DataResult<bool> AsBool();

    /// <summary>
    /// Reads this value as a list.
    /// </summary>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the elements of the list, or an error if this value is not a list.</returns>
    public abstract DataResult<IEnumerable<Dynamic<TObject>>> AsListValues();

    /// <summary>
    /// Reads this value as a map.
    /// </summary>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the entries of the map, or an error if this value is not a map.</returns>
    public abstract DataResult<IEnumerable<KeyValuePair<Dynamic<TObject>, Dynamic<TObject>>>> AsMapEntries();

    /// <summary>
    /// Reads the entry stored under the given <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key of the entry to read.</param>
    /// <returns>An <see cref="T:DataFixerUpper.Serialization.DynamicOps.OptionalDynamic`1"/> containing the entry, or an error if this value is not a map or has no entry under <paramref name="key"/>.</returns>
    public abstract OptionalDynamic<TObject> Get(string key);

    /// <summary>
    /// Reads the entry stored under the given <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key of the entry to read.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the value, or an error if this value is not a map or has no entry under <paramref name="key"/>.</returns>
    public abstract DataResult<TObject> Get(TObject key);
}
