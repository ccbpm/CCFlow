namespace FluentFTP.Streams
{
    using FluentFTP;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    public static class FtpFileStream
    {
        public static void CompleteQuickFileWrite(Stream fileStream, string localPath)
        {
        }

        [AsyncStateMachine(typeof(<CompleteQuickFileWriteAsync>d__7))]
        public static Task CompleteQuickFileWriteAsync(Stream fileStream, string localPath, CancellationToken token)
        {
            <CompleteQuickFileWriteAsync>d__7 d__;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<CompleteQuickFileWriteAsync>d__7>(ref d__);
            return d__.<>t__builder.Task;
        }

        public static DateTime GetFileDateModifiedUtc(string localPath)
        {
            return new FileInfo(localPath).LastWriteTimeUtc;
        }

        [AsyncStateMachine(typeof(<GetFileDateModifiedUtcAsync>d__3))]
        public static Task<DateTime> GetFileDateModifiedUtcAsync(string localPath, CancellationToken token)
        {
            <GetFileDateModifiedUtcAsync>d__3 d__;
            d__.localPath = localPath;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder<DateTime>.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<GetFileDateModifiedUtcAsync>d__3>(ref d__);
            return d__.<>t__builder.Task;
        }

        public static Stream GetFileReadStream(FtpClient client, string localPath, bool isAsync, long fileSizeLimit, long knownLocalFileSize = 0L)
        {
            return new FileStream(localPath, FileMode.Open, FileAccess.Read, FileShare.Read, 0x1000, isAsync);
        }

        public static long GetFileSize(string localPath, bool checkExists)
        {
            if (checkExists && !File.Exists(localPath))
            {
                return 0L;
            }
            return new FileInfo(localPath).Length;
        }

        [AsyncStateMachine(typeof(<GetFileSizeAsync>d__1))]
        public static Task<long> GetFileSizeAsync(string localPath, bool checkExists, CancellationToken token)
        {
            <GetFileSizeAsync>d__1 d__;
            d__.localPath = localPath;
            d__.checkExists = checkExists;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder<long>.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<GetFileSizeAsync>d__1>(ref d__);
            return d__.<>t__builder.Task;
        }

        public static Stream GetFileWriteStream(FtpClient client, string localPath, bool isAsync, long fileSizeLimit, long knownRemoteFileSize = 0L, bool isAppend = false, long restartPos = 0L)
        {
            if (isAppend)
            {
                return new FileStream(localPath, FileMode.Append, FileAccess.Write, FileShare.None, 0x1000, isAsync);
            }
            return new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, 0x1000, isAsync);
        }

        [CompilerGenerated]
        private struct <CompleteQuickFileWriteAsync>d__7 : IAsyncStateMachine
        {
            public int <>1__state;
            public AsyncTaskMethodBuilder <>t__builder;

            private void MoveNext()
            {
                try
                {
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                }
                this.<>1__state = -2;
                this.<>t__builder.SetResult();
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }

        [CompilerGenerated]
        private struct <GetFileDateModifiedUtcAsync>d__3 : IAsyncStateMachine
        {
            public int <>1__state;
            public AsyncTaskMethodBuilder<DateTime> <>t__builder;
            private TaskAwaiter<FileInfo> <>u__1;
            public string localPath;
            public CancellationToken token;

            private void MoveNext()
            {
                DateTime lastWriteTimeUtc;
                int num = this.<>1__state;
                try
                {
                    TaskAwaiter<FileInfo> awaiter;
                    if (num != 0)
                    {
                        FtpFileStream.<>c__DisplayClass3_0 class_1;
                        string localPath = this.localPath;
                        awaiter = Task.Run<FileInfo>(new Func<FileInfo>(class_1.<GetFileDateModifiedUtcAsync>b__0), this.token).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.<>1__state = num = 0;
                            this.<>u__1 = awaiter;
                            this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<FileInfo>, FtpFileStream.<GetFileDateModifiedUtcAsync>d__3>(ref awaiter, ref this);
                            return;
                        }
                    }
                    else
                    {
                        awaiter = this.<>u__1;
                        this.<>u__1 = new TaskAwaiter<FileInfo>();
                        this.<>1__state = num = -1;
                    }
                    lastWriteTimeUtc = awaiter.GetResult().LastWriteTimeUtc;
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
                this.<>1__state = -2;
                this.<>t__builder.SetResult(lastWriteTimeUtc);
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }

        [CompilerGenerated]
        private struct <GetFileSizeAsync>d__1 : IAsyncStateMachine
        {
            public int <>1__state;
            private FtpFileStream.<>c__DisplayClass1_0 <>8__1;
            public AsyncTaskMethodBuilder<long> <>t__builder;
            private TaskAwaiter<bool> <>u__1;
            private TaskAwaiter<FileInfo> <>u__2;
            public bool checkExists;
            public string localPath;
            public CancellationToken token;

            private void MoveNext()
            {
                long num2;
                int num = this.<>1__state;
                try
                {
                    TaskAwaiter<bool> awaiter;
                    TaskAwaiter<FileInfo> awaiter2;
                    if (num != 0)
                    {
                        if (num != 1)
                        {
                            this.<>8__1 = new FtpFileStream.<>c__DisplayClass1_0();
                            this.<>8__1.localPath = this.localPath;
                            if (this.checkExists)
                            {
                                awaiter = Task.Run<bool>(new Func<bool>(this.<>8__1.<GetFileSizeAsync>b__0), this.token).GetAwaiter();
                                if (!awaiter.IsCompleted)
                                {
                                    this.<>1__state = num = 0;
                                    this.<>u__1 = awaiter;
                                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, FtpFileStream.<GetFileSizeAsync>d__1>(ref awaiter, ref this);
                                    return;
                                }
                                goto Label_009F;
                            }
                            goto Label_00B0;
                        }
                        goto Label_00FB;
                    }
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter<bool>();
                    this.<>1__state = num = -1;
                Label_009F:
                    if (!awaiter.GetResult())
                    {
                        num2 = 0L;
                        goto Label_013F;
                    }
                Label_00B0:
                    awaiter2 = Task.Run<FileInfo>(new Func<FileInfo>(this.<>8__1.<GetFileSizeAsync>b__1), this.token).GetAwaiter();
                    if (awaiter2.IsCompleted)
                    {
                        goto Label_0117;
                    }
                    this.<>1__state = num = 1;
                    this.<>u__2 = awaiter2;
                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<FileInfo>, FtpFileStream.<GetFileSizeAsync>d__1>(ref awaiter2, ref this);
                    return;
                Label_00FB:
                    awaiter2 = this.<>u__2;
                    this.<>u__2 = new TaskAwaiter<FileInfo>();
                    this.<>1__state = num = -1;
                Label_0117:
                    num2 = awaiter2.GetResult().Length;
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
            Label_013F:
                this.<>1__state = -2;
                this.<>t__builder.SetResult(num2);
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }
    }
}

