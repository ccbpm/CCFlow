using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;

namespace CCFlow.WF.WorkOpt
{
    public partial class TransferCustomSimple : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string method = Request.QueryString["method"];
            string re = string.Empty;

            if (string.IsNullOrWhiteSpace(method))
                return;

            try
            {
                long workId;

                switch (method)
                {
                    case "findemps":
                        string nid = Request.QueryString["nodeId"];
                        string wid = Request.QueryString["workId"];

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
                        workId = long.Parse(Request.QueryString["workId"] ?? "0");
                        int nodeid = int.Parse(Request.QueryString["nodeId"] ?? "0");
                        string empNos = Request.QueryString["empNos"];
                        string empNames = HttpUtility.UrlDecode(Request.QueryString["empNames"]);
                        string plan = Request.QueryString["plan"];
                        int step = int.Parse(Request.QueryString["step"]);
                        TransferCustom tfc = null;

                        if (workId == 0)
                        {
                            re = ReturnJson(false, "workid参数不正确", false);
                        }
                        else
                        {
                            tfc = new TransferCustom();
                            tfc.MyPK = nodeid + "_" + workId;

                            if (string.IsNullOrWhiteSpace(empNos))
                            {
                                tfc.Delete();
                            }
                            else
                            {
                                tfc.WorkID = workId;
                                tfc.FK_Node = nodeid;
                                tfc.Worker = empNos;
                                tfc.WorkerName = empNames;
                                tfc.PlanDT = plan;
                                tfc.TodolistModel = 0;
                                tfc.Idx = step;
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