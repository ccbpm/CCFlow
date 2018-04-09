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
using BP.WF.Template;
using BP.Sys.XML;
using BP.DA;


namespace CCFlow.WF.MapDef
{
    public partial class UIRegularExpression_old :  BP.Web.WebPage
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
                return MapExtXmlList.RegularExpression;
            }
        }

        public string Lab = null;
        #endregion 属性。

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.DoType == "templete")//选择模版
            {
                this.BindReTemplete();
                return;
            }

            this.Page.Title = "为字段["+this.RefNo+"]设置正则表达式.";

            //this.Pub1.AddTable();
            this.Pub1.AddTable("class='Table' cellpadding='0' cellspacing='0' border='0' style='width:100%'");
            this.Pub1.AddCaptionLeft("为字段[" + this.RefNo + "]设置正则表达式." + BP.WF.Glo.GenerHelpCCForm("正则表达式", "http://ccform.mydoc.io/?v=5769&t=36728", "ss"));

            this.Pub1.AddTR();
            this.Pub1.AddTDGroupTitle("序");
            this.Pub1.AddTDGroupTitle("事件");
            this.Pub1.AddTDGroupTitle("事件内容");
            this.Pub1.AddTDGroupTitle("提示信息");
            this.Pub1.AddTREnd();

            #region 绑定事件
            int idx = 1;
            idx = BindRegularExpressionEditExt(idx, "onblur");
            idx = BindRegularExpressionEditExt(idx, "onchange");

            idx = BindRegularExpressionEditExt(idx, "onclick");
            idx = BindRegularExpressionEditExt(idx, "ondblclick");

            idx = BindRegularExpressionEditExt(idx, "onkeypress");
            idx = BindRegularExpressionEditExt(idx, "onkeyup");
            idx = BindRegularExpressionEditExt(idx, "onsubmit");
            #endregion

            this.Pub1.AddTableEnd();

            //Button btn = new Button();
            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            //btn.ID = "Btn_Save";
            //btn.Text = "保存";
            btn.Click += new EventHandler(BindRegularExpressionEdit_Click);
            this.Pub1.Add(btn);

            LinkBtn  mybtn = new LinkBtn(false, NamesOfBtn.Excel, "加载模版");
            mybtn.Click += new EventHandler(Excel_Click);
            this.Pub1.Add(mybtn);
        }

        void Excel_Click(object sender, EventArgs e)
        {
            string url = "RegularExpression.aspx?s=3&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "&ExtType=" +
                                       this.ExtType + "&OperAttrKey=" + this.OperAttrKey +
                                       "&DoType=templete";
            this.Response.Redirect(url,true);
        }

        void BindRegularExpressionEdit_Click(object sender, EventArgs e)
        {
            #region 保存
            BindRegularExpressionEdit_ClickSave("onblur");
            BindRegularExpressionEdit_ClickSave("onchange");
            BindRegularExpressionEdit_ClickSave("onclick");
            BindRegularExpressionEdit_ClickSave("ondblclick");
            BindRegularExpressionEdit_ClickSave("onkeypress");
            BindRegularExpressionEdit_ClickSave("onkeyup");
            BindRegularExpressionEdit_ClickSave("onsubmit");
            #endregion

            if (this.RefNo.Contains(",") == true)
            {
                this.WinCloseWithMsg("批量设置保存成功...");
            }
            else
            {
                this.Response.Redirect("RegularExpression.aspx?s=3&FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "&OperAttrKey=" + this.OperAttrKey, true);
            }
        }
        public int BindRegularExpressionEditExt(int idx, string myEvent)
        {
            // 查询.
            MapExt me = new MapExt();
            me.FK_MapData = this.FK_MapData;
            me.Tag = myEvent;
            me.AttrOfOper = this.RefNo;
            me.MyPK = me.FK_MapData + "_" + this.RefNo + "_" + this.ExtType + "_" + myEvent;
            me.RetrieveFromDBSources();

            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx);
            this.Pub1.AddTD("style='font-size:12px'", myEvent);

            TextBox tb = new TextBox();
            tb.TextMode = TextBoxMode.MultiLine;
            tb.ID = "TB_Doc_" + myEvent;
            tb.Text = me.Doc;
            tb.Columns = 50;
            tb.Rows = 1;
            tb.Style.Add("width", "99%");
            this.Pub1.AddTD(tb);

            tb = new TextBox();
            tb.ID = "TB_Tag1_" + myEvent;
            tb.Text = me.Tag1;
            tb.Columns = 20;
            tb.Rows = 3;
            tb.Style.Add("width", "99%");
            this.Pub1.AddTD(tb);
            this.Pub1.AddTREnd();
            idx = idx + 1;
            return idx;
        }
        public void BindRegularExpressionEdit_ClickSave(string myEvent)
        {

            string[] fields = this.RefNo.Split(',');
            foreach (string filed in fields)
            {
                if (DataType.IsNullOrEmpty(filed))
                    continue;

                MapExt me = new MapExt();
                me.FK_MapData = this.FK_MapData;
                me.ExtType = this.ExtType;
                me.Tag = myEvent;
                me.AttrOfOper = filed;
                me.MyPK = this.FK_MapData + "_" + filed + "_" + me.ExtType + "_" + me.Tag;
                me.Delete();

                me.Doc = this.Pub1.GetTextBoxByID("TB_Doc_" + myEvent).Text;
                me.Tag1 = this.Pub1.GetTextBoxByID("TB_Tag1_" + myEvent).Text;
                if (me.Doc.Trim().Length == 0)
                    return;
                me.Insert();
            }
        }
        public void BindReTemplete()
        {
            this.Pub1.AddTable("class='Table' cellpadding='0' cellspacing='0' border='0' style='width:100%'");
            this.Pub1.AddCaptionLeft("使用事件模版,能够帮助您快速的定义表单字段事件" + BP.WF.Glo.GenerHelpCCForm("选择模版", "http://ccform.mydoc.io/?v=5769&t=36729", ""));

            //this.Pub1.AddTR();
            //this.Pub1.AddTDGroupTitle("colspan=2", "事件模版-点击名称选用它");
            //this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            ListBox lb = new ListBox();
            lb.Style["width"] = "100%";
            lb.AutoPostBack = false;
            lb.ID = "LBReTemplete";
            lb.Height = 250;

            BP.Sys.XML.RegularExpressions res = new BP.Sys.XML.RegularExpressions();
            res.RetrieveAll();
            foreach (RegularExpression item in res)
            {
                ListItem li = new ListItem(item.Name + "->" + item.Note, item.No);
                lb.Items.Add(li);
            }
            this.Pub1.AddTD("colspan=2", lb);
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            var btn = new LinkBtn(false, NamesOfBtn.Save, "保存");
            btn.Click += new EventHandler(btn_SaveReTemplete_Click);


            this.Pub1.AddTDBegin("colspan=2");
            //this.Pub1.AddTD("colspan=1 width='80'", btn);
            this.Pub1.Add(btn);
            this.Pub1.AddSpace(2);

            if (this.RefNo != null && this.RefNo.Contains(","))
            {

            }
            else
            {
                this.Pub1.Add("<a class='easyui-linkbutton' data-options=\"iconCls:'icon-back'\" href='RegularExpression.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=" + this.ExtType + "&OperAttrKey=" + this.OperAttrKey + "&DoType=New'>返回</a>");
            }

            this.Pub1.AddTDEnd();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();

        }
        public void btn_SaveReTemplete_Click(object sender, EventArgs e)
        {
            ListBox lb = this.Pub1.FindControl("LBReTemplete") as ListBox;
            if (lb == null && lb.SelectedItem.Value == null) return;

            string newMyPk = "";
            RegularExpressionDtls reDtls = new RegularExpressionDtls();
            reDtls.RetrieveAll();

            //删除现有的逻辑.
            BP.Sys.MapExts exts = new BP.Sys.MapExts();


            string[] strs = this.RefNo.Split(',');
            foreach (string field in strs)
            {
                if (DataType.IsNullOrEmpty(field)==true)
                    continue;

                exts.Delete(MapExtAttr.FK_MapData, this.FK_MapData, MapExtAttr.AttrOfOper, field, MapExtAttr.ExtType, BP.Sys.MapExtXmlList.RegularExpression);
            }
       

            // 开始装载.
            foreach (RegularExpressionDtl dtl in reDtls)
            {
                if (dtl.ItemNo != lb.SelectedItem.Value)
                    continue;

                foreach (string field in strs)
                {
                    if (DataType.IsNullOrEmpty(field) == true)
                        continue;

                    BP.Sys.MapExt ext = new BP.Sys.MapExt();
                    ext.MyPK = this.FK_MapData + "_" + field + "_" + MapExtXmlList.RegularExpression + "_" + dtl.ForEvent;
                    ext.FK_MapData = this.FK_MapData;
                    ext.AttrOfOper = field;
                    ext.Doc = dtl.Exp; //表达公式.
                    ext.Tag = dtl.ForEvent; //时间.
                    ext.Tag1 = dtl.Msg;  //消息
                    ext.ExtType = MapExtXmlList.RegularExpression; // 表达公式 .
                    ext.Insert();
                    newMyPk = ext.MyPK;
                }
            }

            if (this.RefNo != null && this.RefNo.Contains(","))
            {
                this.WinCloseWithMsg("已经保存成功.");
                return;
            }
            else
            {
                this.Response.Redirect("RegularExpression.aspx?FK_MapData=" + this.FK_MapData + "&RefNo=" + this.RefNo + "&ExtType=" + this.ExtType + "&MyPK=" + newMyPk + "&OperAttrKey=" + this.OperAttrKey + "&DoType=New", true);
            }
        }
    }
}