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
    /// Method 的摘要说明
    /// </summary>
    public class UpdateCCFlow : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public UpdateCCFlow()
        {
            this.Title = "升级ccflow";
            this.Help = "执行对ccflow升级，如果您更新下来了最新的代码，您就需要执行该功能，进行对ccflow的数据库升级。";
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
            if (BP.Web.WebUser.No != "admin")
                return "非法的用户执行。";

            BP.WF.Glo.UpdataCCFlowVer();
 
            return "执行成功,系统已经修复了最新版本的数据库.";
        }
    }
}
