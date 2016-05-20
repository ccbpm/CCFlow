using System;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.EventEntity
{
    public partial class NodeEventEntity : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ArrayList al = BP.En.ClassFactory.GetObjects("BP.WF.NodeEventBase");
            this.Pub1.Add("<ul>");
            #region  bind it .
            //int i = 1;
            //foreach (BP.WF.NodeEventBase en in al)
            //{
            //   // this.Pub1.AddLi("<a href=\"javascript:ShowIt('" + en.ToString() + "');\"  ></a><br><font size=2 color=Green> </font><br><br>");
            //    this.Pub1.AddLi("节点编号:"+en.NodeMarks+"<br><font size=2 color=Green> </font><br><br>");
            //}
            //this.Pub1.Add("</ul>");
            #endregion
        }
    }
}