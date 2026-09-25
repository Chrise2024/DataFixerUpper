using System.Collections.Generic;
using System.Collections.Immutable;
using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Collections.Builder;

/// <summary>
/// A builder that accumulates entries into a serialized map.
/// </summary>
/// <typeparam name="TObject">The type of the serialized form.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.Collections.Builder.MapBuilderBase`2"/>
public class MapBuilder<TObject> : MapBuilderBase<TObject, ImmutableDictionary<TObject, TObject?>.Builder>
    where TObject : notnull
{
    /// <summary>
    /// Initializes a new instance with the given <paramref name="ops"/>.
    /// </summary>
    /// <param name="ops">The ops used to create the serialized keys and values.</param>
    public MapBuilder(DynamicOps<TObject> ops) : base(ops) { }

    /// <summary>
    /// Creates a new builder that accumulates the entries of the map.
    /// </summary>
    /// <returns>A new builder.</returns>
    protected override ImmutableDictionary<TObject, TObject?>.Builder InitBuilder()
    {
        return ImmutableDictionary.CreateBuilder<TObject, TObject?>();
    }

    /// <summary>
    /// Builds the map accumulated by <paramref name="builder"/>, merging it into <paramref name="prefix"/>.
    /// </summary>
    /// <param name="builder">The builder that contains the accumulated entries.</param>
    /// <param name="prefix">The existing map to merge the built map into, which may be empty.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the built map, or an error if it could not be built.</returns>
    protected override DataResult<TObject> BuildResult(ImmutableDictionary<TObject, TObject?>.Builder builder, TObject? prefix)
    {
        return Ops.MergeToMap(prefix, builder.ToImmutable());
    }

    /// <summary>
    /// Appends an entry that maps <paramref name="key"/> to <paramref name="value"/> to <paramref name="builder"/>.
    /// </summary>
    /// <param name="key">The key of the entry to append.</param>
    /// <param name="value">The value of the entry to append.</param>
    /// <param name="builder">The builder to append the entry to.</param>
    /// <returns>The given <paramref name="builder"/>.</returns>
    protected override ImmutableDictionary<TObject, TObject?>.Builder Append(TObject key, TObject? value, ImmutableDictionary<TObject, TObject?>.Builder builder)
    {
        return builder.AddAndReturn(key, value);
    }

    /// <summary>
    /// Appends an entry that maps the given string <paramref name="key"/> to <paramref name="value"/> to <paramref name="builder"/>.
    /// </summary>
    /// <param name="key">The key of the entry to append.</param>
    /// <param name="value">The value of the entry to append.</param>
    /// <param name="builder">The builder to append the entry to.</param>
    /// <returns>The given <paramref name="builder"/>.</returns>
    /// <remarks>
    /// The key is converted with <c>CreateString</c>.
    /// </remarks>
    protected override ImmutableDictionary<TObject, TObject?>.Builder Append(string key, TObject? value, ImmutableDictionary<TObject, TObject?>.Builder builder)
    {
        return builder.AddAndReturn(Ops.CreateString(key), value);
    }
}
