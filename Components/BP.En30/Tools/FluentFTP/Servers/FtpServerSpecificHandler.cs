namespace FluentFTP.Servers
{
    using FluentFTP;
    using FluentFTP.Servers.Handlers;
    using System;
    using System.Collections.Generic;
    using System.Security.Authentication;
    using System.Text.RegularExpressions;

    internal static class FtpServerSpecificHandler
    {
        internal static List<FtpBaseServer> AllServers;

        static FtpServerSpecificHandler()
        {
            List<FtpBaseServer> list1 = new List<FtpBaseServer> {
                new BFtpdServer(),
                new CerberusServer(),
                new CrushFtpServer(),
                new FileZillaServer(),
                new Ftp2S3GatewayServer(),
                new GlFtpdServer(),
                new GlobalScapeEftServer(),
                new HomegateFtpServer(),
                new IbmZosFtpServer(),
                new NonStopTandemServer(),
                new OpenVmsServer(),
                new ProFtpdServer(),
                new PureFtpdServer(),
                new ServUServer(),
                new SolarisFtpServer(),
                new VsFtpdServer(),
                new WindowsCeServer(),
                new WindowsIisServer(),
                new WuFtpdServer(),
                new XLightServer()
            };
            AllServers = list1;
        }

        public static void AssumeCapabilities(FtpClient client, FtpBaseServer handler, List<FtpCapability> m_capabilities, ref FtpHashAlgorithm m_hashAlgorithms)
        {
            if (handler != null)
            {
                string[] features = handler.DefaultCapabilities();
                if (features != null)
                {
                    GetFeatures(client, m_capabilities, ref m_hashAlgorithms, features);
                }
            }
        }

        public static FtpOperatingSystem DetectFtpOSBySyst(FtpClient client)
        {
            FtpOperatingSystem serverOS = client.ServerOS;
            string str = client.SystemType.ToUpper();
            if (str.StartsWith("WINDOWS"))
            {
                return FtpOperatingSystem.Windows;
            }
            if (str.Contains("UNIX") || str.Contains("AIX"))
            {
                return FtpOperatingSystem.Unix;
            }
            if (str.Contains("VMS"))
            {
                return FtpOperatingSystem.VMS;
            }
            if (str.Contains("OS/400"))
            {
                return FtpOperatingSystem.IBMOS400;
            }
            if (str.Contains("Z/OS"))
            {
                return FtpOperatingSystem.IBMzOS;
            }
            if (str.Contains("SUNOS"))
            {
                return FtpOperatingSystem.SunOS;
            }
            return FtpOperatingSystem.Unknown;
        }

        public static FtpServer DetectFtpServer(FtpClient client, FtpReply HandshakeReply)
        {
            FtpServer serverType = client.ServerType;
            if (HandshakeReply.Success && ((HandshakeReply.Message != null) || (HandshakeReply.InfoMessages != null)))
            {
                string message = HandshakeReply.Message + HandshakeReply.InfoMessages;
                foreach (FtpBaseServer server2 in AllServers)
                {
                    if (server2.DetectByWelcome(message))
                    {
                        serverType = server2.ToEnum();
                        break;
                    }
                }
                if (serverType != FtpServer.Unknown)
                {
                    client.LogLine(FtpTraceLevel.Info, "Status:   Detected FTP server: " + serverType.ToString());
                }
            }
            return serverType;
        }

        public static FtpServer DetectFtpServerBySyst(FtpClient client)
        {
            FtpServer serverType = client.ServerType;
            if (serverType == FtpServer.Unknown)
            {
                foreach (FtpBaseServer server2 in AllServers)
                {
                    if (server2.DetectBySyst(client.SystemType))
                    {
                        serverType = server2.ToEnum();
                        break;
                    }
                }
                if (serverType != FtpServer.Unknown)
                {
                    client.LogStatus(FtpTraceLevel.Info, "Detected FTP server: " + serverType.ToString());
                }
            }
            return serverType;
        }

        public static void GetFeatures(FtpClient client, List<FtpCapability> m_capabilities, ref FtpHashAlgorithm m_hashAlgorithms, string[] features)
        {
            string[] strArray = features;
            for (int i = 0; i < strArray.Length; i++)
            {
                string input = strArray[i].Trim().ToUpper();
                if (input.StartsWith("MLST") || input.StartsWith("MLSD"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.MLSD);
                }
                else if (input.StartsWith("MDTM"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.MDTM);
                }
                else if (input.StartsWith("REST STREAM"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.REST);
                }
                else if (input.StartsWith("SIZE"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.SIZE);
                }
                else if (input.StartsWith("UTF8"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.UTF8);
                }
                else if (input.StartsWith("PRET"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.PRET);
                }
                else if (input.StartsWith("MFMT"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.MFMT);
                }
                else if (input.StartsWith("MFCT"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.MFCT);
                }
                else if (input.StartsWith("MFF"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.MFF);
                }
                else if (input.StartsWith("MMD5"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.MMD5);
                }
                else if (input.StartsWith("XMD5"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.XMD5);
                }
                else if (input.StartsWith("XCRC"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.XCRC);
                }
                else if (input.StartsWith("XSHA1"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.XSHA1);
                }
                else if (input.StartsWith("XSHA256"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.XSHA256);
                }
                else if (input.StartsWith("XSHA512"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.XSHA512);
                }
                else if (input.StartsWith("EPSV"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.EPSV);
                }
                else if (input.StartsWith("CPSV"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.CPSV);
                }
                else if (input.StartsWith("NOOP"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.NOOP);
                }
                else if (input.StartsWith("CLNT"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.CLNT);
                }
                else if (input.StartsWith("SSCN"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.SSCN);
                }
                else if (input.StartsWith("SITE MKDIR"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.SITE_MKDIR);
                }
                else if (input.StartsWith("SITE RMDIR"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.SITE_RMDIR);
                }
                else if (input.StartsWith("SITE UTIME"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.SITE_UTIME);
                }
                else if (input.StartsWith("SITE SYMLINK"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.SITE_SYMLINK);
                }
                else if (input.StartsWith("AVBL"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.AVBL);
                }
                else if (input.StartsWith("THMB"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.THMB);
                }
                else if (input.StartsWith("RMDA"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.RMDA);
                }
                else if (input.StartsWith("DSIZ"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.DSIZ);
                }
                else if (input.StartsWith("HOST"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.HOST);
                }
                else if (input.StartsWith("CCC"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.CCC);
                }
                else if (input.StartsWith("MODE Z"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.MODE_Z);
                }
                else if (input.StartsWith("LANG"))
                {
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.LANG);
                }
                else if (input.StartsWith("HASH"))
                {
                    Match match;
                    m_capabilities.AddOnce<FtpCapability>(FtpCapability.HASH);
                    if ((match = Regex.Match(input, @"^HASH\s+(?<types>.*)$")).Success)
                    {
                        char[] separator = new char[] { ';' };
                        string[] strArray2 = match.Groups["types"].Value.Split(separator);
                        for (int j = 0; j < strArray2.Length; j++)
                        {
                            string s = strArray2[j].ToUpper().Trim();
                            uint num3 = <PrivateImplementationDetails>.ComputeStringHash(s);
                            if (num3 <= 0x7635ffc9)
                            {
                                switch (num3)
                                {
                                    case 0x661f4299:
                                        if (s == "CRC")
                                        {
                                            goto Label_0567;
                                        }
                                        break;

                                    case 0x7360d733:
                                        if (s == "MD5")
                                        {
                                            goto Label_055F;
                                        }
                                        break;

                                    case 0x7635ffc9:
                                        if (s == "CRC*")
                                        {
                                            goto Label_0567;
                                        }
                                        break;

                                    case 0x41c75f35:
                                        goto Label_04D4;

                                    case 0x5cdc1916:
                                        goto Label_04F7;
                                }
                            }
                            else if (num3 <= 0xba729c5b)
                            {
                                switch (num3)
                                {
                                    case 0x87550878:
                                        goto Label_04E7;

                                    case 0xba729c5b:
                                        goto Label_0517;
                                }
                            }
                            else
                            {
                                switch (num3)
                                {
                                    case 0xc64cb93d:
                                    {
                                        if (s == "SHA-256")
                                        {
                                            goto Label_054F;
                                        }
                                        continue;
                                    }
                                    case 0xe6d208cb:
                                    {
                                        if (s == "SHA-1*")
                                        {
                                            goto Label_0547;
                                        }
                                        continue;
                                    }
                                }
                                if ((num3 == 0xee353ec3) && (s == "SHA-1"))
                                {
                                    goto Label_0547;
                                }
                            }
                            continue;
                        Label_04D4:
                            if (s == "SHA-256*")
                            {
                                goto Label_054F;
                            }
                            continue;
                        Label_04E7:
                            if (s == "SHA-512")
                            {
                                goto Label_0557;
                            }
                            continue;
                        Label_04F7:
                            if (s == "SHA-512*")
                            {
                                goto Label_0557;
                            }
                            continue;
                        Label_0517:
                            if (s == "MD5*")
                            {
                                goto Label_055F;
                            }
                            continue;
                        Label_0547:
                            m_hashAlgorithms |= FtpHashAlgorithm.SHA1;
                            continue;
                        Label_054F:
                            m_hashAlgorithms |= FtpHashAlgorithm.SHA256;
                            continue;
                        Label_0557:
                            m_hashAlgorithms |= FtpHashAlgorithm.SHA512;
                            continue;
                        Label_055F:
                            m_hashAlgorithms |= FtpHashAlgorithm.MD5;
                            continue;
                        Label_0567:
                            m_hashAlgorithms |= FtpHashAlgorithm.CRC;
                        }
                    }
                }
            }
        }

        public static FtpBaseServer GetServerHandler(FtpServer value)
        {
            if (value != FtpServer.Unknown)
            {
                foreach (FtpBaseServer server in AllServers)
                {
                    if (server.ToEnum() == value)
                    {
                        return server;
                    }
                }
            }
            return null;
        }

        public static FtpProfile GetWorkingProfileFromHost(string Host, int Port)
        {
            if (Host.IndexOf("ftp.azurewebsites.windows.net", StringComparison.OrdinalIgnoreCase) > -1)
            {
                return new FtpProfile { Protocols = SslProtocols.Tls, DataConnection = FtpDataConnectionType.PASV, RetryAttempts = 5, SocketPollInterval = 0x3e8, Timeout = 0x7d0 };
            }
            return null;
        }
    }
}

