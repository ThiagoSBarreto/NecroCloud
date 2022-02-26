using NecroCloud.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NecroCloud.Services
{
    internal sealed class FileHelper : IFileHelper
    {
        private ILogger _logger = ServiceLocator.Current.GetSingleton<ILogger>();

        public long CalculateSize(string path)
        {
            long size = -1;
            try
            {
                if (Directory.Exists(path))
                {
                    size = CalculateFolderSize(path);
                }
                else
                {
                    size = CalculateFileSize(path);
                }
            }
            catch (Exception ex)
            {
                _logger.CreateLog("Error while calculating file/folder size", LogType.ERROR, ex);
                return 0;
            }
            return size;
        }
        // $ = Folder in use
        // # = File in Use
        // @ = Unknow Error
        // % = Office TempFile or Temp file
        public string CalculateHash(string path)
        {
            if (path.StartsWith("~") || path.EndsWith(".tmp") || path.EndsWith(".TMP"))
            {
                return "%";
            }
            string hash = "$";
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            // IOException The File/Folder may be in use
            catch (IOException ioex)
            {

            }
            // UnauthorizedAccessException The path could be a directory
            catch (UnauthorizedAccessException ex)
            {
                try
                {
                    if (Directory.Exists(path))
                    {
                        FileAttributes fa = File.GetAttributes(path);
                        if (fa.HasFlag(FileAttributes.Directory))
                        {
                            using (var md5 = MD5.Create())
                            {
                                hash = BitConverter.ToString(md5.ComputeHash(Encoding.ASCII.GetBytes(path))).Replace("-", "").ToLowerInvariant();
                            }
                        }
                    }
                    else
                    {
                        hash = "#";
                    }
                }
                // IOException The Folder may be in use
                catch (IOException ioex)
                {

                }
                // Unknow error, check the logs
                catch (Exception ex2)
                {
                    _logger.CreateLog("Error while calculating file/folder hash", LogType.ERROR, ex2);
                    hash = "@";
                }
            }
            return hash;
        }

        public bool IsFolder(string path)
        {
            if (path.StartsWith("~") || path.EndsWith(".tmp") || path.EndsWith(".TMP"))
            {
                return false;
            }
            bool isFolder = false;
            try
            {
                FileAttributes fa = File.GetAttributes(path);
                isFolder = (fa.HasFlag(FileAttributes.Directory));
            }
            catch (Exception ex)
            {
                _logger.CreateLog("Error while checking if path is folder or file", LogType.ERROR,ex);
            }
            return isFolder;
        }

        private long CalculateFileSize(string path)
        {
            if (path.StartsWith("~") || path.EndsWith(".tmp") || path.EndsWith(".TMP"))
            {
                return -1;
            }
            long size = 0;
            try
            {
                FileInfo info = new FileInfo(path);
                size = info.Length;
            }
            catch (Exception ex)
            {
                _logger.CreateLog("Error while calculating file size", LogType.ERROR, ex);
                return 0;
            }
            return size;
        }

        private long CalculateFolderSize(string path)
        {
            long size = 0;
            try
            {
                foreach(string file in Directory.GetFiles(path))
                {
                    size += CalculateFileSize(file);
                }
                foreach(string dir in Directory.GetDirectories(path))
                {
                    size += CalculateFolderSize(dir);
                }
            }
            catch (Exception ex)
            {
                _logger.CreateLog("Error while calculating folder size", LogType.ERROR, ex);
                return 0;
            }
            return size;
        }

        public string GetFileName(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                int index = path.LastIndexOf("\\");
                return path.Substring(index + 2, path.Length - index - 2);
            }
            return null;
        }
    }
}
