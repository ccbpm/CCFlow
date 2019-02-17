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
    /// 配件类型
    /// </summary>
    public class PartType 
    {
        /// <summary>
        /// 前置导航的父子流程关系
        /// </summary>
        public const string ParentSubFlowGuide = "ParentSubFlowGuide";
    }
    /// <summary>
    /// 配件属性
    /// </summary>
    public class PartAttr:BP.En.EntityMyPKAttr
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
        public const string PartType = "PartType";
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
    /// 配件.	 
    /// </summary>
    public class Part : EntityMyPK
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
        /// 配件的事务编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(PartAttr.FK_Flow);
            }
            set
            {
                SetValByKey(PartAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string PartType
        {
            get
            {
                return this.GetValStringByKey(PartAttr.PartType);
            }
            set
            {
                SetValByKey(PartAttr.PartType, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(PartAttr.FK_Node);
            }
            set
            {
                SetValByKey(PartAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 字段存储0
        /// </summary>
        public string Tag0
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag0);
            }
            set
            {
                SetValByKey(PartAttr.Tag0, value);
            }
        }
        /// <summary>
        /// 字段存储1
        /// </summary>
        public string Tag1
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag1);
            }
            set
            {
                SetValByKey(PartAttr.Tag1, value);
            }
        }
        /// <summary>
        /// 字段存储2
        /// </summary>
        public string Tag2
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag2);
            }
            set
            {
                SetValByKey(PartAttr.Tag2, value);
            }
        }
        /// <summary>
        /// 字段存储3
        /// </summary>
        public string Tag3
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag3);
            }
            set
            {
                SetValByKey(PartAttr.Tag3, value);
            }
        }
        /// <summary>
        /// 字段存储4
        /// </summary>
        public string Tag4
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag4);
            }
            set
            {
                SetValByKey(PartAttr.Tag4, value);
            }
        }
        /// <summary>
        /// 字段存储5
        /// </summary>
        public string Tag5
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag5);
            }
            set
            {
                SetValByKey(PartAttr.Tag5, value);
            }
        }
        /// <summary>
        /// 字段存储6
        /// </summary>
        public string Tag6
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag6);
            }
            set
            {
                SetValByKey(PartAttr.Tag6, value);
            }
        }
        /// <summary>
        /// 字段存储7
        /// </summary>
        public string Tag7
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag7);
            }
            set
            {
                SetValByKey(PartAttr.Tag7, value);
            }
        }
        /// <summary>
        /// 字段存储8
        /// </summary>
        public string Tag8
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag8);
            }
            set
            {
                SetValByKey(PartAttr.Tag8, value);
            }
        }
        /// <summary>
        /// 字段存储9
        /// </summary>
        public string Tag9
        {
            get
            {
                return this.GetValStringByKey(PartAttr.Tag9);
            }
            set
            {
                SetValByKey(PartAttr.Tag9, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 配件
        /// </summary>
        public Part() { }
        /// <summary>
        /// 配件
        /// </summary>
        /// <param name="_oid">配件ID</param>	
        public Part(string mypk)
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

                Map map = new Map("WF_Part", "配件");

                map.AddMyPK();

                map.AddTBString(PartAttr.FK_Flow, null, "流程编号", false, true, 0, 100, 10);
                map.AddTBInt(PartAttr.FK_Node, 0, "节点ID", false, false);
                map.AddTBString(PartAttr.PartType, null, "类型", false, true, 0, 100, 10);

                map.AddTBString(PartAttr.Tag0, null, "Tag0", false, true, 0, 2000, 10);
                map.AddTBString(PartAttr.Tag1, null, "Tag1", false, true, 0, 2000, 10);
                map.AddTBString(PartAttr.Tag2, null, "Tag2", false, true, 0, 2000, 10);
                map.AddTBString(PartAttr.Tag3, null, "Tag3", false, true, 0, 2000, 10);
                map.AddTBString(PartAttr.Tag4, null, "Tag4", false, true, 0, 2000, 10);
                map.AddTBString(PartAttr.Tag5, null, "Tag5", false, true, 0, 2000, 10);
                map.AddTBString(PartAttr.Tag6, null, "Tag6", false, true, 0, 2000, 10);
                map.AddTBString(PartAttr.Tag7, null, "Tag7", false, true, 0, 2000, 10);
                map.AddTBString(PartAttr.Tag8, null, "Tag8", false, true, 0, 2000, 10);
                map.AddTBString(PartAttr.Tag9, null, "Tag9", false, true, 0, 2000, 10);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 配件s
    /// </summary>
    public class Parts : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Part();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 配件集合
        /// </summary>
        public Parts()
        {
        }
        /// <summary>
        /// 配件集合.
        /// </summary>
        /// <param name="FlowNo"></param>
        public Parts(string fk_flow)
        {
            this.Retrieve(PartAttr.FK_Flow, fk_flow);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Part> ToJavaList()
        {
            return (System.Collections.Generic.IList<Part>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Part> Tolist()
        {
            System.Collections.Generic.List<Part> list = new System.Collections.Generic.List<Part>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Part)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
