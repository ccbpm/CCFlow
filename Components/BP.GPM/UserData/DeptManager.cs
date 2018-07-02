using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 部门管理员
    /// </summary>
    public class DeptManagerAttr
    {
        /// <summary>
        /// 操作员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
    }
    /// <summary>
    /// 部门管理员
    /// </summary>
    public class DeptManager : EntityMyPK
    {
        #region 属性
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(DeptManagerAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(DeptManagerAttr.FK_Emp, value);
            }
        }
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(DeptManagerAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(DeptManagerAttr.FK_Dept, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 部门管理员
        /// </summary>
        public DeptManager()
        {
        }
        /// <summary>
        /// 部门管理员
        /// </summary>
        /// <param name="mypk"></param>
        public DeptManager(string no)
        {
            this.Retrieve();
        }
        /// <summary>
        /// 部门管理员
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("GPM_DeptManager");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "部门管理员";
                map.EnType = EnType.Sys;

                map.AddMyPK();

                map.AddTBString(DeptManagerAttr.FK_Emp, null, "操作员", true, false, 0, 50, 20);
                map.AddTBString(DeptManagerAttr.FK_Dept, null, "部门", true, false, 0, 50, 20);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 部门管理员s
    /// </summary>
    public class DeptManagers : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 人员s
        /// </summary>
        public DeptManagers()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DeptManager();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DeptManager> ToJavaList()
        {
            return (System.Collections.Generic.IList<DeptManager>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DeptManager> Tolist()
        {
            System.Collections.Generic.List<DeptManager> list = new System.Collections.Generic.List<DeptManager>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DeptManager)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
