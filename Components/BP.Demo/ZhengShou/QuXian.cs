using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.ZS
{
    /// <summary>
    /// 区县 attr
    /// </summary>
    public class QuXianAttr : EntityNoNameAttr
    {
    }
    /// <summary>
    ///  区县
    /// </summary>
    public class QuXian : EntityNoName
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
        /// 区县
        /// </summary>
        public QuXian()
        {
        }
        /// <summary>
        /// 区县
        /// </summary>
        /// <param name="_No"></param>
        public QuXian(string _No) : base(_No) { }
        /// <summary>
        /// 区县Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("ZS_QuXian", "区县");
                map.IsAutoGenerNo = true;
                map.CodeStruct = "4";

                map.AddTBStringPK(QuXianAttr.No, null, "编号", true, true, 0, 50, 50);
                map.AddTBString(QuXianAttr.Name, null, "名称", true, false, 0, 50, 50);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 区县s
    /// </summary>
    public class QuXians : EntitiesNoName
    {
        /// <summary>
        /// 区县s
        /// </summary>
        public QuXians() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new QuXian();
            }
        }
    }
}
