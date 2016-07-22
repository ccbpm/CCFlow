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
using BP.Sys;
using BP.En;
using BP.Web;
using BP.Web.Controls;
using BP.Web.UC;
namespace CCFlow.WF.MapDef
{

    public partial class Comm_MapDef_SFTableEditData : BP.Web.WebPage
    {
        #region 属性.
        public new string DoType
        {
            get
            {
                return this.Request.QueryString["DoType"];
            }
        }
        public string FK_SFTable
        {
            get
            {
                return this.Request.QueryString["FK_SFTable"];
            }
        }
        
        public string IDX
        {
            get
            {
                return this.Request.QueryString["IDX"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Request.QueryString["EnPK"] != null)
            {
                GENoName en = new GENoName(this.FK_SFTable, "");
                en.No = this.Request.QueryString["EnPK"];
                en.Delete();
            }

            this.Title = "编辑表数据";
            this.BindSFTable();
        }

        public void BindSFTable()
        {
            SFTable sf = new SFTable(this.FK_SFTable);
            var canEdit = sf.FK_SFDBSrc == "local"; //todo:此处判断不准确，需更加精确的判断??

            this.Title = (canEdit ? "编辑:" : "查看:") + sf.Name;
            this.Pub1.AddTable("class='table' cellpadding='1' cellspacing='1' border='1' style='width:100%'");
            this.Pub1.AddTR();
            this.Pub1.AddTDGroupTitle("style='width:80px;text-align:center'", "编号");
            this.Pub1.AddTDGroupTitle("名称");

            if (canEdit)
                this.Pub1.AddTDGroupTitle("style='width:80px'", "操作");

            this.Pub1.AddTREnd();

            GENoNames ens = new GENoNames(sf.No, sf.Name);
            QueryObject qo = new QueryObject(ens);
            try
            {
                this.Pub2.BindPageIdxEasyUi(qo.GetCount(), "SFTableEditData.aspx?FK_SFTable=" + this.FK_SFTable, this.PageIdx);
            }
            catch
            {
                sf.CheckPhysicsTable();
                this.Pub2.BindPageIdxEasyUi(qo.GetCount(), "SFTableEditData.aspx?FK_SFTable=" + this.FK_SFTable, this.PageIdx);
            }

            qo.DoQuery("No", 10, this.PageIdx, false);

            foreach (GENoName en in ens)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(en.No);
                TextBox tb = new TextBox();
                tb.ID = "TB_" + en.No;
                tb.Text = en.Name;
                tb.Style.Add("width", "99%");
                tb.ReadOnly = !canEdit;

                this.Pub1.AddTD(tb);

                if (canEdit)
                    this.Pub1.AddTD("<a href=\"javascript:Del('" + this.FK_SFTable + "','" + this.PageIdx + "','" + en.No + "')\" class='easyui-linkbutton' data-options=\"iconCls:'icon-delete'\" >删除</a>");
                else
                    this.Pub1.AddTD();

                this.Pub1.AddTREnd();
            }

            if (canEdit)
            {
                GENoName newen = new GENoName(sf.No, sf.Name);
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx("新记录");
                TextBox tb1 = new TextBox();
                tb1.ID = "TB_Name";
                tb1.Text = newen.Name;
                tb1.Style.Add("width", "99%");

                this.Pub1.AddTD(tb1);

                var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
                btn.Click += new EventHandler(btn_Click);

                this.Pub1.AddTD(btn);
                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTableEnd();

            //this.Pub3.AddTable();
            //this.Pub3.AddTRSum();
            //this.Pub3.AddTD("编号");
            //this.Pub3.AddTD("名称");
            //this.Pub3.AddTD("");
            //this.Pub3.AddTREnd();

            //GENoName newen = new GENoName(sf.No, sf.Name);
            //this.Pub3.AddTRSum();
            //this.Pub3.AddTD(newen.GenerNewNo);
            //TextBox tbn = new TextBox();
            //tbn.ID = "TB_Name";

            //this.Pub3.AddTD(tbn);
            //Button btn = new Button();
            //btn.Text = "增加";
            //btn.Click += new EventHandler(btn_Click);
            //this.Pub3.AddTD(btn);
            //this.Pub3.AddTREnd();
            //this.Pub3.AddTableEnd();
        }

        void btn_Click(object sender, EventArgs e)
        {
            //批量保存数据。
            GENoNames ens = new GENoNames(this.FK_SFTable,"自定义表");
            QueryObject qo = new QueryObject(ens);
            qo.DoQuery("No", 10, this.PageIdx, false);
            foreach (GENoName myen in ens)
            {
                string no = myen.No;
                string name1 = this.Pub1.GetTextBoxByID("TB_" + myen.No).Text;
                if (name1 == "")
                    continue;
                BP.DA.DBAccess.RunSQL("update " + this.FK_SFTable + " set Name='" + name1 + "' WHERE no='" + no + "'");
            }


            BP.En.GENoName en = new GENoName(this.FK_SFTable, "sd");
            string name = this.Pub1.GetTextBoxByID("TB_Name").Text.Trim();
            if (name.Length > 0)
            {
                en.Name = name;
                en.No = en.GenerNewNo;
                en.Insert();
                this.Response.Redirect("SFTableEditData.aspx?FK_SFTable=" + this.FK_SFTable + "&PageIdx=" + this.PageIdx, true);
            }
        }
    }

}