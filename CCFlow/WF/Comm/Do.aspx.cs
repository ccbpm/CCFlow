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
using BP;
namespace CCFlow.Web.Comm
{
	/// <summary>
	/// Do 的摘要说明。
	/// </summary>
	public partial class Do : System.Web.UI.Page
    {
        #region 属性.
        public string GetVal(string key)
        {
            string val = this.Request.QueryString[key];
            return BP.Tools.DealString.DealStr(val);
        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        protected void WinClose()
        {
            this.Response.Write("<script language='JavaScript'> window.close();</script>");
        }
        public string DoType
        {
            get
            {
                return this.GetVal("DoType");
            }
        }
        public string DoWhat
        {
            get
            {
                return this.GetVal("DoWhat");
            }
        }
        public string MyPK
        {
            get
            {
                return this.GetVal("MyPK");
            }
        }
        public string EnName
        {
            get
            {
                return this.GetVal("EnName");
            }
        }
        public string EnsName
        {
            get
            {
                return this.GetVal("EnsName");
            }
        }
        
        #endregion 属性.

        protected void Page_Load(object sender, System.EventArgs e)
		{
			switch (this.DoType)
			{
                case "SearchExp":
                //    this.SearchExp();
                    break;
				case "DownFile":
					Entity enF = BP.En.ClassFactory.GetEn(this.EnName);
					enF.PKVal = this.GetVal("PK");
					enF.Retrieve();
					string pPath = enF.GetValStringByKey("MyFilePath") + "\\" + enF.PKVal + "." + enF.GetValStringByKey("MyFileExt");

					//判断文件是否存在
					if (System.IO.File.Exists(pPath)==false)
					{
                        pPath = enF.EnMap.FJSavePath + "\\" + enF.PKVal + "." + enF.GetValStringByKey("MyFileExt");
                        if (System.IO.File.Exists(pPath) == false)
                        {
                            Response.Write("<script>alert('文件不存在！');</script>");
                            this.WinClose();
                            return;
                        }
					}

                    BP.WF.HttpHandler.HttpHandlerGlo.DownloadFile(pPath,
						enF.GetValStringByKey("MyFileName"));
					this.WinClose();
					return;
				default:
					break;
			}
			this.WinClose();
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
