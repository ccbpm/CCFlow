using System.Data;
using BP.DA;
using BP.En;
using BP.WF.Template;
namespace BP.WF.DTS
{
    /// <summary>
    /// 升级ccflow6 要执行的调度
    /// </summary>
    public class CheckDirCondModel : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public CheckDirCondModel()
        {
            this.Title = "检查所有流程方向条件设置是否正确?";
            this.Help = "1.检查DirCondModel,配置的模式是否正确.";
            this.Help += "\t\n 2.检查条件中配置的SQL";
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
                if (BP.Web.WebUser.No.Equals("admin")==true)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string err = "";

            // 查询出来按照连接线.
            Nodes nds = new Nodes();
            nds.Retrieve(NodeAttr.CondModel, (int)DirCondModel.ByLineCond);

            string sql = "方向条件未配置----------------------";
            foreach (Node item in nds)
            {
                if (item.HisToNDNum <= 0)
                    continue;

                NodeSimples toNDs = item.HisToNodeSimples;
                var num = 0;
                foreach (NodeSimple nd in toNDs)
                {
                    sql = "SELECT * FROM WF_Cond WHERE CondType=2 and FK_Node=" + item.NodeID + " AND ToNodeID=" + nd.NodeID;
                    DataTable DT = DBAccess.RunSQLReturnTable(sql);
                    if (DT.Rows.Count == 0)
                        num++;
                }
                if (num > 1)
                    err += "<br>@流程["+item.FK_Flow+","+item.FlowName+"],节点["+item.NodeID+","+item.Name+"]方向条件设置错误,到达的节点有[" + toNDs.Count + "]个，没有设置连接线条件的有["+num+"]个";
            }


            return err;
        }
    }
}
