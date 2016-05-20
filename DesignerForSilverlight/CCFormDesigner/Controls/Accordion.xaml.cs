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
using System.Windows.Controls.Primitives;

namespace Demo.Controls
{
    //窗口停靠方向
    public enum DockDirection
    {
        Left,       //居左
        Right,      //居右
    }

    [TemplatePart(Name = "RightButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "UpDownButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "LeftButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "UpDownButton2", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "rectangle", Type = typeof(Rectangle))]
    [TemplatePart(Name = "rectangle1", Type = typeof(Rectangle))]
    [TemplatePart(Name = "ContentGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "ContentPanel", Type = typeof(Grid))]
    public partial class Accordion : ContentControl
    {
        private ToggleButton rightButton;
        private ToggleButton upDownButton;
        private ToggleButton leftButton;
        private ToggleButton upDownButton2;
        private Rectangle rectangle;
        private Rectangle rectangle1;
        private Grid contentGrid;
        private Grid contentPanel;

        public Accordion()
        {
            this.DefaultStyleKey = typeof(Accordion);
            DockDirection = DockDirection.Right;
            DockVisibility = Visibility.Visible;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.contentGrid = this.GetTemplateChild("ContentGrid") as Grid;
            this.contentPanel = this.GetTemplateChild("ContentPanel") as Grid;

            this.rightButton = this.GetTemplateChild("RightButton") as ToggleButton;
            if (this.rightButton != null)
                this.rightButton.Click += new RoutedEventHandler(LeftRight_Click);

            this.upDownButton = this.GetTemplateChild("UpDownButton") as ToggleButton;
            if (this.upDownButton != null)
                this.upDownButton.Click += new RoutedEventHandler(UpDown_Click);

            this.leftButton = this.GetTemplateChild("LeftButton") as ToggleButton;
            if (this.leftButton != null)
                this.leftButton.Click += LeftRight_Click;

            this.upDownButton2 = this.GetTemplateChild("UpDownButton2") as ToggleButton;
            if (this.upDownButton2 != null)
                this.upDownButton2.Click += new RoutedEventHandler(UpDown_Click);

            this.rectangle = this.GetTemplateChild("rectangle") as Rectangle;
            this.rectangle1 = this.GetTemplateChild("rectangle1") as Rectangle;

            if (DockDirection == DockDirection.Left)
            {
                this.upDownButton.Visibility = Visibility.Collapsed;
                this.upDownButton2.Visibility = Visibility.Visible;
                this.leftButton.Visibility = Visibility.Visible;
                this.rightButton.Visibility = Visibility.Collapsed;
                this.contentGrid.Margin = new Thickness(33, 0, 0, 0);
            }
            if (DockVisibility == Visibility.Collapsed)
            {
                this.rightButton.Visibility = Visibility.Collapsed;
                this.leftButton.Visibility = Visibility.Collapsed;
                this.contentGrid.Margin = new Thickness(0);
            }
        }

        void LeftRight_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton leftRight = sender as ToggleButton;
            if ((bool)leftRight.IsChecked)
            {
                contentGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                contentGrid.Visibility = Visibility.Visible;
            }
        }

        void UpDown_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton upDown = sender as ToggleButton;
            if ((bool)upDown.IsChecked)
            {
                this.contentPanel.Visibility = Visibility.Collapsed;
                this.rectangle1.Visibility = Visibility.Visible;
                this.rectangle.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.contentPanel.Visibility = Visibility.Visible;
                this.rectangle1.Visibility = Visibility.Collapsed;
                this.rectangle.Visibility = Visibility.Visible;
            }
        }

        #region 依赖属性
        /// <summary>
        /// 窗口标题
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
                        DependencyProperty.Register("Title", typeof(string),
                            typeof(Accordion), null);
        /// <summary>
        /// 窗口标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 停靠方向
        /// </summary>
        public static readonly DependencyProperty DockDirectionProperty =
                        DependencyProperty.Register("DockDirection", typeof(DockDirection), typeof(Accordion), null);
        /// <summary>
        /// 停靠方向
        /// </summary>
        public DockDirection DockDirection
        {
            get { return (DockDirection)GetValue(DockDirectionProperty); }
            set { SetValue(DockDirectionProperty, value); }
        }

        /// <summary>
        /// 停靠按钮可见
        /// </summary>
        public static readonly DependencyProperty DockVisibilityProperty =
                        DependencyProperty.Register("DockVisibility", typeof(Visibility), typeof(Accordion), null);
        /// <summary>
        /// 停靠按钮可见
        /// </summary>
        public Visibility DockVisibility
        {
            get { return (Visibility)GetValue(DockVisibilityProperty); }
            set { SetValue(DockVisibilityProperty, value); }
        }
        #endregion
    }
}
