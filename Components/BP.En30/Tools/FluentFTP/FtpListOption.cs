namespace FluentFTP
{
    using System;

    [Flags]
    public enum FtpListOption
    {
        AllFiles = 4,
        Auto = 0,
        DerefLinks = 0x20,
        ForceList = 8,
        ForceNameList = 0x18,
        IncludeSelfAndParent = 0x200,
        Modify = 1,
        NameList = 0x10,
        NoPath = 0x100,
        Recursive = 0x80,
        Size = 2,
        SizeModify = 3,
        UseLS = 0x48,
        UseStat = 0x400
    }
}

