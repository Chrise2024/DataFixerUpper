using System.Collections.Generic;
using System.Collections.Immutable;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Collections.Builder;

/// <summary>
/// A builder that accumulates entries into a serialized map.
/// </summary>
/// <typeparam name="TObject">The type of the serialized form.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.Collections.Builder.MapBuilderBase`2"/>
public class MapBuilder<TObject> : MapBuilderBase<TObject, ImmutableList<Pair<TObject, TObject?>>.Builder>
    where TObject : notnull
{
    /// <summary>
    /// Initializes a new instance with the given <paramref name="ops"/>.
    /// </summary>
    /// <param name="ops">The ops used to create the serialized keys and values.</param>
    public MapBuilder(DynamicOps<TObject> ops) : base(ops) { }

    /// <inheritdoc/>
    protected override ImmutableList<Pair<TObject, TObject?>>.Builder InitBuilder()
    {
        return ImmutableList.CreateBuilder<Pair<TObject, TObject?>>();
    }

    /// <inheritdoc/>
    protected override DataResult<TObject> BuildResult(ImmutableList<Pair<TObject, TObject?>>.Builder builder, TObject? prefix)
    {
        return Ops.MergeToMap(prefix, builder.ToImmutable());
    }

    /// <inheritdoc/>
    protected override ImmutableList<Pair<TObject, TObject?>>.Builder Append(TObject key, TObject? value, ImmutableList<Pair<TObject, TObject?>>.Builder builder)
    {
        return builder.AddAndReturn(Pair.Create(key, value));
    }

    /// <inheritdoc/>
    protected override ImmutableList<Pair<TObject, TObject?>>.Builder Append(string key, TObject? value, ImmutableList<Pair<TObject, TObject?>>.Builder builder)
    {
        return builder.AddAndReturn(Pair.Create(Ops.CreateString(key), value));
    }
}