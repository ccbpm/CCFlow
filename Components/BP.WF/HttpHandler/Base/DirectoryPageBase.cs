using System;
using System.Collections;
using System.Web;
using BP.Difference;
using BP.Web;
using BP.DA;
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
            string val = HttpContextHelper.RequestParams(key);
            if (val == null && key.Contains("DDL_") == false)
                val = HttpContextHelper.RequestParams("DDL_" + key);

            if (val == null && key.Contains("TB_") == false)
                val = HttpContextHelper.RequestParams("TB_" + key);

            if (val == null && key.Contains("CB_") == false)
                val = HttpContextHelper.RequestParams("CB_" + key);

            if (val == null)
            {
                if (isNullAsVal != null)
                    return isNullAsVal;
                return "";
                //throw new Exception("@获取Form参数错误,参数集合不包含[" + key + "]");
            }

            val = val.Replace("'", "~");
            return val;
        }
        /// <summary>
        /// token for guangxi jisuanzhongxin.
        /// 1. 手机端连接服务需要,身份验证,需要token.
        /// 2. 在全局中配置 TokenHost 地址, 每次调用服务都需要传入Token 参数.
        /// 3. 如果不配置 TokenHost 就提示错误.
        /// 4. 仅仅在会话信息丢失后，在调用该方法.
        /// </summary>
        /// <param name="token">获得token.</param>
        /// <returns></returns>
        public string DealToken(DirectoryPageBase page, string mothodName)
        {
            string token = page.GetRequestVal("Token");
            if (DataType.IsNullOrEmpty(token) == true)
                return null;

            string host = BP.Difference.SystemConfig.GetValByKey("TokenHost", null);
            //根据token直接登录
            if (DataType.IsNullOrEmpty(host) == true)
            {
                BP.WF.Dev2Interface.Port_LoginByToken(token);
                return "";
            }


            token = token.Split(',')[0];
            string url = host + token;
            string data = DataType.ReadURLContext(url, 5000);

            if (DataType.IsNullOrEmpty(data) == true)
                throw new Exception("err@token失效，请重新登录。" + url + "");

            BP.Port.Emp emp = new BP.Port.Emp();
            emp.UserID = data;
            if (emp.RetrieveFromDBSources() == 0)
                throw new Exception("err@根据token获取用户名错误:" + token + ",获取数据为:" + data);

            //执行登录.
            BP.WF.Dev2Interface.Port_Login(data);
            return "info@登录成功.";
        }
        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="obj">对象名</param>
        /// <param name="methodName">方法</param>
        /// <returns>返回执行的结果，执行错误抛出异常</returns>
        public string DoMethod(DirectoryPageBase myEn, string methodName)
        {

            if (methodName.Contains(">") == true)
                return "err@非法的脚本植入.";
            //在用户名为空的情况下处理token.
            if (WebUser.No == null)
            {
                bool isCanDealToken = true;

                if (myEn.DoType.Contains("Login") == true)
                    isCanDealToken = false;

                if (myEn.DoType.Contains("Index") == true)
                    isCanDealToken = false;

                if (myEn.ToString().Contains("Admin") == true)
                    isCanDealToken = false;

                if (isCanDealToken == true)
                    this.DealToken(myEn, myEn.DoType);
            }

            //if (WebUser.IsAdmin == false)
            //{ 
            //}
            string clsID = myEn.ToString();

            //权限判断,管理员.
            if ((clsID.Contains("Admin_")
                && clsID.Contains("WF_Admin_TestingContainer") == false
                 && clsID.Contains("WF_Admin_DevelopDesigner") == false)
                || clsID.Contains("GPMPage")
               )
            {
                if (BP.Web.WebUser.IsAdmin == false)
                    throw new Exception("err@非管理员用户,无法执行:" + clsID + "类.UserNo=" + WebUser.No);
            }

            try
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
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    if (ex.InnerException.Message.IndexOf("err@") == 0)
                        return ex.InnerException.Message;
                    else
                    {
                        string msg = "err@调用类:[EnName=" + myEn.EnName + " - EnsName=" + myEn.EnsName + "]";
                        msg += "方法:[" + myEn.GetRequestVal("MethodName") + " " + methodName + "]";
                        msg += "主键值:[" + myEn.PKVal + "]";
                        msg += "出现错误:" + ex.InnerException;
                        return msg;
                    }
                else
                    if (ex.Message.IndexOf("err@") == 0)
                    return ex.Message;
                else
                    return "err@调用类:[" + myEn + "]方法:[" + methodName + "]出现错误:" + ex.Message;
            }
        }
        /// <summary>
        /// 执行默认的方法名称
        /// </summary>
        /// <returns>返回执行的结果</returns>
        protected virtual string DoDefaultMethod()
        {
            if (this.DoType.Contains(">") == true)
                return "err@非法的脚本植入.";

            return "err@子类[" + this.ToString() + "]没有重写该[" + this.GetRequestVal("DoMethod") + "]方法，请确认该方法是否缺少或者是非public类型的.";
        }
        #endregion 执行方法.

        #region 公共方法.
        public Hashtable ht = null;
        public void AddPara(string key, string val)
        {
            if (ht == null)
                ht = new Hashtable();
            ht.Add(key, val);
        }
        public string GetRequestVal(string key)
        {
            if (ht != null && ht.ContainsKey(key))
            {
                string myval = ht[key] as string;
                return HttpUtility.UrlDecode(myval, System.Text.Encoding.UTF8);
            }

            string val = HttpContextHelper.RequestQueryString(key);
            if (val == null)
            {
                val = HttpContextHelper.RequestParams(key);

                if (val == null)
                    return null;
            }
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
        public int GetRequestValChecked(string param)
        {
            string str = GetRequestVal(param);
            if (str == null || str == "" || str == "null" || str == "undefined")
                return 0;

            return 1;
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

                // 适配framework和core（注：net core的rawurl中不含form data）
                foreach (string key in HttpContextHelper.RequestParamKeys)
                {
                    if (key.Equals("1") == true || key.Equals("t") == true || key.Equals("T") == true) // 过滤url中1=1的情形
                        continue;

                    string value = HttpContextHelper.RequestParams(key);
                    if (!String.IsNullOrEmpty(value))
                        urlExt += string.Format("&{0}={1}", key, value);

                }
                return urlExt;
            }
        }
        /// <summary>
        /// 所有的paras
        /// </summary>
        public string RequestParasOfAll
        {
            get
            {
                string urlExt = "";
                string rawUrl = HttpContextHelper.RequestRawUrl;
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


                    if (para.Contains("DoType=")
                       || para.Contains("DoMethod=")
                       || para.ToLower().Equals("t")
                       || para.Contains("HttpHandlerName="))
                        continue;

                    urlExt += "&" + para;
                }


                foreach (string key in HttpContextHelper.RequestParamKeys)
                {

                    if (key.Equals("DoType")
                        || key.Equals("DoMethod")
                        || key.ToLower().Equals("t")
                        || key.Equals("HttpHandlerName"))
                        continue;
                    if (urlExt.Contains("&" + key + "=") == false)
                        urlExt += "&" + key + "=" + HttpContextHelper.RequestParams(key);
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
        public bool ItIsMobile
        {
            get
            {
                string v = this.GetRequestVal("IsMobile");
                if (v != null && v == "1")
                    return true;

                if (HttpContextHelper.RequestRawUrl.Contains("/CCMobile/") == true)
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
                string str = this.GetRequestVal("No"); // context.Request.QueryString["No"];
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string Name
        {
            get
            {
                string str = this.GetRequestVal("Name");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string UserNo
        {
            get
            {
                string str = this.GetRequestVal("UserNo");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }
        }
        public string DoWhat
        {
            get
            {
                string str = this.GetRequestVal("DoWhat");
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
                if (String.IsNullOrEmpty(doType))
                    doType = this.GetRequestVal("Action");

                if (String.IsNullOrEmpty(doType))
                    doType = this.GetRequestVal("action");

                if (String.IsNullOrEmpty(doType))
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
        /// <summary>
        /// 类名
        /// </summary>
        public string EnsName
        {
            get
            {
                string str = this.GetRequestVal("EnsName");

                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("FK_MapData");

                if (str == null || str == "" || str == "null")
                    str = this.GetRequestVal("FrmID");

                if (str == null || str == "" || str == "null")
                {
                    if (this.EnName == null)
                        return null;
                    return this.EnName + "s";
                }
                return str;
            }
        }

        /// <summary>
        /// 树形结构的类名
        /// </summary>
        public string TreeEnsName
        {
            get
            {
                string str = this.GetRequestVal("TreeEnsName");
                if (str == null || str == "" || str == "null")
                {
                    if (this.EnName == null)
                        return null;
                    return this.EnName + "s";
                }
                return str;
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string DeptNo
        {
            get
            {
                string str = this.GetRequestVal("FK_Dept");
                if (str == null || str == "" || str == "null")
                    return null;
                return str;
            }

        }
        /// <summary>
        /// 主键
        /// </summary>
        public string MyPK
        {
            get
            {
                string str = this.GetRequestVal("MyPK");
                if (DataType.IsNullOrEmpty(str))
                    return null;
                return str;
            }
        }
        public string FK_Event
        {
            get
            {
                string str = this.GetRequestVal("FK_Event");
                if (DataType.IsNullOrEmpty(str))
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
                if (DataType.IsNullOrEmpty(str))
                    return null;
                return str;
            }
        }
        public string EnumKey
        {
            get
            {
                string str = this.GetRequestVal("EnumKey");
                if (DataType.IsNullOrEmpty(str))
                    return null;
                return str;
            }
        }
        public string Key
        {
            get
            {
                string str = this.GetRequestVal("Key");
                if (DataType.IsNullOrEmpty(str))
                    return null;
                return str;
            }
        }
        public string KeyOfEn
        {
            get
            {
                string str = this.GetRequestVal("KeyOfEn");
                if (DataType.IsNullOrEmpty(str))
                    return null;
                return str;
            }
        }
        public string Vals
        {
            get
            {
                string str = this.GetRequestVal("Vals");
                if (DataType.IsNullOrEmpty(str))
                    return null;
                return str;
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FrmID
        {
            get
            {
                string str = this.GetRequestVal("FK_MapData");
                if (DataType.IsNullOrEmpty(str))
                    str = this.GetRequestVal("FrmID");
                if (DataType.IsNullOrEmpty(str))
                    str = this.GetRequestVal("EnsName");
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
                if (DataType.IsNullOrEmpty(str))
                {
                    str = this.GetRequestVal("MyPK");
                    if (DataType.IsNullOrEmpty(str) == true)
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
        public string FlowNo
        {
            get
            {
                string str = this.GetRequestVal("FK_Flow");
                if (str == null)
                    str = this.GetRequestVal("FlowNo");
                if (str == null || str == "" || str == "null")
                    return null;
                if (DataType.IsNumStr(str) == false)
                    return "err@";
                
                return BP.WF.Dev2Interface.Flow_TurnFlowMark2FlowNo(str);
            }
        }
        /// <summary>
        /// 人员编号
        /// </summary>
        public string EmpNo
        {
            get
            {
                string str = this.GetRequestVal("FK_Emp");
                if (DataType.IsNullOrEmpty(str) == true)
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 域
        /// </summary>
        public string Domain
        {
            get
            {
                string str = this.GetRequestVal("Domain");
                if (DataType.IsNullOrEmpty(str) == true)
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 相关编号
        /// </summary>
        public string RefNo
        {
            get
            {
                string str = this.GetRequestVal("RefNo");
                if (DataType.IsNullOrEmpty(str) == true)
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                string str = this.GetRequestVal("OrgNo");
                if (DataType.IsNullOrEmpty(str) == true)
                    return null;
                return str;
            }
        }
        public int GroupField
        {
            get
            {
                string str = this.GetRequestVal("GroupField");
                if (DataType.IsNullOrEmpty(str) == true)
                    return 0;

                return int.Parse(str);
            }
        }
        private int _nodeID = 0;
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                if (_nodeID != 0)
                    return _nodeID;

                int nodeID = this.GetRequestValInt("FK_Node");
                if (nodeID == 0)
                    nodeID = this.GetRequestValInt("NodeID");
                return nodeID;
            }
            set
            {
                _nodeID = value;
            }
        }
        public int ToNodeID
        {
            get
            {
                return this.GetRequestValInt("ToNodeID");
            }
        }
        public Int64 FID
        {
            get
            {
                return this.GetRequestValInt("FID");

                string str = this.GetRequestVal("FID");//  context.Request.QueryString["FID"];
                if (DataType.IsNullOrEmpty(str) == true)
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
                if (DataType.IsNullOrEmpty(str) == true)
                {
                    str = this.GetRequestVal("PKVal"); //@hontyan. 这个方法都要修改.
                    if (DataType.IsNullOrEmpty(str) == true)
                        str = this.GetRequestVal("OID");
                }
                if (DataType.IsNullOrEmpty(str) == true)
                    return 0;

                return int.Parse(str);
            }
            set
            {
                _workID = value;
            }
        }

        public string WorkIDStr
        {
            get
            {
                string val = this.GetRequestVal("WorkID");
                if (DataType.IsNullOrEmpty(val) == true)
                    val = this.GetRequestVal("OID");
                if (DataType.IsNullOrEmpty(val) == true)
                    val = this.GetRequestVal("PKVal");
                return val;
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
        /// SID
        /// </summary>
        public string SID
        {
            get
            {
                string str = this.GetRequestVal("Token"); // context.Request.QueryString["Token"];
                if (DataType.IsNullOrEmpty(str) == true)
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
                string str = this.GetRequestVal("RefOID"); //context.Request.QueryString["RefOID"];

                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("OID"); //  context.Request.QueryString["OID"];

                if (DataType.IsNullOrEmpty(str) == true)
                    return 0;

                return int.Parse(str);
            }
        }
        public int OID
        {
            get
            {
                string str = this.GetRequestVal("RefOID"); // context.Request.QueryString["RefOID"];
                if (DataType.IsNullOrEmpty(str) == true || str.Equals("undefined"))
                    str = this.GetRequestVal("OID");  //context.Request.QueryString["OID"];

                if (DataType.IsNullOrEmpty(str) == true)
                    return 0;

                return int.Parse(str);
            }
        }
        /// <summary>
        /// 明细表
        /// </summary>
        public string MapDtlNo
        {
            get
            {
                string str = this.GetRequestVal("FK_MapDtl"); //context.Request.QueryString["FK_MapDtl"];
                if (DataType.IsNullOrEmpty(str) == true)
                    str = this.GetRequestVal("EnsName");// context.Request.QueryString["EnsName"];
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
                int i = this.GetRequestValInt("PageIdx");
                if (i == 0)
                    return 1;
                return i;
            }
        }
        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize
        {
            get
            {
                int i = this.GetRequestValInt("PageSize");
                if (i == 0)
                    return 10;
                return i;
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
                string str = this.GetRequestVal("Ath");// context.Request.QueryString["Ath"];
                if (DataType.IsNullOrEmpty(str) == true)
                    return null;
                return str;
            }
        }

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
                return this.GetRequestVal("RefPK");

                //string str = this.context.Request.QueryString["RefPK"];
                //return str;
            }
        }
        public string RefPKVal
        {
            get
            {
                string str = this.GetRequestVal("RefPKVal");
                if (str == null)
                    return "0";
                return str;
            }
        }

        /// <summary>
        /// 部门编号
        /// </summary>
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
        #endregion 属性.

        #region 父子流程相关的属性.
        public Int64 PWorkID
        {
            get
            {
                return this.GetRequestValInt64("PWorkID");
            }
        }
        public Int64 PFID
        {
            get
            {
                return this.GetRequestValInt64("PFID");
            }
        }
        public int PNodeID
        {
            get
            {
                return this.GetRequestValInt("PNodeID");
            }
        }
        public string PFlowNo
        {
            get
            {
                return this.GetRequestVal("PFlowNo");
            }
        }
        #endregion 父子流程相关的属性.
    }
}
