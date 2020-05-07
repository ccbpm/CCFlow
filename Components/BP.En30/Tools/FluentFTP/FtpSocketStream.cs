namespace FluentFTP
{
    using FluentFTP.Exceptions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class FtpSocketStream : Stream, IDisposable
    {
        public FtpClient Client;
        private BufferedStream m_bufStream;
        private int m_connectTimeout = 0x7530;
        private DateTime m_lastActivity = DateTime.Now;
        private System.Net.Sockets.NetworkStream m_netStream;
        private int m_readTimeout = -1;
        private System.Net.Sockets.Socket m_socket;
        private int m_socketPollInterval = 0x3a98;
        private SslProtocols m_SslProtocols;
        private System.Net.Security.SslStream m_sslStream;

        [field: CompilerGenerated]
        private event FtpSocketStreamSslValidation m_sslvalidate;

        public event FtpSocketStreamSslValidation ValidateCertificate
        {
            add
            {
                this.m_sslvalidate += value;
            }
            remove
            {
                this.m_sslvalidate -= value;
            }
        }

        public FtpSocketStream(SslProtocols defaultSslProtocols)
        {
            this.m_SslProtocols = defaultSslProtocols;
        }

        public void Accept()
        {
            if (this.m_socket != null)
            {
                this.m_socket = this.m_socket.Accept();
            }
        }

        [AsyncStateMachine(typeof(<AcceptAsync>d__92))]
        public Task AcceptAsync()
        {
            <AcceptAsync>d__92 d__;
            d__.<>4__this = this;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<AcceptAsync>d__92>(ref d__);
            return d__.<>t__builder.Task;
        }

        public void ActivateEncryption(string targethost)
        {
            this.ActivateEncryption(targethost, null, this.m_SslProtocols);
        }

        public void ActivateEncryption(string targethost, X509CertificateCollection clientCerts)
        {
            this.ActivateEncryption(targethost, clientCerts, this.m_SslProtocols);
        }

        public void ActivateEncryption(string targethost, X509CertificateCollection clientCerts, SslProtocols sslProtocols)
        {
            if (!this.IsConnected)
            {
                throw new InvalidOperationException("The FtpSocketStream object is not connected.");
            }
            if (this.m_netStream == null)
            {
                throw new InvalidOperationException("The base network stream is null.");
            }
            if (this.m_sslStream != null)
            {
                throw new InvalidOperationException("SSL Encryption has already been enabled on this stream.");
            }
            try
            {
                this.CreateBufferStream();
                this.m_sslStream = new FtpSslStream(this.GetBufferStream(), true, (sender, certificate, chain, sslPolicyErrors) => this.OnValidateCertificate(certificate, chain, sslPolicyErrors));
                DateTime now = DateTime.Now;
                try
                {
                    this.m_sslStream.AuthenticateAsClient(targethost, clientCerts, sslProtocols, this.Client.ValidateCertificateRevocation);
                }
                catch (IOException exception)
                {
                    if ((exception.InnerException is Win32Exception) && (((Win32Exception) exception.InnerException).NativeErrorCode == 0x2745))
                    {
                        throw new FtpMissingSocketException(exception);
                    }
                    throw;
                }
                TimeSpan span = DateTime.Now.Subtract(now);
                this.Client.LogStatus(FtpTraceLevel.Info, "FTPS Authentication Successful");
                this.Client.LogStatus(FtpTraceLevel.Verbose, string.Concat(new object[] { "Time to activate encryption: ", span.Hours, "h ", span.Minutes, "m ", span.Seconds, "s.  Total Seconds: ", span.TotalSeconds, "." }));
            }
            catch (AuthenticationException)
            {
                this.Close();
                this.Client.LogStatus(FtpTraceLevel.Error, "FTPS Authentication Failed");
                throw;
            }
        }

        [AsyncStateMachine(typeof(<ActivateEncryptionAsync>d__82))]
        public Task ActivateEncryptionAsync(string targethost)
        {
            <ActivateEncryptionAsync>d__82 d__;
            d__.<>4__this = this;
            d__.targethost = targethost;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<ActivateEncryptionAsync>d__82>(ref d__);
            return d__.<>t__builder.Task;
        }

        [AsyncStateMachine(typeof(<ActivateEncryptionAsync>d__84))]
        public Task ActivateEncryptionAsync(string targethost, X509CertificateCollection clientCerts)
        {
            <ActivateEncryptionAsync>d__84 d__;
            d__.<>4__this = this;
            d__.targethost = targethost;
            d__.clientCerts = clientCerts;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<ActivateEncryptionAsync>d__84>(ref d__);
            return d__.<>t__builder.Task;
        }

        [AsyncStateMachine(typeof(<ActivateEncryptionAsync>d__88))]
        public Task ActivateEncryptionAsync(string targethost, X509CertificateCollection clientCerts, SslProtocols sslProtocols)
        {
            <ActivateEncryptionAsync>d__88 d__;
            d__.<>4__this = this;
            d__.targethost = targethost;
            d__.clientCerts = clientCerts;
            d__.sslProtocols = sslProtocols;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<ActivateEncryptionAsync>d__88>(ref d__);
            return d__.<>t__builder.Task;
        }

        public IAsyncResult BeginAccept(AsyncCallback callback, object state)
        {
            if (this.m_socket != null)
            {
                return this.m_socket.BeginAccept(callback, state);
            }
            return null;
        }

        public void Connect(string host, int port, FtpIpVersion ipVersions)
        {
            IAsyncResult asyncResult = null;
            IPAddress[] hostAddresses = Dns.GetHostAddresses(host);
            if (ipVersions == 0)
            {
                throw new ArgumentException("The ipVersions parameter must contain at least 1 flag.");
            }
            for (int i = 0; i < hostAddresses.Length; i++)
            {
                if (ipVersions == FtpIpVersion.ANY)
                {
                    goto Label_004A;
                }
                AddressFamily addressFamily = hostAddresses[i].AddressFamily;
                if (addressFamily != AddressFamily.InterNetwork)
                {
                    if (addressFamily == AddressFamily.InterNetworkV6)
                    {
                        goto Label_0041;
                    }
                    goto Label_004A;
                }
                if ((ipVersions & FtpIpVersion.IPv4) == FtpIpVersion.IPv4)
                {
                    goto Label_004A;
                }
                continue;
            Label_0041:
                if ((ipVersions & FtpIpVersion.IPv6) != FtpIpVersion.IPv6)
                {
                    continue;
                }
            Label_004A:
                if (FtpTrace.LogIP)
                {
                    this.Client.LogStatus(FtpTraceLevel.Info, string.Concat(new object[] { "Connecting to ", hostAddresses[i].ToString(), ":", port }));
                }
                else
                {
                    this.Client.LogStatus(FtpTraceLevel.Info, "Connecting to ***:" + port);
                }
                this.m_socket = new System.Net.Sockets.Socket(hostAddresses[i].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                asyncResult = this.m_socket.BeginConnect(hostAddresses[i], port, null, null);
                asyncResult.AsyncWaitHandle.Close();
                if (!asyncResult.AsyncWaitHandle.WaitOne(this.m_connectTimeout, true))
                {
                    this.Close();
                    if ((i + 1) == hostAddresses.Length)
                    {
                        throw new TimeoutException("Timed out trying to connect!");
                    }
                }
                else
                {
                    this.m_socket.EndConnect(asyncResult);
                    break;
                }
            }
            if ((this.m_socket == null) || !this.m_socket.Connected)
            {
                this.Close();
                throw new IOException("Failed to connect to host.");
            }
            this.m_netStream = new System.Net.Sockets.NetworkStream(this.m_socket);
            this.m_netStream.ReadTimeout = this.m_readTimeout;
            this.m_lastActivity = DateTime.Now;
        }

        [AsyncStateMachine(typeof(<ConnectAsync>d__80))]
        public Task ConnectAsync(string host, int port, FtpIpVersion ipVersions, CancellationToken token)
        {
            <ConnectAsync>d__80 d__;
            d__.<>4__this = this;
            d__.host = host;
            d__.port = port;
            d__.ipVersions = ipVersions;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<ConnectAsync>d__80>(ref d__);
            return d__.<>t__builder.Task;
        }

        private void CreateBufferStream()
        {
            if ((this.Client.SslBuffering == FtpsBuffering.On) || ((this.Client.SslBuffering == FtpsBuffering.Auto) && !this.Client.IsProxy()))
            {
                this.m_bufStream = new BufferedStream(this.NetworkStream, 0x14000);
            }
            else
            {
                this.m_bufStream = null;
            }
        }

        public void DeactivateEncryption()
        {
            if (!this.IsConnected)
            {
                throw new InvalidOperationException("The FtpSocketStream object is not connected.");
            }
            if (this.m_sslStream == null)
            {
                throw new InvalidOperationException("SSL Encryption has not been enabled on this stream.");
            }
            this.m_sslStream.Close();
            this.m_sslStream = null;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this.Client != null)
                {
                    this.Client.LogStatus(FtpTraceLevel.Verbose, "Disposing FtpSocketStream...");
                }
            }
            catch (Exception)
            {
            }
            if (this.m_bufStream != null)
            {
                try
                {
                    this.m_bufStream.Flush();
                }
                catch (Exception)
                {
                }
                this.m_bufStream = null;
            }
            if (this.m_socket != null)
            {
                try
                {
                    if (this.m_socket.Connected)
                    {
                        this.m_socket.Close();
                    }
                    this.m_socket.Dispose();
                }
                catch (Exception)
                {
                }
                this.m_socket = null;
            }
            if (this.m_netStream != null)
            {
                try
                {
                    this.m_netStream.Dispose();
                }
                catch (Exception)
                {
                }
                this.m_netStream = null;
            }
            if (this.m_sslStream != null)
            {
                try
                {
                    this.m_sslStream.Dispose();
                }
                catch (Exception)
                {
                }
                this.m_sslStream = null;
            }
        }

        [AsyncStateMachine(typeof(<EnableCancellation>d__64))]
        internal Task EnableCancellation(Task task, CancellationToken token, Action action)
        {
            <EnableCancellation>d__64 d__;
            d__.task = task;
            d__.token = token;
            d__.action = action;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<EnableCancellation>d__64>(ref d__);
            return d__.<>t__builder.Task;
        }

        [AsyncStateMachine(typeof(<EnableCancellation>d__65))]
        internal Task<T> EnableCancellation<T>(Task<T> task, CancellationToken token, Action action)
        {
            <EnableCancellation>d__65<T> d__;
            d__.task = task;
            d__.token = token;
            d__.action = action;
            d__.<>t__builder = AsyncTaskMethodBuilder<T>.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<EnableCancellation>d__65<T>>(ref d__);
            return d__.<>t__builder.Task;
        }

        public void EndAccept(IAsyncResult ar)
        {
            if (this.m_socket != null)
            {
                this.m_socket = this.m_socket.EndAccept(ar);
                this.m_netStream = new System.Net.Sockets.NetworkStream(this.m_socket);
                this.m_netStream.ReadTimeout = this.m_readTimeout;
            }
        }

        public override void Flush()
        {
            if (!this.IsConnected)
            {
                throw new InvalidOperationException("The FtpSocketStream object is not connected.");
            }
            if (this.BaseStream == null)
            {
                throw new InvalidOperationException("The base stream of the FtpSocketStream object is null.");
            }
            this.BaseStream.Flush();
        }

        [AsyncStateMachine(typeof(<FlushAsync>d__62))]
        public override Task FlushAsync(CancellationToken token)
        {
            <FlushAsync>d__62 d__;
            d__.<>4__this = this;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<FlushAsync>d__62>(ref d__);
            return d__.<>t__builder.Task;
        }

        private Stream GetBufferStream()
        {
            if (this.m_bufStream == null)
            {
                return this.NetworkStream;
            }
            return this.m_bufStream;
        }

        public void Listen(IPAddress address, int port)
        {
            if (!this.IsConnected)
            {
                if (this.m_socket == null)
                {
                    this.m_socket = new System.Net.Sockets.Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                }
                this.m_socket.Bind(new IPEndPoint(address, port));
                this.m_socket.Listen(1);
            }
        }

        protected bool OnValidateCertificate(X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            FtpSocketStreamSslValidation sslvalidate = this.m_sslvalidate;
            if (sslvalidate != null)
            {
                FtpSslValidationEventArgs e = new FtpSslValidationEventArgs {
                    Certificate = certificate,
                    Chain = chain,
                    PolicyErrors = errors,
                    Accept = errors == SslPolicyErrors.None
                };
                sslvalidate(this, e);
                return e.Accept;
            }
            return (errors == SslPolicyErrors.None);
        }

        internal int RawSocketRead(byte[] buffer)
        {
            int num = 0;
            if ((this.m_socket != null) && this.m_socket.Connected)
            {
                num = this.m_socket.Receive(buffer, buffer.Length, SocketFlags.None);
            }
            return num;
        }

        [AsyncStateMachine(typeof(<RawSocketReadAsync>d__66))]
        internal Task<int> RawSocketReadAsync(byte[] buffer, CancellationToken token)
        {
            <RawSocketReadAsync>d__66 d__;
            d__.<>4__this = this;
            d__.buffer = buffer;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<RawSocketReadAsync>d__66>(ref d__);
            return d__.<>t__builder.Task;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            IAsyncResult asyncResult = null;
            if (this.BaseStream == null)
            {
                return 0;
            }
            this.m_lastActivity = DateTime.Now;
            asyncResult = this.BaseStream.BeginRead(buffer, offset, count, null, null);
            asyncResult.AsyncWaitHandle.Close();
            if (!asyncResult.AsyncWaitHandle.WaitOne(this.m_readTimeout, true))
            {
                this.Close();
                throw new TimeoutException("Timed out trying to read data from the socket stream!");
            }
            return this.BaseStream.EndRead(asyncResult);
        }

        [IteratorStateMachine(typeof(<ReadAllLines>d__70))]
        public IEnumerable<string> ReadAllLines(Encoding encoding, int bufferSize)
        {
            return new <ReadAllLines>d__70(-2) { <>4__this = this, <>3__encoding = encoding, <>3__bufferSize = bufferSize };
        }

        [AsyncStateMachine(typeof(<ReadAllLinesAsync>d__72))]
        public Task<IEnumerable<string>> ReadAllLinesAsync(Encoding encoding, int bufferSize, CancellationToken token)
        {
            <ReadAllLinesAsync>d__72 d__;
            d__.<>4__this = this;
            d__.encoding = encoding;
            d__.bufferSize = bufferSize;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder<IEnumerable<string>>.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<ReadAllLinesAsync>d__72>(ref d__);
            return d__.<>t__builder.Task;
        }

        [AsyncStateMachine(typeof(<ReadAsync>d__68))]
        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            <ReadAsync>d__68 d__;
            d__.<>4__this = this;
            d__.buffer = buffer;
            d__.offset = offset;
            d__.count = count;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<ReadAsync>d__68>(ref d__);
            return d__.<>t__builder.Task;
        }

        public string ReadLine(Encoding encoding)
        {
            List<byte> list = new List<byte>();
            byte[] buffer = new byte[1];
            while (this.Read(buffer, 0, buffer.Length) > 0)
            {
                list.Add(buffer[0]);
                if (buffer[0] == 10)
                {
                    char[] trimChars = new char[] { '\r', '\n' };
                    return encoding.GetString(list.ToArray()).Trim(trimChars);
                }
            }
            return null;
        }

        [AsyncStateMachine(typeof(<ReadLineAsync>d__71))]
        public Task<string> ReadLineAsync(Encoding encoding, CancellationToken token)
        {
            <ReadLineAsync>d__71 d__;
            d__.<>4__this = this;
            d__.encoding = encoding;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<ReadLineAsync>d__71>(ref d__);
            return d__.<>t__builder.Task;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException();
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException();
        }

        public void SetSocketOption(SocketOptionLevel level, SocketOptionName name, bool value)
        {
            if (this.m_socket == null)
            {
                throw new InvalidOperationException("The underlying socket is null. Have you established a connection?");
            }
            this.m_socket.SetSocketOption(level, name, value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.BaseStream != null)
            {
                this.BaseStream.Write(buffer, offset, count);
                this.m_lastActivity = DateTime.Now;
            }
        }

        [AsyncStateMachine(typeof(<WriteAsync>d__74))]
        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken token)
        {
            <WriteAsync>d__74 d__;
            d__.<>4__this = this;
            d__.buffer = buffer;
            d__.offset = offset;
            d__.count = count;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<WriteAsync>d__74>(ref d__);
            return d__.<>t__builder.Task;
        }

        public void WriteLine(Encoding encoding, string buf)
        {
            byte[] bytes = encoding.GetBytes(buf + "\r\n");
            this.Write(bytes, 0, bytes.Length);
        }

        [AsyncStateMachine(typeof(<WriteLineAsync>d__76))]
        public Task WriteLineAsync(Encoding encoding, string buf, CancellationToken token)
        {
            <WriteLineAsync>d__76 d__;
            d__.<>4__this = this;
            d__.encoding = encoding;
            d__.buf = buf;
            d__.token = token;
            d__.<>t__builder = AsyncTaskMethodBuilder.Create();
            d__.<>1__state = -1;
            d__.<>t__builder.Start<<WriteLineAsync>d__76>(ref d__);
            return d__.<>t__builder.Task;
        }

        protected Stream BaseStream
        {
            get
            {
                if (this.m_sslStream != null)
                {
                    return this.m_sslStream;
                }
                if (this.m_netStream != null)
                {
                    return this.m_netStream;
                }
                return null;
            }
        }

        public override bool CanRead
        {
            get
            {
                return ((this.m_netStream != null) && this.m_netStream.CanRead);
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return ((this.m_netStream != null) && this.m_netStream.CanWrite);
            }
        }

        public int ConnectTimeout
        {
            get
            {
                return this.m_connectTimeout;
            }
            set
            {
                this.m_connectTimeout = value;
            }
        }

        public bool IsConnected
        {
            get
            {
                try
                {
                    if (this.m_socket == null)
                    {
                        return false;
                    }
                    if (!this.m_socket.Connected)
                    {
                        this.Close();
                        return false;
                    }
                    if (!this.CanRead || !this.CanWrite)
                    {
                        this.Close();
                        return false;
                    }
                    if ((this.m_socketPollInterval > 0) && (DateTime.Now.Subtract(this.m_lastActivity).TotalMilliseconds > this.m_socketPollInterval))
                    {
                        this.Client.LogStatus(FtpTraceLevel.Verbose, "Testing connectivity using Socket.Poll()...");
                        this.m_lastActivity = DateTime.Now;
                        if (this.m_socket.Poll(0x7a120, SelectMode.SelectRead) && (this.m_socket.Available == 0))
                        {
                            this.Close();
                            return false;
                        }
                    }
                }
                catch (SocketException exception)
                {
                    this.Close();
                    this.Client.LogStatus(FtpTraceLevel.Warn, "FtpSocketStream.IsConnected: Caught and discarded SocketException while testing for connectivity: " + exception.ToString());
                    return false;
                }
                catch (IOException exception2)
                {
                    this.Close();
                    this.Client.LogStatus(FtpTraceLevel.Warn, "FtpSocketStream.IsConnected: Caught and discarded IOException while testing for connectivity: " + exception2.ToString());
                    return false;
                }
                return true;
            }
        }

        public bool IsEncrypted
        {
            get
            {
                return (this.m_sslStream > null);
            }
        }

        public override long Length
        {
            get
            {
                return 0L;
            }
        }

        public IPEndPoint LocalEndPoint
        {
            get
            {
                if (this.m_socket == null)
                {
                    return null;
                }
                return (IPEndPoint) this.m_socket.LocalEndPoint;
            }
        }

        private System.Net.Sockets.NetworkStream NetworkStream
        {
            get
            {
                return this.m_netStream;
            }
            set
            {
                this.m_netStream = value;
            }
        }

        public override long Position
        {
            get
            {
                if (this.BaseStream != null)
                {
                    return this.BaseStream.Position;
                }
                return 0L;
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public override int ReadTimeout
        {
            get
            {
                return this.m_readTimeout;
            }
            set
            {
                this.m_readTimeout = value;
                if (this.m_netStream != null)
                {
                    this.m_netStream.ReadTimeout = this.m_readTimeout;
                }
            }
        }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                if (this.m_socket == null)
                {
                    return null;
                }
                return (IPEndPoint) this.m_socket.RemoteEndPoint;
            }
        }

        protected System.Net.Sockets.Socket Socket
        {
            get
            {
                return this.m_socket;
            }
            private set
            {
                this.m_socket = value;
            }
        }

        internal int SocketDataAvailable
        {
            get
            {
                if (this.m_socket != null)
                {
                    return this.m_socket.Available;
                }
                return 0;
            }
        }

        public int SocketPollInterval
        {
            get
            {
                return this.m_socketPollInterval;
            }
            set
            {
                this.m_socketPollInterval = value;
            }
        }

        private System.Net.Security.SslStream SslStream
        {
            get
            {
                return this.m_sslStream;
            }
            set
            {
                this.m_sslStream = value;
            }
        }

        [CompilerGenerated]
        private struct <AcceptAsync>d__92 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpSocketStream <>4__this;
            public AsyncTaskMethodBuilder <>t__builder;
            private TaskAwaiter<Socket> <>u__1;

            private void MoveNext()
            {
                int num = this.<>1__state;
                FtpSocketStream stream = this.<>4__this;
                try
                {
                    TaskAwaiter<Socket> awaiter;
                    if (num != 0)
                    {
                        if (stream.m_socket != null)
                        {
                            IAsyncResult asyncResult = stream.m_socket.BeginAccept(null, null);
                            awaiter = Task.Factory.FromAsync<Socket>(asyncResult, new Func<IAsyncResult, Socket>(stream.m_socket.EndAccept)).GetAwaiter();
                            if (!awaiter.IsCompleted)
                            {
                                this.<>1__state = num = 0;
                                this.<>u__1 = awaiter;
                                this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<Socket>, FtpSocketStream.<AcceptAsync>d__92>(ref awaiter, ref this);
                                return;
                            }
                            goto Label_008E;
                        }
                        goto Label_00B1;
                    }
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter<Socket>();
                    this.<>1__state = num = -1;
                Label_008E:
                    awaiter.GetResult();
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
            Label_00B1:
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
        private struct <ActivateEncryptionAsync>d__82 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpSocketStream <>4__this;
            public AsyncTaskMethodBuilder <>t__builder;
            private TaskAwaiter <>u__1;
            public string targethost;

            private void MoveNext()
            {
                int num = this.<>1__state;
                FtpSocketStream stream = this.<>4__this;
                try
                {
                    TaskAwaiter awaiter;
                    if (num != 0)
                    {
                        awaiter = stream.ActivateEncryptionAsync(this.targethost, null, stream.m_SslProtocols).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.<>1__state = num = 0;
                            this.<>u__1 = awaiter;
                            this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpSocketStream.<ActivateEncryptionAsync>d__82>(ref awaiter, ref this);
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
        private struct <ActivateEncryptionAsync>d__84 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpSocketStream <>4__this;
            public AsyncTaskMethodBuilder <>t__builder;
            private TaskAwaiter <>u__1;
            public X509CertificateCollection clientCerts;
            public string targethost;

            private void MoveNext()
            {
                int num = this.<>1__state;
                FtpSocketStream stream = this.<>4__this;
                try
                {
                    TaskAwaiter awaiter;
                    if (num != 0)
                    {
                        awaiter = stream.ActivateEncryptionAsync(this.targethost, this.clientCerts, stream.m_SslProtocols).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.<>1__state = num = 0;
                            this.<>u__1 = awaiter;
                            this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpSocketStream.<ActivateEncryptionAsync>d__84>(ref awaiter, ref this);
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
        private struct <ActivateEncryptionAsync>d__88 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpSocketStream <>4__this;
            public AsyncTaskMethodBuilder <>t__builder;
            private TaskAwaiter <>u__1;
            private DateTime <auth_start>5__2;
            public X509CertificateCollection clientCerts;
            public SslProtocols sslProtocols;
            public string targethost;

            private void MoveNext()
            {
                int num = this.<>1__state;
                FtpSocketStream stream = this.<>4__this;
                try
                {
                    if (num != 0)
                    {
                        if (!stream.IsConnected)
                        {
                            throw new InvalidOperationException("The FtpSocketStream object is not connected.");
                        }
                        if (stream.m_netStream == null)
                        {
                            throw new InvalidOperationException("The base network stream is null.");
                        }
                        if (stream.m_sslStream != null)
                        {
                            throw new InvalidOperationException("SSL Encryption has already been enabled on this stream.");
                        }
                    }
                    try
                    {
                        if (num != 0)
                        {
                            stream.CreateBufferStream();
                            stream.m_sslStream = new FtpSslStream(stream.GetBufferStream(), true, new RemoteCertificateValidationCallback(stream.<ActivateEncryptionAsync>b__88_0));
                            this.<auth_start>5__2 = DateTime.Now;
                        }
                        try
                        {
                            TaskAwaiter awaiter;
                            if (num != 0)
                            {
                                awaiter = stream.m_sslStream.AuthenticateAsClientAsync(this.targethost, this.clientCerts, this.sslProtocols, true).GetAwaiter();
                                if (!awaiter.IsCompleted)
                                {
                                    this.<>1__state = num = 0;
                                    this.<>u__1 = awaiter;
                                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpSocketStream.<ActivateEncryptionAsync>d__88>(ref awaiter, ref this);
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
                        }
                        catch (IOException exception)
                        {
                            if ((exception.InnerException is Win32Exception) && (((Win32Exception) exception.InnerException).NativeErrorCode == 0x2745))
                            {
                                throw new FtpMissingSocketException(exception);
                            }
                            throw;
                        }
                        TimeSpan span = DateTime.Now.Subtract(this.<auth_start>5__2);
                        stream.Client.LogStatus(FtpTraceLevel.Info, "FTPS Authentication Successful");
                        stream.Client.LogStatus(FtpTraceLevel.Verbose, string.Concat(new object[] { "Time to activate encryption: ", span.Hours, "h ", span.Minutes, "m ", span.Seconds, "s.  Total Seconds: ", span.TotalSeconds, "." }));
                    }
                    catch (AuthenticationException)
                    {
                        stream.Close();
                        stream.Client.LogStatus(FtpTraceLevel.Error, "FTPS Authentication Failed");
                        throw;
                    }
                }
                catch (Exception exception2)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception2);
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
        private struct <ConnectAsync>d__80 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpSocketStream <>4__this;
            public AsyncTaskMethodBuilder <>t__builder;
            private TaskAwaiter<IPAddress[]> <>u__1;
            private TaskAwaiter <>u__2;
            public string host;
            public FtpIpVersion ipVersions;
            public int port;
            public CancellationToken token;

            private void MoveNext()
            {
                int num = this.<>1__state;
                FtpSocketStream stream = this.<>4__this;
                try
                {
                    IPAddress[] addressArray;
                    TaskAwaiter<IPAddress[]> awaiter;
                    if (num != 0)
                    {
                        if (num != 1)
                        {
                            awaiter = Dns.GetHostAddressesAsync(this.host).GetAwaiter();
                            if (!awaiter.IsCompleted)
                            {
                                this.<>1__state = num = 0;
                                this.<>u__1 = awaiter;
                                this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<IPAddress[]>, FtpSocketStream.<ConnectAsync>d__80>(ref awaiter, ref this);
                                return;
                            }
                            goto Label_0071;
                        }
                        goto Label_01D6;
                    }
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter<IPAddress[]>();
                    this.<>1__state = num = -1;
                Label_0071:
                    addressArray = awaiter.GetResult();
                    if (this.ipVersions == 0)
                    {
                        throw new ArgumentException("The ipVersions parameter must contain at least 1 flag.");
                    }
                    for (int i = 0; i < addressArray.Length; i++)
                    {
                        if (this.ipVersions == FtpIpVersion.ANY)
                        {
                            goto Label_00D3;
                        }
                        AddressFamily addressFamily = addressArray[i].AddressFamily;
                        if (addressFamily != AddressFamily.InterNetwork)
                        {
                            if (addressFamily == AddressFamily.InterNetworkV6)
                            {
                                goto Label_00C5;
                            }
                            goto Label_00D3;
                        }
                        if ((this.ipVersions & FtpIpVersion.IPv4) == FtpIpVersion.IPv4)
                        {
                            goto Label_00D3;
                        }
                        continue;
                    Label_00C5:
                        if ((this.ipVersions & FtpIpVersion.IPv6) != FtpIpVersion.IPv6)
                        {
                            continue;
                        }
                    Label_00D3:
                        if (FtpTrace.LogIP)
                        {
                            stream.Client.LogStatus(FtpTraceLevel.Info, string.Concat(new object[] { "Connecting to ", addressArray[i].ToString(), ":", this.port }));
                        }
                        else
                        {
                            stream.Client.LogStatus(FtpTraceLevel.Info, "Connecting to ***:" + this.port);
                        }
                        stream.m_socket = new Socket(addressArray[i].AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                        IAsyncResult asyncResult = stream.m_socket.BeginConnect(addressArray[i], this.port, null, null);
                        TaskAwaiter awaiter2 = stream.EnableCancellation(Task.Factory.FromAsync(asyncResult, new Action<IAsyncResult>(stream.m_socket.EndConnect)), this.token, new Action(stream.<ConnectAsync>b__80_0)).GetAwaiter();
                        if (awaiter2.IsCompleted)
                        {
                            goto Label_01F3;
                        }
                        this.<>1__state = num = 1;
                        this.<>u__2 = awaiter2;
                        this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpSocketStream.<ConnectAsync>d__80>(ref awaiter2, ref this);
                        return;
                    Label_01D6:
                        awaiter2 = this.<>u__2;
                        this.<>u__2 = new TaskAwaiter();
                        this.<>1__state = num = -1;
                    Label_01F3:
                        awaiter2.GetResult();
                        break;
                    }
                    if ((stream.m_socket == null) || !stream.m_socket.Connected)
                    {
                        stream.Close();
                        throw new IOException("Failed to connect to host.");
                    }
                    stream.m_netStream = new NetworkStream(stream.m_socket);
                    stream.m_netStream.ReadTimeout = stream.m_readTimeout;
                    stream.m_lastActivity = DateTime.Now;
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
        private struct <EnableCancellation>d__64 : IAsyncStateMachine
        {
            public int <>1__state;
            public AsyncTaskMethodBuilder <>t__builder;
            private TaskAwaiter <>u__1;
            public Action action;
            public Task task;
            public CancellationToken token;

            private void MoveNext()
            {
                int num = this.<>1__state;
                try
                {
                    TaskAwaiter awaiter;
                    if (num != 0)
                    {
                        FtpSocketStream.<>c__DisplayClass64_0 class_;
                        CancellationTokenRegistration registration = this.token.Register(this.action);
                        this.task.ContinueWith(new Action<Task>(class_.<EnableCancellation>b__0), CancellationToken.None);
                        awaiter = this.task.GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.<>1__state = num = 0;
                            this.<>u__1 = awaiter;
                            this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpSocketStream.<EnableCancellation>d__64>(ref awaiter, ref this);
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
        private struct <EnableCancellation>d__65<T> : IAsyncStateMachine
        {
            public int <>1__state;
            public AsyncTaskMethodBuilder<T> <>t__builder;
            private TaskAwaiter<T> <>u__1;
            public Action action;
            public Task<T> task;
            public CancellationToken token;

            private void MoveNext()
            {
                T result;
                int num = this.<>1__state;
                try
                {
                    TaskAwaiter<T> awaiter;
                    if (num != 0)
                    {
                        FtpSocketStream.<>c__DisplayClass65_0<T> class_;
                        CancellationTokenRegistration registration = this.token.Register(this.action);
                        this.task.ContinueWith(new Action<Task<T>>(class_.<EnableCancellation>b__0), CancellationToken.None);
                        awaiter = this.task.GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.<>1__state = num = 0;
                            this.<>u__1 = awaiter;
                            this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<T>, FtpSocketStream.<EnableCancellation>d__65<T>>(ref awaiter, ref (FtpSocketStream.<EnableCancellation>d__65<T>) ref this);
                            return;
                        }
                    }
                    else
                    {
                        awaiter = this.<>u__1;
                        this.<>u__1 = new TaskAwaiter<T>();
                        this.<>1__state = num = -1;
                    }
                    result = awaiter.GetResult();
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
                this.<>1__state = -2;
                this.<>t__builder.SetResult(result);
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }

        [CompilerGenerated]
        private struct <FlushAsync>d__62 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpSocketStream <>4__this;
            public AsyncTaskMethodBuilder <>t__builder;
            private TaskAwaiter <>u__1;
            public CancellationToken token;

            private void MoveNext()
            {
                int num = this.<>1__state;
                FtpSocketStream stream = this.<>4__this;
                try
                {
                    TaskAwaiter awaiter;
                    if (num != 0)
                    {
                        if (!stream.IsConnected)
                        {
                            throw new InvalidOperationException("The FtpSocketStream object is not connected.");
                        }
                        if (stream.BaseStream == null)
                        {
                            throw new InvalidOperationException("The base stream of the FtpSocketStream object is null.");
                        }
                        awaiter = stream.BaseStream.FlushAsync(this.token).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.<>1__state = num = 0;
                            this.<>u__1 = awaiter;
                            this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpSocketStream.<FlushAsync>d__62>(ref awaiter, ref this);
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
        private struct <RawSocketReadAsync>d__66 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpSocketStream <>4__this;
            public AsyncTaskMethodBuilder<int> <>t__builder;
            private TaskAwaiter<int> <>u__1;
            public byte[] buffer;
            public CancellationToken token;

            private void MoveNext()
            {
                int num2;
                int num = this.<>1__state;
                FtpSocketStream stream = this.<>4__this;
                try
                {
                    int num3;
                    TaskAwaiter<int> awaiter;
                    if (num != 0)
                    {
                        num3 = 0;
                        if ((stream.m_socket != null) && stream.m_socket.Connected)
                        {
                            IAsyncResult asyncResult = stream.m_socket.BeginReceive(this.buffer, 0, this.buffer.Length, SocketFlags.None, null, null);
                            awaiter = stream.EnableCancellation<int>(Task.Factory.FromAsync<int>(asyncResult, new Func<IAsyncResult, int>(stream.m_socket.EndReceive)), this.token, new Action(stream.<RawSocketReadAsync>b__66_0)).GetAwaiter();
                            if (!awaiter.IsCompleted)
                            {
                                this.<>1__state = num = 0;
                                this.<>u__1 = awaiter;
                                this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<int>, FtpSocketStream.<RawSocketReadAsync>d__66>(ref awaiter, ref this);
                                return;
                            }
                            goto Label_00D3;
                        }
                        goto Label_00DB;
                    }
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter<int>();
                    this.<>1__state = num = -1;
                Label_00D3:
                    num3 = awaiter.GetResult();
                Label_00DB:
                    num2 = num3;
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
        private sealed class <ReadAllLines>d__70 : IEnumerable<string>, IEnumerable, IEnumerator<string>, IDisposable, IEnumerator
        {
            private int <>1__state;
            private string <>2__current;
            public int <>3__bufferSize;
            public Encoding <>3__encoding;
            public FtpSocketStream <>4__this;
            private int <>l__initialThreadId;
            private byte[] <buf>5__4;
            private int <charRead>5__2;
            private List<byte> <data>5__3;
            private int <firstByteToReadIdx>5__5;
            private int bufferSize;
            private Encoding encoding;

            [DebuggerHidden]
            public <ReadAllLines>d__70(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Environment.CurrentManagedThreadId;
            }

            private bool MoveNext()
            {
                int num = this.<>1__state;
                FtpSocketStream stream = this.<>4__this;
                if (num == 0)
                {
                    this.<>1__state = -1;
                    this.<data>5__3 = new List<byte>();
                    this.<buf>5__4 = new byte[this.bufferSize];
                    while ((this.<charRead>5__2 = stream.Read(this.<buf>5__4, 0, this.<buf>5__4.Length)) > 0)
                    {
                        int num3;
                        this.<firstByteToReadIdx>5__5 = 0;
                        int num2 = Array.IndexOf<byte>(this.<buf>5__4, 10, this.<firstByteToReadIdx>5__5, this.<charRead>5__2 - this.<firstByteToReadIdx>5__5);
                        goto Label_010D;
                    Label_006F:
                        num3 = this.<firstByteToReadIdx>5__5;
                        this.<firstByteToReadIdx>5__5 = num3 + 1;
                        this.<data>5__3.Add(this.<buf>5__4[num3]);
                    Label_0095:
                        if (this.<firstByteToReadIdx>5__5 <= num2)
                        {
                            goto Label_006F;
                        }
                        char[] trimChars = new char[] { '\r', '\n' };
                        string str = this.encoding.GetString(this.<data>5__3.ToArray()).Trim(trimChars);
                        this.<>2__current = str;
                        this.<>1__state = 1;
                        return true;
                    Label_00DA:
                        this.<>1__state = -1;
                        this.<data>5__3.Clear();
                        num2 = Array.IndexOf<byte>(this.<buf>5__4, 10, this.<firstByteToReadIdx>5__5, this.<charRead>5__2 - this.<firstByteToReadIdx>5__5);
                    Label_010D:
                        if (num2 >= 0)
                        {
                            goto Label_0095;
                        }
                        while (this.<firstByteToReadIdx>5__5 < this.<charRead>5__2)
                        {
                            num3 = this.<firstByteToReadIdx>5__5;
                            this.<firstByteToReadIdx>5__5 = num3 + 1;
                            this.<data>5__3.Add(this.<buf>5__4[num3]);
                        }
                    }
                    return false;
                }
                if (num != 1)
                {
                    return false;
                }
                goto Label_00DA;
            }

            [DebuggerHidden]
            IEnumerator<string> IEnumerable<string>.GetEnumerator()
            {
                FtpSocketStream.<ReadAllLines>d__70 d__;
                if ((this.<>1__state == -2) && (this.<>l__initialThreadId == Environment.CurrentManagedThreadId))
                {
                    this.<>1__state = 0;
                    d__ = this;
                }
                else
                {
                    d__ = new FtpSocketStream.<ReadAllLines>d__70(0) {
                        <>4__this = this.<>4__this
                    };
                }
                d__.encoding = this.<>3__encoding;
                d__.bufferSize = this.<>3__bufferSize;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.System.Collections.Generic.IEnumerable<System.String>.GetEnumerator();
            }

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            [DebuggerHidden]
            void IDisposable.Dispose()
            {
            }

            string IEnumerator<string>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.<>2__current;
                }
            }
        }

        [CompilerGenerated]
        private struct <ReadAllLinesAsync>d__72 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpSocketStream <>4__this;
            public AsyncTaskMethodBuilder<IEnumerable<string>> <>t__builder;
            private TaskAwaiter<int> <>u__1;
            private byte[] <buf>5__4;
            private List<byte> <data>5__2;
            private List<string> <lines>5__3;
            public int bufferSize;
            public Encoding encoding;
            public CancellationToken token;

            private void MoveNext()
            {
                IEnumerable<string> enumerable;
                int num = this.<>1__state;
                FtpSocketStream stream = this.<>4__this;
                try
                {
                    int num2;
                    int num3;
                    TaskAwaiter<int> awaiter;
                    if (num == 0)
                    {
                        goto Label_0144;
                    }
                    this.<data>5__2 = new List<byte>();
                    this.<lines>5__3 = new List<string>();
                    this.<buf>5__4 = new byte[this.bufferSize];
                    goto Label_00F8;
                Label_0040:
                    num3 = 0;
                    int num4 = Array.IndexOf<byte>(this.<buf>5__4, 10, num3, num2 - num3);
                    goto Label_00D3;
                Label_005A:
                    this.<data>5__2.Add(this.<buf>5__4[num3++]);
                Label_0073:
                    if (num3 <= num4)
                    {
                        goto Label_005A;
                    }
                    char[] trimChars = new char[] { '\r', '\n' };
                    string item = this.encoding.GetString(this.<data>5__2.ToArray()).Trim(trimChars);
                    this.<lines>5__3.Add(item);
                    this.<data>5__2.Clear();
                    num4 = Array.IndexOf<byte>(this.<buf>5__4, 10, num3, num2 - num3);
                Label_00D3:
                    if (num4 >= 0)
                    {
                        goto Label_0073;
                    }
                    while (num3 < num2)
                    {
                        this.<data>5__2.Add(this.<buf>5__4[num3++]);
                    }
                Label_00F8:
                    awaiter = stream.ReadAsync(this.<buf>5__4, 0, this.<buf>5__4.Length, this.token).GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        goto Label_0161;
                    }
                    this.<>1__state = num = 0;
                    this.<>u__1 = awaiter;
                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<int>, FtpSocketStream.<ReadAllLinesAsync>d__72>(ref awaiter, ref this);
                    return;
                Label_0144:
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter<int>();
                    this.<>1__state = num = -1;
                Label_0161:
                    if ((num2 = awaiter.GetResult()) > 0)
                    {
                        goto Label_0040;
                    }
                    enumerable = this.<lines>5__3;
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
                this.<>1__state = -2;
                this.<>t__builder.SetResult(enumerable);
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }

        [CompilerGenerated]
        private struct <ReadAsync>d__68 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpSocketStream <>4__this;
            public AsyncTaskMethodBuilder<int> <>t__builder;
            private TaskAwaiter<int> <>u__1;
            private CancellationTokenSource <cts>5__2;
            public byte[] buffer;
            public int count;
            public int offset;
            public CancellationToken token;

            private void MoveNext()
            {
                int result;
                int num = this.<>1__state;
                FtpSocketStream stream = this.<>4__this;
                try
                {
                    if (num != 0)
                    {
                        if (stream.BaseStream == null)
                        {
                            result = 0;
                            goto Label_0169;
                        }
                        stream.m_lastActivity = DateTime.Now;
                        CancellationToken[] tokens = new CancellationToken[] { this.token };
                        this.<cts>5__2 = CancellationTokenSource.CreateLinkedTokenSource(tokens);
                    }
                    try
                    {
                        if (num != 0)
                        {
                            this.<cts>5__2.CancelAfter(stream.ReadTimeout);
                            this.<cts>5__2.Token.Register(new Action(stream.<ReadAsync>b__68_0));
                        }
                        try
                        {
                            TaskAwaiter<int> awaiter;
                            if (num != 0)
                            {
                                awaiter = stream.BaseStream.ReadAsync(this.buffer, this.offset, this.count, this.<cts>5__2.Token).GetAwaiter();
                                if (!awaiter.IsCompleted)
                                {
                                    this.<>1__state = num = 0;
                                    this.<>u__1 = awaiter;
                                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<int>, FtpSocketStream.<ReadAsync>d__68>(ref awaiter, ref this);
                                    return;
                                }
                            }
                            else
                            {
                                awaiter = this.<>u__1;
                                this.<>u__1 = new TaskAwaiter<int>();
                                this.<>1__state = num = -1;
                            }
                            result = awaiter.GetResult();
                        }
                        catch
                        {
                            if (this.token.IsCancellationRequested)
                            {
                                throw new OperationCanceledException("Cancelled read from socket stream");
                            }
                            if (this.<cts>5__2.IsCancellationRequested)
                            {
                                throw new TimeoutException("Timed out trying to read data from the socket stream!");
                            }
                            throw;
                        }
                    }
                    finally
                    {
                        if ((num < 0) && (this.<cts>5__2 != null))
                        {
                            this.<cts>5__2.Dispose();
                        }
                    }
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
            Label_0169:
                this.<>1__state = -2;
                this.<>t__builder.SetResult(result);
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }

        [CompilerGenerated]
        private struct <ReadLineAsync>d__71 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpSocketStream <>4__this;
            public AsyncTaskMethodBuilder<string> <>t__builder;
            private TaskAwaiter<int> <>u__1;
            private byte[] <buf>5__3;
            private List<byte> <data>5__2;
            private string <line>5__4;
            public Encoding encoding;
            public CancellationToken token;

            private void MoveNext()
            {
                string str;
                int num = this.<>1__state;
                FtpSocketStream stream = this.<>4__this;
                try
                {
                    TaskAwaiter<int> awaiter;
                    if (num == 0)
                    {
                        goto Label_00D0;
                    }
                    this.<data>5__2 = new List<byte>();
                    this.<buf>5__3 = new byte[1];
                    this.<line>5__4 = null;
                    goto Label_0086;
                Label_0034:
                    this.<data>5__2.Add(this.<buf>5__3[0]);
                    if (this.<buf>5__3[0] == 10)
                    {
                        char[] trimChars = new char[] { '\r', '\n' };
                        this.<line>5__4 = this.encoding.GetString(this.<data>5__2.ToArray()).Trim(trimChars);
                        goto Label_00F9;
                    }
                Label_0086:
                    awaiter = stream.ReadAsync(this.<buf>5__3, 0, this.<buf>5__3.Length, this.token).GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        goto Label_00EC;
                    }
                    this.<>1__state = num = 0;
                    this.<>u__1 = awaiter;
                    this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter<int>, FtpSocketStream.<ReadLineAsync>d__71>(ref awaiter, ref this);
                    return;
                Label_00D0:
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter<int>();
                    this.<>1__state = num = -1;
                Label_00EC:
                    if (awaiter.GetResult() > 0)
                    {
                        goto Label_0034;
                    }
                Label_00F9:
                    str = this.<line>5__4;
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
                this.<>1__state = -2;
                this.<>t__builder.SetResult(str);
            }

            [DebuggerHidden]
            private void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                this.<>t__builder.SetStateMachine(stateMachine);
            }
        }

        [CompilerGenerated]
        private struct <WriteAsync>d__74 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpSocketStream <>4__this;
            public AsyncTaskMethodBuilder <>t__builder;
            private TaskAwaiter <>u__1;
            public byte[] buffer;
            public int count;
            public int offset;
            public CancellationToken token;

            private void MoveNext()
            {
                int num = this.<>1__state;
                FtpSocketStream stream = this.<>4__this;
                try
                {
                    TaskAwaiter awaiter;
                    if (num != 0)
                    {
                        if (stream.BaseStream != null)
                        {
                            awaiter = stream.BaseStream.WriteAsync(this.buffer, this.offset, this.count, this.token).GetAwaiter();
                            if (!awaiter.IsCompleted)
                            {
                                this.<>1__state = num = 0;
                                this.<>u__1 = awaiter;
                                this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpSocketStream.<WriteAsync>d__74>(ref awaiter, ref this);
                                return;
                            }
                            goto Label_008C;
                        }
                        goto Label_00B7;
                    }
                    awaiter = this.<>u__1;
                    this.<>u__1 = new TaskAwaiter();
                    this.<>1__state = num = -1;
                Label_008C:
                    awaiter.GetResult();
                    stream.m_lastActivity = DateTime.Now;
                }
                catch (Exception exception)
                {
                    this.<>1__state = -2;
                    this.<>t__builder.SetException(exception);
                    return;
                }
            Label_00B7:
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
        private struct <WriteLineAsync>d__76 : IAsyncStateMachine
        {
            public int <>1__state;
            public FtpSocketStream <>4__this;
            public AsyncTaskMethodBuilder <>t__builder;
            private TaskAwaiter <>u__1;
            public string buf;
            public Encoding encoding;
            public CancellationToken token;

            private void MoveNext()
            {
                int num = this.<>1__state;
                FtpSocketStream stream = this.<>4__this;
                try
                {
                    TaskAwaiter awaiter;
                    if (num != 0)
                    {
                        byte[] bytes = this.encoding.GetBytes(this.buf + "\r\n");
                        awaiter = stream.WriteAsync(bytes, 0, bytes.Length, this.token).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            this.<>1__state = num = 0;
                            this.<>u__1 = awaiter;
                            this.<>t__builder.AwaitUnsafeOnCompleted<TaskAwaiter, FtpSocketStream.<WriteLineAsync>d__76>(ref awaiter, ref this);
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

