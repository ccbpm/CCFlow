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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Diagnostics.Eventing;
using System.Diagnostics.PerformanceData;
using System.Diagnostics.SymbolStore;

using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using BP;
using BP.Port;
using BP.DA;
using BP.En;
using BP.Sys;


namespace CCFlow
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            return;
            DirectoryEntry de = BP.GPM.AD.Glo.RootDirectoryEntry;
            foreach (DirectoryEntry item in de.Children)
            {
                this.Response.Write(item.Guid + " - " + item.Name + "<br>");
                if (item.Name.Contains("OU="))
                {
                    foreach (DirectoryEntry item1 in item.Children)
                    {
                        this.Response.Write(item1.Guid+" - "+item1.Name + "<br>");
                    }
                }
            }
            return;

            for (int i = 0; i < de.Properties.Count; i++)
            {
               // var id = de.Properties.Values[i];
               // this.Response.Write(i);

                //System.DirectoryServices.
            }

           
            foreach (DirectoryEntry item in de.Properties)
            {
                this.Response.Write(item.Name);
            }
            return;
          

            BP.GPM.AD.Sync dts = new BP.GPM.AD.Sync();
            dts.Do();
            return; 


            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            //  string json = BP.WF.AppClass.SDK_Page_Init(216);
            string json = BP.WF.AppClass.SDK_Page_Init(219);
            this.Response.Write(json);

            //BP.WF.Template.NodeSimple
            //BP.WF.Template.SubFlowHand
            //BP.WF.Template.SubFlowYanXu
            //BP.WF.Template.SubFlowYanXu
            //BP.WF.Template.SubFlowYanXu
            //BP.WF.Template.SubFlowYanXu
            return;
            BP.WF.Glo.UpdataCCFlowVer();
            // BP.Frm.FrmDict fd = new BP.Frm.FrmDict();
            // fd.CheckPhysicsTable();
            //BP.Sys.MapData md = new MapData();
            //md.No = "ND122201";
            //md.Retrieve();
            return;

            BP.Frm.MethodFunc func = new BP.Frm.MethodFunc("Dict_XueSheng_ZhuXiaoXueJi");
            func.CheckPhysicsTable();
            func.MethodDoc_JavaScript = "update  Dict_XueSheng set Tel='sss' where oid=1";
            string strs = func.MethodDoc_JavaScript;
            //  strs = strs.Replace("update", "UPDATE");
            DBAccess.RunSQL(strs);
            return;

            string docs = "";
            docs += "/* s 说明21  */";
            docs += "/* s 说明2  */";
            docs += "/* s 说明2  */";
            docs += " -- 这是注释. ";
            docs += "@UPDATE Port_Emp set Name=Name where 1=2";
            docs += "@delete Port_Emp  where 1=2 ";
            BP.DA.DBAccess.RunSQLs(docs);

            //  BP.Frm.FrmStationCreates
            // BP.Frm.FrmTemplate
            return;
            BP.WF.Data.GERpt rpt = new BP.WF.Data.GERpt("ND1Rpt", 105);
            //rpt.WFState = WFState.Complete;
            rpt.Update("WFState", 3);
            return;
            BP.Port.Emps emps = new Emps();
            emps.RetrieveAll();

            foreach (BP.Port.Emp item in emps)
            {
                string py = BP.GPM.Emp.GenerPinYin(item.No, item.Name);
                string sql = "update basic_user set pinYin='" + py + "' where PERSON_ID='" + item.No + "'";
                DBAccess.RunSQL(sql);
            }
            return;
            this.TestCash();
            return;

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