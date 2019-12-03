﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 初始化函数
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
                        ps.SQL = "SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + SystemConfig.AppCenterDBVarStr + "WorkID";
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
            BP.WF.Dev2Interface.Flow_DeleteSubThread(this.FK_Flow, this.WorkID, "手工删除");
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
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

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
        /// 初始化(处理分发)
        /// </summary>
        /// <returns></returns>
        public string MyFlow_Init()
        {
            string isCC = this.GetRequestVal("IsCC");
            //手动启动子流程的标志 0父子流程 1 同级子流程
            string isStartSameLevelFlow = this.GetRequestVal("IsStartSameLevelFlow");
            if (isCC != null && isCC == "1")
                return "url@WFRpt.htm?1=2" + this.RequestParasOfAll;

            GenerWorkFlow gwf = new GenerWorkFlow();
            bool IsExistGWF = false;

            if (this.WorkID != 0)
            {
                //判断是否有执行该工作的权限.
                bool isCanDo = Dev2Interface.Flow_IsCanDoCurrentWork(this.WorkID, BP.Web.WebUser.No);
                if (isCanDo == false)
                {
                    GenerWorkFlow mygwf = new GenerWorkFlow(this.WorkID);
                    return "err@您[" + WebUser.No + "," + WebUser.Name + "]不能执行当前工作, 当前工作已经运转到[" + mygwf.NodeName + "],处理人[" + mygwf.TodoEmps + "]。";
                }

                gwf = new GenerWorkFlow();
                gwf.WorkID = this.WorkID;
                if (gwf.RetrieveFromDBSources() == 0)
                    return ("err@该流程ID{" + this.WorkID + "}不存在，或者已经被删除.");
                IsExistGWF = true;
            }

            //判断当前节点是否是打开即阅读
            if (IsExistGWF == true)
            {
                //获取当前节点信息
                Node nd = new Node(gwf.FK_Node);
                if (nd != null && nd.IsOpenOver == true)
                {
                    //如果是结束节点执行流程结束功能
                    if (nd.IsStartNode == false)
                    {
                        //如果启用审核组件
                        if (nd.FrmWorkCheckSta == FrmWorkCheckSta.Enable)
                        {
                            //判断一下审核意见是否有默认值
                            FrmWorkCheck workCheck = new FrmWorkCheck("ND" + nd.NodeID);
                            string msg = "同意";
                            if (workCheck.FWCIsFullInfo == true)
                                msg = workCheck.FWCDefInfo;
                            BP.WF.Dev2Interface.WriteTrackWorkCheck(gwf.FK_Flow, nd.NodeID, gwf.WorkID, gwf.FID, msg, workCheck.FWCOpLabel);
                        }

                        BP.WF.Dev2Interface.Node_SendWork(gwf.FK_Flow, gwf.WorkID);
                        Node toNode = new Node(gwf.FK_Node);
                        if (nd.HisFormType != NodeFormType.SheetTree && nd.HisFormType != NodeFormType.SheetAutoTree)
                            //跳转到查看页面
                            return "url@" + "./CCForm/Frm.htm?WorkID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "&FK_Node=" + gwf.FK_Node + "&FK_MapData=" + toNode.NodeFrmID + "&IsReadonly=1";
                        else
                            //跳转到查看页面
                            return "url@./MyFlowTreeReadonly.htm?WorkID=" + gwf.WorkID + "&FID=" + gwf.FID + "&OID=" + gwf.WorkID + "&FK_Flow=" + gwf.FK_Flow + "&FK_Node=" + nd.NodeID + "&PK=OID&PKVal=" + gwf.WorkID + "&IsEdit=0&IsLoadData=0&IsReadonly=1";
                    }
                }
            }

            //当前工作.
            Work currWK = this.currND.HisWork;

            #region 判断前置导航.
            if (this.currND.IsStartNode && this.IsCC == false && this.WorkID == 0)
            {
                try
                {
                    if (BP.WF.Dev2Interface.Flow_IsCanStartThisFlow(this.FK_Flow, WebUser.No, this.PFlowNo, this.PNodeID, this.PWorkID) == false)
                    {
                        /*是否可以发起流程？ @李国文. */
                        return "err@您(" + BP.Web.WebUser.No + ")没有发起或者处理该流程的权限.";
                    }
                }
                catch (Exception ex)
                {
                    return "err@" + ex.Message;
                }
            }

            //第一次加载.
            if (this.WorkID == 0 && this.currND.IsStartNode && this.GetRequestVal("IsCheckGuide") == null)
            {

                Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, null, null,
                    WebUser.No, null, this.PWorkID, this.PFID, this.PFlowNo, this.PNodeID, null, 0, null, null, isStartSameLevelFlow);

                string hostRun = this.currFlow.GetValStrByKey(FlowAttr.HostRun);
                if (DataType.IsNullOrEmpty(hostRun) == false)
                    hostRun += "/WF/";

                this.WorkID = workid;

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
                            return "err@流程配置错误，您不能同时启用前置导航，自动装载上一笔数据两个功能。";
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
                    case StartGuideWay.ByParentFlowModel: //选择父流程 @yuanlina.
                        return "url@" + hostRun + "./WorkOpt/StartGuideParentFlowModel.htm?FK_Flow=" + this.currFlow.No + "&WorkID=" + workid;
                    default:
                        break;
                }
            }
            #endregion 判断前置导航

            #region 前置导航数据拷贝到第一节点
            if (this.WorkID != 0 && this.GetRequestVal("IsCheckGuide") != null)
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
            if (isStartSameLevelFlow != null && isStartSameLevelFlow.Equals("1") == true && this.WorkID != 0)
            {
                gwf.WorkID = this.WorkID;
                gwf.RetrieveFromDBSources();
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

            #region 处理表单类型.
            if (this.currND.HisFormType == NodeFormType.SheetTree
                 || this.currND.HisFormType == NodeFormType.SheetAutoTree)
            {

                if (this.WorkID == 0)
                {
                    this.WorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, null, null, WebUser.No, null);
                    currWK = currND.HisWork;
                    currWK.OID = this.WorkID;
                    currWK.Retrieve();
                }
                else
                {
                    gwf.WorkID = this.WorkID;
                    gwf.RetrieveFromDBSources();
                }

                if (gwf.PWorkID == 0 && this.WorkID != 0)
                {
                    gwf.WorkID = this.WorkID;
                    gwf.PWorkID = this.PWorkID;
                    if (DataType.IsNullOrEmpty(gwf.PFlowNo) == true)
                        gwf.PFlowNo = this.PFlowNo;
                    gwf.Update();
                }

                if (this.currND.IsStartNode)
                {
                    /*如果是开始节点, 先检查是否启用了流程限制。*/
                    if (BP.WF.Glo.CheckIsCanStartFlow_InitStartFlow(this.currFlow) == false)
                    {
                        /* 如果启用了限制就把信息提示出来. */
                        string msg = BP.WF.Glo.DealExp(this.currFlow.StartLimitAlert, currWK, null);
                        return "err@" + msg;
                    }
                }

                #region 开始组合url.
                string toUrl = "";

                if (this.IsMobile == true)
                {
                    if (gwf.Paras_Frms.Equals("") == false)
                        toUrl = "MyFlowGener.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&SID=" + WebUser.SID + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID + "&Frms=" + gwf.Paras_Frms;
                    else
                        toUrl = "MyFlowGener.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&SID=" + WebUser.SID + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID;
                }
                else
                {
                    if (gwf.Paras_Frms.Equals("") == false)
                        toUrl = "MyFlowTree.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&SID=" + WebUser.SID + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID + "&Frms=" + gwf.Paras_Frms;
                    else
                        toUrl = "MyFlowTree.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&SID=" + WebUser.SID + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID;
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

                if (gwf == null)
                {
                    gwf = new GenerWorkFlow();
                    gwf.WorkID = this.WorkID;
                    gwf.RetrieveFromDBSources();
                }
                //设置url.
                if (gwf.WFState == WFState.Runing || gwf.WFState == WFState.Blank || gwf.WFState == WFState.Draft)
                {
                    if (toUrl.Contains("IsLoadData") == false)
                        toUrl += "&IsLoadData=1";
                    else
                        toUrl = toUrl.Replace("&IsLoadData=0", "&IsLoadData=1");
                }
                //SDK表单上服务器地址,应用到使用ccflow的时候使用的是sdk表单,该表单会存储在其他的服务器上,珠海高凌提出. 
                toUrl = toUrl.Replace("@SDKFromServHost", SystemConfig.AppSettings["SDKFromServHost"]);

                //增加fk_node
                if (toUrl.Contains("&FK_Node=") == false)
                    toUrl += "&FK_Node=" + this.currND.NodeID;


                //如果是开始节点.
                if (currND.IsStartNode == true)
                {
                    if (toUrl.Contains("PrjNo") == true && toUrl.Contains("PrjName") == true)
                    {
                        string sql = "UPDATE " + currWK.EnMap.PhysicsTable + " SET PrjNo='" + this.GetRequestVal("PrjNo") + "', PrjName='" + this.GetRequestVal("PrjName") + "' WHERE OID=" + this.WorkID;
                        BP.DA.DBAccess.RunSQL(sql);
                    }
                }
                return "url@" + toUrl;
            }

            if (this.currND.HisFormType == NodeFormType.SDKForm)
            {
                if (this.WorkID == 0)
                {
                    currWK = this.currFlow.NewWork();
                    this.WorkID = currWK.OID;
                }

                string url = currND.FormUrl;
                if (DataType.IsNullOrEmpty(url))
                {
                    return "err@设置读取状流程设计错误态错误,没有设置表单url.";
                }

                //处理连接.
                url = this.MyFlow_Init_DealUrl(currND, currWK);

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
            /*如果是傻瓜表单，就转到傻瓜表单的解析执行器上，为软通动力改造。*/
            if (this.WorkID == 0)
            {
                currWK = this.currFlow.NewWork();
                this.WorkID = currWK.OID;
            }

            if (frmtype == NodeFormType.FoolTruck)
            {
                /*如果是傻瓜表单，就转到傻瓜表单的解析执行器上，为软通动力改造。*/
                if (this.WorkID == 0)
                {
                    currWK = this.currFlow.NewWork();
                    this.WorkID = currWK.OID;
                }

                //string url = "MyFlowFoolTruck.htm";
                string url = "MyFlowGener.htm";

                //处理连接.
                url = this.MyFlow_Init_DealUrl(currND, currWK, url);
                return "url@" + url;
            }

            if (frmtype == NodeFormType.WebOffice)
            {
                /*如果是公文表单，就转到公文表单的解析执行器上，为软通动力改造。*/
                if (this.WorkID == 0)
                {
                    currWK = this.currFlow.NewWork();
                    this.WorkID = currWK.OID;
                }

                //string url = "MyFlowFoolTruck.htm";
                string url = "MyFlowWebOffice.htm";

                //处理连接.
                url = this.MyFlow_Init_DealUrl(currND, currWK, url);
                return "url@" + url;
            }

            if (frmtype == NodeFormType.FoolForm && this.IsMobile == false)
            {
                /*如果是傻瓜表单，就转到傻瓜表单的解析执行器上。*/
                if (this.WorkID == 0)
                {
                    currWK = this.currFlow.NewWork();
                    this.WorkID = currWK.OID;
                }

                string url = "MyFlowGener.htm";
                if (this.IsMobile)
                    url = "MyFlowGener.htm";

                //处理连接.
                url = this.MyFlow_Init_DealUrl(currND, currWK, url);

                url = url.Replace("DoType=MyFlow_Init&", "");
                url = url.Replace("&DoWhat=StartClassic", "");
                return "url@" + url;
            }

            //自定义表单
            if (frmtype == NodeFormType.SelfForm && this.IsMobile == false)
            {
                if (this.WorkID == 0)
                {
                    currWK = this.currFlow.NewWork();
                    this.WorkID = currWK.OID;
                }

                string url = "MyFlowSelfForm.htm";

                //处理连接.
                url = this.MyFlow_Init_DealUrl(currND, currWK, url);

                url = url.Replace("DoType=MyFlow_Init&", "");
                url = url.Replace("&DoWhat=StartClassic", "");
                return "url@" + url;
            }
            #endregion 内置表单类型的判断.

            string myurl = "MyFlowGener.htm";

            //处理连接.
            myurl = this.MyFlow_Init_DealUrl(currND, currWK, myurl);
            myurl = myurl.Replace("DoType=MyFlow_Init&", "");
            myurl = myurl.Replace("&DoWhat=StartClassic", "");



            return "url@" + myurl;
        }
        private string MyFlow_Init_DealUrl(BP.WF.Node currND, Work currWK, string url = null)
        {
            if (url == null)
                url = currND.FormUrl;

            string urlExt = this.RequestParas;
            //防止查询不到.
            urlExt = urlExt.Replace("?WorkID=", "&WorkID=");
            if (urlExt.Contains("&WorkID") == false)
            {
                urlExt += "&WorkID=" + this.WorkID;
            }
            else
            {
                urlExt = urlExt.Replace("&WorkID=0", "&WorkID=" + this.WorkID);
                urlExt = urlExt.Replace("&WorkID=&", "&WorkID=" + this.WorkID + "&");
            }

            //SDK表单上服务器地址,应用到使用ccflow的时候使用的是sdk表单,该表单会存储在其他的服务器上,珠海高凌提出. 
            url = url.Replace("@SDKFromServHost", SystemConfig.AppSettings["SDKFromServHost"]);

            if (urlExt.Contains("&NodeID") == false)
                urlExt += "&NodeID=" + currND.NodeID;

            if (urlExt.Contains("FK_Node") == false)
                urlExt += "&FK_Node=" + currND.NodeID;

            if (urlExt.Contains("&FID") == false && currWK != null)
            {
                //urlExt += "&FID=" + currWK.FID;
                urlExt += "&FID=" + this.FID;
            }

            if (urlExt.Contains("&UserNo") == false)
                urlExt += "&UserNo=" + HttpUtility.UrlEncode(WebUser.No);

            if (urlExt.Contains("&SID") == false)
                urlExt += "&SID=" + WebUser.SID;

            if (url.Contains("?") == true)
                url += "&" + urlExt;
            else
                url += "?" + urlExt;

            foreach (string str in HttpContextHelper.RequestParamKeys)
            {
                if (DataType.IsNullOrEmpty(str) == true)
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
                string str = BP.WF.Dev2Interface.Flow_DoFlowOver(this.FK_Flow, this.WorkID, "流程成功结束");
                if (str == "" || str == null)
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
                string str = BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.FK_Flow, this.WorkID);
                if (str == "" || str == null)
                    return "流程成功结束";
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
        /// 工具栏
        /// </summary>
        /// <returns></returns>
        public string InitToolBar()
        {
            #region 处理是否是加签，或者是否是会签模式.
            bool isAskForOrHuiQian = false;
            BtnLab btnLab = new BtnLab(this.FK_Node);
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            if (this.FK_Node.ToString().EndsWith("01") == false)
            {
                if (gwf.WFState == WFState.Askfor)
                    isAskForOrHuiQian = true;

                /*判断是否是加签状态，如果是，就判断是否是主持人，如果不是主持人，就让其 isAskFor=true ,屏蔽退回等按钮.*/
                /**说明：针对于组长模式的会签，协作模式的会签加签人仍可以加签*/
                if (gwf.HuiQianTaskSta == HuiQianTaskSta.HuiQianing)
                {
#warning 修复会签状态不正确的问题，如果是会签状态，但是WF_GenerWorkerList中只有一个待办，则说明数据不正确 yuanlina
                    GenerWorkerLists gwls = new GenerWorkerLists();
                    gwls.Retrieve(GenerWorkerListAttr.WorkID, this.WorkID, GenerWorkerListAttr.FK_Node, this.FK_Node);
                    if (gwls.Count == 1)
                    {
                        //修改流程会签状态
                        gwf.HuiQianTaskSta = HuiQianTaskSta.None;
                        isAskForOrHuiQian = false;
                    }
                    else
                    {
                        if (btnLab.HuiQianRole == HuiQianRole.TeamupGroupLeader)
                        {
                            if (btnLab.HuiQianLeaderRole == 0)
                            {
                                if (gwf.HuiQianZhuChiRen != WebUser.No && gwf.GetParaString("AddLeader").Contains(WebUser.No + ",") == false)
                                    isAskForOrHuiQian = true;
                            }
                            else
                            {
                                //不是主持人
                                if (gwf.HuiQianZhuChiRen.Contains(WebUser.No + ",") == false && gwf.GetParaString("AddLeader").Contains(WebUser.No + ",") == false)
                                    isAskForOrHuiQian = true;
                            }
                        }

                    }

                   
                }
            }
            #endregion 处理是否是加签，或者是否是会签模式，.

            string tKey = DateTime.Now.ToString("MM-dd-hh:mm:ss");
            string toolbar = "";
            try
            {
                #region 是否是会签？.
                if (isAskForOrHuiQian == true && SystemConfig.CustomerNo == "LIMS")
                    return "";

                if (isAskForOrHuiQian == true)
                {
                    toolbar += "<input name='Send' type=button value='确定/完成' data-type='isAskFor' enable=true onclick=\" " + btnLab.SendJS + " if(SysCheckFrm()==false) return false;SaveDtlAll();Send(); \" />";
                    // toolbar += "<input name='Send' type=button  value='" + btnLab.SendLab + "' enable=true onclick=\"" + btnLab.SendJS + " if ( SendSelfFrom()==false) return false; Send(); this.disabled=true;\" />";
                    if (btnLab.PrintZipEnable == true)
                    {
                        string packUrl = "./WorkOpt/Packup.htm?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
                        toolbar += "<input type=button name='PackUp'  value='" + btnLab.PrintZipLab + "' enable=true/>";
                    }

                    if (btnLab.TrackEnable)
                        toolbar += "<input type=button name='Track'  value='" + btnLab.TrackLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />";

                    return toolbar;
                }
                #endregion 是否是会签.

                #region 是否是抄送.
                if (this.IsCC)
                {
                    toolbar += "<input type=button  value='流程运行轨迹' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />";
                    // 判断审核组件在当前的表单中是否启用，如果启用了.
                    FrmWorkCheck fwc = new FrmWorkCheck(this.FK_Node);
                    if (fwc.HisFrmWorkCheckSta != FrmWorkCheckSta.Enable)
                    {
                        /*如果不等于启用, */
                        toolbar += "<input type=button  value='填写审核意见' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/CCCheckNote.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />";
                    }
                    return toolbar;
                }
                #endregion 是否是抄送.

                #region 如果当前节点启用了协作会签.
                if (btnLab.HuiQianRole == HuiQianRole.Teamup)
                {
                    if (this.IsMobile == true)
                        toolbar += "<input name='SendHuiQian' type=button value='会签发送' enable=true onclick=\" " + btnLab.SendJS + " if(SysCheckFrm()==false) return false;SaveDtlAll();SendIt(true); \" />";
                    else
                        toolbar += "<input name='SendHuiQian' type=button value='会签发送' enable=true onclick=\" " + btnLab.SendJS + " if(SysCheckFrm()==false) return false;SaveDtlAll();Send(true); \" />";
                }
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
                            /*如果启用了发送按钮.*/
                            toolbar += "<input name='Send' type=button value='" + btnLab.SendLab + "' enable=true onclick=\"" + btnLab.SendJS + " if (SendSelfFrom()==false) return false; this.disabled=true;\" />";
                        }
                    }
                    else
                    {
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
                        {
                            toolbar += "<input name='Send' type=button  value='" + btnLab.SendLab + "' enable=true onclick=\"" + btnLab.SendJS + " if ( SendSelfFrom()==false) return false; this.disabled=true;\" />";
                        }
                    }

                    /*处理保存按钮.*/
                    if (btnLab.SaveEnable)
                    {
                        toolbar += "<input name='Save' type=button value='" + btnLab.SaveLab + "' enable=true onclick=\"SaveSelfFrom();\" />";
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
                            if (this.IsMobile)
                                toolbar += "<input name='Send' type=button value='" + btnLab.SendLab + "' enable=true onclick=\" " + btnLab.SendJS + " if(SysCheckFrm()==false) return false;SaveDtlAll();SendIt(); \" />";
                            else
                                toolbar += "<input name='Send' type=button value='" + btnLab.SendLab + "' enable=true onclick=\" " + btnLab.SendJS + " if(SysCheckFrm()==false) return false;SaveDtlAll();Send(); \" />";

                        }
                    }
                    else
                    {
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
                        {
                            /*如果启用了发送按钮.
                             * 1. 如果是加签的状态，就不让其显示发送按钮，因为在加签的提示。
                             */
                            if (this.IsMobile)
                                toolbar += "<input name='Send' type=button  value='" + btnLab.SendLab + "' enable=true onclick=\" " + btnLab.SendJS + " if(SysCheckFrm()==false) return false;SendIt();\" />";
                            else
                                toolbar += "<input name='Send' type=button  value='" + btnLab.SendLab + "' enable=true onclick=\" " + btnLab.SendJS + " if(SysCheckFrm()==false) return false;Send();\" />";
                        }
                    }

                    /* 处理保存按钮.*/
                    if (btnLab.SaveEnable)
                    {
                        if (this.IsMobile)
                            toolbar += "<input name='Save' type=button  value='" + btnLab.SaveLab + "' enable=true onclick=\"   if(SysCheckFrm()==false) return false; SaveIt();\" />";
                        else
                            toolbar += "<input name='Save' type=button  value='" + btnLab.SaveLab + "' enable=true onclick=\"   if(SysCheckFrm()==false) return false;Save();\" />";
                    }
                }

                if (btnLab.WorkCheckEnable)
                {
                    /*审核*/
                    toolbar += "<input  name='workcheckBtn' type=button  value='" + btnLab.WorkCheckLab + "' enable=true />";
                }

                if (btnLab.ThreadEnable)
                {
                    /*如果要查看子线程.*/
                    string ur2 = "./WorkOpt/ThreadDtl.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button  value='" + btnLab.ThreadLab + "' enable=true onclick=\"WinOpen('" + ur2 + "'); \" />";
                }

                if (btnLab.ShowParentFormEnable && this.PWorkID != 0)
                {
                    /*如果要查看父流程.*/
                    GenerWorkFlow gwf1 = new GenerWorkFlow(this.PWorkID);
                    string ur2 = "./WorkOpt/OneWork/FrmGuide.htm?FK_Node=" + gwf1.FK_Node + "&FID=" + gwf1.FID + "&WorkID=" + gwf1.WorkID + "&FK_Flow=" + gwf1.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button  value='" + btnLab.ShowParentFormLab + "' enable=true onclick=\"WinOpen('" + ur2 + "'); \" />";
                }

                if (btnLab.TCEnable == true)
                {
                    /*流转自定义..*/
                    string ur3 = "./WorkOpt/TransferCustom.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='TransferCustom'  value='" + btnLab.TCLab + "' enable=true onclick=\"TransferCustom('" + ur3 + "'); \" />";
                }

                if (btnLab.HelpRole != 0)
                {
                    toolbar += "<input type=button  value='" + btnLab.HelpLab + "' enable=true onclick=\"HelpAlter(); \" />";
                }

                if (btnLab.JumpWayEnable && 1 == 2)
                {
                    /*跳转*/
                    string urlr = "./WorkOpt/JumpWay.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button  value='" + btnLab.JumpWayLab + "' enable=true onclick=\"To('" + urlr + "'); \" />";
                }

                if (btnLab.ReturnEnable)
                {
                    /*退回*/
                    string urlr = "./WorkOpt/ReturnWork.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input name='Return' type=button  value='" + btnLab.ReturnLab + "' enable=true onclick=\"ReturnWork('" + urlr + "','" + btnLab.ReturnField + "'); \" />";
                }

                //  if (btnLab.HungEnable && this.currND.IsStartNode == false)
                if (btnLab.HungEnable)
                {
                    /*挂起*/
                    string urlr = "./WorkOpt/HungUp.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button  value='" + btnLab.HungLab + "' enable=true onclick=\"WinOpen('" + urlr + "'); \" />";
                }

                if (btnLab.ShiftEnable)
                {
                    /*移交*/
                    string url12 = "./WorkOpt/Forward.htm?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&Info=" + "移交原因.";
                    toolbar += "<input name='Shift' type=button  value='" + btnLab.ShiftLab + "' enable=true onclick=\"To('" + url12 + "'); \" />";
                }

                if ((btnLab.CCRole == CCRole.HandCC || btnLab.CCRole == CCRole.HandAndAuto))
                {
                    if (this.IsMobile)
                    {
                        String urlrDel = "./WorkOpt/CC.htm?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&s=" + tKey;
                        toolbar += "<input name='CC' type=button  value='" + btnLab.CCLab + "' enable=true onclick=\"To('" + urlrDel + "'); \" />";
                    }
                    else
                        // 抄送 
                        toolbar += "<input name='CC' type=button  value='" + btnLab.CCLab + "' enable=true onclick=\"WinOpen('" + "./WorkOpt/CC.htm?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&s=" + tKey + "','ds'); \" />";
                }

                if (btnLab.DeleteEnable != 0)
                {
                    string urlrDel = appPath + "WF/MyFlowInfo.htm?DoType=DeleteFlow&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input name='Delete' type=button  value='" + btnLab.DeleteLab + "' enable=true onclick=\"To('" + urlrDel + "'); \" />";
                }

                if (btnLab.EndFlowEnable && this.currND.IsStartNode == false)
                {
                    toolbar += "<input type=button name='EndFlow'  value='" + btnLab.EndFlowLab + "' enable=true onclick=\"javascript:DoStop('" + btnLab.EndFlowLab + "','" + this.FK_Flow + "','" + this.WorkID + "');\" />";
                }

                // @李国文.
                if (btnLab.PrintDocEnable == true)
                {
                    string urlr = appPath + "WF/WorkOpt/PrintDoc.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='PrintDoc' value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" />";
                }

                if (btnLab.TrackEnable)
                    toolbar += "<input type=button name='Track'  value='" + btnLab.TrackLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />";


                if (btnLab.SearchEnable)
                    toolbar += "<input type=button name='Search'  value='" + btnLab.SearchLab + "' enable=true onclick=\"WinOpen('./RptDfine/Default.htm?RptNo=ND" + int.Parse(this.FK_Flow) + "MyRpt&FK_Flow=" + this.FK_Flow + "&SearchType=My&s=" + tKey + "','dsd0'); \" />";

                if (btnLab.BatchEnable)
                {
                    /*批量处理*/
                    string urlr = appPath + "WF/Batch.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='Batch' value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" />";
                }

                if (btnLab.AskforEnable)
                {
                    /*加签 */
                    string urlr3 = appPath + "WF/WorkOpt/Askfor.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='Askfor'  value='" + btnLab.AskforLab + "' enable=true onclick=\"To('" + urlr3 + "'); \" />";
                }

                if (btnLab.HuiQianRole == HuiQianRole.TeamupGroupLeader)
                {
                    /*会签 */
                    string urlr3 = appPath + "WF/WorkOpt/HuiQian.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='HuiQian'  value='" + btnLab.HuiQianLab + "' enable=true onclick=\"To('" + urlr3 + "'); \" />";
                }

                //原始会签主持人可以增加组长
                if (((DataType.IsNullOrEmpty(gwf.HuiQianZhuChiRen) == true && gwf.TodoEmps.Contains(WebUser.No) == true) || gwf.HuiQianZhuChiRen.Contains(WebUser.No) == true) && btnLab.AddLeaderEnable == true)
                {
                    /*增加组长 */
                    toolbar += "<input type=button name='AddLeader'  value='" + btnLab.AddLeaderLab + "' enable=true  />";
                }


                if (btnLab.WebOfficeWorkModel == WebOfficeWorkModel.Button)
                {
                    /*公文正文 */
                    string urlr = appPath + "WF/WorkOpt/WebOffice.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='WebOffice'  value='" + btnLab.WebOfficeLab + "' enable=true onclick=\"WinOpen('" + urlr + "','公文正文'); \" />";
                }

                // 需要翻译.
                if (this.currFlow.IsResetData == true && this.currND.IsStartNode)
                {
                    /* 启用了数据重置功能 */
                    string urlr3 = appPath + "WF/MyFlow.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&IsDeleteDraft=1&s=" + tKey;
                    toolbar += "<input type=button  value='数据重置' enable=true onclick=\"To('" + urlr3 + "','ds'); \" />";
                }

                //if (btnLab.SubFlowEnable == true )
                //{
                //    /* 子流程 */
                //    string urlr3 = appPath + "WF/WorkOpt/SubFlow.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                //    toolbar += "<input type=button name='SubFlow'  value='" + btnLab.SubFlowLab + "' enable=true onclick=\"WinOpen('" + urlr3 + "'); \" />";
                //}

                if (btnLab.CHRole != 0)
                {
                    /* 节点时限设置 */
                    string urlr3 = appPath + "WF/WorkOpt/CH.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='CH'  value='" + btnLab.CHLab + "' enable=true onclick=\"WinOpen('" + urlr3 + "'); \" />";
                }

                if (btnLab.NoteEnable != 0)
                {
                    /* 备注设置 */
                    toolbar += "<input type=button name='Note'  value='" + btnLab.NoteLab + "' enable=true  />";
                }


                if (btnLab.PRIEnable == true)
                {
                    /* 优先级设置 */
                    string urlr3 = appPath + "WF/WorkOpt/PRI.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='PR'  value='" + btnLab.PRILab + "' enable=true onclick=\"WinOpen('" + urlr3 + "'); \" />";
                }

                /* 关注 */
                if (btnLab.FocusEnable == true)
                {
                    if (HisGenerWorkFlow.Paras_Focus == true)
                        toolbar += "<input type=button  value='取消关注' enable=true onclick=\"FocusBtn(this,'" + this.WorkID + "'); \" />";
                    else
                        toolbar += "<input type=button name='Focus' value='" + btnLab.FocusLab + "' enable=true onclick=\"FocusBtn(this,'" + this.WorkID + "'); \" />";
                }

                /* 分配工作 */
                if (btnLab.AllotEnable == true)
                {
                    /*分配工作*/
                    string urlAllot = "./WorkOpt/AllotTask.htm?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&Info=" + "移交原因.";
                    toolbar += "<input name='Allot' type=button  value='" + btnLab.AllotLab + "' enable=true onclick=\"To('" + urlAllot + "'); \" />";
                }

                /* 确认 */
                if (btnLab.ConfirmEnable == true)
                {
                    if (HisGenerWorkFlow.Paras_Confirm == true)
                        toolbar += "<input type=button  value='取消确认' enable=true onclick=\"ConfirmBtn(this,'" + this.WorkID + "'); \" />";
                    else
                        toolbar += "<input type=button name='Confirm' value='" + btnLab.ConfirmLab + "' enable=true onclick=\"ConfirmBtn(this,'" + this.WorkID + "'); \" />";
                }

                // 需要翻译.

                /* 打包下载zip */
                if (btnLab.PrintZipEnable == true)
                {
                    string packUrl = "./WorkOpt/Packup.htm?FileType=zip&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
                    toolbar += "<input type=button name='PackUp_zip'  value='" + btnLab.PrintZipLab + "' enable=true/>";
                }

                /* 打包下载html */
                if (btnLab.PrintHtmlEnable == true)
                {
                    string packUrl = "./WorkOpt/Packup.htm?FileType=html&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
                    toolbar += "<input type=button name='PackUp_html'  value='" + btnLab.PrintHtmlLab + "' enable=true/>";
                }

                /* 打包下载pdf */
                if (btnLab.PrintPDFEnable == true)
                {
                    string packUrl = "./WorkOpt/Packup.htm?FileType=pdf&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
                    toolbar += "<input type=button name='PackUp_pdf'  value='" + btnLab.PrintPDFLab + "' enable=true/>";
                }

                if (this.currND.IsStartNode == true)
                {
                    if (this.currFlow.IsDBTemplate == true)
                    {
                        string packUrl = "./WorkOpt/DBTemplate.htm?FileType=pdf&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
                        toolbar += "<input type=button name='DBTemplate'  value='模版' enable=true/>";
                    }
                }

                /* 公文标签 */
                if (btnLab.OfficeBtnEnable == true)
                {
                    toolbar += "<input type=button name='Btn_Office'  onclick='OpenOffice(\"" + btnLab.OfficeBtnEnable + "\");'  value='" + btnLab.OfficeBtnLab + "' enable=true/>";
                }
                #endregion


                #region  加载自定义的button.
                BP.WF.Template.NodeToolbars bars = new NodeToolbars();
                bars.Retrieve(NodeToolbarAttr.FK_Node, this.FK_Node);
                foreach (NodeToolbar bar in bars)
                {
                    if (bar.ShowWhere != ShowWhere.Toolbar)
                        continue;

                    if (bar.ExcType == 1 || (!DataType.IsNullOrEmpty(bar.Target) == false && bar.Target.ToLower() == "javascript"))
                    {
                        toolbar += "<input type=button  value='" + bar.Title + "' enable=true onclick='" + bar.Url + "' />";
                    }
                    else
                    {
                        string urlr3 = bar.Url + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                        toolbar += "<input type=button  value='" + bar.Title + "' enable=true onclick=\"WinOpen('" + urlr3 + "'); \" />";
                    }
                }
                #endregion  //加载自定义的button.

            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex);
                toolbar = "err@" + ex.Message;
            }
            return toolbar;
        }

        /// <summary>
        /// 工具栏
        /// </summary>
        /// <returns></returns>
        public string InitToolBarForMobile()
        {
            string str = InitToolBar();
            str = str.Replace("Send()", "SendIt()");
            return str;

            #region 处理是否是加签，或者是否是会签模式，.
            bool isAskForOrHuiQian = false;
            if (this.FK_Node.ToString().EndsWith("01") == false)
            {
                GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
                if (gwf.WFState == WFState.Askfor)
                {
                    isAskForOrHuiQian = true;
                }
                else
                {
                    /*判断是否是加签状态，如果是，就判断是否是主持人，如果不是主持人，就让其 isAskFor=true ,屏蔽退回等按钮. */
                    if (gwf.TodoEmps.Contains(WebUser.No + ",") == false)
                        isAskForOrHuiQian = true;
                }
            }
            #endregion 处理是否是加签，或者是否是会签模式，.

            string tKey = DateTime.Now.ToString("yyyy-MM-dd - hh:mm:ss");
            BtnLab btnLab = new BtnLab(this.FK_Node);
            string toolbar = "";
            try
            {
                #region 是否是会签？.
                if (isAskForOrHuiQian == true)
                {
                    toolbar += "<a data-role='button' name='Send'  value='" + btnLab.SendLab + "' enable=true onclick=\" " + btnLab.SendJS + " if(SysCheckFrm()==false) return false;SaveDtlAll();SendIt(); \" ></a>";
                    if (btnLab.PrintZipEnable == true)
                    {
                        string packUrl = "./WorkOpt/Packup.htm?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
                        toolbar += "<a data-role='button' type=button name='PackUp'  value='" + btnLab.PrintZipLab + "' enable=true></a>";
                    }
                    return toolbar;
                }
                #endregion 是否是抄送.

                #region 是否是抄送.
                if (this.IsCC)
                {
                    toolbar += "<a data-role='button'    value='流程运行轨迹' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" ></a>";
                    // 判断审核组件在当前的表单中是否启用，如果启用了.
                    FrmWorkCheck fwc = new FrmWorkCheck(this.FK_Node);
                    if (fwc.HisFrmWorkCheckSta != FrmWorkCheckSta.Enable)
                    {
                        /*如果不等于启用, */
                        toolbar += "<a data-role='button' type=button  value='填写审核意见' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/CCCheckNote.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" ></a>";
                    }
                    return toolbar;
                }
                #endregion 是否是抄送.

                #region 加载流程控制器 - 按钮
                if (this.currND.HisFormType == NodeFormType.SelfForm)
                {
                    /*如果是嵌入式表单.*/
                    if (currND.IsEndNode)
                    {
                        /*如果当前节点是结束节点.*/
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
                        {
                            /*如果启用了发送按钮.*/
                            toolbar += "<a data-role='button' name='Send'   value='" + btnLab.SendLab + "' enable=true onclick=\"" + btnLab.SendJS + " if (SendSelfFrom()==false) return false; SendIt(); this.disabled=true;\" ></a>";
                        }
                    }
                    else
                    {
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
                        {
                            toolbar += "<a data-role='button' name='Send'  value='" + btnLab.SendLab + "' enable=true onclick=\"" + btnLab.SendJS + " if ( SendSelfFrom()==false) return false; SendIt(); this.disabled=true;\" ></a>";
                        }
                    }

                    /*处理保存按钮.*/
                    if (btnLab.SaveEnable)
                    {
                        toolbar += "<a data-role='button' name='Save'   value='" + btnLab.SaveLab + "' enable=true onclick=\"SaveSelfFrom();\" />";
                    }
                }

                if (this.currND.HisFormType == NodeFormType.FoolForm || this.currND.HisFormType == NodeFormType.FreeForm)
                {
                    /*启用了其他的表单.*/
                    if (currND.IsEndNode)
                    {
                        /*如果当前节点是结束节点.*/
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
                        {
                            /*如果启用了选择人窗口的模式是【选择既发送】.*/
                            toolbar += "<a data-role='button' name='Send' value='" + btnLab.SendLab + "' enable=true onclick=\" " + btnLab.SendJS + " if(SysCheckFrm()==false) return false;SaveDtlAll();SendIt(); \" ></a>";
                        }
                    }
                    else
                    {
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
                        {
                            /*如果启用了发送按钮.
                             * 1. 如果是加签的状态，就不让其显示发送按钮，因为在加签的提示。
                             */
                            toolbar += "<a data-role='button' name='Send'   value='" + btnLab.SendLab + "' enable=true onclick=\" " + btnLab.SendJS + " if(SysCheckFrm()==false) return false;SendIt();\" ></a>";
                        }
                    }

                    /* 处理保存按钮.*/
                    if (btnLab.SaveEnable)
                    {
                        toolbar += "<a data-role='button' name='Save'    value='" + btnLab.SaveLab + "' enable=true onclick=\"   if(SysCheckFrm()==false) return false; SaveIt();\" ></a>";
                    }
                }

                if (btnLab.WorkCheckEnable)
                {
                    /*审核*/
                    string urlr1 = "./WorkOpt/WorkCheck.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' id='Btn_WorkCheck'   value='" + btnLab.WorkCheckLab + "' enable=true onclick=\"WinOpen('" + urlr1 + "','dsdd'); \" ></a>";
                }

                if (btnLab.ThreadEnable)
                {
                    /*如果要查看子线程.*/
                    string ur2 = "./WorkOpt/ThreadDtl.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' value='" + btnLab.ThreadLab + "' enable=true onclick=\"WinOpen('" + ur2 + "'); \" ></a>";
                }

                if (btnLab.TCEnable == true)
                {
                    /*流转自定义..*/
                    string ur3 = "./WorkOpt/TransferCustom.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' type=button  value='" + btnLab.TCLab + "' enable=true onclick=\"To('" + ur3 + "'); \" ></a>";
                }



                if (btnLab.JumpWayEnable)
                {
                    /*如果没有焦点字段*/
                    string urlr = "./WorkOpt/JumpWay.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' type=button  value='" + btnLab.JumpWayLab + "' enable=true onclick=\"To('" + urlr + "'); \" ></a>";
                }

                if (btnLab.ReturnEnable)
                {
                    /*如果没有焦点字段*/
                    string urlr = "./WorkOpt/ReturnWork.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' name='Return' type=button  value='" + btnLab.ReturnLab + "' enable=true onclick=\"ReturnWork('" + urlr + "','" + btnLab.ReturnField + "'); \" ></a>";
                }

                //  if (btnLab.HungEnable && this.currND.IsStartNode == false)
                if (btnLab.HungEnable)
                {
                    /*挂起*/
                    string urlr = "./WorkOpt/HungUp.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' type=button  value='" + btnLab.HungLab + "' enable=true onclick=\"WinOpen('" + urlr + "'); \" ></a>";
                }

                if (btnLab.ShiftEnable)
                {
                    /*移交*/
                    string url12 = "./WorkOpt/Forward.htm?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&Info=" + "移交原因.";
                    toolbar += "<a data-role='button' name='Shift' type=button  value='" + btnLab.ShiftLab + "' enable=true onclick=\"To('" + url12 + "'); \" ></a>";
                }

                if ((btnLab.CCRole == CCRole.HandCC || btnLab.CCRole == CCRole.HandAndAuto))
                {
                    /* 抄送 */
                    toolbar += "<a data-role='button' name='CC' type=button  value='" + btnLab.CCLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/CC.htm?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&s=" + tKey + "','ds'); \" ></a>";
                }

                if (btnLab.DeleteEnable != 0)
                {
                    string urlrDel = appPath + "WF/MyFlowInfo.htm?DoType=DeleteFlow&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' name='Delete' type=button  value='" + btnLab.DeleteLab + "' enable=true onclick=\"To('" + urlrDel + "'); \" ></a>";
                }

                if (btnLab.EndFlowEnable && this.currND.IsStartNode == false)
                {
                    toolbar += "<a data-role='button' type=button name='EndFlow'  value='" + btnLab.EndFlowLab + "' enable=true onclick=\"DoStop('" + btnLab.EndFlowLab + "','" + this.FK_Flow + "','" + this.WorkID + "');\" ></a>";
                }

                if (btnLab.PrintDocEnable)
                {
                    string urlr = appPath + "WF/WorkOpt/PrintDoc.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' type=button name='PrintDoc' value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" ></a>";

                }

                if (btnLab.TrackEnable)
                    toolbar += "<a data-role='button' type=button name='Track'  value='" + btnLab.TrackLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" ></a>";


                if (btnLab.SearchEnable)
                    toolbar += "<a data-role='button' type=button name='Search'  value='" + btnLab.SearchLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Rpt/Search.htm?EnsName=ND" + int.Parse(this.FK_Flow) + "MyRpt&FK_Flow=" + this.FK_Flow + "&s=" + tKey + "','dsd0'); \" ></a>";

                if (btnLab.BatchEnable)
                {
                    /*批量处理*/
                    string urlr = appPath + "WF/Batch.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' type=button name='Batch' value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" ></a>";
                }

                if (btnLab.AskforEnable)
                {
                    /*加签 */
                    string urlr3 = appPath + "WF/WorkOpt/Askfor.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' type=button name='Askfor'  value='" + btnLab.AskforLab + "' enable=true onclick=\"To('" + urlr3 + "'); \" ></a>";
                }

                if (btnLab.HuiQianRole != HuiQianRole.None)
                {
                    /*会签 */
                    string urlr3 = appPath + "WF/WorkOpt/HuiQian.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' type=button name='HuiQian'  value='" + btnLab.HuiQianLab + "' enable=true onclick=\"To('" + urlr3 + "'); \" ></a>";
                }


                if (btnLab.WebOfficeWorkModel == WebOfficeWorkModel.Button)
                {
                    /*公文正文 */
                    string urlr = appPath + "WF/WorkOpt/WebOffice.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' type=button name='WebOffice'  value='" + btnLab.WebOfficeLab + "' enable=true onclick=\"WinOpen('" + urlr + "','公文正文'); \" ></a>";
                }

                if (this.currFlow.IsResetData == true && this.currND.IsStartNode)
                {
                    /* 启用了数据重置功能 */
                    string urlr3 = appPath + "WF/MyFlow.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&IsDeleteDraft=1&s=" + tKey;
                    toolbar += "<a data-role='button' type=button  value='数据重置' enable=true onclick=\"To('" + urlr3 + "','ds'); \" ></a>";
                }

                //if (1==2 && btnLab.SubFlowEnable == true)
                //{
                //    /* 子流程 */
                //    string urlr3 = appPath + "WF/WorkOpt/SubFlow.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                //    toolbar += "<a data-role='button' type=button name='SubFlow'  value='" + btnLab.SubFlowLab + "' enable=true onclick=\"WinOpen('" + urlr3 + "'); \" ></a>";
                //}

                if (btnLab.CHRole != 0)
                {
                    /* 节点时限设置 */
                    string urlr3 = appPath + "WF/WorkOpt/CH.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' type=button name='CH'  value='" + btnLab.CHLab + "' enable=true onclick=\"WinShowModalDialog('" + urlr3 + "'); \" ></a>";
                }



                if (btnLab.PRIEnable == true)
                {
                    /* 优先级设置 */
                    string urlr3 = appPath + "WF/WorkOpt/PRI.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' type=button name='PR'  value='" + btnLab.PRILab + "' enable=true onclick=\"WinShowModalDialog('" + urlr3 + "'); \" ></a>";
                }

                /* 关注 */
                if (btnLab.FocusEnable == true)
                {
                    if (HisGenerWorkFlow.Paras_Focus == true)
                        toolbar += "<a data-role='button' type=button  value='取消关注' enable=true onclick=\"FocusBtn(this,'" + this.WorkID + "'); \" ></a>";
                    else
                        toolbar += "<a data-role='button' type=button name='Focus' value='" + btnLab.FocusLab + "' enable=true onclick=\"FocusBtn(this,'" + this.WorkID + "'); \" ></a>";
                }

                /* 分配工作 */
                if (btnLab.AllotEnable == true)
                {
                    /*分配工作*/
                    string urlAllot = "./WorkOpt/AllotTask.htm?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&Info=" + "移交原因.";
                    toolbar += "<a data-role='button' name='Allot' type=button  value='" + btnLab.AllotLab + "' enable=true onclick=\"To('" + urlAllot + "'); \" ></a>";
                }

                /* 确认 */
                if (btnLab.ConfirmEnable == true)
                {
                    if (HisGenerWorkFlow.Paras_Confirm == true)
                        toolbar += "<a data-role='button' type=button  value='取消确认' enable=true onclick=\"ConfirmBtn(this,'" + this.WorkID + "'); \" ></a>";
                    else
                        toolbar += "<a data-role='button' type=button name='Confirm' value='" + btnLab.ConfirmLab + "' enable=true onclick=\"ConfirmBtn(this,'" + this.WorkID + "'); \" ></a>";
                }

                if (SystemConfig.CustomerNo != "XJTY")
                {
                    /* 打包下载zip */
                    if (btnLab.PrintZipEnable == true)
                    {
                        string packUrl = "./WorkOpt/Packup.htm?FileType=zip&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
                        toolbar += "<input type=button name='PackUp_zip'  value='" + btnLab.PrintZipLab + "' enable=true/>";
                    }

                    /* 打包下载html */
                    if (btnLab.PrintHtmlEnable == true)
                    {
                        string packUrl = "./WorkOpt/Packup.htm?FileType=html&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
                        toolbar += "<input type=button name='PackUp_html'  value='" + btnLab.PrintHtmlLab + "' enable=true/>";
                    }

                    /* 打包下载pdf */
                    if (btnLab.PrintPDFEnable == true)
                    {
                        string packUrl = "./WorkOpt/Packup.htm?FileType=pdf&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
                        toolbar += "<input type=button name='PackUp_pdf'  value='" + btnLab.PrintPDFLab + "' enable=true/>";
                    }
                }

                ///* 打包下载 */
                //if (btnLab.PrintZipEnable == true)
                //{
                //    string packUrl = "./WorkOpt/Packup.htm?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow;
                //    toolbar += "<a data-role='button' type=button name='PackUp'  value='" + btnLab.PrintZipLab + "' enable=true></a>";
                //}

                #endregion

                #region  //加载自定义的button.
                BP.WF.Template.NodeToolbars bars = new NodeToolbars();
                bars.Retrieve(NodeToolbarAttr.FK_Node, this.FK_Node);
                foreach (NodeToolbar bar in bars)
                {
                    if (bar.ShowWhere != ShowWhere.Toolbar)
                        continue;

                    //如果是script.
                    if (bar.ExcType == 1 || (!DataType.IsNullOrEmpty(bar.Target) && bar.Target.ToLower() == "javascript"))
                    {
                        toolbar += "<a data-role='button' type=button  value='" + bar.Title + "' enable=true onclick=\"" + bar.Url + "\" ></a>";
                    }
                    else
                    {
                        string urlr3 = bar.Url + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                        toolbar += "<a data-role='button' type=button  value='" + bar.Title + "' enable=true onclick=\"WinOpen('" + urlr3 + "'); \" ></a>";
                    }
                }
                #endregion  //加载自定义的button.

            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex);
                toolbar = "err@" + ex.Message;
            }
            return toolbar;
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

                if (key.Contains("TB_"))
                {

                    string val = HttpContextHelper.RequestParams(key);
                    if (htMain.ContainsKey(key.Replace("TB_", "")) == false)
                    {
                        val = HttpUtility.UrlDecode(val, Encoding.UTF8);
                        htMain.Add(key.Replace("TB_", ""), val);
                    }
                    continue;
                }

                if (key.Contains("DDL_"))
                {
                    htMain.Add(key.Replace("DDL_", ""), HttpContextHelper.RequestParams(key));
                    continue;
                }

                if (key.Contains("CB_"))
                {
                    htMain.Add(key.Replace("CB_", ""), HttpContextHelper.RequestParams(key));
                    continue;
                }

                if (key.Contains("RB_"))
                {
                    htMain.Add(key.Replace("RB_", ""), HttpContextHelper.RequestParams(key));
                    continue;
                }
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
                return BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.FK_Flow, this.WorkID, true);
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
                    return "该流程的工作已删除,请联系管理员";

                objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, ht, null, this.ToNode, null);
                msg = objs.ToMsgOfHtml();
                BP.WF.Glo.SessionMsg = msg;

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

                        if (myurl.Contains("PWorkID") == false)
                            myurl += "&PWorkID=" + this.WorkID;

                        myurl += "&FromFlow=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
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

                        //        url += "&PFlowNo=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&PWorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
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
                if (ex.Message.Contains("请选择下一步骤工作") == true || ex.Message.Contains("用户没有选择发送到的节点") == true)
                {
                    if (this.currND.CondModel == CondModel.ByUserSelected)
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
                            if (this.HisGenerWorkFlow.TodoEmps.Contains(empStr) == false)
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
                return ex.Message;
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

            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
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
                    this.WorkID, this.GetMainTableHT(), null);

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
            //            if (wkattr.Key.Equals(StartWorkAttr.OID) || wkattr.Key.Equals(StartWorkAttr.FID) || wkattr.Key.Equals(StartWorkAttr.CDT)
            //                || wkattr.Key.Equals(StartWorkAttr.RDT) || wkattr.Key.Equals(StartWorkAttr.MD5) || wkattr.Key.Equals(StartWorkAttr.Emps)
            //                || wkattr.Key.Equals(StartWorkAttr.FK_Dept) || wkattr.Key.Equals(StartWorkAttr.PRI) || wkattr.Key.Equals(StartWorkAttr.Rec)
            //                || wkattr.Key.Equals(StartWorkAttr.Title) || wkattr.Key.Equals(Data.GERptAttr.FK_NY) || wkattr.Key.Equals(Data.GERptAttr.FlowEmps)
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
                wk.SetValByKey(BP.WF.Data.GERptAttr.Title, title);
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
                    gwf.RDT = DataType.CurrentDataTimess;
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
                    gwl.DTOfWarning = DataType.CurrentDataTimess;
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
            {
                frms = gwf.Paras_Frms;
            }
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
                        {
                            continue;
                            //return "err@当前表单设置为仅有参数的时候启用,但是没有传递来参数.";
                        }

                        if (frms.Contains(",") == false)
                        {
                            if (frms != frmNode.FK_Frm)
                                continue;
                        }

                        if (frms.Contains(",") == true)
                        {
                            if (frms.Contains(frmNode.FK_Frm + ",") == false)
                                continue;
                        }

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
                    //@袁丽娜
                    case FrmEnableRole.ByStation:
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
                    //@袁丽娜
                    case FrmEnableRole.ByDept:
                        exp = frmNode.FrmEnableExp.Clone() as string;
                        Sql = "SELECT FK_Dept FROM Port_DeptEmp where FK_Emp='" + WebUser.No + "'";
                        string dept = DBAccess.RunSQLReturnString(Sql);
                        if (DataType.IsNullOrEmpty(dept) == true)
                            continue;
                        string[] depts = dept.Split(';');
                        isExit = false;
                        foreach (string s in depts)
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
                    Entity fn = frmNodes.GetEntityByKey(FrmNodeAttr.FK_Frm, md.No);
                    if (fn != null)
                    {
                        string str = fn.GetValStrByKey(FrmNodeAttr.FrmNameShow);
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

            //扩展工具，显示位置为表单树类型. 
            #warning 不再支持工具栏的连接，可以使用表单来完成，实现该功能。
            //NodeToolbars extToolBars = new NodeToolbars();
            //extToolBars.Retrieve(NodeToolbarAttr.FK_Node, this.FK_Node, NodeToolbarAttr.ShowWhere, (int)ShowWhere.Tree);

            //foreach (NodeToolbar item in extToolBars)
            //{
            //    string url = "";
            //    if (DataType.IsNullOrEmpty(item.Url))
            //        continue;

            //    url = item.Url;

            //    BP.WF.Template.FlowFormTree formTree = new BP.WF.Template.FlowFormTree();
            //    formTree.No = item.OID.ToString();
            //    formTree.ParentNo = "1";
            //    formTree.Name = item.Title;
            //    formTree.NodeType = "tools|0";
            //    if (!DataType.IsNullOrEmpty(item.Target) && item.Target.ToUpper() == "_BLANK")
            //    {
            //        formTree.NodeType = "tools|1";
            //    }

            //    formTree.Url = url;
            //    appFlowFormTree.AddEntity(formTree);
            //}

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
                    if (SystemConfig.SysNo == "YYT")
                    {
                        ico = "icon-boat_16";
                    }
                    url = url.Replace("/", "|");
                    appendMenuSb.Append(",\"attributes\":{\"NodeType\":\"" + formTree.NodeType + "\",\"IsEdit\":\"" + formTree.IsEdit + "\",\"IsCloseEtcFrm\":\"" + formTree.IsCloseEtcFrm + "\",\"Url\":\"" + url + "\"}");
                    //图标
                    if (formTree.NodeType == "form|0")
                    {
                        ico = "form0";
                        if (SystemConfig.SysNo == "YYT")
                        {
                            ico = "icon-Wave";
                        }
                    }
                    if (formTree.NodeType == "form|1")
                    {
                        ico = "form1";
                        if (SystemConfig.SysNo == "YYT")
                        {
                            ico = "icon-Shark_20";
                        }
                    }
                    if (formTree.NodeType.Contains("tools"))
                    {
                        ico = "icon-4";
                        if (SystemConfig.SysNo == "YYT")
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
            try
            {
                DataSet ds = new DataSet();

                ds = BP.WF.CCFlowAPI.GenerWorkNode(this.FK_Flow, this.FK_Node, this.WorkID,
                    this.FID, BP.Web.WebUser.No);

                //Node nd = new Node(this.FK_Node);
                //if (nd.HisFormType == NodeFormType.SheetTree)
                //{
                //    /*把树形表单的表单信息加载到ds里面.*/
                //}
                //把他转化小写,适应多个数据库.
                //   wf_generWorkFlowDt = DBAccess.ToLower(wf_generWorkFlowDt);
                // ds.Tables.Add(wf_generWorkFlowDt);
                // ds.WriteXml("c:\\xx.xml");

                #region 如果是移动应用就考虑多表单的问题.
                if (currND.HisFormType == NodeFormType.SheetTree && this.IsMobile == true)
                {
                    /*如果是表单树并且是，移动模式.*/


                    FrmNodes fns = new FrmNodes();
                    QueryObject qo = new QueryObject(fns);

                    qo.AddWhere(FrmNodeAttr.FK_Node, currND.NodeID);
                    qo.addAnd();
                    qo.AddWhere(FrmNodeAttr.FrmEnableRole, "!=", (int)FrmEnableRole.Disable);
                    qo.addOrderBy("Idx");
                    qo.DoQuery();


                    //把节点与表单的关联管理放入到系统.
                    ds.Tables.Add(fns.ToDataTableField("FrmNodes"));
                }
                #endregion 如果是移动应用就考虑多表单的问题.

                if (WebUser.SysLang.Equals("CH") == true)
                    return BP.Tools.Json.ToJson(ds);

                #region 处理多语言.
                if (WebUser.SysLang.Equals("CH") == false)
                {
                    Langues langs = new Langues();
                    langs.Retrieve(LangueAttr.Model, LangueModel.CCForm,
                        LangueAttr.Sort, "Fields", LangueAttr.Langue, WebUser.SysLang); //查询语言.
                }
                #endregion 处理多语言.

                return BP.Tools.Json.ToJson(ds);


            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex);
                return "err@" + ex.Message;
            }
        }


    }
}
