using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BP.Web;
using BP.DA;
using BP.WF;
using Office = Microsoft.Office.Core;
using CCFlowWord2007;

namespace BP.Comm
{
    public partial class FrmIE : Form
    {
        public FrmIE()
        {
            InitializeComponent();
        }

        public Ribbon1 HisRibbon1;

        #region Load

        private void FrmIE_Load(object sender, EventArgs e)
        {
             
        }
        string MyURL = "";
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (this.MyURL.Contains("SendInfo") == true)
                this.Tag = "SendInfo";

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            base.OnFormClosed(e);
        }

        #endregion

        #region Control Events

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string url = e.Url.AbsoluteUri;
            MyURL = url;
            this.statusStrip1.Text = url;
            string pageID = url.Substring(url.LastIndexOf('/') + 1);

            this.toolStripStatusLabel1.Text = url;

            if (pageID.IndexOf(".aspx") != -1)
            {
                pageID = pageID.Substring(0, pageID.IndexOf(".aspx"));
                url = url.Substring(url.IndexOf(".aspx"));
                url = url.Replace("?", "@");
                url = url.Replace("&", "@");
                url = url.Replace(".aspx", "");

                var para = new AtPara(url);
                switch (pageID)
                {
                    case "DoClient":
                        try
                        {
                            switch (para.DoType)
                            {
                                case DoTypeConst.DoStartFlow: //发起流程
                                    this.DoStartFlow(para);
                                    break;
                                case DoTypeConst.DoStartFlowByTemple: //启动流程
                                    this.DoStartFlowByTemple(para);
                                    break;
                                case DoTypeConst.OpenFlow: //打开流程
                                    this.DoOpenFlow(para);
                                    break;
                                case DoTypeConst.OpenDoc:
                                    this.DoOpenDoc(para);
                                    break;
                                case DoTypeConst.DelFlow: //执行删除流程。
                                    WebUser.FK_Flow = null;
                                    WebUser.FK_Node = 0;
                                    WebUser.WorkID = 0;
                                    this.Close();
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("@错误：" + ex.Message + " PageID=" + pageID + "  DoType=" + para.DoType, "错误",
                                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Close();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 显示url
        /// </summary>
        /// <param name="url"></param>
        public void ShowUrl(string url)
        {
            this.webBrowser1.Url = new Uri(url);
        }
        public void OpenDoc(string file, object _isReadonly)
        {
        }
        #endregion

        #region Flow Methods
        /// <summary>
        /// 执行发送
        /// </summary>
        /// <param name="para"></param>
        public void DoSend(AtPara para)
        {
            Globals.ThisAddIn.DoSave();
            object obj = Type.Missing;
            Globals.ThisAddIn.Application.ActiveDocument.Close(ref obj, ref obj, ref obj);
        }
        public void DoOpenDoc(AtPara para)
        {
            string fk_flow = para.GetValStrByKey("FK_Flow");
            int workid = para.GetValIntByKey("WorkID");
            int fk_node = para.GetValIntByKey("FK_Node");

            string file = Glo.PathOfTInstall + workid + "@" + WebUser.No + ".doc";
            if (File.Exists(file) == false)
            {
                try
                {
                    FtpSupport.FtpConnection conn = Glo.HisFtpConn;
                    if (conn.DirectoryExist("/DocFlow/" + fk_flow + "/" + workid))
                    {
                        conn.SetCurrentDirectory("/DocFlow/" + fk_flow + "/" + workid);
                        conn.GetFile(workid + ".doc", file, true, FileAttributes.Archive);
                        conn.Close();
                    }
                    else
                    {
                        throw new Exception("@没有找到文件，流程错误。");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("@打开公文错误@技术信息:" + ex.Message + "@流程编号：" + WebUser.FK_Flow);
                    return;
                }
            }

            WebUser.FK_Flow = fk_flow;
            WebUser.FK_Node = fk_node;
            WebUser.WorkID = workid;

            WebUser.RetrieveWFNode(WebUser.FK_Node);

            /*如果存在这个文件，就激活它。*/
            WebUser.WriterIt(StartFlag.DoOpenDoc, fk_flow, fk_node, workid);

            this.OpenDoc(file, false);
            this.HisRibbon1.SetState();
            this.Close();
        }

        /// <summary>
        /// 打开流程
        /// </summary>
        /// <param name="para"></param>
        public void DoOpenFlow(AtPara para)
        {
            string fk_flow = para.GetValStrByKey("FK_Flow");
            int workid = para.GetValIntByKey("WorkID");
            int fk_node = para.GetValIntByKey("FK_Node");

            if (WebUser.WorkID == workid && WebUser.FK_Node == fk_node)
            {
                if (MessageBox.Show("当前流程已经打开，您想重新加载吗？",
                    "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    this.webBrowser1.GoBack();
                    return;
                }
            }

            string file = Glo.PathOfTInstall + workid + "@" + WebUser.No + ".doc";
            if (File.Exists(file) == false)
            {
                try
                {
                    FtpSupport.FtpConnection conn = Glo.HisFtpConn;
                    if (conn.DirectoryExist("/DocFlow/" + fk_flow + "/" + workid))
                    {
                        conn.SetCurrentDirectory("/DocFlow/" + fk_flow + "/" + workid);
                        conn.GetFile(workid + ".doc", file, true, FileAttributes.Archive);
                        conn.Close();
                    }
                    else
                    {
                        throw new Exception("@没有找到文件，流程文件丢失错误。");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("@打开公文错误@技术信息:" + ex.Message + "@流程编号：" + WebUser.FK_Flow);
                    return;
                }
            }

            WebUser.FK_Flow = fk_flow;
            WebUser.FK_Node = fk_node;
            WebUser.WorkID = workid;

            WebUser.RetrieveWFNode(WebUser.FK_Node);

            /*如果存在这个文件，就激活它。*/
            WebUser.WriterIt(StartFlag.DoOpenFlow, fk_flow, fk_node, workid);
            this.OpenDoc(file, false);

            
            this.Close();
        }

        /// <summary>
        /// 发起流程
        /// </summary>
        /// <param name="para"></param>
        public void DoStartFlow(AtPara para)
        {
            string fk_flow = para.GetValStrByKey("FK_Flow");
            string workid = para.GetValStrByKey("WorkID");
            string file = Glo.PathOfTInstall + workid + "@" + WebUser.No + ".doc";

            if (File.Exists(file) == false)
            {
                try
                {
                    FtpSupport.FtpConnection conn = Glo.HisFtpConn;
                    if (conn.DirectoryExist("/DocFlow/" + fk_flow + "/" + workid))
                    {
                        conn.SetCurrentDirectory("/DocFlow/" + fk_flow + "/" + workid);
                        if (conn.FileExist(WebUser.FK_Node + "@" + WebUser.No + ".doc"))
                            conn.GetFile(WebUser.FK_Node + "@" + WebUser.No + ".doc", file, true, FileAttributes.Archive);
                        else
                            file = null;
                    }
                    else
                    {
                        file = null;
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("@流程设计错误，没有为该流程维护公文模板。@技术信息:" + ex.Message + "@流程编号：" + WebUser.FK_Flow);
                    return;
                }
            }

            WebUser.FK_Flow = fk_flow;
            WebUser.WorkID = int.Parse(workid);

            /*如果存在这个文件，就激活它。*/
            WebUser.WriterIt(StartFlag.DoNewFlow, fk_flow, int.Parse(fk_flow + "01"), int.Parse(workid));
            this.OpenDoc(file, false);
            this.Close();
        }
        /// <summary>
        /// 按照模板生成公文
        /// </summary>
        /// <param name="para"></param>
        public void DoStartFlowByTemple(AtPara para)
        {
            string fk_flow = para.GetValStrByKey("FK_Flow");
            // 下载流程模板 
            FtpSupport.FtpConnection conn = Glo.HisFtpConn;
            string file = Glo.PathOfTInstall + fk_flow + "@" + DateTime.Now.ToString("MM月dd日hh时mm分ss秒") + ".doc";
            try
            {
                conn.SetCurrentDirectory("/DocFlowTemplete/");
                if (conn.FileExist(fk_flow + ".doc") == false)
                    throw new Exception("@没有为公文启动设置模板。");

                conn.GetFile(fk_flow + ".doc", file, true, FileAttributes.Archive);
                conn.Close();
            }
            catch (Exception ex)
            {
                conn.Close();
                file = null;
                MessageBox.Show("@异常信息:" + ex.Message + "\t\n@流程编号：" + WebUser.FK_Flow + "\t\n@可能的原因如下：1，设计人员没有正确的设置ftp服务器。 \t\n2，没有该流程的公文模板。");
            }

            WebUser.WorkID = 0;
            WebUser.FK_Flow = fk_flow;
            WebUser.FK_Node = int.Parse(fk_flow + "01");
            WebUser.RetrieveWFNode(WebUser.FK_Node);
            this.HisRibbon1.SetState();
            WebUser.WriterIt(StartFlag.DoNewFlow, fk_flow, int.Parse(fk_flow + "01"), 0);
            this.OpenDoc(file, false);
            this.Close();
        }
        #endregion
    }
}
