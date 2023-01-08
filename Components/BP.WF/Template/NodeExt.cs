using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Collections;
using BP.Port;
using System.IO;
using BP.WF.Template.SFlow;
using BP.WF.Template.CCEn;

namespace BP.WF.Template
{
    /// <summary>
    /// 节点属性.
    /// </summary>
    public class NodeExt : Entity
    {
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
        public HuiQianLeaderRole HuiQianLeaderRole
        {
            get
            {
                return (HuiQianLeaderRole)this.GetValIntByKey(BtnAttr.HuiQianLeaderRole);
            }
            set
            {
                this.SetValByKey(BtnAttr.HuiQianLeaderRole, (int)value);
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

        public FWCAth FWCAth
        {
            get
            {
                return (FWCAth)this.GetValIntByKey(NodeWorkCheckAttr.FWCAth);
            }
            set
            {
                this.SetValByKey(NodeWorkCheckAttr.FWCAth, (int)value);
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
        public string DeliveryParas11
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
        public bool IsYouLiTai
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsYouLiTai);
            }
        }
        /// <summary>
        /// 是否是返回节点?
        /// </summary>
        public bool IsSendBackNode
        {
            get
            {
                return this.GetValBooleanByKey(NodeAttr.IsSendBackNode);
            }
            set
            {
                this.SetValByKey(NodeAttr.IsSendBackNode, value);
            }
        }

        public bool AddLeaderEnable
        {
            get
            {
                return this.GetValBooleanByKey(BtnAttr.AddLeaderEnable);
            }
            set
            {
                this.SetValByKey(BtnAttr.AddLeaderEnable, value);
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

                if (BP.Web.WebUser.IsAdmin == false)
                    throw new Exception("err@管理员登录用户信息丢失,当前会话[" + BP.Web.WebUser.No + "," + BP.Web.WebUser.Name + "]");

                uac.IsUpdate = true;
                uac.IsDelete = false;
                uac.IsInsert = false;

                //uac.OpenForAdmin();
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
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;
                map.IndexField = NodeAttr.FK_Flow;
                map.IsEnableVer = true; //启动日志.

                #region  基本信息
                map.AddGroupAttr("基本信息");
                map.AddTBIntPK(NodeAttr.NodeID, 0, "节点ID", true, true);
                // map.SetHelperUrl(NodeAttr.NodeID, "http://ccbpm.mydoc.io/?v=5404&t=17901");
                map.SetHelperUrl(NodeAttr.NodeID,
                    "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3576080&doc_id=31094");

                //map.SetHelperAlert(NodeAttr.Step, "它用于节点的排序，正确的设置步骤可以让流程容易读写."); //使用alert的方式显示帮助信息.
                map.AddTBString(NodeAttr.FK_Flow, null, "流程编号", false, false, 0, 5, 10);
                map.AddTBString(NodeAttr.FlowName, null, "流程名", false, true, 0, 200, 10);

                map.AddTBString(NodeAttr.Name, null, "名称", true, false, 0, 100, 10, false);
                map.SetHelperAlert(NodeAttr.Name, "修改节点名称时如果节点表单名称为空着节点表单名称和节点名称相同，否则节点名称和节点表单名称可以不相同");

                map.AddDDLSysEnum(NodeAttr.WhoExeIt, 0, "谁执行它", true, true, NodeAttr.WhoExeIt,
                    "@0=操作员执行@1=机器执行@2=混合执行");
                map.SetHelperUrl(NodeAttr.WhoExeIt, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3576195&doc_id=31094");

                map.AddDDLSysEnum(NodeAttr.ReadReceipts, 0, "已读回执", true, true, NodeAttr.ReadReceipts,
                "@0=不回执@1=自动回执@2=由上一节点表单字段决定@3=由SDK开发者参数决定");
                map.SetHelperUrl(NodeAttr.ReadReceipts,
                    "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3882411&doc_id=31094");

                //map.AddTBString(NodeAttr.DeliveryParas, null, "访问规则设置", true, false, 0, 300, 10);
                //map.AddDDLSysEnum(NodeAttr.CondModel, 0, "方向条件控制规则", true, true, NodeAttr.CondModel,
                //  "@0=由连接线条件控制@1=按照用户选择计算@2=发送按钮旁下拉框选择");
                //map.SetHelperUrl(NodeAttr.CondModel, "http://ccbpm.mydoc.io/?v=5404&t=17917"); //增加帮助

                // 撤销规则.
                map.AddDDLSysEnum(NodeAttr.CancelRole, (int)CancelRole.OnlyNextStep, "撤销规则", true, true,
                    NodeAttr.CancelRole, "@0=上一步可以撤销@1=不能撤销@2=上一步与开始节点可以撤销@3=指定的节点可以撤销");
                map.SetHelperUrl(NodeAttr.CancelRole, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3576276&doc_id=31094");
                map.AddBoolean(NodeAttr.CancelDisWhenRead, false, "对方已经打开就不能撤销", true, true);

                //map.AddBoolean(NodeAttr.IsTask, true, "允许分配工作否?", true, true, false, "http://ccbpm.mydoc.io/?v=5404&t=17904");
                //map.AddBoolean(NodeAttr.IsExpSender, true, "本节点接收人不允许包含上一步发送人", true, true, false);
                //map.AddBoolean(NodeAttr.IsRM, true, "是否启用投递路径自动记忆功能?", true, true, false, "http://ccbpm.mydoc.io/?v=5404&t=17905");
                map.AddBoolean(NodeAttr.IsOpenOver, false, "已阅即完成?", true, true, false);
                map.SetHelperUrl(NodeAttr.IsOpenOver, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3653663&doc_id=31094");

                //为铁路局,会签子流程. 增加 
                map.AddBoolean(NodeAttr.IsSendDraftSubFlow, false, "是否发送草稿子流程?", true, true, true);
                map.SetHelperAlert(NodeAttr.IsSendDraftSubFlow, "如果有启动的草稿子流程，是否发送它们？"); //增加帮助。

                map.AddBoolean(NodeAttr.IsResetAccepter, false, "可逆节点时重新计算接收人?", true, true, true);
                map.SetHelperAlert(NodeAttr.IsResetAccepter, "-所谓的可逆节点,Reversible Node, 就是双向箭头节点,可以重复执行的节点."
                 + "- 当一个节点，被运动了1次 +，它就是可逆节点, 因为它被重复发送了."
                 + "- 第一次按照接收人规则接收人有a, b, c三个人.如果在发送回来, 需要重新计算接收人就true,"
                 + "不需要重新计算接受人, 把当事人做为接收人就是 false."); //增加帮助。

                map.AddBoolean(NodeAttr.IsGuestNode, false,
                    "是否是外部用户执行的节点(非组织结构人员参与处理工作的节点)?", true, true, true,
                    "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661834&doc_id=31094");

                map.AddBoolean(NodeAttr.IsYouLiTai, false, "该节点是否是游离态", true, true);
                map.SetHelperUrl(NodeAttr.IsYouLiTai, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3653664&doc_id=31094");
                map.AddTBString(NodeAttr.FocusField, null, "焦点字段", true, false, 0, 50, 10, true,
                    "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3653665&doc_id=31094");

                //节点业务类型.
                // map.AddTBInt("NodeAppType", 0, "节点业务类型", false, false);
                map.AddTBInt("FWCSta", 0, "节点状态", false, false);
                map.AddTBInt("FWCAth", 0, "审核附件是否启用", false, false);

                //如果不设置，就不能模版导入导出.
                map.AddTBInt(NodeAttr.DeliveryWay, 0, "接受人规则", false, false);

                map.AddTBString(NodeAttr.SelfParas, null, "自定义属性", true, false, 0, 500, 10, true);
                map.SetHelperUrl(NodeAttr.SelfParas, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3653666&doc_id=31094");

                map.AddTBInt(NodeAttr.Step, 0, "步骤(无计算意义)", true, false);
                map.SetHelperUrl(NodeAttr.Step, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3576085&doc_id=31094");

                map.AddTBString(NodeAttr.Tip, null, "操作提示", true, false, 0, 100, 10, false,
                    "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3653667&doc_id=31094");
                #endregion  基础属性

                #region 运行模式
                map.AddGroupAttr("运行模式");
                map.AddDDLSysEnum(NodeAttr.RunModel, 0, "节点类型",
                    true, false, NodeAttr.RunModel, "@0=普通@1=合流@2=分流@3=分合流@4=同表单子线程@5=异表单子线程");
                map.SetHelperUrl(NodeAttr.RunModel, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661853&doc_id=31094"); //增加帮助.

                map.AddTBDecimal(NodeAttr.PassRate, 100, "完成通过率", true, false);
                map.SetHelperUrl(NodeAttr.PassRate, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661856&doc_id=31094"); //增加帮助.

                //增加对退回到合流节点的 子线城的处理控制.
                map.AddBoolean(BtnAttr.ThreadIsCanDel, true, "是否可以删除子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效，在子线程退回后的操作)？", true, true, true);
                map.AddBoolean(BtnAttr.ThreadIsCanAdd, true, "是否可以增加子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效)？", true, true, true);

                map.AddBoolean(BtnAttr.ThreadIsCanShift, false, "是否可以移交子线程(当前节点已经发送出去的线程，并且当前节点是分流，或者分合流有效，在子线程退回后的操作)？", true, true, true);

                map.AddDDLSysEnum(NodeAttr.USSWorkIDRole, 0, "异表单子线程WorkID生成规则", true, true, NodeAttr.USSWorkIDRole,
                    "@0=仅生成一个WorkID@1=按接受人生成WorkID");
                map.SetHelperAlert(NodeAttr.USSWorkIDRole, "对上一个节点是合流节点，当前节点是异表单子线程有效.");

                //map.AddBoolean(NodeAttr.AutoRunEnable, false, "是否启用自动运行？(仅当分流点向子线程发送时有效)", true, true, true);
                //map.AddTBString(NodeAttr.AutoRunParas, null, "自动运行SQL", true, false, 0, 100, 10, true);

                //为广西计算中心加.
                map.AddBoolean(NodeAttr.IsSendBackNode, false, "是否是发送返回节点(发送当前节点,自动发送给该节点的发送人,发送节点.)?", true, true, true);
                map.SetHelperUrl(NodeAttr.IsSendBackNode, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3661865&doc_id=31094");
                #endregion 分合流子线程属性

                #region 自动跳转规则
                map.AddGroupAttr("跳转规则");
                map.AddBoolean(NodeAttr.AutoJumpRole0, false, "处理人就是发起人", true, true, true);
                map.SetHelperUrl(NodeAttr.AutoJumpRole0, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3980077&doc_id=31094"); //增加帮助

                map.AddBoolean(NodeAttr.AutoJumpRole1, false, "处理人已经出现过", true, true, true);
                map.AddBoolean(NodeAttr.AutoJumpRole2, false, "处理人与上一步相同", true, true, true);
                map.AddBoolean(NodeAttr.WhenNoWorker, false, "(是)找不到人就跳转,(否)提示错误.", true, true, true);

                map.AddTBString(NodeAttr.AutoJumpExp, null, "表达式", true, false, 0, 200, 10, true);
                map.SetHelperAlert(NodeAttr.AutoJumpExp, "可以输入Url或SQL语句，请参考帮助文档。"); //增加帮助

                map.AddDDLSysEnum(NodeAttr.SkipTime, 0, "执行跳转事件", true, true, NodeAttr.SkipTime, "@0=上一个节点发送时@1=当前节点工作打开时");
                map.SetHelperUrl(NodeAttr.SkipTime, "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=3980077&doc_id=31094"); //增加帮助
                #endregion


                BtnLab lab = new BtnLab();
                map.AddAttrs(lab.EnMap.Attrs, true);

                #region 基础功能.
                map.AddGroupMethod("基本信息");
                //节点工具栏,主从表映射.
                map.AddDtl(new NodeToolbars(), NodeToolbarAttr.FK_Node, null, DtlEditerModel.DtlBatch);

                RefMethod rm = null;
                rm = new RefMethod();
                rm.Title = "接收人规则";
                //  rm.Icon = "../../WF/Admin/AttrNode/Img/Sender.png";
                rm.Icon = "icon-people";
                rm.ClassMethodName = this.ToString() + ".DoAccepterRoleNew";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "多人处理规则";
                rm.ClassMethodName = this.ToString() + ".DoTodolistModel";
                rm.Icon = "../../WF/Img/Multiplayer.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-options";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "节点事件"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoAction";
                //rm.Icon = "../../WF/Img/Event.png";
                rm.Icon = "icon-energy";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "节点消息"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoMessage";
                //rm.Icon = "../../WF/Img/Message24.png";
                rm.Icon = "icon-bubbles";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "发送后转向"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoTurnToDeal";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Turnto.png";
                rm.Icon = "icon-share";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "发送阻塞规则";
                rm.ClassMethodName = this.ToString() + ".DoBlockModel";
                // rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/BlockModel.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-close";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流程完成条件";
                rm.ClassMethodName = this.ToString() + ".DoCondFlow";
                rm.Icon = "../../WF/Admin/AttrNode/Img/Cond.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-list";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "节点完成条件";
                rm.ClassMethodName = this.ToString() + ".DoCondNode";
                rm.Icon = "../../WF/Admin/AttrNode/Img/Cond.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-list";
                //map.AddRefMethod(rm);

                ////暂时去掉. 
                //rm = new RefMethod();
                //rm.Title = "待办删除规则";
                //rm.ClassMethodName = this.ToString() + ".DoGenerWorkerListDelRole";
                ////rm.Icon = "../../WF/Admin/AttrNode/Img/Cond.png";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Icon = "icon-layers";
                #endregion 基础功能.

                #region 字段相关功能（不显示在菜单里）
                rm = new RefMethod();
                rm.Title = "上传公文模板";
                rm.ClassMethodName = this.ToString() + ".DocTemp";
                //  rm.Icon = "../../WF/Img/FileType/doc.gif";
                //设置相关字段.
                rm.RefAttrKey = BtnAttr.OfficeBtnEnable;
                rm.RefAttrLinkLabel = "公文模板维护";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                rm.Icon = "icon-briefcase";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "可退回的节点(当退回规则设置可退回指定的节点时,该设置有效.)"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoCanReturnNodes";
                //rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Icon = "icon-layers";

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
                //rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Icon = "icon-layers";

                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.RefAttrKey = NodeAttr.CancelRole; //在该节点下显示连接.
                rm.RefAttrLinkLabel = "";
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "绑定打印格式模版(当打印方式为打印RTF格式模版时,该设置有效)";
                //rm.ClassMethodName = this.ToString() + ".DoBill";
                //rm.Icon = "../../WF/Img/FileType/doc.gif";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;

                //rm = new RefMethod();
                //rm.Title = "打印设置"; // "可撤销发送的节点";
                ////设置相关字段.
                //rm.RefAttrKey = NodeAttr.PrintDocEnable;
                //rm.Target = "_blank";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;
                //map.AddRefMethod(rm);
                //if (BP.Difference.SystemConfig.CustomerNo == "HCBD")
                //{
                //    /* 为海成邦达设置的个性化需求. */
                //    rm = new RefMethod();
                //    rm.Title = "DXReport设置";
                //    rm.ClassMethodName = this.ToString() + ".DXReport";
                //    rm.Icon = "../../WF/Img/FileType/doc.gif";
                //    map.AddRefMethod(rm);
                //}

                rm = new RefMethod();
                rm.Title = "设置自动抄送规则(当节点为自动抄送时,该设置有效.)"; // "抄送规则";
                rm.ClassMethodName = this.ToString() + ".DoCCRole";
                //rm.Icon = "../../WF/Img/Btn/DTS.gif";
                rm.Icon = "icon-layers";
                //设置相关字段.
                rm.RefAttrKey = NodeAttr.CCRole;
                rm.RefAttrLinkLabel = "自动抄送设置";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);
                #endregion 字段相关功能（不显示在菜单里）

                #region 表单设置.
                map.AddGroupMethod("表单设置");

                rm = new RefMethod();
                rm.Title = "表单方案";
                //rm.Icon = "../../WF/Admin/CCFormDesigner/Img/Form.png";
                rm.Icon = "icon-present";
                rm.ClassMethodName = this.ToString() + ".DoSheet";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "手机表单字段顺序";
                //rm.Icon = "../../WF/Admin/CCFormDesigner/Img/telephone.png";
                rm.Icon = "icon-layers";
                //rm.Icon = ../../Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoSortingMapAttrs";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "审核组件";
                //rm.Icon = "../../WF/Img/Components.png";
                rm.Icon = "icon-puzzle";
                //rm.Icon = ../../Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoFrmNodeWorkCheck";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "父子流程组件";
                //rm.Icon = "../../WF/Img/Components.png";
                rm.Icon = "icon-puzzle";
                //rm.Icon = ../../Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoFrmSubFlow";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "轨迹组件";
                //rm.Icon = "../../WF/Img/Components.png";
                rm.Icon = "icon-puzzle";
                //rm.Icon = ../../Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoFrmTrack";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "流转自定义组件";
                //rm.Icon = "../../WF/Img/Components.png";
                rm.Icon = "icon-puzzle";
                //rm.Icon = ../../Img/Mobile.png";
                rm.ClassMethodName = this.ToString() + ".DoFrmTransferCustom";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "批量处理";
                rm.Icon = "icon-calculator";
                rm.ClassMethodName = this.ToString() + ".DoBatchStartFields()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "特别控件特别用户权限";
                rm.Icon = "../../WF/Admin/CCFormDesigner/Img/SpecUserSpecFields.png";
                rm.ClassMethodName = this.ToString() + ".DoSpecFieldsSpecUsers()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-note";
                map.AddRefMethod(rm);
                #endregion 表单设置.

                #region 考核属性.

                map.AddTBInt(NodeAttr.TAlertRole, 0, "逾期提醒规则", false, false); //"限期(天)"
                map.AddTBInt(NodeAttr.TAlertWay, 0, "逾期提醒方式", false, false); //"限期(天)
                map.AddTBInt(NodeAttr.WAlertRole, 0, "预警提醒规则", false, false); //"限期(天)"
                map.AddTBInt(NodeAttr.WAlertWay, 0, "预警提醒方式", false, false); //"限期(天)"

                map.AddTBFloat(NodeAttr.TCent, 2, "扣分(每延期1小时)", false, false);
                map.AddTBInt(NodeAttr.CHWay, 0, "考核方式", false, false); //"限期(天)"

                //考核相关.
                map.AddTBInt(NodeAttr.IsEval, 0, "是否工作质量考核", false, false);
                map.AddTBInt(NodeAttr.OutTimeDeal, 0, "超时处理方式", false, false);

                #endregion 考核属性.

                #region 父子流程.
                map.AddGroupMethod("父子流程");
                rm = new RefMethod();
                rm.Title = "父子流程表单组件";
                //rm.Icon = "../../WF/Admin/AttrNode/Img/SubFlows.png";
                rm.Icon = "icon-settings";
                rm.ClassMethodName = this.ToString() + ".DoSubFlow";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "手动启动子流程"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoSubFlowHand";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-social-spotify";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "自动触发子流程"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoSubFlowAuto";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Icon = "icon-layers";
                rm.Icon = "icon-feed";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "延续子流程"; // "调用事件接口";
                rm.ClassMethodName = this.ToString() + ".DoSubFlowYanXu";
                //  rm.Icon = "../../WF/Img/Event.png";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Icon = "icon-layers";
                rm.Icon = "icon-organization";
                map.AddRefMethod(rm);
                #endregion 父子流程.

                #region 考核.
                map.AddGroupMethod("考核规则");
                rm = new RefMethod();
                rm.Title = "设置考核规则";
                //rm.Icon = "../../WF/Admin/CCFormDesigner/Img/CH.png";
                rm.Icon = "icon-book-open";
                rm.ClassMethodName = this.ToString() + ".DoCHRole";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "超时处理规则";
                //rm.Icon = "../../WF/Admin/CCFormDesigner/Img/OvertimeRole.png";
                rm.Icon = "icon-clock";
                rm.ClassMethodName = this.ToString() + ".DoCHOvertimeRole";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
                #endregion 考核.

                #region 实验中的功能
                map.AddGroupMethod("实验中的功能");
                rm = new RefMethod();
                rm.Title = "自定义属性(通用)";
                rm.ClassMethodName = this.ToString() + ".DoSelfParas()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "实验中的功能";
                rm.Visable = false;
                rm.Icon = "icon-layers";
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "版本快照";
                rm.ClassMethodName = this.ToString() + ".ShowVer";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "自定义属性(自定义)";
                //rm.ClassMethodName = this.ToString() + ".DoNodeAttrExt()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "实验中的功能";
                //rm.Visable = false;
                //rm.Icon = "icon-layers";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "设置节点类型";
                ////rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                //rm.Icon = "icon-layers";
                //rm.ClassMethodName = this.ToString() + ".DoNodeAppType()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "实验中的功能";
                ////rm.Visable = false;
                //map.AddRefMethod(rm);

                if (Glo.CCBPMRunModel != CCBPMRunModel.SAAS)
                {
                    rm = new RefMethod();
                    rm.Title = "设置为模版";

                    string info = "如果把这个节点设置为模版,以后在新建节点的时候,就会按照当前的属性初始化节点数据.";
                    info += "\t\n产生的数据文件存储在/DataUser/Xml/下.";
                    rm.Warning = info;

                    //  rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                    rm.ClassMethodName = this.ToString() + ".DoSetTemplate()";
                    rm.RefMethodType = RefMethodType.Func;
                    rm.GroupName = "实验中的功能";
                    rm.Icon = "icon-layers";
                    //rm.Visable = false;
                    map.AddRefMethod(rm);
                }


                rm = new RefMethod();
                rm.Title = "批量设置节点属性";
                //rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/Node.png";
                rm.Icon = "icon-layers";
                rm.ClassMethodName = this.ToString() + ".DoNodeAttrs()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
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
                rm.Title = "抄送人规则";
                //rm.Icon = "../../WF/Admin/AttrNode/Img/CC.png";
                rm.Icon = "icon-people";
                rm.ClassMethodName = this.ToString() + ".DoCCer";  //要执行的方法名.
                rm.RefMethodType = RefMethodType.RightFrameOpen; // 功能类型
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "设置提示信息";
                //     rm.Icon = "../../WF/Admin/AttrNode/Img/CC.png";
                rm.ClassMethodName = this.ToString() + ".DoHelpRole";  //要执行的方法名.
                rm.RefAttrKey = BtnAttr.HelpRole; //帮助信息.
                rm.RefMethodType = RefMethodType.LinkeWinOpen; // 功能类型
                rm.Icon = "icon-layers";
                map.AddRefMethod(rm);

                /*rm = new RefMethod();
                rm.Title = "退回扩展列";
                rm.ClassMethodName = this.ToString() + ".DtlOfReturn";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.LinkModel;
                rm.RefAttrKey = NodeAttr.ReturnCHEnable;
                rm.Icon = "icon-layers";
                map.AddRefMethod(rm);*/
                #endregion 实验中的功能

                this._enMap = map;
                return this._enMap;
            }
        }

        public string ShowVer()
        {
            return "/WF/Admin/DataVer/DataVerList.htm?FrmID=" + this.ToString() + "&RefPKVal=" + this.NodeID;
        }

        #region 考核规则.
        /// <summary>
        /// 考核规则
        /// </summary>
        /// <returns></returns>
        public string DoCHRole()
        {
            return "../../Admin/AttrNode/EvaluationRole/Default.htm?FK_Node=" + this.NodeID;
        }
        /// <summary>
        /// 超时处理规则
        /// </summary>
        /// <returns></returns>
        public string DoCHOvertimeRole()
        {
            //return "../../Admin/AttrNode/CHOvertimeRole.htm?FK_Node=" + this.NodeID; 
            return "../../Admin/AttrNode/OvertimeRole/Default.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        #endregion 考核规则.

        #region 基础设置.
        /// <summary>
        /// 批处理规则
        /// </summary>
        /// <returns></returns>
        public string DoBatchStartFields()
        {
            return "../../Admin/AttrNode/BatchRole/Default.htm?s=d34&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID;
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
            return "../../Admin/AttrNode/FrmSln/Default.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        public string DoSheetOld()
        {
            return "../../Admin/AttrNode/NodeFromWorkModel.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }

        /// <summary>
        /// 接受人规则
        /// </summary>
        /// <returns></returns>
        public string DoAccepterRoleNew()
        {
            return "../../Admin/AttrNode/AccepterRole/Default.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        public string DoTodolistModel()
        {
            return "../../Admin/AttrNode/TodolistModel/Default.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }

        /// <summary>
        /// 发送阻塞规则
        /// </summary>
        /// <returns></returns>
        public string DoBlockModel()
        {
            return "../../Admin/AttrNode/BlockModel/Default.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 发送后转向规则
        /// </summary>
        /// <returns></returns>
        public string DoTurnToDeal()
        {
            // return "../../Admin/AttrNode/TurnTo/0.TurntoDefault.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
            return "../../Admin/AttrNode/TurnTo/Default.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;

        }
        /// <summary>
        /// 抄送人规则
        /// </summary>
        /// <returns></returns>
        public string DoCCer()
        {
            return "../../Admin/AttrNode/CCRole.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 加载提示信息
        /// </summary>
        /// <returns></returns>
        public string DoHelpRole()
        {
            return "../../Admin/FoolFormDesigner/HelpRole.htm?NodeID=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        #endregion

        #region 表单相关.
        /// <summary>
        /// 审核组件
        /// </summary>
        /// <returns></returns>
        public string DoFrmNodeWorkCheck()
        {
            return "../../Comm/EnOnly.htm?EnName=BP.WF.Template.NodeWorkCheck&PKVal=" + this.NodeID + "&t=" + DataType.CurrentDateTime;
        }
        /// <summary>
        /// 父子流程组件
        /// </summary>
        /// <returns></returns>
        public string DoFrmSubFlow()
        {
            return "../../Comm/EnOnly.htm?EnName=BP.WF.Template.SFlow.FrmSubFlow&PKVal=" + this.NodeID + "&t=" + DataType.CurrentDateTime;
        }
        /// <summary>
        /// 轨迹组件
        /// </summary>
        /// <returns></returns>
        public string DoFrmTrack()
        {
            return "../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmTrack&PKVal=" + this.NodeID + "&t=" + DataType.CurrentDateTime;
        }
        /// <summary>
        /// 流转自定义
        /// </summary>
        /// <returns></returns>
        public string DoFrmTransferCustom()
        {
            return "../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmTransferCustom&PKVal=" + this.NodeID + "&t=" + DataType.CurrentDateTime;
        }
        /// <summary>
        /// 特别用户特殊字段权限.
        /// </summary>
        /// <returns></returns>
        public string DoSpecFieldsSpecUsers()
        {
            return "../../Admin/AttrNode/SepcFiledsSepcUsers.htm?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&FK_Node=" + this.NodeID + "&t=" + DataType.CurrentDateTime;
        }
        /// <summary>
        /// 排序字段顺序
        /// </summary>
        /// <returns></returns>
        public string DoSortingMapAttrs()
        {
            return "../../Admin/MobileFrmDesigner/Default.htm?FK_Flow=" + this.FK_Flow + "&FK_MapData=ND" +
                   this.NodeID + "&FK_Node=" + this.NodeID + "&t=" + DataType.CurrentDateTime;
        }
        #endregion 表单相关.

        #region 实验中的功能.
        /// <summary>
        /// 设置模版
        /// </summary>
        /// <returns></returns>
        public string DoSetTemplate()
        {
            DataTable dt = this.ToDataTableField();
            dt.TableName = "Node";
            dt.WriteXml(BP.Difference.SystemConfig.PathOfDataUser + "XML/DefaultNewNodeAttr.xml");
            return "执行成功.";
        }
        /// <summary>
        /// 自定义参数（通用）
        /// </summary>
        /// <returns></returns>
        public string DoSelfParas()
        {
            return "../../Admin/AttrNode/SelfParas.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 自定义参数（自定义）
        /// </summary>
        /// <returns></returns>
        public string DoNodeAttrExt()
        {
            return "../../../DataUser/OverrideFiles/NodeAttrExt.htm?FK_Flow=" + this.NodeID;
        }

        /// <summary>
        /// 设置节点类型
        /// </summary>
        /// <returns></returns>
        public string DoNodeAppType()
        {
            return "../../Admin/AttrNode/NodeAppType.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow + "&tk=" + new Random().NextDouble();
        }
        #endregion

        #region 子流程。
        /// <summary>
        /// 父子流程
        /// </summary>
        /// <returns></returns>
        public string DoSubFlow()
        {
            return "../../Comm/RefFunc/EnOnly.htm?EnName=BP.WF.Template.SFlow.FrmSubFlow&PK=" + this.NodeID;
        }
        /// <summary>
        /// 自动触发
        /// </summary>
        /// <returns></returns>
        public string DoSubFlowAuto()
        {
            return "../../Admin/AttrNode/SubFlow/SubFlowAuto.htm?FK_Node=" + this.NodeID + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 手动启动子流程
        /// </summary>
        /// <returns></returns>
        public string DoSubFlowHand()
        {
            return "../../Admin/AttrNode/SubFlow/SubFlowHand.htm?FK_Node=" + this.NodeID + "&tk=" + new Random().NextDouble();
        }
        /// <summary>
        /// 延续子流程
        /// </summary>
        /// <returns></returns>
        public string DoSubFlowYanXu()
        {
            return "../../Admin/AttrNode/SubFlow/SubFlowYanXu.htm?FK_Node=" + this.NodeID + "&tk=" + new Random().NextDouble();
        }
        #endregion 子流程。


        /// <summary>
        /// 更新节点名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Do_SaveAndUpdateNodeName(string name)
        {
            //更新节点名称.
            DBAccess.RunSQL("UPDATE  WF_Node SET Name='" + name + "' WHERE NodeID=" + this.NodeID);

            //修改表单名称.
            DBAccess.RunSQL("UPDATE  Sys_MapData SET Name='" + name + "' WHERE No='ND" + this.NodeID + "'");

            //修改分组名称.
            string oid = DBAccess.RunSQLReturnString("SELECT OID FROM Sys_GroupField WHERE FrmID='ND" + this.NodeID + "' ORDER BY Idx  ", null);
            if (oid == null)
                return "更新成功.";

            DBAccess.RunSQL("UPDATE   Sys_GroupField SET Lab='" + name + "' WHERE OID=" + oid);
            return "执行成功";
        }
        /// <summary>
        /// 简单的更新节点名称
        /// </summary>
        /// <param name="name">要更新的节点名称</param>
        /// <returns></returns>
        public string Do_SaveNodeName(string name)
        {
            //更新节点名称.
            DBAccess.RunSQL("UPDATE WF_Node SET Name='" + name + "' WHERE NodeID=" + this.NodeID);

            // //修改表单名称.
            //     DBAccess.RunSQL("UPDATE SET Sys_MapDate SET Name='" + name + "' WHERE No='ND" + this.NodeID + "'");

            //修改分组名称.
            //  string oid = DBAccess.RunSQLReturnString("SELECT OID FROM Sys_GroupField WHERE FrmID='ND" + this.NodeID + "' ORDER BY Idx  ", null);
            //  if (oid == null)
            //     return "更新成功.";

            //  DBAccess.RunSQL("UPDATE SET Sys_GroupField SET Lab='" + name + "' WHERE OID=" + oid);
            return "执行成功";
        }


        public string DoTurn()
        {
            return "../../Admin/AttrNode/TurnTo.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
            //, "节点完成转向处理", "FrmTurn", 800, 500, 200, 300);
            //BP.WF.Node nd = new BP.WF.Node(this.NodeID);
            //return nd.DoTurn();
        }
        /// <summary>
        /// 公文模板
        /// </summary>
        /// <returns></returns>
        public string DocTemp()
        {
            return "../../Admin/AttrNode/DocTemp.htm?FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
        }
        /// <summary>
        /// 抄送规则
        /// </summary>
        /// <returns></returns>
        public string DoCCRole()
        {
            return "../../Comm/En.htm?EnName=BP.WF.Template.CCEn.CC&PKVal=" + this.NodeID + "&FK_Node=" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
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
        /// 流程完成条件
        /// </summary>
        /// <returns></returns>
        public string DoCondFlow()
        {
            return "../../Admin/Cond2020/List.htm?CondType=" + (int)CondType.Flow + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&ToNodeID=" + this.NodeID;
        }
        /// <summary>
        /// 节点完成条件
        /// </summary>
        /// <returns></returns>
        public string DoCondNode()
        {
            return "../../Admin/Cond2020/List.htm?CondType=" + (int)CondType.Node + "&FK_Flow=" + this.FK_Flow + "&FK_Node=" + this.NodeID + "&ToNodeID=" + this.NodeID;
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
                case NodeFormType.Develop:
                    return "../../Admin/FoolFormDesigner/CCForm/Frm.htm?FK_MapData=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
                    break;
                default:
                case NodeFormType.FoolForm:
                    return "../../Admin/FoolFormDesigner/Designer.htm?PK=ND" + this.NodeID + "&FK_Flow=" + this.FK_Flow;
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
        /// 保存提示信息
        /// </summary>
        /// <returns></returns>
        public string SaveHelpAlert(string text)
        {
            string file = BP.Difference.SystemConfig.PathOfDataUser + "CCForm/HelpAlert/" + this.NodeID + ".htm";
            string folder = System.IO.Path.GetDirectoryName(file);
            //若文件夹不存在，则创建
            if (System.IO.Directory.Exists(folder) == false)
                System.IO.Directory.CreateDirectory(folder);

            DataType.WriteFile(file, text);
            return "保存成功！";
        }
        /// <summary>
        /// 读取提示信息
        /// </summary>
        /// <returns></returns>
        public string ReadHelpAlert()
        {
            string doc = "";
            string file = BP.Difference.SystemConfig.PathOfDataUser + "CCForm/HelpAlert/" + this.NodeID + ".htm";
            string folder = System.IO.Path.GetDirectoryName(file);
            if (System.IO.Directory.Exists(folder) != false)
            {
                if (File.Exists(file))
                {
                    doc = DataType.ReadTextFile(file);

                }
            }
            return doc;
        }
        public string DtlOfReturn()
        {
            string url = "../../Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapDtl=BP.WF.ReturnWorks" + "&For=BP.WF.ReturnWorks&FK_Flow=" + this.FK_Flow;
            return url;
        }
        protected override bool beforeUpdate()
        {
            //检查设计流程权限,集团模式下，不是自己创建的流程，不能设计流程.
            BP.WF.Template.TemplateGlo.CheckPower(this.FK_Flow);

            //更新流程版本
            Flow.UpdateVer(this.FK_Flow);

            #region 处理节点数据.
            Node nd = new Node(this.NodeID);
            if (nd.IsStartNode == true)
            {
                //开始节点不能设置游离状态
                if (this.IsYouLiTai == true)
                    throw new Exception("当前节点是开始节点不能设置游离状态");
                if (this.HuiQianRole != WF.HuiQianRole.None)
                    throw new Exception("当前节点是开始节点不能启用会签按钮操作");
                this.SetValByKey(BtnAttr.HungEnable, false);
                this.SetValByKey(BtnAttr.ThreadEnable, false); //子线程.
            }

            //是否是发送返回节点？
            nd.IsSendBackNode = this.IsSendBackNode;

            if (nd.IsSendBackNode == true)
            {
                //强制设置按照连接线控制.
                nd.CondModel = DirCondModel.ByLineCond;
            }
            nd.DirectUpdate(); //直接更新.

            if (this.IsSendBackNode == true)
            {
                if (nd.HisToNDNum != 0)
                    this.IsSendBackNode = false;

                //    throw new Exception("err@您设置当前节点为【发送自动返回节点】，但是该节点配置到到达节点，是不正确的。");
            }


            if (nd.HisRunModel == RunModel.HL || nd.HisRunModel == RunModel.FHL)
            {
                /*如果是合流点*/
            }

            //如果启动了会签,并且是抢办模式,强制设置为队列模式.或者组长模式.
            if (this.HuiQianRole != WF.HuiQianRole.None)
            {
                if (this.HuiQianRole == WF.HuiQianRole.Teamup)
                    DBAccess.RunSQL("UPDATE WF_Node SET TodolistModel=" + (int)TodolistModel.Teamup + "  WHERE NodeID=" + this.NodeID);

                if (this.HuiQianRole == WF.HuiQianRole.TeamupGroupLeader)
                {
                    DBAccess.RunSQL("UPDATE WF_Node SET TodolistModel=" + (int)TodolistModel.TeamupGroupLeader + ", TeamLeaderConfirmRole=" + (int)TeamLeaderConfirmRole.HuiQianLeader + " WHERE NodeID=" + this.NodeID);
                    if (this.HuiQianLeaderRole == HuiQianLeaderRole.OnlyOne && this.AddLeaderEnable == true)
                    {
                        throw new Exception("当前节点是组长模式且组长只有一个，不能启用加主持人的操作");
                        // this.AddLeaderEnable = false;
                    }

                }
            }


            if (nd.CondModel == DirCondModel.ByLineCond)
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
            if (1 == 2 && nd.CondModel == DirCondModel.ByLineCond)
            {
                /*如果是启用了按钮，就检查当前节点到达的节点是否有【按照选择接受人】的方式确定接收人的范围. */
                Nodes nds = nd.HisToNodes;
                foreach (Node mynd in nds)
                {
                    if (mynd.HisDeliveryWay == DeliveryWay.BySelected)
                    {
                        //强制设置安装人员选择器来选择.
                        this.SetValByKey(NodeAttr.CondModel, (int)DirCondModel.ByDDLSelected);
                        break;
                    }
                }
            }
            #endregion 处理节点数据.

            #region 创建审核组件附件
            if (this.FWCAth == FWCAth.MinAth)
            {
                FrmAttachment workCheckAth = new FrmAttachment();
                workCheckAth.setMyPK("ND" + this.NodeID + "_FrmWorkCheck");
                //不包含审核组件
                if (workCheckAth.RetrieveFromDBSources() == 0)
                {
                    workCheckAth = new FrmAttachment();
                    /*如果没有查询到它,就有可能是没有创建.*/
                    workCheckAth.setMyPK("ND" + this.NodeID + "_FrmWorkCheck");
                    workCheckAth.setFK_MapData("ND" + this.NodeID.ToString());
                    workCheckAth.NoOfObj = "FrmWorkCheck";
                    workCheckAth.Exts = "*.*";

                    //存储路径.
                    // workCheckAth.SaveTo = "/DataUser/UploadFile/";
                    workCheckAth.IsNote = false; //不显示note字段.
                    workCheckAth.IsVisable = false; // 让其在form 上不可见.

                    //位置.

                    workCheckAth.H = (float)150;

                    //多附件.
                    workCheckAth.UploadType = AttachmentUploadType.Multi;
                    workCheckAth.Name = "审核组件";
                    workCheckAth.SetValByKey("AtPara", "@IsWoEnablePageset=1@IsWoEnablePrint=1@IsWoEnableViewModel=1@IsWoEnableReadonly=0@IsWoEnableSave=1@IsWoEnableWF=1@IsWoEnableProperty=1@IsWoEnableRevise=1@IsWoEnableIntoKeepMarkModel=1@FastKeyIsEnable=0@IsWoEnableViewKeepMark=1@FastKeyGenerRole=");
                    workCheckAth.Insert();
                }
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

            BtnLab btnLab = new BtnLab(this.NodeID);
            btnLab.RetrieveFromDBSources();

            //如果是合流. 就启用按钮.
            if (nd.IsHL == true)
            {
                this.SetValByKey(BtnAttr.ThreadEnable, true);
            }

            //清除所有的缓存.
            Cash.ClearCash(this.ToString());

            return base.beforeUpdate();
        }
        protected override void afterInsertUpdateAction()
        {
            Node fl = new Node();
            fl.NodeID = this.NodeID;
            fl.RetrieveFromDBSources();
            if (this.IsYouLiTai == true)
                fl.SetPara("IsYouLiTai", 1);
            else
                fl.SetPara("IsYouLiTai", 0);
            fl.Update();

            BtnLab btnLab = new BtnLab();
            btnLab.NodeID = this.NodeID;
            btnLab.RetrieveFromDBSources();
            Cash2019.UpdateRow(btnLab.ToString(), this.NodeID.ToString(), btnLab.Row);


            CC cc = new CC();
            cc.NodeID = this.NodeID;
            cc.RetrieveFromDBSources();
            Cash2019.UpdateRow(cc.ToString(), this.NodeID.ToString(), cc.Row);

            FrmNodeComponent frmNodeComponent = new FrmNodeComponent();
            frmNodeComponent.NodeID = this.NodeID;
            frmNodeComponent.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmNodeComponent.ToString(), this.NodeID.ToString(), frmNodeComponent.Row);


            FrmTrack frmTrack = new FrmTrack();
            frmTrack.NodeID = this.NodeID;
            frmTrack.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmTrack.ToString(), this.NodeID.ToString(), frmTrack.Row);

            FrmTransferCustom frmTransferCustom = new FrmTransferCustom();
            frmTransferCustom.NodeID = this.NodeID;
            frmTransferCustom.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmTransferCustom.ToString(), this.NodeID.ToString(), frmTransferCustom.Row);

            NodeWorkCheck frmWorkCheck = new NodeWorkCheck();
            frmWorkCheck.NodeID = this.NodeID;
            frmWorkCheck.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmWorkCheck.ToString(), this.NodeID.ToString(), frmWorkCheck.Row);

            NodeSheet nodeSheet = new NodeSheet();
            nodeSheet.NodeID = this.NodeID;
            nodeSheet.RetrieveFromDBSources();
            Cash2019.UpdateRow(nodeSheet.ToString(), this.NodeID.ToString(), nodeSheet.Row);

            NodeSimple nodeSimple = new NodeSimple();
            nodeSimple.NodeID = this.NodeID;
            nodeSimple.RetrieveFromDBSources();
            Cash2019.UpdateRow(nodeSimple.ToString(), this.NodeID.ToString(), nodeSimple.Row);

            FrmSubFlow frmSubFlow = new FrmSubFlow();
            frmSubFlow.NodeID = this.NodeID;
            frmSubFlow.RetrieveFromDBSources();
            Cash2019.UpdateRow(frmSubFlow.ToString(), this.NodeID.ToString(), frmSubFlow.Row);

            //GetTask getTask = new GetTask();
            //getTask.NodeID = this.NodeID;
            //getTask.RetrieveFromDBSources();
            //Cash2019.UpdateRow(getTask.ToString(), this.NodeID.ToString(), getTask.Row);

            //如果是组长会签模式，通用选择器只能单项选择
            if (this.HuiQianRole == HuiQianRole.TeamupGroupLeader && this.HuiQianLeaderRole == HuiQianLeaderRole.OnlyOne)
            {
                Selector selector = new Selector();
                selector.NodeID = this.NodeID;
                selector.RetrieveFromDBSources();
                selector.IsSimpleSelector = true;
                selector.Update();

            }

            base.afterInsertUpdateAction();


            //写入日志.
            BP.Sys.Base.Glo.WriteUserLog("更新节点属性：" + this.Name + " - " + this.NodeID);

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

        public NodeExts(string fk_flow)
        {
            this.Retrieve(NodeAttr.FK_Flow, fk_flow, NodeAttr.Step);
            return;
        }
        #endregion

        public override Entity GetNewEntity
        {
            get { return new NodeExt(); }
        }
    }
}
