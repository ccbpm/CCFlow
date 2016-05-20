namespace BP
{
    using System;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Net;
    using System.Net.Browser;
    using System.Threading;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Controls.Primitives;
    using System.Windows.Browser;
    /// <summary>
    /// Main <see cref="Application"/> class.
    /// </summary>
    public partial class App : Application
    {
        public static string strColor { set; get; }
        public static string mypk { set; get; }
        public static string pkval { set; get; }
        public static int W { set; get; }
        public static int H { set; get; }
        /// <summary>
        /// App
        /// </summary>
        public App()
        {
            InitializeComponent();
        }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            #region 获取参数
            foreach (var item in e.InitParams.Keys)
            {
                if (item == "mypk")
                {
                    App.mypk = e.InitParams[item].ToString();
                }

                if (item == "pkval")
                {
                    App.pkval = e.InitParams[item].ToString();
                }

                if (item == "H")
                {
                    App.H = int.Parse(e.InitParams[item].ToString());
                }

                if (item == "W")
                {
                    App.W = int.Parse(e.InitParams[item].ToString());
                }
            }
            #endregion

            bool registerResult = WebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
            bool httpsResult = WebRequest.RegisterPrefix("https://", WebRequestCreator.ClientHttp);
            this.InitializeRootVisual();
        }
        protected virtual void InitializeRootVisual()
        {
            this.RootVisual = new SignControl(); 
        }
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
           
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                e.Handled = true;
                MessageBox.Show(e.ExceptionObject.Message);
            }
        }
    }
}