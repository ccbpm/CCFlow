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
using Ccflow.Web.Component.Workflow;
using Silverlight;
using BP;

namespace BP
{
    public partial class Direction  : IElement
    {
        #region IElement

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

        public WorkFlowElementType ElementType
        {
            get
            {
                return WorkFlowElementType.Direction;
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
                isSelectd = value;
                _container.SelectDirection(this, value);
                ////if ((this.line.Stroke as SolidColorBrush).Color.ToString() == Colors.Red.ToString())
                ////    return;

                //SolidColorBrush brush = new SolidColorBrush();

                //if (IsTrackingLine)
                //{
                //    if (isSelectd)
                //    {
                //        brush.Color = Color.FromArgb(255, 255, 0, 255);
                //        if (!_container.SelectedWFElements.Contains(this))
                //            _container.AddSelectedControl(this);
                //    }
                //    else
                //    {
                //        brush.Color = Colors.Red;
                //    }
                //}
                //else
                //{
                if (isSelectd)
                {
                    //brush.Color = Color.FromArgb(255, 255, 181, 0);
                    if (!_container.SelectedWFElements.Contains(this))
                        _container.AddSelectedControl(this);
                }
                else
                {
                    //brush.Color = Color.FromArgb(255, 0, 128, 0);
                }

                //}

                //begin.Fill = brush;
                //endArrow.Stroke = brush;
                //line.Stroke = brush;
                //if (LineType == DirectionLineType.Polyline)
                //{
                //    ruleTurnPoint1.Fill = brush;
                //    ruleTurnPoint2.Fill = brush;
                //}
            }
        }

        public CheckResult CheckSave()
        {
            CheckResult cr = new CheckResult();
            cr.IsPass = true;

            if (BeginFlowNode == null && EndFlowNode == null)
            {
                cr.Message += "eee";
                //Text.Message_MustBeLinkToBeginAndEndFlowNode;
                cr.IsPass = false;
            }
            else
            {
                if (BeginFlowNode == null)
                {
                    cr.Message += "eddee";
                    //  cr.Message += Text.Message_MustBeLinkToBeginFlowNode;
                    cr.IsPass = false;
                }
                if (EndFlowNode == null)
                {
                    //cr.Message += Text.Message_MustBeLinkToEndFlowNode;
                    cr.IsPass = false;
                }
            }
            isPassCheck = cr.IsPass;
            if (!cr.IsPass)
            {
                errorTipControl.Visibility = Visibility.Visible;
                errorTipControl.ErrorMessage = cr.Message;
            }
            else
            {
                if (_errorTipControl != null)
                {
                    _errorTipControl.Visibility = Visibility.Collapsed;
                    cnDirectionContainer.Children.Remove(_errorTipControl);
                    _errorTipControl = null;
                }
            }
            return cr;
        }

        public void Delete()
        {

            if (!isDeleted)
            {
                isDeleted = true;
                if (this.IsTemporaryDirection)
                {
                    sbBeginClose_Completed(null, null);
                }
                else
                {
                    sbBeginClose.Completed += new EventHandler(sbBeginClose_Completed);
                    sbBeginClose.Begin();
                }
            }

            try
            {
                _container._Service.DoDropLineAsync(int.Parse(this.BeginFlowNode.NodeID), int.Parse(this.EndFlowNode.NodeID));
            }
            catch { }
        }
        void sbBeginClose_Completed(object sender, EventArgs e)
        {
            if (this.EndFlowNode != null)
                this.EndFlowNode.RemoveEndDirection(this);
            if (this.BeginFlowNode != null)
                this.BeginFlowNode.RemoveBeginDirection(this);
            if (DeleteDirection != null)
                DeleteDirection(this);
            _container.RemoveUI(this);

            //if (DirectionChanged != null)
            //    DirectionChanged(this);
        }


        public void UpperZIndex()
        {
            ZIndex = _container.NextMaxIndex;
        }

        public void Zoom(double zoomDeep)
        {
            if (isPositionTurnPointChanged)
            {
                pBegin = PointBegin;
                pEnd = PointEnd;
                pTurn1 = PointTurn1.CenterPosition;
                pTurn2 = PointTurn2.CenterPosition;
                isPositionTurnPointChanged = false;
            }
            if (BeginFlowNode == null)
                PointBegin = new Point(pBegin.X * zoomDeep, pBegin.Y * zoomDeep);
            if (EndFlowNode == null)
                PointEnd = new Point(pEnd.X * zoomDeep, pEnd.Y * zoomDeep);
            if (LineType == DirectionLineType.Polyline)
            {
                PointTurn1.CenterPosition = new Point(pTurn1.X * zoomDeep, pTurn1.Y * zoomDeep);
                onDirectionTurnPointMove(PointTurn1, PointTurn1.CenterPosition);
                PointTurn2.CenterPosition = new Point(pTurn2.X * zoomDeep, pTurn2.Y * zoomDeep);
                onDirectionTurnPointMove(PointTurn2, PointTurn2.CenterPosition);
            }
        }

        public void Edit()
        {
            _container.ShowSetting(this);
        }
        public UserStation Station()
        {
            return null;
        }

        /// <summary>
        /// 轨迹数据
        /// </summary>
        /// <param name="trackDataSet"></param>
        public bool MarkRed(DataSet trackDataSet)
        {
            var brush = new SolidColorBrush();
            brush.Color = Colors.Red;
            if (trackDataSet == null || trackDataSet.Tables.Count == 0)
                return false;

            DataTable dt = trackDataSet.Tables["WF_Track"];
            foreach (DataRow dr in dt.Rows)
            {
                string begin = dr["NDFrom"].ToString();
                string to = dr["NDTo"].ToString();

                if (this.BeginFlowNode.NodeID == begin && this.EndFlowNode.NodeID == to)
                {
                    brush = new SolidColorBrush();
                    brush.Color = Colors.Red;
                    this.dBegin.Fill = brush;
                    this.endArrow.Stroke = brush;
                    this.line.Stroke = brush;

                    this.BeginFlowNode.BorderBrush = brush;
                    this.EndFlowNode.BorderBrush = brush;
                    return true;
                }
            }
            return false;
        }
        #endregion

        public event DeleteDelegate DeleteDirection;
        public event DirectionChangeDelegate DirectionChanged;
        public DirectionMove MoveType;

        string flowID;
        public string FlowID
        {
            get
            {
                if (string.IsNullOrEmpty(flowID))
                {
                    flowID = Guid.NewGuid().ToString();
                }
                return flowID;
            }
            set
            {
                flowID = value;
            }

        }
        string ruleID;
        public string DirectionID
        {
            get
            {
                ruleID = Guid.NewGuid().ToString();
                return ruleID;
            }
            set
            {
                ruleID = value;
            }

        }
        string ruleName;
        public string DirectionName
        {
            get
            {
                return ruleName;
            }
            set
            {
                ruleName = value;
                tbDirectionName.Text = value;
                DirectionData.DirectionName = value;
            }

        }
        
        /// <summary>
        /// 轨迹线
        /// </summary>
        private bool isTrackingLine = false;
        public bool IsTrackingLine
        {
            get { return isTrackingLine; }
            set
            {
                isTrackingLine = value;
                isSelectd = false;

                SolidColorBrush brush = SystemConst.ColorConst.TrackingColor.TrackLineColor as SolidColorBrush;
                brush.Opacity = 2;
               dBegin.Fill = brush;
                endArrow.Stroke = brush;
                line.Stroke = brush;
                if (LineType == DirectionLineType.Polyline)
                {
                    pointTurn1.Fill = brush;
                    pointTurn2.Fill = brush;
                }

#warning 从这里把路过的节点的边框设置成红色。

                BeginFlowNode.IsTrackingNode = true;
                EndFlowNode.IsTrackingNode = true;

                //BeginFlowNode.sdPicture.SetBorderColor(brush);
                //EndFlowNode.sdPicture.SetBorderColor(brush);
                //SolidColorBrush brush1 = new SolidColorBrush(Colors.White);
                //BeginFlowNode.sdPicture.Background = brush1;
                //EndFlowNode.sdPicture.Background = brush1;
                ////(EndFlowNode.sdPicture.currentPic as BP.Picture.OrdinaryNode).picRect.Stroke = brush;
                ////(BeginFlowNode.sdPicture.currentPic as BP.Picture.OrdinaryNode).picRect.Stroke = brush;             
            }
        }
        /// <summary>
        /// 是否为回退线
        /// </summary>
        private DirType dirType = DirType.Forward;
        public DirType DirType
        {
            get { return dirType; }
            set
            {
                dirType = value;
                if (value == BP.DirType.Backward)
                {
                    //IsReturnType = true;
                    line.StrokeDashArray = new DoubleCollection() { 3, 1 };
                    //d.line.StrokeDashCap = PenLineCap.Flat;
                    //d.line.StrokeDashOffset = 1;
                    //d.line.StrokeThickness = 1;
                }
                else
                {
                    line.StrokeDashArray = null;
                }
            }
        }
        DirectionLineType lineType = DirectionLineType.Line;
        public DirectionLineType LineType
        {
            get
            {
                return lineType;
            }
            set
            {
                if (value != lineType)
                {
                    lineType = value;
                    if (lineType == DirectionLineType.Line)
                    {
                        SetDirectionPosition(PointBegin, PointEnd);
                    }
                    else
                    {
                        setTurnPointInitPosition();
                        SetDirectionPosition(PointBegin, PointEnd, PointTurn1.CenterPosition, PointTurn2.CenterPosition);
                    }
                }
            }
        }
        /// <summary>
        /// 是否可原路返回
        /// </summary>
        public bool IsCanBack { get; set; }
       
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

        public bool isPassCheck
        {
            set
            {
                Color color = Colors.Transparent;
                if (value)
                {
                    color = Color.FromArgb(255, 0, 128, 0);
                }
                else
                {
                    color = Color.FromArgb(255, 255, 0, 0);
                }

                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = color;
               dBegin.Fill = brush;
                endArrow.Stroke = brush;
                line.Stroke = brush;

                if (LineType == DirectionLineType.Polyline)
                {
                    pointTurn1.Fill = brush;
                    pointTurn2.Fill = brush;
                }
            }
        }
        DirectionComponent ruleData;
        public DirectionComponent DirectionData
        {
            get
            {
                if (ruleData == null)
                {
                    if (EditType == PageEditType.Add)
                    {
                        ruleData = new DirectionComponent();
                        ruleData.DirectionID = this.DirectionID;
                        ruleData.FlowID = this.FlowID;
                        ruleData.DirectionCondition = "";
                        ruleData.DirectionName = tbDirectionName.Text;
                        ruleData.LineType = LineType.ToString();

                    }
                    else if (EditType == PageEditType.Modify)
                    {
                        ruleData = getDirectionComponentFromServer(this.DirectionID);

                    }
                }
                return ruleData;
            }
            set
            {
                ruleData = value;
            }
        }
        FlowNode beginFlowNode;
        public FlowNode BeginFlowNode
        {
            get
            {
                return beginFlowNode;
            }
            set
            {

                beginFlowNode = value;
                if (beginFlowNode != null)
                {
                    beginFlowNode.AddDirection(this, DirectionMove.Begin);
                    beginFlowNode.FlowNodeMove += new MoveDelegate(OnFlowNodeMove);
                    OnFlowNodeMove(beginFlowNode, null);
                }
            }
        }
        FlowNode endFlowNode;
        public FlowNode EndFlowNode
        {
            get { return endFlowNode; }
            set
            {
                endFlowNode = value;

                if (endFlowNode != null)
                {
                    endFlowNode.AddDirection(this, DirectionMove.End);
                    endFlowNode.FlowNodeMove += new MoveDelegate(OnFlowNodeMove);
                    OnFlowNodeMove(endFlowNode, null);
                }

            }
        }

        Point pBegin, pEnd, pTurn1, pTurn2; 

        public Point PointBegin
        {
            get
            {
                return GetPointPosition(DirectionMove.Begin);
            }
            set
            {
                if (value != null && !double.IsNaN(value.X) && !double.IsNaN(value.Y))
                {
                   dBegin.SetValue(Canvas.TopProperty, value.Y);
                   dBegin.SetValue(Canvas.LeftProperty, value.X);
                    if (LineType == DirectionLineType.Line)
                    {
                        SetDirectionPosition(PointBegin, PointEnd);
                    }
                    else
                    {
                        SetDirectionPosition(PointBegin, PointEnd, PointTurn1.CenterPosition, PointTurn2.CenterPosition);

                    }
                }

            }
        }
        public Point PointEnd
        {
            get
            {
                return GetPointPosition(DirectionMove.End);

            }
            set
            {
                if (value != null && !double.IsNaN(value.X) && !double.IsNaN(value.Y))
                {
                    end.SetValue(Canvas.TopProperty, value.Y);
                    end.SetValue(Canvas.LeftProperty, value.X);
                    if (LineType == DirectionLineType.Line)
                    {
                        SetDirectionPosition(PointBegin, PointEnd);
                    }
                    else
                    {
                        SetDirectionPosition(PointBegin, PointEnd, PointTurn1.CenterPosition, PointTurn2.CenterPosition);

                    }
                }
            }
        }

        DirectionTurnPoint pointTurn2;
        public DirectionTurnPoint PointTurn2
        {
            get
            {
                if (pointTurn2 == null)
                {
                    pointTurn2 = new DirectionTurnPoint();

                    cnDirectionContainer.Children.Add(pointTurn2);
                    pointTurn2.SetValue(Canvas.ZIndexProperty, 200);
                    pointTurn2.DirectionTurnPointMove += new DirectionTurnPoint.DirectionTurnPointMoveDelegate(DirectionTurnPointMove);
                    pointTurn2.OnDoubleClick += new DirectionTurnPoint.DoubleClickDelegate(DirectionTurnPoint_OnDoubleClick);


                }
                return pointTurn2;
            }
        }

        DirectionTurnPoint pointTurn1;
        public DirectionTurnPoint PointTurn1
        {
            get
            {
                if (pointTurn1 == null)
                {
                    pointTurn1 = new DirectionTurnPoint();
                    cnDirectionContainer.Children.Add(pointTurn1);
                    pointTurn1.SetValue(Canvas.ZIndexProperty, 200);
                    pointTurn1.DirectionTurnPointMove += new DirectionTurnPoint.DirectionTurnPointMoveDelegate(DirectionTurnPointMove);
                    pointTurn1.OnDoubleClick += new DirectionTurnPoint.DoubleClickDelegate(DirectionTurnPoint_OnDoubleClick);
                }
                return pointTurn1;
            }
        }

        Direction originDirection;
        public Direction OriginDirection
        {
            get
            {
                return originDirection;
            }
            set
            {
                originDirection = value;
            }

        }
        public bool isTemplateDirection = false;
        public bool IsTemporaryDirection
        {
            get
            {
                return isTemplateDirection;
            }
            set
            {
                isTemplateDirection = value;
                if (value)
                {
                    DoubleCollection d = new DoubleCollection();
                    d.Add(1);
                    line.StrokeDashArray = d;
                }
                else
                {
                    if (DirType == BP.DirType.Backward)
                    {
                        line.StrokeDashArray = new DoubleCollection() { 3, 1 };
                        //line.StrokeDashCap = PenLineCap.Flat;
                        //line.StrokeDashOffset = 1;
                    }
                    else
                    {
                        line.StrokeDashArray = null;
                    }
                }
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

        public Direction(IContainer container)
            : this(container, false)
        {
        }
        public Direction(IContainer container, bool isTemporary)
            : this(container, isTemporary, DirectionLineType.Line)
        {
        }

        double beginPointRadius = 0, endPointRadius = 0;
        public Direction(IContainer container, bool isTemporary, DirectionLineType lineType)
        {
            InitializeComponent();
            this.IsTemporaryDirection = isTemporary;
            LineType = lineType;
            editType = PageEditType.Add;
            _container = container;
            this.Name = FlowID;


            beginPointRadius = dBegin.Width / 2;
            endPointRadius = dEnd.Width / 2;
          
            endArrow.SetValue(Canvas.TopProperty, endPointRadius);
            endArrow.SetValue(Canvas.LeftProperty, endPointRadius);

            if (LineType == DirectionLineType.Line)
                SetDirectionPosition(new Point(0, 0), new Point(50, 50));
            else
                SetDirectionPosition(new Point(0, 0), new Point(50, 50), PointTurn1.CenterPosition, PointTurn2.CenterPosition);
            

             if (!this.IsTemporaryDirection)
            {
                sbBeginDisplay.Begin();
            }
        }
       

        ErrorTip _errorTipControl;
        ErrorTip errorTipControl
        {
            get
            {
                if (_errorTipControl == null)
                {
                    _errorTipControl = new ErrorTip();
                    cnDirectionContainer.Children.Add(_errorTipControl);
                    _errorTipControl.ParentElement = this;
                    _errorTipControl.SetValue(Canvas.ZIndexProperty, 1);
                }
                if (LineType == DirectionLineType.Line)
                {
                    _errorTipControl.SetValue(Canvas.TopProperty, (PointEnd.Y + PointBegin.Y) / 2 - 80);
                    _errorTipControl.SetValue(Canvas.LeftProperty, (PointEnd.X + PointBegin.X) / 2);
                }
                else
                {
                    _errorTipControl.SetValue(Canvas.TopProperty, line.Points[1].Y - 80);
                    _errorTipControl.SetValue(Canvas.LeftProperty, line.Points[1].X);
                }
                return _errorTipControl;
            }
        }
        public Direction Clone()
        {
            Direction clone = new Direction(this._container);
            clone.originDirection = this;
            clone.DirectionData = new DirectionComponent();
            clone.DirectionData.LineType = this.DirectionData.LineType;
            clone.DirectionData.DirectionCondition = this.DirectionData.DirectionCondition;
            clone.DirectionData.DirectionName = this.DirectionData.DirectionName;
            clone.LineType = this.LineType;
            clone.setUIValueByDirectionData(clone.DirectionData);
            clone.PointBegin = this.PointBegin;
            clone.PointEnd = this.PointEnd;
            clone.ZIndex = this.ZIndex;
            if (LineType == DirectionLineType.Polyline)
            {
                clone.PointTurn1.CenterPosition = this.PointTurn1.CenterPosition;
                clone.PointTurn2.CenterPosition = this.PointTurn2.CenterPosition;
            }
            return clone;
        }

     
        DirectionComponent getDirectionComponentFromServer(string ruleID)
        {
            DirectionComponent rc = new DirectionComponent();
            rc.DirectionID = this.DirectionID;
            rc.FlowID = this.FlowID;
            rc.DirectionCondition = "";
            rc.DirectionName = tbDirectionName.Text;
            rc.LineType = Enum.GetName(typeof(DirectionLineType), LineType);
            return rc;
        }

        public string ToXmlString()  { return "";  }
        public void LoadFromXmlString(string xmlString)   {  }

        public Point GetCurrentMovedPointPosition()
        {
            return GetPointPosition(MoveType);
        }
        public Point GetPointPosition(DirectionMove MoveType)
        {
            Point p = new Point();
            if (MoveType == DirectionMove.Begin)
            {
                p.X = (double) dBegin.GetValue(Canvas.LeftProperty);
                p.Y = (double)dBegin.GetValue(Canvas.TopProperty);

            }
            else if (MoveType == DirectionMove.End)
            {
                p.X = (double)end.GetValue(Canvas.LeftProperty);
                p.Y = (double)end.GetValue(Canvas.TopProperty);

            }
            return p;
        }
        public Point GetResetPoint(Point beginPoint, Point endPoint, FlowNode node, DirectionMove type)
        {
            return node.GetPointOfIntersection(beginPoint, endPoint, type);
        }
       

        void OnFlowNodeMove(FlowNode node, MouseEventArgs e)
        {
            if (node != EndFlowNode && node != BeginFlowNode)
                return;

            double newTop =  (double)node.GetValue(Canvas.TopProperty);
            double newLeft =  (double)node.GetValue(Canvas.LeftProperty);
            //if (this.Container.Flow_ChartType == FlowChartType.UserIcon)
            //{
            //    newTop -=  node.Height / 2;
            //    newLeft -= node.Width / 2;
            //}
            //if (this.Container.Flow_ChartType == FlowChartType.Shape)
            //{
            //    newTop = newTop + node.Height / 2;
            //    newLeft = newLeft + node.Width / 2;
            //}


            Point beginPoint = PointBegin;
            Point endPoint = PointEnd;
            if (node == BeginFlowNode)
            {
               
                if (LineType == DirectionLineType.Line)
                    beginPoint = GetResetPoint(endPoint, node.CenterPoint, node, DirectionMove.Begin);
                else
                    beginPoint = GetResetPoint(PointTurn1.CenterPosition, node.CenterPoint, node, DirectionMove.Begin);
               
                if (EndFlowNode != null)
                {
                    if (LineType == DirectionLineType.Line)
                        endPoint = GetResetPoint(BeginFlowNode.CenterPoint, EndFlowNode.CenterPoint, EndFlowNode, DirectionMove.End);
                    else
                        endPoint = GetResetPoint(PointTurn2.CenterPosition, EndFlowNode.CenterPoint, EndFlowNode, DirectionMove.End);
                }
              
            }
            else if (node == EndFlowNode )
            {
                if (LineType == DirectionLineType.Line)
                    endPoint = GetResetPoint(beginPoint, node.CenterPoint, node, DirectionMove.End);
                else
                    endPoint = GetResetPoint(PointTurn2.CenterPosition, node.CenterPoint,
                        node, DirectionMove.End);

                if (BeginFlowNode != null)
                {
                    if (LineType == DirectionLineType.Line)
                        beginPoint = GetResetPoint(EndFlowNode.CenterPoint, 
                            BeginFlowNode.CenterPoint, BeginFlowNode, DirectionMove.Begin);
                    else
                        beginPoint = GetResetPoint(PointTurn1.CenterPosition, 
                            BeginFlowNode.CenterPoint, BeginFlowNode, DirectionMove.Begin);
                }
            }

            //beginPoint.X = beginPoint.X - beginPointRadius;
            //beginPoint.Y = beginPoint.Y - beginPointRadius;
            //endPoint.X = endPoint.X  - endPointRadius;
            //endPoint.Y = endPoint.Y - endPointRadius;

            if (LineType == DirectionLineType.Line)
            {
                bool isReturn = false;

                #region 双向线
                foreach (Direction d in Container.DirectionCollections)
                {
                    if (d == this)
                    { // 同一个规则箭头
                        continue;
                    }
                    else if (d.beginFlowNode == this.beginFlowNode && d.endFlowNode == this.endFlowNode)
                    {//重复的规则箭头
                        continue;
                    }
                    else if (d.beginFlowNode == this.endFlowNode && d.endFlowNode == this.beginFlowNode)
                    {// 往返规则箭头

                        Point pStartBack, pEndBack;
                        Point[] newStartPoint;
                        if (null != this.beginFlowNode && null != this.endFlowNode)
                        {
                            pStartBack = this.beginFlowNode.CenterPoint;
                            pEndBack = this.endFlowNode.CenterPoint;
                            newStartPoint = PointCount.IntPoint(pStartBack, pEndBack, BaseSetup.BidRad, this.DirType, BaseSetup.ActivityWidth, BaseSetup.ActivityHeight);
                            this.PointBegin = new Point(newStartPoint[0].X, newStartPoint[0].Y);
                            this.PointEnd = new Point(newStartPoint[1].X, newStartPoint[1].Y);
                        }

                        if (null != d && null != d.beginFlowNode && null != d.beginFlowNode)
                        {
                            pStartBack = d.beginFlowNode.CenterPoint;
                            pEndBack = d.endFlowNode.CenterPoint;
                            newStartPoint = PointCount.IntPoint(pStartBack, pEndBack, BaseSetup.BidRad, d.DirType, BaseSetup.ActivityWidth, BaseSetup.ActivityHeight);
                            d.PointBegin = new Point(newStartPoint[0].X, newStartPoint[0].Y);
                            d.PointEnd = new Point(newStartPoint[1].X, newStartPoint[1].Y);
                        }

                        isReturn = true;
                        break;
                    }
                }
                #endregion

                if (!isReturn)
                {
                    SetDirectionPosition(beginPoint, endPoint);
                }
            }
            else
            {
                SetDirectionPosition(beginPoint, endPoint,
                    PointTurn1.CenterPosition, PointTurn2.CenterPosition);
            }
        }

        public void RemovFlowNode(FlowNode a)
        {
            if (BeginFlowNode == a)
                BeginFlowNode = null;
            else if (EndFlowNode == a)
                EndFlowNode = null;
            
            //需要删除事件代理 
        }

        public bool IsTurnPoint1Moved, IsTurnPoint2Moved;
        bool isRuleMoved, isPointMoved, isLineMoved, trackingMouseMove, isPositionTurnPointChanged = true;

        Point trackPosition;

        void setDirectionNameControlPosition()
        {
            double top = 0;
            double left = 0;
            if (this.LineType == DirectionLineType.Line)
            {
                left = (PointBegin.X + PointEnd.X) / 2;
                top = (PointBegin.Y + PointEnd.Y) / 2;
            }
            else
            {
                left = (line.Points[1].X + line.Points[2].X) / 2;
                top = (line.Points[1].Y + line.Points[2].Y) / 2;
            }
            tbDirectionName.SetValue(Canvas.TopProperty, top - 15);
            tbDirectionName.SetValue(Canvas.LeftProperty, left - 10);
            _container.IsNeedSave = true;
        }
        void setLinkToFlowNode(DirectionMove movetype)
        {
            double centerX = 0;
            double centerY = 0;

            if (movetype == DirectionMove.Begin)
            {
                centerX = PointBegin.X + beginPointRadius;
                centerY = PointBegin.Y + beginPointRadius;

            }
            if (movetype == DirectionMove.End)
            {
                centerX = PointEnd.X + endPointRadius;
                centerY = PointEnd.Y + endPointRadius;

            }

            FlowNode act = null;
            bool isLinked = false;
            for (int i = 0; i < _container.FlowNodeCollections.Count; i++)
            {

                if (isLinked)
                    break;
                act = _container.FlowNodeCollections[i];


                if (act.PointIsInside(new Point(centerX, centerY)))
                {
                    if (movetype == DirectionMove.Begin)
                    {
                        #region 检查
                        //if (act.Type == FlowNodeType.COMPLETION)
                        //{
                        //    act.Type = FlowNodeType.INTERACTION;
                        //    //ShowMessage(Text.Message_EndFlowNodeCanNotHaveFollowUpActivitiy);
                        //    //isLinked = false;
                        //    //break;
                        //}
                        //else if ((act.Type == FlowNodeType.AND_MERGE
                        //    || act.Type == FlowNodeType.OR_MERGE
                        //    || act.Type == FlowNodeType.VOTE_MERGE)
                        //    && act.BeginDirectionCollections != null)
                        //{

                        //    int count = act.BeginDirectionCollections.Count;
                        //    if (act.BeginDirectionCollections.Contains(this))
                        //        count--;
                        //    if (count > 0)
                        //    {
                        //        //ShowMessage(Text.Message_SubThreadNodeSheetnlyHaveAFollowUpFlowNode);
                        //        //isLinked = false;
                        //        //break;
                        //    }
                        //}
                        #endregion
                        if (this.EndFlowNode == act)
                        {
                            //if (!IsTemporaryDirection)
//                                ShowMessage(Text.Message_BeginAndEndFlowNodeCanNotBeTheSame);
                            //ShowMessage("错误");
                        }
                        else
                        {
                            if (this.EndFlowNode == null)
                            {
                                act.AddDirection(this, DirectionMove.End);
                                if (this.IsTemporaryDirection)
                                    this.DirectionName =  _container.GetNewElementName(WorkFlowElementType.Direction);
                                isLinked = true;
                            }
                            else
                            {
                                bool isExists = false;
                                if (act.BeginDirectionCollections != null)
                                {
                                    for (int j = 0; j < act.BeginDirectionCollections.Count; j++)
                                    {
                                        if (act.BeginDirectionCollections[j].EndFlowNode == this.EndFlowNode
                                           && act.BeginDirectionCollections[j].BeginFlowNode != this.BeginFlowNode
                                            )
                                        {
                                            isExists = true;
                                            break;
                                        }
                                    }
                                }
                                if (isExists)
                                {
                                    _container.ShowMessage("方向已存在");
                                }
                                else
                                {
                                    act.AddDirection(this, DirectionMove.Begin);
                                    if (this.IsTemporaryDirection)
                                        this.DirectionName =  _container.GetNewElementName(WorkFlowElementType.Direction);

                                    isLinked = true;
                                }
                            }
                        }
                    }
                    if (movetype == DirectionMove.End)
                    {
                        #region 检查

                        if (this.IsTemporaryDirection)
                        {
                            if (this.BeginFlowNode == act)
                            {
                                isLinked = false;
                                break;
                            }
                        }
                      
                        //if (act.HisPosType == NodePosType.Start )
                        //{
                        //    //开始节点不能有前驱节点
                        //    _container.ShowMessage("开始节点不能有前驱节点");
                        //    isLinked = false;
                        //    break;
                        //}
                        //else if ((act.Type == FlowNodeType.AND_BRANCH
                        //   || act.Type == FlowNodeType.OR_BRANCH)
                        //   )
                        //{
                        //    if (act.EndDirectionCollections != null
                        //    && act.EndDirectionCollections.Count > 0)
                        //    {
                        //        int count = act.EndDirectionCollections.Count;
                        //        if (act.EndDirectionCollections.Contains(this))
                        //            count--;
                        //        if (count > 0)
                        //        {
                        //            act.AddEndDirection(this);
                        //            if (this.IsTemporaryDirection)
                        //                this.DirectionName = Text.NewDirection + _container.NextNewDirectionIndex.ToString();

                        //            isLinked = true;
                        //            //分支节点有且只能有一个前驱节点
                        //            //ShowMessage(Text.Message_HeLiuNodeSheetnlyHaveOnePreFlowNode);
                        //            //isLinked = false;
                        //            break;
                        //        }
                        //    }
                        //}
                        if (this.IsTemporaryDirection)
                        {
                            if (this.BeginFlowNode != null)
                            {
                                //if (this.BeginFlowNode.Type == FlowNodeType.COMPLETION)
                                //{
                                //    this.beginFlowNode.Type = FlowNodeType.INTERACTION;
                                //    //ShowMessage(Text.Message_EndFlowNodeCanNotHaveFollowUpActivitiy);
                                //    //isLinked = false;
                                //    //break;
                                //}
                                //if ((this.BeginFlowNode.Type == FlowNodeType.AND_MERGE
                                //    || this.BeginFlowNode.Type == FlowNodeType.OR_MERGE
                                //    || this.BeginFlowNode.Type == FlowNodeType.VOTE_MERGE)
                                //&& this.BeginFlowNode.BeginDirectionCollections != null)
                                //{
                                //    int count = BeginFlowNode.BeginDirectionCollections.Count;
                                //    if (this.BeginFlowNode.BeginDirectionCollections.Contains(this))
                                //        count--;
                                //    if (count > 0)
                                //    {
                                //        ////汇聚节点只能有一个后继节点
                                //        //ShowMessage(Text.Message_SubThreadNodeSheetnlyHaveAFollowUpFlowNode);
                                //        //isLinked = false;
                                //        //break;
                                //    }
                                //}
                            }

                        }


                        #endregion
                        if (this.BeginFlowNode == act)
                        {
                            //if (!IsTemporaryDirection)
                            //    ShowMessage(Text.Message_BeginAndEndFlowNodeCanNotBeTheSame);
                        }
                        else
                        {
                            if (this.BeginFlowNode == null)
                            {
                                act.AddDirection(this, DirectionMove.End);
                                if (this.IsTemporaryDirection)
                                    this.DirectionName =  _container.GetNewElementName(WorkFlowElementType.Direction);

                                isLinked = true;
                            }
                            else
                            {
                                bool isExists = false;
                                if (act.EndDirectionCollections != null)
                                {
                                    for (int j = 0; j < act.EndDirectionCollections.Count; j++)
                                    {
                                        if (act.EndDirectionCollections[j].BeginFlowNode == this.BeginFlowNode

                                           && act.EndDirectionCollections[j].EndFlowNode != this.EndFlowNode
                                            )
                                        {
                                            isExists = true;
                                            break;
                                        }
                                    }
                                }
                                if (isExists)
                                {
                                   // ShowMessage(Text.Message_TheSameDirectionThatAlreadyExist);
                                }
                                else
                                {
                                    act.AddDirection(this, DirectionMove.End);
                                    if (this.IsTemporaryDirection)
                                       // this.DirectionName = Text.NewDirection + _container.NextNewDirectionIndex.ToString();

                                    isLinked = true;
                                }


                            }
                        }

                    }
                }
            }


        }
        void setUIValueByDirectionData(DirectionComponent ruleData)
        {
            LineType = (DirectionLineType)Enum.Parse(typeof(DirectionLineType), ruleData.LineType, true);
            tbDirectionName.Text = ruleData.DirectionName;
        }
        public void SetDirectionData(DirectionComponent ruleData)
        {
            bool isChanged = false;
            if (DirectionData.DirectionCondition != ruleData.DirectionCondition
               || DirectionData.DirectionName != ruleData.DirectionName
                || DirectionData.LineType != ruleData.LineType)
            {
                isChanged = true;
            }

            DirectionData = ruleData;
            setUIValueByDirectionData(ruleData);
            if (isChanged)
            {
                if (DirectionChanged != null)
                    DirectionChanged(this);
            }
        }



        private void Point_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPointMoved = false;
            trackingMouseMove = false;
            this.SetValue(Canvas.ZIndexProperty, _container.NextMaxIndex);
            FrameworkElement element = sender as FrameworkElement;

            if (element.Name == "begin")
                MoveType = DirectionMove.Begin;
            if (element.Name == "end")
                MoveType = DirectionMove.End;
            if (element.Name == "line")
                MoveType = DirectionMove.Line;

            trackPosition = e.GetPosition(null);
            if (null != element)
            {
                trackingMouseMove = true;
                element.CaptureMouse();
                element.Cursor = Cursors.Hand;

            }
            e.Handled = true;
        }
        private void Point_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_container.IsMouseSelecting || _container.CurrentTemporaryDirection != null)
            {
                e.Handled = false;
            }
            else
                e.Handled = true;

            trackingMouseMove = false;

            if (!_container.SelectedWFElements.Contains(this))
            {
                FrameworkElement element = sender as FrameworkElement;
                element.ReleaseMouseCapture();

                trackPosition.X = trackPosition.Y = 0;
                element.Cursor = null;

                Point centerPoint = new Point();

                FlowNode node = null;

                if (MoveType == DirectionMove.Begin)
                {
                    centerPoint.X = PointBegin.X + beginPointRadius;
                    centerPoint.Y = PointBegin.Y + beginPointRadius;
                    if (BeginFlowNode != null)
                        node = BeginFlowNode;
                }
                else if (MoveType == DirectionMove.End)
                {
                    centerPoint.X = PointEnd.X + endPointRadius;
                    centerPoint.Y = PointEnd.Y + endPointRadius;
                    if (EndFlowNode != null)
                        node = EndFlowNode;
                }


                if (node != null)
                {
                    //移去原来的关联
                    if (!node.PointIsInside(centerPoint))
                    {
                        if (MoveType == DirectionMove.Begin
                             && BeginFlowNode != null)
                        {
                            BeginFlowNode = null;
                            node.RemoveBeginDirection(this);

                        }
                        if (MoveType == DirectionMove.End
                           && EndFlowNode != null)
                        {
                            EndFlowNode = null;
                            node.RemoveEndDirection(this);
                        }
                    }
                }

                setLinkToFlowNode(MoveType);

                ruleChange();
            }
            if (!isPointMoved && !_container.IsMouseSelecting)
            {
                IsSelectd = !IsSelectd;
                _container.SelectedWorkFlowElement(this, IsSelectd);
            }
        }
        private void Point_MouseMove(object sender, MouseEventArgs e)
        {
            if (trackingMouseMove)
            {
                if (e.GetPosition(null) == trackPosition)
                    return;
                if (_container.SelectedWFElements.Contains(this))
                {
                    if (MoveType == DirectionMove.Begin && BeginFlowNode != null
                        && !_container.SelectedWFElements.Contains(this.BeginFlowNode))
                    {
                        isPointMoved = false;
                        trackPosition = e.GetPosition(null);
                        return;
                    }
                    if (MoveType == DirectionMove.End && EndFlowNode != null
                        && !_container.SelectedWFElements.Contains(this.EndFlowNode))
                    {
                        isPointMoved = false;
                        trackPosition = e.GetPosition(null);
                        return;
                    }
                }

                FrameworkElement element = sender as FrameworkElement;
                Point currentPoint = e.GetPosition(this);

                double deltaV = e.GetPosition(null).Y - trackPosition.Y;
                double deltaH = e.GetPosition(null).X - trackPosition.X;

                double containerWidth = (double)this.Parent.GetValue(Canvas.WidthProperty);
                double containerHeight = (double)this.Parent.GetValue(Canvas.HeightProperty);

                if (currentPoint.X > containerWidth
                   || currentPoint.Y > containerHeight
                    || currentPoint.X < 0
                    || currentPoint.Y < 0
                    )
                {
                    //超过流程容器的范围


                }
                else
                {

                    isPositionTurnPointChanged = true;
                    if (_container.SelectedWFElements.Contains(this))
                    {
                        ResetPosition(deltaH, deltaV);

                    }
                    else
                    {

                        if (MoveType == DirectionMove.Begin)
                        {
                            this.PointBegin = currentPoint;

                            if (EndFlowNode != null)
                            {
                                if (LineType == DirectionLineType.Line)
                                {
                                    this.PointEnd = this.GetResetPoint(currentPoint, EndFlowNode.CenterPoint, EndFlowNode, DirectionMove.End);
                                }
                                else
                                {
                                    this.PointEnd = this.GetResetPoint(PointTurn2.CenterPosition, EndFlowNode.CenterPoint, EndFlowNode, DirectionMove.End);

                                }
                            }


                        }
                        else if (MoveType == DirectionMove.End)
                        {
                            this.PointEnd = currentPoint;

                            if (BeginFlowNode != null)
                            {
                                if (LineType == DirectionLineType.Line)
                                {
                                    this.PointBegin = this.GetResetPoint(currentPoint, BeginFlowNode.CenterPoint, BeginFlowNode, DirectionMove.Begin);
                                }
                                else
                                {
                                    this.PointBegin = this.GetResetPoint(PointTurn1.CenterPosition, BeginFlowNode.CenterPoint, BeginFlowNode, DirectionMove.Begin);

                                }
                            }
                        }
                    }

                }
                isPointMoved = true;
                _container.MoveControlCollectionByDisplacement(deltaH, deltaV, this);
                trackPosition = e.GetPosition(null);
            }

        }
        public void SimulateDirectionPointMouseLeftButtonUpEvent(DirectionMove moveType, object sender, MouseButtonEventArgs e)
        {
            MoveType = moveType;
            Point_MouseLeftButtonUp(sender, e);
        }

       
        void DirectionTurnPointMove(object sender, MouseEventArgs e, Point newPoint)
        {
            isPositionTurnPointChanged = true;

            DirectionTurnPoint tPoint = sender as DirectionTurnPoint;
            if( tPoint == PointTurn1)
                onDirectionTurnPointMove(tPoint,newPoint);
            else if( tPoint == PointTurn2)
                onDirectionTurnPointMove(tPoint,newPoint);
        }

        void onDirectionTurnPointMove(DirectionTurnPoint tPoint, Point newPoint)
        {
            if (tPoint == PointTurn1)
            {
                IsTurnPoint1Moved = true;
                line.Points[1] = newPoint;
                if (BeginFlowNode != null)
                {
                    this.PointBegin = this.GetResetPoint(PointTurn1.CenterPosition, BeginFlowNode.CenterPoint, BeginFlowNode, DirectionMove.Begin);

                }
            }
            else if (tPoint == PointTurn2)
            {
                IsTurnPoint2Moved = true;

                line.Points[2] = newPoint;

                if (EndFlowNode != null)
                {
                    this.PointEnd = this.GetResetPoint(tPoint.CenterPosition, EndFlowNode.CenterPoint, EndFlowNode, DirectionMove.End);

                }
                endArrow.SetAngleByPoint(line.Points[2], line.Points[3]);

            }

            setDirectionNameControlPosition();

        }

        void DirectionTurnPoint_OnDoubleClick(object sender, EventArgs e)
        {
            DirectionTurnPoint tPoint = sender as DirectionTurnPoint;
            if (tPoint == PointTurn1)
            {
                Point p = new Point();
                p.X = (PointBegin.X + pointTurn2.CenterPosition.X) / 2;
                p.Y = (PointBegin.Y + pointTurn2.CenterPosition.Y) / 2;
                SetDirectionPosition(PointBegin, PointEnd, p, pointTurn2.CenterPosition);
            }
            else if (tPoint == PointTurn2)
            {
                Point p = new Point();
                p.X = (PointEnd.X + pointTurn1.CenterPosition.X) / 2;
                p.Y = (PointEnd.Y + pointTurn1.CenterPosition.Y) / 2;
                SetDirectionPosition(PointBegin, PointEnd, pointTurn1.CenterPosition, p);
            }
        }
       
       
        void setTurnPointInitPosition()
        {
            Point p2 = new Point();
            Point p3 = new Point();
            Point p1 = new Point(PointBegin.X + beginPointRadius, PointBegin.Y + beginPointRadius);
            Point p4 = new Point(PointEnd.X + endPointRadius, PointEnd.Y + endPointRadius);
           
            p2.X = (p1.X + p4.X) / 2;
            p2.Y = p1.Y;

            p3.X = p2.X;
            p3.Y = p4.Y;

            PointTurn1.CenterPosition = p2;
            PointTurn2.CenterPosition = p3;
            IsSelectd = IsSelectd;
        }
        public void ResetPosition(double x, double y)
        {
            if (BeginFlowNode != null && EndFlowNode != null
                            && !_container.SelectedWFElements.Contains(BeginFlowNode)
                            && !_container.SelectedWFElements.Contains(EndFlowNode)
                            )
            {
            }
            else if (BeginFlowNode != null && EndFlowNode == null
                           && !_container.SelectedWFElements.Contains(BeginFlowNode)
                            )
            {
                SetDirectionPositionByDisplacement(x, y, DirectionMove.End);

            }
            else if (BeginFlowNode == null && EndFlowNode != null
                   && !_container.SelectedWFElements.Contains(EndFlowNode)
                    )
            {
                SetDirectionPositionByDisplacement(x, y, DirectionMove.Begin);

            }
            else
            {
                SetDirectionPositionByDisplacement(x, y, DirectionMove.Line);

            }
        }
        public void SetDirectionPosition(Point beginPoint, Point endPoint)
        {
            SetDirectionPosition(beginPoint, endPoint, null, null);
        }
        public void SetDirectionPosition(Point beginPoint, Point endPoint, Point? turnPoint1, Point? turnPoint2)
        {
            if (double.IsNaN(beginPoint.X)
                || double.IsNaN(beginPoint.Y)
                || double.IsNaN(endPoint.X)
                || double.IsNaN(endPoint.Y)
                )
                return;


           dBegin.SetValue(Canvas.LeftProperty, beginPoint.X);
           dBegin.SetValue(Canvas.TopProperty, beginPoint.Y);

            end.SetValue(Canvas.LeftProperty, endPoint.X);
            end.SetValue(Canvas.TopProperty, endPoint.Y);


            Point p1 = new Point(beginPoint.X + beginPointRadius, beginPoint.Y + beginPointRadius);
            Point p4 = new Point(endPoint.X + endPointRadius, endPoint.Y + endPointRadius);

            Point p2 = new Point();
            Point p3 = new Point();

            if (LineType == DirectionLineType.Line)
            {
                p2 = p1;
                p3 = p1;

                if (pointTurn1 != null)
                    pointTurn1.Visibility = Visibility.Collapsed;

                if (pointTurn2 != null)
                    pointTurn2.Visibility = Visibility.Collapsed;
            }
            else
            {
                PointTurn1.Visibility = Visibility.Visible;
                PointTurn2.Visibility = Visibility.Visible;
                if (turnPoint1 != null && turnPoint2 != null)
                {

                    PointTurn1.CenterPosition = turnPoint1.Value;
                    PointTurn2.CenterPosition = turnPoint2.Value;
                    p2 = PointTurn1.CenterPosition;
                    p3 = PointTurn2.CenterPosition;
                }
                else
                {
                    if (!isLineMoved)
                    {
                        if (IsTurnPoint1Moved)
                        {
                            p2 = PointTurn1.CenterPosition;
                        } if (IsTurnPoint2Moved)
                        {
                            p3 = PointTurn2.CenterPosition;
                        }
                    }

                    PointTurn1.CenterPosition = p2;
                    PointTurn2.CenterPosition = p3;
                }
            }

            line.Points.Clear();
            line.Points.Add(p1);
            line.Points.Add(p2);
            line.Points.Add(p3);
            line.Points.Add(p4);


            endArrow.SetAngleByPoint(p3, p4);
            setDirectionNameControlPosition();
        }
        public void SetDirectionPositionByDisplacement(double x, double y, DirectionMove moveType)
        {
            Point beginPoint = PointBegin;
            Point endPoint = PointEnd;


            if (moveType == DirectionMove.Begin || moveType == DirectionMove.Line)
            {
                beginPoint.X += x;
                beginPoint.Y += y;
            }

            if (moveType == DirectionMove.End || moveType == DirectionMove.Line)
            {
                endPoint.X += x;
                endPoint.Y += y;
            }

            if (LineType == DirectionLineType.Line)
            {
                SetDirectionPosition(beginPoint, endPoint);

            }
            else
            {
                SetDirectionPosition(beginPoint, endPoint, 
                    new Point(PointTurn1.CenterPosition.X + x , PointTurn1.CenterPosition.Y + y ), 
                    new Point(PointTurn2.CenterPosition.X + x , PointTurn2.CenterPosition.Y + y ));

            }

        }
        
        private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isRuleMoved = false;
            isLineMoved = false;
            if (Glo.IsDbClick)
            {
                //spContentMenu.Visibility = Visibility.Collapsed;
                if (this.IsTrackingLine && this.DirType == BP.DirType.Backward 
                    || !this.IsTrackingLine && this.DirType == BP.DirType.Forward)
                {
                    _container.ShowSetting(this);
                }
            }
            else
            {
                this.SetValue(Canvas.ZIndexProperty, _container.NextMaxIndex);
                FrameworkElement element = sender as FrameworkElement;
                trackPosition = e.GetPosition(null);
                if (null != element)
                {
                    isLineMoved = true;
                    element.CaptureMouse();
                    element.Cursor = Cursors.Hand;

                }
            }
            e.Handled = true;

        }
        private void Line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_container.IsMouseSelecting || _container.CurrentTemporaryDirection !=null )
            {
                e.Handled = false;
            }
            else
                e.Handled = true;

            if (!isRuleMoved && !_container.IsMouseSelecting)
            {
                IsSelectd = !IsSelectd;
                _container.SelectedWorkFlowElement(this, IsSelectd);
            }
            isLineMoved = false;
            FrameworkElement element = sender as FrameworkElement;
            element.ReleaseMouseCapture();

            trackPosition.X = trackPosition.Y = 0;
            element.Cursor = null;

            setLinkToFlowNode(DirectionMove.Begin);
            setLinkToFlowNode(DirectionMove.End);

            ruleChange();
        }
        private void Line_MouseMove(object sender, MouseEventArgs e)
        {


            if (isLineMoved)
            {
                FrameworkElement element = sender as FrameworkElement;


                if (BeginFlowNode != null && EndFlowNode != null
                    && !_container.SelectedWFElements.Contains(BeginFlowNode)
                    && !_container.SelectedWFElements.Contains(EndFlowNode)
                    )
                    return;
                if (trackPosition == e.GetPosition(null))
                    return;
                isRuleMoved = true;

                double deltaV = e.GetPosition(null).Y - trackPosition.Y;
                double deltaH = e.GetPosition(null).X - trackPosition.X;

                double newLeft = e.GetPosition((FrameworkElement)this.Parent).X;
                double newTop = e.GetPosition((FrameworkElement)this.Parent).Y;

                double containerWidth = (double)this.Parent.GetValue(Canvas.WidthProperty);
                double containerHeight = (double)this.Parent.GetValue(Canvas.HeightProperty);

                if (containerWidth < newLeft || containerWidth < newTop
                    || newLeft < 0 || newTop < 0
                    )
                {
                    //超过流程容器的范围

                }

                else
                {
                    isPositionTurnPointChanged = true;

                    ResetPosition(deltaH, deltaV); 
                    _container.MoveControlCollectionByDisplacement(deltaH, deltaV, this);  
                }
                trackPosition = e.GetPosition(null);
            }
        } 

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            canShowMenu = true;
        }
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            canShowMenu = false;
        }
        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (_container.MouseIsInContainer)
            {
                e.GetPosition(null);

                if (canShowMenu && !IsDeleted)
                {
                    _container.ShowContentMenu(this, sender, e);
                }
            }
        }


        void ruleChange()
        {
            if (isRuleMoved && DirectionChanged != null)
                DirectionChanged(this);
        }
    }
}
