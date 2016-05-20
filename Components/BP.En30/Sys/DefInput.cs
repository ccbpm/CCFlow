using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 属性
    /// </summary>
    public class DefInputAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 实体名称
        /// </summary>
        public const string EnName = "EnName";
        /// <summary>
        /// 字段
        /// </summary>
        public const string AttrKey = "AttrKey";
        /// <summary>
        /// 人员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 节点文本
        /// </summary>
        public const string CurValue = "CurValue";
    }
    /// <summary>
    /// 默认值
    /// </summary>
    public class DefInput : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 节点编号
        /// </summary>
        public string EnName
        {
            get
            {
                return this.GetValStringByKey(DefInputAttr.EnName);
            }
            set
            {
                this.SetValByKey(DefInputAttr.EnName, value);
            }
        }
        /// <summary>
        /// 字段
        /// </summary>
        public string AttrKey
        {
            get
            {
                return this.GetValStringByKey(DefInputAttr.AttrKey);
            }
            set
            {
                this.SetValByKey(DefInputAttr.AttrKey, value);
            }
        }
        /// <summary>
        /// 人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(DefInputAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(DefInputAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 节点文本
        /// </summary>
        public string CurValue
        {
            get
            {
                return this.GetValStringByKey(DefInputAttr.CurValue);
            }
            set
            {
                this.SetValByKey(DefInputAttr.CurValue, value);
            }
        }
        #endregion

        #region 构造方法

        /// <summary>
        /// 默认值
        /// </summary>
        public DefInput()
        {
        }
        /// <summary>
        /// map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null) return this._enMap;
                Map map = new Map("Sys_DefInput");
                map.EnType = EnType.Sys;
                map.EnDesc = "默认值";
                map.DepositaryOfEntity = Depositary.Application;
                map.DepositaryOfMap = Depositary.Application;

                map.AddTBStringPK(DefInputAttr.MyPK, null, "MyPK", false, false, 0, 50, 20);
                map.AddTBString(DefInputAttr.EnName, null, "实体名称", false, false, 0, 50, 20);
                map.AddTBString(DefInputAttr.AttrKey, null, "字段", false, false, 0, 50, 20);
                map.AddTBString(DefInputAttr.CurValue, null, "节点文本", false, true, 0, 4000, 10);
                map.AddTBString(DefInputAttr.FK_Emp, null, "人员", false, true, 0, 100, 10);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeInsert()
        {
            this.MyPK = BP.DA.DBAccess.GenerGUID();
            this.FK_Emp = BP.Web.WebUser.No;
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 默认值s
    /// </summary>
    public class DefInputs : EntitiesNoName
    {
        /// <summary>
        /// 默认值
        /// </summary>
        public DefInputs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DefInput();
            }
        }
    }
}
