using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.En;


namespace CCFlow.WF.Comm.RefFunc
{
    public partial class Exp : System.Web.UI.Page
    {
        public string EnsName
        {
            get
            {
                return this.Request.QueryString["EnsName"];
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Entities ens = BP.En.ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;

            // edit by stone : 增加了实施的获取map, 可以让用户动态的设置查询条件.
            Map map = en.EnMapInTime;

            //设置toolbar.
            this.ToolBar1.InitByMapV2(map, 1);
            QueryObject qo = new QueryObject(ens);
            qo = this.ToolBar1.GetnQueryObject(ens, en);

            //形成数据源.
            DataTable dt= qo.DoQueryToTable();

            //找到导入导出的模版.

            //加载填充模版。

            //让用户下载.


            //关闭当前窗口.
            BP.Sys.PubClass.WinClose();
 
        }
    }
}