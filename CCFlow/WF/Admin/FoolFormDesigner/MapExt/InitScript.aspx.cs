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
    public partial class InitScriptUI : BP.Web.WebPage
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
        public string ExtType
        {
            get
            {
                return MapExtXmlList.TBFullCtrl;
            }
        }

        public string Lab = null;
        #endregion 属性。

        protected void Page_Load(object sender, EventArgs e)
        {

            this.Pub1.AddTable("class='Table' cellspacing='0' cellpadding='0' border='0' style='width:100%'");

          //  this.Pub1.AddCaption("内置JavaScript脚本" + BP.WF.Glo.GenerHelpCCForm("帮助", "", ""));

            TextBox tb = new TextBox();
            tb.ID = "TB_Doc";
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 15;
            tb.Columns = 70;
            tb.Style.Add("width", "99%");

            string path= SystemConfig.PathOfDataUser+ "\\JSLibData\\"+this.FK_MapData+"_Self.js";
            if (System.IO.File.Exists(path))
                tb.Text = BP.DA.DataType.ReadTextFile(path);

            this.Pub1.AddTR();
            this.Pub1.AddTDBegin();
            this.Pub1.Add(tb);
            this.Pub1.AddBR();
            this.Pub1.AddP("编写说明：该内容是读取写入文件到:"+path+"。<br>您也可以通过js的编辑工具，编辑后放入该位置，表单在运行的时候会自动加载该文件，使用ccform的内置函数请点击右上角帮助。");
            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();

            //Button btn = new Button();
            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_Save_Click);

            this.Pub1.AddTR();
            this.Pub1.AddTD(btn);
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();

            string html = BP.DA.DataType.ReadTextFile2Html(BP.Sys.SystemConfig.PathOfData + "\\HelpDesc\\InitScript.txt");
            this.Pub1.AddFieldSet("帮助", html);

        }

        void btn_Save_Click(object sender, EventArgs e)
        {
            string txt = this.Pub1.GetTextBoxByID("TB_Doc").Text.Trim();
            if (DataType.IsNullOrEmpty(txt))
            {
                string path = SystemConfig.PathOfDataUser + "\\JSLibData\\" + this.FK_MapData + "_Self.js";
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }
            else
            {
                string path = SystemConfig.PathOfDataUser + "\\JSLibData\\" + this.FK_MapData + "_Self.js";
                BP.DA.DataType.WriteFile(path, txt);
            }
        }
    }
}