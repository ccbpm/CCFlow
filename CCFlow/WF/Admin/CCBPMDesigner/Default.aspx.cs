using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.WF.Admin.CCBPMDesigner
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //让admin登录
            if (string.IsNullOrEmpty(BP.Web.WebUser.No) || BP.Web.WebUser.No != "admin")
            {
                Response.Redirect("Login.aspx?DoType=Logout");
                return;
            }

            // 执行升级
            string str = BP.WF.Glo.UpdataCCFlowVer(); 

            if (str != null)
            {
                if (str == "0")
                    BP.Sys.PubClass.Alert("系统升级错误，请查看日志文件\\DataUser\\log\\*.*");
                else
                    BP.Sys.PubClass.Alert("系统成功升级到:" + str + " ，系统升级不会破坏现有的数据。");
            }
        }
    }
}