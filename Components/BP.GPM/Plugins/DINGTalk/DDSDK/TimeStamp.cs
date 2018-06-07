using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BP.EAI.Plugins.DDSDK
{
    /// <summary>  
    /// 时间戳  
    /// </summary>  
    public class TimeStamp
    {
        public static long Now()
        {
            return (long)((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
        }
        public static DateTime ToDateTime(long timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);
        }
        public static string ToDateTimeString(long timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp).ToString();
        }
        public static string ToDateTimeString(long timestamp, string format)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp).ToString(format);
        }
    }
}
