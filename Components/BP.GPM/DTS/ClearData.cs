using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;

namespace BP.GPM.DTS
{
    /// <summary>
    /// Method 的摘要说明
    /// </summary>
    public class ClearData : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public ClearData()
        {
            this.Title = "清除所有维护的数据";
            this.Help = "系统类别，系统，菜单，用户组，用户权限。";
            this.Icon = "<img src='/WF/Img/Btn/Delete.gif'  border=0 />";
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
            BP.DA.DBAccess.RunSQL("DELETE FROM GPM_AppSort");
            BP.DA.DBAccess.RunSQL("DELETE FROM GPM_App");
            BP.DA.DBAccess.RunSQL("DELETE FROM GPM_Menu");
            BP.DA.DBAccess.RunSQL("DELETE FROM GPM_Group");
            return "清理成功.";
        }
    }
}
