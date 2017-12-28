using System;
using System.Collections.Generic;
using System.Text;
using Word = Microsoft.Office.Interop.Word;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Word;
using BP.Web;
using System.Windows.Forms;
using BP.WF;
using BP.Web;

namespace CCFlowWord2007
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            try
            {
                WebUser.HisRib.SetState();
            }
            catch (Exception ex)
            {
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            if (Profile.IsExitProfile == false)
                return;

            var dr = MessageBox.Show("您要安全退出吗？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
                WebUser.SignOut();
        }

        #region Methods
        /// <summary>
        /// 执行保存
        /// </summary>
        public void DoSave()
        {
            if (WebUser.FK_Flow == null)
            {
                MessageBox.Show("您没有选择流程您不能存盘。");
                return;
            }


            if (WebUser.WorkID == 0)
                WebUser.WorkID = BP.WF.Dev2Interface.Node_CreateBlankWork(BP.Web.WebUser.FK_Flow);

            #region 把文件放在服务器上
            FtpSupport.FtpConnection conn = Glo.HisFtpConn;
            try
            {
                conn.SetCurrentDirectory("/DocFlow/" + WebUser.FK_Flow + "/" + WebUser.WorkID + "/");
            }
            catch
            {
                if (conn.DirectoryExist("/DocFlow/") == false)
                    conn.CreateDirectory("/DocFlow/");

                if (conn.DirectoryExist("/DocFlow/" + WebUser.FK_Flow + "/") == false)
                    conn.CreateDirectory("/DocFlow/" + WebUser.FK_Flow + "/");

                if (conn.DirectoryExist("/DocFlow/" + WebUser.FK_Flow + "/" + WebUser.WorkID + "/") == false)
                    conn.CreateDirectory("/DocFlow/" + WebUser.FK_Flow + "/" + WebUser.WorkID);

                conn.SetCurrentDirectory("/DocFlow/" + WebUser.FK_Flow + "/" + WebUser.WorkID + "/");
            }
            string file = Glo.PathOfTInstall + DateTime.Now.ToString("MMddhhmmss") + ".doc";
            ThisAddIn.SaveAs(file);

            System.IO.File.Copy(file, "c:\\Tmp.doc", true);

            conn.PutFile("c:\\Tmp.doc", WebUser.FK_Node + "@" + WebUser.No + ".doc"); //当前人员的文件.
            conn.PutFile("c:\\Tmp.doc", WebUser.WorkID + ".doc"); //最新的文件.
            conn.Close();
            #endregion 把文件放在服务器上

            //删除临时文件.
            System.IO.File.Delete("c:\\Tmp.doc");
            //  MessageBox.Show("您的文件已经保存到服务器上", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public static void SaveAs(string file)
        {
            object FileName = file;
            object FileFormat = Word.WdSaveFormat.wdFormatDocument;

            Globals.ThisAddIn.Application.ActiveWindow.Document.Save();
            Globals.ThisAddIn.Application.ActiveWindow.Document.SaveAs(ref FileName, ref FileFormat);
        }

        #endregion

        #region VSTO generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        #endregion
    }
}
