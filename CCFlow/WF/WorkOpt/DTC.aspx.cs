using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.Web;
using BP.WF;
using BP.WF.Template;

namespace CCFlow.WF.WorkOpt
{
    public partial class DTC : System.Web.UI.Page
    {
        private long WorkId;

        protected void Page_Load(object sender, EventArgs e)
        {
            string method = Request.QueryString["method"];
            string re = string.Empty;
            string sql = string.Empty;

            if (string.IsNullOrWhiteSpace(method)) return;

            try
            {
                switch (method)
                {
                    case "getflows":
                        sql =
                            "SELECT f.NO,f.NAME,fs.NAME SORT FROM WF_Flow f INNER JOIN WF_FlowSort fs ON fs.No = f.FK_FlowSort ORDER BY fs.Idx";

                        re = BP.Tools.Json.ToJson(DBAccess.RunSQLReturnTable(sql)); 

                        break;
                    case "startflow":
                        string flowno = Request.QueryString["flowNo"];

                        if (string.IsNullOrWhiteSpace(flowno))
                        {
                            re = ReturnJson(false, "flowNo不能为空", false);
                        }
                        else
                        {
                            string msg = StartFlow(flowno);

                            if (!string.IsNullOrEmpty(msg))
                            {
                                re = ReturnJson(false, "发起流程失败：" + msg, false);
                            }
                            else
                            {
                                re = ReturnJson(true, WorkId.ToString(), false);
                            }
                        }

                        break;
                    case "settransfer":
                        flowno = Request.QueryString["flowNo"];
                        string wid = Request.QueryString["workId"];
                        long workId = 0;

                        if (string.IsNullOrWhiteSpace(flowno))
                        {
                            re = ReturnJson(false, "flowNo不能为空", false);
                        }
                        else if (string.IsNullOrWhiteSpace(wid) || !long.TryParse(wid, out workId))
                        {
                            re = ReturnJson(false, "workId参数格式不正确", false);
                        }
                        else
                        {
                            GenerWorkFlow gwf = new GenerWorkFlow();
                            if (gwf.Retrieve(GenerWorkFlowAttr.WorkID, workId) == 0)
                            {
                                re = ReturnJson(false, "workId参数不正确，未找互此WorkId的发起流程", false);
                            }
                            else if (gwf.FK_Flow != flowno)
                            {
                                re = ReturnJson(false, "workId参数不正确，此WorkId的流程与所选流程不匹配", false);
                            }
                            else
                            {
                                TransferCustoms tcs = new TransferCustoms(workId);
                                GenerWorkerLists gwls = new GenerWorkerLists(workId);
                                Nodes nodes = new Nodes();
                                nodes.Retrieve(NodeAttr.FK_Flow, flowno, "Step");
                                TransferCustom tc = null;
                                re = "{\"workid\":" + workId + ", \"nodes\":[";
                                string s = string.Empty;
                                DataSet ds = null;
                                DataTable dtEmp = null;
                                DataTable dtDept = null;
                                DataRow rd = null;
                                string deptName = null;
                                string sWorkers = null;

                                foreach (Node node in nodes)
                                {
                                    if (node.IsStartNode == true)
                                    {
                                        re += "{\"id\": " + node.NodeID + ", \"name\": \"" + node.Name +
                                              "\", \"empNos\":\"" +
                                              gwf.Starter + "\", \"empNames\":\"" + gwf.StarterName +
                                              "\",\"isPass\": true,\"plan\":\"\",\"rdt\":\"" + gwf.RDT +
                                              "\"},";
                                        continue;
                                    }

                                    GenerWorkerList gwl =
                                        gwls.GetEntityByKey(GenerWorkerListAttr.FK_Node, node.NodeID) as GenerWorkerList;

                                    if (gwl == null)
                                    {
                                        /* 还没有到达的节点. */
                                        tc =
                                            tcs.GetEntityByKey(GenerWorkerListAttr.FK_Node, node.NodeID) as
                                            TransferCustom;

                                        if (tc == null)
                                            tc = new TransferCustom();

                                        //ds = BP.WF.Dev2Interface.WorkOpt_AccepterDB(node.NodeID, workId);
                                        //dtEmp = ds.Tables["Port_Emp"];
                                        //dtDept = ds.Tables["Port_Dept"];
                                        //s = string.Empty;
                                        //sWorkers = "," + (tc.Worker ?? "") + ",";

                                        //foreach (DataRow r in dtEmp.Rows)
                                        //{
                                        //    if (dtEmp.Columns.Contains("DeptName"))
                                        //    {
                                        //        deptName = r["DeptName"].ToString();
                                        //    }
                                        //    else
                                        //    {
                                        //        rd = dtDept.Select(string.Format("No='{0}'", r["FK_Dept"]))[0];
                                        //        deptName = rd["Name"].ToString();
                                        //    }

                                        //    s += "{\"no\": \"" + r["No"] + "\", \"name\": \"" + r["Name"] +
                                        //        "\", \"dept\":\"" + deptName + "\"" + (sWorkers.IndexOf("," + r["No"] + ",") != -1 ? ", \"selected\": true" : "") + "},";
                                        //}

                                        re += "{\"id\": " + node.NodeID + ", \"name\": \"" + node.Name +
                                              "\", \"empNos\":\"" +
                                              tc.Worker + "\", \"empNames\":\"" + tc.WorkerName +
                                              "\",\"isPass\": false,\"plan\":\"" +
                                              tc.PlanDT + "\",\"rdt\":\"\"},";
                                        continue;
                                    }

                                    //已经走完节点
                                    re += "{\"id\": " + node.NodeID + ", \"name\": \"" + node.Name + "\", \"empNos\":\"" +
                                          gwl.FK_Emp + "\", \"empNames\":\"" + gwl.FK_EmpText +
                                          "\",\"isPass\": true,\"plan\":\"" + gwl.SDT + "\",\"rdt\":\"" +
                                          gwl.RDT + "\"},";
                                }

                                re = re.TrimEnd(',') + "]}";
                                re = ReturnJson(true, re, true);
                            }
                        }
                        break;
                    case "findemps":
                        string nid = Request.QueryString["nodeId"];
                        wid = Request.QueryString["workId"];

                        if (string.IsNullOrWhiteSpace(nid))
                        {
                            re = "[]";// ReturnJson(false, "nodeid不能为空", false);
                        }
                        else if (string.IsNullOrWhiteSpace(wid) || !long.TryParse(wid, out workId))
                        {
                            re = "[]";// ReturnJson(false, "workId参数格式不正确", false);
                        }
                        else
                        {
                            GenerWorkFlow gwf = new GenerWorkFlow();
                            Node node = null;
                            if (gwf.Retrieve(GenerWorkFlowAttr.WorkID, workId) == 0)
                            {
                                re = "[]";// ReturnJson(false, "workId参数不正确，未找互此WorkId的发起流程", false);
                            }
                            else
                            {
                                node = new Node(int.Parse(nid));
                                re = "[";
                                DataSet ds = null;
                                DataTable dtEmp = null;
                                DataTable dtDept = null;
                                DataRow rd = null;
                                string deptName = null;

                                ds = BP.WF.Dev2Interface.WorkOpt_AccepterDB(node.NodeID, workId);
                                dtEmp = ds.Tables["Port_Emp"];
                                dtDept = ds.Tables["Port_Dept"];

                                foreach (DataRow r in dtEmp.Rows)
                                {
                                    if (dtEmp.Columns.Contains("DeptName"))
                                    {
                                        deptName = r["DeptName"].ToString();
                                    }
                                    else
                                    {
                                        rd = dtDept.Select(string.Format("No='{0}'", r["FK_Dept"]))[0];
                                        deptName = rd["Name"].ToString();
                                    }

                                    re += "{\"no\": \"" + r["No"] + "\", \"name\": \"" + r["Name"] +
                                        "\", \"dept\":\"" + deptName + "\"},";
                                }
                            }

                            re = re.TrimEnd(',') + "]";
                        }
                        break;
                    case "savecfg":
                        workId = long.Parse(Request.QueryString["workid"] ?? "0");
                        string[] data = HttpUtility.UrlDecode(Request.QueryString["data"] ?? "").Split(new[] { '|' },
                                                                                                       StringSplitOptions
                                                                                                           .
                                                                                                           RemoveEmptyEntries);
                        string[] ns = null;
                        int nodeid = 0;
                        TransferCustom tfc = null;
                        int i = 0;

                        if (workId == 0)
                        {
                            re = ReturnJson(false, "workid参数不正确", false);
                        }
                        else
                        {
                            //删除之前保存的
                            TransferCustoms tfcs = new TransferCustoms(workId);
                            tfcs.Delete();

                            foreach (string d in data)
                            {
                                ns = d.Split('_');
                                if (ns.Length != 4 || !int.TryParse(ns[0], out nodeid) ||
                                    ns[1].Length == 0)
                                    continue;

                                tfc = new TransferCustom();
                                tfc.MyPK = nodeid + "_" + workId;
                                tfc.WorkID = workId;
                                tfc.FK_Node = nodeid;
                                tfc.Worker = ns[1];
                                tfc.WorkerName = ns[2];
                                tfc.PlanDT = ns[3];
                                tfc.TodolistModel = 0;
                                tfc.Idx = ++i;
                                tfc.Save();
                            }

                            re = ReturnJson(true, "保存成功！", false);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                re = ReturnJson(false, ex.Message, false);
            }

            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(re);
            Response.End();
        }

        /// <summary>
        /// 发起一个流程，此流程的开始节点要配置至少一个可发起人员
        /// </summary>
        /// <param name="flowNo">流程编号</param>
        /// <returns></returns>
        public string StartFlow(string flowNo)
        {
            string msg = string.Empty;
            SendReturnObjs sres = null;

            //1.获取流程发起人列表
            string sql = Dev2Interface.GetFlowStarters(flowNo);
            DataTable dtFlowStarters = BP.DA.DBAccess.RunSQLReturnTable(sql);

            if (dtFlowStarters == null || dtFlowStarters.Rows.Count == 0)
            {
                msg = string.Format("流程{0}开始节点未设置处理人。", flowNo);
                BP.Sys.Glo.WriteLineError(msg);
                return msg;
            }

            string flowStarter = dtFlowStarters.Rows[0]["No"].ToString();

            //记录当前登录人账号，在处理完当前节点工作后，再切换回当前登录人
            string currEmp = BP.Web.WebUser.No;

            if (currEmp != flowStarter)
            {
                BP.Web.WebUser.Exit();
                Dev2Interface.Port_Login(flowStarter);
            }

            Int64 workID = 0;
            try
            {
                //2.发起流程
                workID = Dev2Interface.Node_CreateStartNodeWork(flowNo, null, null, flowStarter);
                //Dev2Interface.Node_CreateBlankWork("001");

                //3.发送
                sres = Dev2Interface.Node_SendWork(flowNo, workID, null, null, 0, null, WebUser.No, WebUser.Name, WebUser.FK_Dept, WebUser.FK_DeptName, null);

                if (currEmp != flowStarter)
                {
                    BP.Web.WebUser.Exit();

                    if (!string.IsNullOrWhiteSpace(currEmp))
                        Dev2Interface.Port_Login(currEmp);
                }

                WorkId = sres.VarWorkID;
                return null;
            }
            catch (Exception ex)
            {
                msg = string.Format("自动发起流程编号为{0}的工作[WorkID={1}]失败，错误原因：{2}", flowNo, workID, ex.Message);
                BP.Sys.Glo.WriteLineError(msg);
                return msg;
            }
        }

        /// <summary>
        /// 生成返给前台页面的JSON字符串信息
        /// </summary>
        /// <param name="success">是否操作成功</param>
        /// <param name="msg">消息</param>
        /// <param name="haveMsgJsoned">msg是否已经JSON化</param>
        /// <returns></returns>
        private string ReturnJson(bool success, string msg, bool haveMsgJsoned)
        {
            string kh = haveMsgJsoned ? "" : "\"";
            return "{\"success\":" + success.ToString().ToLower() + ",\"msg\":" + kh + (haveMsgJsoned ? msg : msg.Replace("\"", "'")) +
                   kh + "}";
        }
    }
}