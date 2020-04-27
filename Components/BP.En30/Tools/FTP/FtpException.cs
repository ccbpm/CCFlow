namespace BP
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    public class FtpException : ApplicationException
    {
        private int errorCode;

        internal FtpException(string message, int lastError) : base(message)
        {
            this.errorCode = lastError;
        }

        public static FtpException GetFtpException()
        {
            int num = Marshal.GetLastWin32Error();
            string message = null;
            if (num == 0x2ee3)
            {
                NativeMethods.InternetGetLastResponseInfo(out message);
            }
            else if ((num >= 0x2ee0) && (num <= 0x2f8e))
            {
                message = $"TODO: INTERNET_ERROR_* need message mappings {num}";
            }
            else
            {
                message = new Win32Exception(num).Message;
            }
            return new FtpException(message, num);
        }

        internal static void THROWONFALSE(bool rc)
        {
            if (!rc)
            {
                throw GetFtpException();
            }
        }

        internal static void THROWONNULL(IntPtr handle)
        {
            if (handle == IntPtr.Zero)
            {
                throw GetFtpException();
            }
        }

        public int ErrorCode =>
            this.errorCode;
    }
}

