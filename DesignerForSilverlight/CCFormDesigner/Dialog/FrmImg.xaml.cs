using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace CCForm
{
    public partial class FrmImg : ChildWindow
    {
        public FrmImg()
        {
            InitializeComponent();
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.HisImg.LinkURL = this.TB_LinkUrl.Text;
            this.HisImg.LinkTarget = this.TB_WinName.Text;

            if (string.IsNullOrEmpty(TB_En_Seal.Text.Trim()))
            {
                MessageBox.Show("图片英文名不能为空。");
                return;
            }
            //判断英文名称不能包含汉字
            if (BP.SL.Glo.ContainsChinese(this.TB_En_Seal.Text))
            {
                MessageBox.Show("英文名称 不能使用汉字！");
                return;
            }

            BPImg img = Glo.currEle as BPImg;
            img.LinkURL = this.TB_LinkUrl.Text;
            img.LinkTarget = this.TB_WinName.Text;
            
            if (this.RB_0.IsChecked == true)
            {
                if (FileName != null)
                {
                    ImageBrush ib = new ImageBrush();
                    BitmapImage png = new BitmapImage(new Uri(Glo.BPMHost + "/DataUser/ImgAth/Upload/" + FileName, UriKind.RelativeOrAbsolute));
                    ib.ImageSource = png;
                    this.HisImg.Background = ib;
                    this.HisImg.HisPng = png;
                }
                img.SrcType = 0;
                img.ImgPath = this.TB_ImgPath.Text;
            }
            else
            {
                img.SrcType = 1;
                img.ImgURL = this.TB_ImgUrl.Text;
                //不为空，并且不包含ccflow表达式
                if (this.TB_ImgUrl.Text != "" && !this.TB_ImgUrl.Text.ToLower().Contains("@"))
                {
                    //生成预览图片
                    ImageBrush ib = new ImageBrush();
                    BitmapImage png = new BitmapImage(new Uri(this.TB_ImgUrl.Text, UriKind.RelativeOrAbsolute));
                    ib.ImageSource = png;
                    this.HisImg.Background = ib;
                    this.HisImg.HisPng = png;
                }
            }
            this.HisImg.TB_CN_Name = TB_CN_Seal.Text;
            this.HisImg.TB_En_Name = TB_En_Seal.Text;
            ComboBoxItem it = (ComboBoxItem)this.DDL_ImgAppType.SelectedItem;
            if (it != null)
            {
                img.ImgAppType = it.Tag.ToString();
            }
            Glo.currEle = img;
            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        public BPImg HisImg = null;
        public string FileName { get; set; }
        public void BindIt(BPImg img)
        {
            HisImg = img;
            this.TB_LinkUrl.Text = img.LinkURL;
            Glo.BindComboBoxWinOpenTarget(this.DDL_WinName, img.LinkTarget);

            this.DDL_ImgAppType.Items.Clear();
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = "图片";
            cbi.Tag = "0";
            this.DDL_ImgAppType.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "图片公章";
            cbi.Tag = "1";
            this.DDL_ImgAppType.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "CA公章";
            cbi.Tag = "2";
            this.DDL_ImgAppType.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "二维码";
            cbi.Tag = "3";
            this.DDL_ImgAppType.Items.Add(cbi);
            Glo.SetComboBoxSelected(this.DDL_ImgAppType, img.ImgAppType);
            this.RB_0.IsChecked = false;
            this.RB_1.IsChecked = false;

            this.TB_ImgPath.Text = img.ImgPath;
            this.TB_ImgUrl.Text = img.ImgURL;
            TB_CN_Seal.Text = this.HisImg.TB_CN_Name;
            TB_En_Seal.Text = this.HisImg.TB_En_Name;

            if (img.SrcType == 0  )
                this.RB_0.IsChecked = true;
            else
                this.RB_1.IsChecked = true;

            this.Show();
        }
        private void DDL_WinName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem it = (ComboBoxItem)this.DDL_WinName.SelectedItem;
            if (it == null)
                return;
            this.TB_WinName.Text = it.Tag.ToString();
            if (this.TB_WinName.Text == "def")
                this.TB_WinName.Text = "";
        }

        private void TB_CN_Seal_LostFocus(object sender, RoutedEventArgs e)
        {
            Glo.GetKeyOfEn(this.TB_CN_Seal.Text, true, this.TB_En_Seal);
        }
      

        //上传图片
        private void Btn_B_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "PNG 图片 (*.png)|*.png|JPG 图片 (*.jpg)|*.jpg";

            bool? result = dlg.ShowDialog();

            if (result != null && result == true)
            {
                string name = dlg.File.Name;
                string extension = name.Substring(name.LastIndexOf('.'), name.Length - name.LastIndexOf('.')); //取得扩展名（包括“.”） 
                FileName = DateTime.Now.ToString("yyyyMMddhhmmss") + extension; // 根据当前时间重命名 
                uploadImage(FileName, dlg.File.OpenRead());
                this.TB_ImgPath.Text = name;
            }
        }

        private void uploadImage(string fileName, Stream data)
        {
            Uri uri = new Uri(string.Format(Glo.BPMHost + "/WF/Admin/FoolFormDesigner/CCForm/DataHandler.ashx?filename={0}", fileName), UriKind.RelativeOrAbsolute);

            WebClient client = new WebClient();
            client.OpenWriteCompleted += delegate(object s, OpenWriteCompletedEventArgs e)
            {
                uploadData(data, e.Result);
                e.Result.Close();
                data.Close();
            };
            client.OpenWriteAsync(uri);
        }
        private void uploadData(Stream input, Stream output)
        {
            byte[] buffer = new byte[4096];
            int bytes;

            while ((bytes = input.Read(buffer, 0, buffer.Length)) != 0)
            {
                output.Write(buffer, 0, bytes);
            }
        }

        private void RB_0_Checked(object sender, RoutedEventArgs e)
        {
            this.TB_ImgUrl.IsEnabled = false;
            this.TB_ImgPath.IsEnabled = true;
          //  this.TB_LinkUrl.IsEnabled = true;
        }

        private void RB_1_Checked(object sender, RoutedEventArgs e)
        {
            this.TB_ImgUrl.IsEnabled = true;
            this.TB_ImgPath.IsEnabled = false;

           // this.TB_LinkUrl.IsEnabled = false;
        }
    }
}

