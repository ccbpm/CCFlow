using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BP.Sys.SL;
using CCForm.FF;
using System.Runtime.Serialization;
using System.IO;
using System.Reflection;
using Silverlight;
using System.Text.RegularExpressions;

namespace CCForm
{
    public class MouseEventHandlers
    {
        static bool isLeftDown;// 框选框拖动单独使用
        public static bool IsCopy;
        public static Point pointFrom;
        static MousePosition position = MousePosition.None;
        public static MouseEventHandler MouseMove = (object sender, MouseEventArgs e) =>
        {
            #region 拖拽复制.
            if (Keyboard.Modifiers == ModifierKeys.Control && sender != MainPage.RectSelected)
            {
                IsCopy = true;
                return;
            }
            #endregion 复制.

            #region
            try
            {
                Point pointTo = e.GetPosition(null);
                FrameworkElement bp = sender as FrameworkElement;
                IElement ie = bp as IElement;
                if (!ie.TrackingMouseMove)
                {
                    position = MousePointPosition(bp, e, ie.IsCanReSize);
                }
                else
                {
                    if (position != MousePosition.None)
                    {
                        double xOffset = pointTo.X - pointFrom.X;
                        double yOffset = pointTo.Y - pointFrom.Y;
                        if (position == MousePosition.Drag)
                        {
                            // Set new position of object.
                            if (bp is BPLine)
                            {
                                BPLine line = bp as BPLine;
                                if (line != null)
                                {
                                    line.UpdatePos(xOffset, yOffset);
                                }
                            }
                            else
                            {
                                Canvas.SetTop(bp, Canvas.GetTop(bp) + yOffset);
                                Canvas.SetLeft(bp, Canvas.GetLeft(bp) + xOffset);
                            }
                        }
                        if (ie.IsCanReSize)
                        {
                            if (bp is BPLine)
                                return;
                            #region

                            //// 最大最小控制
                            //if (ele is BPTextBox)
                            //{
                            //    if (ele.ActualHeight < 20)
                            //    {
                            //        ele.Height = 20;
                            //        TrackingMouseMove = false;
                            //        return;
                            //    }
                            //    else if (ele.ActualHeight < 40)
                            //    {
                            //        ele.Width = 40;
                            //        TrackingMouseMove = false;
                            //        return;
                            //    }
                            //}

                            switch (position)
                            {
                                case MousePosition.SizeTop: // 向上拉伸，Y轴上移
                                    Canvas.SetTop(bp, Canvas.GetTop(bp) + yOffset);
                                    bp.Height = bp.ActualHeight - yOffset;

                                    break;
                                case MousePosition.SizeBottom:
                                    bp.Height = bp.ActualHeight + yOffset;
                                    break;

                                case MousePosition.SizeLeft: //向左拉伸，X轴左移
                                    Canvas.SetLeft(bp, Canvas.GetLeft(bp) + xOffset);
                                    bp.Width = bp.ActualWidth - xOffset;

                                    break;

                                case MousePosition.SizeRight:
                                    bp.Width = bp.ActualWidth + xOffset;
                                    break;

                                case MousePosition.SizeTopLeft:
                                    bp.Width = bp.ActualWidth - xOffset;
                                    Canvas.SetLeft(bp, Canvas.GetLeft(bp) + xOffset);

                                    bp.Height = bp.ActualHeight - yOffset;
                                    Canvas.SetTop(bp, Canvas.GetTop(bp) + yOffset);

                                    break;

                                case MousePosition.SizeBottomRight:
                                    bp.Width = bp.ActualWidth + xOffset;
                                    bp.Height = bp.ActualHeight + yOffset;

                                    break;
                                case MousePosition.SizeBottomLeft:
                                    Canvas.SetLeft(bp, Canvas.GetLeft(bp) + xOffset);
                                    bp.Width = bp.ActualWidth - xOffset;
                                    bp.Height = bp.ActualHeight + yOffset;

                                    break;

                                case MousePosition.SizeTopRight:
                                    bp.Width = bp.ActualWidth + xOffset;
                                    Canvas.SetTop(bp, Canvas.GetTop(bp) + yOffset);
                                    bp.Height = bp.ActualHeight - yOffset;

                                    break;
                            }
                            #endregion
                        }
                    }

                    Glo.ViewNeedSave = true;
                }

                pointFrom = pointTo;
            }
            catch (System.Exception ex)
            {
            }
            #endregion
        };

        const int Band = 5;
        /// <summary>
        ///  确定光标在控件不同位置的样式
        /// </summary>
        /// <param name="element"></param>
        /// <param name="e"></param>
        /// <param name="IsCanReSize"></param>
        /// <returns></returns>
        public static MousePosition MousePointPosition(FrameworkElement element, MouseEventArgs e, bool IsCanReSize)
        {
            Point pp = e.GetPosition(element);
            //Size size = element.RenderSize;
            Size size = new System.Windows.Size() { Width = element.ActualWidth, Height = element.ActualHeight };
            MousePosition empp = MousePosition.None;
            if ((pp.X >= -1 * Band) | (pp.X <= size.Width) | (pp.Y >= -1 * Band) | (pp.Y <= size.Height))
            {
                if (IsCanReSize)
                {
                    #region
                    if (pp.X < Band)
                    {
                        if (pp.Y < Band)
                        {
                            empp = MousePosition.SizeTopLeft;
                        }
                        else
                        {
                            if (pp.Y > -1 * Band + size.Height)
                            {
                                empp = MousePosition.SizeBottomLeft;
                            }
                            else
                            {
                                empp = MousePosition.SizeLeft;
                            }
                        }
                    }
                    else
                    {
                        if (pp.X > -1 * Band + size.Width)
                        {
                            if (pp.Y < Band)
                            {
                                empp = MousePosition.SizeTopRight;
                            }
                            else
                            {
                                if (pp.Y > -1 * Band + size.Height)
                                {
                                    empp = MousePosition.SizeBottomRight;
                                }
                                else
                                {
                                    empp = MousePosition.SizeRight;
                                }
                            }
                        }
                        else
                        {
                            if (pp.Y < Band)
                            {
                                empp = MousePosition.SizeTop;
                            }
                            else
                            {
                                if (pp.Y > -1 * Band + size.Height)
                                {
                                    empp = MousePosition.SizeBottom;
                                }
                                else
                                {
                                    empp = MousePosition.Drag;
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    empp = MousePosition.Drag;
                }
            }
            else
            {
                empp = MousePosition.None;
            }

            Cursor cursor = Cursors.Arrow;
            switch (empp)
            {
                case MousePosition.Drag:
                    cursor = Cursors.Hand;    //'四方向    
                    break;
                case MousePosition.SizeBottom:
                    cursor = Cursors.SizeNS;       //'南北    
                    break;
                case MousePosition.SizeBottomLeft:

                    cursor = Cursors.SizeNESW;     //'东北到南西    
                    break;
                case MousePosition.SizeBottomRight:

                    cursor = Cursors.SizeNWSE;     //'东南到西北      
                    break;
                case MousePosition.SizeLeft:

                    cursor = Cursors.SizeWE;       //'东西       
                    break;
                case MousePosition.SizeRight:
                    cursor = Cursors.SizeWE;       //'东西  
                    break;
                case MousePosition.SizeTop:
                    cursor = Cursors.SizeNS;       //'南北  
                    break;
                case MousePosition.SizeTopLeft:

                    cursor = Cursors.SizeNWSE;     //'东南到西北       
                    break;
                case MousePosition.SizeTopRight:

                    cursor = Cursors.SizeNESW;     //'东北到南西       
                    break;
                case MousePosition.None:
                default://'箭头
                    cursor = Cursors.Arrow;
                    break;
            }
            element.Cursor = cursor;
            return empp;
        }
    }
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

    /// <summary>
    /// 全局类
    /// </summary>
    public class Glo
    {
        #region 属性.
        /// <summary>
        /// 节点ID.
        /// </summary>
        public static int NodeID
        {
            get
            {
                try
                {
                    return int.Parse(Glo.FK_MapData.Replace("ND", ""));
                }
                catch
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 生成GUID
        /// </summary>
        /// <returns></returns>
        public static string GenerGUID()
        {
            return Guid.NewGuid().ToString("D");
        }
        public static string AppName = "/";
        public static Platform Platform
        {
            get
            {
                string url = System.Windows.Browser.HtmlPage.Document.DocumentUri.OriginalString;
                if (url.ToLower().Contains("jsp") == true)
                    return CCForm.Platform.JFlow;
                return CCForm.Platform.CCFlow;
            }
        }
        /// <summary>
        /// 是否是节点表单?
        /// </summary>
        public static bool IsNodeFrm
        {
            get
            {
                if (Glo.FK_MapData.StartsWith("ND") == true)
                    return true;
                return false;
            }
        }
        #endregion

        #region Json
        public static string ToJson(DataSet dataSet)
        {
            string jsonString = "{";
            foreach (DataTable table in dataSet.Tables)
            {
                jsonString += "\"" + table.TableName.ToUpper() + "\":" + ToJson(table) + ",";
            }
            jsonString = jsonString.TrimEnd(',');
            return jsonString + "}";
        }
        public static string ToJson(DataTable dt)
        {
            System.Text.StringBuilder jsonString = new System.Text.StringBuilder();

            if (dt.Rows.Count == 0)
            {
                jsonString.Append("[{}]");
                return jsonString.ToString();
            }

            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                jsonString.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string strKey = dt.Columns[j].ColumnName.ToUpper();
                    string strValue = drc[i][j] == null ? "" : drc[i][j].ToString();
                    Type type = dt.Columns[j].DataType;
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }
        private static string StringFormat(string str, Type type)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (type == typeof(string))
                {
                    str = String2Json(str);
                    str = "\"" + str + "\"";
                }
                else if (type == typeof(DateTime))
                {
                    str = "\"" + Convert.ToDateTime(str).ToShortDateString() + "\"";
                }
                else if (type == typeof(bool))
                {
                    str = str.ToLower();
                }
            }
            if (str.Length == 0)
                str = "\"\"";

            return str;
        }
        private static string String2Json(String s)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s.ToCharArray()[i];

                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        #endregion

        /// <summary>
        /// 仅允许含有汉字、数字、字母、下划线
        /// <para>示例：</para>
        /// <para>   Console.WriteLine(RegEx.Replace("姓名@-._#:：“｜：$?>a:12",RegEx_Replace_OnlyHSZX,""));</para>
        /// <para>   输出：姓名_a12</para>
        /// </summary>
        public const string RegEx_Replace_OnlyHSZX = @"[^0-9a-zA-Z_\u4e00-\u9fa5]";
        /// <summary>
        /// 仅允许含有数字、字母、下划线
        /// <para>示例：</para>
        /// <para>   Console.WriteLine(RegEx.Replace("姓名@-._#:：“｜：$?>a:12",RegEx_Replace_OnlySZX,""));</para>
        /// <para>   输出：_a12</para>
        /// </summary>
        public const string RegEx_Replace_OnlySZX = @"[\u4e00-\u9fa5]|[^0-9a-zA-Z_]";
        /// <summary>
        /// 匹配字符串开头为数字或下划线
        /// <para>示例：</para>
        /// <para>   Console.WriteLine(RegEx.Replace("_12_a1",RegEx_Replace_FirstXZ,""));</para>
        /// <para>   输出：a1</para>
        /// </summary>
        public const string RegEx_Replace_FirstXZ = "^(_|[0-9])+";
        public static string CustomerNo;
        public static string AppCenterDBType1;
        public static Color BorderBrush = Color.FromArgb(255, 160, 171, 193);
        public static bool ViewNeedSave;
        public static T Clone<T>(T source)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, source);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)serializer.ReadObject(ms);
            }
        }
        /// <summary>
        /// 设置实体对象的修改属性 
        /// 说明:需要复制一个实体类，而又不希望两个使用同一个内存地址，用反射来实现这种功能:把oldObj 赋值给 newObj
        /// 可以将实体类直接继承ICloneable接口，并用如上方法来实现Clone()方法. 
        /// 现在有不少架构直接使用自动代码生成器，也可以不使用反射直接每个属性值进行拷贝亦可
        /// </summary>
        /// <param name="oldObj">原来对象</param>
        /// <param name="newObj">新对象</param>
        public static object CloneEntityObject(object oldObj, object newObj)
        {
            if (oldObj.Equals(newObj))
            {
                return null;
            }

            Type typeOld = oldObj.GetType();
            Type typeNew = newObj.GetType();
            if (typeOld != typeNew)
            {
                return null;
            }
            System.Reflection.PropertyInfo[] info = typeNew.GetProperties();
            foreach (System.Reflection.PropertyInfo prop in info)
            {
                object val = typeOld.GetProperty(prop.Name).GetValue(oldObj, null);
                if (val != null)
                    typeNew.GetProperty(prop.Name).SetValue(newObj, val, null);
            }
            return newObj;
        }
        public static void Remove(Control child)
        {
            Canvas canvas = child.Parent as Canvas;
            if (canvas != null && canvas.Children.Contains(child))
            {
                canvas.Children.Remove(child);
                ViewNeedSave = true;
            }

            if (child is IElement)
            {
                IElement e = child as IElement;
                e.ViewDeleted = true;
            }
        }

        public static void SetTracking(IElement element, bool TrackingMouseMove)
        {
            if (null != element)
            {
                element.TrackingMouseMove = TrackingMouseMove;
            }
        }

        /// <summary>
        /// 全局变量x
        /// </summary>
        public static double X = 0;
        /// <summary>
        /// 全局变量Y
        /// </summary>
        public static double Y = 0;
        private static string _Len_Function = null;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string LEN_Function
        {
            get
            {
                if (_Len_Function == null)
                {
                    switch (Glo.AppCenterDBType)
                    {
                        case DBType.MSSQL:
                            _Len_Function = "LEN";
                            break;
                        default:
                            _Len_Function = "LENGTH";
                            break;
                    }
                }
                return _Len_Function;
            }
        }
        public static string TimeKey
        {
            get
            {
                return "&T=" + DateTime.Now.ToString("yyMMddHHmmss");
            }
        }

        #region 操作步骤的恢复
        public static int CurrOpStep = 0;
        private static List<FuncStep> _FuncSteps = null;
        public static List<FuncStep> FuncSteps
        {
            get
            {
                if (_FuncSteps == null)
                    _FuncSteps = FuncStep.instance.GetEns();
                return _FuncSteps;
            }
        }
        #endregion 操作步骤的恢复

        /// <summary>
        /// 获取汉语拼音
        /// </summary>
        /// <param name="TB_Name">短语</param>
        /// <param name="flag">全拼或简拼</param>
        /// <param name="TB_KeyOfEn">控件拼音结果</param>
        public static void GetKeyOfEn(string TB_Name, bool flag, TextBox TB_KeyOfEn)
        {
            if (string.IsNullOrEmpty(TB_Name))
            {
                // MessageBox.Show("您需要输入字段中英文名称", "将汉字词语转换成拼音期间错误", MessageBoxButton.OK);
                return;
            }

            //将非汉字/字母/数字/下划线的所有字符去掉
            string newStr = Regex.Replace(TB_Name.Trim(), Glo.RegEx_Replace_OnlyHSZX, "");

            CCFormSoapClient ff = Glo.GetCCFormSoapClientServiceInstance();
            ff.ParseStringToPinyinAsync(newStr, flag);
            ff.ParseStringToPinyinCompleted += new EventHandler<FF.ParseStringToPinyinCompletedEventArgs>(
                (object sender, FF.ParseStringToPinyinCompletedEventArgs e) =>
                {
                    if (e.Error == null && e.Result != null)
                        TB_KeyOfEn.Text = e.Result.Length > 20 ? e.Result.Substring(0,20) : e.Result;   //字段KeyOfEn的长度控制在20以内，added by liuxc,2017-9-25
                });
        }

#if DEBUG
        static TimeSpan ts = new TimeSpan(0, 10, 0);
#else
       static TimeSpan ts = new TimeSpan(0, 1, 0);
#endif
        /// <summary>
        /// 得到WebService对象
        /// </summary>
        /// <returns></returns>
        public static FF.CCFormSoapClient GetCCFormSoapClientServiceInstance()
        {
            var basicBinding = new BasicHttpBinding()
            {
                //CloseTimeout = ts,
                //OpenTimeout = ts,
                ReceiveTimeout = ts,
                SendTimeout = ts,
                MaxBufferSize = 2147483647,
                MaxReceivedMessageSize = 2147483647,
                Name = "CCFormSoapClient"
            };
            basicBinding.Security.Mode = BasicHttpSecurityMode.None;
            string url = "";

            if (Glo.Platform == CCForm.Platform.CCFlow)
                url = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/CCForm/CCForm.asmx";
            else
                //url = new Uri(App.Current.Host.Source, "../").ToString() + "service/ccformSoap?wsdl";
                url = Glo.BPMHost + "/service/ccformSoap?wsdl";
            
            var endPoint = new EndpointAddress(url);
            var ctor =
                typeof(CCFormSoapClient).GetConstructor(
                new Type[] { 
                    typeof(Binding),
                    typeof(EndpointAddress) 
                });
            return (CCFormSoapClient)ctor.Invoke(new object[] { basicBinding, endPoint });
        }
        public static bool trackingMouseMove = false;
        public static UIElement currEle;
        public static bool IsMouseDown = false;
        public static bool IsDtlFrm = false;

        /// <summary>
        /// 双击的时间间隔
        /// </summary>
        public const int DoubleClickTime = 300;
        private static System.Windows.Threading.DispatcherTimer doubleClickTimer;
        public static bool IsDbClick
        {
            get
            {
                if (null == doubleClickTimer)
                {
                    doubleClickTimer = new System.Windows.Threading.DispatcherTimer();
                    doubleClickTimer.Interval = new TimeSpan(0, 0, 0, 0, DoubleClickTime);
                    doubleClickTimer.Tick += (object sender, EventArgs e) =>
                    {
                        doubleClickTimer.Stop();
                    };
                }

                if (doubleClickTimer.IsEnabled)
                {
                    doubleClickTimer.Stop();
                    return true;
                }
                else
                {
                    doubleClickTimer.Start();
                    return false;
                }
            }
        }
        /// <summary>
        /// 是否可以重新绑定Frm.
        /// 解决导入字段后，重新绑定的问题.
        /// </summary>
        public static bool IsCanReBindFrmByFrmImp = false;
        /// <summary>
        /// 当前BPMHost 
        /// </summary>
        private static string _BPMHost = null;
        /// <summary>
        /// 当前BPMHost 
        /// </summary>
        public static string BPMHost
        {
            get
            {
                if (_BPMHost != null)
                    return _BPMHost;

                string myurl = System.Windows.Browser.HtmlPage.Document.DocumentUri.AbsoluteUri;

                myurl = myurl.Replace("//", "");
                int posStart = myurl.IndexOf("/");

                string appPath = myurl.Substring(posStart);
                if (appPath.Contains("/WF/MapDef"))
                {
                    appPath = appPath.Substring(0, appPath.IndexOf("/WF/MapDef", StringComparison.CurrentCultureIgnoreCase));
                }
                else
                {
                    appPath = appPath.Substring(0, appPath.IndexOf("/WF/", StringComparison.CurrentCultureIgnoreCase));
                }

                var location = (HtmlPage.Window.GetProperty("location")) as ScriptObject;
                _BPMHost = "http://" + location.GetProperty("host") + appPath;

                return _BPMHost;
            }
        }

        public static void BindComboBoxFrontName(ComboBox cb, string selectVal)
        {
            cb.Items.Clear();
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = "宋体";
            cbi.Tag = "宋体";
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "仿宋";
            cbi.Tag = "仿宋";
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "粗体";
            cbi.Tag = "粗体";
            cb.Items.Add(cbi);

            foreach (ComboBoxItem item in cb.Items)
            {
                if (item.Tag.ToString() == selectVal)
                    item.IsSelected = true;
                else
                    item.IsSelected = false;
            }

            if (cb.SelectedIndex == -1)
                cb.SelectedIndex = 0;
        }
        public static void SetComboBoxSelected(ComboBox cb, string val)
        {
            int index = 0;
            foreach (ComboBoxItem item in cb.Items)
            {
                item.IsSelected = false;
            }
            foreach (ComboBoxItem item in cb.Items)
            {
                if (item.Tag.ToString() == val)
                {
                    cb.SelectedIndex = index;
                    item.IsSelected = true;
                }
                index++;
            }
            if (cb.SelectedIndex == -1)
                cb.SelectedIndex = 0;
        }
        public static void BindComboBoxWinOpenTarget(ComboBox cb, string taget)
        {
            cb.Items.Clear();

            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = "新窗口";
            cbi.Tag = "_blank";
            if (taget == cbi.Tag.ToString())
                cbi.IsSelected = true;

            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "父窗口";
            cbi.Tag = "_parent";
            if (taget == cbi.Tag.ToString())
                cbi.IsSelected = true;
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "本窗口";
            cbi.Tag = "_self";
            if (taget == cbi.Tag.ToString())
                cbi.IsSelected = true;
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "自定义";
            cbi.Tag = "def";
            if ("@_blank,_self,_parent,".Contains(taget) == false)
                cbi.IsSelected = true;
            cb.Items.Add(cbi);
            SetComboBoxSelected(cb, taget);
        }
        public static void BindComboBoxFontSize(ComboBox cb, double selectDB)
        {
            cb.Items.Clear();
            for (int i = 6; i < 25; i++)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = i.ToString();
                if (i.ToString() == selectDB.ToString())
                    cbi.IsSelected = true;
                cb.Items.Add(cbi);
            }
            if (cb.SelectedIndex == -1)
                cb.SelectedIndex = 0;
        }
        public static void BindComboBoxLineBorder(ComboBox cb, double selectDB)
        {
            cb.Items.Clear();
            for (int i = 1; i < 15; i++)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = i.ToString();
                if (i.ToString() == selectDB.ToString())
                    cbi.IsSelected = true;
                else
                    cbi.IsSelected = false;
                cb.Items.Add(cbi);
            }

            if (cb.SelectedIndex == -1)
                cb.SelectedIndex = 0;
        }
        private static string[] _Colors = null;
        public static string[] ColorsStrs
        {
            get
            {
                if (_Colors == null)
                {
                    string cls = "@Black=#FF000000@Red=#FFFF0000@Blue=#FF0000FF@Green=#FF008000";
                    _Colors = cls.Split('@');
                }
                return _Colors;
            }
        }
        public static Color ToColor(string colorName)
        {
            try
            {
                if (colorName.StartsWith("#"))
                {
                    colorName = colorName.Replace("#", string.Empty);
                    //update by dgq 将6位RGB转为8位，FF代表完全不透明
                    if (colorName.Length == 6) colorName = "FF" + colorName;
                }
                int v = int.Parse(colorName, System.Globalization.NumberStyles.HexNumber);
                return new Color()
                {
                    A = Convert.ToByte((v >> 24) & 255),
                    R = Convert.ToByte((v >> 16) & 255),
                    G = Convert.ToByte((v >> 8) & 255),
                    B = Convert.ToByte((v >> 0) & 255)
                };
            }
            catch
            {
                return Colors.Black;
            }
        }

        public static Color PreaseColor_Del(string colorName)
        {
            switch (colorName)
            {
                case "Red":
                    return Colors.Red;
                case "Yellow":
                    return Colors.Yellow;
                case "Blue":
                    return Colors.Blue;
                case "Brown":
                    return Colors.Brown;
                case "Cyan":
                    return Colors.Cyan;
                case "DarkGray":
                    return Colors.DarkGray;
                case "Gray":
                    return Colors.Gray;
                case "Orange":
                    return Colors.Orange;
                case "Green":
                    return Colors.Green;
                default:
                    return Colors.Black;
            }
        }
        public static string PreaseColorToName(string coloVal)
        {
            foreach (string c in Glo.ColorsStrs)
            {
                if (string.IsNullOrEmpty(c))
                    continue;

                string[] kvs = c.Split('=');
                if (kvs[1] == coloVal)
                    return kvs[0];
            }
            return coloVal;
        }
        /// <summary>
        /// 字体类型
        /// </summary>
        /// <param name="cb"></param>
        /// <param name="selectDB"></param>
        public static void BindComboBoxFontStyle(ComboBox cb, string selectDB)
        {
            cb.Items.Clear();
            ComboBoxItem cbi = new ComboBoxItem();
            cbi.Content = "italic";
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "normal";
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "oblique";
            cb.Items.Add(cbi);

            cbi = new ComboBoxItem();
            cbi.Content = "inherit";
            cb.Items.Add(cbi);

            foreach (ComboBoxItem li in cb.Items)
            {
                if (li.Content.ToString() == selectDB)
                {
                    li.IsSelected = true;
                    break;
                }
            }
            if (cb.SelectedIndex < 0)
                cb.SelectedIndex = 0;
        }
        public static void BindComboBoxColors_Del(ComboBox cb, string selectDB)
        {
            cb.Items.Clear();
            foreach (string str in Glo.ColorsStrs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;

                string[] cls = str.Split('=');
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = cls[0];
                cbi.Tag = cls[1];

                if (cbi.Content.ToString() == selectDB || cbi.Tag.ToString() == selectDB)
                    cbi.IsSelected = true;
                cb.Items.Add(cbi);
            }
            if (cb.SelectedIndex < 0)
                cb.SelectedIndex = 0;
        }

        public static void WinOpenDialog(string url, int h = 600, int w = 1024)
        {
            if (Glo.Platform == Platform.JFlow)
            {
                url = url.Replace(".aspx", ".jsp");
                if (url.Contains("/XAP"))
                    url = url.Replace("/XAP", "/xap");
            }

            WinOpen(url, h, w); //2017-04-27，为解决IE弹出双窗口的问题，均改成用window.open，不用dialog，liuxc

            //BrowserInformation info = HtmlPage.BrowserInformation;

            //if (!info.Name.Contains("Netscape"))
            //{
            //    HtmlPage.Window.Eval(
            //    string.Format("window.showModalDialog('{0}',window,'dialogHeight:{1}px;dialogWidth:{2}px;help:no;scroll:auto;resizable:yes;status:no;');",
            //        url, h, w));
            //}
            //else
            //{
            //    WinOpen(url, h, w);
            //}
        }
        public static void WinOpen(string url, int h = 0, int w = 0)
        {

            if (Glo.Platform == CCForm.Platform.JFlow)
            {
                url = url.Replace(".aspx", ".jsp");
                if (url.Contains("/XAP"))
                    url = url.Replace("/XAP", "/xap");
            }

            string tmp = string.Empty;
            if (0 < h && 0 < w)
            {
                tmp = "window.open('{0}', '{1}' ,'height={2},width={3},resizable=yes,help=no,toolbar =no, menubar=no, scrollbars=yes,status=yes,location=no')";
                url = string.Format(tmp, url, "", h, w);
            }
            else
            {
                tmp = "window.open('{0}', '{1}' ,' top=0,left=0,height={2},width={3},resizable=yes,help=no,toolbar =no, menubar=no, scrollbars=yes,status=yes,location=no')";
                url = string.Format(tmp, url, "", App.Current.Host.Content.ActualHeight, App.Current.Host.Content.ActualWidth);

            }
            HtmlPage.Window.Eval(url);
        }
        public static void OpenDtl(string FK_MapData, string FK_MapDtl)
        {
            string url = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?DoType=Edit&FK_MapData=" + FK_MapData + "&FK_MapDtl=" + FK_MapDtl;
            Glo.WinOpen(url, 700, 1100);
        }
        public static void OpenM2M(string FK_MapData, string FK_MapM2M)
        {
            string url = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/MapM2M.aspx?DoType=Edit&FK_MapData=" + Glo.FK_MapData + "&NoOfObj=" + FK_MapM2M;

            //url = Glo.BPMHost + "/WF/Admin/FoolFormDesigner/MapM2M.aspx?DoType=Edit&FK_MapData=" + Glo.FK_MapData + "&FK_MapM2M=" + FK_MapM2M;
            WinOpenDialog(url, 600, 650);
        }
        public static void IE_ShowAddFGuide()
        {
            Glo.WinOpenDialog(Glo.BPMHost + "/WF/Admin/FoolFormDesigner/Do.aspx?DoType=AddF&MyPK=" + Glo.FK_MapData);
        }

        /// <summary>
        /// 获取正确的字段名称，仅允许包含汉字、字母、数字、下划线
        /// </summary>
        /// <param name="text">原始名称</param>
        /// <returns></returns>
        public static string GetCorrectFieldName(string text)
        {
            return Regex.Replace(text, RegEx_Replace_OnlyHSZX, "");
        }

        /// <summary>
        /// 获取正确的字段编号，仅允许包含字母、数字、下划线，且开头不允许是下划线与数字
        /// </summary>
        /// <param name="no">原始编号</param>
        /// <returns></returns>
        public static string GetCorrentFieldNo(string no)
        {
            return Regex.Replace(Regex.Replace(no, RegEx_Replace_OnlySZX, ""), RegEx_Replace_FirstXZ, "");
        }

        #region 参数.
        private static string _AppCenterDBType = null;
        public static string AppCenterDBType
        {
            get
            {
                if (_AppCenterDBType != null)
                    return _AppCenterDBType;
                if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("AppCenterDBType") == false)
                    _AppCenterDBType = null;
                else
                    _AppCenterDBType = System.Windows.Browser.HtmlPage.Document.QueryString["AppCenterDBType"];
                return _AppCenterDBType;
            }
            set
            {
                _AppCenterDBType = value;
            }
        }

        private static string _UserNo = null;
        public static string UserNo
        {
            get
            {
                if (_UserNo != null)
                    return _UserNo;
                if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("UserNo") == false)
                    _UserNo = BP.SessionManager.Session["UserNo"];
                else
                    _UserNo = System.Windows.Browser.HtmlPage.Document.QueryString["UserNo"];
                return _UserNo;
            }
            set
            {
                _UserNo = value;
            }
        }
        private static string _SID = null;
        public static string SID
        {
            get
            {
                if (_SID != null)
                    return _SID;
                if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("SID") == false)
                    _SID = BP.SessionManager.Session["SID"];
                else
                    _SID = System.Windows.Browser.HtmlPage.Document.QueryString["SID"];

                if (_SID == null)
                    _SID = "";
                return _SID;
            }
            set
            {
                _SID = value;
            }
        }

        private static string fk_Flow;
        public static string FK_Flow
        {
            get
            {
                if (!System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FK_Flow"))
                {
                    if (!string.IsNullOrEmpty(fk_Flow))
                        return fk_Flow;
                    else
                        return "004";
                }
                else
                {
                    fk_Flow = System.Windows.Browser.HtmlPage.Document.QueryString["FK_Flow"];
                    return fk_Flow;
                }
            }
            set
            {
                fk_Flow = value;
            }
        }
        private static int _FK_Node = 0;
        public static int FK_Node
        {
            get
            {
                if (_FK_Node != 0)
                    return _FK_Node;
                if (System.Windows.Browser.HtmlPage.Document.QueryString.ContainsKey("FK_Node") == false
                    || string.IsNullOrEmpty(System.Windows.Browser.HtmlPage.Document.QueryString["FK_Node"]))
                    _FK_Node = 999999;
                else
                    _FK_Node = int.Parse(System.Windows.Browser.HtmlPage.Document.QueryString["FK_Node"]);
                return _FK_Node;
            }
            set
            {
                _FK_Node = value;
            }
        }
        #endregion 参数.


        //这是什么意思？
#warning 这是什么意思??.
        public const string FK_MapDataNotFind = "ND1";
        private static string _FK_MapData = null;
        public static string FK_MapData
        {
            get
            {
                if (!HtmlPage.Document.QueryString.ContainsKey("FK_MapData"))
                {
                    if (_FK_MapData != null)
                        return _FK_MapData;
                    else
                        return FK_MapDataNotFind;
                }
                else
                {
                    _FK_MapData = System.Windows.Browser.HtmlPage.Document.QueryString["FK_MapData"];
                    return _FK_MapData;
                }
            }
            set
            {
                _FK_MapData = value;
                // MapData HisMapData = new MapData(value);
            }
        }
        
        public static MapData HisMapData = null;
        public static object TempVal = null;
    }
}

