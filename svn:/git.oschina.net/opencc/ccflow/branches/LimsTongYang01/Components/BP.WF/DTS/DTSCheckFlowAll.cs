using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
namespace BP.WF.DTS
{
    /// <summary>
    /// 修复表单物理表字段长度 的摘要说明
    /// </summary>
    public class DTSCheckFlowAll : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public DTSCheckFlowAll()
        {
            this.Title = "体检全部流程";
            this.Help = "只能功能与单独体检流程相同，体检流程不会伤害数据。";
            this.Help += "<br>1，修复节点表单、流程报表物理表。";
            this.Help += "<br>2，生成预先流程与节点计算数据，从而优化流程执行速度。";
            this.Help += "<br>3，修复流程报表数据。";
            this.Help += "<br>4，系统不会提示体检结果。";
            this.Help += "<br>5，体检的时间长度与流程数量，节点数量，表单字段多少有关系，请耐心等待。";
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
            Flows fls = new Flows();
            fls.RetrieveAllFromDBSource();
            foreach (Flow fl in fls)
            {
                fl.DoCheck();
            }

            return "提示："+fls.Count+"个流程参与了体检。";
        }
    }
}
