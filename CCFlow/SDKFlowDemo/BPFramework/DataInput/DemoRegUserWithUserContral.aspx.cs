using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Demo;
using BP.Demo.BPFramework;
using BP.DA;


namespace CCFlow.SDKFlowDemo
{
    public partial class DemoRegUserWithUserContral : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            this.Pub1.AddTable();
            this.Pub1.AddCaption("演示使用BP的用户控件来呈现与采集数据.");

            this.Pub1.AddTR();
            this.Pub1.AddTD("帐号");
            BP.Web.Controls.TB tb =new BP.Web.Controls.TB();
            tb.ID = "TB_" + BPUserAttr.No;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("不能为空,字母或者下划线组合.");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("密码");
            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_" + BPUserAttr.Pass;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("重输密码");
            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_PW1";
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("两次密码不能重复.");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("姓名");
            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_" + BPUserAttr.Name;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("不能为空");
            this.Pub1.AddTREnd();

            //枚举类型.
            this.Pub1.AddTR();
            this.Pub1.AddTD("性别");
            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            ddl.BindSysEnum("XB"); // 在Sys_Eumm 已经注册了该枚举值.
            ddl.ID = "TB_" + BPUserAttr.XB;
            this.Pub1.AddTD(ddl);
            this.Pub1.AddTD("请选择");
            this.Pub1.AddTREnd();


            // 数值类型.
            this.Pub1.AddTR();
            this.Pub1.AddTD("年龄");
            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_" + BPUserAttr.Age;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("输入int类型数据.");
            this.Pub1.AddTREnd();


            this.Pub1.AddTR();
            this.Pub1.AddTD("地址");
            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_" + BPUserAttr.Addr;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("电话");
            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_" + BPUserAttr.Tel;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTD("邮件");
            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_" + BPUserAttr.Email;
            this.Pub1.AddTD(tb);
            this.Pub1.AddTD("");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            Button btn = new Button();
            btn.ID = "Btn_Reg";
            btn.Text = "注册新用户";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.AddTD("colspan=3",btn);
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();

        }

        void btn_Click(object sender, EventArgs e)
        {
            try
            {
                string pass = this.Pub1.GetTBByID("TB_PW").Text;
                string pass1 = this.Pub1.GetTBByID("TB_PW1").Text;

                //提交前做完整的校验.
                if (DataType.IsNullOrEmpty(pass))
                    throw new Exception("密码不能为空.");
                if (pass != pass1)
                    throw new Exception("输入的密码两次不一致.");

                //创建一个entity.
                BP.Demo.BPFramework.BPUser user = new BP.Demo.BPFramework.BPUser();
                user.CheckPhysicsTable();

                //执行copy, 从Pub1中，至于如何实现请跟踪到方法里面去.
                user = this.Pub1.Copy(user) as BP.Demo.BPFramework.BPUser;
                if (user.IsExits)
                    throw new Exception("@用户名:" + user.No + "已经存在.");

                //赋值当前日期.
                user.FK_NY = BP.DA.DataType.CurrentYearMonth;
                //执行数据插入.
                user.Insert();
                BP.Sys.PubClass.Alert("用户注册成功");
                this.Pub1.GetButtonByID("Btn_Reg").Enabled = false;
            }
            catch (Exception ex)
            {
                BP.Sys.PubClass.Alert(ex.Message);
                return;
            }
        }
    }
}