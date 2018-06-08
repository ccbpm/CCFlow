using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.GPM;

namespace GMP2.GPM
{
    public partial class WhoCanUseApp : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string fk_app = this.Request.QueryString["FK_App"];
            if (this.Request.QueryString["IsRef"] != null)
            {
                /*开始执行刷新.*/
                BP.GPM.App app = new BP.GPM.App(fk_app);
                app.RefData();
                BP.Sys.PubClass.Alert("@刷新成功,点确定后就可以查看可执行系统的人员.");
            }

            this.Pub1.AddH3("可以使用本系统的人员");
            this.Pub1.AddHR();

            // 在这里形成一棵树，部门人员.
            Emps emps = new Emps();
            emps.RetrieveInSQL("SELECT FK_Emp FROM GPM_EmpApp WHERE FK_App='"+fk_app+"'");

            // 部门集合.
            Depts depts = new Depts();
            depts.RetrieveAll();
            foreach (Dept dept in depts)
            {
                this.Pub1.AddBR();
                this.Pub1.AddFieldSet(dept.Name);
                foreach (Emp emp in emps)
                {
                    if (emp.FK_Dept != dept.No)
                        continue;
                    this.Pub1.Add("<a href='EmpAppMenu.aspx?FK_Emp=" + emp.No + "&FK_App=" + fk_app + "'>" + emp.Name + "</a>&nbsp;");
                }
                this.Pub1.AddFieldSetEnd();
            }
        }
    }
}