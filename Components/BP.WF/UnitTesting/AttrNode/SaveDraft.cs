using System;
using System.Collections.Generic;
using System.Text;
using BP.WF;
using BP.WF.Template;
using BP.WF.Data;
using BP.En;
using BP.DA;
using BP.Web;
using System.Data;
using System.Collections;
using BP.UnitTesting;

namespace BP.UnitTesting.NodeAttr
{
    public  class SaveDraft : TestBase
    {
        /// <summary>
        /// 保存草稿-保存草稿
        /// </summary>
        public SaveDraft()
        {
            this.Title = "保存草稿";
            this.DescIt = "新建立一个流程实例，保存草稿是否可以？";
            this.EditState = EditState.Passed;
        }
        /// <summary>
        /// 说明 ：此测试针对于演示环境中的 001 流程编写的单元测试代码。
        /// 涉及到了: 创建，发送，撤销，方向条件、退回等功能。
        /// </summary>
        public override void Do()
        {
            string fk_flow = "032";
            string userNo = "zhanghaicheng";

            Flow fl = new Flow(fk_flow);
            if (fl.DraftRole == DraftRole.None)
            {
                fl.DraftRole = DraftRole.SaveToDraftList;
                fl.Update(); //草稿列表.
            }

            // zhanghaicheng 登录.
            BP.WF.Dev2Interface.Port_Login(userNo);

            //创建空白工作.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            #region 检查创建新工作是否是blank状态.
            GERpt rpt = fl.HisGERpt;
            rpt.OID = workid;
            rpt.RetrieveFromDBSources();
            if (rpt.WFState != WFState.Blank)
                throw new Exception("@创建新工作应该是Blank状态，现在是:" + rpt.WFState);

            GenerWorkFlow gwf = new GenerWorkFlow();
            gwf.WorkID = workid;
            if (gwf.RetrieveFromDBSources() == 0)
                throw new Exception("@创建workid没有写入到流程注册表.");

            if (gwf.WFState != WFState.Blank)
                throw new Exception("@流程注册表内的wfstate 不是空白状态，现在状态是:" + rpt.WFState);
            #endregion


            //执行保存.
            BP.WF.Dev2Interface.Node_SaveWork(fl.No, 3201,  workid);

            #region 检查创建新工作是否是blank状态.
            rpt = fl.HisGERpt;
            rpt.OID = workid;
            rpt.RetrieveFromDBSources();
            if (rpt.WFState != WFState.Draft)
                throw new Exception("@执行保存后应该是Draft状态，现在是:" + rpt.WFState);
            #endregion


            #region 检查草稿是否有？
            bool isHave = false;
            DataTable dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["OID"].ToString() == workid.ToString())
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave == true)
                throw new Exception("@不应该找到草稿。");
            #endregion

            //删除草稿.
            BP.WF.Dev2Interface.Node_DeleteDraft(fl.No, workid);

            //执行创建工作,一个新的workid.
            Int64 workidNew = BP.WF.Dev2Interface.Node_CreateBlankWork(fl.No);

            //比较两个workid是否一致. 
            if (workidNew == workid)
                throw new Exception("@执行删除草稿失败.");

            //设置成草稿.
            BP.WF.Dev2Interface.Node_SetDraft(fl.No, workid);

            #region 检查保存的草稿数据是否完整。
            rpt = fl.HisGERpt;
            rpt.OID = workid;
            rpt.RetrieveFromDBSources();
            if (rpt.WFState != WFState.Draft)
                throw new Exception("@此 GERpt 应该是 Draft 状态,现在是:" + rpt.WFState);

            isHave = false;
            dt = BP.WF.Dev2Interface.DB_GenerDraftDataTable();
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["OID"].ToString() == workid.ToString())
                {
                    isHave = true;
                    break;
                }
            }
            if (isHave == false)
                throw new Exception("@没有从接口里找到草稿。");
            #endregion

        }
    }
}
