using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Services
{
    internal class KeyValueStoreService : IStoreService
    {
        private Dictionary<string, string> _log = new Dictionary<string, string>();
        
        public KeyValueStoreService()
        {

        }

        public void Set(string key, string value)
        {
            if (_log.ContainsKey(key))
            {
                _log[key] = value;
            }
            else
            {
                _log.Add(key, value);
            }
        }

        public string? Get(string key)
        {
            if (_log.ContainsKey(key))
            {
                return _log[key];
            }
            return null;
        }

        public bool TryGet(string key, out string value)
        {
            string? res = Get(key);
            if (res == null)
            {
                value = "";
                return false;
            }
            value = res;
            return true;
        }

        public void Remove(string key)
        {
            if (TryGet(key, out _))
            {
                _log.Remove(key);
            }
        }

        public void SaveToDisk(string path)
        {
            throw new NotImplementedException();
        }
    }
}
