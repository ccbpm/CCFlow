using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;

namespace BP.WF.Port.Admin2Group
{
    /// <summary>
    /// 组织管理员
    /// </summary>
    public class OAFrmTreeAttr : BP.En.EntityMyPKAttr
    {
        /// <summary>
        /// 关联的二级管理员
        /// </summary>
        public const string RefOrgAdminer = "RefOrgAdminer";
        /// <summary>
        /// 管理员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 组织
        /// </summary>
        public const string OrgNo = "OrgNo";

        public const string FrmTreeNo = "FrmTreeNo";
    }
    /// <summary>
    /// 组织管理员-
    /// </summary>
    public class OAFrmTree : EntityMyPK
    {
        #region 属性
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(OAFrmTreeAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(OAFrmTreeAttr.FK_Emp, value);
            }
        }
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(OAFrmTreeAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(OAFrmTreeAttr.OrgNo, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 组织管理员
        /// </summary>
        public OAFrmTree()
        {
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
                Map map = new Map("Port_OrgAdminerFrmTree", "表单目录权限");
                map.AddMyPK();
                map.AddTBString(OAFrmTreeAttr.OrgNo, null, "组织", true, false, 0, 50, 20);
                map.AddTBString(OAFrmTreeAttr.FK_Emp, null, "管理员", true, false, 0, 50, 20);
                map.AddTBString(OAFrmTreeAttr.RefOrgAdminer, null, "组织管理员", true, false, 0, 50, 20);

                //map.AddDDLEntities(OAFrmTreeAttr.FK_Emp, null, "管理员", new Emps(), false);
                //map.AddDDLEntities(OAFrmTreeAttr.RefOrgAdminer, null, "管理员", new Emps(), false);
                map.AddDDLEntities(OAFrmTreeAttr.FrmTreeNo, null, "表单目录", new BP.Sys.FrmTrees(), false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            string str = this.GetValStringByKey("RefOrgAdminer");

            this.MyPK = this.GetValStringByKey("RefOrgAdminer")  + "_" + this.GetValStringByKey("FrmTreeNo");

            OrgAdminer oa = new OrgAdminer(str);

            this.OrgNo = oa.OrgNo;
            this.FK_Emp = oa.FK_Emp;

            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 组织管理员s
    /// </summary>
    public class OAFrmTrees : EntitiesMM
    {
        #region 构造
        /// <summary>
        /// 组织s
        /// </summary>
        public OAFrmTrees()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new OAFrmTree();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<OAFrmTree> ToJavaList()
        {
            return (System.Collections.Generic.IList<OAFrmTree>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<OAFrmTree> Tolist()
        {
            System.Collections.Generic.List<OAFrmTree> list = new System.Collections.Generic.List<OAFrmTree>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((OAFrmTree)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
