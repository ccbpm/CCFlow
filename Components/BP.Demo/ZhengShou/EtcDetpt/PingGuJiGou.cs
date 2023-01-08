using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.ZS
{
    /// <summary>
    /// 评估机构 attr
    /// </summary>
    public class PingGuJiGouAttr : EntityNoNameAttr
    {
        public const string Faren = "Faren";
        public const string Tel = "Tel";
    }
    /// <summary>
    ///  评估机构
    /// </summary>
    public class PingGuJiGou : EntityNoName
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
        /// 评估机构
        /// </summary>
        public PingGuJiGou()
        {
        }
        /// <summary>
        /// 评估机构
        /// </summary>
        /// <param name="_No"></param>
        public PingGuJiGou(string _No) : base(_No) { }
        /// <summary>
        /// 评估机构Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ZS_PingGuJiGou", "评估机构");
                map.IsAutoGenerNo = true;
                map.CodeStruct = "4";

                map.AddTBStringPK(ShiShiDanWeiAttr.No, null, "编号", true, true, 4, 4, 20);
                map.AddTBString(ShiShiDanWeiAttr.Name, null, "名称", true, false, 0, 50, 50);

                map.AddTBString(PingGuJiGouAttr.Faren, null, "法人", false, true, 0, 50, 50);
                map.AddTBString(PingGuJiGouAttr.Tel, null, "管理人员", false, true, 0, 50, 50);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 评估机构s
    /// </summary>
    public class PingGuJiGous : EntitiesNoName
    {
        /// <summary>
        /// 评估机构s
        /// </summary>
        public PingGuJiGous() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new PingGuJiGou();
            }
        }
    }
}
