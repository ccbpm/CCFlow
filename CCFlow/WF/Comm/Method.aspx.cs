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
using BP;
using BP.Web.Controls;

namespace CCFlow.Web.Comm
{
    /// <summary>
    /// RefFuncLink 的摘要说明。
    /// </summary>
    public partial class UIRefMethod1 : BP.Web.PageBase
    {
         
        public void btn_Do_Click(object sender, EventArgs e)
        {
            string ensName = this.Request.QueryString["M"];
            Method rm = BP.En.ClassFactory.GetMethod(ensName);
            // rm.Init();
            int mynum = 0;
            foreach (Attr attr in rm.HisAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                mynum++; 
            }
            int idx = 0;
            foreach (Attr attr in rm.HisAttrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                if (attr.UIVisible == false)
                    continue;
                try
                {
                    switch (attr.UIContralType)
                    {
                        case UIContralType.TB:
                            switch (attr.MyDataType)
                            {
                                case BP.DA.DataType.AppString:
                                case BP.DA.DataType.AppDate:
                                case BP.DA.DataType.AppDateTime:
                                    string str1 = this.UCEn1.GetTBByID("TB_" + attr.Key).Text;
                                    rm.SetValByKey(attr.Key, str1);
                                    break;
                                case BP.DA.DataType.AppInt:
                                    int myInt = int.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.Row[idx] = myInt;
                                    rm.SetValByKey(attr.Key, myInt);
                                    break;
                                case BP.DA.DataType.AppFloat:
                                    float myFloat = float.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.SetValByKey(attr.Key, myFloat);
                                    break;
                                case BP.DA.DataType.AppDouble:
                                case BP.DA.DataType.AppMoney:
                                    decimal myDoub = decimal.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.SetValByKey(attr.Key, myDoub);
                                    break;
                                case BP.DA.DataType.AppBoolean:
                                    int myBool = int.Parse(this.UCEn1.GetTBByID("TB_" + attr.Key).Text);
                                    rm.SetValByKey(attr.Key, myBool);
                                    break;
                                default:
                                    throw new Exception("没有判断的数据类型．");
                            }
                            break;
                        case UIContralType.DDL:
                            try
                            {
                                string str = this.UCEn1.GetDDLByKey("DDL_" + attr.Key).SelectedItemStringVal;
                                rm.SetValByKey(attr.Key, str);
                            }
                            catch
                            {
                                rm.SetValByKey(attr.Key, "");
                            }
                            break;
                        case UIContralType.CheckBok:
                            if (this.UCEn1.GetCBByKey("CB_" + attr.Key).Checked)
                                rm.SetValByKey(attr.Key, 1);
                            else
                                rm.SetValByKey(attr.Key, 0);
                            break;
                        default:
                            break;
                    }
                    idx++;
                }
                catch (Exception ex)
                {
                    throw new Exception("attr=" + attr.Key + " attr = " + attr.Key + ex.Message);
                }
            }

            try
            {
                object obj = rm.Do();
                if (obj != null)
                {
                    switch (rm.HisMsgShowType)
                    {
                        case MsgShowType.SelfAlert:
                          BP.Sys.PubClass.Alert(obj.ToString());
                            return;
                        case MsgShowType.SelfMsgWindows:
                            BP.Sys.PubClass.Alert(obj.ToString());
                            return;
                        case MsgShowType.Blank:
                      //  BP.WF.Glo.ToMsg("流程删除成功");

                            this.ToWFMsgPage(obj.ToString());
                            return;
                        default:
                            return;
                    }
                }
                this.WinClose();
            }
            catch (Exception ex)
            {
                this.UCEn1.AddMsgOfWarning("@执行[" + ensName + "]期间出现错误：",
                    ex.StackTrace);
            }
            return;
        }
        protected void Page_Load(object sender, System.EventArgs e)
        {
            string ensName = this.Request.QueryString["M"];
            Method rm = BP.En.ClassFactory.GetMethod(ensName);
            if (rm == null)
                throw new Exception("@方法名错误或者该方法已经不存在:"+ensName);
            this.Bind(rm);
        }
        public void Bind(Method rm)
        {
            this.UCEn1.AddFieldSet("<b>功能执行:" + rm.Title + "</b>");
            this.UCEn1.AddBR();
            this.UCEn1.Add(rm.Help);
            if (rm.HisAttrs.Count > 0)
            {
                this.UCEn1.BindAttrs(rm.HisAttrs);
            }
            Button btn = new Button();
            btn.CssClass = "Btn";
            btn.Text = "功能执行";
            if (string.IsNullOrEmpty(rm.Warning) == false)
            {
                btn.Attributes["onclick"] = "if (confirm('" + rm.Warning + "')==false) {return false;}else{ this.disabled=true; }";
            }
            else
            {
                btn.OnClientClick = "this.disabled=true;";
                //  btn.Attributes["onclick"] = "this.disabled=true;return window.confirm('" + rm.Warning + "');";
            }

            this.UCEn1.AddBR();
            this.UCEn1.AddBR();
            btn.ID = "Btn_Do";
            btn.UseSubmitBehavior = false;
            btn.Click += new EventHandler(btn_Do_Click);

            this.UCEn1.Add(btn);

            this.UCEn1.Add("<input type=button class=Btn onclick='window.close();' value='关闭(Esc)' />");
            this.UCEn1.AddFieldSetEnd();
        }

        #region Web 窗体设计器生成的代码
        override protected void OnInit(EventArgs e)
        {
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
