using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.Web;
using BP.Web.UC;

namespace CCFlow.WF.MapDef
{
    public partial class WF_MapDef_HidAttr : BP.Web.WebPage
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
            this.Pub1.AddTable(" width='80%' ");
            this.Pub1.AddCaptionLeft("隐藏字段");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("IDX");
            this.Pub1.AddTDTitle("字段");
            this.Pub1.AddTDTitle("名称(点击编辑)");
            this.Pub1.AddTDTitle("类型");
            this.Pub1.AddTREnd();
            MapAttrs mattrs = new MapAttrs(this.FK_MapData);
            GroupFields gfs = new GroupFields(this.FK_MapData);
            int idx = 0;
            foreach (MapAttr attr in mattrs)
            {
                if (attr.UIVisible)
                    continue;
                switch (attr.KeyOfEn)
                {
                    case "BatchID":
                    case "OID":
                    case "FID":
                    case "FK_NY":
                    case "RefPK":
                    case "Emps":
                    case "FK_Dept":
                    case "WFState":
                    case "RDT":
                    case "MyNum":
                    case "Rec":
                    case "CDT":
                        continue;
                    default:
                        break;
                }
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx++);
                this.Pub1.AddTD(attr.KeyOfEn);
                switch (attr.LGType)
                {
                    case FieldTypeS.Normal:
                        this.Pub1.AddTD("<a href=\"javascript:Edit('" + attr.FK_MapData + "','" + attr.MyPK + "','" + attr.MyDataType + "');\">" + attr.Name + "</a>");
                        break;
                    case FieldTypeS.Enum:
                        this.Pub1.AddTD("<a href=\"javascript:EditEnum('" + attr.FK_MapData + "','" + attr.MyPK + "','" + attr.MyDataType + "');\">" + attr.Name + "</a>");
                        break;
                    case FieldTypeS.FK:
                        this.Pub1.AddTD("<a href=\"javascript:EditTable('" + attr.FK_MapData + "','" + attr.MyPK + "','" + attr.MyDataType + "');\">" + attr.Name + "</a>");
                        break;
                    default:
                        break;
                }
                this.Pub1.AddTD(attr.LGType.ToString());
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEndWithHR();
        }
    }
}