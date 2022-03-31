﻿using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.GPM.Menu2020
{
    /// <summary>
    /// 权限组
    /// </summary>
    public class GroupAttr : EntityTreeAttr
    {
        public const string ByEmpAttr = "ByEmpAttr";
    }
    /// <summary>
    /// 权限组
    /// </summary>
    public class Group : EntityNoName
    {
        #region 按钮权限控制
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAppAdmin();
                return uac;
            }
        }
        #endregion

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
        /// <param name="no"></param>
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

                Map map = new Map("GPM_Group", "权限组");
                map.setIsAutoGenerNo( true);

                map.AddTBStringPK(GroupAttr.No, null, "编号", true, true, 3, 3, 3);
                map.AddTBString(GroupAttr.Name, null, "名称", true, false, 0, 300, 20);
             //   map.AddTBString(GroupAttr.ParentNo, null, "父亲节编号", true, true, 0, 100, 20);
                map.AddTBInt(GroupAttr.Idx, 0, "显示顺序", true, false);


                map.AttrsOfOneVSM.Add(new GroupEmps(), new Emps(),
                    GroupEmpAttr.FK_Group, GroupEmpAttr.FK_Emp, EmpAttr.Name, EmpAttr.No, "人员(简单)");


                //节点绑定人员. 使用树杆与叶子的模式绑定.
                map.AttrsOfOneVSM.AddBranchesAndLeaf(new GroupEmps(), new BP.Port.Emps(),
                   GroupEmpAttr.FK_Group,
                   GroupEmpAttr.FK_Emp, "人员(树)", EmpAttr.FK_Dept, EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");

                //map.AttrsOfOneVSM.Add(new GroupEmps(), new Emps(),
                // GroupEmpAttr.FK_Group, GroupEmpAttr.FK_Emp, EmpAttr.Name, EmpAttr.No, "人员(简单)");

                map.AttrsOfOneVSM.Add(new GroupStations(), new BP.Port.Stations(),
                    GroupEmpAttr.FK_Group, GroupStationAttr.FK_Station, EmpAttr.Name, EmpAttr.No, "岗位(简单)");
                    

                map.AttrsOfOneVSM.AddGroupListModel(new GroupStations(), new BP.Port.Stations(),
                  GroupStationAttr.FK_Group,
                  GroupStationAttr.FK_Station, "岗位(平铺)", BP.Port.StationAttr.FK_StationType);

                map.AttrsOfOneVSM.AddBranches(new GroupDepts(), new Depts(),
                   GroupEmpAttr.FK_Group, GroupDeptAttr.FK_Dept, "部门(树)", EmpAttr.Name, EmpAttr.No);


                //节点绑定部门. 节点绑定部门.
                map.AttrsOfOneVSM.AddBranches(new GroupMenus(), new Menus(),
                   GroupMenuAttr.FK_Group,
                   GroupMenuAttr.FK_Menu, "绑定菜单", EmpAttr.Name, EmpAttr.No, "0");


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
