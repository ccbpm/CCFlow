using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Collections;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点属性.
    /// </summary>
    public class NodeExt : Entity
    {
        #region 索引
        /// <summary>
        /// 获取节点的帮助信息url
        /// <para></para>
        /// <para>added by liuxc,2014-8-19</para> 
        /// </summary>
        /// <param name="sysNo">帮助网站中所属系统No</param>
        /// <param name="searchTitle">帮助主题标题</param>
        /// <returns></returns>
        private string this[string sysNo, string searchTitle]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(sysNo) || string.IsNullOrWhiteSpace(searchTitle))
                    return "javascript:alert('此处还没有帮助信息！')";

                return string.Format("http://online.ccflow.org/KM/Tree.aspx?no={0}&st={1}", sysNo, Uri.EscapeDataString(searchTitle));
            }
        }
        #endregion

        #region 常量
        /// <summary>
        /// CCFlow流程引擎
        /// </summary>
        private const string SYS_CCFLOW = "001";
        /// <summary>
        /// CCForm表单引擎
        /// </summary>
        private const string SYS_CCFORM = "002";
        #endregion

        #region 属性.
        /// <summary>
        /// 会签规则
        /// </summary>
        public HuiQianRole HuiQianRole
        {
            get
            {
                return (HuiQianRole)this.GetValIntByKey(BtnAttr.HuiQianRole);
            }
            set
            {
                this.SetValByKey(BtnAttr.HuiQianRole, (int)value);
            }
        }
        /// <summary>
        /// 超时处理方式
        /// </summary>
        public OutTimeDeal HisOutTimeDeal
        {
            get
            {
                return (OutTimeDeal)this.GetValIntByKey(NodeAttr.OutTimeDeal);
            }
            set
            {
                this.SetValByKey(NodeAttr.OutTimeDeal, (int)value);
            }
        }
        /// <summary>
        /// 访问规则
        /// </summary>
        public ReturnRole HisReturnRole
        {
            get
            {
                return (ReturnRole)this.GetValIntByKey(NodeAttr.ReturnRole);
            }
            set
            {
                this.SetValByKey(NodeAttr.ReturnRole, (int)value);
            }
        }
        /// <summary>
        /// 访问规则
        /// </summary>
        public DeliveryWay HisDeliveryWay
        {
            get
            {
                return (DeliveryWay)this.GetValIntByKey(NodeAttr.DeliveryWay);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryWay, (int)value);
            }
        }
        /// <summary>
        /// 步骤
        /// </summary>
        public int Step
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.Step);
            }
            set
            {
                this.SetValByKey(NodeAttr.Step, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 审核组件状态
        /// </summary>
        public FrmWorkCheckSta HisFrmWorkCheckSta
        {
            get
            {
                return (FrmWorkCheckSta)this.GetValIntByKey(NodeAttr.FWCSta);
            }
        }

        /// <summary>
        /// 超时处理内容
        /// </summary>
        public string DoOutTime
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DoOutTime);
            }
            set
            {
                this.SetValByKey(NodeAttr.DoOutTime, value);
            }
        }
        /// <summary>
        /// 超时处理条件
        /// </summary>
        public string DoOutTimeCond
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DoOutTimeCond);
            }
            set
            {
                this.SetValByKey(NodeAttr.DoOutTimeCond, value);
            }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.Name);
            }
            set
            {
                this.SetValByKey(NodeAttr.Name, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FK_Flow);
            }
            set
            {
                this.SetValByKey(NodeAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.FlowName);
            }
            set
            {
                this.SetValByKey(NodeAttr.FlowName, value);
            }
        }
        /// <summary>
        /// 接受人sql
        /// </summary>
        public string DeliveryParas
        {
            get
            {
                return this.GetValStringByKey(NodeAttr.DeliveryParas);
            }
            set
            {
                this.SetValByKey(NodeAttr.DeliveryParas, value);
            }
        }
        /// <summary>
        /// 是否可以退回
        /// </summary>
        public bool ReturnEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.ReturnRole);
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public override string PK
        {
            get
            {
                return "NodeID";
            }
        }
        #endregion 属性.

        #region 初试化全局的 Node
        /// <summary>
        /// 访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (this.FK_Flow == "")
                {
                    uac.OpenForAppAdmin();
                    return uac;
                }

                Flow fl = new Flow(this.FK_Flow);
                if (BP.Web.WebUser.No == "admin")
                    uac.IsUpdate = true;
                else
                    uac.IsUpdate = true; //权宜之计.
                return uac;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 节点
        /// </summary>
        public NodeExt() { }
        /// <summary>
        /// 节点
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public NodeExt(int nodeid)
        {
            this.NodeID = nodeid;
            this.Retrieve();
        }
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
                //map 的基 础信息.
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);

                #region  基础属性
                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                map.SetHelperUrl(NodeAttr.NodeID, "http://ccbpm.mydoc.io/?v=5404&t=17901");

                map.AddTBInt(NodeAttr.Step, 0, "步骤(无计算意义)", true, false);
                map.SetHelperUrl(NodeAttr.Step, "http://ccbpm.mydoc.io/?v=5404&t=17902");

                //map.SetHelperAlert(NodeAttr.Step, "它用于节点的排序，正确的设置步骤可以让流程容易读写."); //使用alert的方式显示帮助信息.
                map.AddTBString(NodeAttr.FK_Flow, null, "流程编号", false, false, 3, 3, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=17023");

                map.AddTBString(NodeAttr.Name, null, "名称", true, true, 0, 100, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=17903");
                map.AddTBString(NodeAttr.Tip, null, "操作提示", true, false, 0, 100, 10, false, "http://ccbpm.mydoc.io/?v=5404&t=18084");

                map.AddDDLSysEnum(NodeAttr.WhoExeIt, 0, "谁执行它", true, true, NodeAttr.WhoExeIt,
                    "@0=操作员执行@1=机器执行@2=混合执行");
                map.SetHelperUrl(NodeAttr.WhoExeIt, "http://ccbpm.mydoc.io/?v=5404&t=17913");

                //map.AddDDLSysEnum(NodeAttr.TurnToDeal, 0, "发送后转向",
                //true, true, NodeAttr.TurnToDeal, "@0=提示ccflow默认信息@1=提示指定信息@2=转向指定的url@3=按照条件转向");
                //map.SetHelperUrl(NodeAttr.TurnToDeal, "http://ccbpm.mydoc.io/?v=5404&t=17914");
                //map.AddTBString(NodeAttr.TurnToDealDoc, null, "转向处理内容", true, false, 0, 1000, 10, true, "http://ccbpm.mydoc.io/?v=5404&t=17914");

                map.AddDDLSysEnum(NodeAttr.ReadReceipts, 0, "已读回执", true, true, NodeAttr.ReadReceipts,
                "@0=不回执@1=自动回执@2=由上一节点表单字段决定@3=由SDK开发者参数决定");
                map.SetHelperUrl(NodeAttr.ReadReceipts, "http://ccbpm.mydoc.io/?v=5404&t=17915");

                //map.AddDDLSysEnum(NodeAttr.CondModel, 0, "方向条件控制规则", true, true, NodeAttr.CondModel,
                //"@0=由连接线条件控制@1=让用户手工选择@2=发送按钮旁下拉框选择");

                map.AddDDLSysEnum(NodeAttr.CondModel, 0, "方向条件控制规则", true, true, NodeAttr.CondModel, "@0=由连接线条件控制@2=发送按钮旁下拉框选择");
                map.SetHelperUrl(NodeAttr.CondModel, "http://ccbpm.mydoc.io/?v=5404&t=17917"); //增加帮助

                // 撤销规则.
                map.AddDDLSysEnum(NodeAttr.CancelRole, (int)CancelRole.OnlyNextStep, "撤销规则", true, true,
                    NodeAttr.CancelRole, "@0=上一步可以撤销@1=不能撤销@2=上一步与开始节点可以撤销@3=指定的节点可以撤销");
                map.SetHelperUrl(NodeAttr.CancelRole, "http://ccbpm.mydoc.io/?v=5404&t=17919");

                map.AddBoolean(NodeAttr.CancelDisWhenRead, false, "对方已经打开就不能撤销", true, true);

                // 节点工作批处理. edit by peng, 2014-01-24.    by huangzhimin 采用功能专题方式，移至左侧列表
                //map.AddDDLSysEnum(NodeAttr.BatchRole, (int)BatchRole.None, "工作批处理", true, true, NodeAttr.BatchRole, "@0=不可以批处理@1=批量审核@2=分组批量审核");
                //map.AddTBInt(NodeAttr.BatchListCount, 12, "批处理数量", true, false);
                ////map.SetHelperUrl(NodeAttr.BatchRole, this[SYS_CCFLOW, "节点工作批处理"]); //增加帮助
                //map.SetHelperUrl(NodeAttr.BatchRole, "http://ccbpm.mydoc.io/?v=5404&t=17920");
                //map.SetHelperUrl(NodeAttr.BatchListCount, "http://ccbpm.mydoc.io/?v=5404&t=17920");
                //map.AddTBString(NodeAttr.BatchParas, null, "批处理参数", true, false, 0, 300, 10, true);
                //map.SetHelperUrl(NodeAttr.BatchParas, "http://ccbpm.mydoc.io/?v=5404&t=17920");


                map.AddBoolean(NodeAttr.IsTask, true, "允许分配工作否?", true, true, false, "http://ccbpm.mydoc.io/?v=5404&t=17904");
                map.AddBoolean(NodeAttr.IsRM, true, "是否启用投递路径自动记忆功能?", true, true, true, "http://ccbpm.mydoc.io/?v=5404&t=17905");

                map.AddTBDateTime("DTFrom", "生命周期从", true, true);
                map.AddTBDateTime("DTTo", "生命周期到", true, true);

                map.AddBoolean(NodeAttr.IsBUnit, false, "是否是节点模版（业务单元）?", true, true, true, "http://ccbpm.mydoc.io/?v=5404&t=17904");

                map.AddTBString(NodeAttr.FocusField, null, "焦点字段", true, false, 0, 50, 10, true, "http://ccbpm.mydoc.io/?v=5404&t=17932");

                //map.AddDDLSysEnum(NodeAttr.SaveModel, 0, "保存方式", true, true);
                //map.SetHelperUrl(NodeAttr.SaveModel, "http://ccbpm.mydoc.io/?v=5404&t=17934");

                map.AddBoolean(NodeAttr.IsGuestNode, false, "是否是外部用户执行的节点(非组织结构人员参与处理工作的节点)?", true, true, true);

                //节点业务类型.
                map.AddTBInt("NodeAppType", 0, "节点业务类型", false, false);
                map.AddTBInt("FWCSta", 0, "节点状态", false, false);

                ////为宝旺达，增加业务类型.
                //if ( this.PKVal!=null )
                //{
                //    int nodeid = int.Parse(this.PKVal.ToString());
                //    if (nodeid != 0)
                //    {
                //        Node nd = new Node(nodeid);
                //        Flow fl = new Flow(nd.FK_Flow);

                //        string nodeAppType = fl.GetValStrByKey("NodeAppType");
                //        map.AddDDLSysEnum("NodeAppType", 0, "节点业务类型", true, true, nodeAppType);
                //    }
                //    // map.AddTBString("NodeAppType", null, "业务类型枚举", true, false, 0, 50, 10, true);
                //}
                map.AddTBString(NodeAttr.SelfParas, null, "自定义参数", true, false, 0, 500, 10, true);
                #endregion  基础属性

                #region 分合流子线程属性
                map.AddDDLSysEnum(NodeAttr.RunModel, 0, "节点类型",
                    true, false, NodeAttr.RunModel, "@0=普通@1=合流@2=分流@3=分合流@4=子线程");

                map.SetHelperUrl(NodeAttr.RunModel, "http://ccbpm.mydoc.io/?v=5404&t=17940"); //增加帮助.

                //子线程类型.
                map.AddDDLSysEnum(NodeAttr.SubThreadType, 0, "子线程类型", true, false, NodeAttr.SubThreadType, "@0=同表单@1=异表单");
                map.SetHelperUrl(NodeAttr.SubThreadType, "http://ccbpm.mydoc.io/?v=5404&t=17944"); //增加帮助

                map.AddTBDecimal(NodeAttr.PassRate, 100, "完成通过率", true, false);
                map.SetHelperUrl(NodeAttr.PassRate, "http://ccbpm.mydoc.io/?v=5404&t=17945"); //增加帮助.

                // 启动子线程参数 2013-01-04
                map.AddDDLSysEnum(NodeAttr.SubFlowStartWay, (int)SubFlowStartWay.None, "子线程启动方式", true, true,
                    NodeAttr.SubFlowStartWay, "@0=不启动@1=指定的字段启动@2=按明细表启动");
                map.AddTBString(NodeAttr.SubFlowStartParas, null, "启动参数", true, false, 0, 100, 10, true);
                map.SetHelperUrl(NodeAttr.SubFlowStartWay, "http://ccbpm.mydoc.io/?v=5404&t=17946"); //增加帮助

                //增加对退回到合流节点的 子线城的处理控制.
                map.AddBoolean(BtnAttr.ThreadIsCanDel, false, "是否可以删除子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效，在子线程退回后的操作)？", true, true, true);
                map.AddBoolean(BtnAttr.ThreadIsCanShift, false, "是否可以移交子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效，在子线程退回后的操作)？", true, true, true);

                ////待办处理模式.
                //map.AddDDLSysEnum(NodeAttr.TodolistModel, (int)TodolistModel.QiangBan, "多人待办处理模式", true, true, NodeAttr.TodolistModel,
                //    "@0=抢办模式@1=协作模式@2=队列模式@3=共享模式@4=协作组长模式");
                //map.SetHelperUrl(NodeAttr.TodolistModel, "http://ccbpm.mydoc.io/?v=5404&t=17947"); //增加帮助.

                //发送阻塞模式.
                //map.AddDDLSysEnum(NodeAttr.BlockModel, (int)BlockModel.None, "发送阻塞模式", true, true, NodeAttr.BlockModel,
                //    "@0=不阻塞@1=当前节点有未完成的子流程时@2=按约定格式阻塞未完成子流程@3=按照SQL阻塞@4=按照表达式阻塞");
                //map.SetHelperUrl(NodeAttr.BlockModel, "http://ccbpm.mydoc.io/?v=5404&t=17948"); //增加帮助.

                //map.AddTBString(NodeAttr.BlockExp, null, "阻塞表达式", true, false, 0, 700, 10,true);
                //map.SetHelperUrl(NodeAttr.BlockExp, "http://ccbpm.mydoc.io/?v=5404&t=17948");

                //map.AddTBString(NodeAttr.BlockAlert, null, "被阻塞时提示信息", true, false, 0, 700, 10, true);
                //map.SetHelperUrl(NodeAttr.BlockAlert, "http://ccbpm.mydoc.io/?v=5404&t=17948");

                map.AddBoolean(NodeAttr.IsAllowRepeatEmps, false, "是否允许子线程接受人员重复(仅当分流点向子线程发送时有效)?", true, true, true);

                map.AddBoolean(NodeAttr.AutoRunEnable, false, "是否启用自动运行？(仅当分流点向子线程发送时有效)", true, true, true);
                map.AddTBString(NodeAttr.AutoRunParas, null, "自动运行SQL", true, false, 0, 100, 10, true);
                #endregion 分合流子线程属性

                #region 自动跳转规则
                map.AddBoolean(NodeAttr.AutoJumpRole0, false, "处理人就是发起人", true, true, true);
                map.SetHelperUrl(NodeAttr.AutoJumpRole0, "http://ccbpm.mydoc.io/?v=5404&t=17949"); //增加帮助

                map.AddBoolean(NodeAttr.AutoJumpRole1, false, "处理人已经出现过", true, true, true);
                map.AddBoolean(NodeAttr.AutoJumpRole2, false, "处理人与上一步相同", true, true, true);
                map.AddBoolean(NodeAttr.WhenNoWorker, false, "(是)找不到人就跳转,(否)提示错误.", true, true, true);
                //         map.AddDDLSysEnum(NodeAttr.WhenNoWorker, 0, "找不到处理人处理规则",
                //true, true, NodeAttr.WhenNoWorker, "@0=提示错误@1=自动转到下一步");
                #endregion

                #region  功能按钮状态
                map.AddTBString(BtnAttr.SendLab, "发送", "发送按钮标签", true, false, 0, 50, 10);
                map.SetHelperUrl(BtnAttr.SendLab, "http://ccbpm.mydoc.io/?v=5404&t=16219");
                map.AddTBString(BtnAttr.SendJS, "", "按钮JS函数", true, false, 0, 999, 10);
                //map.SetHelperBaidu(BtnAttr.SendJS, "ccflow 发送前数据完整性判断"); //增加帮助.
                map.SetHelperUrl(BtnAttr.SendJS, "http://ccbpm.mydoc.io/?v=5404&t=17967");

                map.AddTBString(BtnAttr.SaveLab, "保存", "保存按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SaveEnable, true, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.SaveLab, "http://ccbpm.mydoc.io/?v=5404&t=24366"); //增加帮助

                map.AddTBString(BtnAttr.ThreadLab, "子线程", "子线程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ThreadEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.ThreadLab, "http://ccbpm.mydoc.io/?v=5404&t=16263"); //增加帮助

                map.AddDDLSysEnum(NodeAttr.ThreadKillRole, (int)ThreadKillRole.None, "子线程删除方式", true, true,
           NodeAttr.ThreadKillRole, "@0=不能删除@1=手工删除@2=自动删除", true);

                //map.SetHelperUrl(NodeAttr.ThreadKillRole, ""); //增加帮助

                //功能和子流程组件重复，屏蔽 hzm
                //  map.AddTBString(BtnAttr.SubFlowLab, "子流程", "子流程按钮标签", true, false, 0, 50, 10);
                //  map.SetHelperUrl(BtnAttr.SubFlowLab, "http://ccbpm.mydoc.io/?v=5404&t=16262");
                //   map.AddBoolean(BtnAttr.SubFlowEnable, false, "是否启用", true, true);

                //map.AddDDLSysEnum(BtnAttr.SubFlowCtrlRole, 0, "控制规则", true, true, BtnAttr.SubFlowCtrlRole, "@0=无@1=不可以删除子流程@2=可以删除子流程");

                map.AddTBString(BtnAttr.JumpWayLab, "跳转", "跳转按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.JumpWay, 0, "跳转规则", true, true, NodeAttr.JumpWay);
                map.AddTBString(NodeAttr.JumpToNodes, null, "可跳转的节点", true, false, 0, 200, 10, true);
                map.SetHelperUrl(NodeAttr.JumpWay, "http://ccbpm.mydoc.io/?v=5404&t=16261"); //增加帮助.

                map.AddTBString(BtnAttr.ReturnLab, "退回", "退回按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.ReturnRole, 0, "退回规则", true, true, NodeAttr.ReturnRole);
                //  map.AddTBString(NodeAttr.ReturnToNodes, null, "可退回节点", true, false, 0, 200, 10, true);
                map.SetHelperUrl(NodeAttr.ReturnRole, "http://ccbpm.mydoc.io/?v=5404&t=16255"); //增加帮助.

                map.AddTBString(NodeAttr.ReturnAlert, null, "被退回后信息提示", true, false, 0, 999, 10, true);


                map.AddBoolean(NodeAttr.IsBackTracking, true, "是否可以原路返回(启用退回功能才有效)", true, true, false);
                map.AddTBString(BtnAttr.ReturnField, "", "退回信息填写字段", true, false, 0, 50, 10);
                map.SetHelperUrl(NodeAttr.IsBackTracking, "http://ccbpm.mydoc.io/?v=5404&t=16255"); //增加帮助.

                map.AddTBString(NodeAttr.ReturnReasonsItems, null, "退回原因", true, false, 0, 999, 10, true);

                map.AddTBString(BtnAttr.CCLab, "抄送", "抄送按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(NodeAttr.CCRole, 0, "抄送规则", true, true, NodeAttr.CCRole,
                    "@@0=不能抄送@1=手工抄送@2=自动抄送@3=手工与自动@4=按表单SysCCEmps字段计算@5=在发送前打开抄送窗口");
                map.SetHelperUrl(BtnAttr.CCLab, "http://ccbpm.mydoc.io/?v=5404&t=16259"); //增加帮助.

                // add 2014-04-05.
                map.AddDDLSysEnum(NodeAttr.CCWriteTo, 0, "抄送写入规则",
             true, true, NodeAttr.CCWriteTo, "@0=写入抄送列表@1=写入待办@2=写入待办与抄送列表", true);
                map.SetHelperUrl(NodeAttr.CCWriteTo, "http://ccbpm.mydoc.io/?v=5404&t=17976"); //增加帮助.

                map.AddTBString(BtnAttr.ShiftLab, "移交", "移交按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ShiftEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.ShiftLab, "http://ccbpm.mydoc.io/?v=5404&t=16257"); //增加帮助.note:none

                map.AddTBString(BtnAttr.DelLab, "删除", "删除按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.DelEnable, 0, "删除规则", true, true, BtnAttr.DelEnable);
                map.SetHelperUrl(BtnAttr.DelLab, "http://ccbpm.mydoc.io/?v=5404&t=17992"); //增加帮助.

                map.AddTBString(BtnAttr.EndFlowLab, "结束流程", "结束流程按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.EndFlowEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.EndFlowLab, "http://ccbpm.mydoc.io/?v=5404&t=17989"); //增加帮助

                // add 2019.1.9 for 东孚.
                map.AddTBString(BtnAttr.OfficeBtnLab, "打开公文", "公文按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.OfficeBtnEnable, false, "是否启用", true, true);


                // add 2017.9.1 for 天业集团.
                map.AddTBString(BtnAttr.PrintHtmlLab, "打印Html", "打印Html标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintHtmlEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.PrintPDFLab, "打印pdf", "打印pdf标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintPDFEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.PrintZipLab, "打包下载", "打包下载zip按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PrintZipEnable, false, "是否启用", true, true);

                map.AddTBString(BtnAttr.PrintDocLab, "打印单据", "打印单据按钮标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.PrintDocEnable, 0, "打印方式", true,
                    true, BtnAttr.PrintDocEnable, "@0=不打印@1=打印网页@2=打印RTF模板@3=打印Word模版");
                map.SetHelperUrl(BtnAttr.PrintDocEnable, "http://ccbpm.mydoc.io/?v=5404&t=17979"); //增加帮助

                // map.AddBoolean(BtnAttr.PrintDocEnable, false, "是否启用", true, true);
                //map.AddTBString(BtnAttr.AthLab, "附件", "附件按钮标签", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(NodeAttr.FJOpen, 0, this.ToE("FJOpen", "附件权限"), true, true, 
                //    NodeAttr.FJOpen, "@0=关闭附件@1=操作员@2=工作ID@3=流程ID");

                map.AddTBString(BtnAttr.TrackLab, "轨迹", "轨迹按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TrackEnable, true, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.TrackLab, this[SYS_CCFLOW, "轨迹"]); //增加帮助
                map.SetHelperUrl(BtnAttr.TrackLab, "http://ccbpm.mydoc.io/?v=5404&t=24369");

                map.AddTBString(BtnAttr.HungLab, "挂起", "挂起按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.HungEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.HungLab, "http://ccbpm.mydoc.io/?v=5404&t=16267"); //增加帮助.

                //      map.AddTBString(BtnAttr.SelectAccepterLab, "接受人", "接受人按钮标签", true, false, 0, 50, 10);
                //      map.AddDDLSysEnum(BtnAttr.SelectAccepterEnable, 0, "工作方式",
                //true, true, BtnAttr.SelectAccepterEnable);
                //      map.SetHelperUrl(BtnAttr.SelectAccepterLab, "http://ccbpm.mydoc.io/?v=5404&t=16256"); //增加帮助


                map.AddTBString(BtnAttr.SearchLab, "查询", "查询按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.SearchEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.SearchLab, this[SYS_CCFLOW, "查询"]); //增加帮助
                map.SetHelperUrl(BtnAttr.SearchLab, "http://ccbpm.mydoc.io/?v=5404&t=24373");

                map.AddTBString(BtnAttr.WorkCheckLab, "审核", "审核按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.WorkCheckEnable, false, "是否启用", true, true);

                //map.AddTBString(BtnAttr.BatchLab, "批处理", "批处理按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.BatchEnable, false, "是否启用", true, true);
                //map.SetHelperUrl(BtnAttr.BatchLab, "http://ccbpm.mydoc.io/?v=5404&t=17920"); //增加帮助

                map.AddTBString(BtnAttr.AskforLab, "加签", "加签按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AskforEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.AskforLab, "http://ccbpm.mydoc.io/?v=5404&t=16258");


                map.AddTBString(BtnAttr.HuiQianLab, "会签", "会签标签", true, false, 0, 50, 10);
                map.AddDDLSysEnum(BtnAttr.HuiQianRole, 0, "会签模式", true, true, BtnAttr.HuiQianRole, "@0=不启用@1=协作(同事)模式@4=组长(领导)模式");


                // add by 周朋 2014-11-21. 让用户可以自己定义流转.
                map.AddTBString(BtnAttr.TCLab, "流转自定义", "流转自定义", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.TCEnable, false, "是否启用", true, true);
                map.SetHelperUrl(BtnAttr.TCEnable, "http://ccbpm.mydoc.io/?v=5404&t=17978");

                //map.AddTBString(BtnAttr.AskforLabRe, "执行", "加签按钮标签", true, false, 0, 50, 10);
                //map.AddBoolean(BtnAttr.AskforEnable, false, "是否启用", true, true);

                // map.SetHelperUrl(BtnAttr.AskforLab, this[SYS_CCFLOW, "加签"]); //增加帮助


                // 删除了这个模式,让表单方案进行控制了,保留这两个字段以兼容.
                map.AddTBString(BtnAttr.WebOfficeLab, "公文", "文档按钮标签", false, false, 0, 50, 10);
                map.AddTBInt(BtnAttr.WebOfficeEnable, 0, "文档启用方式", false, false);

                //cut bye zhoupeng.
                //map.AddTBString(BtnAttr.WebOfficeLab, "公文", "文档按钮标签", true, false, 0, 50, 10);
                //map.AddDDLSysEnum(BtnAttr.WebOfficeEnable, 0, "文档启用方式", true, true, BtnAttr.WebOfficeEnable,
                //  "@0=不启用@1=按钮方式@2=标签页置后方式@3=标签页置前方式");//edited by liuxc,2016-01-18,from xc
                //map.SetHelperUrl(BtnAttr.WebOfficeLab, "http://ccbpm.mydoc.io/?v=5404&t=17993");

                // add by 周朋 2015-08-06. 重要性.
                map.AddTBString(BtnAttr.PRILab, "重要性", "重要性", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.PRIEnable, false, "是否启用", true, true);

                // add by 周朋 2015-08-06. 节点时限.
                map.AddTBString(BtnAttr.CHLab, "节点时限", "节点时限", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.CHEnable, false, "是否启用", true, true);

                // add 2017.5.4  邀请其他人参与当前的工作.
                map.AddTBString(BtnAttr.AllotLab, "分配", "分配按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.AllotEnable, false, "是否启用", true, true);

                // add by 周朋 2015-12-24. 节点时限.
                map.AddTBString(BtnAttr.FocusLab, "关注", "关注", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.FocusEnable, false, "是否启用", true, true);

                // add 2017.5.4 确认就是告诉发送人，我接受这件工作了.
                map.AddTBString(BtnAttr.ConfirmLab, "确认", "确认按钮标签", true, false, 0, 50, 10);
                map.AddBoolean(BtnAttr.ConfirmEnable, false, "是否启用", true, true);

                //map.AddBoolean(BtnAttr.SelectAccepterEnable, false, "是否启用", true, true);
                #endregion  功能按钮状态


                //节点工具栏,主从表映射.
                map.AddDtl(new NodeToolbars(), NodeToolbarAttr.FK_Node);

                ////延续子流程.
                //map.AddDtl(new NodeYGFlows(), NodeToolbarAttr.FK_Node);

                #region 基础功能.
                RefMethod rm = null;

                rm = new RefMethod();
                rm.Title = "接收人规则";
                rm.Icon = "../../WF/Admin/AttrNode/Img/Sender.png";
                rm.ClassMethodName = this.ToString() + ".DoAccepterRoleNew";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "节点事件"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                rm.Icon = "../../WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "节点消息"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoMessage";
                rm.Icon = "../../WF/Img/Message24.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);



                rm = new RefMethod();
                rm.Title = "流程完成条件"; // "流程完成条件";
                rm.ClassMethodName = this.ToString() + ".DoCond";
                rm.Icon = "../../WF/Admin/AttrNode/Img/Cond.png";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Menu/Cond.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "发送后转向"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoTurnToDeal";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Turnto.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "发送阻塞规则";
                rm.ClassMethodName = this.ToString() + ".DoBlockModel";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/BlockModel.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "多人处理规则";
                rm.ClassMethodName = this.ToString() + ".DoTodolistModel";
                rm.Icon = "../../WF/Img/Multiplayer.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                #endregion 基础功能.

                #region 字段相关功能（不显示在菜单里）
                rm = new RefMethod();
                rm.Title = "可退回的节点(当退回规则设置可退回指定的节点时,该设置有效.)"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoCanReturnNodes";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.ReturnRole;
                rm.RefAttrLinkLabel = "设置可退回的节点";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "可撤销的节点"; // "可撤销发送的节点";
                rm.ClassMethodName = this.ToString() + ".DoCanCancelNodes";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;

                //设置相关字段.
                rm.RefAttrKey = NodeAttr.CancelRole;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "绑定rtf打印格式模版(当打印方式为打印RTF格式模版时,该设置有效)"; //"单据&单据";
                rm.ClassMethodName = this.ToString() + ".DoBill";
                rm.Icon = "../../WF/Img/FileType/doc.gif";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;

                //设置相关字段.
                rm.RefAttrKey = NodeAttr.PrintDocEnable;
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                if (BP.Sys.SystemConfig.CustomerNo == "HCBD")
                {
                    /* 为海成邦达设置的个性化需求. */
                    rm = new RefMethod();
                    rm.Title = "DXReport设置";
                    rm.ClassMethodName = this.ToString() + ".DXReport";
                    rm.Icon = "../../WF/Img/FileType/doc.gif";
                    map.AddRefMethod(rm);
                }

                rm = new RefMethod();
                rm.Title = "设置自动抄送规则(当节点为自动抄送时,该设置有效.)"; // "抄送规则";
                rm.ClassMethodName = this.ToString() + ".DoCCRole";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.CCRole;
                rm.RefAttrLinkLabel = "自动抄送设置";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                #endregion 字段相关功能（不显示在菜单里）

                #region 表单设置.
                rm = new RefMethod();
                rm.Title = "表单方案";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/Form.png";
                rm.ClassMethodName = this.ToString() + ".DoSheet";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "表单设置";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "手机表单字段顺序";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/telephone.png";
                //rm.Icon = ../../Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoSortingMapAttrs";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "表单设置";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "节点组件";
                rm.Icon = "../../WF/Img/Components.png";
                //rm.Icon = ../../Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoFrmNodeComponent";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "表单设置";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "特别控件特别用户权限";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/SpecUserSpecFields.png";
                rm.ClassMethodName = this.ToString() + ".DoSpecFieldsSpecUsers()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "表单设置";
                map.AddRefMethod(rm);
                #endregion 表单设置.

                #region 父子流程.

                rm = new RefMethod();
                rm.Title = "父子流程";
                rm.Icon = "../../WF/Admin/AttrNode/Img/SubFlows.png";
                rm.ClassMethodName = this.ToString() + ".DoSubFlow";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "父子流程";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "延续子流程"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoYGFlows";
                //  rm.Icon = "../../WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "父子流程";
                map.AddRefMethod(rm);
                #endregion 父子流程.


                #region 考核.

                rm = new RefMethod();
                rm.Title = "设置考核规则";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/CH.png";
                rm.ClassMethodName = this.ToString() + ".DoCHRole";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "考核规则";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "超时处理规则";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/OvertimeRole.png";
                rm.ClassMethodName = this.ToString() + ".DoCHOvertimeRole";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "考核规则";
                map.AddRefMethod(rm);
                #endregion 考核.

                #region 实验中的功能
                rm = new RefMethod();
                rm.Title = "设置节点类型";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoNodeAppType()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                rm.Visable = false;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "批量设置节点属性";
                rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.ClassMethodName = this.ToString() + ".DoNodeAttrs()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                rm.Visable = false;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "设置独立表单树权限";
                //rm.Icon = ../../Img/Btn/DTS.gif";
                //rm.ClassMethodName = this.ToString() + ".DoNodeFormTree";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "实验中的功能";
                //map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "工作批处理规则";
                rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.ClassMethodName = this.ToString() + ".DoBatchStartFields()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "抄送人规则";
                rm.GroupName = "实验中的功能";
                rm.Icon = "../../WF/Admin/AttrNode/Img/CC.png";
                rm.ClassMethodName = this.ToString() + ".DoCCer";  //要执行的方法名.
                rm.RefMethodType = RefMethodType.RightFrameOpen; // 功能类型
                map.AddRefMethod(rm);

                #endregion 实验中的功能


                this._enMap = map;
                return this._enMap;
            }
        }

        #region 考核规则.
        /// <summary>
        /// 考核规则
        /// </summary>
        /// <returns></returns>
        public string DoCHRole()
        {
            return "../../Admin/AttrNode/CHRole.htm?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 超时处理规则
        /// </summary>
        /// <returns></returns>
        public string DoCHOvertimeRole()
        {
            return "../../Admin/AttrNode/CHOvertimeRole.htm?FK_Node=" + this.NodeID;
        }
        #endregion 考核规则.

        #region 基础设置.
        /// <summary>
        /// 多人处理规则.
        /// </summary>
        /// <returns></returns>
        public string DoTodolistModel()
        {
            return "../../Admin/AttrNode/TodolistModel.htm?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 批处理规则
        /// </summary>
        /// <returns></returns>
        public string DoBatchStartFields()
        {
            return "../../Admin/AttrNode/BatchStartFields.htm?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 批量修改节点属性
        /// </summary>
        /// <returns></returns>
        public string DoNodeAttrs()
        {
            return "../../Admin/AttrFlow/NodeAttrs.htm?NodeID=0&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 表单方案
        /// </summary>
        /// <returns></returns>
        public string DoSheet()
        {
            return "../../Admin/AttrNode/FrmSln/Default.htm?FK_Node=" + this.NodeID;
        }
        public string DoSheetOld()
        {
            return "../../Admin/AttrNode/NodeFromWorkModel.htm?FK_Node=" + this.NodeID;
        }

        /// <summary>
        /// 父子流程
        /// </summary>
        /// <returns></returns>
        public string DoSubFlow()
        {
            return "../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.FrmSubFlow&PK=" + this.NodeID;
        }
        /// <summary>
        /// 接受人规则
        /// </summary>
        /// <returns></returns>
        public string DoAccepterRoleNew()
        {
            return "../../Admin/AttrNode/AccepterRole/Default.htm?FK_Node=" + this.NodeID;
        }

        /// <summary>
        /// 发送阻塞规则
        /// </summary>
        /// <returns></returns>
        public string DoBlockModel()
        {
            return "../../Admin/AttrNode/BlockModel.htm?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 发送后转向规则
        /// </summary>
        /// <returns></returns>
        public string DoTurnToDeal()
        {
            return "../../Admin/AttrNode/TurnToDeal.htm?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 抄送人规则
        /// </summary>
        /// <returns></returns>
        public string DoCCer()
        {
            return "../../Admin/AttrNode/CCRole.htm?FK_Node=" + this.NodeID;
        }

        #endregion

        #region 表单相关.
        /// <summary>
        /// 节点组件
        /// </summary>
        /// <returns></returns>
        public string DoFrmNodeComponent()
        {
            return "../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PKVal=" + this.NodeID + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 特别用户特殊字段权限.
        /// </summary>
        /// <returns></returns>
        public string DoSpecFieldsSpecUsers()
        {
            return "../../Admin/AttrNode/SepcFiledsSepcUsers.htm?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&FK_Node=" + this.NodeID + "&t=" + DataType.CurrentDataTime;
        }
        /// <summary>
        /// 排序字段顺序
        /// </summary>
        /// <returns></returns>
        public string DoSortingMapAttrs()
        {
            return "../../Admin/AttrNode/SortingMapAttrs.htm?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&t=" + DataType.CurrentDataTime;
        }
        #endregion 表单相关.

        #region 实验中的功能.
        /// <summary>
        /// 设置节点类型
        /// </summary>
        /// <returns></returns>
        public string DoNodeAppType()
        {
            return "../../Admin/AttrNode/NodeAppType.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        #endregion

        /// <summary>
        /// 延续子流程
        /// </summary>
        /// <returns></returns>
        public string DoYGFlows()
        {
            return "../../Admin/AttrNode/NodeYGFlow.htm?FK_Node=" + this.NodeID + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 集团部门树
        /// </summary>
        /// <returns></returns>
        public string DoDepts()
        {
            PubClass.WinOpen("../../Comm/Port/DeptTree.aspx?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&RefNo=" + DataType.CurrentDataTime, 500, 550);
            return null;
        }

        /// <summary>
        /// 制度
        /// </summary>
        /// <returns></returns>
        public string DoZhiDu()
        {
            PubClass.WinOpen(BP.WF.Glo.CCFlowAppPath + "ZhiDu/NodeZhiDuDtl.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "制度", "Bill", 700, 400, 200, 300);
            return null;
        }
        /// <summary>
        /// 风险点
        /// </summary>
        /// <returns></returns>
        public string DoFengXianDian()
        {
            PubClass.WinOpen(BP.WF.Glo.CCFlowAppPath + "ZhiDu/NodeFengXianDian.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "制度", "Bill", 700, 400, 200, 300);
            return null;
        }

        public string DoTurn()
        {
            return "../../Admin/AttrNode/TurnTo.htm?FK_Node=" + this.NodeID;
            //, "节点完成转向处理", "FrmTurn", 800, 500, 200, 300);
            //BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            //return nd.DoTurn();
        }
        /// <summary>
        /// 抄送规则
        /// </summary>
        /// <returns></returns>
        public string DoCCRole()
        {
            return "../../Comm/En.htm?EnName=BP.WF.Template.CC&PKVal=" + this.NodeID;
        }
        /// <summary>
        /// 个性化接受人窗口
        /// </summary>
        /// <returns></returns>
        public string DoAccepter()
        {
            return "../../Comm/En.htm?EnName=BP.WF.Template.Selector&PK=" + this.NodeID;
        }
        /// <summary>
        /// 可触发的子流程
        /// </summary>
        /// <returns></returns>
        public string DoActiveFlows()
        {
            return "../../Admin/ConditionSubFlow.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 退回节点
        /// </summary>
        /// <returns></returns>
        public string DoCanReturnNodes()
        {
            return "../../Admin/AttrNode/CanReturnNodes.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 撤销发送的节点
        /// </summary>
        /// <returns></returns>
        public string DoCanCancelNodes()
        {
            return "../../Admin/AttrNode/CanCancelNodes.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// DXReport
        /// </summary>
        /// <returns></returns>
        public string DXReport()
        {
            return "../../Admin/DXReport.aspx?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }

        public string DoCond()
        {
            return "../../Admin/Cond/List.htm?CondType=" + (int)CondType.Flow + "&FK_Flow=" + this.FK_Flow + "&FK_MainNode=" + this.NodeID + "&FK_Node=" + this.NodeID + "&FK_Attr=&DirType=&ToNodeID=" + this.NodeID;
        }
        /// <summary>
        /// 设计傻瓜表单
        /// </summary>
        /// <returns></returns>
        public string DoFormCol4()
        {
            return "../../Admin/FoolFormDesigner/Designer.htm?PK=ND" + this.NodeID;
        }
        /// <summary>
        /// 设计自由表单
        /// </summary>
        /// <returns></returns>
        public string DoFormFree()
        {
            return "../../Admin/FoolFormDesigner/CCForm/Frm.htm?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 绑定独立表单
        /// </summary>
        /// <returns></returns>
        public string DoFormTree()
        {
            return "../../Admin/Sln/BindFrms.htm?ShowType=FlowFrms&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&Lang=CH";
        }

        public string DoMapData()
        {
            int i = this.GetValIntByKey(NodeAttr.FormType);

            // 类型.
            NodeFormType type = (NodeFormType)i;
            switch (type)
            {
                case NodeFormType.FreeForm:
                    PubClass.WinOpen("../../Admin/FoolFormDesigner/CCForm/Frm.htm?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow, "设计表单", "sheet", 1024, 768, 0, 0);
                    break;
                default:
                case NodeFormType.FoolForm:
                    PubClass.WinOpen("../../Admin/FoolFormDesigner/Designer.htm?PK=ND" + this.NodeID, "设计表单", "sheet", 800, 500, 210, 300);
                    break;
            }
            return null;
        }

        /// <summary>
        /// 消息
        /// </summary>
        /// <returns></returns>
        public string DoMessage()
        {
            return "../../Admin/AttrNode/PushMessage.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 事件
        /// </summary>
        /// <returns></returns>
        public string DoAction()
        {
            return "../../Admin/AttrNode/Action.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 单据打印
        /// </summary>
        /// <returns></returns>
        public string DoBill()
        {
            return "../../Admin/AttrNode/Bill.htm?FK_Node=" + this.NodeID + "&NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID;
        }


        protected override bool beforeUpdate()
        {
            //更新流程版本
            Flow.UpdateVer(this.FK_Flow);

            #region 处理节点数据.
            Node nd = new Node(this.NodeID);
            if (nd.IsStartNode == true)
            {
                /*处理按钮的问题*/
                //不能退回, 加签，移交，退回, 子线程.
                //this.SetValByKey(BtnAttr.ReturnRole,(int)ReturnRole.CanNotReturn); //开始节点可以退回。
                this.SetValByKey(BtnAttr.HungEnable, false);
                this.SetValByKey(BtnAttr.ThreadEnable, false); //子线程.
            }

            if (nd.HisRunModel == RunModel.HL || nd.HisRunModel == RunModel.FHL)
            {
                /*如果是合流点*/
            }
            else
            {
                this.SetValByKey(BtnAttr.ThreadEnable, false); //子线程.
            }

            //如果启动了会签,并且是抢办模式,强制设置为队列模式.或者组长模式.
            if (this.HuiQianRole != WF.HuiQianRole.None)
            {
                if (this.HuiQianRole == WF.HuiQianRole.Teamup)
                    DBAccess.RunSQL("UPDATE WF_Node SET TodolistModel=" + (int)TodolistModel.Teamup + "  WHERE NodeID=" + this.NodeID);

                if (this.HuiQianRole == WF.HuiQianRole.TeamupGroupLeader)
                    DBAccess.RunSQL("UPDATE WF_Node SET TodolistModel=" + (int)TodolistModel.TeamupGroupLeader + ", TeamLeaderConfirmRole=" + (int)TeamLeaderConfirmRole.HuiQianLeader + " WHERE NodeID=" + this.NodeID);
            }

            // @杜. 翻译&测试.
            if (nd.CondModel == CondModel.ByLineCond)
            {
                /* 如果当前节点方向条件控制规则是按照连接线决定的, 
                 * 那就判断到达的节点的接受人规则，是否是按照上一步来选择，如果是就抛出异常.*/

                //获得到达的节点.
                Nodes nds = nd.HisToNodes;
                foreach (Node mynd in nds)
                {
                    if (mynd.HisDeliveryWay == DeliveryWay.BySelected)
                    {
                        string errInfo = "设置矛盾:";
                        errInfo += "@当前节点您设置的访问规则是按照方向条件控制的";
                        errInfo += "但是到达的节点[" + mynd.Name + "]的接收人规则是按照上一步选择的,设置矛盾.";
                        // throw new Exception(errInfo);
                    }
                }
            }

            //如果启用了在发送前打开, 当前节点的方向条件控制模式，是否是在下拉框边选择.?
            if (1 == 2 && nd.CondModel != CondModel.SendButtonSileSelect)
            {
                /*如果是启用了按钮，就检查当前节点到达的节点是否有【按照选择接受人】的方式确定接收人的范围. */
                Nodes nds = nd.HisToNodes;
                foreach (Node mynd in nds)
                {
                    if (mynd.HisDeliveryWay == DeliveryWay.BySelected)
                    {
                        //强制设置安装人员选择器来选择.
                        this.SetValByKey(NodeAttr.CondModel, (int)CondModel.SendButtonSileSelect);
                        break;
                    }
                }
            }
            #endregion 处理节点数据.


            #region 创建审核组件附件
            FrmAttachment workCheckAth = new FrmAttachment();
            workCheckAth.MyPK = this.NodeID + "_FrmWorkCheck";
            //不包含审核组件
            if (workCheckAth.RetrieveFromDBSources() == 0)
            {
                workCheckAth = new FrmAttachment();
                /*如果没有查询到它,就有可能是没有创建.*/
                workCheckAth.MyPK = this.NodeID + "_FrmWorkCheck";
                workCheckAth.FK_MapData = this.NodeID.ToString();
                workCheckAth.NoOfObj = this.NodeID + "_FrmWorkCheck";
                workCheckAth.Exts = "*.*";

                //存储路径.
                workCheckAth.SaveTo = "/DataUser/UploadFile/";
                workCheckAth.IsNote = false; //不显示note字段.
                workCheckAth.IsVisable = false; // 让其在form 上不可见.

                //位置.
                workCheckAth.X = (float)94.09;
                workCheckAth.Y = (float)333.18;
                workCheckAth.W = (float)626.36;
                workCheckAth.H = (float)150;

                //多附件.
                workCheckAth.UploadType = AttachmentUploadType.Multi;
                workCheckAth.Name = "审核组件";
                workCheckAth.SetValByKey("AtPara", "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=@IsWoEnableTemplete=1");
                workCheckAth.Insert();
            }
            #endregion 创建审核组件附件

            #region 审核组件.
            GroupField gf = new GroupField();
            if (this.HisFrmWorkCheckSta == FrmWorkCheckSta.Disable)
            {
                gf.Delete(GroupFieldAttr.FrmID, "ND" + this.NodeID, GroupFieldAttr.CtrlType, GroupCtrlType.FWC);
            }
            else
            {
                if (gf.IsExit(GroupFieldAttr.CtrlType, GroupCtrlType.FWC, GroupFieldAttr.FrmID, "ND" + this.NodeID) == false)
                {
                    gf = new GroupField();
                    gf.EnName = "ND" + this.NodeID;
                    gf.CtrlType = GroupCtrlType.FWC;
                    gf.Lab = "审核信息";
                    gf.Idx = 0;
                    gf.Insert(); //插入.
                }
            }
            #endregion 审核组件.


            //清除所有的缓存.
            BP.DA.CashEntity.DCash.Clear();

            return base.beforeUpdate();
        }
        #endregion
    }
    /// <summary>
    /// 节点集合
    /// </summary>
    public class NodeExts : Entities
    {
        #region 构造方法
        /// <summary>
        /// 节点集合
        /// </summary>
        public NodeExts()
        {
        }
        #endregion

        public override Entity GetNewEntity
        {
            get { return new NodeExt(); }
        }
    }
}
