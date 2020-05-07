namespace FluentFTP
{
    using FluentFTP.Servers;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public static class FtpExtensions
    {
        private static string[] FtpDateFormats = new string[] { "yyyyMMddHHmmss", "yyyyMMddHHmmss'.'f", "yyyyMMddHHmmss'.'ff", "yyyyMMddHHmmss'.'fff", "MMM dd  yyyy", "MMM  d  yyyy", "MMM dd HH:mm", "MMM  d HH:mm" };
        private static string[] sizePostfix = new string[] { "bytes", "KB", "MB", "GB", "TB" };

        public static bool AddOnce<T>(this List<T> items, T item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                return true;
            }
            return false;
        }

        public static string[] AddPrefix(this string[] values, string prefix, bool trim = false)
        {
            List<string> list = new List<string>();
            foreach (string str in values)
            {
                string item = prefix + (trim ? str.Trim() : str);
                list.Add(item);
            }
            return list.ToArray();
        }

        public static List<string> AddPrefix(this List<string> values, string prefix, bool trim = false)
        {
            List<string> list = new List<string>();
            foreach (string str in values)
            {
                string item = prefix + (trim ? str.Trim() : str);
                list.Add(item);
            }
            return list;
        }

        public static int CalcChmod(FtpPermission owner, FtpPermission group, FtpPermission other)
        {
            int num = 0;
            if (HasPermission(owner, FtpPermission.None | FtpPermission.Read))
            {
                num += 400;
            }
            if (HasPermission(owner, FtpPermission.None | FtpPermission.Write))
            {
                num += 200;
            }
            if (HasPermission(owner, FtpPermission.Execute))
            {
                num += 100;
            }
            if (HasPermission(group, FtpPermission.None | FtpPermission.Read))
            {
                num += 40;
            }
            if (HasPermission(group, FtpPermission.None | FtpPermission.Write))
            {
                num += 20;
            }
            if (HasPermission(group, FtpPermission.Execute))
            {
                num += 10;
            }
            if (HasPermission(other, FtpPermission.None | FtpPermission.Read))
            {
                num += 4;
            }
            if (HasPermission(other, FtpPermission.None | FtpPermission.Write))
            {
                num += 2;
            }
            if (HasPermission(other, FtpPermission.Execute))
            {
                num++;
            }
            return num;
        }

        public static void CalculateChmod(this FtpListItem item)
        {
            item.Chmod = CalcChmod(item.OwnerPermissions, item.GroupPermissions, item.OthersPermissions);
        }

        public static void CalculateFullFtpPath(this FtpListItem item, FtpClient client, string path, bool isVMS)
        {
            if (path == null)
            {
                if (item.Name.IsAbsolutePath())
                {
                    item.FullName = item.Name;
                    item.Name = item.Name.GetFtpFileName();
                }
            }
            else if (isVMS)
            {
                item.FullName = path + item.Name;
            }
            else
            {
                if (path.GetFtpFileName().Contains("*"))
                {
                    path = path.GetFtpDirectoryName();
                }
                if (item.Name != null)
                {
                    if (item.Name.IsAbsolutePath())
                    {
                        item.FullName = item.Name;
                        item.Name = item.Name.GetFtpFileName();
                    }
                    else if (path != null)
                    {
                        string[] segments = new string[] { item.Name };
                        item.FullName = path.GetFtpPath(segments);
                    }
                    else
                    {
                        client.LogStatus(FtpTraceLevel.Warn, "Couldn't determine the full path of this object: " + Environment.NewLine + item.ToString());
                    }
                }
                if ((item.LinkTarget != null) && !item.LinkTarget.StartsWith("/"))
                {
                    if (item.LinkTarget.StartsWith("./"))
                    {
                        string[] textArray2 = new string[] { item.LinkTarget.Remove(0, 2) };
                        item.LinkTarget = path.GetFtpPath(textArray2).Trim();
                    }
                    else
                    {
                        string[] textArray3 = new string[] { item.LinkTarget };
                        item.LinkTarget = path.GetFtpPath(textArray3).Trim();
                    }
                }
            }
        }

        public static void CalculateUnixPermissions(this FtpListItem item, string permissions)
        {
            Match match = Regex.Match(permissions, @"[\w-]{1}(?<owner>[\w-]{3})(?<group>[\w-]{3})(?<others>[\w-]{3})", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                if (match.Groups["owner"].Value.Length == 3)
                {
                    if (match.Groups["owner"].Value[0] == 'r')
                    {
                        item.OwnerPermissions |= FtpPermission.None | FtpPermission.Read;
                    }
                    if (match.Groups["owner"].Value[1] == 'w')
                    {
                        item.OwnerPermissions |= FtpPermission.None | FtpPermission.Write;
                    }
                    if ((match.Groups["owner"].Value[2] == 'x') || (match.Groups["owner"].Value[2] == 's'))
                    {
                        item.OwnerPermissions |= FtpPermission.Execute;
                    }
                    if ((match.Groups["owner"].Value[2] == 's') || (match.Groups["owner"].Value[2] == 'S'))
                    {
                        item.SpecialPermissions |= FtpSpecialPermissions.SetUserID;
                    }
                }
                if (match.Groups["group"].Value.Length == 3)
                {
                    if (match.Groups["group"].Value[0] == 'r')
                    {
                        item.GroupPermissions |= FtpPermission.None | FtpPermission.Read;
                    }
                    if (match.Groups["group"].Value[1] == 'w')
                    {
                        item.GroupPermissions |= FtpPermission.None | FtpPermission.Write;
                    }
                    if ((match.Groups["group"].Value[2] == 'x') || (match.Groups["group"].Value[2] == 's'))
                    {
                        item.GroupPermissions |= FtpPermission.Execute;
                    }
                    if ((match.Groups["group"].Value[2] == 's') || (match.Groups["group"].Value[2] == 'S'))
                    {
                        item.SpecialPermissions |= FtpSpecialPermissions.SetGroupID;
                    }
                }
                if (match.Groups["others"].Value.Length == 3)
                {
                    if (match.Groups["others"].Value[0] == 'r')
                    {
                        item.OthersPermissions |= FtpPermission.None | FtpPermission.Read;
                    }
                    if (match.Groups["others"].Value[1] == 'w')
                    {
                        item.OthersPermissions |= FtpPermission.None | FtpPermission.Write;
                    }
                    if ((match.Groups["others"].Value[2] == 'x') || (match.Groups["others"].Value[2] == 't'))
                    {
                        item.OthersPermissions |= FtpPermission.Execute;
                    }
                    if ((match.Groups["others"].Value[2] == 't') || (match.Groups["others"].Value[2] == 'T'))
                    {
                        item.SpecialPermissions |= FtpSpecialPermissions.Sticky;
                    }
                }
                item.CalculateChmod();
            }
        }

        public static string CombineLocalPath(this string path, string fileOrFolder)
        {
            string str = Path.DirectorySeparatorChar.ToString();
            bool flag = path.EndsWith(str);
            bool flag2 = fileOrFolder.StartsWith(str);
            if ((flag && !flag2) || (!flag & flag2))
            {
                return (path + fileOrFolder);
            }
            if (flag & flag2)
            {
                return (path + fileOrFolder.Substring(1));
            }
            if (!flag && !flag2)
            {
                return (path + str + fileOrFolder);
            }
            return null;
        }

        public static bool ContainsAny(this string field, string[] values, int afterChar = -1)
        {
            foreach (string str in values)
            {
                if (field.IndexOf(str, StringComparison.Ordinal) > afterChar)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool EnsureDirectory(this string localPath)
        {
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
                return true;
            }
            return false;
        }

        public static string EnsurePostfix(this string text, string postfix)
        {
            if (!text.EndsWith(postfix))
            {
                return (text + postfix);
            }
            return text;
        }

        public static string EnsurePrefix(this string text, string prefix)
        {
            if (!text.StartsWith(prefix))
            {
                return (prefix + text);
            }
            return text;
        }

        public static string EscapeStringLiteral(this string input)
        {
            StringBuilder builder = new StringBuilder(input.Length + 2);
            builder.Append("\"");
            foreach (char ch in input)
            {
                switch (ch)
                {
                    case '\'':
                    {
                        builder.Append(@"\'");
                        continue;
                    }
                    case '\\':
                    {
                        builder.Append(@"\\");
                        continue;
                    }
                    case '\0':
                    {
                        builder.Append(@"\0");
                        continue;
                    }
                    case '\a':
                    {
                        builder.Append(@"\a");
                        continue;
                    }
                    case '\b':
                    {
                        builder.Append(@"\b");
                        continue;
                    }
                    case '\t':
                    {
                        builder.Append(@"\t");
                        continue;
                    }
                    case '\n':
                    {
                        builder.Append(@"\n");
                        continue;
                    }
                    case '\v':
                    {
                        builder.Append(@"\v");
                        continue;
                    }
                    case '\f':
                    {
                        builder.Append(@"\f");
                        continue;
                    }
                    case '\r':
                    {
                        builder.Append(@"\r");
                        continue;
                    }
                    case '"':
                    {
                        builder.Append("\\\"");
                        continue;
                    }
                }
                if ((ch >= ' ') && (ch <= '~'))
                {
                    builder.Append(ch);
                }
                else
                {
                    builder.Append(@"\u");
                    builder.Append(((int) ch).ToString("x4"));
                }
            }
            builder.Append("\"");
            return builder.ToString();
        }

        public static bool FileExistsInListing(FtpListItem[] fileList, string path)
        {
            if ((fileList != null) && (fileList.Length != 0))
            {
                char[] trimChars = new char[] { '/' };
                string str = path.Trim(trimChars);
                FtpListItem[] itemArray = fileList;
                for (int i = 0; i < itemArray.Length; i++)
                {
                    if (itemArray[i].FullName.Trim(trimChars) == str)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool FileExistsInNameListing(string[] fileList, string path)
        {
            if (fileList.Length != 0)
            {
                string ftpFileName = path.GetFtpFileName();
                string str2 = path.EnsurePrefix("/");
                foreach (string str3 in fileList)
                {
                    if (((str3 == ftpFileName) || (str3 == path)) || (str3.EnsurePrefix("/") == str2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static string FileSizeToString(this int bytes)
        {
            return ((long) bytes).FileSizeToString();
        }

        public static string FileSizeToString(this long bytes)
        {
            int index = 0;
            double num2 = bytes;
            while ((num2 >= 1024.0) && (index < (sizePostfix.Length - 1)))
            {
                index++;
                num2 /= 1024.0;
            }
            return string.Format("{0:0.#} {1}", num2, sizePostfix[index]);
        }

        public static string FileSizeToString(this uint bytes)
        {
            return ((long) bytes).FileSizeToString();
        }

        public static string FileSizeToString(this ulong bytes)
        {
            return ((long) bytes).FileSizeToString();
        }

        public static Task<TResult> FromAsync<TArg1, TArg2, TArg3, TArg4, TResult>(this TaskFactory factory, Func<TArg1, TArg2, TArg3, TArg4, AsyncCallback, object, IAsyncResult> beginMethod, Func<IAsyncResult, TResult> endMethod, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, object state)
        {
            if (beginMethod == null)
            {
                throw new ArgumentNullException("beginMethod");
            }
            if (endMethod == null)
            {
                throw new ArgumentNullException("endMethod");
            }
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>(state, factory.CreationOptions);
            try
            {
                AsyncCallback callback = delegate (IAsyncResult asyncResult) {
                    tcs.TrySetResult(endMethod(asyncResult));
                };
                beginMethod(arg1, arg2, arg3, arg4, callback, state);
            }
            catch
            {
                tcs.TrySetResult(default(TResult));
                throw;
            }
            return tcs.Task;
        }

        public static DateTime GetFtpDate(this string date, DateTimeStyles style)
        {
            DateTime time;
            if (DateTime.TryParseExact(date, FtpDateFormats, CultureInfo.InvariantCulture, style, out time))
            {
                return time;
            }
            return DateTime.MinValue;
        }

        public static string GetFtpDirectoryName(this string path)
        {
            string str = (path == null) ? "" : path.GetFtpPath();
            if ((str.Length == 0) || (str == "/"))
            {
                return "/";
            }
            int length = str.LastIndexOf('/');
            if (length < 0)
            {
                return ".";
            }
            if (length == 0)
            {
                return "/";
            }
            return str.Substring(0, length);
        }

        public static string GetFtpFileName(this string path)
        {
            string str = (path == null) ? null : path;
            int startIndex = -1;
            if (str == null)
            {
                return null;
            }
            startIndex = str.LastIndexOf('/');
            if (startIndex < 0)
            {
                return str;
            }
            startIndex++;
            if (startIndex >= str.Length)
            {
                return str;
            }
            return str.Substring(startIndex, str.Length - startIndex);
        }

        public static string GetFtpPath(this string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "./";
            }
            path = path.Replace('\\', '/');
            path = Regex.Replace(path, "[/]+", "/");
            char[] trimChars = new char[] { '/' };
            path = path.TrimEnd(trimChars);
            if (path.Length == 0)
            {
                path = "/";
            }
            return path;
        }

        public static string GetFtpPath(this string path, params string[] segments)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = "./";
            }
            foreach (string str in segments)
            {
                if (str != null)
                {
                    if ((path.Length > 0) && !path.EndsWith("/"))
                    {
                        path = path + "/";
                    }
                    char[] chArray1 = new char[] { '/' };
                    path = path + Regex.Replace(str.Replace('\\', '/'), "[/]+", "/").TrimEnd(chArray1);
                }
            }
            char[] trimChars = new char[] { '/' };
            path = Regex.Replace(path.Replace('\\', '/'), "[/]+", "/").TrimEnd(trimChars);
            if (path.Length == 0)
            {
                path = "/";
            }
            return path;
        }

        public static string[] GetPathSegments(this string path)
        {
            if (path.Contains("/"))
            {
                char[] separator = new char[] { '/' };
                return path.Split(separator);
            }
            if (path.Contains(@"\"))
            {
                char[] chArray2 = new char[] { '\\' };
                return path.Split(chArray2);
            }
            return new string[] { path };
        }

        public static bool HasPermission(FtpPermission owner, FtpPermission flag)
        {
            return ((owner & flag) == flag);
        }

        public static bool IsAbsolutePath(this string path)
        {
            if (!path.StartsWith("/") && !path.StartsWith("./"))
            {
                return path.StartsWith("../");
            }
            return true;
        }

        public static bool IsBlank(this IEnumerable value)
        {
            if (value == null)
            {
                return true;
            }
            if (value is IList)
            {
                return (((IList) value).Count == 0);
            }
            return ((value is byte[]) && (((byte[]) value).Length == 0));
        }

        public static bool IsBlank(this IList value)
        {
            if (value != null)
            {
                return (value.Count == 0);
            }
            return true;
        }

        public static bool IsBlank(this string value)
        {
            if (value != null)
            {
                return (value.Length == 0);
            }
            return true;
        }

        public static bool IsFailure(this FtpStatus status)
        {
            return (status == FtpStatus.Failed);
        }

        public static bool IsFtpRootDirectory(this string ftppath)
        {
            if (!(ftppath == ".") && !(ftppath == "./"))
            {
                return (ftppath == "/");
            }
            return true;
        }

        public static bool IsKnownError(this string reply, string[] strings)
        {
            reply = reply.ToLower();
            foreach (string str in strings)
            {
                if (reply.Contains(str))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsNumeric(this string field)
        {
            field = field.Replace(".", "");
            for (int i = 0; i < field.Length; i++)
            {
                if (!char.IsDigit(field[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsResumeAllowed(this IOException exception)
        {
            if ((exception.InnerException == null) && !exception.Message.IsKnownError(FtpServerStrings.unexpectedEOF))
            {
                return false;
            }
            SocketException innerException = exception.InnerException as SocketException;
            if (innerException != null)
            {
                return (innerException.ErrorCode == 0x2746);
            }
            return true;
        }

        public static bool IsSuccess(this FtpStatus status)
        {
            if (status != FtpStatus.Success)
            {
                return (status == FtpStatus.Skipped);
            }
            return true;
        }

        public static bool IsValidCombination(this FtpError options)
        {
            return ((options != (FtpError.Throw | FtpError.Stop)) && (options != (FtpError.Throw | FtpError.Stop | FtpError.DeleteProcessed)));
        }

        public static bool IsValidRegEx(this string pattern)
        {
            bool flag = true;
            if ((pattern != null) && (pattern.Trim().Length > 0))
            {
                try
                {
                    Regex.Match("", pattern);
                }
                catch (ArgumentException)
                {
                    flag = false;
                }
                return flag;
            }
            return false;
        }

        public static List<string> ItemsToString(this object[] args)
        {
            List<string> list = new List<string>();
            if (args != null)
            {
                foreach (object obj2 in args)
                {
                    string str;
                    if (obj2 == null)
                    {
                        str = "null";
                    }
                    else if (obj2 is string)
                    {
                        str = "\"" + obj2 + "\"";
                    }
                    else
                    {
                        str = obj2.ToString();
                    }
                    list.Add(str);
                }
            }
            return list;
        }

        public static string Join(this string[] values, string delimiter)
        {
            return string.Join(delimiter, values);
        }

        public static string Join(this List<string> values, string delimiter)
        {
            return string.Join(delimiter, values);
        }

        public static string RemovePostfix(this string text, string postfix)
        {
            if (text.EndsWith(postfix))
            {
                return text.Substring(0, text.Length - postfix.Length);
            }
            return text;
        }

        public static string RemovePrefix(this string text, string prefix)
        {
            if (text.StartsWith(prefix))
            {
                return text.Substring(prefix.Length);
            }
            return text;
        }

        public static string[] SplitString(this string str)
        {
            List<string> list = new List<string>(str.Split(null));
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].Trim().Length == 0)
                {
                    list.RemoveAt(i);
                }
            }
            return list.ToArray();
        }

        public static void ValidateFtpServer(this Uri uri)
        {
            if (string.IsNullOrEmpty(uri.PathAndQuery))
            {
                throw new UriFormatException("The supplied URI does not contain a valid path.");
            }
            if (uri.PathAndQuery.EndsWith("/"))
            {
                throw new UriFormatException("The supplied URI points at a directory.");
            }
        }
    }
}

