using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Serialization.Codecs;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Collections.Builder;

/// <summary>
/// A base class for builders that accumulate serialized values and build them into a single serialized value.
/// </summary>
/// <typeparam name="TObject">The type of the serialized form.</typeparam>
public abstract class RecordBuilder<TObject>
    where TObject : notnull
{
    /// <summary>
    /// Gets the ops used to create the serialized keys and values.
    /// </summary>
    public DynamicOps<TObject> Ops { get; }

    /// <summary>
    /// Initializes a new instance with the given <paramref name="ops"/>.
    /// </summary>
    /// <param name="ops">The ops used to create the serialized keys and values.</param>
    protected RecordBuilder(DynamicOps<TObject> ops)
    {
        Ops = ops;
    }

    /// <summary>
    /// Adds an entry that maps <paramref name="key"/> to <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <returns>This builder.</returns>
    public abstract RecordBuilder<TObject> Add(TObject key, TObject value);

    /// <summary>
    /// Adds the given <paramref name="pair"/>.
    /// </summary>
    /// <param name="pair">The entry to add.</param>
    /// <returns>This builder.</returns>
    public RecordBuilder<TObject> Add(KeyValuePair<TObject, TObject> pair)
    {
        return Add(pair.Key, pair.Value);
    }

    /// <summary>
    /// Adds all entries of <paramref name="pairs"/>.
    /// </summary>
    /// <param name="pairs">The entries to add.</param>
    /// <returns>This builder.</returns>
    public RecordBuilder<TObject> AddRange(IEnumerable<KeyValuePair<TObject, TObject>> pairs)
    {
        return pairs.Aggregate(this, (builder, pair) => builder.Add(pair));
    }

    /// <summary>
    /// Adds an entry that maps <paramref name="key"/> to the value of <paramref name="value"/>, carrying over the errors of <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The result containing the value of the entry to add.</param>
    /// <returns>This builder.</returns>
    public abstract RecordBuilder<TObject> Add(TObject key, DataResult<TObject> value);

    /// <summary>
    /// Adds an entry that maps the key of <paramref name="key"/> to the value of <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The result containing the key of the entry to add.</param>
    /// <param name="value">The result containing the value of the entry to add.</param>
    /// <returns>This builder.</returns>
    public abstract RecordBuilder<TObject> Add(DataResult<TObject> key, DataResult<TObject> value);

    /// <summary>
    /// Adds an entry that maps the given string <paramref name="key"/> to <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key of the entry to add.</param>
    /// <param name="value">The value of the entry to add.</param>
    /// <returns>This builder.</returns>
    /// <remarks>
    /// The key is converted with <c>CreateString</c>.
    /// </remarks>
    public virtual RecordBuilder<TObject> Add(string key, TObject value)
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
    public virtual RecordBuilder<TObject> Add(string key, DataResult<TObject> value)
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
    public RecordBuilder<TObject> Add<T>(string key, T value, IEncoder<T> encoder)
    {
        return Add(key, encoder.EncodeStart(Ops, value));
    }

    /// <summary>
    /// Carries the errors of <paramref name="result"/> over to this builder.
    /// </summary>
    /// <param name="result">The result whose errors are carried over.</param>
    /// <typeparam name="TOther">The type of the value contained in <paramref name="result"/>.</typeparam>
    /// <returns>This builder.</returns>
    public abstract RecordBuilder<TObject> WithErrorsFrom<TOther>(DataResult<TOther> result);

    /// <summary>
    /// Sets the lifecycle of the built value.
    /// </summary>
    /// <param name="lifecycle">The lifecycle to use.</param>
    /// <returns>This builder.</returns>
    public abstract RecordBuilder<TObject> SetLifecycle(Lifecycle lifecycle);

    /// <summary>
    /// Transforms the error message of this builder with the given <paramref name="mapper"/>.
    /// </summary>
    /// <param name="mapper">The function that transforms the error message.</param>
    /// <returns>This builder.</returns>
    public abstract RecordBuilder<TObject> MapError(UnaryOperation<string> mapper);

    /// <summary>
    /// Builds the map, merging it into <paramref name="prefix"/>.
    /// </summary>
    /// <param name="prefix">The existing map to merge the built map into, which may be empty.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the built map, or an error if the entries could not be merged.</returns>
    public abstract DataResult<TObject> Build(TObject prefix);

    /// <summary>
    /// Builds the map, merging it into the value of <paramref name="prefix"/>.
    /// </summary>
    /// <param name="prefix">The result containing the existing map to merge the built map into.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the built map, or an error if the prefix is an error or the entries could not be merged.</returns>
    public DataResult<TObject> Build(DataResult<TObject> prefix)
    {
        return prefix.FlatMap(Build);
    }
}

/// <inheritdoc/>
/// <typeparam name="TBuilder">The type of the builder that accumulates the serialized values.</typeparam>
#pragma warning disable CS1712
public abstract class RecordBuilderBase<TObject, TBuilder> : RecordBuilder<TObject>
    where TObject : notnull
#pragma warning restore CS1712
{
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

    /// <inheritdoc/>
    protected RecordBuilderBase(DynamicOps<TObject> ops) : base(ops) { }

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
    protected abstract DataResult<TObject> BuildResult(TBuilder builder, TObject prefix);

    /// <inheritdoc/>
    public override RecordBuilder<TObject> SetLifecycle(Lifecycle lifecycle)
    {
        Builder = Builder.SetLifecycle(lifecycle);
        return this;
    }

    /// <inheritdoc/>
    public override RecordBuilder<TObject> MapError(UnaryOperation<string> mapper)
    {
        Builder = Builder.MapError(mapper);
        return this;
    }

    /// <inheritdoc/>
    public override RecordBuilder<TObject> WithErrorsFrom<TOther>(DataResult<TOther> result)
    {
        Builder = Builder.FlatMap(builder => result.Map(_ => builder));
        return this;
    }

    /// <summary>
    /// Builds the accumulated value, merging it into <paramref name="prefix"/>, and resets this builder so that it can be reused.
    /// </summary>
    /// <param name="prefix">The existing value to merge the built value into, which may be empty.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the built value, or an error if it could not be built.</returns>
    public override DataResult<TObject> Build(TObject prefix)
    {
        DataResult<TObject> result = Builder.FlatMap(builder => BuildResult(builder, prefix));
        Builder = CreateBuilder();
        return result;
    }
}