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
	/// Do ��ժҪ˵����
	/// </summary>
	public partial class Do : System.Web.UI.Page
    {
        #region ����.
        public string GetVal(string key)
        {
            string val = this.Request.QueryString[key];
            return BP.Tools.DealString.DealStr(val);
        }
        /// <summary>
        /// �رմ���
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
        
        #endregion ����.

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

					//�ж��ļ��Ƿ����
					if (System.IO.File.Exists(pPath)==false)
					{
                        pPath = enF.EnMap.FJSavePath + "\\" + enF.PKVal + "." + enF.GetValStringByKey("MyFileExt");
                        if (System.IO.File.Exists(pPath) == false)
                        {
                            Response.Write("<script>alert('�ļ������ڣ�');</script>");
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
     
		#region Web ������������ɵĴ���
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: �õ����� ASP.NET Web ���������������ġ�
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭���޸�
		/// �˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
