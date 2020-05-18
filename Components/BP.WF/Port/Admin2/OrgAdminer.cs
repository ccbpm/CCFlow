using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;

namespace BP.WF.Port.Admin2
{
    /// <summary>
    /// 组织管理员
    /// </summary>
    public class OrgAdminerAttr
    {
        /// <summary>
        /// 管理员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 组织
        /// </summary>
        public const string OrgNo = "OrgNo";
    }
    /// <summary>
    /// 组织管理员
    /// </summary>
    public class OrgAdminer : EntityMM
    {
        #region 属性
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(OrgAdminerAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(OrgAdminerAttr.FK_Emp, value);
            }
        }
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(OrgAdminerAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(OrgAdminerAttr.OrgNo, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 组织管理员
        /// </summary>
        public OrgAdminer()
        {
        }
        /// <summary>
        /// 组织管理员
        /// </summary>
        /// <param name="mypk"></param>
        public OrgAdminer(string no)
        {
            this.Retrieve();
        }
        /// <summary>
        /// 组织管理员
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Port_OrgAdminer");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "组织管理员";
                map.EnType = EnType.App;

                map.AddTBStringPK(OrgAdminerAttr.OrgNo, null, "组织", true, false, 0, 50, 20);
                map.AddDDLEntitiesPK(OrgAdminerAttr.FK_Emp, null, "管理员", new Emps(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 组织管理员s
    /// </summary>
    public class OrgAdminers : EntitiesMM
    {
        #region 构造
        /// <summary>
        /// 组织s
        /// </summary>
        public OrgAdminers()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new OrgAdminer();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<OrgAdminer> ToJavaList()
        {
            return (System.Collections.Generic.IList<OrgAdminer>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<OrgAdminer> Tolist()
        {
            System.Collections.Generic.List<OrgAdminer> list = new System.Collections.Generic.List<OrgAdminer>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((OrgAdminer)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
