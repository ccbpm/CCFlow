using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace BP.DA
{
/// <summary>
/// NextPrivate 的摘要描述
/// </summary>
    public class NextPreviouRec
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="pk"></param>
        /// <param name="currentPKVal"></param>
        /// <param name="wherConn"></param>
        public NextPreviouRec(string table, string pk, object currentPKVal, string whereConn)
        {
            this.PreviouID = BP.DA.DBAccess.RunSQLReturnStringIsNull("SELECT TOP 1 " + pk + " FROM " + table + " WHERE  " + whereConn + " AND " + pk + " < '" + currentPKVal + "' ORDER BY " + pk, null);
            this.NextID = BP.DA.DBAccess.RunSQLReturnStringIsNull("SELECT TOP 1 " + pk + " FROM " + table + " WHERE  " + whereConn + " AND " + pk + " > '" + currentPKVal + "' ORDER BY " + pk, null);
        }
        public string NextID = null;
        public string PreviouID = null;
    }
}