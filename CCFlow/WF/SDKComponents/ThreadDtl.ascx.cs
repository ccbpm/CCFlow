using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Web;
using BP.DA;
using BP.En;

namespace CCFlow.WF.SDKComponents
{
    public partial class ThreadDtl : BP.Web.UC.UCBase3
    {
        #region 属性.
        public int FID
        {
            get
            {
                return int.Parse(this.Request.QueryString["FID"]);
            }
        }
        public int WorkID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["WorkID"]);
                }
                catch
                {
                    return int.Parse(this.Request.QueryString["OID"]);
                }
            }
        }
        /// <summary>
        /// 节点编号
        /// </summary>
        public int FK_Node
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["FK_Node"]);
                }
                catch
                {
                    return DBAccess.RunSQLReturnValInt("SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID);
                }
            }
        }
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
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {

            Node nd = new Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();
            if (nd.HisNodeWorkType == NodeWorkType.WorkHL || nd.HisNodeWorkType == NodeWorkType.WorkFHL)
            {
            }
            else
            {
                this.AddFieldSetRed("err", "当前的节点(" + nd.Name + ")非合流点，您不能查看子线程.");
                return;
            }

            GenerWorkerLists wls = new GenerWorkerLists();
            QueryObject qo = new QueryObject(wls);
            qo.AddWhere(GenerWorkerListAttr.FID, wk.OID);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.IsEnable, 1);
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.IsPass, "!=", -2);

            int i = qo.DoQuery();
            if (i == 1)
            {
                wls.Clear();
                qo.clear();
                qo.AddWhere(GenerWorkerListAttr.FID, wk.OID);
                qo.addAnd();
                qo.AddWhere(GenerWorkerListAttr.IsEnable, 1);
                qo.addAnd();
                qo.AddWhere(GenerWorkerListAttr.IsPass, "!=", -2);
                qo.DoQuery();
            }

            //如果没有子流程就不让它显示
            if (wls.Count > 0)
            {
                this.AddTable("border=0");
                this.AddTR();
                this.AddTDTitle("IDX");
                this.AddTDTitle("节点");
                this.AddTDTitle("处理人");
                this.AddTDTitle("名称");
                this.AddTDTitle("部门");
                this.AddTDTitle("状态");
                this.AddTDTitle("应完成日期");
                this.AddTDTitle("实际完成日期");
                this.AddTDTitle("");
                this.AddTREnd();

                bool is1 = false;
                int idx = 0;
                foreach (GenerWorkerList wl in wls)
                {
                    idx++;
                    is1 = this.AddTR(is1);

                    this.AddTDIdx(idx);
                    this.AddTD(wl.FK_NodeText);
                    this.AddTD(wl.FK_Emp);

                    this.AddTD(wl.FK_EmpText);
                    this.AddTD(wl.FK_DeptT);

                    if (wl.IsPass)
                    {
                        this.AddTD("已完成");
                        this.AddTD(wl.SDT);
                        this.AddTD(wl.RDT);
                    }
                    else
                    {
                        this.AddTD("<font color=red>未完成</font>");
                        this.AddTD(wl.SDT);
                        this.AddTD();
                    }

                    if (wl.IsPass == false)
                    {
                        if (nd.ThreadKillRole == ThreadKillRole.ByHand)
                            this.AddTD("<a href=\"javascript:DoDelSubFlow('" + wl.FK_Flow + "','" + wl.WorkID + "')\"><img src='" + BP.WF.Glo.CCFlowAppPath + "WF/Img/Btn/Delete.gif' border=0/>终止</a>");
                        else
                            this.AddTD();
                    }
                    else
                    {
                        this.AddTD("<a href=\"javascript:WinOpen('" + BP.WF.Glo.CCFlowAppPath + "WF/WorkOpt/FHLFlow.aspx?WorkID=" + wl.WorkID + "&FID=" + wl.FID + "&FK_Flow=" + nd.FK_Flow + "&FK_Node=" + this.FK_Node + "','po9')\">打开</a>");
                    }
                    this.AddTREnd();
                }
                this.AddTableEnd();
            }
        }
    }
}