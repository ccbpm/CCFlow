using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.DA;
using BP.Web;
using BP.Port;
using BP.WF;
using BP.En;
namespace CCFlow.WF.OneWork
{
    public partial class WF_WorkOpt_OneWork_OP : BP.Web.WebPage
    {
        #region 属性

        public new string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }

        public string FK_Node
        {
            get
            {
                return this.Request.QueryString["FK_Node"];
            }
        }

        public string FK_Flow
        {
            get
            {
                string flow = this.Request.QueryString["FK_Flow"];
                if (flow == null)
                {
                    throw new Exception("@没有获取它的流程编号。");
                }
                else
                {
                    return flow;
                }
            }
        }

        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request.QueryString["WorkID"]);
            }
        }

        public Int64 FID
        {
            get
            {
                string fid = this.Request.QueryString["FID"];

                if (string.IsNullOrEmpty(fid))
                    return 0;
                else
                    return Int64.Parse(fid);

            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 功能执行
            try
            {
                switch (this.DoType)
                {
                    case "Del":
                        WorkFlow wf = new WorkFlow(FK_Flow, WorkID);
                        wf.DoDeleteWorkFlowByReal(true);
                        this.WinCloseWithMsg("流程已经被删除.");
                        break;
                    case "HungUp":
                        WorkFlow wf1 = new WorkFlow(FK_Flow, WorkID);
                        //wf1.DoHungUp(HungUpWa;
                        this.WinCloseWithMsg("流程已经被挂起.");
                        break;
                    case "UnHungUp":
                        WorkFlow wf2 = new WorkFlow(FK_Flow, WorkID);
                        //  wf2.DoUnHungUp();
                        this.WinCloseWithMsg("流程已经被解除挂起.");
                        break;
                    case "ComeBack":
                        WorkFlow wf3 = new WorkFlow(FK_Flow, WorkID);
                        wf3.DoComeBackWorkFlow("无");
                        this.WinCloseWithMsg("流程已经被回复启用.");
                        break;
                    case "Takeback": /*取回审批.*/
                        break;

                    case "UnSend":
                        // 转化成编号.
                        string message = BP.WF.Dev2Interface.Flow_DoUnSend(FK_Flow, WorkID);

                        Response.Clear();
                        Response.Write(message);
                        Response.End();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                this.Alert("执行功能:" + DoType + ",出现错误:" + ex.Message);
            }
            #endregion

            int wfState = BP.DA.DBAccess.RunSQLReturnValInt("SELECT WFState FROM WF_GenerWorkFlow WHERE WorkID=" + WorkID, 1);
            WFState wfstateEnum = (WFState)wfState;
            //this.Pub2.AddH3("您可执行的操作<hr>");
            switch (wfstateEnum)
            {
                case WFState.Runing: /* 运行时*/
                    this.FlowOverByCoercion(); /*删除流程.*/
                    this.TackBackCheck(); /*取回审批*/
                    this.Hurry(); /*催办*/
                    this.UnSend(); /*撤销发送*/
                    break;
                case WFState.Complete: // 完成.
                case WFState.Delete: // 逻辑删除..
                    this.RollBack(); /*恢复使用流程*/
                    break;
                case WFState.HungUp: // 挂起.
                    this.AddUnHungUp(); /*撤销挂起*/
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 取回审批
        /// </summary>
        public void TackBackCheck()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(this.WorkID);
            /* 判断是否有取回审批的权限。*/
            this.Pub2.AddEasyUiPanelInfoBegin("取回审批");
            string sql = "SELECT NodeID FROM WF_Node WHERE CheckNodes LIKE '%" + gwf.FK_Node + "%'";
            int myNode = DBAccess.RunSQLReturnValInt(sql, 0);
            if (myNode != 0)
            {
                GetTask gt = new GetTask(myNode);
                if (gt.Can_I_Do_It() == true)
                {
                    this.Pub2.Add("功能执行:<a href=\"javascript:Takeback('" + WorkID + "','" + FK_Flow + "','" + gwf.FK_Node + "','" + myNode + "')\" >点击执行取回审批流程</a>。");
                    this.Pub2.AddBR("说明：如果被成功取回，ccflow就会把停留在别人工作节点上的工作发送到您的待办列表里。");
                }
            }
            else
            {
                this.Pub2.Add("您没有此权限.");
            }
            this.Pub2.AddEasyUiPanelInfoEnd();
            Pub2.AddBR();
        }
        /// <summary>
        /// 强制删除流程
        /// </summary>
        public void FlowOverByCoercion()
        {
            GenerWorkFlow gwf = new GenerWorkFlow(WorkID);
            this.Pub2.AddEasyUiPanelInfoBegin("删除流程");
            if (WebUser.No == "admin")
            {
                this.Pub2.Add("功能执行:<a href=\"javascript:DoFunc('" + FlowOpList.FlowOverByCoercion + "','" + WorkID + "','" + FK_Flow + "','" + FK_Node + "')\" >点击执行删除流程</a>。");
                this.Pub2.AddBR("说明：如果执行流程将会被彻底的删除。");
            }
            else
            {
                this.Pub2.Add("只有admin才能删除流程，您没有此权限.");
            }
            this.Pub2.AddEasyUiPanelInfoEnd();
            Pub2.AddBR();
        }
        /// <summary>
        /// 催办
        /// </summary>
        public void Hurry()
        {
            /*催办*/
            this.Pub2.AddEasyUiPanelInfoBegin("工作催办");



            this.Pub2.Add("您没有此权限.");

            this.Pub2.AddEasyUiPanelInfoEnd();
            Pub2.AddBR();
        }
        /// <summary>
        /// 撤销发送
        /// </summary>
        public void UnSend()
        {
            /*撤销发送*/
            this.Pub2.AddEasyUiPanelInfoBegin("撤销发送");


            //查询是否有权限撤销发送
            GenerWorkerLists workerlists = new GenerWorkerLists();

            QueryObject info = new QueryObject(workerlists);
            info.AddWhere(GenerWorkerListAttr.FK_Emp, WebUser.No);
            info.addAnd();
            info.AddWhere(GenerWorkerListAttr.IsPass, "1");
            info.addAnd();
            info.AddWhere(GenerWorkerListAttr.IsEnable, "1");
            info.addAnd();
            info.AddWhere(GenerWorkerListAttr.WorkID, this.WorkID);
            int count = info.DoQuery();
            if (count > 0)
            {
                this.Pub2.Add("<a href =\"javascript:UnSend('" + this.FK_Flow + "','" + this.WorkID + "','" + FID + "')\" >撤销发送</a>");
            }
            else
            {
                this.Pub2.Add("您没有此权限.");
            }

            this.Pub2.AddEasyUiPanelInfoEnd();
            Pub2.AddBR();
        }
        /// <summary>
        /// 恢复启用流程数据到结束节点
        /// </summary>
        public void RollBack()
        {
            this.Pub2.AddEasyUiPanelInfoBegin("恢复启用流程数据到结束节点");
            if (WebUser.No == "admin")
            {
                this.Pub2.Add("功能执行:<a href=\"javascript:DoFunc('ComeBack','" + WorkID + "','" + FK_Flow + "','" + FK_Node + "')\" >点击执行恢复流程</a>。");
                this.Pub2.AddBR("说明：如果被成功恢复，ccflow就会把待办工作发送给最后一个结束流程的工作人员。");
            }
            else
            {
                this.Pub2.Add("您没有权限.");
            }
            this.Pub2.AddEasyUiPanelInfoEnd();
            Pub2.AddBR();
        }
        /// <summary>
        /// 取消挂起
        /// </summary>
        public void AddUnHungUp()
        {
            this.Pub2.AddEasyUiPanelInfoBegin("取消挂起");
            if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(FK_Flow, int.Parse(FK_Node), WorkID, WebUser.No))
            {
                this.Pub2.Add("功能执行:<a href=\"javascript:DoFunc('UnHungUp','" + WorkID + "','" + FK_Flow + "','" + FK_Node + "')\" >点击执行取消挂起流程</a>。");
                this.Pub2.AddBR("说明：解除流程挂起的状态。");
            }
            else
            {
                this.Pub2.AddBR("您没有此权限，或者当前不是挂起的状态。");
            }
            this.Pub2.AddEasyUiPanelInfoEnd();
            Pub2.AddBR();
        }
        /// <summary>
        /// 挂起
        /// </summary>
        public void AddHungUp()
        {
            this.Pub2.AddEasyUiPanelInfoBegin("挂起");
            if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(FK_Flow, int.Parse(FK_Node), WorkID, WebUser.No))
            {
                this.Pub2.Add("功能执行:<a href=\"javascript:DoFunc('" + FlowOpList.HungUp + "','" + WorkID + "','" + FK_Flow + "','" + FK_Node + "','')\" >点击执行挂起流程</a>。");
                this.Pub2.AddBR("说明：对该流程执行挂起，挂起后可以解除挂起，挂起的时间不计算考核。");
            }
            else
            {
                this.Pub2.Add("您没有此权限.");
            }
            this.Pub2.AddEasyUiPanelInfoEnd();
            Pub2.AddBR();
        }
        /// <summary>
        /// 移交
        /// </summary>
        public void AddShift()
        {
            this.Pub2.AddEasyUiPanelInfoBegin("移交");
            if (BP.WF.Dev2Interface.Flow_IsCanDoCurrentWork(FK_Flow, int.Parse(FK_Node), WorkID, WebUser.No))
            {
                this.Pub2.Add("功能执行:<a href=\"javascript:DoFunc('" + FlowOpList.UnHungUp + "','" + WorkID + "','" + FK_Flow + "','" + FK_Node + "')\" >点击执行取消挂起流程</a>。");
                this.Pub2.AddBR("说明：解除流程挂起的状态。");
            }
            else
            {
                this.Pub2.AddBR("您没有此权限，或者当前不是挂起的状态。");
            }
            this.Pub2.AddEasyUiPanelInfoEnd();
            Pub2.AddBR();
        }

        public void AddShiftByCoercion()
        {
            this.Pub2.AddEasyUiPanelInfoBegin("强制移交");
            if (WebUser.No == "admin")
            {
                this.Pub2.Add("功能执行:<a href=\"javascript:DoFunc('" + FlowOpList.ShiftByCoercion + "','" + WorkID + "','" + FK_Flow + "','" + FK_Node + "')\" >点击执行取消挂起流程</a>。");
                this.Pub2.AddBR("说明：解除流程挂起的状态。");
            }
            else
            {
                this.Pub2.AddBR("您没有此权限。");
            }
            this.Pub2.AddEasyUiPanelInfoEnd();
            Pub2.AddBR();
        }
    }
}