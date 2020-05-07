namespace FluentFTP
{
    using System;

    public enum FtpResponseType
    {
        None,
        PositivePreliminary,
        PositiveCompletion,
        PositiveIntermediate,
        TransientNegativeCompletion,
        PermanentNegativeCompletion
    }
}

