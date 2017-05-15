using System;
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
    public class WF_MyFlow : WebContralBase
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

                if (string.IsNullOrEmpty(str) == false)
                {
                    string myps = str;

                    if (myps.Contains("IsCC=1") == true)
                        return true;
                }

                str = this.GetRequestVal("AtPara");
                if (string.IsNullOrEmpty(str) == false)
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
                if (string.IsNullOrEmpty(fk_nodeReq))
                    fk_nodeReq = this.GetRequestVal("NodeID");// this.Request.Form["NodeID"];

                if (string.IsNullOrEmpty(fk_nodeReq) == false)
                    return int.Parse(fk_nodeReq);

                if (_FK_Node == 0)
                {
                    if (this.GetRequestVal("WorkID") != null)
                    {
                        string sql = "SELECT FK_Node from  WF_GenerWorkFlow where WorkID=" + this.WorkID;
                        _FK_Node = DBAccess.RunSQLReturnValInt(sql);
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
        public bool isAskFor = false;

        //杨玉慧
        public string DoType1 {
            get { return this.context.Request.Params["DoType1"]; }
        }
        #endregion

        public string Focus()
        {
            BP.WF.Dev2Interface.Flow_Focus( this.WorkID);
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
        /// 初始化(处理分发)
        /// </summary>
        /// <returns></returns>
        public string MyFlow_Init()
        {
            if (this.WorkID != 0)
            {
                //判断是否有执行该工作的权限.
                bool isCanDo = Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Flow, this.FK_Node, this.WorkID, BP.Web.WebUser.No);
                if (isCanDo == false)
                    return "err@您不能执行当前工作.";
            }

            //当前工作.
            Work currWK = this.currND.HisWork;
            GenerWorkFlow gwf = new GenerWorkFlow();
            if (this.WorkID != 0)
            {
                gwf = new GenerWorkFlow();
                gwf.WorkID = this.WorkID;
                if (gwf.RetrieveFromDBSources() == 0)
                {
                    return ("err@该流程ID{" + this.WorkID + "}不存在，或者已经被删除.");
                }
            }
            #region 判断前置导航.

            if (this.currND.IsStartNode && this.IsCC == false && this.WorkID == 0)
            {
                if (BP.WF.Dev2Interface.Flow_IsCanStartThisFlow(this.FK_Flow, WebUser.No) == false)
                {
                    /*是否可以发起流程？*/
                    return "err@您(" + BP.Web.WebUser.No + ")没有发起或者处理该流程的权限.";
                }
            }

            if (this.WorkID == 0 && this.currND.IsStartNode && this.context.Request.QueryString["IsCheckGuide"] == null)
            {
                Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow);
                switch (this.currFlow.StartGuideWay)
                {
                    case StartGuideWay.None:
                        break;
                    case StartGuideWay.SubFlowGuide:
                    case StartGuideWay.SubFlowGuideEntity:
                        return "url@StartGuide.htm?FK_Flow=" + this.currFlow.No + "&WorkID=" + workid;
                    case StartGuideWay.ByHistoryUrl: // 历史数据.
                        if (this.currFlow.IsLoadPriData == true)
                        {
                            return "err@流程配置错误，您不能同时启用前置导航，自动装载上一笔数据两个功能。";
                        }
                        return "url@StartGuide.htm?FK_Flow=" + this.currFlow.No + "&WorkID=" + workid;
                    case StartGuideWay.BySystemUrlOneEntity:
                    case StartGuideWay.BySQLOne:
                        return "url@StartGuideEntities.htm?FK_Flow=" + this.currFlow.No + "&WorkID=" + workid;
                    case StartGuideWay.BySelfUrl: //按照定义的url.
                        return "url@" + this.currFlow.StartGuidePara1 + this.RequestParas + "&WorkID=" + workid;
                    case StartGuideWay.ByFrms: //选择表单.
                        return "url@./WorkOpt/StartGuideFrms.htm?FK_Flow=" + this.currFlow.No + "&WorkID=" + workid;
                    default:
                        break;
                }
            }

            //string appPath = BP.WF.Glo.CCFlowAppPath; //this.Request.ApplicationPath;
            //this.Page.Title = "第" + this.currND.Step + "步:" + this.currND.Name;
            #endregion 判断前置导航

            #region 处理表单类型.
            if (this.currND.HisFormType == NodeFormType.SheetTree
                 || this.currND.HisFormType == NodeFormType.SheetAutoTree)
            {
                /*如果是多表单流程.*/
                string pFlowNo = this.GetRequestVal("PFlowNo");
                string pWorkID = this.GetRequestVal("PWorkID");
                string pNodeID = this.GetRequestVal("PNodeID");
                string pEmp = this.GetRequestVal("PEmp");
                if (string.IsNullOrEmpty(pEmp))
                    pEmp = WebUser.No;

                if (this.WorkID == 0)
                {
                    if (string.IsNullOrEmpty(pFlowNo) == true)
                        this.WorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, null, null, WebUser.No, null);
                    else
                        this.WorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, null, null, WebUser.No, null, Int64.Parse(pWorkID), 0, pFlowNo, int.Parse(pNodeID));

                    currWK = currND.HisWork;
                    currWK.OID = this.WorkID;
                    currWK.Retrieve();
                    this.WorkID = currWK.OID;
                }
                else
                {
                    gwf.WorkID = this.WorkID;
                    gwf.RetrieveFromDBSources();
                    pFlowNo = gwf.PFlowNo;
                    pWorkID = gwf.PWorkID.ToString();
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

                string toUrl = "";
                if (this.currND.HisFormType == NodeFormType.SheetTree || this.currND.HisFormType == NodeFormType.SheetAutoTree)
                    toUrl = "./FlowFormTree/Default.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&SID=" + WebUser.SID + "&PFlowNo=" + pFlowNo + "&PWorkID=" + pWorkID;
                else
                    toUrl = "./WebOffice/Default.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&SID=" + WebUser.SID + "&PFlowNo=" + pFlowNo + "&PWorkID=" + pWorkID;

                string[] ps = this.RequestParas.Split('&');
                foreach (string s in ps)
                {
                    if (string.IsNullOrEmpty(s))
                        continue;
                    if (toUrl.Contains(s))
                        continue;
                    toUrl += "&" + s;
                }

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

                //// 加入设置父子流程的参数.
                //toUrl += "&DoFunc=" + this.DoFunc;
                //toUrl += "&CFlowNo=" + this.CFlowNo;
                //toUrl += "&Nos=" + this.Nos;
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
                if (string.IsNullOrEmpty(url))
                {
                    return "err@设置读取状流程设计错误态错误,没有设置表单url.";
                }

                //处理连接.
                url = this.MyFlow_Init_DealUrl(currND, currWK);

                //sdk表单就让其跳转.
                return "url@" + url;
            }

            if (this.currND.HisFormType == NodeFormType.FixForm)
            {
                /*如果是傻瓜表单，就转到傻瓜表单的解析执行器上。*/
                if (this.WorkID == 0)
                {
                    currWK = this.currFlow.NewWork();
                    this.WorkID = currWK.OID;
                }

                string url = "MyFlowFool.htm";

                //处理连接.
                url = this.MyFlow_Init_DealUrl(currND, currWK, url);
                return "url@" + url;
            }

            if (this.currND.HisFormType == NodeFormType.FoolTruck)
            {
                /*如果是傻瓜表单，就转到傻瓜表单的解析执行器上，为软通动力改造。*/
                if (this.WorkID == 0)
                {
                    currWK = this.currFlow.NewWork();
                    this.WorkID = currWK.OID;
                }

                string url = "MyFlowFoolTruck.htm";

                //处理连接.
                url = this.MyFlow_Init_DealUrl(currND, currWK, url);
                return "url@" + url;
            }

            #endregion 处理表单类型.

            /*如果是傻瓜表单，就转到傻瓜表单的解析执行器上，为软通动力改造。*/
            if (this.WorkID == 0)
            {
                currWK = this.currFlow.NewWork();
                this.WorkID = currWK.OID;
            }

            string myurl = "MyFlow.aspx";
            if (Glo.IsBeta==true)
                myurl = "MyFlowFree.htm";

            //处理连接.
            myurl = this.MyFlow_Init_DealUrl(currND, currWK, myurl);
            myurl = myurl.Replace("DoType=MyFlow_Init&", "");
            myurl = myurl.Replace("&DoWhat=StartClassic", "");
            return "url@" + myurl;
        }
        public string MyFlow_Init_DealUrl(BP.WF.Node currND, Work currWK, string url = null)
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
                urlExt += "&FID=" + currWK.FID;

            if (urlExt.Contains("&UserNo") == false)
                urlExt += "&UserNo=" + WebUser.No;

            if (urlExt.Contains("&SID") == false)
                urlExt += "&SID=" + WebUser.SID;

            if (url.Contains("?") == true)
                url += "&" + urlExt;
            else
                url += "?" + urlExt;

            url = url.Replace("?&", "?");
            url = url.Replace("&&", "&");
            return url;
        }
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_MyFlow(HttpContext mycontext)
        {
            this.context = mycontext;
        }
        /// <summary>
        /// 工具栏
        /// </summary>
        /// <returns></returns>
        public string InitToolBar()
        {
            string tKey = DateTime.Now.ToString();
            string toolbar = "";
            try
            {
                if (this.IsCC)
                {
                    toolbar += "<input type=button  value='流程运行轨迹' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/OneWork/ChartTrack.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />";
                    // 判断审核组件在当前的表单中是否启用，如果启用了.
                    FrmWorkCheck fwc = new FrmWorkCheck(this.FK_Node);
                    if (fwc.HisFrmWorkCheckSta != FrmWorkCheckSta.Enable)
                    {
                        /*如果不等于启用, */
                        toolbar += "<input type=button  value='填写审核意见' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/CCCheckNote.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />";
                    }
                    return toolbar;
                }

                #region 加载流程控制器 - 按钮
                BtnLab btnLab = new BtnLab(this.FK_Node);
                if (this.currND.HisFormType == NodeFormType.SelfForm)
                {
                    /*如果是嵌入式表单.*/
                    if (currND.IsEndNode && isAskFor == false)
                    {
                        /*如果当前节点是结束节点.*/
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
                        {
                            /*如果启用了发送按钮.*/
                            toolbar += "<input name='Send' type=button value='" + btnLab.SendLab + "' enable=true onclick=\"" + btnLab.SendJS + " if (SendSelfFrom()==false) return false; Send(); this.disabled=true;\" />";
                        }
                    }
                    else
                    {
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group && isAskFor == false)
                        {
                            /*如果启用了发送按钮.*/
                            if (btnLab.SelectAccepterEnable == 2)
                            {
                                /*如果启用了选择人窗口的模式是【选择既发送】.*/
                                toolbar += "<input name='Send' type=button  value='" + btnLab.SendLab + "' enable=true onclick=\"javascript:OpenSelectAccepter('" + this.FK_Flow + "','" + this.FK_Node + "','" + this.WorkID + "','" + this.FID + "');" + btnLab.SendJS + " if ( SendSelfFrom()==false) return false;this.disabled=true;\" />";
                            }
                            else
                            {
                                toolbar += "<input name='Send' type=button  value='" + btnLab.SendLab + "' enable=true onclick=\"" + btnLab.SendJS + " if ( SendSelfFrom()==false) return false; Send(); this.disabled=true;\" />";
                            }
                        }
                    }

                    /*处理保存按钮.*/
                    if (btnLab.SaveEnable && isAskFor == false)
                    {
                        toolbar += "<input name='Save' type=button value='" + btnLab.SaveLab + "' enable=true onclick=\"SaveSelfFrom();\" />";
                    }
                }
                else
                {
                    /*启用了其他的表单.*/
                    if (currND.IsEndNode && isAskFor == false)
                    {
                        /*如果当前节点是结束节点.*/
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
                        {
                            /*如果启用了选择人窗口的模式是【选择既发送】.*/
                            toolbar += "<input name='Send' type=button value='" + btnLab.SendLab + "' enable=true onclick=\" " + btnLab.SendJS + " if(SysCheckFrm()==false) return false;SaveDtlAll();KindEditerSync();Send(); \" />";
                        }
                    }
                    else
                    {
                        if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group && isAskFor == false)
                        {
                            /*如果启用了发送按钮.
                             * 1. 如果是加签的状态，就不让其显示发送按钮，因为在加签的提示。
                             */
                            if (btnLab.SelectAccepterEnable == 2)
                            {
                                /*如果启用了选择人窗口的模式是【选择既发送】.*/
                                toolbar += "<input name='Send' type=button  value='" + btnLab.SendLab + "' enable=true onclick=\"if(SysCheckFrm()==false) return false;KindEditerSync();if (OpenSelectAccepter('" + this.FK_Flow + "','" + this.FK_Node + "','" + this.WorkID + "','" + this.FID + "')==false) return false; Send(); \" />";
                            }
                            else
                            {
                                toolbar += "<input name='Send' type=button  value='" + btnLab.SendLab + "' enable=true onclick=\" " + btnLab.SendJS + " if(SysCheckFrm()==false) return false;KindEditerSync();Send();\" />";
                            }
                        }
                    }

                    /* 处理保存按钮.*/
                    if (btnLab.SaveEnable && isAskFor == false)
                    {
                        toolbar += "<input name='Save' type=button  value='" + btnLab.SaveLab + "' enable=true onclick=\"   if(SysCheckFrm()==false) return false;KindEditerSync();Save();\" />";
                    }
                }

                if (btnLab.WorkCheckEnable && isAskFor == false)
                {
                    /*审核*/
                    string urlr1 = appPath + "WF/WorkOpt/WorkCheck.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button  value='" + btnLab.WorkCheckLab + "' enable=true onclick=\"WinOpen('" + urlr1 + "','dsdd'); \" />";
                }

                if (btnLab.ThreadEnable)
                {
                    /*如果要查看子线程.*/
                    string ur2 = appPath + "WF/WorkOpt/ThreadDtl.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button  value='" + btnLab.ThreadLab + "' enable=true onclick=\"WinOpen('" + ur2 + "'); \" />";
                }

                if (btnLab.TCEnable == true && isAskFor == false)
                {
                    /*流转自定义..*/
                    string ur3 = appPath + "WF/WorkOpt/TransferCustom.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button  value='" + btnLab.TCLab + "' enable=true onclick=\"To('" + ur3 + "'); \" />";
                }

                if (btnLab.JumpWayEnable && isAskFor == false)
                {
                    /*如果没有焦点字段*/
                    string urlr = appPath + "WF/JumpWay.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button  value='" + btnLab.JumpWayLab + "' enable=true onclick=\"To('" + urlr + "'); \" />";
                }

                if (btnLab.ReturnEnable && isAskFor == false && this.currND.IsStartNode == false)
                {
                    /*如果没有焦点字段*/
                    string urlr = appPath + "WF/WorkOpt/ReturnWork.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input name='Return' type=button  value='" + btnLab.ReturnLab + "' enable=true onclick=\"ReturnWork('" + urlr + "','" + btnLab.ReturnField + "'); \" />";
                }

                //  if (btnLab.HungEnable && this.currND.IsStartNode == false)
                if (btnLab.HungEnable)
                {
                    /*挂起*/
                    string urlr = appPath + "WF/WorkOpt/HungUp.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button  value='" + btnLab.HungLab + "' enable=true onclick=\"WinOpen('" + urlr + "'); \" />";
                }

                if (btnLab.ShiftEnable && isAskFor == false)
                {
                    /*移交*/
                    string url12 = "./WorkOpt/Forward.htm?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&Info=" + "移交原因.";
                    toolbar += "<input name='Shift' type=button  value='" + btnLab.ShiftLab + "' enable=true onclick=\"To('" + url12 + "'); \" />";
                }

                if ((btnLab.CCRole == CCRole.HandCC || btnLab.CCRole == CCRole.HandAndAuto))
                {
                    /* 抄送 */
                    toolbar += "<input name='CC' type=button  value='" + btnLab.CCLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/CC.htm?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&s=" + tKey + "','ds'); \" />";
                }

                if (btnLab.DeleteEnable != 0 && isAskFor == false)
                {
                    /*流程删除规则 */
                    switch (this.currND.HisDelWorkFlowRole)
                    {
                        case DelWorkFlowRole.None: /*不删除*/
                            break;
                        case DelWorkFlowRole.ByUser: //需要交互.
                        case DelWorkFlowRole.DeleteAndWriteToLog:
                        case DelWorkFlowRole.DeleteByFlag:
                        case DelWorkFlowRole.DeleteReal: // 不需要交互，直接干净的删除.
                            string urlrDel = appPath + "WF/MyFlowInfo.htm?DoType=DeleteFlow&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                            toolbar += "<input name='Delete' type=button  value='" + btnLab.DeleteLab + "' enable=true onclick=\"To('" + urlrDel + "'); \" />";
                            break;
                        //case DelWorkFlowRole.DeleteReal: // 不需要交互，直接干净的删除.
                        //    toolbar.AddBtn("Btn_Delete", btnLab.DeleteLab);
                        //    this.Btn_Delete.OnClientClick = "return confirm('将要执行删除流程，您确认吗?')";
                        //    this.Btn_Delete.Click += new System.EventHandler(ToolBar1_ButtonClick);
                        //    break;
                        default:
                            break;
                    }
                }

                if (btnLab.EndFlowEnable && this.currND.IsStartNode == false && isAskFor == false)
                {
                    toolbar += "<input type=button name='EndFlow'  value='" + btnLab.EndFlowLab + "' enable=true onclick=\"To('./WorkOpt/StopFlow.htm?&DoType=StopFlow&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey + "'); \" />";
                }

                if (btnLab.PrintDocEnable && isAskFor == false)
                {
                    /*如果不是加签 */
                    if (this.currND.HisPrintDocEnable == PrintDocEnable.PrintRTF)
                    {
                        string urlr = appPath + "WF/WorkOpt/PrintDoc.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                        toolbar += "<input type=button name='PrintDoc' value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" />";
                    }

                    if (this.currND.HisPrintDocEnable == PrintDocEnable.PrintWord)
                    {
                        string urlr = appPath + "WF/Rpt/RptDoc.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&IsPrint=1&s=" + tKey;
                        toolbar += "<input type=button name='PrintDoc'  value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" />";
                    }

                    if (this.currND.HisPrintDocEnable == PrintDocEnable.PrintHtml)
                    {
                        toolbar += "<input type=button  name='PrintDoc' value='" + btnLab.PrintDocLab + "' enable=true onclick=\"printFrom(); \" />";
                    }
                }

                if (btnLab.TrackEnable && isAskFor == false)
                    toolbar += "<input type=button name='Track'  value='" + btnLab.TrackLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/OneWork/ChartTrack.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />";

                switch (btnLab.SelectAccepterEnable)
                {
                    case 1:
                        if (isAskFor == false)
                            toolbar += "<input type=button name='SelectAccepter'  value='" + btnLab.SelectAccepterLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/Accepter.htm?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&s=" + tKey + "','dds'); \" />";
                        break;
                    case 2:
                        //  toolbar.Add("<input type=button  value='" + btnLab.SelectAccepterLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Accepter.htm?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&s=" + tKey + "','dds'); \" />");
                        break;
                    default:
                        break;
                }

                if (btnLab.SearchEnable && isAskFor == false)
                    toolbar += "<input type=button name='Search'  value='" + btnLab.SearchLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Rpt/Search.htm?EnsName=ND" + int.Parse(this.FK_Flow) + "MyRpt&FK_Flow=" + this.FK_Flow + "&s=" + tKey + "','dsd0'); \" />";

                if (btnLab.BatchEnable && isAskFor == false)
                {
                    /*批量处理*/
                    string urlr = appPath + "WF/Batch.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='Batch' value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" />";
                }

                if (btnLab.AskforEnable && HisGenerWorkFlow != null && HisGenerWorkFlow.WFState != WFState.Askfor)
                {
                    /*加签 */
                    string urlr3 = appPath + "WF/WorkOpt/Askfor.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='Askfor'  value='" + btnLab.AskforLab + "' enable=true onclick=\"To('" + urlr3 + "'); \" />";
                }

                if (btnLab.WebOfficeWorkModel == WebOfficeWorkModel.Button)
                {
                    /*公文正文 */
                    string urlr = appPath + "WF/WorkOpt/WebOffice.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='WebOffice'  value='" + btnLab.WebOfficeLab + "' enable=true onclick=\"WinOpen('" + urlr + "','公文正文'); \" />";
                }

                if (this.currFlow.IsResetData == true && this.currND.IsStartNode)
                {
                    /* 启用了数据重置功能 */
                    string urlr3 = appPath + "WF/MyFlow.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&IsDeleteDraft=1&s=" + tKey;
                    toolbar += "<input type=button  value='数据重置' enable=true onclick=\"To('" + urlr3 + "','ds'); \" />";
                }

                if (btnLab.SubFlowEnable == true)
                {
                    /* 子流程 */
                    string urlr3 = appPath + "WF/WorkOpt/SubFlow.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='SubFlow'  value='" + btnLab.SubFlowLab + "' enable=true onclick=\"WinOpen('" + urlr3 + "'); \" />";
                }

                if (btnLab.CHEnable == true)
                {
                    /* 节点时限设置 */
                    string urlr3 = appPath + "WF/WorkOpt/CH.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='CH'  value='" + btnLab.CHLab + "' enable=true onclick=\"WinShowModalDialog('" + urlr3 + "'); \" />";
                }

                if (btnLab.PRIEnable == true)
                {
                    /* 优先级设置 */
                    string urlr3 = appPath + "WF/WorkOpt/PRI.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='PR'  value='" + btnLab.PRILab + "' enable=true onclick=\"WinShowModalDialog('" + urlr3 + "'); \" />";
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
                    string urlAllot = "./WorkOpt/Allot.htm?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&Info=" + "移交原因.";
                    toolbar += "<input name='Allot' type=button  value='" + btnLab.AllotLab + "' enable=true onclick=\"To('" + urlAllot + "'); \" />";
                }

                //    toolbar += "<input type=button name='Allot' value='" + btnLab.AllotLab + "' enable=true onclick=\"AllotBtn(this,'" + this.WorkID + "'); \" />";
                //}

                /* 确认 */
                if (btnLab.ConfirmEnable == true)
                {
                    if (HisGenerWorkFlow.Paras_Confirm == true)
                        toolbar += "<input type=button  value='取消确认' enable=true onclick=\"ConfirmBtn(this,'" + this.WorkID + "'); \" />";
                    else
                        toolbar += "<input type=button name='Confirm' value='" + btnLab.ConfirmLab + "' enable=true onclick=\"ConfirmBtn(this,'" + this.WorkID + "'); \" />";
                }
                #endregion

                //加载自定义的button.
                BP.WF.Template.NodeToolbars bars = new NodeToolbars();
                bars.Retrieve(NodeToolbarAttr.FK_Node, this.FK_Node);
                foreach (NodeToolbar bar in bars)
                {
                    if (bar.ShowWhere == ShowWhere.Toolbar)
                    {
                        if (!string.IsNullOrEmpty(bar.Target) && bar.Target.ToLower() == "javascript")
                        {
                            toolbar += "<input type=button  value='" + bar.Title + "' enable=true onclick=\"" + bar.Url + "\" />";
                        }
                        else
                        {
                            string urlr3 = bar.Url + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                            toolbar += "<input type=button  value='" + bar.Title + "' enable=true onclick=\"WinOpen('" + urlr3 + "'); \" />";
                        }
                    }
                }
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
            foreach (string key in this.context.Request.Form.Keys)
            {
                if (key == null)
                    continue;

                if (key.Contains("TB_"))
                {
                    htMain.Add(key.Replace("TB_", ""), context.Request.Form[key]);
                    continue;
                }

                if (key.Contains("DDL_"))
                {
                    htMain.Add(key.Replace("DDL_", ""), context.Request.Form[key]);
                    continue;
                }

                if (key.Contains("CB_"))
                {
                    htMain.Add(key.Replace("CB_", ""), context.Request.Form[key]);
                    continue;
                }

                if (key.Contains("RB_"))
                {
                    htMain.Add(key.Replace("RB_", ""), context.Request.Form[key]);
                    continue;
                }
            }
            return htMain;
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
                        return "url@" + myurl;
                    case TurnToDeal.TurnToByCond:
                        TurnTos tts = new TurnTos(this.FK_Flow);
                        if (tts.Count == 0)
                        {
                            BP.WF.Dev2Interface.Port_SendMsg("admin", currFlow.Name + "在" + currND.Name + "节点处，出现错误", "您没有设置节点完成后的转向条件。", "Err" + currND.No + "_" + this.WorkID, SMSMsgType.Err, this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
                            throw new Exception("@您没有设置节点完成后的转向条件。");
                        }

                        foreach (TurnTo tt in tts)
                        {
                            tt.HisWork = currNode.HisWork;
                            if (tt.IsPassed == true)
                            {
                                string url = tt.TurnToURL.Clone().ToString();
                                if (url.Contains("?") == false)
                                    url += "?1=1";
                                Attrs attrs = currNode.HisWork.EnMap.Attrs;
                                Work hisWK1 = currNode.HisWork;
                                foreach (Attr attr in attrs)
                                {
                                    if (url.Contains("@") == false)
                                        break;
                                    url = url.Replace("@" + attr.Key, hisWK1.GetValStrByKey(attr.Key));
                                }
                                if (url.Contains("@"))
                                    throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + url);

                                url += "&PFlowNo=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&PWorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
                                return "url@" + url;
                            }
                        }
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
                    if (this.currND.CondModel == CondModel.ByLineCond)
                    {
                        /*如果抛出异常，我们就让其转入选择到达的节点里, 在节点里处理选择人员. */
                        return "url@./WorkOpt/ToNodes.htm?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID;
                    }

                    BtnLab btn = new BtnLab(this.FK_Node);
                    btn.SelectAccepterEnable = 2;
                    btn.Update();

                    return "err@下一个节点的接收人规则是，当前节点选择来选择，在当前节点属性里您没有启动接受人按钮，系统自动帮助您启动了，请关闭窗口重新打开。";
                }

                //绑定独立表单，表单自定义方案验证错误弹出窗口进行提示
                if (this.currND.HisFrms != null && this.currND.HisFrms.Count > 0 && ex.Message.Contains("在提交前检查到如下必输字段填写不完整") == true)
                {
                    return "err@" + ex.Message.Replace("@@", "@").Replace("@", "<BR>@");
                }

                return "err@发送工作出现错误:" + ex.Message;
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        public string Save()
        {
            try
            {
                return BP.WF.Dev2Interface.Node_SaveWork(this.FK_Flow, this.FK_Node, this.WorkID, this.GetMainTableHT(), null);
            }
            catch (Exception ex)
            {
                return "err@保存失败:" + ex.Message;
            }
        }
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
                if (this.DoType1.ToUpper() != "VIEW")
                {
                    ds = BP.WF.CCFlowAPI.GenerWorkNode(this.FK_Flow, this.FK_Node, this.WorkID,
                        this.FID, BP.Web.WebUser.No);

                    //获取WF_GenerWorkFlow
                    string sql = string.Format("select work1.WFState,work2.WFState PWFState,work1.PFID,work1.PWorkID,work1.PNodeID,work1.PFlowNo,ISNULL(work2.PWorkID,0) PWorkID2,work2.PNodeID PNodeID2,work2.PFlowNo PFlowNo2,work1.FK_Flow,work1.FK_Node,work1.WorkID from WF_GenerWorkFlow work1 left join  WF_GenerWorkFlow work2 on  work1.FID=work2.WorkID where work1.WorkID='{0}'", WorkID);

                    DataTable wf_generWorkFlowDt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                    wf_generWorkFlowDt.TableName = "WF_GenerWorkFlow";

                    //把他转化小写,适应多个数据库.
                    wf_generWorkFlowDt = DBAccess.ToLower(wf_generWorkFlowDt);
                    ds.Tables.Add(wf_generWorkFlowDt);
                }

                DataTable trackDt = BP.WF.Dev2Interface.DB_GenerTrack(this.FK_Flow, this.WorkID, this.FID).Tables["Track"];


                ds.Tables.Add(trackDt.Copy());

                //ds.WriteXml(xml);

                json = BP.Tools.Json.ToJson(ds);
                //BP.DA.DataType.WriteFile("c:\\WorkNode.json", json);
            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex);
                json = "err@" + ex.Message;
            }
            return json;
        }
    }
}
