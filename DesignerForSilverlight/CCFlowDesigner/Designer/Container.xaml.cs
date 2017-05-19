#region
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
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.IO;
using Ccflow.Web.Component.Workflow;
using WF;
using WF.WS;
using Liquid;
using System.ServiceModel;
using Silverlight;
using BP.Controls;
using System.Windows.Browser;
using WF.Designer;
using BP.DA;
using System.Windows.Media.Imaging;
#endregion
namespace BP
{
    public partial class Container : IContainer
    {
        double DesignerHeight, DesignerWdith;
        #region 变量
        /// <summary>
        /// 结点ID
        /// </summary>
        public int NodeID;
        /// <summary>
        /// 服务
        /// </summary>
        WSDesignerSoapClient _service = Glo.GetDesignerServiceInstance();//new BasicHttpBinding(), address);
        /// <summary>
        /// 页面编辑类型
        /// </summary>
        PageEditType editType = PageEditType.None;
        /// <summary>
        /// 选中结点
        /// </summary>
        FlowNode nodeSelected = null;


        /// <summary>
        /// 结点集合
        /// </summary>
        public List<FlowNode> flowNodeCollections;
        /// <summary>
        /// 连接线集合
        /// </summary>
        public List<Direction> directionCollections;
        /// <summary>
        /// 标签集合
        /// </summary>
        public List<NodeLabel> lableCollections;
        /// <summary>
        /// 
        /// </summary>
        int nextMaxIndex = 0;

        Stack<string> _workFlowXmlNextStack;
        Stack<string> _workFlowXmlPreStack;
        Dictionary<string, FlowNode> dicFlowNode = new Dictionary<string, FlowNode>();
        string workflowXmlCurrent = @"";

        Point mousePosition;
        bool trackingMouseMove = false;
        Rectangle temproaryEllipse;
        NodeLabel l = null;

        /// <summary>
        /// 弹出窗口标题
        /// </summary>
        string WinTitle;

        Point pos;
        #endregion


        #region FlowChartType变化：一般是设计器已经加载，需要重绘设计器背景和网格
        /// <summary>
        /// 设计器:背景和网格
        /// </summary>
        Color colorBackground = Color.FromArgb(250, 224, 238, 224);
        public Color ColorBackground
        {
            set
            {
                if (value != colorBackground)
                {
                    colorBackground = value;
                    cnsDesignerContainer.Background = new SolidColorBrush(value);
                }
            }
        }
        Color colorGrid = Colors.DarkGray;
        FlowChartType flow_ChartType = FlowChartType.UnKnown;
        /// <summary>
        /// 流程图样式
        /// 更改时：
        /// 1）更新网格样式，包括网格，设计器背景
        /// 2）重绘流程
        /// </summary>
        public FlowChartType Flow_ChartType
        {
            get
            {
                return flow_ChartType;
            }
            set
            {
                if (value == flow_ChartType) return;
                flow_ChartType = value;

                if (flow_ChartType == FlowChartType.UserIcon)
                {

                    colorGrid = Color.FromArgb(255, 220, 220, 220);
                    //ColorBackground = Color.FromArgb(255, 33, 99, 255);
                    ColorBackground = Colors.White;

                }
                else
                {
                    colorGrid = Colors.DarkGray;
                    ColorBackground = Color.FromArgb(255, 224, 238, 224);

                }

                bool result = GenerateFlowChart(ds);
                if (result)
                    ds = null;
                setContainerStyle();
                Glo.Loading(false);
            }
        }
        void setContainerStyle()
        {
            Content_Resized(this, null);
            this.Visibility = System.Windows.Visibility.Visible;
        }
        /// <summary>
        /// 切换流程样式
        /// </summary>
        /// <param name="value"></param>
        void SetChartType(FlowChartType value)
        {
            if (value != flow_ChartType)
            {
                string msg = "确定把当前流程从" + flow_ChartType.ToString() + "切换到" + value.ToString() + "样式";
                if (MessageBox.Show(msg, "确定更换流程样式？", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    this.flow_ChartType = value;

                    // 更新数据库
                    // 重新加载数据
                    this.DrawFlows();
                }
            }
        }


        //public void SetGridLines()
        //{
        //    ImageBrush imageBrush = new ImageBrush()
        //    {
        //         Stretch = Stretch.None
        //    };
        //    imageBrush.ImageSource = new BitmapImage(new Uri("designer.png", UriKind.Relative));
        //    this.cnsDesignerContainer.Background = imageBrush;
        //}

        Canvas _gridLinesContainer;
        public Canvas GridLinesContainer
        {
            get
            {
                if (_gridLinesContainer == null)
                {
                    _gridLinesContainer = new Canvas() { Name = "canGridLinesContainer" };
                }

                if (!cnsDesignerContainer.Children.Contains(_gridLinesContainer))
                {
                    cnsDesignerContainer.Children.Add(_gridLinesContainer);
                }


                return _gridLinesContainer;
            }
        }

        public void SetGridLines(bool isShow)
        {
            GridLinesContainer.Children.Clear();

            if (!isShow)
                return;

            double width = cnsDesignerContainer.Width;
            double height = cnsDesignerContainer.Height;

            if (double.IsNaN(width) || double.IsNaN(height))
                return;

            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = colorGrid;
            double thickness = 1, top = 0, left = 0, stepLength = 40;

            double x, y;
            x = left + stepLength;
            y = top;

            while (x < width + left)
            {
                Line line = new Line();
                line.X1 = x;
                line.Y1 = y;
                line.X2 = x;
                line.Y2 = y + height;

                line.Stroke = brush;
                line.StrokeThickness = thickness;
                ////设置虚线的方法
                //DoubleCollection dc = new DoubleCollection() { thickness, thickness };
                //line.StrokeDashArray = dc;

                line.Stretch = Stretch.Fill;
                GridLinesContainer.Children.Add(line);
                x += stepLength;
            }


            x = left;
            y = top + stepLength;

            while (y < height + top)
            {
                Line line = new Line();
                line.X1 = x;
                line.Y1 = y;
                line.X2 = x + width;
                line.Y2 = y;

                line.Stroke = brush;
                line.Stretch = Stretch.Fill;
                ////设置虚线的方法
                //DoubleCollection dc = new DoubleCollection() { thickness, thickness };
                //line.StrokeDashArray = dc;

                line.StrokeThickness = thickness;
                GridLinesContainer.Children.Add(line);
                y += stepLength;
            }
        }
        #endregion


        #region 属性


        /// <summary>
        /// 当前是否为添加回退连线状态
        /// </summary>
        public bool IsReturnTypeDir { get; set; }

        /// <summary>
        /// 当前是否有节点在编辑状态
        /// </summary>
        public bool IsSomeChildEditing { get; set; }
        /// <summary>
        /// 流程当前是否保存
        /// </summary>
        private bool isNeedSave = false;
        public bool IsNeedSave
        {
            get { return isNeedSave; }
            set
            {
                isNeedSave = value;
                //如果需要保存变化时，将流程名称加粗并在前面加一个星号，反之则恢复正常模式
                if (null != MainPage.Instance)
                {
                    var currentItem = MainPage.Instance.tbDesigner.SelectedItem as TabItemEx;
                    if (null != currentItem)
                    {
                        var txtTitle = ((Panel)(currentItem.Header)).Children[1] as TextBlock;
                        if (null != txtTitle)
                        {
                            if (isNeedSave && !txtTitle.Text.StartsWith("*"))
                            {
                                txtTitle.Text = "*" + txtTitle.Text;
                                txtTitle.FontWeight = FontWeights.Bold;
                            }
                            if (!isNeedSave)
                            {
                                txtTitle.Text = txtTitle.Text.Trim('*');
                                txtTitle.FontWeight = FontWeights.Normal;
                            }

                        }
                    }
                }
            }
        }
        public string FK_Flow
        {
            get
            {
                return this.FlowID;
            }
        }
        public string FlowID
        {
            get;
            set;
        }
        public string WorkID
        {
            get { return ""; }
        }
        public bool IsContainerRefresh { get; set; }
        public bool MouseIsInContainer { get; set; }


        public Double ContainerWidth
        {
            get
            {
                return cnsDesignerContainer.Width;
            }
            set
            {
                cnsDesignerContainer.Width = value;
                //sliWidth.Value = value;
            }
        }

        public Double ContainerHeight
        {
            get
            {
                return cnsDesignerContainer.Height;
            }
            set
            {
                cnsDesignerContainer.Height = value;
                //sliHeight.Value = value;

            }
        }

        public Double ScrollViewerHorizontalOffset
        {
            get
            {
                return svContainer.HorizontalOffset;
            }
            set
            {
                svContainer.ScrollToHorizontalOffset(value);

            }
        }

        public Double ScrollViewerVerticalOffset
        {
            get
            {
                return svContainer.VerticalOffset;
            }
            set
            {
                svContainer.ScrollToVerticalOffset(value);
            }
        }

        public WSDesignerSoapClient _Service
        {
            get { return _service; }
            set { _service = value; }
        }

        public List<FlowNode> FlowNodeCollections
        {
            get
            {
                if (flowNodeCollections == null)
                {
                    flowNodeCollections = new List<FlowNode>();
                }
                return flowNodeCollections;
            }
        }

        public List<Direction> DirectionCollections
        {
            get
            {
                if (directionCollections == null)
                {
                    directionCollections = new List<Direction>();
                }
                return directionCollections;
            }
        }

        public List<NodeLabel> LableCollections
        {
            get
            {
                if (lableCollections == null)
                {
                    lableCollections = new List<NodeLabel>();
                }
                return lableCollections;
            }
        }

        /// <summary>
        /// 对象顺序
        /// </summary>
        public int NextMaxIndex
        {
            get
            {
                nextMaxIndex++;
                return nextMaxIndex;
            }
        }
        /// <summary>
        /// 左边界
        /// </summary>
        public double Left
        {
            get
            {
                return 230;
            }
        }
        /// <summary>
        /// 右边界
        /// </summary>
        public double Top
        {
            get
            {
                return 100;
            }

        }
        /// <summary>
        /// 新建结点名字
        /// </summary>
        public string NewNodeName
        {
            get;
            set;
        }
        public Stack<string> WorkFlowXmlNextStack
        {
            get
            {
                if (_workFlowXmlNextStack == null)
                    _workFlowXmlNextStack = new Stack<string>(50);
                return _workFlowXmlNextStack;
            }
        }

        public Stack<string> WorkFlowXmlPreStack
        {
            get
            {
                if (_workFlowXmlPreStack == null)
                    _workFlowXmlPreStack = new Stack<string>(50);
                return _workFlowXmlPreStack;
            }
        }
        int MoveStepLenght
        {
            get
            {
                if (CtrlKeyIsPress)
                    return 5;
                return 1;
            }
        }
        public Direction CurrentTemporaryDirection { get; set; }

        List<IElement> _SelectedWFElements;
        public List<IElement> SelectedWFElements
        {
            get
            {
                if (_SelectedWFElements == null)
                    _SelectedWFElements = new List<IElement>();
                return _SelectedWFElements;

            }
        }
        public bool CtrlKeyIsPress
        {
            get
            {
                return (Keyboard.Modifiers == ModifierKeys.Control);
                //return ctrlKeyIsPress;
            }
        }
        public bool IsMouseSelecting
        {
            get
            {
                return (temproaryEllipse != null);
            }
        }

        List<IElement> copyElementCollectionInMemory;
        public List<IElement> CopyElementCollectionInMemory
        {
            get
            {
                if (copyElementCollectionInMemory == null)
                    copyElementCollectionInMemory = new List<IElement>();
                return copyElementCollectionInMemory;
            }
            set
            {
                copyElementCollectionInMemory = value;
            }
        }
        public PageEditType EditType
        {
            get
            {
                if (editType == PageEditType.None)
                {
                    editType = PageEditType.Add;
                }
                return editType;
            }
            set
            {
                editType = value;
            }
        }
        private Point GetPosition { get; set; }
        #endregion


        public Container()
        {
            InitializeComponent();
            MessageBody.Visibility = Visibility.Collapsed;

            menuFlowNode.Container = this;
            menuLabel.Container = this;
            menuDirection.Container = this;
            menuDirection.Visibility = Visibility.Collapsed;
            menuFlowNode.Visibility = Visibility.Collapsed;
            menuLabel.Visibility = Visibility.Collapsed;
            menuContainer.Visibility = Visibility.Collapsed;
            menuContainer.Container = this;


            HtmlPage.Document.AttachEvent("oncontextmenu", OnContextMenu);
            Application.Current.Host.Content.Resized += Content_Resized;
            this.Visibility = System.Windows.Visibility.Collapsed;
        }


        void Content_Resized(object sender, EventArgs e)
        {
            #region
            try
            {
                var contentWidth = Application.Current.Host.Content.ActualWidth;
                cnsDesignerContainer.Width = DesignerWdith > contentWidth ? DesignerWdith : contentWidth;

                var contentHeight = Application.Current.Host.Content.ActualHeight;
                cnsDesignerContainer.Height = DesignerHeight > contentHeight ? DesignerHeight : contentHeight;
            }
            catch (System.Exception ex)
            {
                Glo.ShowException(ex, "屏幕自适应错误");
            }
            SetGridLines(true);
            #endregion
        }

        public Container(string flowid)
            : this()
        {
            this.FlowID = flowid;
        }


        FlowNode[] getFlowNodeRightBottom()
        {
            FlowNode[] nodes = null;
            if (null == FlowNodeCollections || FlowNodeCollections.Count <= 0)
                return nodes;

            nodes = new FlowNode[2];
            nodes[0] = nodes[1] = FlowNodeCollections[0];

            foreach (var node in dicFlowNode.Values)
            {
                var left = node.Position.X;
                if (nodes[0].Position.X < node.Position.X)
                    nodes[0] = node;

                if (nodes[1].Position.Y < node.Position.Y)
                    nodes[1] = node;
            }
            return nodes;
        }
        public void DragNodeResizeContainer(FlowNode node)
        {
            FlowNode[] nodes = getFlowNodeRightBottom();

            if (null != nodes)
            {
                bool sizeChanged = false;

                // maxLeft
                if (node.NodeID == nodes[0].NodeID)
                {
                    var left = node.Position.X + node.UIWidth;
                    var hostWidth = this.cnsDesignerContainer.ActualWidth;
                    if (left > hostWidth)
                    {
                        left = left - hostWidth;
                        this.cnsDesignerContainer.Width += left;
                        this.svContainer.ScrollToHorizontalOffset(this.svContainer.HorizontalOffset + left + node.UIWidth);
                        sizeChanged = true;
                    }
                    else
                    {
                        left = left - hostWidth;
                        var newWidth = this.cnsDesignerContainer.Width + left;
                        if (Application.Current.Host.Content.ActualWidth < newWidth)
                        {
                            this.cnsDesignerContainer.Width = newWidth;
                            this.svContainer.ScrollToHorizontalOffset(this.svContainer.HorizontalOffset + left);
                            sizeChanged = true;
                        }
                    }
                }
                // maxTop
                if (node.NodeID == nodes[1].NodeID)
                {
                    var top = node.Position.Y + node.UIHeight;
                    var hostHeight = this.cnsDesignerContainer.ActualHeight;
                    if (top > hostHeight)
                    {
                        top = top - hostHeight;
                        this.cnsDesignerContainer.Height += top;
                        this.svContainer.ScrollToVerticalOffset(this.svContainer.VerticalOffset + top + node.UIHeight);
                        sizeChanged = true;
                    }
                    else
                    {
                        top = top - hostHeight;
                        var newHeight = this.cnsDesignerContainer.Height + top;
                        if (newHeight > Application.Current.Host.Content.ActualHeight)
                        {
                            this.cnsDesignerContainer.Height = newHeight;
                            this.svContainer.ScrollToVerticalOffset(this.svContainer.VerticalOffset + top);
                            sizeChanged = true;
                        }
                    }
                }
                if (sizeChanged)
                {
                    this.SetGridLines(true);
                }
            }
        }

        public bool Contains(UIElement uie)
        {
            return cnsDesignerContainer.Children.Contains(uie);
        }
        public CheckResult CheckSave()
        {
            var cr = new CheckResult();
            cr.IsPass = true;
            return cr;
        }

        #region NewFlow
        public void NewFlow()
        {
            NewFlow(null);
        }
        public void NewFlow(string flowsort)
        {
            if (cnsDesignerContainer.Children.Count > 0)
            {
                if (HtmlPage.Window.Confirm("确定要保存吗？"))
                {
                }
                else
                {
                    return;
                }
            }
            clearContainer();

            if (flowsort != null)
            {
                _Service.DoAsync("NewFlow", flowsort, true);
            }
            else
            {
                _Service.DoAsync("NewFlow", null, true);
            }
            _Service.DoCompleted += _service_DoCompleted;
        }

        void _service_DoCompleted(object sender, DoCompletedEventArgs e)
        {
            _Service.DoCompleted -= _service_DoCompleted;

            if (null != e.Error)
            {
                Glo.ShowException(e.Error, "NewFlow出错");
                return;
            }
            else if (!string.IsNullOrEmpty(e.Result))
            {
                FlowID = e.Result;
                this.DrawFlows(FlowID);
            }
        }
        #endregion

        #region LoadFlow
        DataSet ds;

        public Container DrawFlows()
        {
            DrawFlows(this.FlowID);
            return this;
        }

        public void UpdateFlow(FlowChartType type)
        {
            this.cnsDesignerContainer.Cursor = Cursors.Wait;

            string sql = "UPDATE WF_Flow SET ChartType={0} WHERE No='{1}' ";
            sql = string.Format(sql, (int)type, FlowID);
            var ws = Glo.GetDesignerServiceInstance();
            ws.RunSQLAsync(sql, Glo.UserNo, Glo.SID);
            ws.RunSQLCompleted += delegate(object sender, global::WF.WS.RunSQLCompletedEventArgs e)
            {
                this.cnsDesignerContainer.Cursor = Cursors.Arrow;

                if (e.Error != null)
                {
                    BP.Glo.ShowException(e.Error);
                }
                else
                {
                    int result = e.Result;
                    this.DrawFlows();
                }
            };
        }
        public void DrawFlows(string flowID)
        {
            this.FlowID = flowID;
            WSDesignerSoapClient da = Glo.GetDesignerServiceInstance();
            string
            sqls = "SELECT NodeID,Name,X,Y,NodePosType,HisToNDs, RunModel,Icon FROM WF_Node WHERE FK_Flow='" + flowID + "'";
            sqls += "@SELECT Node,ToNode,DirType,IsCanBack,Dots FROM WF_Direction WHERE FK_Flow='" + flowID + "'";
            sqls += "@SELECT * FROM WF_LabNote WHERE FK_Flow='" + flowID + "'";
            sqls += "@SELECT ChartType,Paras FROM WF_Flow WHERE No='" + flowID + "'";

            Glo.Loading(true);
            da.RunSQLReturnTableSAsync(sqls, Glo.UserNo, Glo.SID);
            da.RunSQLReturnTableSCompleted += (object sender, RunSQLReturnTableSCompletedEventArgs e) =>
            {
                Glo.Loading(false);
                bool toBeContinued = true;
                System.Exception exc = null;
                if (null != e.Error)
                {
                    exc = e.Error;
                    toBeContinued = false;
                }

                if (toBeContinued)
                {
                    try
                    {
                        ds = new DataSet();
                        ds.FromXml(e.Result);
                    }
                    catch (System.Exception ex)
                    {
                        exc = ex;
                        toBeContinued = false;
                    }
                }

                if (!toBeContinued)
                {
                    if (exc != null)
                    {
                        Glo.ShowException(exc, "流程加载错误");
                    }
                    setContainerStyle();
                }
                else if (ds != null)
                {
                    DataTable dtFlow = ds.Tables[3];
                    if (null != dtFlow && 1 == dtFlow.Rows.Count)
                    {
                        // 设置流程样式，变化时切换背景色，网格线，流程节点样式
                        string chartType = dtFlow.Rows[0]["ChartType"];
                        if (chartType == "1")
                        {
                            this.Flow_ChartType = FlowChartType.UserIcon;
                        }
                        else //if (chartType == "0")
                        {
                            this.Flow_ChartType = FlowChartType.Shape;
                        }

                    }
                }
            };
        }

        bool GenerateFlowChart(DataSet ds)
        {
            bool result = false;
            if (null == ds) return result;
            try
            {
                cnsDesignerContainer.Children.Clear();
                dicFlowNode.Clear();
                FlowNodeCollections.Clear();
                DirectionCollections.Clear();
                LableCollections.Clear();
                #region 画节点.
                DataTable dtNode = ds.Tables[0];
                double maxWidth = DesignerWdith;
                double maxHeight = DesignerHeight;
                foreach (DataRow rowNode in dtNode.Rows)
                {
                    string icon = string.Empty;
                    if (this.flow_ChartType == FlowChartType.UserIcon)
                    {
                        icon = rowNode["Icon"];
                    }

                    FlowNodeType nodeType = (FlowNodeType)int.Parse(rowNode["RunModel"]);
                    NodePosType postype = (NodePosType)int.Parse(rowNode["NodePosType"]);
                    double x = double.Parse(rowNode["X"]);
                    double y = double.Parse(rowNode["Y"]);

                    FlowNode flowNode = new FlowNode((IContainer)this);
                    this.isNeedSave = false;
                    flowNode.NodeType = nodeType;
                    flowNode.HisPosType = postype;
                    flowNode.Icon = icon;
                    flowNode.SetValue(Canvas.ZIndexProperty, NextMaxIndex);
                    flowNode.FK_Flow = FlowID;
                    flowNode.NodeID = rowNode["NodeID"];
                    flowNode.NodeName = rowNode["Name"];

                    if (x < 50)
                        x = 50;
                    //else if (x > 1190)
                    //    x = 1190;

                    if (y < 30)
                        y = 30;
                    //else if (y > 770)
                    //    y = 770;

                    flowNode.CenterPoint = new Point(x, y);
                    AddUI(flowNode);

                    // 永远使设计器的宽和高为节点的最大值
                    maxHeight = Math.Max(maxHeight, flowNode.CenterPoint.Y + flowNode.UIHeight / 2);
                    maxWidth = Math.Max(maxWidth, flowNode.CenterPoint.X + flowNode.UIWidth / 2);
                }

                #endregion 画节点.

                #region 画线
                DataTable dtDirection = ds.Tables[1];
                foreach (DataRow rowDirection in dtDirection.Rows)
                {
                    string begin = rowDirection["Node"];
                    string to = rowDirection["ToNode"];

                    if (dicFlowNode.ContainsKey(begin) && dicFlowNode.ContainsKey(to))
                    {

                        Direction d = new Direction((IContainer)this);
                        d.DirType = (DirType)Enum.Parse(typeof(DirType), rowDirection["DirType"], false);
                        d.IsCanBack = (int.Parse(rowDirection["IsCanBack"]) == 0) ? false : true;

                        string dots1 = rowDirection["Dots"] as string;
                        if (string.IsNullOrEmpty(dots1) == false)
                        {
                            string[] strs = dots1.Split('@');
                            IList<double> dots = new List<double>();
                            foreach (string str in strs)
                            {
                                if (str == null || str == "")
                                    continue;
                                string[] mystr = str.Split(',');
                                if (mystr.Length == 2)
                                {
                                    dots.Add(double.Parse(mystr[0]));
                                    dots.Add(double.Parse(mystr[1]));
                                }
                            }

                            d.LineType = DirectionLineType.Polyline;
                            d.PointTurn1.CenterPosition = new Point(dots[0], dots[1]);
                            d.PointTurn2.CenterPosition = new Point(dots[2], dots[3]);
                        }

                        d.FlowID = FlowID;
                        d.BeginFlowNode = dicFlowNode[begin];
                        d.EndFlowNode = dicFlowNode[to];
                        AddUI(d);

                        if (d.LineType == DirectionLineType.Polyline)
                        {
                            maxHeight = Math.Max(maxHeight,
                                                      d.PointTurn1.CenterPosition.Y + d.PointTurn1.ActualHeight / 2);
                            maxWidth = Math.Max(maxWidth,
                                                     d.PointTurn1.CenterPosition.X + d.PointTurn1.ActualWidth / 2);
                            maxHeight = Math.Max(maxHeight,
                                                      d.PointTurn2.CenterPosition.Y + d.PointTurn2.ActualHeight / 2);
                            maxWidth = Math.Max(maxWidth,
                                                     d.PointTurn2.CenterPosition.X + d.PointTurn2.ActualWidth / 2);
                        }
                    }
                }
                #endregion 画线

                #region 画标签
                DataTable dtLabNote = ds.Tables[2];
                foreach (DataRow rowLabel in dtLabNote.Rows)
                {
                    NodeLabel r = new NodeLabel((IContainer)this);
                    r.LabelName = rowLabel["Name"];
                    r.Position = new Point(double.Parse(rowLabel["X"]), double.Parse(rowLabel["Y"]));
                    r.LableID = rowLabel["MyPK"];

                    AddUI(r);

                    maxHeight = Math.Max(maxHeight, r.Position.Y + Math.Ceiling(r.txtLabel.ActualHeight));
                    maxWidth = Math.Max(maxWidth, r.Position.X + Math.Ceiling(r.txtLabel.ActualWidth));
                }
                #endregion 画标签

                DesignerHeight = maxHeight + 10;
                DesignerWdith = maxWidth + 10;
                SetGridLines(true);
                //DrawVirtualNode(dtFlow.Rows[0]["Paras"]);
                result = true;
            }
            catch (System.Exception ex)
            {
                Glo.ShowException(ex, "流程加载错误");
            }

            //画线中会将此设置为true.
            IsNeedSave = false;
            return result;
        }

        #region VirtualNode

        /// 获取真实开始节点
        FlowNode GetBeginFlowNode()
        {
            FlowNode flowNode = null;

            if (FlowNodeCollections != null)
            {
                string beginId = (int.Parse(FlowID + "01")).ToString();

                if (dicFlowNode.ContainsKey(beginId))
                {
                    flowNode = dicFlowNode[beginId];
                }
                else//理论上不会到达
                {
                    foreach (FlowNode item in FlowNodeCollections)
                    {
                        //找一个作为开始节点
                        if (item != null
                            && (item.EndDirectionCollections == null || item.EndDirectionCollections.Count == 0)
                            && item.NodeID != "0" && item.NodeID != "1")
                        {
                            flowNode = item;
                            break;
                        }
                    }
                }
            }

            return flowNode;
        }

        /// 获取未连接到虚结束节点的实结束节点集合
        IList<FlowNode> GetEndFlowNodes()
        {
            IList<FlowNode> list = new List<FlowNode>();

            if (FlowNodeCollections != null)
            {
                foreach (FlowNode item in FlowNodeCollections)
                {
                    //找真实结束节点（不含虚节点）
                    if (item != null
                        && item.NodeID != "0" && item.NodeID != "1" && item.NodeID != (int.Parse(FlowID + "01")).ToString())
                    {
                        //作为首节点有出线，若均为回退线，仍为结束节点
                        if (item.BeginDirectionCollections != null)
                        {
                            bool isEnd = true;
                            foreach (Direction dir in item.BeginDirectionCollections)
                            {
                                if (dir.DirType != DirType.Backward)
                                {
                                    isEnd = false;
                                    break;
                                }
                            }

                            if (!isEnd)
                            {
                                continue;
                            }
                        }

                        list.Add(item);
                    }
                }
            }

            return list;
        }

        void InsertFlowNode(FlowNode node, double x, double y)
        {
            if (x < 50)
                x = 50;

            if (y < 30)
                y = 30;

            if (x > 1190)
                x = 1190;

            if (y > 770)
                y = 770;

            node.CenterPoint = new Point(x, y);

            AddUI(node);
        }
        #endregion
        #endregion

        /// <summary>
        /// 设计报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnDesignerTable()
        {
            Glo.OpenWinByDoType("CH", "WFRpt", this.FlowID, null, null);
        }
        /// <summary>
        /// 检查
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnCheck()
        {
            Glo.OpenWinByDoType("CH", "FlowCheck", FlowID, null, null);
        }
        /// <summary>
        /// 试运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnRun()
        {
            Glo.OpenWinByDoType("CH", "RunFlow", FlowID, null, null);
        }

        public void ShowSetting(Direction r)
        {
            this.WinTitle = "方向设置";
            Glo.OpenWinByDoType("CH", BP.UrlFlag.Dir, FlowID, r.BeginFlowNode.NodeID, r.EndFlowNode.NodeID);
        }
        public void LoadFromXmlString(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                return;

            FlowNodeType nodeType;
            MergePictureRepeatDirection repeatDirection = MergePictureRepeatDirection.None;
            string FlowID = "";
            int zIndex = 0;
            string NodeID = "";
            string NodeName = "";
            Point FlowNodePosition = new Point();
            double temd = 0;
            Byte[] b = System.Text.UTF8Encoding.UTF8.GetBytes(xml);
            XElement xele = XElement.Load(System.Xml.XmlReader.Create(new MemoryStream(b)));

            //txtWorkFlowName.Text = xele.Attribute(XName.Get("Name")).Value;

            FlowID = xele.Attribute(XName.Get("FlowID")).Value;

            var partNos = from item in xele.Descendants("FlowNode") select item;
            foreach (XElement node in partNos)
            {
                nodeType = (FlowNodeType)Enum.Parse(typeof(FlowNodeType), node.Attribute(XName.Get("Type")).Value, true);
                repeatDirection = (MergePictureRepeatDirection)Enum.Parse(typeof(MergePictureRepeatDirection), node.Attribute(XName.Get("RepeatDirection")).Value, true);
                FlowID = node.Attribute(XName.Get("FlowID")).Value;
                NodeID = node.Attribute(XName.Get("NodeID")).Value;
                NodeName = node.Attribute(XName.Get("NodeName")).Value;

                double.TryParse(node.Attribute(XName.Get("PositionX")).Value, out temd);
                FlowNodePosition.X = temd;

                double.TryParse(node.Attribute(XName.Get("PositionY")).Value, out temd);
                FlowNodePosition.Y = temd;
                int.TryParse(node.Attribute(XName.Get("ZIndex")).Value, out zIndex);

                FlowNode a = new FlowNode((IContainer)this);
                a.NodeType = nodeType;
                a.SubFlow = node.Attribute(XName.Get("SubFlow")).Value;
                a.RepeatDirection = repeatDirection;
                a.CenterPoint = FlowNodePosition;
                a.NodeID = NodeID;
                a.NodeName = NodeName;
                a.ZIndex = zIndex;
                a.EditType = this.EditType;
                a.FK_Flow = FlowID;

                AddUI(a);
            }

            string beginNodeID = "";
            string endNodeID = "";
            double beginPointX = 0;
            double beginPointY = 0;
            double endPointX = 0;
            double endPointY = 0;
            double turnPoint1X = 0;
            double turnPoint1Y = 0;
            double turnPoint2X = 0;
            double turnPoint2Y = 0;

            string ruleID = "";
            string ruleName = "";
            string beginFlowNodeFlowID = "";
            string endFlowNodeFlowID = "";
            double containerWidth = 0;
            double containerHeight = 0;
            DirectionLineType lineType = DirectionLineType.Line;
            double.TryParse(xele.Attribute(XName.Get("Width")).Value, out containerWidth);
            double.TryParse(xele.Attribute(XName.Get("Height")).Value, out containerHeight);

            ContainerWidth = containerWidth;
            ContainerHeight = containerHeight;


            FlowNode temFlowNode = null;
            partNos = from item in xele.Descendants("Direction") select item;
            foreach (XElement node in partNos)
            {
                lineType = (DirectionLineType)Enum.Parse(typeof(DirectionLineType), node.Attribute(XName.Get("LineType")).Value, true);

                FlowID = node.Attribute(XName.Get("FlowID")).Value;

                ruleID = node.Attribute(XName.Get("DirectionID")).Value;
                ruleName = node.Attribute(XName.Get("DirectionName")).Value;
                beginFlowNodeFlowID = node.Attribute(XName.Get("BeginFlowNodeFlowID")).Value;
                endFlowNodeFlowID = node.Attribute(XName.Get("EndFlowNodeFlowID")).Value;

                beginNodeID = node.Attribute(XName.Get("BeginNodeID")).Value;
                endNodeID = node.Attribute(XName.Get("EndNodeID")).Value;


                double.TryParse(node.Attribute(XName.Get("TurnPoint1X")).Value, out turnPoint1X);
                double.TryParse(node.Attribute(XName.Get("TurnPoint1Y")).Value, out turnPoint1Y);
                double.TryParse(node.Attribute(XName.Get("TurnPoint2X")).Value, out turnPoint2X);
                double.TryParse(node.Attribute(XName.Get("TurnPoint2Y")).Value, out turnPoint2Y);

                double.TryParse(node.Attribute(XName.Get("BeginPointX")).Value, out beginPointX);
                double.TryParse(node.Attribute(XName.Get("BeginPointY")).Value, out beginPointY);
                double.TryParse(node.Attribute(XName.Get("EndPointX")).Value, out endPointX);
                double.TryParse(node.Attribute(XName.Get("EndPointY")).Value, out endPointY);

                int.TryParse(node.Attribute(XName.Get("ZIndex")).Value, out zIndex);


                Direction r = new Direction(this, false, lineType);
                AddUI(r);
                r.DirectionID = ruleID;
                r.DirectionName = ruleName;
                r.ZIndex = zIndex;
                r.EditType = this.EditType;
                r.FlowID = FlowID;
                r.LineType = lineType;
                if (turnPoint1X > 0 && turnPoint2X > 0)
                {
                    r.IsTurnPoint1Moved = true;
                    r.IsTurnPoint2Moved = true;
                    r.PointTurn1.CenterPosition = new Point(turnPoint1X, turnPoint1Y);
                    r.PointTurn2.CenterPosition = new Point(turnPoint2X, turnPoint2Y);
                }
                if (beginFlowNodeFlowID != "")
                {
                    temFlowNode = getFlowNode(beginFlowNodeFlowID);
                    if (temFlowNode != null)
                        temFlowNode.AddDirection(r, DirectionMove.Begin);
                    else
                        r.PointBegin = new Point(beginPointX, beginPointY);

                }
                else
                {
                    r.PointBegin = new Point(beginPointX, beginPointY);
                }
                temFlowNode = null;
                if (endFlowNodeFlowID != "")
                {
                    temFlowNode = getFlowNode(endFlowNodeFlowID);
                    if (temFlowNode != null)
                        temFlowNode.AddDirection(r, DirectionMove.End);
                    else
                        r.PointEnd = new Point(endPointX, endPointY);
                }
                else
                {
                    r.PointEnd = new Point(endPointX, endPointY);
                }
            }


            partNos = from item in xele.Descendants("Label") select item;
            string labelName = "";
            double labelX = 0;
            double labelY = 0;
            foreach (XElement node in partNos)
            {
                labelName = node.Value;

                double.TryParse(node.Attribute(XName.Get("X")).Value, out labelX);
                double.TryParse(node.Attribute(XName.Get("Y")).Value, out labelY);

                NodeLabel l = new NodeLabel(this);
                l.LabelName = labelName;
                l.Position = new Point(labelX, labelY);
                AddUI(l);
            }
        }

        /// <summary>
        /// 先设置模态对话框的属性，再打开
        /// </summary>
        /// <param name="lang"></param>
        /// <param name="dotype"></param>
        /// <param name="fk_flow"></param>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <param name="title"></param>
        public void SetProper(string lang, string dotype, string fk_flow, string node1, string node2, string title)
        {
            this.WinTitle = title;
        }

        public void CreateDirection(Point point, DirType type)
        {
            this.cnsDesignerContainer.Cursor = Cursors.Wait;

            Direction r = new Direction(this);
            Point begin = new Point(point.X - 20, point.Y - 20), end = new Point(point.X + 20, point.Y + 20);
            if (type == DirType.Backward)
            {

                //if (MessageBox.Show("创建回退连接线，您可按下一个节点的中间，然后拖动到另外一个节点中间后松开手就完成了。现在试一下？",
                //  "您知道吗？", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                //{
                IsReturnTypeDir = true;
                //    return;
                //}

                r.LineType = DirectionLineType.Polyline;
                r.PointTurn1.CenterPosition = new Point(begin.X - 10, begin.Y - 10);
                r.PointTurn2.CenterPosition = new Point(end.X + 10, end.Y + 10);
                r.SetDirectionPosition(
                   begin, end
                    , new Point(r.PointTurn1.CenterPosition.X + 30, r.PointTurn1.CenterPosition.Y + 30)
                    , new Point(r.PointTurn2.CenterPosition.X + 30, r.PointTurn2.CenterPosition.Y + 30));
            }
            else
            {
                r.SetDirectionPosition(begin, end);
            }
            r.SetValue(Canvas.ZIndexProperty, NextMaxIndex);
            r.DirectionName = GetNewElementName(WorkFlowElementType.Direction);
            r.DirType = type;
            AddUI(r);
            SaveChange(HistoryType.New);
            IsNeedSave = true;
            this.cnsDesignerContainer.Cursor = Cursors.Arrow;
        }
        public void CreateLabel(Point p)
        {
            EventHandler<DoNewLabelCompletedEventArgs> DoNewLabelCompleted = null;
            DoNewLabelCompleted = (object sender, DoNewLabelCompletedEventArgs e) =>
            {

                _Service.DoNewLabelCompleted -= DoNewLabelCompleted;
                if (e.Error != null)
                {
                    this.cnsDesignerContainer.Cursor = Cursors.Arrow;
                    return;
                }

                NodeLabel r = new NodeLabel(this);
                r.LabelName = GetNewElementName(WorkFlowElementType.Label);
                r.LableID = e.Result;
                r.SetValue(Canvas.LeftProperty, this.pos.X);
                r.SetValue(Canvas.TopProperty, this.pos.Y);

                AddUI(r);

                this.cnsDesignerContainer.Cursor = Cursors.Arrow;

                SaveChange(HistoryType.New);
                isNeedSave = true;
            };
            this.cnsDesignerContainer.Cursor = Cursors.Wait;
            this.pos = p;
            _Service.DoNewLabelAsync(FlowID, (int)p.X, (int)p.Y, GetNewElementName(WorkFlowElementType.Label), null);
            _Service.DoNewLabelCompleted += DoNewLabelCompleted;
        }


        public void AddUI(Direction r)
        {
            if (!cnsDesignerContainer.Children.Contains(r))
            {
                cnsDesignerContainer.Children.Add(r);
                r.Container = this;
                r.DirectionChanged += (OnDirectionChanged);
            }

            if (!DirectionCollections.Contains(r))
            {
                DirectionCollections.Add(r);
            }
        }
        public void RemoveUI(Direction r)
        {
            if (cnsDesignerContainer.Children.Contains(r))
            {
                cnsDesignerContainer.Children.Remove(r);
            }
            if (DirectionCollections.Contains(r))
            {
                DirectionCollections.Remove(r);
            }
        }
        public void SelectDirection(Direction r, bool isSelectd)
        {
            SolidColorBrush brush = new SolidColorBrush();

            if (isSelectd)
            {
                brush.Color = Color.FromArgb(255, 255, 181, 0);
            }
            else
            {
                brush.Color = Color.FromArgb(255, 0, 128, 0);
            }

            r.dBegin.Fill = brush;
            r.endArrow.Stroke = brush;
            r.line.Stroke = brush;
            if (r.LineType == DirectionLineType.Polyline)
            {
                r.PointTurn1.Fill = brush;
                r.PointTurn2.Fill = brush;
            }
        }


        public void RemoveUI(NodeLabel l)
        {

            if (cnsDesignerContainer.Children.Contains(l))
                cnsDesignerContainer.Children.Remove(l);
            if (LableCollections.Contains(l))
            {
                LableCollections.Remove(l);
                _Service.DoAsync("DelLable", l.LableID, true);
            }
        }

        public void AddUI(NodeLabel l)
        {
            l.IsSelectd = false;

            if (!cnsDesignerContainer.Children.Contains(l))
            {
                cnsDesignerContainer.Children.Add(l);
            }

            if (!LableCollections.Contains(l))
            {
                LableCollections.Add(l);
            }
        }

        public void AddUI(FlowNode a)
        {
            if (!cnsDesignerContainer.Children.Contains(a))
            {
                cnsDesignerContainer.Children.Add(a);
                a.Container = this;
                a.FlowNodeChanged += OnFlowNodeChanged;
                a.FK_Flow = FlowID;
            }
            if (!FlowNodeCollections.Contains(a))
            {
                FlowNodeCollections.Add(a);

                if (!dicFlowNode.ContainsKey(a.NodeID))
                {
                    dicFlowNode.Add(a.NodeID, a);
                }
            }
        }
        public void RemoveUI(FlowNode a)
        {
            if (cnsDesignerContainer.Children.Contains(a))
                cnsDesignerContainer.Children.Remove(a);
            if (FlowNodeCollections.Contains(a))
            {
                FlowNodeCollections.Remove(a);
                _Service.DoAsync("DelNode", a.NodeID, true);
            }
        }

        public void SelectFlowNode(FlowNode a, bool isSelectd)
        {
        }

        void OnDirectionChanged(Direction a)
        {
            SaveChange(HistoryType.New);
        }

        public void OnFlowNodeChanged(FlowNode a)
        {
            SaveChange(HistoryType.New);
        }

        public void ShowContainerCover(bool isShow)
        {
            if (isShow)
            {
                canContainerCover.Visibility = Visibility.Visible;
                sbContainerCoverDisplay.Begin();
            }
            else
            {
                sbContainerCoverClose.Completed += (object sender, EventArgs e) =>
                {
                    canContainerCover.Visibility = Visibility.Collapsed;
                };
                sbContainerCoverClose.Begin();
            }
        }
        public void ShowMessage(string message)
        {
            ShowContainerCover(true);
            MessageTitle.Text = message;
            MessageBody.Visibility = Visibility.Visible;
        }

        ToolbarButton HisCtl = null;
        public bool Save(ToolbarButton ctl)
        {
            if (!IsNeedSave)
                return true;

            Glo.Loading(true);
            if (ctl != null)
                this.HisCtl = ctl;
            else
                this.HisCtl = new ToolbarButton();

            this.HisCtl.IsEnabled = false;
            bool toBeContinued = true;

            // 如果没有要保存的内容，则返回促成成功。
            if (!isNeedSave)
            {
                this.HisCtl.IsEnabled = true;
                toBeContinued = false;
            }

            if (toBeContinued)
            {
                string nodes = "";
                string dirs = "";
                string labes = "";
                CheckResult cr = this.CheckSave();
                if (cr.IsPass)
                {
                    IElement ele;
                    double maxWidth = ContainerWidth;
                    double maxHeight = ContainerHeight;
                    foreach (UIElement c in cnsDesignerContainer.Children)
                    {
                        ele = c as IElement;
                        if (ele != null)
                        {
                            if (ele.IsDeleted)
                                continue;
                            if (ele.ElementType == WorkFlowElementType.FlowNode)
                            {
                                FlowNode f = ele as FlowNode;
                                // 如果节点没有进线，并且不是惟一的开始节点，则设节点为结束节点。
                                //if (f.BeginDirectionCollections.Count == 0
                                //    && cnsDesignerContainer.Children.Count != 2)
                                nodes += "~@Name=" + f.NodeName + "@X=" + (int)f.CenterPoint.X + "@Y=" + (int)f.CenterPoint.Y + "@NodeID=" + int.Parse(f.NodeID)
                                    + "@HisRunModel=" + (int)f.NodeType + "@HisPosType=" + (int)f.HisPosType;

                                maxWidth = Math.Max(maxWidth, f.CenterPoint.X + f.DesiredSize.Width / 2);
                                maxHeight = Math.Max(maxHeight, f.CenterPoint.Y + f.DesiredSize.Height / 2);
                            }
                            else if (ele.ElementType == WorkFlowElementType.Direction)
                            {
                                Direction d = ele as Direction;
                                if (d.EndFlowNode != null
                                    && d.BeginFlowNode != null
                                    && d.BeginFlowNode.NodeID != "0")  //不为虚开始
                                {
                                    // 指向同一点的连线不保存
                                    if (d.EndFlowNode.NodeID == d.BeginFlowNode.NodeID)
                                        continue;
                                    //中间节与虚结束节点有连线时忽略不保存
                                    if (d.EndFlowNode.NodeID == "1" && !CheckVirtualDir(d))
                                        continue;

                                    dirs += "~@Node=" + int.Parse(d.BeginFlowNode.NodeID) + "@ToNode=" + int.Parse(d.EndFlowNode.NodeID)
                                        + "@DirType=" + (int)d.DirType + "@IsCanBack=" + (d.IsCanBack ? 1 : 0)
                                        + "@Dots=" + (d.LineType == DirectionLineType.Polyline
                                        ? ("#" + d.PointTurn1.CenterPosition.X + "," + d.PointTurn1.CenterPosition.Y
                                        + "#" + d.PointTurn2.CenterPosition.X + "," + d.PointTurn2.CenterPosition.Y) : string.Empty)
                                        + "@MyPK=" + (d.BeginFlowNode.NodeID + "_" + d.EndFlowNode.NodeID + "_" + (int)d.DirType);

                                    if (d.LineType == DirectionLineType.Polyline)
                                    {
                                        maxWidth = Math.Max(maxWidth, d.PointTurn1.CenterPosition.X + d.PointTurn1.DesiredSize.Width / 2);
                                        maxHeight = Math.Max(maxHeight, d.PointTurn1.CenterPosition.Y + d.PointTurn1.DesiredSize.Height / 2);
                                        maxWidth = Math.Max(maxWidth, d.PointTurn2.CenterPosition.X + d.PointTurn2.DesiredSize.Width / 2);
                                        maxHeight = Math.Max(maxHeight, d.PointTurn2.CenterPosition.Y + d.PointTurn2.DesiredSize.Height / 2);
                                    }
                                }
                            }
                            else if (ele.ElementType == WorkFlowElementType.Label)
                            {
                                NodeLabel l = ele as NodeLabel;
                                labes += "~@FK_Flow=" + FlowID + "@X=" + (int)l.Position.X + "@Y=" + (int)l.Position.Y + "@MyPK=" + l.LableID + "@Label=" + l.LabelName;

                                maxWidth = Math.Max(maxWidth, l.Position.X + l.txtBorder.ActualWidth);
                                maxHeight = Math.Max(maxHeight, l.Position.Y + l.txtBorder.ActualHeight);
                            }
                        }
                    }  // 结束遍历。

                    ContainerWidth = maxWidth + 10;
                    ContainerHeight = maxHeight + 10;
                    SetGridLines(true);

                    WSDesignerSoapClient ws = Glo.GetDesignerServiceInstance();
                    ws.FlowSaveAsync(FlowID, nodes, dirs, labes);
                    ws.FlowSaveCompleted += new EventHandler<FlowSaveCompletedEventArgs>(
                        (object sender, FlowSaveCompletedEventArgs e) =>
                        {
                            Glo.Loading(false);
                            this.HisCtl.IsEnabled = true;

                            if (e.Error != null)
                            {
                                Glo.ShowException(e.Error);
                            }
                            else if (e.Result != null)
                            {
                                this.IsNeedSave = true;
                                MessageBox.Show(e.Result, "保存流程错误", MessageBoxButton.OK);
                            }
                            else
                                this.IsNeedSave = false;
                        });
                }
                else
                {
                    Glo.Loading(false);
                    HtmlPage.Window.Alert(cr.Message);
                    this.HisCtl.IsEnabled = true;
                    toBeContinued = false;
                }
            }

            return toBeContinued;
        }

        /// <summary>
        /// 保存时检查是否为有效的虚拟方向线
        /// 即其首节点是否为真实结束结点，排除中间节点情况
        /// </summary>
        /// <param name="d">当前虚拟方向线</param>
        /// <returns></returns>
        private bool CheckVirtualDir(Direction d)
        {
            FlowNode item = d.BeginFlowNode;
            bool isValid = false;

            //找真实结束节点（不含虚节点）
            if (item != null
                && item.NodeID != "0" && item.NodeID != "1" && item.NodeID != (int.Parse(FlowID + "01")).ToString())
            {
                isValid = true;
                //首节点切出线若均为回退线，仍为结束节点，否则为中间节点
                foreach (Direction dir in item.BeginDirectionCollections)
                {
                    if (dir == d)
                    {
                        continue;
                    }

                    if (dir.DirType != DirType.Backward)
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            return isValid;
        }

        FlowNode getFlowNode(string FlowNodeFlowID)
        {
            for (int i = 0; i < FlowNodeCollections.Count; i++)
            {
                if (FlowNodeCollections[i].FK_Flow == FlowNodeFlowID)
                {
                    return FlowNodeCollections[i];
                }
            }
            return null;
        }


        public void clearContainer()
        {
            cnsDesignerContainer.Children.Clear();
            _gridLinesContainer = null;
            SetGridLines(true);
            flowNodeCollections = null;
            directionCollections = null;
        }

        #region 新建节点

        public void CreateFlowNode(Point p)
        {
            CreateFlowNode(FlowNodeType.Ordinary, p);
        }

        public void CreateFlowNode(FlowNodeType type, Point point, string icon = "审核")
        {
            this.cnsDesignerContainer.Cursor = Cursors.Wait;

            FlowNode flowNode = new FlowNode(this);
            flowNode.NodeType = type;
            flowNode.HisPosType = NodePosType.Mid;
            flowNode.Icon = icon;
            flowNode.SetValue(Canvas.ZIndexProperty, NextMaxIndex);
            flowNode.NodeName = GetNewElementName(WorkFlowElementType.FlowNode);
            flowNode.CenterPoint = point;

            System.EventHandler<DoNewNodeCompletedEventArgs> DoNewNode1Completed = null;
            DoNewNode1Completed = (object sender, DoNewNodeCompletedEventArgs e) =>
            {
                _Service.DoNewNodeCompleted -= DoNewNode1Completed;
                if (null != e.Error)
                {
                    Glo.ShowException(e.Error, "添加节点出错");
                }
                else
                {
                    flowNode.NodeID = e.Result.ToString();
                    AddUI(flowNode);
                    SaveChange(HistoryType.New);
                    IsNeedSave = true;
                }
                this.cnsDesignerContainer.Cursor = Cursors.Arrow;
            };

            string[] paras = new string[] { 
                FlowID ,
                flowNode.NodeName,
                flowNode.Icon,
                point.X.ToString(),
                point.Y.ToString(),
               ((int)flowNode.NodeType).ToString()
            };

            _Service.DoNewNodeAsync(true, paras);
            _Service.DoNewNodeCompleted += DoNewNode1Completed;

        }

        public string GetNewElementName(WorkFlowElementType type)
        {
            string SolidName = string.Empty;
            if (type == WorkFlowElementType.FlowNode)
            {
                SolidName = "新建节点 ";
            }
            else if (type == WorkFlowElementType.Direction)
            {
                SolidName = "New Line ";
            }
            else if (type == WorkFlowElementType.Label)
            {
                SolidName = "New Label ";
            }

            int max = 0;
            foreach (UIElement c in cnsDesignerContainer.Children)
            {
                if (!(c is IElement)) continue;

                var iElement = c as IElement;
                if (iElement.IsDeleted)
                    continue;

                string elementName = string.Empty;
                string no = "0";
                if (type == WorkFlowElementType.FlowNode)
                {
                    var n = iElement as FlowNode;
                    if (n != null && !string.IsNullOrEmpty(elementName = n.NodeName) && elementName.Contains(SolidName))
                    {
                        no = elementName.Substring(SolidName.Length, elementName.Length - SolidName.Length);
                    }
                }
                else if (type == WorkFlowElementType.Direction)
                {
                    var n = iElement as Direction;
                    if (n != null && !string.IsNullOrEmpty(elementName = n.DirectionName) && elementName.Contains(SolidName))
                    {
                        no = elementName.Substring(SolidName.Length, elementName.Length - SolidName.Length);
                    }
                }
                else if (type == WorkFlowElementType.Label)
                {
                    var n = iElement as NodeLabel;
                    if (n != null && !string.IsNullOrEmpty(elementName = n.LabelName) && elementName.Contains(SolidName))
                    {
                        no = elementName.Substring(SolidName.Length, elementName.Length - SolidName.Length);
                    }
                }

                int index = 0;
                int.TryParse(no, out index);
                if (index > max)
                    max = index;
            }
            return SolidName + (max + 1).ToString();
        }

        #endregion

        void pushNextQueueToPreQueue()
        {
            if (WorkFlowXmlPreStack.Count > 0)
                WorkFlowXmlNextStack.Push(WorkFlowXmlPreStack.Pop());
            int cout = WorkFlowXmlNextStack.Count;

            for (int i = 0; i < cout; i++)
            {
                WorkFlowXmlPreStack.Push(WorkFlowXmlNextStack.Pop());
            }
        }

        public void SaveChange(HistoryType action)
        {
            if (action == HistoryType.New)
            {
                WorkFlowXmlPreStack.Push(workflowXmlCurrent);
                WorkFlowXmlNextStack.Clear();
            }
            if (action == HistoryType.Next)
            {
                if (WorkFlowXmlNextStack.Count > 0)
                {
                    WorkFlowXmlPreStack.Push(workflowXmlCurrent);
                    workflowXmlCurrent = WorkFlowXmlNextStack.Pop();
                    clearContainer();
                    ClearSelected(null);
                }

                LoadFromXmlString(workflowXmlCurrent);
            }

            if (action == HistoryType.Previous)
            {
                if (WorkFlowXmlPreStack.Count > 0)
                {
                    WorkFlowXmlNextStack.Push(workflowXmlCurrent);
                    workflowXmlCurrent = WorkFlowXmlPreStack.Pop();
                    clearContainer();

                    LoadFromXmlString(workflowXmlCurrent);
                    ClearSelected(null);
                }
            }
        }

        public void ActionPrevious()
        {
            SaveChange(HistoryType.Previous);

        }

        public void ActionNext()
        {
            SaveChange(HistoryType.Next);

        }

        public void AddSelectedControl(IElement uc)
        {
            if (!SelectedWFElements.Contains(uc))
                SelectedWFElements.Add(uc);
        }

        public void RemoveSelectedControl(IElement uc)
        {
            if (SelectedWFElements.Contains(uc))
                SelectedWFElements.Remove(uc);
        }

        public void SelectedWorkFlowElement(IElement uc, bool isSelected)
        {
            if (isSelected)
                AddSelectedControl(uc);
            else
                RemoveSelectedControl(uc);

            if (!CtrlKeyIsPress)
                ClearSelected(uc);
        }

        public void ClearSelectFlowElement()
        {
            foreach (var item in this.cnsDesignerContainer.Children)
            {
                if (item is IElement)
                {
                    ((IElement)item).IsSelectd = false;
                }
            }

            SelectedWFElements.Clear();
        }
        public void ClearSelected(IElement uc)
        {
            ClearSelectFlowElement();
            if (uc != null)
            {
                ((IElement)uc).IsSelectd = true;
                AddSelectedControl(uc);
            }
            MouseIsInContainer = true;
        }

        public void DeleteSeleted()
        {
            if (SelectedWFElements == null
                || 0 == SelectedWFElements.Count)
                return;

            for (int i = 0; i < SelectedWFElements.Count; i++)
            {
                IElement iele = SelectedWFElements[i];
                if (iele is FlowNode)
                {
                    if (((FlowNode)iele).HisPosType == NodePosType.Start)
                    {
                        continue;
                    }
                    else
                    {
                        iele.Delete();
                    }
                }
                else if (iele is IElement)
                {
                    iele.Delete();
                }
            }
            ClearSelected(null);
        }

        public void AlignTop()
        {
            if (SelectedWFElements == null || SelectedWFElements.Count == 0)
                return;
            FlowNode a = null;
            double minY = 100000.0;
            for (int i = 0; i < SelectedWFElements.Count; i++)
            {
                if (SelectedWFElements[i] is FlowNode)
                {
                    a = SelectedWFElements[i] as FlowNode;

                    if (a.CenterPoint.Y < minY)
                        minY = a.CenterPoint.Y;
                }

            }
            for (int i = 0; i < SelectedWFElements.Count; i++)
            {
                if (SelectedWFElements[i] is FlowNode)
                {
                    a = SelectedWFElements[i] as FlowNode;
                    a.CenterPoint = new Point(a.CenterPoint.X, minY);
                }
            }
        }

        public void AlignBottom()
        {
            if (SelectedWFElements == null || SelectedWFElements.Count == 0)
                return;
            FlowNode a = null;
            double maxY = 0;
            for (int i = 0; i < SelectedWFElements.Count; i++)
            {
                if (SelectedWFElements[i] is FlowNode)
                {
                    a = SelectedWFElements[i] as FlowNode;

                    if (a.CenterPoint.Y > maxY)
                        maxY = a.CenterPoint.Y;
                }

            }
            for (int i = 0; i < SelectedWFElements.Count; i++)
            {
                if (SelectedWFElements[i] is FlowNode)
                {
                    a = SelectedWFElements[i] as FlowNode;
                    a.CenterPoint = new Point(a.CenterPoint.X, maxY);
                }
            }
        }

        public void AlignLeft()
        {

            if (SelectedWFElements == null || SelectedWFElements.Count == 0)
                return;
            FlowNode a = null;
            double minX = 100000.0;
            for (int i = 0; i < SelectedWFElements.Count; i++)
            {
                if (SelectedWFElements[i] is FlowNode)
                {
                    a = SelectedWFElements[i] as FlowNode;

                    if (a.CenterPoint.X < minX)
                        minX = a.CenterPoint.X;
                }
            }
            for (int i = 0; i < SelectedWFElements.Count; i++)
            {
                if (SelectedWFElements[i] is FlowNode)
                {
                    a = SelectedWFElements[i] as FlowNode;
                    a.CenterPoint = new Point(minX, a.CenterPoint.Y);
                }
            }

        }

        public void AlignRight()
        {
            if (SelectedWFElements == null || SelectedWFElements.Count == 0)
                return;
            FlowNode a = null;
            double maxX = 0;
            for (int i = 0; i < SelectedWFElements.Count; i++)
            {
                if (SelectedWFElements[i] is FlowNode)
                {
                    a = SelectedWFElements[i] as FlowNode;

                    if (a.CenterPoint.X > maxX)
                        maxX = a.CenterPoint.X;
                }

            }
            for (int i = 0; i < SelectedWFElements.Count; i++)
            {
                if (SelectedWFElements[i] is FlowNode)
                {
                    a = SelectedWFElements[i] as FlowNode;
                    a.CenterPoint = new Point(maxX, a.CenterPoint.Y);
                }
            }
        }

        public void MoveUp()
        {
            MoveControlCollectionByDisplacement(0, -MoveStepLenght, null);
            SaveChange(HistoryType.New);
        }

        public void MoveLeft()
        {
            MoveControlCollectionByDisplacement(-MoveStepLenght, 0, null);
            SaveChange(HistoryType.New);

        }

        public void MoveDown()
        {
            MoveControlCollectionByDisplacement(0, MoveStepLenght, null);
            SaveChange(HistoryType.New);

        }

        public void MoveRight()
        {
            MoveControlCollectionByDisplacement(MoveStepLenght, 0, null);
            SaveChange(HistoryType.New);

        }

        public void MoveControlCollectionByDisplacement(double x, double y, IElement element)
        {
            if (SelectedWFElements == null || SelectedWFElements.Count == 0
                || element == null || !element.IsSelectd
                // 如果光标所在的节点没有被选中，则不移动所有被选中的节点，光移动光标所有的节点即可。

                )
            {
                return;
            }

            for (int i = 0; i < SelectedWFElements.Count; i++)
            {
                if (element == SelectedWFElements[i])
                    continue;

                SelectedWFElements[i].ResetPosition(x, y);
            }

            IsNeedSave = true;
        }

        public void PastMemoryToContainer()
        {
            if (CopyElementCollectionInMemory != null
                      && CopyElementCollectionInMemory.Count > 0)
            {
                FlowNode a = null;
                Direction r = null;
                NodeLabel l = null;

                foreach (System.Windows.Controls.Control c in CopyElementCollectionInMemory)
                {
                    if (c is Direction)
                    {
                        r = c as Direction;
                        AddUI(r);
                        if (r.LineType == DirectionLineType.Line)
                        {
                            r.SetDirectionPosition(new Point(r.PointBegin.X + 20, r.PointBegin.Y + 20),
                                new Point(r.PointEnd.X + 20, r.PointEnd.Y + 20));
                        }
                        else
                        {
                            r.SetDirectionPosition(new Point(r.PointBegin.X + 20, r.PointBegin.Y + 20),
                                new Point(r.PointEnd.X + 20, r.PointEnd.Y + 20)
                               , new Point(r.PointTurn1.CenterPosition.X + 20, r.PointTurn1.CenterPosition.Y + 20)
                               , new Point(r.PointTurn2.CenterPosition.X + 20, r.PointTurn2.CenterPosition.Y + 20)
                               );
                        }
                    }
                }


                foreach (System.Windows.Controls.Control c in CopyElementCollectionInMemory)
                {
                    if (c is FlowNode)
                    {
                        a = c as FlowNode;
                        AddUI(a);
                        a.CenterPoint = new Point(a.CenterPoint.X + 20, a.CenterPoint.Y + 20);
                        a.Move(a, null);

                    }

                }
                foreach (System.Windows.Controls.Control c in CopyElementCollectionInMemory)
                {
                    if (c is NodeLabel)
                    {
                        l = c as NodeLabel;
                        AddUI(l);
                        l.Position = new Point(l.Position.X + 20, l.Position.Y + 20);
                    }
                }

                for (int i = 0; i < SelectedWFElements.Count; i++)
                {
                    ((IElement)SelectedWFElements[i]).IsSelectd = false;
                }
                SelectedWFElements.Clear();

                for (int i = 0; i < CopyElementCollectionInMemory.Count; i++)
                {

                    ((IElement)CopyElementCollectionInMemory[i]).IsSelectd = true;
                    AddSelectedControl(CopyElementCollectionInMemory[i]);
                }
                CopySelectedToMemory(null);

                SaveChange(HistoryType.New);


            }
        }

        public void CopySelectedToMemory(IElement currentControl)
        {
            copyElementCollectionInMemory = null;

            if (currentControl != null)
            {
                if (currentControl is FlowNode)
                {

                    CopyElementCollectionInMemory.Add(((FlowNode)currentControl).Clone());
                }
                if (currentControl is Direction)
                {

                    CopyElementCollectionInMemory.Add(((Direction)currentControl).Clone());
                }
                if (currentControl is NodeLabel)
                {

                    CopyElementCollectionInMemory.Add(((NodeLabel)currentControl).Clone());
                }
            }
            else
            {
                if (SelectedWFElements != null
                    && SelectedWFElements.Count > 0)
                {
                    FlowNode a = null;
                    Direction r = null;
                    NodeLabel l = null;
                    foreach (System.Windows.Controls.Control c in SelectedWFElements)
                    {
                        if (c is FlowNode)
                        {
                            a = c as FlowNode;

                            CopyElementCollectionInMemory.Add(a.Clone());
                        }
                    }
                    foreach (System.Windows.Controls.Control c in SelectedWFElements)
                    {
                        if (c is NodeLabel)
                        {
                            l = c as NodeLabel;

                            CopyElementCollectionInMemory.Add(l.Clone());
                        }
                    }
                    foreach (System.Windows.Controls.Control c in SelectedWFElements)
                    {
                        if (c is Direction)
                        {
                            r = c as Direction;
                            r = r.Clone();
                            CopyElementCollectionInMemory.Add(r);

                            if (r.OriginDirection.BeginFlowNode != null)
                            {
                                FlowNode temA = null;
                                foreach (System.Windows.Controls.Control c1 in CopyElementCollectionInMemory)
                                {
                                    if (c1 is FlowNode)
                                    {
                                        temA = c1 as FlowNode;
                                        if (r.OriginDirection.BeginFlowNode == temA.OriginFlowNode)
                                        {
                                            r.BeginFlowNode = temA;
                                        }
                                    }
                                }
                            }
                            if (r.OriginDirection.EndFlowNode != null)
                            {
                                FlowNode temA = null;
                                foreach (System.Windows.Controls.Control c1 in CopyElementCollectionInMemory)
                                {
                                    if (c1 is FlowNode)
                                    {
                                        temA = c1 as FlowNode;
                                        if (r.OriginDirection.EndFlowNode == temA.OriginFlowNode)
                                        {
                                            r.EndFlowNode = temA;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    foreach (System.Windows.Controls.Control c in SelectedWFElements)
                    {
                        if (c is FlowNode)
                        {
                            a = c as FlowNode;

                            a.OriginFlowNode = null;
                        }
                        if (c is Direction)
                        {
                            r = c as Direction;

                            r.OriginDirection = null;
                        }
                    }

                }
            }
        }
        public void UpdateSelectedToMemory(IElement currentControl)
        {
            NodeLabel ll = (NodeLabel)currentControl;
            l = ll;
            ll.txtLabel.Visibility = Visibility.Collapsed;
            ll.txtEdit.Visibility = Visibility.Visible;
            ll.txtEdit.Focus();
            ll.txtEdit.LostFocus += (object sender, RoutedEventArgs e) =>
            {
                _Service.DoNewLabelAsync(FlowID, (int)l.Position.X, (int)l.Position.Y, l.LabelName, l.LableID);
            };
        }

        #region NodeNameRelated
        public void UpdateSelectedNode(IElement currentControl)
        {
            FlowNode f = (FlowNode)currentControl;
            TextBlock tb = f.CurrentNodeUI.Name;
            tb.Visibility = System.Windows.Visibility.Collapsed;
            f.CurrentNodeUI.Name = tb;
        }
        public void ShowSetting(FlowNode fn)
        {
            this.nodeSelected = fn;
            fn.IsSelectd = false;
            TextBlock tb = fn.CurrentNodeUI.Name;
            tb.Visibility = System.Windows.Visibility.Collapsed;
            fn.CurrentNodeUI.Name = tb;
            IsSomeChildEditing = true;
        }
        /// <summary>
        /// 添加开始结束虚节点
        /// </summary>
        /// <param name="tbNode"></param>
        public void DrawVirtualNode(string paras)
        {
            if (string.IsNullOrEmpty(paras))
            {
                paras = "@StartNodeX=200@StartNodeY=50@EndNodeX=200@EndNodeY=350"; ;
            }

            AtPara ap = new AtPara(paras);
            string startNodeX = ap.GetValStrByKey("StartNodeX");
            string startNodeY = ap.GetValStrByKey("StartNodeY");
            string endNodeX = ap.GetValStrByKey("EndNodeX");
            string endNodeY = ap.GetValStrByKey("EndNodeY");

            FlowNode node0 = new FlowNode((IContainer)this);
            node0.NodeType = FlowNodeType.VirtualStart;
            node0.CurrentNodeUI.Name.Foreground = new SolidColorBrush(Colors.Black);
            node0.SetValue(Canvas.ZIndexProperty, NextMaxIndex);
            node0.FK_Flow = FlowID;
            node0.NodeID = "0";
            node0.NodeName = "开始";
            InsertFlowNode(node0, double.Parse(startNodeX), double.Parse(startNodeY));

            FlowNode node1 = new FlowNode((IContainer)this);
            node1.NodeType = FlowNodeType.VirtualEnd;
            node1.CurrentNodeUI.Name.Foreground = new SolidColorBrush(Colors.Black);
            node1.SetValue(Canvas.ZIndexProperty, NextMaxIndex);
            node1.FK_Flow = FlowID;
            node1.NodeID = "1";
            node1.NodeName = "结束";
            InsertFlowNode(node1, double.Parse(endNodeX), double.Parse(endNodeY));


            Direction da = new Direction((IContainer)this);
            da.FlowID = FlowID;
            da.BeginFlowNode = node0;
            da.EndFlowNode = GetBeginFlowNode();
            da.DirType = DirType.Virtual;
            AddUI(da);

            IList<FlowNode> endNodes = GetEndFlowNodes();
            foreach (FlowNode item in endNodes)
            {
                Direction db = new Direction((IContainer)this);
                db.FlowID = FlowID;
                db.BeginFlowNode = item;
                db.EndFlowNode = node1;
                db.DirType = DirType.Virtual;
                AddUI(db);
            }
        }

        #endregion



        #region 事件

        private void Container_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseIsInContainer = true;

        }

        private void Container_MouseLeave(object sender, MouseEventArgs e)
        {
            MouseIsInContainer = false;

        }

        private void cnsDesignerContainer_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MouseIsInContainer)
            {
                if (menuFlowNode.Visibility == Visibility.Collapsed
                    && menuDirection.Visibility == Visibility.Collapsed
                    && menuLabel.Visibility == Visibility.Collapsed)
                {

                    menuContainer.ShowMenu();

                    double top = (double)(e.GetPosition(svContainer).Y);
                    double left = (double)(e.GetPosition(svContainer).X);
                    menuContainer.CenterPoint = new Point(left, top);
                }
            }
            e.Handled = true;
        }

        private void OnContextMenu(object sender, System.Windows.Browser.HtmlEventArgs e)
        {

            if (MouseIsInContainer)
            {
                e.PreventDefault();

                if (menuFlowNode.Visibility == Visibility.Collapsed
                    && menuDirection.Visibility == Visibility.Collapsed
                    && menuLabel.Visibility == Visibility.Collapsed)
                {
                    menuContainer.ShowMenu();

                    double top = (double)(e.ClientY - Top);
                    double left = (double)(e.ClientX - Left);
                    menuContainer.CenterPoint = new Point(left, top);
                }
            }
        }

        public void ShowContentMenu(FlowNode a, object sender, MouseButtonEventArgs e)
        {
            //虚节点不显示菜单
            if (a.NodeID != "0" && a.NodeID != "1")
            {
                menuFlowNode.RelatedFlowNode = a;
                menuContainer.Visibility = Visibility.Collapsed;
                menuDirection.Visibility = Visibility.Collapsed;
                menuLabel.Visibility = Visibility.Collapsed;
                double top = (double)(e.GetPosition(svContainer).Y);
                double left = (double)(e.GetPosition(svContainer).X);
                menuFlowNode.CenterPoint = new Point(left, top);

                menuFlowNode.ShowMenu();
            }
            e.Handled = true;
        }

        public void ShowContentMenu(NodeLabel l, object sender, MouseButtonEventArgs e)
        {
            menuLabel.RelatedLabel = l;
            menuContainer.Visibility = Visibility.Collapsed;
            menuDirection.Visibility = Visibility.Collapsed;
            menuFlowNode.Visibility = Visibility.Collapsed;
            double top = (double)(e.GetPosition(svContainer).Y);
            double left = (double)(e.GetPosition(svContainer).X);
            menuLabel.CenterPoint = new Point(left, top);
            menuLabel.ShowMenu();
            e.Handled = true;
        }

        public void ShowContentMenu(Direction r, object sender, MouseButtonEventArgs e)
        {
            //虚连线不显示菜单
            if (r.DirType != DirType.Virtual)
            {
                menuDirection.RelatedDirection = r;
                menuContainer.Visibility = Visibility.Collapsed;
                menuLabel.Visibility = Visibility.Collapsed;
                menuFlowNode.Visibility = Visibility.Collapsed;
                double top = (double)(e.GetPosition(svContainer).Y);
                double left = (double)(e.GetPosition(svContainer).X);
                menuDirection.CenterPoint = new Point(left, top);
                menuDirection.ShowMenu();
            }

            e.Handled = true;
        }

        void checkUIDisplay(UserControl element)
        {
            double wid = Canvas.GetLeft(element) + element.ActualWidth;
            double hig = Canvas.GetTop(element) + element.ActualHeight;


        }
        private void btnCloseMessageButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBody.Visibility = Visibility.Collapsed;
            ShowContainerCover(false);
        }


        private void Container_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Container_MouseMove(object sender, MouseEventArgs e)
        {
            if (trackingMouseMove)
            {
                FrameworkElement element = sender as FrameworkElement;
                Point beginPoint = mousePosition;
                Point endPoint = e.GetPosition(element);

                if (temproaryEllipse == null)
                {
                    temproaryEllipse = new Rectangle();

                    SolidColorBrush brush = new SolidColorBrush();
                    brush.Color = Color.FromArgb(255, 234, 213, 2);
                    temproaryEllipse.Fill = brush;
                    temproaryEllipse.Opacity = 0.2;

                    brush = new SolidColorBrush();
                    brush.Color = Color.FromArgb(255, 0, 0, 0);
                    temproaryEllipse.Stroke = brush;
                    temproaryEllipse.StrokeMiterLimit = 2.0;

                    cnsDesignerContainer.Children.Add(temproaryEllipse);

                }

                if (endPoint.X >= beginPoint.X)
                {
                    if (endPoint.Y >= beginPoint.Y)
                    {
                        temproaryEllipse.SetValue(Canvas.TopProperty, beginPoint.Y);
                        temproaryEllipse.SetValue(Canvas.LeftProperty, beginPoint.X);
                    }
                    else
                    {
                        temproaryEllipse.SetValue(Canvas.TopProperty, endPoint.Y);
                        temproaryEllipse.SetValue(Canvas.LeftProperty, beginPoint.X);
                    }

                }
                else
                {
                    if (endPoint.Y >= beginPoint.Y)
                    {
                        temproaryEllipse.SetValue(Canvas.TopProperty, beginPoint.Y);
                        temproaryEllipse.SetValue(Canvas.LeftProperty, endPoint.X);
                    }
                    else
                    {
                        temproaryEllipse.SetValue(Canvas.TopProperty, endPoint.Y);
                        temproaryEllipse.SetValue(Canvas.LeftProperty, endPoint.X);
                    }

                }


                temproaryEllipse.Width = Math.Abs(endPoint.X - beginPoint.X);
                temproaryEllipse.Height = Math.Abs(endPoint.Y - beginPoint.Y);


            }
            else
            {
                if (CurrentTemporaryDirection != null)
                {
                    CurrentTemporaryDirection.CaptureMouse();
                    Point currentPoint = e.GetPosition(CurrentTemporaryDirection);
                    CurrentTemporaryDirection.PointEnd = currentPoint;

                    if (CurrentTemporaryDirection.BeginFlowNode != null)
                    {
                        CurrentTemporaryDirection.PointBegin = CurrentTemporaryDirection.GetResetPoint(currentPoint, CurrentTemporaryDirection.BeginFlowNode.CenterPoint, CurrentTemporaryDirection.BeginFlowNode, DirectionMove.Begin);
                    }
                }
            }

        }

        private void Container_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            trackingMouseMove = false;
            if (CurrentTemporaryDirection != null)
            {
                CurrentTemporaryDirection.SimulateDirectionPointMouseLeftButtonUpEvent(DirectionMove.End, CurrentTemporaryDirection, e);
                if (CurrentTemporaryDirection.EndFlowNode == null)
                {
                    CurrentTemporaryDirection.BeginFlowNode.CanShowMenu = false;

                    CurrentTemporaryDirection.CanShowMenu = false;
                    // this.RemoveDirection(CurrentTemporaryDirection);
                    CurrentTemporaryDirection.Delete();

                }
                else
                {
                    CurrentTemporaryDirection.CanShowMenu = false;
                    CurrentTemporaryDirection.BeginFlowNode.CanShowMenu = false;
                    CurrentTemporaryDirection.EndFlowNode.CanShowMenu = true;
                    CurrentTemporaryDirection.DirType = this.IsReturnTypeDir ? DirType.Backward : DirType.Forward;
                    //连接到虚结束节点的线类型为2
                    if (CurrentTemporaryDirection.EndFlowNode.NodeID == "1")
                    {
                        CurrentTemporaryDirection.DirType = DirType.Virtual;
                        CurrentTemporaryDirection.LineType = DirectionLineType.Line;
                    }

                    //CurrentTemporaryDirection.IsReturnType = this.IsReturnTypeDir;
                    if (this.IsReturnTypeDir)
                    {
                        CurrentTemporaryDirection.LineType = DirectionLineType.Polyline;
                    }
                    CurrentTemporaryDirection.IsTemporaryDirection = false;
                    CurrentTemporaryDirection.IsSelectd = false;
                    RemoveSelectedControl(CurrentTemporaryDirection);
                    SaveChange(HistoryType.New);
                }
                CurrentTemporaryDirection.ReleaseMouseCapture();
                CurrentTemporaryDirection = null;

            }

            FrameworkElement element = sender as FrameworkElement;
            mousePosition = e.GetPosition(element);
            if (temproaryEllipse != null)
            {
                double width = temproaryEllipse.Width;
                double height = temproaryEllipse.Height;

                if (width > 10 && height > 10)
                {
                    Point p = new Point();
                    p.X = (double)temproaryEllipse.GetValue(Canvas.LeftProperty);
                    p.Y = (double)temproaryEllipse.GetValue(Canvas.TopProperty);

                    FlowNode a = null;
                    Direction r = null;
                    NodeLabel l = null;
                    foreach (UIElement uie in cnsDesignerContainer.Children)
                    {
                        if (uie is FlowNode)
                        {
                            a = uie as FlowNode;
                            if (p.X < a.CenterPoint.X && a.CenterPoint.X < p.X + width
                                && p.Y < a.CenterPoint.Y && a.CenterPoint.Y < p.Y + height)
                            {
                                AddSelectedControl(a);
                                a.IsSelectd = true;
                            }
                        }
                        if (uie is NodeLabel)
                        {
                            l = uie as NodeLabel;
                            if (p.X < l.Position.X && l.Position.X < p.X + width
                                && p.Y < l.Position.Y && l.Position.Y < p.Y + height)
                            {
                                AddSelectedControl(a);
                                l.IsSelectd = true;
                            }
                        }
                        if (uie is Direction)
                        {
                            r = uie as Direction;

                            Point ruleBeginPointPosition = r.PointBegin;
                            Point ruleEndPointPosition = r.PointEnd;

                            if (p.X < ruleBeginPointPosition.X && ruleBeginPointPosition.X < p.X + width
                                && p.Y < ruleBeginPointPosition.Y && ruleBeginPointPosition.Y < p.Y + height
                                &&
                                p.X < ruleEndPointPosition.X && ruleEndPointPosition.X < p.X + width
                                && p.Y < ruleEndPointPosition.Y && ruleEndPointPosition.Y < p.Y + height
                                )
                            {
                                AddSelectedControl(r);
                                r.IsSelectd = true;
                            }
                        }
                    }
                }
                cnsDesignerContainer.Children.Remove(temproaryEllipse);
                temproaryEllipse = null;
            }
        }


        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    // 如果有节点在编辑状态,则delete按钮只能删除字符,不能删除选中的节点
                    if (IsSomeChildEditing)
                    {
                        return;
                    }
                    if (SelectedWFElements != null && SelectedWFElements.Count > 0)
                    {
                        if (System.Windows.Browser.HtmlPage.Window.Confirm("确定要删除吗？"))
                        {
                            DeleteSeleted();
                            SaveChange(HistoryType.New);
                        }
                    }
                    break;
                case Key.Up:
                    MoveUp();
                    break;
                case Key.Down:
                    MoveDown();
                    break;
                case Key.Left:
                    MoveLeft();
                    break;
                case Key.Right:
                    MoveRight();
                    break;
                default:
                    break;
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            //if (Keyboard.Modifiers == ModifierKeys.Control)
            //{
            //    switch (e.Key)
            //    {
            //        case Key.Z:

            //            SaveChange(HistoryType.Previous);
            //            break;
            //        case Key.Y:
            //            SaveChange(HistoryType.Next);
            //            break;
            //        case Key.C:

            //            CopySelectedControlToMemory(null);
            //            break;
            //        case Key.V:

            //            PastMemoryToContainer();
            //            break;
            //        case Key.A:
            //            FlowNode a = null;
            //            Direction r = null;
            //            NodeLabel l = null;
            //            foreach (UIElement uie in cnsDesignerContainer.Children)
            //            {

            //                if (uie is FlowNode)
            //                {
            //                    a = uie as FlowNode;
            //                    a.IsSelectd = true;
            //                    AddSelectedControl(a);
            //                }

            //                if (uie is Direction)
            //                {
            //                    r = uie as Direction;
            //                    r.IsSelectd = true;
            //                    AddSelectedControl(r);
            //                }
            //                if (uie is NodeLabel)
            //                {
            //                    l = uie as NodeLabel;
            //                    l.IsSelectd = true;
            //                    AddSelectedControl(l);
            //                }

            //            }
            //            break;
            //        case Key.S: 

            //            Save();
            //            break;

            //    }
            //}
        }

        #endregion


    }
}
