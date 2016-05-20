using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CCFlow.WF.Comm.UC;
using BP.WF;
using BP.Sys;
using BP.Port;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;
using BP.WF.Template;

namespace CCFlow.WF.SheetTree
{
    public partial class FlowFormTreeView : System.Web.UI.Page
    {
        #region 属性
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
                        _FK_Node = DBAccess.RunSQLReturnValInt(sql, 0);
                        if (_FK_Node == 0)
                            _FK_Node = int.Parse(this.FK_Flow + "01");
                    }
                    else
                    {
                        _FK_Node = int.Parse(this.FK_Flow + "01");
                    }
                }
                return _FK_Node;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            BtnLab btnLab = new BtnLab(this.FK_Node);
            string toolsDefault = "";

            //打印 
            if (btnLab.PrintDocEnable)
            {
                toolsDefault += "<a id=\"PrintDoc\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-print'\" onclick=\"EventFactory('printdoc')\">" + btnLab.PrintDocLab + "</a>";
            }
            toolsDefault += "<a id=\"closeWin\" href=\"#\" class=\"easyui-linkbutton\" data-options=\"plain:true,iconCls:'icon-no'\" onclick=\"EventFactory('closeWin')\">关闭</a>";
            //添加内容
            this.toolBars.InnerHtml = toolsDefault;
        }
    }
}