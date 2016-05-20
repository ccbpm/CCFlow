using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.Sys;
using BP.En;
using BP.Web;
using BP.Web.Comm;
using BP.DA;
using System.Data;
using BP.WF.CloudWS;

namespace CCFlow.WF.Admin.AttrFlow
{
    public partial class SaveFormToPriTem : WebPage
    {
        private string userNo
        {
            get
            {
                return BP.WF.Cloud.CCLover.UserNo;
            }
        }
        private string pwd
        {
            get
            {
                return BP.WF.Cloud.CCLover.Password;
            }
        }
        private string guid
        {
            get
            {
                return BP.WF.Cloud.CCLover.GUID;
            }
        }
        private WSSoapClient ccflowCloud
        {
            get
            {
                WSSoapClient cloud = BP.WF.Cloud.Glo.GetSoap();
                return cloud;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //测试连接是否可用
            try
            {
                ccflowCloud.GetNetState();
            }
            catch (Exception)
            {
                ClientScript.RegisterClientScriptBlock(this.GetType(), "msg", "<script>netInterruptJs();</script>");
                return;
            }

            try
            {
                if (string.IsNullOrWhiteSpace(userNo) ||
               string.IsNullOrWhiteSpace(pwd) ||
               string.IsNullOrWhiteSpace(guid))
                {
                    this.Response.Redirect("RegUser.aspx");
                }
            }
            catch (Exception)
            {
                this.Response.Redirect("RegUser.aspx");
            }

            DataTable dt = ccflowCloud.PriFormDir(userNo, pwd, guid);

            BP.Web.Controls.DDL.MakeTree(dt, "ParentNo", "0", "MyPK", "Name", this.DropDownList1, -1);
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string str = ccflowCloud.SavePubFormToPri(userNo, pwd, guid,
                          this.DropDownList1.SelectedValue, this.Request.QueryString["GUID"]);

                if (str=="true")
                {
                      this.Alert("保存成功");
                      this.Button1.Enabled = false;
                }
                else
                {
                    this.Alert("保存失败");
                }
            }
            catch (Exception ex)
            {
                this.Response.Write("导入失败：" + ex.Message);
            }
        }
    }
}