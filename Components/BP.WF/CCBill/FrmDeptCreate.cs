using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.En;
using BP.WF.Port;

namespace BP.WF.CCBill
{
    /// <summary>
    /// 单据可创建的部门属性	  
    /// </summary>
    public class FrmDeptCreateAttr
    {
        /// <summary>
        /// 单据
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
    }
    /// <summary>
    /// 单据可创建的部门
    /// 单据的部门有两部分组成.	 
    /// 记录了从一个单据到其他的多个单据.
    /// 也记录了到这个单据的其他的单据.
    /// </summary>
    public class FrmDeptCreate : EntityMM
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
                uac.OpenAll();
                return uac;
            }
        }
        /// <summary>
        ///单据
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(FrmDeptCreateAttr.FrmID);
            }
            set
            {
                this.SetValByKey(FrmDeptCreateAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 部门
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(FrmDeptCreateAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(FrmDeptCreateAttr.FK_Dept, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 单据可创建的部门
        /// </summary>
        public FrmDeptCreate() { }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_FrmDeptCreate", "单据部门");

                map.AddTBStringPK(FrmDeptCreateAttr.FrmID, null, "表单", true, true, 1, 100, 100);
                map.AddDDLEntitiesPK(FrmDeptCreateAttr.FK_Dept, null, "可以创建部门",
                   new BP.GPM.Depts(), true);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    /// 单据可创建的部门
    /// </summary>
    public class FrmDeptCreates : EntitiesMM
    {
        #region 构造函数.
        /// <summary>
        /// 单据可创建的部门
        /// </summary>
        public FrmDeptCreates() { }
        /// <summary>
        /// 单据可创建的部门
        /// </summary>
        /// <param name="nodeID">单据ID</param>
        public FrmDeptCreates(int nodeID)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmDeptCreateAttr.FrmID, nodeID);
            qo.DoQuery();
        }
        /// <summary>
        /// 单据可创建的部门
        /// </summary>
        /// <param name="StationNo">StationNo </param>
        public FrmDeptCreates(string StationNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmDeptCreateAttr.FK_Dept, StationNo);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmDeptCreate();
            }
        }
        #endregion 构造函数.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmDeptCreate> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmDeptCreate>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmDeptCreate> Tolist()
        {
            System.Collections.Generic.List<FrmDeptCreate> list = new System.Collections.Generic.List<FrmDeptCreate>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmDeptCreate)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
