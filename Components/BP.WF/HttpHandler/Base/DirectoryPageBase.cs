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
    abstract public class DirectoryPageBase
    {
        #region 执行方法.
        /// <summary>
        /// 获得Form数据.
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>返回值</returns>
        public string GetValFromFrmByKey(string key, string isNullAsVal = null)
        {
            string val = context.Request.Form[key];

            if (val == null && key.Contains("DDL_") == false)
            {
                val = context.Request.Form["DDL_" + key];
            }

            if (val == null && key.Contains("TB_") == false)
            {
                val = context.Request.Form["TB_" + key];
            }

            if (val == null && key.Contains("CB_") == false)
            {
                val = context.Request.Form["CB_" + key];
            }

            if (val == null)
            {
                if (isNullAsVal != null)
                    return isNullAsVal;
                throw new Exception("@获取Form参数错误,参数集合不包含[" + key + "]");
            }

            val = val.Replace("'", "~");
            return val;
        }
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="obj">对象名</param>
        /// <param name="methodName">方法</param>
        /// <returns>返回执行的结果，执行错误抛出异常</returns>
        public string DoMethod(DirectoryPageBase myEn, string methodName)
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
            return "err@子类[" + this.ToString() + "]没有重写该[" + this.DoType + "]方法，请确认该方法是否缺少或者是非public类型的.";
        }
        #endregion 执行方法.

        #region 公共方法.
        /// <summary>
        /// 公共方法获取值
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string GetRequestVal(string key)
        {
            string val = context.Request[key];
            if (val == null)
                val = context.Request.QueryString[key];
            if (val == null)
                val = context.Request.Form[key];

            if (val == null)
                return null;

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
            if (str == null || str == "" || str == "null" || str == "undefined")
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
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public bool GetRequestValBoolen(string param)
        {
            if (this.GetRequestValInt(param) == 1)
                return true;
            return false;
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
        public decimal GetRequestValDecimal(string param)
        {
            string str = GetRequestVal(param);
            if (str == null || str == "" || str == "null")
                return 0;
            try
            {
                return decimal.Parse(str);
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
        /// 
        /// </summary>
        public string PKVal
        {
            get
            {
                string str = this.GetRequestVal("PKVal");


                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("OID");

                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("No");

                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("MyPK");
                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("NodeID");

                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("WorkID");

                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("PK");

                if ("null".Equals(str) == true)
                    return null;

                return str;
            }
        }
        /// <summary>
        /// 是否是移动？
        /// </summary>
        public bool IsMobile
        {
            get
            {
                string v = this.GetRequestVal("IsMobile");
                if (v != null && v == "1")
                    return true;

                if (System.Web.HttpContext.Current.Request.RawUrl.Contains("/CCMobile/") == true)
                    return true;

                return false;
            }
        }
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
        public string Name
        {
            get
            {
                string str = context.Request.QueryString["Name"];
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
        public string EnName
        {
            get
            {
                string str = this.GetRequestVal("EnName");

                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("FK_MapData");

                if (str == null || str == "" || str == "null")
                    return null;

                return str;
            }
        }
        public string EnsName
        {
            get
            {
                string str = this.GetRequestVal("EnsName");

                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("FK_MapData");

                if (str == null || str == "" || str == "null")
                    return null;

                return str;
            }
        }
        public string FK_Dept
        {
            get
            {
                string str = this.GetRequestVal("FK_Dept");
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
        public string FK_Event
        {
            get
            {
                string str = this.GetRequestVal("FK_Event");
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
                    str = this.GetRequestVal("FrmID");
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
                {
                    str = this.GetRequestVal("MyPK");
                    if (str == null || str == "" || str == "null")
                    {
                        return null;
                    }
                }


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
        /// <summary>
        /// 人员编号
        /// </summary>
        public string FK_Emp
        {
            get
            {
                string str = this.GetRequestVal("FK_Emp");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                string str = this.GetRequestVal("FrmID");
                if (str == null || str == "" || str == "null")
                    return this.GetRequestVal("FK_MapData");

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

                return int.Parse(str);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                int nodeID = this.GetRequestValInt("FK_Node");
                if (nodeID == 0)
                    nodeID = this.GetRequestValInt("NodeID");
                return nodeID;
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
        public Int64 PWorkID
        {
            get
            {
                return this.GetRequestValInt("PWorkID");

                string str = context.Request.QueryString["PWorkID"];
                if (str == null || str == "" || str == "null")
                    return 0;
                return int.Parse(str);
            }
        }
        private Int64 _workID = 0;
        public Int64 WorkID
        {
            get
            {
                if (_workID != 0)
                    return _workID;

                string str = this.GetRequestVal("WorkID");
                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("PKVal");

                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("OID");

                if (str == null || str == "" || str == "null")
                    return 0;

                return int.Parse(str);
            }
            set
            {
                _workID = value;
            }
        }
        public Int64 CWorkID
        {
            get
            {
                return this.GetRequestValInt("CWorkID");
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
        /// SID
        /// </summary>
        public string SID
        {
            get
            {
                string str = context.Request.QueryString["SID"];
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
                    str = context.Request.QueryString["OID"];

                if (str == null || str == "" || str == "null")
                    return 0;

                return int.Parse(str);
            }
        }
        public int OID
        {
            get
            {
                string str = context.Request.QueryString["RefOID"];
                if (str == null || str == "" || str == "null")
                    str = context.Request.QueryString["OID"];

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
                    str = context.Request.QueryString["EnsName"];
                return str;
            }
        }
        /// <summary>
        /// 页面Index.
        /// </summary>
        public int PageIdx
        {
            get
            {
                return this.GetRequestValInt("PageIdx");
            }
        }
        public int Index
        {
            get
            {
                return this.GetRequestValInt("Index");
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
        /// 获得Int数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetValIntFromFrmByKey(string key)
        {
            string str = this.GetValFromFrmByKey(key);
            if (str == null || str == "")
                throw new Exception("@参数:" + key + "没有取到值.");
            return int.Parse(str);
        }

        public float GetValFloatFromFrmByKey(string key)
        {
            string str = this.GetValFromFrmByKey(key);
            if (str == null || str == "")
                throw new Exception("@参数:" + key + "没有取到值.");
            return float.Parse(str);
        }
        public decimal GetValDecimalFromFrmByKey(string key)
        {
            string str = this.GetValFromFrmByKey(key);
            if (str == null || str == "")
                throw new Exception("@参数:" + key + "没有取到值.");
            return decimal.Parse(str);
        }

        public bool GetValBoolenFromFrmByKey(string key)
        {

            string val = this.GetValFromFrmByKey(key, "0");
            if (val == "on" || val == "1")
                return true;
            if (val == null || val == "" || val == "0" || val == "off")
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
                    return "0";
                return str;
            }
        }
        #endregion 属性.

    }
}
