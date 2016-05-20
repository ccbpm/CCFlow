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
using BP;

namespace BP
{
    public partial class LabelMenu : UserControl
    {
        IContainer _container;
        public IContainer Container
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
                var menuHeight = 100;
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
        public void ApplyCulture()
        {
        }
        public LabelMenu()
        {
            InitializeComponent();

        }
         
        bool isMultiControlSelect = false;

        public void ShowMenu()
        {
            isMultiControlSelect = false;

            if (_container.SelectedWFElements != null
                && _container.SelectedWFElements.Count > 0
                )
            {
                if (!_container.SelectedWFElements.Contains(relatedLabel))
                {
                    _container.ClearSelected(null);
                    isMultiControlSelect = false;
                }
                else
                {
                    isMultiControlSelect = true;
                }
            }
            else
            {
                isMultiControlSelect = false;
            }

            this.Visibility = Visibility.Visible;
            MuContentMenu.Show();
        }
        NodeLabel relatedLabel;
        public NodeLabel RelatedLabel
        {
            get
            {
                return relatedLabel;
            }
            set
            {
                relatedLabel = value;
            }
        }
      
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            MuContentMenu.Hide();
        }

        private void menuModifyLabel_Click()
        {
            this.Visibility = Visibility.Collapsed;
            _container.UpdateSelectedToMemory(relatedLabel);
            
            _container.SaveChange(HistoryType.New);
        }

        private void menuDeleteLabel_Click()
        {
            this.Visibility = Visibility.Collapsed;
            if (isMultiControlSelect && _container.SelectedWFElements != null
                && _container.SelectedWFElements.Count > 0)
            {
                IElement iel;
                foreach (System.Windows.Controls.Control c in _container.SelectedWFElements)
                {
                    iel = c as IElement;
                    if (iel != null)
                    {
                        iel.Delete();
                    }
                }
            }
            relatedLabel.Delete();
            _container.SaveChange(HistoryType.New);
            _container.IsNeedSave = true;



        }
        private void Menu_ItemSelected(object sender, MenuEventArgs e)
        {
            if (e.Tag == null)
            {
                return;
            }

            switch (e.Tag.ToString())
            {
                case "menuModifyLabel":
                    menuModifyLabel_Click();
                    break;
                case "menuDeleteLabel":
                    menuDeleteLabel_Click();
                    break;
            }

            MuContentMenu.Hide();
        }
    }
}
