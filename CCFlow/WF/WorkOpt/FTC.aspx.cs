using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.Template;
using BP.DA;
using BP.En;

namespace CCFlow.WF.WorkOpt
{
    public partial class FTC : System.Web.UI.Page
    {
        #region 属性.
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                string str = this.Request.QueryString["FK_Node"];
                if (string.IsNullOrEmpty(str) == true)
                    str = this.Request.QueryString["NodeID"];
                if (string.IsNullOrEmpty(str))
                    str = "0";
                return int.Parse(str);
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                string str=this.Request.QueryString["OID"];
                if (string.IsNullOrEmpty(str)==true)
                    str = this.Request.QueryString["WorkID"];
                return Int64.Parse(str);
            }
        }
        /// <summary>
        /// FID
        /// </summary>
        public Int64 FID
        {
            get
            {
                string str = this.Request.QueryString["FID"];
                    str = "0";
                return int.Parse(str);
            }
        }
        #endregion 属性

        protected void Page_Load(object sender, EventArgs e)
        {
            //配置信息.
            FrmTransferCustom ftc = new FrmTransferCustom(this.FK_Node);
            if (ftc.FTCWorkModel == 1)
            {
                /*如果是高级模式,就让其转到高级模式的设置.*/
                string url=this.Request.RawUrl.Replace("FTC.","TransferCustom.");
                this.Response.Redirect(url,true);
                return;
            }

            TransferCustoms tcs = new TransferCustoms(this.WorkID);
            GenerWorkerLists gwls = new GenerWorkerLists(this.WorkID);

            Nodes nds = new Nodes(this.FK_Flow);

            this.Pub1.AddTable("width=95%");
            this.Pub1.AddTR();
            this.Pub1.AddTD("步骤");
            this.Pub1.AddTD("节点");
            this.Pub1.AddTD("处理人");
            this.Pub1.AddTD("计划完成日期");
            this.Pub1.AddTD("实际完成日期");
            this.Pub1.AddTREnd();

            foreach (Node nd in nds)
            {

                /*如果是当前节点，并且当前节点是开始节点.*/
                if (nd.NodeID == this.FK_Node && nd.IsStartNode==true)
                {
                    this.Pub1.AddTR();
                    this.Pub1.AddTD(nd.Step);
                    this.Pub1.AddTD(nd.Name);
                    this.Pub1.AddTD("您自己");
                    this.Pub1.AddTD("无"); //计划完成日期.
                    this.Pub1.AddTD("无"); //实际完成日期.
                    this.Pub1.AddTREnd();
                    continue;
                }

                /*如果是当前节点, */
                if (nd.NodeID == this.FK_Node)
                {
                    TransferCustom tc = tcs.GetEntityByKey(GenerWorkerListAttr.FK_Node, nd.NodeID) as TransferCustom;
                    if (tc == null)
                        tc = new TransferCustom();

                    this.Pub1.AddTR();
                    this.Pub1.AddTD(nd.Step);
                    this.Pub1.AddTD(nd.Name);
                    this.Pub1.AddTD(tc.WorkerName);
                    this.Pub1.AddTD(tc.PlanDT); //计划完成日期.
                    this.Pub1.AddTD("无"); //实际完成日期.
                    this.Pub1.AddTREnd();
                    continue;
                }


                GenerWorkerList gwl = gwls.GetEntityByKey(GenerWorkerListAttr.FK_Node, nd.NodeID) as GenerWorkerList;
                if (gwl == null)
                {
                    /* 还没有到达的节点. */
                    TransferCustom tc = tcs.GetEntityByKey(GenerWorkerListAttr.FK_Node, nd.NodeID) as TransferCustom;
                    if (tc == null)
                        tc = new TransferCustom();
                    this.Pub1.AddTR();
                    this.Pub1.AddTD(nd.Step);
                    this.Pub1.AddTD(nd.Name);

                    if (ftc.FTCSta == FTCSta.ReadOnly)
                    {
                        /* 如果是只读. */
                        this.Pub1.AddTD(tc.WorkerName);
                        this.Pub1.AddTD(tc.PlanDT); //计划完成日期.
                        this.Pub1.AddTD("无");
                    }
                    else
                    {
                        BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
                        ddl.ID = "DDL_" + nd.NodeID;
                        DataSet ds = BP.WF.Dev2Interface.WorkOpt_AccepterDB(nd.NodeID, this.WorkID);

                        BP.Web.Controls.Glo.DDL_BindDataTable(ddl, ds.Tables["Port_Emp"], tc.Worker,"No","Name");
                        this.Pub1.AddTD(ddl);

                        TextBox tb = new TextBox();
                        tb.ID = "TB_PlanDT_" + nd.NodeID;
                        tb.Text = tc.PlanDT;
                        tb.Attributes["onfocus"] = "WdatePicker();";
                        this.Pub1.AddTD(tb);  //计划完成日期.
                        this.Pub1.AddTD("无");
                    }

                    this.Pub1.AddTREnd();
                }

                if (gwl != null)
                {
                    /*已经走过的节点*/
                    this.Pub1.AddTR();
                    this.Pub1.AddTD(nd.Step);
                    this.Pub1.AddTD(nd.Name);
                    this.Pub1.AddTD(gwl.FK_EmpText);
                    this.Pub1.AddTD(gwl.SDT); //计划完成日期.
                    this.Pub1.AddTD(gwl.CDT); //实际完成日期.
                    this.Pub1.AddTREnd();
                }
            }
            this.Pub1.AddTableEnd();
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            Nodes nds = new Nodes(this.FK_Flow);
            foreach (Node nd in nds)
            {
                TextBox tb = this.Pub1.GetTextBoxByID("TB_PlanDT_"+nd.NodeID);
                if (tb == null)
                    continue;

                TransferCustom tfc = new TransferCustom();
                tfc.FK_Node = nd.NodeID;
                tfc.PlanDT = tb.Text;

                //工作人员,多个用逗号分开.
                tfc.Worker = this.Pub1.GetDDLByID("DDL_" + nd.NodeID).SelectedItemStringVal; ;

                //选择人的名字,多个用逗号分开.
                tfc.WorkerName = this.Pub1.GetDropDownListByID("DDL_" + nd.NodeID).SelectedItem.Text;

                tfc.WorkID = this.WorkID;

                tfc.MyPK = tfc.FK_Node + "_" + this.WorkID ;

                tfc.Idx = nd.Step;

                tfc.Save(); //执行保存.
            }

            //设置流程为自动运行模式.
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            gwf.TransferCustomType = TransferCustomType.ByWorkerSet;
            gwf.Update();

        }
    }
}