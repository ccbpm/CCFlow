using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GMP2.GPM
{
    public partial class EmpForMenus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Emps emps = new Emps();
            //emps.RetrieveAll("No");
            //lbEmp.Items.Clear();
            //if (emps != null && emps.Count > 0)
            //{
            //    foreach (Emp emp in emps)
            //    {
            //        ListItem item = new ListItem();
            //        item.Text = "[" + emp.No + "]" + emp.Name;
            //        item.Value = emp.No;
            //        lbEmp.Items.Add(item);
            //    }
            //}
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string empNo = this.empNo.Value;
            string menuIds = this.menuIds.Value;
            string menuIdsUn = this.menuIdsUn.Value;
        }
    }
}