namespace FluentFTP.Helpers
{
    using FluentFTP;
    using System;

    public static class FtpOperators
    {
        public static bool Validate(FtpOperator op, double value, double x, double y)
        {
            switch (op)
            {
                case FtpOperator.Equals:
                    return (value == x);

                case FtpOperator.NotEquals:
                    return !(value == x);

                case FtpOperator.LessThan:
                    return (value < x);

                case FtpOperator.LessThanOrEquals:
                    return (value <= x);

                case FtpOperator.MoreThan:
                    return (value > x);

                case FtpOperator.MoreThanOrEquals:
                    return (value >= x);

                case FtpOperator.BetweenRange:
                    if (value < x)
                    {
                        return false;
                    }
                    return (value <= y);

                case FtpOperator.OutsideRange:
                    return ((value < x) || (value > y));
            }
            return false;
        }

        public static bool Validate(FtpOperator op, int value, int x, int y)
        {
            switch (op)
            {
                case FtpOperator.Equals:
                    return (value == x);

                case FtpOperator.NotEquals:
                    return (value != x);

                case FtpOperator.LessThan:
                    return (value < x);

                case FtpOperator.LessThanOrEquals:
                    return (value <= x);

                case FtpOperator.MoreThan:
                    return (value > x);

                case FtpOperator.MoreThanOrEquals:
                    return (value >= x);

                case FtpOperator.BetweenRange:
                    if (value < x)
                    {
                        return false;
                    }
                    return (value <= y);

                case FtpOperator.OutsideRange:
                    return ((value < x) || (value > y));
            }
            return false;
        }

        public static bool Validate(FtpOperator op, long value, long x, long y)
        {
            switch (op)
            {
                case FtpOperator.Equals:
                    return (value == x);

                case FtpOperator.NotEquals:
                    return (value != x);

                case FtpOperator.LessThan:
                    return (value < x);

                case FtpOperator.LessThanOrEquals:
                    return (value <= x);

                case FtpOperator.MoreThan:
                    return (value > x);

                case FtpOperator.MoreThanOrEquals:
                    return (value >= x);

                case FtpOperator.BetweenRange:
                    if (value < x)
                    {
                        return false;
                    }
                    return (value <= y);

                case FtpOperator.OutsideRange:
                    return ((value < x) || (value > y));
            }
            return false;
        }
    }
}

