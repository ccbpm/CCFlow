using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.UnitTesting;

namespace BP.UnitTesting.AttrFlow
{
    /// <summary>
    /// 生成标题
    /// </summary>
    public  class GenerTitle : TestBase
    {
        #region 变量
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
        /// 生成标题
        /// </summary>
        public GenerTitle()
        {
            this.Title = "标题生成规则";
            this.DescIt = "流程:023最简单的3节点(轨迹模式).";
            this.EditState = EditState.Passed; 
        }
        /// <summary>
        /// 过程说明：
        /// 1，以流程 023最简单的3节点(轨迹模式)， 为测试用例。
        /// 2，仅仅测试发送功能，与检查发送后的数据是否完整.
        /// 3, 此测试有三个节点发起点、中间点、结束点，对应三个测试方法。
        /// </summary>
        public override void Do()
        {
            #region 定义变量.
            fk_flow = "023";
            userNo = "zhanghaicheng";
            fl = new Flow(fk_flow);
            #endregion 定义变量.

            //让 userNo 登录.
            BP.WF.Dev2Interface.Port_Login(userNo);
            //创建空白工作, 在标题为空的情况下.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            #region 检查标题是否符合预期.
            string title = DBAccess.RunSQLReturnString("SELECT Title FROM " + fl.PTable + " WHERE OID=" + workID);
            if (title != "TitleTest")
                throw new Exception("@没有按指定的数据(TitleTest)生成标题, 在流程报表里，现在是:"+title);
            #endregion 

            //执行发送.
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID);

            #region 检查标题是否符合预期.
            title = DBAccess.RunSQLReturnString("SELECT  Title FROM " + fl.PTable + " where OID=" + workID);
            if (title != "TitleTest")
                throw new Exception("@发送后:没有按指定的数据生成标题, 在流程报表里."+title);
            #endregion 

            //删除测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, workID, false);


            //创建空白工作, 让ccflow根据规则自动生成标题.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            #region 检查标题是否符合预期.
            //title = DBAccess.RunSQLReturnString("SELECT Title FROM WF_GenerWorkFlow where WorkID=" + workID);
            //if (DataType.IsNullOrEmpty(title))
            //    throw new Exception("@标题没有生成 在 WF_GenerWorkFlow中没有找到.");

            title = DBAccess.RunSQLReturnString("SELECT Title FROM " + fl.PTable + " where OID=" + workID);
            if (DataType.IsNullOrEmpty(title))
                throw new Exception("@标题没有生成， 在 PTable中没有找到.");
            #endregion

            //执行发送.
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workID);

            #region 检查标题是否符合预期.
            title = DBAccess.RunSQLReturnString("SELECT Title FROM WF_GenerWorkFlow where WorkID=" + workID);
            if (DataType.IsNullOrEmpty(title))
                throw new Exception("@标题没有生成 在 WF_GenerWorkFlow中没有找到.");

            title = DBAccess.RunSQLReturnString("SELECT Title FROM " + fl.PTable + " WHERE OID=" + workID);
            if (DataType.IsNullOrEmpty(title))
                throw new Exception("@标题没有生成， 在 PTable中没有找到.");
            #endregion 

            //删除测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(fl.No, workID, false);
        }
    }
}
