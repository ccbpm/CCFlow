using BP.En;
using BP.Port;

namespace BP.CCFast.CCMenu
{
    /// <summary>
    /// 权限组Dept
    /// </summary>
    public class GroupDeptAttr
    {
        /// <summary>
        /// Dept
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 权限组
        /// </summary>
        public const string FK_Group = "FK_Group";
    }
    /// <summary>
    /// 权限组部门
    /// </summary>
    public class GroupDept : EntityMM
    {
        #region 属性
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(GroupDeptAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(GroupDeptAttr.FK_Dept, value);
            }
        }
        public string FK_Group
        {
            get
            {
                return this.GetValStringByKey(GroupDeptAttr.FK_Group);
            }
            set
            {
                this.SetValByKey(GroupDeptAttr.FK_Group, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限组部门
        /// </summary>
        public GroupDept()
        {
        }
      
        /// <summary>
        /// 权限组Dept
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("GPM_GroupDept", "权限组部门");

                map.AddTBStringPK(GroupDeptAttr.FK_Group, null, "权限组", false, false, 0, 50, 20);
                map.AddDDLEntitiesPK(GroupDeptAttr.FK_Dept, null, "部门", new Depts(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 权限组部门s
    /// </summary>
    public class GroupDepts : EntitiesMM
    {
        #region 构造
        /// <summary>
        /// 权限组s
        /// </summary>
        public GroupDepts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GroupDept();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GroupDept> ToJavaList()
        {
            return (System.Collections.Generic.IList<GroupDept>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GroupDept> Tolist()
        {
            System.Collections.Generic.List<GroupDept> list = new System.Collections.Generic.List<GroupDept>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GroupDept)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
