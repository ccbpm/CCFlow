using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo
{
    /// <summary>
    /// 供应商
    /// </summary>
    public class SupplierAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 密码
        /// </summary>
        public const string PWD = "PWD";
        /// <summary>
        /// 地址
        /// </summary>
        public const string Addr = "Addr";
        /// <summary>
        /// 电话
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 邮件
        /// </summary>
        public const string Email = "Email";
        #endregion
    }
    /// <summary>
    /// 供应商
    /// </summary>
    public class Supplier : EntityNoName
    {
        #region 基本属性
        /// <summary>
        /// 密码
        /// </summary>
        public string PWD
        {
            get
            {
                return this.GetValStringByKey(SupplierAttr.PWD);
            }
            set
            {
                this.SetValByKey(SupplierAttr.PWD, value);
            }
        }
        /// <summary>
        /// 地址
        /// </summary>
        public string Addr
        {
            get
            {
                return this.GetValStringByKey(SupplierAttr.Addr);
            }
            set
            {
                this.SetValByKey(SupplierAttr.Addr, value);
            }
        }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel
        {
            get
            {
                return this.GetValStringByKey(SupplierAttr.Tel);
            }
            set
            {
                this.SetValByKey(SupplierAttr.Tel, value);
            }
        }
        /// <summary>
        /// 邮件
        /// </summary>
        public string Email
        {
            get
            {
                return this.GetValStringByKey(SupplierAttr.Email);
            }
            set
            {
                this.SetValByKey(SupplierAttr.Email, value);
            }
        }
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
        /// 供应商
        /// </summary>		
        public Supplier() 
        {
        }
        /// <summary>
        /// 供应商
        /// </summary>
        /// <param name="no"></param>
        public Supplier(string no)
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
                Map map = new Map("Demo_Supplier", "供应商");

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
                map.AddTBStringPK(SupplierAttr.No, null, "编号", true, false, 0, 50, 50);
                map.AddTBString(SupplierAttr.Name, null, "名称", true, false, 0, 50, 200);
                map.AddTBString(SupplierAttr.PWD, null, "密码", true, false, 0, 50, 200);

                map.AddTBString(SupplierAttr.Addr, null, "地址", true, false, 0, 50, 200);
                map.AddTBString(SupplierAttr.Tel, null, "电话", true, false, 0, 50, 200);
                map.AddTBString(SupplierAttr.Email, null, "邮件", true, false, 0, 50, 200);

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 获得他的实体集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new Suppliers(); }
        }
        #endregion

        /// <summary>
        /// 校验密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public bool CheckPass(string pwd)
        {
            return this.PWD.Equals(pwd);
        }
    }
    /// <summary>
    /// 供应商
    /// </summary>
    public class Suppliers : EntitiesNoName
    {
        #region 
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Supplier();
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 供应商s
        /// </summary>
        public Suppliers() { }
        #endregion
    }

}
