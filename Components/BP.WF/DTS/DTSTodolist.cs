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
    /// 修复数据库 的摘要说明
    /// </summary>
    public class DTSTodolist : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public DTSTodolist()
        {
            this.Title = "同步待办时间戳,状态,注册表的时间端.";
            this.Help = "把最新的版本的与当前的数据表结构，做一个自动修复, 修复内容：缺少列，缺少列注释，列注释不完整或者有变化。";
            this.Help += "<br>因为表单设计器的错误，丢失了字段，通过它也可以自动修复。";
            this.Help += "<br><a href='/'>进入流程设计器</a>";
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

            return "执行成功...";
        }
    }
}
