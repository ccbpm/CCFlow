using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.DA;
using BP.Difference;

namespace BP.Difference
{
    /// <summary>
    /// 获得IP
    /// </summary>
    public static class Glo
    {
        public static string DealSQLStringEnumFormat(string cfgString)
        {
            //把这个string,转化SQL. @tuanyuan=团员@dangyuan=党员
            AtPara ap = new AtPara(cfgString);
            string sql = "";
            foreach (string item in ap.HisHT.Keys)
            {
                sql += " SELECT '" + item + "' as No, '" + ap.GetValStrByKey(item) + "' as Name FROM Port_Emp WHERE No = 'admin' UNION ";
            }
            sql = sql.Substring(0, sql.Length - 6);
            return sql;
        }
        /// <summary>
        /// 获得ID地址
        /// </summary>
        public static string GetIP
        {
            get
            {
                return HttpContextHelper.Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
        }
        public static string RequestParas
        {
            get
            {
                string urlExt = "";
                string rawUrl = System.Web.HttpContext.Current.Request.RawUrl;
                rawUrl = "&" + rawUrl.Substring(rawUrl.IndexOf('?') + 1);
                string[] paras = rawUrl.Split('&');
                foreach (string para in paras)
                {
                    if (para == null
                        || para == ""
                        || para.Contains("=") == false)
                        continue;
                    urlExt += "&" + para;
                }
                return urlExt;
            }
        }
    }
}
