using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.Difference.Redis
{
    public sealed class RedisConfigInfo : ConfigurationSection
    {
        public static RedisConfigInfo GetConfig()
        {
              var section = (RedisConfigInfo)ConfigurationManager.GetSection("RedisConfig");
              return section;
        }
        public static RedisConfigInfo GetConfig(string sectionName)
        {
            var section = (RedisConfigInfo)ConfigurationManager.GetSection("RedisConfig");
            if (section == null)
            {
                throw new ConfigurationErrorsException("Section " + sectionName + " is not found.");
            }
            return section;
        }
         /// <summary>
         /// 可写的Redis链接地址
         /// </summary>
         [ConfigurationProperty("WriteServerList", IsRequired = false)]
         public string WriteServerList
         {
             get
             {
                  return (string)base["WriteServerList"];
             }
             set
             {
                  base["WriteServerList"] = value;
             }
         }        
        /// <summary>
        /// 可读的Redis链接地址
        /// </summary>
        [ConfigurationProperty("ReadServerList", IsRequired = false)]
        public string ReadServerList
        {
            get
            {
                return (string)base["ReadServerList"];
            }
            set
            {
            base["ReadServerList"] = value;
            }
        }
        /// <summary>
        /// 最大连接数
        /// </summary>
        [ConfigurationProperty("MaxWritePoolSize", IsRequired = false,DefaultValue =5)]
        public int MaxWritePoolSize
        {
            get
            {
                int maxWritePoolSize =  (int)base["MaxWritePoolSize"];
                return maxWritePoolSize > 0 ? maxWritePoolSize : 5;
            }
            set
            {
                base["MaxWritePoolSize"] = value;
            }
        }
        /// <summary>
        /// 最大可读连接数
        /// </summary>
        [ConfigurationProperty("MaxReadPoolSize", IsRequired = false, DefaultValue = 5)]
        public int MaxReadPoolSize
        {
            get
            {
                int maxReadPoolSize = (int)base["MaxReadPoolSize"];
                return maxReadPoolSize > 0 ? maxReadPoolSize : 5;
            }
            set
            {
                base["MaxReadPoolSize"] = value;
            }
        }
        /// <summary>
        /// 自动启动
        /// </summary>
        [ConfigurationProperty("AutoStart", IsRequired = false, DefaultValue = true)]
        public bool AutoStart
        {
            get
            {
                return (bool)base["AutoStart"];
            }
            set
            {
                base["AutoStart"] = value;
            }
        }
        /// <summary>
        /// 本地缓存到期时间，默认单位：秒
        /// </summary>
        [ConfigurationProperty("LocalCacheTime", IsRequired = false, DefaultValue = 36000)]
        public int LocalCacheTime
        {
            get
            {
                return (int)base["LocalCacheTime"];
            }
            set
            {
                base["LocalCacheTime"] = value;
            }
        }
        /// <summary>
        /// 是否记录日志
        /// </summary>
        [ConfigurationProperty("RecordeLog", IsRequired = false, DefaultValue = false)]
        public bool RecordeLog
        {
            get
            {
                return (bool)base["RecordeLog"];
            }
            set
            {
                base["RecordeLog"] = value;
            }
        }
        [ConfigurationProperty("DefaultDb", IsRequired = false)]
        public long DefaultDb
        {
            get
            {
                return (long)base["DefaultDb"];
            }
            set
            {
                base["DefaultDb"] = value;
            }
        }
        [ConfigurationProperty("Password", IsRequired = false)]
        public string Password
        {
            get
            {
                return (string)base["Password"];
            }
            set
            {
                base["Password"] = value;
            }
        }
    }
}
