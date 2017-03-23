using System;
using System.Linq;
using System.Collections.Generic;

namespace Common.DictionaryExtension
{
    static class DictionaryExtension
    {
        public static void RemoveAll<K, V>(this IDictionary<K, V> dict, Func<K, V, bool> match)
        {
            foreach (var key in dict.Keys.ToArray()
            .Where(key => match(key, dict[key])))
                dict.Remove(key);
        }
    }
}
