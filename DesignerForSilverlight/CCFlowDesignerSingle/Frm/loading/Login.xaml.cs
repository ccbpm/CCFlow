using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WF
{
    public partial class Login : ChildWindow
    {
        
        public Login()
        {
            InitializeComponent();
            this.OKButton.Click += new RoutedEventHandler(Button_Click);
            this.CancelButton.Click += new RoutedEventHandler(Button_Click);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            this.OKButton.Focus();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == this.OKButton)
            {
                Entry();
            }
            else if (sender == this.CancelButton)
            {
                this.DialogResult = false;
            }
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            ChildWindow_KeyDown(this, e);
        }
        private void ChildWindow_KeyDown(object sender, KeyEventArgs e)
        {
          
            switch (e.Key)
            {
                case Key.Escape:
                    this.DialogResult = false;
                    break;
                case Key.Enter:
                    Entry();
                    break;
                default:
                    break;
            }
        }

        void Entry()
        {
            string user = this.tbName.Text.Trim();
            string pass = this.tbPwd.Password.Trim();

            var da = BP.Glo.GetDesignerServiceInstance();
            //da.DoTypeAsync("AdminLogin", "admin", "pub", null, null, null);
            da.DoTypeAsync("AdminLogin", user, pass, null, null, null);
            da.DoTypeCompleted += new EventHandler<WS.DoTypeCompletedEventArgs>((object senders, WS.DoTypeCompletedEventArgs ee) =>
            {
                if( null !=ee.Error){
                    MessageBox.Show(ee.Error.Message, "Error", MessageBoxButton.OK);
                    return;
                }
                if (ee.Result != null)
                {
                    MessageBox.Show(ee.Result, "Error", MessageBoxButton.OK);
                    return;
                }
                this.DialogResult = true;
            });
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

