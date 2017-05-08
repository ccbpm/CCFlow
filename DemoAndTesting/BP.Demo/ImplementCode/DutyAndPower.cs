using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BP.DA;
using BP.En;

namespace BP.WF.MS
{
    /// <summary>
    /// 职责与权限属性
    /// </summary>
    public class DutyAndPowerAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 制度编号
        /// </summary>
        public const string FK_Main = "FK_Main";
        /// <summary>
        /// 岗位
        /// </summary>
        public const string StationName = "StationName";
        /// <summary>
        /// 职责
        /// </summary>
        public const string Duty = "Duty";
        /// <summary>
        /// 权限
        /// </summary>
        public const string PowerOfRight = "PowerOfRight";
        #endregion
    }
    /// <summary>
    /// 职责与权限
    /// </summary>
    public class DutyAndPower : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 制度编号
        /// </summary>
        public string FK_Main
        {
            get
            {
                return this.GetValStringByKey(DutyAndPowerAttr.FK_Main);
            }
            set
            {
                this.SetValByKey(DutyAndPowerAttr.FK_Main, value);
            }
        }
        /// <summary>
        /// 岗位
        /// </summary>
        public string StationName
        {
            get
            {
                return this.GetValStringByKey(DutyAndPowerAttr.StationName);
            }
            set
            {
                this.SetValByKey(DutyAndPowerAttr.StationName, value);
            }
        }
        /// <summary>
        /// 职责
        /// </summary>
        public string Duty
        {
            get
            {
                return this.GetValStringByKey(DutyAndPowerAttr.Duty);
            }
            set
            {
                this.SetValByKey(DutyAndPowerAttr.Duty, value);
            }
        }
        /// <summary>
        /// 权限
        /// </summary>
        public string PowerOfRight
        {
            get
            {
                return this.GetValStringByKey(DutyAndPowerAttr.PowerOfRight);
            }
            set
            {
                this.SetValByKey(DutyAndPowerAttr.PowerOfRight, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// Main
        /// </summary>
        public DutyAndPower()
        {
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("MS_DutyAndPower", "职责与权限");
                map.Java_SetEnType(EnType.Admin);

                map.AddTBStringPK(DutyAndPowerAttr.No, null, "编号", true, true, 10, 10, 10);
                map.AddTBString(DutyAndPowerAttr.Name, null, "名称", true, true, 0, 500, 5);
                map.AddTBString(DutyAndPowerAttr.FK_Main, null, "制度编号", true, true, 0, 200, 4);
                map.AddTBString(DutyAndPowerAttr.StationName, null, "岗位", true, true, 0, 500, 4);
                map.AddTBString(DutyAndPowerAttr.Duty, null, "职责", true, true, 0, 1000, 4);
                map.AddTBString(DutyAndPowerAttr.PowerOfRight, null, "权限", true, true, 0, 1000, 4);
                
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 职责与权限s
    /// </summary>
    public class DutyAndPowers : EntitiesNoName
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DutyAndPower();
            }
        }
        /// <summary>
        /// 职责与权限
        /// </summary>
        public DutyAndPowers() { }
        #endregion
    }
}
