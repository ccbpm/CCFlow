using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.SDKFlowDemo.GuestFlow
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            // 获取学生信息。
            string stuNo = this.TB_No.Text.Trim();
            string pass = this.TB_PW.Text.Trim();

            // 应该根据学号，查询出来学生名称，这里是直接定义了.
            string stuName = "张三";

            // 执行Guest登陆
            BP.WF.Dev2InterfaceGuest.Port_Login(stuNo, stuName);

            // 转入学生填写请假申请单.
            string url = "/WF/MyFlow.aspx?FK_Flow=055&FK_Node=05501&GuestNo=" + stuNo + "&GuestName=" + stuName;
            url += "&SysSendEmps=yangyilei";

            //注意这里有两个参数通过URL传递过去了。
            this.Response.Redirect(url, true);
        }
    }
}