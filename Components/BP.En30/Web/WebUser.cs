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
        public static void SignInOfGener(Emp em, string lang = "CH", bool isRememberMe = false, bool IsRecSID = false,
            string authNo = null, string authName = null)
        {
            if (HttpContextHelper.Current == null)
                SystemConfig.IsBSsystem = false;
            else
                SystemConfig.IsBSsystem = true;

            WebUser.No = em.UserID;
            WebUser.Name = em.Name;


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
            if (DataType.IsNullOrEmpty(em.OrgNo) == false && DataType.IsNullOrEmpty(em.FK_Dept) == true)
            {
                BP.Port.DeptEmp de = new BP.Port.DeptEmp();
                de.FK_Dept = em.OrgNo;
                de.FK_Emp = em.No;
                de.OrgNo = em.OrgNo;
                de.Insert();
                // em.FK_Dept = em.OrgNo;
            }


            #region 解决部门的问题.
            if (DataType.IsNullOrEmpty(em.FK_Dept) == true)
            {
                string sql = "";

                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                    sql = "SELECT FK_Dept FROM Port_DeptEmp WHERE FK_Emp='" + em.UserID + "' AND OrgNo='" + WebUser.OrgNo + "' ";
                else
                    sql = "SELECT FK_Dept FROM Port_DeptEmp WHERE FK_Emp='" + em.UserID + "'";

                string deptNo = DBAccess.RunSQLReturnString(sql);
                if (DataType.IsNullOrEmpty(deptNo) == true)
                {
                    if (em.No.Equals("Guest") == true)
                    {
                        if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                        {
                            BP.Port.DeptEmp de = new BP.Port.DeptEmp();
                            de.FK_Dept = "ccs";
                            de.FK_Emp = "Guest";
                            de.Insert();
                        }
                    }
                    else
                    {
                        if (DataType.IsNullOrEmpty(deptNo) == true)
                            throw new Exception("@登录人员(" + em.UserID + "," + em.Name + ")没有维护部门." + sql);
                    }
                }
                else
                {
                    //调用接口更改所在的部门.
                    WebUser.ChangeMainDept(em.UserID, deptNo);
                    em.FK_Dept = deptNo;
                }
            }

            BP.Port.Dept dept = new Dept();
            dept.No = em.FK_Dept;
            if (dept.RetrieveFromDBSources() == 0)
                throw new Exception("@登录人员(" + em.UserID + "," + em.Name + ")没有维护部门,或者部门编号{" + em.FK_Dept + "}不存在.");
            #endregion 解决部门的问题.

            WebUser.FK_Dept = em.FK_Dept;
            WebUser.FK_DeptName = em.FK_DeptText;
            WebUser.DeptParentNo = dept.ParentNo;
            WebUser.SysLang = lang;
            if (BP.Difference.SystemConfig.IsBSsystem)
            {
                // cookie操作，为适应不同平台，统一使用HttpContextHelper
                Dictionary<string, string> cookieValues = new Dictionary<string, string>();

                cookieValues.Add("No", em.UserID);
                cookieValues.Add("Name", HttpUtility.UrlEncode(em.Name));

                if (isRememberMe)
                    cookieValues.Add("IsRememberMe", "1");
                else
                    cookieValues.Add("IsRememberMe", "0");

                cookieValues.Add("FK_Dept", em.FK_Dept);
                cookieValues.Add("FK_DeptName", HttpUtility.UrlEncode(em.FK_DeptText));

                //设置组织编号.
                if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                    cookieValues.Add("OrgNo", em.OrgNo);

                //if (HttpContextHelper.Current.Session != null)
                //{
                //    cookieValues.Add("Token", HttpContextHelper.SessionID);
                //    cookieValues.Add("Token", HttpContextHelper.SessionID);
                //}

                cookieValues.Add("Tel", em.Tel);
                cookieValues.Add("Lang", lang);
                if (authNo == null)
                    authNo = "";
                cookieValues.Add("Auth", authNo); //授权人.

                if (authName == null)
                    authName = "";
                cookieValues.Add("AuthName", authName); //授权人名称..
                //cookieValues.Add("Token", WebUser.Token); //授权人名称..
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

            if (IsBSMode == false)
            {
                HttpContextHelper.ResponseCookieDelete(new string[] {
                        "No", "Name", "Pass", "IsRememberMe", "Auth", "AuthName","DeptParentNo" },
                    "CCS");
                return;
            }
            try
            {
                BP.Pub.Current.Session.Clear();

                HttpContextHelper.ResponseCookieDelete(new string[] {
                        "No", "Name", "Pass", "IsRememberMe", "Auth", "AuthName" },
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
        public static string FK_DeptName
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
        public static string FK_DeptNameOfFull
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
                        ps.Add("No", WebUser.FK_Dept);
                        val = DBAccess.RunSQLReturnStringIsNull(ps, null);

                        if (DataType.IsNullOrEmpty(val))
                            val = WebUser.FK_DeptName;

                        WebUser.FK_DeptNameOfFull = val;
                        return val;
                    }
                    catch
                    {
                        val = WebUser.FK_DeptName;
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
                HttpContextHelper.AddCookie("CCS", "Token", WebUser.Token);

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
        public static string FK_Dept
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
        /// 所在的集团编号
        /// </summary>
        public static string GroupNo111
        {
            get
            {
                string val = GetValFromCookie("GroupNo", null, false);
                if (val == null)
                {
                    if (BP.Difference.SystemConfig.CustomerNo != "Bank")
                        return "0";

                    if (WebUser.No == null)
                        throw new Exception("@登录信息丢失，请你确认是否启用了cookie? ");

                    string sql = "SELECT GroupNo FROM Port_Dept WHERE No='" + WebUser.FK_Dept + "'";
                    string groupNo = DBAccess.RunSQLReturnStringIsNull(sql, null);

                    if (groupNo == null)
                        throw new Exception("@err-003 FK_Dept，当前登录人员(" + WebUser.No + ")，没有设置部门。");

                    SetSessionByKey("GroupNo", groupNo);
                    return groupNo;
                }
                return val;
            }
            set
            {
                SetSessionByKey("GroupNo", value);
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
                    if (BP.Web.WebUser.FK_Dept == null)
                        throw new Exception("@err-001 DeptParentNo, FK_Dept 登录信息丢失。");

                    BP.Port.Dept dept = new BP.Port.Dept(BP.Web.WebUser.FK_Dept);
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
                else if (SystemConfig.IsDebug==false && valKey == "No"  && DataType.IsNullOrEmpty(v))
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
        /// 设置信息.
        /// </summary>
        /// <param name="keyVals"></param>
        public static void SetValToCookie(string keyVals)
        {
            if (BP.Difference.SystemConfig.IsBSsystem == false)
                return;

            /* 2019-7-25 张磊 如下代码没有作用，删除
            HttpCookie hc = BP.Sys.Base.Glo.Request.Cookies["CCS"];
            if (hc != null)
                BP.Sys.Base.Glo.Request.Cookies.Remove("CCS");
            HttpCookie cookie = new HttpCookie("CCS");
            cookie.Expires = DateTime.Now.AddMinutes(BP.Difference.SystemConfig.SessionLostMinute);
            */

            Dictionary<string, string> cookieValues = new Dictionary<string, string>();
            AtPara ap = new AtPara(keyVals);
            foreach (string key in ap.HisHT.Keys)
                cookieValues.Add(key, ap.GetValStrByKey(key));
            cookieValues.Add("Token",WebUser.Token);
            HttpContextHelper.ResponseCookieAdd(cookieValues,
                DateTime.Now.AddMinutes(BP.Difference.SystemConfig.SessionLostMinute),
                "CCS");
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
                    return false; //单机版.

                //SAAS版本. 集团版
                if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                {
                    string sql = "SELECT FK_Emp FROM Port_OrgAdminer WHERE FK_Emp='" + WebUser.No + "' AND OrgNo='" + WebUser.OrgNo + "'";
                    if (DBAccess.RunSQLReturnTable(sql).Rows.Count == 0)
                        return false;
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        public static string No
        {
            get
            {
                return  GetValFromCookie("No", null, true);
            }
            set
            {
                SetSessionByKey("No", value.Trim());
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
                sql = "UPDATE Port_Emp SET OrgNo='" + WebUser.OrgNo + "', FK_Dept='" + WebUser.FK_Dept + "' WHERE No='" + WebUser.No + "'";
                DBAccess.RunSQL(sql);

                sql = "UPDATE WF_Emp SET OrgNo='" + WebUser.OrgNo + "', FK_Dept='" + WebUser.FK_Dept + "' WHERE No='" + WebUser.No + "'";
                DBAccess.RunSQL(sql);
                return;
            }

            //比如: UPDATE XXX SET bumenbianao='@FK_Dept', zhizhibianhao='@OrgNo',  SID='@SID'  WHERE bianhao='@No' 
            sql = BP.Sys.Base.Glo.UpdateSIDAndOrgNoSQL;
            if (DataType.IsNullOrEmpty(sql) == true)
                return;
            //      throw new Exception("err@系统管理员缺少全局配置变量 AppSetting UpdateSIDAndOrgNoSQL ");

            sql = sql.Replace("@FK_Dept", WebUser.FK_Dept);
            sql = sql.Replace("@OrgNo", WebUser.OrgNo);
            sql = sql.Replace("@Token", WebUser.Token);
            sql = sql.Replace("@No", WebUser.No);
            DBAccess.RunSQL(sql);
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
                        SetSessionByKey("OrgNo", no);
                        return no;
                    }

                    if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.GroupInc)
                    {
                        string no = DBAccess.RunSQLReturnString("SELECT OrgNo FROM Port_Emp WHERE No='" + WebUser.No + "'");
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
        /// 域
        /// </summary>
        public static string Domain
        {
            get
            {
                string val = GetValFromCookie("Domain", null, true);
                if (val == null)
                    throw new Exception("@err-003 Domain 登录信息丢失。");
                return val;
            }
            set
            {
                SetSessionByKey("Domain", value);
            }
        }
        public static Stations HisStations
        {
            get
            {
                Stations sts = new Stations();
                QueryObject qo = new QueryObject(sts);
                qo.AddWhereInSQL("No", "SELECT FK_Station FROM Port_DeptEmpStation WHERE FK_Emp='" + WebUser.No + "'");
                qo.DoQuery();

                return sts;
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
