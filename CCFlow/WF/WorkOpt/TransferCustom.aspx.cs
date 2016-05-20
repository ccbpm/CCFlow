using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.WF;
using BP.Web;

namespace CCFlow.WF.WorkOpt
{
    /// <summary>
    /// liuxc
    /// </summary>
    public partial class TransferCustomUI : System.Web.UI.Page
    {
        #region 参数
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["FID"]);
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            var haveNameText = false;
            var mustFilter = false;
            var step = 0;

            //获取已经走完的节点与当前的节点
            var sql = string.Format("SELECT wgw.FK_Node,wgw.FK_NodeText,wgw.FK_Emp,wgw.FK_EmpText,wgw.RDT,wgw.IsPass,wn.Step,'' FK_CC, '' FK_CCText, 0 TodolistModel FROM WF_GenerWorkerlist wgw INNER JOIN WF_Node wn ON wn.NodeID = wgw.FK_Node WHERE WorkID = {0} ORDER BY wgw.RDT", WorkID);
            var dtWorkers = DBAccess.RunSQLReturnTable(sql);
            DataTable ccs = DBAccess.RunSQLReturnTable("SELECT wsa.FK_Node,wsa.FK_Emp,wsa.EmpName,wsa.Idx FROM WF_SelectAccper wsa WHERE wsa.AccType = 1 AND wsa.WorkID = " + WorkID + " ORDER BY wsa.Idx");
            DataTable customs =
                DBAccess.RunSQLReturnTable(
                    "SELECT wtc.FK_Node,wtc.TodolistModel,wtc.Idx FROM WF_TransferCustom wtc WHERE wtc.WorkID = " +
                    WorkID);

            DataRow[] rows = null;
            string cc = string.Empty;
            string cctext = string.Empty;

            foreach(DataRow row in dtWorkers.Rows)
            {
            //处理抄送人
                rows = ccs.Select(string.Format("FK_Node={0}", row["FK_Node"]));
                cc = string.Empty;
                cctext = string.Empty;

                foreach(var r in rows)
                {
                    cc += r["FK_Emp"] + ",";
                    cctext += r["EmpName"] + ",";
                }

                row["FK_CC"] = cc.TrimEnd(',');
                row["FK_CCText"] = cctext.TrimEnd(',');

                //处理模式
                rows = customs.Select(string.Format("FK_Node={0}", row["FK_Node"]));
                if (rows.Length == 1)
                {
                    row["TodolistModel"] = rows[0]["TodolistModel"];
                }
                else if(rows.Length > 1)
                {
                    //todo:此处处理一个流程中自定义插入多个相同结点的情况，可能有
                }
            }


            var overWorks = CreateDataTableFromDataRow(dtWorkers.Clone(), dtWorkers.Select("IsPass=1", "Step ASC"));
            var currWorks = CreateDataTableFromDataRow(dtWorkers.Clone(), dtWorkers.Select("IsPass=0"));
            
            step += overWorks.Rows.Count + currWorks.Rows.Count;

            //如果是发起人，则此处为0
            if (currWorks.Rows.Count == 0)
            {
                string dt = DataType.CurrentDataTime;
                sql = string.Format("SELECT NodeID AS FK_Node,Name AS FK_NodeText, '' as  FK_Emp, '{0}' as FK_EmpText, '" + dt + "' as RDT, 0 as IsPass, Step,'' FK_CC, '' FK_CCText, 0 TodolistModel FROM WF_Node WHERE FK_Flow='{1}' AND NodeID = {2}", WebUser.Name, FK_Flow, FK_Node);
                currWorks = DBAccess.RunSQLReturnTable(sql);
                step += currWorks.Rows.Count;
            }

            litCurrentStep.Text = currWorks.Rows[0]["Step"].ToString();
            lblFK_NodeText.Text = currWorks.Rows[0]["FK_NodeText"].ToString();
            lblFK_EmpText.Text = currWorks.Rows[0]["FK_EmpText"].ToString();
            lblRDT.Text = DateTime.Parse(currWorks.Rows[0]["RDT"].ToString()).ToString("yyyy-MM-dd");

            //获取流程自定义列表
            sql = string.Format("SELECT wfc.FK_Node,wn.Name AS FK_NodeText,wfc.SubFlowNo, wf.Name AS SubFlowName, wfc.Worker,'' as WorkerText,wfc.Idx + {0} + 1 as Idx,'' FK_CC, '' FK_CCText, wfc.TodolistModel  FROM WF_TransferCustom wfc INNER JOIN WF_Node wn ON wn.NodeID = wfc.FK_Node LEFT JOIN WF_Flow wf ON wf.No = wfc.SubFlowNo WHERE wfc.WorkID = {1} ORDER BY wfc.Idx ASC", step, WorkID);
            var dtTCs = DBAccess.RunSQLReturnTable(sql);
            DataTable dtAllNodes = null;

            //如果流程自定义中没有数据，则从WF_SelectAccper获取自动计算的流程各节点处理人信息
            if (dtTCs.Rows.Count == 0)
            {
                haveNameText = true;
                mustFilter = true;
                sql = string.Format("SELECT wsa.FK_Node,wn.Name AS FK_NodeText,'' as SubFlowNo , '——' as SubFlowName,wsa.FK_Emp AS Worker,EmpName AS WorkerText,wn.Step AS Idx,'' FK_CC, '' FK_CCText, 0 TodolistModel  FROM WF_SelectAccper wsa INNER JOIN WF_Node wn ON wn.NodeID = wsa.FK_Node WHERE wsa.WorkID = {0} AND wsa.AccType = 0", WorkID);
                dtTCs = DBAccess.RunSQLReturnTable(sql);

                //如果有数据，判断这些数据是否已经包含所有的节点，如果没有包含所有的节点，则把缺失的节点补上，added by liuxc,2015-10-15
                if(dtTCs.Rows.Count > 0)
                {
                    sql = string.Format("SELECT NodeID AS FK_Node,Name AS FK_NodeText, '' as SubFlowNo, '——' as SubFlowName, '' as Worker,'' as WorkerText,Step AS Idx ,'' FK_CC, '' FK_CCText, 0 TodolistModel FROM WF_Node WHERE FK_Flow='{0}'", FK_Flow);
                    dtAllNodes = DBAccess.RunSQLReturnTable(sql);

                    //计算出所有未计算出处理人的节点
                    DataTable dtNew = dtAllNodes.Clone();
                    foreach (DataRow row in dtAllNodes.Rows)
                    {
                        //已经计算出处理人的
                        if (dtTCs.Select("FK_Node=" + row["FK_Node"]).Length > 0) continue;
                        //已经完成节点工作的
                        if (overWorks.Select("FK_Node=" + row["FK_Node"]).Length > 0) continue;
                        //当前节点工作的
                        if (Equals(FK_Node, row["FK_Node"])) continue;
                        
                        dtTCs.Rows.Add(row.ItemArray);
                    }

                    //重新排序，按照Step[Idx]
                    DataRow[] drsSorted = dtTCs.Select("1=1", "Idx ASC");
                    foreach(DataRow row in drsSorted)
                    {
                        dtNew.Rows.Add(row.ItemArray);
                    }

                    dtTCs = dtNew;
                }
            }
            else
            {
                foreach (DataRow row in dtTCs.Rows)
                {
                    //处理抄送人
                    rows = ccs.Select(string.Format("FK_Node={0}", row["FK_Node"]));
                    cc = string.Empty;
                    cctext = string.Empty;

                    foreach (var r in rows)
                    {
                        cc += r["FK_Emp"] + ",";
                        cctext += r["EmpName"] + ",";
                    }

                    row["FK_CC"] = cc.TrimEnd(',');
                    row["FK_CCText"] = cctext.TrimEnd(',');
                }
            }

            //如果WF_SelectAccper中也没有生成各节点的处理人信息，则从WF_Node中获取所有结点，以供选择
            if (dtTCs.Rows.Count == 0)
            {
                haveNameText = true;
                mustFilter = true;
                sql = string.Format("SELECT NodeID AS FK_Node,Name AS FK_NodeText, '' as SubFlowNo, '——' as SubFlowName, '' as Worker,'' as WorkerText,Step AS Idx ,'' FK_CC, '' FK_CCText, 0 TodolistModel FROM WF_Node WHERE FK_Flow='{0}'", FK_Flow);

                if (dtAllNodes != null)
                    dtTCs = dtAllNodes;
                else
                    dtTCs = DBAccess.RunSQLReturnTable(sql);
            }

            //去除已经完成+当前的结点步骤
            if (mustFilter)
            {
                overWorks.Rows.Add(currWorks.Rows[0].ItemArray);
                var removeSteps = 0;

                for (var i = 0; i < overWorks.Rows.Count; i++)
                {
                    if (dtTCs.Rows[i]["FK_Node"].Equals(overWorks.Rows[i]["FK_Node"]))
                    {
                        removeSteps++;
                    }
                }

                overWorks.Rows.RemoveAt(overWorks.Rows.Count - 1);

                while (removeSteps > 0)
                {
                    dtTCs.Rows.RemoveAt(0);
                    removeSteps--;
                }
            }

            //如果是从流程自定义中取出的数据，因为保存的是节点处理人编号，所以需要重新获取人名
            if (!haveNameText)
            {
                sql = "SELECT No,Name FROM Port_Emp";
                var dtEmps = DBAccess.RunSQLReturnTable(sql);
                string[] empArr = null;
                DataRow[] drs = null;

                foreach (DataRow w in dtTCs.Rows)
                {
                    empArr = (w["Worker"] + "").Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    foreach (var emp in empArr)
                    {
                        drs = dtEmps.Select(string.Format("No='{0}'", emp));
                        w["WorkerText"] += (drs.Length == 0 ? emp : drs[0]["Name"].ToString()) + ",";
                    }

                    w["WorkerText"] = w["WorkerText"].ToString().TrimEnd(',');
                }
            }

            rptOverNodes.DataSource = overWorks;
            rptOverNodes.DataBind();

            rptNextNodes.DataSource = dtTCs;
            rptNextNodes.DataBind();
        }

        public void Cancel()
        {
            this.Response.Redirect("../MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID, true);
        }

        protected void lbtnUseAutomic_Click(object sender, EventArgs e)
        {
            Dev2Interface.Flow_SetFlowTransferCustom(FK_Flow, WorkID,  BP.WF.TransferCustomType.ByCCBPMDefine, hid_idx_all.Value);
            Response.Redirect("../MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID, true);
        }

        protected void lbtnUseManual_Click(object sender, EventArgs e)
        {
            Dev2Interface.Flow_SetFlowTransferCustom(FK_Flow, WorkID, BP.WF.TransferCustomType.ByWorkerSet, hid_idx_all.Value);
            Response.Redirect("../MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID, true);
        }

        private DataTable CreateDataTableFromDataRow(DataTable emptyTable, DataRow[] drs)
        {
            foreach (var dr in drs)
            {
                emptyTable.Rows.Add(dr.ItemArray);
            }

            return emptyTable;
        }
    }
}