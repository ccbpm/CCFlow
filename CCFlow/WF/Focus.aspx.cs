using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF
{
    public partial class Focus : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

       // 通过key获取value
        public string GetKeyValue(string key)
        {
            string sqlkey = "select lab from Sys_Enum  where EnumKey='WFSta' and IntKey=" + key + "";
            return BP.DA.DBAccess.RunSQLReturnString(sqlkey);
        }
    }
}