using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.SDKFlowDemo.SDK.F137
{
    public partial class QingJia :  BP.Web.WebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            
        }

        public void Save()
        {
            

            Hashtable ht = new Hashtable();
            foreach (string str in this.Request.QueryString)
            {
                string val = this.Request.QueryString[Key];
                string mykey = str.Replace("TB_", "");
                mykey = str.Replace("DDL_", "");
                mykey = str.Replace("CB_", "");
                ht.Add(mykey, val);
            }
            BP.WF.Dev2Interface.Node_SaveWork("001", 101, 10001, ht);
        }
    }
}