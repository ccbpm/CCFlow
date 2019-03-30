using System;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;

namespace BP.UnitTesting
{
    public  class Send011ParentFlow : TestBase
    {
        /// <summary>
        /// 父子流程调用
        /// </summary>
        public Send011ParentFlow()
        {
            this.Title = "父子流程调用";
            this.DescIt = "流程的正常运转与表单字段的控制的方向条件.";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 说明 ：此测试针对于演示环境中的 001 流程编写的单元测试代码。
        /// 涉及到了: 创建，发送，撤销，方向条件、退回等功能。
        /// </summary>
        public override void Do()
        {
            // guoxiangbin 登录.
            BP.WF.Dev2Interface.Port_Login("guoxiangbin");

            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork("011");

            //执行发送，发送到 “实施项目(调用多流程节点)”.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork("011", workid);

            //让实施人员 fuhui 登录.
            BP.WF.Dev2Interface.Port_Login("fuhui");

            // 让其发起子流程, 首先创建子流程.
            Int64 workidSubFlow = BP.WF.Dev2Interface.Node_CreateBlankWork("012");

            //设置父子流程关系.
            BP.WF.Dev2Interface.SetParentInfo("012", workidSubFlow,  workid );

            //让 fuhui 向下发送.
            BP.WF.Dev2Interface.Node_SendWork("012", workidSubFlow,0,null);

             
        }
    }
}
