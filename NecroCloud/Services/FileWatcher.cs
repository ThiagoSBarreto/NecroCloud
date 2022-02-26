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
            if (_queue.Where(x => x.LocalFullPath == item.LocalFullPath && x.Process != item.Process).Count() == 0)
            {
                _queue.Enqueue(item);
            }
        }

            public void StartSubRoutine()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            _subRoutine = new Task(async() =>
            {
                while (true)
                {
                    ProcessQueue();
                    await Task.Delay(100);
                }
            }, _cancellationToken);
            _subRoutine.Start();
        }

        public void StopSubRoutine()
        {
            _cancellationTokenSource.Cancel();
        }

        private void ProcessQueue()
        {
            if (_queue.Count > 0)
            {
                Item item = _queue.Dequeue();

                if (item.Name.StartsWith("~") || item.Name.EndsWith(".tmp") || item.Name.EndsWith(".TMP") || item.Hash == "%")
                {
                    return;
                }

                if (CheckFileIntegrity(ref item) == null)
                {
                    return;
                }

                _logger.CreateLog($"Processing Item: {item.LocalFullPath}", LogType.CONSOLE);
                switch (item.Process)
                {
                    case ProcessType.DELETE:
                        DeleteItem(item);
                        break;
                    case ProcessType.SYNC:
                        SyncItem(item);
                        break;
                    case ProcessType.DOWNLOAD:
                        DownloadItem(item);
                        break;
                    case ProcessType.UPLOAD:
                        UploadItem(item);
                        break;
                }
            }
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

            return item;
        }

        private void DeleteItem(Item item)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.CreateLog($"Error while deleting item: {item.LocalFullPath}", LogType.ERROR, ex);
            }
        }

        private void SyncItem(Item item)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.CreateLog($"Error while sync item: {item.LocalFullPath}", LogType.ERROR, ex);
            }
        }

        private void UploadItem(Item item)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.CreateLog($"Error while uploading item: {item.LocalFullPath}", LogType.ERROR, ex);
            }
        }

        private void DownloadItem(Item item)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.CreateLog($"Error while downloading item: {item.LocalFullPath}", LogType.ERROR, ex);
            }
        }
    }
}
