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
using Liquid;
using FtpFile = global::WF.WS.FtpFile;
namespace BP.Controls
{
    public partial class FtpFileExplorer : ChildWindow
    {
        static FtpFile directoryData;
        FtpFile curFile;
        FtpFileDown down ;

        public event EventHandler<global::WF.WS.FlowTemplateLoadCompletedEventArgs> FlowTemplateLoadCompeleted;
        
        public FtpFileExplorer()
        {
            InitializeComponent();
        }
      
        /// <summary>
        /// 下载窗口关闭时如果在线安装成功需要在设计器中打开新增流程
        /// </summary>
        public void Show()
        {
            base.Show();

            if (null == down)
            {
                down = new FtpFileDown();
                down.FlowTempleteLoadCompeleted += (object sender, global::WF.WS.FlowTemplateLoadCompletedEventArgs e) =>
                {
                    this.Close();
                    if (null != FlowTemplateLoadCompeleted)
                        FlowTemplateLoadCompeleted(sender, e);
                };
            }

            if (null == directoryData)
                this.getDirectory();
            else  
                btnBack_MouseLeftButtonDown(null, null);

        }

        public void FtpFileClick(string dir)
        {
            if (null != curFile.Subs)
                foreach (var item in curFile.Subs)
                {
                    if (item.Name.Equals(dir))
                    {
                        item.Super = curFile;
                        curFile = item;
                        break;
                    }
                }

            if (null == curFile) return;

            this.url.Text = curFile.Path;
            if (null != curFile.Super && curFile.CanViewAndDown)
            {
                if (null == down)
                    down = new FtpFileDown();

                down.Init(curFile);
                down.Show();
            }
            else  if (null != curFile.Subs)
            {
                showDirectory(curFile.Subs);
            }
        }

        private void getDirectory()
        {
            this.Cursor = Cursors.Wait;
            var _service = Glo.GetDesignerServiceInstance();
            _service.GetDirectoryAsync();
            _service.GetDirectoryCompleted += (object sender, global::WF.WS.GetDirectoryCompletedEventArgs e) =>
            {
                bool toBeContinued = true;
                if (null != e.Error)
                {
                    MessageBox.Show(e.Error.Message);
                    toBeContinued = false;
                }

                if (toBeContinued)
                {
                    directoryData = e.Result;
                    curFile = directoryData;
                    showDirectory(directoryData.Subs);
                }
                this.Cursor = Cursors.Arrow;
            };
        }
        private void showDirectory(FtpFile[] listDir)
        {
            if (null == listDir) return;

            List<FtpFileLink> list = new List<FtpFileLink>();
            foreach (var item in listDir)
            {
                FtpFileLink link = new FtpFileLink() { Cursor = Cursors.Hand };
                if (null == link.MouseLeftButtonDown)
                    link.MouseLeftButtonDown = (object sender, MouseButtonEventArgs e) =>
                    {
                        FrameworkElement ele = sender as FrameworkElement;
                        string dir = ele.Tag.ToString();
                        this.FtpFileClick(dir);
                    };
                link.txtLink.Text = item.Name;
                link.Type = item.Type;
                list.Add(link);
            }

            this.lbFtpFile.ItemsSource = list;
        }
        private FtpFile get(string name, FtpFile[] files)
        {
            foreach (FtpFile item in files)
            {
                if (item.Name.Equals(name))
                    return item;
                else if( item.Subs != null)
                {
                    return get(name, item.Subs);
                }
            }

            return null;
         
        }

        private void btnBack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            curFile = directoryData;
            this.url.Text = curFile.Path;
            showDirectory(directoryData.Subs);
        }
   
    }
}

