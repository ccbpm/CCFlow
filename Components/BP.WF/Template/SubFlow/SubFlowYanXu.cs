using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
  
    /// <summary>
    /// 延续子流程属性
    /// </summary>
    public class SubFlowYanXuAttr :SubFlowAttr
    {
    }
    /// <summary>
    /// 延续子流程.
    /// </summary>
    public class SubFlowYanXu : EntityMyPK
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
                return this.GetValStringByKey(SubFlowYanXuAttr.FK_Flow);
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
        #endregion

        #region 构造函数
        /// <summary>
        /// 延续子流程
        /// </summary>
        public SubFlowYanXu() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_NodeSubFlow", "延续子流程");

                map.AddMyPK();


                map.AddTBInt(SubFlowYanXuAttr.FK_Node, 0, "节点", false, true);
                map.AddDDLSysEnum(SubFlowYanXuAttr.SubFlowType, 2, "子流程类型", true, false, SubFlowYanXuAttr.SubFlowType,
                "@0=手动启动子流程@1=触发启动子流程@2=延续子流程");

                map.AddDDLSysEnum(SubFlowYanXuAttr.SubFlowModel, 0, "子流程模式", true, false, SubFlowYanXuAttr.SubFlowModel,
                "@0=下级子流程@1=同级子流程");
                

                map.AddTBString(SubFlowYanXuAttr.FK_Flow, null, "子流程编号", true, true, 0, 10, 150, false);
                map.AddTBString(SubFlowYanXuAttr.FlowName, null, "子流程名称", true, true, 0, 200, 150, false);

                map.AddDDLSysEnum(SubFlowYanXuAttr.ExpType, 3, "表达式类型", true, true, SubFlowYanXuAttr.ExpType,
                   "@3=按照SQL计算@4=按照参数计算");

                map.AddTBString(SubFlowYanXuAttr.CondExp, null, "条件表达式", true, false, 0, 500, 150, true);

                //@du.
                map.AddDDLSysEnum(SubFlowYanXuAttr.YBFlowReturnRole, 0, "退回方式", true, true, SubFlowYanXuAttr.YBFlowReturnRole,
                  "@0=不能退回@1=退回到父流程的开始节点@2=退回到父流程的任何节点@3=退回父流程的启动节点@4=可退回到指定的节点");

                // map.AddTBString(SubFlowYanXuAttr.ReturnToNode, null, "要退回的节点", true, false, 0, 200, 150, true);
                map.AddDDLSQL(SubFlowYanXuAttr.ReturnToNode, "0", "要退回的节点",
                    "SELECT NodeID AS No, Name FROM WF_Node WHERE FK_Flow IN (SELECT FK_Flow FROM WF_Node WHERE NodeID=@FK_Node; )", true);

                map.AddTBInt(SubFlowYanXuAttr.Idx, 0, "显示顺序", true, false);
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
            this.MyPK = this.FK_Node + "_" + this.FK_Flow + "_2";
            return base.beforeInsert();
        }

        #region 移动.
        /// <summary>
        /// 上移
        /// </summary>
        /// <returns></returns>
        public string DoUp()
        {
            this.DoOrderUp(SubFlowYanXuAttr.FK_Node, this.FK_Node, SubFlowYanXuAttr.SubFlowType, "2", SubFlowYanXuAttr.Idx);
            return "执行成功";
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <returns></returns>
        public string DoDown()
        {
            this.DoOrderDown(SubFlowYanXuAttr.FK_Node, this.FK_Node, SubFlowYanXuAttr.SubFlowType, "2", SubFlowYanXuAttr.Idx);
            return "执行成功";
        }
        #endregion 移动.

    }
    /// <summary>
    /// 延续子流程集合
    /// </summary>
    public class SubFlowYanXus : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SubFlowYanXu();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 延续子流程集合
        /// </summary>
        public SubFlowYanXus()
        {
        }
        /// <summary>
        /// 延续子流程集合.
        /// </summary>
        /// <param name="fk_node"></param>
        public SubFlowYanXus(int fk_node)
        {
            this.Retrieve(SubFlowYanXuAttr.FK_Node, fk_node, 
                SubFlowYanXuAttr.SubFlowType, (int)SubFlowType.YanXuFlow);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SubFlowYanXu> ToJavaList()
        {
            return (System.Collections.Generic.IList<SubFlowYanXu>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SubFlowYanXu> Tolist()
        {
            System.Collections.Generic.List<SubFlowYanXu> list = new System.Collections.Generic.List<SubFlowYanXu>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SubFlowYanXu)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
