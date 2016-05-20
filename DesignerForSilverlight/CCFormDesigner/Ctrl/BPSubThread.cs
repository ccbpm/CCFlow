using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CCForm
{
    public class BPSubThread : UCExt, IDelete
    {
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(base.Name))
                {
                    base.Name = MainPage.Instance.GenerElementNameFromUI(this);
                }

                return base.Name.Replace("SF", "");
            }
            set
            {
                base.Name = "SF" + value;
            }
        }
        #region
        public override bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                base.IsSelected = value;
                if (value)
                {
                    selected();
                }
                else
                {
                    canChildrenMove = false;
                }
            }
        }

        public virtual bool IsCanReSize { get; set; }
        #endregion


        protected override void OnMouseMove(MouseEventArgs e)
        {
            BPSubThreadMove();
            base.OnMouseMove(e);
        }
        public Grid MyDG;
        /// <summary>
        /// 新建控件时调用
        /// </summary>
        public BPSubThread()
        {
            this.IsSelected = false;
            this.isCanReSize = true;

            new Adjust().Bind(this);
            this.BindDrag();

            this.Foreground = new SolidColorBrush(Colors.Green);
            this.FontStyle = FontStyles.Normal;
            // 填充
            Grid grid = CreateControls();
            MyDG = grid;

            grid.Name = "SF" + this.Name;
            this.layout.Child = grid;
            this.SizeChanged += (object sender, SizeChangedEventArgs e) =>
            {
                this.Width = this.Width;
                this.Height = this.Height;
                if (MyDG != null)
                {
                    if (!Convert.IsDBNull(this.Width))
                        MyDG.Width = this.Width;
                    if (!Convert.IsDBNull(this.Height))
                        MyDG.Height = this.Height;
                }
            };
            this.Width = 600;
            this.Height = 300;
        }

        /// <summary>
        /// 加载现有控件时使用，Name从数据库获取
        /// </summary>
        /// <param name="name"></param>
        public BPSubThread(string name)
            : this()
        {
            this.Name = name;
        }
        /// <summary>
        /// 创建控件.
        /// </summary>
        /// <returns></returns>
        private Grid CreateControls()
        {
            string xaml = @"<Grid Background='Transparent' ShowGridLines='True'
        xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'  
        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
        xmlns:sdk='http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk'>
        <Grid.RowDefinitions>
            <RowDefinition Height='20'></RowDefinition>
            <RowDefinition Height='*'></RowDefinition>
        </Grid.RowDefinitions>

        <Border Background='LightGray' Grid.Row='0'>
            <TextBlock>子线程</TextBlock>
        </Border>
        <StackPanel Grid.Row='1' VerticalAlignment='Center' HorizontalAlignment='Center'>
            <TextBlock >提示:</TextBlock>
            <TextBlock >1,可以使用aswd键改变位置，鼠标移动到边框，按住左键调整大小。</TextBlock>
            <TextBlock >2,只能在节点表单中添加。</TextBlock>
            <TextBlock >3,只能有一个控件，可以使用节点表单的审核功能。</TextBlock>
            <TextBlock >4,该组件的属性，点右键，点编辑菜单就可以弹出，也可以在节点属性里配置。</TextBlock>
            <TextBlock >5,属性里可以控制该组件的，状态、样式等等。</TextBlock>
            <TextBlock >6,按下delete键，可以删除该控件。</TextBlock>
        </StackPanel>
    </Grid>";

            Grid grid = (Grid)System.Windows.Markup.XamlReader.Load(xaml);
            return grid;
        }
        public void UpdatePos()
        {
            OnMouseMove(null);
        }

        List<FrameworkElement> selectedElements = new List<FrameworkElement>();

        bool canChildrenMove = false;
        public void selected()
        {
            Canvas workSpace = this.Parent as Canvas;
            if (null == workSpace) return;

            BPSubThread bpWorkCheck = this;

            start = false;
            selectedElements.Clear();

            //调整框选区自动变量 
            double finalX = double.MaxValue;
            double finalXMax = double.MinValue;
            double finalY = double.MaxValue;
            double finalYMax = double.MinValue;

            double xBegin = Convert.ToDouble(bpWorkCheck.GetValue(Canvas.LeftProperty));
            double yBegin = Convert.ToDouble(bpWorkCheck.GetValue(Canvas.TopProperty));
            double xEnd = xBegin + bpWorkCheck.ActualWidth;
            double yEnd = yBegin + bpWorkCheck.ActualHeight;

            double left = 0.0;
            double top = 0.0;
            double leftM = 0.0;
            double topM = 0.0;

            foreach (UIElement ue in workSpace.Children)
            {
                if (ue is FrameworkElement && !object.ReferenceEquals(ue, bpWorkCheck))
                {
                    FrameworkElement c = ue as FrameworkElement;

                    if (c is BPLine)
                    {// 判断有没有线
                        BPLine line = c as BPLine;
                        left = line.MyLine.X1;
                        top = line.MyLine.Y1;
                        leftM = line.MyLine.X2;
                        topM = line.MyLine.Y2;
                    }
                    else
                    {
                        left = Convert.ToDouble(c.GetValue(Canvas.LeftProperty));
                        top = Convert.ToDouble(c.GetValue(Canvas.TopProperty));
                        leftM = left + c.ActualWidth;
                        topM = top + c.ActualHeight;
                    }

                    if (xBegin < leftM && xEnd > left && yBegin < topM && yEnd > top)
                    {
                        selectedElements.Add(c);
                        if (finalX > left) finalX = left;
                        if (finalY > top) finalY = top;

                        if (finalXMax < leftM) finalXMax = leftM;
                        if (finalYMax < topM) finalYMax = topM;
                    }

                }
            }
            canChildrenMove = true;
        }

        double left = 0.0;
        double top = 0.0;
        bool start = false;
        public void BPSubThreadMove()
        {
            if (!canChildrenMove) return;


            BPSubThread rectSelected = this;
            if (!start)
            {
                left = Convert.ToDouble(rectSelected.GetValue(Canvas.LeftProperty));
                top = Convert.ToDouble(rectSelected.GetValue(Canvas.TopProperty));
                start = true;
            }
            int ind = (int)rectSelected.GetValue(Canvas.ZIndexProperty);
            foreach (var item in selectedElements)
            {
                if (ind >= (int)item.GetValue(Canvas.ZIndexProperty))
                    continue;

                if (item is BPLine)
                {
                    BPLine line = item as BPLine;
                    if (line != null)
                    {
                        line.MyLine.SetValue(Canvas.LeftProperty, Convert.ToDouble(item.GetValue(Canvas.LeftProperty)) + Convert.ToDouble(rectSelected.GetValue(Canvas.LeftProperty)) - left);
                        line.MyLine.SetValue(Canvas.TopProperty, Convert.ToDouble(item.GetValue(Canvas.TopProperty)) + Convert.ToDouble(rectSelected.GetValue(Canvas.TopProperty)) - top);
                    }
                }
                else
                {
                    item.SetValue(Canvas.LeftProperty, Convert.ToDouble(item.GetValue(Canvas.LeftProperty)) + Convert.ToDouble(rectSelected.GetValue(Canvas.LeftProperty)) - left);
                    item.SetValue(Canvas.TopProperty, Convert.ToDouble(item.GetValue(Canvas.TopProperty)) + Convert.ToDouble(rectSelected.GetValue(Canvas.TopProperty)) - top);
                }
            }
            left = Convert.ToDouble(rectSelected.GetValue(Canvas.LeftProperty));
            top = Convert.ToDouble(rectSelected.GetValue(Canvas.TopProperty));
        }
        /// <summary>
        /// 删除它
        /// </summary>
        public void DeleteIt()
        {
            if (MessageBox.Show("您确定要删除[" + this.Name + "]吗？", "删除提示", MessageBoxButton.OKCancel)
                == MessageBoxResult.Cancel)
                return;

            /* 更新审核组件状态. */
            string sql = "UPDATE WF_Node SET SFSta=0 WHERE NodeID=" + Glo.NodeID;
            FF.CCFormSoapClient da = Glo.GetCCFormSoapClientServiceInstance();
            da.RunSQLAsync(sql, Glo.UserNo, Glo.SID);
            da.RunSQLCompleted += (object sender, FF.RunSQLCompletedEventArgs e) =>
            {
                this.ViewDeleted = false;
                if (e.Error != null)
                {
                    MessageBox.Show("删除错误" + e.Error.Message);
                    return;
                }

                Glo.Remove(this);
            };
        }
        /// <summary>
        /// 隐藏它
        /// </summary>
        public void HidIt()
        {
            string sql = "UPDATE WF_Node SET SFSta=0 WHERE NodeID=" + Glo.NodeID;
            FF.CCFormSoapClient hidDA = Glo.GetCCFormSoapClientServiceInstance();
            hidDA.RunSQLsAsync(sql, Glo.UserNo, Glo.SID);
            hidDA.RunSQLsCompleted += (object sender, FF.RunSQLsCompletedEventArgs e) =>
            {
                this.Visibility = System.Windows.Visibility.Collapsed;
            };
        }
    }
}
