using BP.En;
using System.Data;

namespace BP.WF.DTS
{
    /// <summary>
    /// 重新生成关键字
    /// </summary>
    public class GenerSKeyWords : Method
    {
        /// <summary>
        /// 重新生成关键字
        /// </summary>
        public GenerSKeyWords()
        {
            this.Title = "重新生成关键字SKeyWords（为所有的流程，根据新的规则生成流程关键字 SKeyWords）";
            this.Help = "您也可以打开流程属性一个个的单独执行。";
            this.GroupName = "流程维护";
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
                if (BP.Web.WebUser.No.Equals("admin") == true)
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
            string sql = "SELECT WorkID,FK_Flow FROM WF_GenerWorkFlow WHERE SKeyWords is null or SKeyWords='' ";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

            foreach (DataRow item in dt.Rows)
            {
                int workid = int.Parse(item[0].ToString());
                string flowNo = item[1].ToString();
                try {
                    GERpt rpt = new GERpt("ND" + int.Parse(flowNo) + "Rpt", workid);
               
                    GenerWorkFlow gwf = new GenerWorkFlow(workid);
                    WorkFlow wf = new WorkFlow(workid);
                    wf.GenerSKeyWords(gwf, rpt); //生成关键字.
                }
                catch (System.Exception ex)
                {
                    continue;
                }
            }

            return "执行成功...";
        }
    }
}
