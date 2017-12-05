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
	/// RefFuncLink 的摘要说明。
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

                // 如果是link.
                if (rm.RefMethodType == RefMethodType.LinkModel)
                {
                    string url  = rm.Do(null) as string;
                    if (string.IsNullOrEmpty(url))
                    {
                        throw new Exception("@应该返回的url.");
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
                this.Page.Controls.Clear();
                this.Response.Write(info);

                //if (info.Contains("<"))
                //    this.ToWFMsgPage(info);
                //else
                //    this.WinCloseWithMsg(info);
                return;
            }

            this.UCEn1.BindAttrs(rm.HisAttrs);
            this.Label1.Text = this.GenerCaption(en.EnMap.EnDesc + "=>" + rm.GetIcon(this.Request.ApplicationPath) + rm.Title);
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

        protected void BPToolBar1_ButtonClick(object sender, EventArgs e)
        {
            string ensName = this.Request.QueryString["EnsName"];
            int index = int.Parse(this.Request.QueryString["Index"]);
            Entities ens = BP.En.ClassFactory.GetEns(ensName);
            Entity en = ens.GetNewEntity;
            string msg = "";
            string pk = this.Request.QueryString["PK"];
            if (pk == null || pk == "")
                pk = this.Request.QueryString["No"];
            if (pk == null || pk == "")
                pk = this.Request.QueryString["OID"];
            if (pk == null || pk == "")
                pk = this.Request.QueryString["MyPK"];


            if (pk.Contains(",") == false)
            {
                /*批处理的方式.*/
                en.PKVal = pk;
                en.Retrieve();
                msg = DoOneEntity(en, index);
                if (msg == null)
                    this.WinClose();
                else
                    this.ToWFMsgPage(msg);
                return;
            }

            //如果是批处理.
            string[] pks = pk.Split(',');
            foreach (string mypk in pks)
            {
                if (string.IsNullOrEmpty(mypk) == true)
                    continue;
                en.PKVal = mypk;
                en.Retrieve();
                string s = DoOneEntity(en, index);
                if (s != null)
                    msg += "@" + s;
            }
            if (msg == "")
                this.WinClose();
            else
                this.ToWFMsgPage(msg);
        }
        public string DoOneEntity(Entity en, int rmIdx)
        {

            BP.En.RefMethod rm = en.EnMap.HisRefMethods[rmIdx];
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
                                throw new Exception("没有判断的数据类型．");

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
                            // this.ToErrorPage("获取：[" + attr.Desc + "] 期间出现错误，可能是该下拉框中没有选择项目，错误技术信息：" + ex.Message);
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
                  return obj.ToString();

                return null;
            }
            catch (Exception ex)
            {
                string msg = "";
                foreach (object obj in objs)
                    msg += "@" + obj.ToString();
                string err = "@执行[" + this.EnsName + "]期间出现错误：" + ex.Message + " InnerException= " + ex.InnerException + "[参数为：]" + msg;
                return "<font color=red>" + err + "</font>";
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            this.WinClose();
        }
	}
}
