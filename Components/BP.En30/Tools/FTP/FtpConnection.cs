namespace BP
{
    using System;
    using System.Collections;
    using System.IO;

    public class FtpConnection : IDisposable
    {
        private IntPtr hConnect;
        private IntPtr hInternet;

        public FtpConnection()
        {
            this.hInternet = Open();
        }

        public FtpConnection(string serverOrIp) : this(serverOrIp, 0x15, null, null)
        {
        }

        public FtpConnection(string serverOrIp, int serverPort, string userName, string password)
        {
            IntPtr hInt = Open();
            this.hConnect = InternalConnect(hInt, serverOrIp, serverPort, userName, password);
            this.hInternet = hInt;
        }

        public void Close()
        {
            ((IDisposable) this).Dispose();
        }

        public void Connect(string serverOrIp)
        {
            this.Connect(serverOrIp, 0x15, null, null);
        }

        public void Connect(string serverOrIp, int serverPort, string userName, string password)
        {
            SafeCloseHandle(ref this.hConnect);
            this.hConnect = InternalConnect(this.hInternet, serverOrIp, serverPort, userName, password);
        }

        public void CreateDirectory(string CreateDir)
        {
            bool flag = false;
            try
            {
                FtpException.THROWONFALSE(NativeMethods.FtpCreateDirectory(this.hConnect, CreateDir));
            }
            catch
            {
                flag = false;
            }
        }

        public void DeleteFile(string fileName)
        {
            FtpException.THROWONFALSE(NativeMethods.FtpDeleteFile(this.hConnect, fileName));
        }

        public bool DirectoryExist(string directory)
        {
            bool rc = false;
            try
            {
                rc = NativeMethods.FtpSetCurrentDirectory(this.hConnect, directory);
                FtpException.THROWONFALSE(rc);
                NativeMethods.FtpSetCurrentDirectory(this.hConnect, "..");
            }
            catch (Exception)
            {
                rc = false;
            }
            return rc;
        }

        public bool FileExist(string fileName)
        {
            try
            {
                int length = this.FindFiles(fileName).Length;
                return (length > 0);
            }
            catch
            {
                return false;
            }
        }

        ~FtpConnection()
        {
            ((IDisposable) this).Dispose();
        }

        public Win32FindData[] FindFiles() => 
            this.FindFiles(null);

        public Win32FindData[] FindFiles(string filter)
        {
            Win32FindData[] dataArray;
            IntPtr handle = NativeMethods.FtpFindFirstFile(this.hConnect, filter, out Win32FindData data, 0, IntPtr.Zero);
            FtpException.THROWONNULL(handle);
            try
            {
                ArrayList list = new ArrayList();
                do
                {
                    list.Add(data);
                }
                while (NativeMethods.InternetFindNextFile(handle, out data));
                dataArray = (Win32FindData[]) list.ToArray(typeof(Win32FindData));
            }
            finally
            {
                NativeMethods.InternetCloseHandle(handle);
            }
            return dataArray;
        }

        public string GetCurrentDirectory()
        {
            FtpException.THROWONFALSE(NativeMethods.FtpGetCurrentDirectory(this.hConnect, out string str));
            return str;
        }

        public void GetFile(string remoteFile, string localFile, bool failIfExists, FileAttributes attributes)
        {
            this.GetFile(remoteFile, localFile, failIfExists, attributes, FtpTransferType.Binary);
        }

        public void GetFile(string remoteFile, string localFile, bool failIfExists, FileAttributes attributes, FtpTransferType transfer)
        {
            FtpException.THROWONFALSE(NativeMethods.FtpGetFile(this.hConnect, remoteFile, localFile, failIfExists, attributes, transfer, IntPtr.Zero));
        }

        private static IntPtr InternalConnect(IntPtr hInt, string serverOrIp, int serverPort, string userName, string password)
        {
            IntPtr handle = NativeMethods.InternetConnect(hInt, serverOrIp, serverPort, userName, password, InternetService.Ftp, 0, IntPtr.Zero);
            FtpException.THROWONNULL(handle);
            return handle;
        }

        private static IntPtr Open()
        {
            IntPtr handle = NativeMethods.InternetOpen("FtpConnection", InternetOpenType.Direct, null, null, 0);
            FtpException.THROWONNULL(handle);
            return handle;
        }

        public FtpStream OpenFile(string fileName, GenericRights rights) => 
            this.OpenFile(fileName, rights, FtpTransferType.Binary);

        public FtpStream OpenFile(string fileName, GenericRights rights, FtpTransferType flags)
        {
            IntPtr handle = NativeMethods.FtpOpenFile(this.hConnect, fileName, rights, flags, IntPtr.Zero);
            FtpException.THROWONNULL(handle);
            return new FtpStream(handle, rights);
        }

        public void PutFile(string localFile, string newRemoteFile)
        {
            this.PutFile(localFile, newRemoteFile, FtpTransferType.Binary);
        }

        public void PutFile(string localFile, string newRemoteFile, FtpTransferType transfer)
        {
            FtpException.THROWONFALSE(NativeMethods.FtpPutFile(this.hConnect, localFile, newRemoteFile, transfer, IntPtr.Zero));
        }

        public void PutStream(Stream stream, string newRemoteFile)
        {
            this.PutStream(stream, newRemoteFile, FtpTransferType.Binary);
        }

        public void PutStream(Stream stream, string newRemoteFile, FtpTransferType transfer)
        {
            long position = 0L;
            if (stream.CanSeek)
            {
                position = stream.Position;
            }
            using (FtpStream stream2 = this.OpenFile(newRemoteFile, GenericRights.Write, transfer))
            {
                byte[] buffer = new byte[0x400];
                for (int i = stream.Read(buffer, 0, buffer.Length); i > 0; i = stream.Read(buffer, 0, buffer.Length))
                {
                    stream2.Write(buffer, 0, i);
                }
            }
            if (stream.CanSeek)
            {
                stream.Position = position;
            }
        }

        public void RemoveDirectory(string directory)
        {
            FtpException.THROWONFALSE(NativeMethods.FtpRemoveDirectory(this.hConnect, directory));
        }

        public void RenameFile(string existing, string newName)
        {
            FtpException.THROWONFALSE(NativeMethods.FtpRenameFile(this.hConnect, existing, newName));
        }

        internal static void SafeCloseHandle(ref IntPtr handle)
        {
            if (handle != IntPtr.Zero)
            {
                NativeMethods.InternetCloseHandle(handle);
                handle = IntPtr.Zero;
            }
        }

        public void SetCurrentDirectory(string directory)
        {
            FtpException.THROWONFALSE(NativeMethods.FtpSetCurrentDirectory(this.hConnect, directory));
        }

        void IDisposable.Dispose()
        {
            SafeCloseHandle(ref this.hConnect);
            SafeCloseHandle(ref this.hInternet);
            GC.SuppressFinalize(this);
        }
    }
}

