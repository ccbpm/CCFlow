using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.SDKComponents
{
    public partial class AccepterCCerSelecter : BP.Web.UC.UCBase
    {
        #region attr
        public string FK_Flow
        {
            get
            {
                return this.Request["FK_Flow"];
            }
        }
        public int FK_Node
        {
            get
            {
                return int.Parse(this.Request["FK_Node"].ToString());
            }
        }
        public Int64 WorkID
        {
            get
            {
                return Int64.Parse(this.Request["WorkID"].ToString());
            }
        }
        public Int64 FID
        {
            get
            {
                return Int64.Parse(this.Request["FID"].ToString());
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            this.AddTable();
            this.AddCaption("<a href=\"javascript:Select('"+this.FK_Flow+"','"+this.FK_Node+"','"+this.WorkID+"','"+this.FID+"')\">处理意见</a>");

            this.AddTR();
            this.AddTDBegin();

            TextBox tb = new TextBox();
            tb.ID = "TB_Doc";
            tb.TextMode = TextBoxMode.MultiLine;
            tb.Rows = 5;
            tb.Attributes["width"] = "100%";
            this.Add(tb);


            this.AddTDEnd();
            this.AddTREnd();

            this.AddTableEnd();

        }
    }
}