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
using BP.Controls;

namespace WF.Frm
{
    public partial class NodeIconUpdate : ChildWindow
    {
        
        static NodeIconUpdate instance = null;
        public static NodeIconUpdate Instance
        {
            get 
            {
                if (null == instance)
                    instance = new NodeIconUpdate();
                return instance;
            }
        }
        BP.FlowNode node;

        private NodeIconUpdate()
        {
            InitializeComponent(); 
            this.MouseRightButtonDown += BP.Glo.Element_MouseRightButtonDown;

            List<NodeIcon> icons = new List<NodeIcon>();
            icons = ToolBoxNodeIcons.Instance.Icons;
            this.lbTools.AddHandler(ListBox.MouseLeftButtonDownEvent
                , new MouseButtonEventHandler(lbTools_MouseLeftButtonDown), true);
            //this.lbTools.IsSynchronizedWithCurrentItem = true;
            this.lbTools.ItemsSource = icons;
            this.lbTools.SelectedIndex = -1;
            this.lbTools.SelectedValue = null;
            this.lbTools.SelectedItem = null;
            this.lbTools.SelectionMode = SelectionMode.Single;
        }
      
        public void  SetSelected(BP.FlowNode node)
        {
            try
            {
                this.node = node;

                List<NodeIcon> icons = this.lbTools.ItemsSource as List<NodeIcon>;
                foreach (var item in icons)
                {
                    if (node.Icon.Equals(item.Name))
                    {
                        this.lbTools.SelectedValue = item;
                        lbTools_MouseLeftButtonDown(this.lbTools, null);
                        break;
                    }
                }
                this.Show();
            }
            catch (Exception e)
            {
                BP.Glo.ShowException(e);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == this.OKButton)
            {
                if (this.lbTools.SelectedValue != null)
                {
                    this.node.IsIconNeedUpdate = true;
                    this.node.Icon = (this.lbTools.SelectedValue as NodeIcon).Name;
                    this.DialogResult = true;
                }
                else this.DialogResult = false;

            }
            else if (sender == this.CancelButton)
            {
                this.DialogResult = false;
            }
        }

        private void lbTools_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if( null != e)
                e.Handled = true;
        }
    }
}

