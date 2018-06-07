using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.EAI.Plugins.DDSDK
{
    /// <summary>  
    /// 缓存项  
    /// </summary>  
    public class CacheItem
    {

        #region 属性
        private object _value;
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        private string _key;
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
        #endregion

        #region 内部变量
        /// <summary>  
        /// 插入时间  
        /// </summary>  
        private long _insertTime;
        /// <summary>  
        /// 过期时间  
        /// </summary>  
        private int _expire;
        #endregion

        #region 构造函数
        /// <summary>  
        /// 构造函数  
        /// </summary>  
        /// <param name="key">缓存的KEY</param>  
        /// <param name="value">缓存的VALUE</param>  
        /// <param name="expire">缓存的过期时间</param>  
        public CacheItem(string key, object value, int expire)
        {
            this._key = key;
            this._value = value;
            this._expire = expire;
            this._insertTime = TimeStamp.Now();
        }
        #endregion

        #region Expired
        /// <summary>  
        /// 是否过期  
        /// </summary>  
        /// <returns></returns>  
        public bool Expired()
        {
            return TimeStamp.Now() > this._insertTime + _expire;
        }
        #endregion
    }
}
