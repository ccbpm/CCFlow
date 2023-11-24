using System.Security.Cryptography;
using System;
using BP.En;
using BP.DA;
using System.Configuration;
using BP.Port;
using BP.Sys;
using BP.Pub;
using System.Collections.Generic;
using BP.Difference;
using System.Web;
using System.Collections;
using Microsoft.Expression.Interactivity.Media;

namespace BP.Web
{
    /// <summary>
    /// User 的摘要说明。
    /// </summary>
    public class WebUser
    {
        /// <summary>
        /// 密码解密
        /// </summary>
        /// <param name="pass">用户输入密码</param>
        /// <returns>解密后的密码</returns>
        public static string ParsePass(string pass)
        {
            if (pass == "")
                return "";

            string str = "";
            char[] mychars = pass.ToCharArray();
            int i = 0;
            foreach (char c in mychars)
            {
                i++;

                //step 1 
                long A = Convert.ToInt64(c) * 2;

                // step 2
                long B = A - i * i;

                // step 3 
                long C = 0;
                if (B > 196)
                    C = 196;
                else
                    C = B;

                str = str + Convert.ToChar(C).ToString();
            }
            return str;
        }
        /// <summary>
        /// 更改一个人当前登录的主要部门
        /// 再一个人有多个部门的情况下有效.
        /// </summary>
        /// <param name="empNo">人员编号</param>
        /// <param name="fk_dept">当前所在的部门.</param>
        public static void ChangeMainDept(string empNo, string fk_dept)
        {
            //这里要考虑集成的模式下，更新会出现是.

            string sql = BP.Difference.SystemConfig.GetValByKey("UpdataMainDeptSQL", "");
            if (sql == "")
            {
                /*如果没有配置, 就取默认的配置.*/
                sql = "UPDATE Port_Emp SET FK_Dept=@FK_Dept WHERE No=@No";
            }

            sql = sql.Replace("@FK_Dept", "'" + fk_dept + "'");
            sql = sql.Replace("@No", "'" + empNo + "'");

            try
            {
                if (sql.Contains("UPDATE Port_Emp SET FK_Dept=") == true)
                    if (DBAccess.IsView("Port_Emp", BP.Difference.SystemConfig.AppCenterDBType) == true)
                        return;
                DBAccess.RunSQL(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("@执行更改当前操作员的主部门的时候错误,请检查SQL配置:" + ex.Message);
            }
        }
        /// <summary>
        /// 通用的登陆
        /// </summary>
        /// <param name="em">人员</param>
        /// <param name="lang">语言</param>
        /// <param name="auth">授权人</param>
        /// <param name="isRememberMe">是否记录cookies</param>
        /// <param name="IsRecSID">是否记录SID</param>
        public static void SignInOfGener(Emp emp, string lang = "CH", bool isRememberMe = false, bool IsRecSID = false,
            string authNo = null, string authName = null, bool isSSO = false)
        {
            if (HttpContextHelper.Current == null)
                SystemConfig.isBSsystem = false;
            else
                SystemConfig.isBSsystem = true;

            WebUser.No = emp.UserID;
            WebUser.Name = emp.Name;

            if (DataType.IsNullOrEmpty(authNo) == false)
            {
                WebUser.Auth = authNo; //被授权人，实际工作的执行者.
                WebUser.AuthName = authName;
            }
            else
            {
                WebUser.Auth = null;
                WebUser.AuthName = null;
            }

            //解决没有部门编号的问题.
            if (DataType.IsNullOrEmpty(emp.OrgNo) == false && DataType.IsNullOrEmpty(emp.DeptNo) == true)
            {
                BP.Port.DeptEmp de = new BP.Port.DeptEmp();
                de.DeptNo = emp.OrgNo;
                de.EmpNo = emp.No;
                de.OrgNo = emp.OrgNo;
                de.Insert();
                // emp.DeptNo = em.OrgNo;
            }


            #region 解决部门的问题.
            if (DataType.IsNullOrEmpty(emp.DeptNo) == true)
            {
                string sql = "";

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    sql = "SELECT FK_Dept FROM Port_DeptEmp WHERE FK_Emp='" + emp.UserID + "' AND OrgNo='" + WebUser.OrgNo + "' ";
                else
                    sql = "SELECT FK_Dept FROM Port_DeptEmp WHERE FK_Emp='" + emp.UserID + "'";

                string deptNo = DBAccess.RunSQLReturnString(sql);
                if (DataType.IsNullOrEmpty(deptNo) == true)
                {
                    if (emp.No.Equals("Guest") == true)
                    {
                        if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                        {
                            BP.Port.DeptEmp de = new BP.Port.DeptEmp();
                            de.DeptNo = "ccs";
                            de.EmpNo = "Guest";
                            de.Insert();
                        }
                    }
                    else
                    {
                        if (DataType.IsNullOrEmpty(deptNo) == true)
                            throw new Exception("@登录人员(" + emp.UserID + "," + emp.Name + ")没有维护部门." + sql);
                    }
                }
                else
                {
                    //调用接口更改所在的部门.
                    WebUser.ChangeMainDept(emp.UserID, deptNo);
                    emp.DeptNo = deptNo;
                }
            }

            BP.Port.Dept dept = new Dept();
            dept.No = emp.DeptNo;
            if (dept.RetrieveFromDBSources() == 0)
                throw new Exception("@登录人员(" + emp.UserID + "," + emp.Name + ")没有维护部门,或者部门编号{" + emp.DeptNo + "}不存在.");
            #endregion 解决部门的问题.

            WebUser.DeptNo = emp.DeptNo;
            WebUser.DeptName = dept.Name;
            WebUser.DeptParentNo = dept.ParentNo;
            WebUser.OrgNo = dept.OrgNo;
            WebUser.SysLang = lang;
            if (BP.Difference.SystemConfig.isBSsystem)
            {
                // cookie操作，为适应不同平台，统一使用HttpContextHelper
                Dictionary<string, string> cookieValues = new Dictionary<string, string>();

                cookieValues.Add("No", emp.UserID);
                cookieValues.Add("Name", HttpUtility.UrlEncode(emp.Name));

                if (isRememberMe)
                    cookieValues.Add("IsRememberMe", "1");
                else
                    cookieValues.Add("IsRememberMe", "0");

                cookieValues.Add("FK_Dept", emp.DeptNo);
                cookieValues.Add("FK_DeptName", HttpUtility.UrlEncode(emp.DeptText));

                //设置组织编号.
                if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    cookieValues.Add("OrgNo", emp.OrgNo);
             

                cookieValues.Add("Tel", emp.Tel);
                cookieValues.Add("Lang", lang);
                if (authNo == null)
                    authNo = "";
                cookieValues.Add("Auth", authNo); //授权人.

                if (authName == null)
                    authName = "";
                if (isSSO)
                {
                    cookieValues.Add("LoginType", "SSO");
                }
                cookieValues.Add("AuthName", authName); //授权人名称..
                cookieValues.Add("CCBPMRunModel", ((int)SystemConfig.CCBPMRunModel).ToString());
                cookieValues.Add("AppCenterDBType", ((int)SystemConfig.AppCenterDBType).ToString());
                cookieValues.Add("CustomName", SystemConfig.CustomerName);
                cookieValues.Add("CustomNo", SystemConfig.CustomerNo);
                HttpContextHelper.ResponseCookieAdd(cookieValues, null, "CCS");
            }
        }

        #region 静态方法
        /// <summary>
        /// 通过key,取出session.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="isNullAsVal">如果是Null, 返回的值.</param>
        /// <returns></returns>
        public static string GetSessionByKey(string key, string isNullAsVal)
        {
            //2019-07-25 zyt改造
            if (IsBSMode && HttpContextHelper.Current != null && HttpContextHelper.Current.Session != null)
            {
                string str = HttpContextHelper.SessionGetString(key);
                if (DataType.IsNullOrEmpty(str))
                    str = isNullAsVal;
                return str;
            }
            else
            {
                if (BP.Pub.Current.Session[key] == null || BP.Pub.Current.Session[key].ToString() == "")
                {
                    BP.Pub.Current.Session[key] = isNullAsVal;
                    return isNullAsVal;
                }
                else
                    return (string)BP.Pub.Current.Session[key];
            }
        }
        #endregion

        /// <summary>
        /// 是不是b/s 工作模式。
        /// </summary>
        protected static bool IsBSMode
        {
            get
            {
                if (HttpContextHelper.Current == null)
                    return false;
                else
                    return true;
            }
        }
        /// <summary>
        /// 设置session
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        public static void SetSessionByKey(string key, string val)
        {
            if (val == null)
                return;

            //2019-07-25 zyt 改造.
            if (IsBSMode == true
                && HttpContextHelper.Current != null
                && HttpContextHelper.Current.Session != null)
            {
                HttpContextHelper.SessionSet(key, val);
            }
            else
            {
                BP.Pub.Current.SetSession(key, val);
            }
        }
        /// <summary>
        /// 退回
        /// </summary>
        public static void Exit()
        {

            string guid = DBAccess.GenerGUID();

            //Token信息存储在WF_Emp的AtPara表中了，清空Token
            string sql = "UPDATE WF_Emp SET AtPara = REPLACE(AtPara, '@Token_PC=" + BP.Web.WebUser.Token + "', '@Token_PC=" + guid + "') WHERE No = '" + BP.Web.WebUser.No + "'";
            DBAccess.RunSQL(sql);
            sql = "UPDATE WF_Emp SET AtPara=REPLACE(AtPara,'@Online=1','@Online=0') WHERE No = '" + BP.Web.WebUser.No + "'";
            DBAccess.RunSQL(sql);
            // 2023.8.22 wanglu 退出时删除token
            string clearTokenSQL = "DELETE FROM Port_Token WHERE MyPK = '" + WebUser.Token + "'";
            if (IsBSMode == false)
            {
                HttpContextHelper.ResponseCookieDelete(new string[] {
                        "No", "Name", "Pass", "IsRememberMe", "Auth", "AuthName","DeptParentNo","Token" },
                    "CCS");
                return;
            }
            try
            {
                BP.Pub.Current.Session.Clear();

                HttpContextHelper.ResponseCookieDelete(new string[] {
                        "No", "Name", "Pass", "IsRememberMe", "Auth", "AuthName","Token" },
                   "CCS");

                HttpContextHelper.SessionClear();
            }
            catch
            {
            }
        }
        /// <summary>
        /// 授权人
        /// </summary>
        public static string Auth
        {
            get
            {
                string val = GetValFromCookie("Auth", null, false);
                if (val == null)
                    val = GetSessionByKey("Auth", null);
                return val;
            }
            set
            {
                if (value == "")
                    SetSessionByKey("Auth", null);
                else
                    SetSessionByKey("Auth", value);
            }
        }
        /// <summary>
        /// 部门名称
        /// </summary>
        public static string DeptName
        {
            get
            {
                try
                {
                    string val = GetValFromCookie("FK_DeptName", null, true);
                    return val;
                }
                catch
                {
                    return "无";
                }
            }
            set
            {
                SetSessionByKey("FK_DeptName", value);
            }
        }
        /// <summary>
        /// 部门全称
        /// </summary>
        public static string DeptNameOfFull
        {
            get
            {
                string val = GetValFromCookie("FK_DeptNameOfFull", null, true);
                if (DataType.IsNullOrEmpty(val))
                {
                    try
                    {

                        Paras ps = new Paras();
                        ps.SQL = "SELECT NameOfPath FROM Port_Dept WHERE No =" + ps.DBStr + "No";
                        ps.Add("No", WebUser.DeptNo);
                        val = DBAccess.RunSQLReturnStringIsNull(ps, null);

                        if (DataType.IsNullOrEmpty(val))
                            val = WebUser.DeptName;

                        WebUser.DeptNameOfFull = val;
                        return val;
                    }
                    catch
                    {
                        val = WebUser.DeptName;
                    }
                }
                return val;
            }
            set
            {
                SetSessionByKey("FK_DeptNameOfFull", value);
            }
        }
        /// <summary>
        /// 令牌
        /// </summary>
        public static string Token
        {
            get
            {
                return GetValFromCookie("Token", null, false);
            }
            set
            {
                SetSessionByKey("token", value);
                HttpContextHelper.AddCookie("CCS", "Token", value);

            }
        }
        /// <summary>
        /// 语言
        /// </summary>
        public static string SysLang
        {
            get
            {
                return "CH";
                /*
                string no = GetSessionByKey("Lang", null);
                if (no == null || no == "")
                {
                    if (IsBSMode)
                    {
                        // HttpCookie hc1 = BP.Sys.Base.Glo.Request.Cookies["CCS"];
                        string lang = HttpContextHelper.RequestCookieGet("Lang", "CCS");
                        if (String.IsNullOrEmpty(lang))
                            return "CH";
                        SetSessionByKey("Lang", lang);
                    }
                    else
                    {
                        return "CH";
                    }
                    return GetSessionByKey("Lang", "CH");
                }
                else
                {
                    return no;
                }*/
            }
            set
            {
                SetSessionByKey("Lang", value);
            }
        }
        /// <summary>
        /// 当前登录人员的部门
        /// </summary>
        public static string DeptNo
        {
            get
            {
                string val = GetValFromCookie("FK_Dept", null, false);
                if (val == null)
                {
                    if (WebUser.No == null)
                        throw new Exception("@登录信息丢失，请你确认是否启用了cookie? ");

                    string sql = "SELECT FK_Dept FROM Port_Emp WHERE No='" + WebUser.No + "'";
                    string dept = DBAccess.RunSQLReturnStringIsNull(sql, null);
                    if (dept == null)
                    {
                        sql = "SELECT FK_Dept FROM Port_Emp WHERE No='" + WebUser.No + "'";
                        dept = DBAccess.RunSQLReturnStringIsNull(sql, null);
                    }

                    if (dept == null)
                        throw new Exception("@err-003 FK_Dept，当前登录人员(" + WebUser.No + ")，没有设置部门。");

                    SetSessionByKey("FK_Dept", dept);
                    return dept;
                }
                return val;
            }
            set
            {
                SetSessionByKey("FK_Dept", value);
            }
        }
      
        /// <summary>
        /// 当前登录人员的父节点编号
        /// </summary>
        public static string DeptParentNo
        {
            get
            {
                string val = GetValFromCookie("DeptParentNo", null, false);
                if (val == null)
                {
                    if (BP.Web.WebUser.DeptNo == null)
                        throw new Exception("@err-001 DeptParentNo, FK_Dept 登录信息丢失。");

                    BP.Port.Dept dept = new BP.Port.Dept(BP.Web.WebUser.DeptNo);
                    BP.Web.WebUser.DeptParentNo = dept.ParentNo;
                    return dept.ParentNo;
                }
                return val;
            }
            set
            {
                SetSessionByKey("DeptParentNo", value);
            }
        }
        public static string NoOfRel
        {
            get
            {
                string val = GetSessionByKey("No", null);
                if (val == null)
                    return GetValFromCookie("No", null, true);
                return val;
            }
        }
        public static string GetValFromCookie(string valKey, string isNullAsVal, bool isChinese)
        {
            if (IsBSMode == false)
                return BP.Pub.Current.GetSessionStr(valKey, isNullAsVal);

            try
            {
                //先从session里面取.
                //string v = System.Web.HttpContext.Current.Session[valKey] as string;
                //2019-07-25 zyt改造
                string v = HttpContextHelper.SessionGet<string>(valKey);
                if (DataType.IsNullOrEmpty(v) == false)
                    return v;
                else if (SystemConfig.isDebug == false && valKey == "No" && DataType.IsNullOrEmpty(v))
                    return null;
            }
            catch
            {

            }

            try
            {
                string val = HttpContextHelper.RequestCookieGet(valKey, "CCS");

                if (isChinese)
                    val = HttpUtility.UrlDecode(val);

                if (DataType.IsNullOrEmpty(val))
                    return isNullAsVal;
                return val;
            }
            catch
            {
                return isNullAsVal;
            }
        }
        /// <summary>
        /// 是否是操作员？
        /// </summary>
        public static bool IsAdmin
        {
            get
            {
                if (WebUser.No == null)
                    return false;

                if (BP.Web.WebUser.No.ToLower().Equals("admin") == true)
                    return true;

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                {
                    GloVar gloVar = new GloVar();
                    gloVar.No = WebUser.DeptNo + "_" + WebUser.No + "_Adminer";
                    if (gloVar.RetrieveFromDBSources() == 0)
                        return false; //单机版.
                    return true;
                }

                string sql = "SELECT FK_Emp FROM Port_OrgAdminer WHERE FK_Emp='" + WebUser.UserID + "' AND OrgNo='" + WebUser.OrgNo + "'";
                if (DBAccess.RunSQLReturnTable(sql).Rows.Count == 0)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        public static string No
        {
            get
            {
                return GetValFromCookie("No", null, true);
            }
            set
            {
                SetSessionByKey("No", value.Trim());
            }
        }
        public static string UserID
        {
            get
            {
                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    return WebUser.OrgNo + "_" + WebUser.No;
                return WebUser.No;
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public static string Name
        {
            get
            {
                string no = BP.Web.WebUser.No;
                string val = GetValFromCookie("Name", no, true);
                if (val == null)
                    throw new Exception("@err-002 Name 登录信息丢失。");
                return val;
            }
            set
            {
                SetSessionByKey("Name", value);
            }
        }
        /// <summary>
        /// 运行设备
        /// </summary>
        public static string SheBei
        {
            get
            {
                string no = BP.Web.WebUser.No;
                string val = GetValFromCookie("SheBei", no, true);
                if (val == null)
                    return "PC";
                return val;
            }
            set
            {
                SetSessionByKey("SheBei", value);
            }
        }
        /// <summary>
        /// 更新当前管理员的组织SID信息.
        /// </summary>
        public static void UpdateSIDAndOrgNoSQL()
        {
            string sql = "";
            if (DBAccess.IsView("Port_Emp") == false)
            {
                sql = "UPDATE Port_Emp SET OrgNo='" + WebUser.OrgNo + "', FK_Dept='" + WebUser.DeptNo + "' WHERE No='" + WebUser.No + "'";
                DBAccess.RunSQL(sql);

                sql = "UPDATE WF_Emp SET OrgNo='" + WebUser.OrgNo + "', FK_Dept='" + WebUser.DeptNo + "' WHERE No='" + WebUser.No + "'";
                DBAccess.RunSQL(sql);
                return;
            }

            //比如: UPDATE XXX SET bumenbianao='@FK_Dept', zhizhibianhao='@OrgNo',  SID='@SID'  WHERE bianhao='@No' 
            sql = BP.Sys.Base.Glo.UpdateSIDAndOrgNoSQL;
            if (DataType.IsNullOrEmpty(sql) == true)
                return;
            //      throw new Exception("err@系统管理员缺少全局配置变量 AppSetting UpdateSIDAndOrgNoSQL ");

            sql = sql.Replace("@FK_Dept", WebUser.DeptNo);
            sql = sql.Replace("@OrgNo", WebUser.OrgNo);
            sql = sql.Replace("@Token", WebUser.Token);
            sql = sql.Replace("@No", WebUser.No);
            DBAccess.RunSQL(sql);
        }
        /// <summary>
        /// 直属领导.
        /// </summary>
        public static string EmpLeader
        {
            get
            {
                string  str= DBAccess.RunSQLReturnString("SELECT Leader FROM Port_Emp WHERE No='" + WebUser.No + "'");
                if (DataType.IsNullOrEmpty(str) == true)
                    return DeptLeader;
                return str;
            }
        }
        /// <summary>
        /// 部门领导
        /// </summary>
        public static string DeptLeader
        {
            get
            {
                return DBAccess.RunSQLReturnString("SELECT Leader FROM Port_Dept WHERE No='" + WebUser.DeptNo + "'");
            }
        }
        /// <summary>
        /// 所在的组织
        /// </summary>
        public static string OrgNo
        {
            get
            {
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                    return "";

                string val = GetValFromCookie("OrgNo", null, true);
                if (val == null)
                    val = GetSessionByKey("OrgNo", null);

                if (val == null)
                {
                    if (WebUser.No == null)
                        throw new Exception("err@登陆信息丢失，请重新登录.");

                    if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    {
                        string no = DBAccess.RunSQLReturnString("SELECT OrgNo FROM Port_Emp WHERE UserID='" + WebUser.No + "'");
                        if (DataType.IsNullOrEmpty(no) == true)
                            throw new Exception("err@SAAS模式下,人员[" + BP.Web.WebUser.No + "]的组织编号不能为空.");
                        SetSessionByKey("OrgNo", no);
                        return no;
                    }

                    if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.GroupInc)
                    {
                        string no = DBAccess.RunSQLReturnString("SELECT OrgNo FROM Port_Emp WHERE No='" + WebUser.No + "'");
                        if (DataType.IsNullOrEmpty(no) == true)
                            throw new Exception("err@集团模式下,人员[" + BP.Web.WebUser.No + "]的组织编号不能为空.");
                        SetSessionByKey("OrgNo", no);
                        return no;
                    }
                }
                return val;
            }
            set
            {
                SetSessionByKey("OrgNo", value);
            }
        }
        public static string OrgName
        {
            get
            {
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                    return "";

                string val = GetValFromCookie("OrgName", null, true);
                if (val == null)
                {
                    if (WebUser.No == null)
                        throw new Exception("@err-006 OrgName 登录信息丢失，或者在 CCBPMRunModel=0 的模式下不能读取该节点.");

                    val = DBAccess.RunSQLReturnString("SELECT Name FROM Port_Org WHERE No='" + WebUser.OrgNo + "'");
                    SetSessionByKey("OrgName", val);

                }
                if (val == null)
                    val = "";
                return val;
            }
            set
            {
                SetSessionByKey("OrgName", value);
            }
        }
        public static string ToJson()
        {
            Hashtable ht = new Hashtable();
            ht.Add("No", WebUser.No);
            ht.Add("Name", WebUser.Name);
            ht.Add("Token", WebUser.Token);
            ht.Add("FK_Dept", WebUser.DeptNo);
            ht.Add("FK_DeptName", WebUser.DeptName);
            ht.Add("OrgNo", WebUser.OrgNo);
            ht.Add("OrgName", WebUser.OrgName);
            return BP.Tools.Json.ToJson(ht);
        }
        /// <summary>
        /// 手机号
        /// </summary>
        public static string Tel
        {
            get
            {
                string val = GetValFromCookie("Tel", null, false);
                if (val == null)
                {
                    if (WebUser.No == null)
                        throw new Exception("@登录信息丢失，请你确认是否启用了cookie? ");

                    string sql = "SELECT Tel FROM Port_Emp WHERE No='" + WebUser.No + "'";
                    string tel = DBAccess.RunSQLReturnStringIsNull(sql, null);

                    SetSessionByKey("Tel", tel);
                    return tel;
                }
                return val;
            }
            set
            {
                SetSessionByKey("Tel", value);
            }
        }
        /// <summary>
        /// 是否是授权状态
        /// </summary> 
        public static bool IsAuthorize
        {
            get
            {
                if (Auth == null || Auth == "")
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 使用授权人ID
        /// </summary>
        public static string AuthName
        {
            get
            {
                string val = GetValFromCookie("AuthName", null, false);
                if (val == null)
                    val = GetSessionByKey("AuthName", null);
                return val;
            }
            set
            {
                if (value == "")
                    SetSessionByKey("AuthName", null);
                else
                    SetSessionByKey("AuthName", value);
            }
        }
        public static string Theame
        {
            get
            {
                string val = GetValFromCookie("Theame", null, false);
                if (val == null)
                    val = GetSessionByKey("Theame", null);
                return val;
            }
            set
            {
                if (value == "")
                    SetSessionByKey("Theame", null);
                else
                    SetSessionByKey("Theame", value);
            }
        }
    }
}
