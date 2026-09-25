using DataFixerUpper.Serialization.Codecs.Impl;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.Codecs;

public partial interface IDecoder<T>
{
    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> that decodes objects in a record under a field with the given name. 
    /// </summary>
    /// <param name="name">Field name.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapDecoder`1"/> that performs the same decoding as this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/>, but takes the serialized value from a record under the given field.</returns>
    public IMapDecoder<T> Field(string name)
    {
        return new FieldDecoder<T>(name, this);
    }

    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that returns the partial decoded result in this <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> returns an error.
    /// </summary>
    /// <param name="onError">A callback to run on error. It is passed the error string.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IDecoder`1"/> that promotes any partial result to a successful decoded result.</returns>
    public IDecoder<T> PromptPartial(Consumer<string> onError)
    {
        return new PromptPartialDecoder<T>(this, onError);
    }
}