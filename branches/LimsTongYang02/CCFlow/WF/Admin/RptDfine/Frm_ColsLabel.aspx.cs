using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BP.Sys;

namespace CCFlow.WF.MapDef.Rpt
{
    public partial class Frm_ColsLabel : BP.Web.PageBase
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
            FrmReportFields frmReportFields = new FrmReportFields();
            BP.En.QueryObject obj = new BP.En.QueryObject(frmReportFields);
            obj.AddWhere(FrmReportFieldAttr.FK_MapData, this.FK_MapData);
            obj.addOrderBy(FrmReportFieldAttr.Idx);
            obj.DoQuery();

            this.Pub1.AddTable("width='90%' align='center' id='myTable'");
            this.Pub1.AddCaptionLeft("修改报表列属性");

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");
            this.Pub1.AddTDTitle("字段");
            this.Pub1.AddTDTitle("标签");
            this.Pub1.AddTDTitle("显示宽度");
            this.Pub1.AddTDTitle("显示顺序号");
            this.Pub1.AddTDTitle("是否隐藏");
            this.Pub1.AddTREnd();

            int idx = 0;
            string tKey = DateTime.Now.ToString("HHmmss");
            foreach (FrmReportField mattr in frmReportFields)
            {
                idx++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx);
                this.Pub1.AddTD(mattr.KeyOfEn);
                TextBox tb = new TextBox();
                tb.Text = mattr.Name;
                tb.ID = "TB_" + mattr.KeyOfEn;
                this.Pub1.AddTD(tb);

                //宽度
                tb = new TextBox();
                tb.ID = "TB_" + mattr.KeyOfEn + "_width";
                tb.Text = mattr.UIWidth.ToString();
                this.Pub1.AddTD(tb);

                //顺序.
                tb = new TextBox();
                tb.ID = "TB_" + mattr.KeyOfEn + "_Idx";
                tb.Text = mattr.Idx.ToString();
                this.Pub1.AddTD(tb);

                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + mattr.KeyOfEn;
                cb.Checked = mattr.UIVisible == true ? false : true;
                this.Pub1.AddTD(cb);

                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            FrmReportFields frmReportFields = new FrmReportFields();
            frmReportFields.RetrieveByAttr(FrmReportFieldAttr.FK_MapData, this.FK_MapData);
            foreach (FrmReportField item in frmReportFields)
            {
                TextBox tb = this.Pub1.GetTextBoxByID("TB_" + item.KeyOfEn);
                item.Name = tb.Text;

                tb = this.Pub1.GetTextBoxByID("TB_" + item.KeyOfEn + "_width");
                item.UIWidth = tb.Text;

                tb = this.Pub1.GetTextBoxByID("TB_" + item.KeyOfEn + "_Idx");
                item.Idx = int.Parse(tb.Text);

                CheckBox cb = this.Pub1.GetCBByID("CB_" + item.KeyOfEn);
                item.UIVisible = cb.Checked ? false : true;
                item.Update();
            }
            this.Response.Redirect("Frm_ColsLabel.aspx?FK_MapData=" + this.FK_MapData, true);
        }
        protected void Btn_Return_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("Frm_ColsChose.aspx?FK_MapData=" + this.FK_MapData, true);
        }
        protected void Btn_Close_Click(object sender, EventArgs e)
        {
            this.WinClose();
        }
    }
}