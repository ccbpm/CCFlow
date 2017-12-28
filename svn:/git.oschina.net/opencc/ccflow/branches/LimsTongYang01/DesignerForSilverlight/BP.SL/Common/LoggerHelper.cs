using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace BP.SL
{
    /// <summary>
    /// Silverlight日志，写入文件log.txt
    /// </summary>
    public class LoggerHelper
    {
        public static string logFile = "log.txt";

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="message"></param>
        public static void Write(Exception message)
        {
            try
            {
                IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

                if (!isf.FileExists(logFile))
                {
                    IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(logFile, FileMode.Create, isf);
                    isfs.Close();
                }

                using (StreamWriter sw = new StreamWriter(isf.OpenFile(logFile, FileMode.Append, FileAccess.Write)))
                {
                    sw.WriteLine(string.Format("[{0}] {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), message));
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception)
            {

            }
        }
      
        /// <summary>
        /// 读日志
        /// </summary>
        /// <returns></returns>
        public static string Read()
        {
            string content = string.Empty;
            try
            {
                IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
                if (!isf.FileExists(logFile))
                    return string.Empty;

                IsolatedStorageFileStream isfs = new IsolatedStorageFileStream(logFile, FileMode.Open, isf);
                using (StreamReader sr = new StreamReader(isfs))
                {
                    content = sr.ReadToEnd();
                    sr.Close();
                }

                isfs.Close();
            }catch(Exception e){
                content = e.Message;
            }
            return content;
        }

        /// <summary>
        /// 清空日志
        /// </summary>
        public static void Clear()
        {
            IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
            if (isf.FileExists(logFile))
            {
                isf.DeleteFile(logFile);
            }
        }

    }
}
