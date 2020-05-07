namespace FluentFTP
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class FtpTrace
    {
        private static bool m_flushOnWrite;
        private static bool m_functions;
        private static bool m_IP;
        private static bool m_password;
        private static bool m_prefix;
        private static volatile TraceSource m_traceSource;
        private static bool m_tracing;
        private static bool m_username;
        private static object traceSync;

        static FtpTrace()
        {
            TraceSource source1 = new TraceSource("FluentFTP");
            SourceSwitch switch1 = new SourceSwitch("sourceSwitch", "Verbose") {
                Level = ~SourceLevels.Off
            };
            source1.Switch = switch1;
            m_traceSource = source1;
            m_flushOnWrite = true;
            m_prefix = false;
            m_functions = true;
            m_IP = true;
            m_username = true;
            m_password = false;
            m_tracing = true;
            traceSync = new object();
        }

        public static void AddListener(TraceListener listener)
        {
            TraceSource traceSource = m_traceSource;
            lock (traceSource)
            {
                m_traceSource.Listeners.Add(listener);
            }
        }

        private static void EmitEvent(TraceSource traceSource, TraceEventType eventType, string message)
        {
            try
            {
                object traceSync = FtpTrace.traceSync;
                lock (traceSync)
                {
                    if (traceSource.Switch.ShouldTrace(eventType))
                    {
                        foreach (TraceListener listener in traceSource.Listeners)
                        {
                            try
                            {
                                listener.WriteLine(message);
                                listener.Flush();
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public static void RemoveListener(TraceListener listener)
        {
            TraceSource traceSource = m_traceSource;
            lock (traceSource)
            {
                m_traceSource.Listeners.Remove(listener);
            }
        }

        private static TraceEventType TraceLevelTranslation(FtpTraceLevel level)
        {
            switch (level)
            {
                case FtpTraceLevel.Verbose:
                    return TraceEventType.Verbose;

                case FtpTraceLevel.Info:
                    return TraceEventType.Information;

                case FtpTraceLevel.Warn:
                    return TraceEventType.Warning;

                case FtpTraceLevel.Error:
                    return TraceEventType.Error;
            }
            return TraceEventType.Verbose;
        }

        public static void Write(string message)
        {
            Write(FtpTraceLevel.Verbose, message);
        }

        public static void Write(FtpTraceLevel eventType, string message)
        {
            if (EnableTracing)
            {
                if (m_prefix)
                {
                    m_traceSource.TraceEvent(TraceLevelTranslation(eventType), 0, message);
                }
                else
                {
                    EmitEvent(m_traceSource, TraceLevelTranslation(eventType), message);
                }
                if (m_flushOnWrite)
                {
                    m_traceSource.Flush();
                }
            }
        }

        public static void WriteFunc(string function, object[] args = null)
        {
            if (m_functions)
            {
                Write(FtpTraceLevel.Verbose, "");
                Write(FtpTraceLevel.Verbose, "# " + function + "(" + args.ItemsToString().Join(", ") + ")");
            }
        }

        public static void WriteLine(object message)
        {
            Write(FtpTraceLevel.Verbose, message.ToString());
        }

        public static void WriteLine(FtpTraceLevel eventType, object message)
        {
            Write(eventType, message.ToString());
        }

        public static bool EnableTracing
        {
            get
            {
                return m_tracing;
            }
            set
            {
                m_tracing = value;
            }
        }

        public static bool FlushOnWrite
        {
            get
            {
                return m_flushOnWrite;
            }
            set
            {
                m_flushOnWrite = value;
            }
        }

        public static bool LogFunctions
        {
            get
            {
                return m_functions;
            }
            set
            {
                m_functions = value;
            }
        }

        public static bool LogIP
        {
            get
            {
                return m_IP;
            }
            set
            {
                m_IP = value;
            }
        }

        public static bool LogPassword
        {
            get
            {
                return m_password;
            }
            set
            {
                m_password = value;
            }
        }

        public static bool LogPrefix
        {
            get
            {
                return m_prefix;
            }
            set
            {
                m_prefix = value;
            }
        }

        public static bool LogUserName
        {
            get
            {
                return m_username;
            }
            set
            {
                m_username = value;
            }
        }
    }
}

