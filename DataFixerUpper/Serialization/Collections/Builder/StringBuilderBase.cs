using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Collections.Builder;

/// <summary>
/// A base class for builders that build a serialized map whose keys are represented by strings.
/// </summary>
/// <typeparam name="TObject">The type of the serialized form.</typeparam>
/// <typeparam name="TBuilder">The type of the builder that accumulates the entries of the map.</typeparam>
public abstract class StringBuilderBase<TObject, TBuilder> : RecordBuilderBase<TObject, TBuilder>
    where TObject : notnull

{
    /// <summary>
    /// Initializes a new instance with the given <paramref name="ops"/>.
    /// </summary>
    /// <param name="ops">The ops used to create the serialized keys and values.</param>
    protected StringBuilderBase(DynamicOps<TObject> ops) : base(ops) { }

    /// <summary>
    /// Appends an entry that maps the given string <paramref name="key"/> to <paramref name="value"/> to <paramref name="builder"/>.
    /// </summary>
    /// <param name="key">The key of the entry to append.</param>
    /// <param name="value">The value of the entry to append.</param>
    /// <param name="builder">The builder to append the entry to.</param>
    /// <returns>The given <paramref name="builder"/>.</returns>
    protected abstract TBuilder Append(string key, TObject value, TBuilder builder);

    /// <inheritdoc/>
    public override RecordBuilder<TObject> Add(TObject key, TObject value)
    {
        Builder = Ops.GetStringValue(key).FlatMap(s =>
            {
                Add(s, value);
                return Builder;
            }
        );
        return this;
    }

    /// <inheritdoc/>
    public override RecordBuilder<TObject> Add(TObject key, DataResult<TObject> value)
    {
        Builder = Ops.GetStringValue(key).FlatMap(s =>
            {
                Add(s, value);
                return Builder;
            }
        );
        return this;
    }

    /// <inheritdoc/>
    public override RecordBuilder<TObject> Add(DataResult<TObject> key, DataResult<TObject> value)
    {
        Builder = key.FlatMap(Ops.GetStringValue).FlatMap(s =>
            {
                Add(s, value);
                return Builder;
            }
        );
        return this;
    }

    /// <inheritdoc/>
    public override RecordBuilder<TObject> Add(string key, TObject value)
    {
        Builder = Builder.Map(builder => Append(key, value, builder));
        return this;
    }

    /// <inheritdoc/>
    public override RecordBuilder<TObject> Add(string key, DataResult<TObject> value)
    {
        Builder = Builder.CombineStable((b, v) => Append(key, v, b), value);
        return this;
    }
}