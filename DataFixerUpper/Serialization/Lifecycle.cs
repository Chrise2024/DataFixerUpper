using System;
using System.Numerics;

namespace DataFixerUpper.Serialization;

/// <summary>
/// A marker defining the stability of a given result.
/// <remarks>
/// Experimental results are may change incompatibly in the future and deprecated results may be removed in the future.
/// </remarks>
/// </summary>
public abstract class Lifecycle : IEquatable<Lifecycle>, IAdditionOperators<Lifecycle, Lifecycle, Lifecycle>, IEqualityOperators<Lifecycle, Lifecycle, bool>
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
        return obj is Lifecycle other &&  EqualsCore(other);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(State);
        
        if (State == Lifecycles.Deprecated)
        {
            hash.Add(((Deprecated) this).Since);
        }

        return hash.ToHashCode();
    }

    /// <inheritdoc/>
    public static Lifecycle operator +(Lifecycle lifecycle1, Lifecycle lifecycle2)
    {
        return lifecycle1.Add(lifecycle2);
    }

    /// <inheritdoc/>
    public static bool operator ==(Lifecycle? lifecycle1, Lifecycle? lifecycle2)
    {
        return lifecycle1?.State == lifecycle2?.State;
    }

    /// <inheritdoc/>
    public static bool operator !=(Lifecycle? lifecycle1, Lifecycle? lifecycle2)
    {
        return !(lifecycle1 == lifecycle2);
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