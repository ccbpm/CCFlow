using System;
using System.Data;
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
    public class DeletTempFiles : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public DeletTempFiles()
        {
            this.Title = "删除产生的临时文件";
            this.Help = "上传、下载、导入导出流程模版表单模版的临时文件删除.";
            this.Icon = "<img src='/WF/Img/Btn/Delte.gif'  border=0 />";
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

            try
            {
                BP.WF.Glo.DeleteTempFiles();
            }
            catch (Exception ex)
            {
                BP.DA.Log.DebugWriteWarning("删除临时文件错误:" + ex.Message);
                return "err@"+ex.Message;
            }
            return "删除成功.";
        }
    }
}
