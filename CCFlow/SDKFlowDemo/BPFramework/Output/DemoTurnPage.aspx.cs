using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.En;
using BP.Web;
using BP.Port;

namespace CCFlow.SDKFlowDemo
{
    public partial class DemoTurnPage : System.Web.UI.Page
    {
        #region 属性.
        public int PageIdx
        {
            get
            {
                try
                {
                    return int.Parse(this.Request.QueryString["PageIdx"]);
                }
                catch 
                {
                    return 1;
                }
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            //变量定义: 第几页.
            int currPageIdx = this.PageIdx;
            int pageSize = 12; //页面记录条数.

            //实体查询.
            BP.WF.Nodes ens = new BP.WF.Nodes();
            BP.En.QueryObject qo = new QueryObject(ens);
            qo.AddWhere(BP.WF.Template.NodeAttr.NodePosType, "1"); // 设置查询条件.

            //把代码放到表格尾部, 形成 第1,2,3,4,5 页 ....... 
            this.Pub2.Clear();
            this.Pub2.BindPageIdx(qo.GetCount(),
                pageSize, currPageIdx, "DemoTurnPage.aspx?1=2&3=xx");

            //每页有15条数据，取第2页的数据.
            qo.DoQuery("NodeID", pageSize, currPageIdx);

            this.Pub1.AddTable();
            this.Pub1.AddTR();
            this.Pub1.AddTDTitle("序");
            this.Pub1.AddTDTitle("节点编号");
            this.Pub1.AddTDTitle("节点名称");
            this.Pub1.AddTDTitle("操作");
            this.Pub1.AddTREnd();

            int idx = 0;
            foreach (BP.WF.Node en in ens)
            {
                idx++;
                this.Pub1.AddTR();
                this.Pub1.AddTDIdx(idx);
                this.Pub1.AddTD(en.NodeID);
                this.Pub1.AddTD(en.Name);
                this.Pub1.AddTD("<a href='http://ccflow.org/case.aspx?ID="+en.NodeID+"' >打开</a>");
                this.Pub1.AddTREnd();
            }
            this.Pub1.AddTableEnd();


        }
    }
}