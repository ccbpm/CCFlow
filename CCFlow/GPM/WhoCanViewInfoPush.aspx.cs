using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.GPM;

namespace GMP2.GPM
{
    public partial class WhoCanViewInfoPush : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Pub1.AddH3("如下人员可以看到该信息推送.");

            string fk_infoPush = this.Request.QueryString["FK_InfoPush"];

            // 在这里形成一棵树，部门人员.
            Emps emps = new Emps();
            emps.RetrieveInSQL("SELECT FK_Emp FROM GPM_EmpInfoPush WHERE FK_InfoPush='" + fk_infoPush + "'");

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
                    this.Pub1.Add("<a href='EmpInfoPushList.aspx?FK_Emp=" + emp.No + "'>" + emp.Name + "</a>&nbsp;");
                }
                this.Pub1.AddFieldSetEnd();
            }
        }
    }
}