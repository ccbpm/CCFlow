using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.EAI.Plugins.DDSDK
{
    /// <summary>  
    /// 缓存接口  
    /// </summary>  
    interface ICacheProvider
    {
        /// <summary>  
        /// 获取缓存  
        /// </summary>  
        /// <param name="key">缓存key</param>  
        /// <returns>缓存对象或null,不存在或者过期返回null</returns>  
        object GetCache(string key);

        /// <summary>  
        /// 写入缓存  
        /// </summary>  
        /// <param name="key">缓存key</param>  
        /// <param name="value">缓存值</param>  
        /// <param name="expire">缓存有效期，单位为秒，默认300</param>  
        void SetCache(string key, object value, int expire = 300);
    }
}
