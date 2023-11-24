using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;

namespace BP.Port
{
    /// <summary>
    /// 用户组人员
    /// </summary>
    public class TeamEmpAttr
    {
        /// <summary>
        /// 操作员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 用户组
        /// </summary>
        public const string FK_Team = "FK_Team";
    }
    /// <summary>
    /// 用户组人员
    /// </summary>
    public class TeamEmp : EntityMM
    {
        #region 属性
        public string EmpNo
        {
            get
            {
                return this.GetValStringByKey(TeamEmpAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(TeamEmpAttr.FK_Emp, value);
            }
        }
        public string FK_Team
        {
            get
            {
                return this.GetValStringByKey(TeamEmpAttr.FK_Team);
            }
            set
            {
                this.SetValByKey(TeamEmpAttr.FK_Team, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 用户组人员
        /// </summary>
        public TeamEmp()
        {
        }
        /// <summary>
        /// 用户组人员
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Port_TeamEmp", "用户组人员");
                map.setEnType(EnType.App);

                map.AddTBStringPK(TeamEmpAttr.FK_Team, null, "用户组", true, false, 0, 50, 20);
                map.AddDDLEntitiesPK(TeamEmpAttr.FK_Emp, null, "人员", new Emps(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 用户组人员s
    /// </summary>
    public class TeamEmps : EntitiesMM
    {
        #region 构造
        /// <summary>
        /// 用户组s
        /// </summary>
        public TeamEmps()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new TeamEmp();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TeamEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<TeamEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TeamEmp> Tolist()
        {
            System.Collections.Generic.List<TeamEmp> list = new System.Collections.Generic.List<TeamEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TeamEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
