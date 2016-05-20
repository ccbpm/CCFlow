using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class API : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void Demo_StartFlow()
        {
            string flowNo = this.Request.QueryString["FK_Flow"];

            //创建WorkID.
            Int64 workid=BP.WF.Dev2Interface.Node_CreateBlankWork(flowNo);

            //BP.WF.Dev2Interface.Node_StartWork(flowNo,
        }


    }
}