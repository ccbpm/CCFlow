using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.Web.Controls;

namespace CCFlow.WF.MapDef.Rpt
{
    public partial class ColsLabel : BP.Web.PageBase
    {
        #region 属性.
        public int Idx
        {
            get
            {
                string s = this.Request.QueryString["Idx"];
                if (string.IsNullOrEmpty(s))
                    s = "0";
                return int.Parse(s);
            }
        }
        public string FK_MapAttr
        {
            get
            {
                string s = this.Request.QueryString["FK_MapAttr"];
                return s;
            }
        }
        public string RptNo
        {
            get
            {
                return this.Request.QueryString["RptNo"];

            }
        }
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];

            }
        }
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];

            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            #region 处理活动.
            switch (this.Request.QueryString["ActionType"])
            {
                case "Left":
                    MapAttr attr = new MapAttr(this.FK_MapAttr);
                    attr.DoUp();
                    break;
                case "Right":
                    MapAttr attrR = new MapAttr(this.FK_MapAttr);
                    attrR.DoDown();
                    break;
                default:
                    break;
            }
            #endregion 处理活动.


            BP.WF.Rpt.MapRpt mrpt = new BP.WF.Rpt.MapRpt(RptNo);
            BP.Sys.MapAttrs attrs = new BP.Sys.MapAttrs(this.RptNo);

            this.Pub2.AddTable("class='Table' border='1' cellspacing='0' cellpadding='0' style='width:100%'");

            this.Pub2.AddTR();
            this.Pub2.AddTDGroupTitle("style='text-align:center;width:50px'", "序");
            this.Pub2.AddTDGroupTitle("字段");
            this.Pub2.AddTDGroupTitle("标签");
            this.Pub2.AddTDGroupTitle("显示顺序号");
            this.Pub2.AddTDGroupTitle("style='text-align:center;width:100px'", "调整");
            this.Pub2.AddTREnd();

            int idx = 0;
            string tKey = DateTime.Now.ToString("HHmmss");
            foreach (BP.Sys.MapAttr mattr in attrs)
            {
                switch (mattr.KeyOfEn)
                {
                    case BP.WF.Data.NDXRptBaseAttr.Title:
                    case BP.WF.Data.NDXRptBaseAttr.OID:
                    case BP.WF.Data.NDXRptBaseAttr.MyNum:
                        continue;
                    default:
                        break;
                }

                idx++;
                this.Pub2.AddTR();
                this.Pub2.AddTDIdx(idx);
                this.Pub2.AddTD(mattr.KeyOfEn);
                TextBox tb = new TextBox();
                tb.Text = mattr.Name;
                tb.ID = "TB_" + mattr.KeyOfEn;
                this.Pub2.AddTD(tb);

                tb = new TextBox();
                tb.ID = "TB_" + mattr.KeyOfEn + "_Idx";
                tb.Text = idx.ToString();
                //tb.ReadOnly = true;
                this.Pub2.AddTD(tb);

                //顺序.
                this.Pub2.AddTDBegin("style='text-align:center'");
                this.Pub2.Add("<a href='javascript:void(0)' onclick='up(this, 3)' class='easyui-linkbutton' data-options=\"iconCls:'icon-up'\"></a>&nbsp;");
                this.Pub2.Add("<a href='javascript:void(0)' onclick='down(this, 3)' class='easyui-linkbutton' data-options=\"iconCls:'icon-down'\"></a>");

                this.Pub2.AddTDEnd();
                this.Pub2.AddTREnd();
            }

            this.Pub2.AddTableEnd();
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            Save();

            this.Response.Redirect("S3_ColsLabel.aspx?FK_MapData=" + this.FK_MapData + "&RptNo=" + this.RptNo + "&FK_Flow=" + this.FK_Flow + "&s=" + DateTime.Now.ToString("yyyyMMddHHmmssffffff"), true);
        }

        protected void Btn_Cancel_Click(object sender, EventArgs e)
        {
            this.WinClose();
        }

        private void Save()
        {
            MapAttrs attrs = new MapAttrs(this.RptNo);
            foreach (MapAttr item in attrs)
            {
                switch (item.KeyOfEn)
                {
                    case BP.WF.Data.NDXRptBaseAttr.Title:
                    case BP.WF.Data.NDXRptBaseAttr.OID:
                    case BP.WF.Data.NDXRptBaseAttr.MyNum:
                        continue;
                    default:
                        break;
                }

                TextBox tb = this.Pub2.GetTextBoxByID("TB_" + item.KeyOfEn);
                item.Name = tb.Text;

                tb = this.Pub2.GetTextBoxByID("TB_" + item.KeyOfEn + "_Idx");
                item.Idx = int.Parse(tb.Text);

                item.Update();
            }
        }

        protected void Btn_SaveAndNext1_Click(object sender, EventArgs e)
        {
            Save();

            this.Response.Redirect("S5_SearchCond.aspx?FK_MapData=" + this.FK_MapData + "&RptNo=" + this.RptNo + "&FK_Flow=" + this.FK_Flow + "&s=" + DateTime.Now.ToString("yyyyMMddHHmmssffffff"), true);
        }
    }
}