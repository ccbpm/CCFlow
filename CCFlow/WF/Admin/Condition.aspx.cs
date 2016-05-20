using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;

namespace CCFlow.WF.Admin
{
    public partial class Condition1 : System.Web.UI.Page
    {
        #region 属性.
        public string CondType
        {
            get { return Request.QueryString["CondType"]; }
        }

        public string FK_Flow
        {
            get { return Request.QueryString["FK_Flow"]; }
        }

        public string FK_MainNode
        {
            get { return Request.QueryString["FK_MainNode"]; }
        }

        public string FK_Node
        {
            get { return Request.QueryString["FK_Node"]; }
        }

        public string FK_Attr
        {
            get { return Request.QueryString["FK_Attr"]; }
        }

        public string DirType
        {
            get { return Request.QueryString["DirType"]; }
        }

        public string ToNodeId
        {
            get { return Request.QueryString["ToNodeId"]; }
        }

        private Dictionary<ConnDataFrom, string> DataFrom = new Dictionary<ConnDataFrom, string>()
                                                                {
                                                                    {ConnDataFrom.Depts, "CondDept"},
                                                                    {ConnDataFrom.Form, "Cond"},
                                                                    {ConnDataFrom.Paras, "CondByPara"},
                                                                    {ConnDataFrom.SQL, "CondBySQL"},
                                                                    {ConnDataFrom.Stas, "CondStation"},
                                                                    {ConnDataFrom.Url, "CondByUrl"}
                                                                };

        /// <summary>
        /// 当前使用的条件类型
        /// </summary>
        public string CurrentCond { get; set; }
        #endregion 属性.


        protected void Page_Load(object sender, EventArgs e)
        {
            var cond = new Cond();
            if(cond.Retrieve(CondAttr.NodeID, this.FK_Node, CondAttr.ToNodeID, this.ToNodeId) != 0)
            {
                CurrentCond = DataFrom[cond.HisDataFrom];
            }
        }
    }
}