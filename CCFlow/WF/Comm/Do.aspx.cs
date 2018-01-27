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
using BP.Sys;
using BP.En;
using BP.Web;
using BP.DA;
using BP.Web.Comm;
using BP;
namespace CCFlow.Web.Comm
{
	/// <summary>
	/// Do 的摘要说明。
	/// </summary>
	public partial class Do : BP.Web.WebPage
	{
		public ActionType GetActionType
		{
			get
			{
				return (ActionType)int.Parse(this.Request.QueryString["ActionType"]);
			}
		}
		protected void Page_Load(object sender, System.EventArgs e)
		{
			switch (this.DoType)
			{
                case "SearchExp":
                    this.SearchExp();
                    break;
				case "DelGradeEns":
					Entity enGrade = BP.En.ClassFactory.GetEns(this.EnsName).GetNewEntity;
					enGrade.PKVal = this.RefPK;
					enGrade.Delete();
					this.WinClose();
					return;
				case "DownFile":
					Entity enF = BP.En.ClassFactory.GetEn(this.EnName);
					enF.PKVal = this.Request.QueryString["PK"];
					enF.Retrieve();
					string pPath = enF.GetValStringByKey("MyFilePath") + "\\" + enF.PKVal + "." + enF.GetValStringByKey("MyFileExt");

					//判断文件是否存在
					if (!System.IO.File.Exists(pPath))
					{
						Response.Write("<script>alert('文件不存在！');</script>");
						this.WinClose();
						return;
					}

					BP.Sys.PubClass.DownloadFile(pPath,
						enF.GetValStringByKey("MyFileName"));
					this.WinClose();
					return;
				default:
					break;
			}
			switch (this.GetActionType)
			{
				case ActionType.DeleteFile:
					SysFileManager sysfile = new SysFileManager(int.Parse(this.Request.QueryString["OID"]));
					sysfile.Delete();
					break;
				case ActionType.PrintEnBill:
					string className = this.Request.QueryString["MainEnsName"];
					Entity en = ClassFactory.GetEns(className).GetNewEntity;
					try
					{
						en.PKVal = this.Request.QueryString["PK"];
						en.Retrieve();
					}
					catch
					{
						en.PKVal = this.Request.QueryString[en.PK];
						en.Retrieve();
					}
					//this.GenerRptByPeng(en);
					break;
				default:
					throw new Exception("do error" + this.GetActionType);
			}

			this.WinClose();
		}

        public void SearchExp()
        {
            BP.WF.HttpHandler.WF_Comm comm = new BP.WF.HttpHandler.WF_Comm(System.Web.HttpContext.Current);
            DataSet ds = comm.Search_Search();

            DataTable dt = ds.Tables["DT"];

            Entities ens = ClassFactory.GetEns(this.EnsName);
            Entity en = ens.GetNewEntity;
            Map map = en.EnMapInTime;
            foreach (Attr  item in map.Attrs)
            {
                foreach (DataColumn dc in dt.Columns)
                {
                    try
                    {
                        if (item.Key == dc.ColumnName)
                            dc.ColumnName = item.Desc;
                    }
                    catch
                    {

                    }
                }
            }

            string name = map.EnDesc + ".xls";

            string filename = Request.PhysicalApplicationPath + @"\Temp\" +DBAccess.GenerGUID() + ".xls";
            CCFlow.WF.Comm.Utilities.NpoiFuncs.DataTableToExcel(dt, filename, name,
                                                              BP.Web.WebUser.Name, true, true, true);
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
