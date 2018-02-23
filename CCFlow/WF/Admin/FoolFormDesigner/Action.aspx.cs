using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.XML;
using BP.En;
using BP.Port;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;
using BP;

namespace CCFlow.WF.MapDef
{
    public partial class WF_Admin_MapDef_Action : BP.Web.WebPage
    {
        #region 属性.
        public string Event
        {
            get
            {
                return this.Request.QueryString["Event"];
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.DoType == "Del")
            {
                FrmEvent delFE = new FrmEvent();
                delFE.MyPK = this.FK_MapData + "_" + this.Request.QueryString["RefXml"];
                delFE.Delete();
            }

            MapDtl dtl = new MapDtl(this.FK_MapData);
            this.Pub3.AddCaptionLeft("从表:" + dtl.Name);

            this.Title = "设置:从表事件";
            FrmEvents ndevs = new FrmEvents();
            ndevs.Retrieve(FrmEventAttr.FK_MapData, this.FK_MapData);
            EventListDtls xmls = new EventListDtls();
            xmls.RetrieveAll();

            string myEvent = this.Event;
            BP.WF.XML.EventListDtl myEnentXml = null;

            this.Pub1.Add("<a href='http://ccflow.org' target=_blank ><img src='../DataUser/ICON/" + SystemConfig.CustomerNo + "/LogBiger.png' /></a>");
            this.Pub1.AddUL();
            foreach (BP.WF.XML.EventListDtl xml in xmls)
            {

                FrmEvent nde = ndevs.GetEntityByKey(FrmEventAttr.FK_Event, xml.No) as FrmEvent;
                if (nde == null)
                {
                    if (myEvent == xml.No)
                    {
                        myEnentXml = xml;
                        this.Pub1.AddLi("<font color=green><b>" + xml.Name + "</b></font>");
                    }
                    else
                        this.Pub1.AddLi("Action.aspx?FK_MapData=" + this.FK_MapData + "&Event=" + xml.No, xml.Name);
                }
                else
                {
                    if (myEvent == xml.No)
                    {
                        myEnentXml = xml;
                        this.Pub1.AddLi("<font color=green><b>" + xml.Name + "</b></font>");
                    }
                    else
                    {
                        this.Pub1.AddLi("Action.aspx?FK_MapData=" + this.FK_MapData + "&Event=" + xml.No + "&MyPK=" + nde.MyPK, "<b>" + xml.Name + "</b>");
                    }
                }
            }
            this.Pub1.AddULEnd();

            if (myEnentXml == null)
            {
                this.Pub2.AddFieldSet("帮助");
                
                this.Pub2.AddH2("关于驰骋工作流引擎的事件描述.");
                this.Pub2.AddUL();
                this.Pub2.AddLi("事件是ccbpm与您的应用程序接口，ccbpm在运动过程中产生很事件，比如：发送前、发送成功后、发送失败后、撤销前、撤消后、流程结束前、流程结束后。。。。");
                this.Pub2.AddLi("我们可以利用这些接口实现与业务系统的交互。");
                this.Pub2.AddLi("ccbpm为我们提供了两种类型的事件，一种是代码结构的，一种是配置模式的，该模式属于第二种，如何使用代码模式实现与ccbpm的交互请参考FEE，或者百度ccbpm 消息与事件。");
                this.Pub2.AddLi("如果您使用了代码结构的模式，该模式就会失效。");
                this.Pub2.AddULEnd();

                this.Pub2.AddFieldSetEnd();
                return;
            }

            FrmEvent mynde = ndevs.GetEntityByKey(FrmEventAttr.FK_Event, myEvent) as FrmEvent;
            if (mynde == null)
                mynde = new FrmEvent();

            this.Pub2.AddFieldSet(myEnentXml.Name);
            this.Pub2.Add("要执行的内容<br>");
            TextBox tb = new TextBox();
            tb.ID = "TB_Doc";
            tb.Columns = 70;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 5;
            tb.Text = mynde.DoDoc;
            this.Pub2.Add(tb);
            this.Pub2.AddHR();

            this.Pub2.Add("内容类型:");
            DDL ddl = new DDL();
            ddl.BindSysEnum("EventDoType");
            ddl.ID = "DDL_EventDoType";
            ddl.SetSelectItem((int)mynde.HisDoType);
            this.Pub2.Add(ddl);
            this.Pub2.AddHR();

            tb = new TextBox();
            tb.ID = "TB_MsgOK";
            tb.Columns = 70;
            tb.Text = mynde.MsgOKString;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 13;

            this.Pub2.Add("执行成功信息提示<br>");
            this.Pub2.Add(tb);
            this.Pub2.AddHR();

            this.Pub2.Add("执行失败信息提示<br>");
            tb = new TextBox();
            tb.ID = "TB_MsgErr";
            tb.Columns = 70;
            tb.Text = mynde.MsgErrorString;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 3;
            this.Pub2.Add(tb);
            this.Pub2.AddFieldSetEnd();

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.CssClass = "Btn";
            btn.Text = "  Save  ";
            btn.Click += new EventHandler(btn_Click);
            this.Pub2.Add(btn);

            if (this.MyPK != null)
                this.Pub2.Add("&nbsp;&nbsp;<a href=\"javascript:DoDel('" + this.FK_MapData + "','" + this.Event + "')\"><img src='../../Img/Btn/Delete.gif' />删除</a>");
        }
        void btn_Click(object sender, EventArgs e)
        {
            FrmEvent fe = new FrmEvent();
            fe.MyPK = this.FK_MapData + "_" + this.Event;
            fe.RetrieveFromDBSources();

            EventListDtls xmls = new EventListDtls();
            xmls.RetrieveAll();
            foreach (EventListDtl xml in xmls)
            {
                if (xml.No != this.Event)
                    continue;

                string doc = this.Pub2.GetTextBoxByID("TB_Doc").Text.Trim();
                if (doc == "" || doc == null)
                {
                    if (fe.MyPK.Length > 3)
                        fe.Delete();
                    continue;
                }

                fe.MyPK = this.FK_MapData + "_" + xml.No;
                fe.DoDoc = doc;
                fe.FK_Event = xml.No;
                fe.FK_MapData = this.FK_MapData;
                fe.HisDoType = (EventDoType)this.Pub2.GetDDLByID("DDL_EventDoType").SelectedItemIntVal;
                fe.MsgOKString = this.Pub2.GetTextBoxByID("TB_MsgOK").Text;
                fe.MsgErrorString = this.Pub2.GetTextBoxByID("TB_MsgErr").Text;
                fe.Save();
                this.Response.Redirect("Action.aspx?FK_MapData=" + this.FK_MapData + "&MyPK=" + fe.MyPK + "&Event=" + xml.No, true);
                return;
            }
        }
    }
}