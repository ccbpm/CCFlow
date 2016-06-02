using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
namespace BP.WF.DTS
{
    /// <summary>
    /// 增加办事指南连接
    /// </summary>
    public class GenerFrmHelpLink : Method
    {
        /// <summary>
        /// 增加办事指南连接
        /// </summary>
        public GenerFrmHelpLink()
        {
            this.Title = "增加办事指南连接（为所有的流程）";
            this.Help = "您也可以打开流程属性一个个的单独执行，增加一个帮助连接。";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                if (BP.Web.WebUser.No == "admin")
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            BP.WF.Nodes nds = new Nodes();
            nds.RetrieveAll();
            foreach (BP.WF.Node nd in nds)
            {
                string sql = "SELECT TOP 1 Y1, X2 from  sys_frmline WHERE Y1=Y2 AND fk_mapdata='ND" + nd.NodeID + "'  ORDER BY Y1, FK_MapData";

                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                if (dt.Rows.Count == 0)
                    continue;
                float y = float.Parse(dt.Rows[0][0].ToString());
                float x = float.Parse(dt.Rows[0][1].ToString());

                FrmLink lk = new FrmLink();
                lk.MyPK = "ND" + nd.NodeID;
                lk.Delete();

                lk.X = x - 50;
                lk.Y = y - 20;
                lk.FK_MapData = "ND" + nd.NodeID;
                lk.Text = "办事指南";
                lk.Target = "_blank";
                lk.URL = "/App/GovService/FlowHelp.aspx?FK_Flow=" + nd.FK_Flow;
                lk.Insert();
            }
            return "执行成功...";
        }
    }
}
