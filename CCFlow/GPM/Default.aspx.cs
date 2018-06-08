using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GMP2.GPM
{
    public partial class Default : System.Web.UI.Page
    {
        public string usermsg = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (BP.DA.DBAccess.IsExitsObject("GPM_APP") == false)
            {
                this.Response.Redirect("DBInstall.aspx", true);
                return;
            }

            if (BP.Web.WebUser.NoOfRel != "admin")
            {
                this.Response.Write("非法的用户登录.");
                return;
            }
            else
            {
                //让admin 在登录一次.
                //BP.WF.Dev2Interface.Port_Login("admin");
            }

           // BP.GPM.SystemLoginLog

            // delete by stone : BP.Sys.UserLog 与 BP.GPM.SystemLoginLog 作用相同, 去掉 SystemLoginLog.
            ////记录登录日志
            //BP.GPM.SystemLoginLog loginLog = new BP.GPM.SystemLoginLog();
            //loginLog.FK_Emp = BP.Web.WebUser.No;
            //loginLog.FK_App = "GPM";
            //loginLog.RContent = "登录系统";
            //loginLog.LoginDateTime = DateTime.Now.ToString();
            //loginLog.IP = System.Web.HttpContext.Current.Request.UserHostAddress;
            //loginLog.Insert();
            
            usermsg = "帐号：" + BP.Web.WebUser.No + " 姓名： " + BP.Web.WebUser.Name + " 部门：" + BP.Web.WebUser.FK_DeptName;
        }
    }
}