using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices; 

namespace BP
{
    public enum GSMSta
    {
        /// <summary>
        /// 停滞
        /// </summary>
        Stop,
        /// <summary>
        /// 运行
        /// </summary>
        Runing
    }
    public class Modem
    {


        #region API
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        [DllImport("sms.dll", EntryPoint = "Sms_Connection")]
        public static extern uint Sms_Connection(string CopyRight, uint Com_Port, uint Com_BaudRate, out string Mobile_Type, out string CopyRightToCOM);

        [DllImport("sms.dll", EntryPoint = "Sms_Disconnection")]
        public static extern uint Sms_Disconnection();

        [DllImport("sms.dll", EntryPoint = "Sms_Send")]
        public static extern uint Sms_Send(string Sms_TelNum, string Sms_Text);

        [DllImport("sms.dll", EntryPoint = "Sms_Receive")]
        public static extern uint Sms_Receive(string Sms_Type, out string Sms_Text);

        [DllImport("sms.dll", EntryPoint = "Sms_Delete")]
        public static extern uint Sms_Delete(string Sms_Index);

        [DllImport("sms.dll", EntryPoint = "Sms_AutoFlag")]
        public static extern uint Sms_AutoFlag();

        [DllImport("sms.dll", EntryPoint = "Sms_NewFlag")]
        public static extern uint Sms_NewFlag(); 
        #endregion
        /// <summary>
        /// 状态
        /// </summary>
        public static GSMSta GSMState = GSMSta.Stop;
        /// <summary>
        /// 连接到设备
        /// </summary>
        /// <returns></returns>
        public static bool Conn()
        {
            try
            {
                String TypeStr = "";
                String CopyRightToCOM = "";
                String CopyRightStr = "//上海迅赛信息技术有限公司,网址www.xunsai.com//";
                for (int i = 0; i < 10; i++)
                {
                    if (Sms_Connection(CopyRightStr, uint.Parse(i.ToString()), 9600, out TypeStr, out CopyRightToCOM) == 1)
                    {
                        ///5为串口号，0为红外接口，1,2,3,...为串口
                        GSMState = GSMSta.Runing;
                        Console.Beep();
                        return true;
                    }
                    //停止5秒.
                    System.Threading.Thread.Sleep(5000);
                }
                GSMState = GSMSta.Stop;
                return false;
            }
            catch(Exception ex)
            {
                MessageBox.Show("在连接设备时出现错误，请检查是否插好。\t\n技术信息:"+ex.Message, 
                    "提示", MessageBoxButtons.OK);
                return false;
            }
        }
        /// <summary>
        /// 断开链接
        /// </summary>
        /// <returns></returns>
        public static void Close()
        {
            Sms_Disconnection();
        }
        /// <summary>
        /// 执行发送
        /// </summary>
        /// <param name="tel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool Send(string tel, string msg)
        {
            if (Sms_Send(tel, msg) == 1)
                return true;
            else
                return false;
        }
        public Modem()
        {
        }
    }
}
