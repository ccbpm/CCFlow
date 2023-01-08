using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.ZS
{
    /// <summary>
    /// 乡镇街道 attr
    /// </summary>
    public class XiangZhenJieDaoAttr : EntityNoNameAttr
    {

    }
    /// <summary>
    ///  乡镇街道
    /// </summary>
    public class XiangZhenJieDao : EntityNoName
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
        /// 乡镇街道
        /// </summary>
        public XiangZhenJieDao()
        {
        }
        /// <summary>
        /// 乡镇街道
        /// </summary>
        /// <param name="_No"></param>
        public XiangZhenJieDao(string _No) : base(_No) { }
        /// <summary>
        /// 乡镇街道Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                #region 流程的基本字段

                Map map = new Map("ZS_XiangZhenJieDao", "乡镇街道");
                //  map.IsView = true;

                map.AddTBStringPK(XiangZhenJieDaoAttr.No, null, "编号", true, true, 0, 50, 50);
                map.AddTBString(XiangZhenJieDaoAttr.Name, null, "名称", false, true, 0, 50, 50);

                this._enMap = map;
                return this._enMap;
                #endregion 流程的基本字段

            }
        }
        #endregion
    }
    /// <summary>
    /// 乡镇街道s
    /// </summary>
    public class XiangZhenJieDaos : EntitiesNoName
    {
        /// <summary>
        /// 乡镇街道s
        /// </summary>
        public XiangZhenJieDaos() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new XiangZhenJieDao();
            }
        }
    }
}
