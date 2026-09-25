using DataFixerUpper.Serialization.Codecs.Impl;
using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Codecs;

/// <summary>
/// Serializes (encodes) objects of a given type to a serialized form.
/// </summary>
/// <typeparam name="T">The type this <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> serializes.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>
public partial interface IEncoder<T>
{
    /// <summary>
    /// Encodes the given <paramref name="input"/> into the form defined by <paramref name="ops"/>, merged into <paramref name="prefix"/> if it is not <see langword="null"/>.
    /// </summary>
    /// <param name="input">The value to encode.</param>
    /// <param name="ops">The ops used to create the encoded value.</param>
    /// <param name="prefix">The value to merge the encoded value into, or <see langword="null"/> to produce a standalone value.</param>
    /// <typeparam name="TObject">The type of the encoded value.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the encoded value, or an error if the value cannot be encoded.</returns>
    DataResult<TObject> Encode<TObject>(T input, DynamicOps<TObject> ops, TObject? prefix)
        where TObject : notnull;

    /// <summary>
    /// Returns an <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> that marks every result of this <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> with the given <paramref name="lifecycle"/>.
    /// </summary>
    /// <param name="lifecycle">The lifecycle to apply to the encoded results.</param>
    /// <returns>Encoder that applies <paramref name="lifecycle"/> to this <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/>.</returns>
    public IEncoder<T> WithLifecycle(Lifecycle lifecycle)
    {
        return new LifecycleEncoder<T>(this, lifecycle);
    }

    /// <summary>
    /// Gets this <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> implementation viewed as an <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/>.
    /// </summary>
    /// <returns>This as <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/>.</returns>
    public IEncoder<T> AsEncoder()
    {
        return this;
    }

    /// <summary>
    /// A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> that performs no serialization.
    /// Its <see cref="M:DataFixerUpper.Serialization.Codecs.IMapEncoder`1.Encode``1(`0,DataFixerUpper.Serialization.DynamicOps.DynamicOps{``0},DataFixerUpper.Serialization.Collections.Builder.IRecordBuilder{``0})"/> returns the prefix unchanged and has an empty keys.
    /// </summary>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> that performs no serialization.</returns>
    public static IMapEncoder<T> Empty()
    {
        return new EmptyMapEncoder<T>();
    }

    /// <summary>
    /// A <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> that performs no deserialization. Its <see cref="M:DataFixerUpper.Serialization.Codecs.IEncoder`1.Encode``1(`0,DataFixerUpper.Serialization.DynamicOps.DynamicOps{``0},``0)"/> always returns the given error.
    /// </summary>
    /// <param name="message">The error the returned encoder should produce.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/> that performs no deserialization.</returns>
    public static IEncoder<T> Error(string message)
    {
        return new ErrorEncoder<T>(message);
    }
}