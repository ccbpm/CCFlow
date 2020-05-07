namespace FluentFTP.Helpers.Parsers
{
    using FluentFTP;
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal static class FtpNonStopParser
    {
        private static string[] DateTimeFormats = new string[] { "d'-'MMM'-'yy HH':'mm':'ss" };
        private static int MinFieldCount = 7;
        private static char[] TrimValues = new char[] { '"' };

        private static bool IsHeader(string line)
        {
            return ((line.Contains("Code") && line.Contains("EOF")) && line.Contains("RWEP"));
        }

        public static bool IsValid(FtpClient client, string[] records)
        {
            return IsHeader(records[0]);
        }

        public static FtpListItem Parse(FtpClient client, string record)
        {
            bool flag;
            long num;
            if (IsHeader(record))
            {
                return null;
            }
            string[] values = record.SplitString();
            if (values.Length < MinFieldCount)
            {
                return null;
            }
            string name = values[0];
            DateTime lastModifiedTime = ParseDateTime(client, values[3] + " " + values[4]);
            ParseDirAndFileSize(client, values, out flag, out num);
            string str2 = values[5] + values[6];
            string str3 = values[7].Trim(TrimValues);
            return new FtpListItem(record, name, num, flag, ref lastModifiedTime) { RawOwner = str2, RawPermissions = str3 };
        }

        private static DateTime ParseDateTime(FtpClient client, string lastModifiedStr)
        {
            try
            {
                return DateTime.ParseExact(lastModifiedStr, DateTimeFormats, client.ListingCulture.DateTimeFormat, DateTimeStyles.None);
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse date string '" + lastModifiedStr + "'");
            }
            return DateTime.MinValue;
        }

        private static void ParseDirAndFileSize(FtpClient client, string[] values, out bool isDir, out long size)
        {
            isDir = false;
            size = 0L;
            try
            {
                size = long.Parse(values[2]);
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse size: " + values[2]);
            }
        }
    }
}

