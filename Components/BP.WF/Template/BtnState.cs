using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.DA;
using BP;
using BP.Sys;
using BP.En;

namespace BP.WF.Template
{
    /// <summary>
    /// 按钮状态
    /// </summary>
    public class ButtonState
    {
        public Int64 WorkID = 0;
        public int CurrNodeIDOfUI = 0;
        public int CurrNodeIDOfFlow = 0;
        public string FK_Flow = null;
        public void InitNodeIsCurr()
        {
            // 获取.
            Node nd = new Node(this.CurrNodeIDOfFlow);
            if (nd.IsStartNode)
            {
                /* 开始节点允许删除流程 */
                this.Btn_DelFlow = true;
                this.Btn_Send = true;
                this.Btn_Save = true;
                return;
            }

            #region 判断是否可以撤销发送.
            WorkNode wn = new WorkNode(this.WorkID, this.CurrNodeIDOfFlow);
            WorkNode wnPri = wn.GetPreviousWorkNode();

            // 判断它是否可以处理上一步工作.
            GenerWorkerList wl = new GenerWorkerList();
            int num = wl.Retrieve(GenerWorkerListAttr.FK_Emp, BP.Web.WebUser.No,
                GenerWorkerListAttr.FK_Node, wnPri.HisNode.NodeID,
                GenerWorkerListAttr.WorkID, this.WorkID);
            if (num >= 1)
            {
                /*如果能够处理上一步工作*/
            }
            else
            {
                /*不能处理上一步工作, 就可以让其退回*/
                this.Btn_Return = nd.IsCanReturn;
                this.Btn_Send = true;
                this.Btn_Save = true;
            }
            #endregion
        }
        public void InitNodeIsNotCurr()
        {
            string sql = "SELECT count(*) FROM WF_GenerWorkerlist WHERE FK_Node=" + this.CurrNodeIDOfUI + " AND WorkID=" + this.WorkID;
            if (DBAccess.RunSQLReturnValInt(sql, 0) >= 1)
                this.Btn_UnSend = true;
        }
        public ButtonState(string fk_flow, int currNodeID, Int64 workid)
        {
            this.FK_Flow = fk_flow;
            this.CurrNodeIDOfUI = currNodeID;
            this.WorkID = workid;
            if (workid != 0)
                this.Btn_Track = true;

            string sql = "SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + workid;
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                /* 说明没有 workid 初始化工作的情况, 只有保存与发送两个按钮是可用的 */
                this.Btn_Send = true;
                this.Btn_Save = true;
                return;
            }

            // 设置当前流程节点。
            this.CurrNodeIDOfFlow = int.Parse(dt.Rows[0][0].ToString());

            if (this.CurrNodeIDOfUI == this.CurrNodeIDOfFlow)
            {
                /*如果流程运行的节点与当前的节点是相等的*/
                InitNodeIsCurr();
            }
            else
            {
                InitNodeIsNotCurr();
            }
        }
        /// <summary>
        /// 保存按钮
        /// </summary>
        public bool Btn_Send = false;
        /// <summary>
        /// 保存按钮
        /// </summary>
        public bool Btn_Save = false;
        /// <summary>
        /// 转发
        /// </summary>
        public bool Btn_Forward = false;
        /// <summary>
        /// 退回
        /// </summary>
        public bool Btn_Return = false;
        /// <summary>
        /// 撤销发送
        /// </summary>
        public bool Btn_UnSend = false;
        /// <summary>
        /// 删除流程
        /// </summary>
        public bool Btn_DelFlow = false;
        /// <summary>
        /// 新建流程
        /// </summary>
        public bool Btn_NewFlow = false;
        /// <summary>
        /// 工作轨迹
        /// </summary>
        public bool Btn_Track = false;
    }
}
