using System;
using System.Collections;
using System.Reflection;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Sys;
namespace BP.EAI.Plugins.DTS
{
    /// <summary>
    /// 钉钉组织结构增量同步
    /// </summary>
    public class OrgInit_DingIcreMent : Method
    {
        /// <summary>
        /// 钉钉组织结构增量同步
        /// </summary>
        public OrgInit_DingIcreMent()
        {
            this.Title = "同步增量钉钉通讯录到CCGPM";
            this.Help = "增量同步钉钉通讯录,需要时间比较长，请耐心等待。<br> 钉钉相关配置写入Web.config，配置正确才可以被执行";
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
                if (BP.GPM.Glo.IsEnable_DingDing == true)
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
            GPM.Emp emp = new GPM.Emp();
            emp.CheckPhysicsTable();

            DingDing ding = new DingDing();
            string result = ding.AnsyIncrementOrgToGPM();
            if (DataType.IsNullOrEmpty(result))
            {
                return "执行成功,没有发生变化...";
            }
            else if (result.Contains("钉钉获取部门出错"))
            {
                return result;
            }
            else if (result.Length > 0)
            {
                string webPath = "Log/Ding_GPM" + DateTime.Now.ToString("yyyy_MM_dd") + ".log";
                string savePath = BP.Sys.SystemConfig.PathOfDataUser + webPath;

                BP.DA.Log log = new Log(savePath);
                log.WriteLine(result);
                return "执行成功<a href=\"/DataUser/" + webPath + "\" target='_blank'>下载日志</a>";
            }
            else
                return "执行失败...";
        }
    }
}
