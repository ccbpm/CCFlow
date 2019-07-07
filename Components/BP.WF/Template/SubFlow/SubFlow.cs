using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 子流程属性
    /// </summary>
    public class SubFlowAttr : BP.En.EntityOIDNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 流程名称
        /// </summary>
        public const string FlowName = "FlowName";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 显示在那里？
        /// </summary>
        public const string YGWorkWay = "YGWorkWay";
        /// <summary>
        /// 主流程编号
        /// </summary>
        public const string MainFlowNo = "MainFlowNo";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 表达式类型
        /// </summary>
        public const string ExpType = "ExpType";
        /// <summary>
        /// 条件表达式
        /// </summary>
        public const string CondExp = "CondExp";

        /// <summary>
        /// 越轨子流程退回类型
        /// </summary>
        public const string YBFlowReturnRole = "YBFlowReturnRole";
        /// <summary>
        /// 要退回的节点
        /// </summary>
        public const string ReturnToNode = "ReturnToNode";
        /// <summary>
        /// 子流程类型
        /// </summary>
        public const string SubFlowType = "SubFlowType";
        /// <summary>
        /// 子流程模式
        /// </summary>
        public const string SubFlowModel = "SubFlowModel";
        #endregion

        #region 子流程的发起.
        /// <summary>
        /// 如果当前为子流程，仅仅只能被调用1次，不能被重复调用。
        /// </summary>
        public const string StartOnceOnly = "StartOnceOnly";
        /// <summary>
        /// 是否启动
        /// </summary>
        public const string IsEnableSpecFlowStart = "IsEnableSpecFlowStart";
        /// <summary>
        /// 指定的流程启动后，才能启动该子流程.
        /// </summary>
        public const string SpecFlowStart = "SpecFlowStart";
        /// <summary>
        /// 备注
        /// </summary>
        public const string SpecFlowStartNote = "SpecFlowStartNote";
        /// <summary>
        /// 是否启用
        /// </summary>
        public const string IsEnableSpecFlowOver = "IsEnableSpecFlowOver";
        /// <summary>
        /// 指定的子流程结束后，才能启动该子流程.
        /// </summary>
        public const string SpecFlowOver = "SpecFlowOver";
        /// <summary>
        /// 备注
        /// </summary>
        public const string SpecFlowOverNote = "SpecFlowOverNote";
        #endregion

        /// <summary>
        /// 自动启动子流程：发送规则.
        /// </summary>
        public const string SendModel = "SendModel";

    }
    /// <summary>
    /// 子流程.
    /// </summary>
    public class SubFlow : EntityMyPK
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
        /// 子流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(SubFlowYanXuAttr.FK_Flow);
            }
            set
            {
                SetValByKey(SubFlowYanXuAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string FlowName
        {
            get
            {
                return this.GetValStringByKey(SubFlowYanXuAttr.FlowName);
            }
        }
        /// <summary>
        /// 条件表达式.
        /// </summary>
        public string CondExp
        {
            get
            {
                return this.GetValStringByKey(SubFlowYanXuAttr.CondExp);
            }
            set
            {
                SetValByKey(SubFlowYanXuAttr.CondExp, value);
            }
        }
        /// <summary>
        /// 表达式类型
        /// </summary>
        public ConnDataFrom ExpType
        {
            get
            {
                return (ConnDataFrom)this.GetValIntByKey(SubFlowYanXuAttr.ExpType);
            }
            set
            {
                SetValByKey(SubFlowYanXuAttr.ExpType, (int)value);
            }
        }
        /// <summary>
        /// 主流程编号
        /// </summary>
        public string MainFlowNo
        {
            get
            {
                return this.GetValStringByKey(SubFlowYanXuAttr.MainFlowNo);
            }
            set
            {
                SetValByKey(SubFlowYanXuAttr.MainFlowNo, value);
            }
        }
        /// <summary>
        /// 主流程NodeID
        /// </summary>
        public string FK_Node
        {
            get
            {
                return this.GetValStringByKey(SubFlowYanXuAttr.FK_Node);
            }
            set
            {
                SetValByKey(SubFlowYanXuAttr.FK_Node, value);
            }
        }
        public SubFlowType SubFlowType
        {
            get
            {
                return (SubFlowType)this.GetValIntByKey(SubFlowYanXuAttr.SubFlowType);
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
        /// <summary>
        /// 仅仅可以启动一次?
        /// </summary>
        public bool StartOnceOnly
        {
            get
            {
                return this.GetValBooleanByKey(SubFlowYanXuAttr.StartOnceOnly);
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
                return this.GetValStringByKey(SubFlowYanXuAttr.SpecFlowOver);
            }
        }
        public string SpecFlowStart
        {
            get
            {
                return this.GetValStringByKey(SubFlowYanXuAttr.SpecFlowStart);
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
        /// 子流程
        /// </summary>
        public SubFlow() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeSubFlow", "子流程(所有类型子流程属性)");

                map.AddMyPK();

                map.AddTBString(SubFlowAttr.MainFlowNo, null, "主流程编号", true, false, 0, 10, 150, true);
                map.AddTBInt(SubFlowAttr.FK_Node, 0, "主流程节点", false, true);

                map.AddTBInt(SubFlowAttr.SubFlowType, 0, "子流程类型", false, true);
                map.AddTBInt(SubFlowAttr.SubFlowModel, 0, "子流程模式", false, true);
                  
                map.AddTBString(SubFlowAttr.FK_Flow, null, "子流程编号", true, true, 0, 10, 150, false);
                map.AddTBString(SubFlowAttr.FlowName, null, "子流程名称", true, true, 0, 200, 150, false);

                //启动限制规则0.
                map.AddTBInt(SubFlowAttr.StartOnceOnly, 0, "仅能被调用1次", false, true);

                //启动限制规则1.
                map.AddTBInt(SubFlowAttr.IsEnableSpecFlowStart, 0, "指定的流程启动后,才能启动该子流程(请在文本框配置子流程).", false, true);
                map.AddTBString(SubFlowHandAttr.SpecFlowStart, null, "子流程编号", true, false, 0, 200, 150, true);
                map.AddTBString(SubFlowHandAttr.SpecFlowStartNote, null, "备注", true, false, 0, 500, 150, true);

                //启动限制规则2.
                map.AddTBInt(SubFlowHandAttr.IsEnableSpecFlowOver, 0, "指定的流程结束后,才能启动该子流程(请在文本框配置子流程).", true, true);
                map.AddTBString(SubFlowHandAttr.SpecFlowOver, null, "子流程编号", true, false, 0, 200, 150, true);
                map.AddTBString(SubFlowHandAttr.SpecFlowOverNote, null, "备注", true, false, 0, 500, 150, true);


                map.AddTBInt(SubFlowAttr.ExpType, 0, "表达式类型", false, true);
                map.AddTBString(SubFlowAttr.CondExp, null, "条件表达式", true, false, 0, 500, 150, true);

                map.AddTBInt(SubFlowAttr.YBFlowReturnRole, 0, "退回方式", false, true);

                map.AddTBString(SubFlowAttr.ReturnToNode, null, "要退回的节点", true, true, 0, 200, 150, false);

                map.AddTBInt(SubFlowAttr.SendModel, 0, "自动触发的子流程发送方式", false, true);

                map.AddTBInt(SubFlowAttr.Idx, 0, "顺序", true, false);
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
            this.MyPK = this.FK_Node + "_" + this.FK_Flow + "_"+this.SubFlowType;
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 子流程集合
    /// </summary>
    public class SubFlows : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SubFlow();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 子流程集合
        /// </summary>
        public SubFlows()
        {
        }
         /// <summary>
        /// 子流程集合.
        /// </summary>
        /// <param name="fk_node">节点</param>
        public SubFlows(int fk_node)
        {
            this.Retrieve(SubFlowYanXuAttr.FK_Node, fk_node);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SubFlow> ToJavaList()
        {
            return (System.Collections.Generic.IList<SubFlow>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SubFlow> Tolist()
        {
            System.Collections.Generic.List<SubFlow> list = new System.Collections.Generic.List<SubFlow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SubFlow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
