using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Port;
using BP.Web;

namespace CCFlow.WF.UC
{
    public partial class ChangeDept : BP.Web.UC.UCBase3
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string toDept = this.Request.QueryString["FK_Dept"];
            if (string.IsNullOrEmpty(toDept)==false)
            {
                string updataSQL = BP.WF.Glo.UpdataMainDeptSQL.Clone() as string;
                updataSQL = updataSQL.Replace("@FK_Dept", toDept);
                updataSQL = updataSQL.Replace("@No", WebUser.No);
                BP.DA.DBAccess.RunSQL(updataSQL);

                this.AddFieldSet("提示");
                this.Add("切换成功!");
                this.AddFieldSetEnd();
                return;
            }

            if (WebUser.No == null)
            {
                this.AddFieldSet("提示");
                this.Add("您尚未登录!!!!");
                this.AddFieldSetEnd();
                return;
            }

            string sql = "select No,Name FROM Port_Dept WHERE No IN (SELECT FK_Dept FROM Port_DeptEmp WHERE FK_Emp='" + WebUser.No + "')";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 1)
            {
                this.AddFieldSet("提示:");
                this.Add("您只有一个部门，所以不能切换登录。");
                this.AddFieldSetEnd();
                return;
            }

            if (dt.Rows.Count == 0)
            {
                this.AddFieldSet("系统错误:");
                this.Add("没有找到您的部门集合，请告知管理员。");
                this.AddFieldSetEnd();
                return;
            }


            this.AddFieldSet("您好:" + WebUser.No + "," + WebUser.Name + "; 当前的部门:" + WebUser.FK_DeptName);

            this.AddUL();
            foreach (DataRow dr in dt.Rows)
            {
                string fk_dept = dr["No"].ToString();
                if (fk_dept == WebUser.FK_Dept)
                    this.AddLi(WebUser.FK_DeptName);
                else
                    this.AddLi("<a href='ChangeDept.aspx?FK_Dept=" + dr["No"] + "'>" + dr["Name"] + "</a>");
            }
            this.AddULEnd();

            this.AddFieldSetEnd();
            return;
        }
    }
}