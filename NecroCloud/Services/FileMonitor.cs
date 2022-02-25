using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecroCloud.Services
{
    internal sealed class FileMonitor : IServiceBase
    {
        private FileSystemWatcher _watcher;
        private ILogger _logger = ServiceLocator.Current.GetSingleton<ILogger>();

        public bool Configure()
        {
            try
            {
                // To Do
                // Adicionar PATH ( caminho da pasta observada aos registros )
                string path = "";

                if (!Directory.Exists(path))
                {
                    _logger.CreateLog("Pasta a ser Monitorada não Existe", LogType.CRITICAL);
                    throw new DirectoryNotFoundException($"Path doesn't exist: {path}");
                }

                _watcher = new FileSystemWatcher(path);
                _watcher.NotifyFilter = NotifyFilters.Attributes
                    | NotifyFilters.CreationTime
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.FileName
                    | NotifyFilters.LastAccess
                    | NotifyFilters.LastWrite
                    | NotifyFilters.Security
                    | NotifyFilters.Size;
                _watcher.IncludeSubdirectories = true;
                _watcher.InternalBufferSize = 1638400;
                _watcher.Changed += FileChanged;
                _watcher.Deleted += FileChanged;
                _watcher.Renamed += FileRenamed;
                _watcher.Error += OnError;
            }
            catch (Exception ex)
            {
                _logger.CreateLog("Erro ao Iniciar Serviço: FileMonitor", LogType.CRITICAL, ex);
                throw new Exception($"Failed to start FileMonitor, see InnerException for more details", ex);
            }
            return true;
        }

        public bool Start()
        {
            _watcher.EnableRaisingEvents = true;
            return true;
        }

        public bool Stop()
        {
            _watcher.EnableRaisingEvents = false;
            return true;
        }

        public bool Dispose()
        {
            _watcher.Dispose();
            return true;
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                // File/Folder deleted
            }
            else
            {
                // File/Folder changed
                ProcessFileChange(e.FullPath);
            }
        }

        private void FileRenamed(object sender, RenamedEventArgs e)
        {
            // File/Folder renamed
            ProcessFileChange(e.FullPath, e.OldFullPath);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            _logger.CreateLog("Erro ao processar evento de mudança de arquivo/pasta", LogType.ERROR, e.GetException());
        }

        private void ProcessFileChange(string path, string oldPath = "")
        {
            if (!string.IsNullOrEmpty(oldPath))
            {
                // Process File/Folder renamed
            }
            else
            {
                // Process File/Folder general change
            }
        }
    }
}