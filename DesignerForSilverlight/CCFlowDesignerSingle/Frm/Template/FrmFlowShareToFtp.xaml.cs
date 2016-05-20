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
using System.Text.RegularExpressions;


namespace BP.Controls
{
    public partial class FrmFlowShareToFtp : ChildWindow
    {
        public string Fk_Flow,DirTo, BbsNo;
        private FrmFlowShareToFtp()
        { InitializeComponent(); }
        public FrmFlowShareToFtp(string Fk_Flow):this()
        {
      
            this.cobbFtpDirs.SelectionChanged += cobbFtpDirs_SelectionChanged;
            getShareDir();

            this.Closed += new EventHandler((object senders, EventArgs es) =>
            {
               
                if (this.DialogResult == true)
                {
                    string url = "/WF/Admin/XAP/DoPort.aspx?DoType=ShareToFtp&FK_Flow=" + Fk_Flow
                        + "&BBS=" + this.BbsNo + "&ShareTo=" + this.DirTo;
                    Glo.OpenDialog(url, "共享模板", 500, 500);

                    #region
                    //WebClient uploadClient = new WebClient();

                    //string tmp = FK_Flow + "&" + Email + "&" + ShareTo;
                    //byte[] buffer = System.Text.Encoding.Unicode.GetBytes(tmp);
                    ////Stream memStream = new MemoryStream();
                    ////memStream.Write(data, 0, data.Length); 
                    //uploadClient.OpenWriteAsync(new Uri(url, UriKind.Relative), "Post", buffer);   //打开上传连接

                    //uploadClient.OpenWriteCompleted += delegate(object sender, OpenWriteCompletedEventArgs e)
                    //{
                    //    using (Stream serverStream = e.Result)                    // e.Result - 目标地址的流（服务端流）
                    //    {
                    //        serverStream.Write(buffer, 0, buffer.Length);
                    //    }
                    //};
                       #endregion
                }
             
            });
        }
        private void cobbFtpDirs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DirTo = this.cobbFtpDirs.SelectedValue.ToString();
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            #region 验证

            if (string.IsNullOrEmpty(this.cobbFtpDirs.SelectedValue.ToString()))
            {
                MessageBox.Show("目录不能为空");
                return;
            }
           
            if (string.IsNullOrEmpty(txbEmail.Text))
            {
                MessageBox.Show("Email不能为空");
                return;
            }
            BbsNo = txbEmail.Text;
            #endregion
            this.DialogResult = true;
          
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        void getShareDir()
        {
            var _service = Glo.GetDesignerServiceInstance();
          
            _service.GetDirsAsync("FlowTemplate",true);
            _service.GetDirsCompleted +=(object sender, global::WF.WS.GetDirsCompletedEventArgs e)=>
            {
                if (null != e.Error)
                {
                    Glo.ShowException(e.Error);
                    return;
                }
                string dirs = e.Result;
                string[] dir = dirs.Split('@');

                List<string> listDir = new List<string>();
                foreach (string s in dir)
                {
                    listDir.Add(s);
                }
                this.cobbFtpDirs.ItemsSource = listDir;
                this.cobbFtpDirs.SelectedIndex = 0;
                this.cobbFtpDirs.IsEnabled = true;
                this.OKButton.IsEnabled = true;

            };
        }
    }
}

