using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BP.Controls;
using BP.Frm;
using BP.SL;
using Liquid;
using Silverlight;
using WF.WS;

namespace BP
{
    /// <summary>
    /// 设计器主页面
    /// </summary>
    public partial class MainPage
    {
        #region 变量


        private TreeNode nodeFlow,
            nodeForm,
            nodeGpm = new TreeNode();
        private List<ToolbarButton> ToolBarButtonList = new List<ToolbarButton>();
        private const string ToolBarEnableIsFlowSensitived = "EnableIsFlowSensitived";

        public static readonly string CustomerId = "CCFlow";

        private WSDesignerSoapClient _service = Glo.GetDesignerServiceInstance();
        public WSDesignerSoapClient _Service
        {
            get { return _service; }
            set { _service = value; }
        }

        public Container SelectedContainer
        {
            get
            {
                var c = (Container)ccDesigner.Content;
                return c;
            }
        }

        public bool IsRefresh { get; set; }

        #endregion

        static MainPage instance;
        public static MainPage Instance
        {
            get
            {
                if (null == instance)
                    instance = new MainPage();
                return MainPage.instance;
            }
        }

        private MainPage()
        {
            InitializeComponent();
            HtmlPage.RegisterScriptableObject("Designer", this);
            this.LayoutRoot.SizeChanged += this.LayoutRoot_SizeChanged;
            Application.Current.Host.Content.Resized += Content_Resized;
            Application.Current.Host.Content.FullScreenChanged += Content_FullScreenChanged;
            this.Loaded += MainPage_Loaded;

            this.getOsModel();
            this.setCustomerAttribute();
            this.initFlowTempleteLoadCompeletedEventHandler();

            //this.tvwFlow.NodeDoubleClick += (object sender, TreeEventArgs e) =>
            //{
            //    #region 流程设计器
            //    TreeNode node = sender as TreeNode;
            //    if (null != node && !node.IsSort)
            //    {
            //        OpenFlow(node.Name, node.Title);
            //    }
            //    #endregion
            //};
            //this.tvwForm.NodeDoubleClick += (object sender, TreeEventArgs e) =>
            //{
            //    #region 独立表单设计器
            //    TreeNode node = sender as TreeNode;
            //    if (null != node && !node.IsSort)
            //    {
            //        string title = "表单ID: {0} 存储表:{1} 名称:{2}";
            //        title = string.Format(title, node.Name, node.Name, node.Title);
            //        if (Glo.UrlOrForm)
            //        {
            //            string url = "/WF/Admin/XAP/DoPort.aspx?DoType=MapDefFreeModel&FK_MapData=" + node.Name;
            //            OpenBPForm(BPFormType.FormFlow, title, url);
            //        }
            //        else
            //        {
            //            OpenBPForm(BPFormType.FormFlow, title, node.Name, node.Name);
            //        }
            //    }
            //    #endregion
            //};

            MouseButtonEventHandler rbd = (object sender, MouseButtonEventArgs e) =>
            { e.Handled = true; };
            this.MouseRightButtonDown += rbd;
        }

        // 配置用户属性b
        void setCustomerAttribute()
        {
            var designerService = Glo.GetDesignerServiceInstance();
            this.Visibility = System.Windows.Visibility.Collapsed;
            designerService.DoAsync("GetSettings", "CustomerNo", true);  // 图片.
            designerService.DoCompleted += (object senders, DoCompletedEventArgs ee) =>
            {
                System.Exception exc = null;
                bool toBeContinued = true;
                if (null != ee.Error)
                {
                    exc = ee.Error;
                    toBeContinued = false;
                }

                if (toBeContinued)
                    try
                    {
                        var id = ee.Result;
                        if (id == null || id == "" || id.ToLower() == "ccflow" || id.ToLower() == "jflow")
                        {
                            if (Glo.IsJFlow == true)
                                id = "JFlow";
                            else
                                id = "CCFlow";
                        }

                        //Uri imageUri = new Uri(srcRelative, UriKind.Relative);
                        //icon.img.Source = new BitmapImage(imageUri);
                        //icon.txtLink.Text = name;
                        //icons.Add(icon);
                        //setCursorTemplate(name, srcRelative);

                        var brush = new ImageBrush(); //定义图片画刷
                        brush.ImageSource = new BitmapImage(new Uri(string.Format("./Images/Icons/{0}/Welcome.png", id), UriKind.Relative));
                        ccDesigner.Background = brush;
                    }
                    catch (System.Exception ex)
                    {
                        exc = ex;
                        toBeContinued = false;
                    }

                if (!toBeContinued)
                {
                    Glo.ShowException(exc);
                }
                this.Visibility = System.Windows.Visibility.Visible;

            };
        }
        public void LoginCompleted()
        {
            try
            {
                this.SetSelectedTool("Wait");
                //装 toolbar.
                this.LoadToolbar();
                
                //打开流程，2015-10-8，added by liuxc
                if (!string.IsNullOrWhiteSpace(Glo.FK_Flow))
                    OpenFlow(Glo.FK_Flow, "流程设计");
            }
            catch (System.Exception ex)
            {
                Glo.ShowException(ex, "系统登录失败");
            }
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            #region 让其自动登录,如果有userNo, SID.
            bool isAutoLogOn = false;
            try
            {
                if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("UserNo") == true)
                {
                    Glo.UserNo = System.Windows.Browser.HtmlPage.Document.QueryString["UserNo"];
                    Glo.SID = System.Windows.Browser.HtmlPage.Document.QueryString["SID"];
                    isAutoLogOn = String.IsNullOrEmpty(Glo.UserNo) == false && string.IsNullOrEmpty(Glo.SID) == false;
                }

                if (isAutoLogOn == false)
                {
                    Glo.Login();
                    return;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                Glo.Login();
                return;
            }
            #endregion 让其自动登录.

            var ws = Glo.GetDesignerServiceInstance();
            string sql = "SELECT SID FROM Port_Emp WHERE No='" + Glo.UserNo + "'";
            ws.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
            ws.RunSQLReturnTableCompleted
                += (object senders, RunSQLReturnTableCompletedEventArgs ee) =>
                {
                    #region
                    if (null != ee.Error)
                    {
                        Glo.ShowException(ee.Error, "登录错误");
                        Glo.Login();
                        return;
                    }
                    try
                    {
                        DataSet ds = new DataSet();
                        try
                        {
                            if (ee.Result == null)
                            {
                                Glo.Login();
                                throw new System.Exception("@没有查询到该用户的登录SID信息.");
                            }
                            ds.FromXml(ee.Result);
                        }
                        catch (System.Exception ex)
                        {
                            Glo.ShowException(ex, "登录错误");
                            return;
                        }

                        DataTable dt = ds.Tables[0];
                        if (dt.Rows.Count != 1)
                            throw new System.Exception("@没有查询到该用户的登录SID信息.");
                        string s = dt.Rows[0][0].ToString();
                        if (s.Equals(Glo.SID) == false)
                        {
                            throw new System.Exception("@用户名或者密码错误.");
                        }
                        else
                        {
                            //Glo.UserNo = userNo;
                            //Glo.SID = sid;
                            MainPage.Instance.LoginCompleted();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Glo.ShowException(ex, "登录错误");
                        Glo.Login();
                    }
                    #endregion
                };
        }

        #region FlowTree
        
        /// <summary>
        /// 删除工作流类别
        /// </summary>
        /// <param name="flowsortid">工作流类别ID</param>
        public void DeleteFlowSort(string flowsortid)
        {
            if (HtmlPage.Window.Confirm("您确认要删除吗？"))
            {
                _Service.DoAsync("DelFlowSort", flowsortid, true);
                Glo.Loading(true);
                _Service.DoCompleted += Server_DoCompletedToRefreshSortTree;
            }
        }

        string flowNoToDel = string.Empty;
        /// <summary>
        /// 删除工作流并更新流程树菜单和画板
        /// </summary>
        public void DeleteFlow(string flowid)
        {
            if (HtmlPage.Window.Confirm("您确定要删除编号为【" + flowid + "】的工作流吗？"))
            {
                Glo.Loading(true);
                _Service.DoCompleted += Server_DoCompletedToRefreshSortTree;
                _Service.DoAsync("DelFlow", flowid, true);
                flowNoToDel = flowid;

            }
        }
        void Server_DoCompletedToRefreshSortTree(object sender, DoCompletedEventArgs e)
        {
            Glo.Loading(false);
            _Service.DoCompleted -= Server_DoCompletedToRefreshSortTree;

            if (null != e.Error)
            {
                Glo.ShowException(e.Error, "流程删除错误");
                return;
            }
            if (e.Result != null)
            {
                MessageBox.Show(e.Result, "Err", MessageBoxButton.OK);
                return;
            }

            this.flowNoToDel = string.Empty;
        }

        /// <summary>
        /// 新建工作流
        /// </summary>
        public void NewFlow(string flowSortId, string flowName, int dataSaveModel, string pTable, string flowCode)
        {
            Glo.Loading(true);
            _Service.DoAsync("NewFlow", flowSortId + "," + flowName + "," + dataSaveModel + "," + pTable + "," + flowCode, true);
            _Service.DoCompleted += _service_DoCompleted;
        }
        private void _service_DoCompleted(object sender, DoCompletedEventArgs e)
        {
            bool toBeContinued = true;
            System.Exception exc = null;
            _Service.DoCompleted -= _service_DoCompleted;

            if (null != e.Error)
            {
                exc = e.Error;
                toBeContinued = false;
            }

            if (toBeContinued && e.Result.IndexOf(";") < 0)
            {
                MessageBox.Show(e.Result, "错误3", MessageBoxButton.OK);
                toBeContinued = false;
            }

            if (toBeContinued)
            {
                try
                {
                    string[] flow = e.Result.Split(';');

                    OpenFlow(flow[0], flow[1]);
                }
                catch (System.Exception ex)
                {
                    exc = ex;
                }
            }

            if (exc != null)
            {
                Glo.ShowException(exc, "新建流程错误：" + exc.Message);
            }
            Glo.Loading(false);
        }

        public void NewFlowHandler(int tabIdx)
        {
            //Glo.FK_FlowSort = tvwFlow.Selected.ID;
            var newFlow = new FrmNewFlow();
            newFlow.tabControl.TabIndex = tabIdx;

            newFlow.FlowTempleteLoadCompeletedEventHandler += FlowTemplateLoadCompeleted;
            newFlow.Show();
        }

        void ViewShareTemplate()
        {
            //    FtpFileExplorer templateView = new FtpFileExplorer();
            //    templateView.FlowTemplateLoadCompeleted += this.FlowTemplateLoadCompeleted;
            //   templateView.Show();
        }
        // 流程装载到数据库结束事件，在新建和导入时使用，需要手动初始化
        EventHandler<FlowTemplateLoadCompletedEventArgs> FlowTemplateLoadCompeleted;
        void initFlowTempleteLoadCompeletedEventHandler()
        {
            Glo.Loading(false);
            FlowTemplateLoadCompeleted = (object sender, FlowTemplateLoadCompletedEventArgs e) =>
            {
                try
                {
                    if (null != e.Error)
                    {
                        Glo.ShowException(e.Error, "装载流程时出错");
                    }
                    else
                    {
                        // 返回值的格式为FlowSortID,FlowId,FlowName
                        var result = e.Result.Split(',');
                        if (!e.Result.Contains("TRUE") || result.Length != 4)
                        {
                            MessageBox.Show("装载流程时出错" + e.Result);
                        }
                        else
                        {
                            string msgWin =
@"    流程模版已经被成功下载并安装到本地服务器，但该流程模版可能不能正常工作，您需要注意如下几点并做相应的修改:
    1、该模版的节点或者流程绑定的岗位、部门、人员信息，会与您系统的岗位、部门、人员信息编号不一致，你需要重新绑定才能使用。
    2、对于节点、流程、表单事件的特殊业务处理，会影响到在您系统的执行，您可以禁用或者编辑他们符合自己的业务环境需要。
";
                            MessageBox.Show(msgWin, "流程成功导入", MessageBoxButton.OK);
                            OpenFlow(result[1], result[2], result[3]);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Glo.ShowException(ex, "装载流程时出错");
                }
            };
        }

        #endregion
        
        #region GPMTree
        bool isGMPTreeInited;
        
        void getOsModel()
        {
            WSDesignerSoapClient ws = Glo.GetDesignerServiceInstance();
            ws.GetConfigAsync("OSModel");
            ws.GetConfigCompleted += (object sender, GetConfigCompletedEventArgs e) =>
            {
                if (e.Error == null)
                {
                    Glo.OsModel = (OSModel)Enum.Parse(typeof(OSModel), e.Result, true);
                }
            };
        }
        #endregion

        #region Toolbar

        private void LoadToolbar()
        {
            var ens = new List<ToolbarItem>();
            ens = ToolbarItem.Instance.GetLists();
            foreach (ToolbarItem en in ens)
            {
                var btn = new ToolbarButton();
                btn.Name = "Btn_" + en.No;
                btn.IsEnabled = en.IsEnable;
                if (!en.IsEnable)
                {
                    btn.Tag = ToolBarEnableIsFlowSensitived;
                }
                btn.Click += new RoutedEventHandler(ToolBar_Click);

                var mysp = new StackPanel();
                mysp.Orientation = Orientation.Horizontal;
                mysp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                mysp.Name = "sp" + en.No;

                var img = new Image();
                var png = new BitmapImage(new Uri("/Images/" + en.No + ".png", UriKind.Relative));
                img.Source = png;
                mysp.Children.Add(img);

                var tb = new TextBlock();
                tb.Name = "tbT" + en.No;
                tb.Text = en.Name;
                tb.FontSize = 12;
                mysp.Children.Add(tb);

                btn.Content = mysp;
                this.toolbar1.AddBtn(btn);
                ToolBarButtonList.Add(btn);
            }
        }
        private void ToolBar_Click(object sender, RoutedEventArgs e)
        {
            var control = sender as ToolbarButton;
            if (null == control)
                return;
            try
            {
                switch (control.Name)
                {
                    case "Btn_ToolBarFrmLab":
                        Glo.OpenWinByDoType("CH", BP.UrlFlag.FrmLib, "", "0", null);
                        return;
                    case "Btn_ToolBarLogin":
                        string url3 = @"/WF/App/Classic/Login.aspx?DoType=Logout";
                        Glo.OpenWindow(url3, "登陆");
                        return;
                    case "Btn_ToolBarFlowUI":
                        this.ChageFlowUI(control);
                        break;
                    case "Btn_ToolBarSave":
                        this.save();
                        break;
                    case "Btn_ToolBarDesignReport":
                        // 报表设计事件
                        if (SelectedContainer != null)
                        {
                            SelectedContainer.btnDesignerTable();
                        }
                        break;
                    case "Btn_ToolBarCheck":
                        //点击检查流程之前，需要执行一次保存，否则会出现流程数据错误
                        if (SelectedContainer != null)
                        {
                            this.save();

                            SelectedContainer.btnCheck();
                        }
                        break;
                    case "Btn_ToolBarRun":
                        if (SelectedContainer != null)
                            SelectedContainer.btnRun();
                        break;
                    case "Btn_ToolBarEditFlow":
                        Glo.OpenWinByDoType("CH", "FlowP", SelectedContainer.FlowID, null, null);
                        break;
                    case "Btn_ToolBarDeleteFlow":
                        DeleteFlow(SelectedContainer.FlowID);
                        break;
                    case "Btn_ToolBarHelp":
                        Glo.OpenHelp();
                        break;
                    case "Btn_ToolBarToolBox":
                        setToolBoxVisiable(true);
                        break;
                    case "Btn_ToolBarGenerateModel":
                        Glo.OpenWindow("/WF/Admin/XAP/DoPort.aspx?DoType=ExpFlowTemplete&FK_Flow=" + SelectedContainer.FlowID + "&Lang=CH", "Help", 50, 50);
                        break;

                    case "Btn_ToolBarLoadModel":
                        NewFlowHandler(0);
                        break;

                    //case "Btn_ToolBarReleaseToFTP":
                    //    ReleaseToFtp();
                    //    break;
                    case "Btn_ToolBarShareModel":
                        ViewShareTemplate();
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Glo.ShowException(ex);
            }
        }

        //void ReleaseToFtp()
        //{
        //    if (SelectedContainer == null || SelectedContainer.FlowID != tvwFlow.Selected.Name)
        //    {
        //        MessageBox.Show("请先打开流程图");
        //        return;
        //    }

        //    //  FrmFlowShareToFtp ftp = new FrmFlowShareToFtp(SelectedContainer.FlowID);
        //    // ftp.Show();
        //}

        private void setToolBarButtonEnableStatus(bool isEnable)
        {
            foreach (var toolbarButton in ToolBarButtonList)
            {
                if (toolbarButton.Tag != null && toolbarButton.Tag.ToString() == ToolBarEnableIsFlowSensitived)
                {
                    toolbarButton.IsEnabled = isEnable;
                }
            }

        }
        #endregion


        public void AddSubTreeNode(TreeNode myNode, DataTable dt, TreeType type)
        {
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    string parentNo = dr["ParentNo"] as string;
                    if (parentNo == "0" || parentNo != myNode.ID.ToString())
                        continue;

                    TreeNode subNode = new TreeNode();
                    subNode.Title = dr["Name"].ToString();
                    subNode.Name = dr["No"].ToString();
                    subNode.ID = dr["No"].ToString();
                    subNode.IsSort = true;

                    switch (type)
                    {
                        case TreeType.Flow:
                            subNode.Icon = "../Images/MenuItem/FlowSort.png";
                            nodeFlow.Nodes.Add(subNode);
                            break;
                        case TreeType.GPM:
                            subNode.Icon = "../Images/MenuItem/Post.png";
                            nodeGpm.Nodes.Add(subNode);
                            break;
                        case TreeType.MapData:
                            subNode.Icon = "../Images/MenuItem/FlowSheet.png";
                            nodeForm.Nodes.Add(subNode);
                            break;
                        case TreeType.System:
                            break;
                    }

                    myNode.Nodes.Add(subNode);
                    AddSubTreeNode(subNode, dt, type);
                }
            }
            catch (System.Exception e)
            {
                Glo.ShowException(e);
            }
        }

        private void menu_ItemSelected(object sender, MenuEventArgs e)
        {
            Menu menu = sender as Menu;
            if (menu == null) return;
            menu.Hide();

            if (e.Tag == null) return;

            switch (menu.Name)
            {
                case "MuFlowTree":
                    #region

                    switch (e.Tag.ToString())
                    {
                        case "menuExp":
                            BP.Frm.FrmExp exp = new BP.Frm.FrmExp();
                            exp.Show();
                            break;
                        case "Help":
                            Glo.OpenHelp();
                            break;
                        case "NewFlow_Blank":
                            NewFlowHandler(0);
                            break;
                        case "NewFlow_Disk":
                            NewFlowHandler(1);
                            break;
                        case "NewFlow_CCF":
                            NewFlowHandler(2);
                            break;
                    }

                    #endregion
                    break;
                case "menuForm":
                case "menuFormSort":
                    #region

                    Glo.TempCmd = e.Tag.ToString();
                    switch (e.Tag.ToString())
                    {
                        case "Frm_EditForm": //表单属性.
                            //string url = "/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.Sys.MapDataExts&PK=" + selectedNode.Name;
                            //Glo.OpenDialog(url, "表单属性");
                            return;
                        case "Frm_NewForm": //新建表单
                            BP.Frm.Frm frm1 = new BP.Frm.Frm();
                            frm1.SortNo = Glo.FK_FormSort;
                            frm1.BindNew();
                            frm1.HisMainPage = this;
                            frm1.Show();
                            break;
                        case "Frm_FormDesignerFix": //设计傻瓜表单
                            //Glo.OpenWinByDoType("CH", UrlFlag.FormFixModel, selectedNode.Name, null, null);
                            break;
                        case "Frm_FormDesignerFree": //设计free表单
                            string title = "表单ID: {0} 存储表:{1} 名称:{2}";
                            //title = string.Format(title, selectedNode.Name, "Sys_MapData", selectedNode.Title);
                            //OpenBPForm(BPFormType.FormFlow, title, selectedNode.Name, selectedNode.Name);
                            break;
                        case "Help":
                            Glo.OpenHelp();
                            break;
                        case "Frm_Imp":  //Imp
                            BP.Frm.FrmImp imp = new BP.Frm.FrmImp();
                            imp.Show();
                            break;
                        default:
                            MessageBox.Show("功能未完成:" + Glo.TempCmd);
                            break;
                    }
                    #endregion
                    break;
                case "menuApp":
                    switch (e.Tag.ToString())
                    {
                        case "ExceptionLog":
                            BP.SL.OutputChildWindow.ShowException();
                            break;
                        case "Help":
                            Glo.OpenHelp();
                            break;
                    }
                    break;
            }
        }
        
        void Content_FullScreenChanged(object sender, EventArgs e)
        {
            this.LayoutRoot.Width = Application.Current.Host.Content.ActualWidth;
            this.LayoutRoot.Height = Application.Current.Host.Content.ActualHeight;
        }

        void Content_Resized(object sender, EventArgs e)
        {
            this.LayoutRoot.Width = Application.Current.Host.Content.ActualWidth;
            this.LayoutRoot.Height = Application.Current.Host.Content.ActualHeight;
            
            if (ccFormContainer != null)
            {
                ccFormContainer.content.Width = LayoutRoot.Width;
                ccFormContainer.content.Height = LayoutRoot.Height;
            }
        }

        void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (LayoutRoot.Height < 100) return;

            var height = LayoutRoot.Height - 35;

            this.flowToolBoxIcon.lbTools.Height = height - 20;

            ccDesigner.Height = LayoutRoot.Height - 35;
            ccDesigner.Width = LayoutRoot.ActualWidth - 5;
        }
    }
}