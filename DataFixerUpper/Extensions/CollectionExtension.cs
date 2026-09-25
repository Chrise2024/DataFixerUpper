// ReSharper disable once CheckNamespace

namespace System.Collections.Generic;

/// <summary>
/// Extensions for collections
/// </summary>
public static class CollectionExtension
{
    extension<TCollection, TElement>(TCollection collection)
        where TCollection : ICollection<TElement>
    {
        /// <summary>
        /// Add <paramref name="item"/> to <paramref name="collection"/> and return <paramref name="collection"/>.
        /// </summary>
        /// <remarks>If use in <see cref="M:DataFixerUpper.Serialization.DataResultExtension.extension(DataFixerUpper.Serialization.DataResult{`0}).Combine``2(System.Func{`0,``0,``1},DataFixerUpper.Serialization.DataResult{``0})"/>, use <see cref="M:DataFixerUpper.Utils.Functions.AddToFirst``2(``0,``1)"/> instead.</remarks>
        /// <param name="item">Item to add.</param>
        /// <returns><paramref name="collection"/> itself.</returns>
        /// <seealso cref="M:DataFixerUpper.Utils.Functions.AddToFirst``2(``0,``1)"/>
        public TCollection AddAndReturn(TElement item)
        {
            collection.Add(item);
            return collection;
        }
        
        /// <summary>
        /// Add <paramref name="items"/> to <paramref name="collection"/> and return <paramref name="collection"/>.
        /// </summary>
        /// <param name="items">Items to add.</param>
        /// <returns><paramref name="collection"/> itself.</returns>
        public TCollection AddAll(IEnumerable<TElement> items)
        {
            foreach (TElement item in items)
            {
                collection.Add(item);
            }
            
            return collection;
        }
    }

    extension<TDict, TKey, TValue>(TDict dict)
        where TDict : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// Adds an element with the provided key and value to the <paramref name="dict"/> and return <paramref name="dict"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <returns><paramref name="dict"/> itself.</returns>
        public TDict AddAndReturn(TKey key, TValue value)
        {
            dict.Add(key, value);
            return dict;
        }
    }
}