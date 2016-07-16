using System;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.WF.Template.XML;

namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_FrmEvent : BP.Web.WebPage
    {
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Title = "表单事件";
          FrmEventXmls xmls = new FrmEventXmls();
            xmls.RetrieveAll();

            FrmEventXml curr = null;
            this.Pub1.Add("<a href='http://ccflow.org' target=_blank ><img src='../../DataUser/ICON/" + SystemConfig.CustomerNo + "/LogBiger.png' border=0 width='120px;' /></a><hr>");
            this.Pub1.AddUL();
            foreach (FrmEventXml xml in xmls)
            {
                if (xml.No == this.DoType)
                {
                    curr = xml;
                    this.Pub1.AddLi("<a href='FrmEvent.aspx?DoType=" + xml.No + "&FK_MapData=" + this.FK_MapData + "' ><b>" + xml.Name + "</b></a>");
                }
                else
                    this.Pub1.AddLi("<a href='FrmEvent.aspx?DoType=" + xml.No + "&FK_MapData=" + this.FK_MapData + "' >" + xml.Name + "</a>");
            }
            this.Pub1.AddULEnd();

            if (this.DoType == null)
            {
                this.Pub2.AddFieldSet("Help");
                this.Pub2.AddH2("什么是表单事件？");
                this.Pub2.AddH2("如何使用表单事件？");
                this.Pub2.Add("请参考操作手册, http://ccflow.org .");
                this.Pub2.AddFieldSetEnd();
                return;
            }

            FrmEvent fe = new FrmEvent(this.FK_MapData, this.DoType);
            this.Pub2.AddTable("width=100%");
            this.Pub2.AddCaptionLeft("表单事件:" + curr.Name);
            this.Pub2.AddTR();
            this.Pub2.AddTD("事件类型");
            BP.Web.Controls.DDL ddl = new BP.Web.Controls.DDL();
            ddl.ID = "DDL_" + FrmEventAttr.FK_Event;
            ddl.BindSysEnum("EventDoType", (int)fe.HisDoType);
            this.Pub2.AddTD(ddl);
            this.Pub2.AddTREnd();

            this.Pub2.AddTR();
            this.Pub2.AddTDBegin("colspan=2");
            this.Pub2.Add("执行内容<br>");
            BP.Web.Controls.TB tb = new BP.Web.Controls.TB();
            tb.ID = "TB_" + FrmEventAttr.DoDoc;
            tb.Text = fe.DoDoc;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Columns = 60;
            tb.Rows = 5;
            this.Pub2.Add(tb);
            this.Pub2.AddTDEnd();
            this.Pub2.AddTREnd();

            this.Pub2.AddTR();
            this.Pub2.AddTDBegin("colspan=2");
            this.Pub2.Add("执行成功提示<br>");
            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_" + FrmEventAttr.MsgOK;
            tb.Text = fe.MsgOKString;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Columns = 60;
            tb.Rows = 5;
            this.Pub2.Add(tb);
            this.Pub2.AddTDEnd();
            this.Pub2.AddTREnd();

            this.Pub2.AddTR();
            this.Pub2.AddTDBegin("colspan=2");
            this.Pub2.Add("执行错误提示<br>");
            tb = new BP.Web.Controls.TB();
            tb.ID = "TB_" + FrmEventAttr.MsgError;
            tb.Text = fe.MsgErrorString;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Columns = 60;
            tb.Rows = 5;
            this.Pub2.Add(tb);
            this.Pub2.AddTDEnd();
            this.Pub2.AddTREnd();
            this.Pub2.AddTableEndWithHR();

            Button btn = new Button();
            btn.Click += new EventHandler(btn_Click);
            btn.Text = "保存";
            btn.CssClass = "Btn";
            this.Pub2.Add(btn);
        }
        void btn_Click(object sender, EventArgs e)
        {
            FrmEvent fe = new FrmEvent(this.FK_MapData, this.DoType);
            fe = this.Pub2.Copy(fe) as FrmEvent;
            fe.FK_Event = this.DoType;
            fe.FK_MapData = this.FK_MapData;
            fe.MyPK = this.FK_MapData + "_" + this.DoType;
            fe.SetValByKey(FrmEventAttr.DoType, this.Pub2.GetDDLByID("DDL_" + FrmEventAttr.FK_Event).SelectedItemIntVal);
            if (string.IsNullOrEmpty(fe.DoDoc))
                fe.Delete();
            else
                fe.Save();
        }
    }
}