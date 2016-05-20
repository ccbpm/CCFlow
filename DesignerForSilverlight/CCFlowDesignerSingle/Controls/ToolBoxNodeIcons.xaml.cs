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
using Liquid;
using System.IO;
using System.Windows.Browser;
using WF.WS;
using System.Windows.Media.Imaging;
using BP.SL;
using System.Windows.Threading;

namespace BP.Controls
{
    public partial class ToolBoxNodeIcons : UserControl
    {
       
        public const string IconDefault = "Default";
        static Dictionary<string, BitmapImage> NodeImg = new Dictionary<string, BitmapImage>();
        public BitmapImage GetIcon(string name)
        {
            if (NodeImg.ContainsKey(name))
                return NodeImg[name];
            else
            {
                name = IconDefault;
                setIcon(name, "/NodeIcon/Default.jpg");
                return NodeImg[name]; 
            }
        }
        void setIcon(string name, string imSrc)
        {
            if (!NodeImg.ContainsKey(name))
            {
                NodeImg.Add(name, new BitmapImage() { UriSource = new Uri(imSrc, UriKind.Relative) });
            }
           
        }
        static Dictionary<string, DataTemplate> nodeIcons = null;
        EventHandler<GetNodeIconFileCompletedEventArgs> GetNodeIconFileCompleted = null;


        /// <summary>
        /// 在本对象 down时 =true,
        /// 自身、工具箱容器和设计器up时       =false ,
        /// 自身、工具箱容器LostFocu时未down则 =false ,
        /// 双击时 = false
        /// </summary>
        public static bool IsToolDraging;

        static ToolBoxNodeIcons instance = null;
        public static ToolBoxNodeIcons Instance
        {
            get 
            {
                if (null == instance)
                    instance = new ToolBoxNodeIcons();
                return ToolBoxNodeIcons.instance; 
            }
           
        }
        public ToolBoxNodeIcons()
        {
            InitializeComponent();

            this.SetVisiable(false);
            this.close.MouseLeftButtonDown += delegate(object sender, MouseButtonEventArgs e)
            {
                e.Handled = true;
                this.SetVisiable(false);
            };

            initIconSources();

            this.MouseRightButtonDown += _MouseRightButtonDown;

        }

        #region 
        public void SetDefault()
        {
            IsToolDraging = false;
            MainPage.NewElementNameOrIcon = string.Empty;
            MainPage.Instance.SetSelectedTool("Arrow");
        }
       
    
        void lbTools_MouseEnter(object sender, MouseEventArgs e)
        {
        }
       
        // 图标的单击事件迁移到容器的单击事件
        void lbTools_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            IsToolDraging = true;

            if (Glo.IsDbClick)
            {
                string name = (this.lbTools.SelectedValue as NodeIcon).Name;
                MainPage.Instance.CreateAcitivty(FlowNodeType.Ordinary, name);
            }
            else
            {// ToolboxItem 拖拽元素
            }
        }

        void lbTools_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            SetDefault();
        }

        void lbTools_MouseLeave(object sender, MouseEventArgs e)
        {
            if (ToolBoxNodeIcons.IsToolDraging)
            {
                string name = (this.lbTools.SelectedValue as NodeIcon).Name;

                MainPage.NewElementNameOrIcon = name;
            }
            else
            {
                SetDefault();
            }
        }


        #endregion

        void initIconSources()
        {
            if (null == nodeIcons || 0 < nodeIcons.Count)
            {
                nodeIcons = new Dictionary<string, DataTemplate>();
                WSDesignerSoapClient ws = Glo.GetDesignerServiceInstance();
                ws.GetNodeIconFileAsync();
                GetNodeIconFileCompleted = (object sender, GetNodeIconFileCompletedEventArgs e) =>
                {
                    ws.GetNodeIconFileCompleted -= GetNodeIconFileCompleted;
                    if (null != e.Error || null == e.Result || e.Result.Length < 0 )
                    {
                        return;
                    }
                    initToolBox(e.Result);
                };
                ws.GetNodeIconFileCompleted += GetNodeIconFileCompleted;
            }
        }

        List<NodeIcon> icons;
        public List<NodeIcon> Icons
        {
            get { return icons; }
        }
        void initToolBox(string [] iconNames)
        {

            Uri bgUri = HtmlPage.Document.DocumentUri;
            string url = bgUri.AbsoluteUri.Substring(0, bgUri.AbsoluteUri.IndexOf(bgUri.AbsolutePath) + 1);
            url = url + "ClientBin/NodeIcon/";

            icons = new List<NodeIcon>();
            foreach (var item in iconNames)
            {
                string name = item.Replace("\\", "");
                string srcRelative = "/NodeIcon/" + name + ".png";
                NodeIcon icon = new NodeIcon()
                { Cursor = Cursors.Hand };
                icon.Name = name;
                Uri imageUri = new Uri(srcRelative, UriKind.Relative);
                icon.img.Source = new BitmapImage(imageUri);
                icon.txtLink.Text = name;
                icons.Add(icon);
                setCursorTemplate(name, srcRelative);
            }
           
            this.lbTools.ItemsSource = icons;
            this.lbTools.SelectionMode = SelectionMode.Single;
            this.lbTools.AddHandler(ListBox.MouseLeftButtonDownEvent
                , new MouseButtonEventHandler(lbTools_MouseLeftButtonDown), true);
            this.lbTools.MouseLeftButtonUp += lbTools_MouseLeftButtonUp;
            this.lbTools.MouseEnter += lbTools_MouseEnter;
            this.lbTools.MouseLeave +=  lbTools_MouseLeave;

            this.lbTools.MouseRightButtonDown += _MouseRightButtonDown;

        }

       

        /// <summary>
        /// 获取指定图标名称的鼠标指针图标
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DataTemplate GetCursorTemplate(string name)
        {

            if (nodeIcons.ContainsKey(name))
                return nodeIcons[name];
            else
                return setCursorTemplate();
        }
        DataTemplate setCursorTemplate(string name = IconDefault, string imSrc = "/NodeIcon/Default.jpg")
        {
            setIcon(name, imSrc);
           
            if (nodeIcons.ContainsKey(name))
                return nodeIcons[name];
            else
            {
                string xaml = @"
                        <DataTemplate 
                            x:Key='{0}' 
                            xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                            xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                            <Image Width='35' Height='35' Source='{1}' Stretch='UniformToFill' >
                                <Image.RenderTransform>
                                    <TranslateTransform X='-0' Y='-0' />
                                </Image.RenderTransform>
                            </Image>
                        </DataTemplate>";
                xaml = string.Format(xaml, name, imSrc);

                DataTemplate template = (DataTemplate)System.Windows.Markup.XamlReader.Load(xaml);
                nodeIcons.Add(name, template);
                return template;
            }
        }


        private double left = 10, top = 35;
        public bool isOpened;
        public void SetVisiable(bool flag)
        {
            this.LayoutRoot.IsOpen = flag;
            isOpened = flag;
        }



        #region 悬浮工具箱拖动
        void Toolbar_OnMouseEnter(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            //element.Cursor = Cursors.Hand;

            Border b;
            if (typeof(Border).IsInstanceOfType(element))
            {
                b = element as Border;
            }
            else
            {
                b = element.Parent as Border;
            }

            if (b != null)
            {
                b.BorderBrush = new SolidColorBrush(ColorConverter.ToColor("#FFABCEFB"));
                b.Background = new SolidColorBrush(ColorConverter.ToColor("#FFD5E2F2"));//#FFFFE1
            }
        }
        void Toolbar_OnMouseLeave(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;
            element.Cursor = null;

            Border b;
            if (typeof(Border).IsInstanceOfType(element))
            {
                b = element as Border;
            }
            else
            {
                b = element.Parent as Border;
            }

            if (b != null)
            {
                b.BorderBrush = new SolidColorBrush(Colors.Transparent);
                b.Background = new SolidColorBrush(Colors.Transparent);
            }
        }
      

        public bool isToolBoxCanDrag = false;
        Point pointFrom;
        void gridTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (isToolBoxCanDrag)
            {
                FrameworkElement element = this as FrameworkElement;

                double
                    x = Canvas.GetLeft(element),
                    y = Canvas.GetTop(element);

                Point pointTo = e.GetPosition(null);

                x += pointTo.X - pointFrom.X; //xOffset;
                y += pointTo.Y - pointFrom.Y;//yOffset; 

                Canvas container = this.Parent as Canvas;
                if ((this.left < x && this.top < y)
                   && (x < container.ActualWidth - this.ActualWidth + this.left && y < container.ActualHeight - this.ActualHeight - 110 - this.top)
                )
                {
                    Canvas.SetTop(element, y);
                    Canvas.SetLeft(element, x);
                }
                else
                {
                    element.Cursor = Cursors.IBeam;
                }

                pointFrom = pointTo;

            }
        }
        void gridTitle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isToolBoxCanDrag = false;
            (sender as FrameworkElement).ReleaseMouseCapture();
        }
        void gridTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isToolBoxCanDrag = true;
            (sender as FrameworkElement).CaptureMouse();
             pointFrom = e.GetPosition(null);
        }
        private void gridTitle_LostFocus(object sender, RoutedEventArgs e)
        {
            gridTitle_MouseLeftButtonUp(sender, null);
        }
        #endregion

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {          
            this.sp.Height = this.Height;
        }
        private void _MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

    }
}

