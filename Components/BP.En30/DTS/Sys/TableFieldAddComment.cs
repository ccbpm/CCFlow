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
    /// 为表的字段增加中文注释
    /// </summary>
    public class TableFieldAddComment : Method
    {
        /// <summary>
        /// 为表的字段增加中文注释
        /// </summary>
        public TableFieldAddComment()
        {
            this.Title = "为表的字段增加中文注释";
            this.Help = "从map里面去找表的每个字段.";
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
            PubClass.AddComment();
            return "执行成功.";
        }
    }
}
