using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.SDKFlowDemo.GuestApp
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strNo = "";
            string strPs = "";
            if (this.Request.QueryString["DoType"] == "Logout")
                BP.Web.GuestUser.Exit();

            if (this.IsPostBack == false)
            {
                if (Request.QueryString["username"] != null)//判断是否是从其他页面跳转
                {
                    strNo = Request.QueryString["username"].ToString();

                    if (Request.QueryString["password"] != null)
                    {
                        strPs = Request.QueryString["password"].ToString();

                        this.UserName.Value = strPs;
                        this.PassWord.Value = strNo;
                        this.Btn_Login_Click1(null, null);
                    }
                }
            }
        }
        //登录方法
        protected void Btn_Login_Click1(object sender, EventArgs e)
        {
            string user = UserName.Value.Trim();
            string pass = PassWord.Value.Trim();
            try
            {
                //关闭已登录用户.
                if (BP.Web.GuestUser.No != null)
                    BP.WF.Dev2InterfaceGuest.Port_LoginOunt();

                string sql = "SELECT vPassword FROM tEnterpriseUser where vUserName='" + user + "'";
                string password = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);
                if (password == null)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "error1", "alert('用户名密码错误，注意密码区分大小写，请检查是否按下了CapsLock.。');", true);
                    return;
                }

                if (password  == pass)
                {
                    sql = "SELECT vEnterpriseName FROM tEnterpriseUser where vUserName='" + user + "'";
                    string userName = BP.DA.DBAccess.RunSQLReturnStringIsNull(sql, null);

                    //这里是密码明文校验, 让用户登录.
                    BP.WF.Dev2InterfaceGuest.Port_Login(user, userName);

                    //把当前的参数也传递过去.
                    this.Response.Redirect("Home.aspx", false);
                    return;
                }

                Page.ClientScript.RegisterStartupScript(this.GetType(), "error2", "alert('用户名密码错误，注意密码区分大小写，请检查是否按下了CapsLock.。');", true);
            }
            catch (System.Exception ex)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "error3", "alert('" + "@用户名密码错误!@检查是否按下了CapsLock.@更详细的信息:" + ex.Message + "');", true);
            }
        }



    }
}