using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataFixerUpper.Serialization.DynamicOps;

namespace DataFixerUpper.Serialization.Collections;

/// <summary>
/// An unmodifiable store for serialized key-value pairs. This interface can be used when access to and iteration over serialized key-value pairs.
/// </summary>
/// <typeparam name="TObject">The type of the serialized form.</typeparam>
public abstract class MapLike<TObject> : IEnumerable<KeyValuePair<TObject, TObject>>
    where TObject : notnull
{
    /// <summary>
    /// Empty instance.
    /// </summary>
    public static MapLike<TObject> Empty => new EmptyImpl<TObject>();

    /// <summary>
    /// Gets the number of key/value pairs contained in the <see cref="T:DataFixerUpper.Serialization.Collections.MapLike`1"/>.
    /// </summary>
    public abstract int Count { get; }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key, or <see langword="null"/> if pecified key is not found.</returns>
#nullable enable
    public abstract TObject? this[TObject key] { get; }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key, or <see langword="null"/> if pecified key is not found.</returns>
    public abstract TObject? this[string key] { get; }
#nullable restore

    /// <inheritdoc/>
    public abstract IEnumerator<KeyValuePair<TObject, TObject>> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Create a compressed <see cref="T:DataFixerUpper.Serialization.Collections.MapLike`1"/>.
    /// </summary>
    /// <param name="values">Values of compressed map.</param>
    /// <param name="compressor">Compressor to compress values</param>
    /// <returns>Compressed <see cref="T:DataFixerUpper.Serialization.Collections.MapLike`1"/>.</returns>
    public static MapLike<TObject> ForCompressed(IList<TObject> values, KeyCompressor<TObject> compressor)
    {
        return new CompressedImpl<TObject>(values, compressor);
    }

    /// <summary>
    /// Creates a <see cref="T:DataFixerUpper.Serialization.Collections.MapLike`1"/> containing the entries of the given dictionary.
    /// </summary>
    /// <param name="map">The dictionary to wrap.</param>
    /// <param name="ops">A <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> instance defining the serialized form.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.Collections.MapLike`1"/> containing the entries of the given dictionary.</returns>
    public static MapLike<TObject> ForMap(IDictionary<TObject, TObject> map, DynamicOps<TObject> ops)
    {
        if (map.Count == 0)
        {
            return Empty;
        }

        return new DictImpl<TObject>(ops, map);
    }
}

file sealed class CompressedImpl<TObject>(IList<TObject> values, KeyCompressor<TObject> compressor) : MapLike<TObject>
    where TObject : notnull
{
    public override int Count => values.Count;
    public override TObject this[TObject key] => values[compressor.Compress(key)];

    public override TObject this[string key] => values[compressor.Compress(key)];

    public override IEnumerator<KeyValuePair<TObject, TObject>> GetEnumerator()
    {
        return values.Select((t, i) => new KeyValuePair<TObject, TObject>(compressor.Decompress(i), t)).GetEnumerator();
    }

    public override string ToString()
    {
        return $"MapLike[{values}][Compressed]";
    }
}

file sealed class DictImpl<TObject>(DynamicOps<TObject> ops, IDictionary<TObject, TObject> map) : MapLike<TObject>
    where TObject : notnull
{
    public override int Count => map.Count;
    public override TObject this[TObject key] => map.TryGetValue(key, out TObject value) ? value : default;

    public override TObject this[string key] => this[ops.CreateString(key)];

    public override IEnumerator<KeyValuePair<TObject, TObject>> GetEnumerator()
    {
        return map.GetEnumerator();
    }

    public override string ToString()
    {
        return $"MapLike[{map}]";
    }
}

file sealed class EmptyImpl<TObject> : MapLike<TObject>
    where TObject : notnull
{
    public override int Count => 0;
    public override TObject this[TObject key] => default;

    public override TObject this[string key] => default;

    public override IEnumerator<KeyValuePair<TObject, TObject>> GetEnumerator()
    {
        yield break;
    }

    public override string ToString()
    {
        return "EmptyMapLike";
    }
}