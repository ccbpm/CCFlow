using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Port;
using BP.Sys;
using BP.DA;
using BP.WF;
using BP.WF.Template;
using BP.WF.Rpt;

namespace CCFlow.WF.Admin.XAP
{
    public partial class Designer : System.Web.UI.Page
    {
        #region 属性.
        /// <summary>
        /// 检查更新.
        /// </summary>
        public bool IsCheckUpdate
        {
            get
            {
                if (this.Request.QueryString["IsCheckUpdate"] != null)
                    return true;

                 if (this.Request.QueryString["IsUpdate"] != null)
                    return true;

                return false;
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 检查是否是安装了ccflow如果没有就让其安装.
            if (BP.DA.DBAccess.IsExitsObject("WF_Emp") == false)
            {
                this.Response.Redirect("../DBInstall.aspx", true);
                return;
            }
            #endregion 检查是否是安装了ccflow如果没有就让其安装.

            #region 执行admin登陆.
            Emp emp = new Emp();
            emp.No = "admin";
            if (emp.RetrieveFromDBSources() == 1)
            {
                BP.Web.WebUser.SignInOfGener(emp);
            }
            else
            {
                emp.No = "admin";
                emp.Name = "admin";
                emp.FK_Dept = "01";
                emp.Pass = "pub";
                emp.Insert();
                BP.Web.WebUser.SignInOfGener(emp);
                //throw new Exception("admin 用户丢失，请注意大小写。");
            }
            #endregion 执行admin登陆.

            // 执行升级, 现在升级代码移动到 Glo 里面了。
            string str = BP.WF.Glo.UpdataCCFlowVer(); //执行升级.
            if (str != null)
            {
                //   this.Response.Write(str);
                if (str == "0")
                    BP.Sys.PubClass.Alert("系统升级错误，请查看日志文件\\DataUser/\\log");
                else
                    BP.Sys.PubClass.Alert("系统成功升级到:" + str + " ，系统升级不会破坏现有的数据。");
            }
        }
    }
}