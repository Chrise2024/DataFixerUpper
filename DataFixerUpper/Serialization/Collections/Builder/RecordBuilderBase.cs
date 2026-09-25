using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Collections.Builder;

/// <summary>
/// A base class for builders that accumulate serialized values and build them into a single serialized value.
/// </summary>
/// <typeparam name="TObject">The type of the serialized form.</typeparam>
/// <typeparam name="TBuilder">The type of the builder that accumulates the serialized values.</typeparam>
public abstract class RecordBuilderBase<TObject, TBuilder> : IRecordBuilder<TObject>
    where TObject : notnull
{
    /// <summary>
    /// Gets the ops used to create the serialized keys and values.
    /// </summary>
    public DynamicOps<TObject> Ops { get; }

    /// <summary>
    /// Gets or sets the builder that accumulates the serialized values, creating it with <c>InitBuilder</c> on first access.
    /// </summary>
    protected DataResult<TBuilder> Builder
    {
        get
        {
            return field ??= CreateBuilder();
        }
        set;
    }

    /// <summary>
    /// Initializes a new instance with the given <paramref name="ops"/>.
    /// </summary>
    /// <param name="ops">The ops used to create the serialized keys and values.</param>
    protected RecordBuilderBase(DynamicOps<TObject> ops)
    {
        Ops = ops;
    }

    private DataResult<TBuilder> CreateBuilder()
    {
        return DataResult.CreateSuccess(InitBuilder(), Lifecycle.Stable);
    }

    /// <summary>
    /// Creates a new builder that accumulates the serialized values.
    /// </summary>
    /// <returns>A new builder.</returns>
    protected abstract TBuilder InitBuilder();

    /// <summary>
    /// Builds the value accumulated by <paramref name="builder"/>, merging it into <paramref name="prefix"/>.
    /// </summary>
    /// <param name="builder">The builder that contains the accumulated value.</param>
    /// <param name="prefix">The existing value to merge the built value into, which may be empty.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the built value, or an error if it could not be built.</returns>
    protected abstract DataResult<TObject> BuildResult(TBuilder builder, TObject? prefix);

    /// <summary>
    /// Adds an entry that maps <paramref name="key"/> to <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <returns>This builder.</returns>
    public abstract IRecordBuilder<TObject> Add(TObject key, TObject? value);

    /// <summary>
    /// Adds an entry that maps <paramref name="key"/> to the value of <paramref name="value"/>, carrying over the errors of <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The result containing the value of the entry to add.</param>
    /// <returns>This builder.</returns>
    public abstract IRecordBuilder<TObject> Add(TObject key, DataResult<TObject> value);

    /// <summary>
    /// Adds an entry that maps the key of <paramref name="key"/> to the value of <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The result containing the key of the entry to add.</param>
    /// <param name="value">The result containing the value of the entry to add.</param>
    /// <returns>This builder.</returns>
    public abstract IRecordBuilder<TObject> Add(DataResult<TObject> key, DataResult<TObject> value);

    /// <inheritdoc/>
    public IRecordBuilder<TObject> SetLifecycle(Lifecycle lifecycle)
    {
        Builder = Builder.SetLifecycle(lifecycle);
        return this;
    }

    /// <inheritdoc/>
    public IRecordBuilder<TObject> MapError(UnaryOperation<string> mapper)
    {
        Builder = Builder.MapError(mapper);
        return this;
    }

    /// <inheritdoc/>
    public IRecordBuilder<TObject> WithErrorsFrom<TOther>(DataResult<TOther> result)
    {
        Builder = Builder.FlatMap(builder => result.Map(_ => builder));
        return this;
    }

    /// <summary>
    /// Builds the accumulated value, merging it into <paramref name="prefix"/>, and resets this builder so that it can be reused.
    /// </summary>
    /// <param name="prefix">The existing value to merge the built value into, which may be empty.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the built value, or an error if it could not be built.</returns>
    public DataResult<TObject> Build(TObject? prefix)
    {
        DataResult<TObject> result = Builder.FlatMap(builder => BuildResult(builder, prefix));
        Builder = CreateBuilder();
        return result;
    }
}
