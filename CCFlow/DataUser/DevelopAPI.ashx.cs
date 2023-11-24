using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.DA;
using BP.En;
using BP.Web;
using System.Web.SessionState;
using BP.WF.DTS;

namespace CCFlow.DataUser
{
    /// <summary>
    /// HandlerAPI 的摘要说明
    /// 1. 我们使用一般程序处理服务请求.
    /// 2. 所有的请求都有一个DoWhat,SID标记，然后在处理页面处理这个标记.
    /// 3. DoWhat用于标识要处理什么， SID用于标识用户的身份.
    /// </summary>
    public class DevelopAPI : IHttpHandler, IRequiresSessionState
    {
        public HttpContext context = null;
        public string SID = "";
        public void ProcessRequest(HttpContext con)
        {
            context = con;
            string xx = "";
            xx = xx.Replace("''", "'");

            #region 效验问题.
            //让其支持跨域访问.
            string origin = context.Request.Headers["Origin"];
            if (string.IsNullOrEmpty(origin) == false)
            {
                var allAccess_Control_Allow_Origin = System.Web.Configuration.WebConfigurationManager.AppSettings["Access-Control-Allow-Origin"];
                context.Response.Headers["Access-Control-Allow-Origin"] = origin;
                // context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
                context.Response.Headers["Access-Control-Allow-Headers"] = "x-requested-with,content-type";
            }

            string domain = context.Request.QueryString["DoMain"];

            string doType = null;
            try
            {
                //设置通用的变量.
                doType = context.Request.QueryString["DoType"];
                if (BP.DA.DataType.IsNullOrEmpty(doType) == true)
                    doType = context.Request.QueryString["DoWhat"];

                //首先判断是否有token.
                string token = context.Request.QueryString["Token"];
                if (DataType.IsNullOrEmpty(token) == false)
                    BP.WF.Dev2Interface.Port_LoginByToken(token);

                //如果是请求登录. .
                if (doType.Equals("Port_Login_Submit") == true)
                {
                    string key = context.Request.QueryString["PrivateKey"];
                    string userNo = context.Request.QueryString["UserNo"];

                    string localKey = BP.Difference.SystemConfig.GetValByKey("PrivateKey", "");
                    if (DataType.IsNullOrEmpty(localKey) == true)
                        localKey = "DiGuaDiGua,IamCCBPM";

                    if (localKey.Equals(key) == false)
                    {
                        ResponseWrite("err@私约错误，请检查全局文件中配置 PrivateKey ");
                        return;
                    }

                    //执行本地登录.
                    BP.WF.Dev2Interface.Port_Login(userNo);
                    string toke = BP.WF.Dev2Interface.Port_GenerToken();

                    Hashtable ht = new Hashtable();
                    ht.Add("No", WebUser.No);
                    ht.Add("Name", WebUser.Name);
                    ht.Add("FK_Dept", WebUser.DeptNo);
                    ht.Add("FK_DeptName", WebUser.DeptName);
                    ht.Add("OrgNo", WebUser.OrgNo);
                    ht.Add("OrgName", WebUser.OrgName);
                    ht.Add("Token", toke);
                    ResponseWrite(BP.Tools.Json.ToJson(ht));
                    return;
                }
                this.SID = token; //记录下来他的sid.
            }
            catch (Exception ex)
            {
                this.ResponseWrite("err@" + ex.Message);
                return;
            }
            #endregion 效验问题.

            #region 批处理相关.
            try
            {
                if (doType.Equals("Batch_Init") == true)
                {
                    var handle = new BP.WF.HttpHandler.WF();
                    string str = handle.Batch_Init();
                    this.ResponseWrite(str);
                    return;
                }
                if (doType.Equals("En_Node") == true)
                {
                    var node = new Node(this.NodeID);
                    this.ResponseWrite(node.ToJson());
                    return;
                }
                if (doType.Equals("En_Flow") == true)
                {
                    var en = new Flow(this.No);
                    this.ResponseWrite(en.ToJson());
                    return;
                }
                //下拉框.
                if (doType.Equals("WorkCheckModel_Init") == true)
                {
                    var handle = new BP.WF.HttpHandler.WF_WorkOpt_Batch();
                    string str = handle.WorkCheckModel_Init();
                    this.ResponseWrite(str);
                    return;
                }
                

                //下拉框.
                if (doType.Equals("Batch_InitDDL") == true)
                {
                    var handle = new BP.WF.HttpHandler.WF_MyFlow();
                    string str = handle.Batch_InitDDL();
                    this.ResponseWrite(str);
                    return;
                }

                //批處理發送.
                if (doType.Equals("WorkCheckModel_Send") == true)
                {
                    var handle = new BP.WF.HttpHandler.WF_WorkOpt_Batch();
                    string str = handle.WorkCheckModel_Send();
                    this.ResponseWrite(str);
                    return;
                }
                //批量刪除.
                if (doType.Equals("Batch_Delete") == true)
                {
                    var handle = new BP.WF.HttpHandler.WF_WorkOpt_Batch();
                    string str = handle.Batch_Delete();
                    this.ResponseWrite(str);
                    return;
                }

            }
            catch (Exception ex)
            {
                this.ResponseWrite("err@" + ex.Message);
                return;
            }
            #endregion 批处理相关.


            #region  与流程处理相关的接口API.
            try
            {
              

                if (doType.Equals("Node_CreateBlankWorkID") == true)
                {
                    //创建workid.
                    Int64 workid = Dev2Interface.Node_CreateBlankWork(this.FlowNo, BP.Web.WebUser.No);
                    this.ResponseWrite(workid.ToString());
                    return;
                }
                if (doType.Equals("Node_IsCanDealWork") == true)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                    string todoEmps = gwf.TodoEmps;
                    bool isCanDo = false;
                    if (gwf.NodeID.ToString().EndsWith("01") == true)
                    {
                        if (gwf.Starter.Equals(BP.Web.WebUser.No) == false)
                            isCanDo = false; //处理开始节点发送后，撤销的情况，第2个节点打开了，第1个节点撤销了,造成第2个节点也可以发送下去.
                        else
                            isCanDo = true; // 开始节点不判断权限.
                    }
                    else
                    {
                        isCanDo = todoEmps.Contains(";" + WebUser.No + ",");
                        if (isCanDo == false)
                            isCanDo = Dev2Interface.Flow_IsCanDoCurrentWork(this.WorkID, BP.Web.WebUser.No);
                    }
                    this.ResponseWrite(isCanDo==true?"1":"0");
                    return;

                }
                //翻译过去.
                if (doType.Equals("Node_SetDraft") == true)
                {
                    BP.WF.Dev2Interface.Node_SetDraft(this.WorkID);
                    return;
                }
                
                if (doType.Equals("Node_Shift") == true)
                {
                    string toEmpNo = context.Request.QueryString["ToEmpNo"];
                    string msg = context.Request.QueryString["Msg"];
                    BP.WF.Dev2Interface.Node_Shift(this.WorkID, toEmpNo, msg);
                    return;
                }

                // 给人员增加待办.
                if (doType.Equals("Node_AddTodolist") == true)
                {
                    string EmpNo = context.Request.QueryString["EmpNo"];
                    BP.WF.Dev2Interface.Node_AddTodolist(this.WorkID, EmpNo);
                    this.ResponseWrite("增加人员成功.");
                    return;
                }

                //获得流程信息.
                if (doType.Equals("Flow_GenerWorkFlow") == true)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                    this.ResponseWrite(gwf.ToJson());
                    return;
                }
                //保存参数字段. 
                if (doType.Equals("Node_SaveParas") == true)
                {
                    BP.WF.Dev2Interface.Flow_SaveParas(this.WorkID, context.Request.QueryString["Paras"]);
                    this.ResponseWrite("参数保存成功");
                    return;
                }
                //保存参数字段. 
                if (doType.Equals("Flow_SetTitle") == true)
                {
                    BP.WF.Dev2Interface.Flow_SetFlowTitle(this.FlowNo, this.WorkID, context.Request.QueryString["Title"]);
                    this.ResponseWrite("标题设置成功.");
                    return;
                }
                if (doType.Equals("Node_SendWork") == true)
                {
                    //执行发送.
                    Hashtable ht = new Hashtable();
                    foreach (string str in con.Request.QueryString)
                    {
                        if (DataType.IsNullOrEmpty(str) == true)
                            continue;
                        string val = this.GetValByKey(str);
                        if (val == null)
                            continue;
                        ht.Add(str, val);
                    }

                    int toNodeID = this.GetValIntByKey("ToNodeID");
                    string toEmps = this.GetValByKey("ToEmps");
                    try
                    {
                        string flowNo = this.FlowNo;
                        if (DataType.IsNullOrEmpty(flowNo) == true)
                            flowNo = DBAccess.RunSQLReturnString("SELECT FK_Flow FROM WF_GenerWorkFlow WHERE WorkID=" + this.WorkID);

                        //执行发送.
                        SendReturnObjs objs = Dev2Interface.Node_SendWork(flowNo, this.WorkID, ht, null, toNodeID, toEmps);
                        this.ResponseWrite(objs.ToMsgOfText());
                        return;
                    }
                    catch (Exception ex)
                    {
                        this.ResponseWrite("err@" + ex.Message);
                        return;
                    }
                }
                //根据流程编号获得流程信息.
                if (doType.Equals("DB_GenerWorkFlow") == true)
                {
                    GenerWorkFlows gwfs = new GenerWorkFlows();
                    QueryObject qo = new QueryObject(gwfs);
                    qo.AddWhere("FK_Flow", this.FlowNo);
                    qo.addAnd();
                    qo.AddWhere("WFState", ">", 1);
                    //  qo.addAnd();
                    // qo.AddWhere("WFState", "!=", 3);
                    qo.addOrderBy("RDT");
                    qo.DoQuery();
                    this.ResponseWrite(gwfs.ToJson());
                    return;
                }

                if (doType.Equals("DB_GenerWillReturnNodes") == true)
                {
                    //获得可以退回的节点.
                    DataTable dt = Dev2Interface.DB_GenerWillReturnNodes(this.WorkID);
                    this.ResponseWrite(BP.Tools.Json.ToJson(dt));
                    return;
                }
                if (doType.Equals("Node_ReturnWork") == true)
                {
                    int toNodeID = this.GetValIntByKey("ReturnToNodeID");
                    string returnToEmp = this.GetValByKey("ReturnToEmp");
                    if (toNodeID == 0)
                    {
                        DataTable dt = BP.WF.Dev2Interface.DB_GenerWillReturnNodes( this.WorkID);
                        if (dt.Rows.Count == 1)
                        {
                            toNodeID = Int32.Parse(dt.Rows[0]["No"].ToString());
                            returnToEmp = dt.Rows[0]["Rec"].ToString();

                        }
                    }
                    //执行退回.
                    string strs = Dev2Interface.Node_ReturnWork(this.WorkID,
                        toNodeID, returnToEmp, this.GetValByKey("Msg"), this.GetValBoolenByKey("IsBackToThisNode"));
                    this.ResponseWrite(strs);
                    return;
                }
            }
            catch (Exception ex)
            {
                this.ResponseWrite("err@" + ex.Message);
                return;
            }
            #endregion 与流程处理相关的接口API.

            #region 处理相关功能.
            switch (doType)
            {
                case "Search_Init": //
                    Search_Init();
                    return;
                case "DB_Start": //获得发起列表.
                    DataTable dtStrat = Dev2Interface.DB_StarFlows(BP.Web.WebUser.No, domain);
                    this.ResponseWrite(BP.Tools.Json.ToJson(dtStrat));
                    return;
                case "DB_Draft": //草稿.
                    DataTable dtDraft = Dev2Interface.DB_GenerDraftDataTable(null, domain);
                    this.ResponseWrite(BP.Tools.Json.ToJson(dtDraft));
                    return;
                case "GenerFrmUrl": //获得发起的URL.
                    GenerFrmUrl();
                    return;
                case "DB_Todolist": //获得待办.
                    DataTable dtTodolist = Dev2Interface.DB_GenerEmpWorksOfDataTable(BP.Web.WebUser.No, 0, null, domain, null, null);
                    this.ResponseWrite(BP.Tools.Json.ToJson(dtTodolist));
                    return;
                case "DB_Runing": //获得未完成(在途).
                    DataTable dtRuing = Dev2Interface.DB_GenerRuning(BP.Web.WebUser.No, false, domain);
                    this.ResponseWrite(BP.Tools.Json.ToJson(dtRuing));
                    return;
                case "Flow_DoPress": //批量催办.
                    this.Flow_DoPress();
                    return;
                case "CC_BatchCheckOver": //批量抄送审核.
                    this.CC_BatchCheckOver();
                    return;
                case "Flow_BatchDeleteByFlag": //批量删除.
                    this.Flow_BatchDeleteByFlag();
                    return;
                case "Flow_BatchDeleteByReal": //批量删除.
                    this.Flow_BatchDeleteByReal();
                    break;
                case "Flow_BatchDeleteByFlagAndUnDone": //恢复批量删除.
                    this.Flow_BatchDeleteByFlagAndUnDone();
                    return;
                case "Flow_DoUnSend": //撤销发送..
                    this.Flow_DoUnSend();
                    return;
                case "Flow_DeleteDraft": //删除草稿箱..
                    this.Flow_DeleteDraft();
                    return;
                case "Flow_DoFlowOver": //批量结束.
                    this.Flow_DoFlowOver();
                    return;
                case "Port_LoginOut": //退出系统..
                    BP.WF.Dev2Interface.Port_SigOut();
                    return;
                default:
                    break;
            }
            #endregion 处理相关功能.

            #region 特殊处理.infoDtl
            switch (doType)
            {
                case "InfoSorts":
                    string sql = "SELECT * FROM OA_InfoType ";
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    this.ResponseWrite(BP.Tools.Json.ToJson(dt));
                    return;
                case "InfoDtls":
                    string sortNo = this.GetValByKey("SortNo");
                    if (DataType.IsNullOrEmpty(sortNo) == true)
                        sql = "SELECT * FROM OA_Info WHERE InfoPRI=1 ";
                    else
                        sql = "SELECT * FROM OA_Info WHERE InfoType='" + sortNo + "' ";

                    dt = DBAccess.RunSQLReturnTable(sql);
                    this.ResponseWrite(BP.Tools.Json.ToJson(dt));
                    return;
                case "Dtl": //明细信息.
                    string dtlNo = this.GetValByKey("No");
                    sql = "SELECT * FROM OA_Info WHERE No='" + dtlNo + "' ";
                    dt = DBAccess.RunSQLReturnTable(sql);
                    dt.TableName = "OA_InfoDtl";
                    this.ResponseWrite(BP.Tools.Json.ToJson(dt));
                    // DataSet ds = new DataSet();
                    // ds.Tables.Add(dt);
                    // this.ResponseWrite(BP.Tools.Json.ToJson(ds));
                    return;
                default:
                    break;
            }
            #endregion 特殊处理.

            context.Response.ContentType = "text/plain";
            context.Response.Write("err@没有判断的执行类型:" + doType);
        }

        /// <summary>
        /// 流程查询
        /// </summary>
        /// <returns></returns>
        public void Search_Init()
        {
            GenerWorkFlows gwfs = new GenerWorkFlows();

            string scop = this.GetValByKey("Scop");//范围.
            string key = this.GetValByKey("Key");//关键字.
            string dtFrom = this.GetValByKey("DTFrom");//日期从.
            string dtTo = this.GetValByKey("DTTo");//日期到.
            int pageIdx = int.Parse(this.GetValByKey("PageIdx"));//分页.

            //创建查询对象.
            QueryObject qo = new QueryObject(gwfs);
            if (DataType.IsNullOrEmpty(key) == false)
            {
                qo.AddWhere(GenerWorkFlowAttr.Title, " LIKE ", "%" + key + "%");
                qo.addAnd();
            }

            //我参与的.
            if (scop.Equals("0") == true)
                qo.AddWhere(GenerWorkFlowAttr.Emps, "LIKE", "%@" + WebUser.No + ",%");

            //我发起的.
            if (scop.Equals("1") == true)
                qo.AddWhere(GenerWorkFlowAttr.Starter, "=", WebUser.No);

            //我部门发起的.
            if (scop.Equals("2") == true)
                qo.AddWhere(GenerWorkFlowAttr.FK_Dept, "=", WebUser.DeptNo);


            //任何一个为空.
            if (DataType.IsNullOrEmpty(dtFrom) == true || DataType.IsNullOrEmpty(dtTo) == true)
            {

            }
            else
            {
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.RDT, ">=", dtFrom);
                qo.addAnd();
                qo.AddWhere(GenerWorkFlowAttr.RDT, "<=", dtTo);
            }

            var count = qo.GetCount(); //获得总数.

            qo.DoQuery("WorkID", 20, pageIdx);
            //   qo.DoQuery(); // "WorkID", 20, pageIdx);


            DataTable dt = gwfs.ToDataTableField("gwls");

            //创建容器.
            DataSet ds = new DataSet();
            ds.Tables.Add(dt); //增加查询对象.

            //增加数量.
            DataTable mydt = new DataTable();
            mydt.TableName = "count";
            mydt.Columns.Add("CC");
            DataRow dr = mydt.NewRow();
            dr[0] = count.ToString(); //把数量加进去.
            mydt.Rows.Add(dr);
            ds.Tables.Add(mydt);

            string json = BP.Tools.Json.ToJson(ds);
            //      string json = gwfs.ToJson("DT" + count);
            this.ResponseWrite(json);
        }

        #region 通用方法.
        public string GetValByKey(string key)
        {
            string str = context.Request.QueryString[key];
            if (DataType.IsNullOrEmpty(str))
                return null;
            return str;
        }
        public int GetValIntByKey(string key)
        {
            string val = GetValByKey(key);
            if (val == null)
                return 0;
            return int.Parse(val);
        }
        public bool GetValBoolenByKey(string key)
        {
            string val = GetValByKey(key);
            if (val == null)
                return false;
            if (val.Equals("0") == true)
                return false;
            return true;
        }
        #endregion 通用方法.

        #region 定义变量.
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FlowNo
        {
            get
            {
                string str = context.Request.QueryString["FK_Flow"];
                if (DataType.IsNullOrEmpty(str))
                    return null;
                return str;
            }
        }
        public string No
        {
            get
            {
                string str = context.Request.QueryString["No"];
                if (DataType.IsNullOrEmpty(str))
                    return null;
                return str;
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                string str = context.Request.QueryString["FK_Node"];
                if (DataType.IsNullOrEmpty(str) == true)
                    str = context.Request.QueryString["NodeID"];
                if (DataType.IsNullOrEmpty(str) == true)
                    return 0;
                return int.Parse(str);
            }
        }
        public Int64 WorkID
        {
            get
            {
                string str = context.Request.QueryString["WorkID"];
                if (DataType.IsNullOrEmpty(str) == true)
                    return 0;
                return Int64.Parse(str);
            }
        }
        public Int64 FID
        {
            get
            {
                string str = context.Request.QueryString["FID"];
                if (DataType.IsNullOrEmpty(str) == true)
                    return 0;
                return Int64.Parse(str);
            }
        }
        #endregion 定义变量.

        #region case 方法.
        public void Flow_DoFlowOver()
        {
            string workids = this.GetValByKey("WorkIDs");
            string[] strs = workids.Split(',');

            string info = "";
            foreach (string workidStr in strs)
            {
                if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                    continue;

                BP.WF.Dev2Interface.Flow_DoFlowOver(Int64.Parse(workidStr), "批量结束", 1);
            }
            Output("执行成功.");
        }
        /// <summary>
        /// 删除草稿
        /// </summary>
        public void Flow_DeleteDraft()
        {
            string workids = this.GetValByKey("WorkIDs");
            string[] strs = workids.Split(',');

            string info = "";
            foreach (string workidStr in strs)
            {
                if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                    continue;

                BP.WF.Dev2Interface.Node_DeleteDraft(Int64.Parse(workidStr));
            }
            Output("删除成功.");
        }

        /// <summary>
        /// 撤销发送
        /// </summary>
        public void Flow_DoUnSend()
        {
            string workids = this.GetValByKey("WorkIDs");
            string[] strs = workids.Split(',');

            string info = "";
            foreach (string workidStr in strs)
            {
                if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                    continue;

                info += BP.WF.Dev2Interface.Flow_DoUnSend(null, Int64.Parse(workidStr), 0, 0);
            }
            Output(info);
        }
        public void Flow_BatchDeleteByReal()
        {
            string workids = this.GetValByKey("WorkIDs");
            string[] strs = workids.Split(',');

            foreach (string workidStr in strs)
            {
                if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                    continue;

                string st1r = BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(Int64.Parse(workidStr), true);
            }
            Output("删除成功.");
        }
        /// <summary>
        /// 删除功能
        /// </summary>
        public void Flow_BatchDeleteByFlag()
        {
            string workids = this.GetValByKey("WorkIDs");
            string[] strs = workids.Split(',');

            foreach (string workidStr in strs)
            {
                if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                    continue;

                string st1r = BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(Int64.Parse(workidStr), "删除", true);
            }

            Output("删除成功.");
        }
        /// <summary>
        /// 恢复删除
        /// </summary>
        public void Flow_BatchDeleteByFlagAndUnDone()
        {
            string workids = this.GetValByKey("WorkIDs");
            string[] strs = workids.Split(',');

            foreach (string workidStr in strs)
            {
                if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                    continue;

                string st1r = BP.WF.Dev2Interface.Flow_DoUnDeleteFlowByFlag(null, int.Parse(workidStr), "删除");
            }

            Output("恢复成功.");
        }
        public void Flow_DoPress()
        {
            string workids = this.GetValByKey("WorkIDs");
            string[] strs = workids.Split(',');

            string msg = this.GetValByKey("Msg");
            if (msg == null)
                msg = "需要您处理待办工作.";

            string info = "";
            foreach (string workidStr in strs)
            {
                if (BP.DA.DataType.IsNullOrEmpty(workidStr) == true)
                    continue;

                info += "@" + BP.WF.Dev2Interface.Flow_DoPress(int.Parse(workidStr), msg, true);
            }
            Output(info);
        }
        /// <summary>
        /// 批量设置抄送查看完毕
        /// </summary>
        /// <returns></returns>
        public void CC_BatchCheckOver()
        {
            string workids = this.GetValByKey("WorkIDs");
            string str = BP.WF.Dev2Interface.Node_CC_SetCheckOverBatch(workids);
            Output(str);
        }
        public void Output(string info)
        {
            context.Response.ContentType = "text/plain";
            //context.Response.Headers.Add( = "Access - Control - Allow - Origin: *";
            context.Response.Write(info);
        }
        #endregion

        /// <summary>
        /// 获得发起的url. 
        /// </summary>
        public void GenerFrmUrl()
        {
            /*
             * 发起的url需要在该流程的开始节点的表单方案中，使用SDK表单，并把表单的url设置到里面去.
             * 设置步骤:
             * 1. 打开流程设计器.
             * 2. 在开始节点上右键，选择表单方案.
             * 3. 选择SDK表单，把url配置到文本框里去.
             * 比如: /App/F027QingJia.htm
             */

            int nodeID = this.NodeID;
            if (nodeID == 0)
                nodeID = int.Parse(this.FlowNo + "01");

            Int64 workid = this.WorkID;
            if (workid == 0)
                workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FlowNo, BP.Web.WebUser.No);

            string url = "";
            Node nd = new Node(nodeID);
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            string todoEmps = gwf.TodoEmps;
            bool isCanDo = false;
            if (gwf.NodeID.ToString().EndsWith("01") == true)
            {
                if (gwf.Starter.Equals(BP.Web.WebUser.No) == false)
                    isCanDo = false; //处理开始节点发送后，撤销的情况，第2个节点打开了，第1个节点撤销了,造成第2个节点也可以发送下去.
                else
                    isCanDo = true; // 开始节点不判断权限.
            }
            else
            {
                isCanDo = todoEmps.Contains(";" + WebUser.No + ",");
                if (isCanDo == false)
                    isCanDo = Dev2Interface.Flow_IsCanDoCurrentWork(workid, BP.Web.WebUser.No);
            }
            if (nd.FormType == NodeFormType.SDKForm || nd.FormType == NodeFormType.SelfForm)
            {
                url = nd.FormUrl;
                if (url.Contains("?") == true)
                    url += "&FK_Flow=" + this.FlowNo + "&FK_Node=" + nodeID + "&WorkID=" + workid + "&Token=" + this.SID + "&UserNo=" + BP.Web.WebUser.No;
                else
                    url += "?FK_Flow=" + this.FlowNo + "&FK_Node=" + nodeID + "&WorkID=" + workid + "&Token=" + this.SID + "&UserNo=" + BP.Web.WebUser.No;
            }
            else
            {
                url = "/WF/MyFlow.htm?FK_Flow=" + this.FlowNo + "&FK_Node=" + nodeID + "&WorkID=" + this.WorkID + "&Token=" + this.SID;
            }
            url += "&IsReadonly=" + (isCanDo == true ? 0 : 1);
            ResponseWrite(url);

        }
        public void ResponseWrite(string strs)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(strs);

        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}