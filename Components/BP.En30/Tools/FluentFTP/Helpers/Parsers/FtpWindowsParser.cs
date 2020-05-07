namespace FluentFTP.Helpers.Parsers
{
    using FluentFTP;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;

    internal static class FtpWindowsParser
    {
        private static string[] DateTimeFormats = new string[] { "MM'-'dd'-'yy hh':'mmtt", "MM'-'dd'-'yy HH':'mm", "MM'-'dd'-'yyyy hh':'mmtt" };
        private static string DirectoryMarker = "<DIR>";
        private static int MinFieldCount = 4;

        public static bool IsValid(FtpClient client, string[] records)
        {
            int num = Math.Min(records.Length, 10);
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            for (int i = 0; i < num; i++)
            {
                string str = records[i];
                if (str.Trim().Length != 0)
                {
                    string[] strArray = str.SplitString();
                    if (strArray.Length >= MinFieldCount)
                    {
                        if (char.IsDigit(strArray[0][0]) && char.IsDigit(strArray[0][strArray[0].Length - 1]))
                        {
                            flag = true;
                        }
                        if (strArray[1].IndexOf(':') > 0)
                        {
                            flag2 = true;
                        }
                        if ((strArray[2].ToUpper() == DirectoryMarker) || char.IsDigit(strArray[2][0]))
                        {
                            flag3 = true;
                        }
                    }
                }
            }
            if ((flag & flag2) & flag3)
            {
                return true;
            }
            client.LogStatus(FtpTraceLevel.Verbose, "Not in Windows format");
            return false;
        }

        public static FtpListItem Parse(FtpClient client, string record)
        {
            bool flag;
            long num;
            string[] values = record.SplitString();
            if (values.Length < MinFieldCount)
            {
                return null;
            }
            DateTime lastModifiedTime = ParseDateTime(client, values[0] + " " + values[1]);
            ParseTypeAndFileSize(client, values[2], out flag, out num);
            return new FtpListItem(record, ParseName(client, record, values), num, flag, ref lastModifiedTime);
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

        public static FtpListItem ParseLegacy(string record, List<FtpCapability> capabilities, FtpClient client)
        {
            Match match;
            DateTime time2;
            long num;
            FtpListItem item = new FtpListItem();
            string[] formats = new string[] { "MM-dd-yy  hh:mmtt", "MM-dd-yyyy  hh:mmtt" };
            if ((match = Regex.Match(record, @"(?<modify>\d+-\d+-\d+\s+\d+:\d+\w+)\s+<DIR>\s+(?<name>.*)$", RegexOptions.IgnoreCase)).Success)
            {
                DateTime time;
                item.Type = FtpFileSystemObjectType.Directory;
                item.Name = match.Groups["name"].Value;
                if (DateTime.TryParseExact(match.Groups["modify"].Value, formats, CultureInfo.InvariantCulture, client.TimeConversion, out time))
                {
                    item.Modified = time;
                }
                return item;
            }
            if (!(match = Regex.Match(record, @"(?<modify>\d+-\d+-\d+\s+\d+:\d+\w+)\s+(?<size>\d+)\s+(?<name>.*)$", RegexOptions.IgnoreCase)).Success)
            {
                return null;
            }
            item.Type = FtpFileSystemObjectType.File;
            item.Name = match.Groups["name"].Value;
            if (long.TryParse(match.Groups["size"].Value, out num))
            {
                item.Size = num;
            }
            if (DateTime.TryParseExact(match.Groups["modify"].Value, formats, CultureInfo.InvariantCulture, client.TimeConversion, out time2))
            {
                item.Modified = time2;
            }
            return item;
        }

        private static string ParseName(FtpClient client, string record, string[] values)
        {
            int startIndex = 0;
            bool flag = true;
            for (int i = 0; i < 3; i++)
            {
                startIndex = record.IndexOf(values[i], startIndex);
                if (startIndex < 0)
                {
                    flag = false;
                    break;
                }
                startIndex += values[i].Length;
            }
            if (flag)
            {
                return record.Substring(startIndex).Trim();
            }
            client.LogStatus(FtpTraceLevel.Error, "Failed to retrieve name: " + record);
            return null;
        }

        private static void ParseTypeAndFileSize(FtpClient client, string type, out bool isDir, out long size)
        {
            isDir = false;
            size = 0L;
            if (type.ToUpper().Equals(DirectoryMarker.ToUpper()))
            {
                isDir = true;
            }
            else
            {
                try
                {
                    size = long.Parse(type);
                }
                catch (FormatException)
                {
                    client.LogStatus(FtpTraceLevel.Error, "Failed to parse size: " + type);
                }
            }
        }
    }
}

