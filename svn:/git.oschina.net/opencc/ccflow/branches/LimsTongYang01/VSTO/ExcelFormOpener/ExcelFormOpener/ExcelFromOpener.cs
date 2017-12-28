using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExcelFormOpener
{
	public partial class ExcelFromOpener : Form
	{
		string[] args = null;
		public ExcelFromOpener()
		{
			InitializeComponent();
			Log("缺少参数，无法打开Excel！");
		}
		public ExcelFromOpener(string[] args)
		{
			InitializeComponent();

			this.args = args;
#if DEBUG
			Log("参数个数：" + args.Length);
			Log("完整参数：" + string.Join("|", args));
#endif
			//x TODO：检查是否安装了excel 交给try处理

			//检查进程是否已存在
			if (!IsExistExcelProcess())
				if(OpenExcelForm())
					this.Close();
		}

		/// <summary>
		/// 检查进程是否已存在
		/// </summary>
		/// <returns></returns>
		private bool IsExistExcelProcess()
		{
			System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
			foreach (Process process in processes)
			{
				if (process.ProcessName.ToUpper() == "EXCEL")
				{
					Log("错误：请关闭所有已经打开的excel，然后点击下方的“重试”按钮打开excel表单。");
					Log("（若您已关闭所有excel，仍然出现该提示，请到“任务管理器”-“进程”中找到并结束“EXCEL.EXE”，然后点击“重试”打开excel表单）");
					this.btnRetry.Enabled = true;
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 打开excel表单
		/// </summary>
		/// <returns></returns>
		private bool OpenExcelForm()
		{
			var pArgs = args[0].Replace("excelform://", "");
			Log("打开excel：参数：" + pArgs);
			try
			{
				//x System.Diagnostics.Process.Start(args[0]); //可行，但能否传递参数？
				//x System.Diagnostics.Process.Start(args[0] + " /r"); //不可行

				var process = System.Diagnostics.Process.Start("excel", pArgs); //可行

				//System.Diagnostics.Process.Start("D:\\PROGRA~2\\MICROS~3\\Office14\\PROTOC~1.EXE /r ",pArgs); //?

				//System.Diagnostics.Process.Start("D:\\Program Files (x86)\\Microsoft Office\\Office14\\EXCEL.EXE /r", pArgs); //可行
				//System.Diagnostics.Process.Start("D:\\Program Files (x86)\\Microsoft Office\\Office14\\EXCEL.EXE", pArgs); //系统找不到指定的文件

				//x TODO: excel关闭时关闭本opener 成功打开excel即关闭本opener

				this.Text = "打开Excel表单成功！";
				Log("打开Excel表单成功！");
				return true;
			}
			catch (Exception err)
			{
				this.Text = "打开Excel表单失败！";
				Log("打开Excel表单失败！错误信息：" + err.Message);
				return false;
			}
		}

		/// <summary>
		/// 关闭该进程
		/// </summary>
		private void Close()
		{
#if !DEBUG
			//表单正常打开后执行关闭操作
			//Log("执行关闭。");
			System.Environment.Exit(System.Environment.ExitCode);
			this.Dispose();
			this.Close();	
#endif
		}

		private void Log(string info)
		{
			txtInfo.AppendText(DateTime.Now.ToLongTimeString() + " " + info);
			txtInfo.AppendText(Environment.NewLine);
			txtInfo.ScrollToCaret();
		}

		private void ExcelFromOpener_Load(object sender, EventArgs e)
		{

		}

		private void btnRetry_Click(object sender, EventArgs e)
		{
			if (!IsExistExcelProcess())
			{
				this.btnRetry.Enabled = false;
				if (OpenExcelForm())
					this.Close();
			}
		}
	}
}
