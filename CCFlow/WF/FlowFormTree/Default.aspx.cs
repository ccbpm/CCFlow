using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CCFlow.WF.Comm.UC;
using BP.WF.Template;
using BP.WF;
using BP.Sys;
using BP.Port;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF.SheetTree
{
    public partial class Default : System.Web.UI.Page
    {
        public BP.WF.Node currND = null;

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
                    s = this.Request.QueryString["PFlowNo"];
                return s;
            }
        }
        public string FromNode
        {
            get
            {
                return this.Request.QueryString["FromNode"];
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
                        _FK_Node = DBAccess.RunSQLReturnValInt(sql,0);
                        if (_FK_Node == 0)
                            _FK_Node= int.Parse(this.FK_Flow + "01");
                    }
                    else
                    {
                        _FK_Node = int.Parse(this.FK_Flow + "01");
                    }
                }
                return _FK_Node;
            }
        }
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
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            this.currND = new BP.WF.Node(this.FK_Node);
            this.Page.Title = "第" + this.currND.Step + "步:" + this.currND.Name;
            if (IsPostBack==false)
                InitToolsBar();
        }
        /// <summary>
        /// 初始化工具栏
        /// </summary>
        private void InitToolsBar()
        {
            string toolsDefault = "";
            string extMenuHTML = "";
            int toolCount = 0;
            int alowToolCount = 8;
            BtnLab btnLab = new BtnLab(this.FK_Node);
            this.mm3.Visible = false;
            //发送
            if (btnLab.SendEnable && currND.HisBatchRole != BatchRole.Group)
            {
                toolsDefault += "<a id=\"send\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-send'\" onclick=\"EventFactory('send')\">" + btnLab.SendLab + "</a>";
                toolCount++;
            }
            //保存
            if (btnLab.SaveEnable)
            {
                toolsDefault += "<a id=\"save\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-save'\" onclick=\"EventFactory('save')\">" + btnLab.SaveLab + "</a>";
                toolCount++;
            }
            //子线程
            if (btnLab.ThreadEnable)
            {
                toolsDefault += "<a id=\"childline\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-childline'\" onclick=\"EventFactory('childline')\">" + btnLab.ThreadLab + "</a>";
                toolCount++;
            }
            //跳转
            if (btnLab.JumpWayEnable)
            {
                toolsDefault += "<a id=\"jumpNode\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-redo'\"  onclick=\"EventFactory('jumpway')\">" + btnLab.JumpWayLab + "</a>";
                toolCount++;
            }
            //退回
            if (btnLab.ReturnEnable)
            {
                toolsDefault += "<a id=\"turnBack\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-back'\" onclick=\"EventFactory('backcase')\">" + btnLab.ReturnLab + "</a>";
                toolCount++;
            }
            //抄送
            if (btnLab.CCRole != 0)
            {
                toolsDefault += "<a id=\"A1\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-ccsmall'\" onclick=\"EventFactory('CC')\">" + btnLab.CCLab + "</a>";
                toolCount++;
            }
            //移交
            if (btnLab.ShiftEnable)
            {
                toolsDefault += "<a id=\"ShiftLab\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-transfer'\" onclick=\"EventFactory('Shift')\">" + btnLab.ShiftLab + "</a>";
                toolCount++;
            }
            //删除 
            if (btnLab.DeleteEnable != 0)
            {
                toolsDefault += "<a id=\"DeleteLab\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-delete'\"  onclick=\"EventFactory('Del')\">" + "删除" + "</a>";
                toolCount++;
            }
            //结束 
            if (btnLab.EndFlowEnable)
            {
                //超出范围增加到菜单里面
                if (toolCount > alowToolCount)
                {
                    extMenuHTML += "<div data-options=\"plain:true,iconCls:'icon-no'\" onclick=\"EventFactory('endflow')\">" + btnLab.EndFlowLab + "</div>";
                }
                else
                {
                    toolsDefault += "<a id=\"EndFlow\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-no'\" onclick=\"EventFactory('endflow')\">" + btnLab.EndFlowLab + "</a>";
                }
                toolCount++;
            }
            //打印 
            if (btnLab.PrintDocEnable)
            {
                //超出范围增加到菜单里面
                if (toolCount > alowToolCount)
                {
                    extMenuHTML += "<div data-options=\"plain:true,iconCls:'icon-print'\" onclick=\"EventFactory('printdoc')\">" + btnLab.PrintDocLab + "</div>";
                }
                else
                {
                    toolsDefault += "<a id=\"PrintDoc\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-print'\" onclick=\"EventFactory('printdoc')\">" + btnLab.PrintDocLab + "</a>";
                }
                toolCount++;
            }
            //轨迹
            if (btnLab.TrackEnable)
            {
                //超出范围增加到菜单里面
                if (toolCount > alowToolCount)
                {
                    extMenuHTML += "<div data-options=\"plain:true,iconCls:'icon-flowmap'\" onclick=\"EventFactory('showchart')\">" + btnLab.TrackLab + "</div>";
                }
                else
                {
                    toolsDefault += "<a id=\"Track\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-flowmap'\" onclick=\"EventFactory('showchart')\">" + btnLab.TrackLab + "</a>";
                }
                toolCount++;
            }
            //挂起
            if (btnLab.HungEnable)
            {
                //超出范围增加到菜单里面
                if (toolCount > alowToolCount)
                {
                    extMenuHTML += "<div data-options=\"plain:true,iconCls:'icon-hungup'\" onclick=\"EventFactory('hungup')\">" + btnLab.HungLab + "</div>";
                }
                else
                {
                    toolsDefault += "<a id=\"Hung\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-hungup'\" onclick=\"EventFactory('hungup')\">" + btnLab.HungLab + "</a>";
                }
                toolCount++;
            }
            //接收人
            if (btnLab.SelectAccepterEnable == 1)
            {
                //超出范围增加到菜单里面
                if (toolCount > alowToolCount)
                {
                    extMenuHTML += "<div data-options=\"plain:true,iconCls:'icon-person'\" onclick=\"EventFactory('selectaccepter')\">" + btnLab.SelectAccepterLab + "</div>";
                }
                else
                {
                    toolsDefault += "<a id=\"SelectAccepter\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-person'\" onclick=\"EventFactory('selectaccepter')\">" + btnLab.SelectAccepterLab + "</a>";
                }
                toolCount++;
            }
            //查询
            if (btnLab.SearchEnable)
            {
                //超出范围增加到菜单里面
                if (toolCount > alowToolCount)
                {
                    extMenuHTML += "<div data-options=\"plain:true,iconCls:'icon-search'\" onclick=\"EventFactory('search')\">" + btnLab.SearchLab + "</div>";
                }
                else
                {
                    toolsDefault += "<a id=\"Search\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-search'\" onclick=\"EventFactory('search')\">" + btnLab.SearchLab + "</a>";
                }
                toolCount++;
            }
            //审核
            if (btnLab.WorkCheckEnable)
            {
                //超出范围增加到菜单里面
                if (toolCount > alowToolCount)
                {
                    extMenuHTML += "<div data-options=\"plain:true,iconCls:'icon-note'\" onclick=\"EventFactory('workcheck')\">" + btnLab.WorkCheckLab + "</div>";
                }
                else
                {
                    toolsDefault += "<a id=\"WorkCheck\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-note'\" onclick=\"EventFactory('workcheck')\">" + btnLab.WorkCheckLab + "</a>";
                }
                toolCount++;
            }
            //批量审核
            if (btnLab.BatchEnable)
            {
                //超出范围增加到菜单里面
                if (toolCount > alowToolCount)
                {
                    extMenuHTML += "<div data-options=\"plain:true,iconCls:'icon-note'\" onclick=\"EventFactory('batchworkcheck')\">" + btnLab.BatchLab + "</div>";
                }
                else
                {
                    toolsDefault += "<a id=\"BatchWorkCheck\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-note'\" onclick=\"EventFactory('batchworkcheck')\">" + btnLab.BatchLab + "</a>";
                }
                toolCount++;
            }
            //加签
            if (btnLab.AskforEnable)
            {
                //超出范围增加到菜单里面
                if (toolCount > alowToolCount)
                {
                    extMenuHTML += "<div data-options=\"plain:true,iconCls:'icon-tag'\" onclick=\"EventFactory('askfor')\">" + btnLab.AskforLab + "</div>";
                }
                else
                {
                    toolsDefault += "<a id=\"Askfor\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-tag'\" onclick=\"EventFactory('askfor')\">" + btnLab.AskforLab + "</a>";
                }
                toolCount++;
            }
            //超出范围增加到菜单里面
            if (toolCount > alowToolCount)
            {
                //extMenuHTML += "<div data-options=\"plain:true,iconCls:'icon-tag'\" onclick=\"addTab('ycfj','已传附件','/app/function/office/iofficefj.aspx?WorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "');\">已传附件</div>";
                //项目备案数据
                //extMenuHTML += "<div data-options=\"plain:true,iconCls:'icon-tag'\" onclick=\"addTab('basj','备案数据','/DataBak.aspx?WorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "');\">备案数据</div>";
            }
            else
            {
                //toolsDefault += "<a id=\"ycfj\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-tag'\" onclick=\"addTab('ycfj','已传附件','/app/function/office/iofficefj.aspx?WorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "');\">已传附件</a>";
                //一键签名
                //toolsDefault += "<a id=\"ycfj\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-note'\" onclick=\"EventFactory('Sign')\">一键签名</a>";
                //备案数据
                //toolsDefault += "<a id=\"DataBak\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-tag'\" onclick=\"addTab('basj','备案数据','/DataBak.aspx?WorkID=" + this.WorkID + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "');\">备案数据</a>";
            }
            //扩展工具，显示位置为工具栏类型
            NodeToolbars extToolBars = new NodeToolbars();
            QueryObject info = new QueryObject(extToolBars);
            info.AddWhere(NodeToolbarAttr.FK_Node, this.FK_Node);
            info.addAnd();
            info.AddWhere(NodeToolbarAttr.ShowWhere, (int)ShowWhere.Toolbar);
            info.DoQuery();

            foreach (NodeToolbar item in extToolBars)
            {
                string url = "";
                if (string.IsNullOrEmpty(item.Url))
                    continue;

                string urlExt = this.RequestParas;
                //urlExt = "WorkID=" + this.WorkID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.FK_Node + "&UserNo=" + WebUser.No + "&FID=" + this.FID + "&SID=" + WebUser.SID + "&CWorkID=" + this.CWorkID;
                url = item.Url;

                if (url.Contains("?"))
                {
                    url += urlExt;
                }
                else
                {
                    url += "?" + urlExt;
                }
                //超出范围增加到菜单里面
                if (toolCount > alowToolCount)
                {
                    if (!string.IsNullOrEmpty(item.Target) && item.Target.ToLower() == "javascript")
                    {
                        extMenuHTML += "<div data-options=\"plain:true,iconCls:'icon-new'\" onclick=\"" + item.Url + "\">" + item.Title + "</div>";
                    }
                    else
                    {
                        extMenuHTML += "<div data-options=\"plain:true,iconCls:'icon-new'\" onclick=\"WinOpenPage('" + item.Target + "','" + url + "','" + item.Title + "')\">" + item.Title + "</div>";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.Target) && item.Target.ToLower() == "javascript")
                    {
                        toolsDefault += "<a href=\"#\" onclick=\"" + item.Url + "\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-new'\">" + item.Title + "</a>";
                    }
                    else
                    {
                        toolsDefault += "<a target=\"" + item.Target + "\" href=\"" + url + "\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-new'\">" + item.Title + "</a>";
                    }
                    toolCount++;
                }
            }

            //判断是否出现添加菜单
            if (toolCount > alowToolCount)
            {
                this.mm3.Visible = true;
                toolsDefault += "<a href=\"javascript:void(0)\" id=\"mb3\" class=\"easyui-menubutton\" data-options=\"menu:'#mm3',plain:true,iconCls:'icon-add'\"></a>";
            }
            toolsDefault += "<a id=\"closeWin\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-no'\" onclick=\"EventFactory('closeWin')\">关闭</a>";
            //添加内容
            this.toolBars.InnerHtml = toolsDefault;
            this.mm3.InnerHtml = extMenuHTML;
        }
        public string RequestParas
        {
            get
            {
                string urlExt = "";
                string rawUrl = this.Request.RawUrl;
                rawUrl = "&" + rawUrl.Substring(rawUrl.IndexOf('?') + 1);
                string[] paras = rawUrl.Split('&');
                foreach (string para in paras)
                {
                    if (para == null
                        || para == ""
                        || para.Contains("=") == false)
                        continue;
                    urlExt += "&" + para;
                }
                return urlExt;
            }
        }
    }
}