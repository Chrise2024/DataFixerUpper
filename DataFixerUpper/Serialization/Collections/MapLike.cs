using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Collections;

/// <summary>
/// An unmodifiable store for serialized key-value pairs. This interface can be used when access to and iteration over serialized key-value pairs.
/// </summary>
/// <typeparam name="TObject">The type of the serialized form.</typeparam>
public interface IMapLike<TObject> : IEnumerable<KeyValuePair<TObject, TObject?>>
    where TObject : notnull
{
    /// <summary>
    /// Empty instance.
    /// </summary>
    public static IMapLike<TObject> Empty => new EmptyImpl<TObject>();
    
    /// <summary>
    /// Gets the number of key/value pairs contained in the <see cref="T:DataFixerUpper.Serialization.Collections.IMapLike`1"/>.
    /// </summary>
    public int Count { get; }
    
    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key, or <see langword="null"/> if pecified key is not found.</returns>
    public TObject? this[TObject key] { get; }
    
    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key, or <see langword="null"/> if pecified key is not found.</returns>
    public TObject? this[string key] { get; }

    /// <summary>
    /// Create a compressed <see cref="T:DataFixerUpper.Serialization.Collections.IMapLike`1"/>.
    /// </summary>
    /// <param name="values">Values of compressed map.</param>
    /// <param name="compressor">Compressor to compress values</param>
    /// <returns>Compressed <see cref="T:DataFixerUpper.Serialization.Collections.IMapLike`1"/>.</returns>
    public static IMapLike<TObject> ForCompressed(IList<TObject?> values, KeyCompressor<TObject> compressor)
    {
        return new CompressedImpl<TObject>(values, compressor);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Collections.IMapLike`1"/> containing the entries of the given dictionary.
    /// </summary>
    /// <param name="map">The dictionary to wrap.</param>
    /// <param name="ops">A <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> instance defining the serialized form.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Collections.IMapLike`1"/> containing the entries of the given dictionary.</returns>
    public static IMapLike<TObject> ForMap(IDictionary<TObject, TObject?> map, DynamicOps<TObject> ops)
    {
        if (map.Count == 0)
        {
            return Empty;
        }
        
        return new DictImpl<TObject>(ops, map);
    }
}

file sealed class CompressedImpl<TObject>(IList<TObject?> values, KeyCompressor<TObject> compressor) : IMapLike<TObject>
    where TObject : notnull
{
    public int Count => values.Count;
    public TObject? this[TObject key] => values[compressor.Compress(key)];

    public TObject? this[string key] => values[compressor.Compress(key)];

    public IEnumerator<KeyValuePair<TObject, TObject?>> GetEnumerator()
    {
        return values.Select((t, i) => new KeyValuePair<TObject, TObject?>(compressor.Decompress(i), t)).GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override string ToString()
    {
        return $"MapLike[{values}][Compressed]";
    }
}

file sealed class DictImpl<TObject>(DynamicOps<TObject> ops, IDictionary<TObject, TObject?> map) : IMapLike<TObject>
    where TObject : notnull
{
    public int Count => map.Count;
    public TObject? this[TObject key] => map.TryGetValue(key, out TObject? value) ? value : default;

    public TObject? this[string key] => this[ops.CreateString(key)];

    public IEnumerator<KeyValuePair<TObject, TObject?>> GetEnumerator()
    {
        return map.GetEnumerator();
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) map).GetEnumerator();
    }

    public override string ToString()
    {
        return $"MapLike[{map}]";
    }
}

file sealed class EmptyImpl<TObject> : IMapLike<TObject>
    where TObject : notnull
{
    public int Count => 0;
    public TObject? this[TObject key] => default;

    public TObject? this[string key] => default;

    public IEnumerator<KeyValuePair<TObject, TObject?>> GetEnumerator()
    {
        yield break;
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        yield break;
    }

    public override string ToString()
    {
        return "EmptyMapLike";
    }
}