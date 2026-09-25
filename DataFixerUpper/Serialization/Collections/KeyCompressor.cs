using System;
using System.Collections.Generic;
using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Collections;

/// <summary>
/// An immutable collection of indexed, string-convertible keys. When serializing and deserializing maps with compressed keys, a key compressor is used to associate an integer index with each possible key.
/// </summary>
/// <typeparam name="TObject">The type of the serialized form.</typeparam>
/// <seealso cref="M:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1.CompressMaps"/>
public sealed class KeyCompressor<TObject>
    where TObject : notnull
{
    private readonly DynamicOps<TObject> _ops;
    private readonly Dictionary<int, TObject> _decompress = new();
    private readonly Dictionary<TObject, int> _compress = new();
    private readonly Dictionary<string, int> _compressString = new();

    /// <summary>
    /// Count of compressed entries.
    /// </summary>
    public int Count { get; }

    /// <summary>
    /// Constructs a new key compressor for the given keys.
    /// </summary>
    /// <param name="ops">The <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> instance used to deserialize values.</param>
    /// <param name="objects"></param>
    /// <exception cref="NotSupportedException"></exception>
    public KeyCompressor(DynamicOps<TObject> ops, IEnumerable<TObject> objects)
    {
        _ops = ops;
        if (!ops.CompressMaps())
        {
            throw new NotSupportedException($"DynamicOps[{ops}] is not compress maps.");
        }

        foreach (TObject obj in objects)
        {
            if (_compress.ContainsKey(obj))
            {
                continue;
            }

            int next = _compress.Count;
            _compress.Add(obj, next);
            ops.GetStringValue(obj).IfSuccess(s => _compressString.Add(s, next));
            _decompress.Add(next, obj);
        }

        Count = _compress.Count;
    }

    /// <summary>
    /// Returns the key associated with the given key index, or <see langword="null"/> if the key index does not correspond to a key.
    /// </summary>
    /// <param name="key">The key index.</param>
    /// <returns>The key associated with the key index, or <see langword="null"/> if the key index is invalid.</returns>
    public TObject Decompress(int key)
    {
        return _decompress[key];
    }

    /// <summary>
    /// Returns the key index associated with the key the given string represents. If the argument does not represent a valid key, -1 is returned.
    /// </summary>
    /// <param name="key">The string representation of a key.</param>
    /// <returns>The index associated with that key, or -1 if the key does not exist.</returns>
    public int Compress(string key)
    {
        if (_compressString.TryGetValue(key, out int i))
        {
            return i;
        }
        
        return Compress(_ops.CreateString(key));
    }

    /// <summary>
    /// Returns the key index associated with the given key. If the argument is not a valid key, -1 is returned.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The key index associated with the key, or -1 if the key is invalid.</returns>
    public int Compress(TObject key)
    {
        return _compress[key];
    }
}