using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Sys;
using BP.En;
using BP.DA;


namespace CCFlow.WF.MapDef.Rpt
{
    public partial class Frm_ColsChose : BP.Web.PageBase
    {
        /// <summary>
        /// 表单编号
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.Request["FK_MapData"];

            }
        }
        /// <summary>
        /// 选择
        /// </summary>
        public string IsChecked
        {
            get
            {
                string isChecked = this.Request["IsChecked"];
                if (DataType.IsNullOrEmpty(isChecked))
                    return "false";
                return this.Request["IsChecked"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.Pub1.AddTable("width=90% align=center");
            this.Pub1.AddCaptionLeft("请选择要显示的字段,然后点保存按钮.");
            MapAttrs mattrs = new MapAttrs();
            QueryObject qo = new QueryObject(mattrs);
            qo.AddWhere(MapAttrAttr.FK_MapData, this.FK_MapData);
            qo.addOrderBy(MapAttrAttr.X, MapAttrAttr.Y);
            qo.DoQuery();

            //已设置
            FrmReportFields reportFields = new FrmReportFields();
            reportFields.RetrieveByAttr(FrmReportFieldAttr.FK_MapData, FK_MapData);

            int cols = 4; //定义显示列数 从0开始。
            int idx = -1;
            bool is1 = false;
            foreach (MapAttr attr in mattrs)
            {
                idx++;
                if (idx == 0)
                    is1 = this.Pub1.AddTR(is1);

                CheckBox cb = new CheckBox();
                cb.ID = "CB_" + attr.KeyOfEn;
                cb.Text = attr.Name + "(" + attr.KeyOfEn + ")";
                cb.Checked = reportFields.Contains(FrmReportFieldAttr.KeyOfEn, attr.KeyOfEn);
                this.Pub1.AddTD(cb);

                if (idx == cols - 1)
                {
                    idx = -1;
                    this.Pub1.AddTREnd();
                }
            }
            while (idx != -1)
            {
                idx++;
                if (idx == cols - 1)
                {
                    idx = -1;
                    this.Pub1.AddTD();
                    this.Pub1.AddTREnd();
                }
                else
                {
                    this.Pub1.AddTD();
                }
            }
            CheckBox cbAll = new CheckBox();
            cbAll.ID = "CB_CheckedAll";
            cbAll.Text = "全选";
            cbAll.AutoPostBack = true;
            cbAll.Checked = IsChecked == "true" ? true : false;
            cbAll.CheckedChanged += new EventHandler(cbAll_CheckedChanged);
            this.Pub1.AddTR();
            this.Pub1.AddTD("colspan=4", cbAll);
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }

        void cbAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cbAll = (CheckBox)sender;
            if (cbAll != null)
            {
                FrmReportFields reportFields = new FrmReportFields();
                reportFields.Delete(FrmReportFieldAttr.FK_MapData, this.FK_MapData);
                if (cbAll.Checked)
                {
                    //查询表单所有属性
                    MapAttrs mattrs = new MapAttrs(this.FK_MapData);
                    //OID默认必须有
                    FrmReportField reportFieldPK = new FrmReportField();
                    reportFieldPK.MyPK = this.FK_MapData + "_OID";
                    reportFieldPK.FK_MapData = this.FK_MapData;
                    reportFieldPK.KeyOfEn = "OID";
                    reportFieldPK.Name = "编号";
                    reportFieldPK.UIWidth = "100";
                    reportFieldPK.Idx = 0;
                    reportFieldPK.Insert();
                    foreach (MapAttr attr in mattrs)
                    {
                        if (attr.KeyOfEn == "OID")
                            continue;

                        FrmReportField reportField = new FrmReportField();
                        reportField.MyPK = this.FK_MapData + "_" + attr.KeyOfEn;
                        reportField.FK_MapData = this.FK_MapData;
                        reportField.KeyOfEn = attr.KeyOfEn;
                        reportField.Name = attr.Name;
                        reportField.UIWidth = attr.UIWidth.ToString();
                        reportField.Idx = attr.Idx;
                        reportField.Insert();
                    }
                    this.Response.Redirect("Frm_ColsChose.aspx?IsChecked=true&FK_MapData=" + this.FK_MapData, true);
                }
                this.Response.Redirect("Frm_ColsChose.aspx?IsChecked=false&FK_MapData=" + this.FK_MapData, true);
            }
        }
        //保存
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            FrmReportFields reportFields = new FrmReportFields();
            reportFields.Delete(FrmReportFieldAttr.FK_MapData, this.FK_MapData);

            //OID默认必须有
            FrmReportField reportFieldPK = new FrmReportField();
            reportFieldPK.MyPK = this.FK_MapData + "_OID";
            reportFieldPK.FK_MapData = this.FK_MapData;
            reportFieldPK.KeyOfEn = "OID";
            reportFieldPK.Name = "编号";
            reportFieldPK.UIWidth = "100";
            reportFieldPK.Idx = 0;
            reportFieldPK.Insert();
            //查询表单所有属性
            MapAttrs mattrs = new MapAttrs(this.FK_MapData);
            foreach (MapAttr attr in mattrs)
            {
                CheckBox cb = this.Pub1.GetCBByID("CB_" + attr.KeyOfEn);
                if (cb == null || attr.KeyOfEn == "OID")
                    continue;
                if (cb.Checked == false)
                    continue;

                FrmReportField reportField = new FrmReportField();
                reportField.MyPK = this.FK_MapData + "_" + attr.KeyOfEn;
                reportField.FK_MapData = this.FK_MapData;
                reportField.KeyOfEn = attr.KeyOfEn;
                reportField.Name = attr.Name;
                reportField.UIWidth = attr.UIWidth.ToString();
                reportField.Idx = attr.Idx;
                reportField.Insert();
            }
            this.Alert("保存成功。");
        }
        //设置列表显示
        protected void Btn_Column_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("Frm_ColsLabel.aspx?FK_MapData=" + this.FK_MapData, true);
        }
    }
}