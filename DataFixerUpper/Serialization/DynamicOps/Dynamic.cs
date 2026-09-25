using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using DataFixerUpper.Extensions;
using DataFixerUpper.Serialization.Codecs;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Serialization.DynamicOps;

/// <summary>
/// Universal <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/>
/// </summary>
public interface IDynamic
{
    /// <summary>
    /// Converts to the format of <paramref name="otherOps"/>.
    /// </summary>
    /// <param name="otherOps">The ops of the format to convert to.</param>
    /// <typeparam name="TOther">The type <paramref name="otherOps"/> serializes to and deserializes from.</typeparam>
    /// <returns>The converted <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/>.</returns>
    public Dynamic<TOther> Convert<TOther>(DynamicOps<TOther> otherOps)
        where TOther : notnull;

    /// <summary>
    /// Gets a value indicating whether the <see cref="T:DataFixerUpper.Serialization.DynamicOps.IDynamic"/> is the empty value of this format.
    /// </summary>
    /// <returns>A <see langword="true"/> if is empty; otherwise, <see langword="false"/>.</returns>
    public bool IsEmpty();
}

/// <summary>
/// Static methods for working with <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> instances.
/// </summary>
public static class Dynamic
{
    /// <summary>
    /// Converts <paramref name="value"/> from the format of <paramref name="ops1"/> to the format of <paramref name="ops2"/>.
    /// </summary>
    /// <param name="ops1">The ops of the format <paramref name="value"/> is in.</param>
    /// <param name="ops2">The ops of the format to convert to.</param>
    /// <param name="value">The serialized value to convert.</param>
    /// <typeparam name="T1">The type <paramref name="ops1"/> serializes to and deserializes from.</typeparam>
    /// <typeparam name="T2">The type <paramref name="ops2"/> serializes to and deserializes from.</typeparam>
    /// <returns>The converted value, or <paramref name="value"/> itself if both formats use the same type.</returns>
    public static T2? Convert<T1, T2>(DynamicOps<T1> ops1, DynamicOps<T2> ops2, T1? value)
        where T1 : notnull
        where T2 : notnull
    {
        if (ops1.IsEmpty(value))
        {
            return ops2.Empty();
        }
        
        if (typeof(T1) == typeof(T2))
        {
            return Unsafe.As<T1?, T2?>(ref value);
        }

        return ops1.ConvertTo(ops2, value);
    }

    /// <summary>
    /// Copies the entry stored under <paramref name="srcKey"/> in <paramref name="src"/> to <paramref name="dstKey"/> in <paramref name="dst"/>, applying <paramref name="fixer"/> to it.
    /// </summary>
    /// <param name="src">The dynamic to copy from.</param>
    /// <param name="srcKey">The key of the entry to copy.</param>
    /// <param name="dst">The dynamic to copy to.</param>
    /// <param name="dstKey">The key to store the copied entry under.</param>
    /// <param name="fixer">The function that transforms the copied value.</param>
    /// <typeparam name="TObject">The type the dynamics serialize to and deserialize from.</typeparam>
    /// <returns><paramref name="dst"/> with the copied entry added, or <paramref name="dst"/> itself if <paramref name="src"/> has no entry under <paramref name="srcKey"/>.</returns>
    public static Dynamic<TObject> CopyToAndFix<TObject>(Dynamic<TObject> src, string srcKey, Dynamic<TObject> dst, string dstKey, UnaryOperation<Dynamic<TObject>> fixer)
        where TObject : notnull
    {
        Optional<Dynamic<TObject>> value = src.Get(srcKey).Result;
        return value.HasValue ? dst.Set(dstKey, value.Select(fixer.Apply)) : dst;
    }

    /// <summary>
    /// Copies the entry stored under <paramref name="srcKey"/> in <paramref name="src"/> to <paramref name="dstKey"/> in <paramref name="dst"/> without modifying it.
    /// </summary>
    /// <param name="src">The dynamic to copy from.</param>
    /// <param name="srcKey">The key of the entry to copy.</param>
    /// <param name="dst">The dynamic to copy to.</param>
    /// <param name="dstKey">The key to store the copied entry under.</param>
    /// <typeparam name="TObject">The type the dynamics serialize to and deserialize from.</typeparam>
    /// <returns><paramref name="dst"/> with the copied entry added, or <paramref name="dst"/> itself if <paramref name="src"/> has no entry under <paramref name="srcKey"/>.</returns>
    public static Dynamic<TObject> CopyToAndFix<TObject>(Dynamic<TObject> src, string srcKey, Dynamic<TObject> dst, string dstKey)
        where TObject : notnull
    {
        return CopyToAndFix(src, srcKey, dst, dstKey, Functions.Identity);
    }
}

/// <summary>
/// A <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicLike`1"/> that wraps a serialized value together with the ops that produced it.
/// </summary>
/// <remarks>
/// Carrying the ops along with the value allows a serialized value to be read, modified and written back without passing the <see cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicOps`1"/> around separately.
/// </remarks>
/// <param name="ops">The ops used to read and modify the wrapped value.</param>
/// <param name="wrapped">The serialized value to wrap.</param>
/// <typeparam name="TObject">The type this class serializes to and deserializes from, for example <see cref="T:System.Text.Json.Nodes.JsonNode"/>.</typeparam>
/// <seealso cref="T:DataFixerUpper.Serialization.DynamicOps.DynamicLike`1"/>
/// <seealso cref="T:DataFixerUpper.Serialization.DynamicOps.OptionalDynamic`1"/>
public sealed class Dynamic<TObject>(DynamicOps<TObject> ops, TObject? wrapped) : DynamicLike<TObject>(ops), IDynamic, IEquatable<Dynamic<TObject>>
    where TObject : notnull

{
    /// <summary>
    /// Gets the wrapped value, or the empty value of the ops of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> if no value was wrapped.
    /// </summary>
    public TObject? Value => wrapped ?? Ops.Empty();

    /// <summary>
    /// Initializes a new instance that wraps the empty value of <paramref name="ops"/>.
    /// </summary>
    /// <param name="ops">The ops used to read and modify the value.</param>
    public Dynamic(DynamicOps<TObject> ops) : this(ops, ops.Empty()) { }

    /// <summary>
    /// Returns a new dynamic that wraps the result of <paramref name="mapper"/> applied to the value of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/>.
    /// </summary>
    /// <param name="mapper">The function that produces the new value.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the result of <paramref name="mapper"/>.</returns>
    public Dynamic<TObject> Map(Func<TObject?, TObject?> mapper)
    {
        return new Dynamic<TObject>(Ops, mapper.Apply(Value));
    }

    /// <summary>
    /// Returns the result of merging the value of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> and the value of <paramref name="other"/> into a list.
    /// </summary>
    /// <param name="other">The dynamic whose value is merged into this one.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DynamicOps.OptionalDynamic`1"/> containing the merged list, or an error if the values cannot be merged.</returns>
    /// <remarks>
    /// The values are merged with <c>MergeToList</c>.
    /// </remarks>
    public OptionalDynamic<TObject> Combine(Dynamic<TObject> other)
    {
        DataResult<TObject> combined = Ops.MergeToList(Value, other.Value);
        return new OptionalDynamic<TObject>(Ops, combined.Map(m => new Dynamic<TObject>(Ops, m)));
    }

    /// <summary>
    /// Returns the result of merging <paramref name="key"/> and <paramref name="value"/> into the map of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/>.
    /// </summary>
    /// <param name="key">The key to add, which must not be empty.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>An <see cref="T:DataFixerUpper.Serialization.DynamicOps.OptionalDynamic`1"/> containing the merged map, or an error if the values cannot be merged.</returns>
    /// <exception cref="T:System.ArgumentNullException">If <paramref name="key"/> is empty and this format represents empty values with <see langword="null"/>.</exception>
    /// <remarks>
    /// The values are merged with <c>MergeToMap</c>.
    /// </remarks>
    public OptionalDynamic<TObject> Combine(Dynamic<TObject> key, Dynamic<TObject> value)
    {
        ArgumentNullException.ThrowIfNull(key.Value);
        DataResult<TObject> combined = Ops.MergeToMap(Value, key.Value, value.Value);
        return new OptionalDynamic<TObject>(Ops, combined.Map(m => new Dynamic<TObject>(Ops, m)));
    }

    /// <summary>
    /// Reads the value of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> as a map.
    /// </summary>
    /// <returns>A <see cref="T:DataFixerUpper.Serialization.DataResult`1"/> containing the entries of the map, or an error if the value is not a map.</returns>
    public DataResult<ImmutableDictionary<Dynamic<TObject>, Dynamic<TObject>>> GetMapValues()
    {
        return Ops.GetMapValues(Value).Map(map => map.ToImmutableDictionary(entry => new Dynamic<TObject>(Ops, entry.Key), entry => new Dynamic<TObject>(Ops, entry.Value)));
    }

    /// <summary>
    /// Returns a copy of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> with every entry of its map transformed by <paramref name="updater"/>.
    /// </summary>
    /// <param name="updater">The function that transforms an entry.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the updated map, or this dynamic itself if the value is not a map.</returns>
    public Dynamic<TObject> UpdateMapValues(Func<KeyValuePair<TObject, TObject?>, KeyValuePair<TObject, TObject?>> updater)
    {
        return Ops.GetMapValues(Value).Map(map => new Dynamic<TObject>(Ops, Ops.CreateMap(map.Select(updater)))).GetResultOrDefault(this);
    }

    /// <summary>
    /// Returns a copy of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> without the entry stored under <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The key of the entry to remove.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the modified map, or a dynamic wrapping the unchanged value if the value is not a map or has no entry under <paramref name="key"/>.</returns>
    public Dynamic<TObject> Remove(string key)
    {
        return Map(v => Ops.Remove(v, key));
    }

    /// <summary>
    /// Returns a copy of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> with <paramref name="key"/> set to <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to set <paramref name="key"/> to, or <see langword="null"/> to leave this dynamic unchanged.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the modified map, or this dynamic itself if <paramref name="value"/> is <see langword="null"/>.</returns>
    public Dynamic<TObject> Set(string key, Dynamic<TObject>? value)
    {
        if (value is null)
        {
            return this;
        }
        
        return Map(v => Ops.Set(v, key, value.Value));
    }

    /// <summary>
    /// Returns a copy of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> with <paramref name="key"/> set to the value of <paramref name="value"/>.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to set <paramref name="key"/> to, or <see cref="P:DataFixerUpper.Utils.Optional`1.Empty"/> to leave this dynamic unchanged.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the modified map, or this dynamic itself if <paramref name="value"/> is empty.</returns>
    public Dynamic<TObject> Set(string key, Optional<Dynamic<TObject>> value)
    {
        return value.HasValue ? Set(key, value.Value) : this;
    }

    /// <summary>
    /// Returns a copy of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> with the entry stored under <paramref name="key"/> replaced by the result of <paramref name="updater"/>.
    /// </summary>
    /// <param name="key">The key of the entry to update.</param>
    /// <param name="updater">The function that produces the new entry from the current one.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the modified map, or a dynamic wrapping the unchanged value if the value is not a map or has no entry under <paramref name="key"/>.</returns>
    public Dynamic<TObject> Update(TObject key, Func<Dynamic<TObject>, Dynamic<TObject>> updater)
    {
        return Map(i => Ops.Update(i, key, v => updater.Apply(new Dynamic<TObject>(Ops, v)).Value!));
    }

    /// <summary>
    /// Returns a copy of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> with the raw value stored under <paramref name="key"/> replaced by the result of <paramref name="updater"/>.
    /// </summary>
    /// <param name="key">The key of the entry to update.</param>
    /// <param name="updater">The function that produces the new raw value from the current one.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the modified map, or a dynamic wrapping the unchanged value if the value is not a map or has no entry under <paramref name="key"/>.</returns>
    public Dynamic<TObject> Update(TObject key, Func<TObject, TObject> updater)
    {
        return Map(i => Ops.Update(i, key, updater));
    }

    /// <summary>
    /// Returns a copy of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> with the entry stored under <paramref name="key"/> removed and <paramref name="value"/> stored under <paramref name="newKey"/>.
    /// </summary>
    /// <param name="key">The key of the entry to remove.</param>
    /// <param name="newKey">The key to store the replacement value under.</param>
    /// <param name="value">The value to store under <paramref name="newKey"/>, or <see cref="P:DataFixerUpper.Utils.Optional`1.Empty"/> to store nothing.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the modified map.</returns>
    public Dynamic<TObject> Replace(string key, string newKey, Optional<Dynamic<TObject>> value)
    {
        return Remove(key).Set(newKey, value);
    }

    /// <summary>
    /// Returns a copy of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> with the entry stored under <paramref name="key"/> renamed to <paramref name="newKey"/>, applying <paramref name="fixer"/> to it.
    /// </summary>
    /// <param name="key">The key of the entry to rename.</param>
    /// <param name="newKey">The new key of the entry.</param>
    /// <param name="fixer">The function that transforms the renamed value.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the modified map, or a copy without the entry if it was not present.</returns>
    public Dynamic<TObject> RenameAndFix(string key, string newKey, UnaryOperation<Dynamic<TObject>> fixer)
    {
        Optional<Dynamic<TObject>> result = Get(key).Result;
        return Remove(key).Set(newKey, result.Select(fixer.Apply));
    }

    /// <summary>
    /// Returns a copy of this <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> with the entry stored under <paramref name="key"/> renamed to <paramref name="newKey"/> without modifying it.
    /// </summary>
    /// <param name="key">The key of the entry to rename.</param>
    /// <param name="newKey">The new key of the entry.</param>
    /// <returns>New <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> wrapping the modified map, or a copy without the entry if it was not present.</returns>
    public Dynamic<TObject> Rename(string key, string newKey)
    {
        return RenameAndFix(key, newKey, Functions.Identity);
    }

    /// <summary>
    /// Invokes <paramref name="action"/> with this dynamic.
    /// </summary>
    /// <param name="action">The function to invoke with this dynamic.</param>
    /// <typeparam name="T1">The type of the value returned by <paramref name="action"/>.</typeparam>
    /// <returns>The result of <paramref name="action"/>.</returns>
    public T1 Into<T1>(Func<Dynamic<TObject>, T1> action)
    {
        return action.Apply(this);
    }

    /// <inheritdoc/>
    public override OptionalDynamic<TObject> Get(string key)
    {
        DataResult<TObject> valueResult = Ops.Get(Value, Ops.CreateString(key));
        DataResult<Dynamic<TObject>> dynamicValue;
        if (valueResult.TryGetResult(out TObject? result))
        {
            dynamicValue = DataResult.CreateSuccess(new Dynamic<TObject>(Ops, result));
        }
        else
        {
            dynamicValue = DataResult.CreateError<Dynamic<TObject>>($"key missing: {key} in {Value}");
        }

        return new OptionalDynamic<TObject>(Ops, dynamicValue);
    }

    /// <inheritdoc/>
    public override DataResult<TObject> Get(TObject key)
    {
        return Ops.Get(Value, key);
    }

    /// <inheritdoc/>
    public override DataResult<(TResult, TObject?)> Decode<TResult>(IDecoder<TResult> decoder)
    {
        return decoder.Decode(Ops, Value);
    }

    /// <inheritdoc/>
    public override DataResult<TNumber> AsNumber<TNumber>()
    {
        return Ops.GetNumberValue<TNumber>(Value);
    }

    /// <inheritdoc/>
    public override DataResult<string> AsString()
    {
        return Ops.GetStringValue(Value);
    }

    /// <inheritdoc/>
    public override DataResult<bool> AsBool()
    {
        return Ops.GetBoolValue(Value);
    }

    /// <inheritdoc/>
    public override DataResult<IEnumerable<Dynamic<TObject>>> AsListValues()
    {
        return Ops.GetListValues(Value).Map(list => list.Select(item => new Dynamic<TObject>(Ops, item)));
    }

    /// <inheritdoc/>
    public override DataResult<IEnumerable<KeyValuePair<Dynamic<TObject>, Dynamic<TObject>>>> AsMapEntries()
    {
        return Ops.GetMapValues(Value).Map(map => map.Select(entry => KeyValuePair.Create(new Dynamic<TObject>(Ops, entry.Key), new Dynamic<TObject>(Ops, entry.Value))));
    }

    /// <inheritdoc/>
    public Dynamic<TOther> Convert<TOther>(DynamicOps<TOther> otherOps)
        where TOther : notnull
    {
        return new Dynamic<TOther>(otherOps, Dynamic.Convert(Ops, otherOps, Value));
    }

    /// <inheritdoc/>
    public bool IsEmpty()
    {
        return Ops.IsEmpty(Value);
    }

    /// <summary>
    /// Indicates whether this dynamic and <paramref name="other"/> wrap equal values with equal ops.
    /// </summary>
    /// <param name="other">The dynamic to compare with.</param>
    /// <returns>A <see langword="true"/> if <paramref name="other"/> wraps the same value with the same ops; otherwise, <see langword="false"/>.</returns>
    public bool Equals(Dynamic<TObject>? other)
    {
        return other is not null && EqualsCore(other);
    }

    /// <summary>
    /// Indicates whether the given object is a dynamic that wraps the same value with the same ops as this dynamic.
    /// </summary>
    /// <param name="obj">The object to compare with.</param>
    /// <returns>A <see langword="true"/> if <paramref name="obj"/> is a <see cref="T:DataFixerUpper.Serialization.DynamicOps.Dynamic`1"/> that wraps the same value with the same ops; otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is Dynamic<TObject> other && EqualsCore(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        HashCode hash = new();
        
        hash.Add(Ops);
        hash.Add(Value);
        
        return hash.ToHashCode();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{Ops}[{Value}]";
    }

    private bool EqualsCore(Dynamic<TObject> other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        
        if (Value is null && other.Value is null)
        {
            return true;
        }

        return Ops.Equals(other.Ops) && (Value?.Equals(other.Value) ?? false);
    }
}
