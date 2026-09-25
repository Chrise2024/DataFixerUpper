using System;
using System.Collections.Generic;

namespace DataFixerUpper.Utils;

#nullable disable

/// <summary>
/// Function utils.
/// </summary>
public static class Functions
{
    /// <summary>
    /// Function that add second value to first collection.
    /// </summary>
    /// <seealso cref="M:DataFixerUpper.Serialization.DataResultExtension.extension(DataFixerUpper.Serialization.DataResult{`0}).Combine``2(System.Func{`0,``0,``1},DataFixerUpper.Serialization.DataResult{``0})"/>
    public static TCollection AddToFirst<TCollection, TElement>(TCollection c, TElement e)
        where TCollection : ICollection<TElement>
    {
        c.Add(e);
        return c;
    }

    /// <summary>
    /// Consumer receives value and do nothing.
    /// </summary>
    public static void EmptyConsumer<T>(T value) { }
    
    /// <summary>
    /// Function that always returns its input argument.
    /// </summary>
    public static T Identity<T>(T value)
    {
        return value;
    }

    /// <summary>
    /// Function that always returns its first input argument.
    /// </summary>
    public static T1 LiftFirst<T1, T2>(T1 t1, T2 t2)
    {
        return t1;
    }

    /// <summary>
    /// Function that always returns its second input argument.
    /// </summary>
    public static T2 LiftSecond<T1, T2>(T1 t1, T2 t2)
    {
        return t2;
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first argument and returns a function that takes the remaining argument and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, Func<T2, TR>> Curry1<T1, T2, TR>(Func<T1, T2, TR> func)
    {
        return t1 => { return t2 => func(t1, t2); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first argument and returns a function that takes the remaining argument and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, Func<T2, T3, TR>> Curry1<T1, T2, T3, TR>(Func<T1, T2, T3, TR> func)
    {
        return t1 => { return (t2, t3) => func(t1, t2, t3); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first two arguments and returns a function that takes the remaining arguments and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, T2, Func<T3, T4, TR>> Curry2<T1, T2, T3, T4, TR>(Func<T1, T2, T3, T4, TR> func)
    {
        return (t1, t2) => { return (t3, t4) => func(t1, t2, t3, t4); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first two arguments and returns a function that takes the remaining arguments and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, T2, Func<T3, T4, T5, TR>> Curry2<T1, T2, T3, T4, T5, TR>(Func<T1, T2, T3, T4, T5, TR> func)
    {
        return (t1, t2) => { return (t3, t4, t5) => func(t1, t2, t3, t4, t5); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first three arguments and returns a function that takes the remaining arguments and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, T2, T3, Func<T4, T5, T6, TR>> Curry3<T1, T2, T3, T4, T5, T6, TR>(Func<T1, T2, T3, T4, T5, T6, TR> func)
    {
        return (t1, t2, t3) => { return (t4, t5, t6) => func(t1, t2, t3, t4, t5, t6); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first three arguments and returns a function that takes the remaining arguments and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, T2, T3, Func<T4, T5, T6, T7, TR>> Curry3<T1, T2, T3, T4, T5, T6, T7, TR>(Func<T1, T2, T3, T4, T5, T6, T7, TR> func)
    {
        return (t1, t2, t3) => { return (t4, t5, t6, t7) => func(t1, t2, t3, t4, t5, t6, t7); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first four arguments and returns a function that takes the remaining arguments and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, T2, T3, T4, Func<T5, T6, T7, T8, TR>> Curry4<T1, T2, T3, T4, T5, T6, T7, T8, TR>(Func<T1, T2, T3, T4, T5, T6, T7, T8, TR> func)
    {
        return (t1, t2, t3, t4) => { return (t5, t6, t7, t8) => func(t1, t2, t3, t4, t5, t6, t7, t8); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first four arguments and returns a function that takes the remaining arguments and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, T2, T3, T4, Func<T5, T6, T7, T8, T9, TR>> Curry4<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TR> func)
    {
        return (t1, t2, t3, t4) => { return (t5, t6, t7, t8, t9) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first five arguments and returns a function that takes the remaining arguments and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, T2, T3, T4, T5, Func<T6, T7, T8, T9, T10, TR>> Curry5<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TR>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TR> func)
    {
        return (t1, t2, t3, t4, t5) => { return (t6, t7, t8, t9, t10) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first five arguments and returns a function that takes the remaining arguments and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, T2, T3, T4, T5, Func<T6, T7, T8, T9, T10, T11, TR>> Curry5<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TR>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TR> func)
    {
        return (t1, t2, t3, t4, t5) => { return (t6, t7, t8, t9, t10, t11) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first six arguments and returns a function that takes the remaining arguments and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, T2, T3, T4, T5, T6, Func<T7, T8, T9, T10, T11, T12, TR>> Curry6<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TR>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TR> func)
    {
        return (t1, t2, t3, t4, t5, t6) => { return (t7, t8, t9, t10, t11, t12) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first six arguments and returns a function that takes the remaining arguments and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, T2, T3, T4, T5, T6, Func<T7, T8, T9, T10, T11, T12, T13, TR>> Curry6<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TR>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TR> func)
    {
        return (t1, t2, t3, t4, t5, t6) => { return (t7, t8, t9, t10, t11, t12, t13) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first seven arguments and returns a function that takes the remaining arguments and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, Func<T8, T9, T10, T11, T12, T13, T14, TR>> Curry7<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TR>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TR> func)
    {
        return (t1, t2, t3, t4, t5, t6, t7) => { return (t8, t9, t10, t11, t12, t13, t14) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first seven arguments and returns a function that takes the remaining arguments and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, Func<T8, T9, T10, T11, T12, T13, T14, T15, TR>> Curry7<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TR>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TR> func)
    {
        return (t1, t2, t3, t4, t5, t6, t7) => { return (t8, t9, t10, t11, t12, t13, t14, t15) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15); };
    }

    /// <summary>
    /// Curries <paramref name="func"/> into new function that takes the first eight arguments and returns a function that takes the remaining arguments and returns the result.
    /// </summary>
    /// <param name="func">Function to curry.</param>
    /// <returns>Curred function</returns>
    public static Func<T1, T2, T3, T4, T5, T6, T7, T8, Func<T9, T10, T11, T12, T13, T14, T15, T16, TR>> Curry8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TR>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TR> func)
    {
        return (t1, t2, t3, t4, t5, t6, t7, t8) => { return (t9, t10, t11, t12, t13, t14, t15, t16) => func(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16); };
    }
}

/// <summary>
/// Represents a provider of value.
/// </summary>
public delegate T Provider<out T>();

/// <summary>
/// Represents an operation that accepts a single input argument and returns no result.
/// </summary>
public delegate void Consumer<in T>(T value);

/// <summary>
/// Represents an operation on a single operand that produces a result of the same type as its operand.
/// </summary>
public delegate T UnaryOperation<T>(T value);