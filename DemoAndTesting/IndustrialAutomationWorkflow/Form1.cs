using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IndustrialAutomationWorkflow
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 开始执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Do_Click(object sender, EventArgs e)
        {
            string url = this.TB_Url.Text; // webservices url.
            string userNo = this.TB_StartUserNo.Text; //发起人.
            string para = this.TB_Para.Text; //参数.
            string flowNo = this.TB_FlowNo.Text; //流程编号.

            ccbpmAPI.IndustrialAutomationWorkflowWSAPISoapClient myws = null;
            try
            {
                myws = Glo.GetSoapClientInstance(url);
                string text = myws.HelloWorld();
            }
            catch
            {
                MessageBox.Show("WebServices连接错误，请检查:[" + url + "]输入是否正确。");
                return;
            }

            //登录并获得校验码.
            string sid = myws.Port_Login("liyan", "pub");

            //产生一个workid.
            Int64 workid = myws.Node_CreateBlankWork(userNo, sid, flowNo);

            //开始节点, 因为ccbpm约定开始节点必须是 流程编号+‘01’转化成int 类型作为开始节点编号。
            int startNodeID = int.Parse(flowNo + "01");

            //执行发起,获得指定格式的发起结果提示信息.
            string specText = myws.Node_SendWork(userNo, sid, flowNo, startNodeID, workid, int.Parse(flowNo + "02"), "liyan", para);

            //根据这个字符串, 生成一个发送结果对象。
            BP.WF.SendReturnObjs objs = new BP.WF.SendReturnObjs(specText);

            //流程运行参数信息.
            string infoMsg = "";
            foreach (BP.WF.SendReturnObj obj in objs)
            {
                infoMsg += "\t\n@" + obj.MsgFlag + " = " + obj.MsgOfText;
            }
            MessageBox.Show(infoMsg, "发送信息");

            FrmInputParams frm = new FrmInputParams();

            //开始循环自动向下发送.
            while (true)
            {
                if (objs.IsStopFlow)
                {
                    MessageBox.Show("流程运行到{"+objs.VarCurrNodeID+" "+objs.VarCurrNodeName+"}结束了");
                    break;
                }

                // 流程运行参数信息.
                infoMsg = "";
                foreach (BP.WF.SendReturnObj obj in objs)
                {
                    infoMsg += "\t\n@" + obj.MsgFlag + " = " + obj.MsgOfText;
                }
                MessageBox.Show(infoMsg, "发送信息,流程已经运行到:" + objs.VarCurrNodeID + " " + objs.VarCurrNodeName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                #region 如果发送成功，就跳出这个循环。
                while (true)
                {
                    frm.Text = " 请为:" + objs.VarCurrNodeID + " " + objs.VarCurrNodeName + " 设置输入参数。";
                    //参数提示录入，added by liuxc,2015-10-21 
                    if (frm.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
                        para = string.Empty;
                    else
                        para = frm.Params;

                    try
                    {
                        //发送到下一个节点上去.
                        specText = myws.Node_SendWork(userNo, sid, flowNo, objs.VarToNodeID, workid, 0, null, para);

                        //根据约定格式的字符串，初始化一个发送结果实例。
                        objs = new BP.WF.SendReturnObjs(specText);
                        break; //发送成功，就跳出这个循环。
                    }
                    catch (Exception ex)
                    {
                        var v = MessageBox.Show(ex.Message +" \t\n 您需要重新输入参数吗？" ,
                              "发送失败,有可能是参数不完整导致的", MessageBoxButtons.OKCancel);
                        if (v == System.Windows.Forms.DialogResult.Cancel)
                            break;
                        continue; //重新输入参数.
                    }
                }
                #endregion 如果发送成功，就跳出这个循环。

            }
        }
    }
}
