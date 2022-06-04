using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_WorkOpt_Batch : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_WorkOpt_Batch()
        {
        }

        #region  界面 .
        public string WorkCheckModel_Init()
        {
            DataSet ds = new DataSet();

            string FK_Node = GetRequestVal("FK_Node");

            //获取节点信息
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            Flow fl = nd.HisFlow;
            ds.Tables.Add(nd.ToDataTableField("WF_Node"));

            string sql = "";

            if (nd.HisRunModel == RunModel.SubThread)
            {
                sql = "SELECT a.*, b.Starter,b.ADT,b.WorkID FROM " + fl.PTable
                          + " a , WF_EmpWorks b WHERE a.OID=B.FID AND b.WFState Not IN (7) AND b.FK_Node=" + nd.NodeID
                          + " AND b.FK_Emp='" + WebUser.No + "'";
            }
            else
            {
                sql = "SELECT a.*, b.Starter,b.ADT,b.WorkID FROM " + fl.PTable
                        + " a , WF_EmpWorks b WHERE a.OID=B.WorkID AND b.WFState Not IN (7) AND b.FK_Node=" + nd.NodeID
                        + " AND b.FK_Emp='" + WebUser.No + "'";
            }

            //获取待审批的流程信息集合
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Works";
            ds.Tables.Add(dt);


            //获取按钮权限
            BtnLab btnLab = new BtnLab(this.FK_Node);
            ds.Tables.Add(btnLab.ToDataTableField("Sys_BtnLab"));

        
            int nodeID = nd.NodeID;

            //获取字段属性
            MapAttrs attrs = new MapAttrs("ND" + nodeID);

            //获取实际中需要展示的列.
            string batchParas = nd.GetParaString("BatchFields");
            MapAttrs realAttr = new MapAttrs();
            if (DataType.IsNullOrEmpty(batchParas) == false)
            {
                string[] strs = batchParas.Split(',');
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str)
                        || str.Contains("@PFlowNo") == true)
                        continue;

                    foreach (MapAttr attr in attrs)
                    {
                        if (str != attr.KeyOfEn)
                            continue;

                        realAttr.AddEntity(attr);
                    }
                }
            }

            ds.Tables.Add(realAttr.ToDataTableField("Sys_MapAttr"));

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 审核组件模式：批量发送
        /// </summary>
        /// <returns></returns>
        public string WorkCheckModel_Send()
        {
            //审核批量发送.
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

            //获取要批处理数据
            string sql = string.Format("SELECT WorkID, FID,Title FROM WF_EmpWorks WHERE FK_Emp='{0}' and FK_Node='{1}'", WebUser.No, this.FK_Node);
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            int idx = -1;
            string msg = "";

            //判断是否有传递来的参数.
            int toNode = this.GetRequestValInt("ToNode");
            string toEmps = this.GetRequestVal("ToEmps");

            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr[0].ToString());
                string cb = this.GetValFromFrmByKey("CB_" + workid, "0");
                if (cb.Equals("on") == false)
                    continue;

                //是否启用了审核组件？
                if (nd.FrmWorkCheckSta == FrmWorkCheckSta.Enable)
                {
                    //获取审核意见的值
                    string checkNote = "";

                    //选择的多条记录一个意见框.
                    var model = nd.GetParaInt("BatchCheckNoteModel", 0);

                    //多条记录一个意见.
                    if (model == 0)
                        checkNote = this.GetRequestVal("CheckNote");

                    //每条记录都有自己的意见.
                    if (model == 1)
                        checkNote = this.GetValFromFrmByKey("TB_" + workid + "_WorkCheck_Doc", null);

                    if (model == 2)
                        checkNote = " ";

                    //写入审核意见.
                    if (DataType.IsNullOrEmpty(checkNote) == false)
                        BP.WF.Dev2Interface.WriteTrackWorkCheck(nd.FK_Flow, nd.NodeID, workid, Int64.Parse(dr["FID"].ToString()), checkNote, null, null);
                }

                //设置字段的默认值.
                Work wk = nd.HisWork;
                wk.OID = workid;
                wk.Retrieve();
                wk.ResetDefaultVal();
                wk.Update();

                //执行工作发送.
                msg += "@对工作(" + dr["Title"] + ")处理情况如下";
                BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(nd.FK_Flow, workid, toNode, toEmps);
                msg += objs.ToMsgOfHtml();
                msg += "<br/>";
            }

            if (msg == "")
                msg = "没有选择需要处理的工作";

            return msg;
        }

        public string BatchList_Init()
        {
            DataSet ds = new DataSet();

            string FK_Node = GetRequestVal("FK_Node");

            //获取节点信息
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            Flow fl = nd.HisFlow;
            ds.Tables.Add(nd.ToDataTableField("WF_Node"));

            string sql = "";

            if (nd.HisRunModel == RunModel.SubThread)
            {
                sql = "SELECT a.*, b.Starter,b.ADT,b.WorkID FROM " + fl.PTable
                          + " a , WF_EmpWorks b WHERE a.OID=B.FID AND b.WFState Not IN (7) AND b.FK_Node=" + nd.NodeID
                          + " AND b.FK_Emp='" + WebUser.No + "'";
            }
            else
            {
                sql = "SELECT a.*, b.Starter,b.ADT,b.WorkID FROM " + fl.PTable
                        + " a , WF_EmpWorks b WHERE a.OID=B.WorkID AND b.WFState Not IN (7) AND b.FK_Node=" + nd.NodeID
                        + " AND b.FK_Emp='" + WebUser.No + "'";
            }

            //获取待审批的流程信息集合
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            dt.TableName = "Batch_List";
            ds.Tables.Add(dt);

            //获取按钮权限
            BtnLab btnLab = new BtnLab(this.FK_Node);

            ds.Tables.Add(btnLab.ToDataTableField("Sys_BtnLab"));

            //获取报表数据
            string inSQL = "SELECT WorkID FROM WF_EmpWorks WHERE FK_Emp='" + WebUser.No + "' AND WFState!=7 AND FK_Node=" + this.FK_Node;
            Works wks = nd.HisWorks;
            wks.RetrieveInSQL(inSQL);

            ds.Tables.Add(wks.ToDataTableField("WF_Work"));

            //获取字段属性
            MapAttrs attrs = new MapAttrs("ND" + this.FK_Node);

            //获取实际中需要展示的列
            string batchParas = nd.BatchParas;
            MapAttrs realAttr = new MapAttrs();
            if (DataType.IsNullOrEmpty(batchParas) == false)
            {
                string[] strs = batchParas.Split(',');
                foreach (string str in strs)
                {
                    if (string.IsNullOrEmpty(str)
                        || str.Contains("@PFlowNo") == true)
                    {
                        continue;
                    }

                    foreach (MapAttr attr in attrs)
                    {
                        if (str != attr.KeyOfEn)
                        {
                            continue;
                        }

                        realAttr.AddEntity(attr);
                    }
                }
            }

            ds.Tables.Add(realAttr.ToDataTableField("Sys_MapAttr"));

            return BP.Tools.Json.ToJson(ds);
        }

        #endregion 界面方法.

        #region 通用方法.
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <returns></returns>
        public string Batch_Delete()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string workIDs = this.GetRequestVal("WorkIDs");
            if (DataType.IsNullOrEmpty(workIDs) == true)
                return "err@没有选择需要处理的工作";
            string msg = "";
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.RetrieveIn("WorkID", workIDs);
            foreach (GenerWorkFlow gwf in gwfs)
            {
                msg += "@对工作(" + gwf.Title + ")处理情况如下。<br>";
                string mes = BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(gwf.WorkID, "批量删除", true);
                msg += mes;
                msg += "<hr>";
            }
            return "批量删除成功" + msg;

            /*  MapAttrs attrs = new MapAttrs("ND" + this.FK_Node);

              //获取数据
              string sql = string.Format("SELECT Title,RDT,ADT,SDT,FID,WorkID,Starter FROM WF_EmpWorks WHERE FK_Emp='{0}' and FK_Node='{1}'", WebUser.No, this.FK_Node);

              DataTable dt = DBAccess.RunSQLReturnTable(sql);
              int idx = -1;
              string msg = "";
              foreach (DataRow dr in dt.Rows)
              {
                  idx++;
                  if (idx == nd.BatchListCount)
                  {
                      break;
                  }

                  Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                  string cb = this.GetValFromFrmByKey("CB_" + workid, "0");
                  if (cb == "0") //没有选中
                  {
                      continue;
                  }

                  msg += "@对工作(" + dr["Title"] + ")处理情况如下。<br>";
                  string mes = BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(nd.FK_Flow, workid, "批量删除", true);
                  msg += mes;
                  msg += "<hr>";

              }
              if (msg == "")
              {
                  msg = "没有选择需要处理的工作";
              }*/


        }
        /// <summary>
        /// 批量退回 待定
        /// </summary>
        /// <returns></returns>
        public string Batch_Return()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string workIDs = this.GetRequestVal("WorkIDs");
            if (DataType.IsNullOrEmpty(workIDs) == true)
                workIDs = this.WorkID.ToString();

                //return "err@没有选择需要处理的工作";
            string msg = "";

            string[] vals = this.GetRequestVal("ReturnToNode").Split('@');
            int toNodeID = int.Parse(vals[0]);

            string toEmp = vals[1];
            string reMesage = this.GetRequestVal("ReturnInfo");

            bool isBackBoolen = false;
            if (this.GetRequestVal("IsBack").Equals("1") == true)
                isBackBoolen = true;

            bool isKill = false; //是否全部退回.
            string isKillEtcThread = this.GetRequestVal("IsKillEtcThread");
            if (DataType.IsNullOrEmpty(isKillEtcThread) == false && isKillEtcThread.Equals("1") == true)
                isKill = true;

            string pageData = this.GetRequestVal("PageData");
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.RetrieveIn("WorkID", workIDs);
            foreach (GenerWorkFlow gwf in gwfs)
            {
                msg += "@对工作(" + gwf.Title + ")处理情况如下。<br>";
                msg+=BP.WF.Dev2Interface.Node_ReturnWork(gwf.WorkID, toNodeID, toEmp, reMesage, isBackBoolen, pageData, isKill);
                msg += "<hr>";

            }
            return msg;
        }

        #endregion

    }
}
