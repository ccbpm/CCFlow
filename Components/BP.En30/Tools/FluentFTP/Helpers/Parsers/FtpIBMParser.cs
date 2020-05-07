namespace FluentFTP.Helpers.Parsers
{
    using FluentFTP;
    using FluentFTP.Exceptions;
    using System;
    using System.Globalization;

    internal static class FtpIBMParser
    {
        private static string[][] DateTimeFormats;
        private static string DDirectoryMarker = "*DDIR";
        private static string DirectoryMarker = "*DIR";
        private static string FileMarker = "*FILE";
        private static int formatIndex = 0;
        private static string MemoryMarker = "*MEM";
        private static int MinFieldCount = 5;
        private static string[] ValidListFormats;

        static FtpIBMParser()
        {
            string[][] textArrayArray1 = new string[3][];
            textArrayArray1[0] = new string[] { "dd'/'MM'/'yy' 'HH':'mm':'ss", "dd'/'MM'/'yyyy' 'HH':'mm':'ss", "dd'.'MM'.'yy' 'HH':'mm':'ss" };
            textArrayArray1[1] = new string[] { "yy'/'MM'/'dd' 'HH':'mm':'ss", "yyyy'/'MM'/'dd' 'HH':'mm':'ss", "yy'.'MM'.'dd' 'HH':'mm':'ss" };
            textArrayArray1[2] = new string[] { "MM'/'dd'/'yy' 'HH':'mm':'ss", "MM'/'dd'/'yyyy' 'HH':'mm':'ss", "MM'.'dd'.'yy' 'HH':'mm':'ss" };
            DateTimeFormats = textArrayArray1;
            ValidListFormats = new string[] { "*DIR", "*FILE", "*FLR", "*DDIR", "*STMF", "*LIB" };
        }

        public static bool IsValid(FtpClient client, string[] listing)
        {
            int num = Math.Min(listing.Length, 10);
            for (int i = 0; i < num; i++)
            {
                if (listing[i].ContainsAny(ValidListFormats, 0))
                {
                    return true;
                }
            }
            client.LogStatus(FtpTraceLevel.Verbose, "Not in OS/400 format");
            return false;
        }

        public static FtpListItem Parse(FtpClient client, string record)
        {
            string[] strArray = record.SplitString();
            if (strArray.Length == 0)
            {
                return null;
            }
            if ((strArray.Length >= 2) && strArray[1].Equals(MemoryMarker))
            {
                DateTime minValue = DateTime.MinValue;
                string str4 = strArray[0];
                return new FtpListItem(record, strArray[2], 0L, false, ref minValue) { RawOwner = str4 };
            }
            if (strArray.Length < MinFieldCount)
            {
                return null;
            }
            string str = strArray[0];
            long size = long.Parse(strArray[1]);
            string lastModifiedStr = strArray[2] + " " + strArray[3];
            DateTime lastModifiedTime = ParseDateTime(client, lastModifiedStr);
            bool isDir = false;
            if (((strArray[4] == DirectoryMarker) || (strArray[4] == DDirectoryMarker)) || ((strArray.Length == 5) && (strArray[4] == FileMarker)))
            {
                isDir = true;
            }
            string name = (strArray.Length >= 6) ? strArray[5] : ".";
            if (name.EndsWith("/"))
            {
                isDir = true;
                name = name.Substring(0, name.Length - 1);
            }
            return new FtpListItem(record, name, size, isDir, ref lastModifiedTime) { RawOwner = str };
        }

        private static DateTime ParseDateTime(FtpClient client, string lastModifiedStr)
        {
            DateTime minValue = DateTime.MinValue;
            if (FtpIBMParser.formatIndex >= DateTimeFormats.Length)
            {
                client.LogStatus(FtpTraceLevel.Warn, "Exhausted formats - failed to parse date");
                return DateTime.MinValue;
            }
            int formatIndex = FtpIBMParser.formatIndex;
            int num2 = FtpIBMParser.formatIndex;
            while (num2 < DateTimeFormats.Length)
            {
                try
                {
                    minValue = DateTime.ParseExact(lastModifiedStr, DateTimeFormats[FtpIBMParser.formatIndex], client.ListingCulture.DateTimeFormat, DateTimeStyles.None);
                    if (minValue > DateTime.Now.AddDays(2.0))
                    {
                        client.LogStatus(FtpTraceLevel.Verbose, "Swapping to alternate format (found date in future)");
                    }
                    else
                    {
                        break;
                    }
                }
                catch (FormatException)
                {
                }
                num2++;
                FtpIBMParser.formatIndex++;
            }
            if (FtpIBMParser.formatIndex >= DateTimeFormats.Length)
            {
                client.LogStatus(FtpTraceLevel.Warn, "Exhausted formats - failed to parse date");
                return DateTime.MinValue;
            }
            if (FtpIBMParser.formatIndex > formatIndex)
            {
                throw new FtpListParseException();
            }
            return minValue;
        }
    }
}

