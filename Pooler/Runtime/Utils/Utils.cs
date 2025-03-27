using System.Collections.Generic;

namespace Pooler
{
    public static class Utils
    {
        public static T AddWithReturn<T>(this List<T> list, T item)
        {
            list.Add(item);
            return item;
        }

        public static KeyValuePair<TKey, TValue> AddWithReturn<TKey, TValue>(this IDictionary<TKey, TValue> collection, KeyValuePair<TKey, TValue> toBeAddedValue)
        {
            collection.Add(toBeAddedValue);
            return toBeAddedValue;
        }

          public static (TKey key,TValue value) AddWithReturn<TKey, TValue>(this IDictionary<TKey, TValue> collection, TKey key, TValue value)
        {
            collection.Add(key, value);
            return (key, value);
        }

        public static bool TryAddWithReturn<TKey, TValue>(this IDictionary<TKey, TValue> collection, KeyValuePair<TKey, TValue> toBeAddedValue)
        {
            return collection.TryAdd(toBeAddedValue.Key, toBeAddedValue.Value);
        }

        public static T AddWithReturn<T>(this ICollection<T> list, T item)
        {
            list.Add(item);
            return item;
        }
    }
}