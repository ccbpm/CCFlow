using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.CloudWS;

namespace CCFlow.WF.Admin.Clound
{
    public partial class RegUser : System.Web.UI.Page
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
        }

        protected void BtnRegUser_Click(object sender, EventArgs e)
        {
            string userNo = this.Request.Params["userNo"];
            if (userNo.Length < 6 || userNo.Length > 16)
            {
                BP.Sys.PubClass.Alert("请按要求填写用户名");
                return;
            }

            if (!string.IsNullOrWhiteSpace(ccflowCloud.IsExitUser(userNo)))
            {
                BP.Sys.PubClass.Alert("用户名【" + userNo + "】,已经被注册.");
                return;
            }


            string password = this.Request.Params["password"];
            string repeatPassword = this.Request.Params["repeatPassword"];
            if (password != repeatPassword)
            {
                BP.Sys.PubClass.Alert("输入密码不一致");
                return;
            }
            string userName = this.Request.Params["userName"];

            string email = this.Request.Params["email"];
            string tel = this.Request.Params["tel"];
            string qq = this.Request.Params["qq"];

            int userType = Convert.ToInt32(this.DDL_UserType.SelectedValue);
            int userTarget = Convert.ToInt32(this.DDL_UserTarget.SelectedValue);


            string guid = ccflowCloud.PriRegUser(userNo, userName, password, email, tel, qq, userType, userTarget);

            if (string.IsNullOrWhiteSpace(guid))//注册失败
            {
                this.Response.Redirect("RegUser.aspx");
                return;
            }

            BP.WF.Cloud.CCLover.UserNo = userNo;
            BP.WF.Cloud.CCLover.Password = password;
            BP.WF.Cloud.CCLover.GUID = guid;

            //ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script></script>");
            this.Response.Redirect("PriFlow.aspx");
        }
    }
}