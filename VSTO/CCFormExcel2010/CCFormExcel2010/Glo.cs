using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Management;

namespace BP.Excel
{
	/// <summary>
	/// 全局
	/// </summary>
	public class Glo
	{
		/// <summary>
		/// （枚举/外键）根据value获取key
		/// </summary>
		/// <param name="ds">OriginData(Set)</param>
		/// <param name="tableName">TableName(UIBindKey)</param>
		/// <param name="name">value</param>
		/// <returns>key</returns>
		public static string GetNoByName(DataSet ds, string tableName, string value)
		{
			if (ds.Tables.Contains(tableName))
			{
				DataTable dt = ds.Tables[tableName];
				foreach (DataRow dr in dt.Rows)
				{
					if (dr["Name"].ToString() == value)
						return dr["No"].ToString();
				}
			}
			return ""; //TODO: 返回 string.Empty or 原值?
		}

		/// <summary>
		/// （枚举/外键）根据key获取value
		/// </summary>
		/// <param name="ds">OriginData(Set)</param>
		/// <param name="tableName">TableName(UIBindKey)</param>
		/// <param name="name">key</param>
		/// <returns>value</returns>
		public static string GetNameByNo(DataSet ds, string tableName, string key)
		{
			if (ds.Tables.Contains(tableName))
			{
				DataTable dt = ds.Tables[tableName];
				foreach (DataRow dr in dt.Rows)
				{
					if (dr["No"].ToString() == key)
						return dr["Name"].ToString();
				}
			}
			return key; //TODO: 返回 string.Empty or 原值?
		}

		#region 参数.
		/// <summary>
		/// 当前登录用户编号
		/// </summary>
		public static string UserNo = "";
		/// <summary>
		/// 当前登录用户SID
		/// </summary>
		public static string SID = "";
		/// <summary>
		/// Excel表单编号
		/// </summary>
		public static string FrmID = "";
		/// <summary>
		/// 发起流程编号
		/// </summary>
		public static string FK_Flow = "";
		/// <summary>
		/// 当前工作ID
		/// </summary>
		public static int WorkID;
        /// <summary>
        /// 父流程ID
        /// </summary>
		public static int PWorkID;
		/// <summary>
		/// 当前Excel表单绑定的节点ID
		/// </summary>
		public static int FK_Node;
		/// <summary>
		/// 以“@key=value”形式接收的参数
		/// </summary>
		public static string AtParas = "";
		/// <summary>
		/// 插件引用的服务地址
		/// </summary>
		public static string WSUrl = "";

		public static string App = "";

		/// <summary>
		/// 启用的Sheet页
		/// 用于同一个表单/类有可能使用不同的Excel模板时，可以将这些模板以Sheet页的形式存到同一个Excel模板中。
		/// 注意：在Sheet页中所有的命名应该加上“Sheet页名.”前缀，在加载时插件会自动去掉这些前缀。
		/// </summary>
		public static string UseSheet = null;
		#endregion 参数.

		/// <summary>
		/// 参数是否加载成功，加载不成功，所有插件功能不启用
		/// </summary>
		public static bool LoadSuccessful = false;
		/// <summary>
		/// 是否只读
		/// </summary>
		public static bool IsReadonly = false;
		/// <summary>
		/// 本地保存的Excel（路径+文件名）
		/// </summary>
		public static string LocalFile;

		#region 方法.
		/// <summary>
		/// 得到 WebService 对象
		/// </summary>
		/// <returns></returns>
		public static CCFormExcel2010.CCForm.CCFormAPISoapClient GetCCFormAPISoapClient()
		{
			TimeSpan ts = new TimeSpan(0, 5, 0);
			var basicBinding = new BasicHttpBinding()
			{
				ReceiveTimeout = ts,
				SendTimeout = ts,
				MaxBufferSize = 2147483647,
				MaxReceivedMessageSize = 2147483647,
				Name = "PortalInterfaceSoap"
			};
			basicBinding.Security.Mode = BasicHttpSecurityMode.None;

			string url = Glo.WSUrl;
			if (url == null)
				url = "http://localhost/WF/CCForm/CCFormAPI.asmx";

			var endPoint = new EndpointAddress(url);
			var ctor =
				typeof(CCFormExcel2010.CCForm.CCFormAPISoapClient).GetConstructor(
				new Type[] {
					typeof(Binding), 
					typeof(EndpointAddress)
				});
			return (CCFormExcel2010.CCForm.CCFormAPISoapClient)ctor.Invoke(
				new object[] { basicBinding, endPoint });
		}

		/// <summary>
		/// 获取EXCEL的启动参数
		/// <para>fromccflow:true表示是从ccflow启动的Excel进程</para>
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, string> GetArguments()
		{
			string argstr = string.Empty;
			string prefix = "-fromccflow,";
			int beginidx = -1;
			Dictionary<string, string> args = new Dictionary<string, string>();

			using (ManagementObjectSearcher mos = new ManagementObjectSearcher(
				"SELECT CommandLine FROM Win32_Process WHERE ProcessId = "
				+ System.Diagnostics.Process.GetCurrentProcess().Id))
			{
				foreach (ManagementObject mo in mos.Get())
				{
					argstr = mo["CommandLine"] as string;
					break;
				}
			}

			beginidx = argstr.IndexOf(prefix);

			if (beginidx == -1 || (beginidx + prefix.Length) == argstr.Length - 1)
			{
				args.Add("fromccflow", "false");
				return args;
			}

			beginidx = beginidx + prefix.Length;
			argstr = argstr.Substring(beginidx);

			if (argstr.IndexOf(' ') != -1)
				argstr = argstr.Substring(0, argstr.IndexOf(' '));

			Glo.AtParas = "@" + argstr.Replace(",", "@");

			string[] argsArr = argstr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
			string[] ars = null;

			args.Add("fromccflow", "true");

			foreach (string arg in argsArr)
			{
				ars = arg.Split('=');

				if (ars.Length == 1)
					continue;

				args.Add(ars[0], ars[1]);
			}

			return args;
		}

		/// <summary>
		/// 写入一个文件
		/// </summary>
		/// <param name="filePathName"></param>
		/// <param name="objData"></param>
		/// <returns></returns>
		public static string WriteFile(string filePathName, byte[] objData)
		{
			string folder = System.IO.Path.GetDirectoryName(filePathName);
			if (System.IO.Directory.Exists(folder) == false)
				System.IO.Directory.CreateDirectory(folder);

			if (System.IO.File.Exists(filePathName) == true)
				System.IO.File.Delete(filePathName);

			System.IO.FileStream fs = new System.IO.FileStream(filePathName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
			System.IO.BinaryWriter w = new System.IO.BinaryWriter(fs);
			try
			{
				w.Write(objData);
				w.Close();
				fs.Close();
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				w.Close();
				fs.Close();
			}
			return filePathName;
		}

		public static byte[] ReadFile(string strFilePath)
		{
			string folder = System.IO.Path.GetDirectoryName(strFilePath);

			if (System.IO.Directory.Exists(folder) == false)
				return null;

			if (System.IO.File.Exists(strFilePath) == false)
				return null;

			/*
			System.IO.FileInfo fi = new System.IO.FileInfo(strFilePath);
			byte[] buffer = new byte[fi.Length];

			System.IO.FileStream fs = fi.OpenRead(); //会提示：文件“ ”正由另一进程使用,因此该进程无法访问此文件
			fs.Read(buffer, 0, Convert.ToInt32(fs.Length));
			fs.Close();*/


			FileStream fs = new FileStream(strFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			var buffer = new byte[fs.Length];
			fs.Position = 0;
			fs.Read(buffer, 0, buffer.Length);
			fs.Close();

			return buffer;
		}
		#endregion 方法.

		/// <summary>
		/// 获取当前插件的版本号
		/// </summary>
		/// <returns></returns>
		public static string GetCurrentVersion()
		{
			System.Reflection.Assembly fileAssembly = System.Reflection.Assembly.GetExecutingAssembly();
			System.Version fileVersion = fileAssembly.GetName().Version;
			return fileVersion.ToString();
		}

	}
}
