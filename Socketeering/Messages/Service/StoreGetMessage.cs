using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Socketeering.Messages.Service
{
    internal class StoreGetMessage : Message
    {
        public StoreGetMessage(string rawMessage) : base(rawMessage)
        {
        }

        public StoreGetMessage(string source, string destination, string path, Dictionary<string, string>? additionArgs)
            : base(source, destination, NodeControl.STORE, BuildArgs(path, additionArgs))
        {
        }

        private static Dictionary<string, string> BuildArgs(string path, Dictionary<string, string>? additionalArgs)
        {
            if(additionalArgs == null) additionalArgs = new Dictionary<string, string>();
            if(!additionalArgs.ContainsKey("OP")) additionalArgs.Add("OP", "GET");
            additionalArgs.Add("PATH", path);
            return additionalArgs;
        }
    }
}
