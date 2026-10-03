using System.Collections.Generic;
using System.Collections.Immutable;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Collections.Builder;

/// <summary>
/// A builder that accumulates elements into a serialized list.
/// </summary>
/// <typeparam name="TObject">The type of the serialized form.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.Collections.Builder.IListBuilder`1"/>
public sealed class ListBuilder<TObject> : ListBuilderBase<TObject>
    where TObject : notnull
{
    private DataResult<ImmutableList<TObject?>.Builder> _builder = DataResult.CreateSuccess(ImmutableList.CreateBuilder<TObject?>(), Lifecycle.Stable);

    /// <summary>
    /// Initializes a new instance with the given <paramref name="ops"/>.
    /// </summary>
    /// <param name="ops">The ops used to create the serialized elements.</param>
    public ListBuilder(DynamicOps<TObject> ops) : base(ops) { }

    /// <inheritdoc/>
    public override ListBuilderBase<TObject> Add(TObject? value)
    {
        _builder = _builder.Map(builder => builder.AddAndReturn(value));

        return this;
    }

    /// <inheritdoc/>
    public override ListBuilderBase<TObject> Add(DataResult<TObject> value)
    {
        _builder = _builder.CombineStable(Functions.AddToFirst, value.Map(TObject? (v) => v));
        return this;
    }

    /// <inheritdoc/>
    public override ListBuilderBase<TObject> WithErrorsFrom<TOther>(DataResult<TOther> result)
    {
        _builder = _builder.FlatMap(builder => result.Map(_ => builder));
        return this;
    }

    /// <inheritdoc/>
    public override ListBuilderBase<TObject> MapError(UnaryOperation<string> mapper)
    {
        _builder = _builder.MapError(mapper);
        return this;
    }

    /// <inheritdoc/>
    public override DataResult<TObject> Build(TObject? prefix)
    {
        DataResult<TObject> result = _builder.FlatMap(builder => Ops.MergeToList(prefix, builder.ToImmutable()));
        _builder = DataResult.CreateSuccess(ImmutableList.CreateBuilder<TObject?>(), Lifecycle.Stable);
        return result;
    }
}