using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace CCFlow.Web.WF.Comm.Sys
{
	/// <summary>
	/// ManagerTools 的摘要说明。
	/// </summary>
	public partial class ManagerTools : BP.Web.WebPageAdmin
	{
		protected System.Web.UI.WebControls.Label Label1;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            //if (Web.WebUser.No.IndexOf("8888") > 1 || Web.WebUser.No.IndexOf("admin") != -1 || Web.WebUser.No.IndexOf("8888") == 0)
            //{
            //    //this.ToErrorPage("您没有权限使用此功能．");
            //}
            //else
            //{
            //    if (Web.WebUser.No == "288888")
            //    {
            //        //this.ToErrorPage("您没有权限使用此功能．");
            //    }
            //    else
            //    {
            //        this.ToErrorPage("您没有权限使用此功能．");
            //    }
            //}


            DataSet ds = new DataSet();
            ds.ReadXml(BP.Sys.SystemConfig.PathOfXML + "AdminTools.xml");

            DataTable mydt = ds.Tables[0];

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("ICON", typeof(string)));
            dt.Columns.Add(new DataColumn("名称", typeof(string)));
            dt.Columns.Add(new DataColumn("提供给", typeof(string)));
            dt.Columns.Add(new DataColumn("描述", typeof(string)));

            //dt.Columns.Add( new DataColumn("描述", typeof(string)));

            DataRow dr1 = dt.NewRow();
            dr1["ICON"] = "<Img src='./../WF/Img/Btn/Do.gif' border=0/>";
            dr1["名称"] = "<a href='EditWebconfig.aspx' >全局配置</A>";
            dr1["提供给"] = "admin";
            dr1["描述"] = "用于设置全局的站点信息";
            dt.Rows.Add(dr1);

            dr1 = dt.NewRow();
            dr1["ICON"] = "<Img src='./../Img/Btn/Do.gif' border=0/>";
            dr1["名称"] = "<a href='./../Port/ChangePass.aspx' >修改密码</A>";
            dr1["提供给"] = "admin";
            dr1["描述"] = "用于设置全局的站点信息";
            dt.Rows.Add(dr1);


            foreach (DataRow mydr in mydt.Rows)
            {
                if (mydr["Enable"].ToString().Trim() == "0")
                    continue;

                DataRow dr = dt.NewRow();
                dr["ICON"] = "<Img src='" + mydr["ICON"] + "' />";
                dr["名称"] = "<a href='" + mydr["URL"] + "' >" + mydr["Name"] + "</A>";
                dr["提供给"] = mydr["DFor"];
                dr["描述"] = mydr["DESC"];
                dt.Rows.Add(dr);
            }

            this.UCSys1.AddTable();
            this.UCSys1.AddCaptionLeft(this.GenerCaption("管理员工具"));
            this.UCSys1.AddTR();
            this.UCSys1.AddTDTitle("");
            this.UCSys1.AddTDTitle("名称");
            this.UCSys1.AddTDTitle("提供给");
            this.UCSys1.AddTDTitle("描述");
            this.UCSys1.AddTREnd();

            foreach (DataRow dr in dt.Rows)
            {
                this.UCSys1.AddTR();
                this.UCSys1.AddTD(dr["ICON"].ToString());
                this.UCSys1.AddTD(dr["名称"].ToString());
                this.UCSys1.AddTD(dr["提供给"].ToString());
                this.UCSys1.AddTD(dr["描述"].ToString());
                this.UCSys1.AddTREnd();
            }
            this.UCSys1.AddTableEnd();
            //    this.Ucsys1.Add(this.GenerCaption("功能执行"));
            // this.Ucsys1.AddHR();
            // this.Response.Write(this.GenerTablePage(dt, "管理员工具"));
        }

		#region Web 窗体设计器生成的代码
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}
