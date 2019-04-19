using System;
using System.IO;
using BP.Pub;
using BP.Sys;

namespace BP.DA
{
	#region enum LogType
	/// <summary>
	/// 信息类型
	/// </summary>
    public enum LogType
    {
        /// <summary>
        /// 提示
        /// </summary>
        Info = 1,
        /// <summary>
        /// 警告
        /// </summary>
        Warning = 2,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 3
    }
	#endregion

	#region class Log	
	/// <summary>
	/// 日志
	/// </summary>
	public class Log
	{
		#region 在测试状态下
		public static void DebugWriteInfo(string msg)
		{
		//	if (SystemConfig.IsDebug)
				DefaultLogWriteLine(LogType.Info,msg);
		}
		public static void DebugWriteWarning(string msg)
		{
			//if (SystemConfig.IsDebug)
				DefaultLogWriteLine(LogType.Warning,msg);
		}
        public static void DebugWriteError(string msg)
        {
            //  if (SystemConfig.IsDebug)
            DefaultLogWriteLine(LogType.Error, msg);
        }
		#endregion

        public static void DefaultLogWriteLineError(Exception ex)
        {

            DefaultLogWriteLine(LogType.Error, ex.Message);

            DefaultLogWriteLine(LogType.Error, ex.StackTrace);
        }

		public static void DefaultLogWriteLineError(string msg)
		{
			DefaultLogWriteLine(LogType.Error,msg);
		}

        public static void DefaultLogWriteLineError(string msg, bool isOutDos)
        {
            DefaultLogWriteLine(LogType.Error, msg);
            if (isOutDos)
                System.Console.WriteLine(msg);
        }

		public static void DefaultLogWriteLineInfo(string msg)
		{
			DefaultLogWriteLine(LogType.Info,msg);
		}

        public static void DefaultLogWriteLineInfo(string msg, bool isoutDos)
        {
            DefaultLogWriteLine(LogType.Info, msg);
            if (isoutDos)
                System.Console.WriteLine(msg);
        }

		public static void DefaultLogWriteLineWarning(string msg)
		{		
			DefaultLogWriteLine(LogType.Warning,msg);
		}
        public static void DefaultLogWriteLineWarning(string msg, bool isOutdoc)
        {
            DefaultLogWriteLine(LogType.Warning, msg);
            if (isOutdoc)
                System.Console.WriteLine(msg);
        }

		#region 经常使用的静态方法
		private static Log _log = new Log( Log.GetLogFileName() );
		public static void DefaultLogWriteLine(LogType type, string info)
		{
			_log.WriteLine(type, info);
		}
        public static void DefaultLogWriteLineWithOutUseInfo(string info)
        {
            _log.openFile();
            _log.writelog(info);
            _log.closeFile();
        }
		#endregion

		private bool isReady = false;
		private StreamWriter swLog;
		private string strLogFile;
		private string userName="System";
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="LogFileName"></param>
        public Log(string LogFileName)
        {
            this.strLogFile = LogFileName;
            try
            {
                openFile();
                writelog("-----------------------------------------------------------------------------------------------------");
            }
            finally
            {
                closeFile();
            }
        }
		/// <summary>
		/// 用户
		/// </summary>
		public string UserName
		{
			set{this.userName = value;}
		}
		private void writelog(string msg) 
		{
			if(isReady) 
			{
				swLog.WriteLine(msg);
			} 
			else 
			{
				Console.WriteLine("Error Cannot write to log file.");
			}
		}
        public static void ClearLog()
        {
            try
            {
                File.Delete(BP.Sys.SystemConfig.PathOfLog + DateTime.Now.ToString("\\yyyy_MM_dd.log"));
            }
            catch
            {
            }
        }
        private void openFile()
        {
            try
            {
                swLog = File.AppendText(strLogFile);
                isReady = true;
            }
            catch
            {
                isReady = false;
            }
        }
		private void closeFile() 
		{			
			if(isReady)
			{
				try 
				{
					swLog.Flush();
					swLog.Close();
				} 
				catch 
				{
				}
			}
		}
		/// <summary>
		/// 取得Log的文件路径和文件名
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetLogFileName()
		{
			string filepath=SystemConfig.PathOfLog ;
			
			//如果目录没有建立，则建立.
			if (Directory.Exists(filepath) == false) 
				Directory.CreateDirectory(filepath);

			return filepath + "\\"+ DateTime.Now.ToString("yyyy_MM_dd") + ".log";

			//return filepath + "\\"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ".log";
		}
		/// <summary>
		/// 写一行日志
		/// </summary>
		/// <param name="message"></param>
		public void WriteLine(string message)
		{
			WriteLine(LogType.Info,message);
		}
		/// <summary>
		/// 写一行日志
		/// </summary>
		/// <param name="logtype"></param>
		/// <param name="message"></param>
		public void WriteLine(LogType logtype, string message) 
		{
//			string stub = DateTime.Now.ToString("@HH:MM:ss") ;
			string stub = DateTime.Now.ToString("dd日HH时mm分ss秒") ;
			switch(logtype) 
			{
				case LogType.Info:
					stub += "Info:";
					break;
				case LogType.Warning:
					stub += "Warning:";
					break;
				case LogType.Error:
					stub += "Fatal:";
					break;
			}
			stub = stub + userName + "');\"" + message;
			openFile();
			writelog(stub);
			closeFile();
			//Console.WriteLine(stub);
		}
        /// <summary>
        /// 打开日志目录
        /// </summary>
        public static void OpenLogDir()
        {
            string file = BP.Sys.SystemConfig.PathOfLog;
            try
            {
                System.Diagnostics.Process.Start(file);
            }
            catch (Exception ex)
            {
                throw new Exception("@打开日志目录出现错误。" + ex.Message);
            }
        }
        /// <summary>
        /// 打开今天的日志
        /// </summary>
        public static void OpeLogToday()
        {
            string file = BP.Sys.SystemConfig.PathOfLog + DateTime.Now.ToString("yyyy_MM_dd") + ".log";
            try
            {
                System.Diagnostics.Process.Start(file);
            }
            catch(Exception ex)
            {
                throw new Exception("@打开日志文件出现错误。"+ex.Message );
            }
        }

	}
	#endregion
}
