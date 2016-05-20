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

namespace BP.Controls
{
    [TemplatePart(Name = "btnShowList", Type = typeof(Button))]

    [TemplatePart(Name = "TabPanelTop", Type = typeof(TabPanel))]

    [TemplatePart(Name = "HeadList", Type = typeof(TabPanel))]
    public class TabControlEx : TabControl
    {
        private static Button btnShowList;

        private static TabPanel itemPanel;

        private static Popup HeadList;

        public TabControlEx()
        {
          //  this.DefaultStyleKey = typeof(TabControlEx);
        }
        public override void OnApplyTemplate()
        {

            base.OnApplyTemplate();

            btnShowList = (GetTemplateChild("btnShowList") as Button);

            itemPanel = (GetTemplateChild("TabPanelTop") as TabPanel);

            HeadList = (GetTemplateChild("HeadList") as Popup);

            btnShowList.Click += new RoutedEventHandler(btnShowList_Click);

            this.SizeChanged += new SizeChangedEventHandler(EosTabControl_SizeChanged);

            itemPanel.SizeChanged += new SizeChangedEventHandler(itemPanel_SizeChanged);

            btnShowList.LostFocus += new RoutedEventHandler(btnShowList_LostFocus);

            btnShowList.MouseEnter += new MouseEventHandler(btnShowList_MouseEnter);

            btnShowList.MouseLeave += new MouseEventHandler(btnShowList_MouseLeave);

            this.MouseLeftButtonDown += new MouseButtonEventHandler(EosTabControl_MouseLeftButtonDown);

        }



        void btnShowList_MouseLeave(object sender, MouseEventArgs e)
        {

            if (HeadList.IsOpen == false)
            {

                btnShowList.Content = "/WF;component/Themes/img1.png";

            }

        }



        void btnShowList_MouseEnter(object sender, MouseEventArgs e)
        {

            if (HeadList.IsOpen == false)
            {

                btnShowList.Content = "/WF;component/Themes/img2.png";

            }

        }



        void EosTabControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(this.SelectedItem!=null)
            (this.SelectedItem as TabItem).Focus();

        }



        void btnShowList_LostFocus(object sender, RoutedEventArgs e)
        {

            HeadList.IsOpen = false;

            btnShowList.Content = "/WF;component/Themes/img1.png";

        }



        void btnShowList_Click(object sender, RoutedEventArgs e)
        {

            if (HeadList.IsOpen == false)
            {

                double maxWidth = 0;

                ListBox headContent = new ListBox();

                headContent.BorderThickness = new Thickness(3);

                //headContent.ItemsSource = itemPanel.Children;
                foreach (TabItemEx eosItem in itemPanel.Children)
                {
                 
                    headContent.Items.Add(eosItem.Title);

                    if (eosItem.ActualWidth > maxWidth)
                    {

                        maxWidth = eosItem.ActualWidth;

                    }

                }

                headContent.SelectionChanged += new SelectionChangedEventHandler(headContent_SelectionChanged);



                HeadList.Child = headContent;

                HeadList.HorizontalOffset = this.ActualWidth - maxWidth + 24;

                HeadList.VerticalOffset = 0;

                HeadList.IsOpen = true;

                btnShowList.Content = "/WF;component/Themes/img1.png";

            }

            else
            {
                if(this.SelectedItem!=null)
                (this.SelectedItem as TabItem).Focus();

            }

        }



        void EosTabControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (this.ActualWidth > 0)
            {

                double maxWidth = this.ActualWidth;

                double itemsWidth = 30;

                double itemMaxWidth = 0;

                foreach (TabItem eosItem in itemPanel.Children)
                {

                    itemsWidth += eosItem.ActualWidth;

                    if (eosItem.ActualWidth > itemMaxWidth)
                    {

                        itemMaxWidth = eosItem.ActualWidth;

                    }

                    if (itemsWidth < maxWidth)
                    {

                        eosItem.Visibility = Visibility.Visible;

                    }

                    else
                    {

                        eosItem.Visibility = Visibility.Collapsed;

                    }

                }
                if(itemPanel.Children.Count>0)
                if (itemPanel.Children[0].Visibility == Visibility.Collapsed)
                {

                    (itemPanel.Children[0] as TabItem).Width = maxWidth - 40;

                    itemPanel.Children[0].Visibility = Visibility.Visible;

                }



                HeadList.HorizontalOffset = this.ActualWidth - itemMaxWidth + 24;

            }

        }



        void itemPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (this.ActualWidth > 0)
            {

                double maxWidth = this.ActualWidth;

                double itemsWidth = 30;

                double itemMaxWidth = 0;

                //foreach (TabItem eosItem in itemPanel.Children)
                //{

                //    itemsWidth += eosItem.ActualWidth;

                //    if (eosItem.ActualWidth > itemMaxWidth)
                //    {

                //        itemMaxWidth = eosItem.ActualWidth;

                //    }

                //    if (itemsWidth < maxWidth)
                //    {

                //        eosItem.Visibility = Visibility.Visible;

                //    }

                //    else
                //    {

                //        eosItem.Visibility = Visibility.Collapsed;

                //    }

                //}

                for (int i = itemPanel.Children.Count - 1; i >= 0; i--)
                {
                    TabItem eosItem = itemPanel.Children[i] as TabItem;

                    itemsWidth += eosItem.ActualWidth;
                    if (eosItem.ActualWidth > itemMaxWidth)
                    {

                        itemMaxWidth = eosItem.ActualWidth;

                    }

                    if (itemsWidth < maxWidth)
                    {

                        eosItem.Visibility = Visibility.Visible;

                    }

                    else
                    {

                        eosItem.Visibility = Visibility.Collapsed;

                    }

                }
                if (itemPanel.Children.Count > 0)
                    //if (itemPanel.Children[0].Visibility == Visibility.Collapsed)
                    //{

                    //    (itemPanel.Children[0] as TabItem).Width = maxWidth - 40;

                    //    itemPanel.Children[0].Visibility = Visibility.Visible;

                    //}



                HeadList.HorizontalOffset = this.ActualWidth - itemMaxWidth;

            }

        }



        void headContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            foreach (TabItem eosItem in itemPanel.Children)
            {

                if (this.Items.IndexOf(eosItem) == (sender as ListBox).SelectedIndex)
                {

                    if (eosItem.Visibility == Visibility.Collapsed)
                    {

                        eosItem.Visibility = Visibility.Visible;
                   
                        this.Items.Remove(eosItem);

                        this.Items.Add(eosItem);
                      

                        //this.SelectedIndex = 0;
                        this.SelectedItem = eosItem;

                        break;

                    }

                    else
                    {

                        this.SelectedIndex = this.Items.IndexOf(eosItem);

                        break;

                    }

                }

            }

            HeadList.IsOpen = false;

            (this.SelectedItem as TabItem).Focus();

            this.UpdateLayout();

        }

    }

}

