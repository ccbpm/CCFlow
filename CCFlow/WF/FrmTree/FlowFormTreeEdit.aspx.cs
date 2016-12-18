using System;
using System.Collections.Generic;
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

namespace CCFlow.WF.FrmTree
{
    /// <summary>
    /// 关于:/WF/FrmTree/FlowFormTreeEdit.aspx 功能界面使用说明：
    /// 此页面是功能调用页面，它需要两个参数. FK_Flow,WorkID.
    /// 是用来解决独立表单树的查看与编辑，他对应的是流程编号而非节点编号.
    /// 主要应用到如下场景:
    /// 1, 需要查看流程信息，而非指定特定的节点。
    /// 2，需要查看流程信息，并且可以编辑表单信息，应用于数据采集，即时该流程已经走完。
    /// 3, 在该功能工具栏上，仅具有， 保存，轨迹，关闭三个按钮。
    /// 
    /// 相关：
    /// 1, 正常流程节点表单树的信息绑定，实在节点属性，节点表单中设置。
    /// 2, 节点绑定的表单在节点属性设置. 流程绑定的表单在流程属性里设置.
    /// 3, 节点绑定独立表单的功能是页面是 FlowFormTreeView.aspx 需要的参数是 FK_Flow,WorkID,FK_Node,FID
    /// </summary>
    public partial class FlowFormTreeEdit : System.Web.UI.Page
    {
        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                InitToolsBar();
        }
        /// <summary>
        /// 初始化工具栏
        /// </summary>
        private void InitToolsBar()
        {
            string toolsDefault = "";
            //保存
            toolsDefault += "<a id=\"save\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-save'\" onclick=\"EventFactory('save')\">保存</a>";
            
            //轨迹
            toolsDefault += "<a id=\"Track\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-flowmap'\" onclick=\"EventFactory('showchart')\">轨迹</a>";
            ////查询
            //toolsDefault += "<a id=\"Search\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-search'\" onclick=\"EventFactory('search')\">查询</a>";
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
                url = item.Url;
                if (url.Contains("?"))
                {
                    url += urlExt;
                }
                else
                {
                    url += "?" + urlExt;
                }
                toolsDefault += "<a target=\"" + item.Target + "\" href=\"" + url + "\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-new'\">" + item.Title + "</a>";
            }
            //关闭
            toolsDefault += "<a id=\"closeWin\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-no'\" onclick=\"EventFactory('closeWin')\">关闭</a>";
            //添加内容
            this.toolBars.InnerHtml = toolsDefault;
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