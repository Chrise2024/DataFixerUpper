using System;
using System.Threading;
using DataFixerUpper.Extensions;

namespace DataFixerUpper.Utils;

/// <summary>
/// For creating <see cref="T:DataFixerUpper.Utils.ValueHolder`1"/> instance.
/// </summary>
public static class ValueHolder
{
    /// <summary>
    /// Creates a new <see cref="T:DataFixerUpper.Utils.ValueHolder`1"/> instance with the direct value.
    /// </summary>
    /// <param name="value">Value to be wrapped.</param>
    /// <returns>A new <see cref="T:DataFixerUpper.Utils.ValueHolder`1"/> instance.</returns>
    public static ValueHolder<T> Create<T>(T value)
    {
        return new ValueHolder<T>(value);
    }

    /// <summary>
    /// Creates a new <see cref="T:DataFixerUpper.Utils.ValueHolder`1"/> instance with the direct value.
    /// </summary>
    /// <param name="provider">Provider of the value for lazy initialize.</param>
    /// <returns>A new <see cref="T:DataFixerUpper.Utils.ValueHolder`1"/> instance.</returns>
    public static ValueHolder<T> Create<T>(Provider<T> provider)
    {
        return new ValueHolder<T>(provider);
    }
}

/// <summary>
/// A thread-safe holder for a value that can be initialized lazily.
/// </summary>
public struct ValueHolder<T>
{
    private Provider<T>? _provider;
    private T? _value;
    /// <summary>
    /// 0 = not initialized
    /// 1 = initialization in progress
    /// 2 = initialized
    /// </summary>
    private int _state;
    
    /// <summary>
    /// Gets the value, initializing it if necessary.
    /// </summary>
    public T Value => InitializeOrGetValue();

    /// <summary>
    /// Initialize via direct value.
    /// </summary>
    /// <param name="initialValue">Value to wrap.</param>
    public ValueHolder(T initialValue)
    {
        ArgumentNullException.ThrowIfNull(initialValue);
        _value = initialValue;
        _state = 2;
    }

    /// <summary>
    /// Initialize via provider for lazy initialization.
    /// </summary>
    /// <param name="provider">Provider of value.</param>
    public ValueHolder(Provider<T> provider)
    {
        ArgumentNullException.ThrowIfNull(provider);
        _provider = provider;
        _state = 0;
    }

    private T InitializeOrGetValue()
    {
        if (Volatile.Read(ref _state) == 2)
        {
            return _value!;
        }

        if (Interlocked.CompareExchange(ref _state, 1, 0) == 0)
        {
            if (_provider == null)
            {
                throw new InvalidOperationException("Provider is null.");
            }
            
            T value;

            try
            {
                value = _provider.Get();
            }
            catch
            {
                Volatile.Write(ref _state, 0);
                throw;
            }

            _value = value;
            _provider = null;

            Volatile.Write(ref _state, 2);

            return value;
        }

        SpinWait spin = new();
        while (Volatile.Read(ref _state) == 1)
        {
            spin.SpinOnce();
        }

        return _value ?? throw new InvalidOperationException("Fail to initialize value.");
    }

    /// <summary>
    /// Implicitly converts a value to a <see cref="T:DataFixerUpper.Utils.ValueHolder`1"/>.
    /// </summary>
    public static implicit operator ValueHolder<T>(T value)
    {
        return new ValueHolder<T>(value);
    }
    
    /// <summary>
    /// Implicitly converts a provider to a <see cref="T:DataFixerUpper.Utils.ValueHolder`1"/>.
    /// </summary>
    public static implicit operator ValueHolder<T>(Provider<T> provider)
    {
        return new ValueHolder<T>(provider);
    }
}