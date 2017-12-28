using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BP.WF;
using BP.WF.Data;
using BP.En;
using BP.Port;
using System.Threading;

namespace TestingApp
{
    public partial class TestRun : Form
    {
        public TestRun()
        {
            InitializeComponent();
        }

        private void TestRun_Load(object sender, EventArgs e)
        {

        }
        Thread thread;

        /// <summary>
        /// 测试的流程数量。
        /// </summary>
        public int TestNum = 2000;
        private void button1_Click(object sender, EventArgs e)
        {
            Console.Beep();
            MessageBox.Show("开始执行系统会出现假死的情况，请不要管，等待执行完成.");

            label4.Text = "==开始测试024流程的三个节点效率....测试实例数目:" + this.TestNum;
            thread = new Thread(RunText);

            thread.Start();

            ////首先清除所有的流程.
            //Flow fl = new Flow("024");
            //fl.DoDelData();

            ////让zhanghaicheng 登录.
            //BP.WF.Dev2Interface.Port_Login("zhoupeng");

            //#region 测试创建WorkID.
            ////创建第一个workid. 让他启动起来.
            //Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork("024", null, null, BP.Web.WebUser.No, "");

            //BP.Sys.Glo.WriteLineInfo("==开始测试024流程的三个节点效率....");
            //richTextBox1.Text += "==开始测试024流程的三个节点效率....\r";
            //DateTime dtStart = DateTime.Now;
            //// 开始执行运行.
            //try
            //{
            //    for (int i = 0; i < this.TestNum; i++)
            //    {
            //        richTextBox1.Text += "正在创建流程024:Idx:" + i;

            //        //创建workid.
            //        workid = BP.WF.Dev2Interface.Node_CreateBlankWork("024", null, null, BP.Web.WebUser.No, "");

            //        richTextBox1.Text += ",WorkID="+workid;
            //        //向下发送.
            //        BP.WF.Dev2Interface.Node_SendWork("024", workid);

            //        richTextBox1.Text += "发送成功;\r";
            //    }
            //}
            //catch (Exception ex)
            //{
            //    richTextBox1.Text += "错误:"+ex.Message+"\r";
            //    MessageBox.Show(ex.Message);
            //}

            //DateTime dtEnd = DateTime.Now;
            //TimeSpan ts = dtEnd - dtStart;

            //decimal d = 1000 / ts.Seconds;
            //decimal d2 = ts.Seconds / this.TestNum;
            //d2 = Math.Round(d2, 5);

            //string msg = "发起流程" + this.TestNum + "次，使用时间为:" + ts.Seconds + " 平均每秒创建" + d.ToString() + "条.";
            //BP.Sys.Glo.WriteLineInfo(msg);

            //this.label1.Text = msg;
            //#endregion 测试创建WorkID.

            //this.button2_Click(null, null);
            //this.button3_Click(null, null);


        }

        delegate void ChangeText(string message);
        delegate void ChageLabel1(string message);
        public void ChangeRichTextBox1(string result)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ChangeText(ChangeRichTextBox1), result);
            }
            else
            {
                this.richTextBox1.Text = result;
            }
        }

        public void ChangeLabel1(string result)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ChangeText(ChangeLabel1), result);
            }
            else
            {
                this.label1.Text = "发起结果："+result;
                thread.Abort();
                thread = new Thread(RunButton2);
                thread.Start();
            }
        }

        public void ChangeLabel2(string result)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ChangeText(ChangeLabel2), result);
            }
            else
            {
                this.label2.Text ="中间结果:"+result;
                thread.Abort();
                thread = new Thread(RunButton3);
                thread.Start();
            }
        }

        public void ChangeLabel3(string result)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ChangeText(ChangeLabel3), result);
            }
            else
            {
                this.label3.Text ="结束结果:"+ result;
                thread.Abort();
            }
        }





        private void RunText()
        {
            try
            {
                //首先清除所有的流程.
                Flow fl = new Flow("024");
                fl.DoDelData();

                //让zhanghaicheng 登录.
                BP.WF.Dev2Interface.Port_Login("zhoupeng");

                #region 测试创建WorkID.
                //创建第一个workid. 让他启动起来.
                Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork("024", null, null, BP.Web.WebUser.No, "");

                BP.Sys.Glo.WriteLineInfo("==开始测试024流程的三个节点效率....测试实例数目:" + this.TestNum);
                string message = "==开始测试024流程的三个节点效率....测试实例数目:" + this.TestNum + "\r";
                ChangeRichTextBox1(message);
                DateTime dtStart = DateTime.Now;
                // 开始执行运行.
                try
                {
                    for (int i = 0; i < this.TestNum; i++)
                    {

                        //创建workid.
                        workid = BP.WF.Dev2Interface.Node_CreateBlankWork("024", null, null, BP.Web.WebUser.No, "");

                        //向下发送.
                        BP.WF.Dev2Interface.Node_SendWork("024", workid);

                        ChangeRichTextBox1("创建流程024:Idx:" + i + "WorkID=" + workid + ",发送成功;\r");
                    }
                }
                catch (Exception ex)
                {
                    richTextBox1.Text += "错误:" + ex.Message + "\r";
                    MessageBox.Show(ex.Message);
                }

                DateTime dtEnd = DateTime.Now;
                TimeSpan ts = dtEnd - dtStart;

                decimal d = 1000 / ts.Seconds;
                decimal d2 = ts.Seconds / this.TestNum;
                d2 = Math.Round(d2, 5);

                string msg = "发起流程" + this.TestNum + "次，使用时间为:" + ts.Seconds + " 平均每秒创建" + d.ToString() + "条.";
                BP.Sys.Glo.WriteLineInfo(msg);

                ChangeLabel1(msg);
                #endregion 测试创建WorkID.
            }
            catch (Exception ex)
            {
                ChangeRichTextBox1("异常:" + ex.ToString() + "\r");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (sender != null)
                MessageBox.Show("开始执行中间点的发送，开始执行系统会出现假死的情况，请不要管，等待执行完成.");

            //#region 执行开始节点的发送.
            //Console.Beep();

            ////让周朋登录.
            //BP.WF.Dev2Interface.Port_Login("zhoupeng");
            //DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable("zhoupeng", "024");
            ////if (dt.Rows.Count!=1000)
            ////{
            ////    MessageBox.Show("你需要首先执行第一个节点，然后在执行第2个节点，最后执行第3个节点。"+dt.Rows.Count);
            ////    return;
            ////}
            //DateTime dtStart = DateTime.Now;
            //foreach (DataRow dr in dt.Rows)
            //{
            //    Int64 workid = Int64.Parse(dr["WorkID"].ToString());
            //    BP.WF.Dev2Interface.Node_SendWork("024", workid);
            //}
            //DateTime dtEnd = DateTime.Now;
            //TimeSpan ts = dtEnd - dtStart;

            //decimal d = 1000 / ts.Seconds;
            //decimal d2 = ts.Seconds / this.TestNum;

            //string msg = "中间点发起" + this.TestNum + "次，使用时间为:" + ts.Seconds + " 平均每秒执行" + d.ToString("0.000") + "条.";
            //BP.Sys.Glo.WriteLineInfo(msg);

            //BP.Sys.Glo.WriteLineInfo("****************** over *************************");

            //this.label2.Text = msg;
            //#endregion 执行开始节点的发送.

        }

        private void RunButton2()
        {
            #region 执行开始节点的发送.
            Console.Beep();

            //让周朋登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable("zhoupeng", "024");
            //if (dt.Rows.Count!=1000)
            //{
            //    MessageBox.Show("你需要首先执行第一个节点，然后在执行第2个节点，最后执行第3个节点。"+dt.Rows.Count);
            //    return;
            //}
            DateTime dtStart = DateTime.Now;
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                BP.WF.Dev2Interface.Node_SendWork("024", workid);
                ChangeRichTextBox1("发送中间节点:" + workid + ",发送成功!");

            }
            DateTime dtEnd = DateTime.Now;
            TimeSpan ts = dtEnd - dtStart;

            decimal d = 1000 / ts.Seconds;
            decimal d2 = ts.Seconds / this.TestNum;

            string msg = "中间点发起" + this.TestNum + "次，使用时间为:" + ts.Seconds + " 平均每秒执行" + d.ToString("0.000") + "条.";
            BP.Sys.Glo.WriteLineInfo(msg);

            BP.Sys.Glo.WriteLineInfo("****************** over *************************");


            ChangeLabel2(msg);
            #endregion 执行开始节点的发送.
        }

        private void RunButton3()
        {
            //让周朋登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable("zhoupeng", "024");
            DateTime dtStart = DateTime.Now;
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                BP.WF.Dev2Interface.Node_SendWork("024", workid);
                ChangeRichTextBox1("发送最后节点:" + workid + ",发送成功!");
            }
            DateTime dtEnd = DateTime.Now;
            TimeSpan ts = dtEnd - dtStart;

            decimal d = 1000 / ts.Seconds;
            decimal d2 = ts.Seconds / this.TestNum;

            string msg = "最后一个节点" + this.TestNum + "次，使用时间为:" + ts.Seconds + " 平均每秒执行" + d.ToString("0.000") + "条.";
            BP.Sys.Glo.WriteLineInfo(msg);

            BP.Sys.Glo.WriteLineInfo("****************** over *************************");
            ChangeLabel3(msg);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (sender != null)
                MessageBox.Show("开始执行中间点的发送，开始执行系统会出现假死的情况，请不要管，等待执行完成.");

            #region 执行开始节点的发送.
            Console.Beep();

            //让周朋登录.
            BP.WF.Dev2Interface.Port_Login("zhoupeng");
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable("zhoupeng", "024");
            DateTime dtStart = DateTime.Now;
            foreach (DataRow dr in dt.Rows)
            {
                Int64 workid = Int64.Parse(dr["WorkID"].ToString());
                BP.WF.Dev2Interface.Node_SendWork("024", workid);
            }
            DateTime dtEnd = DateTime.Now;
            TimeSpan ts = dtEnd - dtStart;

            decimal d = 1000 / ts.Seconds;
            decimal d2 = ts.Seconds / this.TestNum;

            string msg = "最后一个节点" + this.TestNum + "次，使用时间为:" + ts.Seconds + " 平均每秒执行" + d.ToString("0.000") + "条.";
            BP.Sys.Glo.WriteLineInfo(msg);

            BP.Sys.Glo.WriteLineInfo("****************** over *************************");

            this.label3.Text = msg;
            #endregion 执行开始节点的发送.
        }




    }
}
