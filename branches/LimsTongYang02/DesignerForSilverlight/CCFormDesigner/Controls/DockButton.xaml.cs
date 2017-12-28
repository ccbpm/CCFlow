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
    [TemplatePart(Name = "RightPanel", Type = typeof(Grid))]
    [TemplatePart(Name = "LeftPanel", Type = typeof(Grid))]
    public partial class DockButton : ToggleButton
    {
        private Grid leftPanel;
        private Grid rightPanel;

        public DockButton()
        {
            DefaultStyleKey = typeof(DockButton);

            DockDirection = DockDirection.Left;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.leftPanel = this.GetTemplateChild("LeftPanel") as Grid;
            this.rightPanel = this.GetTemplateChild("RightPanel") as Grid;

            if (DockDirection == DockDirection.Right)
            {
                this.leftPanel.Visibility = Visibility.Collapsed;
                this.rightPanel.Visibility = Visibility.Visible;
            }

        }
        /// <summary>
        /// 停靠方向
        /// </summary>
        public static readonly DependencyProperty DockDirectionProperty =
                        DependencyProperty.Register("DockDirection", typeof(DockDirection), typeof(DockButton), null);
        /// <summary>
        /// 停靠方向
        /// </summary>
        public DockDirection DockDirection
        {
            get { return (DockDirection)GetValue(DockDirectionProperty); }
            set { SetValue(DockDirectionProperty, value); }
        }
    }
}
