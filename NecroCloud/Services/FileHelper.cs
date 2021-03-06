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
                using (var sha256 = SHA256.Create())
                {
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        hash = BitConverter.ToString(sha256.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
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
                            using (var sha256 = SHA256.Create())
                            {
                                hash = BitConverter.ToString(sha256.ComputeHash(Encoding.ASCII.GetBytes(path))).Replace("-", "").ToLowerInvariant();
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
                if (Directory.Exists(path))
                {
                    FileAttributes fa = File.GetAttributes(path);
                    isFolder = (fa.HasFlag(FileAttributes.Directory));
                }
            }
            catch
            {
                // Given path isn't a folder
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
                if (File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    size = info.Length;
                }
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
                return Path.GetFileName(path);
            }
            return null;
        }

        public string GetFileExtension(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                string extension = Path.GetExtension(path);
                return extension;
            }
            return null;
        }

        public DateTime? GetLastModifiedDate(string path)
        {
            DateTime? lastModifiedDate = null;
            try
            {
                if (File.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    lastModifiedDate = info.LastWriteTime;
                }
                else if (Directory.Exists(path))
                {
                    FileInfo info = new FileInfo(path);
                    lastModifiedDate = info.LastWriteTime;
                }
            }
            catch (Exception ex)
            {
                _logger.CreateLog("Error while calculating file size", LogType.ERROR, ex);
                return null;
            }
            return lastModifiedDate;
        }
    }
}
