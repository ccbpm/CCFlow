using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Demo;
using BP.Demo.BPFramework;
using BP.Demo.CRM;
using BP.Demo.SDK;
using BP.Demo.YS;
using BP.Demo.FlowEvent;

namespace CCFlow.SDKFlowDemo.BPFramework.DataInput
{
    public partial class Cell2D : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region  组织维度数据源.
            BP.Pub.YFs ens1 = new BP.Pub.YFs();
            ens1.RetrieveAll();

            BP.Port.Emps ens2 = new BP.Port.Emps();
            ens2.RetrieveAll();
            #endregion  组织维度数据源.

            // 组织数据源.
            EmpCents ensData = new EmpCents();
            ensData.RetrieveAll();

            //开始输出.
            this.Pub1.AddTable();
            this.Pub1.AddCaption("员工月份考勤得分");

            #region  输出维度1
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle();
            foreach (BP.Pub.YF en1 in ens1)
            {
                this.Pub1.AddTDTitle(en1.Name);
            }
            this.Pub1.AddTREnd();
            #endregion  输出维度1


            #region  输出表体
            foreach (BP.Port.Emp en2 in ens2)
            {
                this.Pub1.AddTR();
                this.Pub1.AddTD(en2.Name);
                foreach (BP.Pub.YF en1 in ens1)
                {
                    TextBox tb = new TextBox();
                    tb.CssClass = "TBNum";
                    tb.ID = "TB_" + en1.No + "_" + en2.No;
                    EmpCent enData = ensData.GetEntityByKey(EmpCentAttr.FK_Emp, en2.No, EmpCentAttr.FK_NY, en1.No) as EmpCent;
                    if (enData == null)
                    {
                        tb.Text = "0";
                        this.Pub1.AddTD(tb);
                    }
                    else
                    {
                        tb.Text = enData.Cent.ToString();
                        this.Pub1.AddTD(tb);
                    }
                }
                this.Pub1.AddTREnd();
            }
            #endregion  输出表体
            this.Pub1.AddTableEndWithHR();

            Button btn = new Button();
            btn.ID = "Btn_Save";
            btn.Text = "保存";
            btn.Click += new EventHandler(btn_Click);
            this.Pub1.Add(btn);

        }

        void btn_Click(object sender, EventArgs e)
        {
            // 组织维度数据源.
            BP.Pub.YFs ens1 = new BP.Pub.YFs();
            ens1.RetrieveAll();

            BP.Port.Emps ens2 = new BP.Port.Emps();
            ens2.RetrieveAll();

            //删除保存前的数据.(一定要按照条件删除.)
            BP.DA.DBAccess.RunSQL("DELETE FROM Demo_EmpCent WHERE 1=1 ");

            //创建一个空白的实体.
            EmpCent enData = new EmpCent();
            foreach (BP.Port.Emp en2 in ens2)
            {
                foreach (BP.Pub.YF en1 in ens1)
                {
                    float val = float.Parse( this.Pub1.GetTextBoxByID("TB_" + en1.No + "_" + en2.No).Text);
                    enData.MyPK = en2.No + "_" + en1.No;
                    enData.Cent = val;
                    enData.FK_Emp = en2.No;
                    enData.FK_NY = en1.No;
                    enData.Insert();  //因为已经按照条件删除了，这里就直接执行insert.
                }
            }
            this.Response.Write("保存成功.");
        }
    }
}