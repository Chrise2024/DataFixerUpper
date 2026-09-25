using DataFixerUpper.Serialization.Codecs.Impl;

namespace DataFixerUpper.Serialization.Codecs;

public partial interface IEncoder<T>
{
    /// <summary>
    /// Returns a <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> that encodes objects in a record under a field with the given name. 
    /// </summary>
    /// <param name="name">Field name.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Codecs.IMapEncoder`1"/> that performs the same encoding as this <see cref="T:DataFixerUpper.Serialization.Codecs.IEncoder`1"/>, but places the serialized value in a record under the given field.</returns>
    public IMapEncoder<T> Field(string name)
    {
        return new FieldEncoder<T>(name, this);
    }
}