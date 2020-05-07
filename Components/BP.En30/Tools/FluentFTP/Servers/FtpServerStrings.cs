namespace FluentFTP.Servers
{
    using System;

    internal static class FtpServerStrings
    {
        public static string[] fileNotFound = new string[] { 
            "can't find file", "can't check for file existence", "does not exist", "failed to open file", "not found", "no such file", "cannot find the file", "cannot find", "can't get file", "could not get file", "could not get file size", "cannot get file", "not a regular file", "file unavailable", "file is unavailable", "file not unavailable", 
            "file is not available", "no files found", "no file found", "datei oder verzeichnis nicht gefunden", "can't find the path", "cannot find the path", "could not find the path"
         };
        public static string[] fileSizeNotInASCII = new string[] { "not allowed in ascii", "size not allowed in ascii", "n'est pas autoris\x00e9 en mode ascii" };
        public static string[] folderExists = new string[] { "exist on server", "exists on server", "file exist", "directory exist", "folder exist", "file already exist", "directory already exist", "folder already exist" };
        public static string[] unexpectedEOF = new string[] { "unexpected eof for remote file", "received an unexpected eof", "unexpected eof" };
    }
}

