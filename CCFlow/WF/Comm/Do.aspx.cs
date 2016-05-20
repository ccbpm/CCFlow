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
	/// Do ��ժҪ˵����
	/// </summary>
	public partial class Do : BP.Web.WebPage 
	{
		public ActionType GetActionType
		{
			get
			{
				return (ActionType)int.Parse( this.Request.QueryString["ActionType"] ); 
			}
		}
		protected void Page_Load(object sender, System.EventArgs e)
		{
            switch (this.DoType)
            {
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
                    BP.Sys.PubClass.DownloadFile(pPath,
                        enF.GetValStringByKey("MyFileName"));
                    this.WinClose();
                    return;
                default:
                    break;
            }
			switch(this.GetActionType)
			{
				case ActionType.DeleteFile:
					SysFileManager sysfile = new SysFileManager( int.Parse(this.Request.QueryString["OID"]) );
					sysfile.Delete();
					break;
				case ActionType.PrintEnBill:
					string className=this.Request.QueryString["MainEnsName"];
					Entity en =ClassFactory.GetEns(className).GetNewEntity;
					try
					{
						en.PKVal=this.Request.QueryString["PK"];
						en.Retrieve();
					}
					catch
					{
						en.PKVal=this.Request.QueryString[en.PK];
						en.Retrieve();
					}
					//this.GenerRptByPeng(en);
					break;
				default:
					throw new Exception("do error"+this.GetActionType);
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
