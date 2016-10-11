using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web;

namespace CCFlow.WF.Admin.CCBPMDesigner
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region 检查是否是安装了ccflow如果没有就让其安装.
            if (BP.DA.DBAccess.IsExitsObject("WF_Emp") == false)
            {
                this.Response.Redirect("../DBInstall.aspx", true);
                return;
            }
            #endregion 检查是否是安装了ccflow如果没有就让其安装.

            if (this.Request.QueryString["DoType"] == "Logout")
            {
                //退出.
                BP.WF.Dev2Interface.Port_SigOut();
                return;
            }

            if (this.Request.QueryString["DoType"] == "Login")
            {
                string user = TB_UserName.Value.Trim();
                string pass = TB_Password.Value.Trim();
                try
                {
                    if (WebUser.No != null)
                        BP.WF.Dev2Interface.Port_SigOut();

                    BP.Port.Emp em = new BP.Port.Emp(user);
                    if (em.CheckPass(pass))
                    {
                        BP.WF.Dev2Interface.Port_Login(user);
                        WebUser.Token = this.Session.SessionID;
                        this.Response.Redirect("Default.aspx?SID=" + this.Session.SessionID, false);
                        return;
                    }
                    else
                    {
                        BP.Sys.PubClass.Alert("用户名密码错误，注意密码区分大小写，请检查是否按下了CapsLock.。");
                    }
                }
                catch (System.Exception ex)
                {
                    BP.Sys.PubClass.Alert("用户名密码错误，注意密码区分大小写，请检查是否按下了CapsLock.。");
                    this.Page.ClientScript.RegisterStartupScript(this.GetType(), "kesy", "<script language=JavaScript>alert('@用户名密码错误!@检查是否按下了CapsLock.@更详细的信息:" + ex.Message + "');</script>");
                }
            }
        }
    }
}