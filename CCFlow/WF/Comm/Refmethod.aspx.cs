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
using BP.Web.Controls;
using BP.DA;
using BP;

namespace CCFlow.Web.Comm
{
	/// <summary>
	/// RefFuncLink ��ժҪ˵����
	/// </summary>
	public partial class UIRefMethod : BP.Web.WebPage
	{
        protected void Page_Load(object sender, System.EventArgs e)
        {
            string ensName = this.Request.QueryString["EnsName"];
            int index = int.Parse(this.Request.QueryString["Index"]);
            Entities ens = BP.En.ClassFactory.GetEns(ensName);
            Entity en = ens.GetNewEntity;
            BP.En.RefMethod rm = en.EnMap.HisRefMethods[index];
            
            if (rm.HisAttrs == null || rm.HisAttrs.Count == 0)
            {
                string pk = this.RefEnKey;
                if (pk == null)
                    pk = this.Request.QueryString[en.PK];

                en.PKVal = pk;
                en.Retrieve();
                rm.HisEn = en;

                // �����link.
                if (rm.RefMethodType == RefMethodType.LinkModel)
                {
                    string url  = rm.Do(null) as string;
                    if (string.IsNullOrEmpty(url))
                    {
                        throw new Exception("@Ӧ�÷��ص�url.");
                    }
                    this.Response.Redirect(url, true);
                    return;
                }

                object obj = rm.Do(null);
                if (obj == null)
                {
                    this.WinClose();
                    return;
                }

                string info = obj.ToString();
                info = info.Replace("@", "<br>@");
                if (info.Contains("<"))
                    this.ToWFMsgPage(info);
                else
                    this.WinCloseWithMsg(info);
                return;
            }
            this.Bind(rm);
            this.Label1.Text = this.GenerCaption(en.EnMap.EnDesc + "=>" + rm.GetIcon(this.Request.ApplicationPath) + rm.Title);
        }
        public void Bind(RefMethod rm)
        {

            this.UCEn1.BindAttrs(rm.HisAttrs);
            //����Ƿ���ѡ����Ŀ��
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

        protected void BPToolBar1_ButtonClick(object sender, EventArgs e)
        {
            string ensName = this.Request.QueryString["EnsName"];
            int index = int.Parse(this.Request.QueryString["Index"]);
            Entities ens = BP.En.ClassFactory.GetEns(ensName);
            Entity en = ens.GetNewEntity;
            en.PKVal = this.Request.QueryString[en.PK];
            en.Retrieve();

            BP.En.RefMethod rm = en.EnMap.HisRefMethods[index];
            rm.HisEn = en;
            int mynum = 0;
            foreach (Attr attr in rm.HisAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                mynum++;
            }
            
            object[] objs = new object[mynum];

            int idx = 0;
            foreach (Attr attr in rm.HisAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;

                switch (attr.UIContralType)
                {
                    case UIContralType.TB:
                        switch (attr.MyDataType)
                        {
                            case BP.DA.DataType.AppString:
                            case BP.DA.DataType.AppDate:
                            case BP.DA.DataType.AppDateTime:
                                string str1 = this.UCEn1.GetTBByID("TB_" + attr.Key).Text;
                                objs[idx] = str1;
                                //attr.DefaultVal=str1;
                                break;
                            case BP.DA.DataType.AppInt:
                                int myInt = int.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                objs[idx] = myInt;
                                //attr.DefaultVal=myInt;
                                break;
                            case BP.DA.DataType.AppFloat:
                                float myFloat = float.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                objs[idx] = myFloat;
                                //attr.DefaultVal=myFloat;
                                break;
                            case BP.DA.DataType.AppDouble:
                            case BP.DA.DataType.AppMoney:
                                decimal myDoub = decimal.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                objs[idx] = myDoub;
                                //attr.DefaultVal=myDoub;
                                break;
                            case BP.DA.DataType.AppBoolean:
                                int myBool = int.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                if (myBool == 0)
                                {
                                    objs[idx] = false;
                                    attr.DefaultVal = false;
                                }
                                else
                                {
                                    objs[idx] = true;
                                    attr.DefaultVal = true;
                                }
                                break;
                            default:
                                throw new Exception("û���жϵ��������ͣ�");

                        }
                        break;
                    case UIContralType.DDL:
                        try
                        {
                            string str = this.UCEn1.GetDDLByKey("DDL_" + attr.Key).SelectedItemStringVal;
                            objs[idx] = str;
                            attr.DefaultVal = str;
                        }
                        catch
                        {
                            // this.ToErrorPage("��ȡ��[" + attr.Desc + "] �ڼ���ִ��󣬿����Ǹ���������û��ѡ����Ŀ����������Ϣ��" + ex.Message);
                            objs[idx] = null;
                            // attr.DefaultVal = "";
                        }
                        break;
                    case UIContralType.CheckBok:
                        if (this.UCEn1.GetCBByKey("CB_" + attr.Key).Checked)
                            objs[idx] = "1";
                        else
                            objs[idx] = "0";
                        attr.DefaultVal = objs[idx].ToString();
                        break;
                    default:
                        break;
                }
                idx++;
            }

            try
            {
                object obj = rm.Do(objs);
                if (obj != null)
                {
                    this.ToWFMsgPage(obj.ToString());
                }
                this.WinClose();
            }
            catch (Exception ex)
            {
                string msg = "";
                foreach (object obj in objs)
                    msg += "@" + obj.ToString();
                string err="@ִ��[" + ensName + "]�ڼ���ִ���" + ex.Message + " InnerException= " + ex.InnerException + "[����Ϊ��]" + msg;
                this.ToWFMsgPage("<font color=red>" + err + "</font>");
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            this.WinClose();
        }
	}
}
