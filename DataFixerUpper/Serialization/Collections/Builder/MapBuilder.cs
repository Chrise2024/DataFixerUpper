using System.Collections.Generic;
using System.Collections.Immutable;
using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Collections.Builder;

/// <summary>
/// A builder that accumulates entries into a serialized map.
/// </summary>
/// <typeparam name="TObject">The type of the serialized form.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.Collections.Builder.MapBuilderBase`2"/>
public class MapBuilder<TObject> : MapBuilderBase<TObject, ImmutableDictionary<TObject, TObject>.Builder>
    where TObject : notnull
{
    /// <summary>
    /// Initializes a new instance with the given <paramref name="ops"/>.
    /// </summary>
    /// <param name="ops">The ops used to create the serialized keys and values.</param>
    public MapBuilder(DynamicOps<TObject> ops) : base(ops) { }

    /// <inheritdoc/>
    protected override ImmutableDictionary<TObject, TObject>.Builder InitBuilder()
    {
        return ImmutableDictionary.CreateBuilder<TObject, TObject>();
    }

    /// <inheritdoc/>
    protected override DataResult<TObject> BuildResult(ImmutableDictionary<TObject, TObject>.Builder builder, TObject prefix)
    {
        return Ops.MergeToMap(prefix, builder.ToImmutable());
    }

    /// <inheritdoc/>
    protected override ImmutableDictionary<TObject, TObject>.Builder Append(TObject key, TObject value, ImmutableDictionary<TObject, TObject>.Builder builder)
    {
        return builder.AddAndReturn(key, value);
    }

    /// <inheritdoc/>
    protected override ImmutableDictionary<TObject, TObject>.Builder Append(string key, TObject value, ImmutableDictionary<TObject, TObject>.Builder builder)
    {
        return builder.AddAndReturn(Ops.CreateString(key), value);
    }
}