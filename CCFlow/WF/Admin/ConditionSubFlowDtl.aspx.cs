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
    public partial class ConditionSubFlowDtl : System.Web.UI.Page
    {
        public string CondType
        {
            get { return "3"; }
        }

        public string FK_Flow
        {
            get { return Request.QueryString["FK_Flow"]; }
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
            get { return "SubFlow"; }
        }

        public string ToFlow
        {
            get { return Request.QueryString["ToFlow"]; }
        }

        private Dictionary<ConnDataFrom, string> DataFrom = new Dictionary<ConnDataFrom, string>()
                                                                {
                                                                    {ConnDataFrom.Depts, "CondDept"},
                                                                    {ConnDataFrom.Form, "CondByFrm"},
                                                                    {ConnDataFrom.Paras, "CondByPara"},
                                                                    {ConnDataFrom.SQL, "CondBySQL"},
                                                                    {ConnDataFrom.Stas, "CondStation"},
                                                                    {ConnDataFrom.Url, "CondByUrl"}
                                                                };

        /// <summary>
        /// 当前使用的条件类型
        /// </summary>
        public string CurrentCond { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var cond = new Cond();
            if (cond.Retrieve(CondAttr.NodeID, this.FK_Node, CondAttr.ToNodeID, int.Parse( this.ToFlow)) != 0)
            {
                CurrentCond = DataFrom[cond.HisDataFrom];
            }
        }
    }
}