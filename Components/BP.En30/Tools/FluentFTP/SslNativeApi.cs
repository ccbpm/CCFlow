namespace FluentFTP
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;

    internal static class SslNativeApi
    {
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("secur32.dll", SetLastError=true, ExactSpelling=true)]
        internal static extern int AcceptSecurityContext(ref SSPIHandle credentialHandle, ref SSPIHandle contextHandle, [In] SecurityBufferDescriptor inputBuffer, [In] ContextFlags inFlags, [In] Endianness endianness, ref SSPIHandle outContextPtr, [In, Out] SecurityBufferDescriptor outputBuffer, [In, Out] ref ContextFlags attributes, out long timeStamp);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("secur32.dll", SetLastError=true, ExactSpelling=true)]
        internal static extern int ApplyControlToken(ref SSPIHandle contextHandle, [In, Out] SecurityBufferDescriptor outputBuffer);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("secur32.dll", SetLastError=true, ExactSpelling=true)]
        internal static extern unsafe int InitializeSecurityContextW(ref SSPIHandle credentialHandle, ref SSPIHandle contextHandle, [In] byte* targetName, [In] ContextFlags inFlags, [In] int reservedI, [In] Endianness endianness, [In] SecurityBufferDescriptor inputBuffer, [In] int reservedII, ref SSPIHandle outContextPtr, [In, Out] SecurityBufferDescriptor outputBuffer, [In, Out] ref ContextFlags attributes, out long timeStamp);

        internal enum BufferType
        {
            ChannelBindings = 14,
            Data = 1,
            Empty = 0,
            Extra = 5,
            Header = 7,
            Missing = 4,
            Padding = 9,
            Parameters = 3,
            ReadOnlyFlag = -2147483648,
            ReadOnlyWithChecksum = 0x10000000,
            Stream = 10,
            TargetHost = 0x10,
            Token = 2,
            Trailer = 6
        }

        [Flags]
        internal enum ContextFlags
        {
            AcceptExtendedError = 0x8000,
            AcceptIdentify = 0x80000,
            AcceptIntegrity = 0x20000,
            AcceptStream = 0x10000,
            AllocateMemory = 0x100,
            AllowMissingBindings = 0x10000000,
            Confidentiality = 0x10,
            Connection = 0x800,
            Delegate = 1,
            InitExtendedError = 0x4000,
            InitIdentify = 0x20000,
            InitIntegrity = 0x10000,
            InitManualCredValidation = 0x80000,
            InitStream = 0x8000,
            InitUseSuppliedCreds = 0x80,
            MutualAuth = 2,
            ProxyBindings = 0x4000000,
            ReplayDetect = 4,
            SequenceDetect = 8,
            UnverifiedTargetName = 0x20000000,
            UseSessionKey = 0x20,
            Zero = 0
        }

        internal enum Endianness
        {
            Native = 0x10,
            Network = 0
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class SecurityBufferDescriptor
        {
            public readonly int Version = 0;
            public readonly int Count;
            public unsafe void* UnmanagedPointer;
            public unsafe SecurityBufferDescriptor(int count)
            {
                this.Count = count;
                this.UnmanagedPointer = null;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SecurityBufferStruct
        {
            public int count;
            public SslNativeApi.BufferType type;
            public IntPtr token;
            public static readonly int Size;
            static SecurityBufferStruct()
            {
                Size = sizeof(SslNativeApi.SecurityBufferStruct);
            }
        }

        internal enum SecurityStatus
        {
            AlgorithmMismatch = -2146893007,
            BadBinding = -2146892986,
            BufferNotEnough = -2146893023,
            CannotInstall = -2146893049,
            CannotPack = -2146893047,
            CertExpired = -2146893016,
            CertUnknown = -2146893017,
            CompAndContinue = 0x90314,
            CompleteNeeded = 0x90313,
            ContextExpired = 0x90317,
            ContinueNeeded = 0x90312,
            CredentialsNeeded = 0x90320,
            IllegalMessage = -2146893018,
            IncompleteCredentials = -2146893024,
            IncompleteMessage = -2146893032,
            InternalError = -2146893052,
            InvalidHandle = -2146893055,
            InvalidToken = -2146893048,
            LogonDenied = -2146893044,
            MessageAltered = -2146893041,
            NoAuthenticatingAuthority = -2146893039,
            NoCredentials = -2146893042,
            NoImpersonation = -2146893045,
            NotOwner = -2146893050,
            OK = 0,
            OutOfMemory = -2146893056,
            OutOfSequence = -2146893040,
            PackageNotFound = -2146893051,
            QopNotSupported = -2146893046,
            Renegotiate = 0x90321,
            SecurityQosFailed = -2146893006,
            SmartcardLogonRequired = -2146892994,
            TargetUnknown = -2146893053,
            TimeSkew = -2146893020,
            UnknownCredentials = -2146893043,
            Unsupported = -2146893054,
            UnsupportedPreauth = -2146892989,
            UntrustedRoot = -2146893019,
            WrongPrincipal = -2146893022
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        internal struct SSPIHandle
        {
            public IntPtr HandleHi;
            public IntPtr HandleLo;
            public bool IsZero
            {
                get
                {
                    return ((this.HandleHi == IntPtr.Zero) && (this.HandleLo == IntPtr.Zero));
                }
            }
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            internal void SetToInvalid()
            {
                this.HandleHi = IntPtr.Zero;
                this.HandleLo = IntPtr.Zero;
            }

            public override string ToString()
            {
                return (this.HandleHi.ToString("x") + ":" + this.HandleLo.ToString("x"));
            }
        }
    }
}

