using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CCForm
{
    public partial class FrmAttachmentM : ChildWindow
    {
        public FrmAttachmentM()
        {
            InitializeComponent();
        }
        public BPAttachmentM HisBPAttachment = null;
        public void BindIt(BPAttachmentM ment)
        {
            this.HisBPAttachment = ment;
            this.TB_No.Text = ment.Name;
            this.TB_Name.Text = ment.Label;
            this.TB_SaveTo.Text = ment.SaveTo;
            this.cmbDeleteWay.SelectedIndex = (int)ment.DeleteWay;
            this.CB_IsDownload.IsChecked = ment.IsDownload;
            this.CB_IsUpload.IsChecked = ment.IsUpload;
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.TB_Name.Text)
               || string.IsNullOrEmpty(this.TB_No.Text))
            {
                MessageBox.Show("您需要输入字段中英文名称", "Note", MessageBoxButton.OK);
                return;
            }

            if (this.TB_No.Text.Length >= 50)
            {
                MessageBox.Show("附件的英文名称太长,不能多于50个字符。", "Note", MessageBoxButton.OK);
                return;
            }


            #region 属性.
            string mypk = this.TB_No.Text.Trim();
            string vals = "@EnName=BP.Sys.FrmAttachment@PKVal=" + Glo.FK_MapData + "_" + mypk + "@UploadType=1";
            vals += "@FK_MapData=" + Glo.FK_MapData;
            vals += "@NoOfObj=" + this.TB_No.Text.Trim();
            vals += "@Name=" + this.TB_Name.Text;
            vals += "@SaveTo=" + this.TB_SaveTo.Text.Trim();
            vals += "@Sort=" + this.TB_Sort.Text.Trim();
            vals += "@H=" + this.HisBPAttachment.Height;
            vals += "@W=" + this.HisBPAttachment.Width;
            vals += "@X=" + this.HisBPAttachment.X;
            vals += "@Y=" + this.HisBPAttachment.Y;
            vals += "@DeleteWay=" + this.cmbDeleteWay.SelectedIndex;

            if (this.CB_IsDownload.IsChecked == true)
                vals += "@IsDownload=1";
            else
                vals += "@IsDownload=0";

            if (this.CB_IsUpload.IsChecked == true)
                vals += "@IsUpload=1";
            else
                vals += "@IsUpload=0";
            #endregion 属性.

            FF.CCFormSoapClient daSaveFile = Glo.GetCCFormSoapClientServiceInstance();
            daSaveFile.SaveEnAsync(vals);
            daSaveFile.SaveEnCompleted += (object senders, FF.SaveEnCompletedEventArgs ee)=>
            {
                if (ee.Result.Contains("Err"))
                {
                    MessageBox.Show(ee.Result, "Error", MessageBoxButton.OK);
                    return;
                }

                if (this.HisBPAttachment == null)
                    this.HisBPAttachment = new BPAttachmentM();

                this.HisBPAttachment.Label = this.TB_Name.Text;
                //  this.HisBPAttachment.Exts = this.TB_Exts.Text;
                this.HisBPAttachment.DeleteWay = (AthDeleteWay)this.cmbDeleteWay.SelectedIndex;
                this.HisBPAttachment.IsDownload = (bool)this.CB_IsDownload.IsChecked;
                this.HisBPAttachment.IsUpload = (bool)this.CB_IsUpload.IsChecked;
                this.HisBPAttachment.SaveTo = this.TB_SaveTo.Text;
                this.DialogResult = true;
            };
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void TB_Name_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.TB_No.Text.Trim()))
                Glo.GetKeyOfEn(this.TB_Name.Text, true, this.TB_No);
           
        }

    }
}

