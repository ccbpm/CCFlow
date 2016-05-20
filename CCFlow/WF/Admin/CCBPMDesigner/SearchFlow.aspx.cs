using BP.WF.Template;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Drawing;
using System.Collections.Generic;
namespace CCFlow.WF.Admin.CCBPMDesigner
{
    public partial class SearchFlow : System.Web.UI.Page
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        /// <summary>
        /// Convert Byte[] to Image
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static Image BytesToImage(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream(buffer);
            Image image = System.Drawing.Image.FromStream(ms);
            return image;
        }
        //byte[] 流程模板字节数组 = pubFlowClound.GetFlowXML("00000bde-7a82-4bda-81f0-3a4b34a90496");

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (this.RBT_BD.Checked==true)
            {


                string flowlist = "select a.no,a.name as flowname,b.Name from WF_Flow a,WF_FlowSort b where a.FK_FlowSort=b.No and a.Name like '%"+this.key.Text+"%'";
                System.Data.DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(flowlist);
                //BP.WF.Flows flows = new BP.WF.Flows();
                //BP.En.QueryObject qo = new BP.En.QueryObject(flows);
                //qo.AddWhere(FlowAttr.Name, " LIKE ", "%" + this.key.Text + "%");
                //qo.DoQuery();
                //System.Data.DataTable dt = flows.ToDataTableField();
                this.RepList.DataSource = dt;
                this.RepList.DataBind();

            }

            if (this.RBT_Y.Checked==true)
            {
                BP.WF.CloudWS.WSSoapClient pubFlowClound = BP.WF.Cloud.Glo.GetSoap();
                System.Data.DataTable dt = null; // pubFlowClound.GetFlowTemFromCloud(this.key.Text);
                this.RepList.DataSource = dt;
                this.RepList.DataBind();
            }
        }
    }
}