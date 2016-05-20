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
using System.ComponentModel;
namespace BP
{
   
    public partial class NodeTip : UserControl
    {
        private NodeTip()
        {
            InitializeComponent();
            this.receiver.Text = string.Empty;
            this.receiveTime.Text= string.Empty;
        }
        public NodeTip(string receiver, string time)
            : this()
        {
            this.receiver.Text = receiver;
            this.receiveTime.Text = time;
        }
        public void setReceiver(string receiver)
        {
            this.receiver.Text = receiver;
        }
        public void setReceiveTime(string time)
        {
            this.receiveTime.Text = time;
        }
    }
}
