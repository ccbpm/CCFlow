using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.En;
using BP.PRJ;
using BP.Web;
public partial class ExpandingApplication_PRJ_EmpPrjStations : WebPage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        Prj prj = new Prj(this.RefNo);
        this.Pub1.AddTable("width='80%'");
        this.Pub1.AddCaptionLeft(prj.Name + " - 成员岗位设置");
        this.Pub1.AddTR();
        this.Pub1.AddTDTitle("IDX");
        this.Pub1.AddTDTitle("人员");
        this.Pub1.AddTDTitle("名称");
        this.Pub1.AddTDTitle("现有岗位");
        this.Pub1.AddTDTitle("编辑岗位");
        this.Pub1.AddTDTitle("移出");
        this.Pub1.AddTREnd();

        EmpPrjs emps = new EmpPrjs();
        emps.Retrieve(EmpPrjAttr.FK_Prj, this.RefNo);

        int idx = 1;
        foreach (EmpPrj emp in emps)
        {
            this.Pub1.AddTR();
            this.Pub1.AddTDIdx(idx++);

            this.Pub1.AddTD(emp.FK_Emp);
            this.Pub1.AddTD(emp.FK_EmpT);

            EmpPrjStations stas = new EmpPrjStations();
            stas.Retrieve(EmpPrjStationAttr.FK_EmpPrj, this.RefNo + "_" + emp.FK_Emp);
            string str = "";
            foreach (EmpPrjStation sta in stas)
            {
                str += sta.FK_StationT + ";";
            }

            if (str == "")
                str = "无";
            this.Pub1.AddTD(str);
            this.Pub1.AddTD("<a href=''>编辑</a>");
            this.Pub1.AddTD("移出");
            this.Pub1.AddTREnd();
        }
        this.Pub1.AddTableEnd();
    }
}