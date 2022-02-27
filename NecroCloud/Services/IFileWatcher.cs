using NecroCloud.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecroCloud.Services
{
    internal interface IFileWatcher
    {
        void StartSubRoutine();
        void StopSubRoutine();
        void ProcessItem(Item item);
        void CompareFilesWithServer(List<Item> items);
    }
}
