using System;
using System.Data;
using System.Web;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using System.IO;
using BP.Sys;
namespace BP.DA
{
    public class DTTemp
    {
        /// <summary>
        /// 使用C#把发表的时间改为几个月,几天前,几小时前,几分钟前,或几秒前
        ///  2008年03月15日 星期六 02:35
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string DateStringFromNow(string dt)
        {
            return DateStringFromNow(DataType.ParseSysDateTime2DateTime(dt));
        }
        /// <summary>
        /// 使用C#把发表的时间改为几个月,几天前,几小时前,几分钟前,或几秒前
        ///  2008年03月15日 星期六 02:35
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string DateStringFromNow(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.TotalDays > 60)
            {
                return dt.ToShortDateString();
            }
            else
            {
                if (span.TotalDays > 30)
                {
                    return
                    "1个月前";
                }
                else
                {
                    if (span.TotalDays > 14)
                    {
                        return
                        "2周前";
                    }
                    else
                    {
                        if (span.TotalDays > 7)
                        {
                            return
                            "1周前";
                        }
                        else
                        {
                            if (span.TotalDays > 1)
                            {
                                return
                                string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
                            }
                            else
                            {
                                if (span.TotalHours > 1)
                                {
                                    return
                                    string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
                                }
                                else
                                {
                                    if (span.TotalMinutes > 1)
                                    {
                                        return
                                        string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                                    }
                                    else
                                    {
                                        if (span.TotalSeconds >= 1)
                                        {
                                            return
                                            string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                                        }
                                        else
                                        {
                                            return
                                            "1秒前";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        //C#中使用TimeSpan计算两个时间的差值
        //可以反加两个日期之间任何一个时间单位。
        private string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            dateDiff = ts.Days.ToString() + "天" + ts.Hours.ToString() + "小时" + ts.Minutes.ToString() + "分钟" + ts.Seconds.ToString() + "秒";
            return dateDiff;
        }

        //说明：
        /**/
        /*1.DateTime值类型代表了一个从公元0001年1月1日0点0分0秒到公元9999年12月31日23点59分59秒之间的具体日期时刻。因此，你可以用DateTime值类型来描述任何在想象范围之内的时间。一个DateTime值代表了一个具体的时刻
        2.TimeSpan值包含了许多属性与方法，用于访问或处理一个TimeSpan值
        下面的列表涵盖了其中的一部分：
        Add：与另一个TimeSpan值相加。 
        Days:返回用天数计算的TimeSpan值。 
        Duration:获取TimeSpan的绝对值。 
        Hours:返回用小时计算的TimeSpan值 
        Milliseconds:返回用毫秒计算的TimeSpan值。 
        Minutes:返回用分钟计算的TimeSpan值。 
        Negate:返回当前实例的相反数。 
        Seconds:返回用秒计算的TimeSpan值。 
        Subtract:从中减去另一个TimeSpan值。 
        Ticks:返回TimeSpan值的tick数。 
        TotalDays:返回TimeSpan值表示的天数。 
        TotalHours:返回TimeSpan值表示的小时数。 
        TotalMilliseconds:返回TimeSpan值表示的毫秒数。 
        TotalMinutes:返回TimeSpan值表示的分钟数。 
        TotalSeconds:返回TimeSpan值表示的秒数。
        */
        /// <summary>
        /// 日期比较
        /// </summary>
        /// <param name="today">当前日期</param>
        /// <param name="writeDate">输入日期</param>
        /// <param name="n">比较天数</param>
        /// <returns>大于天数返回true，小于返回false</returns>
        private bool CompareDate(string today, string writeDate, int n)
        {
            DateTime Today = Convert.ToDateTime(today);
            DateTime WriteDate = Convert.ToDateTime(writeDate);
            WriteDate = WriteDate.AddDays(n);
            if (Today >= WriteDate)
                return false;

            return true;
        }
    }

}
