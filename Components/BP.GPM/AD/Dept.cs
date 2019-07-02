using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;

namespace BP.GPM.AD
{
    /// <summary>
    /// 部门属性
    /// </summary>
    public class DeptAttr : EntityTreeAttr
    {
        /// <summary>
        /// 单位全名
        /// </summary>
        public const string NameOfPath = "NameOfPath";
    }
    /// <summary>
    /// 部门
    /// </summary>
    public class Dept : EntityTree
    {
        #region 属性
        /// <summary>
        /// 全名
        /// </summary>
        public string NameOfPath
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.NameOfPath);
            }
            set
            {
                this.SetValByKey(DeptAttr.NameOfPath, value);
            }
        }
        /// <summary>
        /// 父节点的ID
        /// </summary>
        public new string ParentNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.ParentNo, value);
            }
        }
        private Depts _HisSubDepts = null;
        /// <summary>
        /// 它的子节点
        /// </summary>
        public Depts HisSubDepts
        {
            get
            {
                if (_HisSubDepts == null)
                    _HisSubDepts = new Depts(this.No);
                return _HisSubDepts;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 部门
        /// </summary>
        public Dept() { }
        /// <summary>
        /// 部门
        /// </summary>
        /// <param name="no">编号</param>
        public Dept(string no) : base(no) { }
        #endregion

        #region 重写方法
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
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map();
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //连接到的那个数据库上. (默认的是: AppCenterDSN )
                map.PhysicsTable = "Port_Dept";
                map.Java_SetEnType(EnType.Admin);

                map.EnDesc = "部门"; //  实体的描述.
                map.Java_SetDepositaryOfEntity(Depositary.Application); //实体map的存放位置.
                map.Java_SetDepositaryOfMap(Depositary.Application);    // Map 的存放位置.

                map.AddTBStringPK(DeptAttr.No, null, "编号", true, true, 1, 50, 20);

                //比如xx分公司财务部
                map.AddTBString(DeptAttr.Name, null, "名称", true, false, 0, 100, 30);

                //比如:\\驰骋集团\\南方分公司\\财务部
                map.AddTBString(DeptAttr.NameOfPath, null, "部门路径", true, true, 0, 300, 30, true);

                map.AddTBString(DeptAttr.ParentNo, null, "父节点编号", true, false, 0, 100, 30);

                //顺序号.
                map.AddTBInt(DeptAttr.Idx, 0, "顺序号", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

    }
    /// <summary>
    ///部门集合
    /// </summary>
    public class Depts : EntitiesTree
    {
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Dept();
            }
        }
        /// <summary>
        /// 部门集合
        /// </summary>
        public Depts()
        {
        }
        /// <summary>
        /// 部门集合
        /// </summary>
        /// <param name="parentNo">父部门No</param>
        public Depts(string parentNo)
        {
            this.Retrieve(DeptAttr.ParentNo, parentNo);
        }
        public override int RetrieveAll()
        {
            QueryObject qo = new QueryObject(this);
            qo.addOrderBy(GPM.DeptAttr.Idx);
            return qo.DoQuery();
        }

        #region 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Dept> ToJavaList()
        {
            return (System.Collections.Generic.IList<Dept>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Dept> Tolist()
        {
            System.Collections.Generic.List<Dept> list = new System.Collections.Generic.List<Dept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Dept)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
    }
}
