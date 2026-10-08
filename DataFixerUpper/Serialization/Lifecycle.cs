using System;

namespace DataFixerUpper.Serialization;

/// <summary>
/// A marker defining the stability of a given result.
/// <remarks>
/// Experimental results are may change incompatibly in the future and deprecated results may be removed in the future.
/// </remarks>
/// </summary>
public abstract class Lifecycle : IEquatable<Lifecycle>
{
    /// <summary>
    /// Experimental instance.
    /// </summary>
    public static Lifecycle Experimental => new ExperimentalImpl();

    /// <summary>
    /// Stable instance.
    /// </summary>
    public static Lifecycle Stable => new StableImpl();

    /// <summary>
    /// Create Deprecated instance.
    /// </summary>
    /// <param name="since">Deprecated version.</param>
    public static Lifecycle CreateDeprecated(int since)
    {
        return new Deprecated(since);
    }

    private Lifecycle(Lifecycles state)
    {
        State = state;
    }

    private Lifecycles State { get; }

    /// <summary>
    /// Combine two <see cref="Lifecycle"/>.
    /// </summary>
    /// <param name="other">Other <see cref="Lifecycle"/>.</param>
    /// <returns>Combined <see cref="Lifecycle"/>.</returns>
    public Lifecycle Add(Lifecycle other)
    {
        if (this == Experimental || other == Experimental)
        {
            return Experimental;
        }

        if (this is Deprecated)
        {
            if (other is Deprecated deprecated && deprecated.Since < ((Deprecated) this).Since)
            {
                return deprecated;
            }

            return this;
        }

        if (other is Deprecated)
        {
            return other;
        }

        return Stable;
    }

    /// <inheritdoc/>
    public bool Equals(Lifecycle? other)
    {
        return EqualsCore(other);
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return obj is Lifecycle other && EqualsCore(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        int hash = (int) State;

        if (State == Lifecycles.Deprecated)
        {
            hash |= (((Deprecated) this).Since << 2);
        }

        return hash;
    }

    /// <summary>Adds two values together to compute their sum.</summary>
    /// <param name="left">The value to which <paramref name="right" /> is added.</param>
    /// <param name="right">The value which is added to <paramref name="left" />.</param>
    /// <returns>The sum of <paramref name="left" /> and <paramref name="right" />.</returns>
    public static Lifecycle operator +(Lifecycle left, Lifecycle right)
    {
        return left.Add(right);
    }

    /// <summary>Compares two values to determine equality.</summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
    public static bool operator ==(Lifecycle? left, Lifecycle? right)
    {
        return left?.State == right?.State;
    }

    /// <summary>Compares two values to determine inequality.</summary>
    /// <param name="left">The value to compare with <paramref name="right" />.</param>
    /// <param name="right">The value to compare with <paramref name="left" />.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
    public static bool operator !=(Lifecycle? left, Lifecycle? right)
    {
        return !(left == right);
    }

    private bool EqualsCore(Lifecycle? other)
    {
        return State == other?.State;
    }

    private enum Lifecycles
    {
        Stable,
        Experimental,
        Deprecated
    }

    private sealed class StableImpl() : Lifecycle(Lifecycles.Stable)
    {
        public override string ToString()
        {
            return "Stable";
        }
    }

    private sealed class ExperimentalImpl() : Lifecycle(Lifecycles.Experimental)
    {
        public override string ToString()
        {
            return "Experimental";
        }
    }

    /// <summary>
    /// Deprecated Lifecycle
    /// </summary>
    /// <param name="since">Deprecated stamp.</param>
    public sealed class Deprecated(int since) : Lifecycle(Lifecycles.Deprecated)
    {
        /// <summary>
        /// Deprecated stamp.
        /// </summary>
        public int Since => since;

        /// <inheritdoc/>
        public override string ToString()
        {
            return "Deprecated since " + Since;
        }
    }
}