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

namespace CCFlowServices
{
    public partial class FrmEfficiency : Form
    {
        delegate void SetTextCallback(string text);
        public FrmEfficiency()
        {
            InitializeComponent();
        }
        private void SetText(string text)
        {
            if (this.TB_Text.InvokeRequired)
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
                this.TB_Text.Text += "\r\n" + text;
                this.TB_Text.SelectionStart = this.TB_Text.TextLength;
                this.TB_Text.ScrollToCaret();
            }
        }
        public void AddLog(string msg)
        {
            Log.DefaultLogWriteLineInfo(msg);
            this.SetText(msg);
        }
        private void FrmEfficiency_Load(object sender, EventArgs e)
        {
        }
        private void Btn_RunIt_Click(object sender, EventArgs e)
        {
            RunFlow("024", "zhanghaicheng", int.Parse(this.TB_RunTimes.Text));
        }
        public void RunFlow(string fk_flow, string userNo, int runTimes)
        {
            //登录.
            Emp emp = new Emp(userNo);
            BP.Web.WebUser.SignInOfGener(emp);

            //删除数据.
            Flow fl = new Flow(fk_flow);
            fl.DoDelData();
            this.AddLog("********************* 对流程:" + fl.Name + ", 进行" + runTimes + "次执行的单个用户的效率执行测试.");

            //执行一次预热.
            SendReturnObjs objs = BP.WF.Dev2Interface.Node_StartWork(fk_flow, null, null, 0, null, 0, null);

            //执行发起.
            DateTime dtStart = DateTime.Now;
            this.AddLog("=== 开始发起" + runTimes + "流程.");
            int num = 0;
            for (int i = 0; i < runTimes; i++)
            {
                num++;
                if (num == 100)
                {
                    this.SetText("启动了:" + i + "个流程");
                    num = 0;
                }

                BP.WF.Dev2Interface.Node_StartWork(fk_flow, null, null, 0, null, 0, null);
            }
            DateTime dtEnd = DateTime.Now;
            TimeSpan ts = dtEnd - dtStart;
            this.AddLog("流程发起执行结束,合计执行秒:" + ts.TotalSeconds);


            //执行中间.
            Log.DefaultLogWriteLineInfo("=== 中间点开始执行");
            DataTable dt = BP.WF.Dev2Interface.DB_GenerEmpWorksOfDataTable(WebUser.No, WFState.Runing, fk_flow);
            num = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                num++;
                if (num == 100)
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
    }
}
