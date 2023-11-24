using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo
{
    /// <summary>
    /// 产品 属性
    /// </summary>
    public class ProductAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 性别
        /// </summary>
        public const string GuiGe = "GuiGe";
        public const string BeiZhu = "BeiZhu";
        #endregion
    }
    /// <summary>
    /// 产品
    /// </summary>
    public class Product : BP.En.EntityNoName
    {

        #region 构造函数
        /// <summary>
        /// 实体的权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {

                UAC uac = new UAC();
                //  uac.LoadRightFromCCGPM(this); //从GPM里面装载.
                // return uac;
                if (BP.Web.WebUser.No.Equals("admin")==true)
                {
                    uac.IsDelete = true;
                    uac.IsUpdate = true;
                    uac.IsInsert = true;
                    uac.IsView = true;
                }
                else
                {
                    uac.IsView = true;
                }
                uac.IsImp = true;
                return uac;
            }
        }
        /// <summary>
        /// 产品
        /// </summary>
        public Product()
        {
        }
        /// <summary>
        /// 产品
        /// </summary>
        /// <param name="no"></param>
        public Product(string no)
            : base(no)
        {
        }
        #endregion

        #region 重写基类方法
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Demo_Product", "产品");

                //基础信息.
                map.ItIsAllowRepeatName = true; //是否允许名称重复.
                map.ItIsAutoGenerNo = true; //是否自动生成编号.
                map.CodeStruct = "4"; // 4位数的编号，从 0001 开始，到 9999.

                #region 字段映射 - 普通字段.
                map.AddTBStringPK(ProductAttr.No, null, "产品编号", true, true, 4, 4, 90); // 如果设置自动编号字段必须是只读的.
                map.AddTBString(ProductAttr.Name, null, "名称", true, false, 0, 200, 70);
                map.AddTBString(ProductAttr.GuiGe, null, "规格", true, false, 0, 200, 100, true);
                map.AddTBStringDoc(ResumeAttr.BeiZhu, null, "备注", true, false);
                map.AddTBAtParas(2000);
                #endregion 字段映射 - 普通字段.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion 重写基类方法
        protected override bool beforeDelete()
        {
            return base.beforeDelete();
        }

    }
    /// <summary>
    /// 产品s
    /// </summary>
    public class Products : BP.En.EntitiesNoName
    {
        #region 方法
        /// <summary>
        /// 产品s
        /// </summary>
        public Products() { }
        #endregion

        #region 重写基类方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Product();
            }
        }
        #endregion 重写基类方法

        #region 测试方法.
        public string EnsMothed()
        {
            return "EnsMothed@执行成功.";
        }
        public string EnsMothedParas(string para1, string para2)
        {
            return "EnsMothedParas@执行成功." + para1 + " - " + para2;
        }
        #endregion

    }
}
