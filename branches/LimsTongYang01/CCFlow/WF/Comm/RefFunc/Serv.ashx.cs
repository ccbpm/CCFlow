using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BP.En;
using BP.DA;

namespace CCFlow.WF.Comm.RefFunc
{
    /// <summary>
    /// Serv 的摘要说明
    /// </summary>
    public class Serv : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string ensName = context.Request.QueryString["EnsName"];
            string PKVal = context.Request.QueryString["PKVal"];
            BP.En.Entities ens = BP.En.ClassFactory.GetEns(ensName);
            BP.En.Entity en =ens.GetNewEntity; 
            if (PKVal != null)
            {
                en.PKVal = PKVal;
                en.RetrieveFromDBSources();
            }
            en = BP.Sys.PubClass.CopyFromRequest(en, context.Request);
            en.Save();

             

            #region 保存 属性 附件
            try
            {
                //AttrFiles fils = en.EnMap.HisAttrFiles;
                //SysFileManagers sfs = new SysFileManagers(en.ToString(), en.PKVal.ToString());
                //foreach (AttrFile fl in fils)
                //{
                //    HtmlInputFile file = (HtmlInputFile)this.UCEn1.FindControl("F" + fl.FileNo);
                //    if (file.Value.Contains(".") == false)
                //        continue;

                //    SysFileManager enFile = sfs.GetEntityByKey(SysFileManagerAttr.AttrFileNo, fl.FileNo) as SysFileManager;
                //    SysFileManager enN = null;
                //    if (enFile == null)
                //    {
                //        enN = this.FileSave(null, file, en);
                //    }
                //    else
                //    {
                //        enFile.Delete();
                //        enN = this.FileSave(null, file, en);
                //    }

                //    enN.AttrFileNo = fl.FileNo;
                //    enN.AttrFileName = fl.FileName;
                //    enN.EnName = en.ToString();
                //    enN.Update();
                //}
            }
            catch 
            {
           //     this.Alert("保存附件出现错误：" + ex.Message);
            }
            #endregion
            
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}