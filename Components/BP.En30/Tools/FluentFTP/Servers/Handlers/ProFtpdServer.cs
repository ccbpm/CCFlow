namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public class ProFtpdServer : FtpBaseServer
    {
        public override bool CreateDirectory(FtpClient client, string path, string ftppath, bool force)
        {
            if (client.HasFeature(FtpCapability.SITE_MKDIR))
            {
                if (client.Execute("SITE MKDIR " + ftppath).Success)
                {
                    client.LogStatus(FtpTraceLevel.Verbose, "Used the server-specific SITE MKDIR command to quickly create: " + ftppath);
                    return true;
                }
                client.LogStatus(FtpTraceLevel.Verbose, "Failed to use the server-specific SITE MKDIR command to quickly create: " + ftppath);
            }
            return false;
        }

        [AsyncStateMachine(typeof(<CreateDirectoryAsync>d__6))]
        public override Task<bool> CreateDirectoryAsync(FtpClient client, string path, string ftppath, bool force, CancellationToken token)
        {
            <CreateDirectoryAsync>d__6 d__;
            d__.client = client;
            d__.ftppath = ftppath;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<CreateDirectoryAsync>d__6>(ref d__);
            return d__.<>t__builder.Task;
        }

        public override bool DeleteDirectory(FtpClient client, string path, string ftppath, bool deleteContents, FtpListOption options)
        {
            if (deleteContents && client.HasFeature(FtpCapability.SITE_RMDIR))
            {
                if (client.Execute("SITE RMDIR " + ftppath).Success)
                {
                    client.LogStatus(FtpTraceLevel.Verbose, "Used the server-specific SITE RMDIR command to quickly delete directory: " + ftppath);
                    return true;
                }
                client.LogStatus(FtpTraceLevel.Verbose, "Failed to use the server-specific SITE RMDIR command to quickly delete directory: " + ftppath);
            }
            return false;
        }

        [AsyncStateMachine(typeof(<DeleteDirectoryAsync>d__4))]
        public override Task<bool> DeleteDirectoryAsync(FtpClient client, string path, string ftppath, bool deleteContents, FtpListOption options, CancellationToken token)
        {
            <DeleteDirectoryAsync>d__4 d__;
            d__.client = client;
            d__.ftppath = ftppath;
            d__.deleteContents = deleteContents;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<DeleteDirectoryAsync>d__4>(ref d__);
            return d__.<>t__builder.Task;
        }

        public override bool DetectByWelcome(string message)
        {
            return message.Contains("ProFTPD");
        }

        public override bool RecursiveList()
        {
            return true;
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.ProFTPD;
        }

        [CompilerGenerated]
        private struct <CreateDirectoryAsync>d__6 : IAsyncStateMachine
        {
            public int <>1__state;
            public AsyncTaskMethodBuilder<bool> <>t__builder;
            private TaskAwaiter<FtpReply> <>u__1;
            public FtpClient client;
            public string ftppath;
            public CancellationToken token;

            private void MoveNext()
            {
                bool flag;
                int num = this.<>1__state;
                try
                {
                    TaskAwaiter<FtpReply> awaiter;
                    if (num != 0)
                    {
                        if (this.client.HasFeature(FtpCapability.SITE_MKDIR))
                        {
                            awaiter = this.client.ExecuteAsync("SITE MKDIR " + this.ftppath, this.token).GetAwaiter();
                            if (!awaiter.IsCompleted)
                            {
                                this.<>1__state = num = 0;
                                this.<>u__1 = awaiter;
                                this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<FtpReply>, ProFtpdServer.<CreateDirectoryAsync>d__6>(ref awaiter, ref this);
                                return;
                            }
                            goto Label_008B;
                        }
                        goto Label_00D8;
                    }
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter<FtpReply>();
                    this.<>1__state = num = -1;
                Label_008B:
                    if (awaiter.GetResult().Success)
                    {
                        this.client.LogStatus(FtpTraceLevel.Verbose, "Used the server-specific SITE MKDIR command to quickly create: " + this.ftppath);
                        flag = true;
                        goto Label_00F5;
                    }
                    this.client.LogStatus(FtpTraceLevel.Verbose, "Failed to use the server-specific SITE MKDIR command to quickly create: " + this.ftppath);
                Label_00D8:
                    flag = false;
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
            Label_00F5:
                this.<>1__state = -2;
                this.<>t__builder.SetResult(flag);
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }

        [CompilerGenerated]
        private struct <DeleteDirectoryAsync>d__4 : IAsyncStateMachine
        {
            public int <>1__state;
            public AsyncTaskMethodBuilder<bool> <>t__builder;
            private TaskAwaiter<FtpReply> <>u__1;
            public FtpClient client;
            public bool deleteContents;
            public string ftppath;
            public CancellationToken token;

            private void MoveNext()
            {
                bool flag;
                int num = this.<>1__state;
                try
                {
                    TaskAwaiter<FtpReply> awaiter;
                    if (num != 0)
                    {
                        if (this.deleteContents && this.client.HasFeature(FtpCapability.SITE_RMDIR))
                        {
                            awaiter = this.client.ExecuteAsync("SITE RMDIR " + this.ftppath, this.token).GetAwaiter();
                            if (!awaiter.IsCompleted)
                            {
                                this.<>1__state = num = 0;
                                this.<>u__1 = awaiter;
                                this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<FtpReply>, ProFtpdServer.<DeleteDirectoryAsync>d__4>(ref awaiter, ref this);
                                return;
                            }
                            goto Label_0096;
                        }
                        goto Label_00E3;
                    }
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter<FtpReply>();
                    this.<>1__state = num = -1;
                Label_0096:
                    if (awaiter.GetResult().Success)
                    {
                        this.client.LogStatus(FtpTraceLevel.Verbose, "Used the server-specific SITE RMDIR command to quickly delete: " + this.ftppath);
                        flag = true;
                        goto Label_0100;
                    }
                    this.client.LogStatus(FtpTraceLevel.Verbose, "Failed to use the server-specific SITE RMDIR command to quickly delete: " + this.ftppath);
                Label_00E3:
                    flag = false;
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
            Label_0100:
                this.<>1__state = -2;
                this.<>t__builder.SetResult(flag);
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }
    }
}

