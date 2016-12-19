using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using System.Text;
using BP.WF.Template;
using BP.Web;
using BP.WF;
using BP.En;

namespace CCFlow.AppDemoLigerUI.Base
{
    public partial class DataService : BasePage
    {
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (BP.Web.WebUser.No == null)
                return;

            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (!string.IsNullOrEmpty(Request["method"]))
                method = Request["method"].ToString();

            switch (method)
            {
                case "startflow"://启动项
                    s_responsetext = GetStartFlowEUI();
                    break;
                case "startflowTree":
                    s_responsetext = GetStartFlowTreeEUI();
                    break;
                case "getempworks"://获取待办
                    s_responsetext = GetEmpWorksEUI();
                    break;
                case "getccflowlist"://获取抄送
                    s_responsetext = GetCCList();
                    break;
                case "monthplancollect"://月计划汇总
                    s_responsetext = MonthPlanCollect();
                    break;
                case "workflowmanage"://业务流程操作
                    s_responsetext = WorkFlowManage();
                    break;
                case "createmonthplan"://新建月计划
                    s_responsetext = MonthPlan_Create();
                    break;
                case "gethunguplist"://获取挂起流程
                    s_responsetext = GetHungUpList();
                    break;
                case "Running"://获取在途
                    s_responsetext = GetRunning();
                    break;
                case "unsend"://撤销发送
                    s_responsetext = WorkUnSend();
                    break;
                case "flowsearch"://查询
                    s_responsetext = FlowSearchMethod();
                    break;
                case "getemps"://获取通讯录
                    s_responsetext = GetEmpsAndEmp();// Getemps();
                    break;
                case "gettask": //取回审批
                    s_responsetext = Gettask();
                    break;
                case "keySearch"://关键字 查询 
                    s_responsetext = KeySearch();
                    break;
                case "getconfigparm"://获取配置文件参数
                    s_responsetext = GetConfigParm();
                    break;
                case "getempworkcounts"://获取待办、抄送、挂起数量
                    s_responsetext = GetEmpWorkCounts();
                    break;
                case "historystartflow"://获取历史发起
                    s_responsetext = GetHistoryStartFlowEUI();
                    break;
                case "popAlert"://弹出窗口
                    s_responsetext = PopAlert();
                    break;
                case "upMsgSta"://改变数据 状态
                    s_responsetext = UpdateMsgSta();
                    break;
                case "getDetailSms"://详细信息
                    s_responsetext = GetDetailSms();
                    break;
                case "getmenu"://获取菜单
                    s_responsetext = GetMenu();
                    break;
                case "getstoryHistory"://获取历史发起流程
                    s_responsetext = GetStoryHistory();
                    break;
                case "treeData"://流程树
                    s_responsetext = GetTreeData();
                    break;
                case "createemptycase"://创建空流程
                    s_responsetext = CreateEmptyCase();
                    break;
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";

            //组装ajax字符串格式,返回调用客户端
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();


        }
        /// <summary>
        /// 创建空流程
        /// </summary>
        /// <returns></returns>
        private string CreateEmptyCase()
        {
            string flowId = getUTF8ToString("flowId");
            string title = getUTF8ToString("title");
            string Url = "addform";
            int nodeId = int.Parse(flowId + "01");
            Node wfNode = new Node(nodeId);
            if (wfNode.HisFormType == NodeFormType.SheetTree)
            {
                if (title == "")
                    return "noform";
                long workID = BP.WF.Dev2Interface.Node_CreateStartNodeWork(flowId, null, null, WebUser.No, title);
                Url = "../../FlowFormTree/Default.aspx?WorkID=" + workID + "&FK_Flow=" + flowId + "&FK_Node=" + flowId + "01&UserNo=" + WebUser.No + "&FID=0&SID=" + WebUser.SID;
            }
            return Url;
        }
        /// <summary>
        /// 得到流程树 数据
        /// </summary>
        /// <returns></returns>
        public string GetTreeData()
        {
            //加载流程权限
            if (BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
            {
                StringBuilder sbContent = new StringBuilder("");
                string sqlSort = "SELECT No,Name,ParentNo,MenuType,Flag FROM V_GPM_EmpMenu WHERE FK_Emp='" + BP.Web.WebUser.No + "' and FK_App = '" + BP.Sys.SystemConfig.SysNo + "'";
                DataTable dtSort = BP.DA.DBAccess.RunSQLReturnTable(sqlSort);
                Flows fls = BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfEntities(BP.Web.WebUser.No);
                if (dtSort != null)
                {
                    int iCount = 0;
                    sbContent.Append("[");
                    foreach (DataRow dr in dtSort.Rows)
                    {
                        iCount++;
                        sbContent.Append("{");
                        sbContent.AppendFormat("No:\"{0}\",", dr["No"].ToString());
                        sbContent.AppendFormat("Name:\"{0}\",", dr["Name"].ToString());
                        sbContent.AppendFormat("ParentNo:\"{0}\",", dr["ParentNo"].ToString());
                        sbContent.AppendFormat("MenuType:\"{0}\",", dr["MenuType"].ToString());
                        sbContent.AppendFormat("Flag:\"{0}\",", dr["Flag"].ToString());
                        string fk_no = dr["Flag"].ToString().Replace("Flow", "");
                        if (fls.Contains(fk_no) == false)  //判断用户是否具有发起的权限
                        {
                            sbContent.Append("IsStart:\"0\"");
                        }
                        else
                        {
                            sbContent.Append("IsStart:\"1\"");
                        }
                        if (iCount == dtSort.Rows.Count)
                        {
                            sbContent.Append("}");
                        }
                        else
                        {
                            sbContent.Append("},");
                        }

                    }
                    sbContent.Append("]");
                }
                return sbContent.ToString();
            }
            return "";
        }
        /// <summary>
        /// 获取历史流程信息
        /// </summary>
        public string GetStoryHistory()
        {
            string fk_flow = "";
            if (!string.IsNullOrEmpty(Request["FK_Flow"]))
            {
                fk_flow = Request["FK_Flow"].ToString();
            }

            Flow fl = new Flow(fk_flow);
            string sql = "";

            sql = "SELECT " + fl.HistoryFields + ",OID,FID FROM " + fl.PTable + " WHERE FlowStarter='" + BP.Web.WebUser.No + "' AND WFState!='" + (int)WFState.Blank + "'" + "    ORDER by OID DESC";

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            return CommonDbOperator.GetJsonFromTable(dt);
        }
        /// <summary>
        /// 获取配置参数
        /// </summary>
        /// <returns></returns>
        private string GetConfigParm()
        {
            StringBuilder returnVal = new StringBuilder();
            returnVal.Append("{config:[{");
            returnVal.Append(string.Format("IsWinOpenStartWork:'{0}',", Glo.IsWinOpenStartWork));
            returnVal.Append(string.Format("IsWinOpenEmpWorks:'{0}'", Glo.IsWinOpenEmpWorks));
            returnVal.Append("}]}");
            return returnVal.ToString();
        }

        public string GetEasyUIJson(DataTable table)
        {
            foreach (DataColumn column in table.Columns)
            {
                column.ColumnName = column.ColumnName.ToUpper();
            }
            string json = "{\"total\":" + table.Rows.Count + ",\"rows\":" + Newtonsoft.Json.JsonConvert.SerializeObject(table) + "}";

            return json;
        }
        /// <summary>
        /// 获取启动流程项
        /// </summary>
        /// <returns></returns>
        private string GetSartFlow()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfDataTable(WebUser.No);
            return CommonDbOperator.GetJsonFromTable(dt);
        }
        /// <summary>
        /// 获取待办列表
        /// </summary>
        /// <returns></returns>
        private string GetEmpWorks()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable();
            return CommonDbOperator.GetJsonFromTable(dt);
        }
        /// <summary>
        /// 获取待办列表
        /// </summary>
        /// <returns></returns>
        private string GetEmpWorksEUI()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable();
            return GetEasyUIJson(dt);
        }
        private string GetStartFlowEUI()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfDataTable(WebUser.No);
            return GetEasyUIJson(dt);
        }
        
        private string GetStartFlowTreeEUI()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_GenerCanStartFlowsTree(WebUser.No);
            foreach (DataColumn column in dt.Columns)
            {
                column.ColumnName = column.ColumnName.ToUpper();
            }
            throw new Exception("@取消了GPM.");

            //string strFlows = BP.GPM.Utility.CommonDbOperator.GetGridTreeDataString(dt, "PARENTNO", "NO", "ST0", true);
            //if (strFlows.Length > 2)
            //    strFlows = strFlows.Remove(strFlows.Length - 2, 2);
            //return strFlows;
        }
        /// <summary>
        /// 获取历史发起
        /// </summary>
        /// <returns></returns>
        private string GetHistoryStartFlowEUI()
        {
            string fk_flow = getUTF8ToString("FK_Flow");
            Flow startFlow = new Flow(fk_flow);
            string sql = "SELECT * FROM " + startFlow.PTable + " WHERE FlowStarter='" + WebUser.No + "'  and WFState not in (" + (int)WFState.Blank + "," + (int)WFState.Draft + ")";
            DataTable dt = startFlow.RunSQLReturnTable(sql);
            return GetEasyUIJson(dt);
        }
        /// <summary>
        /// 获取抄送列表
        /// </summary>
        /// <returns></returns>
        private string GetCCList()
        {
            DataTable dt = null;
            string strCCSta = getUTF8ToString("ccSta");
            //全部
            if (strCCSta == "all")
            {
                dt = BP.WF.Dev2Interface.DB_CCList(WebUser.No);
            }
            //未读
            if (strCCSta == "unread")
            {
                dt = BP.WF.Dev2Interface.DB_CCList_UnRead(WebUser.No);
            }
            //已读
            if (strCCSta == "isread")
            {
                dt = BP.WF.Dev2Interface.DB_CCList_Read(WebUser.No);
            }
            //删除
            if (strCCSta == "delete")
            {
                dt = BP.WF.Dev2Interface.DB_CCList_Delete(WebUser.No);
            }
            return CommonDbOperator.GetJsonFromTable(dt);
        }
        #region 计划管理
        /// <summary>
        /// 月计划汇总
        /// </summary>
        /// <returns></returns>
        private string MonthPlanCollect()
        {
            string sql = "SELECT a.* ,b.FK_Flow,b.FK_Node,b.FlowName,b.NodeName,b.IsRead,b.Starter,b.ADT,b.SDT,b.WorkID FROM ND22Rpt"
                        + " a , WF_EmpWorks b WHERE a.OID=B.WorkID AND b.WFState not in (7)"
                        + " AND b.FK_Emp='" + WebUser.No + "'"
                        + " ORDER BY ADT DESC";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            return CommonDbOperator.GetJsonFromTable(dt);
        }
        /// <summary>
        /// 新建月计划
        /// </summary>
        /// <returns></returns>
        private string MonthPlan_Create()
        {
            int nextNodeId = 2201;
            string strStation = "";
            BP.Port.Stations userStations = WebUser.HisStations;
            foreach (BP.Port.Station sta in userStations)
            {
                strStation += "," + sta.Name + ",";
            }
            if (strStation.Contains(",部门计划员岗,"))
            {
                nextNodeId = 2211;
            }
            if (strStation.Contains("主任"))
            {
                nextNodeId = 2203;
            }
            long workId = BP.WF.Dev2Interface.Node_CreateStartNodeWork("022", null, null, WebUser.No, WebUser.FK_DeptName + "[" + WebUser.Name + "]" + DateTime.Now.ToString("yyyy-MM") + "部门计划");
            if (nextNodeId != 2201)
                BP.WF.Dev2Interface.Node_SendWork("022", workId, nextNodeId, WebUser.No);
            return "success";
        }
        /// <summary>
        /// 业务流程操作
        /// </summary>
        /// <returns></returns>
        private string WorkFlowManage()
        {
            string doWhat = getUTF8ToString("doWhat");
            string flowIdAndWorkId = getUTF8ToString("flowIdAndWorkId");
            string[] array = flowIdAndWorkId.Split('^');
            string Msg = "";

            //执行发送
            if (doWhat == "send")
            {
                foreach (string item in array)
                {
                    string[] item_c = item.Split(',');
                    Int64 workid = Int64.Parse(item_c[1].ToString());
                    SendReturnObjs objSend = null;
                    objSend = BP.WF.Dev2Interface.Node_SendWork(item_c[0].ToString(), workid);
                    Msg += objSend.ToMsgOfHtml();
                    Msg += "<hr>";
                }
            }
            //执行删除
            if (doWhat == "delete")
            {
                foreach (string item in array)
                {
                    string[] item_c = item.Split(',');
                    Int64 workid = Int64.Parse(item_c[1].ToString());
                    string mes = BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(item_c[0].ToString(), workid, "批量删除", true);
                    Msg += mes;
                    Msg += "<hr>";
                }
            }

            return Msg;
        }
        #endregion

        /// <summary>
        /// 获取挂起流程
        /// </summary>
        /// <returns></returns>
        private string GetHungUpList()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_GenerHungUpList();
            foreach (DataColumn column in dt.Columns)
            {
                column.ColumnName = column.ColumnName.ToUpper();
            }
            return CommonDbOperator.GetJsonFromTable(dt);
        }
        /// <summary>
        /// 获取在途列表
        /// </summary>
        /// <returns></returns>
        public string GetRunning()
        {
            DataTable dt = BP.WF.Dev2Interface.DB_GenerRuning();
            //按照接受日期排序
            dt.DefaultView.Sort = "RDT DESC";
            //return CommonDbOperator.GetJsonFromTable(dt.DefaultView.ToTable());
            return GetEasyUIJson(dt.DefaultView.ToTable());
        }
        /// <summary>
        /// 撤销发送
        /// </summary>
        /// <returns></returns>
        public string WorkUnSend()
        {
            try
            {
                string FK_Flow = getUTF8ToString("FK_Flow");
                string WorkID = getUTF8ToString("WorkID");
                string str1 = BP.WF.Dev2Interface.Flow_DoUnSend(FK_Flow, Int64.Parse(WorkID));
                if (str1.Contains("不能撤消") || str1.Contains("您不能执行撤消发送"))
                    return "{message:'" + str1 + "'}";
                return "{message:'执行撤销成功，请到待办列表进行处理。'}";
            }
            catch (Exception ex)
            {
                return "{message:'执行撤消失败，失败信息" + ex.Message + "'}";
            }
        }
        /// <summary>
        /// 工作流查询
        /// </summary>
        /// <returns></returns>
        private string FlowSearchMethod()
        {
            FlowSorts fss = new FlowSorts();
            fss.RetrieveAll();
            Flows fls = new Flows();
            fls.RetrieveAll();
            StringBuilder appFlow = new StringBuilder();
            appFlow.Append("{");
            appFlow.Append("\"rows\":[");

            foreach (FlowSort fs in fss)
            {
                if (appFlow.Length == 9) { appFlow.Append("{"); } else { appFlow.Append(",{"); }
                if (fs.ParentNo == "0")
                {
                    appFlow.Append(string.Format("\"No\":\"{0}\",\"Name\":\"{1}\",\"NumOfBill\":\"{2}\",\"_parentId\":null,\"state\":\"open\",\"Element\":\"sort\"", fs.No, fs.Name, "0"));
                }
                else
                {
                    appFlow.Append(string.Format("\"No\":\"{0}\",\"Name\":\"{1}\",\"NumOfBill\":\"{2}\",\"_parentId\":\"{3}\",\"state\":\"open\",\"Element\":\"sort\"", fs.No, fs.Name, "0", fs.ParentNo));
                }
                appFlow.Append("}");
            }

            foreach (FlowSort fs in fss)
            {
                foreach (Flow fl in fls)
                {
                    if (fl.FK_FlowSort != fs.No)
                        continue;

                    if (appFlow.Length == 9) { appFlow.Append("{"); } else { appFlow.Append(",{"); }

                    appFlow.Append(string.Format("\"No\":\"{0}\",\"Name\":\"{1}\",\"NumOfBill\":\"{2}\",\"_parentId\":\"{3}\",\"Element\":\"flow\"", fl.No, fl.Name, fl.NumOfBill, fl.FK_FlowSort));
                    appFlow.Append("}");
                }
            }
            appFlow.Append("]");
            appFlow.Append(",\"total\":" + fls.Count + fss.Count + "");
            appFlow.Append("}");
            return appFlow.ToString();
        }

        /// <summary>
        /// 获取部门
        /// </summary>
        public string GetEmpsAndEmp()
        {
            StringBuilder sbJson = new StringBuilder("{Rows:[");
            string deptSql = string.Empty;
            
            if(BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore)
                deptSql = "SELECT No,Name as DeptName,Leader,ParentNo  FROM  Port_Dept";
            else
                deptSql = "SELECT No,Name as DeptName,'' AS Leader,ParentNo  FROM  Port_Dept";

            DataTable deptDT = BP.DA.DBAccess.RunSQLReturnTable(deptSql);
            if (deptDT != null)
            {
                sbJson.Append("{");
                sbJson.AppendFormat("No:\"{0}\",", string.IsNullOrEmpty(deptDT.Rows[0]["No"].ToString()) ? "" : deptDT.Rows[0]["No"].ToString());
                sbJson.AppendFormat("Name:\"{0}\",", "");
                sbJson.AppendFormat("DeptName:\"{0}\",", string.IsNullOrEmpty(deptDT.Rows[0]["DeptName"].ToString()) ? "" : deptDT.Rows[0]["DeptName"].ToString());
                sbJson.AppendFormat("DutyName:\"{0}\",", "");
                sbJson.AppendFormat("Leader:\"{0}\",", string.IsNullOrEmpty(deptDT.Rows[0]["Leader"].ToString()) ? "" : deptDT.Rows[0]["Leader"].ToString());
                sbJson.AppendFormat("Tel:\"{0}\",", "");
                sbJson.AppendFormat("Email:\"{0}\",", "");
                sbJson.AppendFormat("QianMing:\"{0}\",", "");

                //子级
                sbJson.Append("children: [");
                //人员
                int iEmp = 0;
                string strEmp = Getemps(deptDT.Rows[0]["No"].ToString());
                sbJson.Append(strEmp);
                //子级部门
                if (strEmp.Length < 2)
                {
                    iEmp = 0;
                }
                else
                {
                    iEmp = 1;
                }
                Getemps2(deptDT, deptDT.Select("ParentNo=" + deptDT.Rows[0]["No"].ToString()), sbJson, iEmp);
                sbJson.Append("]}");
            }
            sbJson.Append("]}");
            return sbJson.ToString();
        }

        /// <summary>
        /// 获取子部门
        /// </summary>
        /// <returns></returns>
        public void Getemps2(DataTable dt, DataRow[] drChilds, StringBuilder sbJson, int iEmp)
        {
            if (dt != null && drChilds != null)
            {
                for (int i = 0; i < drChilds.Length; i++)
                {
                    if (i == 0)
                    {
                        if (iEmp == 0)
                        {
                            sbJson.Append("{");
                        }
                        else
                        {
                            sbJson.Append(",{");
                        }
                    }
                    else { sbJson.Append(",{"); }
                    sbJson.AppendFormat("No:\"{0}\",", string.IsNullOrEmpty(drChilds[i]["No"].ToString()) ? "" : drChilds[i]["No"].ToString());
                    sbJson.AppendFormat("Name:\"{0}\",", "");
                    sbJson.AppendFormat("DeptName:\"{0}\",", string.IsNullOrEmpty(drChilds[i]["DeptName"].ToString()) ? "" : drChilds[i]["DeptName"].ToString());
                    sbJson.AppendFormat("DutyName:\"{0}\",", "");
                    sbJson.AppendFormat("Leader:\"{0}\",", string.IsNullOrEmpty(drChilds[i]["Leader"].ToString()) ? "" : drChilds[i]["Leader"].ToString());
                    sbJson.AppendFormat("Tel:\"{0}\",", "");
                    sbJson.AppendFormat("Email:\"{0}\",", "");
                    sbJson.AppendFormat("QianMing:\"{0}\",", "");

                    //子级
                    sbJson.Append("children: [");
                    //人员
                    int iEmp2 = 0;
                    string strEmp = Getemps(drChilds[i]["No"].ToString());
                    sbJson.Append(strEmp);
                    //子级部门
                    if (strEmp.Length < 2)
                    {
                        iEmp2 = 0;
                    }
                    else
                    {
                        iEmp2 = 1;
                    }
                    Getemps2(dt, dt.Select("ParentNo='" + drChilds[i]["No"].ToString() + "'"), sbJson, iEmp2);
                    sbJson.Append("]}");
                }
            }
        }

        /// <summary>
        /// 获取 通讯录，人员
        /// /// </summary>
        /// <returns></returns>
        public string Getemps(string DeptNo)
        {
            StringBuilder sbJson = new StringBuilder("");
            string sql = "select a.No,  a.Name"
                + @",b.Name as DeptName"
               + @",c.Name as DutyName"
               + @",a.Leader"
               + @",a.Tel"
               + @",a.Email "
               + @",'/DataUser/Siganture/'+a.No +'.jpg' as QianMing "
               + @",convert(int,b.No) as dtNo "
               + @"from Port_Emp a, Port_Dept b,port_duty c "
               + @"where a.FK_Dept=b.No and a.FK_Duty=c.No  and convert(int,b.No)=" + int.Parse(DeptNo)
               + @"order by dtNo ";

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        sbJson.Append("{");
                    }
                    else
                    {
                        sbJson.Append(",{");
                    }
                    sbJson.AppendFormat("No:\"{0}\",", string.IsNullOrEmpty(dt.Rows[i]["No"].ToString()) ? "" : dt.Rows[i]["No"].ToString());
                    sbJson.AppendFormat("Name:\"{0}\",", string.IsNullOrEmpty(dt.Rows[i]["Name"].ToString()) ? "" : dt.Rows[i]["Name"].ToString());
                    sbJson.AppendFormat("DeptName:\"{0}\",", string.IsNullOrEmpty(dt.Rows[i]["DeptName"].ToString()) ? "" : dt.Rows[i]["DeptName"].ToString());
                    sbJson.AppendFormat("DutyName:\"{0}\",", string.IsNullOrEmpty(dt.Rows[i]["DutyName"].ToString()) ? "" : dt.Rows[i]["DutyName"].ToString());
                    sbJson.AppendFormat("Leader:\"{0}\",", string.IsNullOrEmpty(dt.Rows[i]["Leader"].ToString()) ? "" : dt.Rows[i]["Leader"].ToString());
                    sbJson.AppendFormat("Tel:\"{0}\",", string.IsNullOrEmpty(dt.Rows[i]["Tel"].ToString()) ? "" : dt.Rows[i]["Tel"].ToString());
                    sbJson.AppendFormat("Email:\"{0}\",", string.IsNullOrEmpty(dt.Rows[i]["Email"].ToString()) ? "" : dt.Rows[i]["Email"].ToString());
                    sbJson.AppendFormat("QianMing:\"{0}\"", string.IsNullOrEmpty(dt.Rows[i]["QianMing"].ToString()) ? "" : dt.Rows[i]["QianMing"].ToString());

                    sbJson.Append("}");
                }
            }
            return sbJson.ToString();
        }

        /// <summary>
        /// 取回 审批
        /// </summary>
        /// <returns></returns>
        public string Gettask()
        {
            Flows fls = new Flows();
            BP.En.QueryObject qo = new BP.En.QueryObject(fls);
            qo.addOrderBy(FlowAttr.FK_FlowSort);
            qo.DoQuery();

            //将集合 转换为datatable
            DataTable dt = new DataTable("Flows");

            DataColumn dc0 = new DataColumn("No", Type.GetType("System.String"));//编号
            DataColumn dc1 = new DataColumn("FK_FlowSortText", Type.GetType("System.String")); //流程类别
            DataColumn dc2 = new DataColumn("Name", Type.GetType("System.String"));//名称
            DataColumn dc3 = new DataColumn("FlowImage", Type.GetType("System.String")); //流程图
            DataColumn dc4 = new DataColumn("Note", Type.GetType("System.String"));//描述

            dt.Columns.Add(dc0);
            dt.Columns.Add(dc1);
            dt.Columns.Add(dc2);
            dt.Columns.Add(dc3);
            dt.Columns.Add(dc4);

            foreach (Flow fl in fls)
            {
                DataRow dr = dt.NewRow();

                dr["No"] = fl.No;
                dr["FK_FlowSortText"] = fl.FK_FlowSortText;
                dr["Name"] = fl.Name;
                dr["FlowImage"] = fl.No;
                dr["Note"] = fl.Note;

                dt.Rows.Add(dr);
            }
            //将dt以json的格式 返回
            return CommonDbOperator.GetJsonFromTable(dt);
        }
        /// <summary>
        /// 关键字查询
        /// </summary>
        /// <returns></returns>
        private string KeySearch()
        {
            string queryType = getUTF8ToString("queryType");
            string content = getUTF8ToString("content");
            string ckbQueryOwner = getUTF8ToString("checkBox");
            if (queryType == "workid")
            {
                return KeySearchByWorkID(content, ckbQueryOwner);
            }
            else if (queryType == "title")
            {
                return KeySearchByTitle(content, ckbQueryOwner);
            }
            else if (queryType == "all")
            {
                return KeySearchByAll(content, ckbQueryOwner);
            }
            return "[]";
        }
        /// <summary>
        /// 关键字 查询  按工作ID查
        /// </summary>
        /// <returns></returns>
        public string KeySearchByWorkID(string content, string ck)
        {
            int workid = 0;
            string sql = "";
            try
            {
                workid = int.Parse(content);
            }
            catch
            {
                //this.Alert("您输入的不是一个WorkID" + content);
                //return;
            }
            if (ck.ToUpper() == "TRUE")
                sql = "SELECT A.*,B.Name as FlowName FROM V_FlowData a,WF_Flow b  WHERE A.FK_Flow=B.No AND A.OID=" + workid + " AND FlowEmps LIKE '%@" + WebUser.No + ",%'";
            else
                sql = "SELECT A.*,B.Name as FlowName FROM V_FlowData a,WF_Flow b  WHERE A.FK_Flow=B.No AND A.OID=" + workid;

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            return CommonDbOperator.GetJsonFromTable(dt);
        }
        /// <summary>
        /// 关键字 查询  按流程标题字段关键字查
        /// </summary>
        /// <returns></returns>
        public string KeySearchByTitle(string content, string ck)
        {
            string sql = "";
            if (ck.ToUpper() == "TRUE")
                sql = "SELECT A.*,B.Name as FlowName FROM V_FlowData a,WF_Flow b  WHERE A.FK_Flow=B.No AND a.Title LIKE '%" + content + "%' AND FlowEmps LIKE '%@" + WebUser.No + ",%'";
            else
                sql = "SELECT A.*,B.Name as FlowName FROM V_FlowData a,WF_Flow b  WHERE A.FK_Flow=B.No AND a.Title LIKE '%" + content + "%'";

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            return CommonDbOperator.GetJsonFromTable(dt);

        }
        /// <summary>
        /// 关键字 查询 全部字段关键字查
        /// </summary>
        /// <returns></returns>
        public string KeySearchByAll(string content, string ck)
        {
            return "[]";
        }
        /// <summary>
        /// 获取待办、抄送、挂起数量
        /// </summary>
        /// <returns></returns>
        private string GetEmpWorkCounts()
        {
            //注意变量的命名规则, by peng.
            return "{message:{empwork:'" + EmpWorks + "',ccnum:'" + CCNum + "',hungupnum:'" + HungUpNum + "',TaskPoolNum:'" + TaskPoolNum + "'}}";
            //  return "{message:{empwork:'" + EmpWorks + "',ccnum:'" + CCNum + "',hungupnum:'" + HungUpNum + "',TaskPoolNum:'" + TaskPoolNum + "'}}";
        }
        /// <summary>
        /// 返回待办件数量
        /// </summary>
        /// <returns></returns>
        private int EmpWorks
        {
            get
            {
                DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable();
                return dt.Rows.Count;
                //string sql = "SELECT COUNT(*) AS Num FROM WF_EmpWorks WHERE FK_Emp='" + BP.Web.WebUser.No + "' AND WFState=2 AND TaskSta=0";
                //return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
        }
        /// <summary>
        /// 返回待办件数量
        /// </summary>
        /// <returns></returns>
        private int TaskPoolNum
        {
            get
            {
                if (Glo.IsEnableTaskPool == false)
                    return 0;

                string sql = "SELECT COUNT(WorkID) AS Num FROM WF_EmpWorks WHERE FK_Emp='" + BP.Web.WebUser.No + "' AND WFState=2 AND TaskSta=1";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
        }
        /// <summary>
        /// 返回抄送件数量
        /// </summary>
        /// <returns></returns>
        private int CCNum
        {
            get
            {
                string sql = "SELECT COUNT(MyPK) AS Num FROM WF_CCList WHERE CCTo='" + BP.Web.WebUser.No + "' AND Sta=0";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
        }
        /// <summary>
        /// 返回挂起流程数量
        /// </summary>
        private int HungUpNum
        {
            get
            {
                string sql = "SELECT COUNT(MyPK) AS Num FROM WF_HungUp WHERE Rec='" + BP.Web.WebUser.No + "'";
                return BP.DA.DBAccess.RunSQLReturnValInt(sql);
            }
        }

        /// <summary>
        /// 获取历史发起
        /// </summary>
        /// <returns></returns>
        private string GetHistoryStartFlow()
        {
            string fk_flow = getUTF8ToString("FK_FLOW");
            Flow startFlow = new Flow(fk_flow);
            string sql = "SELECT * FROM " + startFlow.PTable + " WHERE FlowStarter='" + WebUser.No + "'";
            DataTable dt = startFlow.RunSQLReturnTable(sql);
            return CommonDbOperator.GetJsonFromTable(dt);
        }
        /// <summary>
        /// 弹出 窗口  2013.05.23 H
        /// </summary>
        /// <returns></returns>
        public string PopAlert()
        {
            // IsRead = 0 未读 
            string type = getUTF8ToString("type");

            DataTable dt = BP.WF.Dev2Interface.DB_GenerPopAlert(type);
            if (dt.Rows.Count >= 1)
            {
                return CommonDbOperator.GetJsonFromTable(GetNewDataTable(dt));
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// 改变数据状态 2013.05.23 H
        /// </summary> 
        /// <returns></returns>
        public string UpdateMsgSta()
        {
            string myPK = getUTF8ToString("myPK");

            DataTable dt = BP.WF.Dev2Interface.DB_GenerUpdateMsgSta(myPK);
            return CommonDbOperator.GetJsonFromTable(dt);
        }
        /// <summary>
        ///  详细 信息  2013.05.23 H
        /// </summary>
        /// <returns></returns>
        public string GetDetailSms()
        {
            string myPK = getUTF8ToString("myPK");
            string sql = "";

            sql = "SELECT * FROM Sys_SMS WHERE MyPK='" + myPK + "'";

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            return CommonDbOperator.GetJsonFromTable(dt);
        }
        /// <summary>
        /// 整合 datatable
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DataTable GetNewDataTable(DataTable dt)
        {
            DataTable Newdt = new DataTable("DT");
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                Newdt.Columns.Add(dt.Columns[i].ColumnName);
            }
            Newdt.Columns.Add("GroupBy");
            foreach (DataRow dr in dt.Rows)
            {
                DataRow Newdr = Newdt.NewRow();
                for (int a = 0; a < dt.Columns.Count; a++)
                {
                    Newdr[dt.Columns[a].ColumnName] = dr[dt.Columns[a].ColumnName].ToString();
                }
                //Newdr["GroupBy"] = GetGropuBy(dr["RDT"].ToString());  IsToday
                Newdr["GroupBy"] = JugeDay(DateTime.Parse(dr["RDT"].ToString()));
                Newdt.Rows.Add(Newdr);
            }
            return Newdt;
        }

        /// <summary>
        /// 判断选择的日期
        /// </summary>
        /// <param name="someDate"></param>
        /// <returns></returns>
        public static string JugeDay(DateTime someDate)
        {
            DateTime dt = DateTime.Now;
            int Nowhour = dt.Hour;

            TimeSpan ts = dt - someDate;
            int hours = ts.Days == 0 ? ts.Hours : ts.Days * 24 + ts.Hours;

            if (hours <= Nowhour) //今天
            {
                return "今天";
            }
            else if ((hours > Nowhour) && (hours <= (24 + Nowhour)))//昨天
            {
                return "昨天";
            }
            else if ((hours > (24 + Nowhour)) && (hours <= (24 * 2 + Nowhour)))
            {
                switch (Convert.ToInt32(someDate.DayOfWeek))
                {
                    case 1:
                        return "星期一";
                    case 2:
                        return "星期二";
                    case 3:
                        return "星期三";
                    case 4:
                        return "星期四";
                    case 5:
                        return "星期五";
                    case 6:
                        return "星期六";
                    default:
                        return "星期日";
                }

            }
            else if ((hours > (24 * 2 + Nowhour)) && (hours <= (Convert.ToInt32(dt.DayOfWeek) * 24 + Nowhour)))
            {
                return "本周之内";
            }
            else if ((hours > (Convert.ToInt32(dt.DayOfWeek) * 24 + Nowhour)) && (hours <= ((dt.Day + 7) * 24 + Nowhour)))
            {
                return "上周之内";
            }
            else if ((hours > (Convert.ToInt32(dt.DayOfWeek) * 24 + Nowhour)) && (hours <= (dt.Day * 24 + Nowhour)))
            {
                return "本月之内";
            }
            else if ((someDate.Year == dt.Year) && (someDate.Month == dt.Month - 1))
            {
                return "上月之内";
            }
            else
            {
                return "更早";
            }
        }

        /// <summary>
        /// 获取菜单 20130723 H
        /// </summary>
        /// <returns></returns>
        private string GetMenu()
        {
            //子菜单
            string sqlSort = "";
            StringBuilder sbXML = new StringBuilder();
            sbXML.Append("[");
            if (BP.Web.WebUser.No == "admin")
            {
                sqlSort = "select * from wf_flowsort ";
            }
            else
            {
                sqlSort = "select * from V_FlowSortEmp where FK_Emp ='" + BP.Web.WebUser.No + "'";
            }

            DataTable dtSort = BP.DA.DBAccess.RunSQLReturnTable(sqlSort);
            if (dtSort != null)
            {
                int iCount = 0;
                foreach (DataRow dr in dtSort.Rows)
                {
                    if (iCount > 0) sbXML.Append(",");
                    sbXML.Append("{");
                    sbXML.AppendFormat("No:\"{0}\"", dr["No"].ToString());
                    sbXML.AppendFormat(",Name:\"{0}\"", dr["Name"].ToString());
                    sbXML.AppendFormat(",ParentNo:\"{0}\"", dr["ParentNo"].ToString());
                    sbXML.Append("}");
                    string sqlChild = "select  No,Name,Fk_FlowSort as ParentNo  from wf_flow where   FK_FlowSort = '" + dr["No"].ToString() + "'";
                    DataTable dtChild = BP.DA.DBAccess.RunSQLReturnTable(sqlChild);
                    if (dtChild != null)
                    {
                        foreach (DataRow drChild in dtChild.Rows)
                        {
                            sbXML.Append(",");
                            sbXML.Append("{");
                            sbXML.AppendFormat("No:\"{0}\"", drChild["No"].ToString());
                            sbXML.AppendFormat(",Name:\"{0}\"", drChild["Name"].ToString());
                            sbXML.AppendFormat(",ParentNo:\"{0}\"", drChild["ParentNo"].ToString());
                            sbXML.Append("}");
                        }
                    }

                    iCount++;
                }
            }
            sbXML.Append("]");
            return sbXML.ToString();
        }
    }
}