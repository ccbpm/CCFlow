using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.CloudWS;

namespace CCFlow.WF.Admin.Clound
{
    public partial class Login : System.Web.UI.Page
    {
        private WSSoapClient ccflowCloud
        {
            get
            {
                WSSoapClient cloud = BP.WF.Cloud.Glo.GetSoap();
                return cloud;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ccflowCloud.GetNetState();
            }
            catch (Exception)
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script>netInterruptJs();</script>");
                return;
            }

            this.Label1.Text = "";
            this.Label2.Text = "";
        }

        protected void BtnRegUser_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("RegUser.aspx");
        }

        protected void BtnLogin_Click(object sender, EventArgs e)
        {
            string userNo = this.TB_No.Text.ToString();
            string pwd = this.pwd.Text.ToString();

            if (string.IsNullOrWhiteSpace(userNo))
            {
                this.Label1.Text = "请输入账号!";
                return;
            }

            if (string.IsNullOrWhiteSpace(pwd))
            {
                this.Label2.Text = "请输入密码!";
                return;
            }

            string guid = ccflowCloud.PriLogin(userNo, pwd);

            if (guid == "errorUserNo")//未注册
            {
                this.Label1.Text = "账号未注册!";
                return;
            }
            if (guid == "errorPwd")
            {
                this.Label2.Text = "密码错误!";
                return;
            }

            //BP.WF.Cloud.CCLover.UserNo = userNo;
            //BP.WF.Cloud.CCLover.Password = pwd;
            //BP.WF.Cloud.CCLover.GUID = guid;
            this.Response.Redirect("PriFlow.aspx");
        }

    }
}