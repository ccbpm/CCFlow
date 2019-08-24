using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Concurrent;
using System.IO;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF;
using BP.Web;
using BP.Port;
using BP.Demo.BPFramework;
using ThoughtWorks.QRCode.Codec;
  

namespace CCFlow
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //BP.WF.Rpt.RptDfine.
            return;
            BP.WF.Template.NodeExt ext = new BP.WF.Template.NodeExt();
            ext.NodeID = 101;
            ext.RetrieveFromDBSources();
            return;

            BP.WF.Dev2Interface.Port_Login("admin");
            for (int i = 0; i < 10; i++)
            {

            }
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork("057", WebUser.No);
            string msg = BP.WF.Dev2Interface.Node_SendWork("057", workid, 0, "admin").ToMsgOfHtml();
            this.Response.Write(msg);


            //BP.WF.Node nd = new Node(12801);
            // nd.FormType
            // nd.FormType = NodeFormType.SDKForm;
            // nd.DirectUpdate();
            return; 

            BP.WF.Flow fe2 = new Flow();
            fe2.No = "128";
            fe2.Retrieve();
            fe2.DoCheck();
            return;


            BP.WF.Template.FlowExt fe = new BP.WF.Template.FlowExt();
            fe.CheckPhysicsTable();
            fe.No = "128";
            fe.Retrieve();


            BP.DA.Log.DefaultLogWriteLine(LogType.Info, "----------------------------------  start ------------------ ");
            BP.DA.Log.DefaultLogWriteLine(LogType.Info, "----------------------------------  end ------------------ ");

            if (DBAccess.IsExitsObject("WF_Flow") == false)
                this.Response.Redirect("./WF/Admin/DBInstall.htm", true);
            else
                this.Response.Redirect("./WF/Admin/CCBPMDesigner/Login.htm", true);
            return;
        }

        public void TestCash()
        {
            BP.WF.Node nd = new Node(101);
            string html = "";
            foreach (Attr item in nd.EnMap.Attrs)
            {
                html += "@" + item.Key + " - " + nd.GetValStrByKey(item.Key);
            }
            this.Response.Write(html);
            nd.Name = "abc123";
            nd.Update();

            html = "<hr>";
            foreach (Attr item in nd.EnMap.Attrs)
            {
                html += "@" + item.Key + " - " + nd.GetValStrByKey(item.Key);
            }
            this.Response.Write(html);
        }
    }
}