namespace FluentFTP.Rules
{
    using FluentFTP;
    using FluentFTP.Helpers;
    using System;
    using System.Runtime.InteropServices;

    public class FtpSizeRule : FtpRule
    {
        public FtpOperator Operator;
        public long X;
        public long Y;

        public FtpSizeRule(FtpOperator ruleOperator, long x, long y = 0L)
        {
            this.Operator = ruleOperator;
            this.X = x;
            this.Y = y;
        }

        public override bool IsAllowed(FtpListItem result)
        {
            if (result.Type == FtpFileSystemObjectType.File)
            {
                return FtpOperators.Validate(this.Operator, result.Size, this.X, this.Y);
            }
            return true;
        }
    }
}

