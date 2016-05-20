using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Port;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF
{
    public partial class WF_BPR : BP.Web.WebPage
    {
        #region 属性
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        public string FK_NY
        {
            get
            {
                return this.Request.QueryString["FK_NY"];
            }
        }
        public string FK_Emp
        {
            get
            {
                return this.Request.QueryString["FK_Emp"];
            }
        }
        #endregion 属性

        public string GetDoType
        {
            get
            {
                if (this.DoType != null)
                    return this.DoType;

                if (this.FK_NY == null && this.FK_Emp == null && this.FK_Node == null)
                    return "1.ShowFlowNodes";

                if (this.FK_Node != null)
                {
                    return "1.1ShowFlowNodeEmp";
                }

                if (this.FK_Node != null && this.FK_Emp != null)
                {
                    return "1.1.1ShowNodeEmp";
                }

                return "1.ShowFlowNodes";
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            Flow fl = new Flow(this.FK_Flow);
            this.Title = "流程成本分析-" + fl.Name;
            switch (this.GetDoType)
            {
                case "NodeEmp":
                    this.BindNodeEmp(fl);
                    break;
                case "1.1ShowFlowNodeEmp":
                    this.Bind1_1_ShowFlowNodeEmp(fl);
                    break;
                case "1.ShowFlowNodes":
                default:
                    this.Bind1_ShowFlowNodes(fl);
                    break;
            }
        }
        public void Bind1_1_ShowFlowNodeEmp(Flow fl)
        {
            Node nd = new Node(int.Parse(this.FK_Node));

            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft("<a href='BPR.aspx?FK_Flow=" + this.FK_Flow + "'>返回</a> - " + fl.Name + "-" + nd.Name);
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("Idx");
            this.Pub1.AddTDTitle("操作员");
            this.Pub1.AddTDTitle("执行数量");
            this.Pub1.AddTREnd();

            string sql = "SELECT DISTINCT Rec FROM ND" + nd.NodeID;
            BP.WF.Port.WFEmps emps = new BP.WF.Port.WFEmps();
            emps.RetrieveInSQL(sql);

            int idx = 0;
            bool is1 = false;
            float val = 0;
            foreach (BP.WF.Port.WFEmp emp in emps)
            {
                idx++;
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTDIdx(idx);
                this.Pub1.AddTD("<a href=\"javascript:WinOpen('BPR.aspx?FK_Flow=" + this.FK_Flow + "&FK_Emp=" + emp.No + "&FK_Node=" + this.FK_Node + "')\">" + emp.Name);
                val = DBAccess.RunSQLReturnValFloat("SELECT COUNT(WorkID) FROM ND" + nd.NodeID + " WHERE Rec='" + emp.No + "'");
                this.Pub1.AddTDNum("<a href='BPR.aspx?FK_Flow=" + this.FK_Flow + "&FK_Emp=" + emp.No + "&FK_Node=" + this.FK_Node + "'>" + val + "</a>");
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }
        public void Bind1_ShowFlowNodes(Flow fl)
        {
            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft(fl.Name);
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("Idx");
            this.Pub1.AddTDTitle("步骤");
            this.Pub1.AddTDTitle("节点");
            this.Pub1.AddTDTitle("工作数");
            this.Pub1.AddTDTitle("平均用时(天)");
            this.Pub1.AddTDTitle("人员数");


            this.Pub1.AddTDTitle("警告期限(0不警告)");
            this.Pub1.AddTDTitle("限期(天)");


            this.Pub1.AddTDTitle("逾期人次");
            this.Pub1.AddTDTitle("逾期率");


            this.Pub1.AddTDTitle("岗位");
            this.Pub1.AddTREnd();
            Nodes nds = fl.HisNodes;
            int idx = 0;
            bool is1 = false;
            float val = 0;
            foreach (BP.WF.Node nd in nds)
            {
                idx++;
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTDIdx(idx);
                this.Pub1.AddTD("第:" + nd.Step + "步");
                this.Pub1.AddTD(nd.Name);

                val = DBAccess.RunSQLReturnValFloat("SELECT COUNT(WorkID) FROM ND" + nd.NodeID + " ");
                this.Pub1.AddTD(val);

                switch (BP.Sys.SystemConfig.AppCenterDBType)
                {
                    case DBType.Oracle:
                        val = DBAccess.RunSQLReturnValFloat("SELECT NVL(AVG(to_date(substr(RDT,1,10),'yyyy-mm-dd')- to_date(substr(CDT,1,10),'yyyy-mm-dd')),0) FROM ND" + nd.NodeID);
                        break;
                    default:
                        val = DBAccess.RunSQLReturnValFloat("SELECT IsNULL( Avg(dbo.GetSpdays(RDT,CDT)),0) FROM ND" + nd.NodeID + " ");
                        break;
                }

                this.Pub1.AddTD(val);

                val = DBAccess.RunSQLReturnValFloat("SELECT COUNT(DISTINCT Rec) FROM ND" + nd.NodeID + " ");
                this.Pub1.AddTDNum("<a href=\"javascript:WinOpen('BPR.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + nd.NodeID + "&DoType=NodeEmp')\" >" + val + "</a>");

                this.Pub1.AddTD(nd.WarningHour);
             //   this.Pub1.AddTD(nd.TSpanDay+"天"+nd.TSpanHour+"小时");

                this.Pub1.AddTD(0);
                this.Pub1.AddTD(0);

                this.Pub1.AddTDDoc(nd.HisStationsStr, nd.HisStationsStr);
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }

        public void BindNodeEmp(Flow fl)
        {
            Node nd = new Node(int.Parse(this.FK_Node));
            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft("<a href='BPR.aspx?FK_Flow=" + this.FK_Flow + "'>返回</a> - " + fl.Name + " - " + nd.Name);
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("Idx");
            this.Pub1.AddTDTitle("步骤");
            this.Pub1.AddTDTitle("节点");
            this.Pub1.AddTDTitle("工作数");
            this.Pub1.AddTDTitle("平均用时(天)");
            this.Pub1.AddTDTitle("人员数");
            this.Pub1.AddTDTitle("岗位");
            this.Pub1.AddTREnd();
            //Nodes nds = fl.HisNodes;
            //int idx = 0;
            //bool is1 = false;
            //float val = 0;
            //foreach (BP.WF.Node nd in nds)
            //{
            //    idx++;
            //    is1 = this.Pub1.AddTR(is1);
            //    this.Pub1.AddTDIdx(idx);
            //    this.Pub1.AddTD("第:" + nd.Step + "步");
            //    this.Pub1.AddTD(nd.Name);

            //    val = DBAccess.RunSQLReturnValFloat("SELECT COUNT(*) FROM ND" + nd.NodeID + " ");
            //    this.Pub1.AddTD(val);

            //    val = DBAccess.RunSQLReturnValFloat("SELECT IsNULL( Avg(dbo.GetSpdays(RDT,CDT)),0) FROM ND" + nd.NodeID + " ");
            //    this.Pub1.AddTD(val);

            //    val = DBAccess.RunSQLReturnValFloat("SELECT COUNT(DISTINCT Rec) FROM ND" + nd.NodeID + " ");
            //    this.Pub1.AddTDNum("<a href=\"javascript:WinOpen('BPR.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + nd.NodeID + "&DoType=NodeEmp')\" >" + val + "</a>");

            //    this.Pub1.AddTDDoc(nd.HisStationsStr, nd.HisStationsStr);
            //    this.Pub1.AddTREnd();
            //}
            this.Pub1.AddTableEnd();
        }
    }
}