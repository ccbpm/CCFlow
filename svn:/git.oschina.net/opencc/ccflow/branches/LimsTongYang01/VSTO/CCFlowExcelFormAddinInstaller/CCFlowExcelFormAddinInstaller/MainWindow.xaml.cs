using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;

namespace CCFlowExcelFormAddinInstaller
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void btnInstall2010_Click(object sender, RoutedEventArgs e)
		{
			Install(ExcelType.Excel2010);
		}

		private void btnInstall2013_Click(object sender, RoutedEventArgs e)
		{
			Install(ExcelType.Excel2013);
		}

		void Install(ExcelType type)
		{
			#region 复制文件

			Log("开始安装...");
			//安装源-启动器 路径
			var strOpenerSourcePath = System.Environment.CurrentDirectory + "\\appfiles\\ExcelFormOpener.exe";
			//安装目标-启动器 目录
			var strOpenerAimPath = System.Environment.GetEnvironmentVariable("ProgramFiles") + "\\CCFlow";
			Log("安装路径： " + strOpenerAimPath);
			if (!System.IO.Directory.Exists(strOpenerAimPath))
			{
				System.IO.Directory.CreateDirectory(strOpenerAimPath);
			}
			//安装目标-启动器 路径
			var strOpenerPath = strOpenerAimPath + "\\ExcelFormOpener.exe";
			//if (System.IO.File.Exists(aimPath))
			//{
			//    System.IO.File.Delete(aimPath);
			//} //改用Copy()方法的第三个参数:是否覆盖
			//复制-启动器
			Log("正在复制：ExcelFormOpener.exe");
			System.IO.File.Copy(strOpenerSourcePath, strOpenerPath, true);
			if (!System.IO.File.Exists(strOpenerPath))
			{
				Log("安装失败：复制文件失败！");
				return;
			}
			Log("复制 ExcelFormOpener.exe 成功");

			#endregion

			#region 注册 URI Scheme

			//添加注册表项
			Log("正在注册 Excel表单启动器...");
			Microsoft.Win32.RegistryKey k = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey("excelform");
			k.SetValue("", "URL:excelform Protocol");
			k.SetValue("URL Protocol", "");
			Microsoft.Win32.RegistryKey subKey = k.CreateSubKey("DefaultIcon");
			//subKey.SetValue("", excelPath); //使用Excel的图标
			subKey = k.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("Command");
			subKey.SetValue("", strOpenerPath + " \"%1\"");
			Log("注册 Excel表单启动器 成功！");

			#endregion

			#region 安装插件

			var strInstallerSourcePath = System.Environment.CurrentDirectory + "\\appfiles\\ExcelAddin" + (type == ExcelType.Excel2010 ? "2010" : "2013");

			//解除下载文档的锁定
			var strSteamsPath = System.Environment.CurrentDirectory + "\\appfiles\\streams.exe";
			System.Diagnostics.Process.Start(strSteamsPath, "-s -d \"" + strInstallerSourcePath + "\"");

			//安装源-插件安装程序 目录
			var strInstallerPath = strInstallerSourcePath + "\\setup.exe";
			//执行(打开).vsto文件
			Log("请在弹出的窗口中选择安装，若无异常提示，则表示安装成功。");
			Log("若遇到您无法解决的异常，请寻求系统管理员或相关技术人员协助。");
			if (System.IO.File.Exists(strInstallerPath))
				System.Diagnostics.Process.Start(strInstallerPath);

			#endregion

		}

		private void Log(string msg)
		{
			this.txtLog.Text += "\r\n" + msg;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Log("正在尝试解决“证书或其位置不受信任”问题...");
			if (System.Environment.Is64BitOperatingSystem)
			{
				var key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\Wow6432Node\\MICROSOFT\\.NETFramework\\Security\\TrustManager\\PromptingLevel");
				key.SetValue("MyComputer", "Enabled");
				key.SetValue("LocalIntranet", "Enabled");
				key.SetValue("Internet", "AuthenticodeRequired");
				key.SetValue("TrustedSites", "Enabled");
				key.SetValue("UntrustedSites", "Disabled");
				key.Close();
			}
			else
			{
				var key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey("SOFTWARE\\MICROSOFT\\.NETFramework\\Security\\TrustManager\\PromptingLevel");
				key.SetValue("MyComputer", "Enabled");
				key.SetValue("LocalIntranet", "Enabled");
				key.SetValue("Internet", "AuthenticodeRequired");
				key.SetValue("TrustedSites", "Enabled");
				key.SetValue("UntrustedSites", "Disabled");
				key.Close();
			}
			Log("“证书或其位置不受信任”问题已解决，请尝试重新安装！");
		}
	}

	public enum ExcelType
	{
		Excel2010,
		Excel2013
	}
}
