namespace FluentFTP
{
    using System;

    [Flags]
    public enum FtpCapability
    {
        AVBL = 0x1c,
        CCC = 0x21,
        CLNT = 0x16,
        CPSV = 20,
        DSIZ = 0x1f,
        EPSV = 0x13,
        HASH = 12,
        HOST = 0x20,
        LANG = 0x23,
        MD5 = 13,
        MDTM = 4,
        MFCT = 9,
        MFF = 10,
        MFMT = 8,
        MLSD = 2,
        MMD5 = 0x24,
        MODE_Z = 0x22,
        NONE = 1,
        NOOP = 0x15,
        PRET = 7,
        REST = 5,
        RMDA = 30,
        SITE_MKDIR = 0x18,
        SITE_RMDIR = 0x19,
        SITE_SYMLINK = 0x1b,
        SITE_UTIME = 0x1a,
        SIZE = 3,
        SSCN = 0x17,
        STAT = 11,
        THMB = 0x1d,
        UTF8 = 6,
        XCRC = 15,
        XMD5 = 14,
        XSHA1 = 0x10,
        XSHA256 = 0x11,
        XSHA512 = 0x12
    }
}

