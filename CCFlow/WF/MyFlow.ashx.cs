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
                toolbar+="<input type=button  value='流程运行轨迹' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/OneWork/ChartTrack.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />";
                // 判断审核组件在当前的表单中是否启用，如果启用了.
                FrmWorkCheck fwc = new FrmWorkCheck(this.FK_Node);
                if (fwc.HisFrmWorkCheckSta != FrmWorkCheckSta.Enable)
                {
                    /*如果不等于启用, */
                    toolbar += "<input type=button  value='填写审核意见' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/CCCheckNote.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />";
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
                        toolbar += "<input type=button value='" + btnLab.SendLab+ "' enable=true onclick=\""+btnLab.SendJS + " if (SendSelfFrom()==false) return false;this.disabled=true;\" />";
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
                            toolbar += "<input type=button  value='" + btnLab.SendLab + "' enable=true onclick=\"javascript:OpenSelectAccepter('" + this.FK_Flow + "','" + this.FK_Node + "','" + this.WorkID + "','" + this.FID + "');" + btnLab.SendJS + " if ( SendSelfFrom()==false) return false;this.disabled=true;\" />";
                        }
                        else
                        {
                            toolbar += "<input type=button  value='" + btnLab.SendLab + "' enable=true onclick=\"" + btnLab.SendJS + " if ( SendSelfFrom()==false) return false;this.disabled=true;\" />";
                        }
                    }
                }

                /*处理保存按钮.*/
                if (btnLab.SaveEnable && isAskFor == false)
                {
                    toolbar += "<input type=button value='" + btnLab.SaveLab + "' enable=true onclick=\"SaveSelfFrom();\" />";
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
                        toolbar += "<input type=button value='" + btnLab.SaveLab + "' enable=true onclick=\""+btnLab.SendJS + " if(SysCheckFrm()==false) return false;this.disabled=true;SaveDtlAll();KindEditerSync();\" />";
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
                             toolbar += "<input type=button  value='" + btnLab.SendLab + "' enable=true onclick=\"if(SysCheckFrm()==false) return false;KindEditerSync();if (OpenSelectAccepter('" + this.FK_Flow + "','" + this.FK_Node + "','" + this.WorkID + "','" + this.FID + "')==false) return false; \" />";
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
                            toolbar += "<input type=button  value='" + btnLab.SendLab + "' enable=true onclick=\" " + btnLab.SendJS + " if(SysCheckFrm()==false) return false;KindEditerSync();\" />";
                        }
                    }
                }

                /*处理保存按钮.*/
                if (btnLab.SaveEnable && isAskFor == false)
                {
                    toolbar += "<input type=button  value='" + btnLab.SaveLab + "' enable=true onclick=\"   if(SysCheckFrm()==false) return false;KindEditerSync();\" />";
                }
            }

            if (btnLab.WorkCheckEnable && isAskFor == false)
            {
                /*审核*/
                string urlr1 = appPath + "WF/WorkOpt/WorkCheck.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar += "<input type=button  value='" + btnLab.WorkCheckLab + "' enable=true onclick=\"WinOpen('" + urlr1 + "','dsdd'); \" />";
            }

            if (btnLab.ThreadEnable)
            {
                /*如果要查看子线程.*/
                string ur2 = appPath + "WF/WorkOpt/ThreadDtl.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar += "<input type=button  value='" + btnLab.ThreadLab + "' enable=true onclick=\"WinOpen('" + ur2 + "'); \" />";
            }

            if (btnLab.TCEnable == true && isAskFor == false)
            {
                /*流转自定义..*/
                string ur3 = appPath + "WF/WorkOpt/TransferCustom.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar += "<input type=button  value='" + btnLab.TCLab + "' enable=true onclick=\"To('" + ur3 + "'); \" />";
            }

            if (btnLab.JumpWayEnable && isAskFor == false)
            {
                /*如果没有焦点字段*/
                string urlr = appPath + "WF/JumpWay.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar += "<input type=button  value='" + btnLab.JumpWayLab + "' enable=true onclick=\"To('" + urlr + "'); \" />";
            }

            if (btnLab.ReturnEnable && isAskFor == false && this.currND.IsStartNode == false)
            {
                /*如果没有焦点字段*/
                string urlr = appPath + "WF/WorkOpt/ReturnWork.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar += "<input type=button  value='" + btnLab.ReturnLab + "' enable=true onclick=\"ReturnWork('" + urlr + "','" + btnLab.ReturnField + "'); \" />";
            }
          
            //  if (btnLab.HungEnable && this.currND.IsStartNode == false)
            if (btnLab.HungEnable)
            {
                /*挂起*/
                string urlr = appPath + "WF/WorkOpt/HungUp.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar += "<input type=button  value='" + btnLab.HungLab + "' enable=true onclick=\"WinOpen('" + urlr + "'); \" />";
                //toolbar.Add("<input type=button  value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" />");
            }

            if (btnLab.ShiftEnable && isAskFor == false)
            {
                /*移交*/
                // toolbar.AddBtn("Btn_Shift", btnLab.ShiftLab);
                //   this.Btn_Shift.Click += new System.EventHandler(ToolBar1_ButtonClick);
                string url12 = "./WorkOpt/Forward.aspx?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&Info=" + "移交原因.";
                toolbar += "<input type=button  value='" + btnLab.ShiftLab + "' enable=true onclick=\"To('" + url12 + "'); \" />";
            }

            if ((btnLab.CCRole == CCRole.HandCC || btnLab.CCRole == CCRole.HandAndAuto))
            {
                /* 抄送 */
                // toolbar.Add("<input type=button value='" + btnLab.CCLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Msg/Write.aspx?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "','ds'); \" />");
                toolbar += "<input type=button  value='" + btnLab.CCLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/CC.aspx?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&s=" + tKey + "','ds'); \" />";
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
                        string urlrDel = appPath + "WF/MyFlowInfo.aspx?DoType=DeleteFlow&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                        toolbar += "<input type=button  value='" + btnLab.DeleteLab + "' enable=true onclick=\"To('" + urlrDel + "'); \" />";
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
                toolbar += "<input type=button  value='" + btnLab.EndFlowLab + "' enable=true onclick=\"To('./WorkOpt/StopFlow.aspx?&DoType=StopFlow&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey + "'); \" />";
                //toolbar.AddBtn("Btn_EndFlow", btnLab.EndFlowLab);
                //toolbar.GetBtnByID("Btn_EndFlow").OnClientClick = "return confirm('" + this.ToE("AYS", "将要执行终止流程，您确认吗？") + "')";
                //toolbar.GetBtnByID("Btn_EndFlow").Click += new System.EventHandler(ToolBar1_ButtonClick);
            }

            //if (btnLab.RptEnable)
            //    toolbar.Add("<input type=button  value='" + btnLab.RptLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WFRpt.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "','ds0'); \" />");

            if (btnLab.PrintDocEnable && isAskFor == false)
            {
                /*如果不是加签 */
                if (this.currND.HisPrintDocEnable == PrintDocEnable.PrintRTF)
                {
                    string urlr = appPath + "WF/WorkOpt/PrintDoc.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                    toolbar +="<input type=button  value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" />";
                }

                if (this.currND.HisPrintDocEnable == PrintDocEnable.PrintWord)
                {
                    string urlr = appPath + "WF/Rpt/RptDoc.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&IsPrint=1&s=" + tKey;
                    toolbar +="<input type=button  value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" />";
                }

                if (this.currND.HisPrintDocEnable == PrintDocEnable.PrintHtml)
                {
                    toolbar +="<input type=button  value='" + btnLab.PrintDocLab + "' enable=true onclick=\"printFrom(); \" />";
                }
            }

            if (btnLab.TrackEnable && isAskFor == false)
                toolbar +="<input type=button  value='" + btnLab.TrackLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/OneWork/ChartTrack.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />";

            //if (btnLab.OptEnable)
            //    toolbar.Add("<input type=button  value='" + btnLab.OptLab + "' onclick=\"WinOpen('" + appPath + "WF/WorkOpt/Home.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "','opt'); \"  />");

            switch (btnLab.SelectAccepterEnable)
            {
                case 1:
                    if (isAskFor == false)
                        toolbar +="<input type=button  value='" + btnLab.SelectAccepterLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/Accepter.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&s=" + tKey + "','dds'); \" />";
                    break;
                case 2:
                    //  toolbar.Add("<input type=button  value='" + btnLab.SelectAccepterLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Accepter.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&s=" + tKey + "','dds'); \" />");
                    break;
                default:
                    break;
            }

            if (btnLab.SearchEnable && isAskFor == false)
                toolbar +="<input type=button  value='" + btnLab.SearchLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Rpt/Search.aspx?EnsName=ND" + int.Parse(this.FK_Flow) + "MyRpt&FK_Flow=" + this.FK_Flow + "&s=" + tKey + "','dsd0'); \" />";

            if (btnLab.BatchEnable && isAskFor == false)
            {
                /*批量处理*/
                string urlr = appPath + "WF/Batch.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar +="<input type=button  value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" />";
            }

            if (btnLab.AskforEnable && HisGenerWorkFlow != null && HisGenerWorkFlow.WFState != WFState.Askfor)
            {
                /*加签 */
                string urlr3 = appPath + "WF/WorkOpt/Askfor.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar +="<input type=button  value='" + btnLab.AskforLab + "' enable=true onclick=\"To('" + urlr3 + "'); \" />";
                //toolbar.Add("<input type=button  value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
            }

            if (btnLab.WebOfficeWorkModel == WebOfficeWorkModel.Button)
            {
                /*公文正文 */
                string urlr = appPath + "WF/WorkOpt/WebOffice.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar +="<input type=button  value='" + btnLab.WebOfficeLab + "' enable=true onclick=\"WinOpen('" + urlr + "','公文正文'); \" />";
                //toolbar.Add("<input type=button  value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
            }

            if (this.currFlow.IsResetData == true && this.currND.IsStartNode)
            {
                /* 启用了数据重置功能 */
                string urlr3 = appPath + "WF/MyFlow.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&IsDeleteDraft=1&s=" + tKey;
                toolbar +="<input type=button  value='数据重置' enable=true onclick=\"To('" + urlr3 + "','ds'); \" />";
                //toolbar.Add("<input type=button  value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
            }

            if (btnLab.SubFlowCtrlRole != SubFlowCtrlRole.None)
            {
                /* 子流程 */
                string urlr3 = appPath + "WF/WorkOpt/SubFlow.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar +="<input type=button  value='" + btnLab.SubFlowLab + "' enable=true onclick=\"WinOpen('" + urlr3 + "'); \" />";
            }

            if (btnLab.CHEnable == true)
            {
                /* 节点时限设置 */
                string urlr3 = appPath + "WF/WorkOpt/CH.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar +="<input type=button  value='" + btnLab.CHLab + "' enable=true onclick=\"WinShowModalDialog('" + urlr3 + "'); \" />";
            }

            if (btnLab.PRIEnable == true)
            {
                /* 优先级设置 */
                string urlr3 = appPath + "WF/WorkOpt/PRI.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar +="<input type=button  value='" + btnLab.PRILab + "' enable=true onclick=\"WinShowModalDialog('" + urlr3 + "'); \" />";
            }

            /* 关注 */
            if (btnLab.FocusEnable == true)
            {
                if (HisGenerWorkFlow.Paras_Focus == true)
                    toolbar +="<input type=button  value='取消关注' enable=true onclick=\"FocusBtn(this,'" + this.WorkID + "'); \" />";
                else
                    toolbar +="<input type=button  value='" + btnLab.FocusLab + "' enable=true onclick=\"FocusBtn(this,'" + this.WorkID + "'); \" />";
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
            }

            return htMain;
        }
        public string Send()
        {
            try
            {
                Hashtable ht = this.GetMainTableHT();
                SendReturnObjs objs = null;
                objs = BP.WF.Dev2Interface.Node_SendWork(this.FK_Flow, this.WorkID, ht, null);
                return objs.ToMsgOfText();
            }
            catch (Exception ex)
            {
                return "err@发送工作出现错误:" + ex.Message;
            }
        }
        public string Save()
        {
            return BP.WF.Dev2Interface.Node_SaveWork(this.FK_Flow, this.FK_Node, this.WorkID, this.GetMainTableHT() , null);
        }
        /// <summary>
        /// 产生一个工作节点
        /// </summary>
        /// <returns></returns>
        public string GenerWorkNode()
        {
            DataSet ds = BP.WF.CCFlowAPI.GenerWorkNode(this.FK_Flow, this.FK_Node, this.WorkID, 
                this.FID, BP.Web.WebUser.No);

            string xml = "c:\\WorkNode.xml";
            ds.WriteXml(xml);

            string json = BP.Tools.Json.ToJson(ds);
            BP.DA.DataType.WriteFile("c:\\WorkNode.json", json);

            return "";
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
                    resultValue = method + "没有";
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