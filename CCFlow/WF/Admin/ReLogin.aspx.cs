using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin
{
    public partial class ReLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Btn_Login_Click(object sender, EventArgs e)
        {
            string userNo = this.TB_UserNo.Text;
            string password = this.TB_Pass.Text;

            BP.Port.Emp emp = new BP.Port.Emp();
            emp.No = userNo;
            if (emp.RetrieveFromDBSources() == 0)
            {
                this.Response.Write("用户名或密码错误.");
                return;
            }

            if (emp.CheckPass(password) == false)
            {
                this.Response.Write("用户名或密码错误.");
                return;
            }


            BP.Web.WebUser.SignInOfGener(emp);


            string url = this.Session["LastUrl"] as string;
            if (string.IsNullOrEmpty(url) == true)
            {
                this.Response.Redirect(url, true);
                return;
            }

            url = this.Request["LastUrl"] as string;
            url = url.Replace("[", "");
            url = url.Replace("]", "");
            this.Response.Redirect(url, true);

        }
    }
}