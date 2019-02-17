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
    /// 配件属性
    /// </summary>
    public class PartParentSubGuideAttr:BP.En.EntityMyPKAttr
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
        public const string PartParentSubGuideType = "PartParentSubGuideType";
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
    public class PartParentSubGuide : EntityMyPK
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
                return this.GetValStringByKey(PartParentSubGuideAttr.FK_Flow);
            }
            set
            {
                SetValByKey(PartParentSubGuideAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public string PartParentSubGuideType
        {
            get
            {
                return this.GetValStringByKey(PartParentSubGuideAttr.PartParentSubGuideType);
            }
            set
            {
                SetValByKey(PartParentSubGuideAttr.PartParentSubGuideType, value);
            }
        }
        /// <summary>
        /// 节点ID
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(PartParentSubGuideAttr.FK_Node);
            }
            set
            {
                SetValByKey(PartParentSubGuideAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 字段存储0
        /// </summary>
        public string Tag0
        {
            get
            {
                return this.GetValStringByKey(PartParentSubGuideAttr.Tag0);
            }
            set
            {
                SetValByKey(PartParentSubGuideAttr.Tag0, value);
            }
        }
        /// <summary>
        /// 字段存储1
        /// </summary>
        public string Tag1
        {
            get
            {
                return this.GetValStringByKey(PartParentSubGuideAttr.Tag1);
            }
            set
            {
                SetValByKey(PartParentSubGuideAttr.Tag1, value);
            }
        }
        /// <summary>
        /// 字段存储2
        /// </summary>
        public string Tag2
        {
            get
            {
                return this.GetValStringByKey(PartParentSubGuideAttr.Tag2);
            }
            set
            {
                SetValByKey(PartParentSubGuideAttr.Tag2, value);
            }
        }
        /// <summary>
        /// 字段存储3
        /// </summary>
        public string Tag3
        {
            get
            {
                return this.GetValStringByKey(PartParentSubGuideAttr.Tag3);
            }
            set
            {
                SetValByKey(PartParentSubGuideAttr.Tag3, value);
            }
        }
        /// <summary>
        /// 字段存储4
        /// </summary>
        public string Tag4
        {
            get
            {
                return this.GetValStringByKey(PartParentSubGuideAttr.Tag4);
            }
            set
            {
                SetValByKey(PartParentSubGuideAttr.Tag4, value);
            }
        }
        /// <summary>
        /// 字段存储5
        /// </summary>
        public string Tag5
        {
            get
            {
                return this.GetValStringByKey(PartParentSubGuideAttr.Tag5);
            }
            set
            {
                SetValByKey(PartParentSubGuideAttr.Tag5, value);
            }
        }
        /// <summary>
        /// 字段存储6
        /// </summary>
        public string Tag6
        {
            get
            {
                return this.GetValStringByKey(PartParentSubGuideAttr.Tag6);
            }
            set
            {
                SetValByKey(PartParentSubGuideAttr.Tag6, value);
            }
        }
        /// <summary>
        /// 字段存储7
        /// </summary>
        public string Tag7
        {
            get
            {
                return this.GetValStringByKey(PartParentSubGuideAttr.Tag7);
            }
            set
            {
                SetValByKey(PartParentSubGuideAttr.Tag7, value);
            }
        }
        /// <summary>
        /// 字段存储8
        /// </summary>
        public string Tag8
        {
            get
            {
                return this.GetValStringByKey(PartParentSubGuideAttr.Tag8);
            }
            set
            {
                SetValByKey(PartParentSubGuideAttr.Tag8, value);
            }
        }
        /// <summary>
        /// 字段存储9
        /// </summary>
        public string Tag9
        {
            get
            {
                return this.GetValStringByKey(PartParentSubGuideAttr.Tag9);
            }
            set
            {
                SetValByKey(PartParentSubGuideAttr.Tag9, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 配件
        /// </summary>
        public PartParentSubGuide() { }
        /// <summary>
        /// 配件
        /// </summary>
        /// <param name="_oid">配件ID</param>	
        public PartParentSubGuide(string mypk)
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

                Map map = new Map("WF_Part", "前置导航-父子流程");

                map.AddMyPK();

                map.AddTBString(PartParentSubGuideAttr.FK_Flow, null, "流程编号", true, true, 0, 100, 10);
                map.AddTBString(PartParentSubGuideAttr.PartParentSubGuideType, null, "类型", false, true, 0, 100, 10);

                map.AddTBString(PartParentSubGuideAttr.Tag0, null, "流程编号", false, true, 0, 2000, 10);
                map.AddTBString(PartParentSubGuideAttr.Tag1, null, "流程名称", false, true, 0, 2000, 10);
                map.AddTBString(PartParentSubGuideAttr.Tag2, null, "隐藏查询条件", false, true, 0, 2000, 10);
                map.AddTBString(PartParentSubGuideAttr.Tag3, null, "显示的列", false, true, 0, 2000, 10);
                map.AddTBString(PartParentSubGuideAttr.Tag4, null, "Tag4", false, true, 0, 2000, 10);
                map.AddTBString(PartParentSubGuideAttr.Tag5, null, "Tag5", false, true, 0, 2000, 10);
                map.AddTBString(PartParentSubGuideAttr.Tag6, null, "Tag6", false, true, 0, 2000, 10);
                map.AddTBString(PartParentSubGuideAttr.Tag7, null, "Tag7", false, true, 0, 2000, 10);
                map.AddTBString(PartParentSubGuideAttr.Tag8, null, "Tag8", false, true, 0, 2000, 10);
                map.AddTBString(PartParentSubGuideAttr.Tag9, null, "Tag9", false, true, 0, 2000, 10);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 配件s
    /// </summary>
    public class PartParentSubGuides : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new PartParentSubGuide();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 配件集合
        /// </summary>
        public PartParentSubGuides()
        {
        }
        /// <summary>
        /// 配件集合.
        /// </summary>
        /// <param name="FlowNo"></param>
        public PartParentSubGuides(string fk_flow)
        {
            this.Retrieve(PartParentSubGuideAttr.FK_Flow, fk_flow);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<PartParentSubGuide> ToJavaList()
        {
            return (System.Collections.Generic.IList<PartParentSubGuide>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<PartParentSubGuide> Tolist()
        {
            System.Collections.Generic.List<PartParentSubGuide> list = new System.Collections.Generic.List<PartParentSubGuide>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((PartParentSubGuide)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
