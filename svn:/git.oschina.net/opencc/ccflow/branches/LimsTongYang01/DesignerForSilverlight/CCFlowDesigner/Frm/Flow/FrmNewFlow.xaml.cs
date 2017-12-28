using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Silverlight;
using WF.WS;
using System.Windows.Media;
namespace BP.Controls
{
    public partial class FrmNewFlow : ChildWindow
    {
        OpenFileDialog dialog = new OpenFileDialog();
        private byte[] buffer;
        FileInfo file;

        public event EventHandler<FlowTemplateLoadCompletedEventArgs> FlowTempleteLoadCompeletedEventHandler;
     
        /// <summary>
        /// 默认流程类别
        /// </summary>
        public String CurrentFlowSortNo { get; set; }

        public FrmNewFlow()
        {
            InitializeComponent();

            string sql = "SELECT no,name FROM WF_FlowSort ";
            WSDesignerSoapClient da = Glo.GetDesignerServiceInstance();
            da.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableCompleted += da_RunSQLReturnTableCompleted;

            string EnumText = "Name";
            DataTable dt=new DataTable();
            dt.Columns.Add(new DataColumn("No"));
            dt.Columns.Add(new DataColumn(EnumText));

            DataRow dr = dt.NewRow();
            dr[0] = "0";
            dr[1] = "数据轨迹模式";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "1";
            dr[1] = "数据合并模式";
            dt.Rows.Add(dr);
            IList list = dt.GetBindableData(new Connector());

            DDL_DataStoreModel.ItemsSource = list;
            DDL_DataStoreModel.DisplayMemberPath = EnumText;
            DDL_DataStoreModel.SelectedIndex = 0;

            dt = new DataTable();
            dt.Columns.Add(new DataColumn("No"));
            dt.Columns.Add(new DataColumn(EnumText));

            dr = dt.NewRow();
            dr[0] = "0";
            dr[1] = "做为新流程导入";
            dt.Rows.Add(dr);

            dr = dt.NewRow();
            dr[0] = "1";
            dr[1] = "按导入模板流程编号导入";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr[0] = "2";
            dr[1] = "按导入模板流程编号导入若存在就覆盖";
            dt.Rows.Add(dr);
            dr = dt.NewRow();
            dr[0] = "3";
            dr[1] = "按指定流程编号导入";
            dt.Rows.Add(dr);
            list = dt.GetBindableData(new Connector());
            cbxFlowImpType.ItemsSource = list;
            cbxFlowImpType.DisplayMemberPath = EnumText;
            cbxFlowImpType.SelectionChanged += (object sender, SelectionChangedEventArgs e)=>
            {
                System.Windows.Visibility  visi= System.Windows.Visibility.Collapsed;
                if (cbxFlowImpType.SelectedIndex == 3)
                {
                    visi = System.Windows.Visibility.Visible;
                }
                this.flowImpSpecialNo.Visibility = visi;
            };
            cbxFlowImpType.SelectedIndex = 0;

            tabControl.SelectionChanged +=  tabControl_SelectionChanged;
        }

      
        void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem tbItem = (sender as TabControl).SelectedItem as TabItem;
            if (null == tbItem) return;

            if (tbItem == this.tabImportNew)
            {
                this.Title = "导入流程";
            }
            else if (tbItem == this.tabStandardNew)
            {
                this.Title = "新建流程";
            }
            else if (tbItem == this.tabItemCCflow)
            {
               // new FtpFileExplorer().Show();
                this.DialogResult = true;
            }
        }
      
        void da_RunSQLReturnTableCompleted(object sender, RunSQLReturnTableCompletedEventArgs e)
        {
            DataSet ds = new DataSet();
            ds.FromXml(e.Result);
            DataTable dt = ds.Tables[0];

            // 得到默认的流程类别
            int defaultFlowSort = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0] == CurrentFlowSortNo)
                {
                    defaultFlowSort = i;
                }
            }

            IList list = dt.GetBindableData(new Connector());
            if (list.Count > 0)
            {
                cbxFlowSortImport.ItemsSource = list;
                cbxFlowSortImport.DisplayMemberPath = dt.Columns[1].ColumnName;
                cbxFlowSortImport.SelectedIndex = defaultFlowSort;

                DDL_FlowSort.ItemsSource = list;
                DDL_FlowSort.DisplayMemberPath = dt.Columns[1].ColumnName;
                DDL_FlowSort.SelectedIndex = defaultFlowSort;
            }
        }

		void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = tabControl.SelectedItem as TabItem;
            if ( selectedItem == null )
                return;

            if (selectedItem == tabStandardNew)
            {
                if (string.IsNullOrWhiteSpace(TB_FlowName.Text))
                {
                    MessageBox.Show("请输入流程名称", "提示", MessageBoxButton.OK);
                    return;
                }

                //判断流程数据表不能包含汉字
                if (!string.IsNullOrWhiteSpace(this.TB_PTable.Text) && BP.SL.Glo.ContainsChinese(this.TB_PTable.Text))
                {
                    MessageBox.Show("流程数据表 不能使用汉字！");
                    return;
                }
               
                    var flowSortID = (DDL_FlowSort.SelectedItem as BindableObject).GetValue("NO");
                    var DataStoreModel = this.DDL_DataStoreModel.SelectedIndex;
                    var ptable = this.TB_PTable.Text;
                    var flowCode = this.TB_FlowCode.Text;

                    MainPage.Instance.NewFlow(flowSortID, TB_FlowName.Text, DataStoreModel, ptable, flowCode);
                    this.DialogResult = true;
            }

            if (selectedItem == tabImportNew)
            {
                if (buffer == null || buffer.Length <= 0 || file == null || cbxFlowSortImport.SelectedIndex == -1)
                {
                    MessageBox.Show("请选择模板文件", "提示", MessageBoxButton.OK);
                    return;
                }

                if (this.flowImpSpecialNo.Visibility == System.Windows.Visibility.Visible)
                {
                    if (!Glo.IsNum(this.flowImpSpecialNo.Text))
                    {
                        MessageBox.Show("流程编号只允许输入数字", "提示", MessageBoxButton.OK);
                        this.flowImpSpecialNo.Focus();
                        return;
                    }
                }

                //调用服务上传
                try
                {
                    UpLoad();
                }
                catch (System.Exception ex)
                {
                    this.DialogResult = false;
                    MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK);
                }
            }
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        void BtnUpLoad_Click(object sender, RoutedEventArgs e)
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

                tbxFileName.Text = dialog.File.Name;
            }
            else
            {
                MessageBox.Show("请选择文件！", "提示", MessageBoxButton.OK);
            }
        }

        void UpLoad()
        {
            Glo.Loading(true);
            WSDesignerSoapClient service = Glo.GetDesignerServiceInstance();
            service.FlowTemplateUploadAsync(buffer, file.Name);
            service.FlowTemplateUploadCompleted += (object sender, FlowTemplateUploadCompletedEventArgs e) =>
            {
                if (null != e.Error)
                {
                    Glo.Loading(false);
                    Glo.ShowException(e.Error);
                }
                else if (e.Result.Contains("Error:"))
                {
                    Glo.Loading(false);
                    MessageBox.Show(e.Result, "Error", MessageBoxButton.OK);
                }
                else
                {
                    string flowSort = (cbxFlowSortImport.SelectedItem as BindableObject).GetValue("NO");
                    service.FlowTemplateLoadAsync(flowSort, e.Result, cbxFlowImpType.SelectedIndex,-1 );
                    service.FlowTemplateLoadCompleted += (object senders, FlowTemplateLoadCompletedEventArgs ee) =>
                    {
                        if (null != FlowTempleteLoadCompeletedEventHandler)
                            FlowTempleteLoadCompeletedEventHandler(sender, ee);
                    };
                }
            };

            this.DialogResult = true;
        }      

     

        private void flowImpSpecialNo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(flowImpSpecialNo.Text))
            {
                flowImpSpecialNo.Text = "请输入指定流程编号";
                flowImpSpecialNo.SelectAll();
            }
        }
       
    }
}

