using System;
using System.Data;
using System.Text;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.Port;

namespace BP.GPM
{
    /// <summary>
    /// 权限调用API
    /// </summary>
    public class Dev2Interface
    {
        #region 菜单权限
        #endregion 菜单权限
  

        #region 登陆接口
        /// <summary>
        /// 用户登陆,此方法是在开发者校验好用户名与密码后执行
        /// </summary>
        /// <param name="userNo">用户名</param>
        /// <param name="SID">安全ID,请参考流程设计器操作手册</param>
        public static void Port_Login(string userNo, string sid)
        {
            if (BP.Sys.SystemConfig.OSDBSrc == OSDBSrc.Database)
            {
                string sql = "SELECT SID FROM Port_Emp WHERE No='" + userNo + "'";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("用户不存在或者SID错误。");

                if (dt.Rows[0]["SID"].ToString() != sid)
                    throw new Exception("用户不存在或者SID错误。");
            }

            BP.Port.Emp emp = new BP.Port.Emp(userNo);
            WebUser.SignInOfGener(emp, true);
            WebUser.IsWap = false;
            return;
        }
        /// <summary>
        /// 用户登陆,此方法是在开发者校验好用户名与密码后执行
        /// </summary>
        /// <param name="userNo">用户名</param>
        public static void Port_Login(string userNo)
        {
            BP.Port.Emp emp = new BP.Port.Emp(userNo);
            WebUser.SignInOfGener(emp, true);
            WebUser.IsWap = false;
            return;
        }
        /// <summary>
        /// 注销当前登录
        /// </summary>
        public static void Port_SigOut()
        {
            WebUser.Exit();
        }
        /// <summary>
        /// 获取未读的消息
        /// 用于消息提醒.
        /// </summary>
        /// <param name="userNo">用户ID</param>
        public static string Port_SMSInfo(string userNo)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT MyPK, EmailTitle  FROM sys_sms WHERE SendToEmpID=" + SystemConfig.AppCenterDBVarStr + "SendToEmpID AND IsAlert=0";
            ps.Add("SendToEmpID", userNo);
            DataTable dt = DBAccess.RunSQLReturnTable(ps);
            string strs = "";
            foreach (DataRow dr in dt.Rows)
            {
                strs += "@" + dr[0] + "=" + dr[1].ToString();
            }
            ps = new Paras();
            ps.SQL = "UPDATE  sys_sms SET IsAlert=1 WHERE  SendToEmpID=" + SystemConfig.AppCenterDBVarStr + "SendToEmpID AND IsAlert=0";
            ps.Add("SendToEmpID", userNo);
            DBAccess.RunSQL(ps);
            return strs;
        }
        #endregion 登陆接口

        #region GPM接口
        /// <summary>
        /// 获取一个操作人员对于一个系统的权限
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="app">系统编号</param>
        /// <returns>结果集</returns>
        public static DataTable DB_Menus(string userNo, string app)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM GPM_EmpMenu WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp AND FK_App=" + SystemConfig.AppCenterDBVarStr + "FK_App ";
            ps.Add("FK_Emp", userNo);
            ps.Add("FK_App", app);
            return DBAccess.RunSQLReturnTable(ps);
        }
        /// <summary>
        /// 获取一个操作人员对此应用可以访问的系统
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <returns>结果集</returns>
        public static DataTable DB_Apps(string userNo)
        {
            Paras ps = new Paras();
            ps.SQL = "SELECT * FROM GPM_EmpApp WHERE FK_Emp=" + SystemConfig.AppCenterDBVarStr + "FK_Emp ";
            ps.Add("FK_Emp", userNo);
            return DBAccess.RunSQLReturnTable(ps);
        }
        #endregion GPM接口

    }
}
