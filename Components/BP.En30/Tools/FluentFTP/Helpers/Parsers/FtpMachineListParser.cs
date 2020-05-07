namespace FluentFTP.Helpers.Parsers
{
    using FluentFTP;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal static class FtpMachineListParser
    {
        public static FtpListItem Parse(string record, List<FtpCapability> capabilities, FtpClient client)
        {
            Match match;
            FtpListItem item = new FtpListItem();
            if (!(match = Regex.Match(record, "type=(?<type>.+?);", RegexOptions.IgnoreCase)).Success)
            {
                return null;
            }
            string str = match.Groups["type"].Value.ToLower();
            if (!(str == "pdir"))
            {
                if (str != "cdir")
                {
                    if (str == "dir")
                    {
                        item.Type = FtpFileSystemObjectType.Directory;
                        item.SubType = FtpFileSystemObjectSubType.SubDirectory;
                        goto Label_00C3;
                    }
                    if (str == "file")
                    {
                        item.Type = FtpFileSystemObjectType.File;
                        goto Label_00C3;
                    }
                    if ((str == "link") || (str == "device"))
                    {
                    }
                    return null;
                }
            }
            else
            {
                item.Type = FtpFileSystemObjectType.Directory;
                item.SubType = FtpFileSystemObjectSubType.ParentDirectory;
                goto Label_00C3;
            }
            item.Type = FtpFileSystemObjectType.Directory;
            item.SubType = FtpFileSystemObjectSubType.SelfDirectory;
        Label_00C3:
            if ((match = Regex.Match(record, "; (?<name>.*)$", RegexOptions.IgnoreCase)).Success)
            {
                item.Name = match.Groups["name"].Value;
            }
            else
            {
                return null;
            }
            ParseDateTime(record, item, client);
            ParseFileSize(record, item);
            ParsePermissions(record, item);
            return item;
        }

        private static void ParseDateTime(string record, FtpListItem item, FtpClient client)
        {
            Match match;
            if ((match = Regex.Match(record, "modify=(?<modify>.+?);", RegexOptions.IgnoreCase)).Success)
            {
                item.Modified = match.Groups["modify"].Value.GetFtpDate(client.TimeConversion);
            }
            if ((match = Regex.Match(record, "created?=(?<create>.+?);", RegexOptions.IgnoreCase)).Success)
            {
                item.Created = match.Groups["create"].Value.GetFtpDate(client.TimeConversion);
            }
        }

        private static void ParseFileSize(string record, FtpListItem item)
        {
            Match match;
            long num;
            if ((match = Regex.Match(record, @"size=(?<size>\d+);", RegexOptions.IgnoreCase)).Success && long.TryParse(match.Groups["size"].Value, out num))
            {
                item.Size = num;
            }
        }

        private static void ParsePermissions(string record, FtpListItem item)
        {
            Match match;
            if ((match = Regex.Match(record, @"unix.mode=(?<mode>\d+);", RegexOptions.IgnoreCase)).Success)
            {
                char ch;
                if (match.Groups["mode"].Value.Length == 4)
                {
                    ch = match.Groups["mode"].Value[0];
                    item.SpecialPermissions = (FtpSpecialPermissions) int.Parse(ch.ToString());
                    ch = match.Groups["mode"].Value[1];
                    item.OwnerPermissions = (FtpPermission) int.Parse(ch.ToString());
                    ch = match.Groups["mode"].Value[2];
                    item.GroupPermissions = (FtpPermission) int.Parse(ch.ToString());
                    ch = match.Groups["mode"].Value[3];
                    item.OthersPermissions = (FtpPermission) int.Parse(ch.ToString());
                    item.CalculateChmod();
                }
                else if (match.Groups["mode"].Value.Length == 3)
                {
                    ch = match.Groups["mode"].Value[0];
                    item.OwnerPermissions = (FtpPermission) int.Parse(ch.ToString());
                    ch = match.Groups["mode"].Value[1];
                    item.GroupPermissions = (FtpPermission) int.Parse(ch.ToString());
                    ch = match.Groups["mode"].Value[2];
                    item.OthersPermissions = (FtpPermission) int.Parse(ch.ToString());
                    item.CalculateChmod();
                }
            }
        }
    }
}

