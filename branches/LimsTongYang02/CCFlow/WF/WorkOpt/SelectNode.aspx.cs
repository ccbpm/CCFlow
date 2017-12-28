using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.WF;

namespace CCFlow.WF.WorkOpt
{
    public partial class SelectNode : System.Web.UI.Page
    {
        #region 参数
        /// <summary>
        /// 工作人员编号
        /// </summary>
        public string WorkerId
        {
            get
            {
                return this.Request.QueryString["WorkerId"];
            }
        }
        /// <summary>
        /// 当前流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        /// <summary>
        /// 当前流程进行到的节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request.QueryString["FK_Node"]);
            }
        }
        /// <summary>
        /// 只显示选择节点步骤列表
        /// </summary>
        public bool ShowNodeOnly
        {
            get
            {
                return bool.Parse(Request.QueryString["ShowNodeOnly"] ?? "false");
            }
        }
        /// <summary>
        /// 只显示选择流程列表
        /// </summary>
        public bool ShowFlowOnly
        {
            get
            {
                return bool.Parse(Request.QueryString["ShowFlowOnly"] ?? "false");
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ShowFlowOnly)
            {
                if (string.IsNullOrWhiteSpace(FK_Flow))
                    throw new Exception("FK_Flow参数不能为空，且必须是有效的流程编号");

                var sql = string.Format("SELECT NodeID, Name,Step FROM WF_Node WHERE FK_Flow='{0}'", FK_Flow);
                var dtNodes = DBAccess.RunSQLReturnTable(sql);

                foreach (DataRow dr in dtNodes.Rows)
                {
                    if (FK_Node == Convert.ToInt32(dr["NodeID"])) continue;

                    lbNodes.Items.Add(
                        new ListItem(
                            string.Format("{0}. {1}", dr["Step"], dr["Name"]),
                            string.Format("{0},{1},{2}", dr["Step"], dr["NodeID"], dr["Name"])));
                }

                //增加“临时节点”，NodeID=0，added by liuxc,2015-10-13
                //lbNodes.Items.Add(new ListItem("临时节点", "0"));
            }

            if (!ShowNodeOnly)
            {
                var sql = "SELECT wfs.No,wfs.Name,wfs.ParentNo FROM WF_FlowSort wfs";
                var dtFlowSorts = DBAccess.RunSQLReturnTable(sql);

                DataTable dtFlows = null;

                sql = "SELECT wf.No,wf.[Name],wf.FK_FlowSort FROM WF_Flow wf";

                if (!string.IsNullOrWhiteSpace(WorkerId) && WorkerId.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Length == 1)
                    dtFlows = Dev2Interface.DB_GenerCanStartFlowsOfDataTable(WorkerId);
                else
                    dtFlows = DBAccess.RunSQLReturnTable(sql);

                //定义组织列表的数据集合
                //元组定义：是否是流程、树级别[0开始]、文本、值
                var items = new List<Tuple<bool, int, string, string>>();

                GenerateItems(items, dtFlowSorts.Select("ParentNo=0")[0], 0, dtFlows, dtFlowSorts);

                foreach (var item in items)
                {
                    lbFlows.Items.Add(new ListItem(item.Item3, item.Item4));
                }

                //生成的列表样式如下：
                //┌流程树
                //├─线性流程
                //├┄┄001.财务报销演示
            }
        }

        /// <summary>
        /// 生成流程树列表
        /// </summary>
        /// <param name="items">保存的树列表数据集合</param>
        /// <param name="drFlowSort">当前树类别</param>
        /// <param name="level">当前类别级别</param>
        /// <param name="dtFlows">流程集合</param>
        /// <param name="dtFlowSorts">类别集合</param>
        private void GenerateItems(List<Tuple<bool, int, string, string>> items, DataRow drFlowSort, int level, DataTable dtFlows, DataTable dtFlowSorts)
        {
            var prefix = (level == 0 ? "┏" : "┣");
            items.Add(
                Tuple.Create(
                    false,
                    level,
                    prefix.PadRight(level + 1, '━') + drFlowSort["Name"],
                    drFlowSort["No"].ToString()));

            var drSorts = dtFlowSorts.Select(string.Format("ParentNo='{0}'", drFlowSort["No"]));

            foreach (var drSort in drSorts)
                GenerateItems(items, drSort, level + 1, dtFlows, dtFlowSorts);

            var drFlows = dtFlows.Select(string.Format("FK_FlowSort='{0}'", drFlowSort["No"]), "No ASC");

            foreach (var drFlow in drFlows)
            {
                //排除指定的流程
                if (FK_Flow == drFlow["No"].ToString()) continue;

                items.Add(
                    Tuple.Create(
                        true,
                        level,
                        string.Format("{0}{1}{2}", prefix, string.Empty.PadLeft(level + 1, '┅'),
                                      drFlow["No"] + "." + drFlow["Name"]),
                        string.Format("{0},{1}", drFlow["No"], drFlow["Name"])));
            }
        }
    }
}