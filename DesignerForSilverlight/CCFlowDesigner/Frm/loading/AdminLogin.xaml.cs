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
using BP;
namespace WF
{
    public partial class AdminLogin : ChildWindow
    {
        
        public AdminLogin()
        {
            InitializeComponent();
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            this.OKButton.Focus();
        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
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
                    Login();
                    break;
                default:
                    break;
            }
        }

        void Login()
        {
            string user = this.textBox1.Text.Trim();
            string pass = this.passwordBox1.Password.Trim();

            var da = BP.Glo.GetDesignerServiceInstance();
            //da.DoTypeAsync("AdminLogin", "admin", "pub", null, null, null);
            da.DoTypeAsync("AdminLogin", user, pass, null, null, null);
            da.DoTypeCompleted += new EventHandler<WS.DoTypeCompletedEventArgs>((object senders, WS.DoTypeCompletedEventArgs ee) =>
            {
                if( null !=ee.Error){
                    BP.Glo.ShowException(ee.Error,"登陆错误");
                    return;
                }
                if (ee.Result != null && ee.Result.Contains("error"))
                {
                    MessageBox.Show(ee.Result, "Error", MessageBoxButton.OK);
                    return;
                }
                SessionManager.Session["UserNo"] = user;
                SessionManager.Session["SID"] = ee.Result;
                this.DialogResult = true;
            });
        }
    }
}

