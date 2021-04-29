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
    public class SubFlowHandGuideAttr : SubFlowHandAttr
    {
        /// <summary>
        /// 是否启用子流程发起前置导航.
        /// </summary>
        public const string IsSubFlowGuide = "IsSubFlowGuide";
        /// <summary>
        /// SQL 前置导航列表
        /// </summary>
        public const string SubFlowGuideSQL = "SubFlowGuideSQL";
        /// <summary>
        /// 分组的SQL
        /// </summary>
        public const string SubFlowGuideGroup = "SubFlowGuideGroup";
        /// <summary>
        /// 编号字段.
        /// </summary>
        public const string SubFlowGuideEnNoFiled = "SubFlowGuideEnNoFiled";
        /// <summary>
        /// 名称字段
        /// </summary>
        public const string SubFlowGuideEnNameFiled = "SubFlowGuideEnNameFiled";
        /// <summary>
        /// 是否是树形结构
        /// </summary>
        public const string IsTreeConstruct = "IsTreeConstruct";
        /// <summary>
        /// 父节点编号
        /// </summary>
        public const string ParentNo = "ParentNo";
    }
    /// <summary>
    /// 手工启动子流程.
    /// </summary>
    public class SubFlowHandGuide : EntityMyPK
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
                return this.GetValStringByKey(SubFlowHandAttr.SubFlowNo);
            }
            set
            {
                SetValByKey(SubFlowHandAttr.SubFlowNo, value);
            }
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string SubFlowName
        {
            get
            {
                return this.GetValStringByKey(SubFlowHandAttr.SubFlowName);
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

        public string SubFlowGuideEnNoFiled
        {
            get
            {
                return this.GetValStringByKey(SubFlowHandGuideAttr.SubFlowGuideEnNoFiled);
            }
        }

        public string SubFlowGuideEnNameFiled
        {
            get
            {
                return this.GetValStringByKey(SubFlowHandGuideAttr.SubFlowGuideEnNameFiled);
            }
        }

        public bool IsTreeConstruct
        {
            get
            {
                return this.GetValBooleanByKey(SubFlowHandGuideAttr.IsTreeConstruct);
            }
        }
        public string ParentNo
        {
            get
            {
                return this.GetValStringByKey(SubFlowHandGuideAttr.ParentNo);
            }
        }

        public bool SubFlowHidTodolist
        {
            get
            {
                return this.GetValBooleanByKey(SubFlowHandGuideAttr.SubFlowHidTodolist);
            }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 手工启动子流程
        /// </summary>
        public SubFlowHandGuide() { }
        public SubFlowHandGuide(string mypk)
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

                Map map = new Map("WF_NodeSubFlow", "启动子流程前置导航");

                map.AddMyPK();

                map.AddTBString(SubFlowAttr.FK_Flow, null, "主流程编号", true, true, 0, 5, 100);
                map.AddTBInt(SubFlowHandAttr.FK_Node, 0, "节点", false, true);

                map.AddTBString(SubFlowYanXuAttr.SubFlowNo, null, "子流程编号", false, true, 0, 10, 150, false);
                map.AddTBString(SubFlowYanXuAttr.SubFlowName, null, "子流程名称", false, true, 0, 200, 150, false);

                map.AddBoolean(SubFlowHandGuideAttr.IsSubFlowGuide, false, "是否启用子流程批量发起前置导航", true, true, true);
                map.AddTBString(SubFlowHandGuideAttr.SubFlowGuideSQL, null, "子流程前置导航配置SQL", true, false, 0, 200, 150, true);
                string msg = "发起前置导航的实体列表SQL, 必须包含No,Name两个列,与流程发起前置导航相同.";
                msg += "\t\n比如：SELECT No,Name FROM Port_Emp ";
                msg += "\t\nSQL配置支持ccbpm表达式.";
                map.SetHelperAlert(SubFlowHandGuideAttr.SubFlowGuideSQL, msg);


                map.AddTBString(SubFlowHandGuideAttr.SubFlowGuideGroup, null, "分组的SQL", true, false, 0, 200, 150, true);

                map.AddTBString(SubFlowHandGuideAttr.SubFlowGuideEnNoFiled, null, "实体No字段", true, false, 0, 40, 150);
                map.AddTBString(SubFlowHandGuideAttr.SubFlowGuideEnNameFiled, null, "实体Name字段", true, false, 0, 40, 150);

                //@0=单条手工启动, 1=按照简单数据源批量启动. @2=分组数据源批量启动. @3=树形结构批量启动.
                map.AddTBInt(SubFlowHandAttr.SubFlowStartModel, 0, "启动模式", false, false);

                //@0=表格模式, 1=列表模式.
                map.AddTBInt(SubFlowHandAttr.SubFlowShowModel, 0, "展现模式", false, false);
                //  map.Add(SubFlowHandAttr.IsTreeConstruct, 0, "是否是树结构", false, true);

                //批量发送后，是否隐藏父流程的待办. @yln.
                map.AddBoolean(SubFlowAttr.SubFlowHidTodolist, false, "发送后是否隐藏父流程待办",false,false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            //if (this.IsTreeConstruct== true
            //    && DataType.IsNullOrEmpty(this.ParentNo) == true)
            //{
            //    throw new Exception("请配置父节点的编号");
            //}
            return base.beforeUpdateInsertAction();
        }


    }
    /// <summary>
    /// 手工启动子流程集合
    /// </summary>
    public class SubFlowHandGuides : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SubFlowHandGuide();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 手工启动子流程集合
        /// </summary>
        public SubFlowHandGuides()
        {
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SubFlowHandGuide> ToJavaList()
        {
            return (System.Collections.Generic.IList<SubFlowHandGuide>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SubFlowHandGuide> Tolist()
        {
            System.Collections.Generic.List<SubFlowHandGuide> list = new System.Collections.Generic.List<SubFlowHandGuide>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SubFlowHandGuide)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
