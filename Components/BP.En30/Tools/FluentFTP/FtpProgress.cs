namespace FluentFTP
{
    using System;
    using System.Runtime.CompilerServices;

    public class FtpProgress
    {
        public FtpProgress(int fileCount, int fileIndex)
        {
            this.FileCount = fileCount;
            this.FileIndex = fileIndex;
        }

        public FtpProgress(double progress, long bytesTransferred, double transferspeed, TimeSpan remainingtime, string localPath, string remotePath, FtpProgress metaProgress)
        {
            this.Progress = progress;
            this.TransferSpeed = transferspeed;
            this.ETA = remainingtime;
            this.LocalPath = localPath;
            this.RemotePath = remotePath;
            this.TransferredBytes = bytesTransferred;
            if (metaProgress != null)
            {
                this.FileCount = metaProgress.FileCount;
                this.FileIndex = metaProgress.FileIndex;
            }
        }

        public static FtpProgress Generate(long fileSize, long position, long bytesProcessed, TimeSpan elapsedtime, string localPath, string remotePath, FtpProgress metaProgress)
        {
            double d = -1.0;
            double num2 = 0.0;
            TimeSpan zero = TimeSpan.Zero;
            try
            {
                num2 = ((double) bytesProcessed) / elapsedtime.TotalSeconds;
                if (fileSize > 0L)
                {
                    d = (((double) position) / ((double) fileSize)) * 100.0;
                    zero = TimeSpan.FromSeconds(((double) (fileSize - position)) / num2);
                }
            }
            catch (Exception)
            {
            }
            if (double.IsNaN(d) && double.IsInfinity(d))
            {
                d = -1.0;
            }
            if (double.IsNaN(num2) && double.IsInfinity(num2))
            {
                num2 = 0.0;
            }
            return new FtpProgress(d, position, num2, zero, localPath, remotePath, metaProgress);
        }

        public string TransferSpeedToString()
        {
            double num = (this.TransferSpeed > 0.0) ? (this.TransferSpeed / 1024.0) : 0.0;
            if (num < 1024.0)
            {
                return (Math.Round(num, 2).ToString() + " KB/s");
            }
            num /= 1024.0;
            return (Math.Round(num, 2).ToString() + " MB/s");
        }

        public TimeSpan ETA { get; set; }

        public int FileCount { get; set; }

        public int FileIndex { get; set; }

        public string LocalPath { get; set; }

        public double Progress { get; set; }

        public string RemotePath { get; set; }

        public long TransferredBytes { get; set; }

        public double TransferSpeed { get; set; }
    }
}

