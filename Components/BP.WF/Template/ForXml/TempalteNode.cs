using System;
using System.Data;
using BP.DA;
using BP.Sys;
using BP.En;
using System.Collections;
using BP.Port;
using BP.WF.Data;
using BP.WF.Port;
using System.Collections.Generic;
using BP.WF.Template.SFlow;

namespace BP.WF.Template
{
    /// <summary>
    /// 这里存放每个节点的信息.	 
    /// </summary>
    public class TemplateNode : Entity
    {
        #region 构造函数
        /// <summary>
        /// 节点
        /// </summary>
        public TemplateNode() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Node", "节点");

                #region 基本属性.
                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.AddTBString(NodeAttr.Name, null, "名称", true, false, 0, 150, 10);
                map.AddTBString(NodeAttr.Tip, null, "操作提示", true, true, 0, 100, 10, false);
                map.AddTBInt(NodeAttr.Step, (int)NodeWorkType.Work, "流程步骤", true, false);
                map.AddTBString(NodeAttr.Icon, null, "节点ICON图片路径", true, false, 0, 70, 10);
                map.AddTBInt(NodeAttr.NodeWorkType, 0, "节点类型", false, false);
                map.AddTBString(NodeAttr.FK_Flow, null, "FK_Flow", false, false, 0, 3, 10);
                map.AddTBInt(NodeAttr.IsGuestNode, 0, "是否是客户执行节点", false, false);
                map.AddTBString(NodeAttr.FlowName, null, "流程名", false, true, 0, 200, 10);

                //为铁路局,会签子流程. 增加
                map.AddTBInt(NodeAttr.IsSendDraftSubFlow, 0, "是否发送草稿子流程？", false, false);
                map.AddTBInt(NodeAttr.IsResetAccepter, 0, "可逆节点时是否重新计算接收人", false, false);
                map.AddTBString(NodeAttr.FrmAttr, null, "FrmAttr", false, true, 0, 300, 10);
                #endregion 基本属性.

                #region 审核组件.
                map.AddTBInt(NodeAttr.FWCSta, 0, "审核组件", false, false);
                map.AddTBFloat(NodeAttr.FWC_H, 0, "审核组件高度", false, true);
                map.AddTBInt(NodeWorkCheckAttr.FWCOrderModel, 0, "协作模式下操作员显示顺序", false, false);
                map.AddTBInt(NodeWorkCheckAttr.FWCVer, 0, "审核组件版本", false, false);
                map.AddTBInt("FWCAth", 0, "审核附件是否启用", false, false);
                map.AddTBString(NodeWorkCheckAttr.CheckField, null, "签批字段", true, false, 0, 50, 10, false);
                map.AddTBString(NodeWorkCheckAttr.FWCDefInfo, null, "默认意见", true, false, 0, 50, 10);
                #endregion 审核组件.

                #region 子流程信息
                map.AddTBInt(FrmSubFlowAttr.SFSta, 0, "父子流程组件", false, false);
                map.AddTBInt(NodeAttr.SubFlowX, 0, "子流程设计器位置X", false, false);
                map.AddTBInt(NodeAttr.SubFlowY, 0, "子流程设计器位置Y", false, false);
                #endregion 子流程信息

                #region 考核属性.
                map.AddTBString(BtnAttr.CHLab, "节点时限", "节点时限", true, false, 0, 50, 10);
                map.AddTBInt(BtnAttr.CHRole, 0, "时限规则", true, false);

                map.AddTBString(BtnAttr.HelpLab, "帮助提示", "帮助", true, false, 0, 50, 10);
                map.AddTBInt(BtnAttr.HelpRole, 0, "帮助提示规则", true, false);
                map.AddTBFloat(NodeAttr.TimeLimit, 2, "限期(天)", true, false); //"限期(天)".
                                                                             //  map.AddTBFloat(NodeAttr.TSpanHour, 0, "小时", true, false); //"限期(分钟)".
                map.AddTBInt(NodeAttr.TWay, 0, "时间计算方式", true, false); //0=不计算节假日,1=计算节假日.
                map.AddTBInt(NodeAttr.TAlertRole, 0, "逾期提醒规则", false, false); //"限期(天)"
                map.AddTBInt(NodeAttr.TAlertWay, 0, "逾期提醒方式", false, false); //"限期(天)"

                map.AddTBFloat(NodeAttr.WarningDay, 1, "工作预警(天)", true, false);    // "警告期限(0不警告)"

                map.AddTBInt(NodeAttr.WAlertRole, 0, "预警提醒规则", false, false); //"限期(天)"
                map.AddTBInt(NodeAttr.WAlertWay, 0, "预警提醒方式", false, false); //"限期(天)"

                map.AddTBFloat(NodeAttr.TCent, 2, "扣分(每延期1小时)", false, false);
                map.AddTBInt(NodeAttr.CHWay, 0, "考核方式", false, false); //"限期(天)"

                //考核相关.
                map.AddTBInt(NodeAttr.IsEval, 0, "是否工作质量考核", true, true);
                map.AddTBInt(NodeAttr.OutTimeDeal, 0, "超时处理方式", false, false);
                map.AddTBString(NodeAttr.DoOutTime, null, "超时处理内容", true, false, 0, 300, 10, true);
                //map.AddTBString(NodeAttr.DoOutTime, null, "超时处理内容", true, false, 0, 300, 10, true);
                map.AddTBString(NodeAttr.DoOutTimeCond, null, "执行超时的条件", false, false, 0, 200, 100);
                #endregion 考核属性.

                map.AddTBString(NodeWorkCheckAttr.FWCNodeName, null, "节点意见名称", true, false, 0, 100, 10);
                map.AddTBString(NodeAttr.Doc, null, "描述", true, false, 0, 100, 10);
                map.AddBoolean(NodeAttr.IsTask, true, "允许分配工作否?", true, true);

                //退回相关.
                map.AddTBInt(NodeAttr.ReturnRole, 2, "退回规则", true, true);
                map.AddTBString(NodeAttr.ReturnReasonsItems, null, "退回原因", true, false, 0, 50, 10, true);
                map.AddTBString(NodeAttr.ReturnAlert, null, "被退回后信息提示", true, false, 0, 50, 10, true);
                map.AddBoolean(NodeAttr.ReturnCHEnable, false, "是否启用退回考核规则", true, true);

                map.AddTBInt(NodeAttr.ReturnOneNodeRole, 0, "单节点退回规则", true, true);
                map.AddTBString(BtnAttr.ReturnField, null, "退回信息填写字段", true, false, 0, 50, 10, true);

                map.AddTBInt(NodeAttr.DeliveryWay, 0, "访问规则", true, true);
                map.AddTBInt(NodeAttr.IsExpSender, 1, "本节点接收人不允许包含上一步发送人", true, true);

                map.AddTBInt(NodeAttr.CancelRole, 0, "撤销规则", true, true);
                map.AddTBInt(NodeAttr.CancelDisWhenRead, 0, "对方已读不能撤销", true, true);

                map.AddTBInt(NodeAttr.WhenNoWorker, 0, "未找到处理人时", true, true);
                map.AddTBString(NodeAttr.DeliveryParas, null, "访问规则设置", true, false, 0, 300, 10);
                map.AddTBString(NodeAttr.NodeFrmID, null, "节点表单ID", true, false, 0, 50, 10);

                map.AddTBInt(NodeAttr.CCRole, 0, "抄送规则", true, true);
                map.AddTBInt(NodeAttr.CCWriteTo, 0, "抄送数据写入规则", true, true);

                map.AddTBInt(BtnAttr.DelEnable, 0, "删除规则", true, true);
                map.AddTBInt(NodeAttr.SaveModel, 0, "保存模式", true, true);

                map.AddTBInt(NodeAttr.IsCanDelFlow, 0, "是否可以删除流程", true, true);

                map.AddTBInt(NodeAttr.ThreadKillRole, 0, "子线程删除方式", true, true);

                map.AddTBInt(NodeAttr.TodolistModel, 0, "多人处理规则", true, true);

                //add.
                map.AddTBInt(BtnAttr.HuiQianRole, 0, "会签模式", true, true);
                map.AddTBInt(NodeAttr.TeamLeaderConfirmRole, 0, "组长确认规则", true, true);
                map.AddTBString(NodeAttr.TeamLeaderConfirmDoc, null, "组长确认设置内容", true, false, 0, 100, 10);
                map.AddTBInt(BtnAttr.HuiQianLeaderRole, 0, "组长会签规则", true, true);

                map.AddTBInt(BtnAttr.ScripRole, 0, "小纸条规则", true, true);

                map.AddTBInt(NodeAttr.USSWorkIDRole, 0, "是否允许子线程接受人员重复(对子线程点有效)?", true, true);
                map.AddTBInt(NodeAttr.IsBackTracking, 1, "是否可以在退回后原路返回(只有启用退回功能才有效)", true, true);
                map.AddTBInt(NodeAttr.IsBackResetAccepter, 0, "原路返回后是否自动计算接收人", true, true);
                map.AddTBInt(BtnAttr.ThreadIsCanDel, 0, "是否可以删除子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效，在子线程退回后的操作)？", true, true);
                map.AddTBInt(BtnAttr.ThreadIsCanAdd, 0, "是否可以增加子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效)？", true, true);

                map.AddTBInt(NodeAttr.IsKillEtcThread, 0, "是否允许删除所有的子线程(对于子线程向分流节点退回有效)", true, true);

                map.AddTBInt(NodeAttr.IsRM, 1, "是否启用投递路径自动记忆功能?", true, true);
                map.AddTBInt(NodeAttr.IsOpenOver, 0, "是否打开即审批?", true, true);
                map.AddBoolean(NodeAttr.IsHandOver, false, "是否可以移交", true, true);
                map.AddTBInt(NodeAttr.PassRate, 100, "通过率", true, true);
                map.AddTBInt(NodeAttr.RunModel, 0, "运行模式(对普通节点有效)", true, true);
                map.AddTBInt(NodeAttr.BlockModel, 0, "阻塞模式", true, true);
                map.AddTBString(NodeAttr.BlockExp, null, "阻塞表达式", true, false, 0, 200, 10);
                map.AddTBString(NodeAttr.BlockAlert, null, "被阻塞提示信息", true, false, 0, 100, 10);

                map.AddTBInt(NodeAttr.WhoExeIt, 0, "谁执行它", true, true);
                map.AddTBInt(NodeAttr.ReadReceipts, 0, "已读回执", true, true);
                map.AddTBInt(NodeAttr.CondModel, 0, "方向条件控制规则", true, true);

                // 自动跳转.
                map.AddTBInt(NodeAttr.AutoJumpRole0, 0, "处理人就是提交人0", false, false);
                map.AddTBInt(NodeAttr.AutoJumpRole1, 0, "处理人已经出现过1", false, false);
                map.AddTBInt(NodeAttr.AutoJumpRole2, 0, "处理人与上一步相同2", false, false);

                map.AddTBString(NodeAttr.AutoJumpExp, null, "表达式", true, false, 0, 200, 10, true);
                //@0=上一个节点发送时@1=当前节点工作打开时.
                map.AddTBInt(NodeAttr.SkipTime, 0, "执行跳转事件", false, false);

                // 批处理规则， 2021.1.20 为福建人寿重构.
                // @0=不启用，1=审核组件模式，2=审核分组字段模式,3=自定义url模式.
                map.AddTBInt(NodeAttr.BatchRole, 0, "批处理", true, true);

                //map.AddTBInt(NodeAttr.BatchListCount, 12, "批处理数量", true, true);
                //map.AddTBString(NodeAttr.BatchParas, null, "参数", true, false, 0, 500, 10);

                map.AddTBInt(NodeAttr.PrintDocEnable, 0, "打印方式", true, true);
                //打印PDF的处理
                map.AddTBInt(BtnAttr.PrintPDFModle, 0, "PDF打印规则", true, true);
                map.AddTBInt(BtnAttr.PRIEnable, 0, "重要性规则", true, true);
                map.AddTBString(BtnAttr.ShuiYinModle, null, "打印水印规则", true, false, 20, 100, 100, true);

                //表单相关.
                map.AddTBInt(NodeAttr.FormType, 1, "表单类型", false, false);
                map.AddTBString(NodeAttr.FormUrl, "http://", "表单URL", true, false, 0, 300, 10);
                map.AddTBInt(NodeAttr.TurnToDeal, 0, "转向处理", false, false);
                map.AddTBString(NodeAttr.TurnToDealDoc, null, "发送后提示信息", true, false, 0, 200, 10, true);
                map.AddTBInt(NodeAttr.NodePosType, 0, "位置", false, false);
                map.AddTBString(NodeAttr.HisStas, null, "角色", false, false, 0, 300, 10);
                map.AddTBString(NodeAttr.HisDeptStrs, null, "部门", false, false, 0, 600, 10);
                map.AddTBString(NodeAttr.HisToNDs, null, "转到的节点", false, false, 0, 80, 10);
                map.AddTBString(NodeAttr.HisBillIDs, null, "单据IDs", false, false, 0, 50, 10);
                //  map.AddTBString(NodeAttr.HisEmps, null, "HisEmps", false, false, 0, 3000, 10);
                map.AddTBString(NodeAttr.HisSubFlows, null, "HisSubFlows", false, false, 0, 30, 10);
                map.AddTBString(NodeAttr.PTable, null, "物理表", false, false, 0, 100, 10);

                map.AddTBString(NodeAttr.GroupStaNDs, null, "角色分组节点", false, false, 0, 200, 10);
                map.AddTBInt(NodeAttr.X, 0, "X坐标", false, false);
                map.AddTBInt(NodeAttr.Y, 0, "Y坐标", false, false);

                map.AddTBString(NodeAttr.FocusField, null, "焦点字段", false, false, 0, 30, 10);
                map.AddTBString(NodeAttr.JumpToNodes, null, "可跳转的节点", true, false, 0, 20, 10, true);
                map.AddTBInt(NodeAttr.JumpWay, 0, "跳转规则", false, false);
                map.AddTBString(NodeAttr.RefOneFrmTreeType, "", "独立表单类型", false, false, 0, 20, 10);//RefOneFrmTree

                map.AddTBString(NodeAttr.DoOutTimeCond, null, "执行超时的条件", false, false, 0, 20, 100);
                map.AddTBString(NodeAttr.SelfParas, null, "自定义参数(如果太小可以手动扩大)", true, false, 0, 1000, 10);

                #region 子流程相关的参数
                map.AddTBFloat(FrmSubFlowAttr.SF_H, 300, "高度", true, false);
                #endregion 子流程相关的参数


                #region 兼容版本.
                map.AddTBString(NodeAttr.NodeStations, null, "绑定的岗位", false, false, 0, 100, 100);
                map.AddTBString(NodeAttr.NodeStationsT, null, "绑定的岗位t", false, false, 0, 200, 100);

                map.AddTBString(NodeAttr.NodeEmps, null, "绑定的人员", false, false, 0, 100, 100);
                map.AddTBString(NodeAttr.NodeEmpsT, null, "绑定的人员t", false, false, 0, 200, 100);

                map.AddTBString(NodeAttr.NodeDepts, null, "绑定的部门", false, false, 0, 100, 100);
                map.AddTBString(NodeAttr.NodeDeptsT, null, "绑定的部门t", false, false, 0, 200, 100);

                map.AddTBString(NodeAttr.ARDeptModel, null, "部门集合范围", false, false, 0, 200, 100);
                map.AddTBString(NodeAttr.ARStaModel, null, "角色集合范围", false, false, 0, 200, 100);
                map.AddTBInt(NodeAttr.ShenFenModel, 0, "身份规则", false, false);

                #endregion 兼容版本.


                map.AddTBAtParas(500);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 节点集合
    /// </summary>
    public class TemplateNodes : EntitiesOID
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new TemplateNode();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 节点集合
        /// </summary>
        public TemplateNodes()
        {
        }

        public TemplateNodes(String fk_flow)
        {
		    this.Retrieve(NodeAttr.FK_Flow, fk_flow, NodeAttr.Step);
	    }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TemplateNode> ToJavaList()
        {
            return (System.Collections.Generic.IList<TemplateNode>)this;
        }
        /// <summary>
        /// 转化成list 为了翻译成java的需要
        /// </summary>
        /// <returns>List</returns>
        public List<TemplateNode> Tolist()
        {
            List<TemplateNode> list = new List<TemplateNode>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TemplateNode)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
