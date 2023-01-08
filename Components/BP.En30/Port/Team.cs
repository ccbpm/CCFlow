using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;


namespace BP.Port
{
    /// <summary>
    /// 用户组
    /// </summary>
    public class TeamAttr : EntityTreeAttr
    {
        public const string FK_TeamType = "FK_TeamType";
    }
    /// <summary>
    /// 用户组
    /// </summary>
    public class Team : EntityNoName
    {
        #region 构造方法
        /// <summary>
        /// 类型
        /// </summary>
        public string FK_TeamType
        {
            get
            {
                return this.GetValStringByKey(TeamAttr.FK_TeamType);
            }
        }
        /// <summary>
        /// 用户组
        /// </summary>
        public Team()
        {
        }
        /// <summary>
        /// 用户组
        /// </summary>
        /// <param name="no"></param>
        public Team(string no)
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

                Map map = new Map("Port_Team", "标签");
                map.setEnType(EnType.Sys);
                map.setIsAutoGenerNo(true);

                map.AddTBStringPK(TeamAttr.No, null, "编号", true, true, 3, 3, 3);
                map.AddTBString(TeamAttr.Name, null, "名称", true, false, 0, 300, 20);
                map.AddDDLEntities(TeamAttr.FK_TeamType, null, "类型", new TeamTypes(), true);

                //   map.AddTBString(TeamAttr.ParentNo, null, "父亲节编号", true, true, 0, 100, 20);
                map.AddTBInt(TeamAttr.Idx, 0, "顺序", true, false);

                map.AddSearchAttr(TeamAttr.FK_TeamType);

                map.AttrsOfOneVSM.Add(new BP.Port.TeamEmps(), new Emps(),
                    TeamEmpAttr.FK_Team, TeamEmpAttr.FK_Emp, EmpAttr.Name, EmpAttr.No, "人员");


                ////节点绑定人员. 使用树杆与叶子的模式绑定.
                //map.AttrsOfOneVSM.AddBranchesAndLeaf(new BP.Port.TeamEmps(), new BP.Port.Emps(),
                //   TeamEmpAttr.FK_Team,
                //   TeamEmpAttr.FK_Emp, "人员(树)", EmpAttr.FK_Dept, EmpAttr.Name, EmpAttr.No, "@WebUser.FK_Dept");

                //map.AttrsOfOneVSM.Add(new TeamEmps(), new Emps(),
                // TeamEmpAttr.FK_Team, TeamEmpAttr.FK_Emp, EmpAttr.Name, EmpAttr.No, "人员(简单)");

                //map.AttrsOfOneVSM.Add(new TeamStations(), new Stations(),
                //    TeamEmpAttr.FK_Team, TeamStationAttr.FK_Station, EmpAttr.Name, EmpAttr.No, "角色(简单)");
                    

                //map.AttrsOfOneVSM.AddTeamListModel(new TeamStations(), new BP.Port.Stations(),
                //  TeamStationAttr.FK_Team,
                //  TeamStationAttr.FK_Station, "角色(平铺)", StationAttr.FK_StationType);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
        protected override bool beforeUpdateInsertAction()
        {
            if (DataType.IsNullOrEmpty(this.Name) == true)
                throw new Exception("请输入名称");

            if (DataType.IsNullOrEmpty(this.FK_TeamType) == true)
                throw new Exception("请选择类型"); //@hongyan.

            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 用户组s
    /// </summary>
    public class Teams : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 用户组s
        /// </summary>
        public Teams()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Team();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Team> ToJavaList()
        {
            return (System.Collections.Generic.IList<Team>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Team> Tolist()
        {
            System.Collections.Generic.List<Team> list = new System.Collections.Generic.List<Team>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Team)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
