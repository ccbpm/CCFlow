using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Browser;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using System.Windows.Browser;
using System.IO;
using Silverlight;
using BP;
using WF.Designer;

namespace BP
{
    public partial class App : Application
    {
        public App()
        {
            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;
            InitializeComponent();
        }
        /// <summary>
        /// Application_Startup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool registerResult = WebRequest.RegisterPrefix("http://", WebRequestCreator.ClientHttp);
            bool httpsResult = WebRequest.RegisterPrefix("https://", WebRequestCreator.ClientHttp);

            IDictionary<string, string> pars = e.InitParams;
            try
            {
                Glo.AppName = pars["appName"];
            }
            catch
            { 
            }

            if (string.IsNullOrEmpty(Glo.AppName ) || Glo.AppName == "null")
                Glo.AppName = string.Empty;

            //设置当前线程的culture,以加载指定语言的字符
            var culture = new CultureInfo("zh-cn");
            Thread.CurrentThread.CurrentUICulture = culture;

            UIElement ui = null;

            if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("WorkID")
             || System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FK_Flow"))
            {
                var workId = string.Empty;
                var fk_flow = string.Empty;
                var queryString = System.Windows.Browser.HtmlPage.Document.QueryString;
                if (queryString.ContainsKey("WorkID"))
                    workId = queryString["WorkID"];

                if (queryString.ContainsKey("FK_Flow"))
                    Glo.FK_Flow = fk_flow = queryString["FK_Flow"];

                if (queryString.ContainsKey("FID"))
                {
                    string fid = queryString["FID"];
                    if (string.IsNullOrEmpty(fid) == false && fid != "0")
                        workId = fid;
                }

                if (queryString.ContainsKey("UserNo"))
                    Glo.UserNo = queryString["UserNo"];

                if (queryString.ContainsKey("SID"))
                    Glo.SID = queryString["SID"];

                if (string.IsNullOrEmpty(workId) == false)
                    ui = new BP.Viewer(fk_flow, workId);
                else
                    ui = MainPage.Instance;
            }
            else
            {
                ui = MainPage.Instance;
            }

            this.RootVisual = ui;
        }
        private void Application_Exit(object sender, EventArgs e)
        {
        }
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // 如果应用程序是在调试器外运行的，则使用浏览器的
            // 异常机制报告该异常。在 IE 上，将在状态栏中用一个 
            // 黄色警报图标来显示该异常，而 Firefox 则会显示一个脚本错误。
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // 注意: 这使应用程序可以在已引发异常但尚未处理该异常的情况下
                // 继续运行。 
                // 对于生产应用程序，此错误处理应替换为向网站报告错误
                // 并停止应用程序。
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
            else
            {
                Glo.ShowException(e.ExceptionObject);
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
            errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

            string alert = "请您按如下方式处理这个错误。";
            alert += "\t\n1，请按F5刷新一次本网页。";
            alert += "\t\n2，请将iis重新启动一下，用administrator进入服务器,在cmd执行iisreset之后刷新网页。";
            alert += "\t\n3，如果是第一次使用，请打开安装文件中有常见的问题,此文件位于D:\\ccflow\\docs\\.";
            alert += "\t\n4，进入官方网站(http://ccflow.org)加入QQ群，获得更多的ccflow爱好者帮助。";
            alert += "\t\n5，把此屏幕copy一个图片(一定是全屏)，发送到 ccflow@ccflow.org 或http://bbs.ccflow.org 我们会有回复。";
            alert += "\t\n6，请baidu或者google一下 ccflow 常见问题，也许可以找到答案。";
            alert += "\t\n";

            Glo.ShowException(e.ExceptionObject, errorMsg);
        }
       
    }
}
