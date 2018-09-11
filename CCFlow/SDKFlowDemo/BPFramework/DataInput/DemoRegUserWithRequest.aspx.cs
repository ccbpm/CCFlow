using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;

namespace CCFlow.SDKFlowDemo
{
    public partial class DemoRegUserWithRequest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Btn_Reg_Click(object sender, EventArgs e)
        {
            try
            {
                //提交前做完整的校验.
                if (DataType.IsNullOrEmpty(this.TB_PW.Text.Trim()))
                    throw new Exception("密码不能为空.");

                if (this.TB_PW.Text.Trim() != this.TB_PW1.Text.Trim())
                    throw new Exception("输入的密码两次不一致.");

                //创建一个entity.
                BP.Demo.BPFramework.BPUser user = new BP.Demo.BPFramework.BPUser();

                //执行copy, 从Request中，至于如何实现请跟踪到方法里面去.
                user = BP.Sys.PubClass.CopyFromRequest(user) as BP.Demo.BPFramework.BPUser;
                if (user.IsExits)
                    throw new Exception("@用户名:" + user.No + "已经存在.");

                //赋值当前日期.
                user.FK_NY = BP.DA.DataType.CurrentYearMonth;

                //执行数据插入.
                user.Insert();
                BP.Sys.PubClass.Alert("用户注册成功");
                this.Btn_Reg.Enabled = false;
            }
            catch(Exception ex)
            {
                BP.Sys.PubClass.Alert(ex.Message);
                return;
            }

        }
    }
}