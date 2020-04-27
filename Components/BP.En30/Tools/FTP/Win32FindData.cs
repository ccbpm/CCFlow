namespace BP
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto, Pack=4)]
    public struct Win32FindData
    {
        public System.IO.FileAttributes FileAttributes;
        private long ftCreationTime;
        private long ftLastAccessTime;
        private long ftLastWriteTime;
        private int nFileSizeHigh;
        private int nFileSizeLow;
        private int dwReserved0;
        private int dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=260)]
        public string FileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=14)]
        public string AlternateFileName;
        public long FileSize
        {
            get
            {
                long nFileSizeHigh = this.nFileSizeHigh;
                return (nFileSizeHigh | this.nFileSizeLow);
            }
        }
        public DateTime CreationTime =>
            DateTime.FromFileTime(this.ftCreationTime);
        public DateTime LastAccessTime =>
            DateTime.FromFileTime(this.ftLastAccessTime);
        public DateTime LastWriteTime =>
            DateTime.FromFileTime(this.ftLastWriteTime);
    }
}

