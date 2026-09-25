using DataFixerUpper.Serialization.Codecs.Impl;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.Collections.Builder;
using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Codecs;

/// <summary>
/// Serializes (encodes) a fixed set of record fields to a serialized form.
/// </summary>
/// <typeparam name="T">The type this <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> serializes.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/>
public interface IMapEncoder<T> : ICompressable
{
    /// <summary>
    /// Encodes the given <paramref name="input"/> into the fields of <paramref name="prefix"/>.
    /// </summary>
    /// <param name="input">The value to encode.</param>
    /// <param name="ops">The ops used to create the encoded fields.</param>
    /// <param name="prefix">The record builder that receives the encoded fields.</param>
    /// <typeparam name="TObject">The type of the encoded value.</typeparam>
    /// <returns>The record builder that contains the encoded fields, carrying any errors that occurred while an individual field was encoded.</returns>
    IRecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
        where TObject : notnull;

    /// <summary>
    /// Gets this implementation viewed as an <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/>.
    /// </summary>
    /// <returns>This as <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/>.</returns>
    public IMapEncoder<T> AsMapEncoder()
    {
        return this;
    }

    /// <summary>
    /// Gets this implementation as an <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/>.
    /// </summary>
    /// <returns>Encoder backed by this.</returns>
    public IEncoder<T> AsEncoder()
    {
        return new MapEncoderEncoder<T>(this);
    }

    /// <summary>
    /// Returns an <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> that marks every result of this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> with the given <paramref name="lifecycle"/>.
    /// </summary>
    /// <param name="lifecycle">The lifecycle to apply to the encoded results.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> that applies <paramref name="lifecycle"/> to this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/>.</returns>
    public IMapEncoder<T> WithLifecycle(Lifecycle lifecycle)
    {
        return new LifecycleMapEncoder<T>(this, lifecycle);
    }
}

/// <summary>
/// Simple implementation for <see cref="M:DataFixerUpper.Serialization.Collections.ICompressable.GetCompressor``1(DataFixerUpper.Serialization.DynamicOps.DynamicOps{``0})"/>
/// </summary>
/// <typeparam name="T">The type this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> serializes.</typeparam>
public abstract class MapEncoderBase<T> : CompressorHolder, IMapEncoder<T>
{
    /// <inheritdoc/>
    public abstract IRecordBuilder<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, IRecordBuilder<TObject> prefix)
        where TObject : notnull;
}