namespace BP
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class FtpStream : Stream
    {
        private IntPtr hFile;
        private GenericRights readOrWrite;

        public FtpStream(IntPtr hFile, GenericRights readOrWrite)
        {
            this.hFile = hFile;
            this.readOrWrite = readOrWrite;
        }

        public override void Close()
        {
            if (this.hFile != IntPtr.Zero)
            {
                BP.NativeMethods.InternetCloseHandle(this.hFile);
                this.hFile = IntPtr.Zero;
            }
            base.Close();
            GC.SuppressFinalize(this);
        }

        FtpStream()
        {
            this.Close();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int num;
            if (!this.CanRead)
            {
                throw new NotSupportedException();
            }
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, offset);
            bool rc = NativeMethods.InternetReadFile(this.hFile, ptr, count, out  num);
            handle.Free();
            FtpException.THROWONFALSE(rc);
            return num;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!this.CanWrite)
            {
                throw new NotSupportedException();
            }
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, offset);
            int a;
            bool rc = NativeMethods.InternetWriteFile(this.hFile, ptr, count, out a);
            handle.Free();
            FtpException.THROWONFALSE(rc);
        }

        public override bool CanRead =>
            (this.readOrWrite == GenericRights.Read);

        public override bool CanSeek =>
            false;

        public override bool CanWrite =>
            (this.readOrWrite == GenericRights.Write);

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}

