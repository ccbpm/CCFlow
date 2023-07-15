using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.DA;
using BP.Difference;
using BP.En;

namespace BP.Difference
{
    /// <summary>
    /// 获得IP
    /// </summary>
    public static class Glo
    {
        public static string DealExp(string exp, Hashtable ht)
        {
            if (exp == null)
                exp = string.Empty;

            exp = exp.Replace("~", "'");
            exp = exp.Replace("/#", "+"); //为什么？
            exp = exp.Replace("/$", "-"); //为什么？
            if (exp.Contains("@WebUser.No"))
                exp = exp.Replace("@WebUser.No", BP.Web.WebUser.No);

            if (exp.Contains("@WebUser.Name"))
                exp = exp.Replace("@WebUser.Name", BP.Web.WebUser.Name);

            if (exp.Contains("@WebUser.FK_DeptName"))
                exp = exp.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);

            if (exp.Contains("@WebUser.FK_Dept"))
                exp = exp.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

            if (exp.Contains("@") == true && ht != null)
            {
                foreach (string key in ht.Keys)
                {
                    //值为空或者null不替换
                    if (ht[key] == null || ht[key].Equals("") == true)
                        continue;

                    if (exp.Contains("@" + key))
                        exp = exp.Replace("@" + key, ht[key].ToString());

                    //不包含@则返回SQL语句
                    if (exp.Contains("@") == false)
                        break;
                }
            }

            if (exp.Contains("@") && BP.Difference.SystemConfig.IsBSsystem == true)
            {
                /*如果是bs*/
                foreach (string key in HttpContextHelper.RequestParamKeys)
                {
                    if (string.IsNullOrEmpty(key))
                        continue;
                    exp = exp.Replace("@" + key, HttpContextHelper.RequestParams(key));
                }
            }

            //if (exp.Contains("@") == true)
            //    throw new Exception("@外键类型SQL错误," + exp + "部分查询条件没有被替换.");

            return exp;
        }
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
