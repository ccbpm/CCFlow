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
                var c = (Container)tbDesigner.SelectedContent;
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

            this.tvwFlow.NodeDoubleClick += (object sender, TreeEventArgs e) =>
            {
                #region 流程设计器
                TreeNode node = sender as TreeNode;
                if (null != node && !node.IsSort)
                {
                    OpenFlow(node.ID, node.Title);
                }
                #endregion
            };
            this.tvwForm.NodeDoubleClick += (object sender, TreeEventArgs e) =>
            {
                #region 独立表单设计器
                TreeNode node = sender as TreeNode;
                if (null != node && !node.IsSort)
                {
                    string title = "表单ID: {0} 存储表:{1} 名称:{2}";
                    title = string.Format(title, node.ID, node.ID, node.Title);
                    if (Glo.UrlOrForm)
                    {
                        string url = "/WF/Admin/XAP/DoPort.aspx?DoType=MapDefFreeModel&FK_MapData=" + node.ID;
                        OpenBPForm(BPFormType.FormFlow, title, url);
                    }
                    else
                    {
                        OpenBPForm(BPFormType.FormFlow, title, node.ID, node.ID);
                    }
                }
                #endregion
            };

            this.tvwOrg.NodeDoubleClick += (tvwOrg_NodeDoubleClick);
            //this.tvwOrg.MouseRightButtonUp += element_MouseRightButtonUp;

            MouseButtonEventHandler rbd = (object sender, MouseButtonEventArgs e) =>
            { e.Handled = true; };
            this.MouseRightButtonDown += rbd;
            this.tvwSysMenu.MouseRightButtonDown += rbd;
            this.tvwOrg.MouseRightButtonDown += rbd;
            this.tvwFlow.MouseRightButtonDown += rbd;
            this.tvwForm.MouseRightButtonDown += rbd;

            this.MouseRightButtonUp += element_MouseRightButtonUp;
            this.tvwSysMenu.MouseRightButtonUp += element_MouseRightButtonUp;
            this.tvwFlow.MouseRightButtonUp += element_MouseRightButtonUp;
            this.tvwForm.MouseRightButtonUp += element_MouseRightButtonUp;
            this.tbcLeft.SelectionChanged += TabControlTree_SelectionChanged;
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

                        imageLogo.Source = new BitmapImage(new Uri(string.Format("./Images/Icons/{0}/Icon.png", id), UriKind.Relative));

                        var brush = new ImageBrush(); //定义图片画刷
                        brush.ImageSource = new BitmapImage(new Uri(string.Format("./Images/Icons/{0}/Welcome.png", id), UriKind.Relative));
                        tbDesigner.Background = brush;
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

                // 首次加载时加载流程树和表单树，组织结构延后加载
                initLeftTree(new bool[] { true, true, false });

                // InitDesignerXml
                var designerService = Glo.GetDesignerServiceInstance();
                designerService.DoTypeAsync("InitDesignerXml", null, null, null, null, null);
                designerService.DoTypeCompleted += designer_DoTypeCompleted;

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
                            throw new System.Exception("@用户名或者SID错误, UserNo="+Glo.UserNo+" SID="+Glo.SID);
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
        void initLeftTree(params bool[] p)
        {
            this.SetSelectedTool("Arrow");
            _service = Glo.GetDesignerServiceInstance();
            _service.GetFlowDesignerTreeAsync(p);
            _service.GetFlowDesignerTreeCompleted += _service_GetFlowFormTreeCompleted;
        }
        void _service_GetFlowFormTreeCompleted(object sender, GetFlowDesignerTreeCompletedEventArgs e)
        {
            System.Exception exc = null;
            _service.GetFlowDesignerTreeCompleted -= _service_GetFlowFormTreeCompleted;
            if (e.Error != null)
            {
                Glo.ShowException(e.Error);
                return;
            }

            try
            {
                DataSet flowTree = new DataSet();
                flowTree.FromXml(e.Result);

                DataTable dtFlowSort = flowTree.Tables["WF_FlowSort"];
                if (dtFlowSort != null)
                    BindFlowAndFlowSort(dtFlowSort, flowTree.Tables["WF_Flow"]);

                DataTable dtFormSort = flowTree.Tables["Sys_FormTree"];
                if (dtFormSort != null)
                    BindFormTree(dtFormSort, flowTree.Tables["Sys_MapData"]);

                //特殊处理树解构.
                DataTable dtDept = flowTree.Tables["Port_Dept"];
                foreach (DataRow dr in dtDept.Rows)
                {
                    dr["No"] = "Dept" + dr["No"];
                    dr["ParentNo"] = "Dept" + dr["ParentNo"];
                }

                DataTable dtEmp = flowTree.Tables["Port_Emp"];
                foreach (DataRow dr in dtEmp.Rows)
                {
                    dr["FK_Dept"] = "Dept" + dr["FK_Dept"];
                }

                if (dtDept != null)
                    BingTreeDept(dtDept, dtEmp);

                this.SetSelectedTool("Arrow");
            }
            catch (System.Exception ee)
            {
                Glo.ShowException(ee);
            }
        }

        #region FlowTree
        internal void BindFlowAndFlowSort()
        {
            initLeftTree(new bool[] { true, false, false });
        }
        /// <summary>
        /// 绑定工作流树
        /// </summary>
        void BindFlowAndFlowSort(DataTable FlowSort, DataTable flow)
        {
            try
            {
                nodeFlow = new TreeNode();
                tvwFlow.Clear();

                //绑定FlowSort 根目录
                TreeNode rootNode = new TreeNode();
                foreach (DataRow dr in FlowSort.Rows)
                {
                    string parentNo = dr["ParentNo"];
                    if (string.IsNullOrEmpty(parentNo))
                        parentNo = "0";

                    if (parentNo != "0")
                        continue;

                    rootNode.Title = dr["Name"].ToString();

                    rootNode.ID = dr["No"].ToString();
                    //rootNode
                    rootNode.IsSort = true;
                    rootNode.isRoot = true;
                    rootNode.Icon = "../Images/MenuItem/FlowSort.png";
                    nodeFlow.Nodes.Add(rootNode);
                    break;
                }
                if (string.IsNullOrEmpty(rootNode.ID))
                {
                    throw new System.Exception("@绑定流程树出现错误,没有找到ParentNo=0的跟目录:");
                }

                //第二级目录
                foreach (DataRow dr in FlowSort.Rows)
                {
                    string parentNo = dr["ParentNo"];
                    if (parentNo != rootNode.ID.ToString())
                        continue;

                    TreeNode subNode = new TreeNode();
                    subNode.Title = dr["Name"].ToString();
                    subNode.ID = dr["No"].ToString();
                    subNode.IsSort = true;
                    subNode.Icon = "../Images/MenuItem/FlowSort.png";
                    nodeFlow.Nodes.Add(subNode);
                    rootNode.Nodes.Add(subNode);

                    AddSubTreeNode(subNode, FlowSort, TreeType.Flow);
                }
                tvwFlow.Nodes.Add(rootNode);
                rootNode.IsExpanded = true;
                rootNode.Expand();

                #region  绑定流程
                if (flow != null)
                    foreach (DataRow d in flow.Rows)
                    {
                        string tmp_FK_FlowSort = d["FK_FlowSort"];
                        var node = new TreeNode();
                        node.Title = d["No"].ToString() + "." + d["Name"].ToString();
                        node.ID = d["No"].ToString();
                        node.Icon = "../Images/MenuItem/EditTable4.png";
                        node.IsSort = false;
                        if (SelectedContainer != null)
                        {
                            if (SelectedContainer.FlowID == node.ID)
                            {
                                var te = this.tbDesigner.SelectedItem as TabItemEx;
                                te.Title = node.Title;
                                Canvas cs = te.Header as Canvas;
                                TextBlock tbx = cs.Children[1] as TextBlock;
                                tbx.Text = node.Title;
                            }
                        }
                        foreach (TreeNode ne in nodeFlow.Nodes)
                        {
                            if (ne.ID == tmp_FK_FlowSort)
                            {
                                ne.Nodes.Add(node);
                                ne.IsSort = true;
                            }
                        }
                    }
                #endregion

                // 完成绑定后，展开最后的FlowSort
                foreach (TreeNode node in tvwFlow.Nodes)
                {
                    if (node.ID == Glo.FK_FlowSort)
                    {
                        node.IsExpanded = true;
                        node.Expand();
                        Glo.FK_FlowSort = string.Empty;
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Glo.ShowException(ex, "绑定流程树时发生了错误");
            }

            this.SetSelectedTool("Arrow");
        }

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

            foreach (TabItem t in tbDesigner.Items)
            {
                var ct = t.Content as Container;
                if (ct != null && tvwFlow.Selected != null && ct.FlowID == tvwFlow.Selected.Name)
                {
                    tbDesigner.Items.Remove(t);
                    break;
                }
            }

            //TreeNode node = this.findNode(tvwFlow.Nodes[0] as TreeNode, this.flowNameToDel);
            //if (null != node && null != node.ParentNode)
            //    node.ParentNode.Nodes.Remove(node);
            //this.InvalidateArrange();

            this.flowNoToDel = string.Empty;
            this.BindFlowAndFlowSort();

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

                    this.BindFlowAndFlowSort();
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
            Glo.FK_FlowSort = tvwFlow.Selected.ID;
            var newFlow = new FrmNewFlow();
            newFlow.tabControl.TabIndex = tabIdx;

            if (tvwFlow.Selected != null && ((TreeNode)tvwFlow.Selected).IsSort)
            {
                newFlow.CurrentFlowSortNo = tvwFlow.Selected.ID;
            }
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
                            this.BindFlowAndFlowSort();
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

        #region FormTree

        internal void BindFormTree()
        {
            initLeftTree(new bool[] { false, true, false });
        }
        void BindFormTree(DataTable formSort, DataTable form)
        {
            try
            {
                //根目录
                TreeNode rootNode = new TreeNode();
                nodeForm = new TreeNode();
                foreach (DataRow dr in formSort.Rows)
                {
                    string parentNo = dr["ParentNo"];
                    if (string.IsNullOrEmpty(parentNo))
                        parentNo = "0";

                    if (parentNo != "0")
                        continue;

                    rootNode.Title = dr["Name"].ToString();
                    rootNode.ID = dr["No"].ToString();
                    rootNode.IsSort = true;
                    rootNode.isRoot = true;
                    rootNode.Icon = "../Images/MenuItem/FlowSort.png";

                    nodeForm.Nodes.Add(rootNode);
                    break;
                }
                if (string.IsNullOrEmpty(rootNode.ID))
                {
                    throw new System.Exception("@绑定流程树出现错误,没有找到ParentNo=0的跟目录:");
                }
                //rootNode.Title = "表单树";
                //rootNode.ID = "0";
                //rootNode.isRoot = true;
                //rootNode.IsSort = true;
                //rootNode.Icon = "../Images/MenuItem/FlowSheet.png";

                foreach (DataRow dr in formSort.Rows)
                {
                    string parentNo = dr["No"];
                    if (string.IsNullOrEmpty(parentNo))
                        parentNo = "0";

                    if (parentNo == "0")
                    {
                        rootNode.Title = dr["Name"].ToString();
                        rootNode.ID = dr["No"].ToString();
                        rootNode.IsSort = true;
                        rootNode.isRoot = true;
                        rootNode.Icon = "../Images/MenuItem/FlowSheet.png";
                        break;
                    }
                }

                //第二级目录
                foreach (DataRow dr in formSort.Rows)
                {
                    string parentNo = dr["ParentNo"];
                    if (parentNo != rootNode.ID.ToString())
                        continue;

                    var node = new TreeNode();
                    node.Title = dr["Name"].ToString();
                    node.ID = dr["No"].ToString();
                    node.Name = dr["No"].ToString();
                    node.IsSort = true;
                    node.Icon = "../Images/MenuItem/FlowSheet.png";

                    nodeForm.Nodes.Add(node);
                    rootNode.Nodes.Add(node);

                    //增加下级.
                    AddSubTreeNode(node, formSort, TreeType.MapData);
                }
                this.tvwForm.Clear();
                tvwForm.Nodes.Add(rootNode);
                rootNode.IsExpanded = true;
                rootNode.Expand();

                //绑定表单
                if (form != null)
                {
                    foreach (DataRow d in form.Rows)
                    {
                        var node = new TreeNode();
                        node.Title = d["Name"].ToString();
                        node.ID = d["No"].ToString();
                        node.Icon = "../Images/MenuItem/EditTable4.png";
                        node.IsSort = false;
                        foreach (TreeNode ne in nodeForm.Nodes)
                        {
                            if (ne.ID == d["FK_FormTree"].ToString())
                            {
                                ne.Nodes.Add(node);
                            }
                        }
                    }
                }

                // 完成绑定后，展开最后的FlowSort
                foreach (TreeNode node in this.tvwForm.Nodes)
                {
                    if (node.ID == BP.Glo.FK_FormSort)
                    {
                        node.IsExpanded = true;
                        node.Expand();
                        Glo.FK_FlowSort = string.Empty;
                        break;
                    }
                }
            }
            catch (System.Exception ee)
            {
                Glo.ShowException(ee, "加载表单树时发生了错误");
            }
        }

        /// 设置表单树快捷菜单
        /// <param name="isFormOrSort"> true: 启用表单菜单；false：启用类型菜单</param>
        private void EnableMenuFormTree(TreeNode node, Point menuPosition)
        {
            this.menuForm.Visibility = System.Windows.Visibility.Collapsed;
            this.menuFormSort.Visibility = System.Windows.Visibility.Collapsed;
            Liquid.Menu menu = null;
            double
                menuHeight = 235,
                menuWidth = 170;
            if (node.isRoot)
            {
                menu = this.menuFormSort;
                menuFormSort.Get("Frm_Delete").IsEnabled = false;
                menuFormSort.Get("Frm_Up").IsEnabled = false;
                menuFormSort.Get("Frm_Down").IsEnabled = false;
                menuFormSort.Get("Frm_NewSameLevelSort").IsEnabled = false;

                //menuFormSort.Get("Frm_NewForm").IsEnabled = true;
                //menuFormSort.Get("Frm_NewSubSort").IsEnabled = true;
                //menuFormSort.Get("Frm_EditSort").IsEnabled = true;
                //menuFormSort.Get("Frm_Refresh").IsEnabled = true;
            }
            else if (node.IsSort)
            {
                Glo.FK_FormSort = node.ID;

                menu = this.menuFormSort;
                menuFormSort.Get("Frm_Delete").IsEnabled = true;
                menuFormSort.Get("Frm_Up").IsEnabled = true;
                menuFormSort.Get("Frm_Down").IsEnabled = true;
                menuFormSort.Get("Frm_NewSameLevelSort").IsEnabled = true;
                //menuFormSort.Get("Frm_NewForm").IsEnabled = true;
                //menuFormSort.Get("Frm_NewSubSort").IsEnabled = true;
                //menuFormSort.Get("Frm_EditSort").IsEnabled = true;
                //menuFormSort.Get("Frm_Refresh").IsEnabled = true;
            }
            else
            {
                menuHeight = 205;
                menu = this.menuForm;
                menuForm.Get("Frm_EditForm").IsEnabled = true;

                //menuForm.Get("Frm_FormDesignerFix").IsEnabled = true;

                menuForm.Get("Frm_FormDesignerFree").IsEnabled = true;
                menuForm.Get("Frm_Delete").IsEnabled = true;
                menuForm.Get("Frm_Refresh").IsEnabled = true;

            }

            double x = menuPosition.X;
            double y = menuPosition.Y;
            if (x + menuWidth > 220)
            {
                x = x - (x + menuWidth - 220);
            }
            if (y + menuHeight > Application.Current.Host.Content.ActualHeight)
            {
                y = y - (y + menuHeight - Application.Current.Host.Content.ActualHeight);
            }

            menu.SetValue(Canvas.LeftProperty, x);
            menu.SetValue(Canvas.TopProperty, y);
            menu.Visibility = System.Windows.Visibility.Visible;
            menu.Show();
        }

        /// <summary>
        /// 表单类别关闭时要执行的动作，一般来说是刷新窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void frmSameLevelSort_ServiceDoCompletedEvent(object sender, EventArgs e)
        {
            var add = (FrmSortEdit)sender;
            if (add.DialogResult == true)
            {
                this.BindFormTree();
            }
        }

        // 添加流程类别关闭时要执行的动作，一般来说是刷新窗体
        void AddEditFlowSortDoCompletedEventHandler(object sender, EventArgs e)
        {
            var add = (NewFlowSort)sender;
            if (add.DialogResult == true)
            {
                this.BindFlowAndFlowSort();
            }
        }
        void EditFrmSortDoCompletedEventHandler(object sender, EventArgs e)
        {
            switch (Glo.TempCmd)
            {
                case "Frm_EditForm":
                case "Frm_NewForm":
                    var frm = (BP.Frm.Frm)sender;
                    if (frm.DialogResult == true)
                    {
                        this.BindFormTree();
                    }
                    break;
                case "Frm_NewFormSort":
                case "Frm_EditFormSort":
                    var add = (BP.Frm.FrmSortEdit)sender;
                    if (add.DialogResult == true)
                    {
                        this.BindFormTree();
                    }
                    break;
                default:
                    MessageBox.Show("未判断的标记:" + Glo.TempCmd);
                    break;
            }
        }
        #endregion

        #region GPMTree
        bool isGMPTreeInited;
        public void BingTreeDept()
        {
            this.SetSelectedTool("Wait");
            initLeftTree(new bool[] { false, false, true });
        }

        void BingTreeDept(DataTable dtDept, DataTable dtEmp)
        {
            try
            {
                nodeGpm.Nodes.Clear();

                TreeNode rootNode = new TreeNode();
                string rootId = string.Empty;
                foreach (DataRow dr in dtDept.Rows)
                {   //构造根节点 

                    string parentNo = dr["ParentNo"];
                    if (parentNo != "Dept0")
                        continue;

                    rootId = dr["No"];
                    rootNode.isRoot = true;
                    rootNode.Icon = "../Images/TreeMenu/Org.png";
                    rootNode.Title = dr["Name"];
                    rootNode.Name = rootId;
                    rootNode.ID = rootId;

                    nodeGpm.Nodes.Add(rootNode);

                }

                if (string.IsNullOrEmpty(rootId)) return;
                // 二级目录
                foreach (DataRow dr in dtDept.Rows)
                {
                    string parentNo = dr["ParentNo"];
                    if (!rootId.Equals(parentNo))
                        continue;

                    string subId = dr["No"];
                    TreeNode subNode = new TreeNode();
                    subNode.Title = dr["Name"];
                    subNode.Name = subId;
                    subNode.ID = subId;
                    subNode.Icon = "../Images/MenuItem/Depts.png";
                    subNode.isDept = true;
                    rootNode.Nodes.Add(subNode);
                    nodeGpm.Nodes.Add(subNode);

                    AddSubTreeNode(subNode, dtDept, TreeType.GPM);
                }

                this.tvwOrg.Clear();
                this.tvwOrg.Nodes.Add(rootNode);
                rootNode.IsExpanded = true;
                rootNode.Expand();

                if (dtEmp != null)
                    foreach (var dr in dtEmp.Rows)
                    {
                        foreach (TreeNode ne in this.nodeGpm.Nodes)
                            if (ne.ID == dr["FK_Dept"])
                            {
                                string no = dr["No"];
                                TreeNode nodeEmp = new TreeNode();
                                nodeEmp.Title = dr["Name"];
                                nodeEmp.Name = no;
                                nodeEmp.ID = no;
                                nodeEmp.Icon = "../Images/MenuItem/People.png";
                                ne.Nodes.Add(nodeEmp);
                                continue;
                            }
                    }

                isGMPTreeInited = true;
            }
            catch (System.Exception ee)
            {
                Glo.ShowException(ee);
            }
            this.SetSelectedTool("Arrow");

        }

        void tvwOrg_NodeDoubleClick(object sender, TreeEventArgs e)
        {
            //左键按下 获得deptNo
            TreeNode node = sender as TreeNode;
            if (node == null)
                return;

            Glo.CurNodeOrg = node;
            string url = string.Empty;

            if (node.isRoot)
            {
                return;
                //if (Glo.OsModel == BP.Sys.OSModel.OneMore)
                //{
                //    url = "/WF/Comm/Search.aspx?EnsName=BP.GPM.Depts&PK={0}&No={0}";
                //}
                //else
                //{
                //    url = "/WF/Comm/Search.aspx?EnsName=BP.Port.Depts&FK={0}&No={0}";
                //}

                //url = string.Format(url, node.ID);
                //Glo.OpenWindow(url, "部门信息");
            }
            if (node.isDept)
            {
                MessageBox.Show("部门信息不提供维护功能，仅仅提供查看。\t\n如果您使用了ccbpm的权限管理系统，请使用它来维护。\\如果您集成了自己的组织架构，请使用自己的组织架构来维护。", "提示", MessageBoxButton.OK);
                return;

                if (Glo.OsModel == OSModel.OneMore)
                {
                    url = "/WF/Comm/Search.aspx?EnsName=BP.GPM.Depts&FK_Dept={0}&No={0}";
                }
                else
                {
                    url = "/WF/Comm/Search.aspx?EnsName=BP.Port.Depts&FK_Dept={0}&No={0}";
                }
                url = url.Replace("=Dept", "=");
                url = string.Format(url, node.ID);
                Glo.OpenWindow(url, "部门信息");
            }
            else
            {
                MessageBox.Show("人员信息不提供维护功能，仅仅提供查看。\t\n如果您使用了ccbpm的权限管理系统，请使用它来维护。\\如果您集成了自己的组织架构，请使用自己的组织架构来维护。", "提示", MessageBoxButton.OK);
                return;

                if (Glo.OsModel == OSModel.OneMore)
                {
                    url = "/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.GPM.Emps&PK={0}";
                }
                else
                {
                    url = "/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.Port.Emps&PK={0}";
                }
                url = string.Format(url, node.ID);
                Glo.OpenWindow(url, "员工信息");

                //OnTreeNodeDBClick();  //读入相关的数据      

            }
        }

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

        /// <summary>
        /// 当打开一个树节点时
        /// </summary>
        /// <param name="deptNo"></param>
        public void OnTreeNodeDBClick()
        {
            if (Glo.CurNodeOrg == null || string.IsNullOrEmpty(Glo.CurNodeOrg.ID))
                return;
            //BindEmpByDeptNo(Glo.CurNodeOrg);
        }
        private void BindEmpByDeptNo(TreeNode nodeDept)
        {
            EventHandler<RunSQLReturnTableCompletedEventArgs> handler = null;
            handler = (object sender, RunSQLReturnTableCompletedEventArgs e) =>
            {
                _service.RunSQLReturnTableCompleted -= handler;
                if (null != e.Error)
                {
                    Glo.ShowException(e.Error, "组织结构数据访问失败");
                    return;
                }

                DataSet ds = new DataSet();
                ds.FromXml(e.Result);
                DataTable dtEmp = ds.Tables[0]; //人员信息。

                foreach (var emp in dtEmp.Rows)
                {
                    string noEmp = emp["No"];
                    string fk_Dept = emp["FK_Dept"];
                    if (nodeDept.ID.Equals(fk_Dept))
                    {
                        TreeNode nodeEmp = new TreeNode();
                        nodeEmp.Title = emp["Name"];
                        nodeEmp.Name = noEmp;
                        nodeEmp.ID = noEmp;
                        nodeDept.Icon = "../Images/MenuItem/people.png";
                        bool find = false;
                        for (int i = 0; i < nodeDept.Nodes.Count; i++)
                        {
                            if (nodeDept.Nodes[i].ID == noEmp)
                            {
                                nodeDept.Nodes[i] = nodeEmp;
                                find = true;
                            }
                        }
                        if (!find)
                            nodeDept.Nodes.Add(nodeEmp);

                    }
                }
            };

            string sql = "SELECT No,Name,FK_Dept FROM Port_Emp WHERE FK_Dept ='{0}' ";
            sql = string.Format(sql, nodeDept.ID);
            _Service = null;
            _Service = Glo.GetDesignerServiceInstance();
            _service.RunSQLReturnTableCompleted += handler;
            _service.RunSQLReturnTableAsync(sql, Glo.UserNo, Glo.SID);
        }

        void setGMPMenu(TreeNode nodeOrg)
        {
            bool isEnabled;
            Glo.CurNodeOrg = nodeOrg;

            if (nodeOrg.isRoot)
            {
                isEnabled = false;
                menuOrg.Get("Dept_CrateSameLevel").IsEnabled = isEnabled;
                menuOrg.Get("Dept_CrateSubLevel").IsEnabled = isEnabled;
                menuOrg.Get("Dept_Refresh").IsEnabled = isEnabled;
                menuOrg.Get("Dept_Edit").IsEnabled = isEnabled;

                menuOrg.Get("Dept_Delete").IsEnabled = isEnabled;
                menuOrg.Get("Emp_Edit").IsEnabled = isEnabled;
                menuOrg.Get("Emp_Add").IsEnabled = isEnabled;
                menuOrg.Get("Emp_Related").IsEnabled = isEnabled;
                menuOrg.Get("Btn_Up").IsEnabled = isEnabled;
                menuOrg.Get("Btn_Down").IsEnabled = isEnabled;
            }
            else if (nodeOrg.isDept)
            {
                isEnabled = true;
                menuOrg.Get("Dept_Delete").IsEnabled = isEnabled;
                menuOrg.Get("Dept_Edit").IsEnabled = isEnabled;
                menuOrg.Get("Dept_CrateSameLevel").IsEnabled = isEnabled;
                menuOrg.Get("Dept_CrateSubLevel").IsEnabled = isEnabled;
                menuOrg.Get("Dept_Refresh").IsEnabled = isEnabled;
                menuOrg.Get("Btn_Up").IsEnabled = isEnabled;
                menuOrg.Get("Btn_Down").IsEnabled = isEnabled;

                isEnabled = !isEnabled;
                menuOrg.Get("Emp_Edit").IsEnabled = isEnabled;
                menuOrg.Get("Emp_Add").IsEnabled = isEnabled;
                menuOrg.Get("Emp_Related").IsEnabled = isEnabled;
            }
            else
            {
                isEnabled = false;
                menuOrg.Get("Dept_Delete").IsEnabled = isEnabled;
                menuOrg.Get("Dept_Edit").IsEnabled = isEnabled;
                menuOrg.Get("Dept_CrateSameLevel").IsEnabled = isEnabled;
                menuOrg.Get("Dept_CrateSubLevel").IsEnabled = isEnabled;
                menuOrg.Get("Dept_Refresh").IsEnabled = isEnabled;
                menuOrg.Get("Btn_Up").IsEnabled = isEnabled;
                menuOrg.Get("Btn_Down").IsEnabled = isEnabled;

                isEnabled = !isEnabled;
                menuOrg.Get("Emp_Edit").IsEnabled = isEnabled;
                menuOrg.Get("Emp_Add").IsEnabled = isEnabled;
                menuOrg.Get("Emp_Related").IsEnabled = isEnabled;
            }
        }

        private void menu_ItemOrg_Click(object sender, MenuEventArgs e)
        {
            TreeNode td = tvwOrg.Selected as TreeNode;
            if (td == null)
                return;

            Glo.CurNodeOrg = td;

            string deptNo = td.ID.ToString();


        }
        /// <summary>
        /// 刷新人员列表
        /// </summary>
        void OrgRefresh()
        {
            if (Glo.CurNodeOrg != null || string.IsNullOrEmpty(Glo.CurNodeOrg.ID)) return;
            OnTreeNodeDBClick();  //读入相关的数据
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
                        string url3 = "";
                        if (Glo.Platform == BP.Platform.JFlow)
                        {
                            url3 = @"/WF/Login.jsp";
                        }
                        else
                        {
                            url3 = @"/WF/App/Classic/Login.aspx?DoType=Logout";
                        }
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
                    case "Btn_ToolBarEditFlowNew":
                        Glo.OpenWinByDoType("CH", "FlowPNew", SelectedContainer.FlowID, null, null);
                        break;
                    case "Btn_ToolBarDeleteFlow":
                        DeleteFlow(SelectedContainer.FlowID);
                        break;
                    case "ToolBarSystem":
                        Glo.OpenGPM();
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

                    case "Btn_ToolBarReleaseToFTP":
                        ReleaseToFtp();
                        break;
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
        void ReleaseToFtp()
        {
            if (SelectedContainer == null || SelectedContainer.FlowID != tvwFlow.Selected.Name)
            {
                MessageBox.Show("请先打开流程图");
                return;
            }

            //  FrmFlowShareToFtp ftp = new FrmFlowShareToFtp(SelectedContainer.FlowID);
            // ftp.Show();
        }

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

        private void element_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            Tree treeView = null;
            TreeNode node = null;
            if (sender is Tree)
            {
                treeView = sender as Tree;

                if (null == treeView)
                    return;

                if (treeView == this.tvwSysMenu)
                {
                    return;
                }

                TextBlock tb = e.OriginalSource as TextBlock;
                if (tb != null && tb is DependencyObject)
                {
                    node = Glo.GetParentObject<TreeNode>(tb);
                }

                if (null == node)
                    return;
                else
                {
                    node.IsSelected = true;
                    treeView.Selected = node;
                }
            }
            else //// diable the default silverlight rightmenu
                return;

            //取得当前菜单位置坐标 
            Point position = e.GetPosition(treeView);
            Point menuPosition = treeView.TransformToVisual(treeView).Transform(position);

            switch (treeView.Name)
            {
                case "tvwFlow":
                    #region

                    // 调整x,y 值 ，以防止菜单被遮盖住
                    var x = menuPosition.X;
                    var y = menuPosition.Y;
                    var menuHeight = 380;
                    var menuWidth = 170;
                    if (x + menuWidth > 220)
                    {
                        x = x - (x + menuWidth - 220);
                    }
                    if (y + menuHeight > Application.Current.Host.Content.ActualHeight)
                    {
                        y = y - (y + menuHeight - Application.Current.Host.Content.ActualHeight);
                    }
                    MuFlowTree.SetValue(Canvas.LeftProperty, x);
                    MuFlowTree.SetValue(Canvas.TopProperty, y);
                    MuFlowTree.Show();

                    if (node.isRoot)
                    {
                        MuFlowTree.Get("OpenFlow").IsEnabled = false;
                        MuFlowTree.Get("NewFlow_Blank").IsEnabled = true;
                        MuFlowTree.Get("NewSameLevelFlowSort").IsEnabled = false;
                        MuFlowTree.Get("NewSubFlowSort").IsEnabled = true;
                        MuFlowTree.Get("Delete").IsEnabled = false;
                        MuFlowTree.Get("Edit").IsEnabled = true;
                    }
                    else if (node.IsSort)
                    {
                        Glo.FK_FlowSort = node.ID;
                        MuFlowTree.Get("OpenFlow").IsEnabled = false;
                        MuFlowTree.Get("NewFlow_Blank").IsEnabled = true;
                        MuFlowTree.Get("NewSameLevelFlowSort").IsEnabled = true;
                        MuFlowTree.Get("NewSubFlowSort").IsEnabled = true;
                        MuFlowTree.Get("Delete").IsEnabled = true;
                        MuFlowTree.Get("Edit").IsEnabled = true;
                    }
                    else
                    {
                        MuFlowTree.Get("OpenFlow").IsEnabled = true;
                        MuFlowTree.Get("NewFlow_Blank").IsEnabled = true;
                        MuFlowTree.Get("NewSameLevelFlowSort").IsEnabled = false;
                        MuFlowTree.Get("NewSubFlowSort").IsEnabled = false;
                        MuFlowTree.Get("Edit").IsEnabled = false;
                        MuFlowTree.Get("Delete").IsEnabled = true;
                    }
                    #endregion
                    break;
                case "tvwForm":
                    #region



                    EnableMenuFormTree(node, menuPosition);

                    #endregion
                    break;
                case "tvwOrg":

                    setGMPMenu(node);

                    // 调整x,y 值 ，以防止菜单被遮盖住
                    x = menuPosition.X;
                    y = menuPosition.Y;
                    menuHeight = 230;
                    menuWidth = 170;
                    if (x + menuWidth > 220)
                    {
                        x = x - (x + menuWidth - 220);
                    }
                    if (y + menuHeight > Application.Current.Host.Content.ActualHeight)
                    {
                        y = y - (y + menuHeight - Application.Current.Host.Content.ActualHeight);
                    }
                    //定位右键菜单 
                    menuOrg.Margin = new Thickness(x, y, 0, 0);
                    //显示右键菜单 
                    menuOrg.Show();
                    break;
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
                        case "OpenFlow":
                            OpenFlow(tvwFlow.Selected.ID, tvwFlow.Selected.ID, tvwFlow.Selected.Title);
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
                        case "NewSameLevelFlowSort"://新建同级流程类别
                            var newFlowSort = new NewFlowSort(this);
                            newFlowSort.InitControl(tvwFlow.Selected.ID, "");
                            newFlowSort.DisplayType = NewFlowSort.DisplayTypeEnum.AddSameLevel;
                            newFlowSort.ServiceDoCompletedEvent += AddEditFlowSortDoCompletedEventHandler;
                            newFlowSort.Show();
                            break;
                        case "NewSubFlowSort"://新建下级流程类别
                            var subFlowSort = new NewFlowSort(this);
                            subFlowSort.InitControl(tvwFlow.Selected.ID, "");
                            subFlowSort.DisplayType = NewFlowSort.DisplayTypeEnum.AddSub;
                            subFlowSort.ServiceDoCompletedEvent += AddEditFlowSortDoCompletedEventHandler;
                            subFlowSort.Show();
                            break;
                        case "Edit":
                            Glo.FK_FlowSort = tvwFlow.Selected.ID;
                            var editFlowSort = new NewFlowSort(this);
                            editFlowSort.InitControl(tvwFlow.Selected.ID, tvwFlow.Selected.EditedTitle);
                            editFlowSort.DisplayType = NewFlowSort.DisplayTypeEnum.Edit;
                            editFlowSort.ServiceDoCompletedEvent += AddEditFlowSortDoCompletedEventHandler;
                            editFlowSort.Show();
                            break;
                        case "Delete":
                            var deleteFlowNode = tvwFlow.Selected as TreeNode;
                            if (null == deleteFlowNode)
                                break;

                            if (!deleteFlowNode.IsSort)
                            {
                                Glo.FK_FlowSort = tvwFlow.Selected.ID;
                                DeleteFlow(tvwFlow.Selected.ID);
                            }
                            else
                                DeleteFlowSort(deleteFlowNode.ID);

                            break;
                        case "Share":  //分配权限..
                            var dir = tvwFlow.Selected as TreeNode;
                            if (null == dir)
                            {
                                break;
                            }

                            if (dir.IsSort == false)
                            {
                                MessageBox.Show("请选择流程树，您才能分配权限。", "提示", MessageBoxButton.OK);
                                return;
                            }

                            Glo.OpenWinByDoType("CH", UrlFlag.FlowSortP, Glo.FK_Flow, dir.ID, null);
                            break;

                        case "Refresh":
                            this.BindFlowAndFlowSort();
                            break;
                    }

                    #endregion
                    break;
                case "menuForm":
                case "menuFormSort":
                    #region
                    var selectedNode = this.tvwForm.Selected as TreeNode;
                    if (selectedNode == null)
                        return;

                    Glo.TempCmd = e.Tag.ToString();
                    switch (e.Tag.ToString())
                    {
                        case "Frm_Up": //上移
                            var ws = Glo.GetDesignerServiceInstance();
                            if (BP.DA.DataType.IsNum(selectedNode.ID))
                                Glo.TempCmd = "FrmTreeUp";
                            else
                                Glo.TempCmd = "FrmUp";
                            ws.DoTypeAsync(Glo.TempCmd, selectedNode.ID, null, null, null, null);
                            ws.DoTypeCompleted += designer_DoTypeCompleted;
                            break;
                        case "Frm_Down": //上移
                            var wsDown = Glo.GetDesignerServiceInstance();

                            if (BP.DA.DataType.IsNum(selectedNode.ID))
                                Glo.TempCmd = "FrmTreeDown";
                            else
                                Glo.TempCmd = "FrmDown";
                            wsDown.DoTypeAsync(Glo.TempCmd, selectedNode.ID, null, null, null, null);
                            wsDown.DoTypeCompleted += designer_DoTypeCompleted;
                            break;
                        case "Frm_EditForm": //表单属性.
                            //string url = "/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.MapDataExts&PK=" + selectedNode.ID;
                            string url = "/WF/Admin/CCFormDesigner/FrmAttr.aspx?FrmID=" + selectedNode.ID;
                            Glo.OpenDialog(url, "表单属性");
                            return;
                        case "Frm_NewForm": //新建表单
                            BP.Frm.Frm frm1 = new BP.Frm.Frm();
                            frm1.SortNo = Glo.FK_FormSort;
                            frm1.BindNew();
                            frm1.HisMainPage = this;
                            frm1.Show();
                            break;
                        case "Frm_FormDesignerFix": //设计傻瓜表单
                            Glo.OpenWinByDoType("CH", UrlFlag.FormFixModel, selectedNode.ID, null, null);
                            break;
                        case "Frm_FormDesignerFree": //设计free表单
                            string title = "表单ID: {0} 存储表:{1} 名称:{2}";
                            title = string.Format(title, selectedNode.ID, "Sys_MapData", selectedNode.Title);
                            OpenBPForm(BPFormType.FormFlow, title, selectedNode.ID, selectedNode.ID);
                            break;
                        case "Help":
                            Glo.OpenHelp();
                            break;
                        case "Frm_NewSameLevelSort": //新建同级表单类别
                            BP.Frm.FrmSortEdit frmSameLevelSort = new BP.Frm.FrmSortEdit();
                            frmSameLevelSort.No = selectedNode.ID;
                            frmSameLevelSort.TB_Name.Text = "New Form Sort";
                            frmSameLevelSort.ServiceDoCompletedEvent += new EventHandler<DoCompletedEventArgs>(frmSameLevelSort_ServiceDoCompletedEvent);
                            frmSameLevelSort.DisplayType = FrmSortEdit.DisplayTypeEnum.AddSameLevel;
                            frmSameLevelSort.Show();
                            break;
                        case "Frm_NewSubSort"://新建下级表单类别
                            BP.Frm.FrmSortEdit frmSubLevelSort = new BP.Frm.FrmSortEdit();
                            frmSubLevelSort.No = selectedNode.ID;
                            frmSubLevelSort.TB_Name.Text = "New Form Sort";
                            frmSubLevelSort.ServiceDoCompletedEvent += new EventHandler<DoCompletedEventArgs>(frmSameLevelSort_ServiceDoCompletedEvent);
                            frmSubLevelSort.DisplayType = FrmSortEdit.DisplayTypeEnum.AddSub;
                            frmSubLevelSort.Show();
                            break;
                        case "Frm_EditSort": //编辑
                            BP.Frm.FrmSortEdit frmSortEdit1 = new BP.Frm.FrmSortEdit();
                            frmSortEdit1.No = selectedNode.ID;
                            frmSortEdit1.TB_Name.Text = selectedNode.Title;
                            frmSortEdit1.ServiceSaveEnCompletedEvent += new EventHandler<SaveEnCompletedEventArgs>(frmSameLevelSort_ServiceDoCompletedEvent);
                            frmSortEdit1.DisplayType = FrmSortEdit.DisplayTypeEnum.Edit;
                            frmSortEdit1.Show();
                            break;
                        case "Frm_Delete": //删除
                            var deleteFlowNode = this.tvwForm.Selected as TreeNode;
                            if (null == deleteFlowNode)
                                break;

                            if (deleteFlowNode.isRoot)
                            {
                                MessageBox.Show("根节点不允许删除。");
                                return;
                            }

                            if (MessageBox.Show("您确认要删除吗？", "ccflow", MessageBoxButton.OKCancel)
                                == MessageBoxResult.Cancel)
                                return;

                            if (deleteFlowNode.IsSort == true)
                            {
                                if (deleteFlowNode.Nodes.Count > 0)
                                {
                                    if (MessageBox.Show("选择项含有子节点，将子节点一起删除吗？", "ccflow", MessageBoxButton.OKCancel)
                                == MessageBoxResult.Cancel)
                                        return;
                                }
                                var ws1 = Glo.GetDesignerServiceInstance();
                                Glo.TempCmd = "DeleteFrmSort";
                                ws1.DoTypeAsync(Glo.TempCmd, deleteFlowNode.ID, null, null, null, null);
                                ws1.DoTypeCompleted += designer_DoTypeCompleted;
                            }
                            else
                            {
                                var ws2 = Glo.GetDesignerServiceInstance();
                                Glo.TempCmd = "DeleteFrm";
                                ws2.DoTypeAsync(Glo.TempCmd, deleteFlowNode.ID, null, null, null, null);
                                ws2.DoTypeCompleted += designer_DoTypeCompleted;

                            }
                            break;
                        case "Frm_Refresh": //刷新
                            this.BindFormTree();
                            break;
                        case "Frm_Imp":  //Imp
                            BP.Frm.FrmImp imp = new BP.Frm.FrmImp();
                            imp.Show();
                            break;
                        case "Frm_FormSln":
                            Glo.OpenDialog("/WF/MapDef/Sln.aspx?FK_MapData=" + selectedNode.ID, "表单权限方案");
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
                case "menuOrg":
                    menu_ItemOrg_Click(sender, e);
                    break;
            }

        }
        private void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            Liquid.Menu menu = sender as Liquid.Menu;
            menu.Hide();
        }
        void designer_DoTypeCompleted(object sender, DoTypeCompletedEventArgs e)
        {
            if (null != e.Error)
            {
                Glo.ShowException(e.Error);
                return;
            }
            switch (Glo.TempCmd)
            {
                case "DeleteFrmSort":
                case "DeleteFrm":
                case "FrmUp":   //上移动.
                case "FrmDown": //下移动.
                    if (e.Result != null)
                        MessageBox.Show(e.Result, "Error", MessageBoxButton.OK);
                    else
                        this.BindFormTree();
                    return;
                default:
                    break;
            }

            //DataSet ds = new DataSet();
            //ds.FromXml(e.Result);
            //bindTreeFlowData(ds.Tables[0]);
            //bindTreeSys(ds.Tables[1]);

        }

        private void TabControlTree_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            TabItem tbItem = (sender as TabControl).SelectedItem as TabItem;
            if (null == tbItem) return;
            string name = tbItem.Name;
            //没有验证不允许操作加载
            if (SessionManager.Session.Count == 0 || string.IsNullOrEmpty(SessionManager.Session["UserNo"])) return;

            switch (tbItem.Name)
            {
                case "tbiFlowLibrary"://流程树
                    break;
                case "tbiFormLibrary"://表单库
                    break;
                case "tbiOrg"://流程优化
                    if (!isGMPTreeInited)
                        this.BingTreeDept();
                    break;
                case "tbiSysManger"://系统维护
                    if (Glo.Platform == BP.Platform.CCFlow)
                    {
                        MessageBox.Show("我们已经取消了ccflow对GPM的依赖,系统的维护请转到H5版本的流程设计器找到系统维护标签页.", 
                            "提示", MessageBoxButton.OK);
                       // Glo.OpenWindow("/GPM/Default.aspx?RefNo=CCFlowBPM", "系统维护");
                      //  MessageBox.Show("JFlow的系统维护尚未提供", "提示", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("JFlow的系统维护尚未提供", "提示", MessageBoxButton.OK);
                        //Glo.OpenWindow("/WF/GPM/Default.jsp?RefNo=CCFlowBPM", "系统维护");
                    }
                    this.tbcLeft.SelectedIndex = 0;
                    break;

            }
        }

        void bindTreeSys(DataTable dt)
        {

            this.tvwSysMenu.Nodes.Clear();
            TreeNode liP = new TreeNode();
            foreach (DataRow dr in dt.Rows)
            {
                string no = dr["No"];
                string name = dr["Name"];
                string lab = dr["CH"];
                string w = dr["W"];
                string h = dr["H"];
                string url = dr["Url"];
                string icon = dr["Icon"];

                TreeNode tvwNodeSysMenu = new TreeNode();
                if (string.IsNullOrEmpty(icon) == false)
                    tvwNodeSysMenu.Icon = icon;

                tvwNodeSysMenu.Title = lab;
                tvwNodeSysMenu.Tag = dr;
                if (no.Length == 2)
                {
                    liP = tvwNodeSysMenu;
                    this.tvwSysMenu.Nodes.Add(tvwNodeSysMenu);
                }
                else
                {
                    liP.Nodes.Add(tvwNodeSysMenu);
                }
            }

            tvwSysMenu.ExpandAll();

            this.tvwSysMenu.MouseLeftButtonDown += (object sender, MouseButtonEventArgs e) =>
            {
                #region
                Tree tree = sender as Tree;
                TreeNode node = null;
                if (null != tree)
                    node = tree.Selected as TreeNode;
                else return;

                if (null == node) return;

                if (tree == this.tvwSysMenu)
                {
                    DataRow dr = node.Tag as DataRow;
                    string w = dr["W"];
                    string h = dr["H"];
                    string url = dr["Url"];

                    Glo.OpenWindow(Glo.BPMHost + url, "name", int.Parse(h), int.Parse(w));
                };
                #endregion
            };
        }
        void bindTreeFlowData(DataTable dt)
        {
            //this.tvwFlowDataEnum.Nodes.Clear();
            //liP = new TreeNode();
            //foreach (DataRow dr in dt.Rows)
            //{
            //    string no = dr["No"];
            //    string name = dr["Name"];
            //    string lab = dr["CH"];
            //    string w = dr["W"];
            //    string h = dr["H"];
            //    string url = dr["Url"];
            //    string icon = dr["Icon"];
            //    TreeNode tvwNodeFlow = new TreeNode();
            //    if (string.IsNullOrEmpty(icon) == false)
            //        tvwNodeFlow.Icon = icon;

            //    tvwNodeFlow.Title = lab;
            //    tvwNodeFlow.Tag = dr;
            //    if (no.Length == 2)
            //    {
            //        liP = tvwNodeFlow;
            //        this.tvwFlowDataEnum.Nodes.Add(tvwNodeFlow);
            //    }
            //    else
            //    {
            //        liP.Nodes.Add(tvwNodeFlow);
            //    }
            //}
            //tvwFlowDataEnum.ExpandAll();
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

            tbcLeft.Height = LayoutRoot.Height - 70;//imageLogo.Height  margion

            var height = tbcLeft.Height - 35;// tabcontrol.Height
            tvwFlow.Height = height;
            tvwForm.Height = height;
            this.tvwOrg.Height = height;
            tvwSysMenu.Height = height;

            this.flowToolBoxIcon.lbTools.Height = height - 20;

            tbDesigner.Height = LayoutRoot.Height - 35;// toolBar.Height
            tbDesigner.Width = LayoutRoot.ActualWidth - 285;// tbcLeft.ActualWidth + magrion
        }

    }
}