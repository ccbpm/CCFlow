using System;
using System.IO;
using BP.Pub;
using BP.Sys;

namespace BP.DA
{
    /// <summary>
    /// 信息类型
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// Bug.
        /// </summary>
        Debug = 0,
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
    /// <summary>
    /// 日志
    /// </summary>
    public class Log
    {
        private static NLog.Logger _logger = null;
        private static NLog.Logger logger
        {
            get
            {
                try
                {
                    if (_logger != null)
                        return _logger;
                    _logger = NLog.LogManager.GetCurrentClassLogger();
                    return _logger;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }
        #region 在测试状态下
        public static void DebugWriteInfo(string msg)
        {
            if (logger != null)
                logger.Info(msg);

        }
        public static void DebugWriteWarning(string msg)
        {
            if (logger != null)
                logger.Warn(msg);
        }
        public static void DebugWriteError(string msg)
        {
            if (logger != null)
                logger.Error(msg);
        }
        public static void DebugWriteError(Exception ex)
        {
            if (logger != null)
                logger.Error(ex.Message);
        }
        #endregion
    }
}
