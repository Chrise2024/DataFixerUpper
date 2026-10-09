using System;
using System.Collections.Generic;
using System.ComponentModel;
using DataFixerUpper.Extensions;

namespace DataFixerUpper.Utils;

/// <summary>
/// Operations for <see cref="T:DataFixerUpper.Utils.Pair`2"/>.
/// </summary>
public static class Pair
{
    /// <summary>
    /// Creates a new <see cref="T:DataFixerUpper.Utils.Pair`2"/> instance containing given value.
    /// </summary>
    /// <param name="first">The first value.</param>
    /// <param name="second">The second value.</param>
    /// <typeparam name="TFirst">First type.</typeparam>
    /// <typeparam name="TSecond">Second type.</typeparam>
    /// <returns>New <see cref="T:DataFixerUpper.Utils.Pair`2"/> instance contains values.</returns>
    public static Pair<TFirst, TSecond> Create<TFirst, TSecond>(TFirst first, TSecond second)
    {
        return new Pair<TFirst, TSecond>(first, second);
    }

    /// <summary>
    /// Creates a new <see cref="T:DataFixerUpper.Utils.Pair`2"/> instance from <see cref="T:System.Collections.Generic.KeyValuePair`2"/>.
    /// </summary>
    /// <param name="pair">The pair.</param>
    /// <typeparam name="TFirst">First type.</typeparam>
    /// <typeparam name="TSecond">Second type.</typeparam>
    /// <returns>New <see cref="T:DataFixerUpper.Utils.Pair`2"/> instance contains values.</returns>
    public static Pair<TFirst, TSecond> FromKeyValuePair<TFirst, TSecond>(KeyValuePair<TFirst, TSecond> pair)
    {
        return Create(pair.Key, pair.Value);
    }

    /// <summary>
    /// Creates a new <see cref="T:DataFixerUpper.Utils.Pair`2"/> instance from <see cref="T:System.ValueTuple`2"/>.
    /// </summary>
    /// <param name="tuple">The tuple.</param>
    /// <typeparam name="TFirst">First type.</typeparam>
    /// <typeparam name="TSecond">Second type.</typeparam>
    /// <returns>New <see cref="T:DataFixerUpper.Utils.Pair`2"/> instance contains values.</returns>
    public static Pair<TFirst, TSecond> FromTuple<TFirst, TSecond>((TFirst, TSecond) tuple)
    {
        return Create(tuple.Item1, tuple.Item2);
    }

    /// <summary>
    /// Function ref to get first value of <see cref="T:DataFixerUpper.Utils.Pair`2"/>.
    /// </summary>
    public static TFirst First<TFirst, TSecond>(Pair<TFirst, TSecond> pair)
    {
        return pair.First;
    }

    /// <summary>
    /// Function ref to get second value of <see cref="T:DataFixerUpper.Utils.Pair`2"/>.
    /// </summary>
    public static TSecond Second<TFirst, TSecond>(Pair<TFirst, TSecond> pair)
    {
        return pair.Second;
    }

    /// <summary>
    /// Creates a new <see cref="T:System.Collections.Generic.KeyValuePair`2"/> instance from <see cref="T:DataFixerUpper.Utils.Pair`2"/>.
    /// </summary>
    /// <param name="pair">The pair.</param>
    /// <typeparam name="TFirst">First type.</typeparam>
    /// <typeparam name="TSecond">Second type.</typeparam>
    /// <returns>New <see cref="T:System.Collections.Generic.KeyValuePair`2"/> instance contains values.</returns>
    public static KeyValuePair<TFirst, TSecond> ToKeyValuePair<TFirst, TSecond>(Pair<TFirst, TSecond> pair)
    {
        return new KeyValuePair<TFirst, TSecond>(pair.First, pair.Second);
    }
}

/// <summary>
/// Defines an immutable value pair.
/// </summary>
/// <typeparam name="TFirst">First type.</typeparam>
/// <typeparam name="TSecond">Second type.</typeparam>
public readonly struct Pair<TFirst, TSecond> : IEquatable<Pair<TFirst, TSecond>>
{
    /// <summary>
    /// The first value.
    /// </summary>
    public TFirst First { get; }

    /// <summary>
    /// The second value.
    /// </summary>
    public TSecond Second { get; }

    /// <summary>
    /// Create <see cref="T:DataFixerUpper.Utils.Pair`2"/> from values.
    /// </summary>
    /// <param name="first">The first value.</param>
    /// <param name="second">The second value.</param>
    public Pair(TFirst first, TSecond second)
    {
        First = first;
        Second = second;
    }

    /// <summary>
    /// Swap this <see cref="T:DataFixerUpper.Utils.Pair`2"/>'s first and second value.
    /// </summary>
    /// <returns></returns>
    public Pair<TSecond, TFirst> Swap()
    {
        return new Pair<TSecond, TFirst>(Second, First);
    }

    /// <summary>
    /// Transform first value with <paramref name="mapper"/>.
    /// </summary>
    /// <typeparam name="TFirst1">The type of the value returned by <paramref name="mapper"/>.</typeparam>
    /// <param name="mapper">Transformer.</param>
    /// <returns>Transformed <see cref="T:DataFixerUpper.Utils.Pair`2"/>.</returns>
    public Pair<TFirst1, TSecond> MapFirst<TFirst1>(Func<TFirst, TFirst1> mapper)
    {
        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        return new Pair<TFirst1, TSecond>(mapper.Apply(First), Second);
    }

    /// <summary>
    /// Transform second value with <paramref name="mapper"/>.
    /// </summary>
    /// <typeparam name="TSecond1">The type of the value returned by <paramref name="mapper"/>.</typeparam>
    /// <param name="mapper">Transformer.</param>
    /// <returns>Transformed <see cref="T:DataFixerUpper.Utils.Pair`2"/>.</returns>
    public Pair<TFirst, TSecond1> MapSecond<TSecond1>(Func<TSecond, TSecond1> mapper)
    {
        if (mapper is null)
        {
            throw new ArgumentNullException(nameof(mapper));
        }

        return new Pair<TFirst, TSecond1>(First, mapper.Apply(Second));
    }

    /// <summary>
    /// Convert this <see cref="T:DataFixerUpper.Utils.Pair`2"/> into <see cref="T:System.Collections.Generic.KeyValuePair`2"/>.
    /// </summary>
    /// <returns>Converted <see cref="T:System.Collections.Generic.KeyValuePair`2"/>.</returns>
    public KeyValuePair<TFirst, TSecond> ToKeyValuePair()
    {
        return new KeyValuePair<TFirst, TSecond>(First, Second);
    }

    /// <summary>
    /// Convert this <see cref="T:DataFixerUpper.Utils.Pair`2"/> into <see cref="T:System.ValueTuple`2"/>.
    /// </summary>
    /// <returns>Converted <see cref="T:System.ValueTuple`2"/>.</returns>
    public (TFirst, TSecond) ToTuple()
    {
        return (First, Second);
    }

    /// <inheritdoc/>
    public bool Equals(Pair<TFirst, TSecond> other)
    {
        return EqualsCore(other);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is Pair<TFirst, TSecond> pair && EqualsCore(pair);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return (First?.GetHashCode() ?? 0) + (Second?.GetHashCode() ?? 0) * 32;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"({First},{Second})";
    }

    /// <summary>Deconstructs the current <see cref="T:DataFixerUpper.Utils.Pair`2" />.</summary>
    /// <param name="first">The first of the current <see cref="T:DataFixerUpper.Utils.Pair`2" />.</param>
    /// <param name="second">The second of the current <see cref="T:DataFixerUpper.Utils.Pair`2" />.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Deconstruct(out TFirst first, out TSecond second)
    {
        first = First;
        second = Second;
    }

    private bool EqualsCore(Pair<TFirst, TSecond> other)
    {
        return EqualityComparer<TFirst>.Default.Equals(First, other.First) && EqualityComparer<TSecond>.Default.Equals(Second, other.Second);
    }
}