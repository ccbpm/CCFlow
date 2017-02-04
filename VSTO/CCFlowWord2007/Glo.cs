using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using BP.Web;
using BP.DA;
using BP.WF;
using Office = Microsoft.Office.Core;
using CCFlowWord2007;

namespace BP.WF
{
    public class Glo
    {
        #region 与控件有关的操作方法
        public static bool Ctrl_DDL_SetSelectVal(ComboBox ddl, string setVal)
        {
            //string oldVal = "";
            //foreach (LisItem item in ddl.Items)
            //{
            //    if (item.IsEnabled == true)
            //    {
            //        oldVal = item.Tag.ToString();
            //        item.IsSelected = false;
            //        break;
            //    }
            //}
            //foreach (ListBoxItem item in ddl.Items)
            //{
            //    if (item.Tag.ToString() == setVal)
            //    {
            //        item.IsSelected = true;
            //        return true;
            //    }
            //}
            //foreach (ListBoxItem item in ddl.Items)
            //{
            //    if (item.Tag.ToString() == oldVal)
            //    {
            //        item.IsSelected = true;
            //        break;
            //    }
            //}
            return false;
        }
        public static void Ctrl_DDL_BindDataTable(ComboBox ddl, DataTable dt, string selectVal)
        {
            ddl.Items.Clear();
            ddl.DataSource = dt;
            ddl.DisplayMember = "Name";
            ddl.ValueMember = "NodeID";
            ddl.SelectedIndex = 0;
        }
        #endregion

        public static void SetState()
        {
          //  HisRibbon1.SetState();
        }
        /// <summary>
        /// 打开文档
        /// </summary>
        /// <param name="file">文档地址</param>
        /// <param name="_isReadonly">是否以只读方式打开</param>
        public static void OpenDoc(string file, object _isReadonly)
        {
            if (Globals.ThisAddIn.Application.Documents.Count > 0 &&
               Globals.ThisAddIn.Application.ActiveDocument != null)
                Globals.ThisAddIn.Application.ActiveDocument.Close();

            if (string.IsNullOrEmpty(file))
            {
                file = Path.Combine(Glo.PathOfTInstall, DateTime.Now.ToString("yyyyMMddhhmmss") + ".doc");
                if (File.Exists(file))
                    File.Delete(file);

                var doc = Globals.ThisAddIn.Application.Documents.Add(Visible: true);
                doc.Activate();

                Globals.ThisAddIn.Application.Selection.TypeText(DateTime.Now.ToString("yyyy年MM月dd日") + " 无公文模板");
                doc.SaveAs(file);
                return;
            }

            object fileName = file;
            object missing = Type.Missing;

            try
            {
                Globals.ThisAddIn.Application.Documents.Open(ref fileName, ref missing, ref _isReadonly);
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开文件期间异常：" + ex.Message);
            }
        }
        public static FtpSupport.FtpConnection HisFtpConn
        {
            get
            {
                try
                {
                    return new FtpSupport.FtpConnection(Glo.FtpIP, Glo.FtpUser, Glo.FtpPass);
                }
                catch (Exception ex)
                {
                    throw new Exception("@有可能ftp密码与用户名错误，请检查。 @异常信息:" + ex.Message);
                }
            }
        }
        public static string FtpIP { private get; set; }
        public static string FtpUser { private get; set; }
        public static string FtpPass { private get; set; }
        /// <summary>
        /// 临时文件
        /// </summary>
        public static string PathOfTInstall = @"C:\\WF\\";
        /// <summary>
        /// 个人文件
        /// </summary>
        public static string Profile
        {
            get
            {
                return BP.WF.Glo.PathOfTInstall + "\\Profile.txt";
            }
        }
        public static string ProfileLogin
        {
            get
            {
                return BP.WF.Glo.PathOfTInstall + "\\Login.txt";
            }
        }
        /// <summary>
        /// 流程服务器的位置
        /// </summary>
        public static string WFServ = "http://localhost:50572";
    }
}
