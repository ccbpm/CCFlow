using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
namespace BP.WF.DTS
{
    /// <summary>
    /// 修复表单物理表字段长度 的摘要说明
    /// </summary>
    public class ReLoadNDxxxxxxRpt : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public ReLoadNDxxxxxxRpt()
        {
            this.Title = "清除并重新装载流程报表";
            this.Help = "删除NDxxxRpt表数据，重新装载，此功能估计要执行很长时间，如果数据量较大有可能在web程序上执行失败。";
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
            string msg = "";

            Flows fls = new Flows();
            fls.RetrieveAllFromDBSource();
            foreach (Flow fl in fls)
            {
                try
                {
                    msg += fl.DoReloadRptData();
                }
                catch(Exception ex)
                {
                    msg += "@在处理流程(" + fl.Name + ")出现异常" + ex.Message;
                }
            }
            return "提示："+fls.Count+"个流程参与了体检，信息如下：@"+msg;
        }
    }
}
