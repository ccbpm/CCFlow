using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using BP.Web;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Sys;
namespace CCFlow.WF
{
    public partial class WF_CallSubFlow : BP.Web.WebPage
    {
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string FK_FlowFrom
        {
            get
            {
                return this.Request.QueryString["FK_FlowFrom"];
            }
        }
        public int FID
        {
            get
            {
                return int.Parse(this.Request.QueryString["FID"]);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

            Flow fl = new Flow(this.FK_Flow);
            Node nd = new Node(int.Parse(this.FK_Flow + "01"));
            Works wks = nd.HisWorks;
            QueryObject qo = new QueryObject(wks);
            qo.AddWhere(WorkAttr.FID, this.FID);
            qo.DoQuery();

            Flow from = new Flow(this.FK_FlowFrom);
            string currNode = BP.DA.DBAccess.RunSQLReturnVal("SELECT FK_Node FROM WF_GenerWorkFlow WHERE WorkID=" + this.FID) as string;
            if (wks.Count == 0)
            {
                this.Response.Redirect("MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FromNode=" + this.Request.QueryString["FromNode"]);
                return;
            }


            //this.Pub2.BindPageIdx(qo.GetCount(), 10, this.PageIdx, "CallSubFlow.aspx?FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FK_FlowFrom=" + this.FK_FlowFrom);
            //qo.DoQuery("OID", 10, this.PageIdx);

            // 生成页面数据。
            Attrs attrs = nd.HisWork.EnMap.Attrs;
            int colspan = 2;
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;
                colspan++;
            }
            this.Pub1.AddTable("width='100%' align=center");
            this.Pub1.AddTR();
            this.Pub1.Add("<TD class=TitleTop colspan=" + colspan + "></TD>");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.Add("<TD class=TitleMsg  align=left colspan=" + colspan + "><img src='./Img/EmpWorks.gif' > <b>您的位置:<a href='MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&WorkID=" + this.FID + "' >流程处理:" + from.Name + "</a> => <a href='MyFlow.aspx?FK_Flow=" + this.FK_Flow + "&FID=" + this.FID + "&FromNode=" + currNode + "'>流程发起</a></b> - <a href=\"javascript:WinOpen('./Comm/Search.aspx?EnsName=ND" + int.Parse(this.FK_Flow) + "Rpt');\" >流程查询</a></TD>");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                this.Pub1.AddTDTitle(attr.Desc);
            }
            this.Pub1.AddTDTitle("操作");
            this.Pub1.AddTREnd();

            int idx = 0;
            bool is1 = false;
            foreach (Entity en in wks)
            {
                idx++;
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTD(idx);
                foreach (Attr attr in attrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    switch (attr.MyDataType)
                    {
                        case DataType.AppBoolean:
                            this.Pub1.AddTD(en.GetValBoolStrByKey(attr.Key));
                            break;
                        case DataType.AppFloat:
                        case DataType.AppInt:
                        case DataType.AppDouble:
                            this.Pub1.AddTD(en.GetValFloatByKey(attr.Key));
                            break;
                        case DataType.AppMoney:
                            this.Pub1.AddTDMoney(en.GetValDecimalByKey(attr.Key));
                            break;
                        default:
                            this.Pub1.AddTD(en.GetValStrByKey(attr.Key));
                            break;
                    }
                }
                this.Pub1.AddTD("<a href=\"WFRpt.aspx?WorkID=" + en.GetValIntByKey("OID") + "&FID=" + en.GetValByKey("FID") + "&FK_Flow=" + nd.FK_Flow + "\" target=bk >报告</a>-<a href=\"/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + en.GetValIntByKey("OID") + "&FID=" + en.GetValByKey("FID") + "&FK_Flow=" + nd.FK_Flow + "\" target=bk >轨迹</a>");
                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTRSum();
            this.Pub1.AddTD("");
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                switch (attr.MyDataType)
                {
                    case DataType.AppFloat:
                    case DataType.AppInt:
                    case DataType.AppDouble:
                        this.Pub1.AddTDB(wks.GetSumDecimalByKey(attr.Key).ToString());
                        break;
                    case DataType.AppMoney:
                        this.Pub1.AddTDB(wks.GetSumDecimalByKey(attr.Key).ToString("0.00"));
                        break;
                    default:
                        this.Pub1.AddTD();
                        break;
                }
            }
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }
    }

}