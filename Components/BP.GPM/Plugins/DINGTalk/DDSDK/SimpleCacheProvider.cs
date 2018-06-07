using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.EAI.Plugins.DDSDK
{
    /// <summary>  
    /// 简易缓存  
    /// </summary>  
    public class SimpleCacheProvider : ICacheProvider
    {
        private static SimpleCacheProvider _instance = null;

        #region GetInstance
        /// <summary>  
        /// 获取缓存实例  
        /// </summary>  
        /// <returns></returns>  
        public static SimpleCacheProvider GetInstance()
        {
            if (_instance == null) _instance = new SimpleCacheProvider();
            return _instance;
        }
        #endregion

        private Dictionary<string, CacheItem> _caches;

        private SimpleCacheProvider()
        {
            this._caches = new Dictionary<string, CacheItem>();
        }

        #region GetCache
        /// <summary>  
        /// 获取缓存  
        /// </summary>  
        /// <param name="key"></param>  
        /// <returns></returns>  
        public object GetCache(string key)
        {
            object obj = this._caches.ContainsKey(key) ? this._caches[key].Expired() ? null : this._caches[key].Value : null;
            return obj;
        }

        /// <summary>  
        /// 获取缓存  
        /// </summary>  
        /// <typeparam name="T"></typeparam>  
        /// <param name="key"></param>  
        /// <returns></returns>  
        public T GetCache<T>(String key)
        {
            object obj = GetCache(key);
            if (obj == null)
            {
                return default(T);
            }
            T result = (T)obj;
            return result;
        }
        #endregion

        #region SetCache
        /// <summary>  
        /// 设置缓存  
        /// </summary>  
        /// <param name="key"></param>  
        /// <param name="value"></param>  
        /// <param name="expire"></param>  
        public void SetCache(string key, object value, int expire = 300)
        {
            this._caches[key] = new CacheItem(key, value, expire);
        }
        #endregion
    }
}
