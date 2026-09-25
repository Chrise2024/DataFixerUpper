using DataFixerUpper.Serialization.Codecs.Impl;
using DataFixerUpper.Serialization.Collections;
using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Codecs;

/// <summary>
/// Deserializes (decodes) a fixed set of record fields from a serialized form.
/// </summary>
/// <typeparam name="T">The type this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> deserializes.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.MapCodec`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/>
public interface IMapDecoder<T> : ICompressable
{
    /// <summary>
    /// Decodes a value of type <typeparamref name="T"/> from the fields of the given <paramref name="input"/> map.
    /// </summary>
    /// <param name="ops">The ops that define the form of <paramref name="input"/>.</param>
    /// <param name="input">The map that contains the fields to decode.</param>
    /// <typeparam name="TObject">The type of the decoded value.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded value, or an error if the value cannot be decoded.</returns>
    DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
        where TObject : notnull;

    /// <summary>
    /// Gets this implementation viewed as an <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/>.
    /// </summary>
    /// <returns>This as <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/>.</returns>
    public IMapDecoder<T> AsMapDecoder()
    {
        return this;
    }

    /// <summary>
    /// Gets this implementation as an <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>.
    /// </summary>
    /// <returns>Decoder backed by this.</returns>
    public IDecoder<T> AsDecoder()
    {
        return new MapDecoderDecoder<T>(this);
    }

    /// <summary>
    /// Returns an <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> that marks every result of this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> with the given <paramref name="lifecycle"/>.
    /// </summary>
    /// <param name="lifecycle">The lifecycle to apply to the decoded results.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> that applies <paramref name="lifecycle"/> to this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/>.</returns>
    public IMapDecoder<T> WithLifecycle(Lifecycle lifecycle)
    {
        return new LifecycleMapDecoder<T>(this, lifecycle);
    }
}

/// <summary>
/// Simple implementation for <see cref="M:DataFixerUpper.Serialization.Collections.ICompressable.GetCompressor``1(DataFixerUpper.Serialization.DynamicOps.DynamicOps{``0})"/>
/// </summary>
/// <typeparam name="T">The type this <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> deserializes.</typeparam>
public abstract class MapDecoderBase<T> : CompressorHolder, IMapDecoder<T>
{
    /// <inheritdoc/>
    public abstract DataResult<T> Decode<TObject>(DynamicOps<TObject> ops, IMapLike<TObject> input)
        where TObject : notnull;
}