using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 权限组人员
    /// </summary>
    public class GroupEmpAttr
    {
        /// <summary>
        /// 操作员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 权限组
        /// </summary>
        public const string FK_Group = "FK_Group";
    }
    /// <summary>
    /// 权限组人员
    /// </summary>
    public class GroupEmp : EntityMM
    {
        #region 属性
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(GroupEmpAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(GroupEmpAttr.FK_Emp, value);
            }
        }
        public string FK_Group
        {
            get
            {
                return this.GetValStringByKey(GroupEmpAttr.FK_Group);
            }
            set
            {
                this.SetValByKey(GroupEmpAttr.FK_Group, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限组人员
        /// </summary>
        public GroupEmp()
        {
        }
        /// <summary>
        /// 权限组人员
        /// </summary>
        /// <param name="mypk"></param>
        public GroupEmp(string no)
        {
            this.Retrieve();
        }
        /// <summary>
        /// 权限组人员
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("GPM_GroupEmp");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "权限组人员";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(GroupEmpAttr.FK_Group, null, "权限组", true, false, 0, 50, 20);
                map.AddDDLEntitiesPK(GroupEmpAttr.FK_Emp, null, "人员", new Emps(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 权限组人员s
    /// </summary>
    public class GroupEmps : EntitiesMM
    {
        #region 构造
        /// <summary>
        /// 权限组s
        /// </summary>
        public GroupEmps()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GroupEmp();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GroupEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<GroupEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GroupEmp> Tolist()
        {
            System.Collections.Generic.List<GroupEmp> list = new System.Collections.Generic.List<GroupEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GroupEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
