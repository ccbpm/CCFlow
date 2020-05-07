namespace FluentFTP
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public class FtpDataStream : FtpSocketStream
    {
        private FtpReply m_commandStatus;
        private FtpClient m_control;
        private long m_length;
        private long m_position;

        public FtpDataStream(FtpClient conn) : base(conn.SslProtocols)
        {
            if (conn == null)
            {
                throw new ArgumentException("The control connection cannot be null.");
            }
            this.ControlConnection = conn;
            base.ValidateCertificate += (<>c.<>9__22_0 ?? (<>c.<>9__22_0 = new FtpSocketStreamSslValidation(<>c.<>9.<.ctor>b__22_0)));
            this.m_position = 0L;
        }

        [CompilerGenerated, DebuggerHidden]
        private Task<int> <>n__0(byte[] buffer, int offset, int count, CancellationToken token)
        {
            return base.ReadAsync(buffer, offset, count, token);
        }

        [CompilerGenerated, DebuggerHidden]
        private Task <>n__1(byte[] buffer, int offset, int count, CancellationToken token)
        {
            return base.WriteAsync(buffer, offset, count, token);
        }

        public FtpReply Close()
        {
            base.Close();
            try
            {
                if (this.ControlConnection != null)
                {
                    return this.ControlConnection.CloseDataStream(this);
                }
            }
            finally
            {
                this.m_commandStatus = new FtpReply();
                this.m_control = null;
            }
            return new FtpReply();
        }

        ~FtpDataStream()
        {
            try
            {
                this.Dispose(false);
            }
            catch (Exception)
            {
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int num = base.Read(buffer, offset, count);
            this.m_position += num;
            return num;
        }

        [AsyncStateMachine(typeof(<ReadAsync>d__16))]
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            <ReadAsync>d__16 d__;
            d__.<>4__this = this;
            d__.buffer = buffer;
            d__.offset = offset;
            d__.count = count;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<ReadAsync>d__16>(ref d__);
            return d__.<>t__builder.Task;
        }

        public override void SetLength(long value)
        {
            this.m_length = value;
        }

        public void SetPosition(long pos)
        {
            this.m_position = pos;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);
            this.m_position += count;
        }

        [AsyncStateMachine(typeof(<WriteAsync>d__18))]
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            <WriteAsync>d__18 d__;
            d__.<>4__this = this;
            d__.buffer = buffer;
            d__.offset = offset;
            d__.count = count;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<WriteAsync>d__18>(ref d__);
            return d__.<>t__builder.Task;
        }

        public FtpReply CommandStatus
        {
            get
            {
                return this.m_commandStatus;
            }
            set
            {
                this.m_commandStatus = value;
            }
        }

        public FtpClient ControlConnection
        {
            get
            {
                return this.m_control;
            }
            set
            {
                this.m_control = value;
            }
        }

        public override long Length
        {
            get
            {
                return this.m_length;
            }
        }

        public override long Position
        {
            get
            {
                return this.m_position;
            }
            set
            {
                throw new InvalidOperationException("You cannot modify the position of a FtpDataStream. This property is updated as data is read or written to the stream.");
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class <>c
        {
            public static readonly FtpDataStream.<>c <>9 = new FtpDataStream.<>c();
            public static FtpSocketStreamSslValidation <>9__22_0;

            internal void <.ctor>b__22_0(FtpSocketStream obj, FtpSslValidationEventArgs e)
            {
                e.Accept = true;
            }
        }

        [CompilerGenerated]
        private struct <ReadAsync>d__16 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpDataStream <>4__this;
            public AsyncTaskMethodBuilder<int> <>t__builder;
            private TaskAwaiter<int> <>u__1;
            public byte[] buffer;
            public int count;
            public int offset;
            public CancellationToken token;

            private void MoveNext()
            {
                int num2;
                int num = this.<>1__state;
                FtpDataStream stream = this.<>4__this;
                try
                {
                    TaskAwaiter<int> awaiter;
                    if (num != 0)
                    {
                        awaiter = stream.<>n__0(this.buffer, this.offset, this.count, this.token).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.<>1__state = num = 0;
                            this.<>u__1 = awaiter;
                            this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<int>, FtpDataStream.<ReadAsync>d__16>(ref awaiter, ref this);
                            return;
                        }
                    }
                    else
                    {
                        awaiter = this.<>u__1;
                        this.<>u__1 = new TaskAwaiter<int>();
                        this.<>1__state = num = -1;
                    }
                    int result = awaiter.GetResult();
                    stream.m_position += result;
                    num2 = result;
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
                this.<>1__state = -2;
                this.<>t__builder.SetResult(num2);
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }

        [CompilerGenerated]
        private struct <WriteAsync>d__18 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpDataStream <>4__this;
            public AsyncTaskMethodBuilder <>t__builder;
            private TaskAwaiter <>u__1;
            public byte[] buffer;
            public int count;
            public int offset;
            public CancellationToken token;

            private void MoveNext()
            {
                int num = this.<>1__state;
                FtpDataStream stream = this.<>4__this;
                try
                {
                    TaskAwaiter awaiter;
                    if (num != 0)
                    {
                        awaiter = stream.<>n__1(this.buffer, this.offset, this.count, this.token).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.<>1__state = num = 0;
                            this.<>u__1 = awaiter;
                            this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpDataStream.<WriteAsync>d__18>(ref awaiter, ref this);
                            return;
                        }
                    }
                    else
                    {
                        awaiter = this.<>u__1;
                        this.<>u__1 = new TaskAwaiter();
                        this.<>1__state = num = -1;
                    }
                    awaiter.GetResult();
                    stream.m_position += this.count;
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
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
    }
}

