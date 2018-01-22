using System;
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
    /// 重新生成标题
    /// </summary>
    public class GenerTitle : Method
    {
        /// <summary>
        /// 重新生成标题
        /// </summary>
        public GenerTitle()
        {
            this.Title = "重新生成标题（为所有的流程，根据新的规则生成流程标题）";
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
            BP.WF.Template.FlowSheets ens = new BP.WF.Template.FlowSheets();
            foreach (BP.WF.Template.FlowSheet en in ens)
            {
                en.DoGenerTitle();
            }
            return "执行成功...";
        }
    }
}
