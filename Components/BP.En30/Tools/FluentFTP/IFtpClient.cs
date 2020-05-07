namespace FluentFTP
{
    using FluentFTP.Servers;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IFtpClient : IDisposable
    {
        event FtpSslValidation ValidateCertificate;

        FtpProfile AutoConnect();
        Task<FtpProfile> AutoConnectAsync(CancellationToken token = new CancellationToken());
        List<FtpProfile> AutoDetect(bool firstOnly);
        void Chmod(string path, int permissions);
        void Chmod(string path, FtpPermission owner, FtpPermission group, FtpPermission other);
        Task ChmodAsync(string path, int permissions, CancellationToken token = new CancellationToken());
        Task ChmodAsync(string path, FtpPermission owner, FtpPermission group, FtpPermission other, CancellationToken token = new CancellationToken());
        void Connect();
        void Connect(FtpProfile profile);
        Task ConnectAsync(CancellationToken token = new CancellationToken());
        Task ConnectAsync(FtpProfile profile, CancellationToken token = new CancellationToken());
        bool CreateDirectory(string path);
        bool CreateDirectory(string path, bool force);
        Task<bool> CreateDirectoryAsync(string path, CancellationToken token = new CancellationToken());
        Task<bool> CreateDirectoryAsync(string path, bool force, CancellationToken token = new CancellationToken());
        void DeleteDirectory(string path);
        void DeleteDirectory(string path, FtpListOption options);
        Task DeleteDirectoryAsync(string path, CancellationToken token = new CancellationToken());
        Task DeleteDirectoryAsync(string path, FtpListOption options, CancellationToken token = new CancellationToken());
        void DeleteFile(string path);
        Task DeleteFileAsync(string path, CancellationToken token = new CancellationToken());
        FtpListItem DereferenceLink(FtpListItem item);
        FtpListItem DereferenceLink(FtpListItem item, int recMax);
        Task<FtpListItem> DereferenceLinkAsync(FtpListItem item, CancellationToken token = new CancellationToken());
        Task<FtpListItem> DereferenceLinkAsync(FtpListItem item, int recMax, CancellationToken token = new CancellationToken());
        bool DirectoryExists(string path);
        Task<bool> DirectoryExistsAsync(string path, CancellationToken token = new CancellationToken());
        void DisableUTF8();
        void Disconnect();
        Task DisconnectAsync(CancellationToken token = new CancellationToken());
        bool Download(Stream outStream, string remotePath, long restartPosition, Action<FtpProgress> progress = null);
        bool Download(out byte[] outBytes, string remotePath, long restartPosition, Action<FtpProgress> progress = null);
        Task<byte[]> DownloadAsync(string remotePath, long restartPosition = 0L, IProgress<FtpProgress> progress = null, CancellationToken token = new CancellationToken());
        Task<bool> DownloadAsync(Stream outStream, string remotePath, long restartPosition = 0L, IProgress<FtpProgress> progress = null, CancellationToken token = new CancellationToken());
        List<FtpResult> DownloadDirectory(string localFolder, string remoteFolder, FtpFolderSyncMode mode = 1, FtpLocalExists existsMode = 2, FtpVerify verifyOptions = 0, List<FtpRule> rules = null, Action<FtpProgress> progress = null);
        Task<List<FtpResult>> DownloadDirectoryAsync(string localFolder, string remoteFolder, FtpFolderSyncMode mode = 1, FtpLocalExists existsMode = 2, FtpVerify verifyOptions = 0, List<FtpRule> rules = null, IProgress<FtpProgress> progress = null, CancellationToken token = new CancellationToken());
        FtpStatus DownloadFile(string localPath, string remotePath, FtpLocalExists existsMode = 0, FtpVerify verifyOptions = 0, Action<FtpProgress> progress = null);
        Task<FtpStatus> DownloadFileAsync(string localPath, string remotePath, FtpLocalExists existsMode = 0, FtpVerify verifyOptions = 0, IProgress<FtpProgress> progress = null, CancellationToken token = new CancellationToken());
        int DownloadFiles(string localDir, IEnumerable<string> remotePaths, FtpLocalExists existsMode = 0, FtpVerify verifyOptions = 0, FtpError errorHandling = 0, Action<FtpProgress> progress = null);
        Task<int> DownloadFilesAsync(string localDir, IEnumerable<string> remotePaths, FtpLocalExists existsMode = 0, FtpVerify verifyOptions = 0, FtpError errorHandling = 0, CancellationToken token = new CancellationToken(), IProgress<FtpProgress> progress = null);
        FtpReply Execute(string command);
        Task<FtpReply> ExecuteAsync(string command, CancellationToken token = new CancellationToken());
        bool FileExists(string path);
        Task<bool> FileExistsAsync(string path, CancellationToken token = new CancellationToken());
        FtpHash GetChecksum(string path, FtpHashAlgorithm algorithm = 0);
        Task<FtpHash> GetChecksumAsync(string path, CancellationToken token = new CancellationToken(), FtpHashAlgorithm algorithm = 0);
        int GetChmod(string path);
        Task<int> GetChmodAsync(string path, CancellationToken token = new CancellationToken());
        FtpListItem GetFilePermissions(string path);
        Task<FtpListItem> GetFilePermissionsAsync(string path, CancellationToken token = new CancellationToken());
        long GetFileSize(string path);
        Task<long> GetFileSizeAsync(string path, CancellationToken token = new CancellationToken());
        FtpHash GetHash(string path);
        FtpHashAlgorithm GetHashAlgorithm();
        Task<FtpHashAlgorithm> GetHashAlgorithmAsync(CancellationToken token = new CancellationToken());
        Task<FtpHash> GetHashAsync(string path, CancellationToken token = new CancellationToken());
        FtpListItem[] GetListing();
        FtpListItem[] GetListing(string path);
        FtpListItem[] GetListing(string path, FtpListOption options);
        Task<FtpListItem[]> GetListingAsync(CancellationToken token = new CancellationToken());
        Task<FtpListItem[]> GetListingAsync(string path, CancellationToken token = new CancellationToken());
        Task<FtpListItem[]> GetListingAsync(string path, FtpListOption options, CancellationToken token = new CancellationToken());
        string GetMD5(string path);
        Task<string> GetMD5Async(string path, CancellationToken token = new CancellationToken());
        DateTime GetModifiedTime(string path, FtpDate type = 0);
        Task<DateTime> GetModifiedTimeAsync(string path, FtpDate type = 0, CancellationToken token = new CancellationToken());
        string[] GetNameListing();
        string[] GetNameListing(string path);
        Task<string[]> GetNameListingAsync(CancellationToken token = new CancellationToken());
        Task<string[]> GetNameListingAsync(string path, CancellationToken token = new CancellationToken());
        FtpListItem GetObjectInfo(string path, bool dateModified = false);
        Task<FtpListItem> GetObjectInfoAsync(string path, bool dateModified = false, CancellationToken token = new CancellationToken());
        FtpReply GetReply();
        Task<FtpReply> GetReplyAsync(CancellationToken token = new CancellationToken());
        string GetWorkingDirectory();
        Task<string> GetWorkingDirectoryAsync(CancellationToken token = new CancellationToken());
        string GetXCRC(string path);
        Task<string> GetXCRCAsync(string path, CancellationToken token = new CancellationToken());
        string GetXMD5(string path);
        Task<string> GetXMD5Async(string path, CancellationToken token = new CancellationToken());
        string GetXSHA1(string path);
        Task<string> GetXSHA1Async(string path, CancellationToken token = new CancellationToken());
        string GetXSHA256(string path);
        Task<string> GetXSHA256Async(string path, CancellationToken token = new CancellationToken());
        string GetXSHA512(string path);
        Task<string> GetXSHA512Async(string path, CancellationToken token = new CancellationToken());
        bool HasFeature(FtpCapability cap);
        bool MoveDirectory(string path, string dest, FtpRemoteExists existsMode = 2);
        Task<bool> MoveDirectoryAsync(string path, string dest, FtpRemoteExists existsMode = 2, CancellationToken token = new CancellationToken());
        bool MoveFile(string path, string dest, FtpRemoteExists existsMode = 2);
        Task<bool> MoveFileAsync(string path, string dest, FtpRemoteExists existsMode = 2, CancellationToken token = new CancellationToken());
        Stream OpenAppend(string path);
        Stream OpenAppend(string path, FtpDataType type);
        Stream OpenAppend(string path, FtpDataType type, bool checkIfFileExists);
        Task<Stream> OpenAppendAsync(string path, CancellationToken token = new CancellationToken());
        Task<Stream> OpenAppendAsync(string path, FtpDataType type, CancellationToken token = new CancellationToken());
        Task<Stream> OpenAppendAsync(string path, FtpDataType type, bool checkIfFileExists, CancellationToken token = new CancellationToken());
        Stream OpenRead(string path);
        Stream OpenRead(string path, FtpDataType type);
        Stream OpenRead(string path, long restart);
        Stream OpenRead(string path, FtpDataType type, bool checkIfFileExists);
        Stream OpenRead(string path, FtpDataType type, long restart);
        Stream OpenRead(string path, long restart, bool checkIfFileExists);
        Stream OpenRead(string path, FtpDataType type, long restart, bool checkIfFileExists);
        Task<Stream> OpenReadAsync(string path, CancellationToken token = new CancellationToken());
        Task<Stream> OpenReadAsync(string path, FtpDataType type, CancellationToken token = new CancellationToken());
        Task<Stream> OpenReadAsync(string path, long restart, CancellationToken token = new CancellationToken());
        Task<Stream> OpenReadAsync(string path, FtpDataType type, long restart, CancellationToken token = new CancellationToken());
        Task<Stream> OpenReadAsync(string path, FtpDataType type, long restart, bool checkIfFileExists, CancellationToken token = new CancellationToken());
        Stream OpenWrite(string path);
        Stream OpenWrite(string path, FtpDataType type);
        Stream OpenWrite(string path, FtpDataType type, bool checkIfFileExists);
        Task<Stream> OpenWriteAsync(string path, CancellationToken token = new CancellationToken());
        Task<Stream> OpenWriteAsync(string path, FtpDataType type, CancellationToken token = new CancellationToken());
        Task<Stream> OpenWriteAsync(string path, FtpDataType type, bool checkIfFileExists, CancellationToken token = new CancellationToken());
        void Rename(string path, string dest);
        Task RenameAsync(string path, string dest, CancellationToken token = new CancellationToken());
        void SetFilePermissions(string path, int permissions);
        void SetFilePermissions(string path, FtpPermission owner, FtpPermission group, FtpPermission other);
        Task SetFilePermissionsAsync(string path, int permissions, CancellationToken token = new CancellationToken());
        Task SetFilePermissionsAsync(string path, FtpPermission owner, FtpPermission group, FtpPermission other, CancellationToken token = new CancellationToken());
        void SetHashAlgorithm(FtpHashAlgorithm type);
        Task SetHashAlgorithmAsync(FtpHashAlgorithm type, CancellationToken token = new CancellationToken());
        void SetModifiedTime(string path, DateTime date, FtpDate type = 0);
        Task SetModifiedTimeAsync(string path, DateTime date, FtpDate type = 0, CancellationToken token = new CancellationToken());
        void SetWorkingDirectory(string path);
        Task SetWorkingDirectoryAsync(string path, CancellationToken token = new CancellationToken());
        FtpStatus Upload(Stream fileStream, string remotePath, FtpRemoteExists existsMode = 2, bool createRemoteDir = false, Action<FtpProgress> progress = null);
        FtpStatus Upload(byte[] fileData, string remotePath, FtpRemoteExists existsMode = 2, bool createRemoteDir = false, Action<FtpProgress> progress = null);
        Task<FtpStatus> UploadAsync(Stream fileStream, string remotePath, FtpRemoteExists existsMode = 2, bool createRemoteDir = false, IProgress<FtpProgress> progress = null, CancellationToken token = new CancellationToken());
        Task<FtpStatus> UploadAsync(byte[] fileData, string remotePath, FtpRemoteExists existsMode = 2, bool createRemoteDir = false, IProgress<FtpProgress> progress = null, CancellationToken token = new CancellationToken());
        List<FtpResult> UploadDirectory(string localFolder, string remoteFolder, FtpFolderSyncMode mode = 1, FtpRemoteExists existsMode = 1, FtpVerify verifyOptions = 0, List<FtpRule> rules = null, Action<FtpProgress> progress = null);
        Task<List<FtpResult>> UploadDirectoryAsync(string localFolder, string remoteFolder, FtpFolderSyncMode mode = 1, FtpRemoteExists existsMode = 1, FtpVerify verifyOptions = 0, List<FtpRule> rules = null, IProgress<FtpProgress> progress = null, CancellationToken token = new CancellationToken());
        FtpStatus UploadFile(string localPath, string remotePath, FtpRemoteExists existsMode = 2, bool createRemoteDir = false, FtpVerify verifyOptions = 0, Action<FtpProgress> progress = null);
        Task<FtpStatus> UploadFileAsync(string localPath, string remotePath, FtpRemoteExists existsMode = 2, bool createRemoteDir = false, FtpVerify verifyOptions = 0, IProgress<FtpProgress> progress = null, CancellationToken token = new CancellationToken());
        int UploadFiles(IEnumerable<FileInfo> localFiles, string remoteDir, FtpRemoteExists existsMode = 2, bool createRemoteDir = true, FtpVerify verifyOptions = 0, FtpError errorHandling = 0, Action<FtpProgress> progress = null);
        int UploadFiles(IEnumerable<string> localPaths, string remoteDir, FtpRemoteExists existsMode = 2, bool createRemoteDir = true, FtpVerify verifyOptions = 0, FtpError errorHandling = 0, Action<FtpProgress> progress = null);
        Task<int> UploadFilesAsync(IEnumerable<string> localPaths, string remoteDir, FtpRemoteExists existsMode = 2, bool createRemoteDir = true, FtpVerify verifyOptions = 0, FtpError errorHandling = 0, CancellationToken token = new CancellationToken(), IProgress<FtpProgress> progress = null);

        IEnumerable<int> ActivePorts { get; set; }

        Func<string> AddressResolver { get; set; }

        bool BulkListing { get; set; }

        int BulkListingLength { get; set; }

        List<FtpCapability> Capabilities { get; }

        bool CheckCapabilities { get; set; }

        X509CertificateCollection ClientCertificates { get; }

        string ConnectionType { get; }

        int ConnectTimeout { get; set; }

        NetworkCredential Credentials { get; set; }

        int DataConnectionConnectTimeout { get; set; }

        bool DataConnectionEncryption { get; set; }

        int DataConnectionReadTimeout { get; set; }

        FtpDataConnectionType DataConnectionType { get; set; }

        FtpDataType DownloadDataType { get; set; }

        bool DownloadDirectoryDeleteExcluded { get; set; }

        uint DownloadRateLimit { get; set; }

        bool DownloadZeroByteFiles { get; set; }

        bool EnableThreadSafeDataConnections { get; set; }

        System.Text.Encoding Encoding { get; set; }

        FtpEncryptionMode EncryptionMode { get; set; }

        FtpDataType FXPDataType { get; set; }

        int FXPProgressInterval { get; set; }

        FtpHashAlgorithm HashAlgorithms { get; }

        string Host { get; set; }

        FtpIpVersion InternetProtocolVersions { get; set; }

        bool IsConnected { get; }

        bool IsDisposed { get; }

        FtpReply LastReply { get; }

        CultureInfo ListingCulture { get; set; }

        FtpDataType ListingDataType { get; set; }

        FtpParser ListingParser { get; set; }

        int MaximumDereferenceCount { get; set; }

        int NoopInterval { get; set; }

        bool PlainTextEncryption { get; set; }

        int Port { get; set; }

        int ReadTimeout { get; set; }

        bool RecursiveList { get; set; }

        int RetryAttempts { get; set; }

        bool SendHost { get; set; }

        string SendHostDomain { get; set; }

        FtpBaseServer ServerHandler { get; set; }

        FtpOperatingSystem ServerOS { get; }

        FtpServer ServerType { get; }

        bool SocketKeepAlive { get; set; }

        int SocketPollInterval { get; set; }

        FtpsBuffering SslBuffering { get; set; }

        System.Security.Authentication.SslProtocols SslProtocols { get; set; }

        bool StaleDataCheck { get; set; }

        string SystemType { get; }

        DateTimeStyles TimeConversion { get; set; }

        double TimeOffset { get; set; }

        int TransferChunkSize { get; set; }

        bool UngracefullDisconnection { get; set; }

        FtpDataType UploadDataType { get; set; }

        bool UploadDirectoryDeleteExcluded { get; set; }

        uint UploadRateLimit { get; set; }

        bool ValidateAnyCertificate { get; set; }

        bool ValidateCertificateRevocation { get; set; }
    }
}

