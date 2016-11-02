#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BP.En;
using BP.SL;
using BP.Sys.SL;
using Liquid;
using Silverlight;
#endregion

namespace CCForm
{
    public delegate void CCBPFormClosed();
    public delegate void CCBPFormLoaded();
    public partial class MainPage : UserControl
    {
        static MainPage instance = null;
        public static MainPage Instance
        {
            get
            {
                if (instance == null)
                    instance = new MainPage();
                return MainPage.instance;
            }
            set
            {
                MainPage.instance = value;
            }
        }
        public event CCBPFormClosed Closed;
        /// <summary>
        /// 如果是从CCFlowDesigner应用进入到本页面，当表单加载完后需要回调更新调用方
        /// </summary>
        public event CCBPFormLoaded CCBPFormLoaded;

        #region 处理移动的变量

        private Point pFrom;  /* 鼠标点击的位置   */
        public static Rectangle RectSelected;   /*  选择的区域  */

        private enum StateRectangleSelected { SelectBegin, SelectComplete, SelectMoved, SelectDisposed }
        StateRectangleSelected selectState = StateRectangleSelected.SelectDisposed;
        private bool isDrawingLine;// selectedType =  Line ，leftDown-->leftUp……selected-->  selectedType !=  Line ，
        private bool isToolDraging;
        private List<FrameworkElement> selectedElements = new List<FrameworkElement>();

        #endregion

        #region 全局变量
        LoadingWindow loadingWindow = new LoadingWindow();
        public SelectM2M winSelectM2M = new SelectM2M();
        public FlowFrm winFlowFrm = new FlowFrm();
        public FrmLink winFrmLink = new FrmLink();
        public FrmLab winFrmLab = new FrmLab();
        public SelectTB winSelectTB = new SelectTB();
        public SelectDDLTable winSelectDDL = new SelectDDLTable();
        public SelectRB winSelectRB = new SelectRB();
        public FrmImp winFrmImp = new FrmImp();
        public FrmBtn winFrmBtn = new FrmBtn();

        public FrmOp winFrmOp = new FrmOp();
        public FrmImg winFrmImg = new FrmImg();
        public FrmImgAth winFrmImgAth = new FrmImgAth();
        public FrmImgSeal winFrmImgSeal = new FrmImgSeal();
        public FrmEle winFrmEle = new FrmEle();
        public FrmEleMapPin winFrmEleMapPin = new FrmEleMapPin();
        public FrmEleMicHot winFrmEleMicHot = new FrmEleMicHot();

        public NodeFrms winNodeFrms = new NodeFrms();
        public SelectAttachment winSelectAttachment = new SelectAttachment();
        public FrmAttachmentM winFrmAttachmentM = new FrmAttachmentM();
        public bool IsRB;


        string selectType = ToolBox.Mouse; // 当前工具选择类型 hand line1 line2 label txt cannel

        BPLabel currLab; //当前 label
        BPLink currLink;  //当前 linke
        BPLine currLine;  //当前 Line



        private DataTemplate cursor;// customerCursor
        private CustomCursor cCursor = null;// which is used to replace the default cursor of UIElement

        // 元素编辑窗体名称
        const string NameImg = "NameImg",
               NameImgAth = "NameImgAth",
               NameImgSeal = "NameImgSeal",
               NameWorkCheck = "NameWorkCheck",
               NameLab = "NameLab",
               NameM2M = "NameM2M",
               NameLink = "NameLink",
               NameTB = "NameTB",
               NameDDL = "NameDDL",
               NameRB = "NameRB",
               NameImp = "NameImp",
               NameEle = "NameEle",
               NameEleMapPin = "NameEleMapPin",
               NameEleMicHot = "NameEleMicHot",
               NameAttachment = "NameAttachment",
               NameOp = "NameOP",
               NameBtn = "NameBtn",
               NameFlowFrm = "NameFlowFrm",
               NameNodeFrms = "NameNodeFrms",
               NameAttachmentM = "NameAttachmentM";
        #endregion 全局变量

        #region 初始化加载
        /// <summary>
        /// 标志本应用调用方 true: 由web调用，false： 由FlowDesigner调用
        /// </summary>
        bool LoadSource;
        void Content_Resized(object sender, EventArgs e)
        {
            if (!LoadSource)
            {
            }
            else
            {
                this.LayoutRoot.Width = Application.Current.Host.Content.ActualWidth;
                this.LayoutRoot.Height = Application.Current.Host.Content.ActualHeight;
            }
        }
        void _SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender == this.workSpace)
            {
                #region
                this.workSpace.Visibility = System.Windows.Visibility.Collapsed;

                var hostWidth = this.LayoutRoot.ActualWidth - this.lbTools.ActualWidth;
                var hostHeight = this.LayoutRoot.ActualHeight - this.toolbar1.ActualHeight;

                if (workSpace.ActualWidth > hostWidth)
                {
                    this.svWorkSpace.Width = hostWidth;
                }
                else
                {
                    this.svWorkSpace.Width = workSpace.ActualWidth;
                }

                if (hostHeight < workSpace.ActualHeight)
                {
                    svWorkSpace.Height = hostHeight;
                }
                else
                {
                    svWorkSpace.Height = workSpace.ActualHeight;
                }

                this.SetGridLines(this.workSpace, true); //重新画线
                this.workSpace.Visibility = System.Windows.Visibility.Visible;
                #endregion
            }
            else if (sender == this.LayoutRoot)
            {
                #region
                if (LayoutRoot.Height < 100) return;
                this.Visibility = System.Windows.Visibility.Collapsed;

                var containerWidth = LayoutRoot.Width;

                double left = 180;
                double width = containerWidth - left;
                if (width < workSpace.ActualWidth)
                {
                    this.svWorkSpace.Width = width > 0 ? width : this.svWorkSpace.Width;
                }
                else
                {
                    this.svWorkSpace.Width = workSpace.ActualWidth;
                }

                double containerHeight = LayoutRoot.Height;

                //double top =  this.toolbar1.ActualHeight == 0 || double.IsNaN(this.toolbar1.ActualHeight) ? 35 : this.toolbar1.ActualHeight;
                double height = containerHeight - 35;

                if (0 < height)
                {
                    this.lbTools.Height = height;

                    this.toolbar1.Height = this.toolbar1.ActualHeight;
                    this.workSpace.Height = this.workSpace.ActualHeight;
                    if (height < workSpace.ActualHeight)
                    {
                        svWorkSpace.Height = height > 0 ? height : svWorkSpace.Height;
                    }
                    else
                    {
                        svWorkSpace.Height = workSpace.ActualHeight;
                    }
                }
                this.Visibility = System.Windows.Visibility.Visible;
                #endregion
            }
            else if (sender == this)
            {
                if (LoadSource)
                {
                    this.LayoutRoot.Width = this.ActualWidth;
                    this.LayoutRoot.Height = this.ActualHeight;
                }
                else
                {
                    this.LayoutRoot.Width = this.ActualWidth;
                    this.LayoutRoot.Height = this.ActualHeight - 10;
                }
            }
        }

        private MainPage()
        {
            InitializeComponent();

            // 通过属性调用服务获取参数
            if (string.IsNullOrEmpty(Glo.AppCenterDBType))
            {
                FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                da.CfgKeyAsync("AppCenterDBType", Glo.UserNo, Glo.SID);
                da.CfgKeyCompleted += (object sender, FF.CfgKeyCompletedEventArgs e) =>
                {
                    if (null != e.Error || string.IsNullOrEmpty(e.Result))
                    {
                        MessageBox.Show("请检查配置节点 AppCenterDBType,\t\n错误消息:" + e.Error.Message +" \t\n UserNo="+Glo.UserNo+",SID="+Glo.SID);
                        Glo.AppCenterDBType = "MSSQL";
                    }
                    else
                        Glo.AppCenterDBType = e.Result;
                };
            }

            if (string.IsNullOrEmpty(Glo.CustomerNo))
            {
                FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();

                da.CfgKeyAsync("CustomerNo", Glo.UserNo, Glo.SID);
                da.CfgKeyCompleted += (object s, FF.CfgKeyCompletedEventArgs ee) =>
                {
                    if (null != ee.Error || string.IsNullOrEmpty(ee.Result))
                    {
                        // MessageBox.Show("请检查配置节CustomerNo");
                        Glo.CustomerNo = "CCFlow";
                    }
                    else
                        Glo.CustomerNo = ee.Result;
                };
            }

            #region toolbar
            List<Func> ens = new List<Func>();
            ens = Func.instance.GetToolList();
            foreach (Func en in ens)
            {
                Image img = new Image()
                {
                    Width = 13,
                    Height = 13,
                    Source = new BitmapImage(new Uri("/CCFormDesigner;component/Img/" + en.No + ".png", UriKind.Relative))
                };

                TextBlock tb = new TextBlock()
                {
                    Name = "tbT" + en.No,
                    Text = en.Name + " ",
                    FontSize = 13
                };

                StackPanel mysp = new StackPanel()
                {
                    Name = "sp" + en.No,
                    Orientation = Orientation.Horizontal,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    //RenderTransform = new CompositeTransform() { TranslateX = 0 },

                };
                mysp.Children.Add(img);
                mysp.Children.Add(tb);

                Toolbar.ToolbarButton btn = new Toolbar.ToolbarButton()
                {
                    Name = "Btn_" + en.No,
                    Tag = en.No,
                    Content = mysp
                };
                btn.Click += new RoutedEventHandler(ToolBar_Click);

                this.toolbar1.AddBtn(btn);
            }

            List<EleFunc> ensEle = new List<EleFunc>();
            ensEle = EleFunc.instance.getToolList();
            foreach (EleFunc en in ensEle)
            {
                Image img = new Image();
                //   BitmapImage png = new BitmapImage(new Uri("/CCFormDesigner;component/Img/" + en.No + ".png", UriKind.Relative));
                img.Source = en.Img;

                TextBlock tb = new TextBlock();
                tb.Name = "tbT" + en.No;
                tb.Text = en.Name + " ";
                tb.FontSize = 15;
                StackPanel mysp = new StackPanel();
                mysp.Children.Add(img);
                mysp.Children.Add(tb);


                Toolbar.ToolbarBtn btn = new Toolbar.ToolbarBtn();
                btn.Name = "Btn_" + en.No;
                btn.Tag = en.No;
                btn.Click += new RoutedEventHandler(ToolBar_Click);
                btn.Content = mysp;
                this.toolbar1.AddBtn(btn);
            }


            #endregion

            #region 工具箱
            this.lbTools.ItemsSource = ToolBoxes.instance.GetToolBoxList();
            this.lbTools.SelectionMode = SelectionMode.Single;
            this.lbTools.AddHandler(ListBox.MouseLeftButtonDownEvent, new MouseButtonEventHandler(lbTools_MouseLeftButtonDown), true);
            this.lbTools.MouseLeftButtonUp += lbTools_MouseLeftButtonUp;
            this.lbTools.MouseRightButtonDown += UIElement_MouseRightButtonDown;
            #endregion

            #region chinwin
            winFrmImg.Name = NameImg;
            winFrmImgAth.Name = NameImgAth;
            winFrmImgSeal.Name = NameImgSeal;
            winFrmLab.Name = NameLab;
            winSelectM2M.Name = NameM2M;
            winFrmLink.Name = NameLink;
            winSelectTB.Name = NameTB;
            winSelectDDL.Name = NameDDL;
            winSelectRB.Name = NameRB;
            winFrmImp.Name = NameImp;
            winFrmEle.Name = NameEle;
            winFrmEleMapPin.Name = NameEleMapPin;
            winFrmEleMicHot.Name = NameEleMicHot;
            winSelectAttachment.Name = NameAttachment;
            winFrmOp.Name = NameOp;
            winFrmBtn.Name = NameBtn;
            winFlowFrm.Name = NameFlowFrm;
            winNodeFrms.Name = NameNodeFrms;
            winFrmAttachmentM.Name = NameAttachmentM;

            winFrmImg.Closed += WindowDilag_Closed;
            winFrmImgAth.Closed += WindowDilag_Closed;
            winFrmImgSeal.Closed += WindowDilag_Closed;
            winFrmLab.Closed += WindowDilag_Closed;
            winFrmLink.Closed += WindowDilag_Closed;
            winNodeFrms.Closed += WindowDilag_Closed;
            winSelectTB.Closed += WindowDilag_Closed;
            winSelectDDL.Closed += WindowDilag_Closed;
            winSelectRB.Closed += WindowDilag_Closed;
            winFrmImp.Closed += WindowDilag_Closed;
            winFrmEle.Closed += WindowDilag_Closed;
            winFrmEleMapPin.Closed += WindowDilag_Closed;
            winFrmEleMicHot.Closed += WindowDilag_Closed;
            winFrmBtn.Closed += WindowDilag_Closed;
            winSelectAttachment.Closed += WindowDilag_Closed;
            winFrmOp.Closed += WindowDilag_Closed;
            winFlowFrm.Closed += WindowDilag_Closed;
            winFrmAttachmentM.Closed += WindowDilag_Closed;
            winSelectM2M.Closed += WindowDilag_Closed;
            #endregion chinwin.

            initPoint();

            #region eventHandler
            cCursor = new CustomCursor(this);

            this.MouseRightButtonDown += UIElement_MouseRightButtonDown;
            this.svWorkSpace.MouseRightButtonDown += UIElement_MouseRightButtonDown;
            this.workSpace.MouseLeftButtonDown += workSpace_MouseLeftButtonDown;
            this.workSpace.MouseLeftButtonUp += workSpace_MouseLeftButtonUp;
            this.workSpace.MouseMove += workSpace_MouseMove;
            this.workSpace.MouseRightButtonDown += UIElement_MouseRightButtonDown;
            this.workSpace.MouseEnter += workSpace_MouseEnter;
            this.workSpace.MouseLeave += workSpace_MouseLeave;

            this.SizeChanged += _SizeChanged;
            this.LayoutRoot.SizeChanged += _SizeChanged;
            this.workSpace.SizeChanged += _SizeChanged;
            Application.Current.Host.Content.Resized += Content_Resized;
            #endregion

        }

        /// <summary>
        /// 由浏览器直接访问
        /// </summary>
        public void Load(bool loadSource = false)
        {
            LoadSource = loadSource;
            if (Glo.FK_MapDataNotFind.Equals(Glo.FK_MapData) == false)
            {
                this.BindFrm();
            }
        }
        /// <summary>
        /// 由CCFlowDesigner进入
        /// </summary>
        /// <param name="FK_MAPDATA">表单标识字段</param>
        public void Load(string userNo, string sid, string fk_mapData, string fk_flow, string dbtype, string customerNo)
        {
            if (string.IsNullOrEmpty(fk_mapData))
                throw new Exception("表单标识字段不允许空");

            Glo.UserNo = userNo;
            Glo.SID = sid;
            Glo.AppCenterDBType = dbtype;
            Glo.CustomerNo = customerNo;
            Glo.FK_MapData = fk_mapData;
            Glo.FK_Flow = fk_flow;
            Load(true);
        }
        #endregion

        void WindowDilag_Closed(object sender, EventArgs e)
        {
            this.SetSelectedTool(ToolBox.Mouse);
            ChildWindow c = sender as ChildWindow;
            if (c.DialogResult == false)
                return;

            switch (c.Name)
            {
                case NameImp: //导入的窗口关闭了.
                    this.BindFrm();
                    break;
                case NameImg:
                    Glo.currEle = this.winFrmImg.HisImg;
                    break;
                case NameImgAth:
                    Glo.currEle = this.winFrmImgAth.HisImgAth;
                    break;
                case NameM2M:
                    BPM2M m2m = new BPM2M(this.winSelectM2M.IsM2M);
                    m2m.Name = Glo.TempVal.ToString();
                    if (this.workSpace.FindName(m2m.Name) != null)
                    {
                        MessageBox.Show("已经存在对象：" + m2m.Name);
                        break;
                    }

                    m2m.SetValue(Canvas.LeftProperty, Glo.X);
                    m2m.SetValue(Canvas.TopProperty, Glo.Y);

                    attachElementEvent(m2m);
                    Glo.OpenM2M(Glo.FK_MapData, m2m.Name + Glo.TimeKey);
                    break;
                case NameLab:
                    this.currLab.Content = this.winFrmLab.TB_Text.Text.Replace("@", "\n");
                    int size = this.winFrmLab.DDL_FrontSize.SelectedIndex + 6;
                    this.currLab.FontSize = double.Parse(size.ToString());
                    break;
                case NameLink:
                    this.currLink.Content = this.winFrmLink.TB_Text.Text.Replace("@", "\n");
                    this.currLink.WinTarget = this.winFrmLink.TB_WinName.Text;
                    this.currLink.URL = this.winFrmLink.TB_URL.Text;
                    size = this.winFrmLink.DDL_FrontSize.SelectedIndex + 6;
                    this.currLink.FontSize = double.Parse(size.ToString());
                    break;
                case NameTB:
                    if (this.winSelectTB.IsCheckGroup == true)
                    {
                        #region 增加审核分组.

                        if (Glo.X > 300)
                            Glo.X = 300;

                        string gName = this.winSelectTB.TB_Name.Text;
                        string gKey = this.winSelectTB.TB_KeyOfEn.Text;
                        this.SetSelectedTool(ToolBox.Mouse);
                        FF.CCFormSoapClient daCreateCheckGroup = Glo.GetCCFormSoapClientServiceInstance();
                        daCreateCheckGroup.DoTypeAsync("CreateCheckGroup", gKey, gName, Glo.FK_MapData, null, null, null);
                        daCreateCheckGroup.DoTypeCompleted += daCreateCheckGroup_DoTypeCompleted;

                        #endregion
                    }
                    else
                    {
                        #region 属性.
                        TBType tp = TBType.String;
                        if (winSelectTB.RB_String.IsChecked == true)
                            tp = TBType.String;

                        if (winSelectTB.RB_Money.IsChecked == true)
                            tp = TBType.Money;
                        if (winSelectTB.RB_Int.IsChecked == true)
                            tp = TBType.Int;
                        if (winSelectTB.RB_Float.IsChecked == true)
                            tp = TBType.Float;

                        if (winSelectTB.RB_DataTime.IsChecked == true)
                            tp = TBType.DateTime;

                        if (winSelectTB.RB_Data.IsChecked == true)
                            tp = TBType.Date;

                        string keyName = this.winSelectTB.TB_Name.Text.Trim();
                        string keyOfEn = this.winSelectTB.TB_KeyOfEn.Text.Trim();

                        if (winSelectTB.RB_Boolen.IsChecked == true)
                        {
                            /* 如果是boolen 类型. */
                            BPCheckBox cb = new BPCheckBox();
                            cb.Tag = keyOfEn;
                            cb.KeyName = keyName;
                            cb.Content = keyName;
                            cb.Name = keyOfEn;

                            Label cbLab = new Label();
                            cbLab.Name = "Lbl" + cb.Name;
                            cbLab.Content = keyName;
                            cbLab.Tag = cb.Name.Trim();
                            cb.Content = cbLab;
                            cb.SetValue(Canvas.LeftProperty, Glo.X);
                            cb.SetValue(Canvas.TopProperty, Glo.Y);

                            this.attachElementEvent(cb);
                        }
                        else
                        {
                            BPTextBox mytb = new BPTextBox(tp);
                            mytb.NameOfReal = keyOfEn;
                            mytb.Tag = keyOfEn;
                            mytb.KeyName = keyName;
                            mytb.Name = keyOfEn;

                            mytb.SetValue(Canvas.LeftProperty, Glo.X);
                            mytb.SetValue(Canvas.TopProperty, Glo.Y);


                            // 检查是否生成 标签.
                            if (this.attachElementEvent(mytb)
                                && this.winSelectTB.CB_IsGenerLabel.IsChecked == true)
                            {
                                BPLabel lab = new BPLabel();
                                lab.Name = this.GenerElementNameFromUI(lab); //CCForm.Glo.GenerGUID();
                                lab.Content = keyName;
                                lab.Cursor = Cursors.Hand;
                                if (keyName != "")
                                {
                                    lab.SetValue(Canvas.LeftProperty, Glo.X - (keyName.Length * 20 - (keyName.Length * 30 * 0.16)));
                                }
                                else
                                {
                                    lab.SetValue(Canvas.LeftProperty, Glo.X - 20);
                                }
                                lab.SetValue(Canvas.TopProperty, Glo.Y);

                                this.attachElementEvent(lab);
                            }
                        }
                        #endregion
                    }
                    break;

                case NameBtn:
                    BPBtn btn = this.winFrmBtn.HisBtn;
                    if (this.workSpace.Children.Contains(btn))
                    {
                        BPBtn mybtn = (BPBtn)this.workSpace.FindName(btn.Name.Trim());
                        mybtn = btn;
                    }
                    else
                    {
                        btn.SetValue(Canvas.LeftProperty, Glo.X);
                        btn.SetValue(Canvas.TopProperty, Glo.Y);

                        this.attachElementEvent(btn);
                    }
                    break;

                case NameEle:
                    BPEle ele = this.winFrmEle.HisEle;
                    BPEle myEle = this.workSpace.FindName(ele.Name.Trim()) as BPEle;
                    if (myEle == null)
                    {
                        ele.SetValue(Canvas.LeftProperty, Glo.X);
                        ele.SetValue(Canvas.TopProperty, Glo.Y);
                        this.attachElementEvent(ele);
                    }
                    break;
                case NameEleMapPin:
                    BPMapPin mapPin = this.winFrmEleMapPin.HisEle;
                    BPMapPin myMapPin = this.workSpace.FindName(mapPin.Name.Trim()) as BPMapPin;
                    if (myMapPin == null)
                    {
                        mapPin.SetValue(Canvas.LeftProperty, Glo.X);
                        mapPin.SetValue(Canvas.TopProperty, Glo.Y);
                        this.attachElementEvent(mapPin);
                    }
                    else if (this.winFrmEleMapPin.IsNewElement == true)
                    {
                        MessageBox.Show("已经存在对象：" + mapPin.Name);
                    }
                    break;
                case NameEleMicHot:
                    BPMicrophonehot micHot = this.winFrmEleMicHot.HisEle;
                    BPMicrophonehot myMicHot = this.workSpace.FindName(micHot.Name.Trim()) as BPMicrophonehot;
                    if (myMicHot == null)
                    {
                        micHot.SetValue(Canvas.LeftProperty, Glo.X);
                        micHot.SetValue(Canvas.TopProperty, Glo.Y);
                        this.attachElementEvent(micHot);
                    }
                    else if (this.winFrmEleMicHot.IsNewElement == true)
                    {
                        MessageBox.Show("已经存在对象：" + micHot.Name);
                    }
                    break;
                case NameAttachmentM:
                    BPAttachmentM atth = this.winFrmAttachmentM.HisBPAttachment;
                    atth.Label = this.winFrmAttachmentM.TB_Name.Text.Trim();
                    atth.SetValue(Canvas.LeftProperty, Glo.X);
                    atth.SetValue(Canvas.TopProperty, Glo.Y);

                    this.attachElementEvent(atth);
                    break;
                case NameAttachment:
                    #region 单附件.
                    BPAttachment atthMy = new BPAttachment(
                        this.winSelectAttachment.TB_No.Text.Trim(),
                        this.winSelectAttachment.TB_Name.Text.Trim(),
                        this.winSelectAttachment.TB_Exts.Text,
                        70,
                        this.winSelectAttachment.TB_SaveTo.Text);
                    atthMy.SetValue(Canvas.LeftProperty, Glo.X);
                    atthMy.SetValue(Canvas.TopProperty, Glo.Y);
                    atthMy.X = Glo.X;
                    atthMy.Y = Glo.Y;
                    atthMy.IsUpload = (bool)this.winSelectAttachment.CB_IsUpload.IsChecked;
                    atthMy.IsDelete = (bool)this.winSelectAttachment.CB_IsDelete.IsChecked;
                    atthMy.IsDownload = (bool)this.winSelectAttachment.CB_IsDownload.IsChecked;

                    this.attachElementEvent(atthMy);

                    /*生成标签*/
                    BPLabel lb = new BPLabel();
                    lb.Content = this.winSelectAttachment.TB_Name.Text;
                    lb.Name = GenerElementNameFromUI(lb);
                    lb.Cursor = Cursors.Hand;
                    lb.SetValue(Canvas.LeftProperty, Glo.X - 20);
                    lb.SetValue(Canvas.TopProperty, Glo.Y);

                    this.attachElementEvent(lb);
                    #endregion
                    break;
                case NameDDL:
                    if (this.winSelectDDL.listBox1.SelectedIndex < 0)
                        break;

                    #region 需要设置他的类型.
                    ListBoxItem mylbi = this.winSelectDDL.listBox1.SelectedItem as ListBoxItem;
                    string enKey = mylbi.Content.ToString();
                    enKey = enKey.Substring(0, enKey.IndexOf(':'));

                    //判断是否是WebSerivce数据源项，如果是，则_HisDataType=LGType.Normal
                    var type = mylbi.Tag.ToString().Split(':')[2];

                    BPDDL myddl = new BPDDL()
                    {
                        KeyName = this.winSelectDDL.TB_KeyOfName.Text.Trim(),
                        Name = this.winSelectDDL.TB_KeyOfEn.Text.Trim(),
                        Width = 100,
                        Height = 23
                    };

                    myddl.SetValue(Canvas.LeftProperty, Glo.X);
                    myddl.SetValue(Canvas.TopProperty, Glo.Y);

                    if (mylbi.Tag.ToString().EndsWith(":1") || mylbi.Tag.ToString().EndsWith(":2"))
                        myddl.BindNormal(enKey);
                    else
                        myddl.BindEns(enKey);

                    this.attachElementEvent(myddl);

                    // 检查是否生成 标签.
                    if (this.winSelectDDL.CB_IsGenerLab.IsChecked == true)
                    {
                        BPLabel lab = new BPLabel();
                        lab.Content = this.winSelectDDL.TB_KeyOfName.Text.Trim();
                        string lblText = this.winSelectDDL.TB_KeyOfName.Text.Trim();
                        if (lblText != "")
                        {
                            lab.SetValue(Canvas.LeftProperty, Glo.X - (lblText.Length * 20 - (lblText.Length * 30 * 0.16)));
                        }
                        else
                        {
                            lab.SetValue(Canvas.LeftProperty, Glo.X - 20);
                        }
                        lab.SetValue(Canvas.TopProperty, Glo.Y);

                        this.attachElementEvent(lab);
                    }
                    #endregion
                    break;

                case NameRB:
                    if (this.winSelectRB.listBox1.SelectedIndex < 0)
                        break;

                    #region 单选按钮.

                    ListBoxItem lbi = this.winSelectRB.listBox1.SelectedItem as ListBoxItem;
                    string enumKey = lbi.Content.ToString();
                    enumKey = enumKey.Substring(0, enumKey.IndexOf(':'));

                    string cfgKeys = lbi.Tag as string;
                    string[] strs = cfgKeys.Split('@');
                    if (IsRB)
                    {
                        int addX = 0;
                        int addY = 0;
                        string gName = this.winSelectRB.TB_KeyOfEn.Text.Trim();
                        foreach (string str in strs)
                        {
                            if (string.IsNullOrEmpty(str))
                                continue;

                            string[] mykey = str.Split('=');
                            BPRadioBtn rb = new BPRadioBtn();
                            rb.KeyName = this.winSelectRB.TB_KeyOfName.Text.Trim();
                            rb.Content = mykey[1];
                            rb.Tag = mykey[0];
                            rb.Name = Glo.FK_MapData + "_" + gName + "_" + mykey[0];
                            rb.UIBindKey = enumKey;
                            rb.SetValue(Canvas.LeftProperty, Glo.X + addX);
                            rb.SetValue(Canvas.TopProperty, Glo.Y + addY);
                            rb.GroupName = gName;
                            addY += 16;

                            this.attachElementEvent(rb);
                        }

                        // 检查是否生成 标签.
                        if (this.winSelectRB.CB_IsGenerLab.IsChecked == true)
                        {
                            BPLabel lab = new BPLabel();
                            lab.Content = this.winSelectRB.TB_KeyOfName.Text.Trim();
                            string lblText = this.winSelectRB.TB_KeyOfName.Text.Trim();
                            if (lblText != "")
                            {
                                lab.SetValue(Canvas.LeftProperty, Glo.X - (lblText.Length * 20 - (lblText.Length * 30 * 0.16)));
                            }
                            else
                            {
                                lab.SetValue(Canvas.LeftProperty, Glo.X - 20);
                            }
                            lab.SetValue(Canvas.TopProperty, Glo.Y);

                            this.attachElementEvent(lab);
                        }
                    }
                    else
                    {
                        /* 如果是ddl.*/
                        BPDDL myddlEnum = new BPDDL()
                        {
                            Name = this.winSelectRB.TB_KeyOfEn.Text.Trim(),
                            KeyName = this.winSelectRB.TB_KeyOfName.Text.Trim(),
                            Width = 100,
                            Height = 23
                        };

                        myddlEnum.SetValue(Canvas.LeftProperty, Glo.X);
                        myddlEnum.SetValue(Canvas.TopProperty, Glo.Y);
                        myddlEnum.BindEnum(enumKey);

                        this.attachElementEvent(myddlEnum);

                        // 检查是否生成 标签.
                        if (this.winSelectRB.CB_IsGenerLab.IsChecked == true)
                        {
                            /*要生成标签*/
                            BPLabel lab = new BPLabel();
                            lab.Content = this.winSelectRB.TB_KeyOfName.Text.Trim();
                            string lblText = this.winSelectRB.TB_KeyOfName.Text.Trim();
                            if (lblText != "")
                            {
                                lab.SetValue(Canvas.LeftProperty, Glo.X - (lblText.Length * 20 - (lblText.Length * 30 * 0.16)));
                            }
                            else
                            {
                                lab.SetValue(Canvas.LeftProperty, Glo.X - 20);
                            }
                            lab.SetValue(Canvas.TopProperty, Glo.Y);

                            this.attachElementEvent(lab);
                        }
                    }
                    #endregion
                    break;
                /*  
                * Property
                */
                case NameFlowFrm:
                    Glo.FK_MapData = this.winFlowFrm.TB_No.Text;
                    this.BindTreeView();
                    break;
                case NameNodeFrms:
                    this.BindTreeView();
                    break;
                case NameOp: //画布大小改变了.
                    this.changeFormSize(double.Parse(this.winFrmOp.TB_FrmW.Text), double.Parse(this.winFrmOp.TB_FrmH.Text));
                    break;
            }
        }

        // 增加的审核分组
        void daCreateCheckGroup_DoTypeCompleted(object sender, FF.DoTypeCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show(e.Result);
                return;
            }

            if (Glo.X > 300)
                Glo.X = 300;

            /*如果是增加的审核分组.*/
            string gName = this.winSelectTB.HisFrmSelectCheckGroup.TB_GroupName.Text;
            string gKey = this.winSelectTB.HisFrmSelectCheckGroup.TB_GroupKey.Text;

            BPTextBox tbNote = new BPTextBox(TBType.String, gKey + "_Note")
            {
                KeyName = gName,
                Cursor = Cursors.Hand,
                Width = 550,
                Height = 70
            };

            tbNote.SetValue(Canvas.LeftProperty, Glo.X - 10);
            tbNote.SetValue(Canvas.TopProperty, Glo.Y);


            this.attachElementEvent(tbNote);

            BPTextBox tbChecker = new BPTextBox(TBType.String, gKey + "_Checker");
            tbChecker.KeyName = "审核人";
            tbChecker.SetValue(Canvas.LeftProperty, Glo.X + 80);
            tbChecker.SetValue(Canvas.TopProperty, Glo.Y + 75);

            this.attachElementEvent(tbChecker);

            BPTextBox tbRDT = new BPTextBox(TBType.DateTime, gKey + "_RDT");
            tbRDT.KeyName = "审核时间";
            tbRDT.SetValue(Canvas.LeftProperty, Glo.X + 320);
            tbRDT.SetValue(Canvas.TopProperty, Glo.Y + 75);

            this.attachElementEvent(tbRDT);

            /*要生成标签*/
            BPLabel abCheckNote = new BPLabel();
            abCheckNote.Content = gName.Replace("审核意见", "@审核意见"); // "审核意见";
            abCheckNote.Name = "LAB" + gKey + "Note";
            abCheckNote.SetValue(Canvas.LeftProperty, Glo.X - 30);
            abCheckNote.SetValue(Canvas.TopProperty, Glo.Y);

            this.attachElementEvent(abCheckNote);

            BPLabel labChecker = new BPLabel();
            labChecker.Content = "审核人";
            labChecker.Name = "LAB" + gKey + "Checker";
            labChecker.SetValue(Canvas.LeftProperty, Glo.X + 40);
            labChecker.SetValue(Canvas.TopProperty, Glo.Y + 75);

            this.attachElementEvent(labChecker);

            BPLabel abCheckRDT = new BPLabel();
            abCheckRDT.Content = "日期";
            abCheckRDT.Name = "LAB" + gKey + "RDT";
            abCheckRDT.SetValue(Canvas.LeftProperty, Glo.X + 290);
            abCheckRDT.SetValue(Canvas.TopProperty, Glo.Y + 75);

            this.attachElementEvent(abCheckRDT);
        }


        #region load and save
        static DataSet dsLatest;
        JsonObject jsonObject = null;

        //绑定表单.
        public void BindFrm()
        {
            this.workSpace.Children.Clear();
            this.Cursor = Cursors.Wait;
            try
            {
                FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                da.GenerFrmAsync(Glo.FK_MapData, 0);
                da.GenerFrmCompleted += new EventHandler<FF.GenerFrmCompletedEventArgs>(
                (object sender, FF.GenerFrmCompletedEventArgs e) =>
                {
                    if (e.Error != null)
                    {
                        this.Cursor = Cursors.Arrow;
                        BP.SL.LoggerHelper.Write(e.Error);
                        MessageBox.Show("表单数据请求服务错误：" + e.Error.Message);
                        return;
                    }
                    else
                    {
                        OpenFormJson(e.Result);
                    }

                    this.SetSelectedTool(ToolBox.Mouse);
                    HtmlPage.Plugin.Focus();
                    this.Focus();
                });
            }
            catch (Exception e)
            {
                MessageBox.Show("表单数据请求服务错误：" + e.Message);
            }
        }
        /// <summary>
        /// 打开Json字符串.
        /// </summary>
        /// <param name="strs"></param>
        void OpenFormJson(string strs)
        {
            bool toBeContinued = true;
            if (string.IsNullOrEmpty(strs) || strs.Length < 200)
            {
                loadingWindow.DialogResult = false;
                strs = string.IsNullOrEmpty(strs) ? "数据为空" : strs;
                toBeContinued = false;
            }

            if (toBeContinued)
            {
                string table = "";
                try
                {
                    jsonObject = (JsonObject)JsonObject.Parse(strs);
                    if (jsonObject == null || jsonObject.Count == 0)
                        toBeContinued = false;

                    #region
                    if (toBeContinued)
                        foreach (KeyValuePair<string, JsonValue> item in jsonObject)
                        {
                            table = item.Key;
                            Glo.TempVal = table;

                            string tmpStr = string.Empty;
                            double tmpDouble = 0;
                            switch (table)
                            {
                                case EEleTableNames.WF_Node:
                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;

                                        tmpStr = dr["NODEID"].ToString();

                                        string sta = dr["FWCSTA"].ToString();
                                        if (sta != "0")
                                        {
                                            BPWorkCheck dtl = new BPWorkCheck(tmpStr);
                                            tmpDouble = dr["FWC_X"];
                                            dtl.SetValue(Canvas.LeftProperty, tmpDouble);
                                            tmpDouble = dr["FWC_Y"];
                                            dtl.SetValue(Canvas.TopProperty, tmpDouble);
                                            tmpDouble = dr["FWC_W"];
                                            dtl.Width = tmpDouble;
                                            tmpDouble = dr["FWC_H"];
                                            dtl.Height = tmpDouble;
                                            attachElementEvent(dtl);
                                        }

                                        sta = dr["SFSTA"].ToString();
                                        if (sta != "0")
                                        {
                                            BPSubFlow sbCtrl = new BPSubFlow(tmpStr);
                                            tmpDouble = dr["SF_X"];
                                            sbCtrl.SetValue(Canvas.LeftProperty, tmpDouble);
                                            tmpDouble = dr["SF_Y"];
                                            sbCtrl.SetValue(Canvas.TopProperty, tmpDouble);
                                            tmpDouble = dr["SF_W"];
                                            sbCtrl.Width = tmpDouble;
                                            tmpDouble = dr["SF_H"];
                                            sbCtrl.Height = tmpDouble;
                                            attachElementEvent(sbCtrl);
                                        }
                                    }
                                    break;
                                case EEleTableNames.Sys_FrmEle:
                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;

                                        if ((string)dr["FK_MAPDATA"] != Glo.FK_MapData)
                                            continue;

                                        string eleType = (string)dr["ELETYPE"];
                                        if (string.IsNullOrEmpty(eleType))
                                            continue;

                                        //扩展控件
                                        BPEle bpele = new BPEle();
                                        bpele.Name = dr["MYPK"];
                                        
                                        string eldId = (string)dr["ELEID"];
                                        if (string.IsNullOrEmpty(eldId))
                                            continue;

                                        string eleName = dr["ELENAME"];
                                        if (string.IsNullOrEmpty(eleName))
                                            continue;

                                        bpele.EleType = eleType;
                                        bpele.EleName = eleName;
                                        bpele.EleID = eldId;

                                        tmpDouble = dr["X"];
                                        bpele.SetValue(Canvas.LeftProperty, tmpDouble);
                                        tmpDouble = dr["Y"];
                                        bpele.SetValue(Canvas.TopProperty, tmpDouble);

                                        bpele.Width = dr["W"];
                                        bpele.Height = dr["H"];

                                        attachElementEvent(bpele);
                                    }
                                    continue;
                                case EEleTableNames.Sys_MapData:

                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;

                                        string no = (string)dr["NO"];
                                        if (no != Glo.FK_MapData)
                                            continue;

                                        Glo.HisMapData = new MapData();
                                        Glo.HisMapData.FrmH = dr["FRMH"];
                                        Glo.HisMapData.FrmW = dr["FRMW"];
                                        Glo.HisMapData.No = no;
                                        Glo.HisMapData.Name = dr["NAME"];
                                        Glo.IsDtlFrm = false;

                                        this.workSpace.Width = Glo.HisMapData.FrmW;
                                        this.workSpace.Height = Glo.HisMapData.FrmH;
                                    }

                                    break;
                                case EEleTableNames.Sys_FrmBtn:
                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;

                                        if ((string)dr["FK_MAPDATA"] != Glo.FK_MapData)
                                            continue;

                                        BPBtn btn = new BPBtn();

                                        btn.Name = dr["MYPK"];
                                        tmpStr = dr["TEXT"];
                                        btn.Content = tmpStr.Replace("&nbsp;", " ");
                                        // json 值中会有null 或 ""
                                        int type = 0;
                                        //tmpStr = dr["BTNTYPE"].ToString();
                                        //if (!string.IsNullOrEmpty(tmpStr))
                                        //{
                                        //    int.TryParse(tmpStr, out type);
                                        //}
                                        btn.HisBtnType = (BtnType)type;

                                        try
                                        {
                                            type = 0;
                                            tmpStr = dr["EVENTTYPE"].ToString();
                                            if (!string.IsNullOrEmpty(tmpStr))
                                            {
                                                int.TryParse(tmpStr, out type);
                                            }
                                            btn.HisEventType = (EventType)type;

                                            tmpStr = dr["EVENTCONTEXT"];
                                            if (!string.IsNullOrEmpty(tmpStr))
                                                btn.EventContext = tmpStr.Replace("~", "'");

                                            tmpStr = dr["MSGERR"];
                                            if (!string.IsNullOrEmpty(tmpStr))
                                                btn.MsgErr = tmpStr.Replace("~", "'");

                                            tmpStr = dr["MSGOK"];
                                            if (!string.IsNullOrEmpty(tmpStr))
                                                btn.MsgOK = tmpStr.Replace("~", "'");
                                        }
                                        catch
                                        {

                                        }

                                        tmpDouble = dr["X"];
                                        btn.SetValue(Canvas.LeftProperty, tmpDouble);
                                        tmpDouble = dr["Y"];
                                        btn.SetValue(Canvas.TopProperty, tmpDouble);
                                        attachElementEvent(btn);
                                    }
                                    continue;
                                case EEleTableNames.Sys_FrmLine:
                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;

                                        if ((string)dr["FK_MAPDATA"] != Glo.FK_MapData)
                                            continue;

                                        string color = dr["BORDERCOLOR"];
                                        if (string.IsNullOrEmpty(color))
                                            color = "Black";

                                        BPLine myline = new BPLine(color, dr["BORDERWIDTH"],
                                            dr["X1"], dr["Y1"], dr["X2"], dr["Y2"]);
                                        myline.Name = dr["MYPK"];
                                        attachElementEvent(myline);
                                    }
                                    continue;
                                case EEleTableNames.Sys_FrmLab:
                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;

                                        if ((string)dr["FK_MAPDATA"] != Glo.FK_MapData)
                                            continue;

                                        BPLabel lab = new BPLabel();
                                        lab.Name = dr["MYPK"];

                                        tmpStr = dr["TEXT"];
                                        tmpStr = tmpStr.Replace("&nbsp;", " ").Replace("@", "\n");
                                        lab.Content = tmpStr;

                                        lab.FontSize = dr["FONTSIZE"];

                                        lab.SetValue(Canvas.LeftProperty, (double)dr["X"]);
                                        lab.SetValue(Canvas.TopProperty, (double)dr["Y"]);

                                        if (dr["ISBOLD"] == 1)
                                            lab.FontWeight = FontWeights.Bold;
                                        else
                                            lab.FontWeight = FontWeights.Normal;

                                        string color = dr["FONTCOLOR"];
                                        lab.Foreground = new SolidColorBrush(Glo.ToColor(color));

                                        attachElementEvent(lab);
                                    }
                                    continue;
                                case EEleTableNames.Sys_FrmLink:
                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;


                                        if ((string)dr["FK_MAPDATA"] != Glo.FK_MapData)
                                            continue;

                                        BPLink link = new BPLink();
                                        link.Name = dr["MYPK"];
                                        tmpStr = dr["TEXT"];
                                        link.Content = tmpStr;
                                        link.URL = dr["URL"];
                                        link.WinTarget = dr["TARGET"];
                                        link.FontSize = dr["FONTSIZE"];

                                        link.SetValue(Canvas.LeftProperty, (double)dr["X"]);
                                        link.SetValue(Canvas.TopProperty, (double)dr["Y"]);

                                        string color = dr["FONTCOLOR"];
                                        if (string.IsNullOrEmpty(color))
                                            color = "Black";

                                        link.Foreground = new SolidColorBrush(Glo.ToColor(color));

                                        attachElementEvent(link);
                                    }
                                    continue;
                                case EEleTableNames.Sys_FrmImg:
                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;

                                        if ((string)dr["FK_MAPDATA"] != Glo.FK_MapData)
                                            continue;

                                        int ImgAppType = 0;
                                        if (dr["IMGAPPTYPE"] != null)
                                            int.TryParse(dr["IMGAPPTYPE"].ToString().Replace("\"", ""), out ImgAppType);
                                        switch (ImgAppType)
                                        {
                                            case 1:
                                                BPImgSeal imgSeal = new BPImgSeal();
                                                imgSeal.Name = dr["MYPK"];
                                                imgSeal.SetValue(Canvas.LeftProperty, (double)dr["X"]);
                                                imgSeal.SetValue(Canvas.TopProperty, (double)dr["Y"]);

                                                imgSeal.Width = dr["W"];
                                                imgSeal.Height = dr["H"];
                                                imgSeal.TB_CN_Name = dr["NAME"] == null ? dr["MYPK"] : dr["NAME"];
                                                imgSeal.TB_En_Name = dr["ENPK"] == null ? dr["MYPK"] : dr["ENPK"];
                                                imgSeal.Tag0 = dr["TAG0"];
                                                imgSeal.IsEdit = false;

                                                imgSeal.IsEdit = dr["ISEDIT"] == 1 ? true : false;

                                                attachElementEvent(imgSeal);
                                                break;
                                            default:
                                                BPImg img = new BPImg();
                                                img.Name = dr["MYPK"];
                                                img.SetValue(Canvas.LeftProperty, (double)dr["X"]);
                                                img.SetValue(Canvas.TopProperty, (double)dr["Y"]);
                                                img.TB_CN_Name = dr["NAME"] == null ? dr["MYPK"] : dr["NAME"];
                                                img.TB_En_Name = dr["ENPK"] == null ? dr["MYPK"] : dr["ENPK"];
                                                img.Width = dr["W"];
                                                img.Height = dr["H"];

                                                string imgPath = string.Empty;
                                                if (dr["IMGPATH"] != null)
                                                {
                                                    imgPath = dr["IMGPATH"];
                                                }
                                                string imgUrl = string.Empty;
                                                if (dr["IMGURL"] != null)
                                                {
                                                    imgUrl = dr["IMGURL"];
                                                }
                                                //本地图片
                                                if (dr["SRCTYPE"] == 0)
                                                {
                                                    img.SrcType = 0;
                                                    //判断图片路径是否修改
                                                    if (imgPath.Contains("DataUser"))
                                                    {
                                                        ImageBrush ib = new ImageBrush();
                                                        imgPath = Glo.BPMHost + imgPath;
                                                        BitmapImage png = new BitmapImage(new Uri(imgPath, UriKind.RelativeOrAbsolute));
                                                        ib.ImageSource = png;
                                                        img.Background = ib;
                                                        img.HisPng = png;
                                                    }
                                                }
                                                else if (dr["SRCTYPE"] == 1)//指定路径
                                                {
                                                    img.SrcType = 1;
                                                    //判断图片路径不为空，并且不包含ccflow表达式
                                                    if (!imgUrl.Contains("@"))
                                                    {
                                                        ImageBrush ib = new ImageBrush();
                                                        BitmapImage png = new BitmapImage(new Uri(imgUrl, UriKind.RelativeOrAbsolute));
                                                        ib.ImageSource = png;
                                                        img.Background = ib;
                                                        img.HisPng = png;
                                                    }
                                                }

                                                img.LinkTarget = dr["LINKTARGET"];
                                                img.LinkURL = dr["LINKURL"];
                                                img.ImgURL = imgUrl;
                                                img.ImgPath = imgPath;
                                                attachElementEvent(img);
                                                break;
                                        }
                                    }
                                    continue;
                                case EEleTableNames.Sys_FrmImgAth:
                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;

                                        if ((string)dr["FK_MAPDATA"] != Glo.FK_MapData)
                                            continue;

                                        BPImgAth ath = new BPImgAth();
                                        ath.Name = dr["MYPK"];
                                        ath.CtrlID = dr["CTRLID"]; //附件ID.

                                        ath.SetValue(Canvas.LeftProperty, (double)dr["X"]);
                                        ath.SetValue(Canvas.TopProperty, (double)dr["Y"]);
                                        ath.IsEdit = dr["ISEDIT"] == 1 ? true : false;
                                        ath.IsRequired = dr["ISREQUIRED"] == 1 ? true : false;
                                        ath.Height = dr["H"];
                                        ath.Width = dr["W"];
                                        attachElementEvent(ath);
                                    }
                                    continue;
                                case EEleTableNames.Sys_FrmRB:
                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;
                                        if ((string)dr["FK_MAPDATA"] != Glo.FK_MapData)
                                            continue;

                                        BPRadioBtn btn = new BPRadioBtn();
                                        btn.Name = dr["MYPK"];
                                        btn.GroupName = dr["KEYOFEN"];
                                        btn.Content = (string)dr["LAB"];
                                        btn.UIBindKey = dr["ENUMKEY"];
                                        btn.Tag = dr["INTKEY"].ToString();
                                        btn.SetValue(Canvas.LeftProperty, (double)dr["X"]);
                                        btn.SetValue(Canvas.TopProperty, (double)dr["Y"]);


                                        attachElementEvent(btn);
                                    }
                                    continue;
                                case EEleTableNames.Sys_MapAttr:
                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;

                                        if (dr["UIVISIBLE"] == 0)
                                            continue;

                                        if ((string)dr["FK_MAPDATA"] != Glo.FK_MapData)
                                            continue;

                                        //string myPk = dr["MYPK"];
                                        string keyOfEn = dr["KEYOFEN"];
                                        string text = dr["NAME"];
                                        string defVal = dr["DEFVAL"];
                                        string UIContralType = dr["UICONTRALTYPE"].ToString();
                                        string MyDataType = dr["MYDATATYPE"].ToString();
                                        string lgType = dr["LGTYPE"].ToString();
                                        double X = dr["X"];
                                        double Y = dr["Y"];
                                        if (X == 0)
                                            X = 100;
                                        if (Y == 0)
                                            Y = 100;

                                        string uIBindKey = dr["UIBINDKEY"];
                                        switch (UIContralType)
                                        {
                                            case CtrlType.TextBox:
                                                TBType tp = TBType.String;
                                                switch (MyDataType)
                                                {
                                                    case DataType.AppInt:
                                                        tp = TBType.Int;
                                                        break;
                                                    case DataType.AppFloat:
                                                    case DataType.AppDouble:
                                                        tp = TBType.Float;
                                                        break;
                                                    case DataType.AppMoney:
                                                        tp = TBType.Money;
                                                        break;
                                                    case DataType.AppString:
                                                        tp = TBType.String;
                                                        break;
                                                    case DataType.AppDateTime:
                                                        tp = TBType.DateTime;
                                                        break;
                                                    case DataType.AppDate:
                                                        tp = TBType.Date;
                                                        break;
                                                    default:
                                                        break;
                                                }

                                                BPTextBox tb = new BPTextBox(tp)
                                                {
                                                    KeyName = text,
                                                    NameOfReal = keyOfEn,
                                                    Name = keyOfEn,
                                                    X = X,
                                                    Y = Y,
                                                    Width = dr["UIWIDTH"],
                                                    Height = dr["UIHEIGHT"]
                                                };

                                                tb.SetValue(Canvas.LeftProperty, X);
                                                tb.SetValue(Canvas.TopProperty, Y);

                                                if (this.workSpace.FindName(tb.Name) != null)
                                                    continue;

                                                attachElementEvent(tb);
                                                break;
                                            case CtrlType.DDL:
                                                BPDDL ddl = new BPDDL()
                                                {
                                                    KeyName = text,
                                                    Name = keyOfEn,
                                                    UIBindKey = uIBindKey,
                                                    HisLGType = lgType,
                                                    Width = dr["UIWIDTH"]
                                                };

                                                if (lgType == LGType.Enum)
                                                {
                                                    ddl.BindEnum(uIBindKey);
                                                }

                                                if (lgType == LGType.FK)
                                                {
                                                    //此处判断是否是WebService数据源类的DDL，如果是，则不进行填充，added by liuxc,2015.9.21
                                                    if (dr["ATPARA"] != null && !string.IsNullOrWhiteSpace(dr["ATPARA"].ToString()))
                                                    {
                                                        var aps = new BP.DA.AtPara(dr["ATPARA"].ToString());
                                                        if (string.IsNullOrWhiteSpace(aps.GetValStrByKey("WS")))
                                                            ddl.BindEns(uIBindKey);
                                                    }
                                                    else
                                                    {
                                                        ddl.BindEns(uIBindKey);
                                                    }
                                                }

                                                if (lgType == LGType.Normal)
                                                {
                                                    ddl.BindNormal(uIBindKey);
                                                }


                                                ddl.SetValue(Canvas.LeftProperty, X);
                                                ddl.SetValue(Canvas.TopProperty, Y);
                                                attachElementEvent(ddl);
                                                break;
                                            case CtrlType.CheckBox:
                                                BPCheckBox cb = new BPCheckBox()
                                                {
                                                    Name = keyOfEn,
                                                    KeyName = text
                                                };

                                                cb.Content = new Label()
                                                {
                                                    Name = "Lbl" + keyOfEn,
                                                    Content = text,
                                                };

                                                if (defVal == "1")
                                                    cb.IsChecked = true;
                                                else
                                                    cb.IsChecked = false;

                                                cb.SetValue(Canvas.LeftProperty, X);
                                                cb.SetValue(Canvas.TopProperty, Y);

                                                attachElementEvent(cb);
                                                break;
                                            case CtrlType.RB:
                                                break;
                                            case CtrlType.MapPin://地图定位
                                                BPMapPin mapPin = new BPMapPin();
                                                mapPin.MyPK = dr["MYPK"];
                                                mapPin.Name = keyOfEn;
                                                mapPin.KeyName = text;
                                                mapPin.SetValue(Canvas.LeftProperty, X);
                                                mapPin.SetValue(Canvas.TopProperty, Y);

                                                attachElementEvent(mapPin);
                                                break;
                                            case CtrlType.MicHot://录音
                                                BPMicrophonehot micHot = new BPMicrophonehot();
                                                micHot.MyPK = dr["MYPK"];
                                                micHot.Name = keyOfEn;
                                                micHot.KeyName = text;
                                                micHot.SetValue(Canvas.LeftProperty, X);
                                                micHot.SetValue(Canvas.TopProperty, Y);

                                                attachElementEvent(micHot);
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    continue;
                                case EEleTableNames.Sys_MapM2M:
                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;

                                        if ((string)dr["FK_MAPDATA"] != Glo.FK_MapData)
                                            continue;

                                        int m2MTYPE = dr["M2MTYPE"];

                                        BPM2M m2m = new BPM2M(dr["MYPK"], m2MTYPE);
                                        tmpDouble = dr["X"];
                                        m2m.SetValue(Canvas.LeftProperty, tmpDouble);
                                        tmpDouble = dr["Y"];
                                        m2m.SetValue(Canvas.TopProperty, tmpDouble);

                                        m2m.Width = dr["W"];
                                        m2m.Height = dr["H"];

                                        attachElementEvent(m2m);
                                    }
                                    continue;
                                case EEleTableNames.Sys_MapDtl:
                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;

                                        BPDtl dtl = new BPDtl(dr["NO"]);
                                        tmpDouble = dr["X"];
                                        dtl.SetValue(Canvas.LeftProperty, tmpDouble);
                                        tmpDouble = dr["Y"];
                                        dtl.SetValue(Canvas.TopProperty, tmpDouble);
                                        dtl.Width = dr["W"];
                                        dtl.Height = dr["H"];

                                        attachElementEvent(dtl);
                                    }
                                    continue;
                                case EEleTableNames.Sys_FrmAttachment:
                                    foreach (JsonValue dr in item.Value)
                                    {
                                        if (dr.Count == 0)
                                            continue;
                                        if ((string)dr["FK_MAPDATA"] != Glo.FK_MapData)
                                            continue;

                                        int uploadTypeInt = dr["UPLOADTYPE"];
                                        AttachmentUploadType uploadType = (AttachmentUploadType)uploadTypeInt;
                                        if (uploadType == AttachmentUploadType.Single)
                                        {
                                            BPAttachment ath = new BPAttachment(dr["NOOFOBJ"],
                                                dr["NAME"], dr["EXTS"], dr["W"], dr["SAVETO"]);

                                            tmpDouble = dr["X"];
                                            ath.SetValue(Canvas.LeftProperty, tmpDouble);
                                            tmpDouble = dr["Y"];
                                            ath.SetValue(Canvas.TopProperty, tmpDouble);
                                            ath.Label = dr["NAME"];
                                            ath.Exts = dr["EXTS"];
                                            ath.SaveTo = dr["SAVETO"];

                                            ath.X = dr["X"];
                                            ath.Y = dr["Y"];

                                            ath.IsUpload = dr["ISUPLOAD"] == 1 ? true : false;
                                            ath.IsDelete = dr["ISDELETE"] == 1 ? true : false;
                                            ath.IsDownload = dr["ISDOWNLOAD"] == 1 ? true : false;

                                            attachElementEvent(ath);
                                        }
                                        else if (uploadType == AttachmentUploadType.Multi)
                                        {
                                            BPAttachmentM athM = new BPAttachmentM();

                                            tmpDouble = dr["X"];
                                            athM.SetValue(Canvas.LeftProperty, tmpDouble);
                                            tmpDouble = dr["Y"];
                                            athM.SetValue(Canvas.TopProperty, tmpDouble);

                                            athM.Name = dr["NOOFOBJ"];
                                            athM.Width = dr["W"];
                                            athM.Height = dr["H"];
                                            athM.X = dr["X"];
                                            athM.Y = dr["Y"];
                                            athM.SaveTo = dr["SAVETO"];
                                            athM.Label = dr["NAME"];

                                            attachElementEvent(athM);

                                        }
                                        continue;

                                    }
                                    continue;
                                default:
                                    break;
                            }
                        }
                    #endregion

                }
                catch (Exception ex)
                {
                    toBeContinued = false;
                    BP.SL.LoggerHelper.Write(ex);
                    strs = "err:" + table + "，装载表单错误," + ex.Message;
                }
            }


            this.SetGridLines(this.workSpace, true); //重新画线

            if (!toBeContinued)
            {
                MessageBox.Show(strs);
            }
            else
            {
                if (null != CCBPFormLoaded)
                    CCBPFormLoaded();

            }
            this.Cursor = Cursors.Arrow;

        }

        void checkBound()
        {
            DataTable dtMapData = dsLatest.Tables[EEleTableNames.Sys_MapData];
            //表单数据在属性窗体中修改
            DataRow drMapDR = dtMapData.NewRow();
            drMapDR["NAME"] = Glo.HisMapData.Name;
            drMapDR["NO"] = Glo.FK_MapData;
            double tmpD = Glo.HisMapData.FrmW;
            drMapDR["FrmW"] = tmpD == double.NaN ? "900" : tmpD.ToString("0");
            tmpD = Glo.HisMapData.FrmH;
            drMapDR["FrmH"] = tmpD == double.NaN ? "900" : tmpD.ToString("0");
            dtMapData.Rows.Add(drMapDR);

            double MaxLeft = 0, MaxTop = 0, MaxRight = 0, MaxEnd = 0;
            #region 边界校验
            int bound = 0;
            foreach (FrameworkElement ctl in this.workSpace.Children)
            {
                // 只处理BP元素和添加数据
                if (!(ctl is IElement) || (ctl as IElement).ViewDeleted) continue;

                if (ctl is BPLine)
                {
                    bound++;
                    BPLine line = ctl as BPLine;

                    if (MaxLeft == 0 || MaxLeft > line.MyLine.X1)   //计算最左边的X坐标，edited by liuxc,2016-3-29
                        MaxLeft = line.MyLine.X1;
                    if (MaxTop == 0 || MaxTop > line.MyLine.Y1) //计算最上边的Y坐标
                        MaxTop = line.MyLine.Y1;

                    if (MaxRight < line.MyLine.X2)  //计算最右边的X坐标
                        MaxRight = line.MyLine.X2;

                    if (MaxEnd < line.MyLine.Y2)    //计算最下边的Y坐标
                        MaxEnd = line.MyLine.Y2;

                }
                else
                {
                    MatrixTransform transform = ctl.TransformToVisual(this.workSpace) as MatrixTransform;
                    double x = transform.Matrix.OffsetX;
                    double y = transform.Matrix.OffsetY;
                    double tmp = 0;
                    if (x <= 0)
                        x = 0;

                    if (double.IsNaN(y))
                    {
                        x = Canvas.GetLeft(ctl);
                        y = Canvas.GetTop(ctl);
                    }

                    if (MaxLeft == 0 || x < MaxLeft)
                        MaxLeft = x;

                    if (MaxTop == 0 || y < MaxTop)
                        MaxTop = y;

                    tmp = x + ctl.Width;
                    if (MaxRight < tmp)
                        MaxRight = tmp;

                    tmp = x + ctl.Height;
                    if (MaxEnd < tmp)
                        MaxEnd = tmp;
                }
            }
            if (bound < 2)
            {
                //MessageBox.Show("表单中至少需要2个确定边界的元素,请添加线或图片");
            }
            #endregion

            drMapDR["MaxLeft"] = MaxLeft.ToString("0");
            drMapDR["MaxTop"] = MaxTop.ToString("0");
            drMapDR["MaxRight"] = MaxRight.ToString("0");
            drMapDR["MaxEnd"] = MaxEnd.ToString("0");
        }

        public void Save()
        {
            loadingWindow.Show();
            if (!Glo.ViewNeedSave)
            {
            }
            Glo.ViewNeedSave = false;
            try
            {
                this.SetSelectedTool(ToolBox.Mouse);

                if (dsLatest == null)
                    initDataSource();
                else
                    foreach (DataTable item in dsLatest.Tables)
                    {
                        if (item != null)
                            item.Rows.Clear();
                    }

                this.checkBound();

                //this.SaveXml();
                this.SaveJson();
            }
            catch (Exception e)
            {
                loadingWindow.DialogResult = false;
                MessageBox.Show("保存错误:" + e.Message);
            }
        }
        void initDataSource()
        {
            DataTable dtMapData = new DataTable();
            dtMapData.TableName = EEleTableNames.Sys_MapData;
            dtMapData.Columns.Add(new DataColumn("NO", typeof(string)));
            dtMapData.Columns.Add(new DataColumn("NAME", typeof(string)));
            dtMapData.Columns.Add(new DataColumn("FrmW", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("FrmH", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("MaxLeft", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("MaxRight", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("MaxTop", typeof(double)));
            dtMapData.Columns.Add(new DataColumn("MaxEnd", typeof(double)));

            #region line
            DataTable dtLine = new DataTable();
            dtLine.TableName = EEleTableNames.Sys_FrmLine;
            dtLine.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtLine.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));

            dtLine.Columns.Add(new DataColumn("X1", typeof(double)));
            dtLine.Columns.Add(new DataColumn("Y1", typeof(double)));

            dtLine.Columns.Add(new DataColumn("X2", typeof(double)));
            dtLine.Columns.Add(new DataColumn("Y2", typeof(double)));

            dtLine.Columns.Add(new DataColumn("BorderWidth", typeof(string)));
            dtLine.Columns.Add(new DataColumn("BorderColor", typeof(string)));
            // lineDT.Columns.Add(new DataColumn("BorderStyle", typeof(string)));
            #endregion line

            #region btn
            DataTable dtBtn = new DataTable();
            dtBtn.TableName = EEleTableNames.Sys_FrmBtn;
            dtBtn.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtBtn.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtBtn.Columns.Add(new DataColumn("TEXT", typeof(string)));
            dtBtn.Columns.Add(new DataColumn("X", typeof(double)));
            dtBtn.Columns.Add(new DataColumn("Y", typeof(double)));
            #endregion line

            #region label
            DataTable dtLabel = new DataTable();
            dtLabel.TableName = EEleTableNames.Sys_FrmLab;
            dtLabel.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("X", typeof(double)));
            dtLabel.Columns.Add(new DataColumn("Y", typeof(double)));
            dtLabel.Columns.Add(new DataColumn("TEXT", typeof(string)));

            dtLabel.Columns.Add(new DataColumn("FONTCOLOR", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("FontName", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("FontStyle", typeof(string)));
            dtLabel.Columns.Add(new DataColumn("FONTSIZE", typeof(int)));
            dtLabel.Columns.Add(new DataColumn("IsBold", typeof(int)));
            dtLabel.Columns.Add(new DataColumn("IsItalic", typeof(int)));
            #endregion label

            #region Link
            DataTable dtLikn = new DataTable();
            dtLikn.TableName = EEleTableNames.Sys_FrmLink;
            dtLikn.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("X", typeof(double)));
            dtLikn.Columns.Add(new DataColumn("Y", typeof(double)));
            dtLikn.Columns.Add(new DataColumn("TEXT", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("TARGET", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("URL", typeof(string)));

            dtLikn.Columns.Add(new DataColumn("FONTCOLOR", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("FontName", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("FontStyle", typeof(string)));
            dtLikn.Columns.Add(new DataColumn("FONTSIZE", typeof(int)));

            dtLikn.Columns.Add(new DataColumn("IsBold", typeof(int)));
            dtLikn.Columns.Add(new DataColumn("IsItalic", typeof(int)));
            #endregion Link

            #region img  ImgSeal
            DataTable dtImg = new DataTable();
            dtImg.TableName = EEleTableNames.Sys_FrmImg;
            dtImg.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtImg.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtImg.Columns.Add(new DataColumn("X", typeof(double)));
            dtImg.Columns.Add(new DataColumn("Y", typeof(double)));
            dtImg.Columns.Add(new DataColumn("W", typeof(double)));
            dtImg.Columns.Add(new DataColumn("H", typeof(double)));

            dtImg.Columns.Add(new DataColumn("IMGURL", typeof(string)));
            dtImg.Columns.Add(new DataColumn("IMGPATH", typeof(string))); //应用类型 0=图片，1签章..

            dtImg.Columns.Add(new DataColumn("LINKURL", typeof(string)));
            dtImg.Columns.Add(new DataColumn("LINKTARGET", typeof(string)));
            dtImg.Columns.Add(new DataColumn("SRCTYPE", typeof(int))); //图片来源类型.
            dtImg.Columns.Add(new DataColumn("IMGAPPTYPE", typeof(int))); //应用类型 0=图片，1签章..
            dtImg.Columns.Add(new DataColumn("Tag0", typeof(string)));
            dtImg.Columns.Add(new DataColumn("ISEDIT", typeof(int)));
            dtImg.Columns.Add(new DataColumn("NAME", typeof(string)));//中文名
            dtImg.Columns.Add(new DataColumn("ENPK", typeof(string)));//英文名
            #endregion img

            #region eleDT
            DataTable dtEle = new DataTable();
            dtEle.TableName = EEleTableNames.Sys_FrmEle;
            dtEle.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtEle.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));

            //dtEle.Columns.Add(new DataColumn("EleType", typeof(string)));
            //dtEle.Columns.Add(new DataColumn("EleID", typeof(string)));
            //dtEle.Columns.Add(new DataColumn("EleName", typeof(string)));

            dtEle.Columns.Add(new DataColumn("X", typeof(double)));
            dtEle.Columns.Add(new DataColumn("Y", typeof(double)));
            dtEle.Columns.Add(new DataColumn("W", typeof(double)));
            dtEle.Columns.Add(new DataColumn("H", typeof(double)));
            #endregion eleDT

            #region Sys_FrmImgAth
            DataTable imgAthDT = new DataTable();
            imgAthDT.TableName = EEleTableNames.Sys_FrmImgAth;
            imgAthDT.Columns.Add(new DataColumn("MYPK", typeof(string)));
            imgAthDT.Columns.Add(new DataColumn("CTRLID", typeof(string)));
            imgAthDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            imgAthDT.Columns.Add(new DataColumn("ISEDIT", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("ISREQUIRED", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("X", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("Y", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("W", typeof(double)));
            imgAthDT.Columns.Add(new DataColumn("H", typeof(double)));
            #endregion Sys_FrmImgAth

            #region mapAttrDT
            DataTable mapAttrDT = new DataTable();
            mapAttrDT.TableName = EEleTableNames.Sys_MapAttr;
            mapAttrDT.Columns.Add(new DataColumn("NAME", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("MYPK", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("KEYOFEN", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("UICONTRALTYPE", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("MYDATATYPE", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("LGTYPE", typeof(string)));

            mapAttrDT.Columns.Add(new DataColumn("UIWIDTH", typeof(double)));
            mapAttrDT.Columns.Add(new DataColumn("UIHEIGHT", typeof(double)));

            mapAttrDT.Columns.Add(new DataColumn("UIBINDKEY", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("UIRefKey", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("UIRefKeyText", typeof(string)));
            //   mapAttrDT.Columns.Add(new DataColumn("UIVISIBLE", typeof(string)));
            mapAttrDT.Columns.Add(new DataColumn("X", typeof(double)));
            mapAttrDT.Columns.Add(new DataColumn("Y", typeof(double)));
            #endregion mapAttrDT

            #region frmRBDT
            DataTable dtRdb = new DataTable();
            dtRdb.TableName = EEleTableNames.Sys_FrmRB;
            dtRdb.Columns.Add(new DataColumn("MYPK", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("KEYOFEN", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("ENUMKEY", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("INTKEY", typeof(int)));
            dtRdb.Columns.Add(new DataColumn("LAB", typeof(string)));
            dtRdb.Columns.Add(new DataColumn("X", typeof(double)));
            dtRdb.Columns.Add(new DataColumn("Y", typeof(double)));
            #endregion frmRBDT

            #region Dtl
            DataTable dtlDT = new DataTable();

            dtlDT.TableName = EEleTableNames.Sys_MapDtl;
            dtlDT.Columns.Add(new DataColumn("NO", typeof(string)));
            dtlDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));

            dtlDT.Columns.Add(new DataColumn("X", typeof(double)));
            dtlDT.Columns.Add(new DataColumn("Y", typeof(double)));

            dtlDT.Columns.Add(new DataColumn("H", typeof(double)));
            dtlDT.Columns.Add(new DataColumn("W", typeof(double)));
            #endregion Dtl

            // BPWorkCheck
            DataTable dtWorkCheck = new DataTable();
            dtWorkCheck.TableName = EEleTableNames.WF_Node;
            dtWorkCheck.Columns.Add(new DataColumn("NodeID", typeof(string)));
            dtWorkCheck.Columns.Add(new DataColumn("FWC_X", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("FWC_Y", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("FWC_H", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("FWC_W", typeof(double)));

            //子流程属性.
            dtWorkCheck.Columns.Add(new DataColumn("SF_X", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("SF_Y", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("SF_H", typeof(double)));
            dtWorkCheck.Columns.Add(new DataColumn("SF_W", typeof(double)));


            #region m2mDT
            DataTable m2mDT = new DataTable();
            m2mDT.TableName = EEleTableNames.Sys_MapM2M;
            m2mDT.Columns.Add(new DataColumn("MYPK", typeof(string)));
            m2mDT.Columns.Add(new DataColumn("NOOFOBJ", typeof(string)));
            m2mDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));

            m2mDT.Columns.Add(new DataColumn("X", typeof(double)));
            m2mDT.Columns.Add(new DataColumn("Y", typeof(double)));

            m2mDT.Columns.Add(new DataColumn("H", typeof(string)));
            m2mDT.Columns.Add(new DataColumn("W", typeof(string)));
            #endregion m2mDT

            #region athDT
            DataTable athDT = new DataTable();
            athDT.TableName = EEleTableNames.Sys_FrmAttachment;
            athDT.Columns.Add(new DataColumn("MYPK", typeof(string)));
            athDT.Columns.Add(new DataColumn("FK_MAPDATA", typeof(string)));
            athDT.Columns.Add(new DataColumn("NOOFOBJ", typeof(string)));

            //athDT.Columns.Add(new DataColumn("NAME", typeof(string)));
            //athDT.Columns.Add(new DataColumn("EXTS", typeof(string)));
            //athDT.Columns.Add(new DataColumn("SAVETO", typeof(string)));
            athDT.Columns.Add(new DataColumn("UPLOADTYPE", typeof(int)));

            athDT.Columns.Add(new DataColumn("X", typeof(double)));
            athDT.Columns.Add(new DataColumn("Y", typeof(double)));
            athDT.Columns.Add(new DataColumn("W", typeof(double)));
            athDT.Columns.Add(new DataColumn("H", typeof(double)));
            #endregion athDT

            dsLatest = new DataSet();

            dsLatest.Tables.Add(dtWorkCheck);
            dsLatest.Tables.Add(dtLabel);
            dsLatest.Tables.Add(dtLikn);
            dsLatest.Tables.Add(dtImg);
            dsLatest.Tables.Add(dtEle);
            dsLatest.Tables.Add(dtBtn);
            dsLatest.Tables.Add(imgAthDT);
            dsLatest.Tables.Add(mapAttrDT);
            dsLatest.Tables.Add(dtRdb);
            dsLatest.Tables.Add(dtLine);
            dsLatest.Tables.Add(dtlDT);
            dsLatest.Tables.Add(athDT);
            dsLatest.Tables.Add(dtMapData);
            dsLatest.Tables.Add(m2mDT);
        }
        /// <summary>
        /// 保存Json
        /// </summary>
        public void SaveJson()
        {
            DataTable
            dtLine = dsLatest.Tables[EEleTableNames.Sys_FrmLine],
            dtBtn = dsLatest.Tables[EEleTableNames.Sys_FrmBtn],
            dtLabel = dsLatest.Tables[EEleTableNames.Sys_FrmLab],
            dtLink = dsLatest.Tables[EEleTableNames.Sys_FrmLink],
            dtImg = dsLatest.Tables[EEleTableNames.Sys_FrmImg],
            dtEle = dsLatest.Tables[EEleTableNames.Sys_FrmEle],
            dtImgAth = dsLatest.Tables[EEleTableNames.Sys_FrmImgAth],
            dtMapAttr = dsLatest.Tables[EEleTableNames.Sys_MapAttr],
            dtRDB = dsLatest.Tables[EEleTableNames.Sys_FrmRB],
            dtlDT = dsLatest.Tables[EEleTableNames.Sys_MapDtl],
            dtWorkCheck = dsLatest.Tables[EEleTableNames.WF_Node],
            dtM2M = dsLatest.Tables[EEleTableNames.Sys_MapM2M],
            dtAth = dsLatest.Tables[EEleTableNames.Sys_FrmAttachment];

            string pkPrefix = Glo.FK_MapData + "_";
            for (int i = 0; i < this.workSpace.Children.Count; i++)
            {
                FrameworkElement ctl = this.workSpace.Children[i] as FrameworkElement;

                // 只处理BP元素和添加数据
                if (!(ctl is IElement) || (ctl as IElement).ViewDeleted)
                    continue;

                #region 新增
                string name = ctl.Name;
                string myPk = name.Contains(pkPrefix) || name.Contains(Glo.FK_MapData) ? name : pkPrefix + name;
                MatrixTransform transform = ctl.TransformToVisual(this.workSpace) as MatrixTransform;
                double x = transform.Matrix.OffsetX;
                double y = transform.Matrix.OffsetY;
                if (x <= 0)
                    x = 0;

                if (double.IsNaN(y))
                {
                    x = Canvas.GetLeft(ctl);
                    y = Canvas.GetTop(ctl);
                }

                if (ctl is BPLine)
                {
                    #region
                    BPLine line = ctl as BPLine;
                    if (line != null)
                    {
                        DataRow drline = dtLine.NewRow();

                        drline["MYPK"] = myPk;
                        drline["FK_MAPDATA"] = Glo.FK_MapData;

                        //drline["X"] = x.ToString("0.00");
                        //drline["Y"] = y.ToString("0.00");

                        drline["X1"] = line.MyLine.X1.ToString("0.00");
                        drline["X2"] = line.MyLine.X2.ToString("0.00");
                        drline["Y1"] = line.MyLine.Y1.ToString("0.00");
                        drline["Y2"] = line.MyLine.Y2.ToString("0.00");
                        drline["BorderWidth"] = line.MyLine.StrokeThickness.ToString("0.00");
                        drline["BorderColor"] = line.Color;
                        dtLine.Rows.Add(drline);
                    }
                    #endregion
                }
                else if (ctl is BPEle)
                {
                    #region
                    BPEle ele = ctl as BPEle;
                    if (ele != null)
                    {
                        DataRow drImg = dtEle.NewRow();

                        drImg["MYPK"] = myPk;
                        drImg["FK_MAPDATA"] = Glo.FK_MapData;

                        //drImg["EleType"] = ele.EleType;
                        //drImg["EleName"] = ele.EleName;
                        //drImg["EleID"] = ele.EleID;

                        //eleDT.Columns.Add(new DataColumn("EleType", typeof(string)));
                        //eleDT.Columns.Add(new DataColumn("EleID", typeof(string)));
                        //eleDT.Columns.Add(new DataColumn("EleName", typeof(string)));

                        drImg["X"] = x.ToString("0.00");
                        drImg["Y"] = y.ToString("0.00");

                        drImg["W"] = ele.Width.ToString("0.00");
                        drImg["H"] = ele.Height.ToString("0.00");

                        dtEle.Rows.Add(drImg);

                    }
                    #endregion
                }
                else if (ctl is BPTextBox)
                {
                    #region
                    BPTextBox tb = ctl as BPTextBox;
                    if (tb != null)
                    {
                        DataRow mapAttrDR = dtMapAttr.NewRow();
                        mapAttrDR["MYPK"] = myPk;
                        mapAttrDR["NAME"] = tb.KeyName;
                        mapAttrDR["KEYOFEN"] = tb.Name;
                        mapAttrDR["FK_MAPDATA"] = Glo.FK_MapData;
                        mapAttrDR["UICONTRALTYPE"] = CtrlType.TextBox;
                        mapAttrDR["MYDATATYPE"] = tb.HisDataType;
                        mapAttrDR["UIWIDTH"] = tb.Width.ToString("0.00");
                        mapAttrDR["UIHEIGHT"] = tb.Height.ToString("0.00");
                        mapAttrDR["LGTYPE"] = LGType.Normal;

                        mapAttrDR["X"] = x.ToString("0.00");
                        mapAttrDR["Y"] = y.ToString("0.00");
                        // mapAttrDR["UIVISIBLE"] = "1";
                        dtMapAttr.Rows.Add(mapAttrDR);
                    }
                    #endregion
                }
                else if (ctl is BPImg)
                {
                    #region
                    BPImg img = ctl as BPImg;
                    if (img != null)
                    {
                        DataRow drImg = dtImg.NewRow();

                        drImg["MYPK"] = myPk;
                        drImg["FK_MAPDATA"] = Glo.FK_MapData;

                        drImg["X"] = x.ToString("0.00"); // Canvas.GetLeft(ctl).ToString("0.00");
                        drImg["Y"] = y.ToString("0.00"); // Canvas.GetTop(ctl).ToString("0.00");

                        drImg["W"] = img.Width.ToString("0.00");
                        drImg["H"] = img.Height.ToString("0.00");

                        BitmapImage png = img.HisPng;

                        drImg["LINKURL"] = img.LinkURL;
                        drImg["LINKTARGET"] = img.LinkTarget;
                        drImg["SRCTYPE"] = img.SrcType.ToString();

                        drImg["IMGPATH"] = png.UriSource.ToString().Contains("DataUser") ? png.UriSource.ToString().Replace(Glo.BPMHost, "") : png.UriSource.ToString();
                        drImg["IMGURL"] = img.ImgURL;

                        drImg["IMGAPPTYPE"] = "0";
                        drImg["ISEDIT"] = "1";
                        drImg["NAME"] = img.TB_CN_Name;
                        drImg["ENPK"] = img.TB_En_Name;
                        dtImg.Rows.Add(drImg);

                    }
                    #endregion
                }
                else if (ctl is BPImgAth)
                {
                    #region
                    BPImgAth imgAth = ctl as BPImgAth;
                    if (imgAth != null)
                    {
                        DataRow mapAth = dtImgAth.NewRow();
                        mapAth["MYPK"] = myPk;
                        mapAth["CTRLID"] = imgAth.CtrlID; //附件ID.
                        mapAth["FK_MAPDATA"] = Glo.FK_MapData;
                        mapAth["ISEDIT"] = imgAth.IsEdit ? "1" : "0";
                        mapAth["ISREQUIRED"] = imgAth.IsRequired ? "1" : "0";

                        mapAth["X"] = x.ToString("0.00");
                        mapAth["Y"] = y.ToString("0.00");

                        mapAth["W"] = imgAth.Width.ToString("0.00");
                        mapAth["H"] = imgAth.Height.ToString("0.00");
                        dtImgAth.Rows.Add(mapAth);

                    }
                    #endregion
                }
                else if (ctl is BPImgSeal)
                {
                    #region
                    BPImgSeal imgSeal = ctl as BPImgSeal;
                    if (imgSeal != null)
                    {
                        DataRow drImgSeal = dtImg.NewRow();

                        drImgSeal["MYPK"] = myPk;
                        drImgSeal["FK_MAPDATA"] = Glo.FK_MapData;
                        drImgSeal["IMGAPPTYPE"] = "1";


                        drImgSeal["X"] = x.ToString("0.00");
                        drImgSeal["Y"] = y.ToString("0.00");

                        drImgSeal["W"] = imgSeal.Width.ToString("0.00");
                        drImgSeal["H"] = imgSeal.Height.ToString("0.00");

                        BitmapImage png = imgSeal.HisPng;
                        drImgSeal["IMGURL"] = png.UriSource.ToString();
                        drImgSeal["Tag0"] = imgSeal.Tag0;
                        drImgSeal["NAME"] = imgSeal.TB_CN_Name;
                        drImgSeal["ENPK"] = imgSeal.TB_En_Name;
                        drImgSeal["ISEDIT"] = imgSeal.IsEdit ? "1" : "0";
                        dtImg.Rows.Add(drImgSeal);

                    }
                    #endregion
                }
                else if (ctl is BPLabel)
                {
                    #region
                    BPLabel lab = ctl as BPLabel;
                    if (lab != null)
                    {
                        DataRow drLab = dtLabel.NewRow();

                        drLab["MYPK"] = myPk;
                        drLab["TEXT"] = lab.Content.ToString().Replace(" ", "&nbsp;").Replace("\n", "@");
                        drLab["FK_MAPDATA"] = Glo.FK_MapData;

                        drLab["X"] = x.ToString("0.00");
                        drLab["Y"] = y.ToString("0.00");

                        // drLab["FONTCOLOR"] = lab.GetValue( lapp ).ToString();
#warning 如何获取字体颜色 ? .

                        SolidColorBrush d = (SolidColorBrush)lab.Foreground;
                        drLab["FONTCOLOR"] = d.Color.ToString();
                        // Glo.PreaseColorToName(d.Color.ToString());
                        drLab["FontName"] = lab.FontFamily.ToString();
                        drLab["FontStyle"] = lab.FontStyle.ToString();
                        drLab["FONTSIZE"] = lab.FontSize.ToString();

                        if (lab.FontWeight == FontWeights.Normal)
                            drLab["IsBold"] = "0";
                        else
                            drLab["IsBold"] = "1";

                        if (lab.FontStyle.ToString() == "Italic")
                            drLab["IsItalic"] = "1";
                        else
                            drLab["IsItalic"] = "0";

                        dtLabel.Rows.Add(drLab);

                    }
                    #endregion
                }
                else if (ctl is BPLink)
                {
                    #region
                    BPLink link = ctl as BPLink;
                    if (link != null)
                    {
                        DataRow drLink = dtLink.NewRow();

                        drLink["MYPK"] = myPk;

                        drLink["TEXT"] = link.Content.ToString();
                        drLink["FK_MAPDATA"] = Glo.FK_MapData;

                        drLink["X"] = x.ToString("0.00");
                        drLink["Y"] = y.ToString("0.00");

                        SolidColorBrush d = (SolidColorBrush)link.Foreground;
                        drLink["FONTCOLOR"] = Glo.PreaseColorToName(d.Color.ToString());
                        drLink["FontName"] = link.FontFamily.ToString();
                        drLink["FontStyle"] = link.FontStyle.ToString();
                        drLink["FONTSIZE"] = link.FontSize.ToString();
                        drLink["URL"] = link.URL;
                        drLink["TARGET"] = link.WinTarget;

                        if (link.FontWeight == FontWeights.Normal)
                            drLink["IsBold"] = "0";
                        else
                            drLink["IsBold"] = "1";

                        if (link.FontStyle.ToString() == "Italic")
                            drLink["IsItalic"] = "1";
                        else
                            drLink["IsItalic"] = "0";

                        dtLink.Rows.Add(drLink);

                    }
                    #endregion
                }
                else if (ctl is BPAttachment || ctl is BPAttachmentM)
                {
                    #region
                    FrameworkElement athCtl = ctl as FrameworkElement;
                    if (athCtl != null)
                    {

                        DataRow mapAth = dtAth.NewRow();

                        mapAth["MYPK"] = myPk;
                        mapAth["FK_MAPDATA"] = Glo.FK_MapData;
                        mapAth["NOOFOBJ"] = athCtl.Name;

                        mapAth["X"] = x.ToString("0.00");
                        mapAth["Y"] = y.ToString("0.00");

                        if (ctl is BPAttachment)
                        {
                            mapAth["W"] = (athCtl as BPAttachment).HisTB.Width.ToString("0.00");
                            mapAth["UPLOADTYPE"] = "0";
                        }
                        else if (ctl is BPAttachmentM)
                        {
                            mapAth["UPLOADTYPE"] = "1";
                            mapAth["W"] = athCtl.Width.ToString("0.00");
                            mapAth["H"] = athCtl.Height.ToString("0.00");
                        }
                        dtAth.Rows.Add(mapAth);
                    }
                    #endregion
                }
                else if (ctl is BPDtl)
                {
                    #region
                    BPDtl dtlCtl = ctl as BPDtl;
                    if (dtlCtl != null)
                    {
                        DataRow mapDtl = dtlDT.NewRow();

                        mapDtl["NO"] = myPk;
                        mapDtl["FK_MAPDATA"] = Glo.FK_MapData;

                        mapDtl["X"] = x.ToString("0.00");
                        mapDtl["Y"] = y.ToString("0.00");
                        mapDtl["W"] = dtlCtl.Width.ToString("0.00");
                        mapDtl["H"] = dtlCtl.Height.ToString("0.00");
                        dtlDT.Rows.Add(mapDtl);

                    }
                    #endregion
                }
                else if (ctl is BPWorkCheck)
                {
                    #region  审核组件
                    BPWorkCheck wkCheck = ctl as BPWorkCheck;
                    if (wkCheck != null)
                    {
                        if (dtWorkCheck.Rows.Count == 0)
                        {
                            DataRow workCheckDt = dtWorkCheck.NewRow();
                            dtWorkCheck.Rows.Add(workCheckDt);
                        }

                        dtWorkCheck.Rows[0]["NodeID"] = Glo.FK_MapData.Replace("ND", "");
                        dtWorkCheck.Rows[0]["FWC_X"] = x.ToString("0.00");
                        dtWorkCheck.Rows[0]["FWC_Y"] = y.ToString("0.00");
                        dtWorkCheck.Rows[0]["FWC_W"] = wkCheck.Width.ToString("0.00");
                        dtWorkCheck.Rows[0]["FWC_H"] = wkCheck.Height.ToString("0.00");
                    }
                    #endregion
                }
                else if (ctl is BPSubFlow)
                {
                    #region  父子流程组件.
                    BPSubFlow wkSubFlow = ctl as BPSubFlow;
                    if (wkSubFlow != null)
                    {
                        if (dtWorkCheck.Rows.Count == 0)
                        {
                            DataRow workCheckDt = dtWorkCheck.NewRow();
                            dtWorkCheck.Rows.Add(workCheckDt);
                        }
                        dtWorkCheck.Rows[0]["NodeID"] = Glo.FK_MapData.Replace("ND", "");
                        dtWorkCheck.Rows[0]["SF_X"] = x.ToString("0.00");
                        dtWorkCheck.Rows[0]["SF_Y"] = y.ToString("0.00");
                        dtWorkCheck.Rows[0]["SF_W"] = wkSubFlow.Width.ToString("0.00");
                        dtWorkCheck.Rows[0]["SF_H"] = wkSubFlow.Height.ToString("0.00");
                    }
                    #endregion
                }
                else if (ctl is BPM2M)
                {
                    #region
                    BPM2M m2mCtl = ctl as BPM2M;
                    if (m2mCtl != null)
                    {
                        DataRow rowM2M = dtM2M.NewRow();
                        rowM2M["NOOFOBJ"] = myPk.Replace(pkPrefix, "").Replace(Glo.FK_MapData, "");
                        rowM2M["FK_MAPDATA"] = Glo.FK_MapData;
                        rowM2M["MYPK"] = myPk;
                        rowM2M["X"] = x.ToString("0.00");
                        rowM2M["Y"] = y.ToString("0.00");
                        rowM2M["W"] = m2mCtl.Width.ToString("0.00");
                        rowM2M["H"] = m2mCtl.Height.ToString("0.00");

                        dtM2M.Rows.Add(rowM2M);
                    }
                    #endregion
                }
                else if (ctl is BPDatePicker)
                {
                    #region
                    BPDatePicker dp = ctl as BPDatePicker;
                    if (dp != null)
                    {
                        DataRow mapAttrDR = dtMapAttr.NewRow();

                        mapAttrDR["MYPK"] = myPk;
                        mapAttrDR["FK_MAPDATA"] = Glo.FK_MapData;
                        mapAttrDR["KEYOFEN"] = myPk.Replace(pkPrefix, "").Replace(Glo.FK_MapData, "");

                        mapAttrDR["UICONTRALTYPE"] = CtrlType.TextBox;
                        mapAttrDR["MYDATATYPE"] = dp.HisDateType;
                        mapAttrDR["LGTYPE"] = LGType.Normal;

                        mapAttrDR["X"] = x.ToString("0.00");
                        mapAttrDR["Y"] = y.ToString("0.00");

                        // mapAttrDR["UIVISIBLE"] = "1";
                        mapAttrDR["UIWIDTH"] = "50";
                        mapAttrDR["UIHEIGHT"] = "23";

                        dtMapAttr.Rows.Add(mapAttrDR);

                    }
                    continue;
                    #endregion
                }
                else if (ctl is BPBtn)
                {
                    #region
                    BPBtn btn = ctl as BPBtn;
                    if (btn != null)
                    {
                        DataRow drBtn = dtBtn.NewRow();

                        drBtn["MYPK"] = myPk;
                        drBtn["TEXT"] = btn.Content.ToString().Replace(" ", "&nbsp;").Replace("\n", "@");
                        drBtn["FK_MAPDATA"] = Glo.FK_MapData;

                        drBtn["X"] = x.ToString("0.00");
                        drBtn["Y"] = y.ToString("0.00");
                        dtBtn.Rows.Add(drBtn);
                    }
                    #endregion
                }
                else if (ctl is BPDDL)
                {
                    #region
                    BPDDL ddl = ctl as BPDDL;
                    if (ddl != null)
                    {
                        DataRow mapAttrDR = dtMapAttr.NewRow();

                        mapAttrDR["MYPK"] = myPk;
                        mapAttrDR["FK_MAPDATA"] = Glo.FK_MapData;
                        mapAttrDR["KEYOFEN"] = myPk.Replace(pkPrefix, "").Replace(Glo.FK_MapData, "");
                        mapAttrDR["UICONTRALTYPE"] = CtrlType.DDL;


                        mapAttrDR["LGTYPE"] = ddl.HisLGType;

                        mapAttrDR["MYDATATYPE"] = ddl.HisDataType;
                        mapAttrDR["UIWIDTH"] = ddl.Width.ToString("0.00");
                        mapAttrDR["UIHEIGHT"] = "23";
                        mapAttrDR["X"] = x.ToString("0.00");
                        mapAttrDR["Y"] = y.ToString("0.00");
                        mapAttrDR["UIBINDKEY"] = ddl.UIBindKey;
                        mapAttrDR["UIRefKey"] = "NO";
                        mapAttrDR["UIRefKeyText"] = "NAME";
                        //     mapAttrDR["UIVISIBLE"] = "1";
                        dtMapAttr.Rows.Add(mapAttrDR);
                    }
                    #endregion
                }
                else if (ctl is BPCheckBox)
                {
                    #region
                    BPCheckBox cb = ctl as BPCheckBox;
                    if (cb != null)
                    {
                        DataRow mapAttrDR = dtMapAttr.NewRow();
                        mapAttrDR["FK_MAPDATA"] = Glo.FK_MapData;
                        mapAttrDR["KEYOFEN"] = myPk.Replace(pkPrefix, "").Replace(Glo.FK_MapData, "");
                        mapAttrDR["MYPK"] = myPk;
                        mapAttrDR["NAME"] = cb.KeyName;

                        mapAttrDR["UICONTRALTYPE"] = CtrlType.CheckBox;
                        mapAttrDR["MYDATATYPE"] = DataType.AppBoolean;
                        mapAttrDR["LGTYPE"] = LGType.Normal;
                        mapAttrDR["X"] = x.ToString("0.00");
                        mapAttrDR["Y"] = y.ToString("0.00");
                        mapAttrDR["UIWIDTH"] = "100";
                        mapAttrDR["UIHEIGHT"] = "23";

                        dtMapAttr.Rows.Add(mapAttrDR);
                    }
                    #endregion
                }
                else if (ctl is BPRadioBtn)
                {
                    #region
                    BPRadioBtn rb = ctl as BPRadioBtn;
                    if (rb != null)
                    {
                        DataRow mapAttrRB = dtRDB.NewRow();

                        mapAttrRB["MYPK"] = myPk;
                        mapAttrRB["FK_MAPDATA"] = Glo.FK_MapData;
                        mapAttrRB["KEYOFEN"] = rb.GroupName;
                        mapAttrRB["INTKEY"] = rb.Tag.ToString();
                        mapAttrRB["LAB"] = rb.Content as string;
                        mapAttrRB["ENUMKEY"] = rb.UIBindKey;
                        mapAttrRB["X"] = x.ToString("0.00");
                        mapAttrRB["Y"] = y.ToString("0.00");
                        dtRDB.Rows.Add(mapAttrRB);
                    }
                    #endregion
                }
                else if (ctl is BPMapPin)
                {
                    #region
                    BPMapPin mapPin = ctl as BPMapPin;
                    if (mapPin != null)
                    {
                        DataRow mapAttrDR = dtMapAttr.NewRow();
                        mapAttrDR["MYPK"] = mapPin.MyPK;
                        mapAttrDR["NAME"] = mapPin.KeyName;
                        mapAttrDR["KEYOFEN"] = mapPin.Name;
                        mapAttrDR["FK_MAPDATA"] = Glo.FK_MapData;
                        mapAttrDR["UICONTRALTYPE"] = CtrlType.MapPin;
                        mapAttrDR["MYDATATYPE"] = DataType.AppString;
                        mapAttrDR["UIWIDTH"] = mapPin.Width.ToString("0.00");
                        mapAttrDR["UIHEIGHT"] = mapPin.Height.ToString("0.00");
                        mapAttrDR["LGTYPE"] = LGType.WinOpen;

                        mapAttrDR["X"] = x.ToString("0.00");
                        mapAttrDR["Y"] = y.ToString("0.00");
                        dtMapAttr.Rows.Add(mapAttrDR);
                    }
                    #endregion
                }
                else if (ctl is BPMicrophonehot)
                {
                    #region
                    BPMicrophonehot micHot = ctl as BPMicrophonehot;
                    if (micHot != null)
                    {
                        DataRow mapAttrDR = dtMapAttr.NewRow();
                        mapAttrDR["MYPK"] = micHot.MyPK;
                        mapAttrDR["NAME"] = micHot.KeyName;
                        mapAttrDR["KEYOFEN"] = micHot.Name;
                        mapAttrDR["FK_MAPDATA"] = Glo.FK_MapData;
                        mapAttrDR["UICONTRALTYPE"] = CtrlType.MicHot;
                        mapAttrDR["MYDATATYPE"] = DataType.AppBoolean;
                        mapAttrDR["UIWIDTH"] = micHot.Width.ToString("0.00");
                        mapAttrDR["UIHEIGHT"] = micHot.Height.ToString("0.00");
                        mapAttrDR["LGTYPE"] = LGType.WinOpen;

                        mapAttrDR["X"] = x.ToString("0.00");
                        mapAttrDR["Y"] = y.ToString("0.00");
                        dtMapAttr.Rows.Add(mapAttrDR);
                    }
                    #endregion
                }
                #endregion
            }

            #region 处理 RB 枚举值
            string keys = "";
            foreach (DataRow dr in dtRDB.Rows)
            {
                string keyOfEn = dr["KEYOFEN"];
                if (keys.Contains("@" + keyOfEn + "@"))
                    continue;
                else
                    keys += "@" + keyOfEn + "@";


                string enumKey = dr["ENUMKEY"];
                DataRow mapAttrDR = dtMapAttr.NewRow();
                mapAttrDR["MYPK"] = pkPrefix + keyOfEn;
                mapAttrDR["FK_MAPDATA"] = Glo.FK_MapData;
                mapAttrDR["KEYOFEN"] = keyOfEn;

                mapAttrDR["UICONTRALTYPE"] = CtrlType.RB;
                mapAttrDR["MYDATATYPE"] = DataType.AppInt;
                mapAttrDR["LGTYPE"] = LGType.Enum;
                mapAttrDR["X"] = "0";
                mapAttrDR["Y"] = "0";
                mapAttrDR["UIBINDKEY"] = enumKey;
                mapAttrDR["UIRefKey"] = "NO";
                mapAttrDR["UIRefKeyText"] = "NAME";
                mapAttrDR["UIWIDTH"] = "30";
                mapAttrDR["UIHEIGHT"] = "23";
                dtMapAttr.Rows.Add(mapAttrDR);
            }
            #endregion

            string sqls = "",
                table = string.Empty,
                len = Glo.LEN_Function;

            foreach (UIElement ctl in this.workSpace.Children)
            {
                // 只处理BP元素和添加数据
                if (!(ctl is IElement) || (ctl as IElement).ViewDeleted)
                    continue;

                #region UPDATE
                if (ctl is BPCheckBox)
                {
                    BPCheckBox cb = ctl as BPCheckBox;
                    if (null == cb || string.IsNullOrEmpty(cb.KeyName)) continue;

                    Label mylab = cb.Content as Label;
                    sqls += "@UPDATE Sys_MapAttr SET Name='" + mylab.Content + "'  WHERE MyPK='" + cb.Name + "' AND " + len + "(Name)=0";
                    continue;

                }
                else if (ctl is BPTextBox)
                {
                    BPTextBox tb = ctl as BPTextBox;
                    if (tb == null || string.IsNullOrEmpty(tb.KeyName))
                        continue;

                    sqls += "@UPDATE Sys_MapAttr SET Name='" + tb.KeyName + "' WHERE MyPK='" + tb.Name + "' AND ( " + len + "(Name)=0 OR KeyOfEn=Name )";
                    continue;

                }
                else if (ctl is BPDDL)
                {
                    BPDDL ddl = ctl as BPDDL;

                    if (ddl == null || string.IsNullOrEmpty(ddl.KeyName))
                        continue;

                    sqls += "@UPDATE Sys_MapAttr SET Name='" + ddl.KeyName + "' WHERE MyPK='" + Glo.FK_MapData + "_" + ddl.Name + "' AND " + len + "(Name)=0";
                    continue;

                }
                else if (ctl is BPRadioBtn)
                {
                    BPRadioBtn rb = ctl as BPRadioBtn;
                    if (rb == null || string.IsNullOrEmpty(rb.KeyName) || sqls.Contains("_" + rb.GroupName))
                        continue;

                    sqls += "@UPDATE Sys_MapAttr SET Name='" + rb.KeyName + "' WHERE MyPK='" + Glo.FK_MapData + "_" + rb.GroupName + "' AND " + len + "(Name)=0";
                    continue;
                }
                #endregion
            }

            foreach (KeyValuePair<string, JsonValue> item in jsonObject)
            {
                #region DELETE
                if (item.Value.Count <= 0)
                    continue;
                table = item.Key;
                if (
                    //table == EEleTableNames.Sys_FrmAttachment
                    //||table == EEleTableNames.Sys_MapDtl
                    //|| table == EEleTableNames.Sys_MapM2M
                    //|| table == EEleTableNames.Sys_FrmRB
                    //|| table == EEleTableNames.WF_Node  || // 这些元素实现了IDelete 接口，有自己的sql

                    table == EEleTableNames.Sys_MapData // 表单宽高
                    )
                {
                    continue;
                }

                DataTable newDt = dsLatest.Tables[table];
                if (newDt == null)
                    continue;

                string pk = "";
                string tmpStr = string.Empty;
                foreach (JsonValue dr in item.Value)
                {
                    if (dr.Count == 0)
                        continue;

                    #region 求pK
                    if (dr.ContainsKey("MyPK") || dr.ContainsKey("MYPK"))
                    {
                        pk = "MYPK";
                    }
                    else if (dr.ContainsKey("No") || dr.ContainsKey("NO"))
                    {
                        pk = "NO";
                    }
                    else if (dr.ContainsKey("oid") || dr.ContainsKey("OID"))
                    {
                        pk = "OID";
                    }
                    else if (dr.ContainsKey("nodeid") || dr.ContainsKey("NodeID") || dr.ContainsKey("NODEID"))
                    {
                        pk = "NODEID";
                    }
                    #endregion 求pK

                    if (!string.IsNullOrEmpty(pk))
                        break;
                }

                foreach (JsonValue dr in item.Value)
                {
                    if (dr.Count == 0)
                        continue;

                    bool isStillExit = false;
                    string pkVal = dr[pk].ToString().Replace("\"", "");
                    if (table == EEleTableNames.WF_Node)
                    {
                        #region WF_Node,不包含FK_MapData暂放在前面。
                        if (pkVal != Glo.FK_MapData.Replace("ND", ""))
                            continue;

                        // 判断的审核组件 
                        isStillExit = false;
                        foreach (DataRow newDr in newDt.Rows)
                        {
                            if ((string)newDr[pk] == pkVal)
                            {
                                isStillExit = true;
                                break;
                            }
                        }

                        //if (!isStillExit)
                        //    sqls += "@UPDATE WF_Node SET FWCSta=0 WHERE " + pk + "='" + pkVal + "'";

                        break;
                        #endregion
                    }

                    if ((string)dr["FK_MAPDATA"] != Glo.FK_MapData)
                        continue;

                    isStillExit = false;
                    if (table == EEleTableNames.Sys_MapAttr) /* 如果是字段控件 .. */
                    {
                        foreach (DataRow newDr in newDt.Rows)
                        {
                            string tmp = newDr[pk].ToString();
                            if (dr["UIVISIBLE"] == 0 || tmp == pkVal)
                            {
                                isStillExit = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (DataRow newDr in newDt.Rows)
                        {
                            string tmp = newDr[pk].ToString();
                            if (tmp == pkVal)
                            {
                                isStillExit = true;
                                break;
                            }
                        }
                    }

                    if (isStillExit == false)
                        sqls += "@DELETE FROM " + table + " WHERE " + pk + "='" + pkVal + "'";
                }
                #endregion
            }

            try
            {
                string xml = Glo.ToJson(dsLatest);
                FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                da.SaveFrmAsync(Glo.FK_MapData, xml, sqls, Glo.UserNo, Glo.SID);
                da.SaveFrmCompleted += ((object senders, FF.SaveFrmCompletedEventArgs ee) =>
                {
                    isDesignerSizeChanged = false;
                    loadingWindow.DialogResult = true;
                    #region
                    if (ee.Error != null)
                    {
                        BP.SL.LoggerHelper.Write(ee.Error);
                        MessageBox.Show(ee.Result, "保存错误", MessageBoxButton.OK);
                        return;
                    }

                    if (Keyboard.Modifiers == ModifierKeys.Windows)
                    {
                        string url1 = null;
                        if (Glo.IsDtlFrm == false)
                            url1 = Glo.BPMHost + "/WF/CCForm/Frm.aspx?FK_MapData=" + Glo.FK_MapData + "&IsTest=1&WorkID=0&FK_Node=" + Glo.FK_Node + "&sd=s" + Glo.TimeKey;
                        else
                            url1 = Glo.BPMHost + "/WF/CCForm/FrmCard.aspx?EnsName=" + Glo.FK_MapData + "&RefPKVal=0&OID=0" + Glo.TimeKey;

                        Glo.WinOpen(url1, (int)Glo.HisMapData.FrmH, (int)Glo.HisMapData.FrmW);
                    }
                    else
                        this.BindFrm();
                    #endregion
                });
            }
            catch (Exception e)
            {
                loadingWindow.DialogResult = false;
                MessageBox.Show("保存错误:" + e.Message);
            }
        }
        #endregion

        DateTime _lastTime;
        void UIElement_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (null != e)
                e.Handled = true;

            FrameworkElement element = sender as FrameworkElement;

            IElement ele = element as IElement;
            bool dbClicked = false;
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {// 多选
                bool op;
                if (op = selectedElements.Contains(element))// 相同元素 选中 --> 取消选中
                {
                    selectedElements.Remove(element);
                }
                else
                {
                    selectedElements.Add(element);
                }

                if (ele != null)
                {
                    ele.IsSelected = !op;
                }
            }
            else
            {
                if (element is BPLine)
                {

                }
                else
                {
                    // 同一个元素在指定时间内单击两次为双击
                    if (Glo.currEle == element && DateTime.Now.Subtract(_lastTime).TotalMilliseconds < 300)
                    {
                        dbClicked = true;
                    }
                    else
                    {
                        Glo.currEle = element;
                        _lastTime = DateTime.Now;
                        dbClicked = false;
                    }
                }

                //// 单选
                //if (selectedElements.Contains(element))
                //{
                //    if ((ele = element as IElement) != null)
                //    {
                //        ele.IsSelected = false;
                //    }
                //}
                //else
                {
                    foreach (UIElement en in this.selectedElements)
                    {
                        if (en is IElement && (ele = en as IElement) != null)
                        {
                            ele.IsSelected = false;
                        }
                    }

                    this.selectedElements.Clear();
                    if ((ele = element as IElement) != null)
                    {
                        ele.IsSelected = true;
                    }
                    this.selectedElements.Add(element);
                }
            }


            Glo.SetTracking(ele, !dbClicked);
            if (dbClicked || ele is BPLine)
            {
                UIElementDbClickEdit(sender);
            }
            else
            {
                MouseEventHandlers.pointFrom = e.GetPosition(null);
            }
        }
        void UIElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {

            }
        }
        void UIElement_MouseLeave(object sender, MouseEventArgs e)
        {
            if (null != cursor && (this.isToolDraging || this.selectType.Equals(ToolBox.Line)))
            {
                cCursor.SetCursorTemplate(cursor);
            }
            else cCursor.SetCursorDefault(Cursors.Arrow);
        }
        void UIElement_MouseEnter(object sender, MouseEventArgs e)
        {
            cCursor.SetCursorDefault(Cursors.None);
        }
        void UIElementDbClickEdit(object sender)
        {// 元素双击 或 快捷菜单 调用编辑

            Glo.currEle = sender as UIElement;

            if (sender is BPBtn)
            {
                BPBtn btn = sender as BPBtn;
                if (btn != null)
                {
                    this.winFrmBtn.HisBtn = btn;
                    this.winFrmBtn.Show();
                }
            }
            else if (sender is BPLine)
            {
                BPLine line = sender as BPLine;
                if (line != null)
                {
                    this.currLine = line;
                    lineEdit(line);
                }
            }
            else if (sender is BPLabel)
            {
                BPLabel lab = sender as BPLabel;
                if (lab != null)
                {
                    this.currLab = lab;
                    this.winFrmLab.BindIt(this.currLab);
                }
            }
            else if (sender is BPLink)
            {
                BPLink link = sender as BPLink;
                if (link != null)
                {
                    this.currLink = link;
                    this.winFrmLink.BindIt(this.currLink);
                }
            }
            else if (sender is BPImg)
            {
                BPImg img = sender as BPImg;
                if (img != null)
                    this.winFrmImg.BindIt(img);
            }
            else if (sender is BPImgAth)
            {
                BPImgAth imgAth = sender as BPImgAth;
                if (imgAth != null)
                    this.winFrmImgAth.BindIt(imgAth);
            }
            else if (sender is BPImgSeal)
            {
                BPImgSeal imgSeal = sender as BPImgSeal;
                if (imgSeal != null)
                    this.winFrmImgSeal.BindIt(imgSeal);
            }
            else if (sender is BPEle)
            {
                BPEle el = sender as BPEle;
                if (el != null)
                {
                    this.winFrmEle.BindData(el.Name);
                    this.winFrmEle.Show();
                }
            }
            else if (sender is BPMapPin)
            {
                BPMapPin mapPin = sender as BPMapPin;
                if (mapPin != null)
                {
                    this.winFrmEleMapPin.BindIt(mapPin);
                    this.winFrmEleMapPin.Show();
                }
            }
            else if (sender is BPMicrophonehot)
            {
                BPMicrophonehot micHot = sender as BPMicrophonehot;
                if (micHot != null)
                {
                    this.winFrmEleMicHot.BindIt(micHot);
                    this.winFrmEleMicHot.Show();
                }
            }
            else if (sender is BPWorkCheck)
            {
                //BPWorkCheck workCheck = sender as BPWorkCheck;
                //if (workCheck != null)
                //    this.winWorkCheck.BindIt(workCheck);
                string url = Glo.BPMHost + @"/WF/Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmNodeComponent&PK=" + Glo.FK_MapData.Replace("ND", "") + "&tab=审核组件";

                //  string url = Glo.BPMHost + @"/WF/Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmWorkCheck&PK=" + Glo.FK_MapData.Replace("ND", "");
                Glo.WinOpenDialog(url);
            }
            else if (sender is BPSubFlow)
            {
                string url = Glo.BPMHost + @"/WF/Comm/RefFunc/UIEn.aspx?EnName=BP.WF.Template.FrmNodeComponent&PK=" + Glo.FK_MapData.Replace("ND", "") + "&tab=子流程";
                Glo.WinOpenDialog(url);
            }
            else
            {
                string host = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/Do.aspx?DoType=CCForm";

                if (sender is BPTextBox)
                {
                    BPTextBox tb = sender as BPTextBox;
                    if (tb != null)
                    {
                        if (tb.NameOfReal == null)
                            return;
                        string keyName = HttpUtility.UrlEncode(tb.KeyName);
                        string url = host + "&FK_MapData=" + Glo.FK_MapData + "&MyPK=" + Glo.FK_MapData + "_" + tb.Name + "&DataType=" + tb.HisDataType + "&GroupField=0&LGType=" + LGType.Normal
                            + "&KeyOfEn=" + tb.Name + "&UIContralType=" + CtrlType.TextBox + "&KeyName=" + keyName + Glo.TimeKey;
                        Glo.WinOpenDialog(url, 500, 600);
                    }
                }
                else if (sender is BPCheckBox)
                {
                    BPCheckBox cb = sender as BPCheckBox;
                    if (cb != null)
                    {
                        string keyName = HttpUtility.UrlEncode(cb.KeyName);
                        string url = host + "&FK_MapData=" + Glo.FK_MapData + "&MyPK=" + Glo.FK_MapData + "_" + cb.Name + "&DataType=" + DataType.AppBoolean + "&GroupField=0&LGType=" + LGType.Normal
                            + "&KeyOfEn=" + cb.Name + "&UIContralType=" + CtrlType.CheckBox + "&KeyName=" + keyName;
                        Glo.WinOpenDialog(url, 500, 600);
                    }
                }
                else if (sender is BPDatePicker)
                {
                    BPDatePicker dp = sender as BPDatePicker;
                    if (dp != null)
                    {
                        string keyName = HttpUtility.UrlEncode(dp.KeyName);
                        string url = host + "&FK_MapData=" + Glo.FK_MapData + "&MyPK=" + Glo.FK_MapData + "_" + dp.Name + "&DataType=" + dp.HisDateType + "&GroupField=0&LGType=" + LGType.Normal + "&KeyOfEn=" + dp.Name + "&UIContralType=" + CtrlType.TextBox + "&KeyName=" + keyName;
                        Glo.WinOpenDialog(url, 500, 600);
                    }
                }
                else if (sender is BPDDL)
                {
                    BPDDL ddl = sender as BPDDL;
                    if (ddl != null)
                    {
                        string keyName = HttpUtility.UrlEncode(ddl.KeyName);
                        string url = host + "&FK_MapData=" + Glo.FK_MapData + "&MyPK=" + Glo.FK_MapData + "_" + ddl.Name + "&DataType=" + ddl.HisDataType + "&GroupField=0&LGType=" + ddl.HisLGType
                            + "&KeyOfEn=" + ddl.Name + "&UIBindKey=" + ddl.UIBindKey + "&UIContralType=" + CtrlType.DDL + "&KeyName=" + keyName;
                        Glo.WinOpenDialog(url, 500, 600);
                    }
                }
                else if (sender is BPRadioBtn)
                {
                    BPRadioBtn rb = sender as BPRadioBtn;
                    if (rb != null)
                    {
                        string keyName = HttpUtility.UrlEncode(rb.KeyName);
                        string url = host + "&FK_MapData=" + Glo.FK_MapData + "&MyPK=" + Glo.FK_MapData + "_" + rb.GroupName + "&DataType=" + DataType.AppInt + "&GroupField=0&LGType=" + LGType.Enum
                            + "&KeyOfEn=" + rb.GroupName + "&UIBindKey=" + rb.UIBindKey + "&UIContralType=" + CtrlType.RB;
                        Glo.WinOpenDialog(url, 500, 600);
                    }
                }
                else if (sender is BPDtl)
                {
                    BPDtl dtl = sender as BPDtl;
                    if (dtl != null)
                    {
                        Glo.OpenDtl(Glo.FK_MapData, dtl.Name);
                    }
                }
                else if (sender is BPM2M)
                {
                    BPM2M m2m = sender as BPM2M;
                    if (m2m != null)
                    {
                        Glo.OpenM2M(Glo.FK_MapData, m2m.Name + Glo.TimeKey);
                    }
                }
                else if (sender is BPAttachment)
                {
                    BPAttachment ath = sender as BPAttachment;
                    if (ath != null)
                    {
                        //this.winSelectAttachment.BindIt(ath);
                        string url = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/Attachment.aspx?FK_MapData=" + Glo.FK_MapData + "&Ath=" + ath.Name + Glo.TimeKey;
                        Glo.WinOpen(url, 600, 800);
                    }
                }
                else if (sender is BPAttachmentM)
                {
                    BPAttachmentM athm = sender as BPAttachmentM;
                    if (athm != null)
                    {
                        string url = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/Attachment.aspx?FK_MapData=" + Glo.FK_MapData + "&Ath=" + athm.Name + Glo.TimeKey;
                        Glo.WinOpen(url, 600, 800);
                    }
                }
            }
        }

        private void lbTools_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string toolName = string.Empty;
            e.Handled = true;

            FrameworkElement ele = e.OriginalSource as FrameworkElement;
            ele = sender as FrameworkElement;
            if (ele is ListBox)
            {
                CCForm.ToolBox o = lbTools.SelectedValue as CCForm.ToolBox;
                toolName = o.IcoName;
            }
            else
            {
                return;
            }

            this.SetSelectedTool(toolName);

            // 画线 和框选 需要手动取消
            if (ToolBox.Line.Equals(toolName) || ToolBox.Selected.Equals(toolName))
                return;

            isToolDraging = true;
            if (null != cursor)
            {
                if (this.isToolDraging)
                    cCursor.SetCursorTemplate(cursor);
                else
                    cCursor.SetCursorDefault(Cursors.Arrow);
            }
            else
            {
                if (this.isToolDraging)
                    cCursor.SetCursorDefault(Cursors.Hand);
                else
                    cCursor.SetCursorDefault(Cursors.Arrow);
            }


            //其他元素
            if (Glo.IsDbClick)
            {// ToolboxItem双击添加新元素

                addNewElementToWorkSpace(null);
            }
            else
            {// ToolboxItem 拖拽元素

            }
        }

        private void lbTools_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            isToolDraging = false;
            cCursor.SetCursorDefault(Cursors.Arrow);
        }

        private void workSpace_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            pFrom = e.GetPosition(workSpace);
            delPoint();
            this.SetSelectedState(false);

            // 取消矩形选择
            if (selectState != StateRectangleSelected.SelectDisposed)
            {
                selectState = StateRectangleSelected.SelectDisposed;
                RemoveRect();
            }

            if (selectType.Equals(ToolBox.Line))
            {
                try
                {
                    this.SetSelectedState(false);
                    this.isDrawingLine = true;
                    this.isPointSelected = false;

                    currLine = new BPLine("Black", 2, pFrom.X, pFrom.Y, pFrom.X, pFrom.Y);
                    string name = GenerElementNameFromUI(currLine);
                    name = name.Contains(Glo.FK_MapData) ? name : Glo.FK_MapData + "_" + name;
                    currLine.Name = name;
                    this.selectedElements.Add(currLine);
                    Glo.currEle = currLine;

                    attachElementEvent(currLine);
                }
                catch (Exception ex)
                {
                    BP.SL.LoggerHelper.Write(ex);
                    currLine = null;
                    Glo.currEle = null;
                }
            }
            else if (selectType.Equals(ToolBox.Selected))
            {

                // 矩形选择
                //if (selectState != StateRectangleSelected.SelectDisposed )
                //{
                //    selectState = StateRectangleSelected.SelectDisposed;
                //    RemoveRect();
                //}
                //else
                if (e.OriginalSource is Canvas)
                {
                    selectState = StateRectangleSelected.SelectBegin;
                    RectSelected = new Rectangle();

                    RectSelected.BindDrag();
                    RectSelected.MouseLeftButtonDown += new MouseButtonEventHandler(rectSelected_MouseLeftButtonDown);
                    RectSelected.MouseMove += rectSelected_MouseMove;
                    RectSelected.MouseEnter += rectSelected_MouseEnter;
                    RectSelected.MouseLeave += rectSelected_MouseLeave;
                    RectSelected.SetValue(Canvas.LeftProperty, pFrom.X);
                    RectSelected.SetValue(Canvas.TopProperty, pFrom.Y);
                    RectSelected.Fill = new SolidColorBrush(Color.FromArgb(255, 201, 224, 252));//r.FromArgb(201,224,252)
                    RectSelected.Stroke = new SolidColorBrush(Color.FromArgb(255, 36, 93, 219));
                    RectSelected.StrokeThickness = 1;
                    RectSelected.Opacity = 0.3;
                    workSpace.Children.Add(RectSelected);

                }
            }
            else if (selectType.Equals(ToolBox.Mouse))
            {// 在工作区探测是否选中最顶层元素
                #region
                //foreach (var item in this.workSpace.Children)
                //{
                //    if (item is IElement)
                //    {
                //        FrameworkElement element = item as FrameworkElement;
                //        double zIndex = (double)element.GetValue(Canvas.ZIndexProperty),
                //            left = (double)element.GetValue(Canvas.LeftProperty),
                //            top = (double)element.GetValue(Canvas.TopProperty),
                //            width = element.ActualWidth,
                //            height = element.ActualHeight;
                //        int band = 5;
                //        if (left - band < pFrom.X && pFrom.X < left + width + band &&
                //            top - band < pFrom.Y && pFrom.Y < top + height + band)
                //        {
                //            if (null == Glo.currEle)
                //                Glo.currEle = element;
                //            else if ((double)Glo.currEle.GetValue(Canvas.ZIndexProperty) < zIndex)
                //            {
                //                Glo.currEle = element;
                //            }
                //        }
                //    }
                //}
                #endregion
            }
            else
            {

            }
        }

        private void workSpace_MouseMove(object sender, MouseEventArgs e)
        {

            #region 画线
            if (this.isDrawingLine && this.currLine != null && this.selectType == ToolBox.Line)
            {
                currLine.MyLine.X2 = e.GetPosition(this.workSpace).X;
                currLine.MyLine.Y2 = e.GetPosition(this.workSpace).Y;
                double x = currLine.MyLine.X1 - currLine.MyLine.X2;
                double y = currLine.MyLine.Y1 - currLine.MyLine.Y2;
                if (Math.Abs(x) > Math.Abs(y))
                {
                    /*是横线 */
                    currLine.MyLine.Y2 = currLine.MyLine.Y1;
                }
                else
                {
                    currLine.MyLine.X2 = currLine.MyLine.X1;
                }
                return;
            }
            #endregion 画线

            #region 改变线的长度
            if (selectType == ToolBox.Mouse && isPointSelected == true)
            {
                lineSizeChange(e.GetPosition(this.workSpace));
                return;
            }
            #endregion

            #region 矩形选择.
            if (this.selectType == ToolBox.Selected && selectState == StateRectangleSelected.SelectBegin && RectSelected != null) /*  更新rect 的大小  */
            {
                cCursor.SetCursorTemplate(Resources[this.selectType] as DataTemplate);
                Point curPoint = e.GetPosition(workSpace);
                if (curPoint.X > pFrom.X)
                {
                    RectSelected.Width = curPoint.X - pFrom.X;
                }
                if (curPoint.X < pFrom.X)
                {
                    RectSelected.SetValue(Canvas.LeftProperty, curPoint.X);
                    RectSelected.Width = pFrom.X - curPoint.X;
                }
                if (curPoint.Y > pFrom.Y)
                {
                    RectSelected.Height = curPoint.Y - pFrom.Y;
                }
                if (curPoint.Y < pFrom.Y)
                {
                    RectSelected.SetValue(Canvas.TopProperty, curPoint.Y);
                    RectSelected.Height = pFrom.Y - curPoint.Y;
                }

                return;
            }
            #endregion 矩形选择.


            if (null != Glo.currEle && Glo.currEle is BPLine)
            {
                //IElement ie = Glo.currEle as IElement;
                //if (ie.TrackingMouseMove)
                //{
                //    currLine = Glo.currEle as BPLine;

                //    Point curPoint = e.GetPosition(workSpace);

                //    double deltaV = curPoint.Y - pFrom.Y;
                //    double deltaH = curPoint.X - pFrom.X;
                //    currLine.UpdatePos(deltaH, deltaV);
                //    Glo.ViewNeedSave = true;

                //    pFrom = curPoint;

                //}
                return;
            }
        }

        private void workSpace_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CloseMenu();

            this.setUnTracking();

            this.isPointSelected = false;
            this.isDrawingLine = false;


            if (Keyboard.Modifiers == ModifierKeys.Control && MouseEventHandlers.IsCopy)
            {//复制
                this.Paste();
                return;
            }

            if (isToolDraging)
            {// 拖拽控件
                addNewElementToWorkSpace(e);
                return;
            }


            if (eCurrent != null)
            { // 当前选择的点.
                eCurrent.Fill = new SolidColorBrush(Colors.Green);
                return;
            }


            // 矩形选择
            if (selectState == StateRectangleSelected.SelectBegin)
            {
                selectState = StateRectangleSelected.SelectComplete;

                SelectUIElement();
                cCursor.SetCursorDefault(Cursors.Arrow);
            }
            //else if (!(e.OriginalSource is Rectangle))
            //    RemoveRect();

        }

        private void workSpace_MouseEnter(object sender, MouseEventArgs e)
        {
            if (null != cursor && (this.isToolDraging || this.selectType.Equals(ToolBox.Line)))
            {
                cCursor.SetCursorTemplate(cursor);
            }
        }

        private void workSpace_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!this.isToolDraging)
                cCursor.SetCursorDefault(Cursors.Arrow);
        }

        int positionOffset = 0;
        private void addNewElementToWorkSpace(MouseButtonEventArgs e)
        {
            if (string.IsNullOrEmpty(selectType))
            {
                this.SetSelectedTool(ToolBox.Mouse);
                return;
            }

            if (ToolBox.Line.Equals(selectType) || ToolBox.Selected.Equals(selectType))
                return;

            Point point = new Point(50, 50);
            if (null != e)
            {
                point = e.GetPosition(this.workSpace);
                positionOffset = 0;
            }
            else
            {
                positionOffset += 10;
                point = new Point(point.X + positionOffset, point.Y + positionOffset);
            }
            Glo.X = point.X;
            Glo.Y = point.Y;


            #region
            try
            {
                switch (selectType)
                {
                    case ToolBox.Mouse:
                        break;

                    case ToolBox.Btn:
                        BPBtn btn = new BPBtn();
                        this.winFrmBtn.HisBtn = btn;
                        Glo.currEle = btn;
                        this.winFrmBtn.Show();

                        break;
                    case ToolBox.Label: /* 标签。 */
                        BPLabel lab = new BPLabel();
                        Glo.currEle = lab;
                        lab.SetValue(Canvas.LeftProperty, point.X);
                        lab.SetValue(Canvas.TopProperty, point.Y);

                        attachElementEvent(lab);

                        break;
                    case ToolBox.Link: /* Link。 */
                        BPLink link = new BPLink();
                        Glo.currEle = link;
                        link.SetValue(Canvas.LeftProperty, point.X);
                        link.SetValue(Canvas.TopProperty, point.Y);

                        attachElementEvent(link);
                        break;

                    case ToolBox.TextBox:  // 文本框
                        this.winSelectTB.Show();
                        this.winSelectTB.RB_String.IsChecked = true;

                        break;
                    case ToolBox.DateCtl:
                        this.winSelectTB.RB_Data.IsChecked = true;
                        this.winSelectTB.Show();

                        break;
                    case ToolBox.CheckBox:
                        this.winSelectTB.RB_Boolen.IsChecked = true;
                        this.winSelectTB.CB_IsGenerLabel.IsChecked = false;
                        this.winSelectTB.Show();

                        break;
                    case ToolBox.RBS:
                        this.winSelectRB.Show();
                        this.IsRB = true;

                        break;
                    case ToolBox.DDLEnum:
                        this.winSelectRB.Show();
                        this.IsRB = false;

                        break;
                    case ToolBox.DDLTable:  // DDL。
                        this.winSelectDDL.Show();

                        break;
                    case ToolBox.Img://装饰类图片
                        BPImg bpImg = new BPImg();
                        Glo.currEle = bpImg;
                        string name = GenerElementNameFromUI(bpImg);

                        bpImg.Name = name;
                        bpImg.TB_CN_Name = name;
                        bpImg.TB_En_Name = name;
                        bpImg.SetValue(Canvas.LeftProperty, point.X);
                        bpImg.SetValue(Canvas.TopProperty, point.Y);

                        attachElementEvent(bpImg);
                        break;
                    case ToolBox.SealImg://签章
                        BPImgSeal bpImgSeal = new BPImgSeal();
                        Glo.currEle = bpImgSeal;
                        name = GenerElementNameFromUI(bpImgSeal);

                        bpImgSeal.Name = name;
                        bpImgSeal.TB_CN_Name = name;
                        bpImgSeal.TB_En_Name = name;
                        bpImgSeal.SetValue(Canvas.LeftProperty, point.X);
                        bpImgSeal.SetValue(Canvas.TopProperty, point.Y);

                        attachElementEvent(bpImgSeal);

                        break;
                    case ToolBox.FrmEle:
                        this.winFrmEle.SetBlank();
                        this.winFrmEle.Show();

                        break;
                    case ToolBox.Attachment:  // 附件
                        BPAttachment bpAth = new BPAttachment() { X = Glo.X, Y = Glo.Y };
                        Glo.currEle = bpAth;
                        this.winSelectAttachment.BindIt(bpAth);
                        this.winSelectAttachment.Show();

                        break;
                    case ToolBox.AttachmentM:  // 多附件
                        BPAttachmentM myAthM = new BPAttachmentM();
                        Glo.currEle = myAthM;
                        name = GenerElementNameFromUI(myAthM);

                        myAthM.X = Glo.X;
                        myAthM.Y = Glo.Y;
                        myAthM.Name = name;
                        myAthM.Label = "";
                        myAthM.SaveTo = @"/DataUser/UploadFile/";
                        myAthM.IsDelete = true;
                        myAthM.IsDownload = true;
                        myAthM.IsUpload = true;

                        this.winFrmAttachmentM.BindIt(myAthM);
                        this.winFrmAttachmentM.Show();

                        break;
                    case ToolBox.Dtl:

                        BPDtl newDtl = new BPDtl();
                        Glo.currEle = newDtl;
                        name = GenerElementNameFromUI(newDtl);
                        name = name.Contains(Glo.FK_MapData) ? name : Glo.FK_MapData + "_" + name;
                        newDtl.Name = name;
                        newDtl.NewDtl();

                        newDtl.SetValue(Canvas.LeftProperty, point.X);
                        newDtl.SetValue(Canvas.TopProperty, point.Y);

                        attachElementEvent(newDtl);

                        break;
                    case ToolBox.WorkCheck: //审核组件.
                        #region
                        //if (!Glo.FK_MapData.Contains("ND"))
                        //{
                        //    MessageBox.Show("只有节点表单支持审核组件", "ERROR", MessageBoxButton.OK);
                        //    break;
                        //}

                        //表单只能增加一个审核组件
                        bool find = false;
                        foreach (UIElement ctl in this.workSpace.Children)
                        {
                            if (ctl is IElement)
                                if (ctl is BPWorkCheck)
                                {
                                    if ((ctl as BPWorkCheck) == null)
                                        continue;

                                    find = true;
                                    MessageBox.Show("同一表单不允许添加两个审核组件", "ERROR", MessageBoxButton.OK);
                                    break;
                                }
                        }

                        if (!find)
                        {
                            BPWorkCheck wkCheck = new BPWorkCheck();
                            Glo.currEle = wkCheck;
                            wkCheck.SetValue(Canvas.LeftProperty, point.X);
                            wkCheck.SetValue(Canvas.TopProperty, point.Y);
                            attachElementEvent(wkCheck);

                            #region 更新他的状态.
                            /* 更新审核组件状态. */
                            string sql = "UPDATE WF_Node SET FWCSta=1 WHERE NodeID=" + Glo.NodeID;
                            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                            da.RunSQLAsync(sql, Glo.UserNo, Glo.SID);
                            da.RunSQLCompleted += (object sender, FF.RunSQLCompletedEventArgs mye) =>
                            {
                                if (mye.Error != null)
                                {
                                    MessageBox.Show("增加错误:" + mye.Error.Message);
                                    return;
                                }
                                Glo.Remove(this);
                            };
                            #endregion
                        }
                        #endregion
                        break;
                    case ToolBox.SubFlow: //子流程.

                        #region
                        //if (!Glo.FK_MapData.Contains("ND"))
                        //{
                        //    MessageBox.Show("只有节点表单支持审核组件", "ERROR", MessageBoxButton.OK);
                        //    break;
                        //}

                        //表单只能增加一个审核组件
                        bool isHave = false;
                        foreach (UIElement ctl in this.workSpace.Children)
                        {
                            if (ctl is IElement)
                                if (ctl is BPSubFlow)
                                {
                                    if ((ctl as BPSubFlow) == null)
                                        continue;

                                    isHave = true;

                                    BPSubFlow  sf= ctl as BPSubFlow;

                                    //sf.SetValue(Canvas.LeftProperty, point.X);
                                    //sf.SetValue(Canvas.TopProperty, point.Y);
                                    //sf.Width = 300;
                                    //sf.Height = 200;

                                    MessageBox.Show("同一表单不允许添加两个父子流程组件，宽度："+sf.Width+" 高度:"+sf.Height, "Error", MessageBoxButton.OK);
                                    break;
                                }
                        }

                        if (!isHave)
                        {
                            BPSubFlow wkCheck = new BPSubFlow();
                            Glo.currEle = wkCheck;
                            wkCheck.SetValue(Canvas.LeftProperty, point.X);
                            wkCheck.SetValue(Canvas.TopProperty, point.Y);
                            wkCheck.Width = 300;
                            wkCheck.Height = 200;
                            attachElementEvent(wkCheck);

                            #region 更新他的状态.
                            /* 更新状态. */
                            string sql = "UPDATE WF_Node SET SFSta=1 WHERE NodeID=" + Glo.NodeID;
                            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                            da.RunSQLAsync(sql, Glo.UserNo, Glo.SID);
                            da.RunSQLCompleted += (object sender, FF.RunSQLCompletedEventArgs mye) =>
                            {
                                if (mye.Error != null)
                                {
                                    MessageBox.Show("增加错误:" + mye.Error.Message);
                                    return;
                                }
                                Glo.Remove(this);
                            };
                            #endregion
                        }
                        #endregion
                        break;

                    case ToolBox.M2M:
                        this.winSelectM2M.IsM2M = 0;
                        this.winSelectM2M.Show();

                        break;
                    case ToolBox.M2MM:

                        this.winSelectM2M.IsM2M = 1;
                        this.winSelectM2M.X = Glo.X;
                        this.winSelectM2M.Y = Glo.Y;
                        this.winSelectM2M.Show();
                        break;
                    case ToolBox.ImgAth:

                        BPImgAth ath = new BPImgAth();
                        Glo.currEle = ath;
                        name = GenerElementNameFromUI(ath);

                        ath.Name = name;
                        ath.CtrlID = name; // 附件ID.
                        ath.SetValue(Canvas.LeftProperty, point.X);
                        ath.SetValue(Canvas.TopProperty, point.Y);
                        attachElementEvent(ath);
                        break;
                    case ToolBox.MapPin://地图定位
                        BPMapPin mapPin = new BPMapPin();
                        mapPin.MyPK = Glo.FK_MapData + "_" + mapPin.Name;
                        mapPin.SetValue(Canvas.LeftProperty, point.X);
                        mapPin.SetValue(Canvas.TopProperty, point.Y);

                        this.winFrmEleMapPin.HisEle = mapPin;
                        this.winFrmEleMapPin.InitForm();
                        this.winFrmEleMapPin.Show();

                        break;
                    case ToolBox.Microphonehot://语音控件
                        BPMicrophonehot micHot = new BPMicrophonehot();
                        micHot.MyPK = Glo.FK_MapData + "_" + micHot.Name;
                        micHot.SetValue(Canvas.LeftProperty, point.X);
                        micHot.SetValue(Canvas.TopProperty, point.Y);

                        this.winFrmEleMicHot.HisEle = micHot;
                        this.winFrmEleMicHot.InitForm();
                        this.winFrmEleMicHot.Show();
                        break;
                    default:
                        MessageBox.Show("功能未完成:" + selectType, "请期待", MessageBoxButton.OK);
                        break;
                }
            }
            catch (Exception ee) { BP.SL.LoggerHelper.Write(ee); }
            #endregion

            if (!this.selectType.Equals(ToolBox.Line))
                this.SetSelectedTool(ToolBox.Mouse);
        }
        /// <summary>
        /// 判断是否新增.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="isAdded">是否新增</param>
        /// <returns></returns>
        bool attachElementEvent(FrameworkElement element)
        {

            bool isExit = false;

            if (null == element)
                return isExit;



            Glo.currEle = element;

            try
            {
                isExit = !this.IsExist(element.Name);
                if (this.IsExist(element.Name))
                {
                    MessageBox.Show("已存在ID为" + element.Name + "的元素，不允许添加同名元素！", "CCForm提示:", MessageBoxButton.OK);
                    return true;
                }

                if (isExit)
                {
                    if (!this.workSpace.Children.Contains(element))
                        this.workSpace.Children.Add(element);

                    if (element is IRouteEvent)
                    {
                        IRouteEvent route = element as IRouteEvent;
                        if (null != route)
                        {
                            route.LeftDown += UIElement_LeftButtonDown;
                            route.LeftUp += UIElement_MouseLeftButtonUp;
                        }
                    }
                    else
                    {
                        element.MouseLeftButtonDown += UIElement_LeftButtonDown;
                        element.MouseLeftButtonUp += UIElement_MouseLeftButtonUp;
                    }
                    element.MouseRightButtonDown += UIElement_MouseRightButtonDown;
                    element.MouseEnter += UIElement_MouseEnter;
                    element.MouseLeave += UIElement_MouseLeave;

                    if (element is BPLine)
                        (element as BPLine).Moved += new LineMoved(BPLine_MouseMove);
                    element.Cursor = Cursors.Hand;

                    Glo.ViewNeedSave = true;
                }


            }
            catch (Exception ex)
            {
                isExit = false;
                BP.SL.LoggerHelper.Write(ex);
                MessageBox.Show("控件ID:" + element.Name + "添加错误. Error Info:" + ex.Message);
            }
            return isExit;
        }

        public bool IsExist(string name)
        {
            bool flag = false;
            foreach (FrameworkElement ele in this.workSpace.Children)
            {
                if (ele.Name == name)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        private void HidCurrSelectUI()
        {
            if (MessageBox.Show("您确定要隐藏选择的元素吗？", "执行确认", MessageBoxButton.OKCancel)
                == MessageBoxResult.No)
                return;

            BPRadioBtn rb = Glo.currEle as BPRadioBtn;
            if (rb != null)
            {
                rb.DeleteIt();
                return;
            }

            if (this.workSpace.Children.Contains(Glo.currEle))
            {
                BPTextBox tb = Glo.currEle as BPTextBox;
                if (tb != null)
                {
                    tb.HidIt();
                    return;
                }

                BPDDL ddl = Glo.currEle as BPDDL;
                if (ddl != null)
                {
                    ddl.HidIt();
                    return;
                }

                BPDtl dtl = Glo.currEle as BPDtl;
                if (dtl != null)
                {
                    dtl.DeleteIt();
                    return;
                }

                BPM2M m2m = Glo.currEle as BPM2M;
                if (m2m != null)
                {
                    m2m.DeleteIt();
                    return;
                }

                MessageBox.Show("您选择的元素不支持隐藏", "执行错误", MessageBoxButton.OK);
                return;
            }
            Glo.currEle = null;
        }
        private void DeleteCurrSelectUI()
        {
            int number = this.selectedElements.Count;
            if (number == 0)
            {
                MessageBox.Show("您没有选择删除的对象，提示:按下ctrl可实现多选。", "批量删除提示", MessageBoxButton.OK);
                return;
            }
            else if (number > 2)
            {
                string alter = "共有 (" + number + ") 个对象被选择，您确定要删除它们吗？\n\n提示:按下ctrl可实现多选。";
                if (MessageBox.Show(alter, "批量删除提示", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return;
            }

            for (int i = 0; i < number; i++)
            {
                FrameworkElement item = this.selectedElements[i];
                if (item is IElement && this.workSpace.Children.Contains(item))
                {
                    if (item is IDelete)
                    {
                        (item as IDelete).DeleteIt();
                        continue;
                    }
                    else
                    {
                        this.workSpace.Children.Remove(item);
                        //(item as IElement).ViewDeleted = true;
                        //item.Visibility =  System.Windows.Visibility.Collapsed;
                    }
                }
            }

            this.selectedElements.Clear();

            if (Glo.currEle != null)
            {
                Glo.currEle = null;
                this.delPoint();
            }

            this.SetGridLines(this.workSpace, true);
        }

        private void Paste()
        {
            if (!MouseEventHandlers.IsCopy)
                return;

            List<FrameworkElement> copyEles = new List<FrameworkElement>();
            string name = "";
            foreach (FrameworkElement item in this.selectedElements)
            {
                if (item is BPLine)
                {
                    BPLine line = item as BPLine;
                    if (line != null)
                    {
                        BPLine lineN = new BPLine(line.Color, line.MyLine.StrokeThickness,
                            line.MyLine.X1 + 10, line.MyLine.Y1 + 10, line.MyLine.X2 + 10, line.MyLine.Y2 + 10);
                        lineN.Name = GenerElementNameFromUI(lineN);
                        copyEles.Add(lineN);
                        attachElementEvent(lineN);
                    }
                }
                else if (item is BPLabel)
                {
                    BPLabel lab = item as BPLabel;
                    if (lab != null)
                    {
                        name = GenerElementNameFromUI(lab);
                        BPLabel labN = new BPLabel()
                        {
                            Name = name,
                            Content = lab.Content,
                            Foreground = lab.Foreground,
                            FontSize = lab.FontSize,
                            FontWeight = lab.FontWeight
                        };

                        labN.SetValue(Canvas.LeftProperty, (double)lab.GetValue(Canvas.LeftProperty) + 16);
                        labN.SetValue(Canvas.TopProperty, (double)lab.GetValue(Canvas.TopProperty) + 16);

                        attachElementEvent(labN);
                        copyEles.Add(labN);
                    }
                }
                else if (item is BPLink)
                {
                    BPLink link = item as BPLink;
                    if (link != null)
                    {
                        name = GenerElementNameFromUI(link);
                        BPLink labN = new BPLink()
                        {
                            Name = name,
                            Content = link.Content,
                            Foreground = link.Foreground,
                            FontSize = link.FontSize,
                            FontWeight = link.FontWeight
                        };
                        labN.SetValue(Canvas.LeftProperty, (double)link.GetValue(Canvas.LeftProperty) + 16);
                        labN.SetValue(Canvas.TopProperty, (double)link.GetValue(Canvas.TopProperty) + 16);

                        attachElementEvent(labN);
                        copyEles.Add(labN);
                    }
                }
                else if (item is BPTextBox)
                {
                    BPTextBox tb = item as BPTextBox;
                    if (tb != null)//是条件永不成立，textbox复制暂时取消。
                    {
                        name = GenerElementNameFromUI(tb);
                        BPTextBox tbN = new BPTextBox()
                        {
                            TextAlignment = tb.TextAlignment,
                            Text = tb.Text,
                            Name = name,
                            Width = tb.Width,
                            Height = tb.Height,
                            HisTBType = tb.HisTBType,
                            IsReadOnly = tb.IsReadOnly,

                            Background = new SolidColorBrush(Colors.Orange)
                        };
                        tbN.SetValue(Canvas.LeftProperty, (double)tb.GetValue(Canvas.LeftProperty) + 16);
                        tbN.SetValue(Canvas.TopProperty, (double)tb.GetValue(Canvas.TopProperty) + 16);

                        attachElementEvent(tbN);
                        copyEles.Add(tbN);
                    }
                }
                else if (item is BPImg)
                {
                    BPImg img = item as BPImg;
                    if (img != null)
                    {
                        MessageBox.Show("装饰图片复制功能未完成.");
                        return;

                        //  name = "Img" + timeKey + "_" + idx.ToString();
                        //  BPImg labN = new BPImg();
                        //  labN.Name = name;
                        ////  labN.Content = lab.Content;
                        //  labN.SetValue(Canvas.LeftProperty, (double)lab.GetValue(Canvas.LeftProperty) + 16);
                        //  labN.SetValue(Canvas.TopProperty, (double)lab.GetValue(Canvas.TopProperty) + 16);
                        //  labN.Foreground = lab.Foreground;
                        //  this.workSpace.Children.Add(labN);

                        //  labN.MouseLeftButtonDown += new MouseButtonEventHandler(UIElement_Click);
                        //  labN.MouseRightButtonDown += new MouseButtonEventHandler(UIElement_MouseRightButtonDown);

                        //  copyEles.Add(labN);
                    }
                }
            }
            this.SetSelectedState(false);
            this.selectedElements = copyEles;
            this.SetSelectedState(true);

        }

        private void ToolBar_Click(object sender, RoutedEventArgs e)
        {
            string id = "";
            FrameworkElement ele = sender as FrameworkElement;
            if (ele != null)
                id = ele.Tag.ToString();
            else return;

            switch (id)
            {
                case EleFunc.SelectAll:
                    selectAll();
                    break;
                case EleFunc.CopyEle:
                    MouseEventHandlers.IsCopy = true;
                    break;
                case EleFunc.Paste:
                    this.Paste();
                    break;
                case EleFunc.FontSizeAdd:

                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        BPLabel lab = item as BPLabel;
                        if (lab != null)
                        {
                            lab.FontSize = lab.FontSize + 1;
                        }

                        BPLink link = item as BPLink;
                        if (link != null)
                        {
                            link.FontSize = link.FontSize + 1;
                        }

                        BPLine line = item as BPLine;
                        if (line != null)
                        {
                            line.MyLine.StrokeThickness = line.MyLine.StrokeThickness + 2;//re,1
                        }
                    }
                    break;
                case EleFunc.FontSizeCut:
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        BPLabel lab = item as BPLabel;
                        if (lab != null)
                        {
                            if (lab.FontSize < 8)
                                continue;

                            lab.FontSize = Math.Abs(lab.FontSize - 1);
                        }

                        BPLink link = item as BPLink;
                        if (link != null)
                        {
                            if (link.FontSize < 8)
                                continue;

                            link.FontSize = Math.Abs(link.FontSize - 1);
                        }

                        BPLine line = item as BPLine;
                        if (line != null)
                        {
                            if (line.MyLine.StrokeThickness < 3)//re,0.5
                                continue;
                            line.MyLine.StrokeThickness = Math.Abs(line.MyLine.StrokeThickness - 2);//re,1
                        }
                    }
                    break;
                case EleFunc.Colorpicker:

                    ColorPickerWin cw = new ColorPickerWin();
                    cw._ColorChanged += new ColorPickerWin.ColorChanged(ColorChanged);
                    cw.Show();
                    break;
                case EleFunc.Bold:
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        BPLabel lab = item as BPLabel;
                        if (lab != null)
                        {
                            if (lab.FontWeight == FontWeights.Bold)
                                lab.FontWeight = FontWeights.Normal;
                            else
                                lab.FontWeight = FontWeights.Bold;
                        }

                        BPLink link = item as BPLink;
                        if (link != null)
                        {
                            if (link.FontWeight == FontWeights.Bold)
                                link.FontWeight = FontWeights.Normal;
                            else
                                link.FontWeight = FontWeights.Bold;
                        }
                    }
                    break;
                case Func.Property://add by qin 添加对平台的判断
                    string url ="";
                    if (Glo.Platform == Platform.JFlow)
                        url = Glo.BPMHost + "/WF/Comm/RefFunc/UIEn.jsp?EnsName=BP.WF.Template.MapDataExts&PK=" + Glo.FK_MapData;
                    else
                        url = "/WF/Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.MapDataExts&PK=" + Glo.FK_MapData;

                    Glo.WinOpenDialog(url, 680, 900);
                    return;
                case Func.PSize:
                    this.winFrmOp.Show();
                    break;
                case Func.Alignment_Down://底端对齐
                    if (this.selectedElements.Count == 0)
                    {
                        MessageBox.Show("必须选择两个或者两个以上的控件才能执行对齐。");
                        return;
                    }

                    double maxY = 0;
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        double dY = Canvas.GetTop(item) + item.ActualHeight;
                        if (maxY < dY)
                        {
                            maxY = dY;
                        }
                    }

                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        item.SetValue(Canvas.TopProperty, maxY - item.ActualHeight);
                    }
                    break;
                case Func.Alignment_Top:
                    if (this.selectedElements.Count == 0)
                    {
                        MessageBox.Show("必须选择两个或者两个以上的控件才能执行对齐。");
                        return;
                    }
                    double minY = 1000;
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        //MatrixTransform transform = item.TransformToVisual(this.workSpace) as MatrixTransform;
                        //double y = transform.Matrix.OffsetY;
                        double dY = Canvas.GetTop(item);
                        if (minY > dY)
                            minY = dY;
                    }
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        item.SetValue(Canvas.TopProperty, minY);
                    }
                    break;
                case Func.Alignment_Left:
                    if (this.selectedElements.Count == 0)
                    {
                        MessageBox.Show("必须选择两个或者两个以上的控件才能执行对齐。");
                        return;
                    }

                    double minX = 1000;
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        MatrixTransform transform = item.TransformToVisual(this.workSpace) as MatrixTransform;
                        double x = transform.Matrix.OffsetX;
                        if (x <= 0)
                            continue;

                        if (minX > x)
                            minX = x;
                    }

                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        item.SetValue(Canvas.LeftProperty, minX);
                    }
                    break;
                case Func.Alignment_Right:
                    if (this.selectedElements.Count == 0)
                    {
                        MessageBox.Show("必须选择两个或者两个以上的控件才能执行对齐。");
                        return;
                    }
                    double maxX = 0;
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        MatrixTransform transform = item.TransformToVisual(this.workSpace) as MatrixTransform;
                        double x = transform.Matrix.OffsetX + item.ActualWidth;
                        if (maxX < x)
                        {
                            maxX = x;
                        }
                    }
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        MatrixTransform transform = item.TransformToVisual(this.workSpace) as MatrixTransform;
                        item.SetValue(Canvas.LeftProperty, maxX - item.ActualWidth);
                    }
                    break;
                case Func.Alignment_Center:
                    if (this.selectedElements.Count == 0)
                    {
                        MessageBox.Show("必须选择两个或者两个以上的控件才能执行对齐。");
                        return;
                    }
                    double miX = 1000, maX = 0; /* 求最大， 最小*/
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        MatrixTransform transform = item.TransformToVisual(this.workSpace) as MatrixTransform;
                        double x = transform.Matrix.OffsetX + item.ActualWidth;
                        if (maX < x)
                            maX = x;
                        if (miX > transform.Matrix.OffsetX)
                            miX = x;
                    }
                    double miudeLine = (maX - miX) / 2 + miX;
                    foreach (FrameworkElement item in this.selectedElements)
                    {
                        item.SetValue(Canvas.LeftProperty, miudeLine - item.ActualWidth / 2);
                    }
                    break;

                case Func.View:

                    try
                    {
                        string url1 = null;
                        if (Glo.IsDtlFrm == false)
                            url1 = Glo.BPMHost + "/WF/CCForm/Frm.aspx?FK_MapData=" + Glo.FK_MapData + "&FrmType=FreeFrm&IsTest=1&WorkID=0&FK_Node=" + Glo.FK_Node + "&s=2" + Glo.TimeKey;
                        else
                            url1 = Glo.BPMHost + "/WF/CCForm/FrmCard.aspx?EnsName=" + Glo.FK_MapData + "&FrmType=FreeFrm&RefPKVal=0&OID=0" + Glo.TimeKey;

                        Glo.WinOpen(url1, (int)Glo.HisMapData.FrmH, (int)Glo.HisMapData.FrmW);

                    }
                    catch (Exception ee)
                    {
                        BP.SL.LoggerHelper.Write(ee);
                    }

                    break;
                case Func.Exp:
                    string urlExt = Glo.BPMHost + "/WF/Admin/XAP/DoPort.aspx?DoType=DownFormTemplete&FK_MapData=" + Glo.FK_MapData + Glo.TimeKey;
                    Glo.WinOpen(urlExt, 100, 100);

                    if (MessageBox.Show("已经开始执行导出了，如果您的浏览器不能正常被下载，请点确定按钮直接下载模版。", "您看到导出的文件了吗？", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        Glo.WinOpen(Glo.BPMHost + "/DataUser/Temp/" + Glo.FK_MapData + ".xml", 100, 200);
                    return;

                case Func.MapExt: // 扩展设置。
                    Glo.WinOpen(Glo.BPMHost + "/WF/Admin/FoolFormDesigner/MapExt.aspx?FK_MapData=" + Glo.FK_MapData + Glo.TimeKey);
                    return;
                case Func.Imp:
                    winFrmImp.Show();
                    break;
                case Func.Delete:
                    this.DeleteCurrSelectUI();
                    break;
                case Func.Save:
                    this.Save();
                    break;

                case Func.Copy:
                    this.winFrmImp.Show();
                    break;
                case Func.Event:
                    FrmEvent fa = new FrmEvent();
                    fa.Show();
                    break;
                case Func.HiddenField:
                    new FrmHiddenField().Show();
                    break;

                case "Btn_Glo":
                    MessageBox.Show(Glo.currEle.ToString());
                    break;
                case "Btn_Impdddd": // del.
                    this.menuItem_MouseLeftButtonDown(null, null);
                    OpenFileDialog myOpenFileDialog = new OpenFileDialog();
                    myOpenFileDialog.Filter = "驰骋工作独立表单模板(*.xml)|*.xml|All Files (*.*)|*.*";  //SL目前只支持jpg和png格式图像的显示
                    myOpenFileDialog.Multiselect = false;//只允许选择一个图片
                    if (myOpenFileDialog.ShowDialog() == false)
                        return;

                    string mapImageName = myOpenFileDialog.File.Name.ToString();
                    //获得图片的流信息，并与image控件绑定
                    FileInfo aFileInfo = myOpenFileDialog.File;
                    FileInfo fileInfoOfMapImage = myOpenFileDialog.File;

                    if (aFileInfo == null)
                        return;
                    Stream mapImageStream = aFileInfo.OpenRead();
                    //上传图片
                    mapImageStream.Position = 0;
                    byte[] buffer = new byte[mapImageStream.Length + 1];
                    mapImageStream.Read(buffer, 0, buffer.Length);
                    String fileName = fileInfoOfMapImage.Name;
                    FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
                    da.UploadFileAsync(buffer, "\\Temp\\s.xml");
                    da.UploadFileCompleted += (object senders, FF.UploadFileCompletedEventArgs ee) =>
                    {
                        if (ee.Error == null)
                        {
                            //this.OpenFormXml(ee.Result);
                            this.OpenFormJson(ee.Result);
                        }
                    };
                    break;
                default:
                    MessageBox.Show(sender.ToString() + " ID=" + id + " 功能未实现.");
                    break;
            }
        }

        bool isDesignerSizeChanged;
        void changeFormSize(double width, double height)
        {
            this.workSpace.Width = width;
            this.workSpace.Height = height;
            Glo.HisMapData.FrmH = height;
            Glo.HisMapData.FrmW = width;
            this.isDesignerSizeChanged = true;
        }

        #region BPLine 点
        Ellipse e1, e2;//选中线后出现的绿点
        Ellipse eCurrent;//选中的绿点
        bool isPointSelected;//在绿点上判断当前鼠标的状态是否按下.

        void initPoint()
        {
            e1 = new Ellipse();
            e1.Tag = "e1";
            e1.Cursor = Cursors.Hand;
            e1.MouseLeftButtonDown += e_MouseLeftButtonDown;
            e1.Width = 9;
            e1.Height = 9;
            e1.Fill = new SolidColorBrush(Colors.Green);

            e2 = new Ellipse();
            e2.Tag = "e2";
            e2.Cursor = Cursors.Hand;
            e2.MouseLeftButtonDown += e_MouseLeftButtonDown;
            e2.Width = 9;
            e2.Height = 9;
            e2.Fill = new SolidColorBrush(Colors.Green);

        }


        void BPLine_MouseMove(BPLine line)
        {

            e1.SetValue(Canvas.LeftProperty, line.MyLine.X1 - 4);
            e1.SetValue(Canvas.TopProperty, line.MyLine.Y1 - 4);

            e2.SetValue(Canvas.LeftProperty, line.MyLine.X2 - 4);
            e2.SetValue(Canvas.TopProperty, line.MyLine.Y2 - 4);
        }

        void e_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            isPointSelected = true;
            eCurrent = sender as Ellipse;
            eCurrent.Fill = new SolidColorBrush(Colors.Red);
        }

        void lineSizeChange(Point p)
        {
            #region 改变线的长度

            if (eCurrent.Tag.ToString() == "e1")
            {
                double x = p.X - currLine.MyLine.X2;
                double y = p.Y - currLine.MyLine.Y2;
                if (Math.Abs(x) > Math.Abs(y))
                {
                    currLine.MyLine.X1 = p.X;
                    currLine.MyLine.Y1 = currLine.MyLine.Y2;
                    eCurrent.SetValue(Canvas.LeftProperty, p.X - 4);
                    eCurrent.SetValue(Canvas.TopProperty, currLine.MyLine.Y2 - 4);
                }
                else
                {
                    currLine.MyLine.X1 = currLine.MyLine.X2;
                    currLine.MyLine.Y1 = p.Y;
                    eCurrent.SetValue(Canvas.LeftProperty, currLine.MyLine.X2 - 4);
                    eCurrent.SetValue(Canvas.TopProperty, p.Y - 4);
                }
            }
            else //if (eCurrent.Tag.ToString() == "e2")
            {
                double x = p.X - currLine.MyLine.X1;
                double y = p.Y - currLine.MyLine.Y1;
                if (Math.Abs(x) > Math.Abs(y))
                {
                    currLine.MyLine.X2 = p.X;
                    currLine.MyLine.Y2 = currLine.MyLine.Y1;
                    eCurrent.SetValue(Canvas.LeftProperty, p.X - 4);
                    eCurrent.SetValue(Canvas.TopProperty, currLine.MyLine.Y1 - 4);
                }
                else
                {
                    currLine.MyLine.X2 = currLine.MyLine.X1;
                    currLine.MyLine.Y2 = p.Y;
                    eCurrent.SetValue(Canvas.LeftProperty, currLine.MyLine.X1 - 4);
                    eCurrent.SetValue(Canvas.TopProperty, p.Y - 4);
                }
            }

            #endregion
        }
        void lineEdit(BPLine line)
        {
            if (selectType == ToolBox.Mouse)
            {
                if (!workSpace.Children.Contains(e1))
                    this.workSpace.Children.Add(e1);
                if (!workSpace.Children.Contains(e2))
                    this.workSpace.Children.Add(e2);


                e1.SetValue(Canvas.LeftProperty, line.MyLine.X1 - 4);
                e1.SetValue(Canvas.TopProperty, line.MyLine.Y1 - 4);

                e2.SetValue(Canvas.LeftProperty, line.MyLine.X2 - 4);
                e2.SetValue(Canvas.TopProperty, line.MyLine.Y2 - 4);
            }
        }

        //删除主面板上线上的点
        void delPoint()
        {
            if (workSpace.Children.Contains(e1))
                this.workSpace.Children.Remove(e1);

            if (workSpace.Children.Contains(e2))
                this.workSpace.Children.Remove(e2);
        }
        #endregion

        #region 快捷菜单
        private void UIElement_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (sender == this.workSpace)
            {
                ShowMenu(this.muFrm, e);
            }
            else if (sender is IElement)
            {
                UIElement ele = sender as UIElement;
                if (Glo.currEle != ele)
                {
                    if (Glo.currEle != null)
                    {
                        (Glo.currEle as IElement).IsSelected = false;
                    }
                    (ele as IElement).IsSelected = true;
                    (ele as IElement).TrackingMouseMove = false;
                    Glo.currEle = ele;
                }
                ShowMenu(this.muElePanel, e);
            }
        }
        static List<Menu> menus = new List<Menu>();
        void CloseMenu()
        {
            foreach (var item in menus)
            {
                item.Hide();
            }
        }
        void ShowMenu(Menu menu, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(this.workSpace);
            if (this.workSpace.Children.Contains(menu) == false)
                this.workSpace.Children.Add(menu);

            if (menus.Contains(menu) == false)
                menus.Add(menu);

            CloseMenu();
            // 调整x,y 值 ，以防止菜单被遮盖住.
            var x = p.X;
            var y = p.Y;

            double menuHeight = 200, menuWidth = 200;
            if (menu == this.muElePanel || menu == this.muFrm)
            {// 表单快捷方式
                if (menu == this.muElePanel)
                {
                    menuHeight = 180;
                }
                else if (menu == this.muFrm)
                {
                    menuHeight = 270;
                }

                var hostWidth = this.svWorkSpace.ActualWidth;// Application.Current.Host.Content.ActualWidth - 180;
                var hostHeight = this.svWorkSpace.ActualHeight;// Application.Current.Host.Content.ActualHeight - 35;

                if (x + menuWidth > hostWidth)
                {
                    x = x - (x + menuWidth - hostWidth);

                    if (double.IsNaN(this.svWorkSpace.HorizontalOffset) == false)
                        x += this.svWorkSpace.HorizontalOffset;
                }

                double menuTop = y + menuHeight;

                if (double.IsNaN(this.svWorkSpace.VerticalOffset) || this.svWorkSpace.VerticalOffset == 0)
                {
                    if (menuTop > hostHeight)
                    {
                        y = y - (menuTop - hostHeight);
                    }
                }
                else
                {
                    if (menuTop - this.svWorkSpace.VerticalOffset > hostHeight)
                    {
                        y = y - (menuTop - hostHeight) + this.svWorkSpace.VerticalOffset;
                    }
                }
            }
            else
            {

            }

            menu.SetValue(Canvas.LeftProperty, x);
            menu.SetValue(Canvas.TopProperty, y);

            // 重设快捷菜单的ZIndex防止被遮盖
            if (null != Glo.currEle)
            {
                int d = Canvas.GetZIndex(Glo.currEle);
                menu.SetValue(Canvas.ZIndexProperty, ++d);
            }
            menu.Show();
        }

        // 关闭快捷菜单主菜单
        private void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            Menu menu = sender as Menu;
            if (null != menu)
                menu.Hide();
        }
        private void menuItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            MenuItem tb = sender as MenuItem;

            Menu menu = tb.ParentMenu;
            if (null != menu)
                menu.Hide();

            switch (tb.Name)
            {
                case "FrmTempleteShareIt": //共享模板.
                    FrmShare fss = new FrmShare();
                    fss.Show();
                    break;
                case "AdvAction": //事件.
                case "AdvActionExt":
                    FrmEvent fa = new FrmEvent();
                    fa.Show();
                    break;
                case "AdvUAC": //访问控制.
                    FrmUAC fuac = new FrmUAC();
                    fuac.Show();
                    break;
                case "GradeLine":
                case "GradeLine_Ext":
                    this.GradeLine.IsChecked = !this.GradeLine.IsChecked;
                    this.SetGridLines(this.workSpace, this.GradeLine.IsChecked); //重新画线
                    break;
                case "FullScreen": //全屏
                case "FullScreen_Ext": //全屏
                    Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
                    break;
                case "FrmTempleteShare":
                    FrmImpFromInternet impFrmI = new FrmImpFromInternet();
                    impFrmI.HisMainPage = this;
                    impFrmI.Show();
                    break;
                case "FrmTempleteExp": //导出表单模版.
                case "FrmTempleteExp_Ext": //导出表单模版.
                    Glo.WinOpen(Glo.BPMHost + "/WF/Admin/XAP/DoPort.aspx?DoType=DownFormTemplete&FK_MapData=" + Glo.FK_MapData + Glo.TimeKey,
                        100, 100);
                    return;
                case "FrmTempleteImp": //导入表单模版
                case "FrmTempleteImp_Ext": //导入表单模版
                    winFrmImp.Show();
                    break;
                case "eleDel":
                    this.DeleteCurrSelectUI();
                    break;
                case "eleCopyTo": //复制到其它表单
                    FrmCopyEleTo copyIt = new FrmCopyEleTo();
                    copyIt.Tag = sender;
                    copyIt.Show();
                    break;
                case "eleHid":
                    this.HidCurrSelectUI();
                    break;
                case "eleCancel":
                    break;
                case "eleDtlFrm":
                    BPDtl dtlFrm = Glo.currEle as BPDtl;
                    if (dtlFrm != null)
                    {
                        string url = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/CCForm/Frm.aspx?FK_MapData=" + dtlFrm.Name
                            + "&FK_Node=" + Glo.FK_Node + "&UserNo=" + Glo.UserNo + "&SID=" + Glo.SID + "&S=2" + Glo.TimeKey;

                        if (Glo.Platform == Platform.JFlow)
                        {
                            url = url.Replace(".aspx", ".jsp");
                            if (url.Contains("/XAP"))
                                url = url.Replace("/XAP", "/xap");
                        }

                        HtmlPage.Window.Eval("window.open('" + url + "','_blank')");
                        return;
                    }
                    else
                    {
                        MessageBox.Show("当前选中的元素不是明细表", "提示", MessageBoxButton.OK);
                    }
                    break;
                case "eleTabIdx":
                case "eleTabIdx_Ext":
                    string url1 = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/TabIdx.aspx?FK_MapData=" + Glo.FK_MapData + Glo.TimeKey;
                    if (Glo.Platform == Platform.JFlow)
                    {
                        url1 = url1.Replace(".aspx", ".jsp");
                        if (url1.Contains("/XAP"))
                            url1 = url1.Replace("/XAP", "/xap");
                    }
                    HtmlPage.Window.Eval("window.showModalDialog('" + url1 + "',window,'dialogHeight:500px;dialogWidth:700px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
                    return;
                case "eleEdit":

                    UIElementDbClickEdit(Glo.currEle);

                    break;
                case "sysErrorLog":
                    BP.SL.OutputChildWindow.ShowException();
                    break;
                case "refresh":
                    this.BindFrm();
                    break;
                default:
                    MessageBox.Show(tb.Text + " 功能未完成.", "敬请期待", MessageBoxButton.OK);
                    break;
            }
        }

        #endregion

        #region 选中状态
        void SetSelectedTool(string id)
        {  // 设置选择的ToolBox.并配置鼠标样式
            this.selectType = id;

            switch (this.selectType)
            {
                case ToolBox.Mouse:

                    MouseEventHandlers.IsCopy = false;
                    this.isDrawingLine = false;
                    this.isToolDraging = false;
                    this.lbTools.SelectedIndex = 0;
                    this.isPointSelected = false;
                    cursor = null;
                    cCursor.SetCursorDefault(Cursors.Arrow);
                    selectState = StateRectangleSelected.SelectDisposed;
                    this.RemoveRect();
                    this.setUnTracking();
                    this.SetSelectedState(false);
                    break;
                default:
                    cursor = null;
                    cursor = Resources[id] as DataTemplate;
                    break;
            }
        }
        void ColorChanged(Color strColor)
        {
            foreach (FrameworkElement item in this.selectedElements)
            {
                BPLabel lab = item as BPLabel;
                if (lab != null)
                    lab.Foreground = new SolidColorBrush(strColor);

                BPLink link = item as BPLink;
                if (link != null)
                    link.Foreground = new SolidColorBrush(strColor);

                BPLine line = item as BPLine;
                if (line != null)
                {
                    line.Color = Glo.PreaseColorToName(strColor.ToString());
                }
            }
        }
        void setUnTracking()
        {
            Glo.SetTracking(Glo.currEle as IElement, false);
        }

        #endregion

        #region  矩形选择框处理
        double left = 0.0, top = 0.0;
        void rectSelected_MouseMove(object sender, MouseEventArgs e)
        {

            if (selectState == StateRectangleSelected.SelectComplete)
            {
                left = Convert.ToDouble(RectSelected.GetValue(Canvas.LeftProperty));
                top = Convert.ToDouble(RectSelected.GetValue(Canvas.TopProperty));
                selectState = StateRectangleSelected.SelectMoved;
            }
            foreach (var item in this.selectedElements)
            {
                BPLine line = item as BPLine;
                if (line != null)
                {
                    line.UpdatePos(Convert.ToDouble(RectSelected.GetValue(Canvas.LeftProperty)) - left, Convert.ToDouble(RectSelected.GetValue(Canvas.TopProperty)) - top);
                }
                else
                {
                    item.SetValue(Canvas.LeftProperty, Convert.ToDouble(item.GetValue(Canvas.LeftProperty)) + Convert.ToDouble(RectSelected.GetValue(Canvas.LeftProperty)) - left);
                    item.SetValue(Canvas.TopProperty, Convert.ToDouble(item.GetValue(Canvas.TopProperty)) + Convert.ToDouble(RectSelected.GetValue(Canvas.TopProperty)) - top);

                }
            }
            left = Convert.ToDouble(RectSelected.GetValue(Canvas.LeftProperty));
            top = Convert.ToDouble(RectSelected.GetValue(Canvas.TopProperty));

        }
        void rectSelected_MouseLeave(object sender, MouseEventArgs e)
        {
            cCursor.SetCursorDefault(Cursors.Arrow);
        }

        void rectSelected_MouseEnter(object sender, MouseEventArgs e)
        {
            DataTemplate cursor = Resources["Move"] as DataTemplate;
            if (null != cursor)
            {
                cCursor.SetCursorTemplate(cursor);
            }
            else
                cCursor.SetCursorDefault(Cursors.Hand);
        }

        void rectSelected_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void SelectUIElement()
        {
            if (RectSelected == null) return;

            this.selectedElements.Clear();
            //调整框选区自动变量 
            bool isHaveLine = false;//判断有没有线
            double finalLeft = double.MaxValue;
            double finalRight = double.MinValue;
            double finalTop = double.MaxValue;
            double finalBottom = double.MinValue;


            double Left = Convert.ToDouble(RectSelected.GetValue(Canvas.LeftProperty));
            double Top = Convert.ToDouble(RectSelected.GetValue(Canvas.TopProperty));
            double Right = Left + RectSelected.ActualWidth;
            double Bottom = Top + RectSelected.ActualHeight;

            double left = 0.0;
            double top = 0.0;
            double right = 0.0;
            double bottom = 0.0;

            foreach (UIElement ue in workSpace.Children)
            {
                if (ue is IElement && !object.ReferenceEquals(ue, RectSelected))
                {
                    FrameworkElement c = ue as FrameworkElement;

                    if (c is BPLine)
                    {// 判断有没有线
                        BPLine line = c as BPLine;
                        isHaveLine = true;
                        left = line.MyLine.X1;
                        top = line.MyLine.Y1;
                        right = line.MyLine.X2;
                        bottom = line.MyLine.Y2;
                    }
                    else
                    {
                        left = Convert.ToDouble(c.GetValue(Canvas.LeftProperty));
                        top = Convert.ToDouble(c.GetValue(Canvas.TopProperty));
                        right = left + c.ActualWidth;
                        bottom = top + c.ActualHeight;
                    }

                    if (Left < right && left < Right && Top < bottom && top < Bottom)
                    //if (!(right < Left || bottom < Top || Right < left  ||  Bottom < top ))
                    {
                        this.selectedElements.Add(c);

                        if (finalLeft > left) finalLeft = left;
                        if (finalTop > top) finalTop = top;

                        if (finalRight < right) finalRight = right;
                        if (finalBottom < bottom) finalBottom = bottom;
                    }

                }
            }

            //如果没有线进行自动调整框选区
            if (!isHaveLine & this.selectedElements.Count > 0)
            {
                RectSelected.SetValue(Canvas.LeftProperty, finalLeft);
                RectSelected.SetValue(Canvas.TopProperty, finalTop);
                RectSelected.Width = Math.Abs(finalRight - finalLeft);
                RectSelected.Height = Math.Abs(finalBottom - finalTop);
            }
            else if (this.selectedElements.Count <= 0)
            {  //框选空白区域
                RemoveRect();
            }
            else
                this.SetSelectedState(true);
        }
        void RemoveRect()
        {
            this.SetSelectedState(false);
            if (RectSelected != null)
            {
                RectSelected.MouseLeftButtonDown -= rectSelected_MouseLeftButtonDown;
                RectSelected.MouseMove -= rectSelected_MouseMove;
                RectSelected.MouseEnter -= rectSelected_MouseEnter;
                RectSelected.MouseLeave -= rectSelected_MouseLeave;
                workSpace.Children.Remove(RectSelected);
                selectState = StateRectangleSelected.SelectDisposed;
                RectSelected = null;
            }
        }
        void selectAll()
        {
            this.selectedElements.Clear();
            foreach (FrameworkElement item in this.workSpace.Children)
            {
                if (item is IElement)
                    this.selectedElements.Add(item);
            }
            this.SetSelectedState(true);
        }

        void SetSelectedState(bool isSelected)
        {
            IElement e = null;
            if (isSelected)
            {
                foreach (UIElement en in this.selectedElements)
                {
                    if (en is IElement && (e = en as IElement) != null)
                    {
                        e.IsSelected = isSelected;
                    }
                }
            }
            else
            {
                foreach (UIElement en in this.workSpace.Children)
                {
                    if (en is IElement && (e = en as IElement) != null)
                    {
                        e.IsSelected = isSelected;
                    }
                }

                this.selectedElements.Clear();
            }
        }

        #endregion

        #region 获取元素默认名称
        /// <summary>
        /// 产生一个控件的名字
        /// </summary>
        /// <param name="obj">控件类型</param>
        /// <returns>返回控件名字</returns>
        public string GenerElementNameFromUI(object obj)
        {
            string prefix = string.Empty;
            if (obj is BPAttachment) prefix = "Attach";
            else if (obj is BPAttachmentM) prefix = "AttachM";
            else if (obj is BPBtn) prefix = "Btn";
            else if (obj is BPCheckBox) { prefix = "Ckb"; }
            else if (obj is BPDatePicker) { prefix = "Date"; }
            else if (obj is BPDDL) { prefix = "Ddl"; }
            else if (obj is BPDir) { prefix = "Dir"; }
            else if (obj is BPEle) { prefix = "Ele"; }
            else if (obj is BPLabel) { prefix = "LB"; }
            else if (obj is BPLine) { prefix = "LE"; }
            else if (obj is BPLink) { prefix = "LK"; }
            else if (obj is BPM2M) { prefix = "M2M"; }
            else if (obj is BPRadioBtn) { prefix = "Rdb"; }

            else if (obj is BPImg) { prefix = "Img"; }
            else if (obj is BPImgAth) { prefix = "ImgAth"; }
            else if (obj is BPImgSeal) { prefix = "ImgSeal"; }
            else if (obj is BPMapPin) { prefix = "MapPin"; }
            else if (obj is BPMicrophonehot) { prefix = "MicPHot"; }
            else if (obj is BPTextBox) { prefix = "TB"; }
            else if (obj is BPDtl)
            {
                prefix = "Dtl";
            }

            if (obj is BPWorkCheck)
            {
                prefix = "WC" + Glo.FK_MapData.Replace("ND", "");
                return prefix;
            }

            if (obj is BPSubFlow)
            {
                prefix = "SF" + Glo.FK_MapData.Replace("ND", "");
                return prefix;
            }

            /*求出他们的同一个元素的连接串.*/
            string strs = "@";
            Type type = obj.GetType();
            foreach (FrameworkElement item in this.workSpace.Children)
            {
                if ((item is IElement) == false)
                    continue;

                if (item.GetType() == type)
                    strs += "@" + item.Name;
            }
            strs += "@";

            //求出一个新的id..
            for (int i = 1; i < 999; i++)
            {
                if (strs.Contains(prefix + i + "@") == true)
                    continue; /*如果包含就抛弃.*/
                switch (prefix)
                {
                    case "Dtl":
                        return Glo.FK_MapData + prefix + i;
                    case "AttachM":
                    case "Attach":
                        return prefix + i;
                    default:
                        return Glo.FK_MapData + prefix + i;
                    // return prefix + i;
                }
            }


            throw new Exception("@错误，没有形成期望的元素ID.");
            // 以下是 renpan 写的代码，用不到了.

            int maxSuffix = 0;
            foreach (FrameworkElement item in this.workSpace.Children)
            {
                if ((item is IElement) == false)
                    continue;

                if (item.GetType() == type)
                {
                    bool flag = false;
                    int numEle = 0;
                    string num = item.Name;
                    if (!string.IsNullOrEmpty(num) && num.Contains("_") && num.LastIndexOf("_") < num.Length)
                    {
                        flag = true;
                        num = num.Substring(num.LastIndexOf("_") + 1, num.Length - num.LastIndexOf("_") - 1);
                        if (!int.TryParse(num, out numEle))
                            flag = false;
                    }

                    if (flag)
                    {
                        if (numEle >= maxSuffix)
                            maxSuffix = numEle + 1;
                    }
                    else
                        maxSuffix++;
                }


                //元素名称定义规则：
                //prefix = prefix  + Glo.FK_Flow +maxSuffix.ToString();
                prefix = prefix + Glo.FK_Flow + DateTime.Now.ToString("ssfff") + maxSuffix.ToString();
                if (obj is BPLine || obj is BPLabel)
                {
                    prefix = prefix.Contains(Glo.FK_MapData) ? prefix : Glo.FK_MapData + "_" + prefix;
                }
                return prefix;
            }
        }
        #endregion

        #region 绘制网格线
        List<string> gridLineNames = new List<string>();
        public void SetGridLines(Canvas workSpace, bool isShow)
        {
            Color col = Color.FromArgb(255, 160, 160, 160);
            SetGridLines(workSpace, col, isShow, false);
        }

        public void SetGridLines(Canvas workSpace, Color bursh, bool isShow, bool isVirtualLine)
        {
            #region 清除
            foreach (string id in gridLineNames)
            {
                Line mylin = workSpace.FindName(id) as Line;
                if (mylin == null)
                    continue;
                if (workSpace.Children.Contains(mylin))
                    workSpace.Children.Remove(mylin);
            }
            gridLineNames.Clear();

            #endregion

            if (!isShow)
                return;

            #region 显示
            SolidColorBrush brush = new SolidColorBrush(bursh);

            double thickness = 0.3;
            double top = 0, left = 0;
            double width = workSpace.Width;
            double height = workSpace.Height;
            double stepLength = 40;
            double x, y;
            x = left + stepLength;
            y = top;

            while (x < width + left)
            {
                Line line = new Line();
                line.Name = "GLine" + x + "_" + y;
                line.X1 = x;
                line.Y1 = y;
                line.X2 = x;
                line.Y2 = y + height;

                line.Stroke = brush;
                line.StrokeThickness = thickness;
                if (isVirtualLine)  //设置虚线的方法
                    line.StrokeDashArray = new DoubleCollection() { thickness, thickness }; ;

                line.Stretch = Stretch.Fill;
                workSpace.Children.Add(line);
                x += stepLength;
                gridLineNames.Add(line.Name);
            }

            x = left;
            y = top + stepLength;

            while (y < height + top)
            {
                Line line = new Line();
                line.Name = "GLine" + x + "_" + y;
                line.X1 = x;
                line.Y1 = y;
                line.X2 = x + width;
                line.Y2 = y;

                line.Stroke = brush;
                line.Stretch = Stretch.Fill;
                line.StrokeThickness = thickness;
                if (isVirtualLine)  //设置虚线的方法
                    line.StrokeDashArray = new DoubleCollection() { thickness, thickness }; ;

                workSpace.Children.Add(line);
                y += stepLength;
                gridLineNames.Add(line.Name);
            }
            #endregion
        }
        #endregion

        #region 关于撤销与恢复的处理.未实现
        public void DoRecStep(string doType, UIElement obj, double x1, double y1, double x2, double y2)
        {
            Glo.CurrOpStep = Glo.CurrOpStep + 1;
            FuncStep en = new FuncStep();
            en.DoType = doType;
            en.Ele = obj;
            en.X1 = x1;
            en.X2 = x2;
            en.Y1 = y1;
            en.Y2 = y2;
            Glo.FuncSteps.Add(en);
        }
        public void DoRecStep(string doType, UIElement obj)
        {
            DoRecStep(doType, obj, 0, 0, 0, 0);
        }
        #endregion 关于撤销与恢复的处理.

        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            base.OnKeyDown(e);
            this.delPoint();

            switch (e.Key)
            {
                case Key.C:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                        MouseEventHandlers.IsCopy = true;
                    break;

                case Key.Escape:
                    // 取消选择
                    if (this.selectedElements != null && this.selectedElements.Count > 0)
                    {
                        SetSelectedTool(ToolBox.Mouse);
                    }// 退出当前窗口
                    else if (this.LoadSource)
                    {
                        if (MessageBox.Show("是否退出当前表单", "关闭", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            Canvas canvas = new Canvas();
                            canvas = this.workSpace;
                            BP.SL.Glo.SaveUIElementAsPng(canvas, 2, Glo.FK_MapData);

                            if (Closed != null)
                                Closed();
                        }
                    }
                    break;

                // 向上
                case Key.W:
                case Key.Up:
                    foreach (FrameworkElement item in selectedElements)
                    {
                        if (item is BPLine)
                        {
                            BPLine line = item as BPLine;
                            if (line != null)
                            {
                                line.MyLine.Y1 += -1;
                                line.MyLine.Y2 += -1;
                            }
                            continue;
                        }
                        else if (item is BPLabel)
                        {
                            BPLabel lab = item as BPLabel;
                            if (lab != null)
                            {
                                lab.ToUp();
                            }
                            continue;
                        }
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            if (item is IElement && (item as IElement).IsCanReSize)
                                if (item.Height > 18)
                                    item.Height += -1;
                        }
                        else
                        {
                            item.SetValue(Canvas.TopProperty, Canvas.GetTop(item) - 1);
                        }
                    }
                    break;

                // 向下
                case Key.S:
                case Key.Down:
                    if (e.Key == Key.S)
                    {
                        // 保存
                        if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Windows)
                        {
                            this.Save();
                            break;
                        }
                    }
                    foreach (FrameworkElement item in selectedElements)
                    {
                        if (item is BPLine)
                        {
                            BPLine line = item as BPLine;
                            if (line != null)
                            {
                                line.MyLine.Y1 += 1;
                                line.MyLine.Y2 += 1;
                            }
                            continue;
                        }
                        else if (item is BPLabel)
                        {
                            BPLabel lab = item as BPLabel;
                            if (lab != null)
                            {
                                lab.ToDown();
                            }
                            continue;
                        }

                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            if (item is IElement && (item as IElement).IsCanReSize)
                            {
                                item.Height += 1;
                            }
                        }
                        else
                        {
                            item.SetValue(Canvas.TopProperty, Canvas.GetTop(item) + 1);
                        }
                    }
                    break;

                // 向右
                case Key.D:
                case Key.Right:
                    foreach (FrameworkElement item in selectedElements)
                    {
                        if (item is BPLine)
                        {
                            BPLine line = item as BPLine;
                            if (line != null)
                            {
                                line.MyLine.X1 += 1;
                                line.MyLine.X2 += 1;
                                continue;
                            }
                        }
                        else if (item is BPLabel)
                        {

                            BPLabel lab = item as BPLabel;
                            if (lab != null)
                            {
                                lab.ToRight();
                                continue;
                            }
                        }
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            if (item is IElement && (item as IElement).IsCanReSize)
                            {
                                item.Width += 1;
                            }
                        }
                        else
                        {
                            item.SetValue(Canvas.LeftProperty, Canvas.GetLeft(item) + 1);
                        }
                    }
                    break;

                // 向左.
                case Key.A:
                case Key.Left:

                    if (e.Key == Key.A && Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        selectAll();
                        break;
                    }
                    foreach (FrameworkElement item in selectedElements)
                    {
                        if (item is BPLine)
                        {
                            BPLine line = item as BPLine;
                            if (line != null)
                            {
                                line.MyLine.X1 += -1;
                                line.MyLine.X2 += -1;

                                continue;
                            }
                        }
                        else if (item is BPLabel)
                        {
                            BPLabel lab = item as BPLabel;
                            if (lab != null)
                            {
                                lab.ToLeft();
                                continue;
                            }
                        }
                        if (Keyboard.Modifiers == ModifierKeys.Shift)
                        {
                            if (item is IElement && (item as IElement).IsCanReSize)
                                if (item.Height > 18)
                                    item.Width += -1;
                        }
                        else
                        {
                            item.SetValue(Canvas.LeftProperty, Canvas.GetLeft(item) - 1);
                        }
                    }
                    break;
                case Key.Delete: //删除.
                    this.DeleteCurrSelectUI();
                    break;
                default:
                    break;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            e.Handled = true;
            base.OnKeyUp(e);
            //check for the specific 'v' key, then check modifiers
            if (e.Key == Key.V)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    //specific Ctrl+V action here
                    this.Paste();
                }
            }
        }



        #region UnUsed

        void treeViewItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
            //if (_doubleClickTimer.IsEnabled)
            //{
            //    _doubleClickTimer.Stop();
            //    TreeViewItem tvItem = this.tvNode.SelectedItem as TreeViewItem;
            //    if (tvItem == null)
            //        return;

            //    if (tvItem.Tag == null)
            //    {
            //        Glo.FK_MapData = tvItem.Name.ToString();
            //        Glo.FK_Node = int.Parse(tvItem.Name.Replace("ND", ""));
            //    }
            //    else
            //    {
            //        string[] strs = tvItem.Name.Split('_');
            //        Glo.FK_Node = int.Parse(strs[0]);
            //        if (strs.Length == 3)
            //        {
            //            Glo.IsDtlFrm = true;
            //            Glo.FK_MapData = strs[2];
            //        }
            //        else
            //        {
            //            Glo.IsDtlFrm = false;
            //            Glo.FK_MapData = strs[1];
            //        }
            //    }
            //    this.BindFrm();
            //}
            //else
            //{
            //    _doubleClickTimer.Start();
            //}
        }
        private void tvmi_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //e.Handled = true;
            //MenuItem item = sender as MenuItem;
            //TreeViewItem li = this.tvNode.SelectedItem as TreeViewItem;
            //if (li == null)
            //    return;

            //switch (item.Name)
            //{
            //    case "FrmBill":
            //        MessageBox.Show("设置方式，请详看操作说明书的配置章节，可视化的设计在开发过程中。");
            //        break;
            //    case "FrmRef":
            //        this.BindTreeView();
            //        break;
            //    case "FrmAction":
            //        //FrmEvent frm = new FrmEvent();
            //        //frm.Show();
            //        string host = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/FrmEvent.aspx?FK_MapData=" + Glo.FK_MapData;
            //        HtmlPage.Window.Eval("window.showModalDialog('" + host + "',window,'dialogHeight:600px;dialogWidth:450px;center:Yes;help:No;scroll:auto;resizable:1;status:No;');");
            //        return;
            //    case "RefFrm":
            //        if (li.Tag != null)
            //        {
            //            BP.WF.FrmNode myfn = new BP.WF.FrmNode(li.Tag.ToString());
            //            this.winNodeFrms.FK_Node = myfn.FK_Node;
            //        }
            //        else
            //        {
            //            this.winNodeFrms.FK_Node = int.Parse(li.Name.Replace("ND", ""));
            //        }
            //        this.winNodeFrms.listBox1.Items.Clear();
            //        this.winNodeFrms.Show();
            //        break;
            //    case "NewFrm":
            //        // BP.WF.FrmNode fn = new BP.WF.FrmNode(li.Tag.ToString());

            //        this.winFlowFrm.TB_No.Text = "";
            //        this.winFlowFrm.TB_No.IsEnabled = true;
            //        this.winFlowFrm.TB_Name.Text = "";
            //        this.winFlowFrm.TB_PTable.Text = "";
            //        this.winFlowFrm.TB_URL.Text = "";
            //        this.winFlowFrm.NodeID = int.Parse(li.Name.Replace("ND", ""));
            //        this.winFlowFrm.Show();
            //        break;
            //    case "DeleteFrm":
            //        if (li.Tag == null)
            //            return;

            //        if (Glo.IsDtlFrm == true)
            //        {
            //            if (MessageBox.Show("明细表单不能被删除.",
            //           "提示", MessageBoxButton.OK)
            //           == MessageBoxResult.No)
            //                return;
            //            return;
            //        }

            //        if (MessageBox.Show("您确实要删除吗？如果删除所有相关联的节点表单都会被删除!!!",
            //            "删除提示", MessageBoxButton.OKCancel)
            //            == MessageBoxResult.No)
            //            return;

            //        string[] strs = li.Name.Split('_');
            //        string fk_frm = strs[1];
            //        Glo.FK_MapData = "ND" + strs[0];
            //        Glo.FK_Node = int.Parse(strs[0]);
            //        this.DoTypeName = "DeleteFrm";
            //        this.DoType(this.DoTypeName, fk_frm, null, null, null, null);
            //        break;
            //    case "EditFrm":
            //        if (li.Tag == null)
            //            return;

            //        if (Glo.IsDtlFrm)
            //            return;

            //        BP.WF.FrmNode fn = new BP.WF.FrmNode(li.Tag.ToString());
            //        this.winFlowFrm.TB_No.Text = fn.No;
            //        this.winFlowFrm.TB_Name.Text = fn.Name;
            //        this.winFlowFrm.DDL_FrmType.SelectedIndex = fn.FormType;
            //        this.winFlowFrm.TB_URL.Text = fn.URL;
            //        this.winFlowFrm.TB_PTable.Text = fn.PTable;
            //        this.winFlowFrm.CB_IsReadonly.IsChecked = fn.IsReadonly;
            //        this.winFlowFrm.NodeID = fn.FK_Node;
            //        this.winFlowFrm.Show();
            //        break;
            //    case "DeFrm": // 设计表单.
            //        Glo.IsDtlFrm = false;
            //        if (li.Tag == null)
            //        {
            //            Glo.FK_MapData = li.Name as string;
            //            if (Glo.FK_MapData.Contains("ND"))
            //                Glo.FK_Node = int.Parse(Glo.FK_MapData.Replace("ND", ""));
            //        }
            //        else
            //        {
            //            string[] str = li.Name.Split('_');
            //            Glo.FK_Node = int.Parse(str[0]);
            //            if (str.Length == 3)
            //            {
            //                Glo.IsDtlFrm = true;
            //                Glo.FK_MapData = str[2];
            //            }
            //            else
            //            {
            //                Glo.FK_MapData = str[1];
            //                Glo.IsDtlFrm = true;
            //            }
            //        }
            //        this.BindFrm();
            //        break;
            //    case "FrmUp":   // 上移.
            //    case "FrmDown": // 上移.
            //        if (li.Name.ToString().Contains("ND"))
            //            return;

            //        string[] strs1 = li.Name.Split('_');
            //        Glo.FK_MapData = strs1[1];
            //        Glo.FK_Node = int.Parse(strs1[0]);
            //        this.DoType(item.Name, Glo.FK_Node.ToString(), Glo.FK_MapData, null, null, null);
            //        break;
            //    default:
            //        break;
            //}
        }


        public void BindTreeView()
        {
            BindTreeView(Glo.FK_Flow);
        }
        public void BindTreeView(string FK_Flow)
        {
            string
            sqls = "SELECT NodeID, Name,Step FROM WF_Node WHERE FK_Flow='" + FK_Flow + "'";
            sqls += "@SELECT * FROM WF_FrmNode WHERE FK_Flow='" + FK_Flow + "' AND FK_Frm IN (SELECT No FROM Sys_MapData ) ORDER BY FK_Node, Idx";
            sqls += "@SELECT * FROM Sys_MapData ";
            sqls += "@SELECT * FROM Sys_MapDtl WHERE  FK_MapData IN( SELECT No FROM Sys_MapData WHERE FK_Flow='" + FK_Flow + "') AND DtlShowModel=1";
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLReturnTableSAsync(sqls, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableSCompleted += (object sender, FF.RunSQLReturnTableSCompletedEventArgs e) =>
            {
                if (null != e.Error)
                {
                    BP.SL.LoggerHelper.Write(e.Error);
                    MessageBox.Show(e.Error.Message);
                    return;
                }
                try
                {
                    DataSet ds = new DataSet();
                    ds.FromXml(e.Result);

                }
                catch (Exception ee)
                {
                    BP.SL.LoggerHelper.Write(ee);
                    MessageBox.Show(ee.Message);
                }
            };
        }
        //private string DoTypeName = null;
        //public void RunSQL(string sql)
        //{
        //    FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
        //    da.RunSQLsAsync(sql);
        //    da.RunSQLsCompleted += new EventHandler<FF.RunSQLsCompletedEventArgs>(
        //        (object sender, FF.RunSQLsCompletedEventArgs e) =>
        //        {
        //            if (e.Error != null)
        //            {
        //                MessageBox.Show(e.Error.Message, "执行信息", MessageBoxButton.OK);
        //                return;
        //            }

        //            DoTypeCompleted(this.DoTypeName);
        //        });
        //}
        //public void DoType(string doType, string v1, string v2, string v3, string v4, string v5)
        //{
        //    this.DoTypeName = doType;
        //    FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
        //    da.DoTypeAsync(doType, v1, v2, v3, v4, v5);
        //    da.DoTypeCompleted += new EventHandler<FF.DoTypeCompletedEventArgs>((object sender, FF.DoTypeCompletedEventArgs e)=>
        //    {
        //        if (e.Error != null)
        //        {
        //            MessageBox.Show(e.Error.Message, "执行信息", MessageBoxButton.OK);
        //            return;
        //        }
        //        if (e.Result != null)
        //        {
        //            MessageBox.Show(e.Result, "执行信息", MessageBoxButton.OK); 
        //            return;
        //        }

        //        DoTypeCompleted(doType);
        //    });
        //}
        //void DoTypeCompleted(string doType)
        //{
        //    switch (doType)
        //    {
        //        case "FrmUp":
        //        case "FrmDown":
        //        case "DeleteFrm":
        //            this.BindTreeView();
        //            break;
        //        default:
        //              MessageBox.Show("暂未实现"+ doType);
        //            break;
        //    }
        //}

        #endregion
    }
}
