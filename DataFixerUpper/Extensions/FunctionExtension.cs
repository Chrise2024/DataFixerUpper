using System;
using DataFixerUpper.Utils;

namespace DataFixerUpper.Extensions;

/// <summary>
/// Extensions for delegates
/// </summary>
public static class FunctionExtension
{
    extension<T, TR>(Func<T, TR> func)
    {
        /// <summary>
        /// Invoke after <paramref name="func"/> with return value.
        /// </summary>
        /// <code>
        /// t => after(func(t))
        /// </code>
        /// <param name="after">After function</param>
        /// <returns>New function</returns>
        public Func<T, TR1> Then<TR1>(Func<TR, TR1> after)
        {
            return t => after(func(t));
        }
        
        /// <summary>
        /// Invoke before <paramref name="func"/>, return value to invoke <paramref name="func"/>.
        /// </summary>
        /// <code>
        /// t => func(before(t))
        /// </code>
        /// <param name="before">Before function</param>
        /// <returns>New function</returns>
        public Func<T1, TR> Compose<T1>(Func<T1, T> before)
        {
            return t => func(before(t));
        }

        /// <summary>
        /// Apply <paramref name="func"/> on <paramref name="value"/>.
        /// </summary>
        public TR Apply(T value)
        {
            return func(value);
        }
    }

    extension<T>(Provider<T> provider)
    {
        /// <summary>
        /// Get result of <paramref name="provider"/>.
        /// </summary>
        public T Get()
        {
            return provider();
        }

        /// <summary>
        /// Caching return value to avoid multi instance creation.
        /// </summary>
        /// <returns>New <see cref="T:DataFixerUpper.Utils.Provider`1"/>.</returns>
        public Provider<T> AsMemorize()
        {
            return () => new ValueHolder<T>(provider).Value;
        }
    }

    extension<T>(Consumer<T> consumer)
    {
        /// <summary>
        /// Perform operation on <paramref name="value"/>.
        /// </summary>
        public void Accept(T value)
        {
            consumer(value);
        }
    }

    extension<T>(UnaryOperation<T> operation)
    {
        /// <summary>
        /// Perform operation on <paramref name="value"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T Apply(T value)
        {
            return operation(value);
        }
    }

    extension<T1, T2, TR>(Func<T1, T2, TR> combiner)
    {
        /// <summary>
        /// Perform operation on values
        /// </summary>
        public TR Apply(T1 value1, T2 value2)
        {
            return combiner(value1, value2);
        }
    }

    extension<T1, T2, T3, TR>(Func<T1, T2, T3, TR> combiner)
    {
        /// <summary>
        /// Perform operation on values
        /// </summary>
        public TR Apply(T1 value1, T2 value2, T3 value3)
        {
            return combiner(value1, value2, value3);
        }
    }

    extension<T1, T2, T3, T4, TR>(Func<T1, T2, T3, T4, TR> combiner)
    {
        /// <summary>
        /// Perform operation on values
        /// </summary>
        public TR Apply(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            return combiner(value1, value2, value3, value4);
        }
    }

    extension<T>(Predicate<T> predicate)
    {
        /// <summary>
        /// Evaluates on <paramref name="value"/>.
        /// </summary>
        public bool Test(T value)
        {
            return predicate(value);
        }
    }
}