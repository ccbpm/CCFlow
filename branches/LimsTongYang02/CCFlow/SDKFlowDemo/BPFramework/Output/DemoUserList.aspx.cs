using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.SDKFlowDemo.BPFramework.Output
{
    public partial class DemoUserList : BP.Web.WebPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region 执行功能.
            switch (this.DoType)
            {
                case "Del":
                    BP.Port.Emp en = new BP.Port.Emp();
                    en.No = this.RefNo;
                    //en.Delete();
                    break;
                default:
                    break;
            }
            #endregion 执行功能.

            this.Pub1.AddTable("width=100%");
            this.Pub1.AddCaption("操作员列表");
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");
            this.Pub1.AddTDTitle("编号");
            this.Pub1.AddTDTitle("名称");
            this.Pub1.AddTDTitle("部门");
            this.Pub1.AddTDTitle("操作");
            this.Pub1.AddTREnd();

            BP.Port.Emps ens = new BP.Port.Emps();
            ens.RetrieveAllFromDBSource();
            int idx = 0;
            foreach (BP.Port.Emp en in ens)
            {
                idx++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx);
                this.Pub1.AddTD(en.No);
                this.Pub1.AddTD(en.Name);
                this.Pub1.AddTD(en.FK_DeptText);
                this.Pub1.AddTD("<a href=\"javascript:Del('" + en.No + "')\" ><img src='/WF/Img/Btn/Delete.gif' border=0/>操作</a>");
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();


        }

    }
}