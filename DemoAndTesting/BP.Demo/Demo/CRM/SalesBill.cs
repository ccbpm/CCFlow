using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.Demo
{
    /// <summary>
    /// 销售帐单 属性
    /// </summary>
    public class SaleBillAttr
    {
        #region 基本属性
        /// <summary>
        /// 工作节点
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 部门
        /// </summary>
        public const string FK_Dept = "FK_Dept";
        /// <summary>
        /// 年度
        /// </summary>
        public const string FK_ND = "FK_ND";
        /// <summary>
        /// 金额
        /// </summary>
        public const string JE = "JE";
        /// <summary>
        /// 年月
        /// </summary>
        public const string FK_NY = "FK_NY";
        /// <summary>
        /// 产品
        /// </summary>
        public const string FK_Product = "FK_Product";
        #endregion
    }
    /// <summary>
    /// 销售帐单
    /// </summary>
    public class SaleBill : EntityOID
    {
        #region 属性
        /// <summary>
        /// 年月
        /// </summary>
        public string FK_NY
        {
            get
            {
                return this.GetValStringByKey(SaleBillAttr.FK_NY);
            }
            set
            {
                this.SetValByKey(SaleBillAttr.FK_NY, value);
            }
        }
        /// <summary>
        /// 产品编号
        /// </summary>
        public string FK_Product
        {
            get
            {
                return this.GetValStringByKey(SaleBillAttr.FK_Product);
            }
            set
            {
                this.SetValByKey(SaleBillAttr.FK_Product, value);
            }
        }
        /// <summary>
        /// 人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(SaleBillAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(SaleBillAttr.FK_Emp, value);
            }
        }
        /// <summary>
        /// 工作单位
        /// </summary>
        public string FK_Dept
        {
            get
            {
                return this.GetValStringByKey(SaleBillAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(SaleBillAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 年度
        /// </summary>
        public string FK_ND
        {
            get
            {
                return this.GetValStringByKey(SaleBillAttr.FK_ND);
            }
            set
            {
                this.SetValByKey(SaleBillAttr.FK_ND, value);
            }
        }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal JE
        {
            get
            {
                return this.GetValDecimalByKey(SaleBillAttr.JE);
            }
            set
            {
                this.SetValByKey(SaleBillAttr.JE, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 销售帐单
        /// </summary>
        public SaleBill()
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
                Map map = new Map("Demo_SaleBill","销售帐单");

                map.AddTBIntPKOID();
                map.AddDDLEntities(SaleBillAttr.FK_Dept, null, "部门", new BP.Port.Depts(), false);
                map.AddDDLEntities(SaleBillAttr.FK_Emp, null, "人员", new BP.Port.Emps(),false);
                
                map.AddDDLEntities(SaleBillAttr.FK_ND, null, "年度", new BP.Pub.NDs(), false);
                map.AddDDLEntities(SaleBillAttr.FK_NY, null, "年月", new BP.Pub.NYs(), false);

                map.AddDDLEntities(SaleBillAttr.FK_Product, null, "产品", new BP.Demo.CRM.Products(), false);
                map.AddTBDecimal(SaleBillAttr.JE, 0, "销售金额", true, false);
                map.AddTBString(BP.Demo.CRM.ProductAttr.Addr, null, "生产地址", true, false, 0, 50, 200);

                //查询条件映射.
                map.AddSearchAttr(SaleBillAttr.FK_Dept);
                map.AddSearchAttr(SaleBillAttr.FK_NY);
                map.AddSearchAttr(SaleBillAttr.FK_Product);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 销售帐单s
    /// </summary>
    public class SaleBills : EntitiesOID
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SaleBill();
            }
        }
        /// <summary>
        /// 销售帐单s
        /// </summary>
        public SaleBills() { }
        #endregion
    }
}
