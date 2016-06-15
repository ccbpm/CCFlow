#region
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Browser;
using Silverlight;
using WF.WS;
using WF;
using BP.SL;
using BP.Controls;
using System.Collections.Generic;

#endregion
namespace BP
{
    /// <summary>
    /// 平台类型
    /// </summary>
    public enum Platform
    {
        /// <summary>
        /// CCFlow
        /// </summary>
        CCFlow,
        /// <summary>
        /// JFlow
        /// </summary>
        JFlow
    }
    public class UrlFlag
    {
        /// <summary>
        /// 节点属性
        /// </summary>
        public const string NodeP = "NodeP";
        /// <summary>
        /// 节点属性
        /// </summary>
        public const string NodePNew = "NodePNew";
        /// <summary>
        /// 目录权限
        /// </summary>
        public const string FlowSortP = "FlowSortP";
        /// <summary>
        /// 流程属性
        /// </summary>
        public const string FlowP = "FlowP";
        /// <summary>
        /// 新版本
        /// </summary>
        public const string FlowPNew = "FlowPNew";
        /// <summary>
        /// 运行流程
        /// </summary>
        public const string RunFlow = "RunFlow";
        /// <summary>
        /// 流程检查
        /// </summary>
        public const string FlowCheck = "FlowCheck";
        /// <summary>
        /// 报表定义
        /// </summary>
        public const string WFRpt = "WFRpt";
        /// <summary>
        /// 设置方向条件
        /// </summary>
        public const string Dir = "Dir";
        /// <summary>
        /// 节点表单设计-傻瓜
        /// </summary>
        public const string MapDefFixModel = "MapDefFixModel";
        /// <summary>
        /// 节点表单设计-自由
        /// </summary>
        public const string MapDefFreeModel = "MapDefFreeModel";
        /// <summary>
        /// 表单设计-傻瓜
        /// </summary>
        public const string FormFixModel = "FormFixModel";
        /// <summary>
        /// 表单设计-自由
        /// </summary>
        public const string FormFreeModel = "FormFreeModel";
        /// <summary>
        /// 节点岗位
        /// </summary>
        public const string StaDef = "StaDef";
        /// <summary>
        /// 独立表单
        /// </summary>
        public const string FlowFrms = "FlowFrms";
        /// <summary>
        /// 表单库
        /// </summary>
        public const string FrmLib = "FrmLib";
    }
    /// <summary>
    /// 全局
    /// </summary>
    public class Glo
    {
        static Dictionary<string, List<SEnum>> dicEnums = new Dictionary<string, List<SEnum>>();
        public static List<SEnum> GetEnumByTypeKey(string key)
        {
            List<SEnum> lists = null;
            if (dicEnums.ContainsKey(key))
                lists = dicEnums[key];
            else
            {
                lists = new List<SEnum>();
            }
            return lists;
        }
        public static void Element_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        static CustomCursor cCursor = null;// which is used to replace the default cursor of UIElement
        public static CustomCursor CCursor
        {
            get
            {
                if (null == cCursor)
                    cCursor = new CustomCursor(MainPage.Instance);
                return cCursor;
            }
        }        /// <summary>
        ///  节点移动时是否自动调整容器大小
        /// </summary>
        public static bool IsDragNodeResizeContainer = true;
        /// <summary>
        ///  表单设计器打开方式，url 和sl子窗体模式
        /// </summary>
        public static bool UrlOrForm = false;
        public static OSModel OsModel = OSModel.OneOne;
        public static BP.Controls.TreeNode CurNodeFlow;
        public static BP.Controls.TreeNode CurNodeOrg = new TreeNode();
        public static BP.Controls.TreeNode CurrNodeForm;
        /// <summary>
        /// 临时变量.
        /// </summary>
        public static string TempCmd = null;
        /// <summary>
        ///  最后的流程类型，用于重新绑定流程树后，再打开最后操作的流程类别
        /// </summary>
        public static string FK_FlowSort = "01";
        /// <summary>
        /// 当前的流程编号
        /// </summary>
        public static string FK_Flow = null;
        public static string FK_FormSort = "01";

        #region 控件操作方法
        public static bool Ctrl_DDL_SetSelectVal(ComboBox ddl, string setVal)
        {
            string oldVal = "";
            foreach (ComboBoxItem item in ddl.Items)
            {
                if (item.IsEnabled == true)
                {
                    oldVal = item.Tag.ToString();
                    item.IsSelected = false;
                    break;
                }
            }
            foreach (ComboBoxItem item in ddl.Items)
            {
                if (item.Tag.ToString() == setVal)
                {
                    item.IsSelected = true;
                    return true;
                }
            }

            foreach (ComboBoxItem item in ddl.Items)
            {
                if (item.Tag.ToString() == oldVal)
                {
                    item.IsSelected = true;
                    break;
                }
            }
            return false;
        }
        public static void Ctrl_DDL_BindDataTable(ComboBox ddl, DataTable dt, string selectVal)
        {
            ddl.Items.Clear();
            foreach (DataRow dr in dt.Rows)
            {
                ComboBoxItem li = new ComboBoxItem();
                li.Content = dr[1].ToString();
                li.Tag = dr[0].ToString();
                if (dr[0].ToString() == selectVal)
                    li.IsSelected = true;
                ddl.Items.Add(li);
            }
        }
        public static int GetDDLValOfInt(ComboBox cb)
        {
            ComboBoxItem it = cb.SelectedItem as ComboBoxItem;
            if (it == null)
                throw new System.Exception("没有选择数据" + cb.Name);
            return int.Parse(it.Tag.ToString());
        }
        public static string GetDDLValOfString(ComboBox cb)
        {
            ComboBoxItem it = cb.SelectedItem as ComboBoxItem;
            if (it == null)
                throw new System.Exception("没有选择数据" + cb.Name);
            return it.Tag.ToString();
        }
        #endregion
            
        #region 属性
        public static string UserNo =null;
        public static string SID = null;
        public static string CustomerNo = null;
        public static string CustomerName = null;
        public static string AppCenterDBType = null;
        public static bool IsDbClick
        {
            get
            {
                return BP.SL.Glo.IsDbClick;
            }
        }
        /// <summary>
        /// 当前 BPMHost 
        /// </summary>
        private static string _BPMHost = null;
        /// <summary>
        /// 当前BPMHost 
        /// 比如:http://demo.ccflow.org:8888
        /// </summary>
        public static string BPMHost
        {
            get
            {
                if (_BPMHost != null)
                    return _BPMHost;

                try
                {
                    string myurl = System.Windows.Browser.HtmlPage.Document.DocumentUri.AbsoluteUri;

                    myurl = myurl.Replace("//", "");
                    int posStart = myurl.IndexOf("/");

                    string appPath = myurl.Substring(posStart);
                    if (appPath.Contains("/WF/Admin"))
                    {
                        appPath = appPath.Substring(0, appPath.IndexOf("/WF/Admin", StringComparison.CurrentCultureIgnoreCase));
                    }
                    else
                    {
                        appPath = appPath.Substring(0, appPath.IndexOf("/WF/", StringComparison.CurrentCultureIgnoreCase));
                    }
                    var location = (HtmlPage.Window.GetProperty("location")) as ScriptObject;
                    string host = location.GetProperty("host").ToString();
                    _BPMHost = "http://" + host + appPath;
                }
                catch (System.Exception e)
                {
                    BP.SL.LoggerHelper.Write(e);
                }
                return _BPMHost;
            }
        }
        /// <summary>
        /// 宽度
        /// </summary>
        public static int ScreenWidth
        {
            get
            {
                int width = 0;
                try
                {
                    double val = (double)HtmlPage.Window.Invoke("GetBrowserWidth");
                    width = (int)val;
                }
                catch { }

                return width <= 0 ? 1024 : width;
            }
        }
        /// <summary>
        /// 高度
        /// </summary>
        public static int ScreenHeight
        {
            get
            {
                int height = 0;
                try
                {
                    double val = (double)HtmlPage.Window.Invoke("GetBrowserHeight");
                    height = (int)val;
                }
                catch (System.Exception) { }

                return height <= 0 ? 768 : height;
            }
        }
        #endregion

        #region 获取服务
        public static string AppName="/";
        public static  Platform  Platform
        {
            get
            {
                string url = System.Windows.Browser.HtmlPage.Document.DocumentUri.OriginalString;
                if (url.ToLower().Contains("jsp")==true)
                    return BP.Platform.JFlow;
                return BP.Platform.CCFlow;
            }
        } 
        /// <summary>
        /// 是否是JFlow调用.
        /// </summary>
        public static bool IsJFlow
        {
            get
            {
                if (Glo.Platform == BP.Platform.CCFlow )
                    return false;
                return true;
            }
        }
        /// <summary>
        /// 得到WebService对象 
        /// </summary>
        /// <returns></returns>
        public static WSDesignerSoapClient GetDesignerServiceInstance()
        {
            TimeSpan ts = new TimeSpan(0, 5, 0);
            var basicBinding = new BasicHttpBinding()
            {
                ReceiveTimeout = ts,
                SendTimeout = ts,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                Name = "WSDesignerSoap"
            };
            basicBinding.Security.Mode = BasicHttpSecurityMode.None;
         
            string url="";
            if (Glo.Platform == BP.Platform.CCFlow)
            {
                url = "/WF/Admin/XAP/WebService.asmx";
                url = Glo.BPMHost + url;
            }
            else
            {
                // url = string.Format("/{0}webservices/webservice.*", AppName != string.Empty ? AppName + "/" : string.Empty);
                // url = new Uri(App.Current.Host.Source, "../").ToString() + "service/Service?wsdl";
                url = "/service/Service?wsdl";
                url = Glo.BPMHost + url;
            }
        
            url = url.Replace("//", "/");
            url = url.Replace(":/", "://");

            // MessageBox.Show(url);

            var endPoint = new EndpointAddress(url);
            var ctor =
                typeof(WSDesignerSoapClient).GetConstructor(
                new Type[] {
                    typeof(Binding), 
                    typeof(EndpointAddress)
                });
            return (WSDesignerSoapClient)ctor.Invoke(
                new object[] { basicBinding, endPoint });
        }
      
  
        #endregion

        #region 窗口调用方式

        public static void OpenHelp()
        {
            Glo.OpenWindow("http://online.ccflow.org/","");
        }
        public static void OpenGPM()
        {
            Glo.OpenWindow("http://online.ccflow.org/", "");
        }
        /// <summary>
        /// 设置打开网页窗口的属性
        /// </summary>
        /// <param name="lang">语言</param>
        /// <param name="dotype">窗口类型</param>
        /// <param name="fk_flow">工作流ID</param>
        /// <param name="node1">结点1</param>
        /// <param name="node2">结点2</param>
        public static void OpenWinByDoType(string lang, string dotype, string fk_flow, string node1, string node2)
        {
            string url = "";
            switch (dotype)
            {
                case UrlFlag.StaDef:
                    url = "/WF/Admin/XAP/DoPort.aspx?DoType=StaDef&PK=" + node1 + "&Lang=CH";
                    Glo.OpenDialog(Glo.BPMHost + url, "执行", 670, 1050);
                    return;
                case UrlFlag.FrmLib:
                    url = "/WF/Admin/XAP/DoPort.aspx?DoType=FrmLib&FK_Flow=" + fk_flow + "&FK_Node=" + node1 + "&Lang=CH";
                    Glo.OpenWindow(Glo.BPMHost + url, "执行", 670, 1050);
                    return;
                case UrlFlag.FlowFrms:
                    url = "/WF/Admin/XAP/DoPort.aspx?DoType=FlowFrms&FK_Flow=" + fk_flow + "&FK_Node="+node1+"&Lang=CH";
                    Glo.OpenWindow(Glo.BPMHost + url, "执行", 670, 1050);
                    return;
                case UrlFlag.FlowSortP:
                    url = "/WF/Admin/XAP/DoPort.aspx?DoType=En&EnName=BP.WF.Template.FlowSort&PK=" + node1 + "&Lang=CH";
                    Glo.OpenDialog(Glo.BPMHost + url, "执行", 670, 1050);
                    return;
                case UrlFlag.NodeP:
                    url = "/WF/Admin/XAP/DoPort.aspx?DoType=En&EnName=BP.WF.Node&PK=" + node1 + "&Lang=CH";
                    Glo.OpenDialog(Glo.BPMHost + url, "执行", 670, 1050);
                    return;
                case UrlFlag.NodePNew:
                    url = "/WF/Admin/XAP/DoPort.aspx?DoType=En&EnName=BP.WF.Template.NodeExt&PK=" + node1 + "&Lang=CH";
                    Glo.OpenDialog(Glo.BPMHost + url, "执行", 670, 1050);
                    return;
                case UrlFlag.FlowP: // 节点属性与流程属性。
                    url = "/WF/Admin/XAP/DoPort.aspx?DoType=En&EnName=BP.WF.Template.FlowSheet&PK=" + fk_flow + "&Lang=CH";
                    Glo.OpenDialog(Glo.BPMHost + url, "", 670, 1050);
                    return;
                case UrlFlag.FlowPNew: // 节点属性与流程属性。
                    url = "/WF/Admin/XAP/DoPort.aspx?DoType=En&EnName=BP.WF.Template.FlowExt&PK=" + fk_flow + "&Lang=CH";
                    Glo.OpenDialog(Glo.BPMHost + url, "", 670, 1050);
                    return;
                case UrlFlag.MapDefFixModel: // SDK表单设计。
                    url = "/WF/Admin/XAP/DoPort.aspx?DoType=MapDefFixModel&FK_MapData=ND" + node1 + "&FK_Node=" + node1 + "&Lang=CH&FK_Flow=" + fk_flow;
                    Glo.OpenWindow(url, "节点表单设计");
                    return;
                case UrlFlag.MapDefFreeModel: // 自由表单设计。
                    string fk_MapData = "ND" + node1;
                    string title = "表单ID: {0} 存储表:{1} 名称:{2}";
                    title = string.Format(title, fk_MapData, fk_MapData, node2);
                    if (Glo.UrlOrForm)
                    {
                        url = "/WF/Admin/XAP/DoPort.aspx?DoType=MapDefFreeModel&FK_MapData="
                        + fk_MapData + "&FK_Node=" + node1 + "&Lang=CH&FK_Flow=" + fk_flow+"&AppCenterDBType=";
                        MainPage.Instance.OpenBPForm(BPFormType.FormNode, title, url);
                    }
                    else
                    {
                        MainPage.Instance.OpenBPForm(BPFormType.FormNode, title, fk_MapData, fk_flow);
                    }
                  
                    break;
                case UrlFlag.FormFixModel: // 节点表单设计。
                    url = "/WF/Admin/XAP/DoPort.aspx?DoType=MapDefFixModel&FK_MapData=" + fk_flow;
                    Glo.OpenDialog(Glo.BPMHost + url, "节点表单设计");
                    return;
                case UrlFlag.FormFreeModel: // 节点表单设计。
                    url = "/WF/Admin/XAP/DoPort.aspx?DoType=MapDefFreeModel&FK_MapData=" + fk_flow;
                    Glo.OpenDialog(Glo.BPMHost + url, "节点表单设计");
                    return;
                case UrlFlag.Dir: // 方向条件。
                    url = "/WF/Admin/ConditionLine.aspx?FK_Flow=" + fk_flow + "&FK_MainNode=" + node1 + "&FK_Node=" + node1 + "&ToNodeID=" + node2 + "&CondType=2&Lang=CH";
                    Glo.OpenDialog(Glo.BPMHost + url, "方向条件", 670, 1050);
                    break;
                case UrlFlag.RunFlow: // 运行流程。
                    url = "/WF/Admin/TestFlow.aspx?FK_Flow=" + fk_flow + "&Lang=CH";
                    Glo.OpenWindow(Glo.BPMHost + url, "运行流程", 670, 1050);
                    return;
                case UrlFlag.FlowCheck: // 流程设计。
                    url = "/WF/Admin/DoType.aspx?RefNo=" + fk_flow + "&DoType=" + dotype + "&Lang=CH";
                    Glo.OpenWindow(Glo.BPMHost + url, "运行流程", 670, 1050);
                    break;
                case "LoginPage": // 登录。
                    url = @"/WF/App/Classic/Login.aspx?DoType=Logout";
                    Glo.OpenWindow(Glo.BPMHost + url, "登录", 670, 1050);
                    break;
                case UrlFlag.WFRpt: // 流程设计。
                    url = "/WF/Admin/XAP/DoPort.aspx?RefNo=" + fk_flow + "&DoType=" + dotype + "&Lang=CH&PK="+fk_flow;
                    Glo.OpenDialog(Glo.BPMHost + url, "运行流程", 670, 1050);
                    break;
                default:
                    MessageBox.Show("没有判断的url执行标记:" + dotype);
                    break;
            }
        }

        public static void OpenMax(string url, string title)
        {
            OpenWindowOrDialog(url, title, WindowModelEnum.Max);
        }
        public static void OpenDialog(string url, string title, int h = 0, int w = 0)
        {
            OpenWindowOrDialog(url, title,  WindowModelEnum.Dialog,h,w);
        }
        public static void OpenWindow(string url, string title, int h = 0, int w = 0)
        {
            OpenWindowOrDialog(url, title, WindowModelEnum.Window,h,w);
        }
     
        /// <summary>
        /// 弹出网页窗口
        /// </summary>
        /// <param name="url">网页地址</param>
        private static void OpenWindowOrDialog(string url, string title, WindowModelEnum windowModel, int height = 0, int width = 0, int left = 0, int top = 0, bool resizable = true)
        {
            if (!url.Contains("ttp://") || !url.Contains("http"))
                url = Glo.BPMHost + url;

            if (Glo.Platform == BP.Platform.JFlow)
            {
                url = url.Replace(".aspx", ".jsp");
                if (url.Contains("/XAP"))
                    url = url.Replace("/XAP", "/xap");
            }

            try
            {
                if (windowModel == WindowModelEnum.Dialog)
                {
                    BrowserInformation info = HtmlPage.BrowserInformation;
                    if (!info.Name.Contains("Netscape"))
                    {
                        HtmlPage.Window.Eval(
                        string.Format("window.showModalDialog('{0}',window,'dialogHeight:" + height + "px;dialogWidth:" + width + "px;help:no;scroll:auto;resizable:yes;status:no;');",
                            url));
                    }
                    else
                    {

                        //HtmlPage.Window.Invoke("showDialog", url, title, height, width);
                        OpenWindow(url, title, height, width);
                        return;
                        //HtmlPopupWindowOptions options = new HtmlPopupWindowOptions()
                        //{
                        //    Directories = false,
                        //    Location = false,
                        //    Menubar = false,
                        //    Status = false,
                        //    Toolbar = false,
                        //    Width = 1024,
                        //    Height = 600,
                        //    Scrollbars = true,
                        //    Resizeable = false
                        //};
                        //options.Left = (Glo.ScreenWidth - options.Width) / 2;
                        //options.Top = (Glo.ScreenHeight - options.Height) / 2;
                        //HtmlPage.PopupWindow(new Uri(url), "self", options);
                    }
                }
                else if (windowModel == WindowModelEnum.Max)
                {
                    string tmp = "window.open('{0}','{1}','left=0,top=0,height={2},width={3},resizable={4},scrollbars=yes,help=no,toolbar=no,menubar=no,scrollbars=yes,status=yes,location=no')";
                    width = 0 < width ? width : ScreenWidth;
                    height = 0 < height ? height : ScreenHeight-100;// 系统任务栏高度？？
                    string resize = resizable ? "yes" : "no";
                    url = string.Format(tmp, url, title, height, width, resize);
                    HtmlPage.Window.Eval(url);
                }
                else
                {
                    if (0 < height && 0 < width)
                    {
                        string tmp = "window.open('{0}', '{1}' ,'height={2},width={3},resizable=yes,help=no,toolbar =no, menubar=no, scrollbars=yes,status=yes,location=no')";
                        url = string.Format(tmp, url, title, height, width);
                    }
                    else
                    {
                        url = "window.open('" + url + "','_blank')";
                    }
                    HtmlPage.Window.Eval(url);
                }
            }
            catch (System.Exception e)
            {
                Glo.ShowException(e);
            }
        }
        #endregion

        #region 异常、加载提示、登录、颜色
        public static void ShowException(System.Exception e, string error = "")
        {
            BP.SL.LoggerHelper.Write(e);

            if (string.IsNullOrEmpty(error))
            {
                MessageBox.Show("错误:" + e.Message + " \t\n " + e.StackTrace, e.Message, MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show(error + ":" + e.StackTrace + "\t\n@详细请查看异常日志", e.Message, MessageBoxButton.OK);
            }
        }

        static BP.Frm.Waiting waiting;
        /// <summary>
        /// 显示Waiting窗体
        /// </summary>
        public static void Loading(bool showIt)
        {
            if (null == waiting)
                waiting = new Frm.Waiting();

            if (!showIt)
            {
                waiting.DialogResult = true;
                //waiting.Close();
            }
            else
            {
                waiting.Show();
            }
        }

        static ChildWindow login;
        public static void Login()
        {
            if (login == null)
            {
                login = new AdminLogin();
                login.Closed += (object sender, EventArgs e)=>
                {
                    if( login.DialogResult == true)
                        MainPage.Instance.LoginCompleted();
                };
            }

            login.Show();
        }

        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static bool IsNum(string exp)
        {
            try
            {
                Int64 i = Int64.Parse(exp);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取与特定对象相关类型元素
        /// </summary>
        /// <typeparam name="T">要匹配的元素类型</typeparam>
        /// <param name="obj"></param>
        /// <param name="name">特定名称，可为空</param>
        /// <returns></returns>
        public static T GetParentObject<T>(DependencyObject obj, string name= "") where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
           
            //bool flag =typeof(Border).IsInstanceOfType(parent);

            while (parent != null)
            {
                if (parent is T
                    && (string.IsNullOrEmpty(name) | ((T)parent).Name == name))
                {
                    return (T)parent;
                }
              
               parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }

        public static T GetChildObject<T>(DependencyObject obj, string name = "") where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;
  
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
  
                if (child is T && (((T)child).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
         }


        /// <summary>
        /// 获取TreeView控件中指定名称的TreeNode
        /// </summary>
        /// <param name="myNode"></param>
        /// <param name="nodeName"></param>
        /// <returns>匹配TreeNode</returns>
        public static TreeNode findNode(TreeNode myNode, string nodeName)
        {
            TreeNode node = null;

            if (myNode.Name == nodeName || myNode.EditedTitle == nodeName)
                return myNode;

            else if (myNode.HasChildren)
            {
                foreach (TreeNode n in myNode.Nodes)
                {
                    node = findNode(n, nodeName);
                    if (null != node)
                        break;
                }
                return node;
            }

            return node;
        }

        #endregion

    }
}
