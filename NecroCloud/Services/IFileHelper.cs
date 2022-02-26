using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecroCloud.Services
{
    internal interface IFileHelper
    {
        long CalculateSize(string path);
        string CalculateHash(string path);
        bool IsFolder(string path);
        string GetFileName(string path);
    }
}
