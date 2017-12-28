using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.OneWork
{
public partial class WF_WorkOpt_OneWork_CH : System.Web.UI.Page
{
    public string FK_Flow
    {
        get
        {
            return this.Request.QueryString["FK_Flow"];
        }
    }
    public string WorkID
    {
        get
        {
            return this.Request.QueryString["WorkID"];
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

        this.Pub1.AddTable("width=100%");
        //    this.Pub1.AddCaptionLeftTX("工作考核信息");
        this.Pub1.AddTR();
        this.Pub1.AddTDTitle("IDX");
        this.Pub1.AddTDTitle("节点");
        this.Pub1.AddTDTitle("处理人");
        this.Pub1.AddTDTitle("接受时间");
        this.Pub1.AddTDTitle("限期(天)");
        this.Pub1.AddTDTitle("应完成时间");
        this.Pub1.AddTDTitle("实际完成时间");
        this.Pub1.AddTDTitle("状态");
        this.Pub1.AddTREnd();

        string sql = "SELECT A.*, B.Name AS FK_NodeT,B.DeductDays, c.Name as EmpName  FROM V" + this.FK_Flow + " A, WF_Node B, WF_Emp C WHERE A.FK_Node=b.NodeID AND A.Rec=C.No AND A.OID=" + this.WorkID;

        DataTable dt = null;
        try
        {
            dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
        }
        catch
        {
            this.Pub1.AddTableEnd();
            this.Pub1.AddFieldSetRed("错误", 
                "V" + this.FK_Flow + " view没有生成, 请告诉管理员，执行在流程设计器中执行流程检查可以解决此问题.");
            return;
        }

        int idx = 0;
        foreach (DataRow dr in dt.Rows)
        {
            idx++;
            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx);
            this.Pub1.AddTD(dr["FK_NodeT"].ToString());
            this.Pub1.AddTD(dr["EmpName"].ToString());
            this.Pub1.AddTD(dr["RDT"].ToString());

            int deductDays = int.Parse(dr["DeductDays"].ToString());
            this.Pub1.AddTD(deductDays);

            // 应该完成时间.
            DateTime cdt = BP.DA.DataType.ParseSysDateTime2DateTime(dr["CDT"].ToString());
            DateTime sdt = cdt.AddDays(int.Parse(dr["DeductDays"].ToString()));
            this.Pub1.AddTD(sdt.ToString(BP.DA.DataType.SysDatatimeFormatCN));
            this.Pub1.AddTD(dr["CDT"].ToString());

            if (sdt < cdt)
                this.Pub1.AddTD("逾期");
            else
                this.Pub1.AddTD("正常");

            this.Pub1.AddTREnd();
        }
        this.Pub1.AddTableEnd();
    }
}
}