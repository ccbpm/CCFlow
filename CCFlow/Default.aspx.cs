using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF;
using BP.Web;

namespace CCFlow
{
    public partial class Default : WebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //BP.Port.Emp emp = new BP.Port.Emp();
            //emp.No = "zhoupeng";
            //emp.RetrieveFromDBSources();

            //BP.Port.Emp emp = new BP.Port.Emp();
            //emp.No = "zhoupeng";
            //emp.RetrieveFromDBSources();
            this.Response.Redirect("./WF/Admin/CCBPMDesigner/Login.aspx", true);
            return;
           
        }

        public void AddNodeInfo()
        {
            BP.WF.Nodes nds = new Nodes();
            nds.RetrieveAll();
            foreach (Node nd in nds)
            {
                nd.ReturnAlert = "<a href=~MyFlow.aspx?FK_Flow=001&PFlowNo=@FK_Flow&PNodeID=@FK_Node&PWorkID=@WorkID~  target=_blank >我要举报恶意退回</a>";
                nd.DirectUpdate();
            }
        }

        public void BatchReNameByDeptPinYin()
        {

            string sql = "SELECT * FROM Port_Emp"; //要重命名的范围.
            DataTable dt=BP.DA.DBAccess.RunSQLReturnTable(sql);

            foreach (DataRow  dr in dt.Rows)
            {

                BP.Port.Emp emp = new BP.Port.Emp(dr["No"].ToString());
                string deptName = BP.DA.DataType.ParseStringToPinyin(emp.FK_DeptText);

                string isExit = "SELECT * FROM port_emp where no='"+deptName+"'";
                if (DBAccess.RunSQLReturnTable(isExit).Rows.Count > 0)
                    continue;

                sql = "UPDATE Port_Emp Set No='"+deptName+"' where no='"+emp.No+"'";
                BP.DA.DBAccess.RunSQL(sql);

                sql = "UPDATE WF_Emp Set No='" + deptName + "' where no='" + emp.No + "'";
                BP.DA.DBAccess.RunSQL(sql);


                sql = "UPDATE Port_EmpStation Set FK_Emp='" + deptName + "' WHERE FK_Emp='" + emp.No + "'";
                BP.DA.DBAccess.RunSQL(sql);

                sql = "UPDATE Port_EmpDept Set FK_Emp='" + deptName + "' WHERE FK_Emp='" + emp.No + "'";
                BP.DA.DBAccess.RunSQL(sql);

                sql = "UPDATE WF_NodeEmp Set FK_Emp='" + deptName + "' WHERE FK_Emp='" + emp.No + "'";
                BP.DA.DBAccess.RunSQL(sql);



                sql = "UPDATE GPM_ByEmp Set FK_Emp='" + deptName + "' WHERE FK_Emp='" + emp.No + "'";
                BP.DA.DBAccess.RunSQL(sql);


                sql = "UPDATE GPM_EmpApp Set FK_Emp='" + deptName + "' WHERE FK_Emp='" + emp.No + "'";
                BP.DA.DBAccess.RunSQL(sql);


                sql = "UPDATE GPM_EmpMenu Set FK_Emp='" + deptName + "' WHERE FK_Emp='" + emp.No + "'";
                BP.DA.DBAccess.RunSQL(sql);


                sql = "UPDATE GPM_GroupEmp Set FK_Emp='" + deptName + "' WHERE FK_Emp='" + emp.No + "'";
                BP.DA.DBAccess.RunSQL(sql);

                sql = "UPDATE GPM_UserMenu Set FK_Emp='" + deptName + "' WHERE FK_Emp='" + emp.No + "'";
                BP.DA.DBAccess.RunSQL(sql);

            }
        }
 
    }
}