using NecroCloud.Model;
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
        private IFileHelper _fileHelper = ServiceLocator.Current.GetSingleton<IFileHelper>();
        private IFileWatcher _fileWather = ServiceLocator.Current.GetSingleton<IFileWatcher>();

        public bool Configure()
        {
            try
            {
                // To Do
                // Adicionar PATH ( caminho da pasta observada aos registros )
                string path = @"F:\Área de Trabalho\WatchedFolder";

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

        public async void Start()
        {
            _watcher.EnableRaisingEvents = true;
            _logger.CreateLog("FileMonitor Service: STARTED", LogType.CONSOLE);
        }

        public bool Stop()
        {
            _watcher.EnableRaisingEvents = false;
            _logger.CreateLog("FileMonitor Service: STOPED", LogType.CONSOLE);
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
                _logger.CreateLog($"File Deleted: {e.FullPath}", LogType.CONSOLE);
                // File/Folder deleted
                Item deletedItem = new Item();
                deletedItem.Name = _fileHelper.GetFileName(e.FullPath);
                deletedItem.LocalFullPath = e.FullPath;
                deletedItem.Deleted = true;
                deletedItem.DeletionDate = DateTime.Now;
                deletedItem.Process = ProcessType.DELETE;

                _fileWather.ProcessItem(deletedItem);
            }
            else
            {
                _logger.CreateLog($"File Changed: {e.FullPath}", LogType.CONSOLE);
                // File/Folder changed
                ProcessFileChange(null, null, e.FullPath);
            }
        }

        private void FileRenamed(object sender, RenamedEventArgs e)
        {
            _logger.CreateLog($"File Renamed: ", LogType.CONSOLE);
            _logger.CreateLog($"Old Name: {e.OldFullPath}", LogType.CONSOLE);
            _logger.CreateLog($"New Name: {e.FullPath}", LogType.CONSOLE);
            // File/Folder renamed
            ProcessFileChange(e.OldName, e.Name, e.FullPath, e.OldFullPath);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            _logger.CreateLog("Erro ao processar evento de mudança de arquivo/pasta", LogType.ERROR, e.GetException());
        }

        private void ProcessFileChange(string oldName, string newName, string path, string oldPath = "")
        {
            if (!string.IsNullOrEmpty(oldPath))
            {
                
                // Process File/Folder renamed
                Item removeItem = new Item();
                removeItem.Name = _fileHelper.GetFileName(oldPath);
                removeItem.Deleted = true;
                removeItem.DeletionDate = DateTime.Now;
                removeItem.Syncronizaded = false;
                removeItem.LocalFullPath = oldPath;
                removeItem.Process = ProcessType.DELETE;

                Item addItem = new Item();
                addItem.Name = _fileHelper.GetFileName(path);
                addItem.Syncronizaded = false;
                addItem.LastModifiedDate = DateTime.Now;
                addItem.LocalFullPath = path;
                addItem.LocalFatherFullPath = Directory.GetParent(path).FullName;
                addItem.IsFolder = _fileHelper.IsFolder(path);
                addItem.Hash = _fileHelper.CalculateHash(path);
                addItem.Size = _fileHelper.CalculateSize(path);
                addItem.Process = ProcessType.UPLOAD;

                _fileWather.ProcessItem(removeItem);
                _fileWather.ProcessItem(addItem);
            }
            else
            {
                // Process File/Folder general change
                Item item = new Item();
                item.Name = _fileHelper.GetFileName(path);
                item.LocalFullPath = path;
                item.LocalFatherFullPath = Directory.GetParent(path).FullName;
                item.IsFolder = _fileHelper.IsFolder(path);
                item.Size = _fileHelper.CalculateSize(path);
                item.Hash = _fileHelper.CalculateHash(path);
                item.Process = ProcessType.UPLOAD;

                _fileWather.ProcessItem(item);
            }
        }
    }
}