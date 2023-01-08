using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.ZS
{
    /// <summary>
    /// 上岗人员 attr
    /// </summary>
    public class ShangGangRenYuanAttr : EntityNoNameAttr
    {
        public const string Faren = "Faren";
        /// <summary>
        /// 电话
        /// </summary>
        public const string Tel = "Tel";
        /// <summary>
        /// 身份证
        /// </summary>
        public const string SFZ = "SFZ";
        /// <summary>
        /// 性别
        /// </summary>
        public const string XB = "XB";

        /// <summary>
        /// 状态
        /// </summary>
        public const string SGSta = "SGSta";


    }
    /// <summary>
    ///  上岗人员
    /// </summary>
    public class ShangGangRenYuan : EntityNoName
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
        /// 上岗人员
        /// </summary>
        public ShangGangRenYuan()
        {
        }
        /// <summary>
        /// 上岗人员
        /// </summary>
        /// <param name="_No"></param>
        public ShangGangRenYuan(string _No) : base(_No) { }
        /// <summary>
        /// 上岗人员Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ZS_ShangGangRenYuan", "上岗人员");
                map.IsAutoGenerNo = true;
                map.CodeStruct = "5";

                map.AddTBStringPK(ShiShiDanWeiAttr.No, null, "编号", true, true, 5, 5, 20);
                map.AddTBString(ShiShiDanWeiAttr.Name, null, "名称", true, false, 0, 50, 50);

                map.AddDDLSysEnum(ShangGangRenYuanAttr.SGSta, 0, "状态", true, false, ShangGangRenYuanAttr.SGSta, "@0=未培训@1=可用@2=过期");
                map.AddTBString(ShangGangRenYuanAttr.SFZ, null, "身份证", true, false, 0, 50, 50);
                map.AddDDLSysEnum(ShangGangRenYuanAttr.XB, 0, "性别", true, true, ShangGangRenYuanAttr.XB, "@0=女@1=男@2=其他");


                map.AddTBString(ShangGangRenYuanAttr.Faren, null, "法人", false, true, 0, 50, 50);
                map.AddTBString(ShangGangRenYuanAttr.Tel, null, "管理人员", false, true, 0, 50, 50);


                map.AddSearchAttr(ShangGangRenYuanAttr.SGSta);
                map.AddSearchAttr(ShangGangRenYuanAttr.XB);

                map.AddDtl(new ShangGangRenYuanDtls(), ShangGangRenYuanDtlAttr.RefPK);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 上岗人员s
    /// </summary>
    public class ShangGangRenYuans : EntitiesNoName
    {
        /// <summary>
        /// 上岗人员s
        /// </summary>
        public ShangGangRenYuans() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ShangGangRenYuan();
            }
        }
    }
}
