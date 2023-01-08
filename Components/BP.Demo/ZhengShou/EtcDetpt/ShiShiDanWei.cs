using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.ZS
{
    /// <summary>
    /// 实施单位 attr
    /// </summary>
    public class ShiShiDanWeiAttr : EntityNoNameAttr
    {
        public const string Faren = "Faren";
        public const string Tel = "Tel";
    }
    /// <summary>
    ///  实施单位
    /// </summary>
    public class ShiShiDanWei : EntityNoName
    {
        #region 属性.
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenAll();
                return uac;
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 实施单位
        /// </summary>
        public ShiShiDanWei()
        {
        }
        /// <summary>
        /// 实施单位
        /// </summary>
        /// <param name="_No"></param>
        public ShiShiDanWei(string _No) : base(_No) { }
        /// <summary>
        /// 实施单位Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ZS_ShiShiDanWei", "实施单位");
                map.IsAutoGenerNo = true;
                map.CodeStruct = "4";

                map.AddTBStringPK(ShiShiDanWeiAttr.No, null, "编号", true, true, 4, 4, 20);
                map.AddTBString(ShiShiDanWeiAttr.Name, null, "名称", true, false, 0, 50, 900);

              

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 实施单位s
    /// </summary>
    public class ShiShiDanWeis : EntitiesNoName
    {
        /// <summary>
        /// 实施单位s
        /// </summary>
        public ShiShiDanWeis() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ShiShiDanWei();
            }
        }
    }
}
