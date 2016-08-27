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
using BP.WF;
using BP.En;
using BP.Web;
using BP.DA;

namespace CCFlow.WF
{
    public partial class WF_WFDtl : BP.Web.WebPage
    {
        public new string EnName
        {
            get
            {
                return this.Request.QueryString["EnName"];
            }
        }
        public new string RefPK
        {
            get
            {
                return this.Request.QueryString["RefPK"];
            }
        }
        public string OID
        {
            get
            {
                return this.Request.QueryString["OID"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Page.RegisterClientScriptBlock("s",
              "<link href='./Comm/Style/Table" + BP.Web.WebUser.Style + ".css' rel='stylesheet' type='text/css' />");
            this.Page.RegisterClientScriptBlock("sd",
            "<link href='./Comm/Style/Table.css' rel='stylesheet' type='text/css' />");


            BP.Sys.GEDtls dtls = new BP.Sys.GEDtls(this.EnName);
            QueryObject qo = new QueryObject(dtls);
            qo.AddWhere(BP.Sys.GEDtlAttr.RefPK, this.RefPK);
            qo.DoQuery();

            if (this.Request.QueryString["DoType"] == "Exp")
            {
                //  BP.Sys.PubClass.
            }

            //  throw new Exception(qo.SQL);

            Map map = dtls.GetNewEntity.EnMap;
            this.Ucsys1.AddTable();
            this.Ucsys1.AddCaptionLeft(map.EnDesc + " - <a href='WFRptDtl.aspx?RefPK=" + this.RefPK + "&EnName=" + this.EnName + "&DoType=Exp' ><img src='./Img/Btn/Excel.gif' border=0>输出到Excel</a>");
            this.Ucsys1.AddTR();
            this.Ucsys1.AddTDTitle("序");
            foreach (Attr attr in map.Attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                this.Ucsys1.AddTDTitle(attr.Desc);
            }
            this.Ucsys1.AddTREnd();

            int i = 1;
            bool is1 = false;
            foreach (BP.Sys.GEDtl dtl in dtls)
            {
                is1 = this.Ucsys1.AddTR(is1);
                this.Ucsys1.AddTDIdx(i++);
                foreach (Attr attr in map.Attrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.IsRefAttr)
                        continue;

                    switch (attr.MyDataType)
                    {

                        case DataType.AppInt:
                            this.Ucsys1.AddTDNum(dtl.GetValIntByKey(attr.Key));
                            break;
                        case DataType.AppMoney:
                            this.Ucsys1.AddTDNum(dtl.GetValIntByKey(attr.Key).ToString("0.00"));
                            break;
                        case DataType.AppFloat:
                            this.Ucsys1.AddTD(dtl.GetValFloatByKey(attr.Key));
                            break;
                        case DataType.AppBoolean:
                            if (dtl.GetValIntByKey(attr.Key) == 1)
                                this.Ucsys1.AddTD("是");
                            else
                                this.Ucsys1.AddTD("否");
                            break;
                        default:
                            if (attr.IsFKorEnum)
                                this.Ucsys1.AddTD(dtl.GetValRefTextByKey(attr.Key));
                            else
                                this.Ucsys1.AddTD(dtl.GetValStrByKey(attr.Key));
                            break;
                    }
                }
                this.Ucsys1.AddTREnd();
            }


            this.Ucsys1.AddTRSum();
            this.Ucsys1.AddTD();
            foreach (Attr attr in map.Attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                if (attr.IsNum == false || attr.MyDataType == BP.DA.DataType.AppBoolean)
                    this.Ucsys1.AddTD();
                else
                {
                    this.Ucsys1.AddTDNum(dtls.GetSumFloatByKey(attr.Key).ToString());
                }
            }
            this.Ucsys1.AddTREnd();

            this.Ucsys1.AddTableEnd();

            if (this.Request.QueryString["DoType"] != null)
            {
                //  this.ExportDGToExcel(dtls.ToDataTableDesc(), map.EnDesc);
            }
        }
    }

}