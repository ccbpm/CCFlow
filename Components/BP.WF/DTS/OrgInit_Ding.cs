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
    /// 钉钉组织结构同步
    /// </summary>
    public class OrgInit_Ding : Method
    {
        /// <summary>
        /// 钉钉组织结构同步
        /// </summary>
        public OrgInit_Ding()
        {
            this.Title = "同步钉钉通讯录到CCGPM";
            this.Help = "本功能将首先<b style='color:red;'>清空组织结构</b>，然后同步钉钉通讯录。<br> 钉钉相关配置写入Web.config，配置正确才可以被执行";
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
                if( Glo.IsEnable_DingDing == true)
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
            DingDing ding = new DingDing();
            bool result = ding.AnsyOrgToCCGPM();
            if (result == true)
                return "执行成功...";
            else
                return "执行失败...";
        }
    }
}
