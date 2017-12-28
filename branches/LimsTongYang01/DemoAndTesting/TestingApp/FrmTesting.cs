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
using BP.WF;
using BP.Port;
using BP.En;
using BP.Sys;
using BP.DA;
using BP;
using BP.Web;

namespace TestingApp
{
    public partial class FrmTesting : Form
    {
        delegate void SetTextCallback(string text);

        public void Test_Insert_Model1()
        {
            DBAccess.RunSQL("DELETE CN_Area");
            int i = 0;
            DateTime dtNow = DateTime.Now;
            while (i != 100000)
            {
                i++;
                string sql = " INSERT CN_Area (No,Name) VALUES ('" + i + "' , '" + i + "')";
                DBAccess.RunSQL(sql);
            }
            DateTime dtEnd = DateTime.Now;

            TimeSpan ts = dtEnd - dtNow;
            MessageBox.Show(ts.TotalSeconds.ToString() + " - " + ts.TotalMilliseconds.ToString());
        }

        public void Test_Insert_Model2()
        {
            DBAccess.RunSQL("DELETE CN_Area");
            SqlConnection conn = new SqlConnection(SystemConfig.AppCenterDSN);
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.ConnectionString = SystemConfig.AppCenterDSN;
                conn.Open();
            }
            try
            {

                int i = 0;
                DateTime dtNow = DateTime.Now;
                while (i != 100000)
                {
                    i++;
                    string sql = " INSERT CN_Area (No,Name) VALUES ('" + i + "' , '" + i + "')";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.Text;
                     cmd.ExecuteNonQuery();

                }
                DateTime dtEnd = DateTime.Now;

                TimeSpan ts = dtEnd - dtNow;
                MessageBox.Show(ts.TotalSeconds.ToString() + " - " + ts.TotalMilliseconds.ToString());
                conn.Close();
            }
            catch (System.Exception ex)
            {
                conn.Close();
            }
            finally
            {
                conn.Close();
            }
        }


        public void Test_T_1()
        {
            DBAccess.RunSQL("DELETE CN_Area");
            int i = 0;
            DateTime dtNow = DateTime.Now;

            DBAccess.DoTransactionBegin();
            while (i != 10)
            {
                i++;
                string sql = " INSERT CN_Area (No,Name) VALUES ('" + i + "' , '" + i + "')";
                DBAccess.RunSQL(sql);
            }
            DBAccess.DoTransactionCommit();

            DateTime dtEnd = DateTime.Now;
            TimeSpan ts = dtEnd - dtNow;
            MessageBox.Show(ts.TotalSeconds.ToString() + " - " + ts.TotalMilliseconds.ToString());

        }

        public void Test_T_2()
        {
            DBAccess.RunSQL("DELETE CN_Area");
            SqlConnection conn = new SqlConnection(SystemConfig.AppCenterDSN);
            if (conn.State != System.Data.ConnectionState.Open)
            {
                conn.ConnectionString = SystemConfig.AppCenterDSN;
                conn.Open();
            }
            SqlCommand cmd = new SqlCommand("BEGIN TRANSACTION", conn);
            try
            {
                cmd.ExecuteNonQuery();

                int i = 0;
                DateTime dtNow = DateTime.Now;
                while (i != 10)
                {
                    i++;
                    string sql = " INSERT CN_Area (No,Name) VALUES ('" + i + "' , '" + i + "')";

                    cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                DateTime dtEnd = DateTime.Now;
                TimeSpan ts = dtEnd - dtNow;

                cmd.CommandText = "commit transaction";
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch
            {
                cmd.CommandText = "rollback transaction";
                cmd.ExecuteNonQuery();
                //DBAccess.DoTransactionRollback();
                conn.Close();
            }
        }
        public FrmTesting()
        {
            //Test_T_1();
            //return;
            //Test_T_2();
            //return;
            ///* 两者相差在 20% 左右.*/
            //this.Test_Insert_Model1();
            //this.Test_Insert_Model2();
            //return;

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
            // this.toolStripStatusLabel1.Text = "服务停止状态...";
            this.toolStripStatusLabel1.Text = "服务暂停";
            this.textBox1.Text = "服务停止...";
            this.Btn_StartStop.Text = "启动";
        }

        Thread thread = null;
        private void Btn_StartStop_Click(object sender, EventArgs e)
        {
            if (this.Btn_StartStop.Text == "启动")
            {
                if (this.thread == null)
                {
                    ThreadStart ts = new ThreadStart(RunIt);
                    thread = new Thread(ts);
                    thread.Start();
                    this.Btn_StartStop.Text = "暂停";
                }
                this.HisScanSta = ScanSta.Working;
                this.SetText("服务启动***********" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                this.Btn_StartStop.Text = "暂停";
                this.toolStripStatusLabel1.Text = "服务启动";
            }
            else
            {
                this.SetText("服务暂停***********" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                this.HisScanSta = ScanSta.Pause;
                this.Btn_StartStop.Text = "启动";
                this.toolStripStatusLabel1.Text = "服务暂停";
            }
        }
        public ScanSta HisScanSta = ScanSta.Pause;
        /// <summary>
        /// 执行线程.
        /// </summary>
        public void RunIt()
        {
            HisScanSta = ScanSta.Working;
            while (true)
            {
               System.Threading.Thread.Sleep(20000);
                while (this.HisScanSta == ScanSta.Pause)
                {
                    System.Threading.Thread.Sleep(3000);
                    Console.Beep();
                }

                this.SetText("*** 开始执行:");

                this.RunFlow("024", "zhanghaicheng", 1000000000);


                System.Threading.Thread.Sleep(1000);
                switch (this.toolStripStatusLabel1.Text)
                {
                    case "服务启动":
                        this.toolStripStatusLabel1.Text = "服务启动..";
                        break;
                    case "服务启动..":
                        this.toolStripStatusLabel1.Text = "服务启动........";
                        break;
                    case "服务启动....":
                        this.toolStripStatusLabel1.Text = "服务启动.............";
                        break;
                    default:
                        this.toolStripStatusLabel1.Text = "服务启动";
                        break;
                }
            }
        }
        private void SetText(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);

                try
                {
                    this.Invoke(d, new object[] { text+"\t\n" });
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
        public void RunFlow(string fk_flow, string userNo, int runTimes)
        {
            //登录.
            Emp emp = new Emp(userNo);
            BP.Web.WebUser.SignInOfGener(emp);

            //删除数据.
            Flow fl = new Flow(fk_flow);
          //  fl.DoDelData();
            this.AddLog("********************* 对流程:" + fl.Name + ", 进行" + runTimes + "次执行的单个用户的效率执行测试.");

            //执行一次预热.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_StartWork(fk_flow, null, null, 0, null, 0, null);

            //执行发起.
            DateTime dtStart = DateTime.Now;
            DateTime dtEnd = DateTime.Now;
            this.AddLog("=== 开始发起" + runTimes + "流程.");
            int num = 0;
            DateTime dt100 = DateTime.Now;
            for (int i = 0; i < runTimes; i++)
            {
                num++;
                if (num == 100)
                {
                    dtEnd = DateTime.Now;
                    TimeSpan myts = dtEnd - dtStart;

                    TimeSpan myts100 = dtEnd - dt100;
                    string info = "启动:" + i + "个流程,100花费:(" + myts100.TotalSeconds+ "),共花费:" + myts.TotalSeconds + "秒," + dtEnd.ToString("HH:mm:ss");
                    info += "\t\n ";
                    this.SetText(info);
                    if (this.HisScanSta == ScanSta.Stop)
                    {
                        Log.DefaultLogWriteLineInfo(this.textBox1.Text);
                        return;
                    }
                    dt100 = DateTime.Now;
                    num = 0;
                }
                BP.WF.Dev2Interface.Node_StartWork(fk_flow, null, null, 0, null, 0, null);
            }
             dtEnd = DateTime.Now;
            TimeSpan ts = dtEnd - dtStart;
            this.AddLog("流程发起执行结束,合计执行秒:" + ts.TotalSeconds);

            //执行中间.
            Log.DefaultLogWriteLineInfo("=== 中间点开始执行");
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No, WFState.Runing, fk_flow);
            num = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                num++;
                if (num == 101)
                {
                    this.SetText("中间点开始执行:" + i + "个数据");
                    num = 0;
                }
                Int64 workid = Int64.Parse(dt.Rows[i]["WorkID"].ToString());
                BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);
            }
            DateTime dtEnd2 = DateTime.Now;
            ts = dtEnd2 - dtEnd;

            this.AddLog("中间点执行完成, 执行中间点:合计秒:" + ts.TotalSeconds);

            //执行结束点.
            this.AddLog("=== 结束点开始执行");
            dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No, WFState.Runing, fk_flow);
            num = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                num++;
                if (num == 100)
                {
                    this.SetText("结束点开始执行:" + i + "个数据");
                    num = 0;
                }
                Int64 workid = Int64.Parse(dt.Rows[i]["WorkID"].ToString());
                BP.WF.Dev2Interface.Node_SendWork(fk_flow, workid);
            }
            DateTime dtEnd3 = DateTime.Now;
            ts = dtEnd3 - dtEnd2;
            this.AddLog("结束点开始执行完成执行结束点:合计秒:" + ts.TotalSeconds);
        }
        public void AddLog(string msg)
        {
            this.SetText(msg);
        }
        private void Btn_Exit_Click(object sender, EventArgs e)
        {
            this.HisScanSta = ScanSta.Stop;
            thread.Abort();
            this.Close();
        }

        private void FrmMain_SizeChanged(object sender, EventArgs e)
        {
            this.Hide();
            this.notifyIcon1.Visible = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1_Click(null, null);
        }

        private void Btn_ToolBox_Click(object sender, EventArgs e)
        {
            CCFlowServices.ToolBox tb = new CCFlowServices.ToolBox();
            tb.Show();
        }
        private void FrmTesting_Load(object sender, EventArgs e)
        {
        
        }

        private void Btn_OpenData_Click(object sender, EventArgs e)
        {

        }
        private void Btn_YRData_Click(object sender, EventArgs e)
        {
            ThreadStart ts = new ThreadStart(AddData);
            Thread thread = new Thread(ts);
            thread.Start();

            ThreadStart ts2 = new ThreadStart(AddData2000);
            Thread thread2 = new Thread(ts2);
            thread2.Start();

            ThreadStart ts3 = new ThreadStart(AddData3);
            Thread thread3 = new Thread(ts3);
            thread3.Start();

            ThreadStart ts4 = new ThreadStart(AddData4);
            Thread thread4 = new Thread(ts4);
            thread4.Start();
        }
        private void InsertIt(string emp)
        {
            try
            {
                Int32 oid = DBAccess.GenerOIDByGUID();
                string dbstr = SystemConfig.AppCenterDBVarStr;
                Paras ps = new Paras();
                ps.SQL = "INSERT INTO ND24Rpt (OID,Title,FlowStarter) VALUES(" + dbstr + "OID," + dbstr + "Title," + dbstr + "FlowStarter)";
                ps.Add("OID", oid);
                ps.Add("Title", emp + DataType.CurrentData);
                ps.Add("FlowStarter", emp);
                ps.Add("WFState", 1);
                DBAccess.RunSQL(ps);

                ps = new Paras();
                ps.SQL = "INSERT INTO ND24Track (MyPK,ActionType,ActionTypeText,EmpFrom) VALUES(" + dbstr + "MyPK," + dbstr + "ActionType," + dbstr + "ActionTypeText," + dbstr + "EmpFrom)";
                ps.Add("MyPK", oid);
                ps.Add("ActionType", 1);
                ps.Add("ActionTypeText", emp);
                ps.Add("EmpFrom", emp);
                DBAccess.RunSQL(ps);

                ps = new Paras();
                ps.SQL = "INSERT INTO WF_GenerWorkFlow (WorkID,FID,FK_Flow,Starter,FK_Node) VALUES(" + dbstr + "WorkID," + dbstr + "FID," + dbstr + "FK_Flow," + dbstr + "Starter," + dbstr + "FK_Node)";
                ps.Add("WorkID", oid);
                ps.Add("FID", 1);
                ps.Add("FK_Flow", "024");
                ps.Add("Starter", emp);
                ps.Add("FK_Node", 2401);
                DBAccess.RunSQL(ps);


                ps = new Paras();
                ps.SQL = "INSERT INTO WF_GenerWorkerlist (WorkID,FK_Node,FK_Emp) VALUES(" + dbstr + "WorkID," + dbstr + "FK_Node," + dbstr + "FK_Emp)";
                ps.Add("WorkID", oid);
                ps.Add("FK_Emp", emp);
                ps.Add("FK_Node", 2401);
                DBAccess.RunSQL(ps);
            }
            catch(Exception ex)
            {
                Log.DefaultLogWriteLineError(ex.Message);
            }
        }
        /// <summary>
        /// 增加1千万条数据.
        /// </summary>
        public void AddData()
        {
            Emps emps = new Emps();
            emps.RetrieveAll();
            int num = 0;
            for (int i = 0; i < 10000000; i++)
            {
                foreach (Emp item in emps)
                {
                    num++;
                    if (num == 2000)
                    {
                        this.SetText("执行了:2000条");
                        num = 0;
                    }
                    InsertIt(item.No);
                }
            }
        }
        public void AddData2000()
        {
            Emps emps = new Emps();
            emps.RetrieveAll();
            int num = 0;
            for (int i = 0; i < 10000000; i++)
            {
                foreach (Emp item in emps)
                {
                    num++;
                    if (num == 2000)
                    {
                        this.SetText("执行了:2000条"+DateTime.Now.ToString("dd:mm:ss"));
                        num = 0;
                    }
                    InsertIt(item.No);
                }
            }
        }

        public void AddData3()
        {
            Emps emps = new Emps();
            emps.RetrieveAll();
            int num = 0;
            for (int i = 0; i < 10000000; i++)
            {
                foreach (Emp item in emps)
                {
                    num++;
                    if (num == 2000)
                    {
                        this.SetText("第3条执行:2000");
                    }
                    InsertIt(item.No);
                }
            }
        }

        public void AddData4()
        {
            Emps emps = new Emps();
            emps.RetrieveAll();
            int num = 0;
            for (int i = 0; i < 10000000; i++)
            {
                foreach (Emp item in emps)
                {
                    num++;
                    if (num == 2000)
                    {
                        this.SetText("第4条执行:2000");
                        num = 0;
                    }
                    InsertIt(item.No);
                }
            }
        }

        private void Btn_Run024_Click(object sender, EventArgs e)
        {
            //首先清除所有的流程.
            Flow fl = new Flow("023");
            fl.DoDelData();

            //让zhanghaicheng 登录.
            BP.WF.Dev2Interface.Port_Login("zhanghaicheng");

            #region 测试创建WorkID.
            //创建第一个workid. 让他启动起来.
            Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork("024", null, null, BP.Web.WebUser.No, "");

            this.SetText("==开始测试024流程的三个节点效率....");
            this.SetText("测试创建WorkID的效率， BP.WF.Dev2Interface.Node_CreateBlankWork.");

            System.Threading.Thread.Sleep(3000);
            Console.Beep();
            BP.Sys.Glo.WriteLineInfo("==开始测试024流程的三个节点效率....");

            DateTime dtStart = DateTime.Now;
            // 开始执行运行.
            for (int i = 0; i < 1000; i++)
            {
                workid = BP.WF.Dev2Interface.Node_CreateBlankWork("024", null, null, BP.Web.WebUser.No, "");
                BP.WF.Dev2Interface.Node_SendWork("024", workid);
            }

            DateTime dtEnd = DateTime.Now;
            TimeSpan ts = dtEnd - dtStart;

            decimal d = 1000 / ts.Seconds;
            decimal d2 = ts.Seconds / 1000;

            this.SetText("Node_CreateBlankWork: 创建WorkID 1000次，使用时间为:" + ts.Seconds + " 平均每秒创建" + d.ToString("0.00") + "条, 创建一次耗费:" + d2.ToString("0.000") + "秒");

            BP.Sys.Glo.WriteLineInfo("Node_CreateBlankWork: 创建WorkID 1000次，使用时间为:" + ts.Seconds + " 平均每秒创建" + d.ToString("0.00") + "条, 创建一次耗费:" + d2.ToString("0.000") + "秒");
            #endregion 测试创建WorkID.

            #region 执行开始节点的发送.
            this.SetText("****** 测试 执行开始节点的发送， BP.WF.Dev2Interface.Node_CreateBlankWork.");
            System.Threading.Thread.Sleep(3000);
            Console.Beep();

            //让周朋登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable("zhoupeng", "024");
            dtStart = DateTime.Now;
            foreach (DataRow dr in dt.Rows)
            {
                workid = Int64.Parse(dr["WorkID"].ToString());
                BP.WF.Dev2Interface.Node_SendWork("024", workid);
            }
            dtEnd = DateTime.Now;
            ts = dtEnd - dtStart;
            this.SetText("Node_SendWork: 发起1000次，使用时间为:" + ts.Seconds + " 平均每秒发起" + d.ToString("0.000") + "条, 发起一次耗费:" + d2.ToString("0.000") + "秒");
            BP.Sys.Glo.WriteLineInfo("Node_SendWork: 发起  1000次，使用时间为:" + ts.Seconds + " 平均每秒发起" + d.ToString("0.000") + "条, 发起一次耗费:" + d2.ToString("0.000") + "秒");

            BP.Sys.Glo.WriteLineInfo("****************** over *************************");
            #endregion 执行开始节点的发送.

        }
    }
}
