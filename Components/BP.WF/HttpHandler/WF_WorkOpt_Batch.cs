using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Web;
using BP.DA;
using BP.Difference;
using BP.En;
using BP.Sys;
using BP.Web;
using BP.WF.Template;
using Newtonsoft.Json.Linq;

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

            //获取节点信息
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            Flow fl = nd.HisFlow;
            ds.Tables.Add(nd.ToDataTableField("WF_Node"));

            string sql = "";

            if (nd.IsSubThread==true)
            {
                sql = "SELECT a.*, b.Starter,b.StarterName,b.ADT,b.WorkID FROM " + fl.PTable
                          + " a , WF_EmpWorks b WHERE a.OID=B.FID AND b.WFState Not IN (7) AND b.FK_Node=" + nd.NodeID
                          + " AND b.FK_Emp='" + WebUser.No + "'";
            }
            else
            {
                sql = "SELECT a.*, b.Starter,b.StarterName,b.ADT,b.WorkID FROM " + fl.PTable
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

        public string BatchToolBar_Init()
        {
            DataSet ds = new DataSet();
            Node nd = new Node(this.FK_Node);
            //获取按钮权限
            BtnLab btnLab = new BtnLab(this.FK_Node);
            DataTable dt = new DataTable("ToolBar");
            dt.Columns.Add("No");
            dt.Columns.Add("Name");
            dt.Columns.Add("Oper");
            dt.Columns.Add("Role", typeof(int));
            dt.Columns.Add("Icon");
            DataRow dr = dt.NewRow();
            //发送
            dr["No"] = "Send";
            dr["Name"] = btnLab.SendLab;
            dr["Oper"] ="";
            dt.Rows.Add(dr);
            if (btnLab.ReturnEnable)
            {
                /*退回*/
                dr = dt.NewRow();
                dr["No"] = "Return";
                dr["Name"] = btnLab.ReturnLab;
                dr["Oper"] = "";
                dt.Rows.Add(dr);
            }
            if (btnLab.DeleteEnable != 0)
            {
                dr = dt.NewRow();
                dr["No"] = "DeleteFlow";
                dr["Name"] = btnLab.DeleteLab;
                dr["Oper"] = "";
                dt.Rows.Add(dr);

            }

            if (btnLab.EndFlowEnable && nd.IsStartNode == false)
            {
                dr = dt.NewRow();
                dr["No"] = "EndFlow";
                dr["Name"] = btnLab.EndFlowLab;
                dr["Oper"] = "";
                dt.Rows.Add(dr);

            }
           /* int checkModel = nd.GetParaInt("BatchCheckNoteModel");
            if (nd.FrmWorkCheckSta == FrmWorkCheckSta.Enable && checkModel == 0) {
                *//*增加审核意见*//*
                dr = dt.NewRow();
                dr["No"] = "WorkCheckMsg";
                dr["Name"] = "填写审核意见";
                dr["Oper"] = "";
                dt.Rows.Add(dr);
            }*/
            ds.Tables.Add(dt);
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.TodoEmps = WebUser.No + ",";
            DataTable dtNodes = BP.WF.Dev2Interface.Node_GenerDTOfToNodes(gwf, nd);
            if (dtNodes != null)
                ds.Tables.Add(dtNodes);

            ds.Tables.Add(nd.ToDataTableField("WF_Node"));
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

            string editFiles = nd.GetParaString("EditFields");
            //多表单的签批组件的修改 
            FrmNode frmNode = null;
            if (nd.HisFormType == NodeFormType.SheetTree || nd.HisFormType == NodeFormType.RefOneFrmTree)
            {
                FrmNodes frmNodes = new FrmNodes();
                QueryObject qo = new QueryObject(frmNodes);
                qo.AddWhere(FrmNodeAttr.FK_Node, nd.NodeID);
                qo.addAnd();
                qo.AddWhere(FrmNodeAttr.IsEnableFWC, 1);
                qo.addAnd();
                qo.AddWhereIsNotNull(NodeWorkCheckAttr.CheckField);
                qo.DoQuery();
                if (frmNodes.Count != 0)
                    frmNode = frmNodes[0] as FrmNode;
            }
            List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();
            HttpContext ctx = HttpContextHelper.Current;
            // 设置最大线程
            ThreadPool.SetMaxThreads(16, 16);
            // 设置最小线程
            ThreadPool.SetMinThreads(8, 8);
            int count = 0;
            int successCout = 0;
            int errorCount = 0;
            string successMsg = "";
            string errorMsg = "";
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr[0].ToString());
                string cb = this.GetValFromFrmByKey("CB_" + workid, "0");
                if (cb.Equals("on") == false)
                    continue;
                count++;
                //实例化同步工具
                ManualResetEvent mre = new ManualResetEvent(false);
                manualEvents.Add(mre);
                //将任务放入线程池中，让线程池中的线程执行该任务
                ThreadPool.QueueUserWorkItem(o =>
                {
                    HttpContext.Current = ctx;
                    Thread.Sleep(1000);
                    try
                    {
                        //是否启用了审核组件？
                        if (nd.FrmWorkCheckSta == FrmWorkCheckSta.Enable)
                        {
                            //绑定多表单，获取启用审核组件的表单
                            if (frmNode != null)
                            {
                                GEEntity en = new GEEntity(frmNode.FK_Frm, workid);
                                en.SetValByKey(frmNode.CheckField, en.GetValStrByKey(frmNode.CheckField) + "," + nd.NodeID);
                                en.Update();
                            }
                            else
                            {
                                NodeWorkCheck workCheck = new NodeWorkCheck(nd.NodeID);
                                if (DataType.IsNullOrEmpty(workCheck.CheckField) == false)
                                {
                                    GEEntity en = new GEEntity(nd.NodeFrmID, workid);
                                    en.SetValByKey(workCheck.CheckField, en.GetValStrByKey(workCheck.CheckField) + "," + nd.NodeID);
                                    en.Update();
                                }
                            }

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
                                BP.WF.Dev2Interface.Node_WriteWorkCheck( workid, checkNote, null, null);
                        }
                        //设置字段的默认值.
                        Work wk = nd.HisWork;
                        wk.OID = workid;
                        wk.Retrieve();
                        wk.ResetDefaultVal();
                        if (DataType.IsNullOrEmpty(editFiles) == false)
                        {
                            string[] files = editFiles.Split(',');
                            string val = "";
                            foreach (string key in files)
                            {
                                if (DataType.IsNullOrEmpty(key) == true)
                                    continue;
                                val = this.GetRequestVal("TB_" + workid + "_" + key);
                                wk.SetValByKey(key, val);
                            }
                        }

                        wk.Update();
                        BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(nd.FK_Flow, workid, toNode, toEmps);
                        successMsg += "@对工作(" + dr["Title"] + ")处理情况如下";
                        successMsg += objs.ToMsgOfHtml();
                        successMsg += "<br/>";
                        successCout++;
                    }
                    catch(Exception ex)
                    {
                        errorMsg += "@工作(" + dr["Title"] + ")发送出现错误:";
                        errorMsg += ex.Message;
                        errorMsg += "<br/>";
                        errorCount++;
                    }
                    mre.Set();//告诉主线程子线程执行完了,如果不给ManualResetEvent实例调用这个方法,主线程会一直等待子线程调用ManualResetEvent实例的Set方法
                });
            }
            WaitHandle.WaitAll(manualEvents.ToArray());
            if (successCout != 0)
                msg += "@发送成功" + successCout + "条";
            if (errorCount != 0)
                msg += ",发送失败。" + errorCount + "条";
            if (successCout != 0)
                msg += "@发送成功信息如下:" + successMsg;
            if (errorCount != 0)
                msg += "@发送失败信息如下:" + errorMsg;

            if (msg == "")
                msg = "没有选择需要处理的工作";

            return msg;
        }

        public string WorkCheckModelVue_Send()
        {
            //审核批量发送.
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            string workIds = this.GetRequestVal("WorkIDs");
            string checkMsg = this.GetRequestVal("CheckMsg");
            string selectItems = this.GetRequestVal("SelectItems");
            JArray json = null;
            if (DataType.IsNullOrEmpty(selectItems) == false)
            {
               json = JArray.Parse(selectItems);
            }
            //获取要批处理数据
            string sql = string.Format("SELECT WorkID, FID,Title FROM WF_EmpWorks WHERE FK_Emp='{0}' and FK_Node='{1}' and WorkID IN('"+ workIds.Replace(",","','")+"')", WebUser.No, this.FK_Node);
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            int idx = -1;
            string msg = "";

            //判断是否有传递来的参数.
            int toNode = this.GetRequestValInt("ToNode");
            string toEmps = this.GetRequestVal("ToEmps");

            string editFiles = nd.GetParaString("EditFields");
            //多表单的签批组件的修改 
            FrmNode frmNode = null;
            if (nd.HisFormType == NodeFormType.SheetTree || nd.HisFormType == NodeFormType.RefOneFrmTree)
            {
                FrmNodes frmNodes = new FrmNodes();
                QueryObject qo = new QueryObject(frmNodes);
                qo.AddWhere(FrmNodeAttr.FK_Node, nd.NodeID);
                qo.addAnd();
                qo.AddWhere(FrmNodeAttr.IsEnableFWC, 1);
                qo.addAnd();
                qo.AddWhereIsNotNull(NodeWorkCheckAttr.CheckField);
                qo.DoQuery();
                if (frmNodes.Count != 0)
                    frmNode = frmNodes[0] as FrmNode;
            }
            List<ManualResetEvent> manualEvents = new List<ManualResetEvent>();
            HttpContext ctx = HttpContextHelper.Current;
            // 设置最大线程
            ThreadPool.SetMaxThreads(16, 16);
            // 设置最小线程
            ThreadPool.SetMinThreads(8, 8);
            int count = 0;
            int successCout = 0;
            int errorCount = 0;
            string successMsg = "";
            string errorMsg = "";
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr[0].ToString());
                JObject obj = GetObject(workid, json);
                //实例化同步工具
                ManualResetEvent mre = new ManualResetEvent(false);
                manualEvents.Add(mre);
                //将任务放入线程池中，让线程池中的线程执行该任务
                ThreadPool.QueueUserWorkItem(o =>
                {
                    HttpContext.Current = ctx;
                    try
                    {
                        //是否启用了审核组件？
                        if (nd.FrmWorkCheckSta == FrmWorkCheckSta.Enable)
                        {
                            //绑定多表单，获取启用审核组件的表单
                            if (frmNode != null)
                            {
                                GEEntity en = new GEEntity(frmNode.FK_Frm, workid);
                                en.SetValByKey(frmNode.CheckField, en.GetValStrByKey(frmNode.CheckField) + "," + nd.NodeID);
                                en.Update();
                            }
                            else
                            {
                                NodeWorkCheck workCheck = new NodeWorkCheck(nd.NodeID);
                                if (DataType.IsNullOrEmpty(workCheck.CheckField) == false)
                                {
                                    GEEntity en = new GEEntity(nd.NodeFrmID, workid);
                                    en.SetValByKey(workCheck.CheckField, en.GetValStrByKey(workCheck.CheckField) + "," + nd.NodeID);
                                    en.Update();
                                }
                            }

                            //获取审核意见的值
                            string checkNote = "";

                            //选择的多条记录一个意见框.
                            var model = nd.GetParaInt("BatchCheckNoteModel", 0);

                            //多条记录一个意见.
                            if (model == 0)
                                checkNote = checkMsg;

                            //每条记录都有自己的意见.
                            if (model == 1 && obj != null)
                            {
                                JToken result = obj.GetValue("CheckMsg");
                                checkNote = result == null ? "" : result.ToString();

                            }
                            if (model == 2)
                                checkNote = " ";
                            //写入审核意见.
                            if (DataType.IsNullOrEmpty(checkNote) == false)
                                BP.WF.Dev2Interface.Node_WriteWorkCheck( workid, checkNote, null, null);
                        }
                        //设置字段的默认值.
                        Work wk = nd.HisWork;
                        wk.OID = workid;
                        wk.Retrieve();
                        wk.ResetDefaultVal();
                        if (DataType.IsNullOrEmpty(editFiles) == false)
                        {
                            string[] files = editFiles.Split(',');
                            string val = "";
                            foreach (string key in files)
                            {
                                if (DataType.IsNullOrEmpty(key) == true)
                                    continue;
                                if (obj != null)
                                {
                                    JToken result = obj.GetValue(key);
                                    wk.SetValByKey(key, result == null ? "" : result.ToString());
                                }
                            }
                        }

                        wk.Update();

                        //执行工作发送.
                        BP.WF.SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(nd.FK_Flow, workid, toNode, toEmps);
                        successMsg += "@对工作(" + dr["Title"] + ")处理情况如下";
                        successMsg += objs.ToMsgOfHtml();
                        successMsg += "<br/>";
                        successCout++;
                    }
                    catch(Exception ex)
                    {
                        errorMsg += "@工作(" + dr["Title"] + ")发送出现错误:";
                        errorMsg += ex.Message;
                        errorMsg += "<br/>";
                        errorCount++;

                    }
                    
                    mre.Set();//告诉主线程子线程执行完了,如果不给ManualResetEvent实例调用这个方法,主线程会一直等待子线程调用ManualResetEvent实例的Set方法
                }); 
            }
            WaitHandle.WaitAll(manualEvents.ToArray());
            if (successCout != 0)
                msg += "@发送成功信息如下:<br/>" + successMsg;
            if (errorCount != 0)
                msg += "@发送失败信息如下:<br/>" + errorMsg;

            if (msg == "")
                return "没有选择需要处理的工作";

            return "本次批量发送成功" + successCout + "件，失败" + errorCount + "件。<br>" + msg;
        }

        private JObject GetObject(Int64 compareWorkid,JArray json)
        {
            if (json == null)
                return null;
            foreach (JObject item in json)
            {
                JToken result = item.GetValue("WorkID");
                Int64 workid = 0;
                if (result != null)
                    workid = Int64.Parse(result.ToString());
                if (workid == compareWorkid)
                    return item;
            }
            return null;
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

            if (nd.IsSubThread == true)
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
            //BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
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

        }
        public string Batch_StopFlow()
        {
            string workIDs = this.GetRequestVal("WorkIDs");
            if (DataType.IsNullOrEmpty(workIDs) == true)
                return "err@没有选择需要处理的工作";
            string msg = "";
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.RetrieveIn("WorkID", workIDs);
            foreach (GenerWorkFlow gwf in gwfs)
            {
                msg += "@对工作(" + gwf.Title + ")处理情况如下。<br>";
                string mes = BP.WF.Dev2Interface.Flow_DoFlowOver(gwf.WorkID, "批量结束流程");
                msg += mes;
                msg += "<hr>";
            }
            return "批量结束成功" + msg;

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
