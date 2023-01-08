using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.ZS
{
    /// <summary>
    /// 项目类型 attr
    /// </summary>
    public class PrjTypeAttr : EntityNoNameAttr
    {
         
    }
    /// <summary>
    ///  项目类型
    /// </summary>
    public class PrjType : EntityNoName
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
        /// 项目类型
        /// </summary>
        public PrjType()
        {
        }
        /// <summary>
        /// 项目类型
        /// </summary>
        /// <param name="_No"></param>
        public PrjType(string _No) : base(_No) { }
        /// <summary>
        /// 项目类型Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ZS_PrjType", "项目类型");
                map.IsAutoGenerNo = true;
                map.CodeStruct = "2";

                map.AddTBStringPK(PrjTypeAttr.No, null, "编号", true, true, 2, 2, 12);
                map.AddTBString(PrjTypeAttr.Name, null, "名称", true, false, 0, 50, 50);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 项目类型s
    /// </summary>
    public class PrjTypes : EntitiesNoName
    {
        /// <summary>
        /// 项目类型s
        /// </summary>
        public PrjTypes() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new PrjType();
            }
        }
    }
}
