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
    [TemplatePart(Name = "LeftButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "LeftPanel", Type = typeof(Grid))]
    [TemplatePart(Name = "RightButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "RightPanel", Type = typeof(Grid))]
    public partial class DockPanel : HeaderedItemsControl
    {
        private ToggleButton leftButton;
        private Grid leftPanel;
        private ToggleButton rightButton;
        private Grid rightPanel;


        public DockPanel()
        {
            DefaultStyleKey = typeof(DockPanel);

            DockDirection = DockDirection.Left;
            IsExpanded = true;
        }

        public override void OnApplyTemplate()
        {
            this.leftButton = this.GetTemplateChild("LeftButton") as ToggleButton;
            if (this.leftButton != null)
                this.leftButton.Click += new RoutedEventHandler(LeftRight_Click);

            this.leftPanel = this.GetTemplateChild("LeftPanel") as Grid;

            this.rightButton = this.GetTemplateChild("RightButton") as ToggleButton;
            if (this.rightButton != null)
                this.rightButton.Click += new RoutedEventHandler(LeftRight_Click);

            this.rightPanel = this.GetTemplateChild("RightPanel") as Grid;

            if (DockDirection == DockDirection.Right)
            {
                this.leftButton.Visibility = Visibility.Collapsed;
                this.rightButton.Visibility = Visibility.Visible;
                this.rightPanel.Visibility = Visibility.Visible;
                this.leftPanel.Visibility = Visibility.Collapsed;
            }

            if (!IsExpanded)
            {
                Collapse();
                leftButton.IsChecked = true;
                rightButton.IsChecked = true;
            }
            base.OnApplyTemplate();
        }

        void LeftRight_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton leftRight = sender as ToggleButton;

            if ((bool)leftRight.IsChecked)
            {
                Collapse();
            }
            else
            {
                Expand();
            }
        }

        private void Expand()
        {
            if (DockDirection == DockDirection.Left)
            {
                leftButton.HorizontalAlignment = HorizontalAlignment.Right;
                leftPanel.Visibility = Visibility.Visible;
            }
            else
            {
                rightPanel.Visibility = Visibility.Visible;
                rightButton.HorizontalAlignment = HorizontalAlignment.Left;
            }

            IsExpanded = true;
        }

        private void Collapse()
        {
            if (DockDirection == DockDirection.Left)
            {
                leftPanel.Visibility = Visibility.Collapsed;
                leftButton.HorizontalAlignment = HorizontalAlignment.Left;
            }
            else
            {
                rightPanel.Visibility = Visibility.Collapsed;
                rightButton.HorizontalAlignment = HorizontalAlignment.Right;
            }

            IsExpanded = false;
        }

        /// <summary>
        /// 停靠方向
        /// </summary>
        public static readonly DependencyProperty DockDirectionProperty =
                        DependencyProperty.Register("DockDirection", typeof(DockDirection), typeof(DockPanel), null);
        /// <summary>
        /// 停靠方向
        /// </summary>
        public DockDirection DockDirection
        {
            get { return (DockDirection)GetValue(DockDirectionProperty); }
            set { SetValue(DockDirectionProperty, value); }
        }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
                        DependencyProperty.Register("IsExpanded", typeof(bool), typeof(DockPanel), new PropertyMetadata(OnIsExpandedPropertyChanged));
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        private static void OnIsExpandedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DockPanel p = d as DockPanel;

            if (p.leftButton == null)
                return;
            if ((bool)e.NewValue)
            {
                p.Expand();
            }
            else
            {
                p.Collapse();
            }
        }
    }
}
