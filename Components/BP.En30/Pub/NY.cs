using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Pub
{
    /// <summary>
    /// 年月
    /// </summary>
    public class NYAttr : EntityNoNameAttr
    {
        #region 基本属性
        public const string FK_SF = "FK_SF";
        #endregion
    }
    /// <summary>
    /// 年月
    /// </summary>
    public class NY : EntityNoName
    {
        #region 基本属性
        #endregion

        #region 构造函数
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// 年月
        /// </summary>		
        public NY() { }
        public NY(string no) : base(no)
        {
        }


        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Pub_NY", "年月");

                #region 基本属性 
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN);
                map.AdjunctType = AdjunctType.AllType;
                map.DepositaryOfMap = Depositary.Application;
                map.DepositaryOfEntity = Depositary.None;
                map.IsCheckNoLength = false;
                map.setEnType(EnType.App);
                map.CodeStruct = "4";
                #endregion

                #region 字段 
                map.AddTBStringPK(NYAttr.No, null, "编号", true, false, 0, 50, 50);
                map.AddTBString(NYAttr.Name, null, "名称", true, false, 0, 50, 200);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        public override Entities GetNewEntities
        {
            get { return new NYs(); }
        }
        #endregion
    }
    /// <summary>
    /// 年月
    /// </summary>
    public class NYs : EntitiesNoName
    {
        #region 
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new NY();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 年月s
        /// </summary>
        public NYs() { }
        #endregion
    }

}
