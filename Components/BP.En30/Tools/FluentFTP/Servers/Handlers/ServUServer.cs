namespace FluentFTP.Servers.Handlers
{
    using FluentFTP;
    using FluentFTP.Servers;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public class ServUServer : FtpBaseServer
    {
        public override bool DeleteDirectory(FtpClient client, string path, string ftppath, bool deleteContents, FtpListOption options)
        {
            if (deleteContents && client.HasFeature(FtpCapability.RMDA))
            {
                if (client.Execute("RMDA " + ftppath).Success)
                {
                    client.LogStatus(FtpTraceLevel.Verbose, "Used the server-specific RMDA command to quickly delete directory: " + ftppath);
                    return true;
                }
                client.LogStatus(FtpTraceLevel.Verbose, "Failed to use the server-specific RMDA command to quickly delete directory: " + ftppath);
            }
            return false;
        }

        [AsyncStateMachine(typeof(<DeleteDirectoryAsync>d__3))]
        public override Task<bool> DeleteDirectoryAsync(FtpClient client, string path, string ftppath, bool deleteContents, FtpListOption options, CancellationToken token)
        {
            <DeleteDirectoryAsync>d__3 d__;
            d__.client = client;
            d__.ftppath = ftppath;
            d__.deleteContents = deleteContents;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<DeleteDirectoryAsync>d__3>(ref d__);
            return d__.<>t__builder.Task;
        }

        public override bool DetectByWelcome(string message)
        {
            return message.Contains("Serv-U FTP");
        }

        public override FtpServer ToEnum()
        {
            return FtpServer.ServU;
        }

        [CompilerGenerated]
        private struct <DeleteDirectoryAsync>d__3 : IAsyncStateMachine
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
                        if (this.deleteContents && this.client.HasFeature(FtpCapability.RMDA))
                        {
                            awaiter = this.client.ExecuteAsync("RMDA " + this.ftppath, this.token).GetAwaiter();
                            if (!awaiter.IsCompleted)
                            {
                                this.<>1__state = num = 0;
                                this.<>u__1 = awaiter;
                                this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<FtpReply>, ServUServer.<DeleteDirectoryAsync>d__3>(ref awaiter, ref this);
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
                        this.client.LogStatus(FtpTraceLevel.Verbose, "Used the server-specific RMDA command to quickly delete directory: " + this.ftppath);
                        flag = true;
                        goto Label_0100;
                    }
                    this.client.LogStatus(FtpTraceLevel.Verbose, "Failed to use the server-specific RMDA command to quickly delete directory: " + this.ftppath);
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

