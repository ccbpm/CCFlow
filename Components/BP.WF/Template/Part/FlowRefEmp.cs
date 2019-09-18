using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF.Template;
using System.Collections;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程关联人员属性
    /// </summary>
    public class FlowRefEmpAttr:BP.En.EntityMyPKAttr
    {
        #region 基本属性
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 前置导航的父子流程关系
        /// </summary>
        public const string FlowRefEmpType = "FlowRefEmpType";
        /// <summary>
        /// 字段存储0
        /// </summary>
        public const string Tag0 = "Tag0";
        /// <summary>
        /// 字段存储1
        /// </summary>
        public const string Tag1 = "Tag1";
        /// <summary>
        /// 字段存储2
        /// </summary>
        public const string Tag2 = "Tag2";
        /// <summary>
        /// 字段存储3
        /// </summary>
        public const string Tag3 = "Tag3";
        /// <summary>
        /// 字段存储4
        /// </summary>
        public const string Tag4 = "Tag4";
        /// <summary>
        /// 字段存储5
        /// </summary>
        public const string Tag5 = "Tag5";
        /// <summary>
        /// 字段存储6
        /// </summary>
        public const string Tag6 = "Tag6";
        /// <summary>
        /// 字段存储7
        /// </summary>
        public const string Tag7 = "Tag7";
        /// <summary>
        /// 字段存储8
        /// </summary>
        public const string Tag8 = "Tag8";
        /// <summary>
        /// 字段存储9
        /// </summary>
        public const string Tag9 = "Tag9";
        #endregion
    }
    /// <summary>
    /// 流程关联人员.	 
    /// </summary>
    public class FlowRefEmp : EntityMyPK
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
                uac.IsUpdate = true;
                return uac;
            }
        }
        /// <summary>
        /// 流程关联人员的事务编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(FlowRefEmpAttr.FK_Flow);
            }
            set
            {
                SetValByKey(FlowRefEmpAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string FlowRefEmpType
        {
            get
            {
                return this.GetValStringByKey(FlowRefEmpAttr.FlowRefEmpType);
            }
            set
            {
                SetValByKey(FlowRefEmpAttr.FlowRefEmpType, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(FlowRefEmpAttr.FK_Node);
            }
            set
            {
                SetValByKey(FlowRefEmpAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 字段存储0
        /// </summary>
        public string Tag0
        {
            get
            {
                return this.GetValStringByKey(FlowRefEmpAttr.Tag0);
            }
            set
            {
                SetValByKey(FlowRefEmpAttr.Tag0, value);
            }
        }
        /// <summary>
        /// 字段存储1
        /// </summary>
        public string Tag1
        {
            get
            {
                return this.GetValStringByKey(FlowRefEmpAttr.Tag1);
            }
            set
            {
                SetValByKey(FlowRefEmpAttr.Tag1, value);
            }
        }
        /// <summary>
        /// 字段存储2
        /// </summary>
        public string Tag2
        {
            get
            {
                return this.GetValStringByKey(FlowRefEmpAttr.Tag2);
            }
            set
            {
                SetValByKey(FlowRefEmpAttr.Tag2, value);
            }
        }
        /// <summary>
        /// 字段存储3
        /// </summary>
        public string Tag3
        {
            get
            {
                return this.GetValStringByKey(FlowRefEmpAttr.Tag3);
            }
            set
            {
                SetValByKey(FlowRefEmpAttr.Tag3, value);
            }
        }
        /// <summary>
        /// 字段存储4
        /// </summary>
        public string Tag4
        {
            get
            {
                return this.GetValStringByKey(FlowRefEmpAttr.Tag4);
            }
            set
            {
                SetValByKey(FlowRefEmpAttr.Tag4, value);
            }
        }
        /// <summary>
        /// 字段存储5
        /// </summary>
        public string Tag5
        {
            get
            {
                return this.GetValStringByKey(FlowRefEmpAttr.Tag5);
            }
            set
            {
                SetValByKey(FlowRefEmpAttr.Tag5, value);
            }
        }
        /// <summary>
        /// 字段存储6
        /// </summary>
        public string Tag6
        {
            get
            {
                return this.GetValStringByKey(FlowRefEmpAttr.Tag6);
            }
            set
            {
                SetValByKey(FlowRefEmpAttr.Tag6, value);
            }
        }
        /// <summary>
        /// 字段存储7
        /// </summary>
        public string Tag7
        {
            get
            {
                return this.GetValStringByKey(FlowRefEmpAttr.Tag7);
            }
            set
            {
                SetValByKey(FlowRefEmpAttr.Tag7, value);
            }
        }
        /// <summary>
        /// 字段存储8
        /// </summary>
        public string Tag8
        {
            get
            {
                return this.GetValStringByKey(FlowRefEmpAttr.Tag8);
            }
            set
            {
                SetValByKey(FlowRefEmpAttr.Tag8, value);
            }
        }
        /// <summary>
        /// 字段存储9
        /// </summary>
        public string Tag9
        {
            get
            {
                return this.GetValStringByKey(FlowRefEmpAttr.Tag9);
            }
            set
            {
                SetValByKey(FlowRefEmpAttr.Tag9, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 流程关联人员
        /// </summary>
        public FlowRefEmp() { }
        /// <summary>
        /// 流程关联人员
        /// </summary>
        /// <param name="_oid">流程关联人员ID</param>	
        public FlowRefEmp(string mypk)
        {
            this.MyPK = mypk;
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

                Map map = new Map("WF_Part", "流程关联人员");

                map.AddMyPK();

                map.AddTBString(PartAttr.FK_Flow, null, "流程编号", false, true, 0, 100, 10);
                map.AddTBInt(PartAttr.FK_Node, 0, "节点ID", false, false);
                map.AddTBString(PartAttr.PartType, null, "类型", false, true, 0, 100, 10);

                map.AddTBString(FlowRefEmpAttr.Tag0, null, "Tag0", false, true, 0, 2000, 10);
                map.AddTBString(FlowRefEmpAttr.Tag1, null, "Tag1", false, true, 0, 2000, 10);
                map.AddTBString(FlowRefEmpAttr.Tag2, null, "Tag2", false, true, 0, 2000, 10);
                map.AddTBString(FlowRefEmpAttr.Tag3, null, "Tag3", false, true, 0, 2000, 10);
                map.AddTBString(FlowRefEmpAttr.Tag4, null, "Tag4", false, true, 0, 2000, 10);
                map.AddTBString(FlowRefEmpAttr.Tag5, null, "Tag5", false, true, 0, 2000, 10);
                map.AddTBString(FlowRefEmpAttr.Tag6, null, "Tag6", false, true, 0, 2000, 10);
                map.AddTBString(FlowRefEmpAttr.Tag7, null, "Tag7", false, true, 0, 2000, 10);
                map.AddTBString(FlowRefEmpAttr.Tag8, null, "Tag8", false, true, 0, 2000, 10);
                map.AddTBString(FlowRefEmpAttr.Tag9, null, "Tag9", false, true, 0, 2000, 10);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 流程关联人员s
    /// </summary>
    public class FlowRefEmps : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FlowRefEmp();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 流程关联人员集合
        /// </summary>
        public FlowRefEmps()
        {
        }
        /// <summary>
        /// 流程关联人员集合.
        /// </summary>
        /// <param name="FlowNo"></param>
        public FlowRefEmps(string fk_flow)
        {
            this.Retrieve(FlowRefEmpAttr.FK_Flow, fk_flow);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FlowRefEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<FlowRefEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FlowRefEmp> Tolist()
        {
            System.Collections.Generic.List<FlowRefEmp> list = new System.Collections.Generic.List<FlowRefEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FlowRefEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
