using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using BP.DA;
using BP.WF;
using BP.Web;

namespace CCFlow.AppDemoLigerUI
{
    public partial class ProjectInfo : System.Web.UI.Page
    {
        /// <summary>
        /// 获取传入参数
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (string.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();
            switch (method)
            {
                case "getempworks"://获取待办列表
                    s_responsetext = GetEmpWorkData();
                    break;
                case "getruningflowdata"://获取在途流程数据
                    s_responsetext = GetRuningFlowData();
                    break;
                case "getccflowdata"://获取抄送流程数据
                    s_responsetext = GetCCFlowData();
                    break;
                case "gethunupflowdata"://获取挂起流程数据
                    s_responsetext = GetHunupFlowData();
                    break;
                case "iscanstartthisflow"://判断是否可以发起流程
                    s_responsetext = IsCanStartThisFlow();
                    break;
                case "loadoverflowdata"://办结数据
                    s_responsetext = LoadOverFlowData();
                    break;
                case "printprojectword"://导出Word
                    PrintMonthPlanWord();
                    break;
                case "returnflowdata"://回滚流程数据
                    s_responsetext = ReturnFlowData();
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
        /// 回滚流程数据
        /// </summary>
        /// <returns></returns>
        private string ReturnFlowData()
        {
            try
            {
                string FK_Flow = getUTF8ToString("FK_Flow");
                string WorkID = getUTF8ToString("WorkID");
                string remark = getUTF8ToString("Remark");
                long lWorkID = string.IsNullOrEmpty(WorkID) ? 0 : long.Parse(WorkID);

                BP.WF.Template.FlowSheet flowSheet = new BP.WF.Template.FlowSheet(FK_Flow);
                string returnInfo = flowSheet.DoRebackFlowData(lWorkID, int.Parse(FK_Flow + "01"), remark);
                returnInfo = returnInfo.Replace("@", "<br/>");
                return returnInfo;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 获取办结项目信息
        /// </summary>
        /// <returns></returns>
        private string LoadOverFlowData()
        {
            string FK_Flow = getUTF8ToString("FK_Flow");
            string WFState = getUTF8ToString("WFState");
            string ProjNo = getUTF8ToString("ProjNo");
            string keyWords = getUTF8ToString("keyWords");

            string sql = "select a.*,b.Name StarterName,c.Name FK_DeptText,d.Name FlowName,(select Lab from Sys_Enum where EnumKey='WFState' and IntKey=a.WFState) WFStateText "
                        + " from V_FlowData a,Port_Emp b,Port_Dept c,WF_Flow d"
                        + " where a.FlowStarter=b.No and a.FK_Dept=c.No and a.FK_Flow=d.No ";
            //项目编号
            if (!string.IsNullOrEmpty(ProjNo))
            {
                sql += " and a.ProjNo='" + ProjNo + "'";
            }
            else if (!string.IsNullOrEmpty(WebUser.No))
            {
                //如果不使用项目编号就是用部门级别来控制
                string depts = GetPersonDeptAndChild();
                sql += " and (a.FK_Dept in (" + depts + ") OR a.FlowEmps like '%" + WebUser.No + "%')";
            }
            else
            {
                //非法查询
                sql += " and 1=2";
            }
            //流程编号
            if (!string.IsNullOrEmpty(FK_Flow) && FK_Flow != "0")
            {
                sql += " and a.FK_Flow='" + FK_Flow + "'";
            }
            //流程状态
            if (!string.IsNullOrEmpty(WFState) && WFState != "0")
            {
                sql += " and a.WFState=" + WFState;
            }
            //关键字
            if (!string.IsNullOrEmpty(keyWords))
            {
                sql += " and (a.Title like '%" + keyWords + "%' or a.FlowEmps like '%" + keyWords + "%')";
            }
            sql += " order by a.FlowStartRDT DESC";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            return BP.DA.DataTableConvertJson.DataTable2Json(dt, dt.Rows.Count);
        }

        /// <summary>
        /// 获取本部门与子级部门
        /// </summary>
        /// <returns></returns>
        private string GetPersonDeptAndChild()
        {
            string strDepts = "'" + WebUser.FK_Dept + "'";
            GetChildDept(WebUser.FK_Dept, ref strDepts);
            return strDepts;
        }

        /// <summary>
        /// 增加子级
        /// </summary>
        /// <param name="parentNo"></param>
        /// <param name="depts"></param>
        private void GetChildDept(string parentNo, ref string strDepts)
        {
            BP.Port.Depts depts = new BP.Port.Depts(parentNo);
            if (depts != null && depts.Count > 0)
            {
                foreach (BP.Port.Dept item in depts)
                {
                    strDepts += ",'" + item.No + "'";
                    GetChildDept(item.No, ref strDepts);
                }
            }
        }

        /// <summary>
        /// 判断是否有权限发起
        /// </summary>
        /// <returns></returns>
        private string IsCanStartThisFlow()
        {
            string FK_Flow = getUTF8ToString("FK_Flow");
            Flow flow = new Flow();
            flow.RetrieveByAttr("No", FK_Flow);

            string reVal = "{FlowName:'" + flow.Name + "',IsCan:'false'}";
            bool isRight = BP.WF.Dev2Interface.Flow_IsCanStartThisFlow(FK_Flow, WebUser.No);
            if (isRight)
                reVal = "{FlowName:'" + flow.Name + "',IsCan:'true'}";
            return reVal;
        }

        /// <summary>
        /// 获取待办列表,传入流程编号和项目编号
        /// </summary>
        /// <returns></returns>
        private string GetEmpWorkData()
        {
            try
            {
                Paras ps = new Paras();
                string dbstr = BP.Sys.SystemConfig.AppCenterDBVarStr;
                string wfSql = "  WFState=" + (int)WFState.Askfor + " OR WFState=" + (int)WFState.Runing + "  OR WFState=" + (int)WFState.AskForReplay + " OR WFState=" + (int)WFState.Shift + " OR WFState=" + (int)WFState.ReturnSta + " OR WFState=" + (int)WFState.Fix;
                string FK_Flow = getUTF8ToString("FK_Flow");
                string ProjNo = getUTF8ToString("ProjNo");

                ps.SQL = "SELECT * FROM WF_EmpWorks WHERE (" + wfSql + ") AND FK_Emp=" + dbstr + "FK_Emp and FK_Flow=" + dbstr
                    + "FK_Flow and (WorkID in (select OID from V_FlowData where ProjNo =" + dbstr + "ProjNo) OR FID in (select OID from V_FlowData where ProjNo =" + dbstr + "ProjNo)) ORDER BY ADT DESC";
                ps.Add("FK_Emp", BP.Web.WebUser.No);
                ps.Add("FK_Flow", FK_Flow);
                ps.Add("ProjNo", ProjNo);

                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(ps);
                return BP.DA.DataTableConvertJson.DataTable2Json(dt, dt.Rows.Count);
            }
            catch
            {
                return "[{}]";
            }
        }
        /// <summary>
        /// 获取在途数据，传入流程编号和项目编号
        /// </summary>
        /// <returns></returns>
        private string GetRuningFlowData()
        {
            try
            {
                string FK_Flow = getUTF8ToString("FK_Flow");
                string ProjNo = getUTF8ToString("ProjNo");

                string sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + FK_Flow
                    + "'  AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No + "' AND A.WorkID in (select OID from V_FlowData where ProjNo ='" + ProjNo
                    + "') AND B.IsEnable=1 AND (B.IsPass=1 or B.IsPass < 0 ) ";

                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.RetrieveInSQL(GenerWorkFlowAttr.WorkID, "(" + sql + ")");
                DataTable dt = gwfs.ToDataTableField();
                //按照接受日期排序
                dt.DefaultView.Sort = "RDT DESC";
                return GetEasyUIJson(dt.DefaultView.ToTable());
            }
            catch
            {
                return "[{}]";
            }
        }
        /// <summary>
        /// 获取抄送列表
        /// </summary>
        /// <returns></returns>
        private string GetCCFlowData()
        {
            try
            {
                string FK_Flow = getUTF8ToString("FK_Flow");
                string ProjNo = getUTF8ToString("ProjNo");
                string sql = "SELECT a.* FROM WF_CCList a,V_FlowData b where a.WorkID=b.OID and a.Sta=0 and a.CCTo='" + WebUser.No
                    + "' and b.ProjNo='" + ProjNo + "' and b.FK_Flow='" + FK_Flow + "' order by RDT desc ";
                DataTable dt = DBAccess.RunSQLReturnTable(sql);

                return GetEasyUIJson(dt);
            }
            catch
            {
                return "[{}]";
            }
        }
        /// <summary>
        /// 获取挂起数据，传入流程编号与项目编号
        /// </summary>
        /// <returns></returns>
        private string GetHunupFlowData()
        {
            try
            {
                string FK_Flow = getUTF8ToString("FK_Flow");
                string ProjNo = getUTF8ToString("ProjNo");
                int state = (int)WFState.HungUp;
                string sql = "SELECT a.WorkID FROM WF_GenerWorkFlow A, WF_GenerWorkerlist B WHERE A.FK_Flow='" + FK_Flow
                    + "'  AND A.WFState=" + state + " AND A.WorkID=B.WorkID AND B.FK_Emp='" + WebUser.No
                    + "' AND A.WorkID in (select OID from V_FlowData where ProjNo ='" + ProjNo + "') AND B.IsEnable=1 ";

                GenerWorkFlows gwfs = new GenerWorkFlows();
                gwfs.RetrieveInSQL(GenerWorkFlowAttr.WorkID, "(" + sql + ")");
                DataTable dt = gwfs.ToDataTableField();

                return GetEasyUIJson(dt);
            }
            catch
            {
                return "[{}]";
            }
        }

        public string GetEasyUIJson(DataTable table)
        {
            string json = "{rows:" + Newtonsoft.Json.JsonConvert.SerializeObject(table) + ",total:" + table.Rows.Count + "}";
            return json;
        }

        /// <summary>
        /// 打印项目经历月报
        /// </summary>
        private void PrintMonthPlanWord()
        {
            string ProjNo = getUTF8ToString("ProjNo");
            string nian = getUTF8ToString("Nian");
            string yue = getUTF8ToString("yue");


        }
    }
}