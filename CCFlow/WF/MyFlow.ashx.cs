using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.Port;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Template;
using BP.WF.Data;
using BP.Sys;

namespace CCFlow.WF
{
    /// <summary>
    /// MyFlow 的摘要说明
    /// </summary>
    public class MyFlow : IHttpHandler
    {
        HttpRequest Request;
        HttpContext context;

        #region  运行变量
        /// <summary>
        /// 当前的流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                string s = this.Request.Form["FK_Flow"];
                if (string.IsNullOrEmpty(s))
                    throw new Exception("@流程编号参数错误...");

                return BP.WF.Dev2Interface.TurnFlowMarkToFlowNo(s);
            }
        }

        public string DoType {
            get
            {
                string s = this.Request.Form["DoType"];
                if (string.IsNullOrEmpty(s))
                    s = "";

                return s;
            }
        }
        /// <summary>
        /// 从节点.
        /// </summary>
        public string FromNode
        {
            get
            {
                return this.Request.Form["FromNode"];
            }
        }
        /// <summary>
        /// 执行功能
        /// </summary>
        public string DoFunc
        {
            get
            {
                return this.Request.Form["DoFunc"];
            }
        }
        /// <summary>
        /// 子流程编号
        /// </summary>
        public string CFlowNo
        {
            get
            {
                return this.Request.Form["CFlowNo"];
            }
        }
        /// <summary>
        /// 工作IDs
        /// </summary>
        public string WorkIDs
        {
            get
            {
                return this.Request.Form["WorkIDs"];
            }
        }
        /// <summary>
        /// Nos
        /// </summary>
        public string Nos
        {
            get
            {
                return this.Request.Form["Nos"];
            }
        }
        /// <summary>
        /// 是否抄送
        /// </summary>
        public bool IsCC
        {
            get
            {
                if (string.IsNullOrEmpty(this.Request.Form["Paras"]) == false)
                {
                    string myps = this.Request.Form["Paras"];

                    if (myps.Contains("IsCC=1") == true)
                        return true;
                }
                if (string.IsNullOrEmpty(this.Request.Form["AtPara"]) == false)
                {
                    string myps = this.Request.Form["AtPara"];

                    if (myps.Contains("IsCC=1") == true)
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
                return this.RequestValByKey("TrackeID");
            }
        }
        public string RequestValByKey(string key)
        {
            string str = this.Request.Form[key];
            if (str == null)
                str = context.Request.QueryString[key];

            return str;
        }
        /// <summary>
        /// 当前的工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                if (this.Request.Form["WorkID"] == null)
                    return 0;
                else
                    return Int64.Parse(this.Request.Form["WorkID"]);
            }
        }
        /// <summary>
        /// 到达的节点ID
        /// </summary>
        public int ToNode
        {
            get
            {
                if (this.Request.Form["ToNode"] == null)
                    return 0;
                else
                    return int.Parse(this.Request.Form["ToNode"]);
            }
        }
        /// <summary>
        /// 子流程ID
        /// </summary>
        public Int64 CWorkID
        {
            get
            {
                if (this.Request.Form["CWorkID"] == null)
                    return 0;
                else
                    return Int64.Parse(this.Request.Form["CWorkID"]);
            }
        }
        private int _FK_Node = 0;
        /// <summary>
        /// 当前的 NodeID ,在开始时间,nodeID,是地一个,流程的开始节点ID.
        /// </summary>
        public int FK_Node
        {
            get
            {
                string fk_nodeReq = this.Request.Form["FK_Node"];
                if (string.IsNullOrEmpty(fk_nodeReq))
                    fk_nodeReq = this.Request.Form["NodeID"];

                if (string.IsNullOrEmpty(fk_nodeReq) == false)
                    return int.Parse(fk_nodeReq);

                if (_FK_Node == 0)
                {
                    if (this.Request.Form["WorkID"] != null)
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
        /// <summary>
        /// FID
        /// </summary>
        public int FID
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.Form["FID"]);
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 父流程ID.
        /// </summary>
        public int PWorkID
        {
            get
            {
                try
                {
                    string s = this.Request.Form["PWorkID"];
                    if (string.IsNullOrEmpty(s) == true)
                        s = this.Request.Form["PWorkID"];
                    if (string.IsNullOrEmpty(s) == true)
                        s = "0";
                    return int.Parse(s);
                }
                catch
                {
                    return 0;
                }
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
                //float w = float.Parse(_width)+20;
                //return w.ToString();
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
        #endregion

        public string InitToolBar()
        {
            string tKey = DateTime.Now.ToString();
            string toolbar = "";
            if (this.IsCC)
            {
                toolbar+="<input type=button  value='流程运行轨迹' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/OneWork/ChartTrack.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />";
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
                    //  toolbar.Add("<input type=button  value='" + btnLab.SaveLab + "' enable=true onclick=\" SaveSelfFrom();\" />");
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
                            //toolbar.AddBtn(NamesOfBtn.Send, btnLab.SendLab);
                            //Btn_Send.Style.Add("display", "none");
                            //this.Btn_Send.UseSubmitBehavior = false;
                            //if (this.currND.HisFormType == NodeFormType.DisableIt)
                            //    this.Btn_Send.OnClientClick = btnLab.SendJS + "this.disabled=true;"; //this.disabled='disabled'; return true;";
                            //else
                            //    this.Btn_Send.OnClientClick = btnLab.SendJS + "if(SysCheckFrm()==false) return false;this.disabled=true;SaveDtlAll();KindEditerSync();"; //this.disabled='disabled'; return true;";
                            ////   this.Btn_Send.OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";
                            //this.Btn_Send.Click += new System.EventHandler(ToolBar1_ButtonClick);
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
                //toolbar.Add("<input type=button  value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" />");
            }

            if (btnLab.ShiftEnable && isAskFor == false)
            {
                /*移交*/
                // toolbar.AddBtn("Btn_Shift", btnLab.ShiftLab);
                //   this.Btn_Shift.Click += new System.EventHandler(ToolBar1_ButtonClick);
                string url12 = "./WorkOpt/Forward.htm?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&Info=" + "移交原因.";
                toolbar += "<input name='Shift' type=button  value='" + btnLab.ShiftLab + "' enable=true onclick=\"To('" + url12 + "'); \" />";
            }

            if ((btnLab.CCRole == CCRole.HandCC || btnLab.CCRole == CCRole.HandAndAuto))
            {
                /* 抄送 */
                // toolbar.Add("<input type=button value='" + btnLab.CCLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Msg/Write.htm?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "','ds'); \" />");
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
                //toolbar.AddBtn("Btn_EndFlow", btnLab.EndFlowLab);
                //toolbar.GetBtnByID("Btn_EndFlow").OnClientClick = "return confirm('" + this.ToE("AYS", "将要执行终止流程，您确认吗？") + "')";
                //toolbar.GetBtnByID("Btn_EndFlow").Click += new System.EventHandler(ToolBar1_ButtonClick);
            }

            //if (btnLab.RptEnable)
            //    toolbar.Add("<input type=button  value='" + btnLab.RptLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WFRpt.htm?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "','ds0'); \" />");

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
                //toolbar.Add("<input type=button  value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
            }

            if (btnLab.WebOfficeWorkModel == WebOfficeWorkModel.Button)
            {
                /*公文正文 */
                string urlr = appPath + "WF/WorkOpt/WebOffice.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar += "<input type=button name='WebOffice'  value='" + btnLab.WebOfficeLab + "' enable=true onclick=\"WinOpen('" + urlr + "','公文正文'); \" />";
                //toolbar.Add("<input type=button  value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
            }

            if (this.currFlow.IsResetData == true && this.currND.IsStartNode)
            {
                /* 启用了数据重置功能 */
                string urlr3 = appPath + "WF/MyFlow.htm?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&IsDeleteDraft=1&s=" + tKey;
                toolbar +="<input type=button  value='数据重置' enable=true onclick=\"To('" + urlr3 + "','ds'); \" />";
                //toolbar.Add("<input type=button  value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
            }

            if (btnLab.SubFlowEnable ==true)
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
                    toolbar +="<input type=button  value='取消关注' enable=true onclick=\"FocusBtn(this,'" + this.WorkID + "'); \" />";
                else
                    toolbar += "<input type=button name='Focus' value='" + btnLab.FocusLab + "' enable=true onclick=\"FocusBtn(this,'" + this.WorkID + "'); \" />";
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
                        toolbar +="<input type=button  value='" + bar.Title + "' enable=true onclick=\"" + bar.Url + "\" />";
                    }
                    else
                    {
                        string urlr3 = bar.Url + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                        toolbar +="<input type=button  value='" + bar.Title + "' enable=true onclick=\"WinOpen('" + urlr3 + "'); \" />";
                    }
                }
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
            foreach (string key in Request.Form.Keys)
            {
                if (key == null)
                    continue;

                if (key.Contains("TB_"))
                {
                    htMain.Add(key.Replace("TB_", ""), this.Request.Form[key]);
                    continue;
                }

                if (key.Contains("DDL_"))
                {
                    htMain.Add(key.Replace("DDL_", ""), this.Request.Form[key]);
                    continue;
                }

                if (key.Contains("CB_"))
                {
                    htMain.Add(key.Replace("CB_", ""), this.Request.Form[key]);
                    continue;
                }

                if (key.Contains("RB_"))
                {
                    htMain.Add(key.Replace("RB_", ""), this.Request.Form[key]);
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
                try
                {
                    objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, ht, null,this.ToNode,null);
                    msg = objs.ToMsgOfHtml();
                    BP.WF.Glo.SessionMsg = msg;
                    context.Session["Info"] = msg;
                }
                catch (Exception exSend)
                {
                    if (exSend.Message.Contains("请选择下一步骤工作") == true || exSend.Message.Contains("用户没有选择发送到的节点") == true)
                    {
                        /*如果抛出异常，我们就让其转入选择到达的节点里, 在节点里处理选择人员. */
                        return "url@./WorkOpt/ToNodes.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID;
                    }


                    //绑定独立表单，表单自定义方案验证错误弹出窗口进行提示
                    //if (this.currND.HisFrms != null && this.currND.HisFrms.Count > 0 && exSend.Message.Contains("在提交前检查到如下必输字段填写不完整") == true)
                    //if (this.currND.HisFrms != null && this.currND.HisFrms.Count > 0 && exSend.Message.Contains("在提交前检查到如下必输字段填写不完整") == true)
                    //{
                    //    return "err@" + exSend.Message.Replace("@@", "@").Replace("@", "<BR>@");
                    //}

#warning 杨玉慧
                    return "err@" + exSend.Message.Replace("@@", "@").Replace("@", "<BR>@");
                }


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
            catch(Exception ex)
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
            DataSet ds = new DataSet();
            if (this.DoType.ToUpper() != "VIEW")
            {
                ds = BP.WF.CCFlowAPI.GenerWorkNode(this.FK_Flow, this.FK_Node, this.WorkID,
                    this.FID, BP.Web.WebUser.No);

                //获取WF_GenerWorkFlow
                string sql = string.Format("select work1.WFState,work2.WFState PWFState,work1.PFID,work1.PWorkID,work1.PNodeID,work1.PFlowNo,ISNULL(work2.PWorkID,0) PWorkID2,work2.PNodeID PNodeID2,work2.PFlowNo PFlowNo2,work1.FK_Flow,work1.FK_Node,work1.WorkID from WF_GenerWorkFlow work1 left join  WF_GenerWorkFlow work2 on  work1.FID=work2.WorkID where work1.WorkID='{0}'", WorkID);

                DataTable wf_generWorkFlowDt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                wf_generWorkFlowDt.TableName = "WF_GenerWorkFlow";
                ds.Tables.Add(wf_generWorkFlowDt);

                sql = "select * from WF_Node where NodeID=" + this.FK_Node;
                DataTable wf_node = BP.DA.DBAccess.RunSQLReturnTable(sql);
                wf_node.TableName = "WF_Node";
                if (ds.Tables.Contains(wf_node.TableName))
                {
                    ds.Tables.Remove("WF_Node");
                }
                ds.Tables.Add(wf_node);
            }
            DataTable trackDt = BP.WF.Dev2Interface.DB_GenerTrack(this.FK_Flow, this.WorkID, this.FID).Tables["Track"];
            ds.Tables.Add(trackDt.Copy());
            string xml = "c:\\WorkNode.xml";
            //ds.WriteXml(xml);

            string json = BP.Tools.Json.ToJson(ds);
            //BP.DA.DataType.WriteFile("c:\\WorkNode.json", json);
            return json;
        }
        public void ProcessRequest(HttpContext context)
        {
            this.Request = context.Request;
            this.context = context;

            context.Response.ContentType = "text/plain";
            string method = context.Request.QueryString["Method"].ToString();
            string resultValue = "";
            switch (method)
            {
               
                case "Save": //保存.
                    resultValue = Save();
                    break;
                case "Send": //发送.
                    resultValue = Send();
                    break;
                case "InitToolBar":
                    resultValue = InitToolBar();
                    break;
                case "GenerWorkNode":
                    resultValue = GenerWorkNode();
                    break;
              
                default:
                    resultValue = method + "没有判断.";
                    break;
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(resultValue);
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