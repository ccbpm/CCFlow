namespace FluentFTP
{
    using System;

    [Flags]
    public enum FtpSpecialPermissions
    {
        None = 0,
        SetGroupID = 2,
        SetUserID = 4,
        Sticky = 1
    }
}

