using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.En;
using BP;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;


namespace CCFlow.WF.MapDef.MapExtUI
{
    public partial class PageLoadFullUI :  BP.Web.WebPage
    {
        #region 属性。
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }

        public string OperAttrKey
        {
            get
            {
                return this.Request.QueryString["OperAttrKey"];
            }
        }
        

        public string Lab = null;
        #endregion 属性。

        protected void Page_Load(object sender, EventArgs e)
        {
         
            MapExt me = new MapExt();
            me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull, MapExtAttr.FK_MapData, this.FK_MapData);

            this.Pub1.AddTable("class='Table' cellspacing='0' cellpadding='0' border='0' style='width:100%'");
            this.Pub1.AddTR();
            this.Pub1.AddTDGroupTitle("主表SQL设置"+BP.WF.Glo.GenerHelpCCForm("帮助","",""));
            this.Pub1.AddTREnd();

            TextBox tb = new TextBox();
            tb.ID = "TB_" + MapExtAttr.Tag;
            tb.Text = me.Tag;
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 7;
            tb.Columns = 70;
            tb.Style.Add("width", "99%");

            this.Pub1.AddTR();
            this.Pub1.AddTDBegin();
            this.Pub1.Add(tb);
            this.Pub1.AddBR();
            this.Pub1.Add("说明:填充主表的sql,表达式里支持@变量与约定的公用变量。 <br>比如: SELECT No,Name,Tel FROM Port_Emp WHERE No='@WebUser.No' , 如果列名与开始表单字段名相同，就会自动给值。");
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();

            MapDtls dtls = new MapDtls(this.FK_MapData);
            if (dtls.Count != 0)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTDGroupTitle("明细表SQL设置");
                this.Pub1.AddTREnd();

                string[] sqls = me.Tag1.Split('*');
                foreach (MapDtl dtl in dtls)
                {
                    this.Pub1.AddTR();
                    this.Pub1.AddTD("明细表:[" + dtl.No + "]&nbsp;" + dtl.Name);
                    this.Pub1.AddTREnd();
                    tb = new TextBox();
                    tb.ID = "TB_" + dtl.No;
                    foreach (string sql in sqls)
                    {
                        if (DataType.IsNullOrEmpty(sql))
                            continue;
                        string key = sql.Substring(0, sql.IndexOf('='));
                        if (key == dtl.No)
                        {
                            tb.Text = sql.Substring(sql.IndexOf('=') + 1);
                            break;
                        }
                    }

                    tb.TextMode = TextBoxMode.MultiLine;
                    tb.Rows = 5;
                    tb.Columns = 70;
                    tb.Style.Add("width", "99%");

                    this.Pub1.AddTR();
                    this.Pub1.AddTDBegin();
                    this.Pub1.Add(tb);
                    this.Pub1.AddBR();
                    this.Pub1.Add("说明:结果集合填充从表");
                    this.Pub1.AddTREnd();
                }
            }

            //Button btn = new Button();
            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_SavePageLoadFull_Click);

            this.Pub1.AddTR();
            this.Pub1.AddTD(btn);
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
            return;
        }
        /// <summary>
        /// 保存它
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_SavePageLoadFull_Click(object sender, EventArgs e)
        {
            MapExt me = new MapExt();

            me.Retrieve(MapExtAttr.ExtType, MapExtXmlList.PageLoadFull, MapExtAttr.FK_MapData, this.FK_MapData);
            me.RetrieveFromDBSources();

            me.FK_MapData = this.FK_MapData;
            me.ExtType = MapExtXmlList.PageLoadFull;

            me.Tag = this.Pub1.GetTextBoxByID("TB_" + MapExtAttr.Tag).Text;
            string sql = "";
            MapDtls dtls = new MapDtls(this.FK_MapData);
            foreach (MapDtl dtl in dtls)
            {
                sql += "*" + dtl.No + "=" + this.Pub1.GetTextBoxByID("TB_" + dtl.No).Text;
            }
            me.Tag1 = sql;

            me.MyPK = MapExtXmlList.PageLoadFull + "_" + this.FK_MapData;

            string info = me.Tag1 + me.Tag;
            if (DataType.IsNullOrEmpty(info))
                me.Delete();
            else
                me.Save();
        }
    }
}