namespace FluentFTP.Rules
{
    using FluentFTP;
    using System;
    using System.Collections.Generic;

    public class FtpRule
    {
        public static bool IsAllAllowed(List<FtpRule> rules, FtpListItem result)
        {
            using (List<FtpRule>.Enumerator enumerator = rules.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (!enumerator.Current.IsAllowed(result))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public virtual bool IsAllowed(FtpListItem result)
        {
            return true;
        }
    }
}

