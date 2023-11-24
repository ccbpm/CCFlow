using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.Difference.Redis
{
    public abstract class RedisOperatorBase : IDisposable
    {
        protected IRedisClient Redis { get; private set; }
        private bool _disposed = false;
        protected RedisOperatorBase()
        {
            RedisManager redisManager = new RedisManager();
            Redis = redisManager.GetClient();
            
        }        
        public void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
               if (disposing)
               {
                    Redis.Dispose();
                    Redis = null;
               }
            }
            this._disposed = true;
        }
        public void Dispose()
        {
           Dispose(true);
           GC.SuppressFinalize(this);
        }
         /// <summary>
         /// 保存数据DB文件到硬盘
         /// </summary>
         public void Save()
         {
             Redis.Save();
         }
         /// <summary>
         /// 异步保存数据DB文件到硬盘
         /// </summary>
         public void SaveAsync()
         {
             Redis.SaveAsync();
         }
    }
}
