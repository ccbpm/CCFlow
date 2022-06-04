using BP.Sys;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCForm_JS : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCForm_JS()
        {
        }

        /// <summary>
        /// 批注
        /// </summary>
        /// <returns></returns>
        public string FrmDBRemark_GenerReamrkFields()
        {
            FrmDBRemarks ens = new FrmDBRemarks();
            ens.Retrieve(FrmDBRemarkAttr.FrmID, this.FrmID, FrmDBRemarkAttr.RefPKVal, this.PKVal,"RDT");
            return ens.ToJson();
        }
        /// <summary>
        /// 获得版本管理中-变化的字段
        /// </summary>
        /// <returns></returns>
        public string FrmDBVer_GenerChangeFields()
        {
            //FrmDBVers ens = new FrmDBVers();
            //ens.Retrieve();

            FrmDBRemarks ens = new FrmDBRemarks();
            ens.Retrieve(FrmDBRemarkAttr.FrmID, this.FrmID, FrmDBRemarkAttr.RefPKVal, this.RefPKVal);
            return ens.ToJson();
        }

    }
}
