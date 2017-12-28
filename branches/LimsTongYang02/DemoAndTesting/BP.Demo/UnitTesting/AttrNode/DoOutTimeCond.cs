using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.WF;
using BP.WF.Data;
using BP.WF.Template;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.UnitTesting;

namespace BP.UnitTesting.NodeAttr
{
    public  class DoOutTimeCond : TestBase
    {
        /// <summary>
        /// 测试财务报销流程-超时审批功能
        /// </summary>
        public DoOutTimeCond()
        {
            this.Title = "节点考核属性-节点超时处理";
            this.DescIt = "以流程:032(节点属性-超时处理)测试用例,测试多种类型下的超时处理逻辑是否正确.";
            this.EditState = EditState.UnOK;
        }
        /// <summary>
        /// 说明 ：此测试针对于演示环境中的 001 流程编写的单元测试代码。
        /// 涉及到了: 创建，发送，撤销，方向条件、退回等功能。
        /// </summary>
        public override void Do()
        {
            throw new Exception("@此功能尚未完成");

            string fk_flow = "032";
            string userNo = "zhanghaicheng";
            
            Flow fl = new Flow(fk_flow);
            // zhoutianjiao 登录.
            BP.WF.Dev2Interface.Port_Login(userNo);
            //创建空白工作.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //执行发送,并且字段参数为不处理.
            Hashtable ht = new Hashtable();
            ht.Add("OutTimeDealDir", (int)OutTimeDeal.None);
            BP.WF.Dev2Interface.Node_SendWork(fl.No, workid, ht);


        }
    }
}
