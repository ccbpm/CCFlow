using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 手工启动子流程属性
    /// </summary>
    public class SubFlowHandAttr : SubFlowAttr
    {


    }
    /// <summary>
    /// 手工启动子流程.
    /// </summary>
    public class SubFlowHand : EntityMyPK
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
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(SubFlowHandAttr.FK_Flow);
            }
            set
            {
                SetValByKey(SubFlowHandAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValRefTextByKey(SubFlowHandAttr.FK_Flow);
            }
        }
        /// <summary>
        /// 条件表达式.
        /// </summary>
        public string CondExp
        {
            get
            {
                return this.GetValStringByKey(SubFlowHandAttr.CondExp);
            }
            set
            {
                SetValByKey(SubFlowHandAttr.CondExp, value);
            }
        }
        /// <summary>
        /// 表达式类型
        /// </summary>
        public ConnDataFrom ExpType
        {
            get
            {
                return (ConnDataFrom)this.GetValIntByKey(SubFlowHandAttr.ExpType);
            }
            set
            {
                SetValByKey(SubFlowHandAttr.ExpType, (int)value);
            }
        }
        public string FK_Node
        {
            get
            {
                return this.GetValStringByKey(SubFlowHandAttr.FK_Node);
            }
            set
            {
                SetValByKey(SubFlowHandAttr.FK_Node, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 手工启动子流程
        /// </summary>
        public SubFlowHand() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeSubFlow", "手动启动子流程");

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

                map.AddBoolean(SubFlowHandAttr.StartOnceOnly, false, "发起限制规则:如果当前为子流程,仅能被调用1次.",
                    true, true, true);

                //启动限制规则.
                map.AddBoolean(SubFlowHandAttr.IsEnableSpecFlowStart, false, "指定的流程启动后,才能启动该子流程(请在文本框配置子流程).",
                 true, true, true);
                map.AddTBString(SubFlowHandAttr.SpecFlowStart, null, "子流程编号", true, false, 0, 200, 150, true);
                map.SetHelperAlert(SubFlowHandAttr.SpecFlowStart, "指定的流程启动后，才能启动该子流程，多个子流程用逗号分开. 001,002");

                //启动限制规则.
                map.AddBoolean(SubFlowHandAttr.IsEnableSpecFlowOver, false, "指定的流程结束后,才能启动该子流程(请在文本框配置子流程).",
                 true, true, true);
                map.AddTBString(SubFlowHandAttr.SpecFlowOver, null, "子流程编号", true, false, 0, 200, 150, true);
                map.SetHelperAlert(SubFlowHandAttr.SpecFlowOver, "指定的流程结束后，才能启动该子流程，多个子流程用逗号分开. 001,002");

                map.AddTBInt(SubFlowHandAttr.Idx, 0, "显示顺序", true, false);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.MyPK = this.FK_Node + "_" + this.FK_Flow + "_0";
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 手工启动子流程集合
    /// </summary>
    public class SubFlowHands : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SubFlowHand();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 手工启动子流程集合
        /// </summary>
        public SubFlowHands()
        {
        }
        /// <summary>
        /// 手工启动子流程集合
        /// </summary>
        /// <param name="fk_node">节点ID</param>
        public SubFlowHands(int fk_node)
        {
            this.Retrieve(SubFlowYanXuAttr.FK_Node, fk_node,
                SubFlowYanXuAttr.SubFlowType, (int)SubFlowType.HandSubFlow, SubFlowYanXuAttr.Idx);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SubFlowHand> ToJavaList()
        {
            return (System.Collections.Generic.IList<SubFlowHand>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SubFlowHand> Tolist()
        {
            System.Collections.Generic.List<SubFlowHand> list = new System.Collections.Generic.List<SubFlowHand>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SubFlowHand)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
