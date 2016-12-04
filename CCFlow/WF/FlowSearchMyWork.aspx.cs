using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.Data;
using BP.Port;
using BP.Sys;
using BP.Web.Controls;
using BP.DA;
using BP.En;
using BP.Web;

namespace CCFlow.WF
{
    public partial class WF_FlowSearchMyWork : BP.Web.WebPage
    {

        #region attr
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        public string DT_F
        {
            get
            {
                string f = this.Session["DF"] as string;
                if (f == null)
                {
                    this.Session["DF"] = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                    return this.Session["DF"].ToString();
                }
                return f;
            }
        }
        public string DT_T
        {
            get
            {
                string f = this.Session["DT"] as string;
                if (f == null)
                {
                    this.Session["DT"] = DataType.CurrentData;
                    return this.Session["DT"].ToString();
                }
                return f;
            }
        }
        #endregion

        public int PageSize = 600;
        protected void Page_Load(object sender, EventArgs e)
        {
            Flow fl = new Flow(this.FK_Flow);
            this.Page.Title = fl.Name;
            GEEntitys rpts = (GEEntitys)BP.En.ClassFactory.GetEns(this.EnsName);
            GEEntity rpt = (GEEntity)rpts.GetNewEntity;
            QueryObject qo = new QueryObject(rpts);
            try
            {
                qo.AddWhere(GERptAttr.FlowEmps, " LIKE ", "'%@" + WebUser.No + "%'");
                qo.addAnd();
                qo.AddWhere("" + BP.Sys.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,10) >='" + this.DT_F + "' AND " + BP.Sys.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,10) <='" + this.DT_T + "' ");

                this.Pub2.BindPageIdx(qo.GetCount(), this.PageSize, this.PageIdx, "FlowSearchMyWork.aspx?EnsName=" + this.EnsName + "&FK_Flow=" + this.FK_Flow);
                qo.DoQuery("OID", this.PageSize, this.PageIdx);
            }
            catch (Exception ex)
            {
                if (this.Request.QueryString["error"] == null)
                {
                    fl.CheckRpt();
                    this.Response.Redirect(this.Request.RawUrl + "&error=1", true);
                    return;
                }
                throw ex;
            }
            //  this.Response.Write(qo.SQL);
            // 生成页面数据。
            Attrs attrs = BP.WF.Glo.AttrsOfRpt; // rpt.EnMap.Attrs;
            int colspan = 2;
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;
                colspan++;
            }

            this.Pub1.AddTable("width='100%' align=left ");

            this.Pub1.AddCaptionLeft("流程查询:" + fl.Name);

            this.Pub1.AddTRSum();
            this.Pub1.Add("<TD colspan=" + colspan + " class=ToolBar><b>发生日期&nbsp;从:</b>");

            TextBox tb = new TextBox();
            tb.ID = "TB_F";
            tb.Columns = 10;
            tb.Text = this.DT_F;
            tb.Attributes["onfocus"] = "WdatePicker();";
            this.Pub1.Add(tb);

            this.Pub1.AddB("&nbsp;到:");
            tb = new TextBox();
            tb.ID = "TB_T";
            tb.Text = this.DT_T;
            tb.Columns = 10;
            this.Pub1.Add(tb);

            this.Pub1.AddB("&nbsp;");

            Button btn = new Button();
            btn.Text = "查询";
            btn.CssClass = "Btn";
            btn.ID = "Btn_Search";
            btn.Click += new EventHandler(btn_Click);
            tb.Attributes["onfocus"] = "WdatePicker();";
            this.Pub1.Add(btn);

            btn = new Button();
            btn.Text = "导出Excel";
            btn.CssClass = "Btn";
            btn.ID = "Btn_Excel";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);
            this.Pub1.Add("&nbsp;</TD>");
            this.Pub1.AddTREnd();

            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                this.Pub1.AddTDTitle("nowarp=true", attr.Desc);
            }
            this.Pub1.AddTDTitle("nowarp=true", "操作");
            this.Pub1.AddTREnd();

            int idx = 0;
            bool is1 = false;
            foreach (GEEntity en in rpts)
            {
                idx++;
                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTD(idx);
                foreach (Attr attr in attrs)
                {
                    if (attr.UIVisible == false)
                        continue;

                    if (attr.Key == "Title")
                    {
                        this.Pub1.AddTD("<a href=\"./../WF/WFRpt.aspx?WorkID=" + en.GetValIntByKey("OID") + "&FID=" + en.GetValByKey("FID") + "&FK_Flow=" + this.FK_Flow + "\" target=bk >" + en.GetValStrByKey(attr.Key) + "</a>");
                        continue;
                    }

                    if (attr.Key == "WFState")
                    {
                        switch (en.GetValIntByKey(attr.Key))
                        {
                            case 0:
                                this.Pub1.AddTD("进行中");
                                break;
                            case 1:
                                this.Pub1.AddTD("完成");
                                break;
                        }
                        continue;
                    }
                    switch (attr.MyDataType)
                    {
                        case DataType.AppBoolean:
                            this.Pub1.AddTD(en.GetValBoolStrByKey(attr.Key));
                            break;
                        case DataType.AppFloat:
                        case DataType.AppDouble:
                            this.Pub1.AddTD(en.GetValFloatByKey(attr.Key));
                            break;
                        case DataType.AppInt:
                            if (attr.UIContralType == UIContralType.DDL)
                            {
                                this.Pub1.AddTD(en.GetValRefTextByKey(attr.Key));
                            }
                            else
                            {
                                this.Pub1.AddTD(en.GetValIntByKey(attr.Key));
                            }
                            break;
                        case DataType.AppMoney:
                            this.Pub1.AddTDMoney(en.GetValDecimalByKey(attr.Key));
                            break;
                        default:
                            this.Pub1.AddTD(en.GetValStringByKey(attr.Key));
                            break;
                    }
                }
                this.Pub1.AddTD("<a href=\"./../WF/WFRpt.aspx?WorkID=" + en.GetValIntByKey("OID") + "&FID=" + en.GetValByKey("FID") + "&FK_Flow=" + this.FK_Flow + "\" target=bk >报告</a>-<a href=\"./../WF/WorkOpt/OneWork/OneWork.htm?CurrTab=Truck&WorkID=" + en.GetValIntByKey("OID") + "&FID=" + en.GetValByKey("FID") + "&FK_Flow=" + this.FK_Flow + "\" target=bk >轨迹</a>");
                this.Pub1.AddTREnd();
            }

            this.Pub1.AddTRSum();
            this.Pub1.AddTD("");
            foreach (Attr attr in attrs)
            {
                if (attr.UIVisible == false)
                    continue;

                this.Pub1.AddTD();
                continue;
            }
            this.Pub1.AddTD();
            this.Pub1.AddTREnd();
            this.Pub1.AddTableEnd();
        }

        void btn_Click(object sender, EventArgs e)
        {
            GEEntitys rpts = (GEEntitys)BP.En.ClassFactory.GetEns(this.EnsName);
            GEEntity rpt = (GEEntity)rpts.GetNewEntity;
            Flow fl = new Flow(this.FK_Flow);

            Button btn = (Button)sender;
            if (btn.ID == "Btn_Excel")
            {
                QueryObject qo = new QueryObject(rpts);
                qo.AddWhere(WorkAttr.Rec, WebUser.No);
                qo.addAnd();
                if (BP.Sys.SystemConfig.AppCenterDBType == DBType.Access)
                    qo.AddWhere("Mid(RDT,1,10) >='" + this.DT_F + "' AND Mid(RDT,1,10) <='" + this.DT_T + "' ");
                else
                    qo.AddWhere("" + BP.Sys.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,10) >='" + this.DT_F + "' AND " + BP.Sys.SystemConfig.AppCenterDBSubstringStr + "(RDT,1,10) <='" + this.DT_T + "' ");


                this.Pub2.BindPageIdx(qo.GetCount(), this.PageSize, this.PageIdx, "?FK_Flow=" + this.FK_Flow + "&EnsName=" + this.EnsName);

                qo.DoQuery();

                try
                {
                    //this.ExportDGToExcel(ens.ToDataTableDescField(), this.HisEn.EnDesc);
                    this.ExportDGToExcel(rpts.ToDataTableDesc(), fl.Name);
                }
                catch (Exception ex)
                {
                    try
                    {
                        this.ExportDGToExcel(rpts.ToDataTableDescField(), fl.Name);
                    }
                    catch
                    {
                        this.ToErrorPage("数据没有正确导出可能的原因之一是:系统管理员没正确的安装Excel组件，请通知他，参考安装说明书解决。@系统异常信息：" + ex.Message);
                    }
                }
                return;
            }

            this.Session["DF"] = this.Pub1.GetTextBoxByID("TB_F").Text;
            this.Session["DT"] = this.Pub1.GetTextBoxByID("TB_T").Text;
            this.Response.Redirect("FlowSearchMyWork.aspx?FK_Flow=" + this.FK_Flow + "&EnsName=" + this.EnsName, true);
        }
    }
}