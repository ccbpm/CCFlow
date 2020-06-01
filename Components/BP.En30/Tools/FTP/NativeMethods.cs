namespace BP
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class NativeMethods
    {
        public const int ERROR_FTP_DROPPED = 0x2f4f;
        public const int ERROR_FTP_NO_PASSIVE_MODE = 0x2f50;
        public const int ERROR_FTP_TRANSFER_IN_PROGRESS = 0x2f4e;
        public const int ERROR_GOPHER_ATTRIBUTE_NOT_FOUND = 0x2f69;
        public const int ERROR_GOPHER_DATA_ERROR = 0x2f64;
        public const int ERROR_GOPHER_END_OF_DATA = 0x2f65;
        public const int ERROR_GOPHER_INCORRECT_LOCATOR_TYPE = 0x2f67;
        public const int ERROR_GOPHER_INVALID_LOCATOR = 0x2f66;
        public const int ERROR_GOPHER_NOT_FILE = 0x2f63;
        public const int ERROR_GOPHER_NOT_GOPHER_PLUS = 0x2f68;
        public const int ERROR_GOPHER_PROTOCOL_ERROR = 0x2f62;
        public const int ERROR_GOPHER_UNKNOWN_LOCATOR = 0x2f6a;
        public const int ERROR_HTTP_COOKIE_DECLINED = 0x2f82;
        public const int ERROR_HTTP_COOKIE_NEEDS_CONFIRMATION = 0x2f81;
        public const int ERROR_HTTP_DOWNLEVEL_SERVER = 0x2f77;
        public const int ERROR_HTTP_HEADER_ALREADY_EXISTS = 0x2f7b;
        public const int ERROR_HTTP_HEADER_NOT_FOUND = 0x2f76;
        public const int ERROR_HTTP_INVALID_HEADER = 0x2f79;
        public const int ERROR_HTTP_INVALID_QUERY_REQUEST = 0x2f7a;
        public const int ERROR_HTTP_INVALID_SERVER_RESPONSE = 0x2f78;
        public const int ERROR_HTTP_NOT_REDIRECTED = 0x2f80;
        public const int ERROR_HTTP_REDIRECT_FAILED = 0x2f7c;
        public const int ERROR_HTTP_REDIRECT_NEEDS_CONFIRMATION = 0x2f88;
        public const int ERROR_INTERNET_ASYNC_THREAD_FAILED = 0x2f0f;
        public const int ERROR_INTERNET_BAD_AUTO_PROXY_SCRIPT = 0x2f86;
        public const int ERROR_INTERNET_BAD_OPTION_LENGTH = 0x2eea;
        public const int ERROR_INTERNET_BAD_REGISTRY_PARAMETER = 0x2ef6;
        public const int ERROR_INTERNET_CANNOT_CONNECT = 0x2efd;
        public const int ERROR_INTERNET_CHG_POST_IS_NON_SECURE = 0x2f0a;
        public const int ERROR_INTERNET_CLIENT_AUTH_CERT_NEEDED = 0x2f0c;
        public const int ERROR_INTERNET_CLIENT_AUTH_NOT_SETUP = 0x2f0e;
        public const int ERROR_INTERNET_CONNECTION_ABORTED = 0x2efe;
        public const int ERROR_INTERNET_CONNECTION_RESET = 0x2eff;
        public const int ERROR_INTERNET_DIALOG_PENDING = 0x2f11;
        public const int ERROR_INTERNET_DISCONNECTED = 0x2f83;
        public const int ERROR_INTERNET_EXTENDED_ERROR = 0x2ee3;
        public const int ERROR_INTERNET_FAILED_DUETOSECURITYCHECK = 0x2f8b;
        public const int ERROR_INTERNET_FORCE_RETRY = 0x2f00;
        public const int ERROR_INTERNET_FORTEZZA_LOGIN_NEEDED = 0x2f16;
        public const int ERROR_INTERNET_HANDLE_EXISTS = 0x2f04;
        public const int ERROR_INTERNET_HTTP_TO_HTTPS_ON_REDIR = 0x2f07;
        public const int ERROR_INTERNET_HTTPS_HTTP_SUBMIT_REDIR = 0x2f14;
        public const int ERROR_INTERNET_HTTPS_TO_HTTP_ON_REDIR = 0x2f08;
        public const int ERROR_INTERNET_INCORRECT_FORMAT = 0x2efb;
        public const int ERROR_INTERNET_INCORRECT_HANDLE_STATE = 0x2ef3;
        public const int ERROR_INTERNET_INCORRECT_HANDLE_TYPE = 0x2ef2;
        public const int ERROR_INTERNET_INCORRECT_PASSWORD = 0x2eee;
        public const int ERROR_INTERNET_INCORRECT_USER_NAME = 0x2eed;
        public const int ERROR_INTERNET_INSERT_CDROM = 0x2f15;
        public const int ERROR_INTERNET_INTERNAL_ERROR = 0x2ee4;
        public const int ERROR_INTERNET_INVALID_CA = 0x2f0d;
        public const int ERROR_INTERNET_INVALID_OPERATION = 0x2ef0;
        public const int ERROR_INTERNET_INVALID_OPTION = 0x2ee9;
        public const int ERROR_INTERNET_INVALID_PROXY_REQUEST = 0x2f01;
        public const int ERROR_INTERNET_INVALID_URL = 0x2ee5;
        public const int ERROR_INTERNET_ITEM_NOT_FOUND = 0x2efc;
        public const int ERROR_INTERNET_LOGIN_FAILURE = 0x2eef;
        public const int ERROR_INTERNET_LOGIN_FAILURE_DISPLAY_ENTITY_BODY = 0x2f8e;
        public const int ERROR_INTERNET_MIXED_SECURITY = 0x2f09;
        public const int ERROR_INTERNET_NAME_NOT_RESOLVED = 0x2ee7;
        public const int ERROR_INTERNET_NEED_MSN_SSPI_PKG = 0x2f8d;
        public const int ERROR_INTERNET_NEED_UI = 0x2f02;
        public const int ERROR_INTERNET_NO_CALLBACK = 0x2ef9;
        public const int ERROR_INTERNET_NO_CONTEXT = 0x2ef8;
        public const int ERROR_INTERNET_NO_DIRECT_ACCESS = 0x2ef7;
        public const int ERROR_INTERNET_NOT_INITIALIZED = 0x2f8c;
        public const int ERROR_INTERNET_NOT_PROXY_REQUEST = 0x2ef4;
        public const int ERROR_INTERNET_OPERATION_CANCELLED = 0x2ef1;
        public const int ERROR_INTERNET_OPTION_NOT_SETTABLE = 0x2eeb;
        public const int ERROR_INTERNET_OUT_OF_HANDLES = 0x2ee1;
        public const int ERROR_INTERNET_POST_IS_NON_SECURE = 0x2f0b;
        public const int ERROR_INTERNET_PROTOCOL_NOT_FOUND = 0x2ee8;
        public const int ERROR_INTERNET_PROXY_SERVER_UNREACHABLE = 0x2f85;
        public const int ERROR_INTERNET_REDIRECT_SCHEME_CHANGE = 0x2f10;
        public const int ERROR_INTERNET_REGISTRY_VALUE_NOT_FOUND = 0x2ef5;
        public const int ERROR_INTERNET_REQUEST_PENDING = 0x2efa;
        public const int ERROR_INTERNET_RETRY_DIALOG = 0x2f12;
        public const int ERROR_INTERNET_SEC_CERT_CN_INVALID = 0x2f06;
        public const int ERROR_INTERNET_SEC_CERT_DATE_INVALID = 0x2f05;
        public const int ERROR_INTERNET_SEC_CERT_ERRORS = 0x2f17;
        public const int ERROR_INTERNET_SEC_CERT_NO_REV = 0x2f18;
        public const int ERROR_INTERNET_SEC_CERT_REV_FAILED = 0x2f19;
        public const int ERROR_INTERNET_SEC_CERT_REVOKED = 0x2f8a;
        public const int ERROR_INTERNET_SEC_INVALID_CERT = 0x2f89;
        public const int ERROR_INTERNET_SECURITY_CHANNEL_ERROR = 0x2f7d;
        public const int ERROR_INTERNET_SERVER_UNREACHABLE = 0x2f84;
        public const int ERROR_INTERNET_SHUTDOWN = 0x2eec;
        public const int ERROR_INTERNET_TCPIP_NOT_INSTALLED = 0x2f7f;
        public const int ERROR_INTERNET_TIMEOUT = 0x2ee2;
        public const int ERROR_INTERNET_UNABLE_TO_CACHE_FILE = 0x2f7e;
        public const int ERROR_INTERNET_UNABLE_TO_DOWNLOAD_SCRIPT = 0x2f87;
        public const int ERROR_INTERNET_UNRECOGNIZED_SCHEME = 0x2ee6;
        internal const int INTERNET_ERROR_BASE = 0x2ee0;
        internal const int INTERNET_ERROR_FIRST = 0x2ee0;
        internal const int INTERNET_ERROR_LAST = 0x2f8e;
        internal const int MAX_PATH = 260;

        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool FtpCreateDirectory(IntPtr hConnect, string directory);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool FtpDeleteFile(IntPtr hConnect, string lpszFileName);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr FtpFindFirstFile(IntPtr hConnect, string lpszSearchFile, out Win32FindData lpFindFileData, int dwFlags, IntPtr dwContext);
        public static bool FtpGetCurrentDirectory(IntPtr hConnect, out string directory)
        {
            directory = null;
            StringBuilder currentDirectory = new StringBuilder(260);
            int capacity = currentDirectory.Capacity;
            bool flag = FtpGetCurrentDirectory(hConnect, currentDirectory, ref capacity);
            if (flag)
            {
                directory = currentDirectory.ToString();
            }
            return flag;
        }

        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool FtpGetCurrentDirectory(IntPtr hConnect, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder currentDirectory, ref int dwCurrentDirectory);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool FtpGetFile(IntPtr hConnect, string lpszRemoteFile, string lpszNewFile, bool fFailIfExists, FileAttributes flagsAndAttributes, FtpTransferType flags, IntPtr dwContext);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr FtpOpenFile(IntPtr hConnect, string lpszFileName, GenericRights dwAccess, FtpTransferType dwFlags, IntPtr dwContext);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool FtpPutFile(IntPtr hConnect, string localFile, string newRemoteFile, FtpTransferType flags, IntPtr dwContext);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool FtpRemoveDirectory(IntPtr hConnect, string lpszDirectory);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool FtpRenameFile(IntPtr hConnect, string existing, string newName);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool FtpSetCurrentDirectory(IntPtr hConnect, string directory);
        [DllImport("WinInet.dll", SetLastError=true)]
        public static extern bool InternetCloseHandle(IntPtr hInternet);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr InternetConnect(IntPtr hInternet, [MarshalAs(UnmanagedType.LPTStr)] string serverName, int serverPort, [MarshalAs(UnmanagedType.LPTStr)] string username, [MarshalAs(UnmanagedType.LPTStr)] string password, InternetService service, int flags, IntPtr context);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool InternetFindNextFile(IntPtr hFind, out Win32FindData lpvFindData);
        public static int InternetGetLastResponseInfo(out string message)
        {
            message = "";
            StringBuilder buffer = new StringBuilder(260);
            int capacity = buffer.Capacity;
            int num = 1;
            if (!InternetGetLastResponseInfo(out  num, buffer, ref capacity))
            {
                int lastError = Marshal.GetLastWin32Error();
                if (lastError == 0x7a)
                {
                    buffer.Capacity = ++capacity;
                    if (InternetGetLastResponseInfo(out num, buffer, ref capacity))
                    {
                        message = buffer.ToString();
                        return num;
                    }
                }
                throw new FtpException(new Win32Exception().Message, lastError);
            }
            message = buffer.ToString();
            return num;
        }

        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool InternetGetLastResponseInfo(out int error, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer, ref int bufferLength);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr InternetOpen([MarshalAs(UnmanagedType.LPTStr)] string agent, InternetOpenType accessType, [MarshalAs(UnmanagedType.LPTStr)] string lpszProxyName, [MarshalAs(UnmanagedType.LPTStr)] string lpszProxyBypass, int flags);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool InternetReadFile(IntPtr hFile, IntPtr buffer, int numberOfBytesToRead, out int numberOfBytesRead);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool InternetReadFile(IntPtr hFile, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] byte[] buffer, int numberOfBytesToRead, out int numberOfBytesRead);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool InternetWriteFile(IntPtr hFile, IntPtr buffer, int numberOfBytesToWrite, out int numberOfBytesWritten);
        [DllImport("WinInet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool InternetWriteFile(IntPtr hFile, [In, MarshalAs(UnmanagedType.LPArray)] byte[] buffer, int numberOfBytesToWrite, out int numberOfBytesWritten);
        [STAThread]
        private static void Main(string[] args)
        {
            FtpConnection connection = new FtpConnection("test.com", 0x15, "test", "test");
            connection.GetFile("test.cs", @"C:\newdave.txt", true, FileAttributes.Normal);
            connection.Close();
        }
    }
}

