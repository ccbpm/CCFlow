using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Port;
using BP.DA;

namespace CCFlow.WF.Admin.CCBPMDesigner
{
    public partial class DesignerSL : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region 检查是否是安装了ccflow如果没有就让其安装.
            /*数据库链接不通或者有异常，说明没有安装.*/
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
                BP.Web.WebUser.SignInOfGener(emp, true);
            }
            else
            {
                emp.No = "admin";
                emp.Name = "admin";
                emp.FK_Dept = "01";
                emp.Pass = "pub";
                emp.Insert();
                BP.Web.WebUser.SignInOfGener(emp, true);
                //throw new Exception("admin 用户丢失，请注意大小写。");
            }
            #endregion 执行admin登陆.

            // 执行升级, 现在升级代码移动到 Glo 里面了。
            string str = BP.WF.Glo.UpdataCCFlowVer(); //执行升级.
            if (str != null)
            {
                //   this.Response.Write(str);
                if (str == "0")
                    BP.Sys.PubClass.Alert("系统升级错误，请查看日志文件\\DataUser\\log");
                else
                    BP.Sys.PubClass.Alert("系统成功升级到:" + str + " ，系统升级不会破坏现有的数据，ccbpm6的升级都是保证向下兼容的。");
            }
        }
    }
}