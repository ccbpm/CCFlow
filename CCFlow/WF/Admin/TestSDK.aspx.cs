using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Port;
using BP.Web;

namespace CCFlow.WF.Admin
{
    public partial class WF_Admin_LoginTest : BP.Web.WebPage
    {
        public string Lang
        {
            get
            {
                return this.Request.QueryString["Lang"];
            }
        }
        public string UserNo
        {
            get
            {
                return this.Request.QueryString["RefNo"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            BP.WF.XML.SDKs sdks = new BP.WF.XML.SDKs();
            sdks.RetrieveAll();

            Emp emp1 = new Emp(this.RefNo);
            WebUser.SignInOfGenerLang(emp1, this.Lang);

            this.Pub1.AddTable();
            this.Pub1.AddCaptionLeft(" 当前操作员:" + emp1.Name);
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("No");
            this.Pub1.AddTDTitle("Name");
            this.Pub1.AddTDTitle("URL");
            this.Pub1.AddTREnd();
            bool is1 = false;
            foreach (BP.WF.XML.SDK sdk in sdks)
            {
                string url = sdk.Url.Clone() as string;
                url = url.Replace(":", "&");
                url = url.Replace("@UserNo", UserNo);
                url = url.Replace("@SID", null);
                url = url.Replace("//", "/");

                is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTD(sdk.No);
                this.Pub1.AddTD("<a href='" + url + "' target=_blank >" + sdk.Name + "</a>");
                this.Pub1.AddTD(sdk.Url);
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();
            return;

            //Depts depts = new Depts();
            //depts.RetrieveAll();

            //Emps emps = new Emps();
            //emps.RetrieveAll();

            //this.Pub1.AddTable();
            //this.Pub1.AddTR();
            //this.Pub1.AddTDTitle("人员");
            //this.Pub1.AddTDTitle("独立登陆");
            //this.Pub1.AddTDTitle("独立登陆");
            //this.Pub1.AddTREnd();

            //foreach (Dept dept in depts)
            //{
            //    this.Pub1.AddTRSum();
            //    this.Pub1.AddTD(dept.Name);
            //    this.Pub1.AddTD("人员");
            //    this.Pub1.AddTD("独立登陆");
            //    this.Pub1.AddTD("独立登陆");
            //    this.Pub1.AddTREnd();

            //    bool is1 = false;
            //    foreach (Emp en in emps)
            //    {
            //        this.Pub1.AddTR();
            //        this.Pub1.AddTD(en.Name);
            //        this.Pub1.AddTD("人员");
            //        this.Pub1.AddTD("独立登陆");
            //        this.Pub1.AddTD("独立登陆");
            //        this.Pub1.AddTREnd();
            //    }
            //}
            //this.Pub1.AddTableEnd();
        }
    }
}