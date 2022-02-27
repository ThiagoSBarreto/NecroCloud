using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecroCloud.Model
{
    internal sealed class Item
    {
        public long Id { get; set; }

        public string Hash { get; set; }

        public string Name { get; set; }

        public string FileExtension { get; set; }

        public string LocalFullPath { get; set; }

        public string ServerFullPath { get; set; }

        public string LocalFatherFullPath { get; set; }

        public string ServerFatherFullPath { get; set; }

        public string OldFullPath { get; set; }

        public string NewFullPath { get; set; }

        public long Size { get; set; }

        public bool Syncronizaded { get; set; }

        public bool IsFolder { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public DateTime? DeletionDate { get; set; }

        public DateTime? LastSyncDate { get; set; }

        public bool Deleted { get; set; }

        public ProcessType Process { get; set; }
    }

    internal enum ProcessType
    {
        DELETE = 0,
        SYNC = 1,
        UPLOAD = 2,
        DOWNLOAD = 3
    }
}
