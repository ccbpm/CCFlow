using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Liquid;

namespace BP
{
    public partial class ContainerMenu : UserControl
    {
        private FlowNode flowNode = null;

        public ContainerMenu()
        {
            InitializeComponent();
        }
         
        Container _container;
        public Container Container
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
        public Point CenterPoint
        {
            get
            {
                return new Point((double)this.GetValue(Canvas.LeftProperty), (double)this.GetValue(Canvas.TopProperty));
            }
            set
            {
                // 调整x,y 值 ，以防止菜单被遮盖住
                var x = value.X;
                var y = value.Y;
                var menuHeight = 420;
                var menuWidth = 170;
                var hostWidth = Application.Current.Host.Content.ActualWidth - 250;
                var hostHeight = Application.Current.Host.Content.ActualHeight;
                if (x + menuWidth > hostWidth)
                {
                    x = x - (x + menuWidth - hostWidth);
                }
                if (y + menuHeight > hostHeight)
                {
                    y = y - (y + menuHeight - hostHeight);
                }
                this.SetValue(Canvas.TopProperty, y);
                this.SetValue(Canvas.LeftProperty, x);
            }
        }
        public void ShowMenu()
        {
            this.Visibility = Visibility.Visible;
            MuContentMenu.Show();
        }

        private void Menu_ItemSelected(object sender, MenuEventArgs e)
        {
            if (e.Tag == null)
                return;

            switch (e.Tag.ToString())
            {
                case "ExceptionLog":
                    BP.SL.OutputChildWindow.ShowException();
                    break;
                case "Help":
                    BP.Glo.OpenHelp();
                    break;
                case "menuExp":
                    BP.Frm.FrmExp exp = new BP.Frm.FrmExp();
                    exp.Show();
                    break;
                case "menuImp":
                    BP.Frm.FrmImp imp = new BP.Frm.FrmImp();
                    imp.Show();
                    break;
                case "menuFullScreen":
                    Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
                    break;
                case "menuAddNode":
                    Container.CreateFlowNode(this.CenterPoint);
                    break;
                case "menuAddLine":
                    this.Container.CreateDirection(this.CenterPoint, DirType.Forward);
                    break;
                case "menuAddLabel":
                    Container.CreateLabel(CenterPoint);
                    break;
                case "menuFlowPropertity":
                    BP.Glo.OpenWinByDoType("CH", BP.UrlFlag.FlowP, _container.FlowID, null, null);
                
                    break;
                case "menuRunFlow":
                   BP.Glo.OpenWinByDoType("CH", BP.UrlFlag.RunFlow, _container.FlowID, null, null);
                    break;
                case "menuCheckFlow":
                   BP.Glo.OpenWinByDoType("CH", BP.UrlFlag.FlowCheck, _container.FlowID, null, null);
                    break;
                case "menuFlowDefination":
                   
                    BP.Glo.OpenWinByDoType("CH", BP.UrlFlag.WFRpt, _container.FlowID, null, null);
                    break;
                case "menuDelete": // 删除流程。
                    if (System.Windows.Browser.HtmlPage.Window.Confirm("您确定要删除吗？"))
                    {
                        _container.DeleteSeleted();
                        _container.SaveChange(HistoryType.New);
                        _container.IsNeedSave = true;
                    }

                    break;
                case "MenuDisplayHideGrid":
                    Liquid.MenuItem item = e.Parameter as Liquid.MenuItem;
                    if (_container.GridLinesContainer.Children.Count > 0)
                    {
                        _container.GridLinesContainer.Children.Clear();
                        item.IsChecked = false;
                    }
                    else
                    {
                        bool isshow = item.IsChecked == true ? false : true ;
                        item.IsChecked = isshow;
                        _container.SetGridLines(isshow);
                    }
                    break;
            }
            MuContentMenu.Hide();
        }

        private void menu_MouseLeave(object sender, MouseEventArgs e)
        {
            (sender as Menu).Hide();
        }

        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

    }
}
