using System.Collections;

namespace DataFixerUpper.Test;

public class TestComparer : IEqualityComparer<object>
{
    bool IEqualityComparer<object>.Equals(object? x, object? y)
    {
        return EqualsCore(x, y);
    }

    private static bool EqualsCore(object? x, object? y)
    {
        if (x is IDictionary dict1 && y is IDictionary dict2)
        {
            if (dict1.Count != dict2.Count)
            {
                return false;
            }

            return dict1.Keys.Cast<object>().All(key1 => EqualsCore(dict1[key1], dict2[key1]));
        }

        if (x is IList list1 && y is IList list2)
        {
            if (list1.Count != list2.Count)
            {
                return false;
            }

            List<object> lo1 = list1.Cast<object>().ToList();
            List<object> lo2 = list2.Cast<object>().ToList();
            return lo1.All(elem => lo2.Find(e => EqualsCore(elem, e)) != null);
        }

        return EqualityComparer<object>.Default.Equals(x, y);
    }
    
    public int GetHashCode(object obj)
    {
        if (obj is IDictionary dict)
        {
            HashCode hash = new();
            foreach (object entry in dict)
            {
                DictionaryEntry e = (DictionaryEntry) entry;
                hash.Add(e.Key);
                hash.Add(e.Value);
            }

            return hash.ToHashCode();
        }

        if (obj is IList list)
        {
            HashCode hash = new();
            foreach (object element in list)
            {
                hash.Add(element);
            }

            return hash.ToHashCode();
        }

        return EqualityComparer<object>.Default.GetHashCode(obj);
    }
}