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
    /// <summary>
    /// 测试名称
    /// </summary>
    public class TestCaseTemplete : TestBase
    {
        /// <summary>
        /// 测试名称
        /// </summary>
        public TestCaseTemplete()
        {
            this.Title = "测试名称";
            this.DescIt = "流程: 005月销售总结(同表单分合流),执行发送后的数据是否符合预期要求.";
            this.EditState = EditState.UnOK;
        }

        #region 全局变量
        /// <summary>
        /// 流程编号
        /// </summary>
        public string fk_flow = "";
        /// <summary>
        /// 用户编号
        /// </summary>
        public string userNo = "";
        /// <summary>
        /// 所有的流程
        /// </summary>
        public Flow fl = null;
        /// <summary>
        /// 主线程ID
        /// </summary>
        public Int64 workID = 0;
        /// <summary>
        /// 发送后返回对象
        /// </summary>
        public SendReturnObjs objs = null;
        /// <summary>
        /// 工作人员列表
        /// </summary>
        public GenerWorkerList gwl = null;
        /// <summary>
        /// 流程注册表
        /// </summary>
        public GenerWorkFlow gwf = null;
        #endregion 变量

        /// <summary>
        /// 测试案例说明:
        /// 1, .
        /// 2, .
        /// 3，.
        /// </summary>
        public override void Do()
        {
            //初始化变量.
            fk_flow = "023";
            userNo = "zhanghaicheng";
            fl = new Flow(fk_flow);
        }
    }
}
