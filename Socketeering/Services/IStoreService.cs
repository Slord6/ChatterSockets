using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Services
{
    internal interface IStoreService
    {
        public void Set(string key, string value);

        public string? Get(string key);

        public bool TryGet(string key, out string value);
        public void Remove(string key);

        public void SaveToDisk(string path);
    }
}
