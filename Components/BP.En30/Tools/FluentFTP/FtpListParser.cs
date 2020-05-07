namespace FluentFTP
{
    using FluentFTP.Helpers.Parsers;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class FtpListParser
    {
        public FtpClient client;
        public FtpParser CurrentParser;
        public FtpParser DetectedParser;
        public bool HasTimeOffset;
        private static Parser m_customParser;
        public static object m_parserLock;
        public static List<Parser> m_parsers;
        public bool ParserConfirmed;
        private static List<FtpParser> parsers;
        public TimeSpan TimeOffset;
        public static bool VMSNameHasVersion;

        static FtpListParser()
        {
            List<FtpParser> list1 = new List<FtpParser> { 3, 2, 6, 5, 7 };
            parsers = list1;
            VMSNameHasVersion = false;
            m_parserLock = new object();
            m_parsers = null;
        }

        public FtpListParser(FtpClient client)
        {
            this.client = client;
        }

        public static void AddParser(Parser parser)
        {
            object parserLock = m_parserLock;
            lock (parserLock)
            {
                if (m_parsers == null)
                {
                    InitParsers();
                }
                m_parsers.Add(parser);
                m_customParser = parser;
            }
        }

        public static void ClearParsers()
        {
            object parserLock = m_parserLock;
            lock (parserLock)
            {
                if (m_parsers == null)
                {
                    InitParsers();
                }
                m_parsers.Clear();
            }
        }

        public void Init(FtpOperatingSystem system, FtpParser defaultParser = 0)
        {
            this.ParserConfirmed = false;
            if (system == FtpOperatingSystem.Windows)
            {
                this.CurrentParser = FtpParser.Windows;
            }
            else if ((system == FtpOperatingSystem.Unix) || (system == FtpOperatingSystem.SunOS))
            {
                this.CurrentParser = FtpParser.Unix;
            }
            else if (system == FtpOperatingSystem.VMS)
            {
                this.CurrentParser = FtpParser.VMS;
            }
            else if ((system == FtpOperatingSystem.IBMOS400) || (system == FtpOperatingSystem.IBMzOS))
            {
                this.CurrentParser = FtpParser.IBM;
            }
            else
            {
                this.CurrentParser = defaultParser;
                this.client.LogStatus(FtpTraceLevel.Warn, string.Concat(new object[] { "Cannot auto-detect listing parser for system '", system, "', using ", defaultParser, " parser" }));
            }
            this.DetectedParser = this.CurrentParser;
        }

        public static void InitParsers()
        {
            object parserLock = m_parserLock;
            lock (parserLock)
            {
                if (m_parsers == null)
                {
                    m_parsers = new List<Parser>();
                    m_parsers.Add(new Parser(FtpMachineListParser.Parse));
                    m_parsers.Add(new Parser(FtpUnixParser.ParseLegacy));
                    m_parsers.Add(new Parser(FtpWindowsParser.ParseLegacy));
                    m_parsers.Add(new Parser(FtpVMSParser.ParseLegacy));
                }
            }
        }

        private bool IsParserValid(FtpParser p, string[] files)
        {
            switch (p)
            {
                case FtpParser.Windows:
                    return FtpWindowsParser.IsValid(this.client, files);

                case FtpParser.Unix:
                    return FtpUnixParser.IsValid(this.client, files);

                case FtpParser.VMS:
                    return FtpVMSParser.IsValid(this.client, files);

                case FtpParser.IBM:
                    return FtpIBMParser.IsValid(this.client, files);

                case FtpParser.NonStop:
                    return FtpNonStopParser.IsValid(this.client, files);
            }
            return false;
        }

        private bool IsWrongMachineListing()
        {
            return (((this.CurrentParser == FtpParser.Machine) && (this.client != null)) && !this.client.HasFeature(FtpCapability.MLSD));
        }

        private bool IsWrongParser()
        {
            if ((this.CurrentParser != FtpParser.Auto) && this.ParserConfirmed)
            {
                return this.IsWrongMachineListing();
            }
            return true;
        }

        public static FtpListItem ParseLegacy(string path, string buf, List<FtpCapability> capabilities, FtpClient client)
        {
            if (!string.IsNullOrEmpty(buf))
            {
                Parser[] parsers = Parsers;
                for (int i = 0; i < parsers.Length; i++)
                {
                    FtpListItem item = parsers[i](buf, capabilities, client);
                    if (item != null)
                    {
                        item.Input = buf;
                        return item;
                    }
                }
            }
            return null;
        }

        public FtpListItem ParseSingleLine(string path, string file, List<FtpCapability> caps, bool isMachineList)
        {
            FtpListItem item = null;
            if (isMachineList)
            {
                item = FtpMachineListParser.Parse(file, caps, this.client);
            }
            else if (m_customParser != null)
            {
                item = m_customParser(file, caps, this.client);
            }
            else
            {
                if (this.IsWrongParser())
                {
                    string[] files = new string[] { file };
                    this.ValidateParser(files);
                }
                switch (this.CurrentParser)
                {
                    case FtpParser.Legacy:
                        item = ParseLegacy(path, file, caps, this.client);
                        break;

                    case FtpParser.Machine:
                        item = FtpMachineListParser.Parse(file, caps, this.client);
                        break;

                    case FtpParser.Windows:
                        item = FtpWindowsParser.Parse(this.client, file);
                        break;

                    case FtpParser.Unix:
                        item = FtpUnixParser.Parse(this.client, file);
                        break;

                    case FtpParser.UnixAlt:
                        item = FtpUnixParser.ParseUnixAlt(this.client, file);
                        break;

                    case FtpParser.VMS:
                        item = FtpVMSParser.Parse(this.client, file);
                        break;

                    case FtpParser.IBM:
                        item = FtpIBMParser.Parse(this.client, file);
                        break;

                    case FtpParser.NonStop:
                        item = FtpNonStopParser.Parse(this.client, file);
                        break;
                }
            }
            if (item != null)
            {
                if (this.HasTimeOffset)
                {
                    item.Modified -= this.TimeOffset;
                }
                item.CalculateFullFtpPath(this.client, path, false);
            }
            return item;
        }

        public static void RemoveParser(Parser parser)
        {
            object parserLock = m_parserLock;
            lock (parserLock)
            {
                if (m_parsers == null)
                {
                    InitParsers();
                }
                m_parsers.Remove(parser);
            }
        }

        private void ValidateParser(string[] files)
        {
            if (this.IsWrongParser())
            {
                if (this.DetectedParser == FtpParser.Auto)
                {
                    this.DetectedParser = FtpParser.Unix;
                }
                if (this.CurrentParser == FtpParser.Auto)
                {
                    this.CurrentParser = this.DetectedParser;
                }
                if (this.IsWrongMachineListing())
                {
                    this.CurrentParser = this.DetectedParser;
                }
                if (this.IsParserValid(this.CurrentParser, files))
                {
                    this.client.LogStatus(FtpTraceLevel.Verbose, "Confirmed format " + this.CurrentParser.ToString());
                    this.ParserConfirmed = true;
                }
                else
                {
                    foreach (FtpParser parser in parsers)
                    {
                        if (this.IsParserValid(parser, files))
                        {
                            this.CurrentParser = parser;
                            this.client.LogStatus(FtpTraceLevel.Verbose, "Detected format " + this.CurrentParser.ToString());
                            this.ParserConfirmed = true;
                            return;
                        }
                    }
                    this.CurrentParser = FtpParser.Unix;
                    this.client.LogStatus(FtpTraceLevel.Verbose, "Could not detect format. Using default " + this.CurrentParser.ToString());
                }
            }
        }

        public static Parser[] Parsers
        {
            get
            {
                object parserLock = m_parserLock;
                lock (parserLock)
                {
                    if (m_parsers == null)
                    {
                        InitParsers();
                    }
                    return m_parsers.ToArray();
                }
            }
        }

        public delegate FtpListItem Parser(string line, List<FtpCapability> capabilities, FtpClient client);
    }
}

