using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Web;
using BP.En;
using BP.DA;
using BP.WF;
using BP.Sys;
using BP.Port;
using BP;

public partial class App_Login : BP.Web.WebPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        #region 判断是否是退出？
        string strNo = "";
        string strPs = "";
        string isRemember = "";
        if (this.Request.QueryString["DoType"] == "Logout")
        {
            BP.Web.WebUser.Exit();
        }
        #endregion 判断是否是退出？


        if (this.IsPostBack == false)
        {
            #region 判断是否有自动登录的信息？
            if (this.Request.QueryString["username"] != null)//判断是否是从其他页面跳转
            {
                strNo = this.Request.QueryString["username"].ToString();
                if (this.Request.QueryString["password"] != null)
                {
                    strPs = this.Request.QueryString["password"].ToString();
                    Login(strNo, strPs);
                }
                return;
            }
            #endregion pandaun是否有自动登录的信息？

            #region 获取以前的登录信息放入到文本框里.
            if (this.Request.Browser.Cookies == true) //获取cookie
            {
                if (Request.Cookies["CCS"] != null)
                {
                    strNo = Convert.ToString(Request.Cookies["CCS"].Values["No"]);
                    this.TB_No.Text = strNo;

                    string cookiePwd = Request.Cookies["CCS"].Values["PWD"];

                    if (!string.IsNullOrWhiteSpace(cookiePwd))
                    {
                        TB_Pass.Attributes.Add("value", cookiePwd);
                        hidIsRememberedPass.Value = "1";
                    }
                }
            }

            this.Page.RegisterStartupScript("event_handler", "<script>document.body.onkeypress = keyPressed;</script>");
            this.Page.RegisterClientScriptBlock("default_button", "<script> function keyPressed() { if(window.event.keyCode == 13) { document.getElementById(\""
                + this.Btn1.ClientID + "\").click(); } } </script>");

            //if (WebUser.No != null)
            //    this.TB_No.Text = WebUser.No;
            #endregion 获取以前的登录信息放入到文本框里.
        }
    }
    public void Login(string strUser, string strPass)
    {
        TB_No.Text = strUser;
        TB_Pass.Text = strPass;
        Login();
    }
    //登录方法
    private void Login()
    {
        string user = TB_No.Text.Trim();
        string pass = TB_Pass.Text.Trim();

        //判断是否是记住密码
        if(hidIsRememberedPass.Value == "1")
        {
            pass = Emp.DecryptPass(pass);
        }

        try
        {
            if (WebUser.No != null)
                WebUser.Exit();

            if (user.ToLower() == "guest")
            {
                this.Alert("guest 用户不能登录内部用户的处理程序. ");
                return;
            }

            Emp em = new Emp();
            em.No = user;
            if (em.RetrieveFromDBSources() == 0)
            {
                this.Alert("用户名密码错误，注意密码区分大小写，请检查是否按下了CapsLock.。");
                return;
            }
            
            if (em.CheckPass(pass))
            {
                //bool bl = this.IsRemember.Checked;

                //WebUser.SignInOfGenerLang(em, WebUser.SysLang, bl);
                //if (this.Request.RawUrl.ToLower().Contains("wap"))
                //    WebUser.IsWap = true;
                //else
                //    WebUser.IsWap = false;
                //WebUser.Token = this.Session.SessionID;

                //调用登录接口,让BP框架登录.
                BP.WF.Dev2Interface.Port_Login(em.No, this.IsRemember.Checked);

                Response.Redirect("Default.aspx", false);
                return;
            }
            this.Alert("用户名密码错误，注意密码区分大小写，请检查是否按下了CapsLock.。");
        }
        catch (System.Exception ex)
        {
            this.Alert("@用户名密码错误!@检查是否按下了CapsLock.@更详细的信息:" + ex.Message);
        }
    }
    public void Btn1_Click(object sender, System.EventArgs e)
    {
        Login();
    }
}