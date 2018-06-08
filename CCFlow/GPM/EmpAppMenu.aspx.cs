using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace GMP2.GPM
{
    public partial class EmpAppMenu : System.Web.UI.Page
    {
        public string title = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            string fk_emp = this.Request.QueryString["FK_Emp"];
            string fk_app = this.Request.QueryString["FK_App"];

            BP.GPM.Emp emp = new BP.GPM.Emp(fk_emp);
            this.Title = emp.Name;
            title = "<a href='WhoCanUseApp.aspx?FK_App=" + fk_app + "' >返回</a> - " + emp.Name + " - 在本系统中的菜单.";
        }
    }
}