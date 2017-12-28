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

namespace BP.UnitTesting.AttrFlow
{
    /// <summary>
    /// 保存草稿
    /// </summary>
    public class SaveDraft : TestBase
    {
        /// <summary>
        /// 保存草稿
        /// </summary>
        public SaveDraft()
        {
            this.Title = "保存草稿";
            this.DescIt = "第一个节点需要保存草稿.";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 执行的方法
        /// </summary>
        public override void Do()
        {
            //让liyan 登录.
            BP.WF.Dev2Interface.Port_Login("liyan");

            //设置流程为不保存草稿.
            Flow fl = new Flow("001");
            fl.DraftRole = WF.Template.DraftRole.None;
            fl.Update();

            //创建workid.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //删除草稿.
            BP.WF.Dev2Interface.Flow_DoDeleteDraft(fl.No, workid, true);

            //重建workid.
            workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //执行保存.
            BP.WF.Dev2Interface.Node_SaveWork(fl.No, 101, workid);

            #region 检查保存的结果.
            //从待办理找，如果找到就是错误。
            string sql = "SELECT count(workid) as Num FROM WF_EmpWorks WHERE WorkID=" + workid;
            if (DBAccess.RunSQLReturnValInt(sql, 0) >= 1)
                throw new Exception("@系统错误：不应该查询到他的待办,当前的状态是:"+fl.DraftRole );

            //获取该流程下的草稿， 如果在草稿箱里能够找到草稿，就是错误。
            DataTable dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable(fl.No);
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["WorkID"].ToString() == workid.ToString())
                    throw new Exception("@系统错误： 不应该在草稿箱里，找到他的草稿。");
            }
            #endregion 检查保存的结果.

            //设置规则为，保存到待办列表. 
            fl.DraftRole = WF.Template.DraftRole.SaveToTodolist;
            fl.Update();
            //执行保存.
            BP.WF.Dev2Interface.Node_SaveWork(fl.No, 101, workid);

            #region 检查保存的结果.
            //从待办理找，如果找不到就是错误。
            sql = "SELECT count(workid) as Num FROM WF_EmpWorks WHERE WorkID=" + workid;
            if (DBAccess.RunSQLReturnValInt(sql, 0) != 1)
                throw new Exception("@系统错误：没有在待办理列表里，找到他的待办, 当前的状态是:"+fl.DraftRole+" sql="+sql );

            //获取该流程下的草稿， 如果在草稿箱里能够找到草稿，就是错误。
            dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable(fl.No);
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["WorkID"].ToString() == workid.ToString())
                    throw new Exception("@系统错误： 不应该在草稿箱里，找到他的草稿。");
            }
            #endregion 检查保存的结果.


            //设置规则为，保存到草稿箱. 
            fl.DraftRole = WF.Template.DraftRole.SaveToDraftList;
            fl.Update();
            //执行保存.
            BP.WF.Dev2Interface.Node_SaveWork(fl.No, 101, workid);

            #region 检查保存的结果.
            //从待办理找，如果找不到就是错误。
            sql = "SELECT count(workid) as Num FROM WF_EmpWorks WHERE WorkID=" + workid;
            if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                throw new Exception("@系统错误：应该查询到他的待办 .但是没有查询到:" + fl.DraftRole+" SQL="+sql);


            //从待办理找，如果找不到就是错误。
            sql = "SELECT count(workid) as Num FROM WF_EmpWorks WHERE WorkID=" + workid+" AND WFState=1";
            if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                throw new Exception("@系统错误：应该查询到他的待办 .但是没有查询到:" + fl.DraftRole+" 请检查是否是草稿状态, SQL="+sql);


            //获取该流程下的草稿， 如果在草稿箱里能够找到草稿，就是错误。
            dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable(fl.No);
            bool isHave = false;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["WorkID"].ToString() == workid.ToString())
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave == false)
                throw new Exception("@系统错误： 没有在草稿箱里，找到他的草稿。");

            #endregion 检查保存的结果.

            //把规则设置回来.
            fl.DraftRole = WF.Template.DraftRole.None;
            fl.Update();

        }
        /// <summary>
        /// 运行完一个流程，并返回它的workid.
        /// </summary>
        /// <returns></returns>
        public Int64 RunCompeleteOneWork()
        {
            string fk_flow = "024";
            string startUser = "zhanghaicheng";
            BP.Port.Emp starterEmp = new Port.Emp(startUser);

            Flow fl = new Flow(fk_flow);

            //让zhanghaicheng登录, 在以后，就可以访问WebUser.No, WebUser.Name 。
            BP.WF.Dev2Interface.Port_Login(startUser);

            //创建空白工作, 发起开始节点.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fk_flow);
            //执行发送，并获取发送对象,.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);

            //执行第二步 :  .
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);

            //执行第三步完成:  .
            BP.WF.Dev2Interface.Port_Login(objs.VarAcceptersID);
            objs = BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);
            return workid;
        }
    }
}
