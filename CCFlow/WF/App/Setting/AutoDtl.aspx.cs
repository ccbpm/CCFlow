using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.Web.Controls;


namespace CCFlow.WF.App.Setting
{
    public partial class AutoDtl : System.Web.UI.Page
    {   
      
        protected void Page_Load(object sender, EventArgs e)
        {

          
            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(BP.Web.WebUser.No);
            BP.WF.Port.WFEmp empAu = new BP.WF.Port.WFEmp(Request["FK_Emp"]);
            this.sqg.Text = empAu.No + "    " + empAu.Name;


            TB tb = new TB();
            tb.ID = "sqrq";
            System.DateTime dtNow = System.DateTime.Now;
            dtNow = dtNow.AddDays(14);
           this.sqrq.Text = dtNow.ToString(DataType.SysDataTimeFormat);
            tb.ShowType = TBType.DateTime;
            tb.Attributes["onfocus"] = "WdatePicker({dateFmt:'yyyy-MM-dd HH:mm'});";
        }

        protected void BtnSave_Click(object sender, EventArgs e)
        {

            BP.WF.Port.WFEmp emp = new BP.WF.Port.WFEmp(BP.Web.WebUser.No);
            emp.AuthorDate = BP.DA.DataType.CurrentData;
            emp.Author = this.Request["FK_Emp"];
            emp.AuthorToDate = this.sqrq.Text;
            emp.AuthorWay =  int.Parse(this.sel.Items[sel.SelectedIndex].Value);
            if (emp.AuthorWay == 2 && emp.AuthorFlows.Length < 3)
            {
                this.Response.Write("您指定授权方式是按指定的流程范围授权，但是您没有指定流程的授权范围.");
                return;
            }
            emp.Update();
            //BP.Sys.UserLog.AddLog("Auth", WebUser.No, "全部授权");
            BP.Sys.Glo.WriteUserLog("Auth", BP.Web.WebUser.No, "全部授权");
           this.Response.Redirect("Default.aspx");
        }

      
    }
}