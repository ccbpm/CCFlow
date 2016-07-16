using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using BP.WF;
using Silverlight;
using WF.WS;

namespace BP
{
    /// <summary>
    /// 主要用来显示流程轨迹
    /// </summary>
    public partial class Viewer : IContainer
    {
        #region FlowChartType变化：一般是设计器已经加载

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
                    //this.DrawFlows();
                }
            }
        }

       
        #endregion

        #region Constructs
        public Viewer()
        {
            InitializeComponent();
            dicReturnTrackTips = new Dictionary<Direction, IList<string>>();
            dicFlowNode = new Dictionary<string, FlowNode>();

            Application.Current.Host.Content.Resized += Content_Resized;
            this.UcRoot.MouseRightButtonDown += Disable_MouseRightButtonDown;
            this.LayoutRoot.LayoutUpdated += Track_LayoutUpdated;
        }

        const double _minimalHeight = 300;
        void Track_LayoutUpdated(object sender, EventArgs e)
        {
            Size size = this.paint.DesiredSize;
            if (size.Height < _minimalHeight)
                size.Height = _minimalHeight;
            String heightInPixel = String.Format("{0}px", size.Height);
            String containerElementId = "silverlightControlHost";
            HtmlElement element = HtmlPage.Document.GetElementById(containerElementId);
            element.SetStyleAttribute("height", heightInPixel);

        }

       
        private void Disable_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        { // 屏蔽SL默认的右键菜单
            e.Handled = true;
        }


        /// <summary>
        /// Content改变时重设设计器的大小， 设计器的大小取Content及最大节点值中的最大值 
        /// 20为假设的滚动条宽度。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Content_Resized(object sender, EventArgs e)
        {

            var contentWidth = Application.Current.Host.Content.ActualWidth -100;
            paint.Width = DesignerWdith  > contentWidth    ? DesignerWdith : contentWidth;

            var contentHeight = Application.Current.Host.Content.ActualHeight  ;
            paint.Height = (DesignerHeight  > contentHeight  ? DesignerHeight  : contentHeight) + 50;
        }


        /// <summary>
        /// 轨迹图
        /// </summary>
        /// <param name="fk_flow"></param>
        /// <param name="workid"></param>
        public Viewer(string fk_flow, string workid) : this()
        {
            this.FK_Flow = fk_flow;
            this.WorkID = workid;
            try
            {
                if (!string.IsNullOrEmpty(workid))
                {
                    //如果传递过来workid 说明要显示轨图.
                    WSDesignerSoapClient ws = BP.Glo.GetDesignerServiceInstance();
                    ws.GetDTOfWorkListAsync(this.FK_Flow, workid);
                    ws.GetDTOfWorkListCompleted += new EventHandler<GetDTOfWorkListCompletedEventArgs>(
                        (object sender, GetDTOfWorkListCompletedEventArgs e)=>
                        {//// 获取轨迹数据
                            // 给track dataset 赋值.
                            trackDataSet = new DataSet();
                            trackDataSet.FromXml(e.Result);

                            //产生轨迹图.
                            this.GenerFlowChart(FK_Flow);
                        });
                              
                }
                else
                {
                    this.GenerFlowChart(FK_Flow);
                }

            }
            catch (System.Exception e)
            {
                BP.SL.LoggerHelper.Write(e);
            }
        }
        #endregion


        public string GetNewElementName(WorkFlowElementType type)
        {
            return "";
        }

        #region Properties
        public bool IsNeedSave
        {
            get;
            set;
        } 
        private int nextMaxIndex = 0;
        public int NextMaxIndex
        {
            get
            {
                nextMaxIndex++;
                return nextMaxIndex;
            }
        }
      
        private DataSet trackDataSet;
      
        private Dictionary<Direction, IList<string>> dicReturnTrackTips;
        private Dictionary<string, FlowNode> dicFlowNode;

        private WSDesignerSoapClient _service = Glo.GetDesignerServiceInstance();
        public WSDesignerSoapClient _Service
        {
            get { return _service; }
            set { _service = value; }
        }
        public string FK_Flow { get; set; }
        public string WorkID { get; set; }
        public string FlowID { get; set; }
        public int NodeID;
        /// <summary>
        /// 当前是否为添加回退连线状态
        /// </summary>
        public bool IsReturnTypeDir { get; set; }

       
        public List<FlowNode> flowNodeCollections;
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

        public List<Direction> directionCollections;
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

        public List<NodeLabel> lableCollections;
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


        private double DesignerHeight;
        private double DesignerWdith;
        public Double ContainerWidth
        {
            get { return paint.Width; }
            set { paint.Width = value; }
        }

        public Double ContainerHeight
        {
            get { return paint.Height; }
            set { paint.Height = value; }
        }
       
        #endregion

      
        private void GenerFlowChart(string flowID)
        {
            this.FlowID = flowID;
            string sqls = "";
            sqls += "SELECT NodeID,Name,Icon,X,Y,NodePosType,RunModel,HisToNDs,TodolistModel FROM WF_Node WHERE FK_Flow='" + flowID + "'";
            sqls += "@SELECT MyPK,Name,X,Y FROM WF_LabNote WHERE FK_Flow='" + flowID + "'";
            sqls += "@SELECT Node,ToNode,DirType,IsCanBack,Dots FROM WF_Direction WHERE FK_Flow='" + flowID + "'";
            sqls += "@SELECT Paras,ChartType FROM WF_Flow WHERE No='" + flowID + "'";
            try
            {
                WSDesignerSoapClient ws = BP.Glo.GetDesignerServiceInstance();
                //此处使用系统约定用户名和SID;admin 33273d4a-35c8-4ae8-bdd9-0085ea648ccb
                ws.RunSQLReturnTableSAsync(sqls, "admin", "33273d4a-35c8-4ae8-bdd9-0085ea648ccb");
                ws.RunSQLReturnTableSCompleted += new EventHandler<RunSQLReturnTableSCompletedEventArgs>(ws_RunSQLReturnTableSCompleted);
            }
            catch (System.Exception e)
            {
                setContainerStyle();
                BP.SL.LoggerHelper.Write(e);
            }
        }
        bool isAddNodeTip = true;
        void ws_RunSQLReturnTableSCompleted(object sender, RunSQLReturnTableSCompletedEventArgs e)
        {
            if (null != e.Error)
            {
                setContainerStyle();
                BP.SL.LoggerHelper.Write(e.Error);
                return;
            }

            try
            {
                var ds = new DataSet();
                ds.FromXml(e.Result);

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

                #region 画节点.
                DataTable dtNode = ds.Tables[0];
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

                    // 永远使设计器的宽和高为节点的最大值　
                    if (y > DesignerHeight - 10 - flowNode.ActualHeight / 2)
                        DesignerHeight = y + 10 + flowNode.ActualHeight / 2;

                    if (x > DesignerWdith - 10 - flowNode.ActualWidth / 2)
                        DesignerWdith = x + 10 + flowNode.ActualWidth / 2;

                    AddUI(flowNode);
                }

                #endregion 画节点.

                #region 生成标签.
                DataTable dtLabel = ds.Tables[1];
                foreach (DataRow dr in dtLabel.Rows)
                {
                    var nodeLabel = new NodeLabel((IContainer)this);
                    nodeLabel.LabelName = dr["Name"].ToString();
                    nodeLabel.Position = new Point(double.Parse(dr["X"].ToString()), double.Parse(dr["Y"].ToString()));
                    nodeLabel.LableID = dr["MyPK"].ToString();
                    this.AddUI(nodeLabel);
                }
                #endregion 生成标签.

                #region 生成方向.
                DataTable dtDir = ds.Tables[2];

                foreach (DataRow dr in dtDir.Rows)
                {
                    string begin = dr["Node"].ToString();
                    string to = dr["ToNode"].ToString();

                    if (dicFlowNode.ContainsKey(begin) && dicFlowNode.ContainsKey(to))
                    {
                        var d = new Direction((IContainer)this);
                        d.DirType = (DirType)Enum.Parse(typeof(DirType), dr["DirType"].ToString(), false);
                        d.IsCanBack = (int.Parse(dr["IsCanBack"]) == 0) ? false : true;

                        if (dr["Dots"] != null
                            && string.IsNullOrEmpty(dr["Dots"].ToString()) == false)
                        {
                            string[] strs = dr["Dots"].ToString().Split('@');
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
                        d.BeginFlowNode = dicFlowNode[begin]; //开始节点.
                        d.EndFlowNode = dicFlowNode[to]; //结束节点.

                        //增加方向.
                        this.AddUI(d);
                    }
                }

                //Direction da = new Direction((IContainer)this);
                //da.FlowID = FlowID;
                //da.BeginFlowNode = node0;
                //da.EndFlowNode = GetBeginFlowNode();
                //da.IsTrackingLine = true;
                //AddDirection(da);

                Dictionary<FlowNode, Direction> dicEndNodeAndDir = new Dictionary<FlowNode, Direction>();
                //轨迹中有真实结束点key,则value+=1;回退轨迹中有真实结束点key,则value-=1.
                //最后对=1的结束点对应的方向线(表示已走过)进行着色.
                Dictionary<FlowNode, int> dicEndNodeAndTrackCount = new Dictionary<FlowNode, int>();

                //IList<FlowNode> endNodes = GetEndFlowNodes();
                //foreach (FlowNode item in endNodes)
                //{
                //    Direction db = new Direction((IContainer)this);
                //    db.FlowID = FlowID;
                //    db.BeginFlowNode = item;
                //    db.EndFlowNode = node1;
                //    AddDirection(db);
                //    dicEndNodeAndDir.Add(item, db);
                //    dicEndNodeAndTrackCount.Add(item, 0);
                //}
                #endregion 生成方向.

                #region 标记颜色, 显示轨迹。

                if (trackDataSet != null && null != trackDataSet.Tables["WF_Track"] && trackDataSet.Tables["WF_Track"].Rows.Count > 0)
                {
                    /*
                     * 说明已经取到了轨迹数据.
                     * 1,流程在运行过程中WF_Track 忠实的记录了每个操作动作.表中数据只增加不会减少更不修改它.
                     * 2,每个事件都一个事件类型ActionType, 它是一个枚举类型的.
                     */
                    this.dicReturnTrackTips.Clear();

                    foreach (DataRow dr in trackDataSet.Tables["WF_Track"].Rows)
                    {
                        string begin = dr["NDFrom"] == null ? string.Empty : dr["NDFrom"].ToString();
                        string to = dr["NDTo"] == null ? string.Empty : dr["NDTo"].ToString();
                        string rdt = dr["RDT"] == null ? string.Empty : dr["RDT"].ToString();// RDT记录日期
                        string msg = dr["Msg"] == null ? string.Empty : dr["Msg"].ToString();

                        if (isAddNodeTip)
                        {
                            #region 添加节点标注
                          
                            try
                            {
                                DateTime time = Convert.ToDateTime(rdt);
                                rdt = time.ToString("MM月dd日HH:mm");
                            }
                            catch { }

                            string EmpFrom = dr["EmpFrom"] == null ? string.Empty : dr["EmpFrom"].ToString(); // EmpFromT实际执行人员
                            string EmpFromT = dr["EmpFromT"] == null ? string.Empty : dr["EmpFromT"].ToString();

                            FlowNode node = dicFlowNode[begin];
                            if (null != node && !node.IsTrackDealed)
                            {
                                // 计算流程节点是否是多人协作完成
                                if (node.TodolistModel == TodolistModel.QiangBan)
                                    addNodeTip(node, EmpFrom, EmpFromT, rdt);
                                else
                                {
                                    addNodeTip(node, "", "多人协作完成", !string.IsNullOrEmpty(rdt) && rdt.Length > 5 ? rdt.Substring(0, rdt.Length - 5) : "");
                                }
                                node.IsTrackDealed = true;
                            }
                            #endregion
                        }

                        #region 画绿色的轨迹线表示已经走过的节点.
                        foreach (Direction dir in DirectionCollections)
                        {
                            if (dir.BeginFlowNode.NodeID == begin && dir.EndFlowNode.NodeID == to)
                            {
                                dir.IsTrackingLine = true;

                                if (dicEndNodeAndTrackCount.Keys.Contains(dir.EndFlowNode))
                                {
                                    dicEndNodeAndTrackCount[dir.EndFlowNode] += 1;
                                }
                                break;
                            }
                        }
                        #endregion

                     
                        ActionType at = (ActionType)int.Parse(dr["ActionType"].ToString());
                        switch (at)
                        {
                            case ActionType.Forward: /*普通节点发送*/
                            case ActionType.ForwardFL: /*分流点发送*/
                            case ActionType.ForwardHL: /*合流点发送*/
                            case ActionType.SubFlowForward: /*子线程点发送*/
                            case ActionType.HungUp: /*挂起*/
                                break;
                            case ActionType.Skip: /*跳转*/
                               
                                break;
                            case ActionType.Return: /*退回*/

                                bool isExist = false;

                                foreach (var dir in dicReturnTrackTips.Keys)
                                {
                                    if (dir.BeginFlowNode.NodeID == begin && dir.EndFlowNode.NodeID == to)
                                    {
                                        dicReturnTrackTips[dir].Add(msg);
                                        isExist = true;
                                        break;
                                    }
                                }

                                if (!isExist)
                                {
                                    foreach (Direction dir in DirectionCollections)
                                    {
                                        if (dir.BeginFlowNode.NodeID == begin && dir.EndFlowNode.NodeID == to && dir.DirType == BP.DirType.Backward)
                                        {

                                            dir.IsTrackingLine = true;
                                            dicReturnTrackTips.Add(dir, new List<string>());
                                            dicReturnTrackTips[dir].Add(msg);

                                            isExist = true;
                                            break;
                                        }
                                    }
                                }

                                //已经删除回退线，仍需显示出已走轨迹。
                                if (!isExist)
                                {
                                    if (dicFlowNode.ContainsKey(begin) && dicFlowNode.ContainsKey(to))
                                    {
                                        var d = new Direction((IContainer)this);
                                        d.FlowID = FlowID;
                                        d.BeginFlowNode = dicFlowNode[begin]; //开始节点.
                                        d.EndFlowNode = dicFlowNode[to]; ; //结束节点.
                                        d.LineType = DirectionLineType.Polyline;
                                        d.IsTrackingLine = true;
                                        d.DirType = DirType.Backward;
                                        dicReturnTrackTips.Add(d, new List<string>());
                                        dicReturnTrackTips[d].Add(msg);

                                        if (paint.Children.Contains(d) == false)
                                        {
                                            paint.Children.Add(d);
                                            d.Container = this;
                                        }
                                    }
                                }

                                if (dicFlowNode.ContainsKey(begin) && dicEndNodeAndTrackCount.Keys.Contains(dicFlowNode[begin]))
                                {
                                    dicEndNodeAndTrackCount[dicFlowNode[begin]] -= 1;
                                }

                                break;
                          
                            default:
                                break;
                        }

                    }
                }

                foreach (var node in dicEndNodeAndTrackCount)
                {
                    if (node.Value == 1)
                    {
                        dicEndNodeAndDir[node.Key].IsTrackingLine = true;
                    }
                }


                dicEndNodeAndDir.Clear();
                dicEndNodeAndTrackCount.Clear();

               
                #endregion 标记颜色.

                #region 去除没有轨迹的回退方向线
                //List<Direction> dirToDel = new List<Direction>();

                //foreach (var dir in DirectionCollections)
                //{
                //    if (dir.IsReturnType && !dir.IsTrackingLine)
                //    {
                //        dirToDel.Add(dir);
                //    }
                //}

                //foreach (var dir in dirToDel)
                //{
                //    this.RemoveDirection(dir);
                //}

                #endregion

                #region 禁用工作流所有元素以禁用工作流元素的所有事件,只查看
                foreach (UIElement ui in paint.Children)
                {
                    if (ui is IElement)
                    {
                        UserControl uc = ui as UserControl;
                        if (uc != null)
                            uc.IsEnabled = false;
                    }
                }
                #endregion
            }
            catch (System.Exception ee)
            {
                BP.SL.LoggerHelper.Write(ee);
            }
            finally
            {
                setContainerStyle();
            }
        }

        public void AddUI(Direction dir)
        {
           
            if (paint.Children.Contains(dir) == false)
            {
                paint.Children.Add(dir);
                dir.Container = this;
                //SolidColorBrush brush = new SolidColorBrush();
                //brush = SystemConst.ColorConst.UnTrackingColor.DirectionColor as SolidColorBrush;//.Color = Colors.Gray;
                //if (dir.BeginFlowNode.NodeID == "0")
                //{
                //    brush = SystemConst.ColorConst.TrackingColor.DirectionColor as SolidColorBrush;//Colors.Green;
                //}
                ////dir.begin.Fill = brush;
                ////dir.endArrow.Stroke = brush;
                ////dir.line.Stroke = brush;
                ////if (dir.LineType == DirectionLineType.Polyline)
                ////{
                ////    dir.DirectionTurnPoint1.Fill = brush;
                ////    dir.DirectionTurnPoint2.Fill = brush;
                ////}

                //dir.begin.Fill = brush;
                //dir.begin.Width = 4;
                //dir.begin.Height = 4;
                //dir.endArrow.Stroke = brush;
                //dir.endArrow.StrokeThickness = 2;
                //dir.line.Stroke = brush;
                //dir.line.StrokeThickness = 2;
                //if (dir.LineType == DirectionLineType.Polyline)
                //{
                //    dir.DirectionTurnPoint1.Fill = brush;
                //    dir.DirectionTurnPoint2.Fill = brush;
                //    dir.DirectionTurnPoint1.eliTurnPoint.Width = 4;
                //    dir.DirectionTurnPoint1.eliTurnPoint.Height = 4;
                //    dir.DirectionTurnPoint2.eliTurnPoint.Width = 4;
                //    dir.DirectionTurnPoint2.eliTurnPoint.Height = 4;
                //}
            }

            if (DirectionCollections.Contains(dir) == false)
                DirectionCollections.Add(dir);        
        }

        public void AddUI(NodeLabel l)
        {
            if (!paint.Children.Contains(l) )
                paint.Children.Add(l);

        }

        public void AddUI(FlowNode a)
        {
            if (!paint.Children.Contains(a))
            {
                paint.Children.Add(a);
                a.Container = this;
                a.FK_Flow = FlowID;
            }

            if (!FlowNodeCollections.Contains(a) )
            {
                FlowNodeCollections.Add(a);

                if (!dicFlowNode.ContainsKey(a.NodeID))
                {
                    dicFlowNode.Add(a.NodeID, a);
                }
            }
        }

        private void addNodeTip(FlowNode node, string empFrom, string empFromT, string receiveTime)
        {
            string imageSource = "{0}.png";

            if (string.IsNullOrEmpty(empFrom))
                imageSource = string.Format(imageSource, "Default");
            else
                imageSource = string.Format(imageSource, empFrom);

            NodeTip tip = new NodeTip(empFromT, receiveTime);
            setImage(tip.bg, imageSource);
            if (this.flow_ChartType == FlowChartType.UserIcon)
            {
                tip.SetValue(Canvas.LeftProperty, Canvas.GetLeft(node) + node.UIWidth);
            }
            else
            {
                tip.SetValue(Canvas.LeftProperty, Canvas.GetLeft(node) + node.UIWidth/2 + 10);
            }
            tip.SetValue(Canvas.TopProperty, Canvas.GetTop(node) + node.UIHeight / 2 - tip.Height / 2);

            tip.SetValue(Canvas.ZIndexProperty, 1000);
            paint.Children.Add(tip);
        }

        public void setImage(System.Windows.Controls.Image image, String file)
        {
            string uri = Application.Current.Host.Source.AbsoluteUri;//服务器路径
            int index = uri.IndexOf("/ClientBin");
            uri = uri.Substring(0, index) + "/DataUser/UserIcon/" + file;
            Uri u = new Uri(uri);
            System.Net.WebClient wc = new System.Net.WebClient();

            wc.OpenReadAsync(u);
            wc.OpenReadCompleted += delegate(object sender, System.Net.OpenReadCompletedEventArgs e)
            {
                System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
                if (null == e.Error)
                {
                    bitmapImage.SetSource(e.Result);
                    image.Source = bitmapImage;
                }
            };
        }

        #region
        public bool CtrlKeyIsPress
        {
            get { throw new NotImplementedException(); }
        }

        public double ScrollViewerHorizontalOffset
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public double ScrollViewerVerticalOffset
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsSomeChildEditing
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsContainerRefresh
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool MouseIsInContainer
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool IsMouseSelecting
        {
            get { throw new NotImplementedException(); }
        }

        public Direction CurrentTemporaryDirection
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public List<IElement> SelectedWFElements
        {
            get { throw new NotImplementedException(); }
        }

        public PageEditType EditType
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Canvas GridLinesContainer
        {
            get { throw new NotImplementedException(); }
        }

        public void RemoveUI(FlowNode a)
        {
            throw new NotImplementedException();
        }

        public void RemoveUI(NodeLabel l)
        {
            throw new NotImplementedException();
        }

        public void CreateLabel(Point p)
        {
            throw new NotImplementedException();
        }

        public void RemoveUI(Direction r)
        {
            throw new NotImplementedException();
        }

        public void SelectDirection(Direction r, bool isSelected)
        {
            throw new NotImplementedException();
        }

        public void SelectFlowNode(FlowNode a, bool isSelected)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string message)
        {
            throw new NotImplementedException();
        }

        public void ShowSetting(FlowNode ac)
        {
            throw new NotImplementedException();
        }

        public void ShowSetting(Direction rc)
        {
            throw new NotImplementedException();
        }

        public void ShowContentMenu(FlowNode a, object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void ShowContentMenu(NodeLabel l, object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void ShowContentMenu(Direction r, object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void AddSelectedControl(IElement uc)
        {
            throw new NotImplementedException();
        }

        public void RemoveSelectedControl(IElement uc)
        {
            throw new NotImplementedException();
        }

        public void SelectedWorkFlowElement(IElement uc, bool isSelect)
        {
            throw new NotImplementedException();
        }

        public void MoveControlCollectionByDisplacement(double x, double y, IElement uc)
        {
            throw new NotImplementedException();
        }

        public void ClearSelected(IElement uc)
        {
            throw new NotImplementedException();
        }

        public void CopySelectedToMemory(IElement currentControl)
        {
            throw new NotImplementedException();
        }

        public void UpdateSelectedToMemory(IElement currentControl)
        {
            throw new NotImplementedException();
        }

        public void UpdateSelectedNode(IElement currentControl)
        {
            throw new NotImplementedException();
        }

        public void DeleteSeleted()
        {
            throw new NotImplementedException();
        }

        public void PastMemoryToContainer()
        {
            throw new NotImplementedException();
        }

        public void ActionPrevious()
        {
            throw new NotImplementedException();
        }

        public void ActionNext()
        {
            throw new NotImplementedException();
        }

        public bool Contains(UIElement uiel)
        {
            throw new NotImplementedException();
        }

        public CheckResult CheckSave()
        {
            throw new NotImplementedException();
        }

        public void SaveChange(HistoryType action)
        {
            throw new NotImplementedException();
        }

        public void SetProper(string lang, string dotype, string fk_flow, string node1, string node2, string title)
        {
            throw new NotImplementedException();
        }

        public void SetGridLines(bool isShow)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

}
