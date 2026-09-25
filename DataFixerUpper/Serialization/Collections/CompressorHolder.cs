using System.Collections;
using System.Collections.Generic;
using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Collections;

/// <summary>
/// Abstract base class for Compressable objects. This class provides a default implementation of the method <see cref="M:DataFixerUpper.Serialization.Collections.ICompressable.GetCompressor``1(DataFixerUpper.Serialization.DynamicOps.DynamicOps{``0})"/>.
/// </summary>
public abstract class CompressorHolder : ICompressable
{
    private readonly Hashtable _compressors = new();

    /// <inheritdoc/>
    public KeyCompressor<TObject> GetCompressor<TObject>(DynamicOps<TObject> ops)
        where TObject : notnull
    {
        KeyCompressor<TObject>? compressor = (KeyCompressor<TObject>?) _compressors[ops];

        if (compressor is not null)
        {
            return compressor;
        }

        compressor = new KeyCompressor<TObject>(ops, GetKeys(ops));
        _compressors[ops] = compressor;
        return compressor;
    }

    /// <summary>
    /// Get keys to compress of this holder.
    /// </summary>
    /// <param name="ops">The <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> instance used to serialize values.</param>
    /// <typeparam name="TObject">The type this class serializes to and deserializes from.</typeparam>
    /// <returns>Keys which are compressed.</returns>
    public abstract IEnumerable<TObject> GetKeys<TObject>(DynamicOps<TObject> ops)
        where TObject : notnull;
}