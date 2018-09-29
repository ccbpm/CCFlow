using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程测试属性
    /// </summary>
    public class TestCaseDtlAttr : BP.En.EntityOIDNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Node = "FK_Node";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 参数类型
        /// </summary>
        public const string ParaType = "ParaType";
        /// <summary>
        /// Vals
        /// </summary>
        public const string Vals = "Vals";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 显示在那里？
        /// </summary>
        public const string ShowWhere = "ShowWhere";
        #endregion
    }
    /// <summary>
    /// 流程测试.
    /// </summary>
    public class TestCase : EntityMyPK
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
                return uac;
            }
        }
        /// <summary>
        /// 流程测试的事务编号
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(TestCaseDtlAttr.FK_Node);
            }
            set
            {
                SetValByKey(TestCaseDtlAttr.FK_Node, value);
            }
        }
        public string ParaType
        {
            get
            {
                return this.GetValStringByKey(TestCaseDtlAttr.ParaType);
            }
            set
            {
                SetValByKey(TestCaseDtlAttr.ParaType, value);
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(TestCaseDtlAttr.FK_Flow);
            }
            set
            {
                SetValByKey(TestCaseDtlAttr.FK_Flow, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 流程测试
        /// </summary>
        public TestCase() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_TestCase", "自定义流程测试");

                map.AddMyPK();
                map.AddTBString(TestCaseDtlAttr.FK_Flow, null, "流程编号", true, false, 0, 100, 100, true);
                map.AddTBString(TestCaseDtlAttr.ParaType, null, "参数类型", true, false, 0, 100, 100, true);
                map.AddTBString(TestCaseDtlAttr.Vals, null, "值s", true, false, 0, 500, 300, true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 流程测试集合
    /// </summary>
    public class TestCases : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new TestCase();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 流程测试集合
        /// </summary>
        public TestCases()
        {
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TestCase> ToJavaList()
        {
            return (System.Collections.Generic.IList<TestCase>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TestCase> Tolist()
        {
            System.Collections.Generic.List<TestCase> list = new System.Collections.Generic.List<TestCase>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TestCase)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
