using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using BP.WF.Template;
using BP.Sys;

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
            MapData md =new MapData(frmID);
            return md.ExcelGenerFile(oid);
        }
        /// <summary>
        /// 保存成功
        /// </summary>
        /// <param name="userNo">用户编号</param>
        /// <param name="sid">SID</param>
        /// <param name="frmID">表单ID</param>
        /// <param name="oid">OID</param>
        /// <param name="byt">表单</param>
        /// <returns>保存是否成功</returns>
        [WebMethod]
        public bool SaveExcelFile(string userNo, string sid, string frmID, int oid,string json, byte[] byt)
        {
            return true;
        }
    }
}
