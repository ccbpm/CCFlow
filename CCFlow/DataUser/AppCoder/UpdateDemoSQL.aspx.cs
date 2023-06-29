using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CCFlow.DataUser.AppCoder
{
    public partial class UpdateDemoSQL : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //本文件用于自动更新演示数据库代码.

            string path = "";
            //1. 删除所有的表. 视图.
            string sql = "";

            //2. 读取文件sql脚本导入到数据库.
        }
    }
}