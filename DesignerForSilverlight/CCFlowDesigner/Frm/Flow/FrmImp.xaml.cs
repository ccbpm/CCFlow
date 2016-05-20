using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using BP;
using WF.WS;
using Silverlight;
using System.Collections;
using BP.Controls;

namespace BP.Frm
{
    public partial class FrmImp : ChildWindow
    {
        OpenFileDialog dialog = new OpenFileDialog();
        private byte[] buffer;
        FileInfo file;
        string currFK_FrmSort = "01";
        public FrmImp()
        {
            InitializeComponent();
            string sql = "SELECT No,Name FROM Sys_FormTree";
            WSDesignerSoapClient da_InitFrmSort = Glo.GetDesignerServiceInstance();
            da_InitFrmSort.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            da_InitFrmSort.RunSQLReturnTableCompleted += new EventHandler<RunSQLReturnTableCompletedEventArgs>(da_InitFrmSort_RunSQLReturnTableCompleted);
        }

        void da_InitFrmSort_RunSQLReturnTableCompleted(object sender, RunSQLReturnTableCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            this.comboBox1.Items.Clear();
            Glo.Ctrl_DDL_BindDataTable(this.comboBox1, ds.Tables[0], currFK_FrmSort);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = tabControl.SelectedItem as TabItem;
            if (null == selectedItem)
                return;

            if (selectedItem == tabItem1)
            {
                if (buffer == null || buffer.Length <= 0 || file == null
                    || this.comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("请选择模板文件，或者选择导入的类别。", "提示", MessageBoxButton.OK);
                    return;
                }

                //调用服务上传
                Glo.Loading(true);
                try
                {
                    WSDesignerSoapClient da = Glo.GetDesignerServiceInstance();
                    da.UploadfileCCFormAsync(buffer, file.Name, this.comboBox1.SelectedValue.ToString());
                    da.UploadfileCCFormCompleted += da_UploadfileCCFormCompleted;
                }
                catch (System.Exception ex)
                {
                    this.DialogResult = false;
                    Glo.Loading(false);
                    MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                }
            }

            if (selectedItem == tabItem2)
            {
                MessageBox.Show("此功能在施工中，敬请期待。或者访问http://templete.ccflow.org 流程与表单模板网下载到本机在导入。",
                    "Sorry", MessageBoxButton.OK);
            }
            this.DialogResult = false;
        }

        void da_UploadfileCCFormCompleted(object sender, UploadfileCCFormCompletedEventArgs e)
        {
            Glo.Loading(false);

            bool result = true;
            if (null != e.Error)
            {
                Glo.ShowException(e.Error);
                result = false;
                return;
            }
            else if (e.Result != null)
            {
                result = false;
                MessageBox.Show(e.Result, "导入错误", MessageBoxButton.OK);
            }
            else
            {
                result = true;
                this.DialogResult = result;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Btn_Upload_Click(object sender, RoutedEventArgs e)
        {
            dialog.Filter = "Xml Files (.xml)|*.xml|All Files (*.*)|*.*";
            if (dialog.ShowDialog().Value)
            {
                // 选择上传的文件
                file = dialog.File;
                Stream stream = file.OpenRead();
                stream.Position = 0;
                buffer = new byte[stream.Length + 1];
                //将文件读入字节数组
                stream.Read(buffer, 0, buffer.Length);
                this.textBox1.Text = dialog.File.Name;
            }
            else
            {
                MessageBox.Show("请选择文件！", "提示", MessageBoxButton.OK);
            }
        }

        
    }
}

