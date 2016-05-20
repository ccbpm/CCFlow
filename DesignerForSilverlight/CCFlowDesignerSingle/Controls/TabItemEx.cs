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

namespace BP.Controls
{
    public class TabItemEx : TabItem

    {
        public TabItemEx()
        {
           // this.DefaultStyleKey = typeof(TabItemEx);
        }
        public string Title { get; set; }
        public override void OnApplyTemplate()
        {

            base.OnApplyTemplate();
            
        }
    }
}
