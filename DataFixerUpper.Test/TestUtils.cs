using System.Collections;
using System.Collections.Immutable;

namespace DataFixerUpper.Test;

public static class JList
{
    public static IList<T> Of<T>(params IEnumerable<T> values)
    {
        return values.ToImmutableList();
    }
    
    public static IList<T> Of<T>(T value)
    {
        return ImmutableList.Create(value);
    }
    
    public static IList<T> Of<T>()
    {
        return ImmutableList<T>.Empty;
    }
}

public static class JObjList
{
    public static IList Of(params IEnumerable<object> values)
    {
        return values.ToImmutableList();
    }
    
    public static IList Of(object value)
    {
        return ImmutableList.Create(value);
    }
    
    public static IList Of()
    {
        return ImmutableList<object>.Empty;
    }
}

public static class JMap
{
    public static IDictionary<object, object> Of(params object[] values)
    {
        if (values.Length % 2 != 0)
        {
            throw new InvalidOperationException("Values must be pairs");
        }

        ImmutableDictionary<object, object>.Builder builder = ImmutableDictionary.CreateBuilder<object, object>();
        for (int i = 0; i < values.Length / 2; i++)
        {
            builder.Add(values[i * 2], values[i * 2 + 1]);
        }

        return builder.ToImmutable();
    }

    public static IDictionary<object, object> Of(object key, object value)
    {
        return ImmutableDictionary<object, object>.Empty.Add(key, value);
    }
    
    
    public static IDictionary<object, object> Of()
    {
        return ImmutableDictionary<object, object>.Empty;
    }
    
    public static IDictionary<TKey, TValue?> Of<TKey, TValue>(
        TKey key1, TValue? value1,
        TKey key2, TValue? value2,
        TKey key3, TValue? value3
        )
        where TKey : notnull
    {
        return ImmutableDictionary<TKey, TValue?>.Empty.AddRange([
            KeyValuePair.Create(key1, value1),
            KeyValuePair.Create(key2, value2),
            KeyValuePair.Create(key3, value3)
        ]);
    }
    
    public static IDictionary<TKey, TValue?> Of<TKey, TValue>(TKey key1, TValue? value1, TKey key2, TValue? value2)
        where TKey : notnull
    {
        return ImmutableDictionary<TKey, TValue?>.Empty.AddRange([
            KeyValuePair.Create(key1, value1),
            KeyValuePair.Create(key2, value2)
        ]);
    }
    
    public static IDictionary<TKey, TValue?> Of<TKey, TValue>(TKey key, TValue? value)
        where TKey : notnull
    {
        return ImmutableDictionary<TKey, TValue?>.Empty.Add(key, value);
    }
    
    public static IDictionary<TKey, TValue> Of<TKey, TValue>()
        where TKey : notnull
    {
        return ImmutableDictionary<TKey, TValue>.Empty;
    }
}