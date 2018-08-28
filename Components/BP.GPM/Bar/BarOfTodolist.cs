using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 流程待办
    /// </summary>
    public class BarOfTodolist :BarBase
    {
        #region 系统属性.
        /// <summary>
        /// 流程编号/流程标记.
        /// </summary>
        override public string No
        {
            get
            {
                return this.ToString();
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        override public string Name
        {
            get
            {
                return "我的待办";
            }
        }
        /// <summary>
        /// 权限控制-是否可以查看
        /// </summary>
        override public bool IsCanView
        {
            get
            {
                return true; //任何人都可以看到.
            }
        }
        #endregion 系统属性.

        #region 外观行为.
        /// <summary>
        /// 标题
        /// </summary>
        override public string Title
        {
            get
            {
                return "我的待办";
            }
        }
        /// <summary>
        /// 更多连接
        /// </summary>
        override public string More
        {
            get
            {
                return "<a href='/WF/Todolist.htm' target=_self >更多(" + BP.WF.Dev2Interface.Todolist_EmpWorks + ")</a>";
            }
        }
        /// <summary>
        /// 内容信息
        /// </summary>
        override public string Documents
        {
            get
            {

                string sql = "select A.WorkID, A.FK_Flow, A.FK_Node, A.Title , A.Sender, A.RDT from WF_GenerWorkFlow A , WF_GenerWorkerlist B WHERE A.WorkID=B.WorkID AND B.IsPass=0 AND B.FK_Emp='' ";

                DataTable dt = DBAccess.RunSQLReturnTable(sql);

                string html = "<ul>";
                foreach (DataRow dr in dt.Rows)
                {
                    string fk_flow = dr["FK_Flow"].ToString();
                    string workID = dr["WorkID"].ToString();
                    string nodeID = dr["FK_Node"].ToString();
                    string title = dr["Title"].ToString();
                    string sender = dr["Sender"].ToString();
                    string rdt = dr["RDT"].ToString();
                    html += "<li><a href='MyFlow.htm?FK_Flow="+fk_flow+"&WorkID="+workID+"&FK_Node="+nodeID+"&1=2'>"+title+"</a></li>";
                }
                html += "</ul>";
                return html;
            }
        }
        /// <summary>
        /// 宽度
        /// </summary>
        override public string Width
        {
            get
            {
                return "300px";
            }
        }
        /// <summary>
        /// 高度
        /// </summary>
        override public string Height
        {
            get
            {
                return "200px";
            }
        }
        #endregion 外观行为.
    }
}
