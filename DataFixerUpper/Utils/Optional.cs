using System;
using System.Diagnostics.CodeAnalysis;
using DataFixerUpper.Extensions;

namespace DataFixerUpper.Utils;

/// <summary>
/// For create <see cref="T:DataFixerUpper.Utils.Optional`1"/>.
/// </summary>
public static class Optional
{
    /// <summary>
    /// Create <see cref="T:DataFixerUpper.Utils.Optional`1"/> from a nullable value.
    /// </summary>
    /// <param name="value">Value to wrap.</param>
    /// <returns>A <see cref="T:DataFixerUpper.Utils.Optional`1"/> instance.</returns>
    public static Optional<T> Create<T>(T? value)
    {
        return value is null ? Optional<T>.Empty : new Optional<T>(value);
    }

    // public static Optional<T> Create<T>(T? value) where T : struct
    // {
    //     return value.HasValue ? new Optional<T>(value.Value) : Optional<T>.Empty;
    // }

    /// <summary>
    /// Create an empty <see cref="T:DataFixerUpper.Utils.Optional`1"/>.
    /// </summary>
    /// <returns>Empty <see cref="T:DataFixerUpper.Utils.Optional`1"/>.</returns>
    public static Optional<T> Create<T>()
    {
        return Optional<T>.Empty;
    }
}

// #nullable disable

/// <summary>Represents universal wrap for nullables, both ref type and value type.</summary>
/// <typeparam name="T">The underlying type of the <see cref="T:DataFixerUpper.Utils.Optional`1"/> generic type.</typeparam>
public readonly struct Optional<T> : IEquatable<Optional<T>>
{
    /// <summary>
    /// Empty instance.
    /// </summary>
    public static Optional<T> Empty => new();

    private readonly T? _value;

    /// <summary>
    /// Unwrap value. Throws exception if not has value.
    /// </summary>
    /// <exception cref="InvalidOperationException">If <see cref="P:HasValue"/> is false.</exception>
    public T Value => GetOrThrow();

    /// <summary>
    /// Gets a value indicating whether the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> has value or not.
    /// </summary>
    /// <returns>
    /// <see langword="true"/>If the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object has a value; <see langword="false"/> if the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object has no value.
    /// </returns>
    [MemberNotNullWhen(true, nameof(_value))]
    public bool HasValue { get; }

    internal Optional(T value)
    {
        //ArgumentNullException.ThrowIfNull(value);
        _value = value;
        HasValue = value is not null;
    }


    /// <summary>
    /// Gets the value of the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object if it has value or else throw.
    /// </summary>
    /// <returns>The value of the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="true"/>. An exception is thrown if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">If <see cref="P:HasValue"/> is false.</exception>
    public T GetOrThrow()
    {
        return HasValue ? _value : throw new InvalidOperationException("No value present");
    }

    /// <summary>
    /// Gets the value of the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object if it has value or else throw custom exception.
    /// </summary>
    /// <returns>The value of the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="true"/>. Custom exception is thrown if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="false"/>.</returns>
    /// <exception cref="Exception">Provided exception if <see cref="P:HasValue"/> is false.</exception>
    public T GetOrThrow(Provider<Exception> exceptionProvider)
    {
        if (exceptionProvider is null)
        {
            throw new ArgumentNullException(nameof(exceptionProvider));
        }

        return HasValue ? _value : throw exceptionProvider.Get();
    }

    /// <summary>Retrieves the value of the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object, or <see langword="default"/>.</summary>
    /// <returns>The value of the <see cref="P:DataFixerUpper.Utils.Optional`1.Value"/> property if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="true"/>; otherwise, <see langword="default"/>.</returns>
    public T? GetOrDefault()
    {
        return HasValue ? _value : default;
    }

    /// <summary>Retrieves the value of the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object, or the specified default value.</summary>
    /// <param name="defaultValue">A value to return if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="false"/>.</param>
    /// <returns>The value of the <see cref="P:DataFixerUpper.Utils.Optional`1.Value"/> property if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="true"/>; otherwise, the <paramref name="defaultValue"/> parameter.</returns>
    public T GetOrDefault(T defaultValue)
    {
        return HasValue ? _value : defaultValue;
    }

    /// <summary>Retrieves the value of the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object, or the specified default value.</summary>
    /// <param name="defaultValue">A value to return if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="false"/>.</param>
    /// <returns>The value of the <see cref="P:DataFixerUpper.Utils.Optional`1.Value"/> property if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="true"/>; otherwise, the <paramref name="defaultValue"/> parameter.</returns>
    public T GetOrDefault(Provider<T> defaultValue)
    {
        return HasValue ? _value : defaultValue.Get();
    }

    /// <summary>Retrieves the value of the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object, or the specified default value.</summary>
    /// <param name="defaultValue">A value to return if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="false"/>.</param>
    /// <returns>The value of the <see cref="P:DataFixerUpper.Utils.Optional`1.Value"/> property if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="true"/>; otherwise, the <paramref name="defaultValue"/> parameter.</returns>
    public T GetOrDefault(ValueHolder<T> defaultValue)
    {
        return HasValue ? _value : defaultValue.Value;
    }

    /// <summary>Retrieves the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object, or another <see cref="T:DataFixerUpper.Utils.Optional`1"/>.</summary>
    /// <param name="provider">A provider of value to return if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="false"/>.</param>
    /// <returns>The <see cref="T:DataFixerUpper.Utils.Optional`1"/> self if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="true"/>; otherwise, the <paramref name="provider"/> parameter provided.</returns>
    public Optional<T> GetOrElse(Provider<Optional<T>> provider)
    {
        return HasValue ? this : provider.Get();
    }

    /// <summary>Retrieves the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object, or another <see cref="T:DataFixerUpper.Utils.Optional`1"/>.</summary>
    /// <param name="else">A value to return if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="false"/>.</param>
    /// <returns>The <see cref="T:DataFixerUpper.Utils.Optional`1"/> self if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="true"/>; otherwise, the <paramref name="else"/> parameter.</returns>
    public Optional<T> GetOrElse(Optional<T> @else)
    {
        return HasValue ? this : @else;
    }

    /// <summary>
    /// Tries to get the current value of the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object and returns a value that indicates if has value.
    /// </summary>
    /// <param name="result">When this method returns, the result of <see cref="T:DataFixerUpper.Utils.Optional`1"/>, if <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="true"/>; otherwise, the default value. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true" /> if <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="true"/>; otherwise, <see langword="false" />.</returns>
    public bool TryGet([NotNullWhen(true)] out T? result)
    {
        result = _value;
        return HasValue;
    }

    /// <summary>
    /// Projects value of the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object, or do nothing and return <see cref="P:DataFixerUpper.Utils.Optional`1.Empty"/> if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="false"/>.
    /// </summary>
    /// <param name="selector">Transformation function.</param>
    /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
    /// <returns>The transformed <see cref="T:DataFixerUpper.Utils.Optional`1"/>.</returns>
    public Optional<TResult> Select<TResult>(Func<T, TResult> selector)
    {
        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        return HasValue ? new Optional<TResult>(selector.Apply(_value)) : Optional<TResult>.Empty;
    }

    /// <summary>
    /// Projects value of the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object, or do nothing and return <see cref="P:DataFixerUpper.Utils.Optional`1.Empty"/> if the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="false"/>.
    /// </summary>
    /// <param name="selector">Transformation function.</param>
    /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
    /// <returns>The transformed <see cref="T:DataFixerUpper.Utils.Optional`1"/>.</returns>
    public Optional<TResult> SelectMany<TResult>(Func<T, Optional<TResult>> selector)
    {
        if (selector is null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        return HasValue ? selector.Apply(_value) : Optional<TResult>.Empty;
    }

    /// <summary>
    /// Projects value of the current <see cref="T:DataFixerUpper.Utils.Optional`1"/> object.
    /// </summary>
    /// <param name="predicate">The predicate to apply to the value, if <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="true"/>.</param>
    /// <returns>If <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="false"/> or value is not match <paramref name="predicate"/>, <see cref="P:DataFixerUpper.Utils.Optional`1.Empty"/>; or else self.</returns>
    public Optional<T> Where(Predicate<T> predicate)
    {
        if (!HasValue || predicate.Test(_value))
        {
            return this;
        }

        return Empty;
    }

    /// <summary>
    /// If the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="true"/>, perform the given action on the value of <see cref="T:DataFixerUpper.Utils.Optional`1"/>.
    /// </summary>
    /// <param name="action">Action to perform.</param>
    public void IfHasValue(Consumer<T> action)
    {
        if (HasValue)
        {
            action.Accept(_value);
        }
    }
    
    /// <summary>
    /// If the <see cref="P:DataFixerUpper.Utils.Optional`1.HasValue"/> property is <see langword="true"/>, perform the given action on the value of <see cref="T:DataFixerUpper.Utils.Optional`1"/> or else action if <see langword="false"/>.
    /// </summary>
    /// <param name="action">Action to perform if has value.</param>
    /// <param name="else">Action to perform if not has value.</param>
    public void IfHasValueOrElse(Consumer<T> action, Action @else)
    {
        if (HasValue)
        {
            action.Accept(_value);
        }
        else
        {
            @else.Invoke();
        }
    }

    /// <inheritdoc/>
    public bool Equals(Optional<T> other)
    {
        return EqualsCore(other);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is Optional<T> other && EqualsCore(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HasValue ? _value!.GetHashCode() : 0;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return HasValue
            ? $"Optional[{_value}]"
            : "Optional.Empty";
    }

    private bool EqualsCore(Optional<T> other)
    {
        return (HasValue, other.HasValue) switch
        {
            (true, true) => _value!.Equals(other._value),
            (false, false) => true,
            _ => false
        };
    }

    /// <summary>Compares two values to determine equality.</summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(Optional<T> left, Optional<T> right)
    {
        return left.EqualsCore(right);
    }

    /// <summary>Compares two values to determine inequality.</summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(Optional<T> left, Optional<T> right)
    {
        return !(left == right);
    }
}