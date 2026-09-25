using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Collections;

/// <summary>
/// A <see cref="T:DataFixerUpper.Serialization.Collections.IKeyable"/> that furthermore supports key compression.
/// </summary>
/// <seealso cref="T:DataFixerUpper.Serialization.Collections.KeyCompressor`1"/>
public interface ICompressable : IKeyable
{
    /// <summary>
    /// Returns the <see cref="T:DataFixerUpper.Serialization.Collections.KeyCompressor`1"/> used to compress keys in this object's key set for the given serialized form.
    /// </summary>
    /// <param name="ops">The <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> instance used to serialize values.</param>
    /// <typeparam name="TObject">The type this class serializes to and deserializes from.</typeparam>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Collections.KeyCompressor`1"/> for this object's keys.</returns>
    KeyCompressor<TObject> GetCompressor<TObject>(DynamicOps<TObject> ops)
        where TObject : notnull;
}