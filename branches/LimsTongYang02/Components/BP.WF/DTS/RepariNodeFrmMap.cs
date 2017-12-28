using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.Sys;
using BP.En;
namespace BP.WF.DTS
{
    /// <summary>
    /// 修复节点表单map 的摘要说明
    /// </summary>
    public class RepariNodeFrmMap : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public RepariNodeFrmMap()
        {
            this.Title = "修复节点表单";
            this.Help = "检查节点表单系统字段是否被非法删除，如果非法删除自动增加上它，这些字段包括:Rec,Title,OID,FID,WFState,RDT,CDT";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "您确定要执行吗？";
            //HisAttrs.AddTBString("P1", null, "原密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "新密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P3", null, "确认", true, false, 0, 10, 10);
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            Nodes nds = new Nodes();
            nds.RetrieveAllFromDBSource();


            string info = "";
            foreach (Node nd in nds)
            {
                string msg = nd.RepareMap(nd.HisFlow);
                if (msg != "")
                    info += "<b>对流程" + nd.FlowName + ",节点(" + nd.NodeID + ")(" + nd.Name + "), 检查结果如下:</b>" + msg + "<hr>";
            }
            return info + "执行完成...";
        }
    }
}
