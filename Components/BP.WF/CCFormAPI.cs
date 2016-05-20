using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.WF;
using BP.DA;
using BP.Port;
using BP.Web;
using BP.WF.Template;
using BP.En;
using BP.Sys;

namespace BP.WF
{
    /// <summary>
    /// 表单引擎api
    /// </summary>
    public class CCFormAPI:Dev2Interface
    {
        /// <summary>
        /// 生成报表
        /// </summary>
        /// <param name="templeteFilePath">模版路径</param>
        /// <param name="ds">数据源</param>
        /// <returns>生成单据的路径</returns>
        public static void Frm_GenerBill(string templeteFullFile, string saveToDir, string saveFileName,
            BillFileType fileType, DataSet ds, string fk_mapData)
        {

            MapData md = new MapData(fk_mapData);
            GEEntity entity = md.GenerGEEntityByDataSet(ds);

            BP.Pub.RTFEngine rtf = new BP.Pub.RTFEngine();
            rtf.HisEns.Clear();
            rtf.EnsDataDtls.Clear();

            rtf.HisEns.AddEntity(entity);
            var dtls = entity.Dtls;

            foreach (var item in dtls)
                rtf.EnsDataDtls.Add(item);

            rtf.MakeDoc(templeteFullFile, saveToDir, saveFileName, null, false);
        }
    }
}
