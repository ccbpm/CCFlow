using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.SDKFlowDemo
{
    public partial class DemoJZOut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 演示矩阵输出. 也叫N宫格输出.
            BP.Port.Emps ens = new BP.Port.Emps();
            ens.RetrieveAll();

            int cols = 4; //定义显示列数 从0开始。
            decimal widthCell = 100 / cols;
            this.Pub1.AddTable("width=100% border=0");
            this.Pub1.AddCaption("矩阵输出，也叫n宫格输出,输出的列数可以变化.");
            int idx = -1;
            bool is1 = false;
            foreach (BP.Port.Emp en in ens)
            {
                idx++;
                if (idx == 0)
                    is1 = this.Pub1.AddTR(is1);
                this.Pub1.AddTDBegin("width='" + widthCell + "%' border=0 valign=top");

                #region 输出内容.

                this.Pub1.Add("<h2>姓名:" + en.Name+"</h2>");

                this.Pub1.Add("部门:" + en.FK_DeptText);

                #endregion 输出内容.

                this.Pub1.AddTDEnd();
                if (idx == cols - 1)
                {
                    idx = -1;
                    this.Pub1.AddTREnd();
                }
            }

            while (idx != -1)
            {
                idx++;
                if (idx == cols - 1)
                {
                    idx = -1;
                    this.Pub1.AddTD();
                    this.Pub1.AddTREnd();
                }
                else
                {
                    this.Pub1.AddTD();
                }
            }
            this.Pub1.AddTableEnd();
        }
    }
}