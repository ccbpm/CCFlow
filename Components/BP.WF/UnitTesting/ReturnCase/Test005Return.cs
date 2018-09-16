using System;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.UnitTesting;

namespace BP.UnitTesting.ReturnCase
{
    public class Test005Return : TestBase
    {
        /// <summary>
        /// 测试子线程向合流节点退回
        /// </summary>
        public Test005Return()
        {
            this.Title = "测试子线程向合流节点退回";
            this.DescIt = "1,退回到开始点，一步步的向下发送。2，退回到分流点，一步步的向下发送.";
            this.DescIt += "重复1,2,测试退回并原路返回。";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 执行.
        /// </summary>
        public override void Do()
        {
            #region 发送
            string fk_flow = "005";
            string startUser = "zhanghaicheng";
            BP.Port.Emp starterEmp = new Port.Emp(startUser);

            Flow fl = new Flow(fk_flow);

            //让 zhanghaicheng 登录, 在以后，就可以访问WebUser.No, WebUser.Name 。
            BP.WF.Dev2Interface.Port_Login(startUser);

            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);

            //执行向 分流点 发送, qifenglin接受.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);
            if (objs.VarAcceptersID != "zhangyifan,zhoushengyu,")
                throw new Exception("@接受人错误，应当是zhangyifan,zhoushengyu现在是:" + objs.VarAcceptersID);
            #endregion

            //获得workid.
            string[] workids = objs.VarTreadWorkIDs.Split(',');

            Int64 workID1 = Int64.Parse(workids[0]);
            Int64 workID2 = Int64.Parse(workids[1]);

            //让一个子线程登录.
            BP.WF.Dev2Interface.Port_Login("zhangyifan");

            //执行退回.
            string info = BP.WF.Dev2Interface.Node_ReturnWork("005", workID1, workid, 502, 501, startUser, false);

            #region 检查执行退回是否符合要求。
            GenerWorkFlow gwf = new GenerWorkFlow(workID1);
            if (gwf.WFState != WFState.ReturnSta)
                throw new Exception("@子线程的状态应该是退回,现在是:"+gwf.WFStateText);

            if (gwf.Starter!="zhanghaicheng")
                throw new Exception("@子线程的流程发起人应该是，zhanghaicheng,现在是:" + gwf.Starter);

            if (gwf.FK_Node != 501)
                throw new Exception("@子线程的当前节点应该是 501,现在是:" + gwf.FK_Node);

            //检查是否有待办。
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable("zhanghaicheng", "005");
            bool isHave = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[GenerWorkerListAttr.WorkID].ToString() != workID1.ToString())
                    continue;
                if (dr[GenerWorkerListAttr.FK_Node].ToString() != "501")
                    continue;

                isHave = true;
            }
            if (isHave == false)
                throw new Exception("@没有找到合流节点的待办...");
            #endregion 检查执行退回是否符合要求。

        }
    }
}
