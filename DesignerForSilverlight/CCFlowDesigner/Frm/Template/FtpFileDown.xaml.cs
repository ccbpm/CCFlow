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

using Silverlight;
using System.IO;
using System.Windows.Media.Imaging;
using System.Collections;
using FtpFile = global::WF.WS.FtpFile;

namespace BP.Controls
{
    public partial class FtpFileDown : ChildWindow
    {
        string path, fileName, fileType, cmd;

        public FtpFileDown()
        {
            InitializeComponent();
            this.lbFlows.SelectionChanged += new SelectionChangedEventHandler(lbFlows_SelectionChanged);
        }
       
        public void Init(FtpFile superFile)
        {
            this.txtFlow.Text = superFile.Name;
            path = superFile.Path;

            this.lbFlows.ItemsSource = superFile.Subs;
            this.lbFlows.DisplayMemberPath = "Name";
            //this.lbFlows.SelectedItem = "Flow";
            this.lbFlows.SelectedIndex = superFile.Subs.Length-1;
        }

        /// <summary>
        /// 图片预览，默认加载Flow.png
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lbFlows_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (null != this.lbFlows.SelectedValue)
            {
                fileName = (this.lbFlows.SelectedValue as FtpFile).Name + ".png";
                fileType = "PNG";
                cmd = "VIEW";
                DoFtp(path, fileName, fileType, cmd);
            }
        }

        bool sdREsult;
        SaveFileDialog sfd;
        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Name.Equals("btnDown"))
            {// xml 下载
                if (null != this.lbFlows.SelectedValue)
                {
                    sfd = new SaveFileDialog()
                    {
                        DefaultExt = "xml",
                        Filter = "Xml files (*.xml)|*.xml|All files (*.*)|*.*",
                        FilterIndex = 1
                    };

                    bool? result = sfd.ShowDialog();
                    sdREsult = result == null ?　false:(bool)result;

                    fileName = (this.lbFlows.SelectedValue as FtpFile).Name +".xml";
                    fileType = "XML";
                    cmd = "DOWN";
                    DoFtp(path, fileName, fileType, cmd);
                }
            }
            else if (btn.Name.Equals("btnInstall"))
            {// 流程模板文件在线安装

                MessageBoxResult result = MessageBox.Show("您确定要下载该流程模版并安装到本地服务器上吗？", "下载安装", MessageBoxButton.OKCancel);
                if(result == MessageBoxResult.OK)
                {
                    fileName = "Flow.xml";
                    fileType = "XML";
                    cmd = "INSTALL";
                    DoFtp(path, fileName, fileType, cmd);
                }
            }
            else if (btn.Name.Equals("btnOK"))
            {
                this.DialogResult = true;
            }
            else if (btn.Name.Equals("btnCancel"))
            {
                this.DialogResult = false;
            }
        }
      
        void Loading(bool enabled)
        {
            MainPage.Instance.SetSelectedTool(enabled ? "Wait":"Arrow");
            enabled = !enabled;
            this.lbFlows.IsEnabled = enabled;
            this.btnDown.IsEnabled = enabled;
            this.btnInstall.IsEnabled = enabled;
          
        }

        public void DoFtp(string path, string fileName, string fileType, string cmd)
        {
            Loading(true);

            string[] FlowFileName = new string[] { path, fileName, fileType, cmd };
            var _service = Glo.GetDesignerServiceInstance();
            _service.FlowTemplateDownAsync(FlowFileName);
            _service.FlowTemplateDownCompleted += (object sender, global::WF.WS.FlowTemplateDownCompletedEventArgs e)=>
            {
                Loading(false);
                if (null != e.Error)
                {
                    Glo.ShowException(e.Error);
                    return;
                }

                if (fileType == "PNG")
                {
                    if (null == e.Result) return;
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    ms.Write(e.Result, 0, e.Result.Length);
                    if (ms.Capacity > 0)
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.SetSource(ms);
                        this.imgView.Width = bitmap.PixelWidth;
                        this.imgView.Height = bitmap.PixelHeight;
                        this.imgView.Source = bitmap;
                    }
                }
                else if (fileType == "XML")
                {
                    if (cmd == "DOWN")
                    {//// 保存到本地
                        if (sdREsult && null != e.Result)
                        {
                            using (Stream stream = sfd.OpenFile())
                            {
                                stream.Write(e.Result, 0, e.Result.Length);
                                stream.Close();
                            }
                        }
                    }
                    else if (cmd == "INSTALL")
                    {    ////在线安装
                        if (fileName.Equals("Flow.xml"))
                        {
                            Loading(true);
                            string filePath = System.Text.Encoding.UTF8.GetString(e.Result, 0, e.Result.Length);
                            _service.FlowTemplateLoadAsync(Glo.FK_FlowSort, filePath, 0 , 0);//安装流程
                            _service.FlowTemplateLoadCompleted += (object senders, global::WF.WS.FlowTemplateLoadCompletedEventArgs ee)=>
                            {
                                Loading(false);
                                if (null == ee.Error && !string.IsNullOrEmpty(ee.Result))
                                {
                                    this.Close();
                                  
                                    if (null != FlowTempleteLoadCompeleted)
                                        FlowTempleteLoadCompeleted(sender, ee);
                                }
                            };
                        }
                    }
                }
            };
        }


        // 在线安装完成后自动关闭并定位到流程设计器，并打开新增流程
        public event EventHandler<global::WF.WS.FlowTemplateLoadCompletedEventArgs> FlowTempleteLoadCompeleted;
       
    }
}

