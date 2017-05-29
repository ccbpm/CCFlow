using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF.Template;
using BP.WF;
using BP.DA;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class NodeAttrs : System.Web.UI.Page
    {
        #region 属性.
        public string FK_Flow
        {
            get
            {
                return this.Request.QueryString["FK_Flow"];
            }
        }
        #endregion 属性.

        protected void Page_Load(object sender, EventArgs e)
        {
            //this.Pub1.AddTable();
            //this.Pub1.AddCaption("");
            //this.Pub1.AddTR();
            //this.Pub1.AddTDTitle("序");
            //this.Pub1.AddTDTitle("节点ID");
            //this.Pub1.AddTDTitle("节点名称");
            //this.Pub1.AddTDTitle("表单方案");
            //this.Pub1.AddTDTitle("设置处理人");
            //this.Pub1.AddTDTitle("设置抄送人");
            //this.Pub1.AddTDTitle("消息&事件");
            //this.Pub1.AddTDTitle("流程完成条件");
            //this.Pub1.AddTDTitle("消息收听");
            //this.Pub1.AddTREnd();
            //this.Pub1.AddTableEnd();
        }
    }
}