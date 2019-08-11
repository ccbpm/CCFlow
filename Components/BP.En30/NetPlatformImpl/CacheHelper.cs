using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Caching;

namespace BerryInfo.Utility
{
    public static class CacheHelper
    {
        private static MemoryCache mc = MemoryCache.Default;

        public static bool Contains(string key)
        {
            return mc.Contains(key);
        }

        public static T Get<T>(string key)
        {
            return (T)mc.Get(key);
        }

        public static void Add<T>(string key, T v)
        {
            CacheItemPolicy p = new CacheItemPolicy();
            mc.Add(key, (object)v, p);
        }

        public static void Remove(string key)
        {
            mc.Remove(key);
        }
    }
}
