using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Collections.Builder;

/// <summary>
/// A builder that builds a serialized map as a list of values whose positions are determined by a key compressor.
/// </summary>
/// <remarks>
/// Rather than writing the keys, this builder writes only the values, using the index that <see cref="T:DataFixerUpper.Serialization.Collections.KeyCompressor`1"/> associates with each key. It is used for formats whose ops report <c>CompressMaps</c>.
/// </remarks>
/// <typeparam name="TObject">The type of the serialized form.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.Collections.KeyCompressor`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.Collections.Builder.MapBuilderBase`2"/>
public sealed class CompressedRecordBuilder<TObject> : MapBuilderBase<TObject, TObject?[]>
    where TObject : notnull
{
    private readonly KeyCompressor<TObject> _compressor;

    /// <summary>
    /// Initializes a new instance with the given <paramref name="ops"/> and <paramref name="compressor"/>.
    /// </summary>
    /// <param name="ops">The ops used to create the serialized values.</param>
    /// <param name="compressor">The compressor that associates an index with every key.</param>
    public CompressedRecordBuilder(DynamicOps<TObject> ops, KeyCompressor<TObject> compressor) : base(ops)
    {
        _compressor = compressor;
    }

    /// <summary>
    /// Creates a new array that has one position for every compressed key.
    /// </summary>
    /// <returns>A new array with a length equal to the number of compressed keys.</returns>
    protected override TObject[] InitBuilder()
    {
        return new TObject[_compressor.Count];
    }

    /// <summary>
    /// Stores <paramref name="value"/> at the index associated with <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key of the entry to append.</param>
    /// <param name="value">The value of the entry to append.</param>
    /// <param name="builder">The array to store the value in.</param>
    /// <returns>The given <paramref name="builder"/>.</returns>
    protected override TObject?[] Append(TObject key, TObject? value, TObject?[] builder)
    {
        builder[_compressor.Compress(key)] = value;
        return builder;
    }

    /// <summary>
    /// Stores <paramref name="value"/> at the index associated with the given string <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key of the entry to append.</param>
    /// <param name="value">The value of the entry to append.</param>
    /// <param name="builder">The array to store the value in.</param>
    /// <returns>The given <paramref name="builder"/>.</returns>
    protected override TObject?[] Append(string key, TObject? value, TObject?[] builder)
    {
        builder[_compressor.Compress(key)] = value;
        return builder;
    }

    /// <summary>
    /// Builds the array accumulated by <paramref name="builder"/> as a list, merging it into <paramref name="prefix"/>.
    /// </summary>
    /// <param name="builder">The array that contains the accumulated values.</param>
    /// <param name="prefix">The existing value to merge the built value into, which may be empty.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the built list, or an error if it could not be built.</returns>
    protected override DataResult<TObject> BuildResult(TObject?[] builder, TObject? prefix)
    {
        return Ops.MergeToList(prefix, builder);
    }
}
