using System;
using BP.En;

namespace BP.WF.Template.SFlow
{
    /// <summary>
    /// 自动触发子流程属性
    /// </summary>
    public class SubFlowAutoAttr : SubFlowAttr
    {
       
        /// <summary>
        /// 当前节点为子流程时，所有子流程完成后启动他的同级子流程自动运行或者结束
        /// </summary>
        public const string IsAutoSendSLSubFlowOver = "IsAutoSendSLSubFlowOver";
    }
    /// <summary>
    /// 自动触发子流程.
    /// </summary>
    public class SubFlowAuto : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 主流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.FK_Flow);
            }
            set
            {
                SetValByKey(SubFlowAutoAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string SubFlowNo
        {
            get
            {
                return this.GetValStringByKey(SubFlowAttr.SubFlowNo);
            }
            set
            {
                SetValByKey(SubFlowAutoAttr.SubFlowNo, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.SubFlowName);
            }
        }
        /// <summary>
        /// 条件表达式.
        /// </summary>
        public string CondExp
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.CondExp);
            }
            set
            {
                SetValByKey(SubFlowAutoAttr.CondExp, value);
            }
        }
        /// <summary>
        /// 表达式类型
        /// </summary>
        public ConnDataFrom ExpType
        {
            get
            {
                return (ConnDataFrom)this.GetValIntByKey(SubFlowAutoAttr.ExpType);
            }
            set
            {
                SetValByKey(SubFlowAutoAttr.ExpType, (int)value);
            }
        }
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(SubFlowAutoAttr.FK_Node);
            }
        }
        /// <summary>
        /// 调用时间 0=工作发送时, 1=工作到达时.
        /// </summary>
        public int InvokeTime
        {
            get
            {
                return this.GetValIntByKey(SubFlowAutoAttr.InvokeTime);
            }
        }
        
        /// <summary>
        /// 运行类型
        /// </summary>
        public SubFlowModel HisSubFlowModel
        {
            get
            {
                return (SubFlowModel)this.GetValIntByKey(SubFlowAutoAttr.SubFlowModel);
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public SubFlowType HisSubFlowType
        {
            get
            {
                return (SubFlowType)this.GetValIntByKey(SubFlowAutoAttr.SubFlowType);
            }
        }
        /// <summary>
        /// 仅仅发起一次.
        /// </summary>
        public bool StartOnceOnly
        {
            get
            {
                return this.GetValBooleanByKey(SubFlowAutoAttr.StartOnceOnly);
            }
        }

        public bool CompleteReStart
        {
            get
            {
                return this.GetValBooleanByKey(SubFlowAutoAttr.CompleteReStart);
            }
        }
        /// <summary>
        /// 指定的流程启动后,才能启动该子流程(请在文本框配置子流程).
        /// </summary>
        public bool IsEnableSpecFlowStart
        {
            get
            {
                var val = this.GetValBooleanByKey(SubFlowAutoAttr.IsEnableSpecFlowStart);
                if (val == false)
                    return false;

                if (this.SpecFlowStart.Length > 2)
                    return true;
                return false;
            }
        }
        public string SpecFlowStart
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.SpecFlowStart);
            }
        }
        /// <summary>
        /// 指定的流程结束后,才能启动该子流程(请在文本框配置子流程).
        /// </summary>
        public bool IsEnableSpecFlowOver
        {
            get
            {
                var val = this.GetValBooleanByKey(SubFlowAutoAttr.IsEnableSpecFlowOver);
                if (val == false)
                    return false;

                if (this.SpecFlowOver.Length > 2)
                    return true;
                return false;
            }
        }
        public string SpecFlowOver
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.SpecFlowOver);
            }
        }
        /// <summary>
        /// 按SQL配置
        /// </summary>
        public bool IsEnableSQL
        {
            get
            {
                var val = this.GetValBooleanByKey(SubFlowAutoAttr.IsEnableSQL);
                if (val == false)
                    return false;

                if (this.SpecSQL.Length > 2)
                    return true;
                return false;

            }
        }

        public string SpecSQL
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.SpecSQL);
            }
        }

        /// <summary>
        /// 指定平级子流程节点结束后启动子流程
        /// </summary>
        public bool IsEnableSameLevelNode
        {
            get
            {
                var val = this.GetValBooleanByKey(SubFlowAutoAttr.IsEnableSameLevelNode);
                if (val == false)
                    return false;

                if (this.SameLevelNode.Length > 2)
                    return true;
                return false;

            }
        }

        public string SameLevelNode
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.SameLevelNode);
            }
        }


        /// <summary>
        /// 自动发起的子流程发送方式
        /// </summary>
        public int SendModel
        {
            get
            {
                return this.GetValIntByKey(SubFlowAutoAttr.SendModel);
            }
        }

        public bool SubFlowHidTodolist
        {
            get
            {
                return this.GetValBooleanByKey(SubFlowAttr.SubFlowHidTodolist);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 自动触发子流程
        /// </summary>
        public SubFlowAuto() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeSubFlow", "自动触发子流程");

                map.AddMyPK();

                map.AddTBString(SubFlowAttr.FK_Flow, null, "主流程编号", true, true, 0, 5, 100);
                map.AddTBInt(SubFlowHandAttr.FK_Node, 0, "节点", false, true);

                map.AddDDLSysEnum(SubFlowHandAttr.SubFlowType, 0, "子流程类型", true, false, SubFlowHandAttr.SubFlowType,
                "@0=手动启动子流程@1=触发启动子流程@2=延续子流程");

                map.AddTBString(SubFlowYanXuAttr.SubFlowNo, null, "子流程编号", true, true, 0, 10, 150, false);
                map.AddTBString(SubFlowYanXuAttr.SubFlowName, null, "子流程名称", true, true, 0, 200, 150, false);

                map.AddDDLSysEnum(SubFlowYanXuAttr.SubFlowModel, 0, "子流程模式", true, true, SubFlowYanXuAttr.SubFlowModel,
                "@0=下级子流程@1=同级子流程");

                map.AddDDLSysEnum(SubFlowYanXuAttr.SubFlowSta, 1, "状态", true, true, SubFlowYanXuAttr.SubFlowSta,
             "@0=禁用@1=启用@2=只读");

                
                map.AddDDLSysEnum(SubFlowAutoAttr.ParentFlowSendNextStepRole, 0, "父流程自动运行到下一步规则", true, true,
                SubFlowAutoAttr.ParentFlowSendNextStepRole, "@0=不处理@1=该子流程运行结束@2=该子流程运行到指定节点");


                map.AddDDLSysEnum(SubFlowAutoAttr.ParentFlowOverRole, 0, "父流程结束规则", true, true,
                SubFlowAutoAttr.ParentFlowSendNextStepRole, "@0=不处理@1=该子流程运行结束@2=该子流程运行到指定节点");
                map.AddTBInt(SubFlowAutoAttr.SubFlowNodeID, 0, "指定子流程节点ID", true, false);

                map.AddDDLSysEnum(SubFlowAutoAttr.IsAutoSendSLSubFlowOver, 0, "同级子流程结束规则", true, true,
               SubFlowAutoAttr.IsAutoSendSLSubFlowOver, "@0=不处理@1=让同级子流程自动运行下一步@2=结束同级子流程");

                map.AddDDLSysEnum(SubFlowAttr.InvokeTime, 0, "调用时间", true, true, SubFlowAttr.InvokeTime,
                    "@0=发送时@1=工作到达时");

                map.AddBoolean(SubFlowHandAttr.StartOnceOnly, false, "仅能被调用1次.", true, true, true);

                map.AddBoolean(SubFlowHandAttr.CompleteReStart, false, "该子流程运行结束后才可以重新发起.",
                   true, true, true);

                //启动限制规则.
                map.AddBoolean(SubFlowHandAttr.IsEnableSpecFlowStart, false, "指定的流程启动后,才能启动该子流程(请在文本框配置子流程).",
                 true, true, true);
                map.AddTBString(SubFlowHandAttr.SpecFlowStart, null, "子流程编号", true, false, 0, 200, 150, true);
                map.SetHelperAlert(SubFlowHandAttr.SpecFlowStart, "指定的流程启动后，才能启动该子流程，多个子流程用逗号分开. 001,002");
                map.AddTBString(SubFlowHandAttr.SpecFlowStartNote, null, "备注", true, false, 0, 500, 150, true);

                //启动限制规则.
                map.AddBoolean(SubFlowHandAttr.IsEnableSpecFlowOver, false, "指定的流程结束后,才能启动该子流程(请在文本框配置子流程).",
                 true, true, true);
                map.AddTBString(SubFlowHandAttr.SpecFlowOver, null, "子流程编号", true, false, 0, 200, 150, true);
                map.SetHelperAlert(SubFlowHandAttr.SpecFlowOver, "指定的流程结束后，才能启动该子流程，多个子流程用逗号分开. 001,002");
                map.AddTBString(SubFlowHandAttr.SpecFlowOverNote, null, "备注", true, false, 0, 500, 150, true);

                //启动限制规则
                map.AddBoolean(SubFlowHandAttr.IsEnableSQL, false, "按照指定的SQL配置.",
                 true, true, true);
                map.AddTBString(SubFlowHandAttr.SpecSQL, null, "SQL语句", true, false, 0, 500, 150, true);

                //启动限制规则
                map.AddBoolean(SubFlowHandAttr.IsEnableSameLevelNode, false, "按照指定平级子流程节点完成后启动.",
                 true, true, true);
                map.AddTBString(SubFlowHandAttr.SameLevelNode, null, "平级子流程节点", true, false, 0, 500, 150, true);
                map.SetHelperAlert(SubFlowHandAttr.SameLevelNode, "按照指定平级子流程节点完成后启动，才能启动该子流程，多个平级子流程节点用逗号分开. 001,102;002,206");

                //自动发送方式.
                map.AddDDLSysEnum(SubFlowHandAttr.SendModel, 0, "自动发送方式", true, true, SubFlowHandAttr.SendModel,
                    "@0=给当前人员设置开始节点待办@1=发送到下一个节点");
                map.SetHelperAlert(SubFlowHandAttr.SendModel,
                    "如果您选择了[发送到下一个节点]该流程的下一个节点的接受人规则必须是自动计算的,而不能手工选择.");

                map.AddTBString(SubFlowAttr.SubFlowCopyFields, null, "父流程字段对应子流程字段", true, false, 0, 400, 150, true);

                map.AddDDLSysEnum(SubFlowAttr.BackCopyRole, 0, "子流程结束后数据字段反填规则", true, true,
             SubFlowAttr.BackCopyRole, "@0=不反填@1=字段自动匹配@2=按照设置的格式@3=混合模式");

                map.AddTBString(SubFlowAttr.ParentFlowCopyFields, null, "子流程字段对应父流程字段", true, false, 0, 400, 150, true);
                map.SetHelperAlert(SubFlowHandAttr.ParentFlowCopyFields, "子流程结束后，按照设置模式:格式为@SubField1=ParentField1@SubField2=ParentField2@SubField3=ParentField3,即子流程字段对应父流程字段，设置成立复制\r\n如果使用签批字段时，请使用按照设置模式");

                //批量发送后，是否隐藏父流程的待办
                map.AddBoolean(SubFlowHandGuideAttr.SubFlowHidTodolist, false, "发送后是否隐藏父流程待办", true, true, true);

                map.AddTBInt(SubFlowHandAttr.Idx, 0, "顺序", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 设置主键
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            this.setMyPK(this.FK_Node + "_" + this.SubFlowNo + "_1");
            return base.beforeInsert();
        }

        protected override bool beforeUpdateInsertAction()
        {
            if (this.SendModel == 1)
            {
                //设置的发送到，发送到下一个节点上.

                Node nd = new Node(int.Parse(this.SubFlowNo + "01"));

                Nodes tonds = nd.HisToNodes;
                foreach (Node item in tonds)
                {
                    if (item.HisDeliveryWay == DeliveryWay.BySelected)
                        throw new Exception("err@【自动发送方式】设置错误，您选择了[发送到下一个节点]但是该节点的接收人规则为由上一步发送人员选择，这是不符合规则的。");
                }
            }

            //设置主流程ID.
            Node myNd = new Node(this.FK_Node);
            this.FK_Flow = myNd.FK_Flow;

            return base.beforeUpdateInsertAction();
        }

        #region 移动.
        /// <summary>
        /// 上移
        /// </summary>
        /// <returns></returns>
        public string DoUp()
        {
            this.DoOrderUp(SubFlowAutoAttr.FK_Node, this.FK_Node.ToString(), SubFlowAutoAttr.SubFlowType, "1", SubFlowAutoAttr.Idx);
            return "执行成功";
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <returns></returns>
        public string DoDown()
        {
            this.DoOrderDown(SubFlowAutoAttr.FK_Node, this.FK_Node.ToString(), SubFlowAutoAttr.SubFlowType, "1", SubFlowAutoAttr.Idx);
            return "执行成功";
        }
        #endregion 移动.

    }
    /// <summary>
    /// 自动触发子流程集合
    /// </summary>
    public class SubFlowAutos : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SubFlowAuto();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 自动触发子流程集合
        /// </summary>
        public SubFlowAutos()
        {
        }
        /// <summary>
        /// 自动触发子流程集合.
        /// </summary>
        /// <param name="fk_node">节点</param>
        public SubFlowAutos(int fk_node)
        {
            this.Retrieve(SubFlowYanXuAttr.FK_Node, fk_node,
                SubFlowYanXuAttr.SubFlowType, (int)SubFlowType.AutoSubFlow, SubFlowYanXuAttr.Idx);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SubFlowAuto> ToJavaList()
        {
            return (System.Collections.Generic.IList<SubFlowAuto>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SubFlowAuto> Tolist()
        {
            System.Collections.Generic.List<SubFlowAuto> list = new System.Collections.Generic.List<SubFlowAuto>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SubFlowAuto)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
