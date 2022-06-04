using System.Data;
using BP.Port;
using BP.En;

namespace BP.WF.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class GenerAllEmpsStartFlows : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public GenerAllEmpsStartFlows()
        {
            this.Title = "为每个人重置发起流程列表";
            this.Help = "一个操作员能发起那些流程是自动计算出来的，计算的成本有些高.";
            this.Help += "为了解决该问题，执行该方法，可以提高效率.";
            this.GroupName = "流程自动执行定时任务";
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
                return true;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {

            BP.Port.Emps ens = new Emps();
            ens.RetrieveAll(99999);

            foreach (BP.Port.Emp en in ens)
            {
                BP.WF.Dev2Interface.Port_Login(en.No);
                DataTable dt = BP.WF.Dev2Interface.DB_GenerCanStartFlowsOfDataTable(en.No);
            }

            return "调度完成..";
        }

    }
}
