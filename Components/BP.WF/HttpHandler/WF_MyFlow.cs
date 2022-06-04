using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.En;
using BP.WF.Template;
using BP.Difference;
using BP.WF.Template.SFlow;



namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 流程处理类
    /// </summary>
    public class WF_MyFlow : DirectoryPageBase
    {
        #region  运行变量
        /// <summary>
        /// 从节点.
        /// </summary>
        public string FromNode
        {
            get
            {
                return this.GetRequestVal("FromNode");
            }
        }
        /// <summary>
        /// 是否抄送
        /// </summary>
        public bool IsCC
        {
            get
            {
                string str = this.GetRequestVal("Paras");

                if (DataType.IsNullOrEmpty(str) == false)
                {
                    string myps = str;

                    if (myps.Contains("IsCC=1") == true)
                        return true;
                }

                str = this.GetRequestVal("AtPara");
                if (DataType.IsNullOrEmpty(str) == false)
                {
                    if (str.Contains("IsCC=1") == true)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 轨迹ID
        /// </summary>
        public string TrackID
        {
            get
            {
                return this.GetRequestVal("TrackeID");
            }
        }
        /// <summary>
        /// 到达的节点ID
        /// </summary>
        public int ToNode
        {
            get
            {
                return this.GetRequestValInt("ToNode");
            }
        }
        private int _FK_Node = 0;
        /// <summary>
        /// 当前的 NodeID ,在开始时间,nodeID,是地一个,流程的开始节点ID.
        /// </summary>
        public new int FK_Node
        {
            get
            {
                string fk_nodeReq = this.GetRequestVal("FK_Node");  //this.Request.Form["FK_Node"];
                if (DataType.IsNullOrEmpty(fk_nodeReq))
                    fk_nodeReq = this.GetRequestVal("NodeID");// this.Request.Form["NodeID"];

                if (DataType.IsNullOrEmpty(fk_nodeReq) == false)
                    return int.Parse(fk_nodeReq);

                if (_FK_Node == 0)
                {
                    if (this.WorkID != 0)
                    {
                        Paras ps = new Paras();
                        ps.SQL = "SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID";
                        ps.Add("WorkID", this.WorkID);
                        _FK_Node = DBAccess.RunSQLReturnValInt(ps, 0);
                    }
                    else
                    {
                        _FK_Node = int.Parse(this.FK_Flow + "01");
                    }
                }
                return _FK_Node;
            }
            set
            {
                _FK_Node = value;
            }
        }

        private string _width = "";
        /// <summary>
        /// 表单宽度
        /// </summary>
        public string Width
        {
            get
            {
                return _width;
            }
            set { _width = value; }
        }
        private string _height = "";
        /// <summary>
        /// 表单高度
        /// </summary>
        public string Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public string _btnWord = "";
        public string BtnWord
        {
            get { return _btnWord; }
            set { _btnWord = value; }
        }
        private GenerWorkFlow _HisGenerWorkFlow = null;
        public GenerWorkFlow HisGenerWorkFlow
        {
            get
            {
                if (_HisGenerWorkFlow == null)
                    _HisGenerWorkFlow = new GenerWorkFlow(this.WorkID);
                return _HisGenerWorkFlow;
            }
        }
        private Node _currNode = null;
        public Node currND
        {
            get
            {
                if (_currNode == null)
                    _currNode = new Node(this.FK_Node);
                return _currNode;
            }
            set
            {
                _currNode = value;
            }
        }
        private Flow _currFlow = null;
        public Flow currFlow
        {
            get
            {
                if (_currFlow == null)
                    _currFlow = new Flow(this.FK_Flow);
                return _currFlow;
            }
            set
            {
                _currFlow = value;
            }
        }
        /// <summary>
        /// 定义跟路径
        /// </summary>
        public string appPath = "/";


        //杨玉慧
        public string DoType1
        {
            get { return HttpContextHelper.RequestParams("DoType1"); }
        }
        #endregion

        public string Focus()
        {
            BP.WF.Dev2Interface.Flow_Focus(this.WorkID);
            return "设置成功.";
        }
        /// <summary>
        /// 确认
        /// </summary>
        /// <returns></returns>
        public string Confirm()
        {
            BP.WF.Dev2Interface.Flow_Confirm(this.WorkID);
            return "设置成功.";
        }
        /// <summary>
        /// 删除子流程
        /// </summary>
        /// <returns></returns>
        public string DelSubFlow()
        {
            BP.WF.Dev2Interface.Flow_DeleteSubThread(this.WorkID, "手工删除");
            return "删除成功.";
        }
        /// <summary>
        /// 加载前置导航数据
        /// </summary>
        /// <returns></returns>
        public string StartGuide_Init()
        {
            string josnData = "";
            //流程编号
            string fk_flow = this.GetRequestVal("FK_Flow");
            //查询的关键字
            string skey = this.GetRequestVal("Keys");
            try
            {
                //获取流程实例
                Flow fl = new Flow(fk_flow);
                //获取设置的前置导航的sql
                string sql = fl.StartGuidePara2.Clone() as string;
                //判断是否有查询条件
                if (!string.IsNullOrWhiteSpace(skey))
                {
                    sql = fl.StartGuidePara1.Clone() as string;
                    sql = sql.Replace("@Key", skey);
                }
                sql = sql.Replace("~", "'");
                //替换约定参数
                sql = sql.Replace("@WebUser.No", WebUser.No);
                sql = sql.Replace("@WebUser.Name", WebUser.Name);
                sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
                sql = sql.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);

                if (sql.Contains("@") == true)
                {
                    foreach (string key in HttpContextHelper.RequestParamKeys)
                    {
                        sql = sql.Replace("@" + key, this.GetRequestVal(key));
                    }

                    foreach (string key in HttpContextHelper.RequestParamKeys)
                    {
                        sql = sql.Replace("@" + key, this.GetRequestVal(key));
                    }
                }

                //获取数据
                DataTable dt = DBAccess.RunSQLReturnTable(sql);

                //判断前置导航的类型
                switch (fl.StartGuideWay)
                {
                    case StartGuideWay.BySQLOne:
                    case StartGuideWay.BySystemUrlOneEntity:
                        josnData = BP.Tools.Json.ToJson(dt);
                        break;
                    case StartGuideWay.BySQLMulti:
                        josnData = BP.Tools.Json.ToJson(dt);
                        break;
                    default:
                        break;
                }
                return josnData;
            }
            catch (Exception ex)
            {
                return "err@:" + ex.Message.ToString();
            }
        }
        /// <summary>
        /// 没有WorkID
        /// </summary>
        /// <returns></returns>
        public string MyFlow_Init_NoWorkID()
        {
            string isStartSameLevelFlow = this.GetRequestVal("IsStartSameLevelFlow");

            #region 判断是否可以否发起流程. 
            try
            {
                if (BP.WF.Dev2Interface.Flow_IsCanStartThisFlow(this.FK_Flow, WebUser.No, this.PFlowNo, this.PNodeID, this.PWorkID) == false)
                {
                    /*是否可以发起流程？ */
                    throw new Exception("err@您(" + BP.Web.WebUser.No + ")没有发起或者处理该流程的权限.");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("外部用户") == true)
                {
                    //判断是否是开始节点？这里要发起流程.
                    Node nd = new Node(this.FK_Node);
                    if (nd.IsStartNode == true && nd.HisDeliveryWay == DeliveryWay.ByGuest)
                        return "url@./WorkOpt/GuestStartFlow/GenerCode.htm";


                }

                throw new Exception("err@" + ex.Message);
            }

            /*如果是开始节点, 先检查是否启用了流程限制。*/
            if (BP.WF.Glo.CheckIsCanStartFlow_InitStartFlow(this.currFlow) == false)
            {
                /* 如果启用了限制就把信息提示出来. */
                string msg = BP.WF.Glo.DealExp(this.currFlow.StartLimitAlert, null, null);
                return "err@" + msg;
            }
            #endregion 判断是否可以否发起流程

            #region 判断前置导航.
            //生成workid.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, null, null,
                WebUser.No, null, this.PWorkID, this.PFID, this.PFlowNo, this.PNodeID, null, 0, null, null, isStartSameLevelFlow);

            string hostRun = this.currFlow.GetValStrByKey(FlowAttr.HostRun);
            if (DataType.IsNullOrEmpty(hostRun) == false)
                hostRun += "/WF/";

            this.WorkID = workid; //给workid赋值.

            switch (this.currFlow.StartGuideWay)
            {
                case StartGuideWay.None:
                    break;
                case StartGuideWay.SubFlowGuide:
                case StartGuideWay.SubFlowGuideEntity:
                    return "url@" + hostRun + "WorkOpt/StartGuide/Guide.htm?FK_Flow=" + this.currFlow.No + "&WorkID=" + workid;
                case StartGuideWay.ByHistoryUrl: // 历史数据.
                    if (this.currFlow.IsLoadPriData == true)
                    {
                        return "err@流程设计错误，您不能同时启用前置导航，自动装载上一笔数据两个功能。";
                    }
                    return "url@" + hostRun + "WorkOpt/StartGuide/Guide.htm?FK_Flow=" + this.currFlow.No + "&WorkID=" + workid;
                case StartGuideWay.BySystemUrlOneEntity:
                    return "url@" + hostRun + "WorkOpt/StartGuide/GuideEntities.htm?StartGuideWay=BySystemUrlOneEntity&WorkID=" + workid + "" + this.RequestParasOfAll;
                case StartGuideWay.BySQLOne:
                    return "url@" + hostRun + "WorkOpt/StartGuide/Entities.htm?StartGuideWay=BySQLOne&WorkID=" + workid + "" + this.RequestParasOfAll;
                case StartGuideWay.BySQLMulti:
                    return "url@" + hostRun + "WorkOpt/StartGuide/Entities.htm?StartGuideWay=BySQLMulti&WorkID=" + workid + "" + this.RequestParasOfAll;
                case StartGuideWay.BySelfUrl: //按照定义的url.
                    return "url@" + this.currFlow.StartGuidePara1 + this.RequestParasOfAll + "&WorkID=" + workid;
                case StartGuideWay.ByFrms: //选择表单.
                    return "url@" + hostRun + "WorkOpt/StartGuide/Frms.htm?FK_Flow=" + this.currFlow.No + "&WorkID=" + workid;
                case StartGuideWay.ByParentFlowModel: //选择父流程.
                    return "url@" + hostRun + "WorkOpt/StartGuide/ParentFlowModel.htm?FK_Flow=" + this.currFlow.No + "&WorkID=" + workid;
                default:
                    throw new Exception("没有解析的发起导航模式:" + this.currFlow.StartGuideWay);
                    break;
            }
            #endregion 判断前置导航

            return null; //生成了workid.
        }
        public string DictFlow_Init()
        {
            #region 判断是否可以否发起流程. 
            try
            {
                if (BP.WF.Dev2Interface.Flow_IsCanStartThisFlow(this.FK_Flow, WebUser.No, this.PFlowNo, this.PNodeID, this.PWorkID) == false)
                {
                    /*是否可以发起流程？ */
                    throw new Exception("err@您(" + BP.Web.WebUser.No + ")没有发起或者处理该流程的权限.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("err@" + ex.Message);
            }

            /*如果是开始节点, 先检查是否启用了流程限制。*/
            if (BP.WF.Glo.CheckIsCanStartFlow_InitStartFlow(this.currFlow) == false)
            {
                /* 如果启用了限制就把信息提示出来. */
                string msg = BP.WF.Glo.DealExp(this.currFlow.StartLimitAlert, null, null);
                return "err@" + msg;
            }
            #endregion 判断是否可以否发起流程

            //生成workid.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, GuestUser.No);

            #region 设置流程实体关系
            BP.WF.GERpt rpt = new BP.WF.GERpt("ND" + int.Parse(this.FK_Flow) + "Rpt");
            rpt.OID = workid;
            if (rpt.RetrieveFromDBSources() != 0)
            {
                rpt.PFlowNo = this.GetRequestVal("FrmID");
                rpt.PWorkID = this.GetRequestValInt64("FrmOID");
                rpt.Update();
            }
            GenerWorkFlow gwf = new GenerWorkFlow(workid);
            gwf.PWorkID = this.GetRequestValInt64("FrmOID"); ;
            gwf.PFlowNo = this.GetRequestVal("FrmID");
            gwf.Update();
            #endregion 设置流程实体关系

            #region 判断前置导航.
            string hostRun = this.currFlow.GetValStrByKey(FlowAttr.HostRun);
            if (DataType.IsNullOrEmpty(hostRun) == false)
                hostRun += "/WF/";

            this.WorkID = workid; //给workid赋值.

            switch (this.currFlow.StartGuideWay)
            {
                case StartGuideWay.None:
                    break;
                case StartGuideWay.SubFlowGuide:
                case StartGuideWay.SubFlowGuideEntity:
                    return "url@" + hostRun + "StartGuide.htm?FK_Flow=" + this.currFlow.No + "&WorkID=" + workid;
                case StartGuideWay.ByHistoryUrl: // 历史数据.
                    if (this.currFlow.IsLoadPriData == true)
                    {
                        return "err@流程设计错误，您不能同时启用前置导航，自动装载上一笔数据两个功能。";
                    }
                    return "url@" + hostRun + "StartGuide.htm?FK_Flow=" + this.currFlow.No + "&WorkID=" + workid;
                case StartGuideWay.BySystemUrlOneEntity:
                    return "url@" + hostRun + "StartGuideEntities.htm?StartGuideWay=BySystemUrlOneEntity&WorkID=" + workid + "" + this.RequestParasOfAll;
                case StartGuideWay.BySQLOne:
                    return "url@" + hostRun + "StartGuideEntities.htm?StartGuideWay=BySQLOne&WorkID=" + workid + "" + this.RequestParasOfAll;
                case StartGuideWay.BySQLMulti:
                    return "url@" + hostRun + "StartGuideEntities.htm?StartGuideWay=BySQLMulti&WorkID=" + workid + "" + this.RequestParasOfAll;
                case StartGuideWay.BySelfUrl: //按照定义的url.
                    return "url@" + this.currFlow.StartGuidePara1 + this.RequestParasOfAll + "&WorkID=" + workid;
                case StartGuideWay.ByFrms: //选择表单.
                    return "url@" + hostRun + "./WorkOpt/StartGuideFrms.htm?FK_Flow=" + this.currFlow.No + "&WorkID=" + workid;
                case StartGuideWay.ByParentFlowModel: //选择父流程
                    return "url@" + hostRun + "./WorkOpt/StartGuideParentFlowModel.htm?FK_Flow=" + this.currFlow.No + "&WorkID=" + workid;
                default:
                    break;
            }
            #endregion 判断前置导航

            this.WorkID = workid;
            return MyFlow_Init();
        }
        /// <summary>
        /// 初始化(处理分发)
        /// </summary>
        /// <returns></returns>
        public string MyFlow_Init()
        {
            if (this.WorkID == 0 && this.FID==0)
            {
                string val = MyFlow_Init_NoWorkID();
                if (val != null)
                    return val;
            }

            //子线程退回分流节点
            if (BP.WF.Dev2Interface.Flow_IsCanToFLTread(this.WorkID, this.FID, this.FK_Node) == true)
            {
                GenerWorkFlow mgwf = new GenerWorkFlow(this.FID);
                //返回子线程综处理页面
                return "url@MyFLDealThread.htm?WorkID=0&FK_Flow=" + mgwf.FK_Flow + "&FK_Node=" + this.FK_Node + "&PWorkID=" + mgwf.PWorkID + "&FID=" + this.FID;
            }
            //定义变量.
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = this.WorkID;
            if (gwf.RetrieveFromDBSources() == 0)
                return ("err@该流程ID{" + this.WorkID + "}不存在，或者已经被删除.");

           


            //手动启动子流程的标志 0父子流程 1 同级子流程
            string isStartSameLevelFlow = this.GetRequestVal("IsStartSameLevelFlow");
            this.currND = new Node(gwf.FK_Node);

            #region 做权限判断.
            //授权人
            string auther = this.GetRequestVal("Auther");
            if (DataType.IsNullOrEmpty(auther) == false)
            {
                BP.Web.WebUser.Auth = auther;
                BP.Web.WebUser.AuthName = BP.DA.DBAccess.RunSQLReturnString("SELECT Name FROM Port_Emp WHERE No='" + auther + "'");
            }
            else
            {
                BP.Web.WebUser.Auth = "";
                BP.Web.WebUser.AuthName = ""; // BP.DA.DBAccess.RunSQLReturnString("SELECT Name FROM Port_Emp WHERE No='" + auther + "'");
            }

            //判断是否有执行该工作的权限.
            string todEmps = ";" + gwf.TodoEmps;
            bool isCanDo = false;
            if (gwf.FK_Node.ToString().EndsWith("01") == true)
            {
                if (gwf.Starter.Equals(BP.Web.WebUser.No) == false)
                    isCanDo = false; //处理开始节点发送后，撤销的情况，第2个节点打开了，第1个节点撤销了,造成第2个节点也可以发送下去.
                else
                    isCanDo = true; // 开始节点不判断权限.
            }
            else
            {
                isCanDo = todEmps.Contains(";" + WebUser.No + ",");
                if (isCanDo == false)
                    isCanDo = Dev2Interface.Flow_IsCanDoCurrentWork(this.WorkID, BP.Web.WebUser.No);
            }

            if (isCanDo == false)
                return "err@您[" + WebUser.No + "," + WebUser.Name + "]不能执行当前工作, 当前工作已经运转到[" + gwf.NodeName + "],处理人[" + gwf.TodoEmps + "]。";

            string frms = this.GetRequestVal("Frms");
            if (DataType.IsNullOrEmpty(frms) == false)
            {
                gwf.Paras_Frms = frms;
                gwf.Update();
            }
            #endregion 做权限判断.

            #region 判断是否是混合执行.
            if (this.currND.WhoExeIt == 2)
            {
                /*如果当前节点是混合执行，就执行一下发送。*/
                try
                {
                    //这里可能会出现异常,有异常就是要阻止发送动作.
                    SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(gwf.FK_Flow, gwf.WorkID, 0, null);

                    gwf = new GenerWorkFlow(this.WorkID);
                    this.currND = new Node(gwf.FK_Node);
                    this.FK_Node = gwf.FK_Node;

                    //判断是否移动到下一个节点？
                    if (objs.VarToNodeID != gwf.FK_Node)
                        return "url@MyFlow.htm?WorkID=" + gwf.WorkID + "&FK_Node=" + objs.VarToNodeID + "&FID=" + gwf.FID;
                }
                catch (Exception ex)
                {
                    Log.DebugWriteInfo("myflow_int@判断是否是混合执行,提示未发送成功信息:" + ex.Message);
                }
            }
            #endregion 判断是否是混合执行.

            #region 处理打开既阅读.
            //判断当前节点是否是打开即阅读
            //获取当前节点信息
            if (this.currND.IsOpenOver == true)
            {
                //如果是结束节点执行流程结束功能
                if (this.currND.IsStartNode == false)
                {
                    //如果启用审核组件
                    if (this.currND.FrmWorkCheckSta == FrmWorkCheckSta.Enable)
                    {
                        //判断一下审核意见是否有默认值
                        NodeWorkCheck workCheck = new NodeWorkCheck("ND" + this.currND.NodeID);
                        string msg = BP.WF.Glo.DefVal_WF_Node_FWCDefInfo; // 设置默认值;
                        if (workCheck.FWCIsFullInfo == true)
                            msg = workCheck.FWCDefInfo;
                        BP.WF.Dev2Interface.WriteTrackWorkCheck(gwf.FK_Flow, this.currND.NodeID, gwf.WorkID, gwf.FID, msg, workCheck.FWCOpLabel, null);
                    }

                    BP.WF.Dev2Interface.Node_SendWork(gwf.FK_Flow, gwf.WorkID);
                    return "url@" + "./MyView.htm?WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "&FK_Node=" + gwf.FK_Node + "&PWorkID=" + gwf.PWorkID + "&FID=" + gwf.FID;
                }
            }
            #endregion 处理打开既阅读.


            #region 处理打开跳转.
            if (this.currND.SkipTime == 1)
            {
                // string info = CheckSkipTime(this.currND);
                // if (info != null)
                //   return info;

            }
            #endregion 处理打开跳转.


            #region 前置导航数据拷贝到第一节点
            if (this.GetRequestVal("IsCheckGuide") != null)
            {
                string key = this.GetRequestVal("KeyNo");
                DataTable dt = BP.WF.Glo.StartGuidEnties(this.WorkID, this.FK_Flow, this.FK_Node, key);

                /*如果父流程编号，就要设置父子关系。*/
                if (dt != null && dt.Rows.Count > 0 && dt.Columns.Contains("PFlowNo") == true)
                {
                    string pFlowNo = dt.Rows[0]["PFlowNo"].ToString();
                    int pNodeID = int.Parse(dt.Rows[0]["PNodeID"].ToString());
                    Int64 pWorkID = Int64.Parse(dt.Rows[0]["PWorkID"].ToString());
                    string pEmp = ""; // dt.Rows[0]["PEmp"].ToString();
                    if (DataType.IsNullOrEmpty(pEmp))
                        pEmp = WebUser.No;

                    //设置父子关系.
                    BP.WF.Dev2Interface.SetParentInfo(this.FK_Flow, this.WorkID, pWorkID);
                }
            }
            #endregion

            #region 启动同级子流程的信息存储
            if (isStartSameLevelFlow != null && isStartSameLevelFlow.Equals("1") == true)
            {
                string slFlowNo = GetRequestVal("SLFlowNo");
                Int32 slNode = GetRequestValInt("SLNodeID");
                Int64 slWorkID = GetRequestValInt("SLWorkID");
                gwf.SetPara("SLFlowNo", slFlowNo);
                gwf.SetPara("SLNodeID", slNode);
                gwf.SetPara("SLWorkID", slWorkID);
                gwf.SetPara("SLEmp", BP.Web.WebUser.No);
                gwf.Update();
            }
            #endregion 启动同级子流程的信息存储


            if (this.currND.IsStartNode)
            {
                /*如果是开始节点, 先检查是否启用了流程限制。*/
                if (BP.WF.Glo.CheckIsCanStartFlow_InitStartFlow(this.currFlow) == false)
                {
                    /* 如果启用了限制就把信息提示出来. */
                    string msg = BP.WF.Glo.DealExp(this.currFlow.StartLimitAlert, null, null);
                    return "err@" + msg;
                }
            }

            #region 处理表单类型.
            if (this.currND.HisFormType == NodeFormType.SheetTree
                 || this.currND.HisFormType == NodeFormType.SheetAutoTree)
            {

                #region 开始组合url.
                string toUrl = "";
                if (this.IsMobile == true)
                {
                    if (gwf.Paras_Frms.Equals("") == false)
                        toUrl = "MyFlowGener.htm?WorkID=" + this.WorkID + "&NodeID=" + gwf.FK_Node + "&FK_Node=" + gwf.FK_Node + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&Token=" + WebUser.Token + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID + "&Frms=" + gwf.Paras_Frms;
                    else
                        toUrl = "MyFlowGener.htm?WorkID=" + this.WorkID + "&NodeID=" + gwf.FK_Node + "&FK_Node=" + gwf.FK_Node + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&Token=" + WebUser.Token + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID;
                }
                else
                {
                    if (gwf.Paras_Frms.Equals("") == false)
                        toUrl = "MyFlowTree.htm?WorkID=" + this.WorkID + "&NodeID=" + gwf.FK_Node + "&FK_Node=" + gwf.FK_Node + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&Token=" + WebUser.Token + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID + "&Frms=" + gwf.Paras_Frms;
                    else
                        toUrl = "MyFlowTree.htm?WorkID=" + this.WorkID + "&NodeID=" + gwf.FK_Node + "&FK_Node=" + gwf.FK_Node + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&Token=" + WebUser.Token + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID;
                }

                string[] strs = this.RequestParas.Split('&');
                foreach (string str in strs)
                {
                    if (toUrl.Contains(str) == true)
                        continue;
                    if (str.Contains("DoType=") == true)
                        continue;
                    if (str.Contains("DoMethod=") == true)
                        continue;
                    if (str.Contains("HttpHandlerName=") == true)
                        continue;
                    if (str.Contains("IsLoadData=") == true)
                        continue;
                    if (str.Contains("IsCheckGuide=") == true)
                        continue;

                    toUrl += "&" + str;
                }
                foreach (string key in HttpContextHelper.RequestParamKeys)
                {
                    if (toUrl.Contains(key + "=") == true)
                        continue;
                    toUrl += "&" + key + "=" + HttpContextHelper.RequestParams(key);
                }

                #endregion 开始组合url.

                //增加fk_node
                if (toUrl.Contains("&FK_Node=") == false)
                    toUrl += "&FK_Node=" + this.currND.NodeID;

                //如果是开始节点.
                if (currND.IsStartNode == true)
                {
                    if (toUrl.Contains("PrjNo") == true && toUrl.Contains("PrjName") == true)
                    {
                        string sql = "UPDATE " + this.currFlow.PTable + " SET PrjNo='" + this.GetRequestVal("PrjNo") + "', PrjName='" + this.GetRequestVal("PrjName") + "' WHERE OID=" + this.WorkID;
                        DBAccess.RunSQL(sql);
                    }
                }
                return "url@" + toUrl;
            }

            if (this.currND.HisFormType == NodeFormType.SDKForm)
            {
                string url = currND.FormUrl;
                if (DataType.IsNullOrEmpty(url))
                    return "err@设置读取状流程设计错误态错误,没有设置表单url.";

                //处理连接.
                url = this.MyFlow_Init_DealUrl(currND, url);

                //sdk表单就让其跳转.
                return "url@" + url;
            }
            #endregion 处理表单类型.

            //求出当前节点frm的类型.
            NodeFormType frmtype = this.currND.HisFormType;
            if (frmtype != NodeFormType.RefOneFrmTree)
            {
                currND.WorkID = this.WorkID; //为获取表单ID ( NodeFrmID )提供参数.

                if (this.currND.NodeFrmID.Contains(this.currND.NodeID.ToString()) == false)
                {
                    /*如果当前节点引用的其他节点的表单.*/
                    string nodeFrmID = currND.NodeFrmID;
                    string refNodeID = nodeFrmID.Replace("ND", "");
                    BP.WF.Node nd = new Node(int.Parse(refNodeID));

                    //表单类型.
                    frmtype = nd.HisFormType;
                }
            }

            #region 内置表单类型的判断.

            if (frmtype == NodeFormType.FoolTruck)
            {
                string url = "MyFlowGener.htm";

                //处理连接.
                url = this.MyFlow_Init_DealUrl(currND, url);
                return "url@" + url;
            }

            if (frmtype == NodeFormType.FoolForm && this.IsMobile == false)
            {
                /*如果是傻瓜表单，就转到傻瓜表单的解析执行器上。*/
                string url = "MyFlowGener.htm";
                if (this.IsMobile)
                    url = "MyFlowGener.htm";

                //处理连接.
                url = this.MyFlow_Init_DealUrl(currND, url);

                url = url.Replace("DoType=MyFlow_Init&", "");
                url = url.Replace("&DoWhat=StartClassic", "");
                return "url@" + url;
            }

            //自定义表单
            if (frmtype == NodeFormType.SelfForm && this.IsMobile == false)
            {
                string url = "MyFlowSelfForm.htm";

                //处理连接.
                url = this.MyFlow_Init_DealUrl(currND, url);

                url = url.Replace("DoType=MyFlow_Init&", "");
                url = url.Replace("&DoWhat=StartClassic", "");
                return "url@" + url;
            }
            #endregion 内置表单类型的判断.

            string myurl = "MyFlowGener.htm";
            //MapData md = new MapData(this.currND.NodeFrmID);
            //if (md.HisFrmType == FrmType.ChapterFrm)
            //    myurl = "MyFlowTree.htm?NodeFrmType=11";
            //处理连接.
            myurl = this.MyFlow_Init_DealUrl(currND, myurl);
            myurl = myurl.Replace("DoType=MyFlow_Init&", "");
            myurl = myurl.Replace("&DoWhat=StartClassic", "");
            return "url@" + myurl;
        }
        private string MyFlow_Init_DealUrl(BP.WF.Node currND, string url = null)
        {
            if (url == null)
                url = currND.FormUrl;
            string urlExt = "";

            //如果是分流点/分河流。且FID!=0
            if ((currND.HisRunModel == RunModel.FL || currND.HisRunModel == RunModel.FHL) && this.FID != 0)
                urlExt += "WorkID=" + this.FID + "&SubWorkID=" + this.WorkID;
            else
                urlExt += "WorkID=" + this.WorkID;

            urlExt += "&NodeID=" + currND.NodeID;
            urlExt += "&FK_Node=" + currND.NodeID;
            urlExt += "&FID=" + this.FID;
            urlExt += "&UserNo=" + HttpUtility.UrlEncode(WebUser.No);
            urlExt += "&Token=" + WebUser.Token;


            //SDK表单上服务器地址,应用到使用ccflow的时候使用的是sdk表单,该表单会存储在其他的服务器上,珠海高凌提出. 
            url = url.Replace("@SDKFromServHost", BP.Difference.SystemConfig.AppSettings["SDKFromServHost"]);

            if (url.Contains("?") == true)
                url += "&" + urlExt;
            else
                url += "?" + urlExt;

            foreach (string str in HttpContextHelper.RequestParamKeys)
            {
                if (DataType.IsNullOrEmpty(str) == true || str.Equals("T") == true || str.Equals("t") == true)
                    continue;
                if (url.Contains(str + "=") == true)
                    continue;
                url += "&" + str + "=" + this.GetRequestVal(str);
            }

            url = url.Replace("?&", "?");
            url = url.Replace("&&", "&");
            return url;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_MyFlow()
        {

        }
        /// <summary>
        /// 结束流程.
        /// </summary>
        /// <returns></returns>
        public string MyFlow_StopFlow()
        {
            try
            {
                string str = BP.WF.Dev2Interface.Flow_DoFlowOver(this.WorkID, "流程成功结束");
                if (DataType.IsNullOrEmpty(str) == true)
                    return "流程成功结束";
                return str;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 删除流程
        /// </summary>
        /// <returns></returns>
        public string MyFlow_DeleteFlowByReal()
        {
            try
            {
                string str = BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.WorkID);
                if (DataType.IsNullOrEmpty(str) == true)
                    return "流程删除成功";
                return str;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 保存发送参数.
        /// </summary>
        /// <returns></returns>
        public string SaveParas()
        {
            BP.WF.Dev2Interface.Flow_SaveParas(this.WorkID, this.GetRequestVal("Paras"));
            return "保存成功";
        }
        /// <summary>
        /// 子线程退回到分流的时候工具栏.
        /// </summary>
        /// <param name="gwf"></param>
        /// <param name="dt"></param>
        /// <param name="nd"></param>
        /// <returns></returns>
        public string InitToolBar_ForFenLiu(GenerWorkFlow gwf, DataTable dt,
            Node nd)
        {
            DataRow dr = null;

            dr = dt.NewRow();
            dr["No"] = "OpenFrm";
            dr["Name"] = "查看表单";
            dr["Oper"] = "";
            dt.Rows.Add(dr);

            //dr = dt.NewRow();
            //dr["No"] = "KillThread";
            //dr["Name"] = "取消子线程";
            //dr["Oper"] = "KillThread()";
            //dt.Rows.Add(dr);
            if (nd.ThreadIsCanDel == true)
            {
                dr = dt.NewRow();
                dr["No"] = "UnSendAllThread";
                dr["Name"] = "撤销整体发送";
                dr["Oper"] = "UnSendAllThread()";
                dt.Rows.Add(dr);
            }
            

            if (nd.ThreadIsCanAdd==true)
            {//@hongyan
                bool isCanAdd = false;
                //判断分流点到达的节点是同表单子线程还是异表单子线程
                Nodes nds = nd.HisToNodes;
                if (nds.Count == 1)
                    isCanAdd = true;


                if (isCanAdd == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "AddThread";
                    dr["Name"] = "增加子线程";
                    dr["Oper"] = "AddThread()";
                    dt.Rows.Add(dr);
                }
                
            }
 
            dr = dt.NewRow();
            dr["No"] = "Track";
            dr["Name"] = "轨迹";
            dr["Oper"] = "";
            dt.Rows.Add(dr);

            DataSet ds = new DataSet();
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 初始化toolbar.
        /// </summary>
        /// <returns></returns>
        public string InitToolBar()
        {
            DataSet ds = new DataSet();
            //创建一个DataTable，返回按钮信息
            DataTable dt = new DataTable("ToolBar");
            dt.Columns.Add("No");
            dt.Columns.Add("Name");
            dt.Columns.Add("Oper");
            dt.Columns.Add("Role", typeof(int));
            dt.Columns.Add("Icon");

            #region 处理是否是加签，或者是否是会签模式.
            bool isAskForOrHuiQian = false;
            BtnLab btnLab = new BtnLab(this.FK_Node);
            Node nd = new Node(this.FK_Node);
            Int64 workId = this.WorkID;
            if (workId == 0)
                workId = this.FID;
            GenerWorkFlow gwf = new GenerWorkFlow(workId);

            #region 分流点，是否是发送多个子线程，单个子线程退回
            if (BP.WF.Dev2Interface.Flow_IsCanToFLTread(this.WorkID,this.FID,this.FK_Node)==true)
            {
              
                return InitToolBar_ForFenLiu(gwf, dt, nd);
            }
            #endregion 分流点，是否是发送多个子线程，单个子线程退回

            if (this.FK_Node.ToString().EndsWith("01") == false)
            {
                if (gwf.WFState == WFState.Askfor)
                    isAskForOrHuiQian = true;

                /*判断是否是加签状态，如果是，就判断是否是主持人，如果不是主持人，就让其 isAskFor=true ,屏蔽退回等按钮.*/
                /**说明：针对于组长模式的会签，协作模式的会签加签人仍可以加签*/
                if (gwf.HuiQianTaskSta == HuiQianTaskSta.HuiQianing)
                {
                    //初次打开会签节点时
                    if (DataType.IsNullOrEmpty(gwf.HuiQianZhuChiRen) == true)
                    {
                        if (gwf.TodoEmps.Contains(WebUser.No + ",") == false)
                            isAskForOrHuiQian = true;
                    }

                    //执行会签后的状态
                    if (btnLab.HuiQianRole == HuiQianRole.TeamupGroupLeader && btnLab.HuiQianLeaderRole == 0)
                    {
                        if (gwf.HuiQianZhuChiRen != WebUser.No && gwf.GetParaString("AddLeader").Contains(WebUser.No + ",") == false)
                            isAskForOrHuiQian = true;
                    }
                    else
                    {
                        if (gwf.HuiQianZhuChiRen.Contains(WebUser.No + ",") == false && gwf.GetParaString("AddLeader").Contains(WebUser.No + ",") == false)
                            isAskForOrHuiQian = true;
                    }

                }
            }
            #endregion 处理是否是加签，或者是否是会签模式，.

            DataRow dr = dt.NewRow();

            string toolbar = "";
            try
            {
                #region 是否是会签？.
                if (isAskForOrHuiQian == true && BP.Difference.SystemConfig.CustomerNo == "LIMS")
                    return "";

                if (isAskForOrHuiQian == true)
                {
                    dr["No"] = "Send";
                    dr["Name"] = "确定/完成";
                    dr["Oper"] = btnLab.SendJS + " if(SysCheckFrm()==false) return false;Send(false, " + (int)nd.FormType + "); ";
                    dt.Rows.Add(dr);
                    if (btnLab.PrintZipEnable == true)
                    {
                        dr = dt.NewRow();
                        dr["No"] = "PackUp";
                        dr["Name"] = btnLab.PrintZipLab;
                        dr["Oper"] = "";
                        dt.Rows.Add(dr);
                    }

                    if (btnLab.TrackEnable)
                    {
                        dr = dt.NewRow();
                        dr["No"] = "Track";
                        dr["Name"] = btnLab.TrackLab;
                        dr["Oper"] = "";
                        dt.Rows.Add(dr);
                    }

                    return BP.Tools.Json.ToJson(dt);
                }
                #endregion 是否是会签.

               

                #region 是否是抄送.
                if (this.IsCC)
                {
                    dr = dt.NewRow();
                    dr["No"] = "Track";
                    dr["Name"] = "流程运行轨迹";
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                    // 判断审核组件在当前的表单中是否启用，如果启用了.
                    NodeWorkCheck fwc = new NodeWorkCheck(this.FK_Node);
                    if (fwc.HisFrmWorkCheckSta != FrmWorkCheckSta.Enable)
                    {
                        dr = dt.NewRow();
                        /*如果不等于启用, */
                        dr["No"] = "CCWorkCheck";
                        dr["Name"] = "填写审核意见";
                        dr["Oper"] = "";
                        dt.Rows.Add(dr);

                        //toolbar += "<input type=button  value='填写审核意见' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/CCCheckNote.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />";
                    }
                    return toolbar;
                }
                #endregion 是否是抄送.

                #region 如果当前节点启用了协作会签.
                //if (btnLab.HuiQianRole == HuiQianRole.Teamup)
                //{
                //    dr = dt.NewRow();
                //    dr["No"] = "SendHuiQian";
                //    dr["Name"] = "会签发送";
                //    dr["Oper"] = btnLab.SendJS + " if(SysCheckFrm()==false) return false;Send(true, " + (int)nd.FormType + ");";
                //    dt.Rows.Add(dr);

                //}
                #endregion 如果当前节点启用了协作会签

                #region 加载流程控制器 - 按钮
                if (this.currND.HisFormType == NodeFormType.SelfForm)
                {
                    /*如果是嵌入式表单.*/
                    if (currND.IsEndNode)
                    {
                        /*如果当前节点是结束节点.*/
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
                        {
                            dr = dt.NewRow();
                            /*如果启用了发送按钮.*/
                            dr["No"] = "Send";
                            dr["Name"] = btnLab.SendLab;
                            dr["Oper"] = btnLab.SendJS + " if (SysCheckFrm()==false) return false;Send(false, " + (int)nd.FormType + ");";
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
                        {
                            dr = dt.NewRow();
                            dr["No"] = "Send";
                            dr["Name"] = btnLab.SendLab;
                            dr["Oper"] = btnLab.SendJS + " if ( SysCheckFrm()==false) return false; Send(false, " + (int)nd.FormType + ");";
                            dt.Rows.Add(dr);
                        }
                    }

                    /*处理保存按钮.*/
                    if (btnLab.SaveEnable)
                    {
                        dr = dt.NewRow();
                        dr["No"] = "Save";
                        dr["Name"] = btnLab.SaveLab;
                        dr["Oper"] = "if(SysCheckFrm()==false) return false;SaveOnly();SaveEnd(" + (int)nd.FormType + ");";
                        dt.Rows.Add(dr);
                    }
                }

                if (this.currND.HisFormType != NodeFormType.SelfForm)
                {
                    /*启用了其他的表单.*/
                    if (currND.IsEndNode)
                    {
                        /*如果当前节点是结束节点.*/
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
                        {
                            /*如果启用了选择人窗口的模式是【选择既发送】.*/
                            dr = dt.NewRow();
                            dr["No"] = "Send";
                            dr["Name"] = btnLab.SendLab;
                            dr["Oper"] = btnLab.SendJS + " if(SysCheckFrm()==false) return false;Send(false, " + (int)nd.FormType + ");";
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
                        {
                            /*如果启用了发送按钮.
                             * 1. 如果是加签的状态，就不让其显示发送按钮，因为在加签的提示。
                             */
                            dr = dt.NewRow();
                            dr["No"] = "Send";
                            dr["Name"] = btnLab.SendLab;
                            dr["Oper"] = btnLab.SendJS + " if(SysCheckFrm()==false) return false;Send(false, " + (int)nd.FormType + ");";
                            dt.Rows.Add(dr);
                        }
                    }

                    /* 处理保存按钮.*/
                    if (btnLab.SaveEnable)
                    {
                        dr = dt.NewRow();
                        dr["No"] = "Save";
                        dr["Name"] = btnLab.SaveLab;
                        dr["Oper"] = "if (SysCheckFrm() == false) return false; SaveOnly();SaveEnd(" + (int)nd.FormType + "); ";
                        dt.Rows.Add(dr);
                    }
                }

                //发起会签子流程
                if (nd.IsSendDraftSubFlow == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "StartThread";
                    dr["Name"] = "发起会签";
                    dr["Oper"] = "StartThread()";
                    dt.Rows.Add(dr);/*发起会签子流程*/
                }

                //是否启用 挂起.
                if (btnLab.HungEnable == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "Hungup";
                    dr["Name"] = btnLab.HungLab; //挂起.
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);/*挂起*/
                }


                //if (btnLab.WorkCheckEnable)
                //{
                //    dr = dt.NewRow();
                //    dr["No"] = "workcheckBtn";
                //    dr["Name"] = btnLab.WorkCheckLab;
                //    dr["Oper"] = "";
                //    dt.Rows.Add(dr);/*审核*/
                //}

                if (btnLab.ThreadEnable)
                {
                    /*如果要查看子线程.*/
                    dr = dt.NewRow();
                    dr["No"] = "Thread";
                    dr["Name"] = btnLab.ThreadLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }

                if (btnLab.ShowParentFormEnable && this.PWorkID != 0)
                {
                    /*如果要查看父流程.*/
                    dr = dt.NewRow();
                    dr["No"] = "ParentForm";
                    dr["Name"] = btnLab.ShowParentFormLab;
                    dr["Oper"] = "";

                    dt.Rows.Add(dr);
                }

                if (btnLab.TCEnable == true)
                {
                    /*流转自定义..*/
                    dr = dt.NewRow();
                    dr["No"] = "TransferCustom";
                    dr["Name"] = btnLab.TCLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }

                if (btnLab.HelpRole != 0)
                {
                    dr = dt.NewRow();
                    dr["No"] = "Help";
                    dr["Name"] = btnLab.HelpLab;
                    dr["Oper"] = "HelpAlter()";
                    dr["Role"] = btnLab.HelpRole;
                    dt.Rows.Add(dr);
                }

                if (btnLab.JumpWayEnum != JumpWay.CanNotJump)
                {
                    /*跳转*/
                    dr = dt.NewRow();
                    dr["No"] = "JumpWay";
                    dr["Name"] = btnLab.JumpWayLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }

                if (btnLab.ReturnEnable)
                {
                    /*退回*/
                    dr = dt.NewRow();
                    dr["No"] = "Return";
                    dr["Name"] = btnLab.ReturnLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }



                if (btnLab.ShiftEnable)
                {
                    /*移交*/
                    dr = dt.NewRow();
                    dr["No"] = "Shift";
                    dr["Name"] = btnLab.ShiftLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }

                if ((btnLab.CCRole == CCRole.HandCC || btnLab.CCRole == CCRole.HandAndAuto))
                {

                    // 抄送 
                    dr = dt.NewRow();
                    dr["No"] = "CC";
                    dr["Name"] = btnLab.CCLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }

                if (btnLab.DeleteEnable != 0)
                {
                    dr = dt.NewRow();
                    dr["No"] = "Delete";
                    dr["Name"] = btnLab.DeleteLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }

                if (btnLab.EndFlowEnable && this.currND.IsStartNode == false)
                {
                    dr = dt.NewRow();
                    dr["No"] = "EndFlow";
                    dr["Name"] = btnLab.EndFlowLab;
                    dr["Oper"] = "DoStop('" + btnLab.EndFlowLab + "','" + this.FK_Flow + "','" + this.WorkID + "')";
                    dt.Rows.Add(dr);

                }

                if (btnLab.PrintDocEnable == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "PrintDoc";
                    dr["Name"] = btnLab.PrintDocLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);


                }

                if (btnLab.TrackEnable)
                {
                    dr = dt.NewRow();
                    dr["No"] = "Track";
                    dr["Name"] = btnLab.TrackLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }


                if (btnLab.SearchEnable)
                {
                    dr = dt.NewRow();
                    dr["No"] = "Search";
                    dr["Name"] = btnLab.SearchLab;
                    dr["Oper"] = "WinOpen('./RptDfine/Default.htm?RptNo=ND" + int.Parse(this.FK_Flow) + "MyRpt&FK_Flow=" + this.FK_Flow + "&SearchType=My')";
                    dt.Rows.Add(dr);
                }

                if (btnLab.BatchEnable)
                {
                    string urlr = appPath + "WF/Batch.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;

                    /*批量处理*/
                    dr = dt.NewRow();
                    dr["No"] = "Batch";
                    dr["Name"] = btnLab.BatchLab;
                    dr["Oper"] = "To('" + urlr + "');";
                    dt.Rows.Add(dr);

                }

                //if (btnLab.AskforEnable)
                //{
                //    /*加签 */
                //    dr = dt.NewRow();
                //    dr["No"] = "Askfor";
                //    dr["Name"] = btnLab.AskforLab;
                //    dr["Oper"] = "";
                //    dt.Rows.Add(dr);

                //}

                if (btnLab.HuiQianRole != HuiQianRole.None)
                {
                    /*会签 */
                    dr = dt.NewRow();
                    dr["No"] = "HuiQian";
                    dr["Name"] = btnLab.HuiQianLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }

                //原始会签主持人可以增加组长
                if (btnLab.HuiQianRole != HuiQianRole.None && btnLab.AddLeaderEnable == true)
                {
                    /*增加组长 */
                    dr = dt.NewRow();
                    dr["No"] = "AddLeader";
                    dr["Name"] = btnLab.AddLeaderLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }


                //if (btnLab.WebOfficeWorkModel == WebOfficeWorkModel.Button)
                //{
                //    /*公文正文 */
                //    dr = dt.NewRow();
                //    dr["No"] = "WebOffice";
                //    dr["Name"] = btnLab.WebOfficeLab;
                //    dr["Oper"] = "";
                //    dt.Rows.Add(dr);

                //}

                // 需要翻译.
                if (this.currFlow.IsResetData == true && this.currND.IsStartNode)
                {
                    /* 启用了数据重置功能 */
                    dr = dt.NewRow();
                    dr["No"] = "ReSet";
                    dr["Name"] = "数据重置";
                    dr["Oper"] = "resetData();";
                    dt.Rows.Add(dr);
                }


                if (btnLab.CHRole != 0)
                {
                    /* 节点时限设置 */
                    dr = dt.NewRow();
                    dr["No"] = "CH";
                    dr["Name"] = btnLab.CHLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }

                if (btnLab.NoteEnable != 0)
                {
                    /* 备注设置 */
                    dr = dt.NewRow();
                    dr["No"] = "Note";
                    dr["Name"] = btnLab.NoteLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }


                if (btnLab.PRIEnable != 0)
                {
                    /* 优先级设置 */
                    dr = dt.NewRow();
                    dr["No"] = "PR";
                    dr["Name"] = btnLab.PRILab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }

                /* 关注 */
                if (btnLab.FocusEnable == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "Focus";
                    if (HisGenerWorkFlow.Paras_Focus == true)
                        dr["Name"] = "取消关注";
                    else
                        dr["Name"] = btnLab.FocusLab;
                    dr["Oper"] = "FocusBtn(this,'" + this.WorkID + "');";
                    dt.Rows.Add(dr);
                }

                /* 分配工作 */
                if (btnLab.AllotEnable == true)
                {
                    /*分配工作*/
                    dr = dt.NewRow();
                    dr["No"] = "Allot";
                    dr["Name"] = btnLab.AllotLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }

                /* 确认 */
                if (btnLab.ConfirmEnable == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "Confirm";
                    if (HisGenerWorkFlow.Paras_Confirm == true)
                        dr["Name"] = "取消确认";
                    else
                        dr["Name"] = btnLab.ConfirmLab;

                    dr["Oper"] = "ConfirmBtn(this,'" + this.WorkID + "');";
                    dt.Rows.Add(dr);
                }

                // 需要翻译.

                /* 打包下载zip */
                if (btnLab.PrintZipEnable == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "PackUp_zip";
                    dr["Name"] = btnLab.PrintZipLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }

                /* 打包下载html */
                if (btnLab.PrintHtmlEnable == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "PackUp_html";
                    dr["Name"] = btnLab.PrintHtmlLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }

                /* 打包下载pdf */
                if (btnLab.PrintPDFEnable == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "PackUp_pdf";
                    dr["Name"] = btnLab.PrintPDFLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }

                if (btnLab.FrmDBVerEnable == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "FrmDBVer";
                    dr["Name"] = btnLab.FrmDBVerLab;
                    dr["Oper"] = "FrmDBVer_Init()";
                    dt.Rows.Add(dr);
                }
                //小纸条
                if (btnLab.ScripRole == 1)
                {
                    dr = dt.NewRow();
                    dr["No"] = "Scrip";
                    dr["Name"] = btnLab.ScripLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }
                //数据批阅
                if (btnLab.FrmDBRemarkEnable!=0)
                {
                    dr = dt.NewRow();
                    dr["No"] = "FrmDBRemark";
                    dr["Name"] = btnLab.FrmDBRemarkLab;
                    dr["Oper"] = "FrmDBRemark(" + btnLab.FrmDBRemarkEnable + ")";
                    dt.Rows.Add(dr);
                }

                if (btnLab.FlowBBSRole != 0)
                {
                    dr = dt.NewRow();
                    dr["No"] = "FlowBBS";
                    dr["Name"] = btnLab.FlowBBSLab;
                    dr["Oper"] = btnLab.FlowBBSRole;
                    dt.Rows.Add(dr);
                }

                if (btnLab.IMEnable == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "IM";
                    dr["Name"] = btnLab.IMLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }


                if (this.currND.IsStartNode == true)
                {
                    if (this.currFlow.IsDBTemplate == true)
                    {
                        dr = dt.NewRow();
                        dr["No"] = "DBTemplate";
                        dr["Name"] = "模版";
                        dr["Oper"] = "";
                        dt.Rows.Add(dr);
                    }
                }

                /* 公文标签 */
                if (btnLab.OfficeBtnEnable == true && btnLab.OfficeBtnLocal == 0)
                {
                    dr = dt.NewRow();
                    dr["No"] = "DocWord";
                    dr["Name"] = btnLab.OfficeBtnLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }
                bool isMobile = this.GetRequestValBoolen("IsMobile");
                if (isMobile == false && btnLab.QRCodeRole != 0)
                {
                    dr = dt.NewRow();
                    dr["No"] = "QRCode";
                    dr["Name"] = DataType.IsNullOrEmpty(btnLab.QRCodeLab) == true ? "生成二维码" : btnLab.QRCodeLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }
                #endregion

                #region 发起子流程

                if (isMobile == false)
                {
                    SubFlowHands subFlows = new SubFlowHands(this.FK_Node);
                    foreach (SubFlowHand subFlow in subFlows)
                    {
                        if (subFlow.SubFlowStartModel != 0 && subFlow.SubFlowSta == FrmSubFlowSta.Enable)
                        {
                            dr = dt.NewRow();
                            dr["No"] = "SubFlow";
                            dr["Name"] = DataType.IsNullOrEmpty(subFlow.SubFlowLab) == true ? "发起" + subFlow.SubFlowName : subFlow.SubFlowLab;
                            dr["Oper"] = "SendSubFlow(\'" + subFlow.SubFlowNo + "\',\'" + subFlow.MyPK + "\')";
                            dt.Rows.Add(dr);
                        }
                    }
                }
                #endregion

                #region  加载自定义的button.
                BP.WF.Template.NodeToolbars bars = new NodeToolbars();
                bars.Retrieve(NodeToolbarAttr.FK_Node, this.FK_Node, NodeToolbarAttr.IsMyFlow, 1, NodeToolbarAttr.Idx);
                foreach (NodeToolbar bar in bars)
                {
                    if (bar.ExcType == 1 || (!DataType.IsNullOrEmpty(bar.Target) == false && bar.Target.ToLower() == "javascript"))
                    {
                        dr = dt.NewRow();
                        dr["No"] = "NodeToolBar";
                        dr["Name"] = bar.Title;
                        dr["Oper"] = bar.Url;
                        //判断按钮图片路径是否有值
                        string IconPath = bar.IconPath;
                        if (DataType.IsNullOrEmpty(IconPath))
                            dr["Icon"] = bar.Row["WebPath"];
                        else
                            dr["Icon"] = IconPath;
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        string urlr3 = bar.Url + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow;

                        dr = dt.NewRow();
                        dr["No"] = "NodeToolBar";
                        dr["Name"] = bar.Title;
                        dr["Oper"] = "WinOpen('" + urlr3 + "')";
                        //判断按钮图片路径是否有值
                        string IconPath = bar.IconPath;
                        if (DataType.IsNullOrEmpty(IconPath))
                            dr["Icon"] = bar.Row["WebPath"];
                        else
                            dr["Icon"] = IconPath;
                        dt.Rows.Add(dr);

                    }
                }
                ds.Tables.Add(dt);
                #endregion  //加载自定义的button.

                #region 加载到达节点下拉框数据源.
                DataTable dtNodes = GenerDTOfToNodes(gwf, nd);
                if (dtNodes != null)
                    ds.Tables.Add(dtNodes);
                #endregion 加载到达节点下拉框数据源.

                #region 当前节点的流程信息.
                dt = nd.ToDataTableField("WF_Node");
                dt.Columns.Add("IsBackTrack", typeof(int));
                dt.Rows[0]["IsBackTrack"] = 0;
                if (gwf.WFState == WFState.ReturnSta && nd.GetParaInt("IsShowReturnNodeInToolbar") == 0)
                {
                    //当前节点是退回状态，是否原路返回
                    Paras ps = new Paras();
                    ps.SQL = "SELECT ReturnNode,Returner,ReturnerName,IsBackTracking FROM WF_ReturnWork WHERE WorkID=" + BP.Difference.SystemConfig.AppCenterDBVarStr + "WorkID  ORDER BY RDT DESC";
                    ps.Add(ReturnWorkAttr.WorkID, this.WorkID);
                    DataTable mydt = DBAccess.RunSQLReturnTable(ps);

                    //说明退回并原路返回.
                    if (mydt.Rows.Count == 0)
                        throw new Exception("err@没有找到退回信息..");

                    //设置当前是否是退回并原路返回? IsBackTracking
                    dt.Rows[0]["IsBackTrack"] = mydt.Rows[0][3]; //是否发送并返回.
                }
                ds.Tables.Add(dt);
                #endregion  当前节点的流程信息
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError(ex);
                new Exception("err@" + ex.Message);
            }
            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 批量处理
        /// </summary>
        /// <returns></returns>
        public string Batch_InitDDL()
        {
            GenerWorkFlow gwf = new GenerWorkFlow();
            Node nd = new Node(this.FK_Node);
            gwf.TodoEmps = WebUser.No + ",";
            DataTable mydt = GenerDTOfToNodes(gwf, nd);
            return BP.Tools.Json.ToJson(mydt);
        }

        public DataTable GenerDTOfToNodes(GenerWorkFlow gwf, Node nd)
        {
            //增加转向下拉框数据.
            if (nd.CondModel == DirCondModel.ByDDLSelected || nd.CondModel == DirCondModel.ByButtonSelected)
            {
            }
            else
            {
                return null;
            }
            DataTable dtToNDs = new DataTable("ToNodes");
            dtToNDs.Columns.Add("No", typeof(string));   //节点ID.
            dtToNDs.Columns.Add("Name", typeof(string)); //到达的节点名称.
            dtToNDs.Columns.Add("IsSelectEmps", typeof(string)); //是否弹出选择人的对话框？
            dtToNDs.Columns.Add("IsSelected", typeof(string));  //是否选择？
            dtToNDs.Columns.Add("DeliveryParas", typeof(string));  //自定义URL

            DataRow dr = dtToNDs.NewRow();

            if (nd.IsStartNode == true || (gwf.TodoEmps.Contains(WebUser.No + ",") == true))
            {
                /*如果当前不是主持人,如果不是主持人，就不让他显示下拉框了.*/

                /*如果当前节点，是可以显示下拉框的.*/
                //Nodes nds = nd.HisToNodes;

                BP.WF.Template.NodeSimples nds = nd.HisToNodeSimples;

                #region 增加到达延续子流程节点。 @lizhen.
                if (nd.SubFlowYanXuNum >= 1)
                {
                    SubFlowYanXus ygflows = new SubFlowYanXus(this.FK_Node);
                    foreach (SubFlowYanXu item in ygflows)
                    {
                        string[] yanxuToNDs = item.YanXuToNode.Split(',');
                        foreach (string str in yanxuToNDs)
                        {
                            if (DataType.IsNullOrEmpty(str) == true)
                                continue;

                            int toNodeID = int.Parse(str);

                            Node subNode = new Node(toNodeID);

                            dr = dtToNDs.NewRow(); //创建行。 @lizhen.

                            //延续子流程跳转过了开始节点
                            if (toNodeID == int.Parse(int.Parse(item.SubFlowNo) + "01"))
                            {
                                dr["No"] = toNodeID.ToString();
                                dr["Name"] = "启动:" + item.SubFlowName + " - " + subNode.Name;
                                dr["IsSelectEmps"] = "1";
                                dr["IsSelected"] = "0";
                                dtToNDs.Rows.Add(dr);
                            }
                            else
                            {

                                dr["No"] = toNodeID.ToString();
                                dr["Name"] = "启动:" + item.SubFlowName + " - " + subNode.Name;
                                if (subNode.HisDeliveryWay == DeliveryWay.BySelected)
                                    dr["IsSelectEmps"] = "1";
                                else
                                    dr["IsSelectEmps"] = "0";
                                dr["IsSelected"] = "0";
                                dtToNDs.Rows.Add(dr);
                            }
                        }
                    }
                }
                #endregion 增加到达延续子流程节点。

                #region 到达其他节点.
                //上一次选择的节点.
                int defalutSelectedNodeID = 0;
                if (nds.Count > 1)
                {
                    string mysql = "";
                    // 找出来上次发送选择的节点.
                    if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MSSQL)
                        mysql = "SELECT  top 1 NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND ActionType=1 ORDER BY WorkID DESC";
                    else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.Oracle)
                        mysql = "SELECT * FROM ( SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND ActionType=1 ORDER BY WorkID DESC ) WHERE ROWNUM =1";
                    else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.MySQL)
                        mysql = "SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND ActionType=1 ORDER BY WorkID  DESC limit 1,1";
                    else if (BP.Difference.SystemConfig.AppCenterDBType == DBType.PostgreSQL || BP.Difference.SystemConfig.AppCenterDBType == DBType.UX)
                        mysql = "SELECT  NDTo FROM ND" + int.Parse(nd.FK_Flow) + "Track A WHERE A.NDFrom=" + this.FK_Node + " AND ActionType=1 ORDER BY WorkID  DESC limit 1";

                    //获得上一次发送到的节点.
                    defalutSelectedNodeID = DBAccess.RunSQLReturnValInt(mysql, 0);
                }

                #region 为天业集团做一个特殊的判断.
                if (BP.Difference.SystemConfig.CustomerNo == "TianYe" && nd.Name.Contains("董事长") == true)
                {
                    /*如果是董事长节点, 如果是下一个节点默认的是备案. */
                    foreach (Node item in nds)
                    {
                        if (item.Name.Contains("备案") == true && item.Name.Contains("待") == false)
                        {
                            defalutSelectedNodeID = item.NodeID;
                            break;
                        }
                    }
                }
                #endregion 为天业集团做一个特殊的判断.

                #region 是否增加退回的节点
                int returnNode = 0;
                if (gwf.WFState == WFState.ReturnSta && nd.GetParaInt("IsShowReturnNodeInToolbar") == 1)
                {
                    string mysql = "";
                    ReturnWorks returnWorks = new ReturnWorks();
                    QueryObject qo = new QueryObject(returnWorks);
                    qo.AddWhere(ReturnWorkAttr.WorkID, this.WorkID);
                    qo.addAnd();
                    qo.AddWhere(ReturnWorkAttr.ReturnToNode, this.FK_Node);
                    qo.addAnd();
                    qo.AddWhere(ReturnWorkAttr.ReturnToEmp, WebUser.No);
                    qo.addOrderByDesc(ReturnWorkAttr.RDT);
                    qo.DoQuery();
                    if (returnWorks.Count != 0)
                    {
                        ReturnWork returnWork = returnWorks[0] as ReturnWork;
                        dr = dtToNDs.NewRow();
                        dr["No"] = returnWork.ReturnNode;
                        dr["Name"] = returnWork.ReturnNodeName + "(退回)";
                        dr["IsSelected"] = "1";
                        dr["IsSelectEmps"] = "0";
                        dtToNDs.Rows.Add(dr);
                        returnNode = returnWork.ReturnNode;
                        defalutSelectedNodeID = 0;//设置默认。
                    }
                }
                #endregion 是否增加退回的节点.

                foreach (BP.WF.Template.NodeSimple item in nds)
                {
                    if (item.NodeID == returnNode)
                        continue;

                    dr = dtToNDs.NewRow();
                    dr["No"] = item.NodeID;
                    dr["Name"] = item.Name;

                    if (item.HisDeliveryWay == DeliveryWay.BySelected)
                        dr["IsSelectEmps"] = "1";
                    else if (item.HisDeliveryWay == DeliveryWay.BySelfUrl)
                    {
                        dr["IsSelectEmps"] = "2";
                        dr["DeliveryParas"] = item.DeliveryParas;
                    }
                    else if (item.HisDeliveryWay == DeliveryWay.BySelectedEmpsOrgModel)
                        dr["IsSelectEmps"] = "3";
                    else
                        dr["IsSelectEmps"] = "0";  //是不是，可以选择接受人.

                    //设置默认选择的节点.
                    if (defalutSelectedNodeID == item.NodeID)
                        dr["IsSelected"] = "1";
                    else
                        dr["IsSelected"] = "0";

                    dtToNDs.Rows.Add(dr);
                }
                #endregion 到达其他节点。
            }

            return dtToNDs;
        }

        /// <summary>
        /// 获取主表的方法.
        /// </summary>
        /// <returns></returns>
        private Hashtable GetMainTableHT()
        {
            Hashtable htMain = new Hashtable();
            foreach (string key in HttpContextHelper.RequestParamKeys)
            {
                if (key == null)
                    continue;

                string myKey = key;
                string val = HttpContextHelper.RequestParams(key);
                myKey = myKey.Replace("TB_", "");
                myKey = myKey.Replace("DDL_", "");
                myKey = myKey.Replace("CB_", "");
                myKey = myKey.Replace("RB_", "");
                val = HttpUtility.UrlDecode(val, Encoding.UTF8);

                if (htMain.ContainsKey(myKey) == true)
                    htMain[myKey] = val;
                else
                    htMain.Add(myKey, val);
            }
            return htMain;
        }
        /// <summary>
        /// 删除流程
        /// </summary>
        /// <returns></returns>
        public string DeleteFlow()
        {
            try
            {
                string msg = this.GetRequestVal("Msg");
                if (DataType.IsNullOrEmpty(msg) == true)
                    msg = "无";
                DelWorkFlowRole role = (DelWorkFlowRole)this.GetRequestValInt("DelEnable");
                switch (role)
                {
                    case DelWorkFlowRole.DeleteByFlag:
                        return BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(this.WorkID, msg,true);
                    case DelWorkFlowRole.DeleteAndWriteToLog:
                        return BP.WF.Dev2Interface.Flow_DoDeleteFlowByWriteLog(this.FK_Flow, this.WorkID, msg, true);
                    case DelWorkFlowRole.DeleteReal:
                        return BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.WorkID, true);
                }
                return BP.WF.Dev2Interface.Flow_DoDeleteFlowByFlag(this.WorkID, msg, true);
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <returns></returns>
        public string Send()
        {
            try
            {
                Hashtable ht = this.GetMainTableHT();
                SendReturnObjs objs = null;
                string msg = "";

                //判断当前流程工作的GenerWorkFlow是否存在
                GenerWorkFlow gwf = new GenerWorkFlow();
                gwf.WorkID = this.WorkID;
                int i = gwf.RetrieveFromDBSources();
                if (i == 0)
                    return "该流程的工作已删除,请联系管理员.WorkID=" + this.WorkID;

                Int64 workid = this.WorkID;
                //如果包含subWorkID
                Int64 subWorkID = this.GetRequestValInt64("SubWorkID");
                if (subWorkID != 0)
                    workid = subWorkID;

                #region 处理授权人.
                //授权人
                string auther = this.GetRequestVal("Auther");
                if (DataType.IsNullOrEmpty(auther) == false)
                {
                    //  BP.Web.WebUser.IsAuthorize = true;
                    BP.Web.WebUser.Auth = auther;
                    BP.Web.WebUser.AuthName = BP.DA.DBAccess.RunSQLReturnString("SELECT Name FROM Port_Emp WHERE No='" + auther + "'");
                }
                else
                {
                    // BP.Web.WebUser.IsAuthorize = true;
                    BP.Web.WebUser.Auth = "";
                    BP.Web.WebUser.AuthName = ""; // BP.DA.DBAccess.RunSQLReturnString("SELECT Name FROM Port_Emp WHERE No='" + auther + "'");
                }
                #endregion 处理授权人.



                objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, workid, ht, null,
                    this.ToNode, null, WebUser.No, WebUser.Name, WebUser.FK_Dept,
                    WebUser.FK_DeptName, null, this.FID, this.PWorkID,
                    this.GetRequestValBoolen("IsReturnNode"));

                msg = objs.ToMsgOfHtml();
                BP.WF.Glo.SessionMsg = msg;

                #region 处理授权 
                if (DataType.IsNullOrEmpty(auther) == false)
                {
                    gwf = new GenerWorkFlow(this.WorkID);
                    gwf.SetPara("Auth", BP.Web.WebUser.AuthName + "授权");
                    gwf.Update();
                }
                #endregion 处理授权 


                //当前节点.
                Node currNode = new Node(this.FK_Node);

                #region 处理发送后转向.
                /*处理转向问题.*/
                switch (currNode.HisTurnToDeal)
                {
                    case TurnToDeal.SpecUrl:
                        string myurl = currNode.TurnToDealDoc.Clone().ToString();
                        if (myurl.Contains("?") == false)
                            myurl += "?1=1";
                        Attrs myattrs = currNode.HisWork.EnMap.Attrs;
                        Work hisWK = currNode.HisWork;
                        foreach (Attr attr in myattrs)
                        {
                            if (myurl.Contains("@") == false)
                                break;
                            myurl = myurl.Replace("@" + attr.Key, hisWK.GetValStrByKey(attr.Key));
                        }
                        myurl = myurl.Replace("@WebUser.No", BP.Web.WebUser.No);
                        myurl = myurl.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        myurl = myurl.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);

                        if (myurl.Contains("@"))
                        {
                            BP.WF.Dev2Interface.Port_SendMsg("admin", currFlow.Name + "在" + currND.Name + "节点处，出现错误", "流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl, "Err" + currND.No + "_" + this.WorkID, SMSMsgType.Err, this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
                            throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl);
                        }

                        if (myurl.Contains("&WorkID") == false)
                            myurl += "&WorkID=" + this.WorkID;

                        if (myurl.Contains("&PWorkID") == false)
                            myurl += "&PWorkID=" + this.PWorkID;


                        myurl += "&FromFlow=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&UserNo=" + WebUser.No + "&Token=" + WebUser.Token;
                        return "TurnUrl@" + myurl;

                    case TurnToDeal.TurnToByCond:
                        //TurnTos tts = new TurnTos(this.FK_Flow);
                        //if (tts.Count == 0)
                        //{
                        //    BP.WF.Dev2Interface.Port_SendMsg("admin", currFlow.Name + "在" + currND.Name + "节点处，出现错误", "您没有设置节点完成后的转向条件。", "Err" + currND.No + "_" + this.WorkID, SMSMsgType.Err, this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
                        //    throw new Exception("@您没有设置节点完成后的转向条件。");
                        //}

                        //foreach (TurnTo tt in tts)
                        //{
                        //    tt.HisWork = currNode.HisWork;
                        //    if (tt.IsPassed == true)
                        //    {
                        //        string url = tt.TurnToURL.Clone().ToString();
                        //        if (url.Contains("?") == false)
                        //            url += "?1=1";
                        //        Attrs attrs = currNode.HisWork.EnMap.Attrs;
                        //        Work hisWK1 = currNode.HisWork;
                        //        foreach (Attr attr in attrs)
                        //        {
                        //            if (url.Contains("@") == false)
                        //                break;
                        //            url = url.Replace("@" + attr.Key, hisWK1.GetValStrByKey(attr.Key));
                        //        }
                        //        if (url.Contains("@"))
                        //            throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + url);

                        //        url += "&PFlowNo=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&PWorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&Token=" + WebUser.SID;
                        //        return "url@" + url;
                        //    }
                        //}
                        return msg;
                    default:
                        msg = msg.Replace("@WebUser.No", BP.Web.WebUser.No);
                        msg = msg.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        msg = msg.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                        return msg;
                }
                #endregion

            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("url@") == 0)
                    return ex.Message;

                //清楚上次选择的节点信息.
                if (DataType.IsNullOrEmpty(this.HisGenerWorkFlow.Paras_ToNodes) == false)
                {
                    this.HisGenerWorkFlow.Paras_ToNodes = "";
                    this.HisGenerWorkFlow.Update();
                }

                if (ex.Message.Contains("请选择下一步骤工作") == true || ex.Message.Contains("用户没有选择发送到的节点") == true)
                {
                    if (this.currND.CondModel == DirCondModel.ByDDLSelected
                        || this.currND.CondModel == DirCondModel.ByButtonSelected)
                    {
                        /*如果抛出异常，我们就让其转入选择到达的节点里, 在节点里处理选择人员. */
                        return "SelectNodeUrl@./WorkOpt/ToNodes.htm?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID;

                    }

                    //if (this.currND.CondModel != CondModel.SendButtonSileSelect)
                    //{
                    //    currND.CondModel = CondModel.SendButtonSileSelect;
                    //    currND.Update();
                    //}

                    return "err@下一个节点的接收人规则是，当前节点选择来选择，在当前节点属性里您没有启动接受人按钮，系统自动帮助您启动了，请关闭窗口重新打开。" + ex.Message;
                }

                //绑定独立表单，表单自定义方案验证错误弹出窗口进行提示.
                if (ex.Message.Contains("提交前检查到如下必填字段填写不完整") == true || ex.Message.Contains("您没有上传附件") == true || ex.Message.Contains("您没有上传图片附件") == true)
                {
                    return "err@" + ex.Message.Replace("@@", "@").Replace("@", "<BR>@");
                }

                //防止发送失败丢失接受人，导致不能出现下拉方向选择框. @杜.
                if (this.HisGenerWorkFlow != null)
                {
                    //如果是会签状态.
                    if (this.HisGenerWorkFlow.HuiQianTaskSta == HuiQianTaskSta.HuiQianing)
                    {
                        //如果是主持人.
                        if (this.HisGenerWorkFlow.HuiQianZhuChiRen == WebUser.No)
                        {
                            if (this.HisGenerWorkFlow.TodoEmps.Contains(BP.Web.WebUser.No + ",") == false)
                            {
                                this.HisGenerWorkFlow.TodoEmps += WebUser.No + "," + BP.Web.WebUser.Name + ";";
                                this.HisGenerWorkFlow.Update();
                            }
                        }
                        else
                        {
                            //非主持人.
                            string empStr = BP.Web.WebUser.No + "," + BP.Web.WebUser.Name + ";";
                            if (this.HisGenerWorkFlow.TodoEmps.Contains(empStr) == false && WebUser.No.Equals("Guest") == false)
                            {
                                this.HisGenerWorkFlow.TodoEmps += empStr; // BP.Web.WebUser.No +","+BP.Web.WebUser.Name + ";";
                                this.HisGenerWorkFlow.Update();
                            }
                        }
                    }


                    if (this.HisGenerWorkFlow.HuiQianTaskSta != HuiQianTaskSta.HuiQianing)
                    {
                        string empStr = BP.Web.WebUser.No + "," + BP.Web.WebUser.Name + ";";
                        if (this.HisGenerWorkFlow.TodoEmps.Contains(empStr) == false)
                        {
                            this.HisGenerWorkFlow.TodoEmps += empStr;
                            this.HisGenerWorkFlow.Update();
                        }
                    }
                }

                //如果错误，就写标记.
                string msg = ex.Message;
                if (msg.IndexOf("err@") == -1 && msg.IndexOf("url@") != 0)
                    msg = "err@" + msg;
                return msg;
            }
        }
        /// <summary>
        /// 批量发送
        /// </summary>
        /// <returns></returns>
        public string StartGuide_MulitSend()
        {
            //获取设置的数据源
            Flow fl = new Flow(this.FK_Flow);
            string key = this.GetRequestVal("Key");
            string SKey = this.GetRequestVal("Keys");
            string sql = "";
            //判断是否有查询条件
            sql = fl.StartGuidePara2.Clone() as string;
            if (!string.IsNullOrWhiteSpace(key))
            {
                sql = fl.StartGuidePara1.Clone() as string;
                sql = sql.Replace("@Key", key);
            }
            //替换变量
            sql = sql.Replace("~", "'");
            sql = sql.Replace("@WebUser.No", WebUser.No);
            sql = sql.Replace("@WebUser.Name", WebUser.Name);
            sql = sql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);
            sql = sql.Replace("@WebUser.FK_DeptName", WebUser.FK_DeptName);

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            //获取选中的数据源
            DataRow[] drArr = dt.Select("No in(" + SKey.TrimEnd(',') + ")");

            //获取Nos
            string Nos = "";
            for (int i = 0; i < drArr.Length; i++)
            {
                DataRow row = drArr[i];
                Nos += row["No"] + ",";
            }
            return Nos.TrimEnd(',');
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string Save()
        {
            try
            {
                string str = BP.WF.Dev2Interface.Node_SaveWork(this.FK_Flow, this.FK_Node,
                    this.WorkID, this.GetMainTableHT(), null, this.FID, this.PWorkID);

                if (this.PWorkID != 0)
                {
                    GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                    BP.WF.Dev2Interface.SetParentInfo(this.FK_Flow, this.WorkID, this.PWorkID, gwf.PEmp, gwf.PNodeID);
                }
                return str;
            }
            catch (Exception ex)
            {
                return "err@保存失败:" + ex.Message;
            }
        }
        public string MyFlowSelfForm_Init()
        {
            return this.GenerWorkNode();
        }

        public string SaveFlow_ToDraftRole()
        {

            Node nd = new Node(this.FK_Node);
            Work wk = nd.HisWork;
            if (this.WorkID != 0)
            {
                wk.OID = this.WorkID;
                wk.RetrieveFromDBSources();
            }

            //获取表单树的数据
            BP.WF.WorkNode workNode = new WorkNode(this.WorkID, this.FK_Node);
            Work treeWork = workNode.CopySheetTree();
            if (treeWork != null)
            {
                wk.Copy(treeWork);
                wk.Update();
            }

            //获取该节点是是否是绑定表单方案, 如果流程节点中的字段与绑定表单的字段相同时赋值 
            //if (nd.FormType == NodeFormType.SheetTree || nd.FormType == NodeFormType.RefOneFrmTree)
            //{
            //    FrmNodes nds = new FrmNodes(this.FK_Flow, this.FK_Node);
            //    foreach (FrmNode item in nds)
            //    {
            //        if (item.FrmEnableRole == FrmEnableRole.Disable)
            //            continue;
            //        if (item.FK_Frm.Equals("ND"+this.FK_Node) == true)
            //            continue;
            //        GEEntity en = null;
            //        try
            //        {
            //            en = new GEEntity(item.FK_Frm);
            //            en.PKVal = this.WorkID;
            //            if (en.RetrieveFromDBSources() == 0)
            //            {
            //                continue;
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            continue;
            //        }

            //        Attrs frmAttrs = en.EnMap.Attrs;
            //        Attrs wkAttrs = wk.EnMap.Attrs;
            //        foreach (Attr wkattr in wkAttrs)
            //        {
            //            if (wkattr.Key.Equals(GERptAttr.OID) || wkattr.Key.Equals(GERptAttr.FID) || wkattr.Key.Equals(GERptAttr.CDT)
            //                || wkattr.Key.Equals(GERptAttr.RDT) || wkattr.Key.Equals(GERptAttr.MD5) || wkattr.Key.Equals(GERptAttr.Emps)
            //                || wkattr.Key.Equals(GERptAttr.FK_Dept) || wkattr.Key.Equals(GERptAttr.PRI) || wkattr.Key.Equals(GERptAttr.Rec)
            //                || wkattr.Key.Equals(GERptAttr.Title) || wkattr.Key.Equals(Data.GERptAttr.FK_NY) || wkattr.Key.Equals(Data.GERptAttr.FlowEmps)
            //                || wkattr.Key.Equals(Data.GERptAttr.FlowStarter) || wkattr.Key.Equals(Data.GERptAttr.FlowStartRDT) || wkattr.Key.Equals(Data.GERptAttr.WFState))
            //            {
            //                continue;
            //            }

            //            foreach (Attr attr in frmAttrs)
            //            {
            //                if (wkattr.Key.Equals(attr.Key))
            //                {
            //                    wk.SetValByKey(wkattr.Key, en.GetValStrByKey(attr.Key));
            //                    break;
            //                }

            //            }

            //        }

            //    }
            //    wk.Update();
            //}

            #region 为开始工作创建待办.
            if (nd.IsStartNode == true)
            {
                GenerWorkFlow gwf = new GenerWorkFlow();
                Flow fl = new Flow(this.FK_Flow);
                if (fl.DraftRole == DraftRole.None && this.GetRequestValInt("SaveType") != 1)
                    return "保存成功";

                //规则设置为写入待办，将状态置为运行中，其他设置为草稿.
                WFState wfState = WFState.Blank;
                if (fl.DraftRole == DraftRole.SaveToDraftList)
                    wfState = WFState.Draft;
                if (fl.DraftRole == DraftRole.SaveToTodolist)
                    wfState = WFState.Runing;

                //设置标题.
                string title = BP.WF.WorkFlowBuessRole.GenerTitle(fl, wk);

                //修改RPT表的标题
                wk.SetValByKey(BP.WF.GERptAttr.Title, title);
                wk.Update();

                gwf.WorkID = this.WorkID;
                int count = gwf.RetrieveFromDBSources();

                gwf.Title = title; //标题.
                if (count == 0)
                {
                    gwf.FlowName = fl.Name;
                    gwf.FK_Flow = this.FK_Flow;
                    gwf.FK_FlowSort = fl.FK_FlowSort;
                    gwf.SysType = fl.SysType;

                    gwf.FK_Node = this.FK_Node;
                    gwf.NodeName = nd.Name;
                    gwf.WFState = wfState;

                    gwf.FK_Dept = WebUser.FK_Dept;
                    gwf.DeptName = WebUser.FK_DeptName;
                    gwf.Starter = WebUser.No;
                    gwf.StarterName = WebUser.Name;
                    gwf.RDT = DataType.CurrentDateTimess;
                    gwf.Insert();

                    // 产生工作列表.
                    GenerWorkerList gwl = new GenerWorkerList();
                    gwl.WorkID = this.WorkID;
                    gwl.FK_Emp = WebUser.No;
                    gwl.FK_EmpText = WebUser.Name;

                    gwl.FK_Node = gwf.FK_Node;
                    gwl.FK_NodeText = nd.Name;
                    gwl.FID = 0;

                    gwl.FK_Flow = gwf.FK_Flow;
                    gwl.FK_Dept = WebUser.FK_Dept;
                    gwl.FK_DeptT = WebUser.FK_DeptName;

                    gwl.SDT = "无";
                    gwl.DTOfWarning = DataType.CurrentDateTimess;
                    gwl.IsEnable = true;

                    gwl.IsPass = false;
                    //  gwl.Sender = WebUser.No;
                    gwl.PRI = gwf.PRI;
                    gwl.Insert();
                }
                else
                {
                    gwf.WFState = wfState;
                    gwf.DirectUpdate();
                }

            }
            #endregion 为开始工作创建待办

            return "保存到待办";
        }

        #region 表单树操作
        public string FlowFormTree2021_Init()
        {
            //树形表单的类别
            FlowFormTrees formTree = new FlowFormTrees();

            //表单
            FlowFormTrees forms = new FlowFormTrees();

            FlowFormTree root = new FlowFormTree();
            root.No = "1";
            root.ParentNo = "0";
            root.Name = "目录";
            root.NodeType = "root";

            #region 添加表单及文件夹
            //当前节点绑定的表单集合
            FrmNodes frmNodes = new FrmNodes();
            frmNodes.Retrieve(FrmNodeAttr.FK_Node, this.FK_Node, FrmNodeAttr.Idx);

            //所有表单集合信息
            MapDatas mds = new MapDatas();
            mds.RetrieveInSQL("SELECT FK_Frm FROM WF_FrmNode WHERE FK_Node=" + this.FK_Node);

            #region 检查是否有没有目录的表单?
            bool isHave = false;
            string treeNo = "";
            foreach (MapData md in mds)
            {
                if (DataType.IsNullOrEmpty(md.FK_FormTree) == true)
                    isHave = true;
                if (DataType.IsNullOrEmpty(md.FK_FormTree) == false)
                    treeNo = md.FK_FormTree;
            }
            if (isHave == true && DataType.IsNullOrEmpty(treeNo) == true)
            {
                treeNo = "1";
                formTree.AddEntity(root);
            }
            #endregion 检查是否有没有目录的表单?

            //从外部参数获取.
            string frms = HttpContextHelper.RequestParams("Frms");
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (DataType.IsNullOrEmpty(frms) == true)
            {
                frms = gwf.Paras_Frms;
            }
            else
            {
                gwf.Paras_Frms = frms;
                gwf.Update();
            }

            //如果有参数.
            if (DataType.IsNullOrEmpty(frms) == false)
            {
                frms = frms.Trim();
                frms = "," + frms + ","; //特殊处理
                frms = frms.Replace(" ", "");
                frms = frms.Replace(" ", "");
                frms = frms.Replace(" ", "");

                string[] strs = frms.Split(',');
                isHave = false; //检查是不否存在。
                foreach (string str in strs)
                {
                    if (DataType.IsNullOrEmpty(str) == true)
                        continue;

                    //如果集合包含参数里面的表单，就不处理.
                    if (frmNodes.Contains(FrmNodeAttr.FK_Frm, str) == true)
                        continue;

                    //把有参数的表单插入到数据库里.
                    FrmNode fn = new FrmNode();
                    fn.FK_Frm = str;
                    fn.FK_Node = gwf.FK_Node;
                    fn.FK_Flow = gwf.FK_Flow;
                    fn.FrmEnableRole = FrmEnableRole.WhenHaveFrmPara; //设置有参数的时候启用.
                    fn.MyPK = str + "_" + gwf.FK_Node + "_" + gwf.FK_Flow;

                    if (fn.FK_Node.ToString().EndsWith("01") == true)
                        fn.FrmSln = FrmSln.Default; //设置编辑方案, 为默认方案.
                    else
                        fn.FrmSln = FrmSln.Readonly; //设置编辑方案, 为默认方案.


                    fn.Insert();
                    isHave = true; //标记存在，用于更新查询的数据源.
                }
                if (isHave == true)
                {
                    frmNodes.Retrieve(FrmNodeAttr.FK_Node, this.FK_Node, FrmNodeAttr.Idx);
                    mds.RetrieveInSQL("SELECT FK_Frm FROM WF_FrmNode WHERE FK_Node=" + this.FK_Node);
                }

            }

            //求出来要显示的表单集合.
            foreach (FrmNode frmNode in frmNodes)
            {
                #region 增加判断是否启用规则.
                switch (frmNode.FrmEnableRole)
                {
                    case FrmEnableRole.Allways:
                        break;
                    case FrmEnableRole.WhenHaveData: //判断是否有数据.
                        MapData mapData = mds.GetEntityByKey(frmNode.FK_Frm) as MapData;
                        if (mapData == null)
                            continue;
                        Int64 pk = this.WorkID;
                        switch (frmNode.WhoIsPK)
                        {
                            case WhoIsPK.FID:
                                pk = this.FID;
                                break;
                            case WhoIsPK.PWorkID:
                                pk = this.PWorkID;
                                break;
                            case WhoIsPK.CWorkID:
                                pk = this.CWorkID;
                                break;
                            case WhoIsPK.OID:
                            default:
                                pk = this.WorkID;
                                break;
                        }
                        if (DBAccess.RunSQLReturnValInt("SELECT COUNT(*) as Num FROM " + mapData.PTable + " WHERE OID=" + pk) == 0)
                            continue;
                        break;
                    case FrmEnableRole.WhenHaveFrmPara: //判断是否有参数.
                        if (DataType.IsNullOrEmpty(frms) == true)
                            continue;
                        if (frms.Contains("," + frmNode.FK_Frm + ",") == false)
                            continue;
                        break;
                    case FrmEnableRole.ByFrmFields:
                        throw new Exception("@这种类型的判断，ByFrmFields 还没有完成。");
                    case FrmEnableRole.BySQL: // 按照SQL的方式.
                        string mysql = frmNode.FrmEnableExp.Clone() as string;
                        if (DataType.IsNullOrEmpty(mysql) == true)
                        {
                            MapData FrmMd = mds.GetEntityByKey(frmNode.FK_Frm) as MapData;
                            return "err@表单" + frmNode.FK_Frm + ",[" + FrmMd.Name + "]在节点[" + frmNode.FK_Node + "]启用方式按照sql启用但是您没有给他设置sql表达式.";
                        }

                        mysql = mysql.Replace("@OID", this.WorkID.ToString());
                        mysql = mysql.Replace("@WorkID", this.WorkID.ToString());

                        mysql = mysql.Replace("@NodeID", this.FK_Node.ToString());
                        mysql = mysql.Replace("@FK_Node", this.FK_Node.ToString());

                        mysql = mysql.Replace("@FK_Flow", this.FK_Flow);

                        mysql = mysql.Replace("@WebUser.No", WebUser.No);
                        mysql = mysql.Replace("@WebUser.Name", WebUser.Name);
                        mysql = mysql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);

                        //替换特殊字符.
                        mysql = mysql.Replace("~", "'");

                        if (DBAccess.RunSQLReturnValFloat(mysql) <= 0)
                            continue;
                        break;

                    case FrmEnableRole.ByStation://当前人员包含这个岗位
                        string exp = frmNode.FrmEnableExp.Clone() as string;
                        string Sql = "SELECT FK_Station FROM Port_DeptEmpStation where FK_Emp='" + WebUser.No + "'";
                        string station = DBAccess.RunSQLReturnString(Sql);
                        if (DataType.IsNullOrEmpty(station) == true)
                            continue;
                        string[] stations = station.Split(';');
                        bool isExit = false;
                        foreach (string s in stations)
                        {
                            if (exp.Contains(s) == true)
                            {
                                isExit = true;
                                break;
                            }
                        }
                        if (isExit == false)
                            continue;
                        break;

                    case FrmEnableRole.ByDept:
                        exp = frmNode.FrmEnableExp.Clone() as string;
                        Sql = "SELECT FK_Dept FROM Port_DeptEmp where FK_Emp='" + WebUser.No + "'";
                        string dept = DBAccess.RunSQLReturnString(Sql);
                        if (DataType.IsNullOrEmpty(dept) == true)
                            continue;
                        string[] deptStrs = dept.Split(';');
                        isExit = false;
                        foreach (string s in deptStrs)
                        {
                            if (exp.Contains(s) == true)
                            {
                                isExit = true;
                                break;
                            }
                        }
                        if (isExit == false)
                            continue;

                        break;
                    case FrmEnableRole.Disable: // 如果禁用了，就continue出去..
                        continue;
                    default:
                        throw new Exception("err@没有判断的规则." + frmNode.FrmEnableRole);
                }
                #endregion

                MapData md = mds.GetEntityByKey(frmNode.FK_Frm) as MapData;
                if (md == null)
                {
                    frmNode.Delete();
                    continue;
                }

                if (DataType.IsNullOrEmpty(md.FK_FormTree) == true)
                    md.FK_FormTree = treeNo;

                //增加目录.
                if (formTree.Contains("Name", md.FK_FormTreeText) == false)
                {
                    BP.WF.Template.FlowFormTree nodeFolder = new BP.WF.Template.FlowFormTree();
                    nodeFolder.No = md.FK_FormTree;
                    nodeFolder.ParentNo = "1";
                    nodeFolder.Name = md.FK_FormTreeText;
                    nodeFolder.NodeType = "folder";
                    formTree.AddEntity(nodeFolder);
                }

                //检查必填项.
                bool IsNotNull = false;
                FrmFields formFields = new FrmFields();
                QueryObject obj = new QueryObject(formFields);
                obj.AddWhere(FrmFieldAttr.FK_Node, this.FK_Node);
                obj.addAnd();
                obj.AddWhere(FrmFieldAttr.FK_MapData, md.No);
                obj.addAnd();
                obj.AddWhere(FrmFieldAttr.IsNotNull, 1);
                obj.DoQuery();
                if (formFields != null && formFields.Count > 0)
                    IsNotNull = true;

                BP.WF.Template.FlowFormTree nodeForm = new BP.WF.Template.FlowFormTree();
                nodeForm.No = md.No;
                nodeForm.ParentNo = md.FK_FormTree;

                //设置他的表单显示名字. 2019.09.30
                string frmName = md.Name;
                FrmNode fn = frmNodes.GetEntityByKey(FrmNodeAttr.FK_Frm, md.No) as FrmNode;
                if (fn != null)
                {
                    string str = fn.FrmNameShow;
                    if (DataType.IsNullOrEmpty(str) == false)
                        frmName = str;
                }
                nodeForm.Name = frmName;
                nodeForm.ICON = md.ICON;
                nodeForm.NodeType = IsNotNull ? "form|1" : "form|0";
                nodeForm.IsEdit = frmNode.IsEditInt.ToString();// Convert.ToString(Convert.ToInt32(frmNode.IsEdit));
                nodeForm.IsCloseEtcFrm = frmNode.IsCloseEtcFrmInt.ToString();
                forms.AddEntity(nodeForm);
            }
            #endregion

            DataSet ds = new DataSet();
            ds.Tables.Add(formTree.ToDataTableField("FormTree"));
            DataTable formdt = forms.ToDataTableField("Forms");
            formdt.Columns.Add("ICON");
            for (int i = 0; i < forms.Count; i++)
            {
                if (DataType.IsNullOrEmpty(forms[i].ICON) == true)
                    formdt.Rows[i]["ICON"] = "icon-doc";
                else
                    formdt.Rows[i]["ICON"] = forms[i].ICON;
            }

            ds.Tables.Add(formdt);
            ds.Tables.Add(gwf.ToDataTableField("WF_GenerWorkFlow"));

            #region 处理流程-消息提示.
            bool isReadonly = this.GetRequestValBoolen("IsReadonly");
            if (isReadonly == true)
                return BP.Tools.Json.ToJson(ds);

            DataTable dtAlert = new DataTable();
            dtAlert.TableName = "AlertMsg";
            dtAlert.Columns.Add("Title", typeof(string));
            dtAlert.Columns.Add("Msg", typeof(string));
            dtAlert.Columns.Add("URL", typeof(string));
            DataRow drMsg = null;
            string sql = "";
            int sta = gwf.GetParaInt("HungupSta");
            switch (gwf.WFState)
            {
                case WFState.Runing:
                    drMsg = dtAlert.NewRow();
                    drMsg["Title"] = "挂起信息";
                    if (sta == 2 && gwf.FK_Node == gwf.GetParaInt("HungupNodeID"))
                    {
                        drMsg["Msg"] = "您的工单在挂起被拒绝，拒绝原因:" + gwf.GetParaString("HungupCheckMsg");
                        dtAlert.Rows.Add(drMsg);
                    }
                    break;
                case WFState.Hungup:
                    if (sta == -1)
                        break;
                    drMsg = dtAlert.NewRow();
                    drMsg["Title"] = "挂起信息";
                    if (sta == 0)
                        drMsg["Msg"] = "您的工单在挂起状态，等待审批，挂起原因：" + gwf.GetParaString("HungupNote");

                    if (sta == 1)
                        drMsg["Msg"] = "您的工单在挂起获得同意.";

                    if (sta == 2)
                        drMsg["Msg"] = "您的工单在挂起被拒绝，拒绝原因:" + gwf.GetParaString("HungupCheckMsg");

                    dtAlert.Rows.Add(drMsg);
                    break;
                case WFState.AskForReplay: // 返回加签的信息.
                    string mysql = "SELECT Msg,EmpFrom,EmpFromT,RDT FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE WorkID=" + this.WorkID + " AND " + TrackAttr.ActionType + "=" + (int)ActionType.ForwardAskfor;
                    DataTable mydt = DBAccess.RunSQLReturnTable(mysql);

                    foreach (DataRow dr in mydt.Rows)
                    {
                        string msgAskFor = dr[0].ToString();
                        string worker = dr[1].ToString();
                        string workerName = dr[2].ToString();
                        string rdt = dr[3].ToString();

                        drMsg = dtAlert.NewRow();
                        drMsg["Title"] = worker + "," + workerName + "回复信息:";
                        drMsg["Msg"] = DataType.ParseText2Html(msgAskFor) + "<br>" + rdt;
                        dtAlert.Rows.Add(drMsg);
                    }
                    break;
                case WFState.Askfor: //加签.

                    sql = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE WorkID=" + this.WorkID + " AND " + TrackAttr.ActionType + "=" + (int)ActionType.AskforHelp;
                    DataTable dt = DBAccess.RunSQLReturnTable(sql);
                    foreach (DataRow dr in dt.Rows)
                    {
                        string msgAskFor = dr[TrackAttr.Msg].ToString();
                        string worker = dr[TrackAttr.EmpFrom].ToString();
                        string workerName = dr[TrackAttr.EmpFromT].ToString();
                        string rdt = dr[TrackAttr.RDT].ToString();

                        drMsg = dtAlert.NewRow();
                        drMsg["Title"] = worker + "," + workerName + "请求加签:";
                        drMsg["Msg"] = DataType.ParseText2Html(msgAskFor) + "<br>" + rdt + "<a href='./WorkOpt/AskForRe.htm?FK_Flow=" + this.FK_Flow + "&FK_Node=" + gwf.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + gwf.FID + "' >回复加签意见</a> --";
                        dtAlert.Rows.Add(drMsg);

                    }
                    break;
                case WFState.ReturnSta:
                    /* 如果工作节点退回了*/
                    Node nd = new Node(this.FK_Node);
                    ReturnWorks ens = new ReturnWorks();
                    if (nd.HisRunModel == RunModel.FL || nd.HisRunModel == RunModel.FHL)
                    {
                        QueryObject qo = new QueryObject(ens);
                        qo.addLeftBracket();
                        qo.AddWhere(ReturnWorkAttr.WorkID, this.WorkID);
                        qo.addOr();
                        qo.AddWhere(ReturnWorkAttr.FID, this.WorkID);
                        qo.addRightBracket();
                        qo.addAnd();
                        qo.AddWhere(ReturnWorkAttr.ReturnToNode, nd.NodeID);
                        qo.addOrderBy("RDT");
                        qo.DoQuery();
                    }
                    else
                    {
                        ens.Retrieve(ReturnWorkAttr.WorkID, this.WorkID, ReturnWorkAttr.ReturnToNode, nd.NodeID, "RDT");
                    }


                    string msgInfo = "";
                    foreach (ReturnWork rw in ens)
                    {
                        if (rw.ReturnToEmp.Contains(WebUser.No) == true)
                        {
                            msgInfo += "来自节点：" + rw.ReturnNodeName + "@退回人：" + rw.ReturnerName + "@退回日期：" + rw.RDT;
                            msgInfo += "@退回原因：" + rw.BeiZhu;
                            msgInfo += "<hr/>";
                        }
                    }

                    msgInfo = msgInfo.Replace("@", "<br>");
                    if (string.IsNullOrEmpty(msgInfo) == false)
                    {
                        string str = nd.ReturnAlert;
                        if (str != "")
                        {
                            str = str.Replace("~", "'");
                            str = str.Replace("@PWorkID", this.WorkID.ToString());
                            str = str.Replace("@PNodeID", nd.NodeID.ToString());
                            str = str.Replace("@FK_Node", nd.NodeID.ToString());

                            str = str.Replace("@PFlowNo", gwf.FK_Flow);
                            str = str.Replace("@FK_Flow", gwf.FK_Flow);
                            str = str.Replace("@PWorkID", this.WorkID.ToString());

                            str = str.Replace("@WorkID", this.WorkID.ToString());
                            str = str.Replace("@OID", this.WorkID.ToString());

                            drMsg = dtAlert.NewRow();
                            drMsg["Title"] = "退回信息";
                            drMsg["Msg"] = msgInfo + "\t\n" + str;
                            dtAlert.Rows.Add(drMsg);
                        }
                        else
                        {
                            drMsg = dtAlert.NewRow();
                            drMsg["Title"] = "退回信息";
                            drMsg["Msg"] = msgInfo + "\t\n" + str;
                            dtAlert.Rows.Add(drMsg);
                        }
                    }
                    break;
                case WFState.Shift:
                    /* 判断移交过来的。 */
                    string sqlshift = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE ACTIONTYPE=3 AND WorkID=" + this.WorkID + " AND NDFrom='" + gwf.FK_Node + "' ORDER BY RDT DESC ";
                    DataTable dtshift = DBAccess.RunSQLReturnTable(sqlshift);
                    string msg = "";
                    if (dtshift.Rows.Count >= 1)
                    {
                        msg = "";
                        drMsg = dtAlert.NewRow();
                        drMsg["Title"] = "移交信息";
                        //  msg = "<h3>移交信息 </h3><hr/>";
                        foreach (DataRow dr in dtshift.Rows)
                        {
                            if (WebUser.No == dr[TrackAttr.EmpTo].ToString())
                            {
                                string empFromT = dr[TrackAttr.EmpFromT].ToString();
                                string empToT = dr[TrackAttr.EmpToT].ToString();
                                string msgShift = dr[TrackAttr.Msg].ToString();
                                string rdt = dr[TrackAttr.RDT].ToString();
                                if (msgShift == "undefined")
                                    msgShift = "无";

                                msg += "移交人：" + empFromT + "@接受人：" + empToT + "@移交日期：" + rdt;
                                msg += "@移交原因：" + msgShift;
                                msg += "<hr/>";
                            }
                        }

                        msg = msg.Replace("@", "<br>");

                        drMsg["Msg"] = msg;
                        if (!string.IsNullOrEmpty(msg))
                        {
                            dtAlert.Rows.Add(drMsg);
                        }
                    }
                    break;
                default:
                    break;
            }
            ds.Tables.Add(dtAlert);
            #endregion 处理流程-消息提示.

            return BP.Tools.Json.ToJson(ds);
        }



        /// <summary>
        /// 获取表单树数据
        /// </summary>
        /// <returns></returns>
        public string FlowFormTree_Init()
        {
            BP.WF.Template.FlowFormTrees appFlowFormTree = new FlowFormTrees();

            //add root
            BP.WF.Template.FlowFormTree root = new BP.WF.Template.FlowFormTree();
            root.No = "1";
            root.ParentNo = "0";
            root.Name = "目录";
            root.NodeType = "root";
            appFlowFormTree.AddEntity(root);

            #region 添加表单及文件夹

            //节点表单
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);

            FrmNodes frmNodes = new FrmNodes();
            frmNodes.Retrieve(FrmNodeAttr.FK_Node, this.FK_Node, FrmNodeAttr.Idx);

            //文件夹
            //SysFormTrees formTrees = new SysFormTrees();
            //formTrees.RetrieveAll(SysFormTreeAttr.Name);

            //所有表单集合. 为了优化效率,这部分重置了一下.
            MapDatas mds = new MapDatas();
            if (frmNodes.Count <= 3)
            {
                foreach (FrmNode fn in frmNodes)
                {
                    MapData md = new MapData(fn.FK_Frm);
                    mds.AddEntity(md);
                }
            }
            else
            {
                mds.RetrieveInSQL("SELECT FK_Frm FROM WF_FrmNode WHERE FK_Node=" + this.FK_Node);
            }


            string frms = HttpContextHelper.RequestParams("Frms");
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (DataType.IsNullOrEmpty(frms) == true)
                frms = gwf.Paras_Frms;
            else
            {
                gwf.Paras_Frms = frms;
                gwf.Update();
            }

            foreach (FrmNode frmNode in frmNodes)
            {
                #region 增加判断是否启用规则.
                switch (frmNode.FrmEnableRole)
                {
                    case FrmEnableRole.Allways:
                        break;
                    case FrmEnableRole.WhenHaveData: //判断是否有数据.
                        MapData md = mds.GetEntityByKey(frmNode.FK_Frm) as MapData;
                        if (md == null)
                            continue;
                        Int64 pk = this.WorkID;
                        switch (frmNode.WhoIsPK)
                        {
                            case WhoIsPK.FID:
                                pk = this.FID;
                                break;
                            case WhoIsPK.PWorkID:
                                pk = this.PWorkID;
                                break;
                            case WhoIsPK.CWorkID:
                                pk = this.CWorkID;
                                break;
                            case WhoIsPK.OID:
                            default:
                                pk = this.WorkID;
                                break;
                        }
                        if (DBAccess.RunSQLReturnValInt("SELECT COUNT(*) as Num FROM " + md.PTable + " WHERE OID=" + pk) == 0)
                            continue;
                        break;
                    case FrmEnableRole.WhenHaveFrmPara: //判断是否有参数.

                        frms = frms.Trim();
                        frms = frms.Replace(" ", "");
                        frms = frms.Replace(" ", "");

                        if (DataType.IsNullOrEmpty(frms) == true)
                            continue;

                        if (frms.Contains(",") == false && frms.Equals(frmNode.FK_Frm) == false)
                            continue;

                        if (frms.Contains(",") == true && frms.Contains(frmNode.FK_Frm + ",") == false)
                            continue;

                        break;
                    case FrmEnableRole.ByFrmFields:
                        throw new Exception("@这种类型的判断，ByFrmFields 还没有完成。");

                    case FrmEnableRole.BySQL: // 按照SQL的方式.
                        string mysql = frmNode.FrmEnableExp.Clone() as string;

                        if (DataType.IsNullOrEmpty(mysql) == true)
                        {
                            MapData FrmMd = new MapData(frmNode.FK_Frm);
                            return "err@表单" + frmNode.FK_Frm + ",[" + FrmMd.Name + "]在节点[" + frmNode.FK_Node + "]启用方式按照sql启用但是您没有给他设置sql表达式.";
                        }


                        mysql = mysql.Replace("@OID", this.WorkID.ToString());
                        mysql = mysql.Replace("@WorkID", this.WorkID.ToString());

                        mysql = mysql.Replace("@NodeID", this.FK_Node.ToString());
                        mysql = mysql.Replace("@FK_Node", this.FK_Node.ToString());

                        mysql = mysql.Replace("@FK_Flow", this.FK_Flow);

                        mysql = mysql.Replace("@WebUser.No", WebUser.No);
                        mysql = mysql.Replace("@WebUser.Name", WebUser.Name);
                        mysql = mysql.Replace("@WebUser.FK_Dept", WebUser.FK_Dept);


                        //替换特殊字符.
                        mysql = mysql.Replace("~", "'");

                        if (DBAccess.RunSQLReturnValFloat(mysql) <= 0)
                            continue;
                        break;

                    case FrmEnableRole.ByStation:
                        string exp = frmNode.FrmEnableExp;
                        string Sql = "SELECT FK_Station FROM Port_DeptEmpStation where FK_Emp='" + WebUser.No + "'";
                        string station = DBAccess.RunSQLReturnString(Sql);
                        if (DataType.IsNullOrEmpty(station) == true)
                            continue;
                        string[] stations = station.Split(';');
                        bool isExit = false;
                        foreach (string s in stations)
                        {
                            if (exp.Contains(s) == true)
                            {
                                isExit = true;
                                break;
                            }
                        }
                        if (isExit == false)
                            continue;
                        break;

                    case FrmEnableRole.ByDept:
                        exp = frmNode.FrmEnableExp.Clone() as string;
                        Sql = "SELECT FK_Dept FROM Port_DeptEmp where FK_Emp='" + WebUser.No + "'";
                        string dept = DBAccess.RunSQLReturnString(Sql);
                        if (DataType.IsNullOrEmpty(dept) == true)
                            continue;
                        string[] deptStrs = dept.Split(';');
                        isExit = false;
                        foreach (string s in deptStrs)
                        {
                            if (exp.Contains(s) == true)
                            {
                                isExit = true;
                                break;
                            }
                        }
                        if (isExit == false)
                            continue;

                        break;
                    case FrmEnableRole.Disable: // 如果禁用了，就continue出去..
                        continue;
                    default:
                        throw new Exception("@没有判断的规则." + frmNode.FrmEnableRole);
                }
                #endregion

                #region 检查是否有没有目录的表单?
                bool isHave = false;
                foreach (MapData md in mds)
                {
                    if (md.FK_FormTree == "")
                    {
                        isHave = true;
                        break;
                    }
                }

                string treeNo = "0";
                if (isHave && mds.Count == 1)
                {
                    treeNo = "00";
                }
                else if (isHave == true)
                {
                    foreach (MapData md in mds)
                    {
                        if (md.FK_FormTree != "")
                        {
                            treeNo = md.FK_FormTree;
                            break;
                        }
                    }
                }
                #endregion 检查是否有没有目录的表单?

                foreach (MapData md in mds)
                {
                    if (frmNode.FK_Frm != md.No)
                        continue;

                    if (md.FK_FormTree == "")
                        md.FK_FormTree = treeNo;

                    //给他增加目录.
                    if (appFlowFormTree.Contains("Name", md.FK_FormTreeText) == false)
                    {
                        BP.WF.Template.FlowFormTree nodeFolder = new BP.WF.Template.FlowFormTree();
                        nodeFolder.No = md.FK_FormTree;
                        nodeFolder.ParentNo = "1";
                        nodeFolder.Name = md.FK_FormTreeText;
                        nodeFolder.NodeType = "folder";
                        appFlowFormTree.AddEntity(nodeFolder);
                    }

                    //检查必填项.
                    bool IsNotNull = false;
                    FrmFields formFields = new FrmFields();
                    QueryObject obj = new QueryObject(formFields);
                    obj.AddWhere(FrmFieldAttr.FK_Node, this.FK_Node);
                    obj.addAnd();
                    obj.AddWhere(FrmFieldAttr.FK_MapData, md.No);
                    obj.addAnd();
                    obj.AddWhere(FrmFieldAttr.IsNotNull, 1);
                    obj.DoQuery();
                    if (formFields != null && formFields.Count > 0)
                        IsNotNull = true;

                    BP.WF.Template.FlowFormTree nodeForm = new BP.WF.Template.FlowFormTree();
                    nodeForm.No = md.No;
                    nodeForm.ParentNo = md.FK_FormTree;

                    //设置他的表单显示名字. 2019.09.30
                    string frmName = md.Name;
                    FrmNode fn = frmNodes.GetEntityByKey(FrmNodeAttr.FK_Frm, md.No) as FrmNode;
                    if (fn != null)
                    {
                        string str = fn.FrmNameShow;
                        if (DataType.IsNullOrEmpty(str) == false)
                            frmName = str;
                    }
                    nodeForm.Name = frmName;
                    nodeForm.NodeType = IsNotNull ? "form|1" : "form|0";
                    nodeForm.IsEdit = frmNode.IsEditInt.ToString();// Convert.ToString(Convert.ToInt32(frmNode.IsEdit));
                    nodeForm.IsCloseEtcFrm = frmNode.IsCloseEtcFrmInt.ToString();
                    appFlowFormTree.AddEntity(nodeForm);
                    break;
                }
            }
            #endregion
            //增加到数据结构上去.
            TansEntitiesToGenerTree(appFlowFormTree, root.No, "");


            return appendMenus.ToString();
        }
        /// <summary>
        /// 将实体转为树形
        /// </summary>
        /// <param name="ens"></param>
        /// <param name="rootNo"></param>
        /// <param name="checkIds"></param>
        StringBuilder appendMenus = new StringBuilder();
        StringBuilder appendMenuSb = new StringBuilder();
        public void TansEntitiesToGenerTree(Entities ens, string rootNo, string checkIds)
        {
            EntityTree root = ens.GetEntityByKey(rootNo) as EntityTree;
            if (root == null)
                throw new Exception("@没有找到rootNo=" + rootNo + "的entity.");
            appendMenus.Append("[{");
            appendMenus.Append("\"id\":\"" + rootNo + "\"");
            appendMenus.Append(",\"text\":\"" + root.Name + "\"");

            //attributes
            BP.WF.Template.FlowFormTree formTree = root as BP.WF.Template.FlowFormTree;
            if (formTree != null)
            {
                string url = formTree.Url == null ? "" : formTree.Url;
                url = url.Replace("/", "|");
                appendMenus.Append(",\"attributes\":{\"NodeType\":\"" + formTree.NodeType + "\",\"IsEdit\":\"" + formTree.IsEdit + "\",\"IsCloseEtcFrm\":\"" + formTree.IsCloseEtcFrm + "\",\"Url\":\"" + url + "\"}");
            }
            appendMenus.Append(",iconCls:\"icon-Wave\"");
            // 增加它的子级.
            appendMenus.Append(",\"children\":");
            AddChildren(root, ens, checkIds);

            appendMenus.Append(appendMenuSb);
            appendMenus.Append("}]");
        }

        private void AddChildren(EntityTree parentEn, Entities ens, string checkIds)
        {
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();

            appendMenuSb.Append("[");
            foreach (EntityTree item in ens)
            {
                if (item.ParentNo != parentEn.No)
                    continue;

                if (checkIds.Contains("," + item.No + ","))
                    appendMenuSb.Append("{\"id\":\"" + item.No + "\",\"text\":\"" + item.Name + "\",\"checked\":true");
                else
                    appendMenuSb.Append("{\"id\":\"" + item.No + "\",\"text\":\"" + item.Name + "\",\"checked\":false");


                //attributes
                BP.WF.Template.FlowFormTree formTree = item as BP.WF.Template.FlowFormTree;
                if (formTree != null)
                {
                    string url = formTree.Url == null ? "" : formTree.Url;
                    string ico = "icon-tree_folder";
                    if (BP.Difference.SystemConfig.SysNo == "YYT")
                    {
                        ico = "icon-boat_16";
                    }
                    url = url.Replace("/", "|");
                    appendMenuSb.Append(",\"attributes\":{\"NodeType\":\"" + formTree.NodeType + "\",\"IsEdit\":\"" + formTree.IsEdit + "\",\"IsCloseEtcFrm\":\"" + formTree.IsCloseEtcFrm + "\",\"Url\":\"" + url + "\"}");
                    //图标
                    if (formTree.NodeType == "form|0")
                    {
                        ico = "form0";
                        if (BP.Difference.SystemConfig.SysNo == "YYT")
                        {
                            ico = "icon-Wave";
                        }
                    }
                    if (formTree.NodeType == "form|1")
                    {
                        ico = "form1";
                        if (BP.Difference.SystemConfig.SysNo == "YYT")
                        {
                            ico = "icon-Shark_20";
                        }
                    }
                    if (formTree.NodeType.Contains("tools"))
                    {
                        ico = "icon-4";
                        if (BP.Difference.SystemConfig.SysNo == "YYT")
                        {
                            ico = "icon-Wave";
                        }
                    }
                    appendMenuSb.Append(",iconCls:\"");
                    appendMenuSb.Append(ico);
                    appendMenuSb.Append("\"");
                }
                // 增加它的子级.
                appendMenuSb.Append(",\"children\":");
                AddChildren(item, ens, checkIds);
                appendMenuSb.Append("},");
            }
            if (appendMenuSb.Length > 1)
                appendMenuSb = appendMenuSb.Remove(appendMenuSb.Length - 1, 1);
            appendMenuSb.Append("]");
            appendMenus.Append(appendMenuSb);
            appendMenuSb.Clear();
        }
        #endregion

        /// <summary>
        /// 产生一个工作节点
        /// </summary>
        /// <returns></returns>
        public string GenerWorkNode()
        {
            string json = string.Empty;
            DataSet ds = new DataSet();
            Int64 workID = this.WorkID; //表单的主表.

            #region 判断当前的节点类型,获得表单的ID.
            try
            {
                if (this.currND.HisFormType == NodeFormType.RefOneFrmTree)
                {
                    //Hongyan
                    MapData md = new MapData(this.currND.NodeFrmID);
                    if (md.HisFrmType == FrmType.ChapterFrm)
                    {
                        string url = "Frm.htm?FK_MapData=" + md.No;
                        url = MyFlow_Init_DealUrl(this.currND, url);
                        return "url@"+url;
                    }
                        
                    //获取绑定的表单
                    FrmNode frmnode = new FrmNode(this.FK_Node, this.currND.NodeFrmID);
                    switch (frmnode.WhoIsPK)
                    {
                        case WhoIsPK.FID:
                            workID = this.FID;
                            break;
                        case WhoIsPK.PWorkID:
                            workID = this.PWorkID;
                            break;
                        case WhoIsPK.P2WorkID:
                            GenerWorkFlow gwff = new GenerWorkFlow(this.PWorkID);
                            workID = gwff.PWorkID;
                            break;
                        case WhoIsPK.P3WorkID:
                            string sqlId = "Select PWorkID From WF_GenerWorkFlow Where WorkID=(Select PWorkID From WF_GenerWorkFlow Where WorkID=" + this.PWorkID + ")";
                            workID = DBAccess.RunSQLReturnValInt(sqlId, 0);
                            break;
                        case WhoIsPK.RootFlowWorkID:
                            workID = BP.WF.Dev2Interface.GetRootWorkIDBySQL(this.WorkID, this.PWorkID);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
            #endregion 判断当前的节点类型,获得表单的ID.

            #region 主题方法.
            try
            {
                ds = BP.WF.CCFlowAPI.GenerWorkNode(this.FK_Flow, this.currND, workID,
                    this.FID, BP.Web.WebUser.No, this.WorkID);

                json = BP.Tools.Json.ToJson(ds);

                //ds.WriteXml("c:\\generWorkNodeJS.xml");
                //BP.DA.DataType.WriteFile("c:\\generWorkNodeJS.txt", json);
                //ds.Tables.Add(wf_generWorkFlowDt);

                if (WebUser.SysLang.Equals("CH") == true)
                    return BP.Tools.Json.ToJson(ds);

                //#region 处理多语言.
                //if (WebUser.SysLang.Equals("CH") == false)
                //{
                //    Langues langs = new Langues();
                //    langs.Retrieve(LangueAttr.Model, LangueModel.CCForm,
                //        LangueAttr.Sort, "Fields", LangueAttr.Langue, WebUser.SysLang); //查询语言.
                //}
                //#endregion 处理多语言.

                return json;
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteError(ex);
                return "err@" + ex.Message;
            }

            #endregion 主题方法.

        }

        /// <summary>
        /// 初始化子线程信息
        /// </summary>
        /// <returns></returns>
        public string ThreadDtl_Init()
        {
            DataSet ds = new DataSet();
            //当前节点的信息
            Node nd = new Node(this.FK_Node);
            ds.Tables.Add(nd.ToDataTableField("WF_Node"));
            //@hongyan
            Nodes nds = nd.HisToNodes;
            ds.Tables.Add(nds.ToDataTableField("WF_ThreadNode"));
            //发起子流程的workIds;
            DataTable dt= DBAccess.RunSQLReturnTable("SELECT DISTINCT(WorkID) FROM WF_GenerWorkerlist WHERE FID=" + this.FID + " AND FK_Node=" + this.FK_Node + " AND IsPass IN(0,-2)");
            if (dt.Rows.Count == 0)
                return "url@MyFlow";
            string workIds = "";
            foreach(DataRow dr in dt.Rows)
            {
                workIds += dr[0].ToString() + ",";
            }
            if (DataType.IsNullOrEmpty(workIds) == false)
                workIds = workIds.Substring(0, workIds.Length - 1);
            //子线程流程实例信息
            GenerWorkFlows gwfs = new GenerWorkFlows();
            QueryObject qo = new QueryObject(gwfs);
            qo.AddWhereIn(GenerWorkFlowAttr.WorkID, "(" + workIds + ")");
            qo.addAnd();
            qo.AddWhere(GenerWorkFlowAttr.FID, this.FID);
            qo.DoQuery();
            ds.Tables.Add(gwfs.ToDataTableField("WF_GenerWorkFlow"));

            //子线程执行人员信息
            GenerWorkerLists gwls = new GenerWorkerLists();
            qo = new QueryObject(gwls);
            qo.AddWhereIn(GenerWorkerListAttr.WorkID,"("+workIds+")");
            qo.addAnd();
            qo.AddWhere(GenerWorkerListAttr.FID, this.FID);
            qo.DoQuery();
            ds.Tables.Add(gwls.ToDataTableField("WF_GenerWorkerList"));


            if (nd.HisRunModel == RunModel.FL || nd.HisRunModel == RunModel.FHL)
            {
                //获取退回信息
                ReturnWorks rws = new ReturnWorks();
                qo = new QueryObject(rws);
                qo.AddWhereInSQL(ReturnWorkAttr.WorkID, "SELECT WorkID From WF_GenerWorkFlow Where FID=" + this.FID + " AND WFState=5");
                qo.addOrderByDesc("RDT");
                qo.DoQuery();
                ds.Tables.Add(rws.ToDataTableField("WF_ReturnWork"));
            }

            return BP.Tools.Json.ToJson(ds);
        }

        /// <summary>
        /// 发送单个子线程
        /// </summary>
        /// <returns></returns>
        public string ThreadDtl_SendSubThread()
        {
            int toNodeID = this.GetRequestValInt("ToNodeID");
            return BP.WF.Dev2Interface.Node_SendSubTread(this.WorkID, this.FK_Node, toNodeID);
        }

        /// <summary>
        /// 删除子线程
        /// </summary>
        /// <returns></returns>
        public string ThreadDtl_DelSubThread()
        {

            //子线程的流程实例
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.SetValByKey(GenerWorkFlowAttr.WorkID, this.WorkID);
            int count=gwf.RetrieveFromDBSources();
            if (count == 0)
                return "删除成功";
            //干流程的流程实例
            GenerWorkFlow fgwf = new GenerWorkFlow(gwf.FID);

            WorkFlow wf = new WorkFlow(this.WorkID);
            string msg = wf.DoDeleteWorkFlowByReal(false);

            BP.WF.Dev2Interface.WriteTrackInfo(gwf.FK_Flow, gwf.FK_Node, gwf.NodeName, gwf.FID, 0, "分流点手工删除", "删除子线程");

            //发起子线程的数量
            int threadCount = DBAccess.RunSQLReturnValInt("SELECT Count(*) FROM WF_GenerWorkerlist WHERE FID=" + fgwf.WorkID + " AND FK_Node=" + this.FK_Node + " AND IsPass IN(0,-2)");
            //发起的子线程已经全部取消，
            if(threadCount == 0)
            {
                Paras ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=0 WHERE FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID AND FK_Emp="+ SystemConfig.AppCenterDBVarStr+"FK_Emp";
                ps.Add("FK_Node", fgwf.FK_Node);
                ps.Add("WorkID", gwf.FID);
                ps.Add("FK_Emp", WebUser.No);
                DBAccess.RunSQL(ps);
                fgwf.WFState = WFState.Runing;
                fgwf.TodoEmps = WebUser.No + "," + WebUser.Name + ";";
                fgwf.Update();
                return "url@MyFlow.htm";
            }

            //获取子线程对应的合流节点
            string sql = "SELECT DISTINCT(FK_Node) FROM WF_GenerWorkerlist WHERE WorkID=" + gwf.FID + " AND IsPass=3";
            int hlNodeID = DBAccess.RunSQLReturnValInt(sql,0);
            if (hlNodeID == 0 && threadCount != 0)
                return "删除成功";
            //获取已完成的子线程
            sql = "SELECT COUNT(*) FROM WF_GenerWorkFlow  WHERE FID=" + gwf.FID + " AND FK_Node ="+ hlNodeID;
            //已经到合流点的子线程
            int toHLCount = DBAccess.RunSQLReturnValInt(sql);
            //合流点通过的比例
            Node nd = new Node(hlNodeID);
            decimal passRate = (decimal)toHLCount / (decimal)(threadCount) * 100;
            if (nd.PassRate <= passRate)
            {
                //分流点的待办改成已办
                Paras ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=1 WHERE FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                ps.Add("FK_Node", fgwf.FK_Node);
                ps.Add("WorkID", fgwf.WorkID);
                /* 这时已经通过,可以让主线程看到待办. */
                ps = new Paras();
                ps.SQL = "UPDATE WF_GenerWorkerList SET IsPass=0 WHERE FK_Node=" + SystemConfig.AppCenterDBVarStr + "FK_Node AND WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
                ps.Add("FK_Node", nd.NodeID);
                ps.Add("WorkID", fgwf.WorkID);
                int num = DBAccess.RunSQL(ps);
                if (num == 0)
                    throw new Exception("@不应该更新不到它.");
                    
                fgwf.FK_Node = hlNodeID;
                fgwf.SetPara("ThreadCount", 0);
                fgwf.Update();
                return "url@MyView.htm";
            }
        
            return "删除成功";
        }


        /**
         * 驳回给子线程
         */
        public string ReSend()
        {
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID);
            return objs.ToMsgOfHtml();
        }
        /**
         * 删除指定ID的子线程
         */
        public string KillThread()
        {
            Node nd = new Node(this.FK_Node);
            if ((nd.HisRunModel != RunModel.FL && nd.HisRunModel != RunModel.FHL) || this.FID != 0)
                return "err@该节点不是子线程返回的分流节点，不能删除子线程";
            //首先要检查，当前的处理人是否是分流节点的处理人？如果是，就要把，未走完的所有子线程都删除掉。
            GenerWorkerList gwl = new GenerWorkerList();

            //查询已经走得分流节点待办.
            int i = gwl.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.FK_Node, GenerWorkerListAttr.FK_Emp, WebUser.No);
            if (i == 0)
                return "err@您不能执行子线程的操作，因为当前分流工作不是您发送的。";
            gwl.IsPassInt = 1;
            gwl.IsRead = true;
            gwl.SDT = DataType.CurrentDateTimess;
            gwl.Update();


            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = this.WorkID;
            i = gwf.RetrieveFromDBSources();
            if (i == 0)
                return "err@没有获取到子线程操作的流程数据GenerWorkFlow[" + this.WorkID + "]";

            Node thredNode = new Node(gwf.FK_Node);
            //删除子线程的操作
            BP.WF.Dev2Interface.Flow_DeleteSubThread(this.WorkID, "分流节点删除子线程.");

            //删除子线程的数据
            Works wks = thredNode.HisWorks;
            if (new Flow(this.FK_Flow).HisDataStoreModel == BP.WF.Template.DataStoreModel.ByCCFlow)
                wks.Delete(GenerWorkerListAttr.WorkID, this.WorkID);

            return "";
        }

        /**
         * 删除所有的子线程
        **/
        public string UnSendAllTread()
        {
            Node nd = new Node(this.FK_Node);
            if ((nd.HisRunModel != RunModel.FL && nd.HisRunModel != RunModel.FHL)&& this.FID != 0)
                return "err@该节点不是子线程返回的分流节点，不能删除子线程";

            GenerWorkFlow gwf = new GenerWorkFlow(this.FID);
            //首先要检查，当前的处理人是否是分流节点的处理人？如果是，就要把，未走完的所有子线程都删除掉。
            GenerWorkerList gwl = new GenerWorkerList();

            //查询已经走得分流节点待办.
            int i = gwl.Retrieve(GenerWorkerListAttr.WorkID, this.FID, GenerWorkerListAttr.FK_Node, this.FK_Node, GenerWorkerListAttr.FK_Emp, WebUser.No);
            if (i == 0)
                return "err@您不能执行删除子线程的操作，因为当前分流工作不是您发送的。";

            // 更新分流节点，让其出现待办.
            gwl.IsPassInt = 0;
            gwl.IsRead = false;
            gwl.SDT = DataType.CurrentDateTimess;  //这里计算时间有问题.
            gwl.Update();

            // 把设置当前流程运行到分流流程上.
            gwf.FK_Node = this.FK_Node;

            gwf.NodeName = nd.Name;
            gwf.Sender = WebUser.No + "," + WebUser.Name + ";";
            gwf.SendDT = DataType.CurrentDateTimess;
            gwf.SetPara("ThreadCount", 0);
            gwf.WFState = WFState.Runing;
            gwf.Update();


            Work wk = nd.HisWork;
            wk.OID = gwf.WorkID;
            wk.RetrieveFromDBSources();

            // 记录日志..
            WorkNode wn = new WorkNode(wk, nd);
            wn.AddToTrack(ActionType.DeleteSubThread, WebUser.No, WebUser.Name, gwf.FK_Node, gwf.NodeName, "删除分流节点" + nd.Name + "[" + nd.NodeID + "],发起的所有子线程");


            //删除上一个节点的数据。
            foreach (Node ndNext in nd.HisToNodes)
            {
                i = DBAccess.RunSQL("DELETE FROM WF_GenerWorkerList WHERE FID=" + this.FID + " AND FK_Node=" + ndNext.NodeID);
                if (i == 0)
                    continue;

                if (ndNext.HisRunModel == RunModel.SubThread)
                {
                    /*如果到达的节点是子线程,就查询出来发起的子线程。*/
                    GenerWorkFlows gwfs = new GenerWorkFlows();
                    gwfs.Retrieve(GenerWorkFlowAttr.FID, this.FID);
                    foreach (GenerWorkFlow en in gwfs)
                        BP.WF.Dev2Interface.Flow_DeleteSubThread(en.WorkID, "分流节点删除子线程.");

                    continue;
                }

                // 删除工作记录。
                Works wks = ndNext.HisWorks;
                if (new Flow(this.FK_Flow).HisDataStoreModel == BP.WF.Template.DataStoreModel.ByCCFlow)
                    wks.Delete(GenerWorkerListAttr.FID, this.FID);


            }

            return "url@MyFlow.htm?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.FID;
        }

        public string MyFlow_StartThread()
        {
            Node nd = new Node(this.FK_Node);

            //查询出来该流程实例下的所有草稿子流程.
            GenerWorkFlows gwfs = new GenerWorkFlows();
            gwfs.Retrieve(GenerWorkFlowAttr.PWorkID, this.WorkID, GenerWorkFlowAttr.WFState, 1);

            //子流程配置信息.
            SubFlowHandGuide sf = null;
            SendReturnObjs returnObjs;
            string msgHtml = "";

            //开始发送子流程.
            foreach (GenerWorkFlow gwfSubFlow in gwfs)
            {
                //获得配置信息.
                if (sf == null || sf.FK_Flow != gwfSubFlow.FK_Flow)
                {
                    string pkval = this.FK_Flow + "_" + gwfSubFlow.FK_Flow + "_0";
                    sf = new SubFlowHandGuide();
                    sf.setMyPK(pkval);
                    sf.RetrieveFromDBSources();
                }

                //把草稿移交给当前人员. - 更新控制表.
                gwfSubFlow.Starter = WebUser.No;
                gwfSubFlow.StarterName = WebUser.Name;
                gwfSubFlow.Update();
                //把草稿移交给当前人员. - 更新工作人员列表.
                DBAccess.RunSQL("UPDATE WF_GenerWorkerList SET FK_Emp='" + WebUser.No + "',FK_EmpText='" + BP.Web.WebUser.Name + "' WHERE WorkID=" + gwfSubFlow.WorkID);
                //更新track表.
                //DBAccess.RunSQL("UPDATE ND"+int.Parse(gwfSubFlow.FK_Flow) +"Track SET FK_Emp='" + WebUser.No + "',FK_EmpText='" + WebUser.Name + "' WHERE WorkID=" + gwfSubFlow.WorkID);

                //启动子流程. 并把两个字段，写入子流程.
                returnObjs = BP.WF.Dev2Interface.Node_SendWork(gwfSubFlow.FK_Flow, gwfSubFlow.WorkID, null, null);
                msgHtml += returnObjs.ToMsgOfHtml() + "</br>";
            }
            return "启动的子流程信息如下:</br>" + msgHtml;
        }
    }
}
