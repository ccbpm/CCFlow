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
    /// 抄送处理类
    /// </summary>
    public class WF_MyCC : DirectoryPageBase
    {
        /// <summary>
        /// 抄送处理类
        /// </summary>
        public WF_MyCC()
        {

        }

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
                sql = sql.Replace("@WebUser.OrgNo", WebUser.OrgNo);

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
        /// 设置确认的状态
        /// </summary>
        /// <returns></returns>
        public string MyCC_Make_CheckOver()
        {
            return "执行成功.";
        }
        /// <summary>
        /// 初始化(处理分发)
        /// </summary>
        /// <returns></returns>
        public string MyCC_Init()
        {
            //手动启动子流程的标志 0父子流程 1 同级子流程
            string isStartSameLevelFlow = this.GetRequestVal("IsStartSameLevelFlow");

            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);

            CCLists ccs = new CCLists();
            ccs.Retrieve(CCListAttr.WorkID, this.WorkID, CCListAttr.CCTo, WebUser.No);
            foreach (CCList item in ccs)
            {
                if (item.HisSta == CCSta.UnRead)
                {
                    BP.WF.Dev2Interface.Node_CC_SetRead(item.MyPK);
                    //item.HisSta = CCSta.Read;
                    //item.DirectUpdate();
                }
            }


            //当前工作.
            Work currWK = this.currND.HisWork;

            #region 处理表单类型.
            if (this.currND.HisFormType == NodeFormType.SheetTree
                 || this.currND.HisFormType == NodeFormType.SheetAutoTree)
            {

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
                        toUrl = "MyCCGener.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&SID=" + WebUser.SID + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID + "&Frms=" + gwf.Paras_Frms;
                    else
                        toUrl = "MyCCGener.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&SID=" + WebUser.SID + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID;
                }
                else
                {
                    if (gwf.Paras_Frms.Equals("") == false)
                        toUrl = "MyCCTree.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&SID=" + WebUser.SID + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID + "&Frms=" + gwf.Paras_Frms;
                    else
                        toUrl = "MyCCTree.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&SID=" + WebUser.SID + "&PFlowNo=" + gwf.PFlowNo + "&PNodeID=" + gwf.PNodeID + "&PWorkID=" + gwf.PWorkID;
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
                url = this.MyCC_Init_DealUrl(currND, currWK);

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

                //string url = "MyCCFoolTruck.htm";
                string url = "MyCCGener.htm";

                //处理连接.
                url = this.MyCC_Init_DealUrl(currND, currWK, url);
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

                //string url = "MyCCFoolTruck.htm";
                string url = "MyCCWebOffice.htm";

                //处理连接.
                url = this.MyCC_Init_DealUrl(currND, currWK, url);
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

                string url = "MyCCGener.htm";
                if (this.IsMobile)
                    url = "MyCCGener.htm";

                //处理连接.
                url = this.MyCC_Init_DealUrl(currND, currWK, url);

                url = url.Replace("DoType=MyCC_Init&", "");
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

                string url = "MyCCSelfForm.htm";

                //处理连接.
                url = this.MyCC_Init_DealUrl(currND, currWK, url);

                url = url.Replace("DoType=MyCC_Init&", "");
                url = url.Replace("&DoWhat=StartClassic", "");
                return "url@" + url;
            }
            #endregion 内置表单类型的判断.

            string myurl = "MyCCGener.htm";

            //处理连接.
            myurl = this.MyCC_Init_DealUrl(currND, currWK, myurl);
            myurl = myurl.Replace("DoType=MyCC_Init&", "");
            myurl = myurl.Replace("&DoWhat=StartClassic", "");

            return "url@" + myurl;
        }
        private string MyCC_Init_DealUrl(BP.WF.Node currND, Work currWK, string url = null)
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
        /// 工具栏
        /// </summary>
        /// <returns></returns>
        public string InitToolBar()
        {
            BtnLab btnLab = new BtnLab(this.FK_Node);
            string tKey = DateTime.Now.ToString("MM-dd-hh:mm:ss");
            string toolbar = "";
            try
            {
                toolbar += "<input name='ReadAndClose' type=button value='已阅关闭' enable=true onclick=\"ReadAndClose();\" />";

                #region 加载流程控制器 - 按钮
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


                if (btnLab.WebOfficeWorkModel == WebOfficeWorkModel.Button)
                {
                    /*公文正文 */
                    string urlr = appPath + "WF/WorkOpt/WebOffice.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<input type=button name='WebOffice'  value='" + btnLab.WebOfficeLab + "' enable=true onclick=\"WinOpen('" + urlr + "','公文正文'); \" />";
                }

                if (btnLab.NoteEnable != 0)
                {
                    /* 备注设置 */
                    toolbar += "<input type=button name='Note'  value='" + btnLab.NoteLab + "' enable=true  />";
                }

                /* 关注 */
                if (btnLab.FocusEnable == true)
                {
                    if (HisGenerWorkFlow.Paras_Focus == true)
                        toolbar += "<input type=button  value='取消关注' enable=true onclick=\"FocusBtn(this,'" + this.WorkID + "'); \" />";
                    else
                        toolbar += "<input type=button name='Focus' value='" + btnLab.FocusLab + "' enable=true onclick=\"FocusBtn(this,'" + this.WorkID + "'); \" />";
                }

                /* 确认 */
                if (btnLab.ConfirmEnable == true)
                {
                    if (HisGenerWorkFlow.Paras_Confirm == true)
                        toolbar += "<input type=button  value='取消确认' enable=true onclick=\"ConfirmBtn(this,'" + this.WorkID + "'); \" />";
                    else
                        toolbar += "<input type=button name='Confirm' value='" + btnLab.ConfirmLab + "' enable=true onclick=\"ConfirmBtn(this,'" + this.WorkID + "'); \" />";
                }

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
                /* 公文标签 */
                if (btnLab.OfficeBtnEnable ==true)
                {
                    toolbar += "<input type=button name='Btn_Office'  onclick='OpenOffice(\"" + btnLab.OfficeBtnEnable + "\");'  value='" + btnLab.OfficeBtnLab + "' enable=true/>";
                }
                #endregion

                #region  加载自定义的button.
                BP.WF.Template.NodeToolbars bars = new NodeToolbars();
                bars.Retrieve(NodeToolbarAttr.FK_Node, this.FK_Node,NodeToolbarAttr.ShowWhere, (int)ShowWhere.CC);
                foreach (NodeToolbar bar in bars)
                {

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
        public string InitToolBarForVue()
        {
            //创建一个DataTable，返回按钮信息
            DataTable dt = new DataTable();
            dt.Columns.Add("No");
            dt.Columns.Add("Name");
            dt.Columns.Add("Oper");
            dt.Columns.Add("Role", typeof(int));
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

            string tKey = DateTime.Now.ToString("MM-dd-hh:mm:ss");
            string toolbar = "";
            try
            {
                #region 是否是会签？.
                if (isAskForOrHuiQian == true && SystemConfig.CustomerNo == "LIMS")
                    return "";

                if (isAskForOrHuiQian == true)
                {
                    dr["No"] = "Send";
                    dr["Name"] = "确定/完成";
                    dr["Oper"] = btnLab.SendJS + " if(SysCheckFrm()==false) return false;SaveDtlAllSend()";
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
                if (btnLab.HuiQianRole == HuiQianRole.Teamup)
                {
                    dr = dt.NewRow();
                    dr["No"] = "SendHuiQian";
                    dr["Name"] = "会签发送";
                    dr["Oper"] = btnLab.SendJS + " if(SysCheckFrm()==false) return false;SendIt(true);";
                    dt.Rows.Add(dr);

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
                            dr = dt.NewRow();
                            /*如果启用了发送按钮.*/
                            dr["No"] = "Send";
                            dr["Name"] = btnLab.SendLab;
                            dr["Oper"] = btnLab.SendJS + " if (SendSelfFrom()==false) return false; this.disabled=true;";
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
                            dr["Oper"] = btnLab.SendJS + " if ( SendSelfFrom()==false) return false; this.disabled=true;";
                            dt.Rows.Add(dr);
                        }
                    }

                    /*处理保存按钮.*/
                    if (btnLab.SaveEnable)
                    {
                        dr = dt.NewRow();
                        dr["No"] = "Save";
                        dr["Name"] = btnLab.SaveLab;
                        dr["Oper"] = "SaveSelfFrom();";
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
                            dr["Oper"] = btnLab.SendJS + " if(SysCheckFrm()==false) return false;SaveDtlAll();Send();";
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
                            dr["Oper"] = btnLab.SendJS + " if(SysCheckFrm()==false) return false;SaveDtlAll();Send();";
                            dt.Rows.Add(dr);
                        }
                    }

                    /* 处理保存按钮.*/
                    if (btnLab.SaveEnable)
                    {
                        dr = dt.NewRow();
                        dr["No"] = "Save";
                        dr["Name"] = btnLab.SaveLab;
                        dr["Oper"] = "if (SysCheckFrm() == false) return false; Save(); ";
                        dt.Rows.Add(dr);
                    }
                }

                if (btnLab.WorkCheckEnable)
                {
                    dr = dt.NewRow();
                    dr["No"] = "workcheckBtn";
                    dr["Name"] = btnLab.WorkCheckLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);/*审核*/
                }

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

                if (btnLab.JumpWayEnable && 1 == 2)
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

                //  if (btnLab.HungEnable && this.currND.IsStartNode == false)
                if (btnLab.HungEnable)
                {
                    /*挂起*/
                    dr = dt.NewRow();
                    dr["No"] = "Hung";
                    dr["Name"] = btnLab.HungLab;
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
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }

                // @李国文.
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
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }

                if (btnLab.BatchEnable)
                {
                    /*批量处理*/
                    dr = dt.NewRow();
                    dr["No"] = "Batch";
                    dr["Name"] = btnLab.BatchLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }

                if (btnLab.AskforEnable)
                {
                    /*加签 */
                    dr = dt.NewRow();
                    dr["No"] = "Askfor";
                    dr["Name"] = btnLab.AskforLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }

                if (btnLab.HuiQianRole == HuiQianRole.TeamupGroupLeader)
                {
                    /*会签 */
                    dr = dt.NewRow();
                    dr["No"] = "HuiQian";
                    dr["Name"] = btnLab.HuiQianLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }

                //原始会签主持人可以增加组长
                if (((DataType.IsNullOrEmpty(gwf.HuiQianZhuChiRen) == true && gwf.TodoEmps.Contains(WebUser.No) == true) || gwf.HuiQianZhuChiRen.Contains(WebUser.No) == true) && btnLab.AddLeaderEnable == true)
                {
                    /*增加组长 */
                    dr = dt.NewRow();
                    dr["No"] = "AddLeader";
                    dr["Name"] = btnLab.AddLeaderLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
                }


                if (btnLab.WebOfficeWorkModel == WebOfficeWorkModel.Button)
                {
                    /*公文正文 */
                    dr = dt.NewRow();
                    dr["No"] = "WebOffice";
                    dr["Name"] = btnLab.WebOfficeLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);

                }

                // 需要翻译.
                if (this.currFlow.IsResetData == true && this.currND.IsStartNode)
                {
                    /* 启用了数据重置功能 */
                    dr = dt.NewRow();
                    dr["No"] = "ReSet";
                    dr["Name"] = "数据重置";
                    dr["Oper"] = "";
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
                    dr["Oper"] = "";
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

                    dr["Oper"] = "";
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
                if (btnLab.OfficeBtnEnable == true)
                {
                    dr = dt.NewRow();
                    dr["No"] = "Btn_Office";
                    dr["Name"] = btnLab.OfficeBtnLab;
                    dr["Oper"] = "";
                    dt.Rows.Add(dr);
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
                        dr = dt.NewRow();
                        dr["No"] = "Btn_Office";
                        dr["Name"] = bar.Title;
                        dr["Oper"] = bar.Url;
                        dt.Rows.Add(dr);
                    }
                    else
                    {
                        dr = dt.NewRow();
                        dr["No"] = "Btn_Office";
                        dr["Name"] = bar.Title;
                        dr["Oper"] = bar.Url;
                        dt.Rows.Add(dr);

                        //string urlr3 = bar.Url + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                        //toolbar += "<input type=button  value='" + bar.Title + "' enable=true onclick=\"WinOpen('" + urlr3 + "'); \" />";
                    }
                }
                #endregion  //加载自定义的button.

            }
            catch (Exception ex)
            {
                BP.DA.Log.DefaultLogWriteLineError(ex);
                new Exception("err@" + ex.Message);
            }
            return BP.Tools.Json.ToJson(dt);
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
            string tKey = DateTime.Now.ToString("yyyy-MM-dd - hh:mm:ss");
            BtnLab btnLab = new BtnLab(this.FK_Node);
            string toolbar = "";
            try
            {
                #region 加载流程控制器 - 按钮
                if (btnLab.ThreadEnable)
                {
                    /*如果要查看子线程.*/
                    string ur2 = "./WorkOpt/ThreadDtl.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' value='" + btnLab.ThreadLab + "' enable=true onclick=\"WinOpen('" + ur2 + "'); \" ></a>";
                }


                if (btnLab.PrintDocEnable)
                {
                    string urlr = appPath + "WF/WorkOpt/PrintDoc.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' type=button name='PrintDoc' value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" ></a>";

                }

                if (btnLab.TrackEnable)
                    toolbar += "<a data-role='button' type=button name='Track'  value='" + btnLab.TrackLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" ></a>";



                if (btnLab.WebOfficeWorkModel == WebOfficeWorkModel.Button)
                {
                    /*公文正文 */
                    string urlr = appPath + "WF/WorkOpt/WebOffice.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' type=button name='WebOffice'  value='" + btnLab.WebOfficeLab + "' enable=true onclick=\"WinOpen('" + urlr + "','公文正文'); \" ></a>";
                }

                if (this.currFlow.IsResetData == true && this.currND.IsStartNode)
                {
                    /* 启用了数据重置功能 */
                    string urlr3 = appPath + "WF/MyCC.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&IsDeleteDraft=1&s=" + tKey;
                    toolbar += "<a data-role='button' type=button  value='数据重置' enable=true onclick=\"To('" + urlr3 + "','ds'); \" ></a>";
                }


                if (btnLab.CHRole != 0)
                {
                    /* 节点时限设置 */
                    string urlr3 = appPath + "WF/WorkOpt/CH.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar += "<a data-role='button' type=button name='CH'  value='" + btnLab.CHLab + "' enable=true onclick=\"WinShowModalDialog('" + urlr3 + "'); \" ></a>";
                }



                if (btnLab.PRIEnable != 0)
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


                #endregion

                #region  //加载自定义的button.
                BP.WF.Template.NodeToolbars bars = new NodeToolbars();
                bars.Retrieve(NodeToolbarAttr.FK_Node, this.FK_Node,NodeToolbarAttr.ShowWhere , ShowWhere.CC);
                foreach (NodeToolbar bar in bars)
                { 

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
                    else
                    {
                        htMain.Remove(key.Replace("TB_", ""));
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

        public string MyCCSelfForm_Init()
        {
            return this.GenerWorkNode();
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
            //}]]

            //if (appFlowFormTree.Count==1 && nd.FormType== NodeFormType.Tab)
            //{
            //    //nd.FormType
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

                Int64 workID = this.WorkID;
                Node nd = new Node(this.FK_Node);
                if (nd.HisFormType == NodeFormType.RefOneFrmTree)
                {
                    //获取绑定的表单
                    FrmNode frmnode = new FrmNode(this.FK_Flow, this.FK_Node, nd.NodeFrmID);
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
                            workID = BP.DA.DBAccess.RunSQLReturnValInt(sqlId, 0);
                            break;
                        default:
                            break;
                    }

                }

                ds = BP.WF.CCFlowAPI.GenerWorkNode(this.FK_Flow, this.FK_Node, workID,
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
