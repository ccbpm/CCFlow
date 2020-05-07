namespace FluentFTP.Helpers.Parsers
{
    using FluentFTP;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;

    internal static class FtpUnixParser
    {
        private static string[] DateTimeAltFormats1 = new string[] { "MMM'-'d'-'yyyy", "MMM'-'dd'-'yyyy" };
        private static string[] DateTimeAltFormats2 = new string[] { "MMM'-'d'-'yyyy'-'HH':'mm:ss", "MMM'-'dd'-'yyyy'-'HH':'mm:ss", "MMM'-'d'-'yyyy'-'H':'mm:ss", "MMM'-'dd'-'yyyy'-'H':'mm:ss" };
        private static string[] DateTimeFormats1 = new string[] { "MMM'-'d'-'yyyy", "MMM'-'dd'-'yyyy" };
        private static string[] DateTimeFormats2 = new string[] { "MMM'-'d'-'yyyy'-'HH':'mm", "MMM'-'dd'-'yyyy'-'HH':'mm", "MMM'-'d'-'yyyy'-'H':'mm", "MMM'-'dd'-'yyyy'-'H':'mm", "MMM'-'dd'-'yyyy'-'H'.'mm" };
        private static char DirectoryMarker = 'd';
        private static char FileMarker = '-';
        private static int MinFieldCount = 7;
        private static int MinFieldCountAlt = 8;
        private static string SymbolicLinkArrowMarker = "->";
        private static char SymbolicLinkMarker = 'l';

        public static bool IsValid(FtpClient client, string[] records)
        {
            int num = Math.Min(records.Length, 10);
            bool flag = false;
            bool flag2 = false;
            for (int i = 0; i < num; i++)
            {
                string str = records[i];
                if (str.Trim().Length != 0)
                {
                    string[] strArray = str.SplitString();
                    if (strArray.Length >= MinFieldCount)
                    {
                        switch (char.ToLower(strArray[0][0]))
                        {
                            case '-':
                            case 'l':
                            case 'd':
                                flag = true;
                                break;
                        }
                        if (strArray[0].Length > 1)
                        {
                            switch (char.ToLower(strArray[0][1]))
                            {
                                case 'r':
                                case '-':
                                    flag2 = true;
                                    break;
                            }
                        }
                        if ((!flag2 && (strArray[0].Length > 2)) && (strArray[0].IndexOf('-', 2) > 0))
                        {
                            flag2 = true;
                        }
                    }
                }
            }
            if (flag & flag2)
            {
                return true;
            }
            client.LogStatus(FtpTraceLevel.Verbose, "Not in UNIX format");
            return false;
        }

        public static FtpListItem Parse(FtpClient client, string record)
        {
            string str;
            bool flag;
            bool flag2;
            string str2;
            string str3;
            char ch = record[0];
            if (((ch != FileMarker) && (ch != DirectoryMarker)) && (ch != SymbolicLinkMarker))
            {
                return null;
            }
            string[] values = record.SplitString();
            if (values.Length < MinFieldCount)
            {
                StringBuilder builder = new StringBuilder("Unexpected number of fields in listing '");
                builder.Append(record).Append("' - expected minimum ").Append(MinFieldCount).Append(" fields but found ").Append(values.Length).Append(" fields");
                client.LogStatus(FtpTraceLevel.Verbose, builder.ToString());
                return null;
            }
            int index = 0;
            ParsePermissions(values, ref index, out str, out flag, out flag2);
            int num2 = ParseLinkCount(client, values, ref index);
            ParseOwnerGroup(values, ref index, out str2, out str3);
            long size = ParseFileSize(client, values, ref index);
            int dayOfMonth = ParseDayOfMonth(values, ref index);
            int dateTimePos = index;
            DateTime minValue = DateTime.MinValue;
            ParseDateTime(client, values, ref index, dayOfMonth, ref minValue);
            string name = null;
            string linkedname = null;
            ParseName(client, record, values, flag2, dayOfMonth, dateTimePos, ref name, ref linkedname);
            FtpListItem item = new FtpListItem(record, name, size, flag, ref minValue);
            if (flag2)
            {
                item.Type = FtpFileSystemObjectType.Link;
                item.LinkCount = num2;
                item.LinkTarget = linkedname.Trim();
            }
            item.RawGroup = str3;
            item.RawOwner = str2;
            item.RawPermissions = str;
            item.CalculateUnixPermissions(str);
            return item;
        }

        private static DateTime ParseDateTime(FtpClient client, StringBuilder stamp, string[] format)
        {
            DateTime minValue = DateTime.MinValue;
            try
            {
                minValue = DateTime.ParseExact(stamp.ToString(), format, client.ListingCulture.DateTimeFormat, DateTimeStyles.None);
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse date string '" + stamp.ToString() + "'");
            }
            if (minValue > DateTime.Now.AddDays(2.0))
            {
                minValue = minValue.AddYears(-1);
            }
            return minValue;
        }

        private static void ParseDateTime(FtpClient client, string[] values, ref int index, int dayOfMonth, ref DateTime lastModified)
        {
            int num = index;
            index = num + 1;
            StringBuilder stamp = new StringBuilder(values[num]);
            stamp.Append('-');
            if (dayOfMonth > 0)
            {
                stamp.Append(dayOfMonth);
            }
            else
            {
                num = index;
                index = num + 1;
                stamp.Append(values[num]);
            }
            stamp.Append('-');
            num = index;
            index = num + 1;
            string str = values[num];
            if ((str.IndexOf(':') < 0) && (str.IndexOf('.') < 0))
            {
                stamp.Append(str);
                lastModified = ParseYear(client, stamp, DateTimeFormats1);
            }
            else
            {
                int year = client.ListingCulture.Calendar.GetYear(DateTime.Now);
                stamp.Append(year).Append('-').Append(str);
                lastModified = ParseDateTime(client, stamp, DateTimeFormats2);
            }
        }

        private static int ParseDayOfMonth(string[] values, ref int index)
        {
            int num = -1;
            if (values[index].IsNumeric())
            {
                try
                {
                    char[] trimChars = new char[] { '0' };
                    values[index].TrimStart(trimChars);
                    num = int.Parse(values[index]);
                    if (num > 0x1f)
                    {
                        num = -1;
                    }
                    if (!char.IsLetter(values[index + 1][0]))
                    {
                        num = -1;
                    }
                    if (values[index + 2].IndexOf(':') <= 0)
                    {
                        num = -1;
                    }
                }
                catch (FormatException)
                {
                }
                index++;
            }
            return num;
        }

        private static long ParseFileSize(FtpClient client, string[] values, ref int index)
        {
            long num = 0L;
            int num2 = index;
            index = num2 + 1;
            string s = values[num2].Replace(".", "");
            try
            {
                num = long.Parse(s);
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse size: " + s);
            }
            return num;
        }

        public static FtpListItem ParseLegacy(string record, List<FtpCapability> capabilities, FtpClient client)
        {
            Match match;
            long num;
            string pattern = @"(?<permissions>.+)\s+(?<objectcount>\d+)\s+(?<user>.+)\s+(?<group>.+)\s+(?<size>\d+)\s+(?<modify>\w+\s+\d+\s+\d+:\d+|\w+\s+\d+\s+\d+)\s(?<name>.*)$";
            FtpListItem item = new FtpListItem();
            if ((match = Regex.Match(record, pattern, RegexOptions.IgnoreCase)).Success)
            {
                if (match.Groups["permissions"].Value.Length == 0)
                {
                    return null;
                }
                switch (match.Groups["permissions"].Value[0])
                {
                    case 'l':
                        item.Type = FtpFileSystemObjectType.Link;
                        goto Label_0093;

                    case 's':
                    case '-':
                        item.Type = FtpFileSystemObjectType.File;
                        goto Label_0093;

                    case 'd':
                        item.Type = FtpFileSystemObjectType.Directory;
                        goto Label_0093;
                }
            }
            return null;
        Label_0093:
            if (match.Groups["name"].Value.Length >= 1)
            {
                item.Name = match.Groups["name"].Value;
                FtpFileSystemObjectType type = item.Type;
                if (type != FtpFileSystemObjectType.Directory)
                {
                    if (type == FtpFileSystemObjectType.Link)
                    {
                        if (!item.Name.Contains(" -> "))
                        {
                            return null;
                        }
                        item.LinkTarget = item.Name.Remove(0, item.Name.IndexOf("-> ") + 3).Trim();
                        item.Name = item.Name.Remove(item.Name.IndexOf(" -> "));
                    }
                    goto Label_0168;
                }
                if (!(item.Name == ".") && !(item.Name == ".."))
                {
                    goto Label_0168;
                }
            }
            return null;
        Label_0168:
            if ((!capabilities.Contains(FtpCapability.MDTM) || (item.Type == FtpFileSystemObjectType.Directory)) && (match.Groups["modify"].Value.Length > 0))
            {
                item.Modified = match.Groups["modify"].Value.GetFtpDate(DateTimeStyles.AssumeLocal);
                if (item.Modified == DateTime.MinValue)
                {
                    client.LogStatus(FtpTraceLevel.Warn, "GetFtpDate() failed on " + match.Groups["modify"].Value);
                }
            }
            else if (match.Groups["modify"].Value.Length == 0)
            {
                client.LogStatus(FtpTraceLevel.Warn, "RegEx failed to parse modified date from " + record);
            }
            else if (item.Type == FtpFileSystemObjectType.Directory)
            {
                client.LogStatus(FtpTraceLevel.Warn, "Modified times of directories are ignored in UNIX long listings.");
            }
            else if (capabilities.Contains(FtpCapability.MDTM))
            {
                client.LogStatus(FtpTraceLevel.Warn, "Ignoring modified date because MDTM feature is present. If you aren't already, pass FtpListOption.Modify or FtpListOption.SizeModify to GetListing() to retrieve the modification time.");
            }
            if ((match.Groups["size"].Value.Length > 0) && long.TryParse(match.Groups["size"].Value, out num))
            {
                item.Size = num;
            }
            if (match.Groups["permissions"].Value.Length > 0)
            {
                item.CalculateUnixPermissions(match.Groups["permissions"].Value);
            }
            return item;
        }

        private static int ParseLinkCount(FtpClient client, string[] values, ref int index)
        {
            int num = 0;
            if (char.IsDigit(values[index][0]))
            {
                int num2 = index;
                index = num2 + 1;
                string s = values[num2];
                try
                {
                    num = int.Parse(s);
                }
                catch (FormatException)
                {
                    client.LogStatus(FtpTraceLevel.Error, "Failed to parse link count: " + s);
                }
                return num;
            }
            if (values[index][0] == '-')
            {
                index++;
            }
            return num;
        }

        private static void ParseName(FtpClient client, string record, string[] values, bool isLink, int dayOfMonth, int dateTimePos, ref string name, ref string linkedname)
        {
            int startIndex = 0;
            bool flag = true;
            int num2 = (dayOfMonth > 0) ? 2 : 3;
            for (int i = dateTimePos; i < (dateTimePos + num2); i++)
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
                string str = record.Substring(startIndex).Trim();
                if (!isLink)
                {
                    name = str;
                }
                else
                {
                    startIndex = str.IndexOf(SymbolicLinkArrowMarker);
                    if (startIndex <= 0)
                    {
                        name = str;
                    }
                    else
                    {
                        int length = SymbolicLinkArrowMarker.Length;
                        name = str.Substring(0, startIndex).Trim();
                        if ((startIndex + length) < str.Length)
                        {
                            linkedname = str.Substring(startIndex + length);
                        }
                    }
                }
            }
            else
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to retrieve name: " + record);
            }
        }

        private static void ParseOwnerGroup(string[] values, ref int index, out string owner, out string group)
        {
            int num;
            owner = "";
            group = "";
            if (values[index + 2].IsNumeric() && ((values.Length - (index + 2)) > 4))
            {
                num = index;
                index = num + 1;
                owner = values[num];
                num = index;
                index = num + 1;
                group = values[num];
            }
            else if (values[index + 1].IsNumeric() && ((values.Length - (index + 1)) > 4))
            {
                num = index;
                index = num + 1;
                group = values[num];
            }
        }

        private static int ParsePermissions(string[] values, ref int index, out string permissions, out bool isDir, out bool isLink)
        {
            int num = index;
            index = num + 1;
            permissions = values[num];
            char ch = permissions[0];
            isDir = false;
            isLink = false;
            if (ch == DirectoryMarker)
            {
                isDir = true;
                return index;
            }
            if (ch == SymbolicLinkMarker)
            {
                isLink = true;
            }
            return index;
        }

        public static FtpListItem ParseUnixAlt(FtpClient client, string record)
        {
            char ch = record[0];
            if (((ch != FileMarker) && (ch != DirectoryMarker)) && (ch != SymbolicLinkMarker))
            {
                return null;
            }
            string[] strArray = record.SplitString();
            if (strArray.Length < MinFieldCountAlt)
            {
                StringBuilder builder1 = new StringBuilder("Unexpected number of fields in listing '");
                builder1.Append(record).Append("' - expected minimum ").Append(MinFieldCountAlt).Append(" fields but found ").Append(strArray.Length).Append(" fields");
                throw new FormatException(builder1.ToString());
            }
            int index = 0;
            string permissions = strArray[index++];
            ch = permissions[0];
            bool isDir = false;
            bool flag2 = false;
            if (ch == DirectoryMarker)
            {
                isDir = true;
            }
            else if (ch == SymbolicLinkMarker)
            {
                flag2 = true;
            }
            string str2 = strArray[index++];
            int num2 = 0;
            if (char.IsDigit(strArray[index][0]))
            {
                string str7 = strArray[index++];
                try
                {
                    num2 = int.Parse(str7);
                }
                catch (FormatException)
                {
                    client.LogStatus(FtpTraceLevel.Error, "Failed to parse link count: " + str7);
                }
            }
            string str3 = strArray[index++];
            long size = 0L;
            string s = strArray[index++];
            try
            {
                size = long.Parse(s);
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse size: " + s);
            }
            int num4 = index;
            DateTime minValue = DateTime.MinValue;
            StringBuilder stamp = new StringBuilder(strArray[index++]);
            stamp.Append('-').Append(strArray[index++]).Append('-');
            string str5 = strArray[index++];
            if (str5.IndexOf(':') < 0)
            {
                stamp.Append(str5);
                minValue = ParseYear(client, stamp, DateTimeAltFormats1);
            }
            else
            {
                int year = client.ListingCulture.Calendar.GetYear(DateTime.Now);
                stamp.Append(year).Append('-').Append(str5);
                minValue = ParseDateTime(client, stamp, DateTimeAltFormats2);
            }
            string name = null;
            int startIndex = 0;
            bool flag3 = true;
            for (int i = num4; i < (num4 + 3); i++)
            {
                startIndex = record.IndexOf(strArray[i], startIndex);
                if (startIndex < 0)
                {
                    flag3 = false;
                    break;
                }
                startIndex += strArray[i].Length;
            }
            if (flag3)
            {
                name = record.Substring(startIndex).Trim();
            }
            else
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to retrieve name: " + record);
            }
            FtpListItem item = new FtpListItem(record, name, size, isDir, ref minValue);
            if (flag2)
            {
                item.Type = FtpFileSystemObjectType.Link;
                item.LinkCount = num2;
            }
            item.RawGroup = str2;
            item.RawOwner = str3;
            item.RawPermissions = permissions;
            item.CalculateUnixPermissions(permissions);
            return item;
        }

        private static DateTime ParseYear(FtpClient client, StringBuilder stamp, string[] format)
        {
            DateTime minValue = DateTime.MinValue;
            try
            {
                minValue = DateTime.ParseExact(stamp.ToString(), format, client.ListingCulture.DateTimeFormat, DateTimeStyles.None);
            }
            catch (FormatException)
            {
                client.LogStatus(FtpTraceLevel.Error, "Failed to parse date string '" + stamp.ToString() + "'");
            }
            return minValue;
        }
    }
}

