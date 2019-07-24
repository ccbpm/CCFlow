using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 自动触发子流程属性
    /// </summary>
    public class SubFlowAutoAttr : SubFlowAttr
    {

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
        public string MainFlowNo
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.MainFlowNo);
            }
            set
            {
                SetValByKey(SubFlowAutoAttr.MainFlowNo, value);
            }
        }
        /// <summary>
        /// 流程编号
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
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(SubFlowAutoAttr.FlowName);
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

                map.AddTBInt(SubFlowHandAttr.FK_Node, 0, "节点", false, true);
                map.AddDDLSysEnum(SubFlowHandAttr.SubFlowType, 0, "子流程类型", true, false, SubFlowHandAttr.SubFlowType,
                "@0=手动启动子流程@1=触发启动子流程@2=延续子流程");

                map.AddTBString(SubFlowYanXuAttr.FK_Flow, null, "子流程编号", true, true, 0, 10, 150, false);
                map.AddTBString(SubFlowYanXuAttr.FlowName, null, "子流程名称", true, true, 0, 200, 150, false);

                map.AddDDLSysEnum(SubFlowYanXuAttr.SubFlowModel, 0, "子流程模式", true, true, SubFlowYanXuAttr.SubFlowModel,
                "@0=下级子流程@1=同级子流程");

                map.AddDDLSysEnum(FlowAttr.IsAutoSendSubFlowOver, 0, "结束规则", true, true,
                FlowAttr.IsAutoSendSubFlowOver, "@0=不处理@1=让父流程自动运行下一步@2=结束父流程");

                map.AddBoolean(SubFlowHandAttr.StartOnceOnly, false, "仅能被调用1次.", true, true, true);

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

                map.AddTBInt(SubFlowHandAttr.Idx, 0, "显示顺序", true, false);

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
            this.MyPK = this.FK_Node + "_" + this.FK_Flow + "_1";
            return base.beforeInsert();
        }

        protected override bool beforeUpdateInsertAction()
        {
            if (this.SendModel == 1)
            {
                //设置的发送到，发送到下一个节点上.

                Node nd = new Node(int.Parse(this.FK_Flow + "01"));

                Nodes tonds = nd.HisToNodes;
                foreach (Node item in tonds)
                {
                    if (item.HisDeliveryWay == DeliveryWay.BySelected)
                        throw new Exception("err@【自动发送方式】设置错误，您选择了[发送到下一个节点]但是该节点的接收人规则为由上一步发送人员选择，这是不符合规则的。");
                }
            }

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
            this.DoOrderDown(SubFlowAutoAttr.FK_Node, this.FK_Node.ToString(), SubFlowAutoAttr.SubFlowType, "2", SubFlowAutoAttr.Idx);
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
