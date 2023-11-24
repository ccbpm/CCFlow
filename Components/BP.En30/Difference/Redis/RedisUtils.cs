using System;
using System.Collections.Generic;
using ServiceStack.Text;

namespace BP.Difference.Redis
{
    public class RedisUtils: RedisOperatorBase
    {
        public RedisUtils() : base() { }
        public bool Set<T>(string key, T t)
        {
            return Redis.Set(key, t);
        }
        public bool Set(string key, String value)
        {
            return Redis.Set(key, value);
        }

        public T Get<T>(string key)
        {
            return Redis.Get<T>(key);
        }
        public string Get(string key)
        {
            Object obj =  Redis.Get<Object>(key);
            if (obj == null) return null;
            return obj.ToString();
        }
        public bool del(string key)
        {
            return Redis.Remove(key);
        }
        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        public bool ExistInHash<T>(string hashId, string key)
        {
            return Redis.HashContainsEntry(hashId, key);
        }
         /// <summary>
         /// 存储数据到hash表
         /// </summary>
        public bool SetInHash<T>(string hashId, string key, T t)
         {
             var value = JsonSerializer.SerializeToString<T>(t);
             return Redis.SetEntryInHash(hashId, key, value);
         }
        public bool SetInHash(string hashId, string key, string value)
        {
            return Redis.SetEntryInHash(hashId, key, value);
        }
        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        public bool RemoveHash(string hashId, string key)
         {
             return Redis.RemoveEntryFromHash(hashId, key);
         }
         /// <summary>
         /// 移除整个hash
         /// </summary>
         public bool RemoveHash(string key)
         {
             return Redis.Remove(key);
         }
         /// <summary>
         /// 从hash表获取数据
         /// </summary>
         public T GetFromHash<T>(string hashId, string key)
         {
             string value = Redis.GetValueFromHash(hashId, key);
            return JsonSerializer.DeserializeFromString<T>(value);
         }
        public String GetFromHash(string hashId, string key)
        {
            return Redis.GetValueFromHash(hashId, key);
        }
        /// <summary>
        /// 获取整个hash的数据
        /// </summary>
        public List<T> GetAllHashValues<T>(string hashId)
         {
             var result = new List<T>();
             var list = Redis.GetHashValues(hashId);
             if (list != null && list.Count > 0)
             {
                 list.ForEach(x =>
                 {
                    var value = JsonSerializer.DeserializeFromString<T>(x);
                     result.Add(value);
                 });
             }
            return result;
         }
        /// <summary>
        /// 设置缓存过期
         /// </summary>
         public void SetExpire(string key, DateTime datetime)
         {
             Redis.ExpireEntryAt(key, datetime);
         }

    }
}
