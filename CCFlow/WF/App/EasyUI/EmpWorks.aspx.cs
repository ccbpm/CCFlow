using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.AppDemoLigerUI
{
    public partial class EmpWorks : BasePage
    {
        /// <summary>
        /// 是否修改消息状态
        /// </summary>
        private bool IsChangeSmsState
        {
            get
            {
                string smsSta = this.Request.QueryString["SMSta"];
                if (!string.IsNullOrEmpty(smsSta) && smsSta == "1")
                    return true;
                return false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (IsChangeSmsState)
                {
                    //将消息状态修改为已读
                    BP.DA.DBAccess.RunSQL(String.Format("UPDATE Sys_SMS SET EmailSta=1 where SendTo='{0}'", BP.Web.WebUser.No));
                }
            }
        }
    }
}