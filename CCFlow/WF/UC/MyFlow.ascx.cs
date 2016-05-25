using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using CCFlow.WF.Comm.UC;
using BP.WF;
using BP.Port;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Template;
using BP.WF.Data;
using BP.Sys;

namespace CCFlow.WF.UC
{
    public partial class MyFlow : BP.Web.UC.UCBase3
    {
        #region 控件
        public string _PageSamll = null;
        public string PageSmall
        {
            get
            {
                if (_PageSamll == null)
                {
                    if (this.PageID.ToLower().Contains("small"))
                        _PageSamll = "Small";
                    else
                        _PageSamll = "";
                }
                return _PageSamll;
            }
        }
        /// <summary>
        /// 发送
        /// </summary>
        protected Btn Btn_Send
        {
            get
            {
                return this.toolbar.GetBtnByID(NamesOfBtn.Send);
            }
        }
        protected Btn Btn_Delete
        {
            get
            {
                return this.toolbar.GetBtnByID(NamesOfBtn.Delete);
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        protected Btn Btn_Save
        {
            get
            {
                Btn btn = this.toolbar.GetBtnByID(NamesOfBtn.Save);
                if (btn == null)
                    btn = new Btn();
                return btn;
            }
        }
        protected Btn Btn_ReturnWork
        {
            get
            {
                Btn btn = this.toolbar.GetBtnByID("Btn_ReturnWork");
                if (btn == null)
                    btn = new Btn();
                return btn;
            }
        }
        protected Btn Btn_Shift
        {
            get
            {
                return this.toolbar.GetBtnByID(BP.Web.Controls.NamesOfBtn.Shift);
            }
        }
        #endregion

        #region  运行变量
        /// <summary>
        /// 当前的流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
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
                return this.Request.QueryString["FromNode"];
            }
        }
        /// <summary>
        /// 执行功能
        /// </summary>
        public string DoFunc
        {
            get
            {
                return this.Request.QueryString["DoFunc"];
            }
        }
        /// <summary>
        /// 子流程编号
        /// </summary>
        public string CFlowNo
        {
            get
            {
                return this.Request.QueryString["CFlowNo"];
            }
        }
        /// <summary>
        /// 工作IDs
        /// </summary>
        public string WorkIDs
        {
            get
            {
                return this.Request.QueryString["WorkIDs"];
            }
        }
        /// <summary>
        /// Nos
        /// </summary>
        public string Nos
        {
            get
            {
                return this.Request.QueryString["Nos"];
            }
        }
        /// <summary>
        /// 是否抄送
        /// </summary>
        public bool IsCC
        {
            get
            {

                if (string.IsNullOrEmpty(this.Request.QueryString["Paras"]) == false)
                {
                    string myps = this.Request.QueryString["Paras"];

                    if (myps.Contains("IsCC=1") == true)
                        return true;
                }
                if (string.IsNullOrEmpty(this.Request.QueryString["AtPara"]) == false)
                {
                    string myps = this.Request.QueryString["AtPara"];

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
                if (ViewState["WorkID"] == null)
                {
                    if (this.Request.QueryString["WorkID"] == null)
                        return 0;
                    else
                        return Int64.Parse(this.Request.QueryString["WorkID"]);
                }
                else
                    return Int64.Parse(ViewState["WorkID"].ToString());
            }
            set
            {
                ViewState["WorkID"] = value;
            }
        }
        /// <summary>
        /// 子流程ID
        /// </summary>
        public Int64 CWorkID
        {
            get
            {
                if (ViewState["CWorkID"] == null)
                {
                    if (this.Request.QueryString["CWorkID"] == null)
                        return 0;
                    else
                        return Int64.Parse(this.Request.QueryString["CWorkID"]);
                }
                else
                    return Int64.Parse(ViewState["CWorkID"].ToString());
            }
            set
            {
                ViewState["CWorkID"] = value;
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
                string fk_nodeReq = this.Request.QueryString["FK_Node"];
                if (string.IsNullOrEmpty(fk_nodeReq))
                    fk_nodeReq = this.Request.QueryString["NodeID"];

                if (string.IsNullOrEmpty(fk_nodeReq) == false)
                    return int.Parse(fk_nodeReq);

                if (_FK_Node == 0)
                {
                    if (this.Request.QueryString["WorkID"] != null)
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
                    return int.Parse(this.Request.QueryString["FID"]);
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
                    string s = this.Request.QueryString["PWorkID"];
                    if (string.IsNullOrEmpty(s) == true)
                        s = this.Request.QueryString["PWorkID"];
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
        public string Width
        {
            get { return _width; }
            set { _width = value; }
        }
        private string _height = "";
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
        #endregion

        private string tKey = DateTime.Now.ToString("yyMMddhhmmss");
        private string small = null;

        #region 方法.
        public void InitToolbar(bool isAskFor, string appPath, GenerWorkFlow gwf)
        {
            this.Page.Title = this.currND.Name;
            small = this.PageID;
            //定义timekey
            small = small.Replace("MyFlow", "");
            if (small != "")
                toolbar.AddBR();

            this.toolbar.Add("&nbsp;&nbsp;");

            if (this.IsCC)
            {
                toolbar.Add("<input type=button  value='流程运行轨迹' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/OneWork/ChartTrack.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />");
                // 判断审核组件在当前的表单中是否启用，如果启用了.
                FrmWorkCheck fwc = new FrmWorkCheck(this.FK_Node);
                if (fwc.HisFrmWorkCheckSta != FrmWorkCheckSta.Enable)
                {
                    /*如果不等于启用, */
                    toolbar.Add("<input type=button  value='填写审核意见' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/CCCheckNote.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />");
                }
                return;
            }

            #region 加载流程控制器 - 按钮
            BtnLab btnLab = new BtnLab(currND.NodeID);
          //  BtnWord = null;  // btnLab.WebOfficeEnable + "";

            if (this.currND.HisFormType == NodeFormType.SelfForm)
            {
                /*如果是嵌入式表单.*/
                if (currND.IsEndNode && isAskFor == false)
                {
                    /*如果当前节点是结束节点.*/
                    if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
                    {
                        /*如果启用了发送按钮.*/
                        toolbar.AddBtn(NamesOfBtn.Send, btnLab.SendLab);
                        this.Btn_Send.UseSubmitBehavior = false;
                        this.Btn_Send.OnClientClick = btnLab.SendJS + "if (SendSelfFrom()==false) return false;this.disabled=true;";
                        this.Btn_Send.Click += new System.EventHandler(ToolBar1_ButtonClick);
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
                            toolbar.Add("<input type=button  value='" + btnLab.SendLab + "' enable=true onclick=\"javascript:OpenSelectAccepter('" + this.FK_Flow + "','" + this.FK_Node + "','" + this.WorkID + "','" + this.FID + "')\" />");
                            toolbar.AddBtn(NamesOfBtn.Send, btnLab.SendLab);
                            Btn_Send.Style.Add("display", "none");
                            this.Btn_Send.UseSubmitBehavior = false;

                            if (this.currND.HisFormType == NodeFormType.DisableIt)
                                this.Btn_Send.OnClientClick = btnLab.SendJS + "this.disabled=true;"; //this.disabled='disabled'; return true;";
                            else
                                this.Btn_Send.OnClientClick = btnLab.SendJS + "if( SendSelfFrom()==false) return false;this.disabled=true;"; //this.disabled='disabled'; return true;";
                            this.Btn_Send.Click += new System.EventHandler(ToolBar1_ButtonClick);
                        }
                        else
                        {
                            toolbar.AddBtn(NamesOfBtn.Send, btnLab.SendLab);
                            this.Btn_Send.UseSubmitBehavior = false;
                            if (btnLab.SendJS.Trim().Length > 2)
                            {
                                this.Btn_Send.OnClientClick = btnLab.SendJS + ";if(SendSelfFrom()==false) return false;this.disabled=true;"; //this.disabled='disabled'; return true;";
                            }
                            else
                            {
                                this.Btn_Send.UseSubmitBehavior = false;
                                if (this.currND.HisFormType == NodeFormType.DisableIt)
                                    this.Btn_Send.OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";
                                else
                                    this.Btn_Send.OnClientClick = "if(SendSelfFrom()==false) return false;this.disabled=true;"; //this.disabled='disabled'; return true;";
                            }
                            this.Btn_Send.Click += new System.EventHandler(ToolBar1_ButtonClick);
                        }
                    }
                }

                /*处理保存按钮.*/
                if (btnLab.SaveEnable && isAskFor == false)
                    toolbar.Add("<input type=button  value='" + btnLab.SaveLab + "' enable=true onclick=\" SaveSelfFrom();\" />");
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
                        toolbar.AddBtn(NamesOfBtn.Send, btnLab.SendLab);
                        this.Btn_Send.UseSubmitBehavior = false;
                        this.Btn_Send.OnClientClick = btnLab.SendJS + " if(SysCheckFrm()==false) return false;this.disabled=true;SaveDtlAll();KindEditerSync();";
                        this.Btn_Send.Click += new System.EventHandler(ToolBar1_ButtonClick);
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
                            toolbar.Add("<input type=button  value='" + btnLab.SendLab + "' enable=true onclick=\"if(SysCheckFrm()==false) return false;KindEditerSync();if (OpenSelectAccepter('" + this.FK_Flow + "','" + this.FK_Node + "','" + this.WorkID + "','" + this.FID + "')==false) return false; \" />");
                            toolbar.AddBtn(NamesOfBtn.Send, btnLab.SendLab);
                            Btn_Send.Style.Add("display", "none");
                            this.Btn_Send.UseSubmitBehavior = false;

                            if (this.currND.HisFormType == NodeFormType.DisableIt)
                                this.Btn_Send.OnClientClick = btnLab.SendJS + "this.disabled=true;"; //this.disabled='disabled'; return true;";
                            else
                                this.Btn_Send.OnClientClick = btnLab.SendJS + "if(SysCheckFrm()==false) return false;this.disabled=true;SaveDtlAll();KindEditerSync();"; //this.disabled='disabled'; return true;";
                            //   this.Btn_Send.OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";
                            this.Btn_Send.Click += new System.EventHandler(ToolBar1_ButtonClick);
                        }
                        else
                        {
                            toolbar.AddBtn(NamesOfBtn.Send, btnLab.SendLab);
                            this.Btn_Send.UseSubmitBehavior = false;
                            if (btnLab.SendJS.Trim().Length > 2)
                            {
                                this.Btn_Send.OnClientClick = btnLab.SendJS + ";if(SysCheckFrm()==false) return false;this.disabled=true;SaveDtlAll();KindEditerSync();"; //this.disabled='disabled'; return true;";
                            }
                            else
                            {
                                this.Btn_Send.UseSubmitBehavior = false;
                                if (this.currND.HisFormType == NodeFormType.DisableIt)
                                    this.Btn_Send.OnClientClick = "this.disabled=true;"; //this.disabled='disabled'; return true;";
                                else
                                    this.Btn_Send.OnClientClick = "if(SysCheckFrm()==false) return false;this.disabled=true;SaveDtlAll();KindEditerSync();"; //this.disabled='disabled'; return true;";
                            }
                            this.Btn_Send.Click += new System.EventHandler(ToolBar1_ButtonClick);
                        }
                    }
                }

                /*处理保存按钮.*/
                if (btnLab.SaveEnable && isAskFor == false)
                {
                    toolbar.AddBtn(NamesOfBtn.Save, btnLab.SaveLab);
                    this.Btn_Save.UseSubmitBehavior = false;
                    this.Btn_Save.OnClientClick = "if(SysCheckFrm()==false) return false;this.disabled=true;SaveDtlAll();KindEditerSync();"; //this.disabled='disabled'; return true;";
                    this.Btn_Save.Click += new System.EventHandler(ToolBar1_ButtonClick);
                }
            }

            if (btnLab.WorkCheckEnable && isAskFor == false)
            {
                /*审核*/
                string urlr1 = appPath + "WF/WorkOpt/WorkCheck.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar.Add("<input type=button  value='" + btnLab.WorkCheckLab + "' enable=true onclick=\"WinOpen('" + urlr1 + "','dsdd'); \" />");
            }

            if (btnLab.ThreadEnable)
            {
                /*如果要查看子线程.*/
                string ur2 = appPath + "WF/WorkOpt/ThreadDtl.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar.Add("<input type=button  value='" + btnLab.ThreadLab + "' enable=true onclick=\"WinOpen('" + ur2 + "'); \" />");
            }

            if (btnLab.TCEnable==true && isAskFor == false)
            {
                /*流转自定义..*/
                string ur3 = appPath + "WF/WorkOpt/TransferCustom.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar.Add("<input type=button  value='" + btnLab.TCLab + "' enable=true onclick=\"To('" + ur3 + "'); \" />");
            }

            if (btnLab.JumpWayEnable && isAskFor == false)
            {
                /*如果没有焦点字段*/
                string urlr = appPath + "WF/JumpWay.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar.Add("<input type=button  value='" + btnLab.JumpWayLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
            }

            if (btnLab.ReturnEnable && isAskFor == false && this.currND.IsStartNode == false)
            {
                /*如果没有焦点字段*/
                string urlr = appPath + "WF/WorkOpt/ReturnWork.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar.Add("<input type=button  value='" + btnLab.ReturnLab + "' enable=true onclick=\"ReturnWork('" + urlr + "','" + btnLab.ReturnField + "'); \" />");
            }

            //if (btnLab.ReturnEnable && isAskFor == false && this.currND.IsStartNode == false && this.currND.FocusField != ""  )
            //{
            //    /*如果有焦点字段*/
            //    toolbar.AddBtn("Btn_ReturnWork", btnLab.ReturnLab);
            //    this.Btn_ReturnWork.UseSubmitBehavior = false;
            //    this.Btn_ReturnWork.OnClientClick = "this.disabled=true;";
            //    this.Btn_ReturnWork.Click += new System.EventHandler(ToolBar1_ButtonClick);
            //}

            //  if (btnLab.HungEnable && this.currND.IsStartNode == false)
            if (btnLab.HungEnable)
            {
                /*挂起*/
                string urlr = appPath + "WF/WorkOpt/HungUp.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar.Add("<input type=button  value='" + btnLab.HungLab + "' enable=true onclick=\"WinOpen('" + urlr + "'); \" />");
                //toolbar.Add("<input type=button  value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" />");
            }

            if (btnLab.ShiftEnable && isAskFor == false)
            {
                /*移交*/
                toolbar.AddBtn("Btn_Shift", btnLab.ShiftLab);
                this.Btn_Shift.Click += new System.EventHandler(ToolBar1_ButtonClick);
            }

            if ((btnLab.CCRole == CCRole.HandCC || btnLab.CCRole == CCRole.HandAndAuto))
            {
                /* 抄送 */
                // toolbar.Add("<input type=button value='" + btnLab.CCLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Msg/Write.aspx?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "','ds'); \" />");
                toolbar.Add("<input type=button  value='" + btnLab.CCLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/CC.aspx?WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&s=" + tKey + "','ds'); \" />");
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
                        string urlrDel = appPath + "WF/MyFlowInfo.aspx?DoType=DeleteFlow&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                        toolbar.Add("<input type=button  value='" + btnLab.DeleteLab + "' enable=true onclick=\"To('" + urlrDel + "'); \" />");
                        break;
                    case DelWorkFlowRole.DeleteReal: // 不需要交互，直接干净的删除.
                        toolbar.AddBtn("Btn_Delete", btnLab.DeleteLab);
                        this.Btn_Delete.OnClientClick = "return confirm('将要执行删除流程，您确认吗?')";
                        this.Btn_Delete.Click += new System.EventHandler(ToolBar1_ButtonClick);
                        break;
                    default:
                        break;
                }
            }

            if (btnLab.EndFlowEnable && this.currND.IsStartNode == false && isAskFor == false)
            {
                toolbar.Add("<input type=button  value='" + btnLab.EndFlowLab + "' enable=true onclick=\"To('./WorkOpt/StopFlow.aspx?&DoType=StopFlow&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey + "'); \" />");
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
                    toolbar.Add("<input type=button  value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" />");
                }

                if( this.currND.HisPrintDocEnable == PrintDocEnable.PrintWord)
                {
                    string urlr = appPath + "WF/Rpt/RptDoc.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&IsPrint=1&s=" + tKey;
                    toolbar.Add("<input type=button  value='" + btnLab.PrintDocLab + "' enable=true onclick=\"WinOpen('" + urlr + "','dsdd'); \" />");
                }

                if (this.currND.HisPrintDocEnable == PrintDocEnable.PrintHtml)
                {
                    toolbar.Add("<input type=button  value='" + btnLab.PrintDocLab + "' enable=true onclick=\"printFrom(); \" />");
                }
            }

            if (btnLab.TrackEnable && isAskFor == false)
                toolbar.Add("<input type=button  value='" + btnLab.TrackLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/OneWork/ChartTrack.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_Node=" + this.FK_Node + "&s=" + tKey + "','ds'); \" />");

            //if (btnLab.OptEnable)
            //    toolbar.Add("<input type=button  value='" + btnLab.OptLab + "' onclick=\"WinOpen('" + appPath + "WF/WorkOpt/Home.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "','opt'); \"  />");

            switch (btnLab.SelectAccepterEnable)
            {
                case 1:
                    if (isAskFor == false)
                        toolbar.Add("<input type=button  value='" + btnLab.SelectAccepterLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/WorkOpt/Accepter.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&s=" + tKey + "','dds'); \" />");
                    break;
                case 2:
                    //  toolbar.Add("<input type=button  value='" + btnLab.SelectAccepterLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Accepter.aspx?WorkID=" + this.WorkID + "&FK_Node=" + currND.NodeID + "&FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&s=" + tKey + "','dds'); \" />");
                    break;
                default:
                    break;
            }

            if (btnLab.SearchEnable && isAskFor == false)
                toolbar.Add("<input type=button  value='" + btnLab.SearchLab + "' enable=true onclick=\"WinOpen('" + appPath + "WF/Rpt/Search.aspx?EnsName=ND" + int.Parse(this.FK_Flow) + "MyRpt&FK_Flow=" + this.FK_Flow + "&s=" + tKey + "','dsd0'); \" />");

            if (btnLab.BatchEnable && isAskFor == false)
            {
                /*批量处理*/
                string urlr = appPath + "WF/Batch" + small + ".aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar.Add("<input type=button  value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
            }

            if (btnLab.AskforEnable && gwf != null && gwf.WFState != WFState.Askfor)
            {
                /*加签 */
                string urlr3 = appPath + "WF/WorkOpt/Askfor.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar.Add("<input type=button  value='" + btnLab.AskforLab + "' enable=true onclick=\"To('" + urlr3 + "'); \" />");
                //toolbar.Add("<input type=button  value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
            }

            if (btnLab.WebOfficeWorkModel ==  WebOfficeWorkModel.Button)
            {
                /*公文正文 */
                string urlr = appPath + "WF/WorkOpt/WebOffice.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar.Add("<input type=button  value='" + btnLab.WebOfficeLab + "' enable=true onclick=\"WinOpen('" + urlr + "','公文正文'); \" />");
                //toolbar.Add("<input type=button  value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
            }

            if (this.currFlow.IsResetData == true && this.currND.IsStartNode)
            {
                /* 启用了数据重置功能 */
                string urlr3 = appPath + "WF/MyFlow.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&IsDeleteDraft=1&s=" + tKey;
                toolbar.Add("<input type=button  value='数据重置' enable=true onclick=\"To('" + urlr3 + "','ds'); \" />");
                //toolbar.Add("<input type=button  value='" + btnLab.BatchLab + "' enable=true onclick=\"To('" + urlr + "'); \" />");
            }

            if (btnLab.SubFlowCtrlRole != SubFlowCtrlRole.None  )
            {
                /* 子流程 */
                string urlr3 = appPath + "WF/WorkOpt/SubFlow.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar.Add("<input type=button  value='" + btnLab.SubFlowLab + "' enable=true onclick=\"WinOpen('" + urlr3 + "'); \" />");
            }

            if (btnLab.CHEnable ==true )
            {
                /* 节点时限设置 */
                string urlr3 = appPath + "WF/WorkOpt/CH.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar.Add("<input type=button  value='" + btnLab.CHLab + "' enable=true onclick=\"WinShowModalDialog('" + urlr3 + "'); \" />");
            }

            if (btnLab.PRIEnable == true)
            {
                /* 优先级设置 */
                string urlr3 = appPath + "WF/WorkOpt/PRI.aspx?FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                toolbar.Add("<input type=button  value='" + btnLab.PRILab + "' enable=true onclick=\"WinShowModalDialog('" + urlr3 + "'); \" />");
            }

            /* 关注 */
            if (btnLab.FocusEnable == true)
            {
                if (gwf.Paras_Focus == true)
                    toolbar.Add("<input type=button  value='取消关注' enable=true onclick=\"FocusBtn(this,'" + this.WorkID + "'); \" />");
                else
                    toolbar.Add("<input type=button  value='" + btnLab.FocusLab + "' enable=true onclick=\"FocusBtn(this,'" + this.WorkID + "'); \" />");
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
                        toolbar.Add("<input type=button  value='" + bar.Title + "' enable=true onclick=\"" + bar.Url + "\" />");
                    }
                    else
                    {
                        string urlr3 = bar.Url + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&s=" + tKey;
                        toolbar.Add("<input type=button  value='" + bar.Title + "' enable=true onclick=\"WinOpen('" + urlr3 + "'); \" />");
                    }
                }
            }


        }
        public string DealUrl(BP.WF.Node currND)
        {
            string url = currND.FormUrl;
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

            urlExt += "&CWorkID=" + this.CWorkID;

            if (urlExt.Contains("&NodeID") == false)
                urlExt += "&NodeID=" + currND.NodeID;

            if (urlExt.Contains("FK_Node") == false)
                urlExt += "&FK_Node=" + currND.NodeID;

            if (urlExt.Contains("&FID") == false)
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
            return url;
        }
        #endregion 方法.

        #region 变量
        ToolBar toolbar = null;
        public Flow currFlow = null;
        public Work currWK = null;
        private BP.WF.Node _currND = null;

        public BP.WF.Node currND
        {
            get
            {
                if (_currND == null)
                    _currND = new BP.WF.Node(this.FK_Node);
                return _currND;
            }
            set
            {
                _currND = value;
            }
        }
        public GenerWorkFlow gwf = null;
        #endregion

        #region Page load 事件
        /// <summary>
        /// 装载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.currFlow = new Flow(this.FK_Flow);
            this.currND = new BP.WF.Node(this.FK_Node);
            this.Width = "900";

            // 检查是否可以发起该流程？
            if (BP.WF.Glo.CheckIsCanStartFlow_InitStartFlow(this.currFlow) == false)
            {
                this.ToMsg("@您违反了该流程的【" + this.currFlow.StartLimitRole + "】限制规则。" + this.currFlow.StartLimitAlert,"INfo");
                return;
            }

            #region 校验用户是否被禁用。
            try
            {
                string userNo = this.Request.QueryString["UserNo"];
                if (userNo!=null && userNo != BP.Web.WebUser.No)
                    BP.WF.Dev2Interface.Port_Login(userNo);
            }
            catch (Exception ex)
            {
                this.Response.Write("@登录信息WebUser.No丢失，请重新登录。" + ex.Message );
                return;
            }

            try
            {
                string name = BP.Web.WebUser.Name;
            }
            catch (Exception ex)
            {
                this.Response.Write("@登录信息WebUser.Name丢失，请重新登录。" + ex.Message);
                return;
            }

            if (BP.WF.Glo.IsEnableCheckUseSta == true)
            {
                if (BP.WF.Glo.CheckIsEnableWFEmp() == false)
                {
                    this.ToMsg("<font color=red>您的帐号已经被禁用，如果有问题请与管理员联系。</font>", "Info");
                    BP.Web.WebUser.Exit();
                    return;
                }
            }
            #endregion 校验用户是否被禁用

            #region 判断是否有 IsRead
            try
            {
                if (this.IsCC)
                {
                    /*如果是抄送，就设置抄送的读取状态.*/
                    if (this.Request.QueryString["IsRead"] == "0")
                        BP.WF.Dev2Interface.Node_CC_SetRead(this.FK_Node, this.WorkID, BP.Web.WebUser.No);
                }
                else
                {
                    /*正常工作处理*/
                    if (this.Request.QueryString["IsRead"] == "0")
                        BP.WF.Dev2Interface.Node_SetWorkRead(this.FK_Node, this.WorkID);
                }
            }
            catch (Exception ex)
            {
                this.ToMsg("设置读取状态错误,或者当前工作已经被处理,或者当前登录人员已经改变.技术信息:"+ex.Message, ex.Message);
                return;
            }
            #endregion

            #region 判断前置导航.
            if (this.currND.IsStartNode && this.IsCC == false)
            {
               if (BP.WF.Dev2Interface.Flow_IsCanStartThisFlow(this.FK_Flow,WebUser.No)==false)
               {
                   /*是否可以发起流程？*/
                   this.ToMsg("您("+BP.Web.WebUser.No+")没有发起或者处理该流程的权限.","Err");
                   return;
               }
            }

            if (this.WorkID == 0 && this.PWorkID==0 && this.currND.IsStartNode && this.Request.QueryString["IsCheckGuide"] == null)
            {
                switch (this.currFlow.StartGuideWay)
                {
                    case StartGuideWay.None:
                        break;
                    case StartGuideWay.BySystemUrlMulti:
                    case StartGuideWay.BySystemUrlMultiEntity:
                        this.Response.Redirect("StartGuide.aspx?FK_Flow=" + this.currFlow.No, true);
                        break;
                    case StartGuideWay.ByHistoryUrl: // 历史数据.
                        if (this.currFlow.IsLoadPriData == true)
                        {
                            this.ToMsg("流程配置错误，您不能同时启用前置导航，自动装载上一笔数据两个功能。", "Info");
                            return;
                        }
                        this.Response.Redirect("StartGuide.aspx?FK_Flow=" + this.currFlow.No, true);
                        break;
                    case StartGuideWay.BySystemUrlOneEntity:
                    case StartGuideWay.BySQLOne:
                        this.Response.Redirect("StartGuideEntities.aspx?FK_Flow=" + this.currFlow.No, true);
                        return;
                    case StartGuideWay.BySelfUrl: //按照定义的url.
                        this.Response.Redirect( this.currFlow.StartGuidePara1 + this.RequestParas + "&WorkID=" + BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, null, null, WebUser.No, null), true);
                        break;
                    case StartGuideWay.ByFrms: //选择表单.
                        this.Response.Redirect("./WorkOpt/StartGuideFrms.aspx?FK_Flow=" + this.currFlow.No, true);
                        break;
                    default:
                        break;
                }
            }

            string appPath = BP.WF.Glo.CCFlowAppPath; //this.Request.ApplicationPath;
            this.Page.Title = "第" + this.currND.Step + "步:" + this.currND.Name;
            #endregion 判断前置导航

            #region 处理表单类型.
            if (this.currND.HisFormType == NodeFormType.SheetTree
                 || this.currND.HisFormType == NodeFormType.SheetAutoTree
                 //|| this.currND.HisFormType == NodeFormType.ExcelForm
                 //|| this.currND.HisFormType == NodeFormType.WordForm
                )
            {
                /*如果是多表单流程.*/
                string pFlowNo = this.Request.QueryString["PFlowNo"];
                string pWorkID = this.Request.QueryString["PWorkID"];
                string pNodeID = this.Request.QueryString["PNodeID"];
                string pEmp = this.Request.QueryString["PEmp"];
                if (string.IsNullOrEmpty(pEmp))
                    pEmp = WebUser.No;

                if (this.WorkID == 0)
                {
                    if (string.IsNullOrEmpty(pFlowNo) == true)
                        this.WorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, null, null, WebUser.No, null);
                    else
                        this.WorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(this.FK_Flow, null, null, WebUser.No, null, Int64.Parse(pWorkID),0, pFlowNo, int.Parse(pNodeID));

                    currWK = currND.HisWork;
                    currWK.OID = this.WorkID;
                    currWK.Retrieve();
                    this.WorkID = currWK.OID;
                }
                else
                {
                    gwf = new GenerWorkFlow();
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
                        this.ToMsg(BP.WF.Glo.DealExp(this.currFlow.StartLimitAlert, this.currWK, null), "Info");
                        return;
                    }
                }
               

                string toUrl = "";
                if (this.currND.HisFormType == NodeFormType.SheetTree || this.currND.HisFormType == NodeFormType.SheetAutoTree)
                    toUrl = "./FlowFormTree/Default.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&SID=" + WebUser.SID + "&CWorkID=" + this.CWorkID + "&PFlowNo=" + pFlowNo + "&PWorkID=" + pWorkID;
                else
                    toUrl = "./WebOffice/Default.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&SID=" + WebUser.SID + "&CWorkID=" + this.CWorkID + "&PFlowNo=" + pFlowNo + "&PWorkID=" + pWorkID;

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
                this.Response.Redirect(toUrl, true);
                return;
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
                    this.ToMsg("设置读取状流程设计错误态错误", "没有设置表单url.");
                    return;
                }

                //处理连接.
                url = this.DealUrl(currND);

                //sdk表单就让其跳转.
                this.Response.Redirect(url, true);
                return;
            }
            #endregion 处理表单类型.

            #region 判断是否有 workid
            bool isAskFor = false;
            if (this.WorkID == 0)
            {
                currWK = this.currFlow.NewWork();
                this.WorkID = currWK.OID;
                gwf = new GenerWorkFlow();
            }
            else
            {
                gwf = new GenerWorkFlow();
                gwf.WorkID = this.WorkID;
                gwf.RetrieveFromDBSources();

                #region 处理分合流的退回信息.
                if (this.currND.HisRunModel == RunModel.FL || this.currND.HisRunModel == RunModel.FHL)
                {
                    if (gwf.WFState == WFState.ReturnSta)
                    {
                        /*如果是退回的状态，就说明该信息是子线程退回到合流节点.*/
                        this.Response.Redirect("./WorkOpt/DealSubThreadReturnToHL.aspx?FK_Flow="+this.FK_Flow+"&FK_Node="+this.FK_Node+"&WorkID="+this.WorkID+"&FID="+this.FID, true);
                        return;
                    }
                }
                #endregion 处理分合流的退回信息.

                currWK = this.currFlow.GenerWork(this.WorkID, this.currND, this.IsPostBack);
                if (BP.WF.Glo.IsEnableTaskPool && gwf.TaskSta == TaskSta.Takeback)
                {
                    /*如果是任务池状态，并且被人取走，要检查取走的人是不是自己。*/
                }

                string msg = "";
                switch (gwf.WFState)
                {
                    case WFState.AskForReplay: // 返回加签的信息.
                        if (this.IsCC == false)
                        {
                            string mysql = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE WorkID=" + this.WorkID + " AND " + TrackAttr.ActionType + "=" + (int)ActionType.ForwardAskfor;
                            DataTable mydt = BP.DA.DBAccess.RunSQLReturnTable(mysql);
                            foreach (DataRow dr in mydt.Rows)
                            {
                                string msgAskFor = dr[TrackAttr.Msg].ToString();
                                string worker = dr[TrackAttr.EmpFrom].ToString();
                                string workerName = dr[TrackAttr.EmpFromT].ToString();
                                string rdt = dr[TrackAttr.RDT].ToString();

                                //提示信息.
                                this.FlowMsg.AlertMsg_Info(worker + "," + workerName + "回复信息:",
                                    DataType.ParseText2Html(msgAskFor) + "<br>" + rdt);
                            }
                        }
                        break;
                    case WFState.Askfor: //加签.
                        if (this.IsCC == false)
                        {
                            string sql = "SELECT * FROM ND" + int.Parse(this.FK_Flow) + "Track WHERE WorkID=" + this.WorkID + " AND " + TrackAttr.ActionType + "=" + (int)ActionType.AskforHelp;
                            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                            foreach (DataRow dr in dt.Rows)
                            {
                                string msgAskFor = dr[TrackAttr.Msg].ToString();
                                string worker = dr[TrackAttr.EmpFrom].ToString();
                                string workerName = dr[TrackAttr.EmpFromT].ToString();
                                string rdt = dr[TrackAttr.RDT].ToString();

                                //提示信息.
                                this.FlowMsg.AlertMsg_Info(worker + "," + workerName + "请求加签:",
                                    DataType.ParseText2Html(msgAskFor) + "<br>" + rdt + " --<a href='./WorkOpt/AskForRe.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "' >回复加签意见</a> --");
                            }
                            isAskFor = true;
                        }
                        break;
                    case WFState.ReturnSta:
                        /* 如果工作节点退回了*/
                        ReturnWorks rws = new ReturnWorks();
                        rws.Retrieve(ReturnWorkAttr.ReturnToNode, this.FK_Node,
                            ReturnWorkAttr.WorkID, this.WorkID,
                            ReturnWorkAttr.RDT);
                        if (rws.Count != 0)
                        {
                            string msgInfo = "";
                            foreach (BP.WF.ReturnWork rw in rws)
                            {
                                msgInfo += "<fieldset width='100%' ><legend>&nbsp; 来自节点:" + rw.ReturnNodeName + " 退回人:" + rw.ReturnerName + "  " + rw.RDT + "&nbsp;<a href='" + appPath + "DataUser/ReturnLog/" + this.FK_Flow + "/" + rw.MyPK + ".htm' target=_blank>工作日志</a></legend>";
                                msgInfo += rw.NoteHtml;
                                msgInfo += "</fieldset>";
                            }
                            this.FlowMsg.AlertMsg_Info("流程退回提示", msgInfo);
                            //gwf.WFState = WFState.Runing;
                            //gwf.DirectUpdate();
                        }
                        break;
                    case WFState.Shift:
                        /* 判断移交过来的。 */
                        ShiftWorks fws = new ShiftWorks();
                        BP.En.QueryObject qo = new QueryObject(fws);
                        qo.AddWhere(ShiftWorkAttr.WorkID, this.WorkID);
                        qo.addAnd();
                        qo.AddWhere(ShiftWorkAttr.FK_Node, this.FK_Node);
                        qo.addOrderBy(ShiftWorkAttr.RDT);
                        qo.DoQuery();
                        if (fws.Count >= 1)
                        {
                            this.FlowMsg.AddFieldSet("移交历史信息");
                            foreach (ShiftWork fw in fws)
                            {
                                msg = "@移交人[" + fw.FK_Emp + "," + fw.FK_EmpName + "]。@接受人：" + fw.ToEmp + "," + fw.ToEmpName + "。<br>移交原因：-------------" + fw.NoteHtml;
                                if (fw.FK_Emp == WebUser.No)
                                    msg = "<b>" + msg + "</b>";

                                msg = msg.Replace("@", "<br>@");
                                this.FlowMsg.Add(msg + "<hr>");
                            }
                            this.FlowMsg.AddFieldSetEnd();
                        }
                       // gwf.WFState = WFState.Runing;
                       // gwf.DirectUpdate();
                        break;
                    default:
                        break;
                }
            }
            #endregion 判断是否有workid

            #region 判断权限 toolbar.
            if (this.IsPostBack == false)
            {
                if (this.IsCC == false && currND.IsStartNode == false && Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Flow, this.FK_Node, this.WorkID, WebUser.No) == false)
                {
                    this.ToMsg(" @当前的工作已经被处理，或者您没有执行此工作的权限。", "Info");
                    return;
                }

                if (BP.Web.WebUser.UserWorkDev == UserWorkDev.Mobile)
                {
                    this.Response.Redirect("/CCMobile/MyFlow.aspx?WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&FID=" + this.FID, true);
                    return;
                }
            }
            #endregion 判断权限

            #region 处理ctrl显示
            this.ToolBar1.Visible = false;
            this.ToolBar2.Visible = false;
            if (BP.WF.Glo.FlowCtrlBtnPos == "Top")
            {
                this.ToolBar1.Visible = true;
                toolbar = this.ToolBar1;
            }
            else
            {
                this.ToolBar2.Visible = true;
                toolbar = this.ToolBar2;
            }

            //try
            //{
                //初始化控件.
                this.InitToolbar(isAskFor, appPath, gwf);
                this.BindWork(currND, currWK);
                this.Session["Ect"] = null;
            //}
            //catch (Exception ex)
            //{
            //    #region 解决开始节点数据库字段变化修复数据库问题 。
            //    string rowUrl = this.Request.RawUrl;
            //    if (rowUrl.IndexOf("rowUrl") > 1)
            //    {
            //    }
            //    else
            //    {
            //        this.Response.Redirect(rowUrl + "&rowUrl=1", true);
            //        return;
            //    }
            //    #endregion

            //    this.FlowMsg.DivInfoBlock(ex.Message);
            //    string Ect = this.Session["Ect"] as string;
            //    if (Ect == null)
            //        Ect = "0";
            //    if (int.Parse(Ect) < 2)
            //    {
            //        this.Session["Ect"] = int.Parse(Ect) + 1;
            //        return;
            //    }
            //    return;
            //}
            #endregion 处理ctrl显示
        }
        #endregion

        #region 公共方法
        /// <summary>
        /// BindWork
        /// </summary>
        public void BindWork(BP.WF.Node nd, Work wk)
        {
            if (nd.HisFlow.IsMD5 && nd.IsStartNode == false && wk.IsPassCheckMD5() == false)
            {
                this.ToMsg("<font color=red>数据已经被非法篡改，请通知管理员解决问题。</font>", "Info");
                return;
            }

            if (this.IsPostBack == true)
                this.UCEn1.IsLoadData = false;
            else
                this.UCEn1.IsLoadData = true;
 
            switch (nd.HisNodeWorkType)
            {
                case NodeWorkType.StartWorkFL:
                case NodeWorkType.WorkFHL:
                case NodeWorkType.WorkFL:
                case NodeWorkType.WorkHL:
                    if (this.FID != 0 && this.FID != this.WorkID)
                    {
                        /* 这种情况是分流节点向退回到了分河流。*/
                        this.FlowMsg.AddFieldSet("分流节点退回信息");

                        BP.WF.ReturnWork rw = new ReturnWork();
                        rw.Retrieve(ReturnWorkAttr.WorkID, this.WorkID, ReturnWorkAttr.ReturnToNode, nd.NodeID);
                        this.FlowMsg.Add(rw.NoteHtml);
                        this.FlowMsg.AddHR();
                        //this.UCEn1.addb
                        TextBox tb = new TextBox();
                        tb.ID = "TB_Doc";
                        tb.TextMode = TextBoxMode.MultiLine;
                        tb.Rows = 7;
                        tb.Columns = 50;
                        this.UCEn1.Add(tb);

                        this.UCEn1.AddBR();
                        Btn btn = new Btn();
                        btn.ID = "Btn_Reject";
                        btn.Text = "驳回工作";
                        btn.Click += new EventHandler(ToolBar1_ButtonClick);
                        this.FlowMsg.Add(btn);

                        btn = new Btn();
                        btn.ID = "Btn_KillSubFlow";
                        btn.Text = "终止工作";
                        btn.Click += new EventHandler(ToolBar1_ButtonClick);
                        this.FlowMsg.Add(btn);
                        this.FlowMsg.AddFieldSetEnd(); // ("分流节点退回信息");

                        //this.ToolBar1.Controls.Clear();//.Clear();
                        //this.Response.Write("<script language='JavaScript'> DoSubFlowReturn('" + this.FID + "','" + wk.OID + "','" + nd.NodeID + "');</script>");
                        //this.Response.Write("<javascript ></javascript>");
                        return;
                    }
                    break;
                default:
                    break;
            }
            if (nd.IsStartNode)
            {
                /*判断是否来与子流程.*/
                if (string.IsNullOrEmpty(this.Request.QueryString["FromNode"]) == false)
                {
                    if (this.PWorkID == 0)
                        throw new Exception("流程设计错误，调起子流程时，没有接受到PWorkID参数。");

                    ///* 如果来自于主流程 */
                    //int FromNode = int.Parse(this.Request.QueryString["FromNode"]);
                    //BP.WF.Node FromNode_nd = new BP.WF.Node(FromNode);
                    //Work fromWk = FromNode_nd.HisWork;
                    //fromWk.OID = this.PWorkID;
                    //fromWk.RetrieveFromDBSources();
                    //wk.Copy(fromWk);
                    //   wk.FID = this.FID;
                }

                if (this.DoFunc == "SetParentFlow")
                {
                    /*如果需要设置父流程信息。*/
                    string cFlowNo = this.CFlowNo;
                    string[] workids = this.WorkIDs.Split(',');
                    int count = workids.Length - 1;
                    this.Pub1.AddFieldSet("分组审阅", "一共选择了(" + count + ")个子流程被合并审阅,分别是:" + this.WorkIDs);
                }
            }

            // 处理传递过来的参数。
            foreach (string k in this.Request.QueryString.AllKeys)
            {
                wk.SetValByKey(k, this.Request.QueryString[k]);
            }

            wk.ResetDefaultVal();
            wk.DirectUpdate(); //需要把默认值保存里面去，不然，就会导致当前默认信息存储不了。

            NodeFormType ft = nd.HisFormType;
            if (BP.Web.WebUser.IsWap)
                ft = NodeFormType.FixForm;

            switch (nd.HisFormType)
            {
                case NodeFormType.FreeForm:
                case NodeFormType.DisableIt:
                case NodeFormType.WebOffice:
                case NodeFormType.FixForm:
                    Frms frms = nd.HisFrms;
                    if (frms.Count == 0 && nd.HisFormType == NodeFormType.FreeForm)
                    {
                        /* 仅仅只有节点表单的情况。 */
                        /* 添加保存表单函数，以便自定义按钮调用，执行表单的保存前后事件。 */
                        this.UCEn1.Add("\t\n<script type='text/javascript'>");
                        this.UCEn1.Add("\t\n function SaveFormData() {");
                        this.UCEn1.Add("\t\n     var btn = document.getElementById('" + Btn_Save.ClientID + "');");
                        this.UCEn1.Add("\t\n     if (btn) {");
                        this.UCEn1.Add("\t\n         btn.click();");
                        this.UCEn1.Add("\t\n      }");
                        this.UCEn1.Add("\t\n  }");
                        this.UCEn1.Add("\t\n</script>");
                        /* 自由表单 */

                        MapData map = new MapData(nd.NodeFrmID);
                        Width = map.MaxRight + map.MaxLeft * 2 + 10 + "";
                        if (float.Parse(Width) < 500)
                            Width = "900";

                        Height = map.MaxEnd > map.FrmH ? map.MaxEnd + "" : map.FrmH + "";
                        if (float.Parse(Height) <= 800)
                            Height = "800";

                        this.UCEn1.Add("<div id=divCCForm style='width:" + Width + "px;height:" + Height + "px' >");
                        //是否要重新装载数据.
                        bool isLoadData = false;
                        //设置url.
                        if (gwf.WFState == WFState.Runing || gwf.WFState == WFState.Blank || gwf.WFState == WFState.Draft)
                            isLoadData = true;

                        this.UCEn1.BindCCForm(wk, nd.NodeFrmID, false, 0, isLoadData); 
                        if (wk.WorkEndInfo.Length > 2)
                            this.Pub3.Add(wk.WorkEndInfo);
                        this.UCEn1.Add("</div>");

                    }
                    else if (frms.Count == 0 && nd.HisFormType == NodeFormType.FixForm)
                    {
                        /* 仅仅只有节点表单的情况。 */
                        /*傻瓜表单*/
                        MapData map = new MapData("ND" + FK_Node);

                        if (map.TableWidth.Contains("px"))
                            Width = map.TableWidth.Replace("px", "");
                        else
                            Width = map.TableWidth + "";
                        if (map.TableWidth.Equals("100%"))
                            Width = "900";
                        int labCol = 80;
                        int ctrlCol = 260;
                        int width1 = (labCol + ctrlCol) * map.TableCol / 2;
                        Width = width1 + "";

                        Height = map.MaxEnd + "";
                        this.UCEn1.Add("<div id=divCCForm style='width:" + Width + "px;height:" + Height + "px;overflow-x:scroll;' >");
                        this.UCEn1.BindColumn4(wk, nd.NodeFrmID); //, false, false, null);
                        if (wk.WorkEndInfo.Length > 2)
                            this.Pub3.Add(wk.WorkEndInfo);
                        this.UCEn1.Add("</div>");
                    }
                    else
                    {
                        /* 节点表单与独立表单混合存在的情况。  */
                        //隐藏保存按钮
                        if (this.ToolBar1.IsExit(BP.Web.Controls.NamesOfBtn.Save) == true)
                            this.Btn_Save.Visible = false;

                        // 让其直接update，来接受外部传递过来的信息。
                        if (nd.HisFormType != NodeFormType.DisableIt)
                            wk.DirectUpdate();

                        /*涉及到多个表单的情况...*/
                        if (nd.HisFormType != NodeFormType.DisableIt)
                        {
                            Frm myfrm = new Frm();
                            myfrm.No = "ND" + nd.NodeID;
                            myfrm.Name = wk.EnDesc;
                            //myfrm.HisFormType = nd.HisFormType;
                            myfrm.HisFormRunType = (FormRunType)(int)nd.HisFormType;

                            FrmNode fnNode = new FrmNode();
                            fnNode.FK_Frm = myfrm.No;
                            fnNode.IsEdit = true;
                            fnNode.IsPrint = false;
                            switch (nd.HisFormType)
                            {
                                case NodeFormType.FixForm:
                                    fnNode.HisFrmType = FrmType.Column4Frm;
                                    break;
                                case NodeFormType.FreeForm:
                                    fnNode.HisFrmType = FrmType.FreeFrm;
                                    break;
                                case NodeFormType.SelfForm:
                                    fnNode.HisFrmType = FrmType.Url;
                                    break;
                                default:
                                    throw new Exception("出现了未判断的异常。");
                            }
                            myfrm.HisFrmNode = fnNode;
                            frms.AddEntity(myfrm, 0);
                        }

                        Int64 fid = this.FID;
                        if (this.FID == 0)
                            fid = this.WorkID;

                        if (frms.Count == 1)
                        {
                            /* 仅仅只有一个独立表单的情况。 */
                            Frm frm = (Frm)frms[0];
                            FrmNode fn = frm.HisFrmNode;
                            string src = "";

                            #region update by dgq 一个表单也添加tab页.
                            //src = "./CCForm/" + fn.FrmUrl + ".aspx?FK_MapData=" + frm.No + "&FID=" + fid + "&IsEdit=" + fn.IsEditInt + "&IsPrint=" + fn.IsPrintInt + "&FK_Node=" + nd.NodeID + "&WorkID=" + this.WorkID;
                            //this.UCEn1.Add("\t\n <DIV id='" + frm.No + "' style='width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;' >");
                            //this.UCEn1.Add("\t\n <iframe ID='F" + frm.No + "' src='" + src + "' frameborder=0  style='position:absolute;width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;'  leftMargin='0'  topMargin='0'  /></iframe>");
                            //this.UCEn1.Add("\t\n </DIV>");

                            //this.UCEn1.Add("\t\n<script type='text/javascript'>");
                            //this.UCEn1.Add("\t\n function SaveDtlAll(){}");
                            //this.UCEn1.Add("\t\n</script>");
                            #endregion

                            //********************BEGIN****************************
                            #region 载入相关文件.
                            this.Page.RegisterClientScriptBlock("sg", "<link href='" + BP.WF.Glo.CCFlowAppPath + "WF/Style/Frm/Tab.css' rel='stylesheet' type='text/css' />");
                            this.Page.RegisterClientScriptBlock("s2g4", "<script language='JavaScript' src='" + BP.WF.Glo.CCFlowAppPath + "WF/Style/Frm/jquery.min.js' ></script>");
                            this.Page.RegisterClientScriptBlock("sdf24j", "<script language='JavaScript' src='" + BP.WF.Glo.CCFlowAppPath + "WF/Style/Frm/jquery.idTabs.min.js' ></script>");
                            this.Page.RegisterClientScriptBlock("sdsdf24j", "<script language='JavaScript' src='" + BP.WF.Glo.CCFlowAppPath + "WF/Style/Frm/TabClick.js' ></script>");
                            #endregion 载入相关文件.

                            #region 参数.
                            string urlExtFrm = this.RequestParas;
                            if (urlExtFrm.Contains("WorkID") == false)
                                urlExtFrm += "&WorkID=" + this.WorkID;

                            if (urlExtFrm.Contains("NodeID") == false)
                                urlExtFrm += "&NodeID=" + nd.NodeID;

                            if (urlExtFrm.Contains("FK_Node") == false)
                                urlExtFrm += "&FK_Node=" + nd.NodeID;

                            if (urlExtFrm.Contains("UserNo") == false)
                                urlExtFrm += "&UserNo=" + WebUser.No;

                            if (urlExtFrm.Contains("SID") == false)
                                urlExtFrm += "&SID=" + WebUser.SID;

                            //设置url.
                            if (gwf.WFState == WFState.Runing || gwf.WFState == WFState.Blank || gwf.WFState == WFState.Draft)
                            {
                                if (urlExtFrm.Contains("IsLoadData") == false)
                                    urlExtFrm += "&IsLoadData=1";
                                else
                                    urlExtFrm = urlExtFrm.Replace("&IsLoadData=1", "&IsLoadData=0");
                            }

                            #endregion 载入相关文件.

                            src = fn.FrmUrl + ".aspx?FK_MapData=" + frm.No + "&FID=" + fid + "&IsEdit=" + fn.IsEditInt + "&IsPrint=" + fn.IsPrintInt + urlExtFrm;

                            Width = frm.FrmW + "";

                            this.UCEn1.Add("\t\n<div  id='usual2'  class='usual' style='width:" + frm.FrmW + "px;height:auto;margin:0 auto;background-color:white;'>");  //begain.

                            #region 输出标签.
                            this.UCEn1.Add("\t\n <ul  class='abc' style='background:red;border-color: #800000;border-width: 10px;' >");
                            this.UCEn1.Add("\t\n<li><a ID='HL" + frm.No + "' href=\"#" + frm.No + "\" onclick=\"TabClick('" + frm.No + "','" + src + "');\" >" + frm.Name + "</a></li>");
                            this.UCEn1.Add("\t\n </ul>");
                            #endregion 输出标签.

                            #region 输出表单 iframe 内容.
                            this.UCEn1.Add("\t\n <DIV id='" + frm.No + "' style='width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;' >");
                            this.UCEn1.Add("\t\n <iframe ID='F" + frm.No + "' Onblur=\"OnTabChange('" + frm.No + "',this);\" src='" + src + "' frameborder=0  style='width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;'  leftMargin='0'  topMargin='0'   /></iframe>");
                            this.UCEn1.Add("\t\n </DIV>");
                            #endregion 输出表单 iframe 内容.

                            this.UCEn1.Add("\t\n</div>"); // end  usual2

                            // 设置选择的默认值.
                            this.UCEn1.Add("\t\n<script type='text/javascript'>");
                            this.UCEn1.Add("\t\n  $(\"#usual2 ul\").idTabs(\"" + frm.No + "\");");

                            #region SaveDtlAll
                            this.UCEn1.Add("\t\n function SaveDtlAll(){");
                            this.UCEn1.Add("\t\n   var tabText = document.getElementById('HL" + frm.No + "').innerText;");
                            this.UCEn1.Add("\t\n   var scope = document.getElementById('F" + frm.No + "');");
                            this.UCEn1.Add("\t\n   var lastChar = tabText.substring(tabText.length - 1, tabText.length);");
                            this.UCEn1.Add("\t\n   if (lastChar == \"*\") {");
                            this.UCEn1.Add("\t\n   var contentWidow = scope.contentWindow;");
                            this.UCEn1.Add("\t\n   contentWidow.SaveDtlData();");
                            this.UCEn1.Add("\t\n   }");
                            this.UCEn1.Add("\t\n}");
                            #endregion

                            this.UCEn1.Add("\t\n</script>");
                            //*********************END***************************

                        }
                        else
                        {
                            /* 节点表单与独立表单混合存在。 */
                            #region 载入相关文件.
                            this.Page.RegisterClientScriptBlock("sg", "<link href='" + BP.WF.Glo.CCFlowAppPath + "WF/Style/Frm/Tab.css' rel='stylesheet' type='text/css' />");
                            this.Page.RegisterClientScriptBlock("s2g4", "<script language='JavaScript' src='" + BP.WF.Glo.CCFlowAppPath + "WF/Style/Frm/jquery.min.js' ></script>");
                            this.Page.RegisterClientScriptBlock("sdf24j", "<script language='JavaScript' src='" + BP.WF.Glo.CCFlowAppPath + "WF/Style/Frm/jquery.idTabs.min.js' ></script>");
                            this.Page.RegisterClientScriptBlock("sdsdf24j", "<script language='JavaScript' src='" + BP.WF.Glo.CCFlowAppPath + "WF/Style/Frm/TabClick.js' ></script>");
                            #endregion 载入相关文件.

                            #region 参数.
                            string urlExtFrm = this.RequestParas;
                            if (urlExtFrm.Contains("WorkID") == false)
                                urlExtFrm += "&WorkID=" + this.WorkID;

                            if (urlExtFrm.Contains("NodeID") == false)
                                urlExtFrm += "&NodeID=" + nd.NodeID;

                            if (urlExtFrm.Contains("FK_Node") == false)
                                urlExtFrm += "&FK_Node=" + nd.NodeID;

                            if (urlExtFrm.Contains("UserNo") == false)
                                urlExtFrm += "&UserNo=" + WebUser.No;

                            if (urlExtFrm.Contains("SID") == false)
                                urlExtFrm += "&SID=" + WebUser.SID;

                            //设置url.
                            if (gwf.WFState == WFState.Runing || gwf.WFState == WFState.Blank || gwf.WFState == WFState.Draft)
                            {
                                if (urlExtFrm.Contains("IsLoadData") == false)
                                    urlExtFrm += "&IsLoadData=1";
                                else
                                    urlExtFrm = urlExtFrm.Replace("&IsLoadData=1", "&IsLoadData=0");
                            }
                            #endregion 载入相关文件.

                            Frm frmFirst = null;
                            foreach (Frm frm in frms)
                            {
                                if (frmFirst == null) frmFirst = frm;

                                if (frmFirst.FrmW < frm.FrmW)
                                    frmFirst = frm;
                            }

                            this.UCEn1.Clear();
                            this.UCEn1.Add("<div  style='clear:both' ></div>"); //
                            this.UCEn1.Add("\t\n<div  id='usual2' class='usual' style='width:" + frmFirst.FrmW + "px;height:auto;margin:0 auto;background-color:white;'>");  //begain.
                            Width = frmFirst.FrmW + "";

                            //added by liuxc,修改了多个excel表单放于一个页面中的逻辑，2015.02.07
                            var excelFrmNos = string.Empty;
                            var excelFrmIdx = 0;
                            var excelFrmCount = 0;
                            foreach(Frm frm in frms)
                            {
                                if (frm.HisFrmNode.HisFrmType != FrmType.ExcelFrm)
                                    continue;
                                excelFrmNos += frm.No + ",";
                                excelFrmCount ++;
                            }
                            //end added

                            #region 输出标签.
                            this.UCEn1.Add("\t\n <ul  class='abc' style='background:red;border-color: #800000;border-width: 10px;' >");
                            foreach (Frm frm in frms)
                            {
                                FrmNode fn = frm.HisFrmNode;

                                //2015.02.07,added by liuxc
                                if(fn.HisFrmType == FrmType.ExcelFrm)
                                {
                                    if (excelFrmIdx >= 1)
                                        continue;

                                    excelFrmIdx++;
                                }
                                //end added

                                string src = "";
                                //src = fn.FrmUrl + ".aspx?FK_MapData=" + frm.No + "&IsEdit=" + fn.IsEditInt + "&IsPrint=" + fn.IsPrintInt + urlExtFrm;
                                //this.UCEn1.Add("\t\n<li><a ID='HL" + frm.No + "' href=\"#" + frm.No + "\" onclick=\"TabClick('" + frm.No + "','" + src + "');\" >" + frm.Name + "</a></li>");

                                //2015.02.07,edited by liuxc
                                src = fn.FrmUrl + ".aspx?FK_MapData=" + (fn.HisFrmType == FrmType.ExcelFrm ? excelFrmNos.TrimEnd(',') : frm.No) + "&IsEdit=" + fn.IsEditInt + "&IsPrint=" + fn.IsPrintInt + urlExtFrm;
                                this.UCEn1.Add("\t\n<li><a ID='HL" + frm.No + "' href=\"#" + frm.No + "\" onclick=\"TabClick('" + frm.No + "','" + src + "');\" >" +(fn.HisFrmType == FrmType.ExcelFrm ? string.Format("Excel表单[{0}]",excelFrmCount) : frm.Name) + "</a></li>");
                            }
                            this.UCEn1.Add("\t\n </ul>");
                            #endregion 输出标签.

                            #region 输出表单 iframe 内容.
                            excelFrmIdx = 0;
                            foreach (Frm frm in frms)
                            {
                                FrmNode fn = frm.HisFrmNode;

                                //2015.02.07,added by liuxc
                                if (fn.HisFrmType == FrmType.ExcelFrm)
                                {
                                    if (excelFrmIdx >= 1)
                                        continue;

                                    excelFrmIdx++;
                                }
                                //end added

                                this.UCEn1.Add("\t\n <DIV id='" + frm.No + "' style='width:" + frmFirst.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;margin:0px;padding:0px;' >");
                                this.UCEn1.Add("\t\n <iframe ID='F" + frm.No + "' Onblur=\"OnTabChange('" + frm.No + "',this);\" src='loading.htm' frameborder=0  style='margin:0px;padding:0px;width:" + frm.FrmW + "px; height:" + frm.FrmH + "px;text-align: left;' /></iframe>");
                                this.UCEn1.Add("\t\n </DIV>");
                            }
                            #endregion 输出表单 iframe 内容.

                            this.UCEn1.Add("\t\n</div>"); // end  usual2

                            // 设置选择的默认值.
                            this.UCEn1.Add("\t\n<script type='text/javascript'>");
                            this.UCEn1.Add("\t\n  $(\"#usual2 ul\").idTabs(\"" + frms[0].No + "\");");
                            this.UCEn1.Add("\t\n function SaveDtlAll(){}");

                            #region SaveDtlAll
                            this.UCEn1.Add("\t\n function SaveDtlAll(){");
                            this.UCEn1.Add("\t\n   var tabText = document.getElementById('HL' + currentTabId).innerText;");
                            this.UCEn1.Add("\t\n   var scope = document.getElementById('F' + currentTabId);");
                            this.UCEn1.Add("\t\n   var lastChar = tabText.substring(tabText.length - 1, tabText.length);");
                            this.UCEn1.Add("\t\n   if (lastChar == \"*\") {");
                            this.UCEn1.Add("\t\n   var contentWidow = scope.contentWindow;");
                            this.UCEn1.Add("\t\n   contentWidow.SaveDtlData();");
                            this.UCEn1.Add("\t\n   }");
                            this.UCEn1.Add("\t\n}");
                            #endregion

                            this.UCEn1.Add("\t\n</script>");
                        }
                    }
                    return;
                case NodeFormType.SelfForm:
                    wk.Save();
                    if (this.WorkID == 0)
                        this.WorkID = wk.OID;

                    string url = this.DealUrl(currND);

                    #region 在这里增加 url.
                    string info = "<iframe ID='SelfForm' src='" + url + "' frameborder=0  style='width:100%; height:800px' leftMargin='0' topMargin='0' />";
                    info += "\t\n</iframe>";
                    this.UCEn1.Add(info);
                    #endregion

                    return;
                case NodeFormType.SDKForm:
                default:
                    throw new Exception("@没有涉及到的扩充。" + nd.HisFormType + " 节点表单类型.");
            }
        }
        /// <summary>
        /// 生成js.
        /// </summary>
        /// <param name="en"></param>
        public void OutJSAuto(Entity en)
        {
            if (en.EnMap.IsHaveJS == false)
                return;

            Attrs attrs = en.EnMap.Attrs;
            string js = "";
            foreach (Attr attr in attrs)
            {
                if (attr.UIContralType != UIContralType.TB)
                    continue;

                if (attr.IsNum == false)
                    continue;

                string tbID = "TB_" + attr.Key;
                TB tb = this.UCEn1.GetTBByID(tbID);
                if (tb == null)
                    continue;

                tb.Attributes["OnKeyPress"] = "javascript:C();";
                tb.Attributes["onkeyup"] = "javascript:C();";

                if (attr.MyDataType == DataType.AppInt)
                    tb.Attributes["OnKeyDown"] = "javascript:return VirtyInt(this);";
                else
                    tb.Attributes["OnKeyDown"] = "javascript:return VirtyNum(this);";

                //   tb.Attributes["OnKeyDown"] = "javascript:return VirtyNum(this);";

                if (attr.MyDataType == DataType.AppMoney)
                    tb.Attributes["onblur"] = "this.value=VirtyMoney(this.value);";

                if (attr.AutoFullWay == AutoFullWay.Way1_JS)
                {
                    js += attr.Key + "," + attr.AutoFullDoc + "~";
                    tb.Enabled = true;
                }
            }

            string[] strs = js.Split('~');
            ArrayList al = new ArrayList();
            foreach (string str in strs)
            {
                if (str == null || str == "")
                    continue;

                string key = str.Substring(0, str.IndexOf(','));
                string exp = str.Substring(str.IndexOf(',') + 1);

                string left = "\n  document.forms[0].UCEn1_TB_" + key + ".value = ";
                foreach (Attr attr in attrs)
                {
                    exp = exp.Replace("@" + attr.Key, "  parseFloat( document.forms[0].UCEn1_TB_" + attr.Key + ".value.replace( ',' ,  '' ) ) ");
                    exp = exp.Replace("@" + attr.Desc, " parseFloat( document.forms[0].UCEn1_TB_" + attr.Key + ".value.replace( ',' ,  '' ) ) ");
                }
                al.Add(left + exp);
            }
            string body = "";
            foreach (string s in al)
            {
                body += s;
            }
            this.Response.Write("<script language='JavaScript'> function  C(){ " + body + " }  \n </script>");
        }
        /// <summary>
        /// 显示 关联表单
        /// </summary>
        /// <param name="nd"></param>
        public void ShowSheets(BP.WF.Node nd, Work currWk)
        {
            //if (nd.HisFJOpen != FJOpen.None)
            //{
            //    string url = "FileManager.aspx?WorkID=" + this.WorkID + "&FID=" + currWk.FID
            //        + "&FJOpen=" + (int)nd.HisFJOpen + "&FK_Node=" + nd.NodeID;
            //    this.Pub1.Add("<iframe leftMargin='0' topMargin='0' src='" + url + "' width='100%' height='200px' class=iframe name=fm style='border-style:none;' id=fm > </iframe>");
            //}
            //this.Pub1.AddIframe("FileManager.aspx?WorkID="+this.WorkID+"&FID=0&FJOpen=1&FK_Node="+nd.NodeID);

            if (this.Pub1.Controls.Count > 20)
                return;

            // 显示设置的步骤.
            string[] strs = nd.ShowSheets.Split('@');

            if (strs.Length >= 1)
                this.Pub1.AddHR();

            foreach (string str in strs)
            {
                if (str == null || str == "")
                    continue;

                int FK_Node = int.Parse(str);
                BP.WF.Node mynd;
                try
                {
                    mynd = new BP.WF.Node(FK_Node);
                }
                catch
                {
                    nd.ShowSheets = nd.ShowSheets.Replace("@" + FK_Node, "");
                    nd.Update();
                    continue;
                }

                Work nwk = mynd.HisWork;
                nwk.OID = this.WorkID;
                if (nwk.RetrieveFromDBSources() == 0)
                    continue;

                // this.Pub1.AddB("== " + mynd.Name + " ==<hr width=90%>");

                this.Pub1.AddFieldSet("历史步骤:" + mynd.Name);
                // this.Pub1.DivInfoBlockBegin();
                // this.Pub1.ADDWork(nwk,nd.NodeID);
                this.Pub1.AddFieldSetEnd(); // (mynd.Name);
                //this.Pub1.DivInfoBlockEnd(); // (mynd.Name);
            }
        }
        #endregion

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// 设计器支持所需的方法 - 不要使用代码编辑器修改
        /// 此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion

        #region toolbar 2
        private void ToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            string id = "";
            Btn btn = sender as Btn;
            if (btn != null)
                id = btn.ID;
            switch (btn.ID)
            {
                case "Btn_Reject":
                case "Btn_KillSubFlow":
                    try
                    {
                        WorkFlow wkf = new WorkFlow(this.FK_Flow, this.WorkID);
                        if (btn.ID == "Btn_KillSubFlow")
                        {
                            this.ToMsg("删除流程信息:<hr>" + wkf.DoDeleteWorkFlowByReal(true), "info");
                        }
                        else
                        {
                            string msg = wkf.DoReject(this.FID, this.FK_Node, this.UCEn1.GetTextBoxByID("TB_Doc").Text);
                            this.ToMsg(msg, "info");
                        }
                        return;
                    }
                    catch (Exception ex)
                    {
                        this.ToMsg(ex.Message, "info");
                        return;
                    }

                case NamesOfBtn.Delete:
                case "Btn_Del":
                    // 这是彻底删除的不需要交互。
                    string delMsg = BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.FK_Flow, this.WorkID, true);
                    this.ToMsg("删除流程提示<hr>" + delMsg, "info");
                    break;
                case NamesOfBtn.Save:
                    this.Send(true);
                    if (string.IsNullOrEmpty(this.Request.QueryString["WorkID"]))
                    {
                        // this.Response.Redirect(this.PageID + ".aspx?FID=" + this.FID + "&WorkID=" + this.WorkID + "&FK_Node=" + this.FK_Node + "&FK_Flow=" + this.FK_Flow + "&FromNode=" + this.FromNode+"&PWorkID="+this.PWorkID, true);
                        return;
                    }
                    break;
                case "Btn_ReturnWork":
                    this.BtnReturnWork();
                    break;
                case BP.Web.Controls.NamesOfBtn.Shift:
                    this.DoShift();
                    break;
                case "Btn_WorkerList":
                    if (WorkID == 0)
                        throw new Exception("没有指定当前的工作,不能查看工作者列表.");
                    break;
                case "Btn_PrintWorkRpt":
                    if (WorkID == 0)
                        throw new Exception("没有指定当前的工作,不能打印工作报告.");
                    this.WinOpen("WFRpt.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + WorkID, "工作报告", 800, 600);
                    break;
                case NamesOfBtn.Send:
                    this.Send(false);
                    break;
                default:
                    break;
            }
            //}
            //catch (Exception ex)
            //{
            //    this.FlowMsg.AlertMsg_Warning("信息提示", ex.Message);
            //}
        }
        #region 按钮事件
        /// <summary>
        /// 保存工作
        /// </summary>
        /// <param name="isDraft">是不是做为草稿保存</param> 
        private void Send(bool isSave)
        {
            // 判断当前人员是否有执行该人员的权限。
            if (currND.IsStartNode == false
                && BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(this.FK_Flow, this.FK_Node, this.WorkID, WebUser.No) == false)
                throw new Exception("您好：" + WebUser.No + "," + WebUser.Name + "：<br> 当前工作已经被其它人处理，您不能在执行保存或者发送!!!");

            Paras ps = new Paras();
            string dtStr = SystemConfig.AppCenterDBVarStr;
            try
            {
                switch (currND.HisFormType)
                {
                    case NodeFormType.SelfForm:
                        break;
                    case NodeFormType.FixForm:
                    case NodeFormType.FreeForm:
                        currWK = (Work)this.UCEn1.Copy(this.currWK);

                        // 设置默认值....
                        MapAttrs mattrs = currND.MapData.MapAttrs;
                        foreach (MapAttr attr in mattrs)
                        {
                            if (attr.TBModel == 2)
                            {
                                /* 如果是富文本 */
                                currWK.SetValByKey(attr.KeyOfEn, this.Request.Form["ctl00$ContentPlaceHolder1$MyFlowUC1$MyFlow1$UCEn1$TB_" + attr.KeyOfEn]);
                            }

                            if (attr.UIIsEnable)
                                continue;
                            if (attr.DefValReal.Contains("@") == false)
                                continue;
                            currWK.SetValByKey(attr.KeyOfEn, attr.DefVal);
                        }
                        break;
                    case NodeFormType.DisableIt:
                        currWK.Retrieve();
                        break;
                    default:
                        throw new Exception("@未涉及到的情况。");
                }
            }
            catch (Exception ex)
            {
                this.Btn_Send.Enabled = true;
                throw new Exception("@在保存前执行逻辑检查错误。" + ex.Message + " @StackTrace:" + ex.StackTrace);
            }

            #region 判断特殊的业务逻辑
            string dbStr = SystemConfig.AppCenterDBVarStr;
            if (currND.IsStartNode)
            {
                if (this.currND.HisFlow.HisFlowAppType == FlowAppType.PRJ)
                {
                    /*对特殊的流程进行检查，检查是否有权限。*/
                    string prjNo = currWK.GetValStringByKey("PrjNo");
                    ps = new Paras();
                    ps.SQL = "SELECT * FROM WF_NodeStation WHERE FK_Station IN ( SELECT FK_Station FROM Prj_EmpPrjStation WHERE FK_Prj=" + dbStr + "FK_Prj AND FK_Emp=" + dbStr + "FK_Emp )  AND  FK_Node=" + dbStr + "FK_Node ";
                    ps.Add("FK_Prj", prjNo);
                    ps.AddFK_Emp();
                    ps.Add("FK_Node", this.FK_Node);

                    if (DBAccess.RunSQLReturnTable(ps).Rows.Count == 0)
                    {
                        string prjName = currWK.GetValStringByKey("PrjName");
                        ps = new Paras();
                        ps.SQL = "SELECT * FROM Prj_EmpPrj WHERE FK_Prj=" + dbStr + "FK_Prj AND FK_Emp=" + dbStr + "FK_Emp ";
                        ps.Add("FK_Prj", prjNo);
                        ps.AddFK_Emp();
                        //   ps.AddFK_Emp();

                        if (DBAccess.RunSQLReturnTable(ps).Rows.Count == 0)
                            throw new Exception("您不是(" + prjNo + "," + prjName + ")成员，您不能发起改流程。");
                        else
                            throw new Exception("您属于这个项目(" + prjNo + "," + prjName + ")，但是在此项目下您没有发起改流程的岗位。");
                    }
                }
            }
            #endregion 判断特殊的业务逻辑。

            currWK.Rec = WebUser.No;
            currWK.SetValByKey("FK_Dept", WebUser.FK_Dept);
            currWK.SetValByKey("FK_NY", BP.DA.DataType.CurrentYearMonth);

            // 处理节点表单保存事件.
            currND.MapData.FrmEvents.DoEventNode(FrmEventList.SaveBefore, currWK);
            try
            {
                if (currND.IsStartNode)
                    currWK.FID = 0;

                if (currND.HisFlow.IsMD5)
                {
                    /*重新更新md5值.*/
                    currWK.SetValByKey("MD5", BP.WF.Glo.GenerMD5(currWK));
                }

                if (currND.IsStartNode && isSave)
                    currWK.SetValByKey(StartWorkAttr.Title, WorkNode.GenerTitle(currND.HisFlow, this.currWK));

                currWK.Update();
                /*如果是保存*/

                //为草稿设置标题.
                if (this.currND.IsStartNode == true)
                {
                    if (this.currFlow.DraftRole != DraftRole.None)
                    {
                        string title = WorkNode.GenerTitle(this.currFlow, currWK);
                        BP.WF.Dev2Interface.Flow_SetFlowTitle(this.FK_Flow,this.WorkID, title);
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    currWK.CheckPhysicsTable();
                }
                catch (Exception ex1)
                {
                    throw new Exception("@保存错误:" + ex.Message + "@检查物理表错误：" + ex1.Message);
                }
                this.Btn_Send.Enabled = true;
                this.Pub1.AlertMsg_Warning("错误", ex.Message + "@有可能此错误被系统自动修复,请您从新保存一次.");
                return;
            }

            #region 处理保存后事件
            bool isHaveSaveAfter = false;
            try
            {
                //处理表单保存后。
                string s = currND.MapData.FrmEvents.DoEventNode(FrmEventList.SaveAfter, currWK);
                if (s != null)
                {
                    /*如果不等于null,说明已经执行过数据保存，就让其从数据库里查询一次。*/
                    currWK.RetrieveFromDBSources();
                    isHaveSaveAfter = true;
                }
            }
            catch (Exception ex)
            {
                this.FlowMsg.AddFieldSetGreen("执行SaveAfter事件错误");
                this.FlowMsg.Add(ex.Message.Replace("@@", "@").Replace("@", "<BR>@"));
                this.FlowMsg.AddFieldSetEnd();
                return;
            }
            #endregion

            #region 2012-10-15  数据也要保存到Rpt表里.
            if (currND.SaveModel == SaveModel.NDAndRpt)
            {
                /* 如果保存模式是节点表与Node与Rpt表. */
                WorkNode wn = new WorkNode(currWK, currND);
                GERpt rptGe = currND.HisFlow.HisGERpt;
                rptGe.SetValByKey("OID", this.WorkID);
                wn.rptGe = rptGe;
                if (rptGe.RetrieveFromDBSources() == 0)
                {
                    rptGe.SetValByKey("OID", this.WorkID);
                    wn.DoCopyRptWork(currWK);

                    rptGe.SetValByKey(GERptAttr.FlowEmps, "@" + WebUser.No + "," + WebUser.Name);
                    rptGe.SetValByKey(GERptAttr.FlowStarter, WebUser.No);
                    rptGe.SetValByKey(GERptAttr.FlowStartRDT, DataType.CurrentDataTime);
                    rptGe.SetValByKey(GERptAttr.WFState, 0);

                    rptGe.WFState = WFState.Draft;

                    rptGe.SetValByKey(GERptAttr.FK_NY, DataType.CurrentYearMonth);
                    rptGe.SetValByKey(GERptAttr.FK_Dept, WebUser.FK_Dept);
                    rptGe.Insert();
                }
                else
                {
                    wn.DoCopyRptWork(currWK);
                    rptGe.Update();
                }
            }
            #endregion

            string msg = "";
            // 调用工作流程，处理节点信息采集后保存后的工作。
            if (isSave == true)
            {
                if (isHaveSaveAfter)
                {
                    /*如果有保存后事件，就让其重新绑定. */
                    currWK.RetrieveFromDBSources();
                    this.UCEn1.ResetEnVal(currWK);
                }
                //处理草稿.
                if (currND.IsStartNode == true && this.currFlow.DraftRole != DraftRole.None)
                    BP.WF.Dev2Interface.Node_SetDraft(this.FK_Flow, currWK.OID);

                if (string.IsNullOrEmpty(this.Request.QueryString["WorkID"]))
                    return;

                currWK.RetrieveFromDBSources();
                this.UCEn1.ResetEnVal(currWK);
                return;
            }

            //检查是否是退回?
            if (gwf.WFState == WFState.ReturnSta && gwf.Paras_IsTrackBack == false)
            {
                /* 如果是退回 */
            }
            else
            {
                if (currND.CondModel == CondModel.ByUserSelected && currND.HisToNDNum > 1)
                {
                    //如果是用户选择的方向条件.
                    this.Response.Redirect("./WorkOpt/ToNodes.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID, true);
                    return;
                }
            }

            /*判断是否是转入接受人界面才能选择.*/
            // if (this.currND.hissele

            //执行发送.
            WorkNode firstwn = new WorkNode(this.currWK, this.currND);
            try
            {
                msg = firstwn.NodeSend().ToMsgOfHtml();
            }
            catch (Exception exSend)
            {
                if (exSend.Message.Contains("请选择下一步骤工作") == true || exSend.Message.Contains("用户没有选择发送到的节点") == true)
                {
                    /*如果抛出异常，我们就让其转入选择到达的节点里, 在节点里处理选择人员. */
                    this.Response.Redirect("./WorkOpt/ToNodes.aspx?FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID, true);
                    return;
                }
                //绑定独立表单，表单自定义方案验证错误弹出窗口进行提示
                if (this.currND.HisFrms != null && this.currND.HisFrms.Count > 0 && exSend.Message.Contains("在提交前检查到如下必输字段填写不完整") == true)
                {
                    this.Alert(exSend.Message.Replace("@@", "@").Replace("@", "<BR>@"));
                    return;
                }
                BP.WF.Dev2Interface.Port_SendMsg("admin", currFlow.Name + "在" + currND.Name + "节点处，出现错误", msg, "Err" + currND.No + "_" + this.WorkID, SMSMsgType.Err, this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
                this.FlowMsg.AddFieldSetGreen("错误");
                this.FlowMsg.Add(exSend.Message.Replace("@@", "@").Replace("@", "<BR>@"));
                this.FlowMsg.AddFieldSetEnd();
                return;
            }

            #region 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
            try
            {
                //处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.
                BP.WF.Glo.DealBuinessAfterSendWork(this.FK_Flow, this.WorkID, this.DoFunc, WorkIDs, this.CFlowNo, 0, null);
            }
            catch (Exception ex)
            {
                this.ToMsg(msg, ex.Message);
                return;
            }
            #endregion 处理通用的发送成功后的业务逻辑方法，此方法可能会抛出异常.


            this.Btn_Send.Enabled = false;
            /*处理转向问题.*/
            switch (firstwn.HisNode.HisTurnToDeal)
            {
                case TurnToDeal.SpecUrl:
                    string myurl = firstwn.HisNode.TurnToDealDoc.Clone().ToString();
                    if (myurl.Contains("?") == false)
                        myurl += "?1=1";
                    Attrs myattrs = firstwn.HisWork.EnMap.Attrs;
                    Work hisWK = firstwn.HisWork;
                    foreach (Attr attr in myattrs)
                    {
                        if (myurl.Contains("@") == false)
                            break;
                        myurl = myurl.Replace("@" + attr.Key, hisWK.GetValStrByKey(attr.Key));
                    }
                    if (myurl.Contains("@"))
                    {
                        BP.WF.Dev2Interface.Port_SendMsg("admin", currFlow.Name + "在" + currND.Name + "节点处，出现错误", "流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl, "Err" + currND.No + "_" + this.WorkID, SMSMsgType.Err, this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
                        throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + myurl);
                    }

                    if (myurl.Contains("PWorkID") == false)
                        myurl += "&PWorkID=" + this.WorkID;

                    myurl += "&FromFlow=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
                    this.Response.Redirect(myurl, true);
                    return;
                case TurnToDeal.TurnToByCond:
                    TurnTos tts = new TurnTos(this.FK_Flow);
                    if (tts.Count == 0)
                    {
                        BP.WF.Dev2Interface.Port_SendMsg("admin", currFlow.Name + "在" + currND.Name + "节点处，出现错误", "您没有设置节点完成后的转向条件。", "Err" + currND.No + "_" + this.WorkID, SMSMsgType.Err, this.FK_Flow, this.FK_Node, this.WorkID, this.FID);
                        throw new Exception("@您没有设置节点完成后的转向条件。");
                    }
                    foreach (TurnTo tt in tts)
                    {
                        tt.HisWork = firstwn.HisWork;
                        if (tt.IsPassed == true)
                        {
                            string url = tt.TurnToURL.Clone().ToString();
                            if (url.Contains("?") == false)
                                url += "?1=1";
                            Attrs attrs = firstwn.HisWork.EnMap.Attrs;
                            Work hisWK1 = firstwn.HisWork;
                            foreach (Attr attr in attrs)
                            {
                                if (url.Contains("@") == false)
                                    break;
                                url = url.Replace("@" + attr.Key, hisWK1.GetValStrByKey(attr.Key));
                            }
                            if (url.Contains("@"))
                                throw new Exception("流程设计错误，在节点转向url中参数没有被替换下来。Url:" + url);

                            url += "&PFlowNo=" + this.FK_Flow + "&FromNode=" + this.FK_Node + "&PWorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&SID=" + WebUser.SID;
                            this.Response.Redirect(url, true);
                            return;
                        }
                    }

                    this.ToMsg(msg, "info");
                    //throw new Exception("您定义的转向条件不成立，没有出口。");
                    break;
                default:
                    this.ToMsg(msg, "info");
                    break;
            }
            return;
        }


        public void ToMsg(string msg, string type)
        {
            this.Session["info"] = msg;
            this.Application["info" + WebUser.No] = msg;
            BP.WF.Glo.SessionMsg = msg;
            this.Response.Redirect("MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);

            //if (this.PageID.Contains("Single") == true)
            //    this.Response.Redirect("MyFlowInfoSmallSingle.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
            //else if (this.PageID.Contains("Small"))
            //    this.Response.Redirect("MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
            //else
            //    this.Response.Redirect("MyFlowInfo.aspx?FK_Flow=" + this.FK_Flow + "&FK_Type=" + type + "&FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID, false);
        }
        public void BtnReturnWork()
        {
            BP.WF.Node nd = new BP.WF.Node(this.FK_Node);
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.Retrieve();
            wk = (Work)this.UCEn1.Copy(wk);

            string msg = BP.WF.Glo.DealExp(nd.FocusField, wk, null);
            this.Response.Redirect("./WorkOpt/ReturnWork.aspx?FK_Node=" + this.FK_Node + "&FID=" + wk.FID + "&WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&Info=" + msg, true);
            return;
        }
        public void DoShift()
        {
            //GenerWorkFlow gwf = new GenerWorkFlow();
            //if (gwf.Retrieve(GenerWorkFlowAttr.WorkID, this.WorkID) == 0)
            //{
            //    this.Alert("工作还没有发出，您不能移交。");
            //    return;
            //}

            string msg = "";
            BP.WF.Node nd = new BP.WF.Node(gwf.FK_Node);
            if (nd.FocusField != "")
            {
                Work wk = nd.HisWork;
                wk.OID = this.WorkID;
                wk.Retrieve();
                msg = BP.WF.Glo.DealExp(nd.FocusField, wk, null);
                // wk.Update(nd.FocusField, msg);
            }
            string url = "./WorkOpt/Forward.aspx?FK_Node=" + this.FK_Node + "&WorkID=" + this.WorkID + "&FID=" + this.FID + "&FK_Flow=" + this.FK_Flow + "&Info=" + msg;
            this.Response.Redirect(url, true);
        }
        #endregion

        #endregion

    }
}