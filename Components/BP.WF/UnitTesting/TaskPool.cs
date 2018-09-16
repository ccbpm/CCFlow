using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.En;
using BP.DA;
using BP.Web;
using System.Collections;


namespace BP.UnitTesting
{
    /// <summary>
    /// 共享任务池
    /// </summary>
    public class TaskPool : TestBase
    {
        /// <summary>
        /// 共享任务池
        /// </summary>
        public TaskPool()
        {
            this.Title = "共享任务池";
            this.DescIt = "流程: 以demo 流程068 为例测试。";
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
        /// 1， 测试共享任务的取出，放入。
        /// 2， 共享任务池的两种模式
        /// </summary>
        public override void Do()
        {
            if (BP.WF.Glo.IsEnableTaskPool == false)
                throw new Exception("@此单元测试需要打开web.config中的IsEnableTaskPool配置.");

            this.fk_flow = "068";
            fl = new Flow("068");

            Node nd = new Node(6899);
        //    nd.IsEnableTaskPool = true;
            nd.Update();

            string sUser = "zhoupeng";
            BP.WF.Dev2Interface.Port_Login(sUser);

            //创建.
            workID = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //执行发送，指定发送给两个人。
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fl.No, workID, null, null, 0, "zhangyifan,zhanghaicheng");

            #region 检查数据结果
            GenerWorkFlow gwf = new GenerWorkFlow(this.workID);
            if (gwf.TodoEmpsNum != 2)
                throw new Exception("@应当有两个工作人员，现在是:" + gwf.TodoEmpsNum);

            if (gwf.TodoEmps != "zhangyifan,张一帆;zhanghaicheng,张海成;")
                throw new Exception("@应当有两个工作人员，现在是:" + gwf.TodoEmps);

            if (gwf.TaskSta != TaskSta.Sharing)
                throw new Exception("@应该共享的状态，现在是:" + gwf.TaskSta.ToString());
            #endregion 检查数据结果。

            //让zhanghaicheng登陆。
            BP.WF.Dev2Interface.Port_Login("zhangyifan");

            #region 检查是否具有她的待办。
            // 检查 zhangyifan
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable();
            bool isHave = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["WorkID"].ToString() == this.workID.ToString())
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave == true)
                throw new Exception("@不应该找到她的待办，因为它是共享任务。");

            // 检查 zhanghaicheng， 他应当有待办任务。
            int v = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(*) from wf_empWorks where WorkID=" + this.workID + " AND FK_Emp='zhanghaicheng' ", 100);
            if (v != 1)
                throw new Exception("@不应该找不到到他的待办。");


            // 从任务池里找。
            dt = BP.WF.Dev2Interface.DB_TaskPool();
            isHave = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["WorkID"].ToString() == this.workID.ToString())
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave == false)
                throw new Exception("@没有在任务池里找到她的待办。");
            #endregion 检查是否具有她的待办。

            // 测试1
            CheckData();

            // 测试2
            CheckData();

            // 测试3
            CheckData();
        }
        /// <summary>
        /// 检查数据
        /// </summary>
        private void CheckData()
        {
            // 执行获取任务。
            BP.WF.Dev2Interface.Node_TaskPoolTakebackOne(workID);

            #region 检查任务
            gwf = new GenerWorkFlow(this.workID);
            if (gwf.TaskSta != TaskSta.Takeback)
                throw new Exception("@应该是取走的状态，但是现在是:" + gwf.TaskSta.ToString());

            // 检查 zhanghaicheng， 他不应该有待办任务。
            int v = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(*) from wf_empWorks where WorkID=" + this.workID + " AND FK_Emp='zhanghaicheng' ", 100);
            if (v != 0)
                throw new Exception("@不应该找到到他的待办。");

            // 从待办里找,来检查zhangyifan 的任务。
            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable();
            bool isHave = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["WorkID"].ToString() == this.workID.ToString())
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave == false)
                throw new Exception("@不应该找不到[" + WebUser.No + "]待办，共享任务，申请下来后也要放在待办里。");

            // 从任务池里找。
            dt = BP.WF.Dev2Interface.DB_TaskPool();
            isHave = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["WorkID"].ToString() == this.workID.ToString())
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave == true)
                throw new Exception("@执行取走这个任务后，不应该再找到她的任务了。");

            // 获得我申请下来的任务
            dt = BP.WF.Dev2Interface.DB_TaskPoolOfMyApply();
            isHave = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["WorkID"].ToString() == this.workID.ToString())
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave == false)
                throw new Exception("@没有找到" + WebUser.No + "申请的任务");
            #endregion 检查任务

            // 放入任务池
            BP.WF.Dev2Interface.Node_TaskPoolPutOne(workID);

            #region 数据检查
            gwf = new GenerWorkFlow(this.workID);
            if (gwf.TaskSta != TaskSta.Sharing)
                throw new Exception("@应当是sharing 现在是:" + gwf.TaskSta.ToString());

            // 检查 zhangyifan
            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable();
            isHave = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["WorkID"].ToString() == this.workID.ToString())
                {
                    isHave = true;
                    break;
                }
            }

            if (isHave == true)
                throw new Exception("@不应该找到她的待办，因为它是共享任务。");

            // 检查 zhanghaicheng， 他应当有待办任务。
            v = BP.DA.DBAccess.RunSQLReturnValInt("SELECT COUNT(*) from wf_empWorks where WorkID=" + this.workID + " AND FK_Emp='zhanghaicheng' ", 100);
            if (v != 1)
                throw new Exception("@不应该找不到到他的待办。");

            // 从任务池里找。
            dt = BP.WF.Dev2Interface.DB_TaskPool();
            isHave = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["WorkID"].ToString() == this.workID.ToString())
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave == false)
                throw new Exception("@没有在任务池里找到她的待办。");
            #endregion 检查是否具有她的待办。
        }
    }
}
