using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace CCFlow.WF.App.Setting
{
    public partial class Auto : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
               string sql = "SELECT a.No,a.Name,b.Name as DeptName FROM Port_Emp a, Port_Dept b WHERE a.FK_Dept=b.No AND a.FK_Dept LIKE '" + BP.Web.WebUser.FK_Dept + "%' ORDER  BY a.FK_Dept ";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                this.repList.DataSource = dt;
               this.repList.DataBind();
 
           
        }
    }
}