using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 控件类型
    /// </summary>
    public class GroupCtrlType
    {
        public const string Frame = "Frame";
        public const string Dtl = "Dtl";
        public const string Ath = "Ath";
        public const string FWC = "FWC";
        public const string SubFlow = "SubFlow";
        public const string Track = "Track";
        public const string Thread = "Thread";
        /// <summary>
        /// 流转自定义组件
        /// </summary>
        public const string FTC = "FTC";
    }
    /// <summary>
    /// GroupField
    /// </summary>
    public class GroupFieldAttr : EntityOIDAttr
    {
        /// <summary>
        /// 主表
        /// </summary>
        public const string EnName = "EnName";
        /// <summary>
        /// Lab
        /// </summary>
        public const string Lab = "Lab";
        /// <summary>
        /// Idx
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 控件类型
        /// </summary>
        public const string CtrlType = "CtrlType";
        /// <summary>
        /// 控件ID
        /// </summary>
        public const string CtrlID = "CtrlID";
    }
    /// <summary>
    /// GroupField
    /// </summary>
    public class GroupField : EntityOID
    {
        #region 属性
        public bool IsUse = false;
        public string EnName
        {
            get
            {
                return this.GetValStrByKey(GroupFieldAttr.EnName);
            }
            set
            {
                this.SetValByKey(GroupFieldAttr.EnName, value);
            }
        }
        /// <summary>
        /// 标签
        /// </summary>
        public string Lab
        {
            get
            {
                return this.GetValStrByKey(GroupFieldAttr.Lab);
            }
            set
            {
                this.SetValByKey(GroupFieldAttr.Lab, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(GroupFieldAttr.Idx);
            }
            set
            {
                this.SetValByKey(GroupFieldAttr.Idx, value);
            }
        }
        /// <summary>
        /// 控件类型
        /// </summary>
        public string CtrlType
        {
            get
            {
                return this.GetValStrByKey(GroupFieldAttr.CtrlType);
            }
            set
            {
                this.SetValByKey(GroupFieldAttr.CtrlType, value);
            }
        }
        /// <summary>
        /// 控件ID
        /// </summary>
        public string CtrlID
        {
            get
            {
                return this.GetValStrByKey(GroupFieldAttr.CtrlID);
            }
            set
            {
                this.SetValByKey(GroupFieldAttr.CtrlID, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// GroupField
        /// </summary>
        public GroupField()
        {
        }
        public GroupField(int oid)
            : base(oid)
        {
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
                Map map = new Map("Sys_GroupField");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "傻瓜表单分组";
                map.EnType = EnType.Sys;

                map.AddTBIntPKOID();
                map.AddTBString(GroupFieldAttr.Lab, null, "标签", true, false, 0, 500, 20);
                map.AddTBString(GroupFieldAttr.EnName, null, "类", true, false, 0, 200, 20);
                map.AddTBString(FrmBtnAttr.GUID, null, "GUID", true, false, 0, 128, 20);
                map.AddTBInt(GroupFieldAttr.Idx, 99, "顺序号", true, false);
                map.AddTBString(GroupFieldAttr.CtrlType, null, "控件类型", true, false, 0, 50, 20);
                map.AddTBString(GroupFieldAttr.CtrlID, null, "控件ID", true, false, 0, 500, 20);
                map.AddTBAtParas(3000);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public void DoDown()
        {
            this.DoOrderDown(GroupFieldAttr.EnName, this.EnName, GroupFieldAttr.Idx);
            return;
        }
        public void DoUp()
        {
            this.DoOrderUp(GroupFieldAttr.EnName, this.EnName, GroupFieldAttr.Idx);
            return;
        }
        protected override bool beforeInsert()
        {
            //if (this.IsExit(GroupFieldAttr.EnName, this.EnName, GroupFieldAttr.Lab, this.Lab) == true)
            //    throw new Exception("@已经在("+this.EnName+")里存在("+this.Lab+")的分组了。");
            try
            {
                string sql = "SELECT MAX(IDX) FROM " + this.EnMap.PhysicsTable + " WHERE EnName='" + this.EnName + "'";
                this.Idx = DBAccess.RunSQLReturnValInt(sql, 0) + 1;
            }
            catch
            {
                this.Idx = 1;
            }
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// GroupFields
    /// </summary>
    public class GroupFields : EntitiesOID
    {
        #region 构造
        /// <summary>
        /// GroupFields
        /// </summary>
        public GroupFields()
        {
        }
        /// <summary>
        /// GroupFields
        /// </summary>
        /// <param name="enName">名称</param>
        public GroupFields(string enName)
        {
            int i = this.Retrieve(GroupFieldAttr.EnName, enName, GroupFieldAttr.Idx);
            if (i == 0)
            {
                GroupField gf = new GroupField();
                gf.EnName = enName;
                MapData md = new MapData();
                md.No = enName;
                if (md.RetrieveFromDBSources() == 0)
                    gf.Lab = "基础信息";
                else
                    gf.Lab = md.Name;
                gf.Idx = 0;
                gf.Insert();
                this.AddEntity(gf);
            }
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GroupField();
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="enName"></param>
        /// <returns></returns>
        public int RetrieveFieldGroup(string enName)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(GroupFieldAttr.EnName, enName);
            qo.addAnd();
            qo.AddWhereIsNull(GroupFieldAttr.CtrlID);
            //qo.AddWhereLen(GroupFieldAttr.CtrlID, " = ", 0, SystemConfig.AppCenterDBType);
            int num=qo.DoQuery();

            if (num==0)
            {
                GroupField gf = new GroupField();
                gf.EnName = enName;
                MapData md = new MapData();
                md.No = enName;
                if (md.RetrieveFromDBSources() == 0)
                    gf.Lab = "基础信息";
                else
                    gf.Lab = md.Name;
                gf.Idx = 0;
                gf.Insert();
                this.AddEntity(gf);
                return 1;
            }
            return num;
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GroupField> ToJavaList()
        {
            return (System.Collections.Generic.IList<GroupField>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GroupField> Tolist()
        {
            System.Collections.Generic.List<GroupField> list = new System.Collections.Generic.List<GroupField>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GroupField)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
