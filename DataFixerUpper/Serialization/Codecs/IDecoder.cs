using DataFixerUpper.Serialization.Codecs.Impl;
using DataFixerUpper.Serialization.DynamicOps;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs;

/// <summary>
/// Deserializes (decodes) objects of a given type from a serialized form.
/// </summary>
/// <typeparam name="T">The type this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> deserializes.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.Codec`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/>
public partial interface IDecoder<T>
{
    /// <summary>
    /// Decodes a value of type <typeparamref name="T"/> from the given <paramref name="input"/>.
    /// </summary>
    /// <param name="ops">The ops that define the form of <paramref name="input"/>.</param>
    /// <param name="input">The value to decode.</param>
    /// <typeparam name="TObject">The type of the decoded value.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the decoded value together with the remaining input, or an error if the value cannot be decoded.</returns>
    DataResult<(T, TObject?)> Decode<TObject>(DynamicOps<TObject> ops, TObject? input)
        where TObject : notnull;

    /// <summary>
    /// Returns an <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that marks every result of this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> with the given <paramref name="lifecycle"/>.
    /// </summary>
    /// <param name="lifecycle">The lifecycle to apply to the decoded results.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that applies <paramref name="lifecycle"/> to this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>.</returns>
    public IDecoder<T> WithLifecycle(Lifecycle lifecycle)
    {
        return new LifecycleDecoder<T>(this, lifecycle);
    }

    /// <summary>
    /// Gets this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> implementation viewed as an <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>.
    /// </summary>
    /// <returns>This as <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>.</returns>
    public IDecoder<T> AsDecoder()
    {
        return this;
    }

    /// <summary>
    /// A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> that performs no deserialization and always returns the given value.
    /// </summary>
    /// <remarks>
    ///  Its <see cref="M:DataFixerUpper.Serialization.Codecs.IMapDecoder`1.Decode``1(DataFixerUpper.Serialization.DynamicOps.DynamicOps{``0},DataFixerUpper.Serialization.Collections.IMapLike{``0})"/> method always returns the instance and its keys is always empty.
    /// </remarks>
    /// <param name="value">The value to return from the decoder.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> that always returns the given value.</returns>
    public static IMapDecoder<T> Unit(T value)
    {
        return new UnitMapDecoder<T>(value);
    }

    /// <summary>
    /// A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> that performs no deserialization and always returns the given value.
    /// </summary>
    /// <remarks>
    ///  Its <see cref="M:DataFixerUpper.Serialization.Codecs.IMapDecoder`1.Decode``1(DataFixerUpper.Serialization.DynamicOps.DynamicOps{``0},DataFixerUpper.Serialization.Collections.IMapLike{``0})"/> method always returns the instance and its keys is always empty.
    /// </remarks>
    /// <param name="value">The value to return from the decoder.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> that always returns the given value.</returns>
    public static IMapDecoder<T> Unit(Provider<T> value)
    {
        return new UnitMapDecoder<T>(value);
    }

    /// <summary>
    /// A <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that performs no deserialization. Its <see cref="M:DataFixerUpper.Serialization.Codecs.IDecoder`1.Decode``1(DataFixerUpper.Serialization.DynamicOps.DynamicOps{``0},``0)"/> always returns the given error.
    /// </summary>
    /// <param name="message">The error the returned decoder should produce.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that performs no deserialization.</returns>
    public static IDecoder<T> Error(string message)
    {
        return new ErrorDecoder<T>(message);
    }
}