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
public sealed class ListBuilder<TObject> : IListBuilder<TObject>
    where TObject : notnull
{
    /// <summary>
    /// Gets the ops used to create the serialized elements.
    /// </summary>
    public DynamicOps<TObject> Ops { get; }
    
    private DataResult<ImmutableList<TObject>.Builder> _builder = DataResult.CreateSuccess(ImmutableList.CreateBuilder<TObject>(), Lifecycle.Stable);

    /// <summary>
    /// Initializes a new instance with the given <paramref name="ops"/>.
    /// </summary>
    /// <param name="ops">The ops used to create the serialized elements.</param>
    public ListBuilder(DynamicOps<TObject> ops)
    {
        Ops = ops;
    }

    /// <summary>
    /// Adds <paramref name="value"/> to the list.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>This builder.</returns>
    public IListBuilder<TObject> Add(TObject? value)
    {
        if (value is not null)
        {
            _builder = _builder.Map(builder => builder.AddAndReturn(value));
        }
        
        return this;
    }

    /// <summary>
    /// Adds the value of <paramref name="value"/> to the list, carrying over the errors of <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The result containing the value to add.</param>
    /// <returns>This builder.</returns>
    public IListBuilder<TObject> Add(DataResult<TObject> value)
    {
        _builder = _builder.CombineStable(Functions.AddToFirst, value);
        return this;
    }

    /// <summary>
    /// Carries the errors of <paramref name="result"/> over to this builder.
    /// </summary>
    /// <param name="result">The result whose errors are carried over.</param>
    /// <typeparam name="TOther">The type of the value contained in <paramref name="result"/>.</typeparam>
    /// <returns>This builder.</returns>
    public IListBuilder<TObject> WithErrorsFrom<TOther>(DataResult<TOther> result)
    {
        _builder = _builder.FlatMap(builder => result.Map(_ => builder));
        return this;
    }

    /// <summary>
    /// Transforms the error message of this builder with the given <paramref name="mapper"/>.
    /// </summary>
    /// <param name="mapper">The function that transforms the error message.</param>
    /// <returns>This builder.</returns>
    public IListBuilder<TObject> MapError(UnaryOperation<string> mapper)
    {
        _builder = _builder.MapError(mapper);
        return this;
    }

    /// <summary>
    /// Builds the list, merging it into <paramref name="prefix"/>, and resets this builder so that it can be reused.
    /// </summary>
    /// <param name="prefix">The existing list to merge the built list into, which may be empty.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the built list, or an error if the elements could not be merged.</returns>
    public DataResult<TObject> Build(TObject? prefix)
    {
        DataResult<TObject> result = _builder.FlatMap(builder => Ops.MergeToList(prefix, builder.ToImmutable()));
        _builder = DataResult.CreateSuccess(ImmutableList.CreateBuilder<TObject>(), Lifecycle.Stable);
        return result;
    }
}
