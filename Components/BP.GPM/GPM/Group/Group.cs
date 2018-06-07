using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 权限组
    /// </summary>
    public class GroupAttr : EntitySimpleTreeAttr
    {
        /// <summary>
        /// 序号
        /// </summary>
        public const string Idx = "Idx";
    }
    /// <summary>
    /// 权限组
    /// </summary>
    public class Group : EntityNoName
    {
        #region 构造方法
        /// <summary>
        /// 权限组
        /// </summary>
        public Group()
        {
        }
        /// <summary>
        /// 权限组
        /// </summary>
        /// <param name="mypk"></param>
        public Group(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("GPM_Group");
                map.DepositaryOfEntity = Depositary.None;
                map.EnDesc = "权限组";
                map.EnType = EnType.Sys;
                map.IsAutoGenerNo = true;

                map.AddTBStringPK(GroupAttr.No, null, "编号", true, true, 3, 3, 3);
                map.AddTBString(GroupAttr.Name, null, "名称", true, false, 0, 300, 20);
                map.AddTBString(GroupAttr.ParentNo, null, "父亲节编号", false, false, 0, 3, 20);
                map.AddTBInt(GroupAttr.Idx, 0, "显示顺序", true, false);

                map.AttrsOfOneVSM.Add(new GroupEmps(), new Emps(),
                    GroupEmpAttr.FK_Group, ByEmpAttr.FK_Emp, EmpAttr.Name, EmpAttr.No, "人员");

                map.AttrsOfOneVSM.Add(new GroupStations(), new Stations(),
                    GroupEmpAttr.FK_Group, GroupStationAttr.FK_Station, EmpAttr.Name, EmpAttr.No, "岗位");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 权限组s
    /// </summary>
    public class Groups : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 权限组s
        /// </summary>
        public Groups()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Group();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Group> ToJavaList()
        {
            return (System.Collections.Generic.IList<Group>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Group> Tolist()
        {
            System.Collections.Generic.List<Group> list = new System.Collections.Generic.List<Group>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Group)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
