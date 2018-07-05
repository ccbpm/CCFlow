using System;
using System.Data;
using System.Collections;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.DA;
using BP.En;
using BP.Sys;
namespace BP.WF.DTS
{
    /// <summary>
    /// 创建索引
    /// </summary>
    public class CreateIndex : Method
    {
        /// <summary>
        /// 创建索引
        /// </summary>
        public CreateIndex()
        {
            this.Title = "创建索引（为所有的流程,NDxxxTrack, NDxxRpt, 创建索引.）";
            this.Help = "创建索引字段,调高流程的运行效率.";
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
            string info = "开始为Track表创建索引.";

            Flows fls = new Flows();
            foreach (Flow fl in fls)
            {
                info += fl.CreateIndex();
            }
            return info;

        }
    }
}
