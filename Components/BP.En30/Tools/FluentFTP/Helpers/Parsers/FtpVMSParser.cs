namespace FluentFTP.Helpers.Parsers
{
    using FluentFTP;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;

    internal static class FtpVMSParser
    {
        private static string DirectoryMarker = ".DIR";
        private static int FileBlockSize = 0x80000;
        private static string HDirectoryMarker = "Directory";
        private static int MinFieldCount = 4;
        private static string TotalMarker = "Total";

        public static bool IsValid(FtpClient client, string[] records)
        {
            int num = Math.Min(records.Length, 10);
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            for (int i = 0; i < num; i++)
            {
                string source = records[i];
                if (source.Trim().Length != 0)
                {
                    int num3 = 0;
                    if ((((num3 = source.IndexOf(';')) > 0) && (++num3 < source.Length)) && char.IsDigit(source[num3]))
                    {
                        flag = true;
                    }
                    if (source.Contains<char>('['))
                    {
                        flag2 = true;
                    }
                    if (source.Contains<char>(']'))
                    {
                        flag3 = true;
                    }
                }
            }
            if ((flag & flag2) & flag3)
            {
                return true;
            }
            client.LogStatus(FtpTraceLevel.Verbose, "Not in VMS format");
            return false;
        }

        public static FtpListItem Parse(FtpClient client, string record)
        {
            string[] values = record.SplitString();
            if (values.Length == 0)
            {
                return null;
            }
            if ((values.Length >= 2) && values[0].Equals(HDirectoryMarker))
            {
                return null;
            }
            if ((values.Length != 0) && values[0].Equals(TotalMarker))
            {
                return null;
            }
            if (values.Length < MinFieldCount)
            {
                return null;
            }
            string name = values[0];
            int length = name.LastIndexOf(';');
            if (length <= 0)
            {
                client.LogStatus(FtpTraceLevel.Verbose, "File version number not found in name '" + name + "'");
                return null;
            }
            string str2 = name.Substring(0, length);
            string s = values[0].Substring(length + 1);
            try
            {
                long.Parse(s);
            }
            catch (FormatException)
            {
            }
            bool isDir = false;
            if (str2.EndsWith(DirectoryMarker))
            {
                isDir = true;
                name = str2.Substring(0, str2.Length - DirectoryMarker.Length);
            }
            if (!FtpListParser.VMSNameHasVersion && !isDir)
            {
                name = str2;
            }
            long size = ParseFileSize(values[1]);
            DateTime lastModifiedTime = ParseDateTime(client, values[2], values[3]);
            string group = null;
            string owner = null;
            ParseGroupOwner(values, out group, out owner);
            string str6 = ParsePermissions(values);
            return new FtpListItem(record, name, size, isDir, ref lastModifiedTime) { RawGroup = group, RawOwner = owner, RawPermissions = str6 };
        }

        private static DateTime ParseDateTime(FtpClient client, string date, string time)
        {
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            for (int i = 0; i < date.Length; i++)
            {
                if (!char.IsLetter(date[i]))
                {
                    builder.Append(date[i]);
                }
                else if (!flag)
                {
                    builder.Append(date[i]);
                    flag = true;
                }
                else
                {
                    builder.Append(char.ToLower(date[i]));
                }
            }
            builder.Append(" ").Append(time);
            string s = builder.ToString();
            try
            {
                return DateTime.Parse(s, client.ListingCulture.DateTimeFormat);
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse date string '" + s + "'");
            }
            return DateTime.MinValue;
        }

        private static long ParseFileSize(string sizeStr)
        {
            int index = sizeStr.IndexOf('/');
            string s = sizeStr;
            if (index == -1)
            {
                return long.Parse(sizeStr);
            }
            if (index > 0)
            {
                s = sizeStr.Substring(0, index);
            }
            return (long.Parse(s) * FileBlockSize);
        }

        private static void ParseGroupOwner(string[] values, out string group, out string owner)
        {
            group = null;
            owner = null;
            if (((values.Length >= 5) && (values[4][0] == '[')) && (values[4][values[4].Length - 1] == ']'))
            {
                int index = values[4].IndexOf(',');
                if (index < 0)
                {
                    owner = values[4].Substring(1, values[4].Length - 2);
                    group = "";
                }
                else
                {
                    group = values[4].Substring(1, index - 1);
                    owner = values[4].Substring(index + 1, (values[4].Length - index) - 2);
                }
            }
        }

        public static FtpListItem ParseLegacy(string record, List<FtpCapability> capabilities, FtpClient client)
        {
            Match match;
            DateTime minValue;
            string pattern = @"(?<name>.+)\.(?<extension>.+);(?<version>\d+)\s+(?<size>\d+)\s+(?<modify>\d+-\w+-\d+\s+\d+:\d+)";
            if (!(match = Regex.Match(record, pattern)).Success)
            {
                return null;
            }
            FtpListItem item = new FtpListItem();
            string[] textArray1 = new string[] { match.Groups["name"].Value, ".", match.Groups["extension"].Value, ";", match.Groups["version"].Value };
            item.Name = string.Concat(textArray1);
            if (match.Groups["extension"].Value.ToUpper() == "DIR")
            {
                item.Type = FtpFileSystemObjectType.Directory;
            }
            else
            {
                item.Type = FtpFileSystemObjectType.File;
            }
            long result = 0L;
            if (!long.TryParse(match.Groups["size"].Value, out result))
            {
                result = -1L;
            }
            item.Size = result;
            if (!DateTime.TryParse(match.Groups["modify"].Value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out minValue))
            {
                minValue = DateTime.MinValue;
            }
            item.Modified = minValue;
            return item;
        }

        private static string ParsePermissions(string[] values)
        {
            if (((values.Length >= 6) && (values[5][0] == '(')) && (values[5][values[5].Length - 1] == ')'))
            {
                return values[5].Substring(1, values[5].Length - 2);
            }
            return null;
        }
    }
}

