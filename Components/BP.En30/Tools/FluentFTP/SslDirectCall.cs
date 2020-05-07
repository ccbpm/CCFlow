namespace FluentFTP
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Security;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal static class SslDirectCall
    {
        public static unsafe void CloseNotify(SslStream sslStream)
        {
            if (sslStream.IsAuthenticated && sslStream.CanWrite)
            {
                SslNativeApi.SecurityBufferStruct* structPtr;
                SslNativeApi.SecurityBufferStruct[] pinned structArray2;
                void* voidPtr;
                byte[] pinned buffer3;
                bool isServer = sslStream.IsServer;
                Assembly assembly = typeof(Authorization).Assembly;
                byte[] bytes = BitConverter.GetBytes(1);
                object field = sslStream.GetField("_SslState");
                object property = field.GetProperty("Context");
                object obj2 = property.GetField("m_SecurityContext").GetField("_handle");
                SslNativeApi.SSPIHandle contextHandle = new SslNativeApi.SSPIHandle {
                    HandleHi = (IntPtr) obj2.GetField("HandleHi"),
                    HandleLo = (IntPtr) obj2.GetField("HandleLo")
                };
                object obj3 = property.GetField("m_CredentialsHandle").GetField("_handle");
                SslNativeApi.SSPIHandle credentialHandle = new SslNativeApi.SSPIHandle {
                    HandleHi = (IntPtr) obj3.GetField("HandleHi"),
                    HandleLo = (IntPtr) obj3.GetField("HandleLo")
                };
                SslNativeApi.SecurityBufferDescriptor outputBuffer = new SslNativeApi.SecurityBufferDescriptor(1);
                SslNativeApi.SecurityBufferStruct[] structArray = new SslNativeApi.SecurityBufferStruct[1];
                if (((structArray2 = structArray) == null) || (structArray2.Length == 0))
                {
                    structPtr = null;
                }
                else
                {
                    structPtr = structArray2;
                }
                if (((buffer3 = bytes) == null) || (buffer3.Length == 0))
                {
                    voidPtr = null;
                }
                else
                {
                    voidPtr = (void*) &(buffer3[0]);
                }
                outputBuffer.UnmanagedPointer = (void*) structPtr;
                structArray[0].token = (IntPtr) voidPtr;
                structArray[0].count = bytes.Length;
                structArray[0].type = SslNativeApi.BufferType.Token;
                SslNativeApi.SecurityStatus status = (SslNativeApi.SecurityStatus) SslNativeApi.ApplyControlToken(ref contextHandle, outputBuffer);
                if (status != SslNativeApi.SecurityStatus.OK)
                {
                    throw new InvalidOperationException(string.Format("ApplyControlToken returned [{0}] during CloseNotify.", status));
                }
                structArray[0].token = IntPtr.Zero;
                structArray[0].count = 0;
                structArray[0].type = SslNativeApi.BufferType.Token;
                SslNativeApi.SSPIHandle outContextPtr = new SslNativeApi.SSPIHandle();
                SslNativeApi.ContextFlags zero = SslNativeApi.ContextFlags.Zero;
                long timeStamp = 0L;
                SslNativeApi.ContextFlags inFlags = SslNativeApi.ContextFlags.AcceptExtendedError | SslNativeApi.ContextFlags.AllocateMemory | SslNativeApi.ContextFlags.Confidentiality | SslNativeApi.ContextFlags.SequenceDetect | SslNativeApi.ContextFlags.ReplayDetect;
                if (isServer)
                {
                    status = (SslNativeApi.SecurityStatus) SslNativeApi.AcceptSecurityContext(ref credentialHandle, ref contextHandle, null, inFlags, SslNativeApi.Endianness.Native, ref outContextPtr, outputBuffer, ref zero, out timeStamp);
                }
                else
                {
                    status = (SslNativeApi.SecurityStatus) SslNativeApi.InitializeSecurityContextW(ref credentialHandle, ref contextHandle, null, inFlags, 0, SslNativeApi.Endianness.Native, null, 0, ref outContextPtr, outputBuffer, ref zero, out timeStamp);
                }
                if (status != SslNativeApi.SecurityStatus.OK)
                {
                    throw new InvalidOperationException(string.Format("AcceptSecurityContext/InitializeSecurityContextW returned [{0}] during CloseNotify.", status));
                }
                byte[] destination = new byte[structArray[0].count];
                Marshal.Copy(structArray[0].token, destination, 0, destination.Length);
                Marshal.FreeCoTaskMem(structArray[0].token);
                byte[] buffer = destination;
                int length = destination.Length;
                buffer3 = null;
                structArray2 = null;
                ((Stream) field.GetProperty("InnerStream")).Write(buffer, 0, length);
            }
        }
    }
}

