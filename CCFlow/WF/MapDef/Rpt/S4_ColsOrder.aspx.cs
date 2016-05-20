using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.WF;

namespace CCFlow.WF.MapDef.Rpt
{
    public partial class ShowCols : BP.Web.PageBase
    {
        #region 属性.
        public string FK_Flow
        {
            get
            {
                string s = this.Request.QueryString["FK_Flow"];
                if (s == null)
                    s = "007";
                return s;
            }
        }
        public int Idx
        {
            get
            {
                string s = this.Request.QueryString["Idx"];
                if (string.IsNullOrEmpty(s) )
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
                string s = this.Request.QueryString["RptNo"];
                return s;
            }
        }
        #endregion 属性.

        private void SetTitleFirst()
        {
           
        }

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

            MapAttrs attrs = new MapAttrs(this.RptNo);
            this.Pub2.AddTable("align=left");
            this.Pub2.AddCaptionLeft("列表字段显示顺序- 移动箭头改变顺序");
            this.Pub2.AddTR();
            this.Pub2.AddTDTitle("序");

            int idx = -1;
            string tKey = DateTime.Now.ToString("HHmmddss");
            foreach (MapAttr attr in attrs)
            {
                idx++;
                this.Pub2.Add("<TD class=Title>");
                if (idx != 0)
                    this.Pub2.Add("<a href=\"javascript:DoLeft('" + FK_Flow + "','" + this.RptNo + "','" + attr.MyPK + "','" + tKey + "')\" ><img src='/WF/Img/Arr/Arrowhead_Previous_S.gif' ></a>");

                this.Pub2.Add(attr.Name);

                this.Pub2.Add("<a href=\"javascript:DoRight('" + FK_Flow + "','" + this.RptNo + "','" + attr.MyPK + "','" + tKey + "')\" ><img src='/WF/Img/Arr/Arrowhead_Next_S.gif' ></a>");

                this.Pub2.Add("</TD>");
            }
            this.Pub2.AddTREnd();


            for (int i = 0; i < 12; i++)
            {
                this.Pub2.AddTR();
                this.Pub2.AddTDIdx(i);

                foreach (MapAttr attr in attrs)
                    this.Pub2.AddTD();
                this.Pub2.AddTREnd();
            }
            this.Pub2.AddTableEnd();
        }
        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("S5_SearchCond.aspx?RptNo=" + this.RptNo + "&FK_Flow=" + this.FK_Flow, true);
        }

        protected void Btn_Close_Click(object sender, EventArgs e)
        {
            this.WinClose();
        }
    }
}