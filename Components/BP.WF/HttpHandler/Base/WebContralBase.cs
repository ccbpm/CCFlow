using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;
using System.Reflection;

namespace BP.WF.HttpHandler
{
    abstract public class WebContralBase
    {
        #region 执行方法.
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(context.Request[param], System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="obj">对象名</param>
        /// <param name="methodName">方法</param>
        /// <returns>返回执行的结果，执行错误抛出异常</returns>
        public string DoMethod(WebContralBase myEn, string methodName)
        {

            Type tp = myEn.GetType();
            MethodInfo mp = tp.GetMethod(methodName);

            if (mp == null)
            {
                /* 没有找到方法名字，就执行默认的方法. */
                return myEn.DoDefaultMethod();
            }

            //执行该方法.
            object[] paras = null;
            return mp.Invoke(this, paras) as string;  //调用由此 MethodInfo 实例反射的方法或构造函数。

        }
        /// <summary>
        /// 执行默认的方法名称
        /// </summary>
        /// <returns>返回执行的结果</returns>
        protected virtual string DoDefaultMethod()
        {
            return "@子类没有重写该["+this.DoType+"]方法.";
        }
        #endregion 执行方法.

        #region 公共方法.
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string GetRequestVal(string param)
        {
            string val = context.Request[param];
            if (val == null)
                val = context.Request.QueryString[param];

            return HttpUtility.UrlDecode(val, System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public int GetRequestValInt(string param)
        {
            string str = GetRequestVal(param);
            if (str == null || str == "" || str == "null")
                return 0;
            try
            {
                return int.Parse(str);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Int64 GetRequestValInt64(string param)
        {
            string str = GetRequestVal(param);
            if (str == null || str == "" || str == "null")
                return 0;
            try
            {
                return Int64.Parse(str);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public float GetRequestValFloat(string param)
        {
            string str = GetRequestVal(param);
            if (str == null || str == "" || str == "null")
                return 0;
            try
            {
                return float.Parse(str);
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 获得参数.
        /// </summary>
        public string RequestParas
        {
            get
            {
                string urlExt = "";
                string rawUrl = this.context.Request.RawUrl;
                rawUrl = "&" + rawUrl.Substring(rawUrl.IndexOf('?') + 1);
                string[] paras = rawUrl.Split('&');
                foreach (string para in paras)
                {
                    if (para == null
                        || para == ""
                        || para.Contains("=") == false)
                        continue;

                    if (para == "1=1")
                        continue;

                    urlExt += "&" + para;
                }
                return urlExt;
            }
        }
        #endregion

        #region 属性参数.
        /// <summary>
        /// 编号
        /// </summary>
        public string No
        {
            get
            {
                string str = context.Request.QueryString["No"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 执行类型
        /// </summary>
        public string DoType
        {
            get
            {
                //获得执行的方法.
                string doType = "";

                doType = this.GetRequestVal("DoType");
                if (doType == null)
                    doType = this.GetRequestVal("Action");

                if (doType == null)
                    doType = this.GetRequestVal("action");

                if (doType == null)
                    doType = this.GetRequestVal("Method");

                return doType;
            }
        }
        public string EnsName
        {
            get
            {
                string str = this.GetRequestVal("EnsName");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string MyPK
        {
            get
            {
                string str = this.GetRequestVal("MyPK");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 字典表
        /// </summary>
        public string FK_SFTable
        {
            get
            {
                string str = this.GetRequestVal("FK_SFTable");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string EnumKey
        {
            get
            {
                string str = this.GetRequestVal("EnumKey");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string KeyOfEn
        {
            get
            {
                string str = this.GetRequestVal("KeyOfEn");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
                 
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                string str = this.GetRequestVal("FK_MapData");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 扩展信息
        /// </summary>
        public string FK_MapExt
        {
            get
            {
                string str = this.GetRequestVal("FK_MapExt");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                string str = this.GetRequestVal("FK_Flow");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public int GroupField
        {
            get
            {
                string str = this.GetRequestVal("GroupField");
                if (str == null || str == "" || str == "null")
                    return 0;

                return int.Parse( str);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetRequestValInt("FK_Node");
            }
        }
        public Int64 FID
        {
            get
            {
                return this.GetRequestValInt("FID");

                string str = context.Request.QueryString["FID"];
                if (str == null || str == "" || str == "null")
                    return 0;
                return int.Parse(str);
            }
        }
        public Int64 WorkID
        {
            get
            {
                string str = context.Request.QueryString["WorkID"];
                if (str == null || str == "" || str == "null")
                    return 0;
                return int.Parse(str);
            }
        }
        /// <summary>
        /// 框架ID
        /// </summary>
        public string FK_MapFrame
        {
            get
            {
                string str = context.Request.QueryString["FK_MapFrame"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        ///   RefOID
        /// </summary>
        public int RefOID
        {
            get
            {
                string str = context.Request.QueryString["RefOID"];

                if (str == null || str == "" || str == "null")
                    str= context.Request.QueryString["OID"];

                if (str == null || str == "" || str == "null")
                    return 0;

                return int.Parse(str);
            }
        }
        /// <summary>
        /// 明细表
        /// </summary>
        public string FK_MapDtl
        {
            get
            {
                string str = context.Request.QueryString["FK_MapDtl"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }

        /// <summary>
        /// 字段属性编号
        /// </summary>
        public string Ath
        {
            get
            {
                string str = context.Request.QueryString["Ath"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }

        public HttpContext context = null;
        /// <summary>
        /// 获得表单的属性.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValFromFrmByKey(string key)
        {
            string val = context.Request.Form[key];
            if (val == null)
                return null;
            val = val.Replace("'", "~");
            return val;
        }
        public int GetValIntFromFrmByKey(string key)
        {
            string str = this.GetValFromFrmByKey(key);
            if (str == null || str == "")
                throw new Exception("@参数:" + key + " 没有取到值.");

            return int.Parse(str);
        }
        public bool GetValBoolenFromFrmByKey(string key)
        {
            string val = this.GetValFromFrmByKey(key);
            if (val == null || val == "")
                return false;
            return true;
        }
       
        public new string RefPK
        {
            get
            {
                string str = this.context.Request.QueryString["RefPK"];
                return str;
            }
        }
        public string RefPKVal
        {
            get
            {
                string str = this.context.Request.QueryString["RefPKVal"];
                if (str == null)
                    return "1";
                return str;
            }
        }
        #endregion 属性.

    }
}
