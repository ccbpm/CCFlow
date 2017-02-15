using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Data;
using System.Web.Services;
using BP.WF.Template;
using BP.Sys;
using BP.DA;

namespace CCFlow.WF.CCForm
{
    /// <summary>
    /// CCFormAPI 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。
    // [System.Web.Script.Services.ScriptService]
    public class CCFormAPI : System.Web.Services.WebService
    {
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        /// <summary>
        /// 获得Excel文件
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sid">SID</param>
        /// <param name="frmID">表单ID</param>
        /// <param name="oid">表单主键</param>
        /// <returns></returns>
        [WebMethod]
        public byte[] GenerExcelFile(string userNo, string sid, string frmID, int oid)
        {
            return null;
            //MapData md =new MapData(frmID);
            //return md.ExcelGenerFile(oid,null);
        }
        /// <summary>
        /// 生成vsto模式的数据
        /// </summary>
        /// <param name="userNo"></param>
        /// <param name="sid"></param>
        /// <param name="frmID"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        [WebMethod]
        public System.Data.DataSet GenerDBForVSTOExcelFrmModel(string userNo, string sid, string frmID, int oid)
        {
            return BP.WF.CCFormAPI.GenerDBForVSTOExcelFrmModel(frmID, oid);
        }
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sid">SID</param>
        /// <param name="frmID">表单编号</param>
        /// <param name="oid">主键</param>
        /// <param name="mainTableAtParas">主表参数</param>
        /// <param name="dsDtls">从表参数</param>
        /// <param name="byt">文件流</param>
        [WebMethod]
        public void SaveExcelFile(string userNo, string sid, string frmID, int oid, string mainTableAtParas, System.Data.DataSet dsDtls, byte[] byt)
        {
            // 执行保存文件.
            MapData md = new MapData(frmID);
            md.ExcelSaveFile(oid, byt);

            //实体.
            GEEntity wk = new GEEntity(frmID, oid);
            wk.ResetDefaultVal();

            if (mainTableAtParas != null)
            {
                AtPara ap = new AtPara(mainTableAtParas);
                foreach (string str in ap.HisHT.Keys)
                {
                    if (wk.Row.ContainsKey(str))
                        wk.SetValByKey(str, ap.GetValStrByKey(str));
                    else
                        wk.Row.Add(str, ap.GetValStrByKey(str));
                }
            }

            wk.OID = oid;
            wk.Save();

            if (dsDtls == null)
                return;

            #region 保存从表
            //明细集合.
            MapDtls dtls = new MapDtls(frmID);

            //保存从表
            foreach (System.Data.DataTable dt in dsDtls.Tables)
            {
                foreach (MapDtl dtl in dtls)
                {
                    if (dt.TableName != dtl.No)
                        continue;
                    //获取dtls
                    GEDtls daDtls = new GEDtls(dtl.No);
                    daDtls.Delete(GEDtlAttr.RefPK, oid); // 清除现有的数据.

                    // 为从表复制数据.
                    foreach (DataRow dr in dt.Rows)
                    {
                        GEDtl daDtl = daDtls.GetNewEntity as GEDtl;
                        daDtl.RefPK = oid.ToString();
                        //明细列.
                        foreach (DataColumn dc in dt.Columns)
                        {
                            //设置属性.
                            daDtl.SetValByKey(dc.ColumnName, dr[dc.ColumnName]);
                        }

                        daDtl.ResetDefaultVal();

                        daDtl.RefPK = oid.ToString();
                        daDtl.RDT = DataType.CurrentDataTime;

                        //执行保存.
                        if (daDtl.OID > 100)
                            daDtl.Update(); //插入数据.
                        else
                            daDtl.InsertAsOID(DBAccess.GenerOID("Dtl")); //插入数据.
                    }
                }
            }
            #endregion 保存从表结束
        }
    }
}
