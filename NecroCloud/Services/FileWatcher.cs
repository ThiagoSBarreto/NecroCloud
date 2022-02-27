using NecroCloud.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecroCloud.Services
{
    internal sealed class FileWatcher : IFileWatcher
    {
        private ILogger _logger = ServiceLocator.Current.GetSingleton<ILogger>();
        private IFileHelper _fileHelper = ServiceLocator.Current.GetSingleton<IFileHelper>();

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private static Task _subRoutine;

        private Queue<Item> _queue = new Queue<Item>();

        public void ProcessItem(Item item)
        {
            Item tempItem = _queue.Where(w => w.LocalFullPath == item.LocalFullPath &&
                                  w.Hash != item.Hash).FirstOrDefault();
            if (tempItem != null)
            {
                if (tempItem.LastModifiedDate.HasValue && item.LastModifiedDate.HasValue)
                {
                    if (tempItem.LastSyncDate.Value > item.LastModifiedDate.Value)
                    {
                        _queue = new Queue<Item>(_queue.Where(w => w.Hash != tempItem.Hash).ToList());
                    }
                }
            }
            _queue.Enqueue(item);
        }

        public void StartSubRoutine()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _subRoutine = new Task(async () =>
            {
                while (true)
                {
                    int state = await ProcessQueue();
                    if (state == 0)
                    {
                        await Task.Delay(5000);
                    }
                    await Task.Delay(500);
                }
            }, _cancellationToken);
            _subRoutine.Start();
        }

        public void StopSubRoutine()
        {
            _cancellationTokenSource.Cancel();
        }

        public void CompareFilesWithServer(List<Item> items)
        {

        }

        private async Task<int> ProcessQueue()
        {
            if (_queue.Count > 0)
            {
                Item item = _queue.Dequeue();

                if (CheckFileIntegrity(ref item) == null)
                {
                    if (_queue.Count > 0)
                    {
                        return 1;
                    }
                    return 0;
                }

                _logger.CreateLog($"Processing Item: {item.LocalFullPath}", LogType.CONSOLE);
                switch (item.Process)
                {
                    case ProcessType.DELETE:
                        return await DeleteItem(item);
                    case ProcessType.SYNC:
                        return await SyncItem(item);
                    case ProcessType.DOWNLOAD:
                        return await DownloadItem(item);
                    case ProcessType.UPLOAD:
                        return await UploadItem(item);
                    default:
                        return 0;
                }
            }
            return 0;
        }

        private Item CheckFileIntegrity(ref Item item)
        {
            if (item.Hash == "@" || item.Hash == "#" || item.Hash == "$")
            {
                item.Hash = _fileHelper.CalculateHash(item.LocalFullPath);
                if (item.Hash == "@" || item.Hash == "#" || item.Hash == "$")
                {
                    if (item.Hash != "%")
                    {
                        ProcessItem(item);
                    }
                    return null;
                }
            }

            if (item.Size <= 0)
            {
                item.Size = _fileHelper.CalculateSize(item.LocalFullPath);
                if (item.Size <= 0)
                {
                    ProcessItem(item);
                    return null;
                }
            }

            if (item.Name.StartsWith("~") || item.Name.EndsWith(".tmp") || item.Name.EndsWith(".TMP") || item.Hash == "%")
            {
                if (!item.IsFolder && string.IsNullOrWhiteSpace(item.FileExtension))
                {
                    return null;
                }
            }

            return item;
        }

        private async Task<int> DeleteItem(Item item)
        {
            try
            {
                return 1;
            }
            catch (Exception ex)
            {
                _logger.CreateLog($"Error while deleting item: {item.LocalFullPath}", LogType.ERROR, ex);
                return 0;
            }
        }

        private async Task<int> SyncItem(Item item)
        {
            try
            {
                return 1;
            }
            catch (Exception ex)
            {
                _logger.CreateLog($"Error while sync item: {item.LocalFullPath}", LogType.ERROR, ex);
                return 0;
            }
        }

        private async Task<int> UploadItem(Item item)
        {
            try
            {
                return 1;
            }
            catch (Exception ex)
            {
                _logger.CreateLog($"Error while uploading item: {item.LocalFullPath}", LogType.ERROR, ex);
                return 0;
            }
        }

        private async Task<int> DownloadItem(Item item)
        {
            try
            {
                return 1;
            }
            catch (Exception ex)
            {
                _logger.CreateLog($"Error while downloading item: {item.LocalFullPath}", LogType.ERROR, ex);
                return 0;
            }
        }
    }
}
