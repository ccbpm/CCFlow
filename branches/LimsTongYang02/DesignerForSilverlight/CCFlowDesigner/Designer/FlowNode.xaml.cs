#region
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ccflow.Web.Component.Workflow;
using WF.WS;
using Silverlight;
using WF.Designer;
using BP.Picture;
using System.ComponentModel;
using BP;
using System.Windows.Media.Imaging;
using System.IO;
using BP.Controls;
#endregion

namespace BP
{
    public partial class FlowNode :  IElement
    {

        #region IElement 成员
        public CheckResult CheckSave()
        {
            CheckResult cr = new CheckResult();
            cr.IsPass = true;


            //if (BeginDirectionCollections != null
            //    && BeginDirectionCollections.Count > 0)
            //{
            //    cr.IsPass = false;//不能有后继节点
            //    cr.Message += string.Format(Text.Message_NotHaveFollowUpFlowNode, NodeName);
            //}
            //if (EndDirectionCollections == null
            //    || EndDirectionCollections.Count == 0)
            //{
            //    cr.IsPass = false;//必须至少有一个前驱节点
            //    cr.Message += string.Format(Text.Message_MustHaveAtLeastOnePreFlowNode, NodeName);
            //}

            isPassCheck = cr.IsPass;
            if (!cr.IsPass)
            {
                errorTipControl.Visibility = Visibility.Visible;
                errorTipControl.ErrorMessage = cr.Message.TrimEnd("\r\n".ToCharArray());
            }
            else
            {
                if (_errorTipControl != null)
                {
                    _errorTipControl.Visibility = Visibility.Collapsed;
                    container.Children.Remove(_errorTipControl);
                    _errorTipControl = null;
                }
            }
            return cr;
        }
        public void UpperZIndex()
        {
            ZIndex = _container.NextMaxIndex;
        }
        public void Zoom(double zoomDeep)
        {
            if (origPictureWidth == 0)
            {
                origPictureWidth = bgShape.PictureWidth;
                origPictureHeight = bgShape.PictureHeight;
            }
            if (isPositionChanged)
            {
                origPosition = this.Position;
                isPositionChanged = false;
            }

            bgShape.PictureHeight = origPictureHeight * zoomDeep;
            bgShape.PictureWidth = origPictureWidth * zoomDeep;
            this.Position = new Point(origPosition.X * zoomDeep, origPosition.Y * zoomDeep);
        }
        public void Edit()
        {
            _container.ShowSetting(this);
        }

        public UserStation Station()
        {
            UserStation us = new UserStation();
            us.IsPass = true;
            return us;
        }
        public void Worklist_del(DataSet dataSet)
        {
            if (dataSet == null || dataSet.Tables.Count == 0)
                return;

            bool ishave = false;
            string empName = "";
            string sdt = "";
            int rowIndex = 0;
            foreach (DataRow dr in dataSet.Tables[1].Rows)
            {
                if (this.NodeID == dr["NodeID"].ToString())
                {
                    ishave = true;
                    empName += dr["EmpName"].ToString() + ";";

                    // 第一个点应该是 xxx在xxx时间发起，而非xxx在什么时间接受.
                    if (rowIndex == 0)
                        sdt = DateTime.Parse(dr["RDT"].ToString()).ToString("MM月dd号HH时mm分") + "发起";
                    else
                        sdt = DateTime.Parse(dr["RDT"].ToString()).ToString("MM月dd号HH时mm分") + "接收";
                }
                rowIndex++;
            }
            if (ishave == false)
                return;

            var dir = new Direction(_container)
            {
                BeginFlowNode = this,
                EndFlowNode = stationTipControl,
                IsTemporaryDirection = true,
                // FK_Flow = this.FK_Flow,
                Container = _container
            };

            _container.AddUI(dir);
            stationTipControl.Visibility = Visibility.Visible;
            stationTipControl.StationMessage = empName.TrimEnd(';') + "\n" + sdt;
            _stationTipControl = null;

            //  stationTipControl.Width = stationTipControl.Width + 40;
            //  stationTipControl.Height = stationTipControl.Height -20;
            //   _container.ContainerHeight = _container.ContainerHeight / 3;
        }
        public string StationMessage
        {
            set
            {

                //  bgShape.picSubThreadNode.tbMessage.Text = value;
            }
        }

        #endregion 
      
        double origPictureWidth = 0,origPictureHeight = 0;
        Point origPosition;
        bool isPositionChanged ;
        public event MoveDelegate FlowNodeMove;
        public event DeleteDelegate DeleteFlowNode;
        public event FlowNodeChangeDelegate FlowNodeChanged;


        #region StyleProperties

        public bool IsIconNeedUpdate;
        string icon = "Default";
        /// <summary>
        /// 如果流程ChartType = UserIcon,则节点使用图标显示
        /// 该值存储图标名称，路径固定
        /// </summary>
        public string Icon
        {
            get
            {
                return icon;
            }
            set
            {
                if (icon != value )
                {
                    icon = value;
                    if (IsIconNeedUpdate)
                    {
                        this.update();
                    }
                }
             
                this.SetStyle();
            }
        }

        NodePosType hisPosType = NodePosType.Mid;
        public NodePosType HisPosType
        {
            get { return hisPosType; }
            set
            {
                if (value != hisPosType)
                {
                    hisPosType = value;
                    this.SetStyle();
                }
            }
        }

        bool isSelectd = false;
        public bool IsSelectd
        {
            get
            {
                return isSelectd;
            }
            set
            {
                if (value != IsSelectd)
                {
                    isSelectd = value;
                    if (isSelectd && !_container.SelectedWFElements.Contains(this))
                        _container.AddSelectedControl(this);
                    this.SetStyle();
                }
            }
        }


        /// <summary>
        /// 轨迹节点
        /// </summary>
        private bool isTrackingNode = false;
        public bool IsTrackingNode
        {
            get { return isTrackingNode; }
            set
            {
                if (isTrackingNode != value)
                {
                    isTrackingNode = value;
                    this.SetStyle();
                }
            }
        }


        FlowNodeType nodeType = FlowNodeType.Ordinary;
        /// <summary>
        /// 更改节点类型的同时，要更新UI，保存数据
        /// </summary>
        public FlowNodeType NodeType
        {
            get
            {
                return nodeType;
            }
            set
            {
                if (Container.Flow_ChartType != FlowChartType.UserIcon)
                {
                    bgShape.NodeType = value;//Update UI
                }
               
                if (nodeType != value)
                {
                    nodeType = value;// Only Update value ,be used to update DB

                    if (IsIconNeedUpdate)
                    {
                        IsIconNeedUpdate = false;
                        this.update();
                        Move(this, null);
                    }
                }
            }
        }

        public IFlowNode CurrentNodeUI
        {
            get 
            {
                this.bgShape.Visibility = System.Windows.Visibility.Collapsed;
                this.bgIcon.Visibility = System.Windows.Visibility.Collapsed;

                if (Container.Flow_ChartType == FlowChartType.UserIcon)
                {
                   this.bgIcon.Visibility = System.Windows.Visibility.Visible;
                   return this.bgIcon;
                }
                else
                {
                    this.bgShape.Visibility = System.Windows.Visibility.Visible;
                    return this.bgShape;
                }
            }
        }

        public double UIWidth
        {
            get
            {
                return CurrentNodeUI.PictureWidth;
            }
        }

        public double UIHeight
        {
            get
            {
                return CurrentNodeUI.PictureHeight;
            }
        }

        public string NodeName
        {
            get
            {
                return CurrentNodeUI.NodeName;
            }
            set
            {
                CurrentNodeUI.NodeName = value;
            }
        }
        bool isPassCheck
        {

            set
            {
                if (!value)
                {
                    this.bgShape.Background = SystemConst.ColorConst.WarningColor as SolidColorBrush;
                }
            }
        }

        void SetStyle()
        {
            if (this.nodeType == FlowNodeType.VirtualEnd || this.nodeType == FlowNodeType.VirtualStart) return;

            if (Container.Flow_ChartType == FlowChartType.UserIcon)
            {
                this.bgIcon.Visibility = System.Windows.Visibility.Visible;
                this.bgShape.Visibility = System.Windows.Visibility.Collapsed;
                
                BitmapImage imageObj = null;
                if (!string.IsNullOrEmpty(icon))
                    imageObj = ToolBoxNodeIcons.Instance.GetIcon(icon);
                else
                    imageObj = ToolBoxNodeIcons.Instance.GetIcon(ToolBoxNodeIcons.IconDefault);

                if (imageObj != null)
                {
                    this.bgIcon.bgBorder.Background = new ImageBrush() { ImageSource = imageObj, Stretch = Stretch.Uniform };
                }

                if (this.IsTrackingNode)
                {

                }
                else  if (this.isSelectd)
                {
                    this.bgIcon.bgBorder.BorderThickness = new Thickness(2);
                    this.bgIcon.bgBorder.BorderBrush = BaseSetup.DefaultSelectColor;

                    this.bgIcon.bgBorder.RenderTransform = new RotateTransform() { Angle = 1 };
                }
                else
                {
                    this.bgIcon.bgBorder.RenderTransform = new RotateTransform() { Angle = 0 };

                    SolidColorBrush bgBorder = new SolidColorBrush();
                    this.bgIcon.bgBorder.BorderThickness = new Thickness(2);

                    switch (this.hisPosType)
                    {
                        case NodePosType.Start:
                        case NodePosType.End:

                            if(this.hisPosType== NodePosType.Start)
                                bgBorder.Color = Colors.Green;
                            else
                                bgBorder.Color = Colors.Red;

                            this.bgIcon.bgBorder.BorderBrush = bgBorder;
                            this.bgIcon.bgBorder.BorderThickness = new Thickness(1);

                            break;
                        case NodePosType.Mid:
                            this.bgIcon.bgBorder.BorderThickness = new Thickness(0);
                            this.bgIcon.bgBorder.BorderBrush = bgBorder;

                            break;
                    }

                }
            }
            else //if (Container.Flow_ChartType == FlowChartType.Shape)
            {
                this.Width = CurrentNodeUI.PictureWidth;
                this.Height = CurrentNodeUI.PictureHeight;

                SolidColorBrush fg = new SolidColorBrush(Colors.Black);
                SolidColorBrush bg = new SolidColorBrush();
                SolidColorBrush bgBorder = new SolidColorBrush();
                if (this.IsTrackingNode)
                {
                    //fg.Color = Colors.White;
                    //bgBorder = SystemConst.ColorConst.TrackingColor.TrackLineColor as SolidColorBrush;
                    //bg = SystemConst.ColorConst.TrackingColor.TrackLineColor as SolidColorBrush;
                    fg.Color = Colors.Black;
                    bgBorder = new SolidColorBrush(Colors.DarkGray);
                    switch (this.hisPosType)
                    {
                        case NodePosType.Start:
                        case NodePosType.End:
                            if (this.hisPosType == NodePosType.Start)
                                bg.Color = Colors.Green;
                            else
                                bg.Color = Colors.Red;
                            break;
                        case NodePosType.Mid:
                            bg.Color = Color.FromArgb(255, 48, 114, 241);
                            break;
                    }
                }
                else  if (this.isSelectd)
                {
                    fg.Color = Colors.White;
                    bgBorder = SystemConst.ColorConst.SelectedBorder as SolidColorBrush;
                    bg = SystemConst.ColorConst.TrackingColor.SelectedColor as SolidColorBrush;
                }
                else
                {
                    fg.Color = Colors.Black;
                    bgBorder = new SolidColorBrush(Colors.DarkGray);
                    switch (this.hisPosType)
                    {
                        case NodePosType.Start:
                        case NodePosType.End:
                            if( this.hisPosType == NodePosType.Start)
                                bg.Color = Colors.Green;
                            else
                                bg.Color = Colors.Red;
                            break;
                        case NodePosType.Mid:
                            bg.Color = Color.FromArgb(255, 48, 114, 241);
                            break;
                    }
                }
              
                this.CurrentNodeUI.Name.Foreground = fg;
                this.bgShape.Background = bg;
                this.bgShape.SetBorderColor(bgBorder);  

                this.bgIcon.Visibility = System.Windows.Visibility.Collapsed;
                this.bgShape.Visibility = System.Windows.Visibility.Visible;
            }
            
        }


        #endregion

        #region Properties


        public TodolistModel TodolistModel;

        /// <summary>
        /// 节点轨迹是否已处理过
        /// </summary>
        public bool IsTrackDealed;
       
        public List<Direction> BeginDirectionCollections = new List<Direction>();

        public List<Direction> EndDirectionCollections = new List<Direction>();

      
        ErrorTip _errorTipControl;
        ErrorTip errorTipControl
        {
            get
            {
                if (_errorTipControl == null)
                {
                    _errorTipControl = new ErrorTip();
                    _errorTipControl.ParentElement = this;
                    container.Children.Add(_errorTipControl);

                }
                _errorTipControl.SetValue(Canvas.ZIndexProperty, 1);

                var top = -this.UIHeight / 2;
                var left = this.UIWidth;

                _errorTipControl.SetValue(Canvas.TopProperty, top);
                _errorTipControl.SetValue(Canvas.LeftProperty, left);
                return _errorTipControl;
            }
        }

        FlowNode _stationTipControl;
        /// <summary>
        /// 状态提示节点
        /// </summary>
        FlowNode stationTipControl
        {
            get
            {
                if (_stationTipControl == null)
                {
                    _stationTipControl = new FlowNode(_container);
                    _stationTipControl.NodeType = FlowNodeType.Ordinary;

                    _stationTipControl.Container = this.Container;
                  // _stationTipControl.bgShape.txtNodeName.Text = "";
                    // container.Children.Add(_stationTipControl);
                    _container.AddUI(_stationTipControl);

                }
                _stationTipControl.SetValue(Canvas.ZIndexProperty, 1);
                _stationTipControl.SetValue(Canvas.TopProperty, -(this.UIHeight / 2) - 40);
                _stationTipControl.SetValue(Canvas.LeftProperty, this.UIWidth - 60);
                _stationTipControl.CenterPoint = new Point(this.CenterPoint.X + this.UIWidth + 40, this.CenterPoint.Y);
                return _stationTipControl;
            }
        }

        IContainer _container;
        public IContainer Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
                bgShape.CurrentContainer = value;
                bgIcon.CurrentContainer = value;
            }
        }

      
        string fk_Flow;
        public string FK_Flow
        {
            get
            {
                if (string.IsNullOrEmpty(fk_Flow))
                {
                    fk_Flow = Guid.NewGuid().ToString();
                }
                return fk_Flow;
            }
            set
            {
                fk_Flow = value;
            }
        }

        string nodeID;
        public string NodeID
        {
            get
            {

                return nodeID;
            }
            set
            {
                nodeID = value;
            }
        }

     
        public int ZIndex
        {
            get
            {
                return (int)this.GetValue(Canvas.ZIndexProperty);

            }
            set
            {
                this.SetValue(Canvas.ZIndexProperty, value);
            }
        }

        string _subFlow;
        public string SubFlow
        {
            get
            {
                return _subFlow;
            }
            set
            {
                _subFlow = value;
            }
        }
        bool isDeleted = false;
        public bool IsDeleted
        {
            get
            {
                return isDeleted;
            }
        }

       
        FlowNodeComponent flowNodeData;
        public FlowNodeComponent FlowNodeData
        {
            get
            {
                if (flowNodeData == null)
                {
                    if (EditType == PageEditType.Add)
                    {
                        flowNodeData = new FlowNodeComponent();
                        flowNodeData.NodeID = this.NodeID;
                        flowNodeData.FK_Flow = this.FK_Flow;
                        flowNodeData.NodeName = NodeName;
                        flowNodeData.FlowNodeType = NodeType.ToString();
                        flowNodeData.RepeatDirection = RepeatDirection.ToString();
                        flowNodeData.SubFlow = SubFlow;


                    }
                    else if (EditType == PageEditType.Modify)
                    {
                        flowNodeData = getFlowNodeComponentFromServer(this.NodeID);

                    }
                }
                return flowNodeData;
            }
            set
            {
                flowNodeData = value;
            }
        }

        PageEditType editType = PageEditType.None;
        public PageEditType EditType
        {
            get
            {
                return editType;
            }
            set
            {
                editType = value;
            }
        }

        public Point CenterPoint
        {
            get
            {
                return new Point((double)this.GetValue(Canvas.LeftProperty) + this.Width / 2,
                    (double)this.GetValue(Canvas.TopProperty) + this.Height / 2);
            }
            set
            {
                this.SetValue(Canvas.LeftProperty, value.X - this.Width / 2);
                this.SetValue(Canvas.TopProperty, value.Y - this.Height / 2);
                Move(this, null);
            }
        }

        public Point Position
        {
            get
            {
                Point position = new Point();
                position.Y = (double)this.GetValue(Canvas.TopProperty);
                position.X = (double)this.GetValue(Canvas.LeftProperty);

                return position;
            }
            set
            {
                this.SetValue(Canvas.TopProperty, value.Y);
                this.SetValue(Canvas.LeftProperty, value.X);
                Move(this, null);
            }
        }
        public WorkFlowElementType ElementType
        {
            get
            {
                return WorkFlowElementType.FlowNode;
            }
        }

        bool canShowMenu = false;
        public bool CanShowMenu
        {
            get
            {
                return canShowMenu;
            }
            set
            {
                canShowMenu = value;
            }
        }

        FlowNode originFlowNode;
        public FlowNode OriginFlowNode
        {
            get
            {
                return originFlowNode;
            }
            set
            {
                originFlowNode = null;
            }
        }


        public bool PointIsInside(Point p)
        {
            bool isInside = false;


            double thisWidth = nodeUI.ActualWidth;
            double thisHeight = nodeUI.ActualHeight;

            double thisX = CenterPoint.X - thisWidth / 2;
            double thisY = CenterPoint.Y - thisHeight / 2;

            if (thisX < p.X && p.X < thisX + thisWidth
                && thisY < p.Y && p.Y < thisY + thisHeight)
            {
                isInside = true;
            }


            return isInside;
        } 
        #endregion

     
        public FlowNode(IContainer container) : this()
        {
            _container = container;

            if (Container.Flow_ChartType == FlowChartType.UserIcon)
            {
                this.Icon = "Default";
            }
           
            editType = PageEditType.Add;
            this.Name = FK_Flow;
            this.NodeType = FlowNodeType.Ordinary;
                  
            sbDisplay.Begin();
        }

        private FlowNode()
        {
            InitializeComponent();
            #region
            System.Windows.Browser.HtmlPage.Document.AttachEvent("oncontextmenu",
                (object sender, System.Windows.Browser.HtmlEventArgs e)=>
                {
                    //if (_container.MouseIsInContainer)
                    //{
                    //    e.PreventDefault();
                    //    if (canShowMenu && !IsDeleted)
                    //    {
                    //        _container.ShowFlowNodeContentMenu(this, sender, e);
                    //    }
                    //}
                });
            this.container.MouseRightButtonDown += (object sender, MouseButtonEventArgs e)=>
            {
                if (_container.MouseIsInContainer)
                {
                    e.GetPosition(null);

                    if (canShowMenu && !IsDeleted)
                    {
                        _container.ShowContentMenu(this, sender, e);
                    }
                }
            };
            #endregion
        }

        /// <summary>
        /// 更新节点运行类型和图标
        /// </summary>
        private void update()
        {
            if (string.IsNullOrEmpty(FK_Flow))
                return;
            try
            {
                string sql = "UPDATE WF_Node SET RunModel={0},Icon='{1}' WHERE FK_Flow='{2}' AND NodeID='{3}'";
                sql = string.Format(sql, (int)NodeType, Icon, FK_Flow, NodeID);
                this.Cursor = Cursors.Wait;
                var ws = Glo.GetDesignerServiceInstance();
                ws.RunSQLAsync(sql, Glo.UserNo, Glo.SID);
                ws.RunSQLCompleted += delegate(object sender, global::WF.WS.RunSQLCompletedEventArgs e)
                {
                    this.Cursor = Cursors.Arrow;

                    if (e.Error != null)
                    {
                        BP.Glo.ShowException(e.Error);
                    }
                    else
                    {
                        int result = e.Result;
                        IsIconNeedUpdate = false;
                    }
                };
            }
            catch (System.Exception e)
            {
                BP.Glo.ShowException(e);
            }
        }


        public MergePictureRepeatDirection RepeatDirection
        {
            get
            {
                return bgShape.RepeatDirection;
            }
            set
            {
                bool isChanged = false;
                if (bgShape.RepeatDirection != value)
                {
                    isChanged = true;
                }
                bgShape.RepeatDirection = value;

                if (isChanged)
                {
                    Move(this, null);
                }
            }
        }

        public void SetFlowNodeData(FlowNodeComponent FlowNodeData)
        {
            bool isChanged = false;

            if (FlowNodeData.NodeName != FlowNodeData.NodeName
                || FlowNodeData.FlowNodeType != FlowNodeData.FlowNodeType
                || FlowNodeData.RepeatDirection != FlowNodeData.RepeatDirection)
            {
                isChanged = true;

            }

            setUIValueByFlowNodeData(FlowNodeData);
            if (isChanged)
            {
                if (FlowNodeChanged != null)
                    FlowNodeChanged(this);
            }
            IsSelectd = IsSelectd;

        }

        void setUIValueByFlowNodeData(FlowNodeComponent FlowNodeData)
        {
            NodeName = FlowNodeData.NodeName;
            FlowNodeType type = (FlowNodeType)Enum.Parse(typeof(FlowNodeType), FlowNodeData.FlowNodeType, true);
            MergePictureRepeatDirection repeatDirection = (MergePictureRepeatDirection)Enum.Parse(typeof(MergePictureRepeatDirection), FlowNodeData.RepeatDirection, true);
            NodeType = type;
            RepeatDirection = repeatDirection;
            SubFlow = FlowNodeData.SubFlow;
        }

        public PointCollection ThisPointCollection
        {
            get
            {
                return bgShape.ThisPointCollection;
            }
        }

        FlowNodeComponent getFlowNodeComponentFromServer(string NodeID)
        {
            FlowNodeComponent ac = new FlowNodeComponent();
            ac = new FlowNodeComponent();
            ac.NodeID = this.NodeID;
            ac.FK_Flow = this.FK_Flow;
            ac.NodeName = NodeName;
            ac.FlowNodeType = NodeType.ToString();
            ac.SubFlow = this.SubFlow;
            return ac;
        }

        public string ToXmlString()
        {
            var xml = new System.Text.StringBuilder();

            xml.Append(@"       <FlowNode ");
            xml.Append(@" FK_Flow=""" + FK_Flow + @"""");
            xml.Append(@" NodeID=""" + NodeID + @"""");
            xml.Append(@" NodeName=""" + NodeName + @"""");
            xml.Append(@" Type=""" + NodeType.ToString() + @"""");
            xml.Append(@" SubFlow=""" + (NodeType == FlowNodeType.Ordinary ? SubFlow : @"") + @"""");
            xml.Append(@" PositionX=""" + CenterPoint.X + @"""");
            xml.Append(@" PositionY=""" + CenterPoint.Y + @"""");
            xml.Append(@" RepeatDirection=""" + RepeatDirection.ToString() + @"""");
            xml.Append(@" ZIndex=""" + ZIndex + @""">");

            xml.Append(Environment.NewLine);
            xml.Append("        </FlowNode>");
            return xml.ToString();
        }

        public void LoadFromXmlString(string xmlString)
        {
        }

        public void AddDirection(Direction r, DirectionMove moveType)
        {
            switch (moveType)
            {
                case DirectionMove.Begin:
                    if (!BeginDirectionCollections.Contains(r))
                    {
                        BeginDirectionCollections.Add(r);
                        r.BeginFlowNode = this;
                        Move(this, null);

                        // 添加节点后，将结束节点变为交互节点
                        //if (FlowNodeType.COMPLETION == this.Type)
                        //{
                        //    this.Type = FlowNodeType.INTERACTION;
                        //}
                    }
                    break;
                case DirectionMove.End:
                   
                    if (!EndDirectionCollections.Contains(r))
                    {
                        EndDirectionCollections.Add(r);
                        r.EndFlowNode = this;
                        Move(this, null);

                    }
                    break;

            }
        }

        public void RemoveBeginDirection(Direction r)
        {
            if (BeginDirectionCollections.Contains(r))
            {
                BeginDirectionCollections.Remove(r);
                r.RemovFlowNode(this);

                //移除连线后，如果如果节点不是开始节点，并且没有开始节点，则变为结束节点
                if (0 == BeginDirectionCollections.Count && this.hisPosType!= NodePosType.Start&& this.hisPosType != NodePosType.End)
                {
                    this.HisPosType = NodePosType.End;
                }
            }
        }

        public void RemoveEndDirection(Direction r)
        {
            if (EndDirectionCollections.Contains(r))
            {
                EndDirectionCollections.Remove(r);
                r.RemovFlowNode(this);
            }
        }

        public void Delete()
        {
            if (!isDeleted)
            {
                _container._Service.DoAsync("DelNode", this.NodeID, true);
                _container._Service.DoCompleted += (_Service_DoCompleted);
            }
        }

        void _Service_DoCompleted(object sender, DoCompletedEventArgs e)
        {
            _container._Service.DoCompleted -= (_Service_DoCompleted);

            if (e.Result != null)
            {
                MessageBox.Show(e.Result);
                return;
            }


            isDeleted = true;
            canShowMenu = false;
            sbClose.Completed += new EventHandler(sbClose_Completed);
            sbClose.Begin();
            foreach (Direction d in _container.DirectionCollections)
            {
                if (d.BeginFlowNode == this || d.EndFlowNode == this)
                {
                    d.Delete();
                }
            }

        }
        
        void sbClose_Completed(object sender, EventArgs e)
        {
            if (isDeleted)
            {
                if (this.BeginDirectionCollections != null)
                {
                    foreach (Direction r in this.BeginDirectionCollections)
                    {
                        r.RemovFlowNode(this);
                    }
                }
                if (this.EndDirectionCollections != null)
                {
                    foreach (Direction r in this.EndDirectionCollections)
                    {
                        r.RemovFlowNode(this);
                    }
                }
                if (DeleteFlowNode != null)
                    DeleteFlowNode(this);

                _container.RemoveUI(this);

                //if (FlowNodeChanged != null)
                //    FlowNodeChanged(this);
            }
        }


        public FlowNode Clone()
        {
            FlowNode clone = new FlowNode(this._container);
            clone.NodeType = this.NodeType;
            clone.originFlowNode = this;
            clone.FlowNodeData = new FlowNodeComponent();
            clone.FlowNodeData.NodeName = this.FlowNodeData.NodeName;
            clone.FlowNodeData.FlowNodeType = this.FlowNodeData.FlowNodeType;
            clone.setUIValueByFlowNodeData(clone.FlowNodeData);
            // clone.CenterPoint = this.CenterPoint;
            clone.CenterPoint = this.CenterPoint;
            clone.ZIndex = this.ZIndex;
            //_container.AddFlowNode(clone);
            return clone;
        }

        void FlowNodeChange()
        {
            if (FlowNodeChanged != null)
            {
                FlowNodeChanged(this);
            }
        }


        #region Mouse Move and Click 
       


        bool trackingMouseMove, isMoved;
        Point mousePosition;
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            canShowMenu = true;
            this.nodeCenter.Opacity = 0.6;
            this.Cursor = Cursors.Hand;
        }
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            canShowMenu = false;
            this.nodeCenter.Opacity = 0.1;
            this.Cursor = Cursors.Arrow;

        }
        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            isMoved = false;

            if (!isMoved && !_container.IsMouseSelecting)
            {
                IsSelectd = !IsSelectd;
                _container.SelectedWorkFlowElement(this, IsSelectd);
            }

            if (Glo.IsDbClick)
            {
                trackingMouseMove = false;
                _container.ShowSetting(this);
            }
            else
            {
                this.SetValue(Canvas.ZIndexProperty, _container.NextMaxIndex);
                mousePosition = e.GetPosition(null);

                trackingMouseMove = true;
                FrameworkElement element = sender as FrameworkElement;
                if (null != element)
                {
                    element.CaptureMouse();
                }
            }
        }
        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (_container.IsMouseSelecting || null != _container.CurrentTemporaryDirection )
                e.Handled = false;
            else
                e.Handled = true;

            if (isMoved)
                FlowNodeChange();
            
            FrameworkElement element = sender as FrameworkElement;
            mousePosition.X = mousePosition.Y = 0;
            element.Cursor = null;
            trackingMouseMove = false;
            element.ReleaseMouseCapture();

        }
        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (trackingMouseMove)
            {
                FrameworkElement element = sender as FrameworkElement;
            
                try
                {
                    //FlowNodePictureContainer fn = sender as FlowNodePictureContainer;
                    //OrdinaryNode sn = fn.currentPic as OrdinaryNode;

                    //if (sn != null && sn.picRect.Cursor == Cursors.SizeNWSE)
                    //{
                    //    sn.picRect.Width += 1;
                    //    sn.picRect.Height += 1;
                    //}
                    //else
                    {
                        element.Cursor = Cursors.Hand;

                        if (e.GetPosition(null) == mousePosition)
                            return;

                        isMoved = true;
                        double deltaV = e.GetPosition(null).Y - mousePosition.Y;
                        double deltaH = e.GetPosition(null).X - mousePosition.X;
                        double newTop = deltaV + Position.Y;
                        double newLeft = deltaH + Position.X;

                        double containerWidth = (double)this.Parent.GetValue(Canvas.WidthProperty);
                        double containerHeight = (double)this.Parent.GetValue(Canvas.HeightProperty);
                        if ((deltaH < 0 && CenterPoint.X - nodeUI.ActualWidth / 2 < 2)
                            || (deltaV < 0 && CenterPoint.Y - nodeUI.ActualHeight / 2 < 2))
                        {
                            //超过流程容器的范围
                        }
                        else
                        {
                            isPositionChanged = true;
                            this.SetValue(Canvas.TopProperty, newTop);
                            this.SetValue(Canvas.LeftProperty, newLeft);

                            Move(this, e);
                            mousePosition = e.GetPosition(null);
                            _container.MoveControlCollectionByDisplacement(deltaH, deltaV, this);
                            _container.IsNeedSave = true;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private void nodeCenter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("WorkID")
                || System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FK_Flow"))
                return;
         
            if (isDeleted)
                return;

            if (_container.CurrentTemporaryDirection == null)
            {
                _container.CurrentTemporaryDirection = new Direction(_container, true);
                _container.CurrentTemporaryDirection.BeginFlowNode = this;
                _container.CurrentTemporaryDirection.PointBegin = this.CenterPoint;
                _container.CurrentTemporaryDirection.PointEnd = _container.CurrentTemporaryDirection.PointBegin;
                _container.CurrentTemporaryDirection.ZIndex = _container.NextMaxIndex;

                _container.AddUI(_container.CurrentTemporaryDirection);
                _container.IsNeedSave = true;
            }
           
        }
        private void nodeCenter_MouseMove(object sender, MouseEventArgs e)
        {
            if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("WorkID")
                || System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FK_Flow"))
            {
                return;

            }
            if (isDeleted)
            {
                return;
            }

        }
        private void nodeCenter_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (isDeleted)
                return;

            if (_container.CurrentTemporaryDirection != null 
                && (_container.CurrentTemporaryDirection.BeginFlowNode == this
                ||_container.CurrentTemporaryDirection.BeginFlowNode == _container.CurrentTemporaryDirection.EndFlowNode))
            {
                this.RemoveBeginDirection(_container.CurrentTemporaryDirection);
                _container.RemoveUI(_container.CurrentTemporaryDirection);
                _container.CurrentTemporaryDirection = null;
            }           
        }
        
        public void ResetPosition(double x, double y)
        {
            double left = (double)this.GetValue(Canvas.LeftProperty),
                top = (double)this.GetValue(Canvas.TopProperty);

            this.SetValue(Canvas.TopProperty, top + y);
            this.SetValue(Canvas.LeftProperty, left + x);
            Move(this, null);

        }
        
        public void Move(FlowNode a, MouseEventArgs e)
        {
            if (FlowNodeMove != null)
                FlowNodeMove(a, e);

            //  节点移动时是否自动调整容器大小
            if (Glo.IsDragNodeResizeContainer)
            {
                Container container = _container as Container;
                if (null != container)
                {
                    container.DragNodeResizeContainer(this);
                }
            }
        }

        #endregion


        public Point GetPointOfIntersection(Point pBegin, Point pEnd, DirectionMove type)
        {
            
            Point p = new Point();
            #region

            if (Math.Abs(pEnd.X - pBegin.X) <= UIWidth / 2
                && Math.Abs(pEnd.Y - pBegin.Y) <= UIHeight / 2)
            {
                p = pEnd;
            }
            else
            {
                //起始点坐标和终点坐标之间的夹角（相对于Y轴坐标系）
                double angle = Math.Abs(Math.Atan((pEnd.X - pBegin.X) / (pEnd.Y - pBegin.Y)) * 180.0 / Math.PI);
                //节点的长和宽之间的夹角（相对于Y轴坐标系）
                double angel2 = Math.Abs(Math.Atan(UIWidth / UIHeight) * 180.0 / Math.PI);
                //半径
                double radio = UIHeight < UIWidth ? UIHeight / 2 : UIWidth / 2;

                if (angle <= angel2)//起点坐标在终点坐标上/下方
                {
                    if (pEnd.Y < pBegin.Y)//在上方
                    {
                        if (pEnd.X < pBegin.X)
                            p.X = pEnd.X + Math.Tan(Math.PI * angle / 180.0) * radio;
                        else
                            p.X = pEnd.X - Math.Tan(Math.PI * angle / 180.0) * radio;

                        p.Y = pEnd.Y + UIHeight / 2;
                    }
                    else//在下方
                    {
                        if (pEnd.X < pBegin.X)
                            p.X = pEnd.X + Math.Tan(Math.PI * angle / 180.0) * radio;
                        else
                            p.X = pEnd.X - Math.Tan(Math.PI * angle / 180.0) * radio;

                        p.Y = pEnd.Y - UIHeight / 2;
                    }
                }
                else//左右方
                {
                    if (pEnd.X < pBegin.X)//在右方
                    {
                        p.X = pEnd.X + UIWidth / 2;
                        if (pEnd.Y < pBegin.Y)
                            p.Y = pEnd.Y + Math.Tan(Math.PI * (90 - angle) / 180.0) * radio;
                        else
                            p.Y = pEnd.Y - Math.Tan(Math.PI * (90 - angle) / 180.0) * radio;
                    }
                    else//在左方
                    {
                        p.X = pEnd.X - UIWidth / 2;
                        if (pEnd.Y < pBegin.Y)
                            p.Y = pEnd.Y + Math.Tan(Math.PI * (90 - angle) / 180.0) * radio;
                        else
                            p.Y = pEnd.Y - Math.Tan(Math.PI * (90 - angle) / 180.0) * radio;

                    }
                }
            }

            #endregion

            //double beginPointRadius = 0
            //   , endPointRadius = 0;
            //switch (type)
            //{
            //    case DirectionMove.Begin:
            //        p.X -= beginPointRadius;
            //        p.Y -= beginPointRadius;
            //        break;
            //    case DirectionMove.End: 
            //        p.X -= endPointRadius;
            //        p.Y -= endPointRadius;
            //        break;
            //}
           
            return p;
        }

        public void e(Rect R,Rect r)
        {
            //if (R.Left < r.Right && r.Left < R.Right && R.Top < r.Bottom && r.Top < R.Bottom)
            if (!(r.Right < R.Left || r.Bottom < R.Top
                || R.Right < r.Left || R.Bottom < r.Top))// 相离
            {
                if (r.Right < R.Left)// ←
                {

                }
                else if (r.Bottom < R.Top)//↑
                {

                }
                else if (R.Right < r.Left)// right
                {

                }
                else if (R.Bottom < r.Top)// bottom
                {

                }
            }
            else
            {

            }

           
        }
    }
}
