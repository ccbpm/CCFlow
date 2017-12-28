using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.DA;
namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_TabIdx : BP.Web.WebPage
    {
        #region 属性.
        public string FK_MapData
        {
            get
            {
                return this.Request.QueryString["FK_MapData"];
            }
        }
        public string KeyOfEn
        {
            get
            {
                return this.Request.QueryString["KeyOfEn"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            switch (this.DoType)
            {
                case "Up":
                    MapAttr ma = new MapAttr(this.FK_MapData + "_" + this.KeyOfEn);
                    ma.DoUpTabIdx();
                    break;
                case "Down":
                    MapAttr ma1 = new MapAttr(this.FK_MapData + "_" + this.KeyOfEn);
                    ma1.DoDownTabIdx();
                    break;
                default:
                    break;
            }

            string sql = "SELECT KeyOfEn,Name,IDX FROM Sys_MapAttr WHERE FK_MapData='" + this.FK_MapData + "' AND UIVisible=1  ORDER BY Idx ";
            this.Title = "设置tab键顺序";
            DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
            this.Pub1.AddTable("width='100%'");
            this.Pub1.AddCaption("正确的调整好Tab键的顺序,可以方便用户数据录入.");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("IDX");
            this.Pub1.AddTDTitle("字段");
            this.Pub1.AddTDTitle("描述");
            this.Pub1.AddTDTitle("顺序号");
            this.Pub1.AddTDTitle("移动");
            this.Pub1.AddTDTitle("移动");
            this.Pub1.AddTREnd();

            bool is1 = false;
            int idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTDIdx(idx);
                this.Pub1.AddTD(dr["KeyOfEn"].ToString());
                this.Pub1.AddTD(dr["Name"].ToString());
                this.Pub1.AddTD(dr["Idx"].ToString());
                this.Pub1.AddTD("<a href='TabIdx.aspx?DoType=Up&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + dr["KeyOfEn"].ToString() + "' ><img src='../../Img/Btn/Up.gif' border=0></a>");
                this.Pub1.AddTD("<a href='TabIdx.aspx?DoType=Down&FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + dr["KeyOfEn"].ToString() + "' ><img src='../../Img/Btn/Down.gif' border=0></a>");
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
        }
    }
}