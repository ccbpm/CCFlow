using System;
using System.Data;
using System.Collections;
using BP.DA;
using System.Reflection;
using BP.Port;
using BP.En;
namespace BP.WF.DTS
{
    /// <summary>
    /// 修复表单物理表字段长度 的摘要说明
    /// </summary>
    public class DTSAutoRpt : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public DTSAutoRpt()
        {
            this.Title = "自动报表";
            this.Help = "放在定时任务里，读取WF_AutoRpt数据配置，想指定的人员推送数据。";
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
            //BP.WF.Template.AutoRpts rpts = new Template.AutoRpts();
            //rpts.RetrieveAll();

            //string html = "执行内容如下:";
            //foreach (BP.WF.Template.AutoRpt rpt in rpts)
            //{
            //    if (DataType.IsNullOrEmpty(rpt.StartDT) == false)
            //    {
            //        html += "<br>" + rpt.No + rpt.Name + "没有启用.";
            //        continue;
            //    }

            //    #region 判断是否可以启动?
            //    //要发起的时间点.
            //    string[] strs = rpt.StartDT.Split('@');
            //    string datetime = DateTime.Now.ToString("HH:mm");
            //    foreach (string str in strs)
            //    {
            //        if (DataType.IsNullOrEmpty(str)==true)
            //            continue;


            //    }
            //    #endregion 判断是否可以启动?

            //    #region 获得可以发送的人员集合.
            //    Hashtable htEmps = new Hashtable();

            //    foreach (var item in rpt.ToEmps.Split(','))
            //    {
            //    }
            //    #endregion 获得可以发送的人员集合.


            //}

            return null;
            //return html;
        }
    }
}
