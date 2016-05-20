using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.Sys;

namespace CCFlow.WF.MapDef
{
    public partial class RenameFieldsName : System.Web.UI.Page
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
            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("IDX");

            this.Pub1.AddTDTitle("字段中文名-原来");
            this.Pub1.AddTDTitle("字段中文名");

            this.Pub1.AddTDTitle("字段英文名-原来");
            this.Pub1.AddTDTitle("字段英文名");

            this.Pub1.AddTDTitle("最小长度");
            this.Pub1.AddTDTitle("最大长度");
            //add by myflow-大连 2014-08-01
            this.Pub1.AddTDTitle("排序");
            //end
            this.Pub1.AddTREnd();

            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            int idx = 0;
            foreach (MapAttr  attr in attrs)
            {
                if (attr.IsPK)
                    continue;

                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx);

                //描述.
                this.Pub1.AddTD(attr.Name);
                TextBox tb = new TextBox();
                tb.ID = "TB_Name_" + attr.KeyOfEn;
                tb.Text = attr.Name;
                this.Pub1.AddTD(tb);

                // 字段.
                this.Pub1.AddTD(attr.KeyOfEn);
                  tb = new TextBox();
                tb.ID = "TB_"+attr.KeyOfEn;
                tb.Text = attr.KeyOfEn;
                this.Pub1.AddTD(tb);


                //最小长度.
                tb = new TextBox();
                tb.ID = "TB_MinLen_" + attr.KeyOfEn;
                tb.Columns = 3;
                tb.Text = attr.MinLen.ToString();
                this.Pub1.AddTD(tb);

                //最大长度.
                tb = new TextBox();
                tb.ID = "TB_MaxLen_" + attr.KeyOfEn;
                tb.Text = attr.MaxLen.ToString();
                tb.Columns = 3;
                this.Pub1.AddTD(tb);
                //add by myflow-大连 2014-08-01
                //排序.
                tb = new TextBox();
                tb.ID = "TB_IDX_" + attr.KeyOfEn;
                tb.Text = attr.Idx.ToString();
                tb.Columns = 3;
                this.Pub1.AddTD(tb);
                //end

                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "保存";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
        }

        void btn_Click(object sender, EventArgs e)
        {
            MapAttrs attrs = new MapAttrs(this.FK_MapData);
            int idx = 0;
            string err = "";
            string info = "";
            foreach (MapAttr attr in attrs)
            {
                if (attr.IsPK || attr.KeyOfEn=="Title")
                    continue;

                try
                {
                    TextBox tb = this.Pub1.GetTextBoxByID("TB_" + attr.KeyOfEn);
                    string filed = tb.Text.Trim();

                    tb = this.Pub1.GetTextBoxByID("TB_Name_" + attr.KeyOfEn);
                    string name = tb.Text.Trim();

                    int minLen = int.Parse(this.Pub1.GetTextBoxByID("TB_MinLen_" + attr.KeyOfEn).Text);
                    int maxLen = int.Parse(this.Pub1.GetTextBoxByID("TB_MaxLen_" + attr.KeyOfEn).Text);
                    idx = int.Parse(this.Pub1.GetTextBoxByID("TB_IDX_" + attr.KeyOfEn).Text);
                    if (attr.KeyOfEn != filed)
                    {
                        attr.Delete();

                        attr.KeyOfEn = filed;
                        attr.Name = name;
                        attr.MaxLen = maxLen;
                        attr.MinLen = minLen;
                        //add by myflow-大连 2014-08-01
                        attr.Idx = idx;
                        //end
                        attr.MyPK = attr.FK_MapData + "_" + filed;
                        attr.Insert();

                        info += "@字段:" + attr.KeyOfEn + "," + attr.Name + "重命名成功.";
                        continue;
                    }

                    bool isChange = false;
                    if (attr.Name != name)
                        isChange = true;

                    if (attr.MinLen != minLen)
                        isChange = true;

                    if (attr.MaxLen != maxLen)
                        isChange = true;
                    if (attr.Idx != idx)
                        isChange = true;
                    if (isChange == false)
                        continue;

                    attr.MaxLen = maxLen;
                    attr.MinLen = minLen;
                    attr.Name = name;
                    //add by myflow-大连 2014-08-01
                    attr.Idx = idx;
                    //end
                    attr.Update();
                    info += "@字段:" + attr.KeyOfEn + "," + attr.Name + "更改成功.";

                }
                catch (Exception ex)
                {
                    err += "@字段:" + attr.KeyOfEn + "," + attr.Name + ";保存失败:" + ex.Message;
                }
            }

            if (string.IsNullOrEmpty(info) == false)
                this.Pub2.AddFieldSet("保存成功信息", info);

            if (string.IsNullOrEmpty(err) == false)
                this.Pub2.AddFieldSet("保存失败信息", err);
            return;
        }
    }
}