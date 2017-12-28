using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.CN
{
    /// <summary>
    /// 省份
    /// </summary>
    public class SFAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 片区
        /// </summary>
        public const string FK_PQ = "FK_PQ";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Names = "Names";
        /// <summary>
        /// 级次
        /// </summary>
        public const string JC = "JC";
    }
    /// <summary>
    /// 省份
    /// </summary>
    public class SF : EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 片区编号
        /// </summary>
        public string FK_PQ
        {
            get
            {
                return this.GetValStrByKey(SFAttr.FK_PQ);
            }
            set
            {
                this.SetValByKey(SFAttr.FK_PQ, value);
            }
        }
        /// <summary>
        /// 片区名称
        /// </summary>
        public string FK_PQT
        {
            get
            {
                return this.GetValRefTextByKey(SFAttr.FK_PQ);
            }
        }
        /// <summary>
        /// 小名称
        /// </summary>
        public string Names
        {
            get
            {
                return this.GetValStrByKey(SFAttr.Names);
            }
            set
            {
                this.SetValByKey(SFAttr.Names, value);
            }
        }
        /// <summary>
        /// 简称
        /// </summary>
        public string JC
        {
            get
            {
                return this.GetValStrByKey(SFAttr.JC);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 访问权限.
        /// </summary>
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
        /// 省份
        /// </summary>		
        public SF() { }
        /// <summary>
        /// 省份
        /// </summary>
        /// <param name="no"></param>
        public SF(string no)
            : base(no)
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
                Map map = new Map( "CN_SF", "省份");

                #region 基本属性
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN);
                map.AdjunctType = AdjunctType.AllType;
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.IsCheckNoLength = false;
                map.Java_SetEnType(EnType.App);
                map.Java_SetCodeStruct("4");
                #endregion

                #region 字段
                map.AddTBStringPK(SFAttr.No, null, "编号", true, false, 2, 2, 2);
                map.AddTBString(SFAttr.Name, null, "名称", true, false, 0, 50, 200);
                map.AddTBString(SFAttr.Names, null, "小名称", true, false, 0, 50, 200);
                map.AddTBString(SFAttr.JC, null, "简称", true, false, 0, 50, 200);
                map.AddDDLEntities(SFAttr.FK_PQ, null, "片区", new PQs(), true);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 省份s
    /// </summary>
    public class SFs : EntitiesNoName
    {
        #region 省份.
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SF();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 省份s
        /// </summary>
        public SFs() { }
        #endregion
    }
}
