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

namespace BP.UnitTesting.SendCase
{
    /// <summary>
    /// 发送多人测试
    /// </summary>
    public class SendCase : TestBase
    {
        /// <summary>
        /// 发送多人测试
        /// </summary>
        public SendCase()
        {
            this.Title = "发送多人测试";
            this.DescIt = "流程: 023 测试发送给多个人,执行发送后的数据是否符合预期要求.";
            this.EditState = EditState.Passed;
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
        public Int64 workid = 0;
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
        /// 发送测试
        /// </summary>
        public override void Do()
        {
            //初始化变量.
            fk_flow = "023";
            userNo = "zhanghaicheng";
            fl = new Flow(fk_flow);

            //执行登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            //执行第1步检查，创建工作与发送.
            this.workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow, null, null, userNo, null);

            //下一步骤的接受人员定义多个.
            string toEmps = "zhoushengyu,zhangyifan,";

            //向发送多个人.
            objs = BP.WF.Dev2Interface.Node_SendWork(this.fk_flow, this.workid, null, null, 0, toEmps);


            #region 检查发送变量是否正确?
            if (objs.VarAcceptersID != toEmps)
                throw new Exception("@应该是接受者ID多人，现在是:" + objs.VarAcceptersID);

            if (objs.VarAcceptersName != "周升雨,张一帆,")
                throw new Exception("@应该是接受者Name多人，现在是:" + objs.VarAcceptersID);

            if (objs.VarCurrNodeID != 2301)
                throw new Exception("@当前节点应该是 2301 ，现在是:" + objs.VarCurrNodeID);

            if (objs.VarToNodeID != 2302)
                throw new Exception("@到达节点应该是 2302 ，现在是:" + objs.VarToNodeID);
            #endregion 检查发送变量是否正确?

            #region 检查流程引擎表是否正确?
            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = this.workid;
            if (gwf.RetrieveFromDBSources() != 1)
                throw new Exception("@丢失了流程引擎注册表数据");
            if (gwf.FK_Dept != WebUser.FK_Dept)
                throw new Exception("@隶属部门错误应当是:" + WebUser.FK_Dept + ",现在是:" + gwf.FK_Dept);

            if (gwf.FK_Flow != fk_flow)
                throw new Exception("@流程编号错误应当是:" + fk_flow + ",现在是:" + gwf.FK_Flow);

            if (gwf.FK_Node != 2302)
                throw new Exception("@当前节点错误应当是:" + 2302 + ",现在是:" + gwf.FK_Node);

            if (gwf.Starter != userNo)
                throw new Exception("@当前 Starter 错误应当是:" + userNo + ",现在是:" + gwf.Starter);

            if (gwf.StarterName != WebUser.Name)
                throw new Exception("@当前 StarterName 错误应当是:" + WebUser.Name + ",现在是:" + gwf.StarterName);

            if (DataType.IsNullOrEmpty(gwf.Title))
                throw new Exception("@ Title 错误, 不能为空. ");

            // 检查工作人员列表.
            GenerWorkerLists wls = new GenerWorkerLists();
            wls.Retrieve(GenerWorkerListAttr.WorkID, this.workid);
            if (wls.Count != 3)
                throw new Exception("@应当有3条数据，现在是:" + wls.Count);
            foreach (GenerWorkerList wl in wls)
            {
                if (wl.FID != 0)
                    throw new Exception("@ FID 错误,应该是" + 0 + ",现在是:" + wl.FID);

                if (wl.FK_Emp == "zhanghaicheng")
                {
                    if (wl.FK_Dept != WebUser.FK_Dept)
                        throw new Exception("@部门错误,应该是" + WebUser.FK_Dept + ",现在是:" + wl.FK_Dept);

                    if (wl.FK_Flow != fk_flow)
                        throw new Exception("@ FK_Flow 错误,应该是" + fk_flow + ",现在是:" + wl.FK_Flow);

                    if (wl.FK_Node != 2301)
                        throw new Exception("@ FK_Node 错误,应该是" + 2301 + ",现在是:" + wl.FK_Node);

                    if (wl.IsEnable == false)
                        throw new Exception("@ IsEnable 错误,应该是true,现在是:" + wl.IsEnable);

                    if (wl.IsRead == true)
                        throw new Exception("@ IsRead 错误,应该是false,现在是:" + wl.IsEnable);

                    if (wl.IsPass == false)
                        throw new Exception("@ IsPass 错误,应该是true,现在是:" + wl.IsEnable);

                    //if (wl.Sender != WebUser.No )
                    //    throw new Exception("@ Sender 错误,应该是:" + WebUser.No + ",现在是:" + wl.Sender);
                }

                if (wl.FK_Emp == "zhoushengyu" || wl.FK_Emp == "zhangyifan")
                {
                    BP.Port.Emp emp = new Port.Emp(wl.FK_Emp);

                    if (wl.FK_Dept != emp.FK_Dept)
                        throw new Exception("@部门错误,应该是" + emp.FK_Dept + ",现在是:" + wl.FK_Dept);

                    if (wl.FK_Flow != fk_flow)
                        throw new Exception("@ FK_Flow 错误,应该是" + fk_flow + ",现在是:" + wl.FK_Flow);

                    if (wl.FK_Node != 2302)
                        throw new Exception("@ FK_Node 错误,应该是" + 2302 + ",现在是:" + wl.FK_Node);

                    if (wl.IsEnable == false)
                        throw new Exception("@ IsEnable 错误,应该是true,现在是:" + wl.IsEnable);

                    if (wl.IsRead == true)
                        throw new Exception("@ IsRead 错误,应该是false,现在是:" + wl.IsEnable);

                    if (wl.IsPass == true)
                        throw new Exception("@ IsPass 错误,应该是false,现在是:" + wl.IsEnable);

                    //if (wl.Sender != "zhanghaicheng")
                    //    throw new Exception("@ Sender 错误,应该是" + WebUser.No + ",现在是:" + wl.Sender);
                }
            }

            string sql = "SELECT COUNT(*) FROM WF_EmpWorks WHERE WorkID=" + this.workid + " AND FK_Emp='zhoushengyu'";
            if (DBAccess.RunSQLReturnValInt(sql) != 1)
                throw new Exception("@不应该查询不到 zhoushengyu 的待办.");

            sql = "SELECT COUNT(*) FROM WF_EmpWorks WHERE WorkID=" + this.workid + " AND FK_Emp='zhangyifan'";
            if (DBAccess.RunSQLReturnValInt(sql) != 1)
                throw new Exception("@不应该查询不到 zhangyifan 的待办.");
            #endregion 检查流程引擎表是否正确?

            // 让其中的一个人登录.
            BP.WF.Dev2Interface.Port_Login("zhoushengyu");
            //让他发送.
            BP.WF.Dev2Interface.Node_SendWork(this.fk_flow, this.workid);


            #region 检查流程引擎表是否正确?
            sql = "SELECT COUNT(*) FROM WF_EmpWorks WHERE WorkID=" + this.workid + " AND FK_Emp='zhoushengyu'";
            if (DBAccess.RunSQLReturnValInt(sql) != 0)
                throw new Exception("@不应该在查询到 zhoushengyu 的待办.");

            sql = "SELECT COUNT(*) FROM WF_EmpWorks WHERE WorkID=" + this.workid + " AND FK_Emp='zhangyifan'";
            if (DBAccess.RunSQLReturnValInt(sql) != 0)
                throw new Exception("@不应该在查询到 zhangyifan 的待办.");
            #endregion 检查流程引擎表是否正确?


            //删除该测试数据.
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(this.fk_flow, workid, true);
        }
    }
}
