using System;
using System.Collections.Generic;

namespace DataFixerUpper.Extensions;

/// <summary>
/// Extension for <see cref="T:System.ValueTuple`2"/>
/// </summary>
public static class TupleExtension
{
    extension<T1, T2>((T1, T2) tuple)
    {
        /// <summary>
        /// First value of tuple.
        /// </summary>
        /// <remarks><see cref="F:System.ValueTuple`2.Item1"/>.</remarks>
        public T1 First => tuple.Item1;

        /// <summary>
        /// Second value of tuple.
        /// </summary>
        /// <remarks><see cref="F:System.ValueTuple`2.Item2"/>.</remarks>
        public T2 Second => tuple.Item2;

        /// <summary>
        /// Transform first value with <paramref name="mapper"/>.
        /// </summary>
        /// <param name="mapper">Transformer.</param>
        public (T1M, T2) MapFirst<T1M>(Func<T1, T1M> mapper)
        {
            return (mapper.Apply(tuple.Item1), tuple.Item2);
        }

        /// <summary>
        /// Transform second value with <paramref name="mapper"/>.
        /// </summary>
        /// <param name="mapper">Transformer.</param>
        public (T1, T2M) MapSecond<T2M>(Func<T2, T2M> mapper)
        {
            return (tuple.Item1, mapper.Apply(tuple.Item2));
        }
    }

    extension<TKey, TValue>(KeyValuePair<TKey, TValue> pair)
    {
        /// <summary>
        /// Transform key with <paramref name="mapper"/>.
        /// </summary>
        /// <param name="mapper">Transformer.</param>
        public KeyValuePair<TM, TValue> MapKey<TM>(Func<TKey, TM> mapper)
        {
            return KeyValuePair.Create(mapper.Apply(pair.Key), pair.Value);
        }

        /// <summary>
        /// Transform value with <paramref name="mapper"/>.
        /// </summary>
        /// <param name="mapper">Transformer.</param>
        public KeyValuePair<TKey, TM> MapValue<TM>(Func<TValue, TM> mapper)
        {
            return KeyValuePair.Create(pair.Key, mapper.Apply(pair.Value));
        }
    }
}