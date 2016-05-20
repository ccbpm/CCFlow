using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.GPM;

namespace GMP2.GPM
{
    public partial class AppMenu : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //如果没有BPM的数据，就初始化他的菜单.
            BP.GPM.App app = new BP.GPM.App();
            app.No = "CCFlowBPM";
            if (app.RetrieveFromDBSources() == 0)
            {
                BP.GPM.App.InitBPMMenu();
                app.Retrieve();
            }
        }
    }
}