using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.Common;
using System.Net;
using System.Net.Mail;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Security.Cryptography;
using BP.Port;
using BP.En;
using BP.Sys;
using BP.DA;
using BP;
using BP.Web;
using System.Collections;
using BP.WF;

namespace SMSServices
{
    public enum ScanSta
    {
        Working,
        Pause,
        Stop
    }
    public partial class FrmMain : Form
    {
        delegate void SetTextCallback(string text);
        public FrmMain()
        {
            InitializeComponent();
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Show();
            this.notifyIcon1.Visible = true;
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.toolStripStatusLabel1.Text = "等待启动...";
            this.textBox1.Text = "等待启动...";
        }
        public ScanSta HisScanSta = ScanSta.Pause;
        /// <summary>
        /// 执行线程.
        /// </summary>
        public void RunIt()
        {
            this.HisScanSta = ScanSta.Working;
            int sec = 0;

            string dayly = "";
            string monthly = "";

            BP.Port.Emp emp = new Emp("admin");
            BP.Web.WebUser.SignInOfGener(emp);

            SMS sms = new SMS();
            sms.CheckPhysicsTable();

            while (this.HisScanSta != ScanSta.Stop)
            {
                //执行任务.
                sec++;
                if (sec == 3)
                {
                    sec = 0;
                    // 发送短信.
                    if (this.CB_Alter.Checked)
                        Console.Beep();

                    if (this.CB_GSM.Checked == true
                        && (DateTime.Now.Hour >= BP.WF.Glo.SMSSendTimeFromHour && DateTime.Now.Hour <= BP.WF.Glo.SMSSendTimeToHour))
                    {
                        #region 向内部人员发 sms.
                        string sql = "UPDATE Sys_SMS SET TEL=(SELECT TEL FROM WF_Emp WHERE WF_Emp.No=Sys_SMS.SendToEmpID ),EMAIL=(SELECT Email FROM WF_Emp WHERE WF_Emp.No=Sys_SMS.Fk_Emp )  WHERE TEL IS NULL OR " + SystemConfig.AppCenterDBLengthStr + "(TEL) <10";
                        DBAccess.RunSQL(sql);
                        sql = "SELECT Tel, Title, MyPK FROM Sys_SMS WHERE TelSta=0 AND " + SystemConfig.AppCenterDBLengthStr + "(TEL)=11";
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        string tels = "";
                        foreach (DataRow dr in dt.Rows)
                        {
                            string tel = dr[0].ToString();
                            /* 避免一号码一次通知 多次发送. */
                            if (tels.Contains("," + tel + ",") == true)
                                continue;
                            tels += "," + tel + ",";

                            string info = dr[1].ToString();
                            string mypk = dr[2].ToString();
                            if (string.IsNullOrEmpty(info.Trim()))
                                continue;

                            bool isOk = BP.Modem.Send(tel, info);
                            if (isOk == false)
                            {
                                sql = "UPDATE Sys_SMS SET TelSta=3 WHERE MyPK='" + mypk + "'";
                                this.SetText("内部人员工作待办:" + tel + "发送失败.");
                            }
                            else
                            {
                                this.SetText("内部人员工作待办:" + tel + "成功.");
                                sql = "UPDATE Sys_SMS SET TelSta=1 WHERE MyPK='" + mypk + "'";
                            }
                            DBAccess.RunSQL(sql);
                        }
                        #endregion 向内部人员发
                    }
                }

                #region 检查状态。
                if (this.HisScanSta == ScanSta.Pause)
                {
                    while (this.HisScanSta == ScanSta.Pause)
                    {
                        Thread.Sleep(1000);
                        this.toolStripStatusLabel1.Text = "暂停中";
                    }
                }

                // 休息1秒.
                Thread.Sleep(1000);

                //设置活动的状态.
                if (this.toolStripStatusLabel1.Text == "启动中>")
                    this.toolStripStatusLabel1.Text = "启动中>>";
                else if (this.toolStripStatusLabel1.Text == "启动中>>")
                    this.toolStripStatusLabel1.Text = "启动中>>>";
                else if (this.toolStripStatusLabel1.Text == "启动中>>>")
                    this.toolStripStatusLabel1.Text = "启动中>>>>";
                else if (this.toolStripStatusLabel1.Text == "启动中>>>>")
                    this.toolStripStatusLabel1.Text = "启动中>";
                else
                    this.toolStripStatusLabel1.Text = "启动中>";

                #endregion 检查状态。
            }
        }
        /// <summary>
        /// 设置内容
        /// </summary>
        /// <param name="text"></param>
        private void SetText(string text)
        {
            //写入信息.
            BP.DA.Log.DefaultLogWriteLineInfo(text);
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                try
                {
                    this.Invoke(d, new object[] { text });
                }
                catch
                {
                }
            }
            else
            {
                this.textBox1.Text += "\r\n" + text;
                this.textBox1.SelectionStart = this.textBox1.TextLength;
                this.textBox1.ScrollToCaret();
            }
        }
        private void Btn_Exit_Click(object sender, EventArgs e)
        {
            this.HisScanSta = ScanSta.Stop;
            if (this.thread != null)
                thread.Abort();
            Application.Exit();
        }
        private void Btn_Pause_Click(object sender, EventArgs e)
        {
            this.HisScanSta = ScanSta.Pause;
        }

        public Thread thread = null;
        private void Btn_Runing_Click(object sender, EventArgs e)
        {
            if (this.Btn_StartStop.Text == "启动")
            {
                if (this.CB_GSM.Checked == true)
                {
                    if (BP.Modem.Conn() == false)
                    {
                        MessageBox.Show("GSM设备没有正确的启动起来了，请检查是否链接上，或者重启动机器。");
                        return;
                    }
                }

                if (this.thread == null)
                {
                    ThreadStart ts = new ThreadStart(RunIt);
                    thread = new Thread(ts);
                    thread.Start();
                    this.Btn_StartStop.Text = "暂停";
                }
                this.HisScanSta = ScanSta.Working;
                this.SetText("服务启动 *** " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                this.Btn_StartStop.Text = "暂停";
                this.toolStripStatusLabel1.Text = "服务启动";
            }
            else
            {
                BP.Modem.Close();
                this.SetText("服务暂停 *** " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                this.HisScanSta = ScanSta.Pause;
                this.Btn_StartStop.Text = "启动";
                this.toolStripStatusLabel1.Text = "服务暂停";
            }
        }
    }
}
