using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 我发起的流程
    /// </summary>
    public class BarOfStartlist : BarBase
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
                return "我发起的流程";
            }
        }
        /// <summary>
        /// 权限控制-是否可以查看
        /// </summary>
        override public bool IsCanView
        {
            get
            {
                if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MSSQL)
                    return true;
                return false;
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
                return "我发起的流程";
            }
        }
        /// <summary>
        /// 更多连接
        /// </summary>
        override public string More
        {
            get
            {
                return "<a href='/WF/Start.htm' target=_self >我要发起流程</a>";
            }
        }
        /// <summary>
        /// 内容信息
        /// </summary>
        override public string Documents
        {
            get
            {
                Paras ps = new Paras();
                if (BP.Sys.SystemConfig.AppCenterDBType == DBType.MSSQL)
                    ps.SQL = "SELECT top 17 Title,RDT,FK_Flow,WorkID,FK_Node,Sender FROM WF_GenerWorkFlow WHERE Starter=" + ps.DBStr + "FK_Emp ORDER BY WorkID ";

                ps.AddFK_Emp();

                DataTable dt = DBAccess.RunSQLReturnTable(ps);

                string html = "<ul>";
                int idx = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    string fk_flow = dr["FK_Flow"].ToString();
                    string workID = dr["WorkID"].ToString();
                    string nodeID = dr["FK_Node"].ToString();
                    string title = dr["Title"].ToString();
                    string sender = dr["Sender"].ToString();
                    string rdt = dr["RDT"].ToString();
                    idx++;
                    html += "<li style='list-style-type:none'>" + idx + ".<a href='../WF/WFRpt.htm?FK_Flow=" + fk_flow + "&WorkID=" + workID + "&FK_Node=" + nodeID + "&1=2' target=_blank >" + title + "</a></li>";
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
                return "300";
            }
        }
        /// <summary>
        /// 高度
        /// </summary>
        override public string Height
        {
            get
            {
                return "200";
            }
        }
        /// <summary>
        /// 是否整行显示
        /// </summary>
        public override bool IsLine
        {
            get
            {
                return true;
            }
        }
        #endregion 外观行为.
    }
}
