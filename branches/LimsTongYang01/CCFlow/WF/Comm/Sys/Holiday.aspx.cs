using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Comm.Sys
{
    public partial class Holiday : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BP.Sys.GloVar var =new BP.Sys.GloVar();
            var.No="Holiday";
            var.RetrieveFromDBSources();

            System.DateTime dt = System.DateTime.Now;
            while (dt.DayOfWeek != DayOfWeek.Sunday)
                dt = dt.AddDays(-1);
         //System.DateTime dtEnd = System.DateTime.Parse(dt.Year + "-12-31");
            int idx = 0;
            while (true)
            {
                CheckBox cb = new CheckBox();
                cb.Text = dt.ToString("MM月dd日");
                cb.ID = dt.ToString("MM-dd");
                cb.Checked = var.Val.Contains(dt.ToString("MM-dd"));

                switch (dt.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        idx++;
                        this.Pub1.AddTR();
                        this.Pub1.AddTDIdx(idx);
                        this.Pub1.AddTD(dt.ToString("MM")+"月");
                        this.Pub1.AddTD(cb);
                        break;
                    case DayOfWeek.Saturday:
                        this.Pub1.AddTD(cb);
                        this.Pub1.AddTREnd();
                        break;
                    default:
                        this.Pub1.AddTD("class=TRSum",cb);
                        break;
                }

                if (dt.Year != DateTime.Now.Year)
                    break;
                dt = dt.AddDays(1);
            }
        }

        protected void Btn_Save_Click(object sender, EventArgs e)
        {
            string str = "";
            foreach (Control ctl in this.Pub1.Controls)
            {
                CheckBox cb = ctl as CheckBox;
                if (cb == null)
                    continue;

                if (cb.Checked == false)
                    continue;
                str += "," + cb.ID;
            }

            // 保存.
            BP.Sys.GloVar var = new BP.Sys.GloVar();
            var.No = "Holiday";
            var.RetrieveFromDBSources();
            var.Val = str;
            //设置一下空值，让其重新从数据库取.
            BP.Sys.GloVar.Holidays = str;
            var.Save();

        }
    }
}