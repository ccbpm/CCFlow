namespace FluentFTP.Proxy
{
    using FluentFTP;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class FtpClientHttp11Proxy : FtpClientProxy
    {
        public FtpClientHttp11Proxy(ProxyInfo proxy) : base(proxy)
        {
            base.ConnectionType = "HTTP 1.1 Proxy";
        }

        [CompilerGenerated, DebuggerHidden]
        private Task <>n__0(FtpSocketStream stream, CancellationToken token)
        {
            return base.ConnectAsync(stream, token);
        }

        protected override void Connect(FtpSocketStream stream)
        {
            this.Connect(stream, base.Host, base.Port, FtpIpVersion.ANY);
        }

        protected override void Connect(FtpSocketStream stream, string host, int port, FtpIpVersion ipVersions)
        {
            base.Connect(stream);
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine("CONNECT {0}:{1} HTTP/1.1", host, port);
            writer.WriteLine("Host: {0}:{1}", host, port);
            if (base.Proxy.Credentials != null)
            {
                writer.WriteLine("Proxy-Authorization: Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(base.Proxy.Credentials.UserName + ":" + base.Proxy.Credentials.Password)));
            }
            writer.WriteLine("User-Agent: custom-ftp-client");
            writer.WriteLine();
            writer.Flush();
            this.ProxyHandshake(stream);
        }

        protected override Task ConnectAsync(FtpSocketStream stream, CancellationToken token)
        {
            return this.ConnectAsync(stream, base.Host, base.Port, FtpIpVersion.ANY, token);
        }

        [AsyncStateMachine(typeof(<ConnectAsync>d__6))]
        protected override Task ConnectAsync(FtpSocketStream stream, string host, int port, FtpIpVersion ipVersions, CancellationToken token)
        {
            <ConnectAsync>d__6 d__;
            d__.<>4__this = this;
            d__.stream = stream;
            d__.host = host;
            d__.port = port;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<ConnectAsync>d__6>(ref d__);
            return d__.<>t__builder.Task;
        }

        protected override FtpClient Create()
        {
            return new FtpClientHttp11Proxy(base.Proxy);
        }

        private FtpReply GetProxyReply(FtpSocketStream stream)
        {
            FtpReply reply = new FtpReply();
            object @lock = base.Lock;
            lock (@lock)
            {
                string str;
                if (!base.IsConnected)
                {
                    throw new InvalidOperationException("No connection to the server has been established.");
                }
                stream.ReadTimeout = base.ReadTimeout;
                while ((str = stream.ReadLine(base.Encoding)) != null)
                {
                    Match match;
                    base.LogLine(FtpTraceLevel.Info, str);
                    if ((match = Regex.Match(str, @"^HTTP/.*\s(?<code>[0-9]{3}) (?<message>.*)$")).Success)
                    {
                        reply.Code = match.Groups["code"].Value;
                        reply.Message = match.Groups["message"].Value;
                        break;
                    }
                    reply.InfoMessages = reply.InfoMessages + str + "\n";
                }
                while ((str = stream.ReadLine(base.Encoding)) != null)
                {
                    base.LogLine(FtpTraceLevel.Info, str);
                    if (FtpExtensions.IsNullOrWhiteSpace(str))
                    {
                        return reply;
                    }
                    reply.InfoMessages = reply.InfoMessages + str + "\n";
                }
            }
            return reply;
        }

        [AsyncStateMachine(typeof(<GetProxyReplyAsync>d__10))]
        private Task<FtpReply> GetProxyReplyAsync(FtpSocketStream stream, CancellationToken token = new CancellationToken())
        {
            <GetProxyReplyAsync>d__10 d__;
            d__.<>4__this = this;
            d__.stream = stream;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder<FtpReply>.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<GetProxyReplyAsync>d__10>(ref d__);
            return d__.<>t__builder.Task;
        }

        protected override void Handshake()
        {
            FtpReply reply = base.GetReply();
            if (!reply.Success)
            {
                string[] textArray1 = new string[] { "Can't connect ", base.Host, " via proxy ", base.Proxy.Host, ".\nMessage : ", reply.ErrorMessage };
                throw new FtpException(string.Concat(textArray1));
            }
            base.HandshakeReply = reply;
        }

        private void ProxyHandshake(FtpSocketStream stream)
        {
            FtpReply proxyReply = this.GetProxyReply(stream);
            if (!proxyReply.Success)
            {
                string[] textArray1 = new string[] { "Can't connect ", base.Host, " via proxy ", base.Proxy.Host, ".\nMessage : ", proxyReply.ErrorMessage };
                throw new FtpException(string.Concat(textArray1));
            }
        }

        [AsyncStateMachine(typeof(<ProxyHandshakeAsync>d__8))]
        private Task ProxyHandshakeAsync(FtpSocketStream stream, CancellationToken token = new CancellationToken())
        {
            <ProxyHandshakeAsync>d__8 d__;
            d__.<>4__this = this;
            d__.stream = stream;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<ProxyHandshakeAsync>d__8>(ref d__);
            return d__.<>t__builder.Task;
        }

        [CompilerGenerated]
        private struct <ConnectAsync>d__6 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpClientHttp11Proxy <>4__this;
            public AsyncTaskMethodBuilder <>t__builder;
            private TaskAwaiter <>u__1;
            private StreamWriter <writer>5__2;
            public string host;
            public int port;
            public FtpSocketStream stream;
            public CancellationToken token;

            private void MoveNext()
            {
                int num = this.<>1__state;
                FtpClientHttp11Proxy proxy = this.<>4__this;
                try
                {
                    TaskAwaiter awaiter;
                    switch (num)
                    {
                        case 0:
                            break;

                        case 1:
                            goto Label_0104;

                        case 2:
                            goto Label_017F;

                        case 3:
                            goto Label_0234;

                        case 4:
                            goto Label_0299;

                        case 5:
                            goto Label_02F9;

                        case 6:
                            goto Label_0359;

                        case 7:
                            goto Label_03BD;

                        default:
                            awaiter = proxy.<>n__0(this.stream, this.token).GetAwaiter();
                            if (awaiter.IsCompleted)
                            {
                                goto Label_0094;
                            }
                            this.<>1__state = num = 0;
                            this.<>u__1 = awaiter;
                            this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpClientHttp11Proxy.<ConnectAsync>d__6>(ref awaiter, ref this);
                            return;
                    }
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter();
                    this.<>1__state = num = -1;
                Label_0094:
                    awaiter.GetResult();
                    this.<writer>5__2 = new StreamWriter(this.stream);
                    awaiter = this.<writer>5__2.WriteLineAsync(string.Format("CONNECT {0}:{1} HTTP/1.1", this.host, this.port)).GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        goto Label_0120;
                    }
                    this.<>1__state = num = 1;
                    this.<>u__1 = awaiter;
                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpClientHttp11Proxy.<ConnectAsync>d__6>(ref awaiter, ref this);
                    return;
                Label_0104:
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter();
                    this.<>1__state = num = -1;
                Label_0120:
                    awaiter.GetResult();
                    awaiter = this.<writer>5__2.WriteLineAsync(string.Format("Host: {0}:{1}", this.host, this.port)).GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        goto Label_019B;
                    }
                    this.<>1__state = num = 2;
                    this.<>u__1 = awaiter;
                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpClientHttp11Proxy.<ConnectAsync>d__6>(ref awaiter, ref this);
                    return;
                Label_017F:
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter();
                    this.<>1__state = num = -1;
                Label_019B:
                    awaiter.GetResult();
                    if (proxy.Proxy.Credentials == null)
                    {
                        goto Label_0257;
                    }
                    string str = Convert.ToBase64String(Encoding.UTF8.GetBytes(proxy.Proxy.Credentials.UserName + ":" + proxy.Proxy.Credentials.Password));
                    awaiter = this.<writer>5__2.WriteLineAsync("Proxy-Authorization: Basic " + str).GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        goto Label_0250;
                    }
                    this.<>1__state = num = 3;
                    this.<>u__1 = awaiter;
                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpClientHttp11Proxy.<ConnectAsync>d__6>(ref awaiter, ref this);
                    return;
                Label_0234:
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter();
                    this.<>1__state = num = -1;
                Label_0250:
                    awaiter.GetResult();
                Label_0257:
                    awaiter = this.<writer>5__2.WriteLineAsync("User-Agent: custom-ftp-client").GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        goto Label_02B5;
                    }
                    this.<>1__state = num = 4;
                    this.<>u__1 = awaiter;
                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpClientHttp11Proxy.<ConnectAsync>d__6>(ref awaiter, ref this);
                    return;
                Label_0299:
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter();
                    this.<>1__state = num = -1;
                Label_02B5:
                    awaiter.GetResult();
                    awaiter = this.<writer>5__2.WriteLineAsync().GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        goto Label_0315;
                    }
                    this.<>1__state = num = 5;
                    this.<>u__1 = awaiter;
                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpClientHttp11Proxy.<ConnectAsync>d__6>(ref awaiter, ref this);
                    return;
                Label_02F9:
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter();
                    this.<>1__state = num = -1;
                Label_0315:
                    awaiter.GetResult();
                    awaiter = this.<writer>5__2.FlushAsync().GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        goto Label_0375;
                    }
                    this.<>1__state = num = 6;
                    this.<>u__1 = awaiter;
                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpClientHttp11Proxy.<ConnectAsync>d__6>(ref awaiter, ref this);
                    return;
                Label_0359:
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter();
                    this.<>1__state = num = -1;
                Label_0375:
                    awaiter.GetResult();
                    awaiter = proxy.ProxyHandshakeAsync(this.stream, this.token).GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        goto Label_03D9;
                    }
                    this.<>1__state = num = 7;
                    this.<>u__1 = awaiter;
                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpClientHttp11Proxy.<ConnectAsync>d__6>(ref awaiter, ref this);
                    return;
                Label_03BD:
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter();
                    this.<>1__state = num = -1;
                Label_03D9:
                    awaiter.GetResult();
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

        [CompilerGenerated]
        private struct <GetProxyReplyAsync>d__10 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpClientHttp11Proxy <>4__this;
            public AsyncTaskMethodBuilder<FtpReply> <>t__builder;
            private TaskAwaiter<string> <>u__1;
            private FtpReply <reply>5__2;
            public FtpSocketStream stream;
            public CancellationToken token;

            private void MoveNext()
            {
                FtpReply reply;
                int num = this.<>1__state;
                FtpClientHttp11Proxy proxy = this.<>4__this;
                try
                {
                    string str;
                    Match match;
                    TaskAwaiter<string> awaiter;
                    switch (num)
                    {
                        case 0:
                            goto Label_011B;

                        case 1:
                            goto Label_01BF;

                        default:
                            this.<reply>5__2 = new FtpReply();
                            if (!proxy.IsConnected)
                            {
                                throw new InvalidOperationException("No connection to the server has been established.");
                            }
                            this.stream.ReadTimeout = proxy.ReadTimeout;
                            goto Label_00D0;
                    }
                Label_0050:
                    proxy.LogLine(FtpTraceLevel.Info, str);
                    if ((match = Regex.Match(str, @"^HTTP/.*\s(?<code>[0-9]{3}) (?<message>.*)$")).Success)
                    {
                        this.<reply>5__2.Code = match.Groups["code"].Value;
                        this.<reply>5__2.Message = match.Groups["message"].Value;
                        goto Label_0177;
                    }
                    this.<reply>5__2.InfoMessages = this.<reply>5__2.InfoMessages + str + "\n";
                Label_00D0:
                    awaiter = this.stream.ReadLineAsync(proxy.Encoding, this.token).GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        goto Label_0138;
                    }
                    this.<>1__state = num = 0;
                    this.<>u__1 = awaiter;
                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<string>, FtpClientHttp11Proxy.<GetProxyReplyAsync>d__10>(ref awaiter, ref this);
                    return;
                Label_011B:
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter<string>();
                    this.<>1__state = num = -1;
                Label_0138:
                    if ((str = awaiter.GetResult()) != null)
                    {
                        goto Label_0050;
                    }
                    goto Label_0177;
                Label_0148:
                    proxy.LogLine(FtpTraceLevel.Info, str);
                    if (FtpExtensions.IsNullOrWhiteSpace(str))
                    {
                        goto Label_01EA;
                    }
                    this.<reply>5__2.InfoMessages = this.<reply>5__2.InfoMessages + str + "\n";
                Label_0177:
                    awaiter = this.stream.ReadLineAsync(proxy.Encoding, this.token).GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        goto Label_01DC;
                    }
                    this.<>1__state = num = 1;
                    this.<>u__1 = awaiter;
                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<string>, FtpClientHttp11Proxy.<GetProxyReplyAsync>d__10>(ref awaiter, ref this);
                    return;
                Label_01BF:
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter<string>();
                    this.<>1__state = num = -1;
                Label_01DC:
                    if ((str = awaiter.GetResult()) != null)
                    {
                        goto Label_0148;
                    }
                Label_01EA:
                    reply = this.<reply>5__2;
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
                this.<>1__state = -2;
                this.<>t__builder.SetResult(reply);
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }

        [CompilerGenerated]
        private struct <ProxyHandshakeAsync>d__8 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpClientHttp11Proxy <>4__this;
            public AsyncTaskMethodBuilder <>t__builder;
            private TaskAwaiter<FtpReply> <>u__1;
            public FtpSocketStream stream;
            public CancellationToken token;

            private void MoveNext()
            {
                int num = this.<>1__state;
                FtpClientHttp11Proxy proxy = this.<>4__this;
                try
                {
                    TaskAwaiter<FtpReply> awaiter;
                    if (num != 0)
                    {
                        awaiter = proxy.GetProxyReplyAsync(this.stream, this.token).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.<>1__state = num = 0;
                            this.<>u__1 = awaiter;
                            this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<FtpReply>, FtpClientHttp11Proxy.<ProxyHandshakeAsync>d__8>(ref awaiter, ref this);
                            return;
                        }
                    }
                    else
                    {
                        awaiter = this.<>u__1;
                        this.<>u__1 = new TaskAwaiter<FtpReply>();
                        this.<>1__state = num = -1;
                    }
                    FtpReply result = awaiter.GetResult();
                    if (!result.Success)
                    {
                        string[] textArray1 = new string[] { "Can't connect ", proxy.Host, " via proxy ", proxy.Proxy.Host, ".\nMessage : ", result.ErrorMessage };
                        throw new FtpException(string.Concat(textArray1));
                    }
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

